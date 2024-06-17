using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{

    public class GlobalIndicator : TrackUserSettingOperation
    {
        [Key]
        public int GlobalIndicatorId { get; set; }
        [Display(Name = "Indicator Name ")]
        [Required]
        public string Indicator { get; set; }
        public string Description { get; set; }

    }
}
