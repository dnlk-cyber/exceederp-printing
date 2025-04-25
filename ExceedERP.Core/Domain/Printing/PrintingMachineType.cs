using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingMachineType : TrackUserSettingOperation
    {
        [Key]
        public int PrintingMachineTypeId { get; set; }
        [Display(Name ="Machine Name"), Required]
        public string MachineTypeName { get; set; }
        [Display(Name = "Paper Size"), Required]
        public int PaperSizeId { get; set; }
        public int NumberOfColors { get; set; }
        [Display(Name = "Output Rate"), Required]
        public double OutputRate { get; set; }
        public int PrintSide { get; set; }
        //public decimal PrintingRate { get; set; } //
        //public decimal MakeReadyRate { get; set; } // Remove if you put the rate on the LaborActivity

    }
    public class PrintingMachineDetail
    {
        [Key]
        public int PrintingMachineDetailId { get; set; }
        [Display(Name ="Paper Size"), Required]
        public int PaperSizeId { get; set; }
        [Display(Name ="Section/Up"), Required]
        public int SectionUp { get; set; }
        public int PrintingMachineTypeId { get; set; }

    }
}
