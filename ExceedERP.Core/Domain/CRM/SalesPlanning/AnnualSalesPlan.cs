using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class AnnualSalesPlan : Operations
    {
        [Key]
        public int AnnualSalesPlanId { get; set; }

        [Display(Name = "Budget Year")]
        public int GlFiscalYearId { get; set; }

        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        [Display(Name = "Sub Category")]
        public string SubCategoryId { get; set; }

        [ExclusiveRange(0, ErrorMessage ="Average Price should be greater than 0")]
        [Display(Name = "Avg Price")]
        public decimal AveragePrice { get; set; }
        [Display(Name = "Profit %")]
        public decimal Profit { get; set; }
        
        [ExclusiveRange(0, ErrorMessage ="Quantity should be greater than 0")]
        [Display(Name = "Quantity")]
        public decimal TotalAnnualPlanQuantity { get; set; }
        [Display(Name = "Value")]
        public decimal TotalAnnualPlanValue { get; set; }

        [Display(Name = "Quantity")]
        public decimal TotalMonthlyPlanQuantity { get; set; }
        [Display(Name = "Value")]
        public decimal TotalMonthlyPlanValue { get; set; }

        [Display(Name = "Quantity")]
        public decimal TotalWeeklyPlanQuantity { get; set; }
        [Display(Name = "Value")]
        public decimal TotalWeeklyPlanValue { get; set; }

        [Display(Name = "Quantity")]
        public decimal TotalDailyPlanQuantity { get; set; }
        [Display(Name = "Value")]
        public decimal TotalDailyPlanValue { get; set; }

    }
    public class BranchAnnualSalesPlan : Operations
    {
        [Key]
        public int BranchAnnualSalesPlanId { get; set; }
        public int AnnualSalesPlanId { get; set; }
        public virtual AnnualSalesPlan AnnualSalesPlan { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        public decimal Ratio { get; set; }
        public decimal Quantity { get; set; }
        //public decimal AveragePrice { get; set; }
        //public decimal Profit { get; set; }
        public decimal Value { get; set; }
    }
    public class StoreAnnualSalesPlan : TrackUserOperation
    {
        [Key]
        public int StoreAnnualSalesPlanId { get; set; }

        public int BranchAnnualSalesPlanId { get; set; }
        public virtual BranchAnnualSalesPlan BranchAnnualSalesPlan { get; set; }

        //[Display(Name = "Sales Center")]
        ////public int SalesStoreId { get; set; }
        //public string StoreCode { get; set; }

        [Display(Name ="Sales Center")]
        public int SalesStoreId { get; set; }

        //[Display(Name ="Period")]
        //public int CRMPeriodId { get; set; }
        public decimal Ratio { get; set; }
        public decimal AnnualQuantity { get; set; }

        public decimal AnnualValue { get; set; }

        [NotMapped]
        public bool IsReadonly { get; set; }
    }
    public class StoreMonthlySalesPlan : TrackUserOperation
    {
        [Key]
        public int StoreMonthlySalesPlanId { get; set; }
        public int StoreAnnualSalesPlanId { get; set; }
        public virtual StoreAnnualSalesPlan StoreAnnualSalesPlan { get; set; }

        [Display(Name = "Period")]
        public int CRMPeriodId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Value { get; set; }
    }

    public class AnnualSalesPlanValidation : VoucherValidation
    {
        [Key]
        public int AnnualSalesPlanValidationId { get; set; }


        public int AnnualSalesPlanId { get; set; }

        public virtual AnnualSalesPlan AnnualSalesPlan { get; set; }
    }
}
