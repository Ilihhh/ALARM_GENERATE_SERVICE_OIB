using Contracts;
using System;
using System.IO;

namespace SecServiceApp
{
    public class WCFService : IWCFContract
    {
        private static readonly string FilePath = "SecondaryAlarms.txt";

        public void ReplicateAlarm(Alarm alarm)
        {
            if (alarm == null)
            {
                throw new ArgumentNullException(nameof(alarm), "Alarm cannot be null.");
            }

            // Format the log entry
            string logEntry = $"{DateTime.Now}: Alarm received - Client: {alarm.ClientName}, Message: {alarm.Message}, Risk: {alarm.Risk}";

            try
            {
                // Append the alarm to the text file
                File.AppendAllText(FilePath, logEntry + Environment.NewLine);

                // Print the log entry to the console
                Console.WriteLine(logEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Failed to save alarm: " + ex.Message);
            }
        }
    }
}
