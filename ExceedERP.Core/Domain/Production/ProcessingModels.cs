using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Production
{
    public class ProcessingPlant : TrackUserOperation
    {
        [Key]
        public int ProcessingPlantId { get; set; }

        [Display(Name = "Location")]
        public int BranchId { get; set; }
        public ProcessingPlantType PlantType { get; set; }
        public string Name { get; set; }

        //public Branch Branch { get; set; }
    }
    public class ProcessingOrderBase : TrackUserOperation
    {
        [Display(Name = "Processing Plant")]
        public int ProcessingPlantId { get; set; }
        public virtual ProcessingPlant ProcessingPlant { get; set; }
        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }
        public virtual ProductionPeriod ProductionPeriod { get; set; }
    }
    public class SeedProcessingOrder : ProcessingOrderBase
    {
        [Key]
        public int SeedProcessingOrderId { get; set; }
    }
    public class GumProcessingOrder : ProcessingOrderBase
    {
        [Key]
        public int GumProcessingOrderId { get; set; }

    }
    public class ProcessingLineItemBase : TrackUserOperation
    {
        //[Required]
        //[Display(Name = "Store")]
        //public string StoreCode { get; set; }
        //[Required(ErrorMessage ="Issue Reference is Required")]
        ////[Display(Name ="Issue Reference")]
        ////public int ProductionIssueId { get; set; }
        [Display(Name = "Item Category")]
        public string ItemCategoryCode { get; set; }
        public ItemCategory ItemCategory { get; set; }
        [Display(Name = "Item")]
        public string ItemCode { get; set; }
        public InventoryItem InventoryItem { get; set; }


        [Display(Name = "Code")]
        public string ProcessingCode { get; set; }

        // inputs
        [Display(Name ="Production Cost")]
        public decimal TotalProductionCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        [Display(Name = "Machinery Cost")]
        public decimal EquipmentCost { get; set; }
        public decimal DirectCost { get; set; }
        public decimal OverheadCost { get; set; }
        //public decimal ProfitCost { get; set; }
        public decimal TotalCost { get; set; }

        // output produced
        [Display(Name = "Raw Quantity")]
        public decimal RawQuantity { get; set; }

        [Display(Name = "Recovery Rate")]
        public decimal RecoveryRate { get; set; }

        [Display(Name = "Cleaned Qty")]
        public decimal QuantityProduced { get; set; }
        [Display(Name = "UoM")]
        public string UnitOfOutPutProduced { get; set; }
    }
    public class SeedProcessingOrderItem : ProcessingLineItemBase
    {
        [Key]
        public int SeedProcessingOrderItemId { get; set; }

        public int SeedProcessingOrderId { get; set; }
        public virtual SeedProcessingOrder SeedProcessingOrder { get; set; }

        //[Display(Name ="Production Order")]
        //public int SeedProductionOrderItemId { get; set; }
        //public virtual SeedProductionOrderItem SeedProductionOrderItem { get; set; }

        //[Display(Name ="Processing Plant")]
        //public int ProcessingPlantId { get; set; }
        //public virtual ProcessingPlant ProcessingPlant { get; set; }

        //[Display(Name ="Seed")]
        //public string SeedName { get; set; }

        #region 
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name = "From")]
        //public DateTime DateFrom { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name = "To")]
        //public DateTime DateTo { get; set; }

        #endregion
    }
    public class GumProcessingOrderItem : ProcessingLineItemBase
    {
        [Key]
        public int GumProcessingOrderItemId { get; set; }
        public int GumProcessingOrderId { get; set; }
        public virtual GumProcessingOrder GumProcessingOrder { get; set; }
        //[Display(Name = "Production Order")]
        //public int GumProductionOrderItemId { get; set; }
        //public virtual GumProductionOrderItem GumProductionOrderItem { get; set; }

        //[Display(Name ="Processing Plant")]
        //public int ProcessingPlantId { get; set; }
        //public virtual ProcessingPlant ProcessingPlant { get; set; }

        //[Display(Name ="Product Type")]
        //public string GumName { get; set; }

        #region 
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name = "From")]
        //public DateTime DateFrom { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name = "To")]
        //public DateTime DateTo { get; set; }

        //[Display(Name = "Code")]
        //public string ProcessingCode { get; set; }

        //// inputs
        //public decimal MaterialCost { get; set; }
        //public decimal LaborCost { get; set; }
        //[Display(Name ="Machinery Cost")]
        //public decimal EquipmentCost { get; set; }
        //public decimal DirectCost { get; set; }
        //public decimal OverheadCost { get; set; }
        ////public decimal ProfitCost { get; set; }
        //public decimal TotalCost { get; set; }

        //// output produced

        //[Display(Name = "Raw Quantity")]
        //public decimal RawQuantity { get; set; }

        //[Display(Name = "Recovery Rate")]
        //public decimal RecoveryRate { get; set; }

        //[Display(Name = "Cleaned Qty")]
        //public decimal QuantityProduced { get; set; }
        //[Display(Name = "UoM")]
        //public string UnitOfOutPutProduced { get; set; } 
        #endregion
    }
    public class ProcessingOrderActivity : TrackUserOperation
    {
        [Key]
        public int ProcessingOrderActivityId { get; set; }

        public int GumProcessingOrderItemId { get; set; }
        public int SeedProcessingOrderItemId { get; set; }


        #region MyRegion
        [Display(Name = "Activity")]
        public int ProductionActivityId { get; set; }
        public OperationType OperationType { get; set; }
        [Display(Name = "No.")]
        public int Quantity { get; set; }
        //public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal EquipmentCost { get; set; }
        public decimal TotalCost { get; set; }

        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Information")]
        public DateTime DateOfInformation { get; set; }
        //public string BugdetYear { get; set; }
        [Display(Name = "Document Ref.")]
        public string Reference { get; set; }
        public virtual ProductionActivity Activity { get; set; }
        #endregion
    }
    public class ProcessingInputMaterial : TrackUserOperation
    {
        [Key]
        public int ProcessingInputMaterialId { get; set; }

        public int GumProcessingOrderItemId { get; set; }
        public int SeedProcessingOrderItemId { get; set; }

        #region 
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
        //public string BugdetYear { get; set; }
        [Display(Name = "Document Ref.")]
        public string Reference { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        #endregion
    }
}
