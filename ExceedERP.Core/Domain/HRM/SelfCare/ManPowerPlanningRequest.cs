using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class ManPowerPlanningRequest : TrackUserOperation
    {
        [Key]
        public int ManPowerPlanningRequestId { get; set; }
      
        [Display(Name = "HR_SelfCare_RecruitmentRequest_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "HR_SelfCare_RecruitmentRequest_EmploymentType", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus EmploymentType { get; set; }
        
        //[Required]
        [Display(Name = "HR_SelfCare_RecruitmentRequest_LookupId", ResourceType = typeof(Localization.Resources))]
        public int? LookupId { get; set; }
        [Required]
        [Display(Name = "No of Staff")]
        public int NumberOfStaff { get; set; }
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
        [Display(Name = "HR_RecruitmentManagment_Vacancy_Location", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }
        public bool IsNewJobTitle { get; set; }
        [Display(Name ="New Job Title")]
        public string JobTitle { get; set; }
        [Display(Name = "Education")]
        public string Education { get; set; }
        [Display(Name = "Experience")]
        public string Experience { get; set; }
        [Display(Name = "Reason")]
        public string Reason { get; set; }
    }
}
