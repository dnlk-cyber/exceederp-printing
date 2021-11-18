using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
   public class JobOrderTimeListVM
    {
        public DateTime Date { get; set; }
        public string Work { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ComplationTime { get; set; }
        public TimeSpan TimeTook { get; set; }
        public string TimeTookString { get; set; }
        public int NoOfEmployee { get; set; }
        public string Remark { get; set; }

    }
}
