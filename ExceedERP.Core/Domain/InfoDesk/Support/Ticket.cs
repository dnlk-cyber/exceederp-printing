using System;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{

    public class Ticket_HelpDesk : Operations
    {
        public int Ticket_HelpDeskId { get; set; }
        
        public int Priority { get; set; }
        public int Group { get; set; }
        public int Status { get; set; }

        public string AssignedTo { get; set; }

        public string EscalationLevel { get; set; }

        public string Source { get; set; }

        [DisplayName("Solution Given")]
        public string SolutionGiven { get; set; }
        [DisplayName("Future Measurement")]
        public string FutureMeasurement { get; set; }
        public DateTime DateResponded { get; set; }

        public int TicketDefinitionID { get; set; }
        public virtual TicketDefinition_HelpDesk TicketDefinition { get; set; }


    }
}
