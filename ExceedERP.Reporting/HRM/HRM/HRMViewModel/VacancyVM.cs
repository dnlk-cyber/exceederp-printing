using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class VacancyViewModel
    {
        public string VaccancyCode { get; set; }
        public string VacancyMediaCode { get; set; }
        public string JobtitleName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string VacancyType { get; set; }
        public string RecruSource { get; set; }
        public bool IsActive { get; set; }
        public int NoOfVacant { get; set; }
        public int TotalYearOfExperience { get; set; }
        public string ExperienceLevel { get; set; }
        public string JobRequirement { get; set; }
        public string Responsibility { get; set; }
        public string EthioJobsCode { get; set; }
        public decimal? Salary { get; set; }
        public bool Isposted { get; set; }
        public string CGPA { get; set; }

        // List of Vacancy Details
        public List<VacancyDetailViewModel> Details { get; set; }
    }

    public class VacancyDetailViewModel
    {
        public string  JobTitle { get; set; }
        public decimal Experience { get; set; }
        public string Description { get; set; }
    }

}