using System.Collections.Generic;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    class WorkItemViewModels
    {
    }
    public class EstimationWorkItemViewModel
    {
        public EstimationWorkItemViewModel()
        {
            MaterialList = new List<EstimationMaterialViewModel>();
            EquipmentList = new List<EstimationEquipmentViewModel>();
            LaborList = new List<EstimationLabourViewModel>();
        }
        public virtual string Category { get; set; }
        public string ItemName { get; set; }
        public decimal TotalQuantityOfWork { get; set; }
        public decimal TotalUnitCost { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string ProjectType { get; set; }
        public decimal HourlyOutput { get; set; }

        public decimal DirectCost { get; set; }
        public decimal MaterialUnitCost { get; set; }
        public decimal LaborUnitCost { get; set; }
        public decimal EquipmentUnitCost { get; set; }
        public decimal OverHeadRate { get; set; }
        public decimal ProfitRate { get; set; }
        public decimal OverheadCost { get; set; }
        public decimal ProfitCost { get; set; }
        //public decimal TotalCost { get; set; }

        public List<EstimationMaterialViewModel> MaterialList { get; set; }
        public List<EstimationEquipmentViewModel> EquipmentList { get; set; }
        public List<EstimationLabourViewModel> LaborList { get; set; }

    }
    public class EstimationMaterialViewModel
    {

        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }

    }
    public class EstimationLabourViewModel
    {

        public string Title { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitFactor { get; set; }
        public decimal HourlySalary { get; set; }
        public decimal TotalHourlyCost { get; set; }

    }
    public class EstimationEquipmentViewModel
    {

        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitFactor { get; set; }
        public decimal HourlyRentalCost { get; set; }
        public decimal HourlyCost { get; set; }

    }

    public class EstimationPieceRateWorkItemViewModel
    {
        public string ProjectType { get; set; }
        public string WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Area { get; set; }
        public decimal Price { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal CylinderLock { get; set; }
        public decimal DirectCost { get; set; }
        public decimal OverHeadProfitCost { get; set; }
    }

    public class EstimationSubContractWorkItemViewModel
    {
        public  string ProjectType { get; set; }
        public  string WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        public string UnitOfMeasurement { get; set; }
        public decimal Quntity { get; set; }
        public decimal SubContractCostOne { get; set; }
        public decimal SubContractCostTwo { get; set; }
        public decimal SubContractCostThree { get; set; }
        public decimal SelectedCost { get; set; }
        public decimal OverHeadProfit { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class EstimationCustomWorkItemViewModel
    {
        public EstimationCustomWorkItemViewModel()
        {
            WorkItems = new List<EstimationCustomWorkItemContentViewModel>();
        }
        public string WorkItemName { get; set; }

        public decimal TotalUnitRate { get; set; }
        public string ProjectType { get; set; }
        public List<EstimationCustomWorkItemContentViewModel> WorkItems { get; set; }
    }

    public class EstimationCustomWorkItemContentViewModel
    {
        public string WorkItem { get; set; }
        public decimal QuantityPerUnit { get; set; }
        public decimal UnitCost { get; set; }
        public decimal UnitPrice { get; set; }
    }
    // 
    // Summary:
    //      View models for showing all the work items, piece ratte, sub contract work item
    //      and custom work items used in estimation templates
    public class ProjectEstimaionTemplateViewModel
    {
        public  string ProjectType { get; set; }

        public ProjectEstimaionTemplateViewModel()
        {
            WorkItemList = new List<ProjectEstimaionTemplateWorkItemViewModel>();
            PieceRateList = new List<ProjectEstimaionTemplatePieceRateViewModel>();
            SubContractWorkItemList = new List<ProjectEstimaionTemplateSubContractViewModel>();
            CustomWorkItemList = new List<ProjectEstimaionTemplateCustomWorkItemViewModel>();
        }
        public List<ProjectEstimaionTemplateWorkItemViewModel> WorkItemList { get; set; }
        public List<ProjectEstimaionTemplatePieceRateViewModel> PieceRateList { get; set; }
        public List<ProjectEstimaionTemplateSubContractViewModel> SubContractWorkItemList { get; set; }
        public List<ProjectEstimaionTemplateCustomWorkItemViewModel> CustomWorkItemList { get; set; }

    }

    public class ProjectEstimaionTemplateWorkItemViewModel
    {
        public string ItemCode { get; set; }
        public string Category { get; set; }
        public  string Name { get; set; }
        public decimal UnitRate { get; set; }
    }

    public class ProjectEstimaionTemplatePieceRateViewModel
    {
        public string ItemCode { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public decimal UnitRate { get; set; }
    }

    public class ProjectEstimaionTemplateSubContractViewModel
    {
        public string ItemCode { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public decimal UnitRate { get; set; }
    }

    public class ProjectEstimaionTemplateCustomWorkItemViewModel
    {
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public decimal UnitRate { get; set; }
    }

}
