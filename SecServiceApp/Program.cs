using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using Contracts;
using Manager;
using SecurityManager;

namespace SecServiceApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Common Name (CN) of the secondary server certificate
            string secondaryCertCN = "secondaryservercert";

            // Load the secondary server certificate from the Windows Certificate Store
            X509Certificate2 secondaryCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, secondaryCertCN);

            if (secondaryCert == null)
            {
                Console.WriteLine("[ERROR] Secondary server certificate not found.");
                return;
            }

            // Create NetTcpBinding with security settings
            NetTcpBinding binding = new NetTcpBinding
            {
                Security =
                {
                    Mode = SecurityMode.Transport,
                    Transport = { ClientCredentialType = TcpClientCredentialType.Certificate }
                }
            };

            string address = "net.tcp://localhost:8888/SecondaryService";

            // Create ServiceHost instance
            ServiceHost host = new ServiceHost(typeof(WCFService));
            host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

            // Set certificate validation mode to ChainTrust
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;

            // Disable certificate revocation check (CRL)
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            // Assign the secondary server certificate
            host.Credentials.ServiceCertificate.Certificate = secondaryCert;

            try
            {
                // Start the service
                host.Open();
                Console.WriteLine("Secondary WCFService is started.\nPress <Enter> to stop...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
