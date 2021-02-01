using ExceedERP.Core.Domain.Common.HRM;
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

namespace ExceedERP.Web.Areas.Production.Controllers.ProjectInformation
{
    public class PlotController : Controller
    {
        ExceedDbContext db;

        public PlotController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            var stages = db.ProductionSettings.Where(s => s.Type == ProductionSettingType.SeedProductionStage || s.Type == ProductionSettingType.NaturalGumProductionStage).ToList();
            ViewData["LandStatus"] = stages;

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();
            ViewData["ProductionSites"] = db.ProductionSites.Select(s => new { s.Name, s.ProductionSiteId}).ToList();

            return View();
        }
        public ActionResult Branchs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Branch> branches = db.Branch.OrderByDescending(b => b.BranchId);
            DataSourceResult result = branches.ToDataSourceResult(request, branch => new
            {
                BranchId = branch.BranchId,
                Name = branch.Name,
                AmharicName = branch.AmharicName,
                Abbreviation = branch.Abbreviation,
                Remark = branch.Remark,
                
            });
            return Json(result);
        }
        public ActionResult Plots_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Plot> plots = db.Plots;
            DataSourceResult result = plots.ToDataSourceResult(request, plot => new
            {
                ProductionSiteId = plot.ProductionSiteId,
                PlotId = plot.PlotId,
                //Width = plot.Width,
                //Length = plot.Length,
                IsolationDistance = plot.IsolationDistance,
                Area = plot.Area,
                ProductionSettingId = plot.ProductionSettingId,
                BranchId = plot.BranchId,
                PlotCode = plot.PlotCode,
            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Plot_Create([DataSourceRequest]DataSourceRequest request, Plot plot)
        {
            if (ModelState.IsValid)
            {
                var entity = new Plot
                {
                    ProductionSiteId = plot.ProductionSiteId,
                    //Width = plot.Width,
                    //Length = plot.Length,
                    IsolationDistance = plot.IsolationDistance,
                    Area = plot.Area,
                    ProductionSettingId = plot.ProductionSettingId,
                    BranchId = plot.BranchId,
                    PlotCode = plot.PlotCode,
                };
                //entity.Area = entity.Width * entity.Length;

                db.Plots.Add(entity);
                db.SaveChanges();
                plot.PlotId = entity.PlotId;
            }

            return Json(new[] { plot }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Plot_Update([DataSourceRequest]DataSourceRequest request, Plot plot)
        {
            if (ModelState.IsValid)
            {
                var entity = new Plot
                {
                    ProductionSiteId = plot.ProductionSiteId,
                    PlotId = plot.PlotId,
                    //Width = plot.Width,
                    //Length = plot.Length,
                    IsolationDistance = plot.IsolationDistance,
                    Area = plot.Area,
                    ProductionSettingId = plot.ProductionSettingId,
                    BranchId = plot.BranchId,
                    PlotCode = plot.PlotCode,
                };
                //entity.Area = entity.Width * entity.Length;


                db.Plots.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { plot }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Plot_Destroy([DataSourceRequest]DataSourceRequest request, Plot plot)
        {
            if (ModelState.IsValid)
            {
                var entity = new Plot
                {
                    ProductionSiteId = plot.ProductionSiteId,
                    PlotId = plot.PlotId,
                    //Width = plot.Width,
                    //Length = plot.Length,
                    IsolationDistance = plot.IsolationDistance,
                    Area = plot.Area,
                    ProductionSettingId = plot.ProductionSettingId,
                    BranchId = plot.BranchId,
                    PlotCode = plot.PlotCode,
                };

                db.Plots.Attach(entity);
                db.Plots.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { plot }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}