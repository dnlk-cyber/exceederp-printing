

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
   public class FinishedGoods
    {
        public int FinishedGoodsId { get; set; }
        public int FinishedGoodReceivingId { get; set; }
        public string CodeNo { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public string Remark { get; set; }
    }
}
