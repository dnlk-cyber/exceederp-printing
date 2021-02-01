using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionOrders
{
    public class ProductionOrderActivitiyController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Activities_Read([DataSourceRequest]DataSourceRequest request, int itemId, ProductType productionType)
        {

            IQueryable<ProductionOrderActivitiy> activities = db.ProductionOrderActivitiy.Where(a => a.SeedProductionOrderItemId == itemId || a.GumProductionOrderItemId == itemId);
            if (productionType == ProductType.Seed)
            {
                activities = activities.Where(a => a.SeedProductionOrderItemId == itemId);
            }
            else if (productionType == ProductType.NaturalGum)
            {
                activities = activities.Where(a => a.GumProductionOrderItemId == itemId);
            }
            DataSourceResult result = activities.ToDataSourceResult(request, activity => new
            {
                SeedProductionOrderItemId = activity.SeedProductionOrderItemId,
                ProductionActivityId = activity.ProductionActivityId,
                GumProductionOrderItemId = activity.GumProductionOrderItemId,
                ProductionOrderActivitiyId = activity.ProductionOrderActivitiyId,

                OperationType = activity.OperationType,
                Quantity = activity.Quantity,
                ProductionPeriodId = activity.ProductionPeriodId,
                DateOfInformation = activity.DateOfInformation,
                //BugdetYear = activity.BugdetYear,
                Reference = activity.Reference,

                LaborCost = activity.LaborCost,
                EquipmentCost = activity.EquipmentCost,
                TotalCost = activity.TotalCost,

                OrgBranchName = activity.OrgBranchName,
                DateCreated = activity.DateCreated,
                LastModified = activity.LastModified,
                CreatedBy = activity.CreatedBy,
                ModifiedBy = activity.ModifiedBy,
            });

            return Json(result);
        }

        private bool IsInvalidTimeRange(int periodId, DateTime time)
        {
            var isInvalid = false;
            var currentPeriod = db.ProductionPeriods.Find(periodId);
            if (currentPeriod != null)
            {
                if (currentPeriod.DateFrom > time || currentPeriod.DateTo < time)
                {
                    isInvalid = true;
                }
            }
            return isInvalid;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activity_Create([DataSourceRequest]DataSourceRequest request, ProductionOrderActivitiy activity, int itemId, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(activity.ProductionPeriodId, activity.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");
                }
                else
                {
                    var entity = new ProductionOrderActivitiy
                    {
                        SeedProductionOrderItemId = productionType == ProductType.Seed ? itemId : activity.SeedProductionOrderItemId,
                        GumProductionOrderItemId = productionType == ProductType.NaturalGum ? itemId : activity.GumProductionOrderItemId,
                        ProductionActivityId = activity.ProductionActivityId,
                        ProductionOrderActivitiyId = activity.ProductionOrderActivitiyId,

                        OperationType = activity.OperationType,
                        Quantity = activity.Quantity,
                        ProductionPeriodId = activity.ProductionPeriodId,
                        DateOfInformation = activity.DateOfInformation,
                        //BugdetYear = ProductionOrderHelper.GetProductionPeriodFiscalYear(activity.ProductionPeriodId),
                        Reference = activity.Reference,

                        LaborCost = activity.LaborCost,
                        EquipmentCost = activity.EquipmentCost,
                        TotalCost = activity.TotalCost,

                        OrgBranchName = activity.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = activity.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = activity.ModifiedBy,

                    };
                    entity.TotalCost = entity.LaborCost + entity.EquipmentCost;

                    db.ProductionOrderActivitiy.Add(entity);
                    db.SaveChanges();

                    activity.ProductionOrderActivitiyId = entity.ProductionOrderActivitiyId;

                    if (productionType == ProductType.Seed)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                        this.UpdateFarmingLandStatusAccordingActivityPerformed(entity.ProductionActivityId, entity.SeedProductionOrderItemId, productionType);

                    }
                    else if (productionType == ProductType.NaturalGum)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
                        this.UpdateFarmingLandStatusAccordingActivityPerformed(entity.ProductionActivityId, entity.SeedProductionOrderItemId, productionType);

                    }
                }
            }

            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activity_Update([DataSourceRequest]DataSourceRequest request, ProductionOrderActivitiy activity, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(activity.ProductionPeriodId, activity.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");

                }
                else
                {
                    var entity = new ProductionOrderActivitiy
                    {
                        SeedProductionOrderItemId = activity.SeedProductionOrderItemId,
                        ProductionActivityId = activity.ProductionActivityId,
                        GumProductionOrderItemId = activity.GumProductionOrderItemId,
                        ProductionOrderActivitiyId = activity.ProductionOrderActivitiyId,

                        OperationType = activity.OperationType,
                        Quantity = activity.Quantity,
                        ProductionPeriodId = activity.ProductionPeriodId,
                        DateOfInformation = activity.DateOfInformation,
                        //BugdetYear = ProductionOrderHelper.GetProductionPeriodFiscalYear(activity.ProductionPeriodId),
                        Reference = activity.Reference,

                        LaborCost = activity.LaborCost,
                        EquipmentCost = activity.EquipmentCost,
                        TotalCost = activity.TotalCost,

                        OrgBranchName = activity.OrgBranchName,
                        DateCreated = activity.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = activity.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };
                    entity.TotalCost = entity.LaborCost + entity.EquipmentCost;

                    db.ProductionOrderActivitiy.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    if (productionType == ProductType.Seed)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                        this.UpdateFarmingLandStatusAccordingActivityPerformed(entity.ProductionActivityId, entity.SeedProductionOrderItemId, productionType);

                    }
                    else if (productionType == ProductType.NaturalGum)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
                        this.UpdateFarmingLandStatusAccordingActivityPerformed(entity.ProductionActivityId, entity.SeedProductionOrderItemId, productionType);

                    }
                }
            }

            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activity_Destroy([DataSourceRequest]DataSourceRequest request, ProductionOrderActivitiy activity, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionOrderActivitiy
                {
                    SeedProductionOrderItemId = activity.SeedProductionOrderItemId,
                    ProductionActivityId = activity.ProductionActivityId,
                    GumProductionOrderItemId = activity.GumProductionOrderItemId,
                    ProductionOrderActivitiyId = activity.ProductionOrderActivitiyId,

                    OperationType = activity.OperationType,
                    Quantity = activity.Quantity,
                    ProductionPeriodId = activity.ProductionPeriodId,
                    DateOfInformation = activity.DateOfInformation,
                    //BugdetYear = activity.BugdetYear,
                    Reference = activity.Reference,

                    LaborCost = activity.LaborCost,
                    EquipmentCost = activity.EquipmentCost,
                    TotalCost = activity.TotalCost,

                    OrgBranchName = activity.OrgBranchName,
                    DateCreated = activity.DateCreated,
                    LastModified = activity.LastModified,
                    CreatedBy = activity.CreatedBy,
                    ModifiedBy = activity.ModifiedBy,

                };

                db.ProductionOrderActivitiy.Attach(entity);
                db.ProductionOrderActivitiy.Remove(entity);
                db.SaveChanges();

                if (productionType == ProductType.Seed)
                {
                    ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                }
                else if (productionType == ProductType.NaturalGum)
                {
                    ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
                }
            }

            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }
        private void UpdateFarmingLandStatusAccordingActivityPerformed(int settingActivityId, int orderId, ProductType productType) {
            var activity = db.ProductionActivities.Find(settingActivityId);
            if(productType == ProductType.Seed)
            {
                var order = db.SeedProductionOrderItems.Find(orderId);
                if(order != null && activity != null)
                {
                    var plot = db.Plots.Find(order.PlotId);

                    plot.ProductionSettingId = activity.ProductionSettingId;

                    db.Plots.Attach(plot);
                    db.Entry(plot).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else if(productType == ProductType.NaturalGum)
            {
                var order = db.GumProductionOrderItems.Find(orderId);
                //if (order != null && activity != null)
                //{
                    
                //}
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}