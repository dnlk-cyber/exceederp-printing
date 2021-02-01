using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeEductaion
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Departement { get; set; }
        public ICollection<EmployeeEductaionList> EmployeeEductaionList { get; set; }
        public bool Assigned { get; set; }
        public EmployeeEductaion()
        {
            EmployeeEductaionList=new List<EmployeeEductaionList>();
        }

    }
}