using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.MarketingUser)] 
    public class PreCustomerScheduleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PreCustomerSchedules_Read([DataSourceRequest]DataSourceRequest request,string preCustomerID)
        {
            IQueryable<PreCustomerSchedule> precustomerschedules = db.PreCustomerSchedules.Where(pcs=>pcs.PreCustomerID.ToString()==preCustomerID);
            DataSourceResult result = precustomerschedules.ToDataSourceResult(request, preCustomerSchedule => new {
                PreCustomerScheduleID = preCustomerSchedule.PreCustomerScheduleID,
                PreCustomerID = preCustomerSchedule.PreCustomerID,
                ScheduledDate = preCustomerSchedule.ScheduledDate,
                ScheduleStatus = preCustomerSchedule.ScheduleStatus,
                ScheduleOutcome = preCustomerSchedule.ScheduleOutcome,
                CreatedBy = preCustomerSchedule.CreatedBy,
                DateCreated = preCustomerSchedule.DateCreated,
                LastModified = preCustomerSchedule.LastModified,
                ModifiedBy = preCustomerSchedule.ModifiedBy,
                OrgBranchName = preCustomerSchedule.OrgBranchName,
                IsActive = preCustomerSchedule.IsActive
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreCustomerSchedules_Create([DataSourceRequest]DataSourceRequest request, PreCustomerSchedule preCustomerSchedule, string preCustomerCreateID)
        {
            if (ModelState.IsValid)
            {
                var entity = new PreCustomerSchedule
                {
                    PreCustomerID = preCustomerCreateID,
                    ScheduledDate = preCustomerSchedule.ScheduledDate,
                    ScheduleStatus = preCustomerSchedule.ScheduleStatus,
                    ScheduleOutcome = preCustomerSchedule.ScheduleOutcome,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    IsActive = preCustomerSchedule.IsActive
                };

                db.PreCustomerSchedules.Add(entity);
                db.SaveChanges();
                preCustomerSchedule.PreCustomerScheduleID = entity.PreCustomerScheduleID;
            }

            return Json(new[] { preCustomerSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreCustomerSchedules_Update([DataSourceRequest]DataSourceRequest request, PreCustomerSchedule preCustomerSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new PreCustomerSchedule
                {
                    PreCustomerScheduleID = preCustomerSchedule.PreCustomerScheduleID,
                    PreCustomerID = preCustomerSchedule.PreCustomerID,
                    ScheduledDate = preCustomerSchedule.ScheduledDate,
                    ScheduleStatus = preCustomerSchedule.ScheduleStatus,
                    ScheduleOutcome = preCustomerSchedule.ScheduleOutcome,
                    IsActive = preCustomerSchedule.IsActive
                };

                db.PreCustomerSchedules.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { preCustomerSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreCustomerSchedules_Destroy([DataSourceRequest]DataSourceRequest request, PreCustomerSchedule preCustomerSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new PreCustomerSchedule
                {
                    PreCustomerScheduleID = preCustomerSchedule.PreCustomerScheduleID,
                    PreCustomerID = preCustomerSchedule.PreCustomerID,
                    ScheduledDate = preCustomerSchedule.ScheduledDate,
                    ScheduleStatus = preCustomerSchedule.ScheduleStatus,
                    ScheduleOutcome = preCustomerSchedule.ScheduleOutcome,
                    IsActive = preCustomerSchedule.IsActive
                };

                db.PreCustomerSchedules.Attach(entity);
                db.PreCustomerSchedules.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted PreCustomer Schedule number " + preCustomerSchedule.PreCustomerID);
      
            }

            return Json(new[] { preCustomerSchedule }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
