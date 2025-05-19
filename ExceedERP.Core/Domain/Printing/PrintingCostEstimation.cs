using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public PrintJobType PrintJobType { get; set; } 
        [Display(Name ="Paper for text")]
        public int TextPaperSizeId { get; set; }
        [Display(Name ="Paper for cover")]
        public int CoverPaperSizeId { get; set; }
        [Display(Name ="Customer")]
        public int CustomerId { get; set; } 
       
        
        [Display(Name ="Quantity")]
        public int ProductQuantity { get; set; }
        [Display(Name ="Pages")]
        public int Pages { get; set; }  
        [Display(Name ="Text No of Colors")]
        public int TextNoOfColors { get; set; }
        [Display(Name ="Cover No of Colors")]
        public int CoverNoOfColor { get; set; }
        public string JobDescription { get; set; }
        [Display(Name = "Machine For Text")]
        public int TextMachineTypeId { get; set; }
        [Display(Name ="Machine For Cover")]
        public int? CoverMachineTypeId { get; set; }
        [Display(Name = "Pack")]
        public decimal Pack { get; set; }
        [Display(Name ="Binding Style")]
        public BindingStyle BindingStyle { get; set; }
        public int Copy { get; set; }
        public int Staples { get; set; } 
        //--//
        public double Width { get; set; }
        public double Length { get; set; }          
        public bool CoverRequired { get; set; }
        public bool OnlyOneSideCover { get; set; }
        public bool IsMarginAssigned { get; set; }
        public bool UseLength { get; set; } = true;// use the Longer side of the paper for glue binding
        public int TextPrintSide { get; set; } // 1 or 2
        public int CoverPrintSide { get; set; }
        public bool UseAdvancedCalculations { get; set; } // enables advanced calculations

    }

    public enum BindingStyle
    {
        [Display(Name ="Perfect Bind"), Description("Perfect Bind")]
        PerfectBind,
        [Display(Name ="Saddle Stitch"), Description("Saddle Stitch")]
        SaddleStitch,
        [Display(Name ="Side Stitch"), Description("Side Stitch")]
        SideStitch,
    }
    public enum PrintJobType  
    {
        Book = 1,
        Magazine,
        Card,
        Flyer,
        Brochure,
        Format,
        Poster,
        Sticker,
        Voucher,
        
        Calendar,
        
        Newsletter,
        
        
    }

}
