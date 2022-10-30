using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class PosSetting : TrackUserOperation
    {
        [Key]
        public int PosSettingId { get; set; }

        [Display(Name ="Setting Type")]
        public PosSettingType SettingType { get; set; }
        public string Value { get; set; }
    }
}
