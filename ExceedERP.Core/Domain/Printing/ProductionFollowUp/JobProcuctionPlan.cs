using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Localization;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{

    public class YearlyProductionPlan : ValidationBase
    {
        public int YearlyProductionPlanId { get; set; }
        [Display(Name="Budget Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [NotMapped]
        public string Name { get; set; }
        public virtual ICollection<HalfYearlyProductionPlan> HalfYearlyPlans { get; set; }





    }
    public class HalfYearlyProductionPlan : NameSettingBase
    {
        public int HalfYearlyProductionPlanId { get; set; }
        public int YearlyProductionPlanId { get; set; }
        public virtual YearlyProductionPlan YearlyPlan { get; set; }
        public virtual ICollection<QuarterlyProductionPlan> QuarterlyPlans { get; set; }





    }
    public class QuarterlyProductionPlan : NameSettingBase
    {
        [Display(Name="Quarterly Plan")]
        public int QuarterlyProductionPlanId { get; set; }
        public int HalfYearlyProductionPlanId { get; set; }
        public virtual HalfYearlyProductionPlan HalfYearlyProductionPlan { get; set; }
        public virtual ICollection<MonthlyProductionPlan> MonthlyProductionPlans { get; set; }







    }
    public class MonthlyProductionPlan : NameSettingBase
    {
        [Display(Name ="Monthly Plan")]
        public int MonthlyProductionPlanId { get; set; }
        [Display(Name = "Quarterly Plan")]
        public int QuarterlyProductionPlanId { get; set; }
        [Display(Name = "Working Hours")]
        public decimal WorkingDays { get; set; }
        [Display(Name = "Working Days ")]
        public decimal WorkingHours { get; set; }
        public virtual QuarterlyProductionPlan QuarterlyProductionPlan { get; set; }   
        public virtual ICollection<WeeklyProductionPlan> WeeklyProductionPlans { get; set; }






    }
    public class WeeklyProductionPlan : NameSettingBase
    {
        public int WeeklyProductionPlanId { get; set; }
        public int MonthlyProductionPlanId { get; set; }
        public virtual MonthlyProductionPlan MonthlyPlan { get; set; }
    }
    public class ProductionJobTypePlan : ProductionPlan
    {
        public int ProductionJobTypePlanId { get; set; }
        public int WeeklyProductionPlanId { get; set; }
        [Display(Name = "Job Type")]
        public int JobTypeId { get; set; }
        public JobCategory JobType { get; set; }
        public virtual WeeklyProductionPlan WeeklyPlan { get; set; }


    }
    public class ProductionPlan
    {
        [Display(Name = "Production Plan in Qty ")]
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        public decimal ProductionPlanQuantity { get; set; }
        [Display(Name = "Production Plan in Birr ")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal ProductionPlanBirr { get; set; }
    }
   public class ValidationBase: ProductionPlan
    { 
        [Display(Name ="Start Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Start Date Eth")]
        public string StartDateEth { get; set; }
        [Display(Name = "End Date Eth")]
        public string EndDateEth { get; set; }
    }

        public class NameSettingBase : ValidationBase
        {
          [Display(Name ="Name Am")]
           public string NameAm { get; set; }
          [Display(Name = "Name")]
           public string Name { get; set; }            
    }
    
}
