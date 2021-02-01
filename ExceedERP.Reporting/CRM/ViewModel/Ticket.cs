using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.ViewModel
{
            public class Ticket
    {

        public int TicketID { get; set; }

        public string TicketDescription { get; set; }

        public string Customer { get; set; }
        public string TicketGroup { get; set; }

        public string Priority { get; set; }

        public string AssignedTo { get; set; }

        public DateTime? CreatedOn { get; set; }
        
        public string Source { get; set; }
    }
}
