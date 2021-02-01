using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
