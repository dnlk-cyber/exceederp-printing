using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Web.Controllers;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.Company;
using System;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class SalarySettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult SalarySettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<SalarySetting> settings = db.SalarySettings;

            var result = settings.ToDataSourceResult(request, s => new
            {
                s.SalarySettingId,
                s.IsJobtitleIncludedInSalary,
                s.CreatedBy,
                s.DateCreated,
                s.ModifiedBy,
                s.LastModified,
                s.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        public ActionResult SalarySettings_Create([DataSourceRequest] DataSourceRequest request, SalarySetting setting)
        {
            ModelState.Clear();

            // Prevent more than one record (similar to RewardSetting)
            var existing = db.SalarySettings.FirstOrDefault();
            if (existing != null)
            {
                ModelState.AddModelError("", "You cannot add more than one Salary Setting record.");
                return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
            }
            setting.CreatedBy = User.Identity.Name;
            setting.DateCreated = DateTime.Now;

            if (ModelState.IsValid)
            {
                var entity = new SalarySetting
                {
                    IsJobtitleIncludedInSalary = setting.IsJobtitleIncludedInSalary,
                    DateCreated = setting.DateCreated,
                    CreatedBy = setting.CreatedBy,
                    ModifiedBy = setting.ModifiedBy,
                    LastModified = setting.LastModified,
                    Remark = setting.Remark
                };

                db.SalarySettings.Add(entity);
                db.SaveChanges();

                setting.SalarySettingId = entity.SalarySettingId;
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        public ActionResult SalarySettings_Update([DataSourceRequest] DataSourceRequest request, SalarySetting setting)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = db.SalarySettings.FirstOrDefault(x => x.SalarySettingId == setting.SalarySettingId);
                if (entity != null)
                {
                    entity.IsJobtitleIncludedInSalary = setting.IsJobtitleIncludedInSalary;
                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;
                    entity.Remark = setting.Remark;

                    db.SaveChanges();
                }
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        public ActionResult SalarySettings_Destroy([DataSourceRequest] DataSourceRequest request, SalarySetting setting)
        {
            ModelState.Clear();

            var entity = db.SalarySettings.FirstOrDefault(x => x.SalarySettingId == setting.SalarySettingId);
            if (entity != null)
            {
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name,
                    "Deleted salary setting. Id: " + setting.SalarySettingId);
                #endregion

                db.SalarySettings.Remove(entity);
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
