using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{

    public class EmployeeTransferRequest : DepartmentNewOperations
    {
        public int EmployeeTransferRequestId { get; set; }
        [Required]
        [Display(Name = "Employee Name")]
        public int EmployeeId { get; set; }
        [Required]
        [DisplayName("To Work Unit")]
        public int OrgStructureId { get; set; }
        [DisplayName("From Work Unit")]
        public int? FromOrgStructureId { get; set; }
        public string Reason { get; set; }
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [Display(Name = "To  Branch ")]
        public int BranchId { get; set; }
        [Display(Name = "To Branch")]
        public int? ToBranch1Id { get; set; }
        [Display(Name = "To Branch ")]
        public int? ToBranchId { get; set; }
        [DisplayName("From Business Unit ")]
        public int? FromBusinessUnitId { get; set; }
        [Display(Name = "From Branch")]
        public int FromBranchId { get; set; }
        [DisplayName(" To Business Unit ")]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "From JobTitle")]
        public int? FromLookupId { get; set; }
        [Display(Name = "To JobTitle")]
        public int? LookupId { get; set; }
        [DisplayName("Total Experience")]
        public decimal? TotalExperience { get; set; }
        [Display(Name = "Desert Allowance")]
        public decimal? DesertAllowance { get; set; }
        [Display(Name = "Desert Work Experience (Years)")]
        public decimal? DesertWorkExperienceYears { get; set; }
        [NotMapped]
        [DisplayName("Total Experience")]
        public string Experience { get; set; }
        public int? Count { get; set; }

        public bool IsDGMApproved { get; set; }
        public string DGMApprovedBy { get; set; }
        public DateTime? DGMApprovedTime { get; set; }

        public bool IsForOther { get; set; }
        public bool IsGMApproved { get; set; }
        public string GMApprovedBy { get; set; }
        public DateTime? GMApprovedTime { get; set; }

        public virtual ICollection<TransferRequestDetail> TransferRequestDetails { get; set; }
        public virtual ICollection<TransferRequestValidation> TransferRequestValidations { get; set; }
        public virtual ICollection<TransferRequestDGMValidation> TransferRequestDGMValidations { get; set; }
        public virtual ICollection<TransferRequestGMValidation> TransferRequestGMValidations { get; set; }
        public virtual ICollection<TransferRequestDepartmentValidation> TransferRequestDepartmentValidations { get; set; }
    }


    public class TransferRequestDetail : TrackUserSettingOperation
    {
        public int TransferRequestDetailId { get; set; }
        public int Quantity { get; set; }
        [DisplayName("Job Title")]
        public int JobTitleId { get; set; }
        [DisplayName("Employee")]
        public int? EmployeeId { get; set; }
        public string Description { get; set; }
        public int EmployeeTransferRequestId { get; set; }
        public virtual EmployeeTransferRequest EmployeeTransferRequest { get; set; }
    }

    public class TransferRequestValidation : VoucherValidation
    {
        public int TransferRequestValidationId { get; set; }
        public int EmployeeTransferRequestId { get; set; }
        public virtual EmployeeTransferRequest EmployeeTransferRequest { get; set; }

    }
    public class TransferRequestDGMValidation : VoucherValidation
    {
        public int TransferRequestDGMValidationId { get; set; }
        public int EmployeeTransferRequestId { get; set; }
        public virtual EmployeeTransferRequest EmployeeTransferRequest { get; set; }

    }
    public class TransferRequestGMValidation : VoucherValidation
    {
        public int TransferRequestGMValidationId { get; set; }
        public int EmployeeTransferRequestId { get; set; }
        public virtual EmployeeTransferRequest EmployeeTransferRequest { get; set; }

    }

    public class TransferRequestDepartmentValidation : VoucherValidation
    {
        public int TransferRequestDepartmentValidationId { get; set; }
        public int EmployeeTransferRequestId { get; set; }
        public virtual EmployeeTransferRequest EmployeeTransferRequest { get; set; }

    }
    public class EmployeeTransferAttachment
    {
        public int EmployeeTransferAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int EmployeeTransferRequestId { get; set; }
        public virtual EmployeeTransferRequest EmployeeTransferRequest { get; set; }
    }

}
