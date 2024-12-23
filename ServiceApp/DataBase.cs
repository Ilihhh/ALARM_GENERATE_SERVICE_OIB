/*using Contracts;
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
*/

using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServiceApp
{
    public class Database
    {
        private const string FilePath = "alarms.txt";
        internal static List<Alarm> alarms = new List<Alarm>();

        static Database()
        {
            // Load alarms from file during initialization
            LoadAlarms();
        }

        public static void SaveAlarms()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    foreach (var alarm in alarms)
                    {
                        writer.WriteLine($"{DateTime.Now}: Alarm received - Client: {alarm.ClientName}, Message: {alarm.Message}, Risk: {alarm.Risk}");
                    }
                }
                Console.WriteLine("Alarms saved to file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Failed to save alarms: " + ex.Message);
            }
        }

        public static void LoadAlarms()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    alarms.Clear(); // Clear existing alarms to avoid duplication

                    using (StreamReader reader = new StreamReader(FilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            // Parse the line using the expected format
                            var parts = line.Split(new[] { " - Client: ", ", Message: ", ", Risk: " }, StringSplitOptions.None);

                            if (parts.Length == 4) // Ensure the format matches
                            {
                                string clientName = parts[1];
                                string message = parts[2];
                                if (double.TryParse(parts[3], out double risk))
                                {
                                    alarms.Add(new Alarm(clientName, message, risk));
                                }
                                else
                                {
                                    Console.WriteLine($"[WARNING] Invalid risk value in line: {line}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[WARNING] Line does not match expected format: {line}");
                            }
                        }
                    }
                    Console.WriteLine("Alarms loaded from file.");
                }
                else
                {
                    Console.WriteLine("No alarm file found. Starting with an empty database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Failed to load alarms: " + ex.Message);
            }
        }
    }
}
