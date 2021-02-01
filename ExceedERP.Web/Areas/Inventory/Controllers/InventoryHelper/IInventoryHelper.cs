namespace ExceedERP.Web.Areas.Inventory.Controllers.InventoryHelper
{
    public interface IInventoryHelper
    {
        /// <summary>
        /// this method help for checking if the store keeper in the given store
        /// </summary>
        /// <param name="storeCode"></param>
        /// <returns></returns>
        bool CheckStoreKeeper(string storeCode);
        /// <summary>
        /// this method return store keepreName if exist
        /// </summary>
        /// <param name="storecode"></param>
        /// <returns></returns>
        string GetStoreKeeper ( string storecode);
       
    
}
}
