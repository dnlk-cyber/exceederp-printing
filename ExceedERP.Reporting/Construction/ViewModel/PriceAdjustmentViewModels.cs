using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    //class PriceAdjustmentViewModels
    //{
    //}

    public class PriceAdjustmentWeightViewMoedl
    {
        //public PriceAdjustmentWeightViewMoedl()
        //{
        //    CostItems = new List<PriceAdjustmentWeightItemViewModel>();
        //}
        public string CategoryName { get; set; }
        public decimal ContractAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public string MaterialName { get; set; }
        public decimal Amount { get; set; }

        //public List<PriceAdjustmentWeightItemViewModel> CostItems { get; set; }
    }

    //public class PriceAdjustmentWeightItemViewModel
    //{
    //    public string ItemName { get; set; }
    //    public decimal Amount { get; set; }
    //}


    public class PriceAdjustmentProjectViewModel
    {
        public PriceAdjustmentProjectViewModel()
        {
            CategoryWeightList = new List<PriceAdjFormulaWeightViewMoedl>();
        }
        public string Name { get; set; }
        public decimal TotalContractAmount { get; set; }
        public decimal TotalActualAmount { get; set; }
        public string ConcatnatedWeightValue { get; set; }
        public string Formula { get; set; }

        public List<PriceAdjFormulaWeightViewMoedl> CategoryWeightList { get; set; }
    }
    public class PriceAdjFormulaWeightViewMoedl
    {
        public PriceAdjFormulaWeightViewMoedl()
        {
            CostItems = new List<PriceAdjustmentWeightItemViewModel>();
        }
        public string CategoryName { get; set; }
        public decimal ContractAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public string MaterialName { get; set; }
        public decimal Amount { get; set; }

        public List<PriceAdjustmentWeightItemViewModel> CostItems { get; set; }
    }

    public class PriceAdjustmentWeightItemViewModel
    {
        public string ItemName { get; set; }
        public decimal Amount { get; set; }
    }
}
