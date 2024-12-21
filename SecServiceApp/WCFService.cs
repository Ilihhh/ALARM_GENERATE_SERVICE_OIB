using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecServiceApp
{
    public class WCFService : IWCFContract
    {
        private static readonly string filePath = "SecondaryAlarms.txt";

        public void ReplicateAlarm(Alarm alarm)
        {
            // Čuvanje alarma u tekstualnu datoteku
            string logEntry = $"{DateTime.Now}: Alarm received - Client: {alarm.ClientName}, Message: {alarm.Message}, Risk: {alarm.Risk}";
            File.AppendAllText(filePath, logEntry + Environment.NewLine);

            // Ispis na konzolu
            Console.WriteLine(logEntry);
        }
    }
}
