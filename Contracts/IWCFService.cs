using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IWCFService
    {
        /// <summary>
        /// Vraća sve alarme iz baze podataka.
        /// Potrebna dozvola: Read
        /// </summary>
        [OperationContract]
        List<Alarm> GetAllAlarms();

        /// <summary>
        /// Generiše novi alarm i dodaje ga u bazu podataka.
        /// Potrebna dozvola: Alarm Generator
        /// </summary>
        [OperationContract]
        void GenerateAlarm(Alarm alarm);

        /// <summary>
        /// Briše sve alarme iz baze podataka.
        /// Potrebna dozvola: AlarmAdmin
        /// </summary>
        [OperationContract]
        void DeleteAllAlarms();

        /// <summary>
        /// Briše sve alarme koje je generisao trenutni klijent (na osnovu imena klijenta).
        /// Potrebna dozvola: AlarmAdmin
        /// </summary>
        [OperationContract]
        void DeleteClientAlarms();

        /// <summary>
        /// Upravljanje permisijama za određene uloge.
        /// </summary>
        [OperationContract]
        void ManagePermission(bool isAdd, string rolename, params string[] permissions);

        /// <summary>
        /// Upravljanje korisničkim ulogama.
        /// </summary>
        [OperationContract]
        void ManageRoles(bool isAdd, string rolename);
    }
}
