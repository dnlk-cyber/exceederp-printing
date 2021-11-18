using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
   public class JobTimeSheetVM
    {
        public string JobNo { get; set; }
        public string TotalSum { get; set; }
        public List<JobOrderTimeListVM> JobOrderTimeList { get; set; }
    }
}
