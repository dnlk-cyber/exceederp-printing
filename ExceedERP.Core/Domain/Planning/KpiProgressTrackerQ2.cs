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

    public class KpiProgressTracker : TrackUserSettingOperation
    {
        [Key]
        public int KpiProgressTrackerId { get; set; }
        [Display(Name = "KPI-Name ")]
        public int GlobalKPIId { get; set; }
        public virtual GlobalKPI KPI { get; set; }
        [Display(Name = "Weight % ")]
        public decimal? Weight { get; set; }
        [Display(Name = "Annual Plan  ")]
        public decimal? AnnualPlan { get; set; }
        [Display(Name = "Plan")]
        public decimal? UpTo2ndQuarterPlan { get; set; }
        [Display(Name = " Performance")]
        public decimal? UpTo2ndQuarterPerformance { get; set; }
        [Display(Name = " %")]
        public decimal? UpTo2ndQuarterPercent { get; set; }
        [Display(Name = " Evaluation")]
        public decimal? Evaluation { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }


    }

    public class SubKpiProgressTracker : TrackUserSettingOperation
    {
        [Key]
        public int SubKpiProgressTrackerId { get; set; }
        [Display(Name = "Sub-KPI-Name ")]
        public int GlobalSubKPIId { get; set; }
        [Display(Name = "Indicators ")]
        public string Indicators { get; set; }
        [Display(Name = "Saf ")]
        public string Saf { get; set; }
        [Display(Name = "Weight % ")]
        public decimal Weight  { get; set; }
        [Display(Name = "Annual Plan  ")]
        public decimal AnnualPlan  { get; set; }
        [Display(Name = "Plan")]
        public decimal UpTo2ndQuarterPlan  { get; set; }
        [Display(Name = " Performance")]
        public decimal UpTo2ndQuarterPerformance { get; set; }
        [Display(Name = " %")]
        public decimal UpTo2ndQuarterPercent  { get; set; }
        [Display(Name = " Evaluation")]
        public decimal Evaluation { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
        public int KpiProgressTrackerId { get; set; }
        public virtual KpiProgressTracker KpiProgress { get; set; }


    }
}
