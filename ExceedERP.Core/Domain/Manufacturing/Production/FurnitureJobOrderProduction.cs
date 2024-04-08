using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.Production
{
    public class FurnitureJobOrderProduction : Operations
    {
        [Key]
        public int FurnitureJobOrderProductionId { get; set; }
        [Display(Name ="Customer")]
        public int CustomerId { get; set; }
        [Display(Name ="Job Name")]
        public int JobTypeId { get; set; }
        [Display(Name ="Job.No")]
        public string JobNo { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; } 
        [Display(Name = "Delivery Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        public int FurnitureJobOrderFormId { get; set; }
        public bool IsSendForFinishing { get; set; }
        public string SendForFinishingBy { get; set; }
        public DateTime? SendForFinishingDate { get; set; } 
        public bool IsClosed { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? DateClosed { get; set; } 

    }
    public class FurnitureProductionBillOfMaterial
    {
        public int FurnitureProductionBillOfMaterialId { get; set; }
        [Display(Name = "Material Category")]
        public int ManufacturingMaterialCategoryId { get; set; }
        [Display(Name = "Item Name")]
        public int ManufacturingMaterialCategoryItemId { get; set; }
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }
        [Display(Name = "Unit of measurement")]
        public string UnitOfMeasurement { get; set; }
        public decimal Quantity { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Work shop")]
        public WorkShop WorkshopType { get; set; }

        public int FurnitureJobOrderProductionId { get; set; }
        public FurnitureJobOrderProduction FurnitureJobOrderProduction { get; set; }
    }
    public class FurnitureProductionBillOfLabor
    {
        public int FurnitureProductionBillOfLaborId { get; set; }
        [Display(Name = "Task Category")]
        public int ManufacturingTaskCategoryId { get; set; }
        [Display(Name = "Work Shop")]
        public WorkShop WorkShopType { get; set; }
        [Display(Name = "Number of Employees")]
        public int NumberOfEmloyees { get; set; }
        [Display(Name = "Estimated Minute")]
        public int EstimatedMinute { get; set; }
        public int FurnitureJobOrderProductionId { get; set; }
        public FurnitureJobOrderProduction FurnitureJobOrderProduction { get; set; }
    }
}
