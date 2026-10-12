using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.Letters.ViewModel
{
    public class RecruitmentLetterVM
    {
        public string VacancyCode { get; set; }
        public string VacancyDate { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string JobPosition { get; set; }
        public string JobPositionIDNO { get; set; }
        public string EducationLevel { get; set; }
        public string EducationField { get; set; }
        public decimal WorkExperiance { get; set; }
        public decimal Qty { get; set; }
        public string JobTitle { get; set; }
        public string BusinessUnit { get; set; }
        public string Grade { get; set; }
        public string Branch { get; set; }
        public string TeminationDateRequest { get; set; }
        public string TeminatedDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string WorkingTime { get; set; }
        public decimal Salary { get; set; }
        public string  SalaryInWord { get; set; }
        public string EmployeementType { get; set; }

    }
}
