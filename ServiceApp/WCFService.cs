using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Contracts;
using SecurityManager;

namespace ServiceApp
{
    public class WCFService : IWCFService
    {

        [PrincipalPermission(SecurityAction.Demand, Role = "Generate")]
        public void GenerateAlarm(Alarm alarm)
        {
            if (alarm == null)
            {
                throw new ArgumentNullException(nameof(alarm), "Alarm cannot be null.");
            }

            Database.alarms.Add(alarm);
            Console.WriteLine($"New alarm generated: {alarm}");
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Read")]
        public List<Alarm> GetAllAlarms()
        {
            return new List<Alarm>(Database.alarms);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "AlarmAdmin")]
        public void DeleteAllAlarms()
        {
            Database.alarms.Clear();
            Console.WriteLine("All alarms have been deleted.");
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "AlarmAdmin")]
        public void DeleteClientAlarms()                                            //ime klijenta treba da se pribavi kao sto se pribavlja u vezbama 1 (zadatak 1.3) (MOZDA)
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;

            //Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            //Console.WriteLine("Ime klijenta koji je pozvao metodu : " + windowsIdentity.Name);

            string clientName = windowsIdentity.Name;

            Console.WriteLine("Ime klijenta koji je pozvao metodu : " + clientName);

            var alarmsToDelete = Database.alarms.Where(a => a.ClientName == clientName).ToList();

            // Brisanje tih alarma
            foreach (var alarm in alarmsToDelete)
            {
                Database.alarms.Remove(alarm);
                Console.WriteLine($"Alarm za klijenta {clientName} je obrisan: {alarm}");
            }

            if (alarmsToDelete.Count == 0)
            {
                Console.WriteLine($"Nema alarma za klijenta {clientName}.");
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManagePermission(bool isAdd, string rolename, params string[] permissions)
        {
            if (isAdd) // u pitanju je dodavanje
            {
                RolesConfig.AddPermissions(rolename, permissions);
            }
            else // u pitanju je brisanje
            {
                RolesConfig.RemovePermissions(rolename, permissions);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManageRoles(bool isAdd, string rolename)
        {
            if (isAdd) // u pitanju je dodavanje
            {
                RolesConfig.AddRole(rolename);
            }
            else // u pitanju je brisanje
            {
                RolesConfig.RemoveRole(rolename);
            }
        }
    }
}
