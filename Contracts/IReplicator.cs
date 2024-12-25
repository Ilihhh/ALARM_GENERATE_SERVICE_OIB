using System.Collections.Generic;
using System.ServiceModel;

namespace Contracts
{
    public interface IReplicatorCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyClient();
    }

    [ServiceContract(CallbackContract = typeof(IReplicatorCallback))]
    public interface IReplicator
    {
        [OperationContract]
        List<Alarm> GetAllAlarms();

        [OperationContract(IsOneWay = true)]
        void Subscribe();

        [OperationContract(IsOneWay = true)]
        void Unsubscribe();
    }
}
