using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.ServiceModel;
using Contracts;

namespace ClientApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/WCFService";

            binding.Security.Mode = SecurityMode.Transport; // Windows autentifikacija
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address))))
            {
                int choice;
                do
                {
                    Console.Clear();
                    Console.WriteLine("=== WCF Service Client Menu ===");
                    Console.WriteLine("1. Generate Alarm");
                    Console.WriteLine("2. Get All Alarms");
                    Console.WriteLine("3. Delete All Alarms");
                    Console.WriteLine("4. Delete Client Alarms");
                    Console.WriteLine("0. Exit");
                    Console.Write("Choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out choice))
                    {
                        Console.WriteLine("Invalid input! Press Enter to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            Console.Write("Enter alarm description: ");
                            string description = Console.ReadLine();
                            Alarm newAlarm = new Alarm(WindowsIdentity.GetCurrent().Name, description);
                            proxy.GenerateAlarm(newAlarm);
                            break;

                        case 2:
                            proxy.GetAllAlarms();
                            break;

                        case 3:
                            proxy.DeleteAllAlarms();
                            break;

                        case 4:
                            proxy.DeleteClientAlarms();
                            break;


                        case 0:
                            Console.WriteLine("Exiting...");
                            break;

                        default:
                            Console.WriteLine("Invalid choice! Press Enter to continue...");
                            break;
                    }

                    if (choice != 0)
                    {
                        Console.WriteLine("Press Enter to return to the menu...");
                        Console.ReadLine();
                    }
                } while (choice != 0);
            }
        }
    }
}
