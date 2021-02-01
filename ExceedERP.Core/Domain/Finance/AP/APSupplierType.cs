using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class APSupplierType : TrackUserSettingOperation
    {
        public int APSupplierTypeID { get; set; }
        [Required]
        [Display(Name = "Supplier Type")]
        public string Supplier { get; set; }
        public string Description { get; set; }
    }
}
