using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Contracts
{
    [DataContract]
    public class Alarm
    {
        private DateTime timeGenerated;
        private string clientName;
        private string message;
        private double risk;

        public Alarm(string clientName, string message)
        {
            this.TimeGenerated = DateTime.Now;
            this.ClientName = clientName;
            this.Message = message;
            this.Risk = CalculateRisk();
        }

        [DataMember]
        public DateTime TimeGenerated { get => timeGenerated; private set => timeGenerated = value; }

        [DataMember]
        public string ClientName { get => clientName; set => clientName = value; }

        [DataMember]
        public string Message { get => message; set => message = value; }

        [DataMember]
        public double Risk { get => risk; private set => risk = value; }

        private double CalculateRisk()
        {
            // Dummy implementation for risk calculation
            // You can replace this with a more complex calculation logic
            Random random = new Random();
            return random.NextDouble() * 100; // Risk value between 0 and 100
        }

        public override string ToString()
        {
            return String.Format("TimeGenerated: {0}, ClientName: {1}, Message: {2}, Risk: {3:F2}", TimeGenerated, ClientName, Message, Risk);
        }
    }
}
