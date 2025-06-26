using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HelpDesk.ViewModel
{
   public class MaintenanceFormVM
    {
        public DateTime Date { get; set; }
        public string Customr { get; set; }
        public string TicketType { get; set; }
        public string Handler { get; set; }
        public int SatisfactionLevel { get; set; }
        public string Solution { get; set; }
        public string MaterialUsed { get; set; }
        public string SupportGroup { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }

    }
}
