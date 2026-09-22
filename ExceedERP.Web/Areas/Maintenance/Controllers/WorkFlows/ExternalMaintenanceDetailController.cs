using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{
    public class ExternalMaintenanceDetailController : Controller
    {
        // GET: Maintenance/ExternalMaintenanceDetail
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            var fetchDetail = db.ExternalServiceCenterDetail.Where(x=>x.ServiceCenterJobId==id).ToList();
            var Result = fetchDetail.ToDataSourceResult(request, x => new
            {
                ExternalServiceCenterDetailId = x.ExternalServiceCenterDetailId,
                ServiceCenterJobId = x.ServiceCenterJobId,
                Description = x.Description,
                Remark = x.Remark,
                Unit = x.Unit,
                Quantity = x.Quantity
            });

            return Json(Result);
        }

        public ActionResult Create([DataSourceRequest]DataSourceRequest request,ExternalServiceCenterDetail detail,string id)
        {
            if(ModelState.IsValid)
            {
                detail.ServiceCenterJobId = id;
                db.ExternalServiceCenterDetail.Add(detail);
                db.SaveChanges();
            }

            return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request,ExternalServiceCenterDetail detail)
        {
            var fetchDetail = db.ExternalServiceCenterDetail.FirstOrDefault(x => x.ExternalServiceCenterDetailId == detail.ExternalServiceCenterDetailId);
            fetchDetail.Description = detail.Description;
            fetchDetail.Remark = detail.Remark;
            fetchDetail.Quantity = detail.Quantity;
            fetchDetail.Unit = detail.Unit;
            if(ModelState.IsValid)
            {
                db.Entry(fetchDetail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request,ExternalServiceCenterDetail detail)
        {
            var fetchDetail = db.ExternalServiceCenterDetail.FirstOrDefault(x => x.ExternalServiceCenterDetailId == detail.ExternalServiceCenterDetailId);
            if(ModelState.IsValid)
            {
                db.ExternalServiceCenterDetail.Remove(fetchDetail);
                db.SaveChanges();
            }
            return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}