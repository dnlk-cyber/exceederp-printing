
using ExceedERP.Core.Domain.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Inventory
{

    //not used will be same model with store requisition

    public class FuelRequisition : OperationWithSecondValidation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int FuelRequisitionID { get; set; }
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
        [JsonIgnore]
        public virtual ICollection<FuelRequisitionItem> FuelRequisitionItem { get; set; }
        [JsonIgnore]
        public virtual ICollection<FuelRequisitionValidation> FuelRequisitionValidation { get; set; }
        [JsonIgnore]
        public virtual ICollection<FuelRequisitionPropertyValidation> FuelRequisitionPropertyValidation { get; set; }
    }

    public class FuelRequisitionItem : TrackUserSettingOperation
    {
        public int FuelRequisitionItemID { get; set; }
        public int FuelRequisitionID { get; set; }
        public virtual FuelRequisition FuelRequisition { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
       // public string ItemName { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Part No.")]
        public string PartNo { get; set; }
        [Display(Name = "Requested QTY")]
        public decimal RequestedQuantity { get; set; }
        [Display(Name = "First Approved QTY")]
        public decimal FirstApprovalQuantity { get; set; }
        [Display(Name = "Second Approved QTY")]
        public decimal SecondApprovalQuantity { get; set; }
        public bool IsApproved { get; set; }//item by item
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
    }

    public class FuelRequisitionValidation : TrackUserSettingOperation
    {
        public int FuelRequisitionValidationID { get; set; }
        public int FuelRequisitionID { get; set; }
        public virtual FuelRequisition FuelRequisition { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

    public class FuelRequisitionPropertyValidation : TrackUserSettingOperation
    {
        public int FuelRequisitionPropertyValidationID { get; set; }
        public int FuelRequisitionID { get; set; }
        public virtual FuelRequisition FuelRequisition { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string Approver { get; set; }
    }
}
