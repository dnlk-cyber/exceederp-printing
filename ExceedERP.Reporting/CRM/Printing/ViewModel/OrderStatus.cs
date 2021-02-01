using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.CRM.Printing.ViewModel
{
   public class OrderStatus : Organization
    {

        [DisplayName("Order Number")]
       public string QuoteNumber { get; set; }

        [DisplayName("Order Number")]
        public string OrderNumber { get; set; }
        public string CustomerAddress { get; set; }
        public string SalesPerson { get; set; }
        public string PaymentMethod { get; set; }
        public string CustomerType { get; set; }

        [DisplayName("Customer")]
        public string Consignee { get; set; }
        [DisplayName("Store Name")]
        public string Store { get; set; }
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        public string Variety { get; set; }
        public string Class { get; set; }
       
        [DisplayName("Quote Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime QuoteDate { get; set; }
        [DisplayName("Order Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public string OrderDate { get; set; }
        [DisplayName("Order Quantity")]
        public decimal OrderQuantity { get; set; }
        [DisplayName("Quote Quantity")]
        public decimal QuoteQuantity { get; set; }
        [DisplayName(" Selling price ")]
        public decimal SellingPrice { get; set; }
        [DisplayName(" Sub Totoal ")]
        public decimal SubTotoal { get; set; }
        public decimal Tax { get; set; }
        [DisplayName(" Grand Total ")]
        public decimal GrandTotal { get; set; }

        public string UOM { get; set; }

        public string Status { get; set; }


        public bool IsOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OnlineApprovedTime { get; set; }
        public bool IsOnlineTransferred { get; set; }
        public string IsOnlineTransferredBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OnlineTransferredTime { get; set; }
        public string RequestedBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        //added
        //public bool IsSendForApproval { get; set; }
        public string SendForApprovalBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SendForApprovalTime { get; set; }


    }
}
