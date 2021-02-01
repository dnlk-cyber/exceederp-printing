using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Production
{
    public static class CostTrackingHelper
    {
        private static ExceedDbContext db = new ExceedDbContext();

        public static void UpdateCostTrackingRecord(int id) {
            var record = db.ProjectCostByMonth.Find(id);
            var laborCost = new decimal();
            var materialCost = new decimal();
            var equipmentCost = new decimal();

            if (record != null)
            {
                var details = db.ProjectCostByMonthDetails.Where(d => d.ProjectCostByMonthId == record.ProjectCostByMonthId).ToList();
                var materials = db.ProjectCostByMonthMaterials.Where(m => m.ProjectCostByMonthId == record.ProjectCostByMonthId).ToList();
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

                record.TotalCost = materialCost + laborCost + equipmentCost;

                db.ProjectCostByMonth.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
