
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = HR_Admin +  "," + HR_Benefit_Admin)]
    public class AllowanceSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Benefit_Admin = "HR_Benefit_Admin";
        private const string HR_Benefit_User = "HR_Benefit_User";
        private const string HR_Admin = "HR_Admin";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);
            ViewData["jobTitles"] = lookups;
            return View();
        }
           [Authorize(Roles = HR_Admin + "," + HR_Benefit_Admin + "," + HR_Benefit_User)]
        public ActionResult AllowanceSettings_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<AllowanceSetting> allowances = db.AllowanceSettings;
            DataSourceResult result = allowances.ToDataSourceResult(request, allowance => new {
                AllowanceSettingId = allowance.AllowanceSettingId,
                AllowanceType = allowance.AllowanceType,
                IsOnlyForHead = allowance.IsOnlyForHead,
                LookupId = allowance.LookupId,
                DateCreated = allowance.DateCreated,
                LastModified = allowance.LastModified,
                CreatedBy = allowance.CreatedBy,
                ModifiedBy = allowance.ModifiedBy,
            });

            return Json(result);
        }
  [Authorize(Roles = HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceSettings_Create([DataSourceRequest]DataSourceRequest request, AllowanceSetting allowance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {


                var entity = GetMappedObject(allowance);
                db.AllowanceSettings.Add(entity);
                db.SaveChanges();
                allowance.AllowanceSettingId = entity.AllowanceSettingId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, " allowance setting", "Success", User.Identity.Name, $"created new allowance name {entity.AllowanceType.ToString()} and type {entity.AllowanceType} ", Modules.Payroll);
                #endregion
                
            }

            return Json(new[] { allowance }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles =  HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceSettings_Update([DataSourceRequest]DataSourceRequest request, AllowanceSetting allowance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(allowance);
                db.AllowanceSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, " allowance setting", "Success", User.Identity.Name, $"updated allowance name {entity.AllowanceType} and type {entity.AllowanceType}", Modules.Payroll);
                #endregion

                //}

            }

            return Json(new[] { allowance }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = HR_Admin + "," + HR_Benefit_Admin )]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceSettings_Destroy([DataSourceRequest]DataSourceRequest request, AllowanceSetting allowance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(allowance);
                db.AllowanceSettings.Attach(entity);
                db.AllowanceSettings.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Allowance Setting", "Success", User.Identity.Name, $"deleted allowance name {allowance.AllowanceSettingId} and type {allowance.AllowanceType}", Modules.Payroll);
                #endregion
                
            }

            return Json(new[] { allowance }.ToDataSourceResult(request, ModelState));
        }

        private AllowanceSetting GetMappedObject(AllowanceSetting allowance)
        {
            var entity = new AllowanceSetting
            {
                AllowanceSettingId = allowance.AllowanceSettingId,
                AllowanceType = allowance.AllowanceType,
                IsOnlyForHead = allowance.IsOnlyForHead,
                LookupId = allowance.LookupId,
                DateCreated = allowance.DateCreated,
                LastModified = allowance.LastModified,
                CreatedBy = allowance.CreatedBy,
                ModifiedBy = allowance.ModifiedBy
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
