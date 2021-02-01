using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.PurchaseOrder
{


    public class PurchaseFollowUp : TrackUserOperation
    {
        public int PurchaseFollowUpID { get; set; }
        [Display(Name = "Store Requisition Number")]
        public int PurchaseRequisitionID { get; set; }
        public virtual PurchaseRequisition PurchaseRequisition { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "Store Keeper Name")]
        public int EmployeeId { get; set; }
        [Display(Name = "Reference Number")]
        public string ReferenceNo { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Procurement Officer")]
        public int ProcurementOfficer { get; set; }
        [Display(Name = "Method")]
        public ProcurementMethod ProcurementMethod { get; set; }
        [Display(Name = "Category")]
        public PurchaseCategory PurchaseCategory { get; set; }
        [Display(Name = "Status")]
        public PurchaseStatus PurchaseStatus { get; set; }
        public virtual ICollection<PurchaseFollowUpItems> PurchaseFollowUpItems { get; set; }
        public virtual ICollection<PurchaseFollowUpStatus> PurchaseFollowUpStatus { get; set; }
        public virtual ICollection<PurchaseFollowUpSystemStatus> PurchaseFollowUpSystemStatus { get; set; }
    }
    public class PurchaseFollowUpItems
    {
        public int PurchaseFollowUpItemsID { get; set; }
        public int PurchaseFollowUpID { get; set; }
        public virtual PurchaseFollowUp PurchaseFollowUp { get; set; }
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
        public PurchaseItemType ItemType { get; set; }
        [Display(Name = "Part No.")]
        public string PartNo { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        [Display(Name = "Supplier")]
        public string Supplier { get; set; }
        [Display(Name = "Purchase order")]
        public string PurchaserOrder { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Insurance Applied Date")]
        [DataType(DataType.Date)]
        public DateTime? InsuranceAppliedDate { get; set; }
        [Display(Name = "Insurance certificate No.")]
        public string InsuranceNo { get; set; }
        [Display(Name = "Insurance Certificate Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? InsuranceCertificateDate { get; set; }
        [Display(Name = "Date F.C Permit Applied ")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FCAppliedDate { get; set; }
        [Display(Name = "Date F.C Permit")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FCPermitDate { get; set; }
        [Display(Name = "F.C Permit No.")]
        public string FCPermitNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "F.C.Permit Expiry ")]
        [DataType(DataType.Date)]
        public DateTime? FCPermitExpiryDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date L.C Applied ")]
        [DataType(DataType.Date)]
        public DateTime? LCAppliedDate { get; set; }
        [Display(Name = "L.C  No.")]
        public string LCNo { get; set; }
        [Display(Name = "Date L.C Opened ")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? LCOpenedDate { get; set; }
        [Display(Name = "Name of Vessel ")]
        public string Vessel { get; set; }
        [Display(Name = "Port of Loading ")]
        public string Port { get; set; }
        [Display(Name = "Date Shipped ")]
        [DataType(DataType.Date)]
        public DateTime? ShippedDate { get; set; }
        [Display(Name = "Date vessel arrived ")]
        [DataType(DataType.Date)]
        public DateTime? VesselArriveDate { get; set; }
        [Display(Name = "Bill of Loading No.")]
        public string BillOfLoading { get; set; }
        [Display(Name = "Date Document Collect From Bank ")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? DocumentCollectionDate { get; set; }
        [Display(Name = "Date & Ref. Doc. Submitted to M.T.S.C")]
        [DataType(DataType.Date)]
        public DateTime? DocumentSubmitDate { get; set; }
        [Display(Name = "Maritime Operation No.")]
        public string Maritimeno { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Goods Arrived")]
        [DataType(DataType.Date)]
        public DateTime? ArriveDate { get; set; }
        [Display(Name = "Condition of goods")]
        public string Condition { get; set; }
        [Display(Name = "Condition of goods(Damaged)")]
        public decimal Damaged { get; set; }
        [Display(Name = "Condition of goods(Missing)")]
        public decimal Missing { get; set; }
        [Display(Name = "Date Claim Loading to Ins.")]
        [DataType(DataType.Date)]
        public DateTime? ClaimDate { get; set; }
        [Display(Name = "Claim Loading to Ins.Ref. No.")]
        public string RefNo { get; set; }
    }
    public class PurchaseFollowUpStatus : TrackUserSettingOperation
    {
        public int PurchaseFollowUpStatusId { get; set; }
        public int PurchaseFollowUpID { get; set; }
        public virtual PurchaseFollowUp PurchaseFollowUp { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
    public class PurchaseFollowUpSystemStatus
    {
        public int PurchaseFollowUpSystemStatusId { get; set; }
        public int PurchaseFollowUpID { get; set; }
        public virtual PurchaseFollowUp PurchaseFollowUp { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
    }
}
