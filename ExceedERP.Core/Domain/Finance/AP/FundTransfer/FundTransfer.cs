using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.FundTransfer
{

    public class FundTransfer: DepartmentNewOperations
    {
        public int FundTransferId { get; set; }
        [Required]
        public string ReasonOfTransfer { get; set; }
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }      
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }

        [Display(Name = "From Branch/Project")]
        public int FromBranchId { get; set; }
        [Display(Name = "From Business Unit")]
        public int FromBusinessUnitId { get; set; }
        //requestor
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [Display(Name = "Department")]
        public int OrgStuructureId { get; set; }      
        [Display(Name = "To Bank Account")]
        public int BankAccountId { get; set; }
        public bool Paid { get; set; }
        //budget controll
        public bool IsBudgetApproved { get; set; }
        public string BudgetApprovedBy { get; set; }
        public DateTime? BudgetApprovedTime { get; set; }
        //for access or view
        public int CreatedAtBranchId { get; set; }
        public int CreatedAtBusinessUnitId { get; set; }
        public int CreatedAtOrgStructureId { get; set; }
        public virtual ICollection<FundTransferDetail> FundTransferDetails { get; set; }
        public virtual ICollection<FundTransferAttachment> FundTransferAttachments { get; set; }
        public virtual ICollection<FundTransferDepartmentValidation> FundTransferDepartmentValidations { get; set; }
        public virtual ICollection<FundTransferBudgetValidation> FundTransferBudgetValidations { get; set; }
        public virtual ICollection<FundTransferValidation> FundTransferValidations { get; set; }
       
    }

    public class FundTransferAttachment
    {
        public int FundTransferAttachmentId { get; set; }
        public int FundTransferId { get; set; }
        public FundTransfer FundTransfer { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
