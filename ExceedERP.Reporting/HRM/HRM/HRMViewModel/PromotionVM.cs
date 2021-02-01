using ExceedERP.Reporting.SharedDataSources;
using System.Collections.Generic;
using System.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class PromotionVM : AddressVm//newly added
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string CurrentDepartement { get; set; }
        public string CurrentJobTitle { get; set; }
        public string CurrentRank { get; set; }
        public decimal CurrentSalary { get; set; }
        public string CurrentAmount { get; set; }
        public string PreviousDepartement { get; set; }
        public string PreviousJobTitle { get; set; }
        public string PreviousRank { get; set; }
        public decimal PreviousSalary { get; set; }
        public string PreviousAmount { get; set; }
        public string AssignmentDate { get; set; }
        public decimal Diffrence { get; set; }
        public string DiffrenceAmount { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }

        public string Date { get; set; }
       

    }

 
       
    
}