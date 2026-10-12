using System.Collections.Generic;
using System.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class TransferVM//newly added
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string AssignmentDate { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Date { get; set; }
        public    Current Currents { get; set; }
        public Previous Previouses { get; set; }

    }

    public class Current
    {
        public string CurrentDepartement { get; set; }
        public string CurrentJobTitle { get; set; }
        public string CurrentRank { get; set; }
        public decimal CurrentSalary { get; set; }
        public string CurrentType { get; set; }
        public string Departemnt { get; set; }
        //public string Remark { get; set; }
        public string Branch { get; set; }
        public string HaveFixedAsset { get; set; }
    }

    public class Previous
    {
        public string PreviousJobTitle { get; set; }
        public string PreviousRank { get; set; }
        public decimal PreviousSalary { get; set; }
        public string PreviousType { get; set; }
        
        public string Departemnt { get; set; }
        public string Branch { get; set; }
        public string HaveFixedAsset { get; set; }
    }
}