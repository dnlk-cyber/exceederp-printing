using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.PR
{
    public enum PurchaseItemType
    {
        Stock,
        FixedAsset,
        Service
    }

    public class PurchaseRequisition : OperationWithThirdValidation
    {

        public int PurchaseRequisitionID { get; set; }
        [Display(Name = "Store Requisition Number")]
        public int StoreRequisitionID { get; set; }//
         //public virtual StoreRequisition StoreRequisition { get; set; }
        [DisplayName("Business Unit")]
        public int BusinessUnitId { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        
        [DisplayName("For Branch or Project")]
        public int ? ForBranchId { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "Requestor")]
        public int EmployeeId { get; set; }
        [Display(Name = "Reference Number")]
        public string ReferenceNo { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Team Leader")]
        public int ProcurementOfficer { get; set; }
        [Display(Name = "Category")]
        public PurchaseCategory PurchaseCategory { get; set; }
        [Display(Name = "Method")]
        public ProcurementMethod ProcurementMethod { get; set; }
        [Display(Name = "Tender Type")]
        public TenderType TenderType { get; set; }
        [Display(Name = "From Previous Purchase ?")]
        public bool FromPreviousPurchase { get; set; }

        [Display(Name = "Previous Method")]
        public ProcurementMethod PreviousProcurementMethod { get; set; }
        [Display(Name = "Previous Purchase Reference")]
        public string PreviousPurchaseReference { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public PurchaseItemType ItemType { get; set; }
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Plate No.")]
        public string PlateNo { get; set; }
        public bool IsManual { get; set; }

        [DisplayName("Is Fullfilled")]
        public bool IsFullfilled { get; set; }
        public string NextApprover { get; set; }       
        //budget controll
        public bool IsBudgetApproved { get; set; }
        public string BudgetApprovedBy { get; set; }
        public DateTime? BudgetApprovedTime { get; set; }
        public virtual ICollection<PurchaseRequisitionItem> PurchaseRequisitionItems { get; set; }
        public virtual ICollection<PurchaseRequisitionUserDepartmentValidation> PurchaseRequisitionUserDepartmentValidation { get; set; }
        public virtual ICollection<PurchaseRequisitionSecondValidation> PurchaseRequisitionSecondValidation { get; set; }
        public virtual ICollection<PurchaseRequisitionPurchaseDepartmentValidation> PurchaseRequisitionPurchaseDepartmentValidation { get; set; }
        public virtual ICollection<PurchaseRequisitionAssignment> PurchaseRequisitionAssignments { get; set; }
        public virtual ICollection<PurchaseRequisitionAttachment> PurchaseRequisitionAttachments { get; set; }
    }
}