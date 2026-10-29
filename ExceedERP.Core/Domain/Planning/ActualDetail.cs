using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
    public class ActualDetail
    {
        [Key]
        public int ActualDetailId { get; set; }  
        public string Unit { get; set; }
        public KPIType KPIType { get; set; }
        public decimal Plan { get; set; }
        public decimal Baseline { get; set; }
        public decimal Target { get; set; }
        public bool HasSubIndicator { get; set; }

        public int ParentId { get; set; }
        public int? PlanningDetailId { get; set; }
        [Display(Name = "Indicator(activity) Name")]
        public int GlobalIndicatorId { get; set; }
        [Display(Name = "Indicator(Activity)")]
        public int IndicatorId { get; set; }
        public virtual Indicator GetIndicator { get; set; }
        public int BSCActualId { get; set; }
        public virtual BSCActual GetSCActual { get; set; }



    }
    //public class ActualDetailWithSubIndicator
    //{
    //    [Key]
    //    public int ActualDetailWithSubIndicatorId { get; set; }
    //    [Display(Name = "Sub-Indicator")]
    //    public int SubIndicatorId { get; set; }
    //    [Display(Name = "Indicator")]
    //    public int? IndicatorId { get; set; }
    //    public string Unit { get; set; }
    //    public KPIType KPIType { get; set; }
    //    public int? PlanningWithSubIndicatorId { get; set; }
    //    public int ActualDetailId { get; set; }
    //    public virtual ActualDetail GetDetail { get; set; }


    //}

    public class MonthlyActual
    {
        [Key]
        public int MonthlyActualId { get; set; }
        [Required]
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [Display(Name = "Plan Value")]
        public decimal PlanValue { get; set; }
        [Display(Name = "Actual Value")]
        public decimal ActualValue { get; set; }
        public int? MonthlyPlanId { get; set; }
        public int ActualDetailId { get; set; }
        public virtual ActualDetail GetDetail { get; set; }

    }
    public class WeeklyActual
    {
        [Key]
        public int WeeklyActualId { get; set; }
        [Display(Name = "Week One")]
        public decimal  WeekOne { get; set; }
        [Display(Name = "Week Two")]
        public decimal WeekTwo { get; set; }
        [Display(Name = "Week Three")]
        public decimal WeekThree { get; set; }
        [Display(Name = "Week Four")]
        public decimal WeekFour { get; set; }
        public int? WeeklyPlanId { get; set; }
        public int MonthlyActualId { get; set; }
        public virtual MonthlyActual MonthlyDetail { get; set; }


    }
}
