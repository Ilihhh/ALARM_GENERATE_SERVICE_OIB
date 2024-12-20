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
				proxy.GenerateAlarm(new Alarm(WindowsIdentity.GetCurrent().Name, "False Alarm - The Weekend"));
				proxy.GetAllAlarms();
				proxy.DeleteClientAlarms();
				proxy.GetAllAlarms();

            }

			Console.ReadLine();
		}
	}
}
