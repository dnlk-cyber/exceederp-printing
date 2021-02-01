using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    public class BoQProjectViewModel
    {
        public BoQProjectViewModel()
        {
            SubProjects = new List<BoQSubProjectViewModel>();
        }
        public string Name { get; set; }
        public List<BoQSubProjectViewModel> SubProjects { get; set; }

    }
    public class BoQSubProjectViewModel
    {
        public BoQSubProjectViewModel()
        {
            StructureTypes = new List<BoQStructureTypeViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BoQStructureTypeViewModel> StructureTypes { get; set; }
    }
    public class BoQStructureTypeViewModel
    {
        public BoQStructureTypeViewModel()
        {
            CategoryList = new List<BoQWorkCategoryViewModel>();
        }
        public string Name { get; set; }
        public int? Code { get; set; }
        public List<BoQWorkCategoryViewModel> CategoryList { get; set; }
    }
    public class BoQWorkCategoryViewModel
    {
        public BoQWorkCategoryViewModel()
        {
            WorkItemList = new List<BoQWorkItemViewModel>();
        }
        public string Name { get; set; }
        public int Code { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BoQWorkItemViewModel> WorkItemList { get; set; }

    }
    public class BoQWorkItemViewModel
    {
        public string Description { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public bool HasChildWorkItems { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
