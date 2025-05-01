using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingJobOrder : Operations
    {
        [Key]
        public int PrintingJobOrderId { get; set; }
        public int CustomerId { get; set; }
        [Display(Name = "Job Start Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JobStartDate { get; set; }
        [Display(Name = "Delivery Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        public string CRVNo { get; set; }
        [Display(Name = "Advance Payment(%)")]
        public decimal AdvancePaymentInPercent { get; set; }
        [Display(Name = "Payment Type")]
        public TypeOfPayment PaymentType { get; set; }
        public int PrintingProformaInvoiceId { get; set; } 
        public bool JobClosed { get; set; }
    }

    public class PrintingJobOrderItem : TrackUserSettingOperation
    {
        [Key]
        public int PrintingJobOrderItemId { get; set; }
        [Display(Name = "Job Type")]
        public int JobTypeId { get; set; }
        public PrintJobType PrintJobType { get; set; } 
        public string JobDescription { get; set; }
        public string Description { get; set; } 
        public int NumberOfPages { get; set; }

        public decimal Quantity { get; set; }
        public BindingStyle BindingStyle { get; set; }
        [Display(Name = "Paper for text")]
        public int TextPaperSizeId { get; set; }
        [Display(Name = "Paper for cover")]
        public int CoverPaperSizeId { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "VAT")]
        public decimal VAT { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        public bool IsClosed { get; set; }
        public string Remark { get; set; }

        public int PrintingJobOrderId { get; set; }
        public int PrintingCostEstimationId { get; set; }
    }
    public class PrintingJobOrderJobDesign
    {
        [Key]
        public int PrintingJobOrderJobDesignId { get; set; }
        public int ReferenceId { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
}
