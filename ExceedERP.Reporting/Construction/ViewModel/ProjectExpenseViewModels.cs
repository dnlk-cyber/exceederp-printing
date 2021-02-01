using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    public class MaterialExpenseViewModel
    {
        public string MaterialType { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DeliveredQuantity { get; set; }
        public decimal ConsumedQuantity { get; set; }
        public decimal TranIn { get; set; }
        public decimal TranOut { get; set; }
        // previous month
        public decimal DeliveredQuantityPrevMonth { get; set; }
        public decimal ConsumedQuantityPrevMonth { get; set; }
        public decimal MaterilOnSitePrevMonth { get; set; }
        // to date
        public decimal DeliveredQuantityToDate { get; set; }
        public decimal ConsumedQuantityToDate { get; set; }
        public decimal ShipedQuantityToDate { get; set; }
        public decimal MaterialOnSiteToDate { get; set; }
        public decimal StockBalance { get; set; }
        // cost
        public decimal CostOfConsumedMaterialThisMonth { get; set; }
        public decimal CostOfConsumedMaterialPrevMonth { get; set; }
        public decimal CostOfConsumedMaterialToDate { get; set; }
        public decimal StockAmount { get; set; }
        public decimal MaterialOnSiteAmount { get; set; }
    }
    //
    public class PeriodViewModel
    {
        public int PeriodId { get; set; }
        public string Name { get; set; }

    }
    public class ItemCategoryViewModel
    {
        public string ItemCategoryCode { get; set; }
        public string Name { get; set; }
    }
}
