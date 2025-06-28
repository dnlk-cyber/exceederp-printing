using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.SalesStores.Datasource
{
    [DataObject]
   public class ProductbySalesStoreODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
       
        public List<Product> GetThingListByStore(string storeId)
        {
             int.TryParse(storeId, out int StoreID);
            
            var SaleStores = db.SalesStores.Where(x => x.SalesStoreId == StoreID).ToList();
            var thingList = new List<Product>();
            foreach (var SaleStore in SaleStores)
            {
                string SId = db.InventoryStores.Where(x => x.StoreCode == SaleStore.StoreCode).FirstOrDefault().StoreCode.ToString();
                var stockBalance = db.StoreItemAssignments.Where(p => p.InventoryStore.StoreCode == SId).ToList();
                var products = db.Products.ToList();
                foreach (var sb in stockBalance)
                {

                    var product = products.Where(x => x.ItemCode == sb.ItemCode).FirstOrDefault();
                    if (product != null && product.IsActive)
                    {
                        thingList.Add(product);
                    }

                }

            }
            return thingList;
        }
    }
}
