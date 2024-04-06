using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EmployeePerformanceEvaluationSetting: TrackUserSettingOperation
    {
        public int EmployeePerformanceEvaluationSettingId { get; set; }
        public EmployeePerformanceResponsibilityLevel ResponsibilityLevel { get; set; }
        public PerformanceEvaluationType EvaluationType { get; set; }
        public int? LookupId { get; set; }
        public decimal Weight { get; set; }
    }

    public enum PerformanceEvaluationType
    {
        NA,
        [Display(Name = "Enum_Evaluation_Type_Company_level", ResourceType = typeof(Resources))]
        Companylevel=5,
        [Display(Name = "Enum_Evaluation_Type_Corporate_Result", ResourceType = typeof(Resources))]
        Corporate_Result,
        [Display(Name = "Enum_Evaluation_Type_Sector_Result", ResourceType = typeof(Resources))]
        Sector_Result,
        [Display(Name = "Enum_Evaluation_Type_Others", ResourceType = typeof(Resources))]
        Others,
        [Display(Name = "Main Department Level")]
        MainDepartmentLevel

    }
    public enum EmployeePerformanceResponsibilityLevel
    {
        [Display(Name = "Enum_ResponsibilityLevel_TOP_MANAGMENT", ResourceType = typeof(Resources))]
        TOP_MANAGMENT,
        [Display(Name = "Enum_ResponsibilityLevel_MEDIUM_MANAGMENT", ResourceType = typeof(Resources))]
        MEDIUM_MANAGMENT,
        [Display(Name = "Enum_ResponsibilityLevel_LOW_MANAGMENT", ResourceType = typeof(Resources))]
        LOW_MANAGMENT,
        [Display(Name = "Enum_ResponsibilityLevel_STAFF", ResourceType = typeof(Resources))]
        STAFF,
        [Display(Name = "JOB TITLE")]
        JOBTITLE
    }

}
