using System;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Timers;
using Contracts;
using SecurityManager;

namespace SecondaryServiceApp
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]

    public class ReplicatorClientHandler : IReplicatorCallback, IDisposable
    {
        private readonly SecondaryServiceClient secondaryServiceClient;
        private readonly EndpointAddress secondaryServiceAddress;
        private readonly NetTcpBinding binding = new NetTcpBinding
        {
            Security =
                {
                    Mode = SecurityMode.Transport,
                    Transport =
                    {
                        ClientCredentialType = TcpClientCredentialType.Windows,
                        ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign
                    }
                }
        };
        private readonly EndpointAddress replicatorAddress;
        private DuplexChannelFactory<IReplicator> duplexFactory;
        private IReplicator replicatorProxy;
        private bool disposed = false;

        public ReplicatorClientHandler()
        {
            // Podesi replicator klijent
            

            replicatorAddress = new EndpointAddress("net.tcp://localhost:9997/Replicator");

            // Kreiranje duplex kanala za replicator
            InstanceContext callbackContext = new InstanceContext(this);
            duplexFactory = new DuplexChannelFactory<IReplicator>(callbackContext, binding, replicatorAddress);
            replicatorProxy = duplexFactory.CreateChannel();

            // Podesi secondary service klijent
            secondaryServiceAddress = new EndpointAddress("net.tcp://localhost:9998/WCFSecondaryService");
            secondaryServiceClient = new SecondaryServiceClient(binding, secondaryServiceAddress);

            // Pretplati se na obaveštenja
            Subscribe();
        }

        public void GetAllAlarms()
        {
            try
            {
                Audit.LogReplicationInitiated();

                var alarms = replicatorProxy.GetAllAlarms();
                if (alarms != null)
                {
                    secondaryServiceClient.WriteAlarms(alarms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAlarms: {ex.Message}");
            }
        }

        public void Subscribe()
        {
            try
            {
                replicatorProxy.Subscribe();
                //Console.WriteLine("Successfully subscribed to replicator notifications.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during subscription: {ex.Message}");
            }
        }

        public void Unsubscribe()
        {
            try
            {
                replicatorProxy.Unsubscribe();
                //Console.WriteLine("Successfully unsubscribed from replicator notifications.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during unsubscription: {ex.Message}");
            }
        }

        public void NotifyClient()
        {
            Console.WriteLine("Notification received from replicator.");
            GetAllAlarms();                 //bukv radimo isto kao i inicijalno
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                    Unsubscribe();
                    DisposeClient(secondaryServiceClient);

                    if (duplexFactory != null)
                    {
                        ((ICommunicationObject)duplexFactory)?.Close();
                    }
                }
                disposed = true;
            }
        }

        private void DisposeClient<T>(T client) where T : class, ICommunicationObject
        {
            try
            {
                if (client?.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
            catch
            {
                client?.Abort();
            }
        }

        ~ReplicatorClientHandler()
        {
            Dispose(false);
        }
    }
}
