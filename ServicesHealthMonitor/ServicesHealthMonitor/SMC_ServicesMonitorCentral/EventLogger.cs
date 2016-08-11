using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace SMC_ServicesMonitorCentral
{
    public enum EventLoggerThreadType
    {
        Sync,
        Async
    }
    public enum EventLoggerCode
    {
        Successful_Process = 100,
        Failure_to_Start = 200,
        General_Failure = 300,
        Unknown_Error = 400
    }

    internal static class EventLogger
    {
        private static string source;
        private static int nextID = 1;
        private static bool hasAccess;
        public static bool CanWrite
        {
            get
            {
                return hasAccess;
            }
        }

        public static void SetLogInfo(string sourceName, string machineName)
        {
            // check for administrator access
            {
                WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                hasAccess = wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (!hasAccess)
                return;

            source = sourceName;
            if (!EventLog.SourceExists(sourceName))
                EventLog.CreateEventSource(new EventSourceCreationData(sourceName, "Application"));
        }
        public static void Log(EventLoggerThreadType threadType, EventLoggerCode eventCode, string eventInfo)
        {
            if (!hasAccess)
                return;

            string msg = (threadType == EventLoggerThreadType.Async ? "RECOV0" : "NON0") +
                ((int)eventCode).ToString();
            EventLogEntryType type;
            if (eventCode == EventLoggerCode.General_Failure || eventCode == EventLoggerCode.Unknown_Error)
                type = EventLogEntryType.Error;
            else if (eventCode == EventLoggerCode.Failure_to_Start)
                type = EventLogEntryType.Warning;
            else
                type = EventLogEntryType.Information;
            EventLog.WriteEntry(source, msg + " " + eventInfo, type, nextID++);

            if (eventCode == EventLoggerCode.General_Failure ||
                eventCode == EventLoggerCode.Unknown_Error)
            {
                // in the event of an error, send a notification email to the emergency distribution list.
                SmcNotification newMsg = new SmcNotification();
                newMsg.SourceName = "SHM";
                newMsg.Status = ServiceNotificationStatus.Exception;
                newMsg.TimeOccurred = DateTime.Now;
                newMsg.Problem = eventInfo;
            }
        }
    }
}
