using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class InternalVacancy
    {
        public int No { get; set; }
        public string JobTitle { get; set; }
        public string EduandExp { get; set; }
        public string EduandExpRelated { get; set; }
        public string Location { get; set; }
        public int Quntity { get; set; }
        public string Salary { get; set; }
        public string VacancyCode { get; set; }
        public string RecruitmentType { get; set; }
        public string DeadLine { get; set; }
    }
}