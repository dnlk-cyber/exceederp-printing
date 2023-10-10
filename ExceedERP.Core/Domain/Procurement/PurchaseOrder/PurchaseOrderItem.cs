using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.PR;


namespace ExceedERP.Core.Domain.Procurement.PurchaseOrder
{
    public class PurchaseOrderItem : TrackUserOperation
    {
        public int PurchaseOrderItemID { get; set; }
        [Display(Name = "Purchase Order")]
        public int PurchaseOrderID { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        [Required]
        [DisplayName("Category Code")]
        public string CatCode { get; set; }
        [Required]
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItmCode { get; set; }
        [Required]
        [DisplayName("Item")]
        public string ItemName { get; set; }
        [Display(Name = "Part No.")]
        public string PartNo { get; set; }
        public decimal Quantity { get; set; }
        public bool IsApproved { get; set; }//item by item
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        public string Name { get; set; }//fixed asset
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        public PurchaseItemType ItemType { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        public string Remark { get; set; }
       public bool FromPreviousPurchase { get; set; }
    }
}
