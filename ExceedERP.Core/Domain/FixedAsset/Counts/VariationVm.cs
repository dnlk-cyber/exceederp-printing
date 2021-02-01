using System.ComponentModel;

namespace ExceedERP.Core.Domain.FixedAsset.Counts
{
    public class VariationVm : FixedAssetCountItem
    {
        [DisplayName("Shortage Qty.")]
        public decimal ShortageQty { get; set; }
        [DisplayName("Overage Qty.")]
        public decimal OverageQty { get; set; }
        [DisplayName("Shortage Cost")]

        public decimal ShortagePrice { get; set; }
        [DisplayName("Overage Cost")]

        public decimal OveragePrice { get; set; }
        [DisplayName("Physical Amount")]
        public decimal Amouont { get; set; }
        public decimal TotalAmount { get; set; }
        [DisplayName("Amount")]
        public decimal PhysicalAmount { get; set; }
        [DisplayName("Unit")]
        public string Uom { get; set; }
        [DisplayName("Item Description")]
        public string ItemDescription { get; set; }

    }
}