// Replikator.cs - Zaseban WCF servis za replikaciju
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading;
using Contracts;

namespace ServiceApp
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Replikator : IReplikator
    {
        private readonly IWCFContract _secondaryService;
        private readonly string _bufferFilePath = "replication_buffer.txt";
        private Thread _replicationThread;
        private bool _isRunning;

        public Replikator(IWCFContract secondaryService)
        {
            _secondaryService = secondaryService ?? throw new ArgumentNullException(nameof(secondaryService));
        }

        public void Start()
        {
            _isRunning = true;
            _replicationThread = new Thread(ReplicationLoop) { IsBackground = true };
            _replicationThread.Start();

            Console.WriteLine("Replikator started.");

            // Initial replication on start
            InitialReplication();
        }

        public void Stop()
        {
            _isRunning = false;
            _replicationThread?.Join();
            Console.WriteLine("Replikator stopped.");
        }

        private void ReplicationLoop()
        {
            while (_isRunning)
            {
                try
                {
                    if (File.Exists(_bufferFilePath))
                    {
                        var lines = File.ReadAllLines(_bufferFilePath);
                        foreach (var line in lines)
                        {
                            var alarm = ParseAlarm(line);
                            if (alarm != null)
                            {
                                _secondaryService.ReplicateAlarm(alarm);
                                Console.WriteLine("Replicated alarm: {0}", alarm);
                            }
                        }

                        File.Delete(_bufferFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Replication failed: {0}", ex.Message);
                }

                Thread.Sleep(5000); // Wait 5 seconds before next check
            }
        }

        private void InitialReplication()
        {
            try
            {
                foreach (var alarm in Database.alarms)
                {
                    _secondaryService.ReplicateAlarm(alarm);
                    Console.WriteLine("Initially replicated alarm: {0}", alarm);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Initial replication failed: {0}", ex.Message);
            }
        }

        private Alarm ParseAlarm(string line)
        {
            try
            {
                var parts = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    string clientName = parts[0];
                    string message = parts[1];
                    double risk = double.Parse(parts[2]);
                    return new Alarm(clientName, message, risk);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WARNING] Failed to parse alarm: {0}", ex.Message);
            }

            return null;
        }
    }

}
