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
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,
                      HrmConstants.HRPerformanceUser,
                      HrmConstants.HRAdmin,
                      HrmConstants.HRDepartmentHead,
                      HrmConstants.HRPerformanceReport,
                      HrmConstants.HREmployeeSelfService)]
    public class EvaluationCriteriaDetailController : BaseController
    {
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
            #region EmployeeMeasurement
            //var EmployeeMeasurement = db.EmployeeMeasurement.Where(x => x.MeasurementId == id).FirstOrDefault();
            //if (EmployeeMeasurement != null)
            //{
            //    ErrorMessage = "You cannot delete this Measurment Defination. Employees are evaluated by this measurement.";
            //    return false;
            //}
            #endregion


            return true;
        }
        public ActionResult EvaluationCriteriaDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Measurement> measurements = db.Measurements.Where(x => x.MeasurementDefinitionId == id);
            DataSourceResult result = measurements.ToDataSourceResult(request, measurement => new
            {
                MeasurementId = measurement.MeasurementId,
                MeasurementDefinitionId = measurement.MeasurementDefinitionId,
                MeasurementName = measurement.MeasurementName,
                MesPercentageofWeight = measurement.MesPercentageofWeight,
                Remark = measurement.Remark,
                Result = measurement.Result
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCriteriaDetail_Create([DataSourceRequest]DataSourceRequest request, Measurement measurement, int id)
        {
            measurement.MeasurementDefinitionId = id;
            var definition = db.MeasurementDefinitions.Find(id);
            if (definition != null)
            {
                var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == definition.MeasurementDefinitionId).ToList();
                if (measurments.Any())
                {
                    decimal sumMeasur = measurments.Sum(x => x.MesPercentageofWeight) + measurement.MesPercentageofWeight;
                    if (sumMeasur > definition.DefnPercentageofWeight)
                    {
                        ModelState.AddModelError("","sum of percentage weight should less or equal to its parent weight");
                    }
                }
                else if (measurement.MesPercentageofWeight > definition.DefnPercentageofWeight)
                {
                    ModelState.AddModelError("", "percentage weight should less or equal to its parent weight");
                }

            }
            else
            {
                ModelState.AddModelError("", "check parent data");
            }
            if (ModelState.IsValid)
            {
                var entity = new Measurement
                {
                    MeasurementName = measurement.MeasurementName,
                    MesPercentageofWeight = measurement.MesPercentageofWeight,
                    MeasurementDefinitionId = measurement.MeasurementDefinitionId,
                    Remark = measurement.Remark,
                    Result = measurement.Result,
                };

                db.Measurements.Add(entity);
                db.SaveChanges();
                measurement.MeasurementId = entity.MeasurementId;
            }

            return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCriteriaDetail_Update([DataSourceRequest]DataSourceRequest request, Measurement measurement)
        {
            var definition = db.MeasurementDefinitions.Find(measurement.MeasurementDefinitionId);
            if (definition != null)
            {
                var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == definition.MeasurementDefinitionId && x.MeasurementId != measurement.MeasurementId).ToList();
                if (measurments.Any())
                {
                    decimal sumMeasur = measurments.Sum(x => x.MesPercentageofWeight) + measurement.MesPercentageofWeight;
                    if (sumMeasur > definition.DefnPercentageofWeight)
                    {
                        ModelState.AddModelError("", "sum of percentage weight should less or equal to its parent weight");
                        return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));

                    }
                }
                else if(measurement.MesPercentageofWeight > definition.DefnPercentageofWeight)
                {
                    ModelState.AddModelError("", "percentage weight should less or equal to its parent weight");
                    return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));

                }


            }
            else
            {
                ModelState.AddModelError("", "check parent data");
                return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                var entity = new Measurement
                {
                    MeasurementId = measurement.MeasurementId,
                    MeasurementName = measurement.MeasurementName,
                    MesPercentageofWeight = measurement.MesPercentageofWeight,
                    MeasurementDefinitionId = measurement.MeasurementDefinitionId,
                    Result = measurement.Result,
                    Remark = measurement.Remark
                };

                db.Measurements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { measurement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCriteriaDetail_Destroy([DataSourceRequest]DataSourceRequest request, Measurement measurement)
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
                        MeasurementName = measurement.MeasurementName,
                        MesPercentageofWeight = measurement.MesPercentageofWeight,
                        MeasurementDefinitionId = measurement.MeasurementDefinitionId,
                        Result = measurement.Result,
                        Remark = measurement.Remark
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
