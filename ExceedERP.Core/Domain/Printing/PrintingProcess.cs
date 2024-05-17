using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingProcess : TrackUserSettingOperation
    {
        [Key]
        public int PrintingProcessId { get; set; }
        public PrintingProcessCategory PrintingCategory { get; set; }
        
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
        Pre_Printing,
        Printing,
        Finishing
    }
}
