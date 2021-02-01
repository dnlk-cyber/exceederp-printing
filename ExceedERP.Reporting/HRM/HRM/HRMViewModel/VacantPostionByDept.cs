using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class VacantPostionByDept
    {
        public string Department { get; set; }
        public List<VacantDetail> vacantDetail { get; set; }
      
    }
}