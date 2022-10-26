using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class OrganizationFullAddress : Organization
    {
        public string SubCity { get; set; }
        public string HouseNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string PoBox { get; set; }
        public string VatRegNumber { get; set; }
        public string DateOfRegistration { get; set; }
        public string TinNumber { get; set; }
    }
    public class CashSales : OrganizationFullAddress
    {

        [DisplayName("Invoice Number")]
        public int InvoiceNumber { get; set; }

        [DisplayName("Order Number")]
        public string OrderNumber { get; set; }

        [DisplayName("Customer")]
        public string Consignee { get; set; }
        public string CustomerAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string Remark { get; set; }
        public string SalesPerson { get; set; }
        [DisplayName("Store Name")]
        public string Store { get; set; }
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        public string Variety { get; set; }
        public string Class { get; set; }
        public string CustomerType { get; set; }
       
        [DisplayName("Invoice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public string InvoiceDate { get; set; }
        [DisplayName("Order Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Order Quantity")]
        public decimal OrderQuantity { get; set; }
        [DisplayName(" Quantity")]
        public decimal InvoiceQuantity { get; set; }
        [DisplayName(" Selling price ")]
        public decimal SellingPrice { get; set; }
        public decimal SubTotoal { get; set; }
        public decimal Tax { get; set; }
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
        //added
        //public bool IsSendForApproval { get; set; }
        public string SendForApprovalBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SendForApprovalTime { get; set; }


        public string RequestedBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }

        public string PaymentTerm { get; set; }
        public string CustomerTinNumber { get; internal set; }
        public string TotalAmountInWords { get; internal set; }
        public string Branch { get; internal set; }
        public string Shop { get; internal set; }
        public string PrintStatus { get; internal set; }
        public string ReceiptReference { get; internal set; }
        public string FsInvoiceNumber { get; internal set; }
    }
}
