using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.PrintingEstimation
{
    public class LaborCost: TrackUserSettingOperation
    {
        public LaborCost()
        {
            this.DateCreated = DateTime.UtcNow;
            this.LastModified = DateTime.UtcNow;
        }
        public int LaborCostId { get; set; }
        [Required]
        public PrintingSteps PrintingSteps { get; set; }
        public int EstimationFormId { get; set; }
        public EstimationForm EstimationForm { get; set; }
        public int PrintingStepId { get; set; }
        public PrintingStep PrintingStep { get; set; }
        [Required]
        public decimal WorkingTime { get; set; }

        [Required]
        public decimal CostPerHour { get; set; }

        [ReadOnly(true)]
        public decimal TotalCost { get; set; }
    }
}
