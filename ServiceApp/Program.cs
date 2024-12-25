/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using Contracts;
using System.IdentityModel.Policy;
using SecurityManager;
using System.Threading;

namespace ServiceApp
{
	public class Program
	{
		static void Main(string[] args)
		{
			NetTcpBinding binding = new NetTcpBinding();
			string address = "net.tcp://localhost:9999/WCFService";
            Database db = new Database();
            binding.Security.Mode = SecurityMode.Transport;                                                     //windows autentifikacija
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(WCFService));
			host.AddServiceEndpoint(typeof(IWCFService), binding, address);

            // podesavamo da se koristi MyAuthorizationManager umesto ugradjenog
            host.Authorization.ServiceAuthorizationManager = new CustomAuthorizationManager();

            // podesavamo custom polisu, odnosno nas objekat principala
            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            host.Open();
			Console.WriteLine("WCFService is opened. Press <enter> to finish...");

            Replicator replicator = new Replicator();
            Thread replicatorThread = new Thread(() => replicator.Run());
            replicatorThread.IsBackground = true;
            replicatorThread.Start();

            Console.ReadLine();
            host.Close();
		}
	}
}
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Security;
using Contracts;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.Threading;

namespace ServiceApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Postavljanje sigurnosnih parametara za primarni server
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/WCFService";
            Database db = new Database();

            // Windows autentifikacija za klijenta
            binding.Security.Mode = SecurityMode.Transport; // Windows autentifikacija
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            // Dodavanje sertifikata za server
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

            // Kreiranje hosta za primarni server
            ServiceHost host = new ServiceHost(typeof(WCFService));
            host.AddServiceEndpoint(typeof(IWCFService), binding, address);

            // Podesavanje autentifikacije između primarnog i sekundarnog servera
            host.Credentials.ServiceCertificate.SetCertificate(
                StoreLocation.LocalMachine,
                StoreName.My,
                X509FindType.FindBySubjectName,
                "primaryserver"); // Sertifikat za primarni server

            // Podesavanje ChainTrust validacije
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            // Otvoriti host
            host.Open();
            Console.WriteLine("WCFService is opened. Press <enter> to finish...");

            // Replikator za sinhronizaciju podataka sa sekundarnim serverom
            Replicator replicator = new Replicator();
            Thread replicatorThread = new Thread(() => replicator.Run());
            replicatorThread.IsBackground = true;
            replicatorThread.Start();

            Console.ReadLine();
            host.Close();
        }
    }
}
