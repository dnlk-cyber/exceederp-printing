using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeExperience//newly added
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Departement { get; set; }
        public bool Assigned { get; set; }
        public double TotalExpreince { get; set; }
        public string TotalExp { get; set; }
        public ICollection<EmployeeExperienceList> EmployeeExperienceList { get; set; }
        public ICollection<EmployeeEducationList> EmployeeEducationList { get; set; }
        public EmployeeExperience()
        {
            EmployeeExperienceList = new List<EmployeeExperienceList>();
            EmployeeEducationList = new List<EmployeeEducationList>();
        }

    }
}