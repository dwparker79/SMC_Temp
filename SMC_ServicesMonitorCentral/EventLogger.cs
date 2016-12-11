using System;
using System.Diagnostics;
using System.Collections.Generic;
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
        private static int nextID = 10001;
        public static void SetLogInfo(string sourceName)
        {
            source = sourceName;
            EventLog.CreateEventSource(new EventSourceCreationData(sourceName, "Application"));
        }
        public static void Log(EventLoggerThreadType threadType, EventLoggerCode eventCode, string eventInfo)
        {
            string msg = (threadType == EventLoggerThreadType.Async ? "RECOV" : "NON") +
                ((int)eventCode).ToString();
            EventLogEntryType type;
            if (eventCode == EventLoggerCode.General_Failure || eventCode == EventLoggerCode.Unknown_Error)
                type = EventLogEntryType.Error;
            else if (eventCode == EventLoggerCode.Failure_to_Start)
                type = EventLogEntryType.Warning;
            else
                type = EventLogEntryType.Information;
            EventLog.WriteEntry(source, msg + " " + eventInfo, type, nextID++);
        }
    }
}
