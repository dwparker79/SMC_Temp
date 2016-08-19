using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Web;

/// <summary>
/// Summary description for ServiceLogger
/// </summary>
[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class ServiceLogger : IServiceLogger
{
    private static bool eventSourceCreated;
    static ServiceLogger()
    {
        eventSourceCreated = false;
        try
        {
            EventLog.CreateEventSource("SHM Service logger", "Application");
            eventSourceCreated = true;
        }
        catch (Exception) { }
    }

    private static List<List<string>> logs = new List<List<string>>();

    /// <summary>Returns a <c>Dictionary&lt;DateTime, List&lt;string&gt;&gt;</c> of information about the event logs for the given machine name.
    /// <para>The head of each entry is the time when the log was WRITTEN to the target machine.</para>
    /// <para>The tail of each log is a list of the log data in this order:</para>
    /// <list type="bullet">
    /// <item>
    /// <description>The name of the log's source application.</description>
    /// </item>
    /// <item>
    /// <description>The name of the machine from which the log was pulled.</description>
    /// </item>
    /// <item>
    /// <description>The name of the user that caused the log.</description>
    /// </item>
    /// <item>
    /// <description>The category of the log (i.e. "Information", "Warning", "Error", etc.)</description>
    /// </item>
    /// <item>
    /// <description>The log message.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="machineName"></param>
    /// <returns>A Dictionary where the keys are when the logs were written and the values are the log data.</returns>
    public List<List<string>> GetLogs(string[] serviceNames, string machineName, int pageNum, int logsPerPage)
    {
        EventLog[] logs = EventLog.GetEventLogs(machineName);
        Array.Reverse(logs);
        ServiceLogger.logs.Clear();
        List<List<string>> response = new List<List<string>>();
        int currentNum = 0;
        foreach (EventLog el in logs)
        {
            if (el.LogDisplayName != "Application")
                continue;

            foreach (EventLogEntry ele in el.Entries)
            {
                if (serviceNames.Length > 0 && !Array.Exists(serviceNames, s => s.Length == 0 || s == ele.Source))
                    continue;

                List<string> data = new List<string>();
                data.Add(ele.TimeGenerated.ToString("MM/dd/yy hh:mm:ss"));
                data.Add(ele.Source);
                data.Add(ele.MachineName);
                data.Add(ele.UserName);
                data.Add(ele.Category);
                data.Add(ele.Message);

                ServiceLogger.logs.Add(data);
                currentNum++;
                if (currentNum > logsPerPage * (pageNum - 1) && currentNum <= logsPerPage * pageNum)
                    response.Add(data);
            }
            if (currentNum >= logsPerPage * pageNum)
                break;
        }

        return response;
    }

    public List<List<string>> GetLogPage(int pageNum, int logsPerPage, string[] serviceNameFilters)
    {
        int currentNum = 0,
            minPageNum = logsPerPage * (pageNum - 1),
            maxPageNum = logsPerPage * pageNum;
        List<List<string>> toReturn = new List<List<string>>();

        foreach (List<string> log in logs)
        {
            if (serviceNameFilters.Length == 0 || Array.Exists(serviceNameFilters, s => s.Length == 0 || s == log[1]))
                currentNum++;
            else
                continue;
            if (currentNum >= maxPageNum)
                break;
            else if (currentNum >= minPageNum)
                toReturn.Add(log);
        }

        return toReturn;
    }

    public int GetNumPages(int logsPerPage, string[] filter)
    {
        if (filter.Length == 0)
            return logs.Count / logsPerPage + (logs.Count % logsPerPage > 0 ? 1 : 0);
        else
        {
            int count = 0;
            foreach (List<string> log in logs)
                if (Array.Exists(filter, s => s.Length == 0 || s == log[1]))
                    count++;
            return count / logsPerPage + (count % logsPerPage > 0 ? 1 : 0);
        }
    }
    
    /// <summary>
    /// Writes the log to Windows' Application event log.
    /// </summary>
    /// <param name="log"></param>
    public void WriteLog(string log)
    {
        EventLog.WriteEntry("SHM Event logger", log);
    }
}