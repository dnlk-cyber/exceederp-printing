using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class RoadController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Construction/Road
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Roads_Read([DataSourceRequest]DataSourceRequest request, int roadProjectIdRead)
        {
            IQueryable<Road> roads = db.Roads.Where(x => x.ProjectInformationId == roadProjectIdRead);
            DataSourceResult result = roads.ToDataSourceResult(request, road => new {
                Roadid = road.RoadId,
                Name = road.Name,
                ProjectInformationId = road.ProjectInformationId,
                Remark = road.Remark
            });

            return Json(result);
        }

        private bool CheckExistance(string description)
        {
            var road = db.Roads.Where(x => x.Name == description).FirstOrDefault();
            if (road != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Road_Create([DataSourceRequest]DataSourceRequest request, Road road, int roadProjectIdCreate)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(road.Name))
                {
                    ModelState.AddModelError("", "This road is already registered!!");
                }
                else
                {
                    var entity = new Road
                    {
                        RoadId = road.RoadId,
                        Name = road.Name,
                        ProjectInformationId = roadProjectIdCreate,
                        Remark = road.Remark
                    };

                    db.Roads.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { road }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Road_Update([DataSourceRequest]DataSourceRequest request, Road road)
        {
            if (ModelState.IsValid)
            {
                var entity = new Road
                {
                    RoadId = road.RoadId,
                    Name = road.Name,
                    ProjectInformationId = road.ProjectInformationId,
                    Remark = road.Remark
                };

                db.Roads.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { road }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Road_Destroy([DataSourceRequest]DataSourceRequest request, Road road)
        {
            if (ModelState.IsValid)
            {
                var entity = new Road
                {
                    RoadId = road.RoadId,
                    Name = road.Name,
                    ProjectInformationId = road.ProjectInformationId,
                    Remark = road.Remark
                };

                db.Roads.Attach(entity);
                db.Roads.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { road }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}