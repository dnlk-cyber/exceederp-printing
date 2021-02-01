using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    class GrandSummaryViewModels
    {
    }

    public class GSProjectViewModel
    {
        public GSProjectViewModel()
        {
            Blocks = new List<ViewModel.GSBlockViewModel>();

        }
        public string ProjectName { get; set; }
        public List<GSBlockViewModel> Blocks { get; set; }
        public string TotalCost { get; set; }
        public string VAT { get; set; }
        public string TotalWithVAT { get; set; }
    }
    public class GSBlockViewModel
    {
        public string Name { get; set; }
        public int NumberOfBlocks { get; set; }
        public decimal PricePerBlock { get; set; }
        public decimal PricePerTotalBlock { get; set; }
    }
    public class GSSubProjectTypeViewModel
    {
        public string Name { get; set; }
        public int NumberOfBlocks { get; set; }
        public decimal PricePerBlock { get; set; }
        public decimal PricePerTotalBlock { get; set; }
    }

    public class RoadGrandSummaryProjectViewModel
    {
        public RoadGrandSummaryProjectViewModel()
        {
            StructuresList = new List<RoadGrandSummaryStructureTypeViewModel>();
        }
        public string Name { get; set; }
        public List<RoadGrandSummaryStructureTypeViewModel> StructuresList { get; set; }
    }
    public class RoadGrandSummaryStructureTypeViewModel
    {
        public string Name { get; set; }
        public int? Code { get; set; }
        public decimal TotalCost { get; set; }
    }
}
