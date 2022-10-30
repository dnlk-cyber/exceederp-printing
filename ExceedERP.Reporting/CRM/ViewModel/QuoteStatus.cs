using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class QuoteStatus : Organization
    {
        [DisplayName("# Quote")]
        public int QuoteNumber { get; set; }

        [DisplayName("Customer")]
        public string Consignee { get; set; }
        //[DisplayName("Product Name")]
        public string ProductName { get; set; }
        public string CustomerAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string SalesPerson { get; set; }
        public string CustomerType { get; set; }
        public string Variety { get; set; }
        public string Class { get; set; }
        public string UOM { get; set; }
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime QuoteDate { get; set; }
        public decimal Quantity { get; set; }
        [DisplayName("Selling Price")]
        public decimal SellingPrice { get; set; }
        [DisplayName("Sub Totoal")]
        public decimal SubTotoal { get; set; }
        public decimal Tax { get; set; }
        [DisplayName("Grand Total")]
        public decimal GrandTotal { get; set; }
        public string TotalAmountInWords { get; set; }
        public string Status { get; set; }

        public decimal TotalTax { get; set; }
        public decimal TotalWithholding { get; set; }
        public decimal NetPay { get; set; }

        public string Store { get; set; }

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
        public DateTime? DueDate { get; set; }

       
    }

    
}
