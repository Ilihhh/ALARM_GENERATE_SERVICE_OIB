using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SecurityManager
{
    public class Audit : IDisposable
    {

        private static EventLog customLog = null;
        const string SourceName = "SecurityManager.Audit";
        const string LogName = "ReplicationLogs";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName,
                    Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }


        public static void LogReplicationInitiated()
        {

            if (customLog != null)
            {
                string LogReplicationInitiatedMessage =
                    AuditEvents.LogReplicationInitiated;
                string message = String.Format(LogReplicationInitiatedMessage);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.LogReplicationInitiated));
            }
        }

        public static void LogReplicationReceived()
        {

            if (customLog != null)
            {
                string LogReplicationReceivedMessage =
                    AuditEvents.LogReplicationReceived;
                string message = String.Format(LogReplicationReceivedMessage);                  //ovde se dodaju parametri ako su potrebni
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.LogReplicationReceived));
            }
        }



        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
