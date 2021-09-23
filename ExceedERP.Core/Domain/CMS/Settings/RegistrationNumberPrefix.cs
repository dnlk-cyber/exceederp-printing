using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CMS
{
    public class RegistrationNumberPrefix : TrackUserSettingOperation
    {
        [Key]
        public int RegistrationNumberPrefixId { get; set; }

        [Required]
        [Display(Name ="Prefix Value")]
        public string PrefixValue { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }

    }
}
