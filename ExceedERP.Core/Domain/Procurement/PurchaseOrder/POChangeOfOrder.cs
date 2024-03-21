using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Procurement.PurchaseOrder
{
    public class PoChangeOforder : NewOperations
    {
        public int PoChangeOforderId { get; set; }
        public int PurchaseOrderID { get; set; }
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual ICollection<PoChangeOfOrderItem> PoChangeOfOrderItems { get; set; }
        public virtual ICollection<PoChangeOforderValidation> PoChangeOforderValidations { get; set; }
    }

    public class PoChangeOfOrderItem : TrackUserOperation
    {
        public int PoChangeOfOrderItemId { get; set; }
        [Display(Name = "Purchase Order")]
        public int PoChangeOforderId { get; set; }
        public virtual PoChangeOforder PoChangeOforder { get; set; }
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
        public decimal Discount { get; set; }
        public string Remark { get; set; }
    }

    public class PoChangeOforderValidation : TrackUserOperation
    {
        public int PoChangeOforderValidationId { get; set; }
        public int PoChangeOforderId { get; set; }
        public virtual PoChangeOforder PoChangeOforder { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}