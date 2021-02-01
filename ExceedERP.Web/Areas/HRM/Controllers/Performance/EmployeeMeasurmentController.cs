using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    public class EmployeeMeasurmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmployeeMeasurement_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeeMeasurement> employeemeasurement = db.EmployeeMeasurement;
            DataSourceResult result = employeemeasurement.ToDataSourceResult(request, employeeMeasurement => new
            {
                EmpMeasurementId = employeeMeasurement.EmpMeasurementId,
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
                    EmpMesPercentageofWeight = employeeMeasurement.EmpMesPercentageofWeight,
                    Result = employeeMeasurement.Result,
                    Remark = employeeMeasurement.Remark
                };

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
