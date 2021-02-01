using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.Equipment_Plan_User, FleetManagementRoles.Equipment_Plan_Approver)]
    public class PMScheduleDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PMScheduleDetails_Read([DataSourceRequest]DataSourceRequest request, string Id)
        {
            IQueryable<PMScheduleDetail> PMScheduleDetails = db.PMScheduleDetails.Where(x => x.PMScheduleId.ToString() == Id);
            DataSourceResult result = PMScheduleDetails.ToDataSourceResult(request, PMScheduleDetail => new
            {
                PMScheduleDetailId = PMScheduleDetail.PMScheduleDetailId,
                ServiceId = PMScheduleDetail.ServiceId,
                FleetsId = PMScheduleDetail.FleetsId,
                GLPeriodId = PMScheduleDetail.GLPeriodId,
                Weeks = PMScheduleDetail.Weeks,
                Remark = PMScheduleDetail.Remark,
                DateCreated = PMScheduleDetail.DateCreated,
                LastModified = PMScheduleDetail.LastModified,
                CreatedBy = PMScheduleDetail.CreatedBy,
                ModifiedBy = PMScheduleDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PMScheduleDetails_Create([DataSourceRequest]DataSourceRequest request, PMScheduleDetail PMScheduleDetail, string Id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PMScheduleDetail
                {
                    PMScheduleId = int.Parse(Id),
                    ServiceId = PMScheduleDetail.ServiceId,
                    GLPeriodId = PMScheduleDetail.GLPeriodId,
                    FleetsId = PMScheduleDetail.FleetsId,
                    Weeks = PMScheduleDetail.Weeks,
                    Remark = PMScheduleDetail.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.PMScheduleDetails.Add(entity);
                db.SaveChanges();
                PMScheduleDetail.PMScheduleDetailId = entity.PMScheduleDetailId;
            }

            return Json(new[] { PMScheduleDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PMScheduleDetails_Update([DataSourceRequest]DataSourceRequest request, PMScheduleDetail PMScheduleDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new PMScheduleDetail
                {
                    PMScheduleDetailId = PMScheduleDetail.PMScheduleDetailId,
                    DateCreated = PMScheduleDetail.DateCreated,
                    LastModified = PMScheduleDetail.LastModified,
                    CreatedBy = PMScheduleDetail.CreatedBy,
                    ModifiedBy = PMScheduleDetail.ModifiedBy
                };

                db.PMScheduleDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PMScheduleDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PMScheduleDetails_Destroy([DataSourceRequest]DataSourceRequest request, PMScheduleDetail PMScheduleDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new PMScheduleDetail
                {
                    PMScheduleDetailId = PMScheduleDetail.PMScheduleDetailId,
                    DateCreated = PMScheduleDetail.DateCreated,
                    LastModified = PMScheduleDetail.LastModified,
                    CreatedBy = PMScheduleDetail.CreatedBy,
                    ModifiedBy = PMScheduleDetail.ModifiedBy
                };

                db.PMScheduleDetails.Attach(entity);
                db.PMScheduleDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { PMScheduleDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
