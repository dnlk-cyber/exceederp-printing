
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting.Settings
{
    public class FurnitureJcPensionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetAllJcPensions()
        {
            var data = db.JcPensions;
            return Json(data.Select(s => new { s.JcPensionId, s.Rate }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult JcPensions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJcPension> JcPensions = db.FurnitureJcPensions;
            DataSourceResult result = JcPensions.ToDataSourceResult(request, JcPension => new {
                FurnitureJcPensionId = JcPension.FurnitureJcPensionId,
                Rate = JcPension.Rate,
                Description = JcPension.Description
            });

            return Json(result);
        }

        public ActionResult JcPensions_Create([DataSourceRequest]DataSourceRequest request, FurnitureJcPension JcPension)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureJcPension)this.GetObject(JcPension);
                db.FurnitureJcPensions.Add(entity);
                db.SaveChanges();
                JcPension.FurnitureJcPensionId = entity.FurnitureJcPensionId;
            }

            return Json(new[] { JcPension }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JcPensions_Update([DataSourceRequest]DataSourceRequest request, FurnitureJcPension JcPension)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureJcPension)this.GetObject(JcPension);

                db.FurnitureJcPensions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { JcPension }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JcPensions_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJcPension JcPension)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureJcPension)this.GetObject(JcPension);

                db.FurnitureJcPensions.Attach(entity);
                db.FurnitureJcPensions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { JcPension }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureJcPension JcPension)
        {
            var entity = new FurnitureJcPension
            {
                FurnitureJcPensionId = JcPension.FurnitureJcPensionId,
                Rate = JcPension.Rate,
                Description = JcPension.Description

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}