using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{

    public class ProjectCostAnalysisSubProjectTypeVM
    {
        public ProjectCostAnalysisSubProjectTypeVM()
        {
            StructureList = new List<ProjectCostAnalysisStructureTypeVM>();
        }
        public string Name { get; set; }
        public List<ProjectCostAnalysisStructureTypeVM> StructureList { get; set; }
    }
    public class ProjectCostAnalysisStructureTypeVM
    {
        public ProjectCostAnalysisStructureTypeVM()
        {
            WorkCategoryList = new List<ProjectCostAnalysisWorkCategoryVM>();
        }
        public string Name { get; set; }
        public List<ProjectCostAnalysisWorkCategoryVM> WorkCategoryList { get; set; }
    }
    public class ProjectCostAnalysisWorkCategoryVM
    {
        public ProjectCostAnalysisWorkCategoryVM()
        {
            WorkItemList = new List<ProjectCostAnalysisWorkItemVM>();
        }
        public string Name { get; set; }
        public List<ProjectCostAnalysisWorkItemVM> WorkItemList { get; set; }

    }
    public class ProjectCostAnalysisWorkItemVM
    {
        public ProjectCostAnalysisWorkItemVM()
        {
            MaterialList = new List<ProjectCostAnalysisMaterialVM>();
            LaborList = new List<ProjectCostAnalysisLaborVM>();
            EquipmentList = new List<ProjectCostAnalysisEquipmentVM>();
        }
        public string Name { get; set; }
        public decimal HourlyOutput { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Unit { get; set; }

        public decimal TotalMaterialCost { get; set; }
        public decimal TotalLaborCost { get; set; }
        public decimal TotalEquipmentCost { get; set; }

        public decimal MaterialCostPerUnit { get; set; }

        public decimal EquipmentCostPerUnit { get; set; }

        public decimal LaborCostPerUnit { get; set; }

        public decimal DirectCost { get; set; }

        public decimal OverHeadCost { get; set; }
        public decimal ProfitCost { get; set; }
        public decimal TotalUnitCost { get; set; }

        public List<ProjectCostAnalysisMaterialVM> MaterialList { get; set; }
        public List<ProjectCostAnalysisLaborVM> LaborList { get; set; }
        public List<ProjectCostAnalysisEquipmentVM> EquipmentList { get; set; }

    }
    public class ProjectCostAnalysisMaterialVM
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }
    }
    public class ProjectCostAnalysisLaborVM
    {
        public string Title { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitFactor { get; set; }
        public decimal HourlySalary { get; set; }
        public decimal TotalHourlyCost { get; set; }
    }
    public class ProjectCostAnalysisEquipmentVM
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitFactor { get; set; }
        public decimal HourlyRentalCost { get; set; }
        public decimal HourlyCost { get; set; }
    }
}
