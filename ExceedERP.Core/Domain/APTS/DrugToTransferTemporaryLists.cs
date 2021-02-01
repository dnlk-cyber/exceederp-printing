using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.APTS
{
   public class DrugToTransferTemporaryLists
    {
        public int DrugToTransferTemporaryListsId { get; set; }
        public int DrugListsId { get; set; }
        public int DrugUnitId { get; set; }
        public int DrugStoreTransferId { get; set; }
        public decimal Quantity { get; set; }
        public bool IsNew { get; set; }
        public bool HaveNoBalance { get; set; }
    }
}
