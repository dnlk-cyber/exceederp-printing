using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
    public class ArSalesOrderHeaderVm : Organization

    {
        public string VoucherId { get; set; } 
        public string DocTitile { get; set; }
        public string OrderNumber { get; set; }
        public string Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public string Station { get; set; }
        public string BroadcastDateTime { get; set; }
        public string SalesPerson { get; set; }
        public string Partner { get; set; }
        public int PaymentTerm { get; set; }
        public string OrderReference { get; set; }
        public string Product { get; set; }
        public string Length { get; set; }
        public string Repeatition { get; set; }
        public OrderStatus OrderStatus { get; set; }
        //
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string PrintStatus { get; set; }
    }


    public class ArSalesOrderLineVm 
    {
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
        public string TaxName { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        public string Remark { get; set; }
    }
}
