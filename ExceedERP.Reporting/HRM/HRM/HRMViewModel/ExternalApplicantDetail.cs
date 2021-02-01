using System.Collections.Generic;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class ExternalApplicantDetail
    {

        public int No { get; set; }
        public string JobTitle { get; set; }
        public string EduandExp { get; set; }
        public string Location { get; set; }
        public int Quntity { get; set; }
        public string Salary { get; set; }
        public string Rank { get; set; }
        public string RecruitmentType { get; set; }
        public string ApplicantName { get; set; }
        public string TotalExpriance { get; set; }
        public List<ExternalApplicantEducation> Education { get; set; }
        public string Remark { get; set; }
     
        public List<ExternalApplicantExpriance> OtherExpriance { get; set; }
        public List<EmpApplicantTraining> Trainings { get; set; }

        public int DYear { get; set; }
        public int DMonth { get; set; }
        public int DDays { get; set; }
        public int OtherYear { get; set; }
        public int OtherMonth { get; set; }
        public int OtherDays { get; set; }
        public string TotalYear { get; set; }
        public string TotalMonth { get; set; }
    }
}