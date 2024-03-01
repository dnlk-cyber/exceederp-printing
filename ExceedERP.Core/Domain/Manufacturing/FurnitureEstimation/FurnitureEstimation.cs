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
   public class FurnitureEstimationForm : TrackUserSettingOperation
    {
        [Key]
        public int FurnitureEstimationId { get; set;}
        public int CustomerId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Type of Job ")]
        public int  JobTypeId { get; set; }
        public string  JT { get; set; }
        [Required]
        public int JobNumber { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        public string Description { get; set; }
        public string Others { get; set; }
        public bool IsChecked { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsApproved { get; set; }
        [NotMapped]
        public bool IsCalculated { get; set; }

        public object EstimationId { get; set; }
    }
    
    public class DirectLaborCost : TrackUserSettingOperation
    {
        public DirectLaborCost()
        {
            this.DateCreated = DateTime.UtcNow;
            this.LastModified = DateTime.UtcNow;
        }
        [Key]
        public int DirectLaborCostId { get; set; }
        [Required]
        [Display(Name ="Category")]
        public int FurnitureCategoryId { get; set; }
        
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
        public ManifucturingTaskType TaskType { get; set; }
    }

   
    public class FurnitureOverHeadCost
    {
        public int FurnitureOverHeadCostId { get; set; }
        public decimal OverHeadCost { get; set; }
        public int FurnitureEstimationId { get; set; }
        
    }
}
