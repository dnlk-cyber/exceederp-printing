using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EvaluationPeiord
    {
        public string Period { get; set; }
        public List< Behaviours> Behaviours { get; set; }
    }
    public class Behaviours
    {
      
        public string Behaviour { get; set; }
        public List<Details> Detailses { get; set; }
      
    }

    public class Details
    {
        public string Measurment { get; set; }
        public decimal Performance { get; set; }
    }
}