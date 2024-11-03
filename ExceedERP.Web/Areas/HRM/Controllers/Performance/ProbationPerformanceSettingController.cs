using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Global;
using System.Web.Mvc;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead,
        HrmConstants.HRPerformanceUser,
        HrmConstants.HRPerformanceAdmin,
        HrmConstants.HREmployeeSelfService)]
    public class ProbationPerformanceSettingController : Controller
    {
        // GET: HRM/ProbationPerformanceSetting
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Employees"] = db.Person_Employee.ToList();
            ViewData["Org"] = db.MainCompanyStructures.ToList();
            ViewData["Jobtitle"] = db.Lookups.ToList();
            ViewData["Measurments"] = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.PROBATION_MEASUREMENT).ToList();
        }
        public ActionResult ProbationPerformanceSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProbationPerformanceSetting> settings = db.ProbationPerformanceSettings;
            DataSourceResult result = settings.ToDataSourceResult(request, probationPerformanceSetting => new
            {

                ProbationPerformanceSettingId = probationPerformanceSetting.ProbationPerformanceSettingId,
                ProbationMeasurement = probationPerformanceSetting.ProbationMeasurement,
                IntialWeight = probationPerformanceSetting.IntialWeight,
                EndWeight = probationPerformanceSetting.EndWeight
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceSetting_Create([DataSourceRequest]DataSourceRequest request, ProbationPerformanceSetting probationPerformanceSetting)
        {

            if (ModelState.IsValid)
            {

                var entity = new ProbationPerformanceSetting
                {
                    ProbationMeasurement = probationPerformanceSetting.ProbationMeasurement,
                    IntialWeight = probationPerformanceSetting.IntialWeight,
                    EndWeight = probationPerformanceSetting.EndWeight
                };

                db.ProbationPerformanceSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Probation Performance Setting", "Success", User.Identity.Name, User.Identity.Name + " add Probation Performance setting called " + entity.ProbationPerformanceSettingId, Modules.HRM);

                #endregion

            }

            return Json(new[] { probationPerformanceSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceSetting_Update([DataSourceRequest]DataSourceRequest request, ProbationPerformanceSetting probationPerformanceSetting)
        {
            if (ModelState.IsValid)
            {

                var entity = new ProbationPerformanceSetting
                {
                    ProbationPerformanceSettingId = probationPerformanceSetting.ProbationPerformanceSettingId,
                    ProbationMeasurement = probationPerformanceSetting.ProbationMeasurement,
                    IntialWeight = probationPerformanceSetting.IntialWeight,
                    EndWeight = probationPerformanceSetting.EndWeight
                };

                db.ProbationPerformanceSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Probation Performance Setting", "Success", User.Identity.Name, User.Identity.Name + " update Probation Performance setting called " + entity.ProbationPerformanceSettingId, Modules.HRM);

                #endregion


            }

            return Json(new[] { probationPerformanceSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceSetting_Destroy([DataSourceRequest]DataSourceRequest request, ProbationPerformanceSetting probationPerformanceSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProbationPerformanceSetting
                {
                    ProbationPerformanceSettingId = probationPerformanceSetting.ProbationPerformanceSettingId,
                    ProbationMeasurement = probationPerformanceSetting.ProbationMeasurement,
                    IntialWeight = probationPerformanceSetting.IntialWeight,
                    EndWeight = probationPerformanceSetting.EndWeight
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Probation Performance setting" + entity.ProbationPerformanceSettingId);
                #endregion
                db.ProbationPerformanceSettings.Attach(entity);
                db.ProbationPerformanceSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { probationPerformanceSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
