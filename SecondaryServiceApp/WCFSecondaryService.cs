using System;
using System.Collections.Generic;
using System.IO;
using Contracts;

namespace SecondaryServiceApp
{
    internal class WCFSecondaryService : IWCFSecondaryService
    {
        private const string FilePath = "alarms.txt";

        public void WriteAlarms(List<Alarm> alarms)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(FilePath, false)) 
                {
                    foreach (var alarm in alarms)
                    {
                        writer.WriteLine($"Alarm received: {alarm.Message} at {alarm.TimeGenerated}");
                    }
                }
                Console.WriteLine("Data replication done.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing alarms to file: {ex.Message}");
            }
        }

    }
}
