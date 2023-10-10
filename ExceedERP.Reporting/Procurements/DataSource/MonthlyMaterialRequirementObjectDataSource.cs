using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class MonthlyMaterialRequirementObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MonthlyMaterialRequirementViewModel> GetMonthlyMaterialRequirementByPrice(string branch, string fisicalYear, string businessUnitId)
        {
            List<MonthlyMaterialRequirementViewModel> materialRequirementList = new List<MonthlyMaterialRequirementViewModel>();

            var procurementPlans = db.ProcurementPlans
                .Where(x => x.IsOnlineApproved && x.isVoid == false).AsQueryable();

            if (!String.IsNullOrEmpty(fisicalYear))
            {

                procurementPlans = procurementPlans
                    .Where(x => x.GlFiscalYearId.ToString() == fisicalYear).AsQueryable();
            }

            if (!String.IsNullOrEmpty(branch))
            {
                procurementPlans = procurementPlans.Where(x => x.BranchId.ToString() == branch).AsQueryable();
            }

            if (!String.IsNullOrEmpty(businessUnitId))
            {
                int bdid = int.Parse(businessUnitId);
                procurementPlans = procurementPlans.Where(x => x.BusinessUnitId == bdid).AsQueryable();
            }


            foreach (var inventoryPlan in procurementPlans)
                {
                   
                    var planItems = db.ProcurementPlanItems.Where(x => x.ProcurementPlanId == inventoryPlan.ProcurementPlanId).ToList();

                    foreach (var planItem in planItems)
                    {
                        MonthlyMaterialRequirementViewModel materialRequirement =new MonthlyMaterialRequirementViewModel
                            {
                                Description = planItem.ItmCode + "_" + planItem.ItemName,
                                Unit = planItem.UoM,
                                TotalQuantity = planItem.Quantity,
                                UnitPrice = planItem.UnitPrice,
                                TotalPrice = planItem.UnitPrice * planItem.Quantity,
                                July = planItem.Period1 * planItem.UnitPrice,
                                Aug = planItem.Period2 * planItem.UnitPrice,
                                Sep = planItem.Period3 * planItem.UnitPrice,
                                Oct = planItem.Period4 * planItem.UnitPrice,
                                Nov = planItem.Period5 * planItem.UnitPrice,
                                Dec = planItem.Period6 * planItem.UnitPrice,
                                Jan = planItem.Period7 * planItem.UnitPrice,
                                Feb = planItem.Period8 * planItem.UnitPrice,
                                Mar = planItem.Period9 * planItem.UnitPrice,
                                Apr = planItem.Period10 * planItem.UnitPrice,
                                May = planItem.Period11 * planItem.UnitPrice,
                                June = planItem.Period12 * planItem.UnitPrice
                            };
                        materialRequirementList.Add(materialRequirement);

                      }
                   
                }
            
            return materialRequirementList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MonthlyMaterialRequirementViewModel> GetMonthlyMaterialRequirementByQuantity (string branch, string fisicalYear, string businessUnitId)
        {
            List<MonthlyMaterialRequirementViewModel> materialRequirementList = new List<MonthlyMaterialRequirementViewModel>();

            var procurementPlans = db.ProcurementPlans
                   .Where(x => x.IsOnlineApproved && x.isVoid == false).AsQueryable();

            if (!String.IsNullOrEmpty(fisicalYear))
            {

                procurementPlans = procurementPlans
                    .Where(x => x.GlFiscalYearId.ToString() == fisicalYear).AsQueryable();
            }

            if (!String.IsNullOrEmpty(branch))
            {
                procurementPlans = procurementPlans.Where(x => x.BranchId.ToString() == branch).AsQueryable();
            }

            if (!String.IsNullOrEmpty(businessUnitId))
            {
                int bdid = int.Parse(businessUnitId);
                procurementPlans = procurementPlans.Where(x => x.BusinessUnitId == bdid).AsQueryable();
            }


            foreach (var inventoryPlan in procurementPlans)
            {
                
                var planItems = db.ProcurementPlanItems.Where(x => x.ProcurementPlanId == inventoryPlan.ProcurementPlanId).ToList();
                foreach (var planItem in planItems)
                {
                    MonthlyMaterialRequirementViewModel materialRequirement =new MonthlyMaterialRequirementViewModel
                        {
                            Description = planItem.ItmCode + "-" + planItem.ItemName,
                            Unit = planItem.UoM,
                            TotalQuantity = planItem.Quantity,
                            UnitPrice = planItem.UnitPrice,
                            TotalPrice = planItem.UnitPrice * planItem.Quantity,
                            July = planItem.Period1,
                            Aug = planItem.Period2,
                            Sep = planItem.Period3,
                            Oct = planItem.Period4,
                            Nov = planItem.Period5,
                            Dec = planItem.Period6,
                            Jan = planItem.Period7,
                            Feb = planItem.Period8,
                            Mar = planItem.Period9,
                            Apr = planItem.Period10,
                            May = planItem.Period11,
                            June = planItem.Period12
                        };

                    materialRequirementList.Add(materialRequirement);
                }

            }
            return materialRequirementList;
        }
    }
}
