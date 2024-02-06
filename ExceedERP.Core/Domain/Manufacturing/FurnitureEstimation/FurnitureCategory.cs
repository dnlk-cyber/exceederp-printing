using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureCategory: TrackUserSettingOperation
    {
        public int FurnitureCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Value { get; set; }
        [Display(Name= "Standard Working Time")]
        [Required]
        public decimal WorkingTime { get; set; }
        [Display(Name = "Standard Cost/Hr")]
        [Required]
        public decimal CostPerHour { get; set; }
    }
}
