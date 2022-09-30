
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.InfoDesk;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class TicketActivityDefinition_HelpDesk : TrackUserOperation
    {
        public int TicketActivityDefinition_HelpDeskId { get; set; }
        [Required]
        public string Description { get; set; }
        public TicketActivityStatus TicketActivityStatus { get; set; }
        [Required]
        public int Index { get; set; }
        public bool CanFinalize { get; set; }
        public string Remark { get; set; }

    }

}
