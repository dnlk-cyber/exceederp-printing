using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    //class BoQSummaryViewModels
    //{

    //}
    public class BoQSummaryProjectViewModel
    {
        public BoQSummaryProjectViewModel()
        {
            SubProjects = new List<BoQSummarySubProjectViewModel>();
        }
        public string Name { get; set; }
        public List<BoQSummarySubProjectViewModel> SubProjects { get; set; }

    }
    public class BoQSummarySubProjectViewModel
    {
        public BoQSummarySubProjectViewModel()
        {
            StructureTypes = new List<BoQSummaryStructureTypeViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BoQSummaryStructureTypeViewModel> StructureTypes { get; set; }
    }
    public class BoQSummaryStructureTypeViewModel
    {
        public BoQSummaryStructureTypeViewModel()
        {
            CategoryList = new List<BoQSummaryWorkCategoryViewModel>();
        }
        public string Name { get; set; }
        public int? Code { get; set; }

        public List<BoQSummaryWorkCategoryViewModel> CategoryList { get; set; }
    }
    public class BoQSummaryWorkCategoryViewModel
    {
        //public BoQSummaryWorkCategoryViewModel()
        //{
        //    WorkItemList = new List<BoQWorkItemViewModel>();
        //}
        public string Name { get; set; }
        public int Code { get; set; }
        public decimal TotalPrice { get; set; }
        //public List<BoQWorkItemViewModel> WorkItemList { get; set; }

    }
    //public class BoQWorkItemViewModel
    //{
    //    public string Description { get; set; }
    //    public string Unit { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal UnitRate { get; set; }
    //    public decimal TotalAmount { get; set; }
    //}


}
