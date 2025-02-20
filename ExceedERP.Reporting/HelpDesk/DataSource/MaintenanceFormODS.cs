using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HelpDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HelpDesk.DataSource
{
    [DataObject]
    public class MaintenanceFormODS
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MaintenanceFormVM> GetMainteined(DateTime? from, DateTime? to, string ticketTypeId, string groupId,string ticketStatus,string assignedToId)
        {

            List<MaintenanceFormVM> maintenanceList = new List<MaintenanceFormVM>();

           
            var maintenances = db.CustomerTicket_HelpDesks.ToList();
            if (from.HasValue)
            {
                 maintenances = maintenances.Where(x => x.DateCreated >= from.Value).ToList();
            }
            if (to.HasValue)
            {
                 maintenances = maintenances.Where(x => x.DateCreated <= to.Value).ToList();
            }
            if (!string.IsNullOrEmpty(ticketTypeId))
            {
                int.TryParse(ticketTypeId,out int typeId);
                maintenances = maintenances.Where(x=>x.TicketType_HelpDeskId == typeId).ToList();
            }
            if(!string.IsNullOrEmpty(groupId))
            {
                int.TryParse(groupId, out int supportGroupId);
                maintenances = maintenances.Where(x => x.SupportGroup_HelpDeskId == supportGroupId).ToList();
            }
            if (!string.IsNullOrEmpty(ticketStatus))
            {
                maintenances = maintenances.Where(x => x.TicketStatus.ToString() == ticketStatus).ToList();
                
            }
            if (!string.IsNullOrEmpty(assignedToId))
            {
                int.TryParse(assignedToId, out int emplopyeeId);
                var masterIds = db.AssignTicket_HelpDesks.Where(x => x.AssinedTo == emplopyeeId).Select(x => x.CustomerTicketId);

                maintenances = maintenances.Where(x =>masterIds.Contains( x.CustomerTicket_HelpDeskId)).ToList();
                
            }
            foreach (var maint in maintenances)
            {
                var maintenance = new MaintenanceFormVM();
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == maint.EmployeeId);
                if (person != null)
                {
                    maintenance.Customr = person.FullNameaEnglish;
                }
                var ticketType = db.TicketType_HelpDesks.FirstOrDefault(x=>x.TicketType_HelpDeskId == maint.TicketType_HelpDeskId);
                if(ticketType != null)
                {
                    maintenance.TicketType = ticketType.Type;
                }
                var group = db.SupportGroup_HelpDesks.FirstOrDefault(x=>x.SupportGroup_HelpDeskId == maint.SupportGroup_HelpDeskId);
                if(group != null)
                {
                    maintenance.SupportGroup = group.Name;
                }
                maintenance.Date = maint.DateCreated.Value;
                maintenance.SatisfactionLevel = maint.Rate;
                var assigns = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == maint.CustomerTicket_HelpDeskId).ToList();

                string solution = maint.SolutionGiven+" ";
                string materialUsed = "";
                string handler = "";
                foreach (var assign in assigns)
                {
                    solution = solution + assign.Solution + ", ";

                    var materials = db.MaterialsUseds.Where(x => x.AssignedTicketId == assign.AssignTicket_HelpDeskId).ToList();

                    var assignedTo = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == assign.AssinedTo);

                    if (assignedTo != null)
                    {
                        handler = handler + assignedTo.FullNameaEnglish + ", ";
                    }
                    foreach (var mat in materials)
                    {

                        var material = db.MaterialItems.FirstOrDefault(x => x.MaterialItemsId == mat.MaterialItemsId);

                        if (material != null)
                        {
                            materialUsed = materialUsed + material.ItemName + " - " + mat.Quantity + ", ";
                        }
                    }
                }

                maintenance.Handler = handler;
                maintenance.Solution = solution;
                maintenance.MaterialUsed = materialUsed;

                maintenanceList.Add(maintenance);
            }

            return maintenanceList;
        }
    }
}
