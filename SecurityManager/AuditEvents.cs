using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace SecurityManager
{
    public enum AuditEventTypes
    {
        LogReplicationInitiated = 0,
        LogReplicationReceived = 1
    }

    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock (resourceLock)
                {
                    if (resourceManager == null)
                    {
                        resourceManager = new ResourceManager
                            (typeof(AuditEventFile).ToString(),
                            Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }

        public static string LogReplicationInitiated
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.LogReplicationInitiated.ToString());
            }
        }

        public static string LogReplicationReceived
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.LogReplicationReceived.ToString());
            }
        }

    }
}
