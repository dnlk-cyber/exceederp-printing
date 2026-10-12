using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    public class EmployeeSalaryIncrementSettingController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/EmployeeSalaryIncrementSetting
        public ActionResult Index()
        {
            return View();
        }

        // READ
        public ActionResult EmployeeSalaryIncrementSetting_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<EmployeeSalaryIncrementSetting> settings = db.EmployeeSalaryIncrementSettings;
            DataSourceResult result = settings.ToDataSourceResult(request, setting => new
            {
                setting.EmployeeSalaryIncrementSettingId,
                setting.ResponsibilityLevel,
                setting.EligibilityServiceMonths,
                setting.Remark
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // CREATE
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeSalaryIncrementSetting_Create([DataSourceRequest] DataSourceRequest request, EmployeeSalaryIncrementSetting setting)
        {
            ModelState.Clear();

            // check duplicate ResponsibilityLevel
            var checkExistence = db.EmployeeSalaryIncrementSettings
                                   .FirstOrDefault(x => x.ResponsibilityLevel == setting.ResponsibilityLevel);
            if (checkExistence != null)
            {
                ModelState.AddModelError("", "A setting for this Responsibility Level already exists.");
                return Json(new[] { setting }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                db.EmployeeSalaryIncrementSettings.Add(setting);
                db.SaveChanges();

                // Operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Salary Increment Settings", "Success", User.Identity.Name,
                    $"Created EmployeeSalaryIncrementSetting {setting.EmployeeSalaryIncrementSettingId}", Modules.HRM);
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        // UPDATE
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeSalaryIncrementSetting_Update([DataSourceRequest] DataSourceRequest request, EmployeeSalaryIncrementSetting setting)
        {
            ModelState.Clear();

            var checkExistence = db.EmployeeSalaryIncrementSettings
                                   .FirstOrDefault(x => x.ResponsibilityLevel == setting.ResponsibilityLevel
                                                     && x.EmployeeSalaryIncrementSettingId != setting.EmployeeSalaryIncrementSettingId);
            if (checkExistence != null)
            {
                ModelState.AddModelError("", "A setting for this Responsibility Level already exists.");
                return Json(new[] { setting }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                db.EmployeeSalaryIncrementSettings.Attach(setting);
                db.Entry(setting).State = EntityState.Modified;
                db.SaveChanges();

                // Operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Salary Increment Settings", "Success", User.Identity.Name,
                    $"Updated EmployeeSalaryIncrementSetting {setting.EmployeeSalaryIncrementSettingId}", Modules.HRM);
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        // DELETE
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeSalaryIncrementSetting_Destroy([DataSourceRequest] DataSourceRequest request, EmployeeSalaryIncrementSetting setting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                db.EmployeeSalaryIncrementSettings.Attach(setting);
                db.EmployeeSalaryIncrementSettings.Remove(setting);
                db.SaveChanges();

                // Delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name,
                    $"Deleted EmployeeSalaryIncrementSetting {setting.EmployeeSalaryIncrementSettingId}");
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