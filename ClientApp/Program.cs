using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using System.Security.Principal;

namespace ClientApp
{
	public class Program
	{
		static void Main(string[] args)
		{
			NetTcpBinding binding = new NetTcpBinding();
			string address = "net.tcp://localhost:9999/WCFService";

            binding.Security.Mode = SecurityMode.Transport;                                                         //windows autentifikacija
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address))))
            {
                while (true)
                {
                    menu();
                    string input = Console.ReadLine();

                    switch (input)
                    {
                        case "1":
                            Console.WriteLine("Enter alarm name:");
                            string alarmName = Console.ReadLine();
                            proxy.GenerateAlarm(new Alarm(WindowsIdentity.GetCurrent().Name, alarmName));
                            break;
                        case "2":
                            proxy.GetAllAlarms();
                            break;
                        case "3":
                            proxy.DeleteClientAlarms();
                            break;
                        case "q":
                            Console.WriteLine("Exiting application...");
                            return;
                        default:
                            Console.WriteLine("Not valid input.");
                            break;
                    }
                }
            }
		}

		private static void menu()
		{
			string result = "Menu:\n1 - Generate Alarm\n2 - Get Alarm\n3 - Delete Alarms\nq - Exit app";
			Console.WriteLine(result);
		}
	}
}
