using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.PR
{
    public class PurchaseRequisitionReassignmentItem : TrackUserSettingOperation
    {
        public int PurchaseRequisitionReassignmentItemId { get; set; }
        public int PurchaseRequisitionAssignmentId { get; set; }
        public virtual PurchaseRequisitionAssignment PurchaseRequisitionAssignment { get; set; }

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
        public decimal Quantity { get; set; }
        public PurchaseItemType ItemType { get; set; }
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        public string Name { get; set; }//fixed asset only
        [Display(Name = "Unit")]
        public string UoM { get; set; }

    }
}