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
    public class EmployeeAccidentInsuranceClaim: DepartmentNewOperations
    {
        public int EmployeeAccidentInsuranceClaimId { get; set; }
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
        [Display(Name = " Date of the accident", ResourceType = typeof(Resources))]
        public DateTime Date { get; set; }
        [Display(Name = " Date of the accident", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string DateAm { get; set; }
        [Display(Name = "Cause of the Accident")]
        public string CauseOfTheAccident { get; set; }
        [Display(Name = "Type of damage caused by the accident")]
        public string TypeOfDamageCaussedByAccident { get; set; }
        [Display(Name = "Location of the accident")]
        public string LocationOfAccident { get; set; }
        [Display(Name = "Opinion of the immediate superior")]
        public string OpinionOfImmediateSuperior { get; set; }
        [Display(Name = "Salary(daily/monthly)Before the accident:")]
        public decimal SalaryDailyOrmonthlyBeforeTtheAccident { get; set; }
        [Display(Name ="Description", ResourceType = typeof(Resources))]
        public string Description { get; set; }
        public bool IsVerifyied { get; set; }
        public bool IsTheDocumentComplete { get; set; }
        public bool IsTheDocumentCompleteByEIC { get; set; }
        public bool IsPaid { get; set; }
        public InsuranceClaimType InsuranceType { get; set; } 
        public ICollection<AccidentInsuranceClaimAttachment> Attachments { get; set; }
        public ICollection<AccidentInsuranceClaimDepartmentApproval> DepartmentApprovals { get; set; }
        public ICollection<AccidentInsuranceClaimApproval> AccidentInsuranceClaims { get; set; }


    }
     public class PeoplePresentAtTimeAccident
    {
        public string PeoplePresentAtTimeAccidentId { get; set; }
        [Display(Name ="")]
        public string IdentityNumber { get; set; }
        public string FullName { get; set; }
        public string WorkPlace { get; set; }
    }
    public class AccidentInsuranceClaimAttachment
    {
        public int AccidentInsuranceClaimAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int EmployeeAccidentInsuranceClaimId { get; set; }
        public virtual EmployeeAccidentInsuranceClaim AccidentInsuranceClaim { get; set; }

    }


    public class AccidentInsuranceClaimDepartmentApproval : TrackUserOperation
    {
        public int AccidentInsuranceClaimDepartmentApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public int EmployeeAccidentInsuranceClaimId { get; set; }
        public virtual EmployeeAccidentInsuranceClaim AccidentInsuranceClaim { get; set; }

    }
    public class AccidentInsuranceClaimApproval : TrackUserOperation
    {
        public int AccidentInsuranceClaimApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
        public int EmployeeAccidentInsuranceClaimId { get; set; }
        public virtual EmployeeAccidentInsuranceClaim AccidentInsuranceClaim { get; set; }

    }

    public enum InsuranceClaimType
    {
        Accident,
        Death,
        Other

    }

}
