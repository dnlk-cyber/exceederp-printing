using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingEstimationLaborCost : TrackUserSettingOperation
    {
        [Key]
        public int PrintingEstimationLaborCostId { get; set; }
        [Display(Name ="Process Category")]
        public PrintingProcessCategory ProcessCategory { get; set; }
        [Display(Name ="Process Name")]
        public int? PrintingProcessId { get; set; }
        [Display(Name ="Machine Type")]
        public int? PrintingMachineTypeId { get; set; }
        [Display(Name ="Estimated Hour")]
        public decimal EstimatedHours { get; set; }
        [Display(Name ="Labor Rate")]
        public decimal LaborRate { get; set; }
        [Display(Name ="Total Cost")]
        public decimal TotalCost { get; set; }
        //fk
        public int PrintingCostEstimationId { get; set; }

    }
}
