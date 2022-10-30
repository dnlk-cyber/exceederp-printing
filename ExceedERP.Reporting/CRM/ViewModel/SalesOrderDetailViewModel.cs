using System;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class SalesOrderDetailViewModel
    {
        public string Customer { get; set; }
        public string OrderType { get; set; }
        public int OrderNumber { get; set; }
        public string OrderReference { get; set; }
        public DateTime OrderDate { get; set; }
        public string QuotationTerm { get; set; }
        public DateTime? OrderDueDate { get; set; }
        public string OrderStatus { get; set; }

        //public List<SalesOrderDetailLineItemsViewModel> LineItems { get; set; } = new List<SalesOrderDetailLineItemsViewModel>();

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
    public class SalesOrderDetailLineItemsViewModel
    {
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
    }
}
