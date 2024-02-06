

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
   public class FurnitureFinishedGoods
    {
        public int FurnitureFinishedGoodsId { get; set; }
        public int FurnitureFinishedGoodReceivingId { get; set; }
        public string CodeNo { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public string Remark { get; set; }
    }
}
