using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class TicketActivityDefinition : TrackUserOperation
    {
        public int TicketActivityDefinitionID { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public int Index { get; set; }


        public bool CanFinalize { get; set; }

        public string Remark { get; set; }

    }

}
