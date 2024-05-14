using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintMachinePaperList : TrackUserSettingOperation
    {
        [Key]
        public int PrintMachinePaperListId { get; set; }
        public int PrintMachineTypeId { get; set; }
        public string PaperSizeCode { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public string PaperName { get; set; }
        public int NumberOfPagesPerSheet { get; set; }
        public int NumberOfPagesPerPlate { get; set; }


    }
}
