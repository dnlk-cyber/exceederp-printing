using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Production
{
    public static class StoreHelper
    {
        public static int GetStoreBranch(string storecode)
        {
            ExceedDbContext db = new ExceedDbContext();
            int branchid = 0;
            var record = db.InventoryStores.FirstOrDefault(x => x.StoreCode == storecode);
            if (record != null) branchid = record.BranchId;
            return branchid;
        }
    }
}
