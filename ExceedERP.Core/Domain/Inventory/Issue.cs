using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Inventory
{
    public class DepartmentCostCenter
    {
        public int DepartmentCostCenterID { get; set; }
        [Required]
        [Display(Name = "Department or work Unit")]
        public string DepartmentName { get; set; }
        [Required]
        [Display(Name = "Cost Center Code")]
        public string LocationCode { get; set; }
    }



    public class Issue : Operations
    {
        //public static readonly string[] MachineConditions = { "New", "Used" };
        public int IssueID { get; set; }
        [Display(Name = "Store Requisition Number")]
        public int StoreRequisitionID { get; set; }
        //public virtual StoreRequisition StoreRequisition { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = "Owner")]
        public IssueFuelVehicleOwner Owner { get; set; }
        [Display(Name = "Vehicle Plate No.")]
        public string PlateNo { get; set; }
        [Display(Name = "Account Code")]
        public string SupplierAccountCode { get; set; }
        [Display(Name = "Has Reading?")]
        public bool HasReading { get; set; }
        [Display(Name = "Machine Reading KM.")]
        public string ReadingKM { get; set; }
        [Display(Name = "Issue Reference")]
        public string Reference { get; set; }
        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime IssueDate { get; set; }
        [DisplayName("Is Fuel Request")]
        public bool IsFuel { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [Display(Name = "Job Order No.")]
        public string JobOrderNo { get; set; }
        [DisplayName("For Branch or Project")]
        public int ForBranchId { get; set; }
        [Display(Name = "Requester Name")]
        public int EmployeeId { get; set; }
        [Display(Name = "Recipient Name")]
        public int RecipientId { get; set; }
        public bool IsManual { get; set; }
        [Display(Name = "Issue for Asset")]
        public int? AssetRegistrationId { get; set; }
        public virtual ICollection<IssueItem> IssueItems { get; set; }
        public virtual ICollection<IssueValidation> IssueValidation { get; set; }
        public virtual ICollection<IssueDistribution> IssueDistribution { get; set; }
    }

   
    public class IssueItem : TrackUserOperation
    {
        public int IssueItemID { get; set; }
        public int IssueID { get; set; }
        public virtual Issue Issue { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        //public string ItemName { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Requested QTY")]
        public decimal RequestedQuantity { get; set; }
        [Display(Name = "Approved QTY")]
        public decimal ApprovedQuantity { get; set; }
        [Display(Name = "Issued QTY")]
        public decimal IssuedQuantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Additional Cost")]
        public decimal AdditionalCost { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        //[Required]
        //[Display(Name = "GL Account")]
        //public string GLAccountCode { get; set; }
        //public string IssueGoodReceiveItemID { get; set; }
        //public string IssueGoodReceiveItemPrice { get; set; }
        [Display(Name = "Employee of Fuel Company")]
        public string EmployeeOfFuelCompany { get; set; }
        [Display(Name = "Employee Who got Refueled")]
        public string EmployeeWhoGotItRefuelled { get; set; }
        public string Name { get; set; }
        [Display(Name = "Work Item Code")] 
        public string WorkItemCode { get; set; }

        [Display(Name = "Job No")]
        public string ProductionJobNo { get; set; }
        [Display(Name = "Job No")]
        public string MaintenanceJobNo { get; set; }
    }
    public class IssueValidation : VoucherValidation
    {
        public int IssueValidationID { get; set; }
        public int IssueID { get; set; }
        public virtual Issue Issue { get; set; }
    }

    public class IssueDistribution:VoucherDistribution
    {
        public int IssueDistributionID { get; set; }
        public int IssueID { get; set; }
        public virtual Issue Issue { get; set; }
        public int IssueItemIDTrackBack { get; set; }
    }
}