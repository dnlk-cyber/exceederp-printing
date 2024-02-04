using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
    public class ProductionYearlyPlan
    {
        public int ProductionYearlyPlanId { get; set; }
        [Display(Name="Fiscal Year")]
        public int GlFiscalYearId   { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Production Plan")]
        public decimal? TotalProduct { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Sales Plan")]
        public decimal TotalPrice { get; set; }
    }
    public class ProductionMonthlyPlan
    {
        public int ProductionMonthlyPlanId { get; set; }
        public int ProductionYearlyPlanId { get; set; }
        [Display(Name = "Monthly Plan")]
        public int GLPeriodId { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Production Plan")]
        public decimal? TotalProduct { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total Sales Plan")]
        public decimal TotalPrice { get; set; }
    }
    public class ProductionMonthlyJobTypePlan
    {
        public int ProductionMonthlyJobTypePlanId { get; set; }
        public int ProductionMonthlyPlanId { get; set; }
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
