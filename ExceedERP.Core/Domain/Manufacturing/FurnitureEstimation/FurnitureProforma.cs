using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureProformaInvoice : Operations
    {
        [Key]
        [Display(Name ="Proforma No.")]
        public int FurnitureProformaInvoiceId { get; set; }
        
        [Display(Name ="Customer")]
        public int CustomerId { get; set; }
        [Display(Name ="Delivery Period(Days)")]
        public int DeliveryPeriod { get; set; }
        [Display(Name ="Delivery Date"), DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }
        [Display(Name ="Price Validity Period(Days)")]
        public int PriceValidityPeriod { get; set; }
        [Display(Name ="Mode of payment")]
        public string ModeOfPayment { get; set; }
        [Display(Name = "Advance Payment(%)")]
        public decimal AdvancePaymentInPercent { get; set; }
        [Display(Name ="Payment Type")]
        public TypeOfPayment PaymentType { get; set; }
        public bool HaveEstimation { get; set; }
        public bool IsExistingCustomer { get; set; }

    }
    public class FurnitureProformaInvoiceItem : TrackUserSettingOperation
    {
        public int FurnitureProformaInvoiceItemId { get; set; }

        // Fk for Estimation
        public int FurnitureEstimationId { get; set; }

        [Display(Name ="Item")]
        public int JobTypeId { get; set; }
        [Display(Name ="Unit of measurment")]
        public string UnitOfMeasurment { get; set; }
       
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price Before VAT")]
        public decimal UnitPriceBeforeVAT { get; set; }
        [Display(Name = "VAT")]
        public decimal VAT { get; set; }
        [Display(Name = "Unit Price With VAT")]
        public decimal UnitPriceWithVAT { get; set; }
        [Display(Name ="Total Price")]
        public decimal TotalPrice { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }  

        public int FurnitureProformaInvoiceId { get; set; }
        public FurnitureProformaInvoice FurnitureProformaInvoice { get; set; } 

    }

    public enum TypeOfPayment
    {
        Cash,
        Credit
    }
}
