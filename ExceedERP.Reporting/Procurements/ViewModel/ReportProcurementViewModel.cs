using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
   

    public class ProcurementNeedViewModel
    {
        public string Id { get; set; }
        
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        public string Function { get; set; }
        public string PerformanceRequirement { get; set; }
        public string PhysicalCharacteristics { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinimumQualityRequired { get; set; }
        public string BudgetSource { get; set; }
        public string ProcurementCategory { get; set; }
        public decimal? EstimatedBudget { get; set; }
        public DateTime? EstimatedTimeTheItemIsNeeded { get; set; }
        public string Unit { get; set; }
        public decimal July { get; set; }
        public decimal Aug { get; set; }
        public decimal Sep { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Dec { get; set; }
        public decimal Jan { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }
        public decimal Apr { get; set; }
        public decimal May { get; set; }
        public decimal June { get; set; }
    }

    public class ProcurementPlanViewModel
    {
        public string Id { get; set; }
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "Source of Budget")]
        public string BudgetSource { get; set; }
        [Display(Name = "Procurement Category")]
        public string ProcurementCategory { get; set; }
        [Display(Name = "Procurement Method")]
        public string ProcurementMethod { get; set; }
        [Display(Name = "Tender Floating Time")]
        public DateTime? TenderFloatingTime { get; set; }
        [Display(Name = "Delivery Time")]
        public DateTime? PurchaseDeliveryTime { get; set; }
    }
    
}