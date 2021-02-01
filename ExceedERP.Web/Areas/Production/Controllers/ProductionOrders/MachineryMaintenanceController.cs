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
    public class MachineryMaintenanceController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult MachineryMaintenances_Read([DataSourceRequest]DataSourceRequest request, int itemId, ProductType productionType)
        {

            IQueryable<MachineryMaintenance> maintenaces = db.MachineryMaintenance.Where(a => a.SeedProductionOrderItemId == itemId || a.GumProductionOrderItemId == itemId);
            if (productionType == ProductType.Seed)
            {
                maintenaces = maintenaces.Where(a => a.SeedProductionOrderItemId == itemId);
            }
            else if (productionType == ProductType.NaturalGum)
            {
                maintenaces = maintenaces.Where(a => a.GumProductionOrderItemId == itemId);
            }
            DataSourceResult result = maintenaces.ToDataSourceResult(request, maintenance => new
            {
                SeedProductionOrderItemId = maintenance.SeedProductionOrderItemId,
                MachineryMaintenanceId = maintenance.MachineryMaintenanceId,
                GumProductionOrderItemId = maintenance.GumProductionOrderItemId,

                MachineryDescription = maintenance.MachineryDescription,
                ActivityPerformed = maintenance.ActivityPerformed,
                ProductionPeriodId = maintenance.ProductionPeriodId,
                DateOfInformation = maintenance.DateOfInformation,

               
                TotalCost = maintenance.TotalCost,
                //BugdetYear = maintenance.BugdetYear,
                Reference = maintenance.Reference,

                OrgBranchName = maintenance.OrgBranchName,
                DateCreated = maintenance.DateCreated,
                LastModified = maintenance.LastModified,
                CreatedBy = maintenance.CreatedBy,
                ModifiedBy = maintenance.ModifiedBy,
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
        public ActionResult MachineryMaintenance_Create([DataSourceRequest]DataSourceRequest request, MachineryMaintenance maintenance, int itemId, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(maintenance.ProductionPeriodId, maintenance.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");
                }
                else
                {
                    var entity = new MachineryMaintenance
                    {
                        SeedProductionOrderItemId = productionType == ProductType.Seed ? itemId : maintenance.SeedProductionOrderItemId,
                        GumProductionOrderItemId = productionType == ProductType.NaturalGum ? itemId : maintenance.GumProductionOrderItemId,
                        
                        MachineryDescription = maintenance.MachineryDescription,
                        ActivityPerformed = maintenance.ActivityPerformed,
                        ProductionPeriodId = maintenance.ProductionPeriodId,
                        DateOfInformation = maintenance.DateOfInformation,
                        //BugdetYear = ProductionOrderHelper.GetProductionPeriodFiscalYear(maintenance.ProductionPeriodId),
                        Reference = maintenance.Reference,

                        TotalCost = maintenance.TotalCost,

                        OrgBranchName = maintenance.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = maintenance.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = maintenance.ModifiedBy,

                    };
                   

                    db.MachineryMaintenance.Add(entity);
                    db.SaveChanges();

                    maintenance.MachineryMaintenanceId = entity.MachineryMaintenanceId;

                    if (productionType == ProductType.Seed)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                    }
                    else if (productionType == ProductType.NaturalGum)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
                    }
                }
            }

            return Json(new[] { maintenance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryMaintenance_Update([DataSourceRequest]DataSourceRequest request, MachineryMaintenance maintenance, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(maintenance.ProductionPeriodId, maintenance.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");
                }
                else
                {
                    var entity = new MachineryMaintenance
                    {
                        SeedProductionOrderItemId = maintenance.SeedProductionOrderItemId,
                        MachineryMaintenanceId = maintenance.MachineryMaintenanceId,
                        GumProductionOrderItemId = maintenance.GumProductionOrderItemId,

                        MachineryDescription = maintenance.MachineryDescription,
                        ActivityPerformed = maintenance.ActivityPerformed,
                        ProductionPeriodId = maintenance.ProductionPeriodId,
                        DateOfInformation = maintenance.DateOfInformation,
                        //BugdetYear = ProductionOrderHelper.GetProductionPeriodFiscalYear(maintenance.ProductionPeriodId),
                        Reference = maintenance.Reference,

                        TotalCost = maintenance.TotalCost,

                        OrgBranchName = maintenance.OrgBranchName,
                        DateCreated = maintenance.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = maintenance.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.MachineryMaintenance.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
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
            }

            return Json(new[] { maintenance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryMaintenance_Destroy([DataSourceRequest]DataSourceRequest request, MachineryMaintenance maintenance, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachineryMaintenance
                {
                    SeedProductionOrderItemId = maintenance.SeedProductionOrderItemId,
                    MachineryMaintenanceId = maintenance.MachineryMaintenanceId,
                    GumProductionOrderItemId = maintenance.GumProductionOrderItemId,
                    
                    MachineryDescription = maintenance.MachineryDescription,
                    ActivityPerformed = maintenance.ActivityPerformed,
                    ProductionPeriodId = maintenance.ProductionPeriodId,
                    DateOfInformation = maintenance.DateOfInformation,

                    TotalCost = maintenance.TotalCost,
                    //BugdetYear = maintenance.BugdetYear,
                    Reference = maintenance.Reference,

                    OrgBranchName = maintenance.OrgBranchName,
                    DateCreated = maintenance.DateCreated,
                    LastModified = maintenance.LastModified,
                    CreatedBy = maintenance.CreatedBy,
                    ModifiedBy = maintenance.ModifiedBy,

                };

                db.MachineryMaintenance.Attach(entity);
                db.MachineryMaintenance.Remove(entity);
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

            return Json(new[] { maintenance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}