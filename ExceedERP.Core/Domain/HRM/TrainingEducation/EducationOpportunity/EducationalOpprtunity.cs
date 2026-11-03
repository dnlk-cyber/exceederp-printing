using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
    public class EducationalOpportunity: TrackUserOperation
    {
        public int EducationalOpportunityId { get; set; }
        [Display(Name = "Education Field")]
        public int? EducationFieldId { get; set; }
        [Display(Name = "Education Level")]
        public int EducationLevelId { get; set; }

        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }

        public int Quantity { get; set; }
        [DataType(DataType.Date)] // Correct usage of DataType attribute
        public DateTime ApplicationStartDate { get; set; }
        [DataType(DataType.Date)] // Correct usage of DataType attribute
        [Display(Name = "Application End-Date")]
        public DateTime ApplicationEndDate { get; set; }
        public EducationPlanType Type { get; set; }
        public EducationCategoryType CategoryType { get; set; }
        public string Organization { get; set; }

    }
}
