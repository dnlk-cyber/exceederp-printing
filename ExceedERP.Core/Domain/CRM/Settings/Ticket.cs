using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class Ticket : Operations
    {
        public int TicketID { get; set; }

        public string TicketDefinitionID { get; set; }

        public string ConsigneeID { get; set; }
        public string CustomerAddress { get; set; }

        public int Priority { get; set; }
        public int Group { get; set; }
        public int Status { get; set; }

        public string AssignedTo { get; set; }

        public string EscalationLevel { get; set; }

        public string Source { get; set; }


    }
}
