using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    public class MajorMaterialProjectViewModel
    {
        public MajorMaterialProjectViewModel()
        {
            MaterialList = new List<MajorMaterialViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public List<MajorMaterialViewModel> MaterialList { get; set; }
    }
    public class MajorMaterialViewModel
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }
        [Display(Name = "% wrt total Project Cost")]
        public decimal PercentageToTotalCostOfProject { get; set; }
    }

    public class RepresentativeElementProjectViewModel
    {
        public RepresentativeElementProjectViewModel()
        {
            RepresentativeElementsList = new List<RepresentativeElementViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public List<RepresentativeElementViewModel> RepresentativeElementsList { get; set; }
    }
    public class RepresentativeElementViewModel
    {
        
        public string IndexCode { get; set; }
        public string Description { get; set; }
        
        public string RepresentedItems { get; set; }
        public string Source { get; set; }

        public decimal BasicPrice { get; set; }
    }

}
