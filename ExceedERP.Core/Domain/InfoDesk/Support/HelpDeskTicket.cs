using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class HelpDeskTicket
    {
        public string HelpDeskTicketId { get; set; }
        public int TicketType_HelpDeskId { get; set; }
        public virtual TicketType_HelpDesk TicketType_HelpDesk { get;set;}
        public virtual SupportGroup_HelpDesk SupportGroup_HelpDesk { get; set; }
        public string Description { get; set; }
    }
}
