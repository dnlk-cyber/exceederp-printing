using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Production
{
   public static class CleanedProductsSellingPriceHelper
    {
        public static void CalculateSellingPrice(int id, ProductType productType)
        {
            var db = new ExceedDbContext();

            if(productType == ProductType.Seed)
            {
                var record = db.CleanedSeedSellingPrices.Find(id);
                if(record != null)
                {
                    var items = db.CleanedSeedSellingPriceItems.Where(itm => itm.CleanedSeedSellingPriceId == id).ToList();
                    if (items.Any())
                    {
                        var qt = items.Sum(i => i.CleanedQuantity);
                        var cost = items.Sum(i => i.TotalNetCost);

                        var unitCost = qt > 0 ? cost / qt : 0;

                        record.SellingPrice = unitCost;
                        record.ApprovedSellingPrice = record.ApprovedSellingPrice == 0 ? unitCost : record.ApprovedSellingPrice;

                        db.CleanedSeedSellingPrices.Attach(record);
                        db.Entry(record).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            else if(productType == ProductType.NaturalGum)
            {
                var record = db.CleanedNaturalGumSellingPrices.Find(id);
                if (record != null)
                {
                    var items = db.CleanedNaturalGumSellingPriceItem.Where(itm => itm.CleanedNaturalGumSellingPriceId == id).ToList();
                    if (items.Any())
                    {
                        var qt = items.Sum(i => i.CleanedQuantity);
                        var cost = items.Sum(i => i.TotalNetCost);

                        var unitCost = qt > 0 ? cost / qt : 0;

                        record.SellingPrice = unitCost;
                        record.ApprovedSellingPrice = record.ApprovedSellingPrice == 0 ? unitCost : record.ApprovedSellingPrice;

                        db.CleanedNaturalGumSellingPrices.Attach(record);
                        db.Entry(record).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
        }
    }
}
