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
        public PrintingProcessName PrintingProcessName { get; set; }

        [Display(Name ="Process Name")]
        public int? PrintingProcessId { get; set; }
        [Display(Name ="Machine Type")]
        public int? PrintingMachineTypeId { get; set; }
        public decimal Quantity { get; set; } // quantity to be produced (a)
        public decimal EstProductionPerHour { get; set; } // est. prod. target per hour (c)
        [Display(Name ="Estimated Hour")]
        public decimal EstimatedHours { get; set; } // Quantity/EstProductionPerHour (d = a/c)
        [Display(Name ="Labor Rate")]
        public decimal LaborRate { get; set; } // chargeable hours rate in birr (e)
        [Display(Name ="Total Cost")]
        public decimal TotalCost { get; set; } // EstimatedHours*LaborRate (f = d*e)

        //
        
        //fk
        public int PrintingCostEstimationId { get; set; }

    }
}
