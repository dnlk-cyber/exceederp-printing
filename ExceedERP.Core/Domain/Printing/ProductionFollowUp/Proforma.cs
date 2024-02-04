using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.printing.PrintingEstimation.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
    public class Proforma : TrackUserSettingOperation
    {
        public  int ProformaId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Time { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ValidityDate { get; set; }
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
        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }
        [Display(Name = "Tin No ")]
        public string TinNo { get; set; }
        public bool IsApproved { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsChecked { get; set; }

    }
    public class PFProformaItems
    {
        public int PFProformaItemsId { get; set; }
        public int ProformaId { get; set; }
        [Display(Name = "Job Type")]
        public string JT { get; set; }
        public int JobTypeId { get; set; }
       
        [Display(Name = ("Quantity"))]
        public decimal Quantity { get; set; }
        public string Size { get; set; }
        [Display(Name = ("Unit Price"))]
        public decimal UnitPrice { get; set; }     
        public decimal BeforeTax { get; set; }
        [Display(Name = ("Tax"))]
        public decimal Tax { get; set; }
        [Display(Name=("Total Price"))]
        public decimal TotalPrice { get; set; }
        public string Description { get; set; }
    }
}
