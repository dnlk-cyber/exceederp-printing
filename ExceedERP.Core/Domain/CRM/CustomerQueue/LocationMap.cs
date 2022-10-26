using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class LocationMap : TrackUserOperation
    {
        [Key]
        public int LocationMapId { get; set; }
        [Display(Name ="Branch")]
        public int BranchId { get; set; }
        public virtual Branch    Branch { get; set; }

        public string LocationName { get; set; }
        public string Description { get; set; }

      
    }
}
