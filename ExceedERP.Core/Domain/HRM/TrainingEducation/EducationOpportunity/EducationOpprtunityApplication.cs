using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
    public class EducationalOpportunityApplication: TrackUserOperation
    {
        public int EducationalOpportunityApplicationId { get; set; }
        public int EducationalOpportunityId { get; set; }
        public int EducationLevelId { get; set; }
        public int EducationFieldId { get; set; }
        public int EmployeeId { get; set; }
        [Display(Name = "Category Type")]
        public EducationCategoryType CategoryType { get; set; }
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        public string Organization { get; set; }

    }
}
