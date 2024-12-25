using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts;

namespace SecondaryServiceApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9998/WCFSecondaryService";

            binding.Security.Mode = SecurityMode.Transport;                                                     //windows autentifikacija
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(WCFSecondaryService));
            host.AddServiceEndpoint(typeof(IWCFSecondaryService), binding, address);


            host.Open();
            Console.WriteLine("WCFSecondaryService is opened. Press <enter> to finish...");

            using (var replicatorHandler = new ReplicatorClientHandler())
            {
                replicatorHandler.GetAllAlarms();
                Console.ReadLine();
            }

            host.Close();
        }
    }

    
}
