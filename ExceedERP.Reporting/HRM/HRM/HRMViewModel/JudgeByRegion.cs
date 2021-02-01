using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class JudgeByRegion
    {
        public ExceedERP.Core.Domain.Common.Region RegionName { get; set; }
        public string Nation { get; set; }
        public string Judicial { get; set; }
        public decimal Total { get; set; }
        public decimal GTotal { get; set; }
        public List<Nation> Nations { get; set; }
    }
}