using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceApp
{
    public class Database
    {
        internal static List<Alarm> alarms = new List<Alarm>();

        static Database()
        {
            // Inicijalizacija baze sa nekoliko uzoraka alarma
            alarms.Add(new Alarm("Client1", "High CPU Usage"));
            alarms.Add(new Alarm("Client2", "Low Disk Space"));
            alarms.Add(new Alarm("Client3", "Memory Leak Detected"));
        }
    }
}
