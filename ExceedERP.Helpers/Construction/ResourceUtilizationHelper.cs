using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Construction
{
    public static class ResourceUtilizationHelper
    {
         
        public static string GetConcatnatedProjectandBlockName(int wiId) {
            ExceedDbContext _db = new ExceedDbContext();
            WorkItemPerProject workItem = new WorkItemPerProject();
            var concatnatedName =  "";
            var currentWorkItem = _db.WorkItemPerProject.Find(wiId);
            if(currentWorkItem != null)
            {
                workItem = currentWorkItem;
                var project = _db.ProjectInformation.Find(currentWorkItem.ProjectInformationId);
                concatnatedName = concatnatedName + project.Name;
                if(currentWorkItem.BlockId != null)
                {
                    var block = _db.Blocks.Find(currentWorkItem.BlockId);
                    if(block != null)
                    {
                        concatnatedName = concatnatedName + "\nBlock: " + block.Name;
                    }
                }

            }
            return concatnatedName;
        }
        public static decimal GetCurrentOverheadRate()
        {
            var db = new ExceedDbContext();
            decimal overhead = new decimal();
            var configartion = db.Configurations.ToList();
            if (configartion.Any())
            {
                var ovH = configartion.Where(x => x.Attribute == "OVER_HEAD_COST_RATE").FirstOrDefault();
                if (ovH != null)
                {
                    overhead = Convert.ToDecimal(ovH.CurrentValue);
                }
                
            }
            return overhead;
        }
        public static decimal GetCurrentProfitRate()
        {
            var db = new ExceedDbContext();
            var profit = new decimal();
            var configartion = db.Configurations.ToList();
            if (configartion.Any())
            {
                
                var profMa = configartion.Where(x => x.Attribute == "PROFIT_COST_RATE").FirstOrDefault();
                if (profMa != null)
                {
                    profit = Convert.ToDecimal(profMa.CurrentValue);
                }
            }
            return profit;
        }
        public static UnitOfMeasurement GetWorkItemUnitOfMeasurnment(int workItemId)
        {
            var db = new ExceedDbContext();
            UnitOfMeasurement unit = new UnitOfMeasurement();
            var workItem = db.WorkItemPerProject.Find(workItemId);
            if (workItem != null)
            {
                var tempUnit = db.UnitOfMeasurement.Find(workItem.UnitOfMeasurementId);
                if (tempUnit != null)
                {
                    unit = tempUnit;
                }
            }

            return unit;
        }
        /// <summary>
        /// Gets the cost of materials consumed by the project in the given period
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static decimal GetProjectMaterialConsumptionPerPeriod(int projectId, ConstructionPeriod period) {
            var db = new ExceedDbContext();
            var amount = new decimal();
            var currentProject = db.ProjectInformation.Find(projectId);
            if(currentProject != null && period != null)
            {
                // find the period from inventory periods matching the name in the construction period
                // because the consumption is stored in relation to inventory period, not construction period
                var targetInventoryPeriod = db.InventoryPeriods.FirstOrDefault(p => p.Name == period.Name);
                if(targetInventoryPeriod != null)
                {
                    var materialconsumptions = db.ProjectDirectMaterialExpenses.Where(mex => mex.BranchId == currentProject.BranchId &&
                                                mex.GLPeriodId == targetInventoryPeriod.InventoryPeriodId).ToList();
                    if (materialconsumptions.Any())
                    {
                        amount = materialconsumptions.Sum(me => me.CostOfConsumedMaterialThisMonth);
                    }
                }
            }
            return amount;
        }
    }
}
