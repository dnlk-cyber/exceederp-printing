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
    public class PerformanceAllowedScoreController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PerformanceAllowedScore_Read([DataSourceRequest] DataSourceRequest request)
        {
            var scores = db.PerformanceAllowedScores
                           .OrderBy(x => x.Order);

            return Json(scores.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult PerformanceAllowedScore_Create(
            [DataSourceRequest] DataSourceRequest request,
            PerformanceAllowedScore model)
        {
            if (ModelState.IsValid)
            {
                db.PerformanceAllowedScores.Add(model);
                db.SaveChanges();

                #region Log
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Performance Allowed Score",
                    "Success",
                    User.Identity.Name,
                    $"{User.Identity.Name} added score value {model.ScoreValue}",
                    Modules.HRM);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult PerformanceAllowedScore_Update(
            [DataSourceRequest] DataSourceRequest request,
            PerformanceAllowedScore model)
        {
            if (ModelState.IsValid)
            {
                db.PerformanceAllowedScores.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                #region Log
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Performance Allowed Score",
                    "Success",
                    User.Identity.Name,
                    $"{User.Identity.Name} updated score value {model.ScoreValue}",
                    Modules.HRM);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult PerformanceAllowedScore_Destroy(
            [DataSourceRequest] DataSourceRequest request,
            PerformanceAllowedScore model)
        {
            var entity = db.PerformanceAllowedScores
                           .Find(model.PerformanceAllowedScoreId);

            if (entity != null)
            {
                db.PerformanceAllowedScores.Remove(entity);
                db.SaveChanges();

                #region Delete Log
                UserHelper.DeleteLog(
                    Request.UserHostAddress,
                    "Delete",
                    "Success",
                    User.Identity.Name,
                    $"Deleted Performance Allowed Score {entity.ScoreValue}");
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult EnableOrDisable(int id)
        {
            var setting = db.PerformanceAllowedScores.Find(id);

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
                "Performance Allowed Scores Setting",
                "Success",
                User.Identity.Name,
                $"{userFullName} toggled PerformanceAllowedScore ID={id}, IsActive={setting.IsActive}",
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
