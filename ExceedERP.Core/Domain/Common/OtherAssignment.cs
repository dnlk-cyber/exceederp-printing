using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common
{


    public enum AssignmentType
    {
        [Display(Name = "Enum_EmployeeStatus_NOTSELECTED", ResourceType = typeof(Resources))]
        NOTSELECTED = -1,
        [Display(Name = "Enum_AssignmentType_NEW", ResourceType = typeof(Resources))]
        NEW = 0,
        [Display(Name = "Enum_AssignmentType_PROMOTION", ResourceType = typeof(Resources))]
        PROMOTION,
        [Display(Name = "Enum_AssignmentType_YEARLY_PROMOTION", ResourceType = typeof(Resources))]
        YEARLY_PROMOTION,
        [Display(Name = "Enum_AssignmentType_TRANSFER", ResourceType = typeof(Resources))]
        TRANSFER,
        [Display(Name = "Enum_AssignmentType_DEMOTION", ResourceType = typeof(Resources))]
        DEMOTION,
        [Display(Name = "Enum_AssignmentType_RECOMENDATION", ResourceType = typeof(Resources))]
        RECOMENDATION,
        [Display(Name = "Enum_AssignmentType_ASSIGNMENT", ResourceType = typeof(Resources))]
        ASSIGNMENT,
        [Display(Name = "Enum_AssignmentType_STRUCTURE_CHANGE", ResourceType = typeof(Resources))]
        STRUCTURE_CHANGE,
        [Display(Name = "Enum_AssignmentType_ANNUAL_SALARY_INCREMENT", ResourceType = typeof(Resources))]
        ANNUAL_SALARY_INCREMENT,
        [Display(Name = "Enum_AssignmentType_TEMPORARY", ResourceType = typeof(Resources))]
        TEMPORARY,
        [Display(Name = "Enum_AssignmentType_TEMPORARY_REHIRE", ResourceType = typeof(Resources))]
        REHIRE,
        [Display(Name = "Enum_AssignmentType_TEMPORARY_Job_Title_Change", ResourceType = typeof(Resources))]
        JOBTITLECHANGE,
        [Display(Name = "Enum_AssignmentType_ACTING", ResourceType = typeof(Resources))]
        ACTING,



    }
}
