using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Production
{
    public static class ProductionOrderHelper
    {
        public static void UpdateProductionOrderInCostItemChange(int id, ProductType productionType)
        {
            var db = new ExceedDbContext();

            var laborCost = new decimal();
            var materialCost = new decimal();
            var equipmentCost = new decimal();
            var purchaseCost = new decimal();
            #region update seed production order

            if (productionType == ProductType.Seed)
            {
                var record = db.SeedProductionOrderItems.Find(id);

                if (record != null)
                {
                    var details = db.ProductionOrderActivitiy.Where(d => d.SeedProductionOrderItemId == record.SeedProductionOrderItemId).ToList();
                    var materials = db.ProductionOrderInputMaterial.Where(m => m.SeedProductionOrderItemId == record.SeedProductionOrderItemId).ToList();
                    var maintenances = db.MachineryMaintenance.Where(m => m.SeedProductionOrderItemId == record.SeedProductionOrderItemId).ToList();
                    var purchases = db.ProductionRawProductPurchase.Where(m => m.SeedProductionOrderItemId == record.SeedProductionOrderItemId).ToList();
                    if (details.Any())
                    {
                        laborCost = details.Sum(d => d.LaborCost);
                        equipmentCost = details.Sum(d => d.EquipmentCost);
                    }
                    if (materials.Any())
                    {
                        materialCost = materials.Sum(m => m.TotalCost);
                    }
                    if (maintenances.Any())
                    {
                        equipmentCost += maintenances.Sum(m => m.TotalCost);
                    }
                    if (purchases.Any())
                    {
                        purchaseCost += purchases.Sum(m => m.TotalPrice);
                    }

                    // update the record accordingly 
                    record.MaterialCost = materialCost;
                    record.LaborCost = laborCost;
                    record.EquipmentCost = equipmentCost;
                    record.PurchaseCost = purchaseCost;

                    record.DirectCost = materialCost + laborCost + equipmentCost + record.PurchaseCost;
                    record.TotalCost = materialCost + laborCost + equipmentCost + record.PurchaseCost;
                    if(record.ProductionType == SeedProductionType.ContractualProduction && purchases.Any())
                    {
                        record.QuantityProduced = purchases.Sum(p => p.PurchasedQuantity);
                    }

                    db.SeedProductionOrderItems.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
            } 
            #endregion
            #region update gum production production order
            else if (productionType == ProductType.NaturalGum)
            {
                var record = db.GumProductionOrderItems.Find(id);

                if (record != null)
                {
                    var details = db.ProductionOrderActivitiy.Where(d => d.GumProductionOrderItemId == record.GumProductionOrderItemId).ToList();
                    var materials = db.ProductionOrderInputMaterial.Where(m => m.GumProductionOrderItemId == record.GumProductionOrderItemId).ToList();
                    var maintenances = db.MachineryMaintenance.Where(m => m.GumProductionOrderItemId == record.GumProductionOrderItemId).ToList();
                    var purchases = db.ProductionRawProductPurchase.Where(m => m.GumProductionOrderItemId == record.GumProductionOrderItemId).ToList();
                    if (details.Any())
                    {
                        laborCost = details.Sum(d => d.LaborCost);
                        equipmentCost = details.Sum(d => d.EquipmentCost);
                    }
                    if (materials.Any())
                    {
                        materialCost = materials.Sum(m => m.TotalCost);
                    }
                    if (maintenances.Any())
                    {
                        equipmentCost += maintenances.Sum(m => m.TotalCost);
                    }
                    if (purchases.Any())
                    {
                        purchaseCost += purchases.Sum(m => m.TotalPrice);
                    }
                    // update the record accordingly 
                    record.MaterialCost = materialCost;
                    record.LaborCost = laborCost;
                    record.EquipmentCost = equipmentCost;
                    record.PurchaseCost = purchaseCost;

                    record.DirectCost = materialCost + laborCost + equipmentCost + record.PurchaseCost;
                    record.TotalCost = materialCost + laborCost + equipmentCost + record.PurchaseCost;
                    if (record.ProductionType == GumProductionType.ProductionByConcession && purchases.Any())
                    {
                        record.QuantityProduced = purchases.Sum(p => p.PurchasedQuantity);
                    }

                    db.GumProductionOrderItems.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
            } 
            #endregion
        }
        public static string GetProductionPeriodFiscalYear(int periodId)
        {
            var year = "";
            var db = new ExceedDbContext();

            var productionPeriod = db.ProductionPeriods.Find(periodId);
            if (productionPeriod != null)
            {
                var glPeriod = db.GLPeriods.FirstOrDefault(p => p.Name == productionPeriod.Name);
                if (glPeriod != null)
                {
                    year = glPeriod.GlFiscalYear.Name;
                }
            }

            return year;
        }

        public static string GetSeedName(int id) {
            var db = new ExceedDbContext();
            var name = "";
            var temp = db.Crops.Find(id);
            if(temp != null)
            {
                name = temp.Name;
            }
            return name;
        }
    }
}
