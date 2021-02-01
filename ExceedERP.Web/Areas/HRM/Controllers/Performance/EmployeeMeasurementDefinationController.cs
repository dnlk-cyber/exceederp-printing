﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
  
    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRPerformanceUser)]
    public class EmployeeMeasurementDefinationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }
        public ActionResult EmployeeMeasurementDefinition_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeeMeasurementDefinition> employeemeasurementdefinition = db.EmployeeMeasurementDefinition;
            DataSourceResult result = employeemeasurementdefinition.ToDataSourceResult(request, employeeMeasurementDefinition => new
            {
                EmpMeasurementDefinitionId = employeeMeasurementDefinition.EmpMeasurementDefinitionId,
                MeasurementDefinitionId = employeeMeasurementDefinition.MeasurementDefinitionId,
                EmployeeId = employeeMeasurementDefinition.EmployeeId,
                EmpPercentageofWeight = employeeMeasurementDefinition.EmpPercentageofWeight,
                Result = employeeMeasurementDefinition.Result,
                Remark = employeeMeasurementDefinition.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeMeasurementDefinition_Create([DataSourceRequest]DataSourceRequest request, EmployeeMeasurementDefinition employeeMeasurementDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeMeasurementDefinition
                {
                    MeasurementDefinitionId = employeeMeasurementDefinition.MeasurementDefinitionId,
                    EmployeeId = employeeMeasurementDefinition.EmployeeId,
                    EmpPercentageofWeight = employeeMeasurementDefinition.EmpPercentageofWeight,
                    Result = employeeMeasurementDefinition.Result,
                    Remark = employeeMeasurementDefinition.Remark
                };

                db.EmployeeMeasurementDefinition.Add(entity);
                db.SaveChanges();
                employeeMeasurementDefinition.EmpMeasurementDefinitionId = entity.EmpMeasurementDefinitionId;
            }

            return Json(new[] { employeeMeasurementDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeMeasurementDefinition_Update([DataSourceRequest]DataSourceRequest request, EmployeeMeasurementDefinition employeeMeasurementDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeMeasurementDefinition
                {
                    EmpMeasurementDefinitionId = employeeMeasurementDefinition.EmpMeasurementDefinitionId,
                    MeasurementDefinitionId = employeeMeasurementDefinition.MeasurementDefinitionId,
                    EmployeeId = employeeMeasurementDefinition.EmployeeId,
                    EmpPercentageofWeight = employeeMeasurementDefinition.EmpPercentageofWeight,
                    Result = employeeMeasurementDefinition.Result,
                    Remark = employeeMeasurementDefinition.Remark
                };

                db.EmployeeMeasurementDefinition.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { employeeMeasurementDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeMeasurementDefinition_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeMeasurementDefinition employeeMeasurementDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeMeasurementDefinition
                {
                    EmpMeasurementDefinitionId = employeeMeasurementDefinition.EmpMeasurementDefinitionId,
                    MeasurementDefinitionId = employeeMeasurementDefinition.MeasurementDefinitionId,
                    EmployeeId = employeeMeasurementDefinition.EmployeeId,
                    EmpPercentageofWeight = employeeMeasurementDefinition.EmpPercentageofWeight,
                    Result = employeeMeasurementDefinition.Result,
                    Remark = employeeMeasurementDefinition.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee performance measurement defination information of employee " + employeeMeasurementDefinition.Employee.EmployeeCode);
                #endregion
                db.EmployeeMeasurementDefinition.Attach(entity);
                db.EmployeeMeasurementDefinition.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeMeasurementDefinition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
