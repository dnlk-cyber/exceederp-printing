using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.APTS
{
   public class DrugStoreTransfer
    {
        public int DrugStoreTransferId { get; set; }    
        public int FromDrugStoreId { get; set; }
        public int ToDrugStoreId { get; set; }
        public int BranchId { get; set; }
    }
}
