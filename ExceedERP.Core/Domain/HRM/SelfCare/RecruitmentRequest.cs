using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class RecruitmentRequest : OperationWithThirdValidation
    {
        [Key]
        public int RecruitmentRequestId { get; set; }

        [Required]
        [Display(Name = "HR_SelfCare_RecruitmentRequest_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }

        [Required]
        [Display(Name = "HR_SelfCare_RecruitmentRequest_EmploymentType", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus EmploymentType { get; set; }

        [Required]
        [Display(Name = "HR_SelfCare_RecruitmentRequest_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }

        [Required]
        [Display(Name = "HR_SelfCare_RecruitmentRequest_NoOfVaccantPostion", ResourceType = typeof(Localization.Resources))]
        public int NoOfVaccantPostion { get; set; }

        [Display(Name = "HR_SelfCare_RecruitmentRequest_NoOfCurrentStaff", ResourceType = typeof(Localization.Resources))]
        public int NoOfCurrentStaff { get; set; }

        [Display(Name = "HR_SelfCare_RecruitmentRequest_NoOfShouldBeStaff", ResourceType = typeof(Localization.Resources))]
        public int NoOfShouldBeStaff { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_RecruitmentRequest_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }

        [Display(Name = "HR_SelfCare_RecruitmentRequest_PlanType", ResourceType = typeof(Localization.Resources))]
        public JobTitlePlan PlanType { get; set; }

        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }

        [Display(Name = "HR_RecruitmentManagment_Vacancy_Location", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }

        [Display(Name = "Next Approver")]
        public int? NextApproverEmpId { get; set; }
        public string NextApprover { get; set; }

        // Tier 1: Project HR Admin
        [Display(Name = "Is Project HR Approved")]
        public bool IsProjectHRApproved { get; set; }
        public string ProjectHRApprovedBy { get; set; }
        public DateTime? ProjectHRApprovedDate { get; set; }

        // Tier 2: Project Manager (PM)
        [Display(Name = "Is PM Approved")]
        public bool IsPMApproved { get; set; }
        public string PMApprovedBy { get; set; }
        public DateTime? PMApprovedDate { get; set; }

        // Tier 3: Head Office Department Manager (Inherited: IsDepartmentOnlineApproved, DepartmentOnlineApprovedBy, DepartmentOnlineApprovedTime)

        // Tier 4: Head Office Human Resource Admin (Inherited: IsOnlineApproved, OnlineApprovedBy, OnlineApprovedTime)

        // Tier 5: Deputy General Manager (DGM)
        [Display(Name = "Is DGM Approved")]
        public bool IsDGMApproved { get; set; }
        public string DGMApprovedBy { get; set; }
        public DateTime? DGMApprovedDate { get; set; }

        // Tier 6: General Manager (GM - Authorized)
        [Display(Name = "Is GM Authorized")]
        public bool IsGMApproved { get; set; }
        public string GMApprovedBy { get; set; }
        public DateTime? GMApprovedDate { get; set; }

        // Validation Collections
        public virtual ICollection<RecruitmentRequestProjectHRValidation> RecruitmentRequestProjectHRValidations { get; set; }
        public virtual ICollection<RecruitmentRequestPMValidation> RecruitmentRequestPMValidations { get; set; }
        public virtual ICollection<RecruitmentRequestDepartmentValidation> RecruitmentRequestDepartmentValidations { get; set; }
        public virtual ICollection<RecruitmentRequestHRValidation> RecruitmentRequestHRValidations { get; set; }
        public virtual ICollection<RecruitmentRequestDGMValidation> RecruitmentRequestDGMValidations { get; set; }
        public virtual ICollection<RecruitmentRequestGMValidation> RecruitmentRequestGMValidations { get; set; }

        // Legacy Validation Collection
        public virtual ICollection<RecruitmentRequestSecondValidation> RecruitmentRequestSecondValidations { get; set; }
    }

    public class RecruitmentRequestProjectHRValidation : VoucherValidation
    {
        [Key]
        public int RecruitmentRequestProjectHRValidationId { get; set; }
        public int RecruitmentRequestId { get; set; }
        public virtual RecruitmentRequest RecruitmentRequest { get; set; }
    }


    public class RecruitmentRequestPMValidation : VoucherValidation
    {

        [Key]
        public int RecruitmentRequestPMValidationId { get; set; }
        public int RecruitmentRequestId { get; set; }
        public virtual RecruitmentRequest RecruitmentRequest { get; set; }

    }

    public class RecruitmentRequestDepartmentValidation : VoucherValidation
    {
        [Key]
        public int RecruitmentRequestDepartmentValidationId { get; set; }
        public int RecruitmentRequestId { get; set; }
        public virtual RecruitmentRequest RecruitmentRequest { get; set; }
    }

    public class RecruitmentRequestHRValidation : VoucherValidation
    {
        [Key]
        public int RecruitmentRequestHRValidationId { get; set; }
        public int RecruitmentRequestId { get; set; }
        public virtual RecruitmentRequest RecruitmentRequest { get; set; }
    }

    public class RecruitmentRequestDGMValidation : VoucherValidation
    {
        [Key]
        public int RecruitmentRequestDGMValidationId { get; set; }
        public int RecruitmentRequestId { get; set; }
        public virtual RecruitmentRequest RecruitmentRequest { get; set; }
    }

    public class RecruitmentRequestGMValidation : VoucherValidation
    {
        [Key]
        public int RecruitmentRequestGMValidationId { get; set; }
        public int RecruitmentRequestId { get; set; }
        public virtual RecruitmentRequest RecruitmentRequest { get; set; }
    }

    public class RecruitmentRequestSecondValidation : VoucherValidation
    {
        [Key]
        public int RecruitmentRequestSecondValidationId { get; set; }
        public int RecruitmentRequestId { get; set; }
        public virtual RecruitmentRequest RecruitmentRequest { get; set; }
    }
}