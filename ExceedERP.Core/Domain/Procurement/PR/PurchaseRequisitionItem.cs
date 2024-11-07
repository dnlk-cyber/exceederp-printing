using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PR
{
    public class PurchaseRequisitionItem : TrackUserSettingOperation
    {
        public int PurchaseRequisitionItemID { get; set; }

        [Display(Name = "Purchase Requisition")]
        public int PurchaseRequisitionID { get; set; }
        public virtual PurchaseRequisition PurchaseRequisition { get; set; }
        
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
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Chasis No.")]
        public string ChasisNo { get; set; }
        public decimal RequestedQuantity { get; set; }
        [Display(Name = "Approved Quantity")]
        public decimal Quantity { get; set; }//final
        public bool IsApproved { get; set; }//item by item
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        public string Name { get; set; }//fixed asset only
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        public PurchaseItemType ItemType { get; set; }
        [DisplayName("Budget Code")]
        public string BudgetCode { get; set; }
        public string ProductionJobNo { get; set; }
        public virtual ICollection<PurchaseRequisitionItemSpecification> Specifications { get; set; }

    }

    public class PurchaseRequisitionItemSpecification : TrackUserSettingOperation
    {
        public int PurchaseRequisitionItemSpecificationId { get; set; }

        [Display(Name = "Purchase Requisition")]
        public int PurchaseRequisitionItemID { get; set; }
        public virtual PurchaseRequisitionItem PurchaseRequisitionItem { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Specification { get; set; }
    }
}