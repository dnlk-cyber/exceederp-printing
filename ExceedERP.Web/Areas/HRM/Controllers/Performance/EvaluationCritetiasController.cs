using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin)]
    public class EvaluationCritetiasController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        string ErrorMessage = "";
        public ActionResult Index()
        {
            PopulateViewData();
            GenerateSettingValue();
            return View();
        }
        private void PopulateViewData()
        {
            var types = db.Lookups.Where(l => l.LookupType == LookUpTypes.EVALUATION_TYPE);
            ViewData["EvaluationTypes"] = types;
            ViewData["CriteriaCategory"] = db.CriteriaCategories.Select(x => new { x.CriteriaCategoryId, x.Name }).ToList();
            var workunits = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
            ViewData["workunits"] = workunits;
            ViewData["jobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();

        }
        public void GenerateSettingValue()
        {
            var evaluationTypes = db.EvaluationCriteriaActionTypes.ToList();
            if (!evaluationTypes.Any())
            {
                var eval1 = new EvaluationCriteriaActionType()
                {
                    PerformanceCriteriaActionType = PerformanceCriteriaActionType.Open,
                    IsActive = false
                };

                db.EvaluationCriteriaActionTypes.Add(eval1);

                var eval2 = new EvaluationCriteriaActionType()
                {
                    PerformanceCriteriaActionType = PerformanceCriteriaActionType.Sum_Of_Actions,
                    IsActive = true
                };

                db.EvaluationCriteriaActionTypes.Add(eval2);

                db.SaveChanges();
            }
        }
        private bool CheckPerformanceValidation(int id)
        {
            ErrorMessage = "";

            #region Action
            var Action = db.PerformanceActions.Where(x => x.MeasurementDefinitionId == id).FirstOrDefault();
            if (Action != null)
            {
                ErrorMessage = "You cannot delete this Measurment Defination. Please Delete Performance action information first.";
                return false;
            }
            #endregion
            #region Measurement
            var Measurement = db.Measurements.Where(x => x.MeasurementDefinitionId == id).FirstOrDefault();
            if (Measurement != null)
            {
                ErrorMessage = "You cannot delete this Measurment Defination. Please Delete Measurement information first.";
                return false;
            }
            #endregion

            #region EmployeeMeasurement
            //var EmployeeMeasurement = db.EmployeeMeasurementDefinition.Where(x => x.EmpMeasurementDefinitionId == id).FirstOrDefault();
            //if (EmployeeMeasurement != null)
            //{
            //    ErrorMessage = "You cannot delete this Measurment Defination. Employees are evaluated by this definition.";
            //    return false;
            //}
            #endregion

            return true;
        }
        public ActionResult EvaluationCritetias_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MeasurementDefinition> measurementdefinitions = db.MeasurementDefinitions.Where(x => !x.IsDepartmentLevel);
            DataSourceResult result = measurementdefinitions.ToDataSourceResult(request, measurementDefinition => new
            {
                MeasurementDefinitionId = measurementDefinition.MeasurementDefinitionId,
                EvaluationCriteria = measurementDefinition.EvaluationCriteria,
                LookupId = measurementDefinition.LookupId,
                CriteriaCategoryId = measurementDefinition.CriteriaCategoryId,
                EvalutionType = measurementDefinition.EvalutionType,
                DefnPercentageofWeight = measurementDefinition.DefnPercentageofWeight,
                Result = measurementDefinition.Result,
                Role = measurementDefinition.Role,
                Remark = measurementDefinition.Remark,
                EmployeeId = measurementDefinition.EmployeeId,
                IsDepartmentLevel = measurementDefinition.IsDepartmentLevel,
                IsMainCompanyLevel = measurementDefinition.IsMainCompanyLevel,
                DateCreated = measurementDefinition.DateCreated,
                LastModified = measurementDefinition.LastModified,
                CreatedBy = measurementDefinition.CreatedBy,
                ModifiedBy = measurementDefinition.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCritetias_Create([DataSourceRequest]DataSourceRequest request, MeasurementDefinition measurementDefinition)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var lookup = db.Lookups.Find(measurementDefinition.LookupId);
                if (lookup != null
                    && (lookup.EvalutionType == EvaluationType.MainDepartmentLevel
                    || lookup.EvalutionType == EvaluationType.Project_Manager))
                {
                    measurementDefinition.IsMainCompanyLevel = true;
                }
                else
                {
                    measurementDefinition.IsMainCompanyLevel = false;
                }
                var entity = new MeasurementDefinition
                {
                    EvaluationCriteria = measurementDefinition.EvaluationCriteria,
                    DefnPercentageofWeight = measurementDefinition.DefnPercentageofWeight,
                    LookupId = measurementDefinition.LookupId,
                    CriteriaCategoryId = measurementDefinition.CriteriaCategoryId,
                    EvalutionType = measurementDefinition.EvalutionType,
                    Result = measurementDefinition.Result,
                    Role = measurementDefinition.Role,
                    Remark = measurementDefinition.Remark,
                    EmployeeId = measurementDefinition.EmployeeId,
                    IsMainCompanyLevel = measurementDefinition.IsMainCompanyLevel,
                    DateCreated = DateTime.Now,
                    LastModified = measurementDefinition.LastModified,
                    CreatedBy = User.Identity.Name,
                    IsDepartmentLevel = false,
                    ModifiedBy = measurementDefinition.ModifiedBy
                };

                db.MeasurementDefinitions.Add(entity);
                db.SaveChanges();
                measurementDefinition.MeasurementDefinitionId = entity.MeasurementDefinitionId;

                #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "Evaluation Criteria ", "Success", User.Identity.Name, User.Identity.Name + " add new Evaluation Criteria called "+entity.EvaluationCriteria  + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
               

                #endregion

            }

            return Json(new[] { measurementDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCritetias_Update([DataSourceRequest]DataSourceRequest request, MeasurementDefinition measurementDefinition)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var lookup = db.Lookups.Find(measurementDefinition.LookupId);
                if(lookup !=null 
                    &&(lookup.EvalutionType== EvaluationType.MainDepartmentLevel
                    || lookup.EvalutionType == EvaluationType.Project_Manager))
                {
                    measurementDefinition.IsMainCompanyLevel = true;
                }
                else
                {
                    measurementDefinition.IsMainCompanyLevel = false;
                }

                var entity = new MeasurementDefinition
                {
                    MeasurementDefinitionId = measurementDefinition.MeasurementDefinitionId,
                    EvaluationCriteria = measurementDefinition.EvaluationCriteria,
                    DefnPercentageofWeight = measurementDefinition.DefnPercentageofWeight,
                    LookupId = measurementDefinition.LookupId,
                    CriteriaCategoryId = measurementDefinition.CriteriaCategoryId,
                    EvalutionType = measurementDefinition.EvalutionType,
                    Result = measurementDefinition.Result,
                    Role = measurementDefinition.Role,
                    EmployeeId = measurementDefinition.EmployeeId,
                    IsMainCompanyLevel = measurementDefinition.IsMainCompanyLevel,
                    Remark = measurementDefinition.Remark,
                    DateCreated = measurementDefinition.DateCreated,
                    LastModified = DateTime.Now,
                    IsDepartmentLevel = measurementDefinition.IsDepartmentLevel,
                    CreatedBy = measurementDefinition.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.MeasurementDefinitions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Evaluation Criteria ", "Success", User.Identity.Name, User.Identity.Name + " update Evaluation Criteria called "+entity.EvaluationCriteria + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                
                #endregion

            }

            return Json(new[] { measurementDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EvaluationCritetias_Destroy([DataSourceRequest]DataSourceRequest request, MeasurementDefinition measurementDefinition)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckPerformanceValidation(measurementDefinition.MeasurementDefinitionId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new MeasurementDefinition
                    {
                        MeasurementDefinitionId = measurementDefinition.MeasurementDefinitionId,
                        EvaluationCriteria = measurementDefinition.EvaluationCriteria,
                        DefnPercentageofWeight = measurementDefinition.DefnPercentageofWeight,
                        LookupId = measurementDefinition.LookupId,
                        CriteriaCategoryId = measurementDefinition.CriteriaCategoryId,
                        EvalutionType = measurementDefinition.EvalutionType,
                        Result = measurementDefinition.Result,
                        Role = measurementDefinition.Role,
                        Remark = measurementDefinition.Remark,
                        EmployeeId = measurementDefinition.EmployeeId,
                        DateCreated = measurementDefinition.DateCreated,
                        LastModified = measurementDefinition.LastModified,
                        IsDepartmentLevel = measurementDefinition.IsDepartmentLevel,
                        IsMainCompanyLevel = measurementDefinition.IsMainCompanyLevel,
                        CreatedBy = measurementDefinition.CreatedBy,
                        ModifiedBy = measurementDefinition.ModifiedBy
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted performance measurement definition information of evaluation criteria" + measurementDefinition.EvaluationCriteria);
                    #endregion
                    db.MeasurementDefinitions.Attach(entity);
                    db.MeasurementDefinitions.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { measurementDefinition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
