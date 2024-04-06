using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeePlacementVM
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Branch { get; set; }
        public string BussinessUnit { get; set; }
        public string ParentStructureName { get; set; }
        public string Departement { get; set; }
        public string Boss { get; set; }
        public string JobTitle { get; set; }
        public string HireDate { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string Grade { get; set; }
        public string Step { get; set; }
        public decimal Salary { get; set; }
        public string AssignmentType { get; set; }
        public int YearOfPromotion { get; set; }



    }
}