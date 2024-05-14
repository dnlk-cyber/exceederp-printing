using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingMachineList : TrackUserSettingOperation
    {
        [Key]
        public int PrintingMachineListId { get; set; }
        public string MachineTypeCode { get; set; }
        public int PrintingCostCenterId { get; set; }
        public string PaperFeedStyle { get; set; }
        public int NumberUnits { get; set; }
        public string MachineTypeName { get; set; }
        public decimal PrintingHourlyChargeRate { get; set; }
        public decimal MakereadyHours { get; set; }
        public string MachineStatus { get; set; }
        public int NumberOfColors { get; set; }
        public double OutputRate { get; set; }
        public string Remark { get; set; }

    }
}
