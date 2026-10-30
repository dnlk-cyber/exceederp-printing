using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Termination
{
    public class TerminationSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/TerminationSettings
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TerminationSettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            var settings = db.TerminationSettings.ToList();

            var result = settings.ToDataSourceResult(request, s => new
            {
                TerminationSettingId = s.TerminationSettingId,
                ResponsibilityLevel = s.ResponsibilityLevel,
                NoticePeriodDays = s.NoticePeriodDays,
                IsActive = s.IsActive,
                CreatedBy = s.CreatedBy,
                DateCreated = s.DateCreated,
                LastModified = s.LastModified,
                ModifiedBy = s.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TerminationSettings_Create([DataSourceRequest] DataSourceRequest request, TerminationSetting setting)
        {
            // 🚫 Prevent duplicate ResponsibilityLevel (important since unique index exists)
            var exists = db.TerminationSettings
                .Any(x => x.ResponsibilityLevel == setting.ResponsibilityLevel);

            if (exists)
            {
                ModelState.AddModelError("", "Setting already exists for this Responsibility Level.");
                return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                setting.CreatedBy = User.Identity.Name;
                setting.DateCreated = DateTime.Now;
                db.TerminationSettings.Add(setting);
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "Termination Setting", "Success",
                    User.Identity.Name, "Termination Setting created", Modules.HRM);
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TerminationSettings_Update([DataSourceRequest] DataSourceRequest request, TerminationSetting setting)
        {
            // 🚫 Prevent duplicate on update
            var exists = db.TerminationSettings
                .Any(x => x.ResponsibilityLevel == setting.ResponsibilityLevel
                       && x.TerminationSettingId != setting.TerminationSettingId);

            if (exists)
            {
                ModelState.AddModelError("", "Setting already exists for this Responsibility Level.");
                return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                setting.ModifiedBy = User.Identity.Name;
                setting.LastModified = DateTime.Now;

                db.TerminationSettings.Attach(setting);
                db.Entry(setting).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "Termination Setting", "Success",
                    User.Identity.Name, "Termination Setting updated", Modules.HRM);
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TerminationSettings_Destroy([DataSourceRequest] DataSourceRequest request, TerminationSetting setting)
        {
            if (ModelState.IsValid)
            {
                var entity = db.TerminationSettings.Find(setting.TerminationSettingId);

                if (entity != null)
                {
                    db.TerminationSettings.Remove(entity);
                    db.SaveChanges();

                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success",
                        User.Identity.Name, "Deleted Termination Setting " + setting.TerminationSettingId);
                }
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult EnableOrDisable(int id)
        {
            var setting = db.TerminationSettings.Find(id);

            if (setting == null)
            {
                return Json(new
                {
                    Message = "The Termination Settings was not found.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            // Toggle IsActive
            setting.IsActive = !setting.IsActive;
            db.Entry(setting).State = EntityState.Modified;
            db.SaveChanges();
            // Logging
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            string userFullName = User.Identity.Name;

            if (empId != 0)
            {
                var employee = db.Person_Employee.Find(empId);
                if (employee != null)
                {
                    userFullName = employee.FullNameaEnglish;
                }
            }

            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Termination Settings",
                "Success",
                User.Identity.Name,
                $"{userFullName} toggled TerminationSettings ID={id}, IsActive={setting.IsActive}",
                Modules.HRM
            );

            return Json(new
            {
                Message = setting.IsActive ? "You successfully enabled this setting." : "You successfully disabled this setting.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}