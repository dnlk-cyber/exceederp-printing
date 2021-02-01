using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
    public class ArSalesOrderPartial: TrackUserOperation
    {
        public int ArSalesOrderPartialId { get; set; }
        public int ArSalesOrderPartialPrintId { get; set; }
        public virtual ArSalesOrderPartialPrint ArSalesOrderPartialPrint { get; set; }
       
        [Display(Name = "Item Description")]
        public string ItemName { get; set; }
        [Display(Name = "Item")]
        public string SubItem { get; set; }
        
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
       
        [Display(Name = "Unit of Measurement")]
        public string UoM { get; set; }
       
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
       
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
       
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
       
    }
}
