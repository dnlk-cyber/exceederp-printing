using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Inventory.Controllers.InventoryHelper
{
    public class InventoryHelper :IInventoryHelper
    {
        ExceedDbContext dbContext = new ExceedDbContext();
        public bool CheckStoreKeeper(string storeCode)
        {
            bool isexist = dbContext.StoreKeeperAssignments.Count(x => x.StoreCode == storeCode)  > 0;
            return isexist; 
        }

        public string GetStoreKeeper(string storecode)
        {
            StoreKeeperAssignment keeper = dbContext.StoreKeeperAssignments.FirstOrDefault(x => x.StoreCode == storecode);
            return keeper.StoreKeeper; 
        }
    }
}