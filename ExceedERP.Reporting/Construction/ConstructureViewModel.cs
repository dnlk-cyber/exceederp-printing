using System.Collections.Generic;

namespace ExceedERP.Reporting.Construction
{
    public class ConstructureViewModel
    {
    }

    public class BlockGrandSummary
    {
        public string BlockName { get; set; }
        public int NoOfBlock { get; set; }
        public decimal UnitPriceWithVat { get; set; }
        public decimal TotalPriceWithVat { get; set; }
        public decimal TotalPrcWOVat { get; set; }
    }
    public class SummaryOfBOQ
    {
        public string ProjectName { get; set; }

        public List<WorkItemCategory> Categories { get; set; }

        public decimal Total { get; set; }
        public double SubTotal { get; set; }

        public decimal Rebate { get; set; }
        public decimal AfterRebate { get; set; }
        public decimal Vat { get; set; }
        public decimal GrandTotal { get; set; }
        public List<Structures> Structures { get; set; }

    }

    public class Structures
    {
        public string StructureType { get; set; }
        public List<SuperWorkItemCategory> SuperCategories { get; set; }
    }
    public class WorkItemCategory
    {
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public decimal Total { get; set; }
        public decimal Quntity { get; set; }
        public decimal UnitRate { get; set; }
    }
    public class SuperWorkItemCategory
    {
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public decimal Total { get; set; }
        public decimal Quntity { get; set; }
        public decimal UnitRate { get; set; }

    }
    public class GrandSummary
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public double SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal GrandTotal { get; set; }
        public List<GrandStructureType> GrandStructureType { get; set; }

    }

    public class GrandStructureType
    {
        public string StructureType { get; set; }
        public List<GrandSuperWorkItemCategory> GrandSuperWorkItemCategory { get; set; }
    }
    public class WorkItem
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public decimal Quntity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal Total { get; set; }
        public List<TakeOffSheet> TakeOffSheet { get; set; }
    }
    public class GrandSubWorkItemCategory
    {
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public decimal Total { get; set; }
        public decimal Quntity { get; set; }
        public decimal UnitRate { get; set; }
        public List<WorkItem> SubWorkItems { get; set; }
    }
    public class GrandSuperWorkItemCategory
    {
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public decimal Total { get; set; }
        public decimal Quntity { get; set; }
        public decimal UnitRate { get; set; }
        public List<WorkItem> SuperWorkItems { get; set; }
    }
    public class TakeOffSheet
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public decimal Quntity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal Total { get; set; }
    }

    //
    // modified view models for reporting
    public class WorkItemViewModel
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public decimal Quntity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal Total { get; set; }
    }
    public class WorkItemOfSubStructureViewModel
    {
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public decimal Total { get; set; }
        public decimal Quntity { get; set; }
        public decimal UnitRate { get; set; }
        public List<WorkItemViewModel> SubStructureWorkItems { get; set; }
    }
    public class WorkItemOfSuperStructureViewModel
    {
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public decimal Total { get; set; }
        public decimal Quntity { get; set; }
        public decimal UnitRate { get; set; }
        public List<WorkItemViewModel> SuperStructureWorkItems { get; set; }
    }
    public class WorkCategoryViewModel
    {
        public string CategoryName { get; set; }
        public List<WorkItemViewModel> WorkItems { get; set; }
    }
    public class StructureTypeViewModel
    {
        public string StructureType { get; set; }
        public List<WorkCategoryViewModel> Categories { get; set; } 
    }
    public class BlockViewModel
    {
        public string BlockName { get; set; }
        public List<StructureTypeViewModel> StructureTypes { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCostWithVat { get; set; }
    }
    public class BoQSummaryViewModel
    {
        public string ProjectName { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCostWithVat { get; set; }
        public List<BlockViewModel> Blocks { get; set; }
    }
    //
    //
}

