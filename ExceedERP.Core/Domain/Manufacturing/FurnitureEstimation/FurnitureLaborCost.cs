using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureLaborCost : TrackUserSettingOperation
    {
        public FurnitureLaborCost()
        {
            this.DateCreated = DateTime.UtcNow;
            this.LastModified = DateTime.UtcNow;
        }
        [Key]
        public int FurnitureLaborCostId { get; set; }
        [Required]
        public FurnitureSteps FurnitureSteps { get; set; }
        public int EstimationFormId { get; set; }
        public FurnitureEstimationForm EstimationForm { get; set; }
        public int FurnitureStepId { get; set; }
        public FurnitureStep FurnitureStep { get; set; }
        [Required]
        public decimal WorkingTime { get; set; }

        [Required]
        public decimal CostPerHour { get; set; }

        [ReadOnly(true)]
        public decimal TotalCost { get; set; }
    }
    //public enum FurnitureSteps
    //{
    //    PrePrint,
    //    Printing,
    //    Finishing

    //}
}
