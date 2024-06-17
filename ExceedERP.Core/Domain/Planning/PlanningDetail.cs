using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
    public class PlanningDetail
    {
        public int PlanningDetailId { get; set; }      
        public string Unit { get; set; }
        [Display(Name = "KPI-Type")]
        public KPIType KPIType { get; set; }
        public bool HasSubIndicator { get; set; }
        public int ParentId { get; set; }
        [Display(Name="Annual Plan")]
        public decimal Plan { get; set; }
        [Display(Name = "Base line")]
        public decimal Baseline { get; set; }
        [Display(Name = "Target")]
        public decimal Target { get; set; }
        [Display(Name = "Indicator Name")]
        public int GlobalIndicatorId { get; set; }
        [Display(Name = "Indicator")]
        public int IndicatorId { get; set; }
        public  virtual Indicator GetIndicator { get; set; }
        public int BSCPlanId { get; set; }
        public virtual BSCPlan GetBSCPlan { get; set; }

    }
    //public class PlanningWithSubIndicator
    //{
    //    [Key]
    //    public int PlanningWithSubIndicatorId { get; set; }
    //    public decimal Plan { get; set; }
    //    public decimal Baseline { get; set; }
    //    public decimal Target { get; set; }
    //    [Display(Name = "Sub-Indicator")]
    //    public int SubIndicatorId { get; set; }
    //    [Display(Name = "Indicator")]
    //    public int IndicatorId { get; set; }
    //    public int PlanningDetailId { get; set; }
    //    public virtual PlanningDetail GetDetail { get; set; }

    //}
   
    public class MonthlyPlan
    {
        [Key]
        public int MonthlyPlanId { get; set; }
        [Required]
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [Display(Name ="Plan Value")]
        public decimal Value { get; set; }
        [Display(Name ="Actual Value")]
        public decimal ActualValue { get; set; }
        public int PlanningDetailId { get; set; }
        public virtual PlanningDetail GetDetail { get; set; }
    }
    public class WeeklyPlan
    {
        [Key]
        public int WeeklyPlanId { get; set; }
        [Display(Name = "Week One")]
        public decimal WeekOne { get; set; }
        [Display(Name = "Week Two")]
        public decimal WeekTwo { get; set; }
        [Display(Name = "Week Three")]
        public decimal WeekThree { get; set; }
        [Display(Name = "Week Four")]
        public decimal WeekFour { get; set; }
        [Display(Name = "Week 1")]
        public decimal ActualWeekOne { get; set; }
        [Display(Name = "Actual Week 2")]
        public decimal ActualWeekTwo { get; set; }
        [Display(Name = "Actual Week 3")]
        public decimal ActualWeekThree { get; set; }
        [Display(Name = "Actual Week 4")]
        public decimal ActualWeekFour { get; set; }
        public int MonthlyPlanId { get; set; }
        public virtual MonthlyPlan MonthlyDetail { get; set; }



    }
    public class PlanFile
    {
        public int DesignFileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        [Display(Name = " Date"),
         DataType(DataType.Date),
         DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AttachedDate { get; set; }

    }
}
