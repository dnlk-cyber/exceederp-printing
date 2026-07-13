using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Severance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Severance
{
    [AuthorizeRoles(HrmConstants.HRAdmin)]
    public class SeveranceCalculationSettingController : BaseController
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/SeveranceCalculationSetting
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult SeveranceCalculationSetting_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<SeveranceCalculationSetting> settings = db.SeveranceCalculationSettings;

            DataSourceResult result = settings.ToDataSourceResult(request, setting => new
            {
                SeveranceCalculationSettingId = setting.SeveranceCalculationSettingId,
                FormulaType = setting.FormulaType,
                IsActive = setting.IsActive,
                Description = setting.Description,
                DateCreated = setting.DateCreated,
                CreatedBy = setting.CreatedBy,
                ModifiedBy = setting.ModifiedBy,
                LastModified = setting.LastModified
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeveranceCalculationSetting_Create( [DataSourceRequest] DataSourceRequest request, SeveranceCalculationSetting setting)
        {
            if (ModelState.IsValid)
            {
                // Only one active setting allowed
                if (setting.IsActive)
                {
                    var activeSettings = db.SeveranceCalculationSettings
                        .Where(x => x.IsActive)
                        .ToList();

                    foreach (var item in activeSettings)
                    {
                        item.IsActive = false;
                        db.Entry(item).State = EntityState.Modified;
                    }
                }

                var entity = GetMappedObject(setting);

                db.SeveranceCalculationSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Severance Calculation Setting",
                    "Success",
                    User.Identity.Name,
                    "Severance Calculation Setting Created By " + User.Identity.Name,
                    Modules.HRM);

                #endregion
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeveranceCalculationSetting_Update([DataSourceRequest] DataSourceRequest request,SeveranceCalculationSetting setting)
        {
            if (ModelState.IsValid)
            {
                // Only one active setting allowed
                if (setting.IsActive)
                {
                    var activeSettings = db.SeveranceCalculationSettings
                        .Where(x => x.IsActive &&
                                    x.SeveranceCalculationSettingId != setting.SeveranceCalculationSettingId)
                        .ToList();

                    foreach (var item in activeSettings)
                    {
                        item.IsActive = false;
                        db.Entry(item).State = EntityState.Modified;
                    }
                }

                var entity = GetMappedObject(setting);

                db.SeveranceCalculationSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;

                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Severance Calculation Setting",
                    "Success",
                    User.Identity.Name,
                    "Severance Calculation Setting Updated By " + User.Identity.Name,
                    Modules.HRM);

                #endregion
            }

            return Json(new[] { setting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeveranceCalculationSetting_Destroy( [DataSourceRequest] DataSourceRequest request, SeveranceCalculationSetting calculationSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(calculationSetting);

                #region delete log

                UserHelper.DeleteLog(
                    Request.UserHostAddress,
                    "Delete",
                    "Success",
                    User.Identity.Name,
                    "Deleted by " + User.Identity.Name);

                #endregion

                db.SeveranceCalculationSettings.Attach(entity);
                db.SeveranceCalculationSettings.Remove(entity);

                db.SaveChanges();
            }

            return Json(new[] { calculationSetting }.ToDataSourceResult(request, ModelState));
        }

        private SeveranceCalculationSetting GetMappedObject(SeveranceCalculationSetting setting)
        {
            var entity = new SeveranceCalculationSetting
            {
                SeveranceCalculationSettingId = setting.SeveranceCalculationSettingId,
                FormulaType = setting.FormulaType,
                IsActive = setting.IsActive,
                Description = setting.Description,
                DateCreated = setting.DateCreated,
                CreatedBy = setting.CreatedBy,
                ModifiedBy = setting.ModifiedBy,
                LastModified = setting.LastModified
            };

            return entity;
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            #region export log

            UserHelper.ExportLog(
                Request.UserHostAddress,
                "Excel-Export",
                "Success",
                User.Identity.Name,
                "Exported Severance Calculation Setting");

            #endregion

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            #region export log

            UserHelper.ExportLog(
                Request.UserHostAddress,
                "PDF-Export",
                "Success",
                User.Identity.Name,
                "Exported Severance Calculation Setting");

            #endregion

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}