using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Production
{
    public class ProductionProject : TrackUserOperation
    {
        [Key]
        public int ProductionProjectId { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }
    }
    public class ProductionSite : TrackUserOperation
    {
        [Key]
        public int ProductionSiteId { get; set; }
        [Display(Name = "Site Name")]
        public string Name { get; set; }
        [Display(Name = "Location/Branch")]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }

    }
    public class Plot
    {
        [Key]
        public int PlotId { get; set; }
        [Display(Name = "Field/block no.")]
        public string PlotCode { get; set; }
        //[Display(Name = "Width(m)")]
        //public decimal Width { get; set; }
        //[Display(Name = "Length(m)")]
        //public decimal Length { get; set; }
        [Display(Name = "Area(ha)")]
        public decimal Area { get; set; }

        [Display(Name = "Site")]
        public int ProductionSiteId { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Status")]
        public int ProductionSettingId { get; set; }
        [Display(Name ="Isolation Distance (m)")]
        public decimal IsolationDistance { get; set; }
        //public virtual ProductionProject Project { get; set; }
    }
    public class ProductionOrder : TrackUserOperation
    {
        [Key]
        public int ProductionOrderId { get; set; }
        public ProductionOrderType ProductionType { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [Display(Name = "Site")]
        public int ProductionSiteId { get; set; }
        //public virtual Branch Branch { get; set; }
        //[Display(Name ="Plot or land")]
        //public int PlotId { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name ="From")]
        //public DateTime  DateFrom { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name ="To")]
        //public DateTime DateTo { get; set; }

        //[Display(Name ="Code")]
        //public string ProductionCode { get; set; }
        public string Description { get; set; }

        //[Display(Name = "Crop/Seed")]
        //public int CropId { get; set; }

        //[Display(Name = "Seed Type")]
        //public CropType SeedType { get; set; }

        //[Display(Name = "Seed Generation")]
        //public CropGeneration Generation { get; set; }
    }
    public class SeedProductionOrderItem : TrackUserOperation
    {
        [Key]
        public int SeedProductionOrderItemId { get; set; }

        public int ProductionOrderId { get; set; }


        [Display(Name = "Plot or land")]
        public int? PlotId { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name = "From")]
        //public DateTime DateFrom { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name = "To")]
        //public DateTime DateTo { get; set; }

        [Display(Name = "Production Year")]
        public int ProductionYearId { get; set; }


        [Display(Name = "Code")]
        public string ProductionCode { get; set; }
        public string Description { get; set; }
        public ProductStatus Status { get; set; }
        // own farm vs. contractual production
        public SeedProductionType ProductionType { get; set; }
        [Display(Name = "Crop/Seed")]
        public int CropId { get; set; }

        //[Display(Name = "Seed Type")]
        //public CropType SeedType { get; set; }

        //[Display(Name = "Seed Generation")]
        //public CropGeneration Generation { get; set; }

        // if its purchased from other producers
        public decimal PurchaseCost { get; set; }

        // inputs
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        [Display(Name ="Machinery Cost")]
        public decimal EquipmentCost { get; set; }
        public decimal DirectCost { get; set; }
        public decimal OverheadCost { get; set; }
        //public decimal ProfitCost { get; set; }
        public decimal TotalCost { get; set; }

        // output produced
        [Display(Name = "Qty of Raw Seed Produced")]
        public decimal QuantityProduced { get; set; }
        [Display(Name = "UoM")]
        public string UnitOfOutPutProduced { get; set; }
        public virtual ProductionOrder ProductionOrder { get; set; }
    }
    public class SeedPurchasePriceViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Purchased Qty")]
        public decimal PurchasedQuantity { get; set; }
        [Display(Name = "Total Purchase Cost")]
        public decimal PurchaseCost { get; set; }
        public decimal TotalCost { get; set; }
    }
    public class GumProductionOrderItem : TrackUserOperation
    {
        [Key]
        public int GumProductionOrderItemId { get; set; }

        public int ProductionOrderId { get; set; }

        //public string ProductType { get; set; }
        public GumProductionType ProductionType { get; set; }

        [Display(Name = "Production Year")]
        public int ProductionYearId { get; set; }

        [Display(Name = "Code/Ref. no. ")]
        public string ProductionCode { get; set; }
        [Display(Name = "Product Type")]
        public int NaturalGumId { get; set; }
        public ProductStatus Status { get; set; }

        // if its purchased from other producers
        public decimal PurchaseCost { get; set; }

        // inputs
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        [Display(Name ="Machinery Cost")]
        public decimal EquipmentCost { get; set; }
        public decimal DirectCost { get; set; }
        public decimal OverheadCost { get; set; }
        //public decimal ProfitCost { get; set; }
        public decimal TotalCost { get; set; }

        // outputs produced
        [Display(Name = "Qty")]
        public decimal QuantityProduced { get; set; }
        [Display(Name = "UoM")]
        public string UnitOfOutPutProduced { get; set; }
        public virtual ProductionOrder ProductionOrder { get; set; }

    }
    public class GumPurchasePriceViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Purchased Qty")]
        public decimal PurchasedQuantity { get; set; }
        [Display(Name = "Total Purchase Cost")]
        public decimal PurchaseCost { get; set; }
        public decimal TotalCost { get; set; }
    }
    public class ProductionOrderActivitiy : TrackUserOperation
    {
        [Key]
        public int ProductionOrderActivitiyId { get; set; }

        public int SeedProductionOrderItemId { get; set; }
        public int GumProductionOrderItemId { get; set; }

        [Display(Name = "Activity")]
        public int ProductionActivityId { get; set; }
        public OperationType OperationType { get; set; }
        [Display(Name ="No.")]
        public int Quantity { get; set; }
        //public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        [Display(Name ="Machinery Cost")]
        public decimal EquipmentCost { get; set; }
        public decimal TotalCost { get; set; }

        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Information")]
        public DateTime DateOfInformation { get; set; }
        [Display(Name ="Document Ref.")]
        public string Reference { get; set; }
        //public string BugdetYear { get; set; }
        public virtual ProductionActivity Activity { get; set; }
    }
    public class ProductionRawProductPurchase : TrackUserOperation
    {
        [Key]
        public int ProductionRawProductPurchaseId { get; set; }

        public int SeedProductionOrderItemId { get; set; }
        public int GumProductionOrderItemId { get; set; }

        [Display(Name ="Supplier")]
        public string SupplierName { get; set; }
        [Display(Name ="Quantity")]
        public decimal PurchasedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class MachineryMaintenance : TrackUserOperation
    {
        [Key]
        public int MachineryMaintenanceId { get; set; }

        public int SeedProductionOrderItemId { get; set; }
        public int GumProductionOrderItemId { get; set; }

        public string MachineryDescription { get; set; }
        public string ActivityPerformed { get; set; }
        public decimal TotalCost { get; set; }

        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Information")]
        public DateTime DateOfInformation { get; set; }
        [Display(Name = "Document Ref.")]
        public string Reference { get; set; }
        //public string BugdetYear { get; set; }
    }
    public class ProductionOrderInputMaterial : TrackUserOperation
    {
        [Key]
        public int ProductionOrderInputMaterialId { get; set; }

        public int SeedProductionOrderItemId { get; set; }
        public int GumProductionOrderItemId { get; set; }

        [Display(Name = "Material Category")]
        public string ItemCategoryCode { get; set; }
        [Display(Name = "Material Type")]
        public string ItemCode { get; set; }

        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }

        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Information")]
        public DateTime DateOfInformation { get; set; }
        [Display(Name = "Document Ref.")]
        public string Reference { get; set; }
        //public string BugdetYear { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
    }

}
