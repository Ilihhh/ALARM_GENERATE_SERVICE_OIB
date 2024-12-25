using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServiceApp
{
    public class Database
    {
        private const string AlarmsFilePath = "alarms.txt";
        internal static List<Alarm> alarms = new List<Alarm>();

        static Database()
        {
            EnsureAlarmsFileExists();
            LoadAlarmsFromFile();
        }

        /// <summary>
        /// Proverava da li fajl postoji i kreira ga ako ne postoji.
        /// </summary>
        private static void EnsureAlarmsFileExists()
        {
            try
            {
                if (!File.Exists(AlarmsFilePath))
                {
                    // Kreiraj prazan fajl ako ne postoji
                    File.WriteAllText(AlarmsFilePath, string.Empty);
                    Console.WriteLine("Alarms file created.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring alarms file exists: {ex.Message}");
            }
        }

        private static void LoadAlarmsFromFile()
        {
            try
            {
                var lines = File.ReadAllLines(AlarmsFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4) // Očekujemo četiri stavke: vreme generisanja, ime klijenta, poruka, rizik
                    {
                        DateTime timeGenerated = DateTime.Parse(parts[0]); // Parsiranje vremena
                        string clientName = parts[1]; // Ime klijenta
                        string message = parts[2]; // Poruka
                        double risk = double.Parse(parts[3]); // Rizik

                        alarms.Add(new Alarm(timeGenerated, clientName, message, risk));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading alarms from file: {ex.Message}");
            }
        }

        private static void SaveAlarmsToFile()
        {
            try
            {
                var lines = alarms.Select(alarm => $"{alarm.TimeGenerated:yyyy-MM-dd HH:mm:ss}|{alarm.ClientName}|{alarm.Message}|{alarm.Risk:F2}"); // Format za upisivanje u fajl
                File.WriteAllLines(AlarmsFilePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving alarms to file: {ex.Message}");
            }
        }

        public static void AddAlarm(Alarm alarm)
        {
            alarms.Add(alarm);
            SaveAlarmsToFile();
        }

        public static void RemoveAlarm(Alarm alarm)
        {
            alarms.Remove(alarm);
            SaveAlarmsToFile();
        }

        public static void ClearAlarms()
        {
            alarms.Clear();
            SaveAlarmsToFile();
        }
    }
}
