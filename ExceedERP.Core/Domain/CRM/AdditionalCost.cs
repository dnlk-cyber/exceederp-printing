using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.CRM;
using System.ComponentModel;
namespace ExceedERP.Core.Domain.CRM
{
    public class AdditionalCost : TrackUserOperation
    {
        [DisplayName("Id")]
        public int AdditionalCostId { get; set; }
         [Required]
        public CRMEnums.AdditionalCostType Type { get; set; }
        [Required]
        public string Name { get; set; }
          [Required]
        public string SalesAccountCode { get; set; }
        
    }
}
