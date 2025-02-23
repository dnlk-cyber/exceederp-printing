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
        public string Description { get; set; } 
        public int NumberOfPages { get; set; }

        public decimal Quantity { get; set; }

        [Display(Name = "Unit Price Before VAT")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "VAT")]
        public decimal VAT { get; set; }
        [Display(Name = "Unit Price With VAT")]
        public decimal TotalPrice { get; set; }

        public bool IsClosed { get; set; }
        public string Remark { get; set; }

        public int PrintingJobOrderId { get; set; }

    }
}
