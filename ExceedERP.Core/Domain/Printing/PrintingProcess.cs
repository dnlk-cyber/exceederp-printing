using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingProcess : TrackUserSettingOperation
    {
        [Key]
        public int PrintingProcessId { get; set; }
        public PrintingProcessCategory PrintingCategory { get; set; }
        public PrintingProcessName PrintingProcessName { get; set; }
        public decimal EstProductionPerHour { get; set; } // est. prod. target per hour
        public decimal LaborRate { get; set; } // chargeable hours rate in birr
        // not useful for now
        public string ProcessName { get; set; }
        public double HoursRequired { get; set; }
        public int Quantity { get; set; }
        public string ProcessScope { get; set; }
        public double SetupCost { get; set; }
        public double CostPerItem { get; set; }
        public string Remark { get; set; }         
    }

    public enum PrintingProcessCategory
    {
        NA,
        Pre_Printing,
        Printing,
        Finishing
    }
    public enum PrintingProcessName
    {
        NA,
        PlateMaking,
        PaperCutting,
        CoverPrinting,
        TextPrinting,
        CoverPlateChange,
        TextPlateChange,
        CoverColorChange,
        TextColorChange,
        Folding_FourPage_Sec,
        Collating_Manual_Gatering,
        Stitching_Manual_Side_Saddle, 
        Gathering,
        Thread_Sewing,
        GlueBinding,
        Trimming,
        Packing_Wrapping,
        Dispatching_Loading_Forklifting
        
    }
}
