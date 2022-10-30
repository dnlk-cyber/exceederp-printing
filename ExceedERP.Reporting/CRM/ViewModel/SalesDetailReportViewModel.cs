using System;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class SalesDetailReportViewModel
    {
        //public string InvoiceNumber { get; set; }
        public string ItemCategory { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UoM { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Adds { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
       
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerTIN { get; set; }
        public DateTime SalesDate { get; set; }
        public string Type { get; set; }
        public string SystemReference { get; set; }
        public string DocumentReference { get; set; }
        public string FsInvoiceNumber { get; set; }
        public string SalesPerson { get; set; }
        public string Branch { get; set; }
        public string Shop { get; set; }
        public string Remark { get; set; }
    }
}
