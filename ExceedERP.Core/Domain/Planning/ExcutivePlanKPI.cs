using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
    public class ExcutivePlanKPI: TrackUserSettingOperation
    {
        public int ExcutivePlanKPIId { get; set; }
        [Display(Name = "Indicator")]
        public int IndicatorId { get; set; }


    }
}