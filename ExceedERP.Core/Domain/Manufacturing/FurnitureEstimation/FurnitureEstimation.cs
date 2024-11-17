using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
   public class FurnitureEstimationForm : Operations
    {
        [Key]
        [Display(Name ="Estimation No")]
        public int FurnitureEstimationId { get; set;}
        public int FurnitureBillOfQuantityId { get; set; }
        public int CustomerId { get; set; }
        [Required]
        [Display(Name = "Type of Job ")]
        public int JobTypeId { get; set; }
        
        [Required]
        public decimal Quantity { get; set; }
        public string Description { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsMarginAssigned { get; set; }
        public bool JobCreated { get; set; }

    }
    
    public class DirectLaborCost : TrackUserSettingOperation
    {
        
        [Key]
        public int DirectLaborCostId { get; set; }
       
        public int FurnitureEstimationId { get; set; }
       
        [Required]
        [Display(Name = "Total Working Time")]
        public decimal WorkingTime { get; set; }
        [ReadOnly(true)]
        public decimal CostPerHour { get; set; }
        [ReadOnly(true)]
        public decimal TotalCost { get; set; }
        [Display(Name ="Task Category")]
        public int TaskCategoryId { get; set; }
        [Display(Name ="Task Type")]
        public WorkShop TaskType { get; set; }
    }

   
    public class FurnitureOverHeadCost
    {
        public int FurnitureOverHeadCostId { get; set; }
        public decimal OverHeadCost { get; set; }
        public int FurnitureEstimationId { get; set; }
        
    }
}
