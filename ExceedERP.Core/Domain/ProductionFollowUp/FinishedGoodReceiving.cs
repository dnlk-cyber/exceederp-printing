namespace ExceedERP.Core.Domain.ProductionFollowUp
{
    public class FinishedGoodReceiving
    {
        public int FinishedGoodReceivingId { get; set; }
        public string VoucherNo { get; set; }
        public string Store { get; set; }
        public  int JobId { get; set; }
        public string DeliveredBy { get; set; }
        public string ReceivedBy { get; set; }
        public string Supervisor { get; set; }
    }
}
