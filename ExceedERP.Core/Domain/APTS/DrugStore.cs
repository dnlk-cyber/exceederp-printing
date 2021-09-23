using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Inventory;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.APTS
{
    public class DrugStore
    {
        public int DrugStoreId { get; set; }
        public string StoreCode { get; set; }
        public string Name { get; set; }
        public InventoryStore InventoryStore { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
       
        public string Remark { get; set; }
        [NotMapped]
        public string NameWithCode => string.Concat(StoreCode + "-" + Name );
    }
}
