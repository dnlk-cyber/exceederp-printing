using System;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class SalesQuoteDetailViewModel
    {
        public string Customer { get; set; }
        public string OrderType { get; set; }
        public int QuoteNumber { get; set; }
        public string QuoteReference { get; set; }
        public DateTime QuoteDate { get; set; }
        public string QuotationTerm { get; set; }
        public DateTime? QuoteDueDate { get; set; }
        public string QuoteStatus { get; set; }

        //public List<SalesQuoteDetailLineItemsViewModel> LineItems { get; set; } = new List<SalesQuoteDetailLineItemsViewModel>();

        //
        //
        public string Store { get; set; }
        public string ItemCategory { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string Variety { get; set; }
        public string Class { get; set; }
        public string UoM { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
        //
    }
    //public class SalesQuoteDetailLineItemsViewModel
    //{
    //    public string Store { get; set; }
    //    public string ItemCategory { get; set; }
    //    public string ItemName { get; set; }
    //    public string ItemCode { get; set; }
    //    public string Variety { get; set; }
    //    public string Class { get; set; }
    //    public string UoM { get; set; }
    //    public decimal Quantity { get; set; }
    //    public decimal UnitPrice { get; set; }
    //    public decimal SubTotal { get; set; }
    //    public decimal Tax { get; set; }
    //    public decimal TotalAmount { get; set; }
    //    public string Remarks { get; set; }
    //}
}
