using System.Collections.Generic;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Reporting.SharedDataSources;
using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class InternalApplicantDetail : AddressVm
    {

        public int No { get; set; }
        public string JobTitle { get; set; }
        public string EduandExp { get; set; }
        public string ExperienceLevel { get; set; }
        public string Location { get; set; }
        public int Quntity { get; set; }
        public string Salary { get; set; }
        public string Rank { get; set; }
        public string RecruitmentType { get; set; }
        public string ApplicantName { get; set; }
        public string Perfromance { get; set; }
        public string TotalExpriance { get; set; }
        public  string VacancyCode { get; set; }
        public string Remark { get; set; }
        public string TotalYear { get; set; }
        public string TotalMonth { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
         public List<EmployeeEducation> Educations { get; set; }
        public List<CurrentEmployeeExpriance> Expriance { get; set; }
        public List<OtherEmployeeExpriance> OtherExpriance { get; set; }
        public List<EmpApplicantTraining> Trainings { get; set; }
        public int DYear { get; set; }
        public int DMonth { get; set; }
        public int DDays { get; set; }
        public int OtherYear { get; set; }
        public int OtherMonth { get; set; }
        public int OtherDays { get; set; }
        public string Address { get; set; }
    }
}