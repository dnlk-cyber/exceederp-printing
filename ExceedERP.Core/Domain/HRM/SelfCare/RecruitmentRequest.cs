using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class RecruitmentRequest: DepartmentOperations
    {
        [Key]
        public int RecruitmentRequestId { get; set; }
        [Required ]
        [Display(Name = "HR_SelfCare_RecruitmentRequest_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        [Required ]
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
        [Display(Name = "Common_Identification_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark;
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Display(Name = "HR_RecruitmentManagment_Vacancy_Location", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }

    }
}