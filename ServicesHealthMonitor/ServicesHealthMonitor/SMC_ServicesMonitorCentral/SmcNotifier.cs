using SMC_ServicesMonitorCentral.Properties;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace SMC_ServicesMonitorCentral
{
    public enum ServiceNotificationStatus
    {
        Exception,
        Stopped,
        Started
    }
    public struct SmcNotification
    {
        public string SourceName;
        public DateTime TimeOccurred;
        public ServiceNotificationStatus Status;
        public string Problem;
    }

    public enum MailStatus
    {
        Sent,
        Cancelled,
        Error
    }

    public static class SmcNotifier
    {
        private static SmtpClient mailer;

        static SmcNotifier()
        {
            // AWS server
            // mailer = new SmtpClient("email-smtp.us-west-2.amazonaws.com", 587);
            // Google server
            mailer = new SmtpClient("smtp.gmail.com", 587);
            mailer.EnableSsl = true;
            // AWS credentials
            // mailer.Credentials = new NetworkCredential("AKIAIU7BKOFSLUVHPLAQ", "ArGlNgTzMmkINfO9V0oXOOyP5tCjgnzh+6X3RloO8VZ9");
            // Google credentials
            mailer.Credentials = new NetworkCredential("dwparker79@gmail.com", Resources.EmailPassword);
            mailer.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailer.Timeout = 20000;
            mailer.SendCompleted += Mailer_SendCompleted;
        }

        private static string[] recipientTokens;
        private static MailStatus[] sendStatuses;
        private static bool[] sendSuccess;
        private static bool sending;

        public static bool Sending
        {
            get
            {
                return sending;
            }
        }

        public static KeyValuePair<string, bool>[] CurrentMailSent
        {
            get
            {
                KeyValuePair<string, bool>[] retVal = new KeyValuePair<string, bool>[recipientTokens.Length];
                for (int i = 0; i < retVal.Length; i++)
                    retVal[i] = new KeyValuePair<string, bool>(recipientTokens[i], sendSuccess[i]);
                return retVal;
            }
        }
        public static KeyValuePair<string, MailStatus>[] CurrentMailStatuses
        {
            get
            {
                KeyValuePair<string, MailStatus>[] retval = new KeyValuePair<string, MailStatus>[recipientTokens.Length];
                for (int i = 0; i < retval.Length; i++)
                    retval[i] = new KeyValuePair<string, MailStatus>(recipientTokens[i], sendStatuses[i]);
                return retval;
            }
        }

        private static void Mailer_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            int index = -1;
            for (int i = 0; i < recipientTokens.Length; i++)
            {
                if (recipientTokens[i] == (string)e.UserState)
                {
                    index = i;
                    break;
                }
            }

            if (index > -1)
            {
                sendSuccess[index] = true;
                if (e.Cancelled)
                    sendStatuses[index] = MailStatus.Cancelled;
                else if (e.Error != null)
                    sendStatuses[index] = MailStatus.Error;
                else
                    sendStatuses[index] = MailStatus.Sent;
            }

            if (sendStatuses[index] != MailStatus.Sent)
                EventLogger.Log(EventLoggerThreadType.Async, EventLoggerCode.General_Failure,
                    "Notification failed: message sent to " + (string)e.UserState + " failed to send.");

            // check if all the mails we tried to send received a response.
            foreach (bool b in sendSuccess)
            {
                if (!b)
                    return;
            }
            sending = false;
        }

        // yes, the name has a double meaning.  Not intentional.
        private static System.Text.StringBuilder bodyBuilder;
        public static void Notify(SmcNotification message, MailAddress[] addresses)
        {
            MailAddress fromAddr = new MailAddress("dwparker79@gmail.com");
            MailMessage newMsg = new MailMessage();

            recipientTokens = new string[addresses.Length];
            sendSuccess = new bool[addresses.Length];
            sendStatuses = new MailStatus[addresses.Length];
            sending = true;

            string statusLine, nameLine, issueLine, recommendationsLine;
            switch (message.Status)
            {
                case ServiceNotificationStatus.Exception:
                    statusLine = "SHM has encountered an exception:";
                    nameLine = message.Problem;
                    issueLine = "Services Health Monitor has encountered the above exception.";
                    recommendationsLine = "Contact the Network Security Services team and inform them of this problem.";
                    break;
                case ServiceNotificationStatus.Started:
                    statusLine = "The following Service has started:";
                    nameLine = message.SourceName;
                    issueLine = "The service " + nameLine + " has been restarted.";
                    recommendationsLine = "No action necessary.";
                    break;
                case ServiceNotificationStatus.Stopped:
                default:
                    statusLine = "The following Service has stopped processing:";
                    nameLine = message.SourceName;
                    issueLine = "The service " + nameLine + " has stopped processing.";
                    recommendationsLine = "Manually restart the service through the Services Health Monitor application.<br/>\n" +
                    "In the application, you can click AUTO to always receive alerts like this.";
                    break;
            }
            for (int i = 0; i < addresses.Length; i++)
            {
                recipientTokens[i] = addresses[i].Address;

                bodyBuilder = new System.Text.StringBuilder();
                bodyBuilder.Append("<html>\n");
                bodyBuilder.Append("<h3>Service Health Monitor Notification</h3>\n");
                bodyBuilder.Append("<p>");
                  bodyBuilder.Append(statusLine);
                  bodyBuilder.Append("\n");
                bodyBuilder.Append("<b>");
                  bodyBuilder.Append(nameLine);
                  bodyBuilder.Append("</b></p><br/>\n");
                bodyBuilder.Append("<table>\n");
                bodyBuilder.Append(" <tr>\n");
                bodyBuilder.Append("  <td>Issue:</td>\n");
                bodyBuilder.Append("  <td>");
                  bodyBuilder.Append(issueLine);
                  bodyBuilder.Append("</td>\n");
                bodyBuilder.Append(" </tr>\n");
                bodyBuilder.Append(" <tr>\n");
                bodyBuilder.Append("  <td>Recommendation:</td>\n");
                bodyBuilder.Append("  <td>");
                  bodyBuilder.Append(recommendationsLine);
                  bodyBuilder.Append("</td>\n");
                bodyBuilder.Append(" </tr>\n");
                bodyBuilder.Append("</table><br/>\n");
                bodyBuilder.Append("<p>Thanks,</p>\n");
                bodyBuilder.Append("<p>Network Security Services</p>\n");
                bodyBuilder.Append("<br/>\n");
                bodyBuilder.Append("<h5>This message was automatically generated ");
                  bodyBuilder.Append(message.TimeOccurred.ToString("at hh:mm:ss tt on MM/d/yyy"));
                  bodyBuilder.Append(".</h5>\n");
                bodyBuilder.Append("<h5>Please do not reply to this email.</h5>\n");
                bodyBuilder.Append("</html>");

                newMsg = new MailMessage(fromAddr, addresses[i]);
                newMsg.Subject = "Services Health Monitor Notification";
                newMsg.IsBodyHtml = true;
                newMsg.Body = bodyBuilder.ToString();
                
                mailer.SendAsync(newMsg, recipientTokens[i]);
                //newMsg.Dispose();
            }
        }

        public static void OnClose()
        {
        }
    }
}
