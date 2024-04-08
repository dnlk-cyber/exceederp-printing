using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.Setting
{
    public class FurnitureStandardJobType : TrackUserSettingOperation
    {
        public int FurnitureStandardJobTypeId { get; set; }
        public bool IsStandard { get; set; }
        [Display(Name ="Job-Code")]
        public string JobCode { get; set; } 
        [Display(Name ="Job Title")]
        public string JobTypeName { get; set; }
        public string Unit { get; set; }
        public string Remark { get; set; }

    }

    public class StandardBillOfMaterial : TrackUserSettingOperation
    {
        public int StandardBillOfMaterialId { get; set; }
        
        [Display(Name ="Material Category")]
        public int ManufacturingMaterialCategoryId { get; set; }
        [Display(Name ="Item Name")]
        public int ManufacturingMaterialCategoryItemId { get; set; }
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }
        [Display(Name = "Unit of measurement")]
        public string UnitOfMeasurement { get; set; }
        public decimal Quantity { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Work shop")]
        public WorkShop WorkshopType { get; set; }

        public int FurnitureStandardJobTypeId { get; set; }
        public FurnitureStandardJobType FurnitureStandardJobType { get; set; }
    }
    
    public class StandardBillOfLabor : TrackUserSettingOperation
    {
        public int StandardBillOfLaborId { get; set; }

        [Display(Name = "Task Category")]
        public int ManufacturingTaskCategoryId { get; set; }
        [Display(Name = "Work Shop")]
        public WorkShop WorkShopType { get; set; }
        [Display(Name = "Number of Employees")]
        public int NumberOfEmloyees { get; set; }
        [Display(Name = "Estimated Minute")]
        public int EstimatedMinute { get; set; }

        public int FurnitureStandardJobTypeId { get; set; } 
        public FurnitureStandardJobType FurnitureStandardJobType { get; set; }
    }
}
