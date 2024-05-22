using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintMachinePaperList : TrackUserSettingOperation
    {
        [Key]
        public int PrintMachinePaperListId { get; set; }
        public int PrintMachineTypeId { get; set; }
        public int PrintingPaperSizeId { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }       
        public int NumberOfPagesPerSheet { get; set; }
        public int NumberOfPagesPerPlate { get; set; }


    }
}
