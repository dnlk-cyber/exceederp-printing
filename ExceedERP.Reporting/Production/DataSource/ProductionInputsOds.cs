using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Production;
using ExceedERP.Reporting.Production.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Production.DataSource
{
    [DataObject]
    public class ProductionInputsOds : List<ProductionOrderSeedSpecificVM>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProductionOrderSeedSpecificVM> GetMonthlyInputs(string branchId)
        {
            List<ProductionOrderSeedSpecificVM> list = new List<ProductionOrderSeedSpecificVM>();
            int bId = Convert.ToInt32(branchId);
            var currentBranch = db.Branch.Find(bId);
            if(currentBranch == null)
            {
                return null;
            }
            else
            {
                var productionOrderItems = db.SeedProductionOrderItems.Where(itm => itm.ProductionOrder.BranchId == bId).ToList();
                foreach (SeedProductionOrderItem item in productionOrderItems)
                {
                    var record = new ProductionOrderSeedSpecificVM
                    {
                        SeedName = ProductionOrderHelper.GetSeedName(item.CropId)
                    };

                    var allActivities = db.ProductionOrderActivitiy.Where(inp => inp.SeedProductionOrderItemId == item.SeedProductionOrderItemId).ToList();
                    var allMaterials = db.ProductionOrderInputMaterial.Where(inp => inp.SeedProductionOrderItemId == item.SeedProductionOrderItemId).ToList();
                    if (allActivities.Any() || allMaterials.Any())
                    {
                        var idsInMaterial = new HashSet<int>(allMaterials.Select(m => m.ProductionPeriodId));
                        var idsInActivities = new HashSet<int>(allActivities.Select(a => a.ProductionPeriodId));
                        var combinedIds = new HashSet<int>(allMaterials.Select(m => m.ProductionPeriodId));
                        foreach (var id in idsInActivities)
                        {
                            if (!(combinedIds.Contains(id)))
                            {
                                combinedIds.Add(id);
                            }
                        }
                        var periodsWithValue = db.ProductionPeriods.Where(p => (combinedIds.Contains(p.ProductionPeriodId))).OrderBy(p => p.DateTo).ToList();

                        // foreach (var periodId in combinedIds)
                        var periodsArray = periodsWithValue.ToArray();
                        //foreach(ProductionPeriod period in periodsWithValue)
                        for (int i = 0; i < periodsWithValue.Count; i++)
                        {
                            var currentPeriod = periodsArray[i];
                            var materials = allMaterials.Where(m => m.ProductionPeriodId == currentPeriod.ProductionPeriodId).ToList();
                            var activities = allActivities.Where(a => a.ProductionPeriodId == currentPeriod.ProductionPeriodId).ToList();
                            if ((materials.Any() || activities.Any()) && currentPeriod != null)
                            {
                                //var record = new ProductionSeedSpecificInputsVM
                                //{
                                //    Month = currentPeriod.Name,
                                //    MaterialCost = materials.Sum(m => m.TotalCost),
                                //    LaborCost = activities.Sum(a => a.LaborCost),
                                //    EquipmentCost = activities.Sum(a => a.EquipmentCost)
                                //};
                                //record.TotalCost = record.LaborCost + record.MaterialCost + record.EquipmentCost;
                                var materialCost = new ProductionSeedSpecificInputsVM
                                {
                                    CostType = "Material Input",
                                    Month = currentPeriod.Name,
                                    Order = i,
                                    CostAmount = materials.Sum(m => m.TotalCost)
                                };
                                var laborCost = new ProductionSeedSpecificInputsVM
                                {
                                    CostType = "Labor Cost",
                                    Month = currentPeriod.Name,
                                    Order = i,
                                    CostAmount = activities.Sum(m => m.LaborCost)
                                };
                                var equipmentCost = new ProductionSeedSpecificInputsVM
                                {
                                    CostType = "Equipment Cost",
                                    Month = currentPeriod.Name,
                                    Order = i,
                                    CostAmount = activities.Sum(m => m.EquipmentCost)
                                };
                                //var totalCost = new ProductionSeedSpecificInputsVM
                                //{
                                //    CostType = "Total Cost",
                                //    Month = currentPeriod.Name,
                                //    CostAmount = materials.Sum(m => m.TotalCost) + activities.Sum(m => m.EquipmentCost) + activities.Sum(m => m.LaborCost)
                                //};
                                record.Inputs.Add(materialCost);
                                record.Inputs.Add(laborCost);
                                record.Inputs.Add(equipmentCost);
                                //list.Add(totalCost);
                            }
                        }
                    }
                    list.Add(record);
                }
            }
            
            return list;
        }
    }
}
