using System;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.Core.Domain.Inventory;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Dashboard
{
    public enum FinanceDashboradCategory
    {
        CashAndBank,
        INVENTORY,
        COST_OF_GOOD_SOLD,
        RECEIVABLES,
        SALES_ON_CREDIT,
        FIXED_ASSETS,
        TOTAL_ASSETS,
        RECEIVABLE_TURN_OVER,
        CURRENT_ASSET,
        CURRENT_LIABLITIES,
        SHORT_TERM_MARKETABLE_SECURITIES,
        CASH,
        TOTAL_LIABLITIES,
        TOTAL_DEBT,
        TOTAL_SHAREHOLDERS_EQUITY,
        GROSS_INCOME,
        OPERATING_INCOME,
        NET_INCOME,
        COST_OF_SERVICES,
        SALES,
        INTEREST,
        OVERHEAD,
        INVESTMENT,
        LIQUID_ASSETS,
        TOTAL_DEPOSITS,
        Other
    }
    public class FinanceDashboard
    {
        public int FinanceDashboardId { get; set; }
        [Required]
        public FinanceDashboradCategory Category { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public virtual ICollection<FinanceDashboardRange> FinanceDashboardRanges { get; set; }
    }
    public class FinanceDashboardRange : ReportSetupBase
    {
        public int FinanceDashboardRangeId { get; set; }
        public int FinanceDashboardId { get; set; }
        public virtual FinanceDashboard FinanceDashboard { get; set; }
    }
    //inventory
    public class InventoryItembalance
    {
        public int InventoryItembalanceId { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public decimal Balance { get; set; }
        public string StoreCode { get; set; }
        public DateTime? LastUpdateAt { get; set; }
    }

    public class DashboardDictionary
    {
        public int DashboardDictionaryId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Dimesion { get; set; }
        public decimal Value { get; set; }

        public int? GlFiscalYearId { get; set; }
    }
}
