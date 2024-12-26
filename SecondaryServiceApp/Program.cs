using System;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using Contracts;
using System.ServiceModel.Security;

namespace SecondaryServiceApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Postavljanje sigurnosnih parametara za sekundarni server
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9998/WCFSecondaryService";

            // Windows autentifikacija za klijenta
            binding.Security.Mode = SecurityMode.Transport; // Windows autentifikacija
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            // Dodavanje sertifikata za server
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

            // Kreiranje hosta za sekundarni server
            ServiceHost host = new ServiceHost(typeof(WCFSecondaryService));
            host.AddServiceEndpoint(typeof(IWCFSecondaryService), binding, address);

            // Podesavanje autentifikacije između sekundarnog i primarnog servera
            host.Credentials.ServiceCertificate.SetCertificate(
                StoreLocation.LocalMachine,
                StoreName.My,
                X509FindType.FindBySubjectName,
                "secondaryserver"); // Sertifikat za sekundarni server

            // Podesavanje ChainTrust validacije
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            // Otvoriti host
            host.Open();
            Console.WriteLine("WCFSecondaryService is opened. Press <enter> to finish...");

            // Replikator za sinhronizaciju podataka sa primarnim serverom
            using (var replicatorHandler = new ReplicatorClientHandler())
            {
                replicatorHandler.GetAllAlarms();
                Console.ReadLine();
            }

            host.Close();
        }
    }
}
