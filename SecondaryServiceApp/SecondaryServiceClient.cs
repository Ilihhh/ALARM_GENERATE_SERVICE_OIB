using System;
using System.Collections.Generic;
using System.ServiceModel;
using Contracts;

namespace SecondaryServiceApp
{
    public class SecondaryServiceClient : ChannelFactory<IWCFSecondaryService>, IWCFSecondaryService, IDisposable
    {
        private readonly IWCFSecondaryService factory;

        public SecondaryServiceClient(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void WriteAlarms(List<Alarm> alarms)
        {
            factory.WriteAlarms(alarms);
        }

        public void Dispose()
        {
            if (this.State == CommunicationState.Opened)
            {
                this.Close();
            }
            else
            {
                this.Abort();
            }
        }
    }
}
