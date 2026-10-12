using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeTerminationSummaryByBussinessUnitVM
    {
        public string BussinesUnit { get; set; }
        public string Male { get; set; }
        public string Female { get; set; }
        public decimal PermanentMale { get; set; }
        public decimal PermanentFemale { get; set; }
        public decimal TotalPermanentEmployee { get; set; }
        public decimal ContractMale { get; set; }
        public decimal ContractFemale { get; set; }
        public decimal TotalContaractEmployee { get; set; }
        public decimal OtherMale { get; set; }
        public decimal OtherFemale { get; set; }
        public decimal TotalOtherEmployee { get; set; }
        public decimal TotalMale { get; set; }
        public decimal TotalFemale { get; set; }
        public decimal TotalEmployee { get; set; }

    }

}
