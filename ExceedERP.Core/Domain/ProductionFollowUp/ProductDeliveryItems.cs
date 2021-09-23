namespace ExceedERP.Core.Domain.ProductionFollowUp
{
    public class ProductDeliveryItems
    {
        public int ProductDeliveryItemsId { get; set; }
        public int ProductDeliveryId { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
