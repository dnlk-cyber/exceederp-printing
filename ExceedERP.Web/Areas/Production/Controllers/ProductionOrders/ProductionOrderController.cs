using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
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
    public class ProductionOrderController : Controller
    {
        ExceedDbContext db;
        // GET: Production/SeedProductionPlanYear
        public ProductionOrderController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            ViewData["FarmingPlots"] = db.Plots.Select(p => new { p.PlotId, p.PlotCode}).ToList();

            var crops = db.Crops.ToList();
            ViewData["CropTypes"] = db.Crops.Select(c => new { c.CropId, c.Name});

            ViewData["NaturalGumNames"] = db.NaturalGums.Select(g => new { g.Name, g.NaturalGumId}).ToList();

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId,b.Name }).ToList();

            ViewData["ProductionSites"] = db.ProductionSites.Select(s => new { s.Name, s.ProductionSiteId}).ToList();

            //ViewData["Activities"] = db.ProductionActivities.Select(a => new { a.ProductionActivityId, a.Name}).ToList();

            //ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name}).ToList();

            //ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode}).ToList();

            //ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(p => new { p.ProductionPeriodId, p.Name}).ToList();

            ViewData["ProductionYears"] = db.ProductionYears.Select(y => new { y.ProductionYearId, y.Name}).ToList();

            return View();
        }
        public ActionResult ProductionOrders_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionOrder> productionOrders = db.ProductionOrders;
            DataSourceResult result = productionOrders.ToDataSourceResult(request, productionOrder => new
            {
                ProductionOrderId = productionOrder.ProductionOrderId,
                ProductionType = productionOrder.ProductionType,
                BranchId = productionOrder.BranchId,
                ProductionSiteId = productionOrder.ProductionSiteId,
                //PlotId = productionOrder.PlotId,
                //DateFrom = productionOrder.DateFrom,
                //DateTo = productionOrder.DateTo,
                //ProductionCode = productionOrder.ProductionCode,
                //Description = productionOrder.Description,
                //CropId = productionOrder.CropId,
                //SeedType = productionOrder.SeedType,
                //Generation = productionOrder.Generation,

                OrgBranchName = productionOrder.OrgBranchName,
                DateCreated = productionOrder.DateCreated,
                LastModified = productionOrder.LastModified,
                CreatedBy = productionOrder.CreatedBy,
                ModifiedBy = productionOrder.ModifiedBy,
            });

            return Json(result);
        }

        private bool PeriodExists(int projectId, SeedProductionPlanYear model, ControllerFunctionType action)
        {
            var valueExists = false;
            if (action == ControllerFunctionType.Create)
            {
                var temp = db.SeedProductionPlanYear
                    .FirstOrDefault(y => y.ProductionProjectId == projectId &&
                                    y.GlFiscalYearId == model.GlFiscalYearId);
                if (temp != null)
                {
                    valueExists = true;
                }
            }
            else if (action == ControllerFunctionType.Update)
            {
                var temp = db.SeedProductionPlanYear
                    .FirstOrDefault(y => y.ProductionProjectId == projectId &&
                                    y.GlFiscalYearId == model.GlFiscalYearId &&
                                    y.SeedProductionPlanYearId != model.SeedProductionPlanYearId);
                if (temp != null)
                {
                    valueExists = true;
                }
            }

            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrder_Create([DataSourceRequest]DataSourceRequest request, ProductionOrder productionOrder)
        {
            if (ModelState.IsValid)
            {
                //if (PeriodExists(id, productionOrder, ControllerFunctionType.Create))
                //{
                //    ModelState.AddModelError("", "This Year is Registered already");
                //}
                //else
                //{
                var entity = new ProductionOrder
                {
                    ProductionOrderId = productionOrder.ProductionOrderId,
                    ProductionType = productionOrder.ProductionType,
                    BranchId = productionOrder.BranchId,
                    ProductionSiteId = productionOrder.ProductionSiteId,
                    //PlotId = productionOrder.PlotId,
                    //DateFrom = productionOrder.DateFrom,
                    //DateTo = productionOrder.DateTo,
                    //ProductionCode = productionOrder.ProductionCode,
                    //Description = productionOrder.Description,
                    //CropId = productionOrder.CropId,
                    //SeedType = productionOrder.SeedType,
                    //Generation = productionOrder.Generation,

                    OrgBranchName = productionOrder.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = productionOrder.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = productionOrder.ModifiedBy,

                };

                db.ProductionOrders.Add(entity);
                db.SaveChanges();

                productionOrder.ProductionOrderId = entity.ProductionOrderId;
                //}
            }

            return Json(new[] { productionOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrder_Update([DataSourceRequest]DataSourceRequest request, ProductionOrder productionOrder)
        {
            if (ModelState.IsValid)
            {
                //if (PeriodExists(productionOrder.ProductionProjectId, productionOrder, ControllerFunctionType.Update))
                //{
                //    ModelState.AddModelError("", "This Year is Registered already");
                //}
                //else
                //{
                var entity = new ProductionOrder
                {
                    ProductionOrderId = productionOrder.ProductionOrderId,
                    ProductionType = productionOrder.ProductionType,
                    BranchId = productionOrder.BranchId,
                    ProductionSiteId = productionOrder.ProductionSiteId,
                    //PlotId = productionOrder.PlotId,
                    //DateFrom = productionOrder.DateFrom,
                    //DateTo = productionOrder.DateTo,
                    //ProductionCode = productionOrder.ProductionCode,
                    //Description = productionOrder.Description,
                    //CropId = productionOrder.CropId,
                    //SeedType = productionOrder.SeedType,
                    //Generation = productionOrder.Generation,

                    OrgBranchName = productionOrder.OrgBranchName,
                    DateCreated = productionOrder.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = productionOrder.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.ProductionOrders.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            //}

            return Json(new[] { productionOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrder_Destroy([DataSourceRequest]DataSourceRequest request, ProductionOrder productionOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionOrder
                {
                    ProductionOrderId = productionOrder.ProductionOrderId,
                    ProductionType = productionOrder.ProductionType,
                    BranchId = productionOrder.BranchId,
                    ProductionSiteId = productionOrder.ProductionSiteId,
                    //PlotId = productionOrder.PlotId,
                    //DateFrom = productionOrder.DateFrom,
                    //DateTo = productionOrder.DateTo,
                    //ProductionCode = productionOrder.ProductionCode,
                    //Description = productionOrder.Description,
                    //CropId = productionOrder.CropId,
                    //SeedType = productionOrder.SeedType,
                    //Generation = productionOrder.Generation,

                    OrgBranchName = productionOrder.OrgBranchName,
                    DateCreated = productionOrder.DateCreated,
                    LastModified = productionOrder.LastModified,
                    CreatedBy = productionOrder.CreatedBy,
                    ModifiedBy = productionOrder.ModifiedBy,

                };

                db.ProductionOrders.Attach(entity);
                db.ProductionOrders.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { productionOrder }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}