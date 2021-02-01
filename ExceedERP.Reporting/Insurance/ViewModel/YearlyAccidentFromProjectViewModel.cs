using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.ViewModel
{
   public class YearlyAccidentFromProjectViewModel
    {
        public string ProjectName { get; set; }
        public List<YearlyAccidentFromProjectListViewModel> YearlyAccidentFromProjectListViewModel { get; set; }
    }

    public class YearlyAccidentFromProjectListViewModel
    {
        public string AccidentCarPlateNo { get; set; }
        public int NoOfAccident { get; set; }
    }

}
