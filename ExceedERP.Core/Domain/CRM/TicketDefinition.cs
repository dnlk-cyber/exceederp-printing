using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class TicketDefinition : TrackUserOperation
    {
        public int TicketDefinitionID { get; set; }
        [Required]
        public string Description { get; set; }

        public string Category { get; set; }
    }

}
