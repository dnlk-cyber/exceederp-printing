using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Insurance.InsuranceNew
{
    public class EmployeeAccidentCreditService : DepartmentNewOperations
    {
        public int EmployeeAccidentCreditServiceId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "Common_PersonEmployee_Status", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus EmployementType { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_ResponsibilityLevel", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel ResponsibilityLevel { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int BusinessUnitId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Date", ResourceType = typeof(Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string DateAm { get; set; }
        [Display(Name ="Description", ResourceType = typeof(Resources))]
        public string Description { get; set; }
        [Display(Name = "Medical Center")]
        public string MedicalCenter   { get; set; }
        public decimal Credit   { get; set; }
        public bool IsVerified { get; set; }
        public bool IsTheDocumentComplete { get; set; }
        public bool IsTheDocumentCompleteByEIC { get; set; }
        public bool IsPaid { get; set; }


    }
    public class AccidentCreditServiceAttachment
    {
        public int AccidentCreditServiceAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int EmployeeAccidentCreditServiceId { get; set; }
        public virtual EmployeeAccidentCreditService CreditService { get; set; }

    }


    public class AccidentCreditServiceDepartmentApproval : TrackUserOperation
    {
        public int AccidentCreditServiceDepartmentApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public int EmployeeAccidentCreditServiceId { get; set; }
        public virtual EmployeeAccidentCreditService CreditService { get; set; }

    }
    public class AccidentCreditServiceApproval : TrackUserOperation
    {
        public int AccidentCreditServiceApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public int EmployeeAccidentCreditServiceId { get; set; }
        public virtual EmployeeAccidentCreditService CreditService { get; set; }

    }


}
