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
    public class FurnitureBillOfQuantity : Operations
    {
        [Key]
        public int FurnitureBillOfQuantityId { get; set; }
        [Display(Name ="Product Name")]
        public int JobTypeId { get; set; }
        [Display(Name ="Customer")]
        public int CustomerId { get; set; }
        public decimal Quantity { get; set; }      
        public string Size { get; set; }

    }
    public class FurnitureBOQMaterial : TrackUserSettingOperation
    {
        [Key]
        public int FurnitureBOQMaterialId { get; set; }
        [Display(Name ="Item")]
        public int ItemCode { get; set; }
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
        public WorkShop WorkShopType { get; set; }
        public int NumberOfEmloyees { get; set; }
        public float EstimatedHour { get; set; }

        public int FurnitureBillOfQuantityId { get; set; }
        public FurnitureBillOfQuantity FurnitureBillOfQuantity { get; set; }
    }

}
