using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    public class DepartmentMeasurmentController : Controller
    {
        // GET: HRM/DepartmentMeasurment
        private ExceedDbContext db = new ExceedDbContext();
        string ErrorMessage = "";
        public ActionResult Index()
        {
            return View();
        }
        private bool CheckPerformanceValidation(int id)
        {
            ErrorMessage = "";

            #region Action
            var Action = db.PerformanceActions.Where(x => x.MeasurementId == id).FirstOrDefault();
            if (Action != null)
            {
                ErrorMessage = "You cannot delete this Measurment Defination. Please Delete Performance action information first.";
                return false;
            }
            #endregion
            //#region EmployeeMeasurement
            //var EmployeeMeasurement = db.EmployeeMeasurement.Where(x => x.MeasurementId == id).FirstOrDefault();
            //if (EmployeeMeasurement != null)
            //{
            //    ErrorMessage = "You cannot delete this Measurment Defination. Employees are evaluated by this measurement.";
            //    return false;
            //}
            //#endregion


            return true;
        }
        public ActionResult DepartmentMeasurment_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Measurement> measurements = db.Measurements.Where(x => x.MeasurementDefinitionId == id);
            DataSourceResult result = measurements.ToDataSourceResult(request, measurement => new
            {
                MeasurementDefinitionId = measurement.MeasurementDefinitionId,
                MeasurementId = measurement.MeasurementId,
                MeasurementName = measurement.MeasurementName,
                MesPercentageofWeight = measurement.MesPercentageofWeight,
                Remark = measurement.Remark,
                Result = measurement.Result
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentMeasurment_Create([DataSourceRequest]DataSourceRequest request, Measurement measurement, int id)
        {
            var definationWeigt = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == id);
            if (definationWeigt != null)
            {

                var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == definationWeigt.MeasurementDefinitionId).ToList();
                var sum = measurments.Sum(x => x.MesPercentageofWeight) + measurement.MesPercentageofWeight;

                if (sum > definationWeigt.DefnPercentageofWeight)
                {
                    ModelState.AddModelError("", "Sum of detail Weight can't exceed it parent weight!!");
                }
            }

            if (ModelState.IsValid)
            {
                var entity = new Measurement
                {
                    MeasurementDefinitionId = id,
                    MeasurementName = measurement.MeasurementName,
                    MesPercentageofWeight = measurement.MesPercentageofWeight,
                    Remark = measurement.Remark,
                    Result = measurement.Result
                };

                db.Measurements.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentMeasurment_Update([DataSourceRequest]DataSourceRequest request, Measurement measurement)
        {

            var definationWeigt = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == measurement.MeasurementDefinitionId);
            if (definationWeigt != null)
            {
                var Onemeasumrnt = db.Measurements.Where(x => x.MeasurementDefinitionId == definationWeigt.MeasurementDefinitionId && x.MeasurementId != measurement.MeasurementId).Sum(x => x.MesPercentageofWeight) + measurement.MesPercentageofWeight;

                if (Onemeasumrnt > definationWeigt.DefnPercentageofWeight)
                {
                    ModelState.AddModelError("", "Sum of detail Weight can't exceed it parent weight!!");
                }
            }
            if (ModelState.IsValid)
            {
                var entity = new Measurement
                {
                    MeasurementDefinitionId = measurement.MeasurementDefinitionId,
                    MeasurementId = measurement.MeasurementId,
                    MeasurementName = measurement.MeasurementName,
                    MesPercentageofWeight = measurement.MesPercentageofWeight,
                    Remark = measurement.Remark,
                    Result = measurement.Result
                };

                db.Measurements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentMeasurment_Destroy([DataSourceRequest]DataSourceRequest request, Measurement measurement)
        {
            if (ModelState.IsValid)
            {
                if (!CheckPerformanceValidation(measurement.MeasurementId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new Measurement
                    {
                        MeasurementId = measurement.MeasurementId,
                        MeasurementDefinitionId = measurement.MeasurementDefinitionId,
                        MeasurementName = measurement.MeasurementName,
                        MesPercentageofWeight = measurement.MesPercentageofWeight,
                        Remark = measurement.Remark,
                        Result = measurement.Result
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted measurement information measurement Name " + measurement.MeasurementName);
                    #endregion
                    db.Measurements.Attach(entity);
                    db.Measurements.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
