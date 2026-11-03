using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
    public class EducationOpportunityApplicationScreeningWomen: TrackUserOperation
    {
        public int EducationOpportunityApplicationScreeningWomenId { get; set; }

        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string EmployeeCode { get; set; }
        public int OrgStructureId { get; set; }
        public int LookUpId { get; set; }
        public string AppliedEducationOrganization { get; set; }
        public int AppliedEducationFieldId { get; set; }
        public int? AppliedEducationLevelId { get; set; }
        public decimal InternalExperience { get; set; }
        public int? EducationFieldId { get; set; }
        public int? EducationLevelId { get; set; }
        public decimal TotalExperience { get; set; }
        public decimal EducationField { get; set; }
        public decimal Performance { get; set; }
        public decimal PerformanceResult { get; set; }
        public decimal Discipline { get; set; }
        //evaluation Criteria
        public decimal Result { get; set; }
        public string Grade { get; set; }
        public EmployeeStatus EmploymentType { get; set; }
        public int Age { get; set; }
        public string PhoneNo { get; set; }
        public int EducationOpprtunityId { get; set; }

    }
}
