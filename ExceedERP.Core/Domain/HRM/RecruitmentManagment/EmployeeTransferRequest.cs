using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }       
        public string Reason { get; set; }
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [Display(Name = "To Branch")]
        public int BranchId { get; set; }
        [DisplayName("From Business Unit ")]
        public int? FromBusinessUnitId { get; set; }
        [Display(Name = "From Branch")]
        public int FromBranchId { get; set; }
        [DisplayName(" To Business Unit ")]
        public int? BusinessUnitId { get; set; }
        public virtual ICollection<TransferRequestDetail> TransferRequestDetails { get; set; }
        public virtual ICollection<TransferRequestValidation> TransferRequestValidations { get; set; }
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

    public class TransferRequestDepartmentValidation : VoucherValidation
    {
        public int TransferRequestDepartmentValidationId { get; set; }
        public int EmployeeTransferRequestId { get; set; }
        public virtual EmployeeTransferRequest EmployeeTransferRequest { get; set; }

    }
}
