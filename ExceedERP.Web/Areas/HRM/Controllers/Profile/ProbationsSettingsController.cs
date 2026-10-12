using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Global;
namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    public class ProbationsSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ProbationsSettings
        public ActionResult Index()
        {
            return View();

        }
        private void PopulateViewData()
        {
            var LeaveSetting = Enum.GetValues(typeof(LeaveSetting))
                .Cast<LeaveSetting>().Select(v => v.ToString()).ToList();

            var LeaveSettings = LeaveSetting.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["LeaveSetting"] = LeaveSettings;
        }
        public ActionResult ProbationsSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            var probations = db.ProbationsSettings.ToList();
            DataSourceResult result = probations.ToDataSourceResult(request, probation => new
            {
                ProbationsSettingId = probation.ProbationsSettingId,
                ResponsibilityLevel = probation.ResponsibilityLevel,
                Remark = probation.Remark,
                ProbationDays = probation.ProbationDays,
                MinimumFitValue = probation.MinimumFitValue,
                CreatedBy = probation.CreatedBy,
                DateCreated = probation.DateCreated,
                LastModified = probation.LastModified,
                ModifiedBy = probation.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationsSettings_Create([DataSourceRequest]DataSourceRequest request, ProbationsSetting probation)
        {
            var check =db.ProbationsSettings.Where(p=>p.ResponsibilityLevel==probation.ResponsibilityLevel).FirstOrDefault();
            if (check != null)
            {
                ModelState.AddModelError("", "you can't add more than one records");
                return Json(new[] { probation }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                probation.CreatedBy = User.Identity.Name;
                probation.DateCreated = DateTime.Now;
                var entity = GetMappedObject(probation);

                db.ProbationsSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "probation Setting", "Success", User.Identity.Name, "probation Setting is Created By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationsSettings_Update([DataSourceRequest]DataSourceRequest request, ProbationsSetting probationsSetting)
        {
            var check = db.ProbationsSettings.Where(p => p.ResponsibilityLevel == probationsSetting.ResponsibilityLevel
                                                        &&p.ProbationsSettingId != probationsSetting.ProbationsSettingId)
                                                        .FirstOrDefault();
            if (check != null)
            {
                ModelState.AddModelError("", "you can't add more than one records");
                return Json(new[] { probationsSetting }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                probationsSetting.ModifiedBy = User.Identity.Name;
                probationsSetting.LastModified = DateTime.Now;
                var entity = GetMappedObject(probationsSetting);
                db.ProbationsSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "probation Setting", "Success", User.Identity.Name, "probation Setting is Updated By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { probationsSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationsSettings_Destroy([DataSourceRequest]DataSourceRequest request, ProbationsSetting probation)
        {

            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(probation);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted probation setting " + probation.ProbationsSettingId);
                #endregion
                db.ProbationsSettings.Attach(entity);
                db.ProbationsSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        private ProbationsSetting GetMappedObject(ProbationsSetting probation)
        {
            var entity = new ProbationsSetting
            {
                ProbationsSettingId = probation.ProbationsSettingId,
                ResponsibilityLevel = probation.ResponsibilityLevel,
                Remark = probation.Remark,
                ProbationDays = probation.ProbationDays,
                MinimumFitValue = probation.MinimumFitValue,
                CreatedBy = probation.CreatedBy,
                DateCreated = probation.DateCreated,
                LastModified = probation.LastModified,
                ModifiedBy = probation.ModifiedBy,

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