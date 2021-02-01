using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    //class WorkProportionReportViewModels
    //{
    //}
    public class WorkProportionProjectViewModel
    {
        public WorkProportionProjectViewModel()
        {
            WorkCategoryList = new List<WorkProportionWorkCategoryViewModel>();
        }
        public string Name { get; set; }

        public List<WorkProportionWorkCategoryViewModel> WorkCategoryList { get; set; }
    }
    public class WorkProportionWorkCategoryViewModel
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CostPercentage { get; set; }
    }
}
