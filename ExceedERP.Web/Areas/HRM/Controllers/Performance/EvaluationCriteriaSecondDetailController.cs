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
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin)]
    public class EvaluationCriteriaSecondDetailController : Controller
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


            #region EmployeeMeasurement
            //var EmployeeMeasurement = db.EmployeePerformanceAction.Where(x => x.ActionId == id).FirstOrDefault();
            //if (EmployeeMeasurement != null)
            //{
            //    ErrorMessage = "You cannot delete this Measurment Defination. Employees are evaluated by this measurement.";
            //    return false;
            //}
            #endregion


            return true;
        }
        public ActionResult EvaluationCriteriaSecondDetail_Read([DataSourceRequest]DataSourceRequest request, int defid, int mesid)
        {
            IQueryable<PerformanceAction> performanceactions = db.PerformanceActions;
            if (defid != 0)
            {
                performanceactions = performanceactions.Where(x => x.MeasurementDefinitionId == defid);
            }
            if (mesid != 0)
            {
                performanceactions = performanceactions.Where(x => x.MeasurementId == mesid);
            }
            DataSourceResult result = performanceactions.ToDataSourceResult(request, performanceAction => new
            {
                ActionId = performanceAction.ActionId,
                ActionName = performanceAction.ActionName,
                AcPercentageofWeight = performanceAction.AcPercentageofWeight,
                Remark = performanceAction.Remark,
                Result = performanceAction.Result,
                MeasurementDefinitionId = performanceAction.MeasurementDefinitionId,
                MeasurementId = performanceAction.MeasurementId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCriteriaSecondDetail_Create([DataSourceRequest]DataSourceRequest request, PerformanceAction performanceAction, int defid, int mesid)
        {
            var measurement = db.Measurements.FirstOrDefault(x => x.MeasurementId == mesid);
            if (measurement != null)
            {
                if (performanceAction.AcPercentageofWeight > measurement.MesPercentageofWeight)
                {
                    ModelState.AddModelError("", "The performance action weight should be less or equal to measurement weight!!");
                }

                var criteriaType = db.EvaluationCriteriaActionTypes.FirstOrDefault(x=>x.IsActive);
                if(criteriaType != null && criteriaType.PerformanceCriteriaActionType == PerformanceCriteriaActionType.Sum_Of_Actions)
                {
                    var performanceAct = db.PerformanceActions.Where(x => x.MeasurementId == measurement.MeasurementId).ToList();
                    if (performanceAct.Any())
                    {
                        var performanceActWeightSum = performanceAct.Sum(x => x.AcPercentageofWeight) + performanceAction.AcPercentageofWeight;

                        if (performanceActWeightSum > measurement.MesPercentageofWeight)
                        {
                            ModelState.AddModelError("", "sum of your measurment weight can't exceed the paret weight");
                        }
                    }
                }               
            }

            if (ModelState.IsValid)
            {
                var entity = new PerformanceAction
                {
                    ActionName = performanceAction.ActionName,
                    AcPercentageofWeight = performanceAction.AcPercentageofWeight,
                    MeasurementDefinitionId = defid,
                    MeasurementId = mesid,
                    Remark = performanceAction.Remark
                };

                db.PerformanceActions.Add(entity);
                db.SaveChanges();
                performanceAction.ActionId = entity.ActionId;
            }

            return Json(new[] { performanceAction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCriteriaSecondDetail_Update([DataSourceRequest]DataSourceRequest request, PerformanceAction performanceAction)
        {
            var measurement = db.Measurements.FirstOrDefault(x => x.MeasurementId == performanceAction.MeasurementId);
            if (measurement != null)
            {
                if (performanceAction.AcPercentageofWeight > measurement.MesPercentageofWeight)
                {
                    ModelState.AddModelError("", "The performance action weight should be less or equal to measurement weight!!");
                }

                var criteriaType = db.EvaluationCriteriaActionTypes.FirstOrDefault(x => x.IsActive);
                if (criteriaType != null && criteriaType.PerformanceCriteriaActionType == PerformanceCriteriaActionType.Sum_Of_Actions)
                {
                    var performanceAct = db.PerformanceActions.Where(x => x.MeasurementId == measurement.MeasurementId && x.ActionId != performanceAction.ActionId).ToList();
                    if (performanceAct.Any())
                    {
                        var performanceActWeightSum = performanceAct.Sum(x => x.AcPercentageofWeight) + performanceAction.AcPercentageofWeight;

                        if (performanceActWeightSum > measurement.MesPercentageofWeight)
                        {
                            ModelState.AddModelError("", "sum of your measurment weight can't exceed the paret weight");
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var entity = new PerformanceAction
                {
                    ActionId = performanceAction.ActionId,
                    ActionName = performanceAction.ActionName,
                    AcPercentageofWeight = performanceAction.AcPercentageofWeight,
                    Remark = performanceAction.Remark,
                    MeasurementDefinitionId = performanceAction.MeasurementDefinitionId,
                    Result = performanceAction.Result,
                    MeasurementId = performanceAction.MeasurementId,
                };

                db.PerformanceActions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { performanceAction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCriteriaSecondDetail_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceAction performanceAction)
        {
            if (ModelState.IsValid)
            {
                if (!CheckPerformanceValidation(performanceAction.ActionId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new PerformanceAction
                    {
                        ActionId = performanceAction.ActionId,
                        ActionName = performanceAction.ActionName,
                        AcPercentageofWeight = performanceAction.AcPercentageofWeight,
                        Remark = performanceAction.Remark,
                        Result = performanceAction.Result,
                        MeasurementDefinitionId = performanceAction.MeasurementDefinitionId,
                        MeasurementId = performanceAction.MeasurementId
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted performance action information action Name " + performanceAction.ActionName);
                    #endregion
                    db.PerformanceActions.Attach(entity);
                    db.PerformanceActions.Remove(entity);
                    db.SaveChanges();
                }
            }
            return Json(new[] { performanceAction }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
