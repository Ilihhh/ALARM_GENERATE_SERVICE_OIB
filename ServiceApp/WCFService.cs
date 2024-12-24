using System;
using System.Collections.Generic;
using System.IO;
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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WCFService : IWCFService
    {
        private readonly IWCFContract _secondaryService;

        public WCFService(IWCFContract secondaryService)
        {
            _secondaryService = secondaryService ?? throw new ArgumentNullException(nameof(secondaryService), "Secondary service cannot be null.");
        }
        
        [PrincipalPermission(SecurityAction.Demand, Role = "Generate")]
        public void GenerateAlarm(Alarm alarm)
        {
            if (alarm == null)
            {
                throw new ArgumentNullException(nameof(alarm), "Alarm cannot be null.");
            }

            Database.alarms.Add(alarm);
            Console.WriteLine($"New alarm generated: {alarm}");

            Database.SaveAlarms(); // Save to file

            // Dodavanje alarma u buffer za replikaciju
            try
            {
                File.AppendAllText("replication_buffer.txt", $"{alarm.ClientName},{alarm.Message},{alarm.Risk}{Environment.NewLine}");
                Console.WriteLine("Alarm added to replication buffer.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add alarm to buffer: {ex.Message}");
            }
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

            Database.SaveAlarms(); // Save to file
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "AlarmAdmin")]
        public void DeleteClientAlarms()
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            string clientName = windowsIdentity?.Name;
            Console.WriteLine("Client name who called the method: " + clientName);

            var alarmsToDelete = Database.alarms.Where(a => a.ClientName == clientName).ToList();

            foreach (var alarm in alarmsToDelete)
            {
                Database.alarms.Remove(alarm);
                Console.WriteLine($"Alarm for client {clientName} has been deleted: {alarm}");
            }

            if (alarmsToDelete.Count == 0)
            {
                Console.WriteLine($"No alarms found for client {clientName}.");
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManagePermission(bool isAdd, string rolename, params string[] permissions)
        {
            if (isAdd)
            {
                RolesConfig.AddPermissions(rolename, permissions);
            }
            else
            {
                RolesConfig.RemovePermissions(rolename, permissions);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManageRoles(bool isAdd, string rolename)
        {
            if (isAdd)
            {
                RolesConfig.AddRole(rolename);
            }
            else
            {
                RolesConfig.RemoveRole(rolename);
            }
        }
    }
}