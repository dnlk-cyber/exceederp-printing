using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
