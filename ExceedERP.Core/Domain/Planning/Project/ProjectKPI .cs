using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning.Project
{
    public class ProjectKPI
    {
        [Key]
        public int ProjectKPIId { get; set; }
        [Display(Name = "KPI Name")]
        [Required]
        public string KPIName { get; set; }
        public string Description { get; set; }
        //[Display(Name = "Target Value")]
        //public decimal TargetValue { get; set; }
        //public decimal Actual { get; set; }
        //The direction and rate of change in the KPI over time.
        public ProjectKPIType ProjectKPIType  { get; set; }


    }
    public class ProjectSubKPI
    {
        public int ProjectSubKPIId { get; set; }
        [Display(Name ="Sub-KPI Name")]
        [Required]
        public string SubKPIName { get; set; }
        [Required]
        public string Unit { get; set; }
        public ProjectSubKPI Parent { get; set; }
        //public string Trend { get; set; }
        //The formula or method used to calculate the KPI.
        //[Display(Name = "Calculation Method")]
        //public string CalculationMethod { get; set; }
        //[Display(Name = "Measurement Method")]
        //public string Frequency { get; set; }
        //public string Status { get; set; }
        //public string Responsibility { get; set; }
        //public string Comments { get; set; }
        [Required]
        public KPIType KPIType { get; set; }
        public string Description { get; set; }
        public int ProjectKPIId { get; set; }

    }
    
    public enum ProjectKPIType
    {

        NA,
        [Display(Name = "Own-Force")]
        OwnForce,
        [Display(Name = "Sublet ")]
        Sublet
    }
}
