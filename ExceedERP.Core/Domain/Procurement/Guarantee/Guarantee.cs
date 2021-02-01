using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.Guarantee
{



    public class Guarantee : TrackTransaction
    {
        public int GuaranteeId { get; set; }
        [Required]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }
        public string Purpose { get; set; }
        [Display(Name = "Category")]
        public ProcurementCategory ProcurementCategory { get; set; }
        [Display(Name = "Guarantee Type")]
        public GuaranteeTypes GuaranteeType { get; set; }
        [Display(Name = "Guarantee Mode")]
        public string GuaranteeMode { get; set; }
        [Display(Name = "Guarantee Ref No.")]
        public string GuaranteeRefNo { get; set; }
        [Display(Name = "Procurement Ref No.")]
        public string ProcurementRefNo { get; set; }
        [Display(Name = "Contract Ref No.")]
        public string ContractRefNo { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Currency")]
        public string Currency { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "IssueDate")]
        public DateTime IssueDate { get; set; }
        [Display(Name = "Issuing Bank")]
        public string Bank { get; set; }
        [Display(Name = "Bank Branch")]
        public string BankBranch { get; set; }
        [Display(Name = "Validity Period (Days)")]
        public int ValidityPeriod { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expire Date")]
        public DateTime ExpireDate { get; set; }
        [Display(Name = "Validity Status")]
        public GuaranteeValidityStatus ValidityStatus { get; set; }
        public GuaranteeRenewalStatus RenewalStatus { get; set; }

        [NotMapped]
        public bool IsExpired { get; set; }
       
        public virtual ICollection<GuaranteeRenewalRequest> RenewalRequests { get; set; }
        public virtual ICollection<GuaranteeRenewal> GuaranteeRenewals { get; set; }
        public virtual ICollection<GuaranteeValidation> GuaranteeValidation { get; set; }
        public virtual ICollection<GuaranteeFile> GuaranteeFile { get; set; }
    }

    public class GuaranteeRenewalRequest : TrackUserOperation
    {
        public int GuaranteeRenewalRequestId { get; set; }
        public int GuaranteeId { get; set; }
        public virtual Guarantee Guarantee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "request Date")]
        public DateTime Date { get; set; }
        public string Remark { get; set; }
    }
    public class GuaranteeRenewal : TrackUserOperation
    {
        public int GuaranteeRenewalId { get; set; }
        public int GuaranteeId { get; set; }
        public virtual Guarantee Guarantee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Validity period (Days)")]
        public int ValidityPeriod { get; set; }
        [Display(Name = "Guarantee Ref No.")]
        public string GuaranteeRefNo { get; set; }
        public string Remark { get; set; }
    }
    public class GuaranteeValidation : TrackUserOperation
    {
        public int GuaranteeValidationId { get; set; }
        public int GuaranteeId { get; set; }
        public virtual Guarantee Guarantee { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class GuaranteeMode : TrackUserSettingOperation
    {
        public int GuaranteeModeId { get; set; }
        public string  Mode { get; set; }
        public string Remark { get; set; }
    }
    public class GuaranteeFile
    {
        public int GuaranteeFileId { get; set; }
        public int GuaranteeId { get; set; }
        public virtual Guarantee Guarantee { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
    }

}
