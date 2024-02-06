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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{

    public class FurnitureJobProcuctionPlan : ValidationBase
    {
        public int FurnitureJobProcuctionPlanId { get; set; }
        [Display(Name = "Budget Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [NotMapped]
        public string Name { get; set; }
        public virtual ICollection<FurnitureHalfYearlyProductionPlan> FurnitureHalfYearlyPlans { get; set; }





    }

    public class FurnitureHalfYearlyProductionPlan : NameSettingBase
    {
        public int FurnitureHalfYearlyProductionPlanId { get; set; }
        public int YearlyProductionPlanId { get; set; }
        public virtual FurnitureJobProcuctionPlan FurnitureJobProcuctionPlan { get; set; }
        public virtual ICollection<FurnitureQuarterlyProductionPlan> FurnitureQuarterlyPlans { get; set; }





    }
    public class FurnitureQuarterlyProductionPlan : NameSettingBase
    {
        [Display(Name = "Quarterly Plan")]
        public int FurnitureQuarterlyProductionPlanId { get; set; }
        public int FurnitureHalfYearlyProductionPlanId { get; set; }
        public virtual FurnitureHalfYearlyProductionPlan FurnitureHalfYearlyProductionPlan { get; set; }
        public virtual ICollection<MonthlyProductionPlan> FurnitureMonthlyProductionPlans { get; set; }







    }
    public class FurnitureMonthlyProductionPlan : NameSettingBase
    {
        [Display(Name = "Monthly Plan")]
        public int FurnitureMonthlyProductionPlanId { get; set; }
        [Display(Name = "Quarterly Plan")]
        public int QuarterlyProductionPlanId { get; set; }
        [Display(Name = "Working Hours")]
        public decimal WorkingDays { get; set; }
        [Display(Name = "Working Days ")]
        public decimal WorkingHours { get; set; }
        public virtual FurnitureQuarterlyProductionPlan QuarterlyProductionPlan { get; set; }
        public virtual ICollection<FurnitureWeeklyProductionPlan> WeeklyProductionPlans { get; set; }






    }
    public class FurnitureWeeklyProductionPlan : NameSettingBase
    {
        public int FurnitureWeeklyProductionPlanId { get; set; }
        public int FurnitureMonthlyProductionPlanId { get; set; }
        public virtual FurnitureMonthlyProductionPlan MonthlyPlan { get; set; }
    }
    public class FurnitureProductionJobTypePlan : ProductionPlan
    {
        public int FurnitureProductionJobTypePlanId { get; set; }
        public int FurnitureWeeklyProductionPlanId { get; set; }
        [Display(Name = "Job Type")]
        public int JobTypeId { get; set; }
        public FurnitureJobCategory FurnitureJobType { get; set; }
        public virtual FurnitureWeeklyProductionPlan WeeklyPlan { get; set; }


    }
    public class FurnitureProductionPlan
    {
        [Display(Name = "Production Plan in Qty ")]
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        public decimal ProductionPlanQuantity { get; set; }
        [Display(Name = "Production Plan in Birr ")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal ProductionPlanBirr { get; set; }
    }
    public class FurnitureValidationBase : FurnitureProductionPlan
    {
        [Display(Name = "Start Date")]
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

    public class FurnitureNameSettingBase : FurnitureValidationBase
    {
        [Display(Name = "Name Am")]
        public string NameAm { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
    public class FurnitureYearlyProductionPlan : ValidationBase
    {
        public int FurnitureYearlyProductionPlanId { get; set; }
        [Display(Name = "Budget Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [NotMapped]
        public string Name { get; set; }
        public virtual ICollection<HalfYearlyProductionPlan> HalfYearlyPlans { get; set; }


    }
}

