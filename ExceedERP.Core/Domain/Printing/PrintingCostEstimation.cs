using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingCostEstimation : TrackUserSettingOperation
    {
        [Key]
        public int PrintingCostEstimationId { get; set; }
        public int PaperSizeId { get; set; }
        public int PaperTypeId { get; set; }
        public int ProductQuantity { get; set; }
        public int TotalTextNoPages { get; set; }
        public int BindingStyleId { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public int TextNoOfColors { get; set; }
        public int CoverNoOfColor { get; set; }
        public double CoverPaperSize { get; set; }
        public bool CoverRequired { get; set; }
        public bool OnlyOneSideCover { get; set; }
        public int NoUpPagesPerA1 { get; set; }
        public int ProcessCategoryId { get; set; }
        public int TypeOfWorkId { get; set; }
        public double EstimatedHrs { get; set; }
        public double DLaborRate { get; set; }
        public decimal TotalCost { get; set; }
        public double OverHeadRate { get; set; }
        public decimal OverHeadCost { get; set; }
        public decimal TotalLaborCost { get; set; }


    }
}
