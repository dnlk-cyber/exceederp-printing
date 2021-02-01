using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
