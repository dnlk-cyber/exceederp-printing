using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class GeneralJobtitleSetting : TrackUserOperation
    {
        [Key]
        public int GeneralJobtitleSettingId { get; set; }

        [Display(Name = "Is Manual")]
        public bool IsManual { get; set; }

    }

}