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
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [StringLength(50)]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        [StringLength(50)]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }
        public string ProformaNo { get; set; }
    }
    public class PFProformaItems
    {
        public int PFProformaItemsId { get; set; }
        public int ProformaId { get; set; }
        [Display(Name = "Job Type")]
        public int JobTypeId { get; set; }
        [Display(Name = "Job Type Item")]
        public int? JobTypeItemId { get; set; }
        [Display(Name = ("Sub Category"))]      
        public string SubCategory { get; set; }
        [Display(Name = ("Quantity"))]
        public int Quantity { get; set; }
        public string Page { get; set; }
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
