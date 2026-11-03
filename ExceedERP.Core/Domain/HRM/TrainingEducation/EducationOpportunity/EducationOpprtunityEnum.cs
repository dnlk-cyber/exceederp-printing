using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
    public enum EducationCriteria
    {
        Performance,
        [Display(Name = "Total Experience")]
        TotalExperience,
        [Display(Name = "Educational preparation")]
        Educational_Preparation,
        Discipline,
        Others
    }

    public enum EducationPlanType
    {
        [Display(Name = "All Employees")]
        Employee,
        Women,
        Management
    }
    public enum EducationCategoryType
    {
        [Display(Name = "Natural Science")]
        Natural_Science,

        [Display(Name = "Social Science")]
        Social_Science,

        [Display(Name = "Natural & Social Science")]
        Natural_And_Social_Science
    }

}
