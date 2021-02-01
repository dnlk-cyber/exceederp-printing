using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Inventory.ViewModels
{
    public enum InventoryDataType
    {
        BinCard
    }
    public class InventoryDataArchiveVm
    {
        public InventoryDataType Type { get; set; }
        public int GlFiscalYearId { get; set; }
        public int GLPeriodId { get; set; }
    }
}
