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
     [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,HrmConstants.HRAdmin)]
    public class EmployeePeerToPeerAndInternalCustomerPerformanceSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeePeerToPeerAndInternalCustomerPerformanceSetting> performances = db.EmployeePeerToPeerAndInternalCustomerPerformanceSettings;
            DataSourceResult result = performances.ToDataSourceResult(request, performance => new {
                EmployeePeerToPeerAndInternalCustomerPerformanceSettingId = performance.EmployeePeerToPeerAndInternalCustomerPerformanceSettingId,
                IsInternalCustomerHasMultipleEvaluator = performance.IsInternalCustomerHasMultipleEvaluator,
                IsPeerToPeerHasMultipleEvaluator = performance.IsPeerToPeerHasMultipleEvaluator,
                DateCreated = performance.DateCreated,
                CreatedBy = performance.CreatedBy,
                LastModified = performance.LastModified,
                ModifiedBy = performance.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, EmployeePeerToPeerAndInternalCustomerPerformanceSetting performance)
        {
            if (ModelState.IsValid)
            {

                performance.CreatedBy = User.Identity.Name;
                performance.DateCreated = DateTime.Now;
                var entity = GetMappedObject(performance);
                db.EmployeePeerToPeerAndInternalCustomerPerformanceSettings.Add(entity);
                db.SaveChanges();
                performance.EmployeePeerToPeerAndInternalCustomerPerformanceSettingId = entity.EmployeePeerToPeerAndInternalCustomerPerformanceSettingId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Performance setting ", "Success", User.Identity.Name, User.Identity.Name + " add Performance setting called "  + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);                
                #endregion
                
            }

            return Json(new[] { performance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, EmployeePeerToPeerAndInternalCustomerPerformanceSetting performance)
        {
            if (ModelState.IsValid)
            {
                performance.ModifiedBy = User.Identity.Name;
                performance.LastModified = DateTime.Now;
                var entity = GetMappedObject(performance);
                db.EmployeePeerToPeerAndInternalCustomerPerformanceSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "Performance Setting ", "Success", User.Identity.Name, User.Identity.Name + " update Performance Seting called "  + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);
                
                #endregion
            }

            return Json(new[] { performance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, EmployeePeerToPeerAndInternalCustomerPerformanceSetting performance)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(performance);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress,
                    "Delete", "Success", User.Identity.Name,
                    "Deleted performance setting  Id " 
                    + performance.EmployeePeerToPeerAndInternalCustomerPerformanceSettingId);
                #endregion
                db.EmployeePeerToPeerAndInternalCustomerPerformanceSettings.Attach(entity);
                db.EmployeePeerToPeerAndInternalCustomerPerformanceSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { performance }.ToDataSourceResult(request, ModelState));
        }
        private EmployeePeerToPeerAndInternalCustomerPerformanceSetting GetMappedObject(EmployeePeerToPeerAndInternalCustomerPerformanceSetting performance)
        {
            var entity = new EmployeePeerToPeerAndInternalCustomerPerformanceSetting
            {
                EmployeePeerToPeerAndInternalCustomerPerformanceSettingId = performance.EmployeePeerToPeerAndInternalCustomerPerformanceSettingId,
                IsInternalCustomerHasMultipleEvaluator = performance.IsInternalCustomerHasMultipleEvaluator,
                IsPeerToPeerHasMultipleEvaluator = performance.IsPeerToPeerHasMultipleEvaluator,
                DateCreated= performance.DateCreated,
                CreatedBy = performance.CreatedBy,
                LastModified = performance.LastModified,
                ModifiedBy =performance.ModifiedBy

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
