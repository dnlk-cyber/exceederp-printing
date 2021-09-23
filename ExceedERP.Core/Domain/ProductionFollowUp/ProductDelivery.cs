namespace ExceedERP.Core.Domain.ProductionFollowUp
{
    public class ProductDelivery
    {
        public int ProductDeliveryId { get; set; }
        public int JobId { get; set; }
        public string CustomerRefNo { get; set; }
        public string CreditSaleNo { get; set; }
        public string CashSaleNo { get; set; }
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string IdentificationNo { get; set; }
    }
}
