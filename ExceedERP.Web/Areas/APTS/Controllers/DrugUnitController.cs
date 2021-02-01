using ExceedERP.Core.Domain.APTS;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class DrugUnitController : Controller
    {
        // GET: APTS/DrugUnit
        ExceedDbContext db = new ExceedDbContext();
     
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["UoM"] = db.UoMs.ToList();
        }
        public ActionResult DrugUnit_Read([DataSourceRequest]DataSourceRequest request)
        {
            var medisince = db.DrugUnits.ToList();
            DataSourceResult result = medisince.ToDataSourceResult(request, unit => new
            {
                DrugUnitId = unit.DrugUnitId,
                UoMName = unit.UoMName,
                UnitName = unit.UnitName,
                Remark = unit.Remark
          

            });

            return Json(result);
        }
        public ActionResult DrugUnit_Create([DataSourceRequest]DataSourceRequest request, DrugUnit drugUnit)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugUnit
                {
                    UnitName = drugUnit.UoMName,
                    UoMName = drugUnit.UoMName,
                    Remark = drugUnit.Remark
                };
                db.DrugUnits.Add(entity);
                db.SaveChanges();
                drugUnit.DrugUnitId = entity.DrugUnitId;
            }

            return Json(new[] { drugUnit }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugUnit_Update([DataSourceRequest]DataSourceRequest request, DrugUnit drugUnit)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugUnit
                {
                    DrugUnitId = drugUnit.DrugUnitId,
                    UoMName = drugUnit.UoMName,
                    UnitName = drugUnit.UnitName,
                    Remark = drugUnit.Remark
                };

                db.DrugUnits.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

               
            }

            return Json(new[] { drugUnit }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugUnit_Destroy([DataSourceRequest]DataSourceRequest request, DrugUnit drugUnit)
        {
            if (ModelState.IsValid)
            {

                var entity = new DrugUnit
                {
                    DrugUnitId = drugUnit.DrugUnitId,
                    UoMName = drugUnit.UoMName,
                    UnitName = drugUnit.UnitName,
                    Remark = drugUnit.Remark
                };
                db.DrugUnits.Attach(entity);
                db.DrugUnits.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { drugUnit }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetUoM()
        {
            var uoms = db.UoMs.ToList();
            return Json(uoms.Select(p=>new
            {
                UoMName = p.UoMName,
                Description = p.UoMName
            }),JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}