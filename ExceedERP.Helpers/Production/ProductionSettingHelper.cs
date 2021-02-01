using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Production
{
    public static class ProductionSettingHelper
    {
        public static string GetInventoryItemName(string itemCode)
        {
            var name = "";
            var db = new ExceedDbContext();
            var item = db.InventoryItems.Find(itemCode);
            if (item != null)
            {
                name = item.Name;
            }
            return name;
        }

        public static string GetProductionItemUnitOfMeasurnment(int cropId)
        {
            var name = "";
            var db = new ExceedDbContext();
            var cr = db.Crops.Find(cropId);
            if (cr != null)
            {
                var item = db.InventoryItems.Find(cr.ItemCode);
                if (item != null)
                {
                    name = item.UoMName;
                }
            }
            return name;
        }
        public static string GetGumProductionItemUnitOfMeasurnment(int gumId)
        {
            var name = "";
            var db = new ExceedDbContext();
            var gum = db.NaturalGums.Find(gumId);
            if (gum != null)
            {
                var item = db.InventoryItems.Find(gum.ItemCode);
                if (item != null)
                {
                    name = item.UoMName;
                }
            }
            return name;
        }
        public static ProductionSetting GetDefaultSetting(ProductionSettingType type) {
            var db = new ExceedDbContext();
            var existingSetting = db.ProductionSettings.FirstOrDefault(st => st.Value == type.ToString());
            if(existingSetting != null)
            {
                return existingSetting;
            }
            else
            {
                var newSetting = new ProductionSetting
                {
                    Type = type,
                    DateCreated = DateTime.Now,
                    Value = type.ToString(),
                };
                db.ProductionSettings.Add(newSetting);
                db.SaveChanges();

                return newSetting;
            }
        }
    }
}
