using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.Plan
{



    public enum FundSources
    {
        Government,
        InternalRevenue,
        Loan,
        Support
    }

    public enum Budgets
    {
        CapitalBudget,
        RecurrentBudget
    }
    public class ProcurementPlanItem : TrackUserOperation
    {
        public int ProcurementPlanItemId { get; set; }
        [Display(Name = "Procurement Plan")]
        public int ProcurementPlanId { get; set; }
        public virtual ProcurementPlan ProcurementPlan { get; set; }
        [Required]
        [DisplayName("Category Code")]
        public string CatCode { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItmCode { get; set; }
        [DisplayName("Item")]
        public string ItemName { get; set; }
        [Display(Name = "Part No.")]
        public string PartNo { get; set; }
        public string Name { get; set; }//fixed asset name
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public PurchaseItemType ItemType { get; set; }
        public string Type { get; set; }//same as above for UI purpose
        [Display(Name = "Source of Budget")]
        public ProcurementBudgetSource BudgetSource { get; set; }
        [Display(Name = "Procurement Category")]
        public ProcurementCategory ProcurementCategory { get; set; }
        [Display(Name = "Procurement Method")]
        public ProcurementMethod ProcurementMethod { get; set; }
        [Display(Name = "Tender Floating Time")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TenderFloatingTime { get; set; }
        [Display(Name = "Delivery Time")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PurchaseDeliveryTime { get; set; }
        [Display(Name = "July")]
        public decimal Period1 { get; set; }
        [Display(Name = "August")]
        public decimal Period2 { get; set; }
        [Display(Name = "September")]
        public decimal Period3 { get; set; }
        [Display(Name = "Octobeber")]
        public decimal Period4 { get; set; }
        [Display(Name = "November")]
        public decimal Period5 { get; set; }
        [Display(Name = "December")]
        public decimal Period6 { get; set; }
        [Display(Name = "January")]
        public decimal Period7 { get; set; }
        [Display(Name = "February")]
        public decimal Period8 { get; set; }
        [Display(Name = "March")]
        public decimal Period9 { get; set; }
        [Display(Name = "April")]
        public decimal Period10 { get; set; }
        [Display(Name = "May")]
        public decimal Period11 { get; set; }
        [Display(Name = "June")]
        public decimal Period12 { get; set; }
        public string Remark { get; set; }
    }
}
