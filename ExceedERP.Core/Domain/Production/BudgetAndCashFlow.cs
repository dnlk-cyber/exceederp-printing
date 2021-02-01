using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Production
{
    #region Projected Monthly Cash Flow
    public class ProjectedMonthlyCashFlow : TrackUserOperation
    {
        [Key]
        public int ProjectedMonthlyCashFlowId { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "Budget Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        public string Remark { get; set; }
    }
    public class ProjectedMonthlyCashFlowItem : TrackUserOperation
    {
        public int ProjectedMonthlyCashFlowItemId { get; set; }
        public int ProjectedMonthlyCashFlowId { get; set; }
        public virtual ProjectedMonthlyCashFlow ProjectedMonthlyCashFlow { get; set; }
        [Display(Name ="Period")]
        public int ProductionPeriodId { get; set; }
        public virtual ProductionPeriod ProductionPeriod { get; set; }

        // income
        [Display(Name ="Sale of Clean Seed")]
        public decimal CleanProductSales { get; set; }
        public decimal CustomCleaningIncome { get; set; }
        [Display(Name ="By-product Sales")]
        public decimal ByproductSales { get; set; }
        public decimal TotalIncome { get; set; }

        // expenditure
        [Display(Name ="Raw Seed Purchase")]
        public decimal RawProductPurchase { get; set; }
        public decimal OtherDirectCosts { get; set; }
        public decimal ProcessingAndStorage { get; set; }
        public decimal Maintenance { get; set; }
        public decimal Administration { get; set; }
        public decimal HeadOfficeAllocation { get; set; }
        public decimal TotalExpenditure { get; set; }

        // non-cash expenditure
        public decimal Depreciation { get; set; }

        public decimal TotalCashExpenditure { get; set; }

        public decimal NetCashIncome { get; set; }

        public decimal CumulativeCashIncome { get; set; }

        // Corrected
        public decimal AdditionalIncome { get; set; }
        public decimal RevisedNetCashIncome { get; set; }
        public decimal RevisedCumulativeCashIncome { get; set; }
    }
    #endregion

    #region Cleaned Product Sale
    public class CleanedProductSale : TrackUserOperation
    {
        [Key]
        public int CleanedProductSaleId { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "Budget Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
    }
    public class CleanedProductSaleItem : TrackUserOperation
    {
        [Key]
        public int CleanedProductSaleItemId { get; set; }
        public int CleanedProductSaleId { get; set; }
        public virtual CleanedProductSale CleanedProductSale { get; set; }

        [Display(Name ="Item")]
        public string ItemCode { get; set; }
        [Display(Name ="External")]
        public decimal ExternalSaleQuantity { get; set; }
        [Display(Name = "Farm")]
        public decimal OnFarmSaleQuantity { get; set; }
        [Display(Name = "Total")]
        public decimal TotalQuantity { get; set; }

        [Display(Name = "External")]
        public decimal UnitPriceExternal { get; set; }
        [Display(Name = "Farm")]
        public decimal UnitPriceOnFarm { get; set; }

        [Display(Name = "External")]
        public decimal ExternalSaleAmount { get; set; }
        [Display(Name = "Farm")]
        public decimal OnFarmSaleAmount { get; set; }
        [Display(Name = "Total")]
        public decimal TotalSaleAmount { get; set; }
    }
    public class CleanedProductSaleItemPerMonth : TrackUserOperation
    {
        [Key]
        public int CleanedProductSaleItemPerMonthId { get; set; }
        public int CleanedProductSaleItemId { get; set; }
        public virtual CleanedProductSaleItem CleanedProductSaleItem { get; set; }
        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }
        public virtual ProductionPeriod ProductionPeriod { get; set; }
        public decimal Amount { get; set; }
    }
    #endregion

    #region Raw Product Purchase
    public class RawProductPurchase : TrackUserOperation
    {
        public int RawProductPurchaseId { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "Budget Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
    }
    public class RawProductPurchaseItem : TrackUserOperation
    {
        public int RawProductPurchaseItemId { get; set; }
        public int RawProductPurchaseId { get; set; }
        public virtual RawProductPurchase RawProductPurchase { get; set; }

        [Display(Name ="Item")]
        public string ItemCode { get; set; }
        [Display(Name = "External")]
        public decimal ExternalPurchaseQuantity { get; set; }
        [Display(Name = "Farm")]
        public decimal OnFarmPurchaseQuantity { get; set; }
        [Display(Name = "Total")]
        public decimal TotalQuantity { get; set; }

        [Display(Name = "External")]
        public decimal UnitPriceExternal { get; set; }
        [Display(Name = "Farm")]
        public decimal UnitPriceOnFarm { get; set; }

        [Display(Name = "External")]
        public decimal ExternalPurchaseAmount { get; set; }
        [Display(Name = "Farm")]
        public decimal OnFarmPurchaseAmount { get; set; }
        [Display(Name = "Total")]
        public decimal TotalPurchaseAmount { get; set; }
    }
    public class RawProductPurchaseItemPerMonth : TrackUserOperation
    {
        [Key]
        public int RawProductPurchaseItemPerMonthId { get; set; }
        public int RawProductPurchaseItemId { get; set; }
        public virtual RawProductPurchaseItem RawProductPurchaseItem { get; set; }
        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }
        public virtual ProductionPeriod ProductionPeriod { get; set; }
        public decimal Amount { get; set; }
    }
    #endregion

    #region Custom Cleaning Service & Storage
    public class CustomCleaningAndStorage : TrackUserOperation
    {
        [Key]
        public int CustomCleaningAndStorageId { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "Budget Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
    }
    public class CustomCleaningAndStorageItem : TrackUserOperation
    {
        [Key]
        public int CustomCleaningAndStorageItemId { get; set; }
        public int CustomCleaningAndStorageId { get; set; }
        public virtual CustomCleaningAndStorage CustomCleaningAndStorage { get; set; }

        [Display(Name = "Item")]
        public string ItemCode { get; set; }
        [Display(Name = "External")]
        public decimal ExternalQuantity { get; set; }
        [Display(Name = "Farm")]
        public decimal OnFarmQuantity { get; set; }
        [Display(Name = "Total")]
        public decimal TotalQuantity { get; set; }

        [Display(Name = "External")]
        public decimal UnitPriceExternal { get; set; }
        [Display(Name = "Farm")]
        public decimal UnitPriceOnFarm { get; set; }

        [Display(Name = "External")]
        public decimal ExternalAmount { get; set; }
        [Display(Name = "Farm")]
        public decimal OnFarmAmount { get; set; }
        [Display(Name = "Total")]
        public decimal TotalAmount { get; set; }
    }
    public class CustomCleaningAndStorageItemPerMonth : TrackUserOperation
    {
        [Key]
        public int CustomCleaningAndStorageItemPerMonthId { get; set; }
        public int CustomCleaningAndStorageItemId { get; set; }
        public virtual CustomCleaningAndStorageItem CustomCleaningAndStorageItem { get; set; }
        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }
        public virtual ProductionPeriod ProductionPeriod { get; set; }
        public decimal Amount { get; set; }
    }
    #endregion
}
