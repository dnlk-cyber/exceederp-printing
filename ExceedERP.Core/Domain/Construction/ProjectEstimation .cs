using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.Construction
{
    public class ProjectEstimation
    {
    }
    public class ProjectEstimationName : TrackUserOperation
    {
        //Create project 
        [Key]
        public int ProjectEstimationNameId { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        [Required]

        [Display(Name = "Project Location")]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }
        public virtual ICollection<ProjectEstimationBoq> ProjectEstimationBoq { get; set; }
        [Display(Name = "Project Type")]
        public int LookupId { get; set; }

    }
    public class ProjectEstimationBoq : TrackUserOperation
    {
        [Key]
        public int ProjectEstimationBoqid { get; set; }
        public int ProjectInformationId { get; set; }
        // public virtual ProjectInformation ProjectInformation { get; set; }
        [Required]
        [Display(Name = "Type of BOQ")]
        public TypeOfConstruction TypeOfBoq { get; set; }
        [Required]
        [Display(Name = "Bill Number")]
        public string BillNumber { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        // public virtual ICollection<ProjectEstimationBoqBreakdown> ProjectEstimationBoqBreakdown { get; set; }

    }
    public class ProjectEstimationBoqBreakdown : TrackUserOperation //this class can us project work item summary as an input 
    {
        //Item breakdown for BOQ
        [Key]
        public int ProjectEstimationBoqBreakdownId { get; set; }

        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
        public int GenericWorkItemPerProjectId { get; set; }
        //  public virtual ProjectEstimationWorkItem ProjectEstimationWorkItem { get; set; }
        [Required]
        [Display(Name = "Item No.")]
        public string BillNumber { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public int UnitOfMeasurementId { get; set; }
        //public virtual UnitOfMeasurement UnitOfMeasurement { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public Decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Rate")]
        public decimal UnitRate { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        [Display(Name = "Type")]
        public int ConfigurationId { get; set; }
        [Display(Name = "Work Item Category")]
        public int WorkCategoryId { get; set; }
        //public  WorkCategory WorkCategory { get; set; }

        public string WorkCategory { get; set; }
        public string StructureyType { get; set; }
        public string SubProjectTypeName { get; set; }

        public int Index { get; set; }
        [Display(Name = "Sub-Project Type")]
        public int SubProjectTypeId { get; set; }

    }
    public class ProjectEstimationBoqDetail : TrackUserOperation
    {
        [Key]
        public int ProjectEstmnBoqDetailId { get; set; }
        public int ProjectEstimationBoqBreakdownId { get; set; }
        public virtual ProjectEstimationBoqBreakdown ProjectEstimationBoqBreakdown { get; set; }

        [Display(Name = "Bill Number")]
        public string BillNumber { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public int UnitOfMeasurementId { get; set; }
        //public virtual  UnitOfMeasurement UnitOfMeasurement { get; set; }
        public decimal UnitRate { get; set; }

        public decimal Quntity { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public int Index { get; set; }

        // public virtual ICollection<ProjectEstimationBoqBreakdown> ProjectEstimationBoqBreakdown { get; set; }

    }
    public class ProjectEstmnBoqSummary : TrackUserOperation
    {
        [Key]
        public int ProjectEstmnBoqSummaryId { get; set; }
        public string Unit { get; set; }
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public int ProjectInformationId { get; set; }
        public virtual ProjectInformation ProjectInformation { get; set; }
        [Display(Name = "sub-project type")]
        public int SubProjectTypeId { get; set; }

        [Display(Name ="Description")]
        public string WorkCategory { get; set; }
        public string StructureyType { get; set; }
        public string SubProjectTypeName { get; set; }
    }

    //public class ProjectSummary
    //{
    //    public int ProjectSummary { get; set; }
    //    public StructureType Type { get; set; }

    //   // public 
    //}
    public class ProjectEstimationWorkCategory : TrackUserOperation
    {
        //Define direct cost [Work Category] 
        [Key]
        public int ProjectEstimationWorkCategoryId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Category Name")]
        public string Category { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal TotalCost { get; set; }
        //public virtual ICollection<ProjectEstimationWorkItem> ProjectEstimationWorkItem { get; set; }

    }
    public class WorkItemDetailViewModel
    {
        public int Id { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }

    }
    public class ProjectEstimationWorkItem : TrackUserOperation
    {
        [Key]
        public int ProjectEstimationWorkItemId { get; set; }

        [Display(Name = "Category")]
        //[Required]
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }

        //[DataType(DataType.MultilineText)]
        [Required]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Display(Name = "Total Qty Of Work")]
        [Required]
        public decimal TotalQuantityOfWork { get; set; }
        // total costs
        [Display(Name = "Total Material Cost")]
        public decimal MaterialCost { get; set; }

        [Display(Name = "Total Labour Cost")]
        public decimal LabourOutPut { get; set; }

        [Display(Name = "Total Equipment Cost")]
        public decimal EquipmentOutput { get; set; }
        //

        //
        //  costs per unit of work per hour 
        [Display(Name ="Material unit Cost")]
        public decimal MaterialCostPerUnit { get; set; }

        [Display(Name = "Equipment unit Cost")]
        public decimal EquipmentCostPerUnit { get; set; }

        [Display(Name = "Labor unit Cost")]
        public decimal LaborCostPerUnit { get; set; }

        [Display(Name = "Direct Cost")]
        public decimal DirectCost { get; set; }

        public decimal OverHeadCost { get; set; }
        public decimal ProfitCost { get; set; }
        [Display(Name = "Total Unit Cost")]
        public decimal TotalUnitCost { get; set; }
        //

        [Display(Name = "Project Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Hrly Output")]
        public decimal LabourHourlyOutPut { get; set; }
        [Display(Name = "UOM")]
        public int? UnitOfMeasurementId { get; set; }
        [Display(Name = "Overhead Rate")]
        public decimal OverheadRate { get; set; }
        [Display(Name = "Profit Rate")]
        public decimal ProfitMargin { get; set; }

        [Display(Name = "Parent")]
        public int ParentId { get; set; }//work item if it has parent

        public int? BlockId { get; set; }
        public bool IsSubCategory { get; set; }
        public bool ContainsWorkItemAsInput { get; set; }
        public string ItemCode { get; set; }

        public virtual ICollection<ProjectEstimationMaterial> ProjectEstimationMaterial { get; set; }
        public virtual ICollection<ProjectEstimationDirectLabour> ProjectEstimationDirectLabour { get; set; }
        public virtual ICollection<ProjectEstimationIndirectLabour> ProjectEstimationIndirectLabour { get; set; }
        public virtual ICollection<ProjectEstimationEquipment> ProjectEstimationEquipment { get; set; }
    }
    public class CopyTemplateWorkItemViewModel
    {
        public int Id { get; set; }
        public bool IsCopied { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public decimal TotalQuantityOfWork { get; set; }
        public decimal TotalUnitRate { get; set; }
        public string UnitOfMeasurement { get; set; }
        public bool IsSubCategory { get; set; }
    }
    public class AddVariationAndSupplementaryWorkItemViewModel
    {
        public int Id { get; set; }
        public bool IsCopied { get; set; }
        public bool IsCopiedAsVariationWork { get; set; }
        public bool IsCopiedAsSupplementaryWork { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public decimal TotalQuantityOfWork { get; set; }
        public decimal TotalUnitRate { get; set; }
        public string UnitOfMeasurement { get; set; }
    }
    public class ProjectEstimationAreaWorkItem : TrackUserOperation
    {
        [Key]
        public int ProjectEstimationAreaWorkItemId { get; set; }
        [DisplayName("Category")]
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("W")]
        public decimal Width { get; set; }
        [DisplayName("H")]
        public decimal Height { get; set; }
        public decimal Area { get; set; }
        [DisplayName("Price/m2")]
        public decimal Price { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal CylinderLock { get; set; }
        public decimal DirectCost { get; set; }
        [DisplayName("Over Head + Profit")]
        public decimal OverHeadProfitCost { get; set; }
        [Display(Name = "Project Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        public int? BlockId { get; set; }
        public string ItemCode { get; set; }
    }
    public class CopyTemplateAreaWorkItemViewModel
    {
        public int Id { get; set; }
        public bool IsCopied { get; set; }
        public  string WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        
        [DisplayName("Price/m2")]
        public decimal Price { get; set; }
        [DisplayName("Over Head + Profit")]
        public decimal OverHeadProfitCost { get; set; }
        public string ItemCode { get; set; }
    }
    public class EstimationSubContractWorkItem : TrackUserOperation
    {
        [Key]
        public int EstimationSubContractWorkItemId { get; set; }
        [DisplayName("Category")]
        public int WorkCategoryId { get; set; }
        public virtual WorkCategory WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        [DisplayName("UOM")]
        public int UnitOfMeasurementId { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        [DisplayName("QTY")]
        public decimal Quntity { get; set; }
        [DisplayName("Sub contract cost  1")]
        public decimal SubContractCostOne { get; set; }
        [DisplayName("Sub contract cost  2")]
        public decimal SubContractCostTwo { get; set; }
        [DisplayName("Sub contract cost  3")]
        public decimal SubContractCostThree { get; set; }
        [DisplayName("Selected cost")]
        public decimal SelectedCost { get; set; }
        [DisplayName("Over heaad & profit")]
        public decimal OverHeadProfit { get; set; }
        [DisplayName("Total cost(including0")]
        public decimal TotalCost { get; set; }
        [Display(Name = "Project Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        public int? BlockId { get; set; }
        public string ItemCode { get; set; }
    }
    public class CopyTemplateSubContractWorkItemViewModel {
        public int Id { get; set; }
        public bool IsCopied { get; set; }
        public  string WorkCategory { get; set; }
        public string WorkItemName { get; set; }
        public string UoM { get; set; }
        public decimal TotalCost { get; set; }
        public string ItemCode { get; set; }
    }
    public class ProjectEstimationCustomWorkItem : TrackUserOperation
    {
        [Key]
        public int ProjectEstimationCustomWorkItemId { get; set; }
        [Display(Name = "Category")]
        public int WorkCategoryId { get; set; }

        [Display(Name = "Item Name")]
        public string WorkItemName { get; set; }

        
        [Display(Name = "Project Type")]
        public AvailableProjectTypes ProjectType { get; set; }
        [Display(Name ="Total Material Cost")]
        public decimal MaterialCost { get; set; }
        [Display(Name ="Total Equipment Cost")]
        public decimal EquipmentCost { get; set; }
        [Display(Name ="Total Labor Cost")]
        public decimal LaborCost { get; set; }
        public string Remark { get; set; }
        public string ItemCode { get; set; }

        [Display(Name = "Total Qty Of Work")]
        [Required]
        public decimal TotalQuantityOfWork { get; set; }

        [Display(Name = "Hrly Output")]
        public decimal HourlyOutPut { get; set; }
        [Display(Name = "UOM")]
        public int? UnitOfMeasurementId { get; set; }
        [Display(Name = "Overhead Rate")]
        public decimal OverheadRate { get; set; }
        [Display(Name = "Profit Rate")]
        public decimal ProfitRate { get; set; }

        //
        //  costs per unit of work per hour 
        [Display(Name = "Material unit Cost")]
        public decimal MaterialCostPerUnit { get; set; }

        [Display(Name = "Equipment unit Cost")]
        public decimal EquipmentCostPerUnit { get; set; }

        [Display(Name = "Labor unit Cost")]
        public decimal LaborCostPerUnit { get; set; }

        [Display(Name = "Direct Cost")]
        public decimal DirectCost { get; set; }

        [Display(Name ="Overhead Cost")]
        public decimal OverHeadCost { get; set; }
        [Display(Name ="Profit Cost")]
        public decimal ProfitCost { get; set; }
        [Display(Name = "Unit Rate")]
        public decimal TotalUnitRate { get; set; }
        //

       
        public virtual ICollection<EstimationCustomWorkItemContent> WorkItems { get; set; }

    }
    public class EstimationCustomWorkItemContent : TrackUserOperation
    {
        [Key]
        public int EstimationCustomWorkItemContentId { get; set; }
        [Display(Name = "Work Item")]
        public int ProjectEstimationWorkItemId { get; set; }
        public int ProjectEstimationCustomWorkItemId { get; set; }

        [Display(Name = "Qty/unit")]
        public decimal QuantityPerUnit { get; set; }
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }


        public virtual ProjectEstimationWorkItem RootWorkItem { get; set; }
        //public virtual ProjectEstimationCustomWorkItem CustomWorkItem { get; set; }
    }
    public class EstimationTemplateCustomWorkItemDetail : TrackUserOperation
    {
        [Key]
        public int EstimationTemplateCustomWorkItemDetailId { get; set; }

        [Display(Name = "Work Item")]
        public int ProjectEstimationWorkItemId { get; set; }
        // its ProjectEstimationWorkItemId but used to show under which work item this content/detail is used
        public int ParentId { get; set; }

        [Display(Name = "Qty/unit")]
        public decimal QuantityPerUnit { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitCost { get; set; }
        [Display(Name = "Totl Cost")]
        public decimal TotalCost { get; set; }

        public virtual ProjectEstimationWorkItem RootWorkItem { get; set; }
    }
    public class ProjectEstimationMaterial : TrackUserOperation
    {
        //Define direct cost [Material Cost] 
        [Key]
        public int ProjectEstimationMaterialId { get; set; }
        public int ProjectEstimationWorkItemId { get; set; }
        public virtual ProjectEstimationWorkItem ProjectEstimationWorkItem { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Category")]
        public string ItemCategoryCode { get; set; }
        //public virtual ExceedERP.Core.Domain.Inventory.InventoryItem Item { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Type Of Material")]
        public string ItemCode { get; set; }
        public string Unit { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal MaterialUnitCost { get; set; }
    }
    public class ProjectEstimationDirectLabour : TrackUserOperation
    {
        //Define direct cost [Direct Labour Cost] 
        [Key]
        public int ProjectEstimationDirectLabourId { get; set; }
        public int ProjectEstimationWorkItemId { get; set; }
        public virtual ProjectEstimationWorkItem ProjectEstimationWorkItem { get; set; }

        //[Required]
        //[StringLength(100)]
        //[Display(Name = "Item No.")]
        //public string ItemNo { get; set; }
        [Required]

        [Display(Name = "Labour Title")]
        // public int LabourName { get; set; }
        public int ProjectEstimationIndexLabourId { get; set; }
        public virtual ProjectEstimationIndexLabour ProjectEstimationIndexLabour { get; set; }
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "U.F.")]
        public decimal UnitFactory { get; set; }
        [Required]
        [Display(Name = "Daily Pay")]
        public decimal DailyPay { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal ManpowerUnitCost { get; set; }

        [Display(Name = "Indexed hrly sal.")]
        public decimal IndexedHourlySalary { get; set; }
    }
    public class ProjectEstimationIndirectLabour : TrackUserOperation
    {
        //Define direct cost [Direct Labour Cost] 
        [Key]
        public int ProjectEstimationIndirectLabourId { get; set; }
        public int ProjectEstimationWorkItemId { get; set; }
        public virtual ProjectEstimationWorkItem ProjectEstimationWorkItem { get; set; }
        [StringLength(100)]
        [Required]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Labour Name")]
        public string LabourName { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Factory")]
        public decimal UnitFactory { get; set; }
        [Required]
        [Display(Name = "Daily Pay")]
        public decimal DailyPay { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

    }
    public class ProjectEstimationEquipment : TrackUserOperation
    {
        //Define direct cost [Equipment Cost] 
        [Key]
        public int ProjectEstimationEquipmentId { get; set; }
        public int ProjectEstimationWorkItemId { get; set; }
        public virtual ProjectEstimationWorkItem ProjectEstimationWorkItem { get; set; }
        //[Display(Name = "Equipment Type")]
        //public Guid EquipmentTypeID { get; set; }
        //[Display(Name = "Equipment Category")]
        //public Guid EquipmentIdentityID { get; set; }
        [Required]
        [Display(Name = "Equipment Name")]
        public Guid FleetsID { get; set; }
        public virtual Fleets Fleet { get; set; }
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Factory")]
        public decimal UnitFactory { get; set; }
        [Required]
        [Display(Name = "Rental Cost")]
        public decimal RentalCost { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal EquipmentUnitCost { get; set; }
        public string EquipmentType { get; set; }

    }
    public class EstimationCustomWorkItemMaterial : TrackUserOperation
    {
        [Key]
        public int EstimationCustomWorkItemMaterialId { get; set; }
        public int ProjectEstimationCustomWorkItemId { get; set; }
        public virtual ProjectEstimationCustomWorkItem CustomWorkItem { get; set; }

        [StringLength(100)]
        [Display(Name = "Category")]
        public string ItemCategoryCode { get; set; }
        //public virtual ExceedERP.Core.Domain.Inventory.InventoryItem Item { get; set; }

        [StringLength(100)]
        [Display(Name = "Type Of Material")]
        public string ItemCode { get; set; }
        public string Unit { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
    }
    public class EstimationCustomWorkItemEquipment : TrackUserOperation
    {
        [Key]
        public int EstimationCustomWorkItemEquipmentId { get; set; }
        public int ProjectEstimationCustomWorkItemId { get; set; }
        public virtual ProjectEstimationCustomWorkItem CustomWorkItem { get; set; }

        [Display(Name = "Equipment Name")]
        public Guid FleetsID { get; set; }
        public virtual Fleets Fleet { get; set; }
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Factory")]
        public decimal UnitFactory { get; set; }
        [Required]
        [Display(Name = "Rental Cost")]
        public decimal RentalCost { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        //public decimal EquipmentUnitCost { get; set; }
        //public string EquipmentType { get; set; }
    }
    public class EstimationCustomWorkItemDirectLabor : TrackUserOperation
    {
        [Key]
        public int EstimationCustomWorkItemDirectLaborId { get; set; }
        public int ProjectEstimationCustomWorkItemId { get; set; }
        public virtual ProjectEstimationCustomWorkItem CustomWorkItem { get; set; }

        [Display(Name = "Labour Title")]
        public int ProjectEstimationIndexLabourId { get; set; }
        public virtual ProjectEstimationIndexLabour ProjectEstimationIndexLabour { get; set; }
        public string Unit { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Factory")]
        public decimal UnitFactory { get; set; }
        [Required]
        [Display(Name = "Daily Pay")]
        public decimal DailyPay { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        public decimal ManpowerUnitCost { get; set; }

        [Display(Name = "Indexed hrly sal.")]
        public decimal IndexedHourlySalary { get; set; }
    }
    public class ProjectEstimationUnitRate : TrackUserOperation
    {
        //Define Unit Rate  
        [Key]
        public int ProjectEstimationUnitRateId { get; set; }
        [StringLength(100)]
        [Required]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Required]
        [Display(Name = "Direct Cost")]
        public decimal DirectCost { get; set; }
        [Required]
        [Display(Name = "Overheard Cost")]
        public decimal Overheard { get; set; }
        [Required]
        [Display(Name = "Profit Margin")]
        public decimal Profit { get; set; }
        [Required]
        [Display(Name = "Unit Rate")]
        public decimal UnitRate { get; set; }

    }
    public class ProjectEstimationUnitFactory : TrackUserOperation
    {
        //Define Unit Factory  
        [Key]
        public int ProjectEstimationUnitFactoryId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Factory Name")]
        public string FactoryName { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Rate")]
        public decimal UnitRate { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

    }
    public class ProjectEstimationMobilization : TrackUserOperation
    {
        //Input for BOQ [ Mobilization ]  
        [Key]
        public int ProjectEstimationMobilizationId { get; set; }
        //[Machinery and Equipment Mobilization]
        [Required]
        [StringLength(100)]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Type of Machinery")]
        public string TypeOfMachinery { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Required]
        [Display(Name = "Trip")]
        public decimal Trip { get; set; }

        [Required]
        [Display(Name = "Distance")]
        public decimal Distance { get; set; }
        [Required]
        [Display(Name = "Speed(km/hr)")]
        public decimal Speed { get; set; }
        [Required]
        [Display(Name = "Time(hr)")]
        public decimal Time { get; set; }
        [Required]
        [Display(Name = "Rental Cost")]
        public decimal RentalCost { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        //Staffs Mobilization
        [Required]
        [StringLength(200)]
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal StaffQuantity { get; set; }
        [Required]
        [Display(Name = "Time")]
        public decimal StaffTime { get; set; }
        [Required]
        [Display(Name = "Perdiem")]
        public decimal Perdiem { get; set; }
        [Required]
        [Display(Name = "Total Cost")]
        public decimal StaffTotalCost { get; set; }

    }
    public class ProjectEstimationCoefficients : TrackUserOperation
    {
        //Input for BOQ [Coefficients of construction Material  ]  
        [Key]
        public int ProjectEstimationCoefficientsId { get; set; }
        //[Coefficients of construction Material]
        [Required]
        [StringLength(100)]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Required]
        [Display(Name = "Ratio")]
        public decimal Ratio { get; set; }

        [Required]
        [Display(Name = "Kg")]
        public decimal Kg { get; set; }

        [Required]
        [Display(Name = "Shrinkage")]
        public decimal Shrinkage { get; set; }
        [Required]
        [Display(Name = "Wastage")]
        public decimal Wastage { get; set; }
        [Required]
        [Display(Name = "Time(hr)")]
        public decimal Time { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

    }
    public class ProjectEstimationRawData : TrackUserOperation
    {
        //Input for BOQ [ Raw data of construction Material]  
        [Key]
        public int ProjectEstimationRawDataId { get; set; }
        //[Raw data of construction Material]
        [Required]
        [StringLength(100)]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Required]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        public int Category { get; set; }
        public string Remark { get; set; }

    }
    public class ProjectEstimationIndexLabour : TrackUserOperation
    {

        [Key]
        public int ProjectEstimationIndexLabourId { get; set; }
        //[Index Hourly Cost For Labour]

        [Display(Name = "Labour by Trade")]
        public int ProjectJobTitleId { get; set; }
        //public virtual ProjectJobTitle ProjectJobTitle { get; set; }
        public int? WorkingDays { get; set; }

        [Display(Name = "Approved Ind. Sal.")]
        public decimal ApprovedIndexedHourlySalary { get; set; }

        [Display(Name = "Average")]
        public decimal HourlyAverageSalary { get; set; }

        [Display(Name = "Indexed Sal.")]
        public decimal IndexedHourlySalary { get; set; }

        [Display(Name = "Min")]
        public decimal SalaryperdayMin { get; set; }

        [Display(Name = "Max")]
        public decimal SalaryperdayMax { get; set; }

        [Display(Name = "Min")]
        public decimal SalaryperMonthMin { get; set; }

        [Display(Name = "Max")]
        public decimal SalaryperMonthMax { get; set; }

        [Required]
        [Display(Name = "Basic Salary")]
        public decimal Unit { get; set; }

        [Required]
        [Display(Name = "Index Factor")]
        public decimal IndexFactory { get; set; }

        [Required]
        [Display(Name = "Birr/Month")]
        public decimal BirrMonth { get; set; }

        [Required]
        [Display(Name = "Ideal Birr/Day")]
        public decimal IdealBirrDay { get; set; }

        [Required]
        [Display(Name = "Actual Birr/Day")]
        public decimal ActualBirrDay { get; set; }
        //[Per Diem]
        [Required]
        [Display(Name = "Ideal Birr/Day")]
        public decimal IdealBirrDays { get; set; }

        [Required]
        [Display(Name = "Actual Birr/Day")]
        public decimal ActualBirrDays { get; set; }

        [Required]
        [Display(Name = "Desert Allowance Birr/Day")]
        public decimal DesertBirrDays { get; set; }
        //[Net Payment]
        [Required]

        [Display(Name = "Birr/Day")]
        public decimal NetBirrDays { get; set; }
        [Required]
        [Display(Name = "Birr/Hour")]
        public decimal NetBirrHour { get; set; }
        [Display(Name = "From HR")]
        public bool FormHr { get; set; }
        //[Display (Name ="Title")]
        //[Required ]
        //[StringLength (50)]
        //public string Title { get; set; }
    }
    public class ProjectMachineryRentHeader : TrackUserOperation
    {
        //Input for BOQ [ Machinery Rent]  
        [Key]
        public int ProjectEstimationMachineryRentId { get; set; }
        //[Fuel Cost at project location]
        [Required]
        [Display(Name = "Fuel Purchase Cost")]
        public decimal FuelPurchase { get; set; }
        [Required]
        [Display(Name = "Fuel Cost")]
        public decimal FuelCost { get; set; }

        [Required]
        [Display(Name = "Fuel Supply Birr/ltr")]
        public decimal FuelSupply { get; set; }
        [Required]
        [Display(Name = "Fuel Distribution Birr/ltr")]
        public decimal FuelDistribution { get; set; }

        public string Remark { get; set; }
        public int ProjectEstimationNameId { get; set; }
        public virtual ProjectEstimationName ProjectEstimationName { get; set; }

    }

    public class MachinaryRent : TrackUserOperation
    {
        //[List of Equipment]
        [Key]
        public int MachinaryRentId { get; set; }
        [Required]

        [Display(Name = "Type of Equipment")]
        public Guid FleetsID { get; set; }
        public virtual Fleets Fleets { get; set; }

        [Display(Name = "Fuel Type")]
        //public MachineryFuelTypes FuelType { get; set; }
        public string ItemCode { get; set; }
        [Display(Name = "Model")]
        [StringLength(200)]
        public string Model { get; set; }

        [Display(Name = "Rated Capacity")]
        public string RatedCapaciy { get; set; }

        [Display(Name = "HP")]
        public decimal HP { get; set; }

        [StringLength(50)]
        [Display(Name = "Year of Manufacture")]
        public string YearManufacture { get; set; }

        [Required]

        [Display(Name = "Rental Cost /hour")]
        public decimal Rental { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        [Display(Name = "Fuel Consumption /hour")]
        public decimal FuelPerHour { get; set; }

        [Required]
        [Display(Name = "Fuel Cost /lit")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public decimal BirrPerLitter { get; set; }

        [Required]
        [Display(Name = "Fuel Consumption Cost")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public decimal FuelCostPerHr { get; set; }

        [Required]
        [Display(Name = "Equipment Cost /hour")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public decimal RentalWithFuel { get; set; }

        [Display(Name = "Approved Cost")]
        public decimal ApprovedCost { get; set; }
    }


    public class InternalMachinaryRent : TrackUserOperation
    {
        [Key]
        public int InternalMachinaryRentId { get; set; }

        [Required]
        [Display(Name = "Type of Equipment")]
        public Guid FleetsID { get; set; }
        //
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "Capacity (HP)")]
        public string Capacity { get; set; }

        [Display(Name = "Purchase cost")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public decimal PurchaseCost { get; set; }

        [Display(Name = "Maximum Life (years)")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public decimal MaximumLife { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        [Display(Name = "Hours Used Per Year")]
        public int HoursUsedPerYear { get; set; }

        [Display(Name = "Depereciation Cost")]
        public decimal DepCostPerHour { get; set; }

        [Display(Name = "Investment Cost")]
        public decimal InvestmentCost { get; set; }

        [Display(Name = "Total Owning Cost")]
        public decimal TotalOwningCost { get; set; }

        [Display(Name = "Fuel Cost")]
        public decimal FuelCost { get; set; }

        [Display(Name = "Lub. Cost")]
        public decimal LubCost { get; set; }

        [Display(Name = "Maint. & Repair")]
        public decimal MaintAndRepair { get; set; }

        [Display(Name = "Tyre Cost")]
        public decimal TyreCost { get; set; }

        [Display(Name = "Total Operating Cost")]
        public decimal TotalOperatingCost { get; set; }

        //[Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        //[Display(Name = "Rate /day")]
        //public decimal DailyRentalCost { get; set; }

        [Display(Name = "Rate /hour")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Index for Sever Condition")]
        public decimal IndexForSeverCondition { get; set; }

        [Display(Name = "Rate /hour (without fuel)")]
        public decimal RatePerHourWithoutFuel { get; set; }

        public string Remark { get; set; }

    }
    public class ProjectEstimationTransportCost : TrackUserOperation
    {
        //Input for BOQ [Transport Cost]  
        [Key]
        public int ProjectEstimationTransportCostId { get; set; }
        //[ Material transport cost]
        [Required]
        [StringLength(100)]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Unit Factory")]
        public decimal UnitFactory { get; set; }
        [Required]
        [Display(Name = "Unit")]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Required]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

    }
    public class InternalExternalEquipmentViewModel
    {
        public Guid AssetNumber { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public decimal? PurchaseCost { get; set; }
        public string Type { get; set; }
        public Guid EquipmentTypeID { get; set; }
    }
    public class MaterialViewModel
    {
        public int ProjectMaterialId { get; set; }
        public string Name { get; set; }
    }

    public class DeformedBar
    {
        public int DeformedBarId { get; set; }
        //public string ConcatnatedCode { get; set; }
        [Display(Name = "Type")]
        [Required]
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public decimal Diameter { get; set; }
        public decimal UnitWeight { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
    }

    public class ProjectCrew
    {
        public int ProjectCrewId { get; set; }
        public int WorkCategoryId { get; set; }
        public string CrewCode { get; set; }
        public string CrewLeader { get; set; }

        public decimal AmountofWorkDone { get; set; }
        public decimal TotalCrewCost { get; set; }

        public virtual WorkCategory WrorkType { get; set; }

        public virtual ICollection<CrewHumanResource> HumanResource { get; set; }
        public virtual ICollection<CrewEquipment> Equipments { get; set; }
        public virtual ICollection<CrewMaterial> Materials { get; set; }
    }

    public class CrewHumanResource
    {
        public int CrewHumanResourceId { get; set; }
        public int ProjectCrewId { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public int Quantity { get; set; }

        public virtual ProjectCrew Crew { get; set; }
    }
    public class CrewEquipment
    {
        public int CrewEquipmentId { get; set; }
        public int ProjectCrewId { get; set; }
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public int Quantity { get; set; }

        public virtual ProjectCrew Crew { get; set; }
    }
    public class CrewMaterial
    {
        public int CrewMaterialId { get; set; }
        public int ProjectCrewId { get; set; }
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid  Number")]
        public int Quantity { get; set; }

        public virtual ProjectCrew Crew { get; set; }
    }

    public class BarDiameter
    {
        public int BarDiameterId { get; set; }
        public string ConcatnatedCode { get; set; }
        public decimal Diameter { get; set; }
        public decimal UnitWeight { get; set; }
        public string Description { get; set; }
    }

    public class WorkItemDuration
    {
        public int WorkItemDurationId { get; set; }
        public int ProjectInformationId { get; set; }
        public int WorkItemPerProjectId { get; set; }
        //public int GenericWorkItemPerProjectId { get; set; }

        [Display(Name = "Item")]
        public string ItemCode { get; set; }
        [Display(Name = "Work Description")]
        public string WorkDescription { get; set; }
        //public int TakeOffPerProjectId { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }

        [Display(Name = "Productivity /Crew /Day")]
        public decimal ProductivityOfCrewPerDay { get; set; }
        [Display(Name = "Duration /Crew")]
        public decimal DurationPerCrew { get; set; }
        [Display(Name = "No. of Crews")]
        public int NumberOfCrew { get; set; }
        public decimal EstimatedDuration { get; set; }

        //public virtual WorkItemPerProject WorkItemPerProject { get; set; }
        public bool HasSubWorkItems { get; set; }
        public int Parent { get; set; }
        public DurationItemSource Source { get; set; }

        public string SubProjectTypeName { get; set; }
        public string BlockName { get; set; }
        public string StructureyType { get; set; }
        public string WorkCategory { get; set; }

        public decimal BudgetCost { get; set; }
        public decimal DirectCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LabourCost { get; set; }
        public decimal EquipmentCost { get; set; }
        public decimal ProfitCost { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal TotalUnitRate { get; set; }
        public int WorkCategoryId { get; set; }
        public int ConfigurationId { get; set; }
        public int BlockId { get; set; }
        public int SubProjectTypeId { get; set; }

        //public virtual GenericWorkItemPerProject GenericWorkItemPerProject { get; set; }
    }
    public class ResourceEstimationMaterial : TrackUserOperation
    {
        [Key]
        public int ResourceEstimationMaterialId { get; set; }
        public int WorkItemDurationId { get; set; }
        public virtual WorkItemDuration WorkItemDuration { get; set; }

        [Display(Name = "Category")]
        public string ItemCategoryCode { get; set; }
        [Display(Name = "Type Of Material")]
        public string ItemCode { get; set; }
        public virtual ExceedERP.Core.Domain.Inventory.InventoryItem Item { get; set; }

        public string Unit { get; set; }

        [Display(Name = "Qty/Unit")]
        public decimal QuantityPerUnit { get; set; }
        [Display(Name ="Tot. Qty")]
        public decimal TotalQuantity { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

    }
    public class ResourceEstimationEquipment : TrackUserOperation
    {
        [Key]
        public int ResourceEstimationEquipmentId { get; set; }
        public int WorkItemDurationId { get; set; }
        public virtual WorkItemDuration WorkItemDuration { get; set; }

        [Display(Name = "Equipment Name")]
        public Guid FleetsID { get; set; }
        public virtual Fleets Fleet { get; set; }

        [Display(Name = "No.")]
        public decimal Number { get; set; }

        [Display(Name = "U.F.")]
        public decimal UnitFactor { get; set; }

        // = UF * No.
        [Display(Name = "UF*No/Crew")]
        public decimal UnitFactorAndNumberPerCrew { get; set; }

        // = No. of Crew * U.F. * No.
        [Display(Name = "No/day")]
        public decimal NumberPerDay { get; set; }

        [Display(Name = "Hourly Rental Cost")]
        public decimal HourlyCost { get; set; }

        // 8 * hourlyCost * NumberPerDay
        [Display(Name = "Total Cost")]
        public decimal TotalCostPerDay { get; set; }
    }
    public class ResourceEstimationLabour : TrackUserOperation
    {
        [Key]
        public int ResourceEstimationLabourId { get; set; }
        public int WorkItemDurationId { get; set; }
        public virtual WorkItemDuration WorkItemDuration { get; set; }

        [Display(Name = "Labour Title")]
        public int ProjectEstimationIndexLabourId { get; set; }
        public virtual ProjectEstimationIndexLabour ProjectEstimationIndexLabour { get; set; }
       
        [Display(Name = "No.")]
        public decimal Number { get; set; }

        [Display(Name = "U.F.")]
        public decimal UnitFactor { get; set; }

        // = UF * No.
        [Display(Name ="UF*No/Crew")]
        public decimal UnitFactorAndNumberPerCrew { get; set; }

        // = No. of Crew * U.F. * No.
        [Display(Name ="No/day")]
        public decimal NumberPerDay { get; set; }

        // = 8 * IndexedHourlyCost * NumberPerDay
        [Display(Name = "Total Cost/day")]
        public decimal TotalCostPerDay { get; set; }

        [Display(Name = "Indexed hrly sal.")]
        public decimal IndexedHourlySalary { get; set; }
    }
}