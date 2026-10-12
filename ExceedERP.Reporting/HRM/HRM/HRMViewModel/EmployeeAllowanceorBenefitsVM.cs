using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class EmployeeAllowanceorBenefitsVM
   {

        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public string Branch { get; set; }
        public string Department { get; set; }

        public string JobTitle { get; set; }
        public string AssignmentDate { get; set; }
       
        public List<AllowanceorBenefitsVM> AllowanceorBenefits { get; set; }
    }
    public class AllowanceorBenefitsVM
    {
        public string AllowanceOrAllowance { get; set; }
        public decimal Balance { get; set; }
    }

}
