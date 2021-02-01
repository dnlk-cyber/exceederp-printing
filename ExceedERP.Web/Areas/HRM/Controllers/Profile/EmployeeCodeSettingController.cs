using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
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
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class EmployeeCodeSettingController : Controller
    {
        // GET: HRM/EmployeeCodeSetting
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmployeeCodeSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeeCodeSetting> employeeCodeSettings = db.EmployeeCodeSettings;
            DataSourceResult result = employeeCodeSettings.ToDataSourceResult(request, employeeCodeSetting => new
            {
                EmployeeCodeSettingId = employeeCodeSetting.EmployeeCodeSettingId,
                Type = employeeCodeSetting.Type,
                Index = employeeCodeSetting.Index,
                Remark = employeeCodeSetting.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCodeSetting_Create([DataSourceRequest]DataSourceRequest request, EmployeeCodeSetting employeeCodeSetting)
        {
            ModelState.Clear();
            var checkTypeExistance = db.EmployeeCodeSettings.FirstOrDefault(x => x.Type == employeeCodeSetting.Type);
            if (checkTypeExistance != null)
            {
                ModelState.AddModelError("", "you can't save duplicate setting for the same type");
            }
            var checkIndexExistance = db.EmployeeCodeSettings.FirstOrDefault(x => x.Index == employeeCodeSetting.Index);
            if (checkIndexExistance != null)
            {
                ModelState.AddModelError("", "this index is taken by other type");
            }
            if (ModelState.IsValid)
            {

                var entity = new EmployeeCodeSetting
                {
                    Type = employeeCodeSetting.Type,
                    Index = employeeCodeSetting.Index,
                    Remark = employeeCodeSetting.Remark
                };

                db.EmployeeCodeSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Employee Code Settings", "Success", User.Identity.Name, "Create Employee Code Setting " + entity.EmployeeCodeSettingId, Modules.HRM);

                #endregion



            }

            return Json(new[] { employeeCodeSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCodeSetting_Update([DataSourceRequest]DataSourceRequest request, EmployeeCodeSetting employeeCodeSetting)
        {
            ModelState.Clear();
            var checkTypeExistance = db.EmployeeCodeSettings.FirstOrDefault(x => x.Type == employeeCodeSetting.Type && x.EmployeeCodeSettingId != employeeCodeSetting.EmployeeCodeSettingId);
            if (checkTypeExistance != null)
            {
                ModelState.AddModelError("", "you can't save duplicate setting for the same type");
            }
            var checkIndexExistance = db.EmployeeCodeSettings.FirstOrDefault(x => x.Index == employeeCodeSetting.Index && x.EmployeeCodeSettingId != employeeCodeSetting.EmployeeCodeSettingId);
            if (checkIndexExistance != null)
            {
                ModelState.AddModelError("", "this index is taken by other type");
            }
            if (ModelState.IsValid)
            {
                var entity = new EmployeeCodeSetting
                {
                    EmployeeCodeSettingId = employeeCodeSetting.EmployeeCodeSettingId,
                    Type = employeeCodeSetting.Type,
                    Index = employeeCodeSetting.Index,
                    Remark = employeeCodeSetting.Remark
                };

                db.EmployeeCodeSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Employee Code Settings", "Success", User.Identity.Name, "Update Employee Code Setting " + entity.EmployeeCodeSettingId, Modules.HRM);

                #endregion

            }
            return Json(new[] { employeeCodeSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCodeSetting_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeCodeSetting employeeCodeSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeCodeSetting
                {
                    EmployeeCodeSettingId = employeeCodeSetting.EmployeeCodeSettingId,
                    Type = employeeCodeSetting.Type,
                    Index = employeeCodeSetting.Index,
                    Remark = employeeCodeSetting.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Employee Code Setting " + entity.EmployeeCodeSettingId);
                #endregion
                db.EmployeeCodeSettings.Attach(entity);
                db.EmployeeCodeSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeCodeSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}