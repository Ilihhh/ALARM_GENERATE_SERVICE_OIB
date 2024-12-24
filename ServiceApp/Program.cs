/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using Contracts;
using System.IdentityModel.Policy;
using SecurityManager;

namespace ServiceApp
{
	public class Program
	{
		static void Main(string[] args)
		{
			NetTcpBinding binding = new NetTcpBinding();
			string address = "net.tcp://localhost:9999/WCFService";

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
			Console.ReadLine();

			host.Close();
		}
	}
}
*/// Program.cs - Primarni server
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using Contracts;
using SecurityManager;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Policy;
using Manager;
using System.ServiceModel.Security;

namespace ServiceApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Sertifikati za primarni i sekundarni server
            string primaryCertCN = "primaryservercert";
            string secondaryCertCN = "secondaryservercert";

            // Učitavanje sertifikata iz Windows Certificate Store-a
            X509Certificate2 primaryCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, primaryCertCN);
            X509Certificate2 secondaryCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, secondaryCertCN);

            if (primaryCert == null)
            {
                Console.WriteLine("[ERROR] Primary server certificate not found.");
                return;
            }

            if (secondaryCert == null)
            {
                Console.WriteLine("[ERROR] Secondary server certificate not found.");
                return;
            }

            // Kreiranje ChannelFactory za komunikaciju sa sekundarnim serverom
            ChannelFactory<IWCFContract> factory = new ChannelFactory<IWCFContract>(
                new NetTcpBinding(SecurityMode.Transport)
                {
                    Security =
                    {
                        Transport = { ClientCredentialType = TcpClientCredentialType.Certificate }
                    }
                },
                new EndpointAddress(new Uri("net.tcp://localhost:8888/SecondaryService"),
                                    new X509CertificateEndpointIdentity(secondaryCert))
            );

            // Postavljanje klijentskog sertifikata
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.LocalMachine,
                StoreName.My,
                X509FindType.FindBySubjectName,
                primaryCertCN
            );

            // Postavljanje validacije sertifikata
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            IWCFContract secondaryService = factory.CreateChannel();

            // Kreiranje NetTcpBinding za komunikaciju sa klijentima
            NetTcpBinding binding = new NetTcpBinding
            {
                Security =
                {
                    Mode = SecurityMode.Transport, // Windows autentifikacija
                    Transport =
                    {
                        ClientCredentialType = TcpClientCredentialType.Windows,
                        ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign
                    }
                }
            };

            string address = "net.tcp://localhost:9999/WCFService";

            // Kreiranje ServiceHost instance sa prosleđenim sekundarnim servisom
            ServiceHost host = new ServiceHost(new WCFService(secondaryService));
            host.AddServiceEndpoint(typeof(IWCFService), binding, address);

            // Podesavanje custom autorizacije
            host.Authorization.ServiceAuthorizationManager = new CustomAuthorizationManager();
            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>
            {
                new CustomAuthorizationPolicy()
            };
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            // Kreiranje i pokretanje replikatora
            Replikator replikator = new Replikator(secondaryService);
            replikator.Start();

            try
            {
                // Pokretanje hosta za klijente
                host.Open();
                Console.WriteLine("Primary WCFService is opened. Press <enter> to finish...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                // Zatvaranje hosta i replikatora
                replikator.Stop();

                if (host.State == CommunicationState.Opened)
                    host.Close();

                if (((IClientChannel)secondaryService).State == CommunicationState.Opened)
                    ((IClientChannel)secondaryService).Close();
            }
        }
    }
}