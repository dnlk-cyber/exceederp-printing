using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingMachineType : TrackUserSettingOperation
    {
        [Key]
        public int PrintingMachineTypeId { get; set; }
        public string MachineTypeName { get; set; }
        public int NumberOfColors { get; set; }
        public double OutputRate { get; set; }
        public string PaperFeedStyle { get; set; }
        public int NumberUnits { get; set; }
       
        public decimal PrintingHourlyChargeRate { get; set; }
        public decimal MakeReadyHours { get; set; }
        public string MachineStatus { get; set; }
       
        public string Remark { get; set; }

        // Fk
        public int PrintingCostCenterId { get; set; }

    }
}
