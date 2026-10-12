using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class BenefitVM
    {
        public string JobTitle { get; set; }
        public decimal NumberOfEmployee { get; set; }
        public string BenefitName { get; set; }
        public string UnitOFMeasuremnet { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Description { get; set; }
        public IList<BenefitDetailVM> BenefitDetails { get; set; }
        public IList<EmployeeBenefitDetailVM> EmployeeBenefitDetails { get; set; }
        
    }
    
    public class BenefitDetailVM
    {
      
        public string BenefitName { get; set; }
        public string UnitOFMeasuremnet { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Description { get; set; }

    }
    public class EmployeeBenefitDetailVM
    {
        public string Employee { get; set; }
        public string Branch { get; set; }
        

    }

}
