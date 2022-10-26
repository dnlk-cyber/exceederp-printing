namespace ExceedERP.Reporting.CRM.ViewModel
{
    public   class CustomerOrderStatusSummaryViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal ShipedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
    }
}
