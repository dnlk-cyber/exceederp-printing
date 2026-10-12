﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead,
        HrmConstants.HRPerformanceUser,
        HrmConstants.HRPerformanceAdmin)]
    public class PerformanceEvaluationSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PerformanceEvaluationSetting_Read([DataSourceRequest] DataSourceRequest request)
        {
            var settings = db.PerformanceEvaluationSettings;
            return Json(settings.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult PerformanceEvaluationSetting_Create(
            [DataSourceRequest] DataSourceRequest request,
            PerformanceEvaluationSetting model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsActive)
                {
                    var activeSettings = db.PerformanceEvaluationSettings
                                           .Where(x => x.IsActive)
                                           .ToList();

                    foreach (var item in activeSettings)
                        item.IsActive = false;
                }

                db.PerformanceEvaluationSettings.Add(model);
                db.SaveChanges();

                #region Log
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Performance Evaluation Setting",
                    "Success",
                    User.Identity.Name,
                    $"{User.Identity.Name} created Performance Evaluation Setting {model.Name}",
                    Modules.HRM);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult PerformanceEvaluationSetting_Update(
            [DataSourceRequest] DataSourceRequest request,
            PerformanceEvaluationSetting model)
        {
            if (ModelState.IsValid)
            {
                db.PerformanceEvaluationSettings.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                #region Log
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Performance Evaluation Setting",
                    "Success",
                    User.Identity.Name,
                    $"{User.Identity.Name} updated Performance Evaluation Setting {model.Name}",
                    Modules.HRM);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult PerformanceEvaluationSetting_Destroy(
            [DataSourceRequest] DataSourceRequest request,
            PerformanceEvaluationSetting model)
        {
            var entity = db.PerformanceEvaluationSettings
                           .Find(model.PerformanceEvaluationSettingId);

            if (entity != null)
            {
                db.PerformanceEvaluationSettings.Remove(entity);
                db.SaveChanges();

                #region Delete Log
                UserHelper.DeleteLog(
                    Request.UserHostAddress,
                    "Delete",
                    "Success",
                    User.Identity.Name,
                    $"Deleted Performance Evaluation Setting {entity.Name}");
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult EnableOrDisable(int id)
        {
            var setting = db.PerformanceEvaluationSettings.Find(id);

            if (setting == null)
            {
                return Json(new
                {
                    Message = "The setting was not found.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            // Toggle IsActive
            setting.IsActive = !setting.IsActive;
            db.Entry(setting).State = EntityState.Modified;
            db.SaveChanges();
            // Logging
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            string userFullName = User.Identity.Name;

            if (empId != 0)
            {
                var employee = db.Person_Employee.Find(empId);
                if (employee != null)
                {
                    userFullName = employee.FullNameaEnglish;
                }
            }

            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Performance Evaluation Settings",
                "Success",
                User.Identity.Name,
                $"{userFullName} toggled Performance Evaluation Setting ID={id}, IsActive={setting.IsActive}",
                Modules.HRM
            );

            return Json(new
            {
                Message = setting.IsActive ? "You successfully enabled this setting." : "You successfully disabled this setting.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
