using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
    public class FurnitureProductionYearlyPlan
    {
        public int FurnitureProductionYearlyPlanId { get; set; }
        [Display(Name="Fiscal Year")]
        public int GlFiscalYearId   { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Production Plan")]
        public decimal? TotalProduct { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Sales Plan")]
        public decimal TotalPrice { get; set; }
    }
    public class FurnitureProductionMonthlyPlan
    {
        public int FurnitureProductionMonthlyPlanId { get; set; }
        public int FurnitureProductionYearlyPlanId { get; set; }
        [Display(Name = "Monthly Plan")]
        public int GLPeriodId { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Production Plan")]
        public decimal? TotalProduct { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Sales Plan")]
        public decimal TotalPrice { get; set; }
    }
    public class FurnitureProductionMonthlyJobTypePlan
    {
        public int FurnitureProductionMonthlyJobTypePlanId { get; set; }
        public int FurnitureProductionMonthlyPlanId { get; set; }
        [Display(Name = "Job Type")]
        public int JobTypeId { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Production Plan")]
        public decimal? TotalProduct { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Sales Plan")]
        public decimal TotalPrice { get; set; }
    }
}
