using ExceedERP.Core.Domain.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Inventory
{

    
    public class StoreRequisitionBase : OperationWithSecondValidation
    {
        [Display(Name = "Request Reference")]
        public string Reference { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "Requester Name")]
        public int EmployeeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }
        [Required]
        [Display(Name = "Store Code")]
        public string StoreCode { get; set; }
        [DisplayName("To Store")]
        public string ToStoreCode { get; set; }
        [DisplayName("Is Fuel Request")]
        public bool IsFuel { get; set; }
        [DisplayName("Is Transfer")]
        public bool IsTransfer { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [DisplayName("Is Fullfilled")]
        public bool IsFullfilled { get; set; }
        [DisplayName("Is For Other")]
        public bool IsForOther { get; set; }
       
        [DisplayName("For Branch or Project")]
        public int? ForBranchId { get; set; }
        
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Job Order No.")]
        public string JobOrderNo { get; set; }
        [Display(Name = "Plate No")]
        public string PlateNo { get; set; }

        
    }

    public class StoreRequisitionItemBase : TrackUserSettingOperation
    {
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        //public string ItemName { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Part No.")]
        public string PartNo { get; set; }
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Chasis No.")]
        public string ChasisNo { get; set; }
        [Display(Name = "Requested QTY")]
        public decimal RequestedQuantity { get; set; }
        [Display(Name = "Approved QTY")]
        public decimal ApprovedQuantity { get; set; }
        [Display(Name = "Issued QTY")]
        public decimal IssuedQuantity { get; set; }
        [Display(Name = "Purchase QTY")]
        public decimal PurchaseQuantity { get; set; }
        public bool IsApproved { get; set; }//item by item
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        [Display(Name = "Generate PR")]
        public bool GeneratePurchaseReq { get; set; }
        [NotMapped]
        public string Unit { get; set; }
        [NotMapped]
        public decimal CurrentBalance { get; set; }
    }

    public class StoreRequisitionValidationBase : TrackUserSettingOperation
    {
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }

    public class StoreRequisitionPropertyValidationBase : TrackUserSettingOperation
    {
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public string Approver { get; set; }
    }

}