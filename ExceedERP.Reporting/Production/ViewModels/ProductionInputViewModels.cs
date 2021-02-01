using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Production.ViewModels
{
    //class ProductionInputViewModels
    //{
    //}
    public class ProductionYearlyInputsVM
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    //public class ProductionMonthlyInputsVM
    //{
    //    public string Name { get; set; }
    //}
    public class ProductionOrderSeedSpecificVM
    {
        public ProductionOrderSeedSpecificVM()
        {
            Inputs = new List<ProductionSeedSpecificInputsVM>();

            MachinaryOperationList = new List<ProductionSeedSpecificMachinaryOperationVM>();
            LaborOperationList = new List<ProductionSeedSpecificLaborOperationVM>();
            MaterialInputList = new List<ProductionSeedSpecificInputMaterialVM>();
        }
        public string SeedName { get; set; }

        public List<ProductionSeedSpecificInputsVM> Inputs { get; set; }

        public List<ProductionSeedSpecificMachinaryOperationVM> MachinaryOperationList { get; set; }
        public List<ProductionSeedSpecificLaborOperationVM> LaborOperationList { get; set; }
        public List<ProductionSeedSpecificInputMaterialVM> MaterialInputList { get; set; }

    }
    public class ProductionSeedSpecificInputsVM
    {
        //public ProductionSeedSpecificInputsVM()
        //{
            
        //}
        public string Month { get; set; }
        // for sorting by month
        public int Order { get; set; }
        public string CostType { get; set; }
        public decimal CostAmount { get; set; }

    }

    public class ProductionSeedSpecificMachinaryOperationVM
    {
        public string OperationName { get; set; }
        public string Month { get; set; }
        public int Order { get; set; }
        public decimal CostAmount { get; set; }
    }
    public class ProductionSeedSpecificLaborOperationVM
    {
        public string OperationName { get; set; }
        public string Month { get; set; }
        public int Order { get; set; }
        public decimal CostAmount { get; set; }
    }
    public class ProductionSeedSpecificInputMaterialVM
    {
        public string Name { get; set; }
        public string Month { get; set; }
        public int Order { get; set; }
        public decimal CostAmount { get; set; }
    }
}
