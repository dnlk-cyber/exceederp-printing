using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    //class BoQSummaryViewModels_Min
    //{
    //}
    public class MinBoQSummarySubProjectViewModel
    {
        public MinBoQSummarySubProjectViewModel()
        {
            WorkCategoryList = new List<MinBoQSummaryWorkCategoryViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string TotalPrice { get; set; }
        public List<MinBoQSummaryWorkCategoryViewModel> WorkCategoryList { get; set; }
    }
    
    public class MinBoQSummaryWorkCategoryViewModel
    {
        public string StructureType { get; set; }
        public string Name { get; set; }
        public decimal TotalPrice { get; set; }
        
    }
}
