using Contracts;
using ServiceApp;
using System;
using System.Collections.Generic;
using System.ServiceModel;

[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
public class Replicator : IReplicator
{
    private readonly static List<IReplicatorCallback> subscribers = new List<IReplicatorCallback>(); // Lista pretplaćenih klijenata
    private readonly NetTcpBinding binding = new NetTcpBinding();

    public void Run()
    {
        string address = "net.tcp://localhost:9997/Replicator";

        WCFService.DatabaseChanged += NotifyClients;                //prijava na dogadjaj promene baze

        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
        binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

        ServiceHost host = new ServiceHost(typeof(Replicator));
        host.AddServiceEndpoint(typeof(IReplicator), binding, address);

        host.Open();
        Console.WriteLine("Replicator live.");
        Console.ReadLine();

        host.Close();
    }

    public List<Alarm> GetAllAlarms()
    {
        try
        {
            return Database.alarms;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending alarms: {ex.Message}");
        }
        return new List<Alarm>();
    }

    public void Subscribe()
    {
        try
        {
            var callback = OperationContext.Current.GetCallbackChannel<IReplicatorCallback>();
            if (callback == null)
            {
                Console.WriteLine("Callback is null. Check if OperationContext is valid.");
            }
            else
            {
                Console.WriteLine("Callback obtained successfully.");
            }

            if (!subscribers.Contains(callback))
            {
                subscribers.Add(callback);
                
            }
            else
            {
                Console.WriteLine("Client is already subscribed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Subscribe: {ex.Message}");
        }
    }


    public void Unsubscribe()
    {
        var callback = OperationContext.Current.GetCallbackChannel<IReplicatorCallback>();
        if (subscribers.Contains(callback))
        {
            subscribers.Remove(callback);
            Console.WriteLine("Client unsubscribed.");
        }
    }

    public void NotifyClients()
    {
        Console.WriteLine("Notifying clients...");
        //Console.WriteLine(subscribers.Count);

        //Console.WriteLine(subscribers);
        foreach (var subscriber in subscribers.ToArray())
        {
            try
            {
                if (((ICommunicationObject)subscriber).State == CommunicationState.Opened)
                {
                    subscriber.NotifyClient();
                }
                else
                {
                    Console.WriteLine("Client is disconnected, removing from subscribers.");
                    subscribers.Remove(subscriber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error notifying client: {ex.Message}. Removing from subscribers.");
                subscribers.Remove(subscriber);
            }
        }
    }

}
