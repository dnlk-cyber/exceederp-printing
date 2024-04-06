using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class RetirementSettingController : BaseController
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
        public ActionResult RetirementSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            var retirements = db.RetirementSettings.ToList();
            DataSourceResult result = retirements.ToDataSourceResult(request, retirement => new
            {

                RetirementSettingId = retirement.RetirementSettingId,
                RetireSetting = retirement.RetireSetting,
                CurrentValue = retirement.CurrentValue

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RetirementSetting_Create([DataSourceRequest]DataSourceRequest request, RetirementSetting retirement)
        {

            if (ModelState.IsValid)
            {

                var entity = new RetirementSetting
                {
                    RetireSetting = retirement.RetireSetting,
                    CurrentValue = retirement.CurrentValue
                };

                db.RetirementSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Retirement Setting", "Success", User.Identity.Name, "Retirement Setting is Created By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { retirement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RetirementSetting_Update([DataSourceRequest]DataSourceRequest request, RetirementSetting retirement)
        {

            if (ModelState.IsValid)
            {
                var entity = new RetirementSetting
                {
                    RetirementSettingId = retirement.RetirementSettingId,
                    RetireSetting = retirement.RetireSetting,
                    CurrentValue = retirement.CurrentValue
                };


                db.RetirementSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Retirement Setting", "Success", User.Identity.Name, "Retirement Setting is Updated By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { retirement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RetirementSetting_Destroy([DataSourceRequest]DataSourceRequest request, RetirementSetting retirement)
        {

            if (ModelState.IsValid)
            {

                var entity = new RetirementSetting
                {
                    RetirementSettingId = retirement.RetirementSettingId,
                    RetireSetting = retirement.RetireSetting,
                    CurrentValue = retirement.CurrentValue
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted retirement setting " + retirement.RetireSetting);
                #endregion
                db.RetirementSettings.Attach(entity);
                db.RetirementSettings.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { retirement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}