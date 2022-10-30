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
    public class PriceEstimationQuoteVoucher : OperationWithThirdValidation
    {
        [Key]
        public int PriceEstimationQuoteVoucherId { get; set; }

        [Display(Name = "Customer")]
        public int OrganizationCustomerId { get; set; }
        
        [Display(Name ="Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime Date { get; set; }

        [Display(Name ="Grand Total")]
        public decimal GrandTotal { get; set; }

        [Display(Name ="Plate No.")]
        public string PlateNo { get; set; }

        public virtual OrganizationCustomer Customer { get; set; }

        [Display(Name ="Bank Account")]
        public int? BankAccountId { get; set; }

    }
    public class PriceEstimationQuoteVoucherLine : PriceEstimationQuoteVoucherLineBase
    {
        public int PriceEstimationQuoteVoucherLineId { get; set; }

        [Display(Name ="Category")]
        public int CategoryId { get; set; }

        [Display(Name ="Item")]
        public int ProductId { get; set; }
    }
    public class PriceEstimationQuoteVoucherService : PriceEstimationQuoteVoucherLineBase
    {
        public int PriceEstimationQuoteVoucherServiceId { get; set; }

        [Display(Name ="Category")]
        public string CategoryCode { get; set; }
        [Display(Name ="Sub Category")]
        public string SubCategoryCode { get; set; }
        //[Display(Name ="Service Group")]
        //public ServiceGroup ServiceGroup { get; set; }
        
        [Display(Name = "Service Type")]
        public string ServiceTypeId { get; set; }
    }
    public class PriceEstimationQuoteVoucherLineBase
    {
        public int PriceEstimationQuoteVoucherId { get; set; }
        public virtual PriceEstimationQuoteVoucher PriceEstimationQuoteVoucher { get; set; }
        public decimal Quantity { get; set; }

        [DisplayName("Unit Amount")]
        public decimal UnitAmount { get; set; }

        [Display(Name = "Sub Total")]
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
