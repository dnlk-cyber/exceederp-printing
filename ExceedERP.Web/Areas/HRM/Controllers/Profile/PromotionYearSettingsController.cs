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

    public class PromotionYearSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/RetirementSetting
        public ActionResult Index()
        {
            return View();
        }
        private void PopulateViewData()
        {
            var LeaveSetting = Enum.GetValues(typeof(LeaveSetting))
                .Cast<LeaveSetting>()
                .Select(v => v.ToString())
                .ToList();
            var LeaveSettings = LeaveSetting.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["LeaveSetting"] = LeaveSettings;
        }
        public ActionResult PromotionYearSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            var promotions = db.PromotionYearSettings.ToList();
            DataSourceResult result = promotions.ToDataSourceResult(request, promotion => new
            {

                PromotionYearSettingId = promotion.PromotionYearSettingId,
                NumberOfPromotionYears = promotion.NumberOfPromotionYears,
                StepAmountsToPromote = promotion.StepAmountsToPromote,
                CreatedBy = promotion.CreatedBy,
                DateCreated = promotion.DateCreated,
                LastModified = promotion.LastModified,
                ModifiedBy = promotion.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PromotionYearSettings_Create([DataSourceRequest]DataSourceRequest request, PromotionYearSetting promotion)
        {
            var check =db.PromotionYearSettings.FirstOrDefault();
            if (check != null)
            {
                ModelState.AddModelError("", "you can't add more than one records");
                return Json(new[] { promotion }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(promotion);

                db.PromotionYearSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "promotion Setting", "Success", User.Identity.Name, "promotion Setting is Created By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { promotion }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PromotionYearSettings_Update([DataSourceRequest]DataSourceRequest request, PromotionYearSetting promotion)
        {

            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(promotion);
                db.PromotionYearSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "promotion Setting", "Success", User.Identity.Name, "promotion Setting is Updated By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { promotion }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PromotionYearSettings_Destroy([DataSourceRequest]DataSourceRequest request, PromotionYearSetting promotion)
        {

            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(promotion);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted promotion setting " + promotion.PromotionYearSettingId);
                #endregion
                db.PromotionYearSettings.Attach(entity);
                db.PromotionYearSettings.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { promotion }.ToDataSourceResult(request, ModelState));
        }

        private PromotionYearSetting GetMappedObject(PromotionYearSetting promotion)
        {
            var entity = new PromotionYearSetting
            {
                PromotionYearSettingId = promotion.PromotionYearSettingId,
                NumberOfPromotionYears = promotion.NumberOfPromotionYears,
                StepAmountsToPromote = promotion.StepAmountsToPromote,
                CreatedBy = promotion.CreatedBy,
                DateCreated = promotion.DateCreated,
                LastModified = promotion.LastModified,
                ModifiedBy = promotion.ModifiedBy,

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