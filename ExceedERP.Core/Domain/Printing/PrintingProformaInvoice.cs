using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing
{
    public class PrintingProformaInvoice : Operations
    {
        [Key]
        [Display(Name = "Proforma No.")]
        public int PrintingProformaInvoiceId { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Display(Name = "Delivery Period(Days)")]
        public int DeliveryPeriod { get; set; }
        [Display(Name = "Delivery Date"), DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }
        [Display(Name = "Price Validity Period(Days)")]
        public int PriceValidityPeriod { get; set; }
        [Display(Name = "Mode of payment")]
        public string ModeOfPayment { get; set; }
        [Display(Name = "Advance Payment(%)")]
        public decimal AdvancePaymentInPercent { get; set; }
        [Display(Name = "Payment Type")]
        public TypeOfPayment PaymentType { get; set; }
        public bool HaveEstimation { get; set; }
        public bool IsExistingCustomer { get; set; }
    }
    public class PrintingProformaInvoiceItem : TrackUserSettingOperation
    {
        public int PrintingProformaInvoiceItemId { get; set; }

        // Fk for Estimation
        public int PrintingCostEstimationId { get; set; }

        [Display(Name = "Item")]
        public int JobTypeId { get; set; }
        public int NumberOfPages { get; set; }
        [Display(Name = "Unit of measurment")] 
        public string UnitOfMeasurment { get; set; }
        
        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; } //totalprice/quantity
        public decimal TotalPrice { get; set; }
        public decimal TransportCost { get; set; }        
        public int? GLTaxRateID { get; set; }
        public decimal GrandTotalIncludingVAT { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }

        public int PrintingProformaInvoiceId { get; set; }
    }
}
