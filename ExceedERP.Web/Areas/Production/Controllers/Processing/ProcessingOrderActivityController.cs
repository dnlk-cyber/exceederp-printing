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

namespace ExceedERP.Web.Areas.Production.Controllers.Processing
{
    public class ProcessingOrderActivityController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Activities_Read([DataSourceRequest]DataSourceRequest request, int itemId, ProductType productionType)
        {

            IQueryable<ProcessingOrderActivity> activities = db.ProcessingOrderActivities.Where(a => a.SeedProcessingOrderItemId == itemId || a.GumProcessingOrderItemId == itemId);
            if (productionType == ProductType.Seed)
            {
                activities = activities.Where(a => a.SeedProcessingOrderItemId == itemId);
            }
            else if (productionType == ProductType.NaturalGum)
            {
                activities = activities.Where(a => a.GumProcessingOrderItemId == itemId);
            }
            DataSourceResult result = activities.ToDataSourceResult(request, activity => new
            {
                SeedProcessingOrderItemId = activity.SeedProcessingOrderItemId,
                ProductionActivityId = activity.ProductionActivityId,
                GumProcessingOrderItemId = activity.GumProcessingOrderItemId,
                ProcessingOrderActivityId = activity.ProcessingOrderActivityId,

                OperationType = activity.OperationType,
                ProductionPeriodId = activity.ProductionPeriodId,
                DateOfInformation = activity.DateOfInformation,
                //BugdetYear = activity.BugdetYear,
                Reference = activity.Reference,
                Quantity = activity.Quantity,

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
        public ActionResult Activity_Create([DataSourceRequest]DataSourceRequest request, ProcessingOrderActivity activity, int itemId, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(activity.ProductionPeriodId, activity.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");
                }
                else
                {
                    var entity = new ProcessingOrderActivity
                    {
                        SeedProcessingOrderItemId = productionType == ProductType.Seed ? itemId : activity.SeedProcessingOrderItemId,
                        GumProcessingOrderItemId = productionType == ProductType.NaturalGum ? itemId : activity.GumProcessingOrderItemId,
                        ProductionActivityId = activity.ProductionActivityId,
                        ProcessingOrderActivityId = activity.ProcessingOrderActivityId,

                        OperationType = activity.OperationType,
                        ProductionPeriodId = activity.ProductionPeriodId,
                        DateOfInformation = activity.DateOfInformation,
                        //BugdetYear = ProductionOrderHelper.GetProductionPeriodFiscalYear(activity.ProductionPeriodId),
                        Reference = activity.Reference,
                        Quantity = activity.Quantity,

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

                    db.ProcessingOrderActivities.Add(entity);
                    db.SaveChanges();

                    activity.ProcessingOrderActivityId = entity.ProcessingOrderActivityId;
                    if(productionType == ProductType.Seed)
                    {
                        ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.SeedProcessingOrderItemId, productionType);
                    }
                    else if(productionType == ProductType.NaturalGum)
                    {
                        ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.GumProcessingOrderItemId, productionType);
                    }
                }
            }

            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activity_Update([DataSourceRequest]DataSourceRequest request, ProcessingOrderActivity activity, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(activity.ProductionPeriodId, activity.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");

                }
                else
                {
                    var entity = new ProcessingOrderActivity
                    {
                        SeedProcessingOrderItemId = activity.SeedProcessingOrderItemId,
                        ProductionActivityId = activity.ProductionActivityId,
                        GumProcessingOrderItemId = activity.GumProcessingOrderItemId,
                        ProcessingOrderActivityId = activity.ProcessingOrderActivityId,

                        OperationType = activity.OperationType,
                        ProductionPeriodId = activity.ProductionPeriodId,
                        DateOfInformation = activity.DateOfInformation,
                        //BugdetYear = ProductionOrderHelper.GetProductionPeriodFiscalYear(activity.ProductionPeriodId),
                        Reference = activity.Reference,
                        Quantity = activity.Quantity,

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

                    db.ProcessingOrderActivities.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    if (productionType ==  ProductType.Seed)
                    {
                        ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.SeedProcessingOrderItemId, productionType);
                    }
                    else if (productionType == ProductType.NaturalGum)
                    {
                        ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.GumProcessingOrderItemId, productionType);
                    }
                }
            }

            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activity_Destroy([DataSourceRequest]DataSourceRequest request, ProcessingOrderActivity activity, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcessingOrderActivity
                {
                    SeedProcessingOrderItemId = activity.SeedProcessingOrderItemId,
                    ProductionActivityId = activity.ProductionActivityId,
                    GumProcessingOrderItemId = activity.GumProcessingOrderItemId,
                    ProcessingOrderActivityId = activity.ProcessingOrderActivityId,

                    OperationType = activity.OperationType,
                    ProductionPeriodId = activity.ProductionPeriodId,
                    DateOfInformation = activity.DateOfInformation,
                    //BugdetYear = activity.BugdetYear,
                    Reference = activity.Reference,
                    Quantity = activity.Quantity,

                    LaborCost = activity.LaborCost,
                    EquipmentCost = activity.EquipmentCost,
                    TotalCost = activity.TotalCost,

                    OrgBranchName = activity.OrgBranchName,
                    DateCreated = activity.DateCreated,
                    LastModified = activity.LastModified,
                    CreatedBy = activity.CreatedBy,
                    ModifiedBy = activity.ModifiedBy,

                };

                db.ProcessingOrderActivities.Attach(entity);
                db.ProcessingOrderActivities.Remove(entity);
                db.SaveChanges();

                if (productionType ==  ProductType.Seed)
                {
                    ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.SeedProcessingOrderItemId, productionType);
                }
                else if (productionType == ProductType.NaturalGum)
                {
                    ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.GumProcessingOrderItemId, productionType);
                }
            }

            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}