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
    public class GlobalKPI : TrackUserSettingOperation
    {
        [Key]
        public int GlobalKPIId { get; set; }
        [Display(Name = "KPI-Name ")]
        [Required]
        public string KPIName { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Strategic Objective")]
        public string StrategicObjective { get; set; }

        [Display(Name = "Key Result Area (KRA)")]
        public string KeyResultArea { get; set; }

        [Display(Name = "Annual Objective (KPI Focus)")]
        public string AnnualObjective { get; set; }

        [Display(Name = "Standard KPI")]
        public string StandardKPI { get; set; }

        [Display(Name = "Measurement Formula / Metric")]
        public string MeasurementFormula { get; set; }

        [Display(Name = "Annual Target")]
        public string AnnualTargetText { get; set; }

        [Display(Name = "Threshold")]
        public string ThresholdText { get; set; }

    }

    public class GlobalSubKPI : TrackUserSettingOperation
    {
        [Key]
        public int GlobalSubKPIId { get; set; }
        [Display(Name = "Sub-KPI-Name ")]
        [Required]
        public string SubKPIName { get; set; }
        public int GlobalKPIId { get; set; }
        public virtual GlobalKPI KPI { get; set; }


    }
  
}
