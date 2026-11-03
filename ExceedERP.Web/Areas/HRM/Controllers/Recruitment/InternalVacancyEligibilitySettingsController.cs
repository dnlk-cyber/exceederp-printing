using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
    public class InternalVacancyEligibilitySettingsController : BaseController
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InternalVacancyEligibilitySettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            var settings = db.InternalVacancyEligibilitySettings.ToList();

            DataSourceResult result = settings.ToDataSourceResult(request, setting => new
            {
                InternalVacancyEligibilitySettingId = setting.InternalVacancyEligibilitySettingId,
                EnforceMinimumServicePeriod = setting.EnforceMinimumServicePeriod,
                MinimumYearsOfService = setting.MinimumYearsOfService,
                AllowContractEmployees = setting.AllowContractEmployees,
                AllowStandbyTechnicians = setting.AllowStandbyTechnicians,
                AllowHROverride = setting.AllowHROverride,
                CreatedBy = setting.CreatedBy,
                DateCreated = setting.DateCreated,
                ModifiedBy = setting.ModifiedBy,
                LastModified = setting.LastModified
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalVacancyEligibilitySettings_Create([DataSourceRequest] DataSourceRequest request,
            InternalVacancyEligibilitySetting setting)
        {
            // Allow only one settings record
            if (db.InternalVacancyEligibilitySettings.Any())
            {
                ModelState.AddModelError("", "Only one Internal Vacancy Eligibility Setting record is allowed.");
                return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                setting.CreatedBy = User.Identity.Name;
                setting.DateCreated = DateTime.Now;

                var entity = GetMappedObject(setting);

                db.InternalVacancyEligibilitySettings.Add(entity);
                db.SaveChanges();

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Internal Vacancy Eligibility Setting",
                    "Success",
                    User.Identity.Name,
                    "Internal Vacancy Eligibility Setting created by " + User.Identity.Name,
                    Modules.HRM);
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalVacancyEligibilitySettings_Update([DataSourceRequest] DataSourceRequest request,
            InternalVacancyEligibilitySetting setting)
        {
            if (ModelState.IsValid)
            {
                setting.ModifiedBy = User.Identity.Name;
                setting.LastModified = DateTime.Now;

                var entity = GetMappedObject(setting);

                db.InternalVacancyEligibilitySettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Internal Vacancy Eligibility Setting",
                    "Success",
                    User.Identity.Name,
                    "Internal Vacancy Eligibility Setting updated by " + User.Identity.Name,
                    Modules.HRM);
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalVacancyEligibilitySettings_Destroy(
            [DataSourceRequest] DataSourceRequest request,
            InternalVacancyEligibilitySetting setting)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(setting);

                UserHelper.DeleteLog(
                    Request.UserHostAddress,
                    "Delete",
                    "Success",
                    User.Identity.Name,
                    "Deleted Internal Vacancy Eligibility Setting " + setting.InternalVacancyEligibilitySettingId);

                db.InternalVacancyEligibilitySettings.Attach(entity);
                db.InternalVacancyEligibilitySettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        private InternalVacancyEligibilitySetting GetMappedObject(
            InternalVacancyEligibilitySetting setting)
        {
            return new InternalVacancyEligibilitySetting
            {
                InternalVacancyEligibilitySettingId = setting.InternalVacancyEligibilitySettingId,

                EnforceMinimumServicePeriod = setting.EnforceMinimumServicePeriod,
                MinimumYearsOfService = setting.MinimumYearsOfService,
                AllowContractEmployees = setting.AllowContractEmployees,
                AllowStandbyTechnicians = setting.AllowStandbyTechnicians,
                AllowHROverride = setting.AllowHROverride,

                CreatedBy = setting.CreatedBy,
                DateCreated = setting.DateCreated,
                ModifiedBy = setting.ModifiedBy,
                LastModified = setting.LastModified
            };
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
