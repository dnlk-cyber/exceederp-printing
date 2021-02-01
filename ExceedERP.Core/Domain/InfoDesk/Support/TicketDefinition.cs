using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class TicketDefinition_HelpDesk : TrackUserOperation
    {
        public int TicketDefinition_HelpDeskId { get; set; }
        [Required]
        public string Description { get; set; }

        public string Category { get; set; }
    }

}
