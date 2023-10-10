using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Procurement.Supplier
{
    public enum BlackListType
    {
        Indefinite,
        Bounded
    }
    public class ProcurementSupplier : TrackUserOperation
    {
        public int ProcurementSupplierID { get; set; }
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Required]
        [Display(Name = "Supplier Name")]
        public string Name { get; set; }
         [Required]
        [Display(Name = "Supplier Id")]
        public string SupplierId { get; set; }
        [Display(Name = "Trade License")]
        public string TradeLicense { get; set; }
        [Required]
        [Display(Name = "TIN")]
        public string TIN { get; set; }
        public string Address { get; set; }
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }
        [Display(Name = "Fax")]
        public string Fax { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Is Credit ?")]
        public bool IsCredit { get; set; }
        [DisplayName("Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Is Active ?")]
        public bool IsActive { get; set; }
        [Display(Name = "Is Black Listed ?")]
        public bool IsBlackListed { get; set; }
        [Display(Name = "Is Short List ?")]
        public bool IsForShortList { get; set; }
        [DisplayName("Minute No.")]
        public string MinuteNo { get; set; }
        public string SupplierReference { get; set; }
        public string Remark { get; set; }
        public string Rank { get; set; }
        [DisplayName("T Code")]
        public string TCode { get; set; }// telebirr
        [DisplayName("T Account")]
        public string TAccount { get; set; }// telebirr
        public virtual ICollection<SupplierBlackList> BlackLists { get; set; }
        public virtual ICollection<SupplierRemoveBlackList> SupplierRemoveBlackLists { get; set; }
        public virtual ICollection<SupplierPerformance> Performances { get; set; }
       
    }
    public class SupplierBlackList : TrackTransaction
    {
        public int SupplierBlackListId { get; set; }
        public int ProcurementSupplierID { get; set; }
        public virtual ProcurementSupplier Supplier { get; set; }
        [Required]
        [Display(Name = "Reason for Blacklist")]
        public string Reason { get; set; }
        [Display(Name = "Black List Type")]
        public BlackListType BlackListType { get; set; }
        [Display(Name = "Black List Duration (Days)")]
        public int DurationDays { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Black Listed On")]
        public DateTime DateBlackListed { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Black List Expire Date")]
        public DateTime DateBlackListedExpire { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<SupplierBlackListValidation> SupplierBlackListValidations { get; set; }
    }
    public class SupplierBlackFile
    {
        public int SupplierBlackFileId { get; set; }
        public int SupplierBlackListId { get; set; }
        public virtual SupplierBlackList SupplierBlackList { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
    }
    public class SupplierBlackListValidation : TrackUserOperation
    {
        public int SupplierBlackListValidationId { get; set; }
        public int SupplierBlackListId { get; set; }
        public virtual SupplierBlackList SupplierBlackList { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class SupplierRemoveBlackList : TrackTransaction
    {
        public int SupplierRemoveBlackListId { get; set; }
        public int ProcurementSupplierID { get; set; }
        public virtual ProcurementSupplier Supplier { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Cleared On")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ClearedOn { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<SupplierRemoveBlackListValidation> SupplierRemoveBlackListValidations { get; set; }
    }
    public class SupplierRemoveBlackListValidation : TrackUserOperation
    {
        public int SupplierRemoveBlackListValidationId { get; set; }
        public int SupplierRemoveBlackListId { get; set; }
        public virtual SupplierRemoveBlackList SupplierRemoveBlackList { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class SupplierPerformance : TrackTransaction
    {
        public int SupplierPerformanceId { get; set; }
        public int ProcurementSupplierID { get; set; }
        public virtual ProcurementSupplier Supplier { get; set; }
        [Required]
        [Display(Name = "Procurement Ref.")]
        public string ProfurementRef { get; set; }
        [Required]
        [Display(Name = "Quality")]
        public string Quality { get; set; }
        [Required]
        [Display(Name = "Delivery Time(Days)")]
        public int PlannedDeliveryPeriod { get; set; }
        [Required]
        [Display(Name = "Actual Delivery Time (Days)")]
        public int ActualDeliveryPeriod { get; set; }
        [Required]
        [Display(Name = "Difference (Days)")]
        public int Difference { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<SupplierPerformanceValidation> SupplierPerformanceValidations { get; set; }
    }
    public class SupplierPerformanceValidation : TrackUserOperation
    {
        public int SupplierPerformanceValidationId { get; set; }
        public int SupplierPerformanceId { get; set; }
        public virtual SupplierPerformance SupplierPerformance { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class SupplierListMinuteFile
    {
        public int SupplierListMinuteFileId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
    }
}
