using ExceedERP.Core.Domain.PrintingEstimation.Setting;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.ProductionFollowUp
{
    public class Proforma
    {
        public int ProformaId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Time { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ValidityDate { get; set; }
        public PaymentTer PaymentTerm { get; set; }
        public bool HaveEstimation { get; set; }
        public bool IsExistingCustomer { get; set; }
        public string ProformaNo { get; set; }

        public string InvoiceNo { get; set; }
        public int? EstimationFormId { get; set; }
        public int CustomerId { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [StringLength(50)]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        [StringLength(50)]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }
        public string City { get; set; }
        public string SubCity { get; set; }
        public string Woreda { get; set; }
        public string POBox { get; set; }

        public decimal? CustomerVAT { get; set; }
        public decimal? CustomerMargin { get; set; }
        public decimal? CustomerOverHead { get; set; }
        public string CustomerDescription { get; set; }
    }
    public class PFProformaItems
    {
        public int PFProformaItemsId { get; set; }
        public int ProformaId { get; set; }
        [Display(Name = "Job Type")]
        public int JobTypeId { get; set; }

        [Display(Name = ("Quantity"))]
        public int Quantity { get; set; }
        public string Size { get; set; }
        [Display(Name = ("Unit Price"))]
        public decimal UnitPrice { get; set; }
        public decimal BeforeTax { get; set; }
        [Display(Name = ("Tax"))]
        public decimal Tax { get; set; }
        [Display(Name = ("Total Price"))]
        public decimal TotalPrice { get; set; }
        public string Description { get; set; }
    }
}
