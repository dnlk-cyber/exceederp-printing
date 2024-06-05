using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class ExternalComponentDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExternalComponentDetail_Read([DataSourceRequest] DataSourceRequest request , Guid Id)
        {
            var ExternalComponentDetail = db.ExternalComponentDetail.Where(x=>x.ExternalComponentId == Id).ToList();

            return Json(ExternalComponentDetail.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalComponentDetail_Create([DataSourceRequest] DataSourceRequest request, ExternalComponentDetail ExternalComponentDetail, Guid Id)
        {
            ModelState.Remove("ExternalComponentDetailId");
            ModelState.Remove("ExternalComponentId");

            ExternalComponentDetail.ExternalComponentDetailId = Guid.NewGuid();
            ExternalComponentDetail.ExternalComponentId = Id;
            ExternalComponentDetail.DateCreated = DateTime.Now;
            ExternalComponentDetail.CreatedBy = User.Identity.Name;

            db.ExternalComponentDetail.Add(ExternalComponentDetail);
            db.SaveChanges();


            return Json(new[] { ExternalComponentDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalComponentDetail_Update([DataSourceRequest] DataSourceRequest request, ExternalComponentDetail ExternalComponentDetail)
        {
            if (ExternalComponentDetail != null)
            {
                ExternalComponentDetail.ModifiedBy = User.Identity.Name;
                ExternalComponentDetail.LastModified = DateTime.Now;

                db.ExternalComponentDetail.Attach(ExternalComponentDetail);
                db.Entry(ExternalComponentDetail).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ExternalComponentDetail }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalComponentDetail_Destroy([DataSourceRequest] DataSourceRequest request, ExternalComponentDetail ExternalComponentDetail)
        {
            var ExternalComponentDetails = db.ExternalComponentDetail.Where(x => x.ExternalComponentDetailId == ExternalComponentDetail.ExternalComponentDetailId).FirstOrDefault();
            if (ExternalComponentDetails != null)
            {

                db.ExternalComponentDetail.Attach(ExternalComponentDetails);
                db.ExternalComponentDetail.Remove(ExternalComponentDetails);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted External Component Detail number " + ExternalComponentDetail.ExternalComponentDetailId);

            }

            return Json(new[] { ExternalComponentDetail }.ToDataSourceResult(request));
        }



    }
}
