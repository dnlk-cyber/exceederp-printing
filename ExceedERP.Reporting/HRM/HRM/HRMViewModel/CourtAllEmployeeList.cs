using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class CourtAllEmployeeList
    {
        public string Branch { get; set; }
        public decimal BranchTotal { get; set; }
        public List<CourtWorkUnit> CourtWorkUnitList { get; set; }
    }
}