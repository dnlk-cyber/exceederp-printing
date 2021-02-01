using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
    public class ArSalesOrderLine : TrackUserOperation
    {
        public int ArSalesOrderLineId { get; set; }
        public int ArSalesOrderId { get; set; }
        public virtual ArSalesOrder ArSalesOrder { get; set; }
        [Required]
        [Display(Name = "Item Description")]
        public string ItemName { get; set; }
        [Display(Name = "Item")]
        public string SubItem { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        [Required]
        [Display(Name = "Unit of Measurement")]
        public string UoM { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Sales Price")]
        public decimal SalesPrice { get; set; }
        [Display(Name = "Sales Discount")]
        public decimal Discount { get; set; }
        [Display(Name = "Sales Commission")]
        public decimal Commission { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Required]
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        public string Remark { get; set; }
    }
}
