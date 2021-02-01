using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Production
{
    public static class ProductionStoreHelper
    {
        public static int GetGlPeriodIdFromProductionPeriod(int id) {
            var db = new ExceedDbContext();
            var idToReturn = 0;
            var productionPeriod = db.ProductionPeriods.Find(id);

            if(productionPeriod != null)
            {
                var glPeriod = db.GLPeriods.FirstOrDefault(p => p.Name == productionPeriod.Name);
                if(glPeriod != null)
                {
                    idToReturn = glPeriod.GLPeriodId;
                }
            }
            return idToReturn;
        }

        public static decimal GetAveragePriceOfRawProductInStore( string itemCode, string storeCode)
        {
            var avgPrice = new decimal();
            var db = new ExceedDbContext();
            var aprovedStoreReceives = db.ProductionStoreReceives.Where(s => s.StoreCode == storeCode && s.IsOnlineApproved == true);
            if (aprovedStoreReceives.Any())
            {
                var ids = new HashSet<int>(aprovedStoreReceives.Select(r => r.ProductionStoreReceiveId));
                var items = db.ProductionStoreReceiveItems.Where(itm => itm.ItemCode == itemCode && ids.Contains(itm.ProductionStoreReceiveId)).ToList();
                if (items.Any())
                {
                    avgPrice = items.Average(i => i.UnitPrice);
                }
            }
            return avgPrice;
        }
    }

}
