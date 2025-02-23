using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingCostEstimation : Operations
    {
        [Key]
        public int PrintingCostEstimationId { get; set; }
        [Display(Name ="Paper Size")]
        public int PaperSizeId { get; set; }
        [Display(Name ="Customer")]
        public int CustomerId { get; set; } 
        [Display(Name ="Job Type")]
        public int JobTypeId { get; set; }
        [Display(Name ="Binding style")]
        public int BindingStyleId { get; set; }
        [Display(Name ="Quantity")]
        public int ProductQuantity { get; set; }
        [Display(Name ="Text No of Colors")]
        public int TextNoOfColors { get; set; }
        [Display(Name ="Cover No of Colors")]
        public int CoverNoOfColor { get; set; }

        //---------------///////////////
        public int TotalTextNoPages { get; set; }
        
        public double Width { get; set; }
        public double Length { get; set; }
        
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

        public bool IsMarginAssigned { get; set; }
    }
}
