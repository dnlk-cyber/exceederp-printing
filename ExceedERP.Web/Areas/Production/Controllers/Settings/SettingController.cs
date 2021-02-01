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

namespace ExceedERP.Web.Areas.Production.Controllers.Settings
{
    public class SettingController : Controller
    {
        private ExceedDbContext db;
        public SettingController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionSetting> settings = db.ProductionSettings;
            DataSourceResult result = settings.ToDataSourceResult(request, setting => new
            {
                ProductionSettingId = setting.ProductionSettingId,
                Type = setting.Type,
                Value = setting.Value,
                Remark = setting.Remark
            });

            return Json(result);
        }

        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Setting_Create([DataSourceRequest]DataSourceRequest request, ProductionSetting setting)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionSetting
                {
                    //ProductionSettingId = setting.ProductionSettingId,
                    Type = setting.Type,
                    Value = setting.Value,
                    Remark = setting.Remark
                };

                db.ProductionSettings.Add(entity);
                db.SaveChanges();
                setting.ProductionSettingId = entity.ProductionSettingId;
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Setting_Update([DataSourceRequest]DataSourceRequest request, ProductionSetting setting)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionSetting
                {
                    ProductionSettingId = setting.ProductionSettingId,
                    Type = setting.Type,
                    Value = setting.Value,
                    Remark = setting.Remark
                };


                db.ProductionSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Setting_Destroy([DataSourceRequest]DataSourceRequest request, ProductionSetting setting)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionSetting
                {
                    ProductionSettingId = setting.ProductionSettingId,
                    Type = setting.Type,
                    Value = setting.Value,
                    Remark = setting.Remark
                };


                db.ProductionSettings.Attach(entity);
                db.ProductionSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}