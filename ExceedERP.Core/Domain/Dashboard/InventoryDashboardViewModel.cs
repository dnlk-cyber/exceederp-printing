using System.Collections.Generic;

namespace ExceedERP.Core.Domain.Dashboard
{
    public class InventorySeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class ItemBalanceSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
}
