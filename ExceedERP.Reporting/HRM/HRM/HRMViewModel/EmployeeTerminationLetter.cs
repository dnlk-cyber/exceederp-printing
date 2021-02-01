using System.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeTerminationLetter
    {
        public string EmployeeId
        {
            get; set;

        }
        public string FullName { get; set; }


        public string ApplicationDate { get; set; }
        public string BirthDate { get; set; }
        public string HireDate { get; set; }
        public string EduLevel { get; set; }
        public string JobTitle { get; set; }
        public decimal Salary { get; set; }
        public decimal Loan { get; set; }
        public string PensionId { get; set; }
        public string TerminationDate { get; set; }
        public string CurrentExprinace { get; set; }
        public string OtherExprinace { get; set; }
        public string TotalExprinace { get; set; }
        public string TerminationReason { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }

        public string Year{get;set;}
    }
}