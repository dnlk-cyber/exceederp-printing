using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingPaperSize
    {
        [Key]
        public int PrintingPaperSizeId { get; set; }
        public string PaperSizeName { get; set; }
        public PrintPaperSize PrintPaperSize { get; set; } 
        public double Width { get; set; }
        public double Length { get; set; }
        public int NoOfPagesToA1Ratio { get; set; }
        public string Remark { get; set; }

    }
    public enum PrintPaperSize
    {
        A1,
        A2,
        A3,
        A4,
        A5,
        A6,
        B1,
        B2,
        B3,
        B4,
        B5,
        B6
    }
}
