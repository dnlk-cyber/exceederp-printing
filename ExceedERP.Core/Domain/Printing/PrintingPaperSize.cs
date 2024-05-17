using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingPaperSize
    {
        public int PrintingPaperSizeId { get; set; }
        public string PaperSizeName { get; set; }
        public int NoOfPagesPerSheet { get; set; } 
        public int NoOfPagesPerPlate { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double NoOfPagesToA1Ratio { get; set; }
        public string Remark { get; set; }

        //Fk
        public int PrintingMachineTypeId { get; set; }

    }
}
