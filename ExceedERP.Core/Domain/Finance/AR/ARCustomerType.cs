using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArCustomerType : TrackUserSettingOperation
    {
        public int ArCustomerTypeId { get; set; }
        [Required]
        [Display(Name = "Customer Type")]
        public string CustomerTypeName { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
    }
}
