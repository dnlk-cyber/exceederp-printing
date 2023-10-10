using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lot
{
    public class ParticipantsItem
    {
        public int ParticipantsItemId { get; set; }
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
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        public string Name { get; set; }//fixed asset
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        public PurchaseItemType ItemType { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        [Display(Name = "Rebate Amount")]
        public decimal DiscountAmount { get; set; }
        public virtual Procurement.Tender.Lot.Lot Lot { get; set; }
        public int LotID { get; set; }
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        public float Technical { get; set; }
        public int LotItemID { get; set; }//link to lot item
    }
}
