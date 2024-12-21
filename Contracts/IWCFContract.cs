using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IWCFContract
    {
        /// <summary>
        /// Prima alarm za replikaciju od primarnog servera.
        /// </summary>
        /// <param name="alarm">Objekat alarma koji sadrži informacije o alarmu.</param>
        [OperationContract]
        void ReplicateAlarm(Alarm alarm);
    }
}
