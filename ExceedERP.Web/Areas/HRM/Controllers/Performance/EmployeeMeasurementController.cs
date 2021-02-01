﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    public class EmployeeMeasurementController : Controller
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
        public ActionResult EmployeeMeasurement_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeeMeasurement> employeemeasurement = db.EmployeeMeasurement;
            DataSourceResult result = employeemeasurement.ToDataSourceResult(request, employeeMeasurement => new
            {
                EmpMeasurementId = employeeMeasurement.EmpMeasurementId,
                MeasurementDefinitionId = employeeMeasurement.MeasurementDefinitionId,
                MeasurementId = employeeMeasurement.MeasurementId,
                EmployeeId = employeeMeasurement.EmployeeId,
                EmpMesPercentageofWeight = employeeMeasurement.EmpMesPercentageofWeight,
                Result = employeeMeasurement.Result,
                Remark = employeeMeasurement.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeMeasurement_Create([DataSourceRequest]DataSourceRequest request, EmployeeMeasurement employeeMeasurement)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeMeasurement
                {
                    MeasurementDefinitionId = employeeMeasurement.MeasurementDefinitionId,
                    MeasurementId = employeeMeasurement.MeasurementId,
                    EmployeeId = employeeMeasurement.EmployeeId,
                    EmpMesPercentageofWeight = employeeMeasurement.EmpMesPercentageofWeight,
                    Result = employeeMeasurement.Result,
                    Remark = employeeMeasurement.Remark
                };

                db.EmployeeMeasurement.Add(entity);
                db.SaveChanges();
                employeeMeasurement.EmpMeasurementId = entity.EmpMeasurementId;
            }

            return Json(new[] { employeeMeasurement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeMeasurement_Update([DataSourceRequest]DataSourceRequest request, EmployeeMeasurement employeeMeasurement)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeMeasurement
                {
                    EmpMeasurementId = employeeMeasurement.EmpMeasurementId,
                    MeasurementDefinitionId = employeeMeasurement.MeasurementDefinitionId,
                    MeasurementId = employeeMeasurement.MeasurementId,
                    EmployeeId = employeeMeasurement.EmployeeId,
                    EmpMesPercentageofWeight = employeeMeasurement.EmpMesPercentageofWeight,
                    Result = employeeMeasurement.Result,
                    Remark = employeeMeasurement.Remark
                };

                db.EmployeeMeasurement.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { employeeMeasurement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeMeasurement_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeMeasurement employeeMeasurement)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeMeasurement
                {
                    EmpMeasurementId = employeeMeasurement.EmpMeasurementId,
                    MeasurementDefinitionId = employeeMeasurement.MeasurementDefinitionId,
                    MeasurementId = employeeMeasurement.MeasurementId,
                    EmployeeId = employeeMeasurement.EmployeeId,
                    EmpMesPercentageofWeight = employeeMeasurement.EmpMesPercentageofWeight,
                    Result = employeeMeasurement.Result,
                    Remark = employeeMeasurement.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee performance measurement information of employee " + employeeMeasurement.Employee.EmployeeCode);
                #endregion
                db.EmployeeMeasurement.Attach(entity);
                db.EmployeeMeasurement.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeMeasurement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
