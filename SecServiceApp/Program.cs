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
            // Ime sertifikata za primarni i sekundarni server (Common Name - CN)
            string primaryCertCN = "primaryservercert";
            string secondaryCertCN = "secservercert";

            // Učitavanje sertifikata iz Windows Certificate Store-a
            X509Certificate2 secondaryCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, secondaryCertCN);

            if (secondaryCert == null)
            {
                Console.WriteLine("[ERROR] Secondary server certificate not found.");
                return;
            }

            // Kreiranje NetTcpBinding sa sigurnosnim postavkama
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:8888/SecondaryService";

            // Kreiranje ServiceHost instance
            ServiceHost host = new ServiceHost(typeof(WCFService));
            host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

            // Postavljanje validacije sertifikata na ChainTrust
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            //host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            //host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new Custom();

            // Onemogućavanje provere opoziva sertifikata (CRL)
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            // Postavljanje sertifikata za sekundarni server
            host.Credentials.ServiceCertificate.Certificate = secondaryCert;

            try
            {
                // Pokretanje servisa
                host.Open();
                Console.WriteLine("Secondary WCFService is started.\nPress <enter> to stop ...");
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
