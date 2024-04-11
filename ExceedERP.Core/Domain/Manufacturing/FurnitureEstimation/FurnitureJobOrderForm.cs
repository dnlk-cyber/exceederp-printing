using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;


namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureJobOrderForm : Operations
    {
        [Key]
        public int FurnitureJobOrderFormId { get; set; }
       
        public int CustomerId { get; set; }       
       
        [Display(Name = "Delivery Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        public string CRVNo { get; set; }
        [Display(Name ="Advance Payment(%)")]
        public decimal AdvancePaymentInPercent { get; set; }
        [Display(Name ="Payment Type")]
        public TypeOfPayment PaymentType { get; set; }
        public int FurnitureProformaInvoiceId { get; set; }
        public bool JobClosed { get; set; }  
    }

    public class FurnitureJobOrderItem : TrackUserSettingOperation
    {
        public int FurnitureJobOrderItemId { get; set; }
        [Display(Name ="Job Type")]
        public int JobTypeId { get; set; }
        [Display(Name ="Job-No")]
        public string JobNo { get; set; }
        [Display(Name = "Unit of measurment")]
        public string UnitOfMeasurment { get; set; }

        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price Before VAT")]
        public decimal UnitPriceBeforeVAT { get; set; }
        [Display(Name = "VAT")]
        public decimal VAT { get; set; }
        [Display(Name = "Unit Price With VAT")]
        public decimal UnitPriceWithVAT { get; set; }

        public int FurnitureJobOrderFormId { get; set; }
        public bool IsClosed { get; set; }
        public FurnitureJobOrderForm FurnitureJobOrderForm { get; set; }
    }

    
}
