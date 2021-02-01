using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class CourtWorkUnit
    {
        public string Name{get;set;}
        public decimal Total { get; set; }
        public List<CourtEmployees> CourtEmployeesList{get;set;}
    }
}