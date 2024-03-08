using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation
{
    public class FurnitureJobOrderForm : Operations
    {
        [Key]
        public int FurnitureJobOrderFormId { get; set; }
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Display(Name = "Job Type")]
        public int JobTypeId { get; set; }
        [Display(Name = "Unit")]
        public string UnitOfMeasurment { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price Before VAT")]
        public decimal UnitPriceBeforeVAT { get; set; }
        public decimal UnitPriceWithVAT { get; set; }
        [Display(Name ="DeliveryDate"), DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }
        public string CRVNo { get; set; }
        [Display(Name ="Advance Payment(%)")]
        public decimal AdvancePaymentInPercent { get; set; }
        public int FurnitureProformaInvoiceId { get; set; }

    }
}
