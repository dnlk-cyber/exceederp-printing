using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{
    public class AdditionalWorkShopDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Maintenance/AdditionalWorkShopDetail
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            var additionalInfo = db.AdditionalWorkShopDetails.Where(x => x.ServiceCenterJobId == Id);
            return Json(additionalInfo.ToDataSourceResult(request));
        }

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, AdditionalWorkShopDetail model, Guid Id)
        {
            ModelState.Remove("ServiceCenterJobId");
            model.ServiceCenterJobId = Id;
            if (ModelState.IsValid)
            {
                
                db.AdditionalWorkShopDetails.Add(model);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, AdditionalWorkShopDetail model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, AdditionalWorkShopDetail model)
        {
            if (ModelState.IsValid)
            {
                var additional = db.AdditionalWorkShopDetails.Find(model.AdditionalWorkShopDetailId);
                db.AdditionalWorkShopDetails.Remove(additional);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }
    }
}