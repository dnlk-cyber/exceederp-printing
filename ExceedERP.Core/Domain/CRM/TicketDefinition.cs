using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
