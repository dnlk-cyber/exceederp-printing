using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing
{
    public enum JobTypeCategory
    {
        Standard,
        [Display(Name ="Custom-Made")]
        Custom_Made
    }
    public class FurnitureBillOfQuantity : Operations
    {
        [Key]
        public int FurnitureBillOfQuantityId { get; set; }
        public bool IsCustomizedJob { get; set; }
        [Display(Name ="JobType Category")]
        public JobTypeCategory JobTypeCategory { get; set; }

        // If JobCategory == Customized
        [Display(Name ="Job.Code")]
        public string JobNo { get; set; }
        [Display(Name ="Type Of Job")]
        public string JobTypeName { get; set; }
        public string Unit { get; set; }
        
        // else

        [Display(Name ="Type of Job")]
        public int? JobTypeId { get; set; }
        [Display(Name ="Customer")]
        public int CustomerId { get; set; }
        public decimal Quantity { get; set; }      
        public string Size { get; set; }


    }
    public class FurnitureBOQMaterial : TrackUserSettingOperation
    {
        [Key]
        public int FurnitureBOQMaterialId { get; set; }

        public int ManufacturingMaterialCategoryId { get; set; }
        public int ManufacturingMaterialCategoryItemId { get; set; }
        [Display(Name ="Item")]
        public string ItemCode { get; set; }
        [Display(Name ="Unit of measurement")]
        public string UnitOfMeasurement { get; set; }
        public decimal Quantity { get; set; }
        public string Remark { get; set; }
        [Display(Name ="Work shop")]
        public WorkShop WorkshopType { get; set; }

        public int FurnitureBillOfQuantityId { get; set; }
        public FurnitureBillOfQuantity FurnitureBillOfQuantity { get; set; }
    }
    public class FurnitureBOQLabor : TrackUserSettingOperation
    {
        [Key]
        public int FurnitureBOQLaborId { get; set; }
        [Display(Name ="Task Category")]
        public int ManufacturingTaskCategoryId { get; set; }
        [Display(Name ="Work Shop")]
        public WorkShop WorkShopType { get; set; }
        [Display(Name ="Number of Employees")]
        public int NumberOfEmloyees { get; set; }
        [Display(Name = "Estimated Minute")]
        public int EstimatedMinute { get; set; }
        

        public int FurnitureBillOfQuantityId { get; set; }
        public FurnitureBillOfQuantity FurnitureBillOfQuantity { get; set; }
    }

}
