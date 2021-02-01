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
    public static class ProcessingOrderHelper
    {
        public static void UpdateProcessingOrderInCostItemChange(int id, ProductType productionType)
        {
            var db = new ExceedDbContext();

            var laborCost = new decimal();
            var materialCost = new decimal();
            var equipmentCost = new decimal();

            #region update seed processing order

            if (productionType == ProductType.Seed)
            {
                var record = db.SeedProcessingOrderItems.Find(id);

                if (record != null)
                {
                    var details = db.ProcessingOrderActivities.Where(d => d.SeedProcessingOrderItemId == record.SeedProcessingOrderItemId).ToList();
                    var materials = db.ProcessingInputMaterials.Where(m => m.SeedProcessingOrderItemId == record.SeedProcessingOrderItemId).ToList();
                    if (details.Any())
                    {
                        laborCost = details.Sum(d => d.LaborCost);
                        equipmentCost = details.Sum(d => d.EquipmentCost);
                    }
                    if (materials.Any())
                    {
                        materialCost = materials.Sum(m => m.TotalCost);
                    }

                    // update the record accordingly 
                    record.MaterialCost = materialCost;
                    record.LaborCost = laborCost;
                    record.EquipmentCost = equipmentCost;

                    record.DirectCost = materialCost + laborCost + equipmentCost;
                    record.TotalCost = materialCost + laborCost + equipmentCost;

                    db.SeedProcessingOrderItems.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
            } 
            #endregion
            #region update gum production production order
            else if (productionType == ProductType.NaturalGum)
            {
                var record = db.GumProcessingOrderItems.Find(id);

                if (record != null)
                {
                    var details = db.ProcessingOrderActivities.Where(d => d.GumProcessingOrderItemId == record.GumProcessingOrderItemId).ToList();
                    var materials = db.ProcessingInputMaterials.Where(m => m.GumProcessingOrderItemId == record.GumProcessingOrderItemId).ToList();
                    if (details.Any())
                    {
                        laborCost = details.Sum(d => d.LaborCost);
                        equipmentCost = details.Sum(d => d.EquipmentCost);
                    }
                    if (materials.Any())
                    {
                        materialCost = materials.Sum(m => m.TotalCost);
                    }

                    // update the record accordingly 
                    record.MaterialCost = materialCost;
                    record.LaborCost = laborCost;
                    record.EquipmentCost = equipmentCost;

                    record.DirectCost = materialCost + laborCost + equipmentCost;
                    record.TotalCost = materialCost + laborCost + equipmentCost;

                    db.GumProcessingOrderItems.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
            } 
            #endregion
        }
        //public static string GetProductionPeriodFiscalYear(int periodId)
        //{
        //    var year = "";
        //    var db = new ExceedDbContext();

        //    var productionPeriod = db.ProductionPeriods.Find(periodId);
        //    if (productionPeriod != null)
        //    {
        //        var glPeriod = db.GLPeriods.FirstOrDefault(p => p.Name == productionPeriod.Name);
        //        if (glPeriod != null)
        //        {
        //            year = glPeriod.GlFiscalYear.Name;
        //        }
        //    }

        //    return year;
        //}

        //public static string GetSeedName(int id) {
        //    var db = new ExceedDbContext();
        //    var name = "";
        //    var temp = db.Crops.Find(id);
        //    if(temp != null)
        //    {
        //        name = temp.Name;
        //    }
        //    return name;
        //}
    }
}
