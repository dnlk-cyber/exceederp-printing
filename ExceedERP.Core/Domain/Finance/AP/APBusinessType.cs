using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class APBusinessType : TrackUserSettingOperation
    {
        public int APBusinessTypeID { get; set; }
        [Required]
        [Display(Name = "Business Type")]
        public string TypeOfBusiness { get; set; }
        public string Description { get; set; }
    }
}
