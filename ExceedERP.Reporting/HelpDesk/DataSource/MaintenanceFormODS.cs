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
        public List<MaintenanceFormVM> GetMainteined(DateTime frm, DateTime t)
        {

            List<MaintenanceFormVM> maintenanceList = new List<MaintenanceFormVM>();

            DateTime from = new DateTime(frm.Year, frm.Month, frm.Day);
            DateTime to = new DateTime(t.Year, t.Month, t.Day);

            var maintenances = db.CustomerTicket_HelpDesks.Where(x=>x.DateCreated >= from && x.DateCreated <= to).ToList();
            foreach (var maint in maintenances)
            {
                var maintenance = new MaintenanceFormVM();

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == maint.EmployeeId);
                if (person != null)
                {
                    maintenance.Customr = person.FullNameaEnglish;
                }

                maintenance.Date = maint.DateCreated.Value;

                maintenance.SatisfactionLevel = maint.Rate;

                var assigns = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == maint.CustomerTicket_HelpDeskId).ToList();

                string solution = "";
                string materialUsed = "";
                string handler = "";

                foreach (var assign in assigns)
                {
                    solution = solution  + assign.Solution + ", ";

                    var materials = db.MaterialsUseds.Where(x=>x.AssignedTicketId == assign.AssignTicket_HelpDeskId).ToList();

                    var assignedTo = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == assign.AssinedTo);

                    if (assignedTo != null)
                    {
                        handler = handler + assignedTo.FullNameaEnglish + ", ";
                    }
                    foreach (var mat in materials)
                    {                      

                        var material = db.MaterialItems.FirstOrDefault(x=>x.MaterialItemsId == mat.MaterialItemsId);

                        if(material != null)
                        {
                            materialUsed = materialUsed + ", " + material.ItemName;
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
