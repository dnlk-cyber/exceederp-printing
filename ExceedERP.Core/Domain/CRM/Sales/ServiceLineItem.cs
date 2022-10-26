using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class ServiceLineItem : TrackUserOperation
    {

        [Key]
        public int ServiceLineItemId { get; set; }
        public CRMVoucherType VoucherType { get; set; }
        /// <summary>
        /// reference to vouchers (sales quote, sales order or sales invoice)
        /// depending on the Voucher Type field
        /// </summary>
        public int VoucherId { get; set; }

        /// <summary>
        /// maintenance service type
        /// </summary>
        [Display(Name ="Service Type")]
        public string ServiceTypeId { get; set; }
        public string ServiceTypeCode { get; set; }

        public decimal Quantity { get; set; }

        [DisplayName("Unit Amount")]
        public decimal UnitAmount { get; set; }

        [Display(Name ="Sub Total")]
        public decimal SubTotal { get; set; }

        [DisplayName("Tax Type")]
        public int? TaxTypeId { get; set; }

        [DisplayName("Tax Amount")]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Withholding Type")]
        public int? WithholdingTypeId { get; set; }

        [Display(Name = "Withholding Amount")]
        public decimal WithholdingAmount { get; set; }

        [Display(Name = "Withhold Tax(VAT)")]
        public bool WithholdTax { get; set; }
        [Display(Name = "Withhold Tax (VAT)")]
        public decimal WithholdTaxAmount { get; set; }
        [DisplayName("Total Amount")]
        public decimal TotalAmount { get; set; }
    }
}
