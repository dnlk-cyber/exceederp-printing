using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingJobOrderProduction : Operations
    {
        [Key]
        public int PrintingJobOrderProductionId { get; set; }
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Display(Name = "Job Name")]
        public int JobTypeId { get; set; }
        [Display(Name = "Paper Size")]
        public int PaperSizeId { get; set; }
        [Display(Name = "Binding style")]
        public int BindingStyleId { get; set; }
        [Display(Name = "Text No of Colors")]
        public int TextNoOfColors { get; set; }
        [Display(Name = "Cover No of Colors")]
        public int CoverNoOfColor { get; set; }
        public int TotalTextNoPages { get; set; }

        [Display(Name = "Job.No")]
        public string JobOrderNo { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public string JobDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public DateTime DateOrdered { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), Display(Name = "Delivery Date"), DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }
        public int PrintingJobOrderId { get; set; }
        public bool IsSendForFinishing { get; set; }
        public string SendForFinishingBy { get; set; }
        public DateTime? SendForFinishingDate { get; set; }
        public bool IsClosed { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? DateClosed { get; set; }
    }

    public class PrintingProductionMaterialCost : TrackUserSettingOperation
    {
        [Key]
        public int PrintingProductionMaterialCostId { get; set; }
        [Display(Name = "Item Category")]
        public int PrintingMaterialCategoryId { get; set; }
        [Display(Name = "Item Name")]
        public int PrintingMaterialCategoryItemId { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }
        [Display(Name = "UoM")]
        public string UnitOfMeasurment { get; set; }

        //FK
        public int PrintingJobOrderProductionId { get; set; }
    }

    public class PrintingProductionLaborCost : TrackUserSettingOperation
    {
        public int PrintingProductionLaborCostId { get; set; }
        [Display(Name = "Process Category")]
        public PrintingProcessCategory ProcessCategory { get; set; }
        [Display(Name = "Process Name")]
        public int? PrintingProcessId { get; set; }
        [Display(Name = "Machine Type")]
        public int? PrintingMachineTypeId { get; set; }
        [Display(Name = "Estimated Hour")]
        public decimal EstimatedHours { get; set; }
        [Display(Name = "Labor Rate")]
        public decimal LaborRate { get; set; }
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        //FK
        public int PrintingJobOrderProductionId { get; set; }    
    }
}
