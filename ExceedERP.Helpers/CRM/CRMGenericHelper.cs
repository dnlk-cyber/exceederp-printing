using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.CRM
{

    public static  class CRMGenericHelper
    {
        public static int GetSalesStoreBranchId(int salesStoreId)
        {
            var db = new ExceedDbContext();
            var branchId = 0;
            var store = db.SalesStores.Find(salesStoreId);
            if (store != null)
            {
                branchId = store.InventoryStore.BranchId;
            }
            return branchId;
        }

    }

}
