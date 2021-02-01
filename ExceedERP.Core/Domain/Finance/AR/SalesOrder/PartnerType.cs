using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
    public class PartnerType : TrackUserSettingOperation
    {
        public int PartnerTypeId { get; set; }
        [Required]
        [Display(Name = "Partner Type")]
        public string TypeName { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
    }
}
