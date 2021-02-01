using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    //class ResourceUtilizationViewModels
    //{
    //}
    public class ResourceUtilizationDayViewModel
    {
        public ResourceUtilizationDayViewModel()
        {
            WorkItemList = new List<ViewModel.ResourceUtilizationWorkItemViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ResourceUtilizationWorkItemViewModel> WorkItemList { get; set; }
    }
    public class ResourceUtilizationWorkItemViewModel
    {
        public ResourceUtilizationWorkItemViewModel()
        {
            MaterialList = new List<ResourceUtilizationaterialViewModel>();
            EquipmentList = new List<ResourceUtilizationEquipmentViewModel>();
            LaborList = new List<ResourceUtilizationLabourViewModel>();
        }
        public virtual string Category { get; set; }
        public string MethodOfWork { get; set; }
        public string WorkItem { get; set; }
        public string UnitOfMeasurnment { get; set; }

        public string ProjectName { get; set; }
        public decimal ExecutedWork { get; set; }
        public decimal WorkDurationInHoursPerDay { get; set; }
        public decimal OutPutPerDay { get; set; }
        public decimal DirectContractUnitPrice { get; set; }
        public decimal Income { get; set; }

        public string LastModified { get; set; }

        public decimal MaterialCost { get; set; }
        public decimal EquipmentCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal TotalDirectCost { get; set; }
        public decimal OverheadCost { get; set; }
        public decimal ProfitCost { get; set; }
        public decimal TotalCost { get; set; }

        // ratios of income, material, equpiment and labor costs
        public decimal DirectIncomeExpenseRatio { get; set; }
        public decimal PercentageOfMaterialCostWrtTotalCost { get; set; }
        public decimal PercentageOfLaborCostWrtTotalCost { get; set; }
        public decimal PercentageOfEquipmentCostWrtTotalCost { get; set; }


        public List<ResourceUtilizationaterialViewModel> MaterialList { get; set; }
        public List<ResourceUtilizationEquipmentViewModel> EquipmentList { get; set; }
        public List<ResourceUtilizationLabourViewModel> LaborList { get; set; }

    }
    public class ResourceUtilizationaterialViewModel
    {


        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }

    }
    public class ResourceUtilizationLabourViewModel
    {

        public string Title { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitFactor { get; set; }
        public decimal HourlySalary { get; set; }
        public decimal TotalHourlyCost { get; set; }

    }
    public class ResourceUtilizationEquipmentViewModel
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitFactor { get; set; }
        public decimal HourlyRentalCost { get; set; }
        public decimal HourlyCost { get; set; }

    }
}
