using System.Collections.Generic;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeCourtInsurance
    {
      

        public string FullName { get; set; }
        public Gender Sex { get; set; }
        public string  Dob { get; set; }
        public string WifHusName { get; set; }
        public string WifeHusBob { get; set; }
        public Gender WifeHusSex { get; set; }
        public List<Children> Children { get; set; }

    }
}