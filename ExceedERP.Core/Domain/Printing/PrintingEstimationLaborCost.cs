using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingEstimationLaborCost : TrackUserSettingOperation
    {
        [Key]
        public int PrintingEstimationLaborCostId { get; set; }
        [Display(Name ="Process Category")]
        public PressCategory PressCategory { get; set; }
        public LabourActivity LabourActivity { get; set; }
        [Display(Name ="Process Name")] 
        public int? PrintingProcessId { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; } // quantity to be produced (a)       
        [Display(Name ="Labor Rate")]
        public decimal LaborRate { get; set; } // chargeable hours rate in birr (e)
        [Display(Name ="Total Cost")]
        public decimal TotalCost { get; set; } // EstimatedHours*LaborRate (f = d*e)
        
        //fk
        public int PrintingCostEstimationId { get; set; }

    }
}
