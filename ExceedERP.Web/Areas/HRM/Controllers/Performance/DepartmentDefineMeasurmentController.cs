using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
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
    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,
                   HrmConstants.HRPerformanceUser,
                   HrmConstants.HRAdmin,
                   HrmConstants.HRDepartmentHead,
                   HrmConstants.HRPerformanceReport,
                   HrmConstants.HREmployeeSelfService)]
    public class DepartmentDefineMeasurmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        string ErrorMessage = "";
        // GET: HRM/DepartmentLevelCriteria
        public ActionResult Index()
        {
            PopulateViewData();
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

            //#region EmployeeMeasurement
            //var EmployeeMeasurement = db.EmployeeMeasurementDefinition.Where(x => x.MeasurementDefinitionId == id).FirstOrDefault();
            //if (EmployeeMeasurement != null)
            //{
            //    ErrorMessage = "You cannot delete this Measurment Defination. Employees are evaluated by this definition.";
            //    return false;
            //}
            //#endregion

            return true;
        }
        public JsonResult GetDepartmentEvaluationType()
        {

            var types = db.Lookups.Where(l => l.LookupType == LookUpTypes.EVALUATION_TYPE&&l.EvalutionType==EvaluationType.DirectBoss);
            return Json(types.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult DepartmentDefineMeasurment_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MeasurementDefinition> departmentLevelCriterias = db.MeasurementDefinitions.Where(x=>x.IsDepartmentLevel);
            DataSourceResult result = departmentLevelCriterias.ToDataSourceResult(request, departmentLevelCriteria => new {
                MeasurementDefinitionId = departmentLevelCriteria.MeasurementDefinitionId,
                EmployeeId = departmentLevelCriteria.EmployeeId,
                OrgStuructureId = departmentLevelCriteria.OrgStuructureId,
                EvaluationCriteria = departmentLevelCriteria.EvaluationCriteria,
                LookupId = departmentLevelCriteria.LookupId,
                CriteriaCategoryId = departmentLevelCriteria.CriteriaCategoryId,
                IsMainCompanyLevel = departmentLevelCriteria.IsMainCompanyLevel,
                EvalutionType = departmentLevelCriteria.EvalutionType,
                DefnPercentageofWeight = departmentLevelCriteria.DefnPercentageofWeight,
                Result = departmentLevelCriteria.Result,
                Role = departmentLevelCriteria.Role,
                Remark = departmentLevelCriteria.Remark,
                OrgBranchName = departmentLevelCriteria.OrgBranchName,
                DateCreated = departmentLevelCriteria.DateCreated,
                LastModified = departmentLevelCriteria.LastModified,
                CreatedBy = departmentLevelCriteria.CreatedBy,
                IsDepartmentLevel = departmentLevelCriteria.IsDepartmentLevel,
                ModifiedBy = departmentLevelCriteria.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentDefineMeasurment_Create([DataSourceRequest]DataSourceRequest request, MeasurementDefinition departmentLevelCriteria)
        {

            departmentLevelCriteria.DateCreated = DateTime.Now;
            departmentLevelCriteria.CreatedBy = User.Identity.Name;
            departmentLevelCriteria.IsDepartmentLevel = true;
            var lookup = db.Lookups.Find(departmentLevelCriteria.LookupId);
            if (lookup != null
                && (lookup.EvalutionType == EvaluationType.MainDepartmentLevel
                || lookup.EvalutionType == EvaluationType.Project_Manager))
            {
                departmentLevelCriteria.IsMainCompanyLevel = true;
            }
            else
            {
                departmentLevelCriteria.IsMainCompanyLevel = false;
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(departmentLevelCriteria);
                db.MeasurementDefinitions.Add(entity);
                db.SaveChanges();

                #region operation log
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Department Evaluation Criteria ", "Success", User.Identity.Name, User.Identity.Name + " add new Department Evaluation Criteria called " + entity.EvaluationCriteria + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                
                #endregion
            }

            return Json(new[] { departmentLevelCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentDefineMeasurment_Update([DataSourceRequest]DataSourceRequest request, MeasurementDefinition departmentLevelCriteria)
        {
            departmentLevelCriteria.LastModified = DateTime.Now;
            departmentLevelCriteria.ModifiedBy = User.Identity.Name;
            var lookup = db.Lookups.Find(departmentLevelCriteria.LookupId);
            if (lookup != null
                && (lookup.EvalutionType == EvaluationType.MainDepartmentLevel
                || lookup.EvalutionType == EvaluationType.Project_Manager))
            {
                departmentLevelCriteria.IsMainCompanyLevel = true;
            }
            else
            {
                departmentLevelCriteria.IsMainCompanyLevel = false;
            }
           
          
            if (ModelState.IsValid)
            {
                //var entity = GetMappedObject(departmentLevelCriteria);
                //db.MeasurementDefinitions.Attach(entity);
                //db.Entry(entity).State = EntityState.Modified;
                //db.SaveChanges();
                var measurementDefinition = db.MeasurementDefinitions.Find(departmentLevelCriteria.MeasurementDefinitionId);
                if (measurementDefinition != null)
                {
                    measurementDefinition.LookupId = departmentLevelCriteria.LookupId;
                    measurementDefinition.EvaluationCriteria = departmentLevelCriteria.EvaluationCriteria;
                    measurementDefinition.EvalutionType = departmentLevelCriteria.EvalutionType;
                    measurementDefinition.CriteriaCategoryId = departmentLevelCriteria.CriteriaCategoryId;
                    measurementDefinition.Role = departmentLevelCriteria.Role;
                    measurementDefinition.OrgStuructureId = departmentLevelCriteria.OrgStuructureId;
                    measurementDefinition.DefnPercentageofWeight = departmentLevelCriteria.DefnPercentageofWeight;
                    measurementDefinition.IsMainCompanyLevel = departmentLevelCriteria.IsMainCompanyLevel;
                    measurementDefinition.IsDepartmentLevel = departmentLevelCriteria.IsDepartmentLevel;
                    measurementDefinition.LastModified = departmentLevelCriteria.LastModified;
                    measurementDefinition.ModifiedBy = departmentLevelCriteria.ModifiedBy;
                    measurementDefinition.Remark = departmentLevelCriteria.Remark;
                    db.Entry(measurementDefinition).State = EntityState.Modified;
                    db.SaveChanges();
                }
                

                #region operation log
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Department Evaluation Criteria ", "Success", User.Identity.Name, User.Identity.Name + " update Department Evaluation Criteria called " + departmentLevelCriteria.EvaluationCriteria + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                
                #endregion
            }

            return Json(new[] { departmentLevelCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentDefineMeasurment_Destroy([DataSourceRequest]DataSourceRequest request, MeasurementDefinition departmentLevelCriteria)
        {
            if (!CheckPerformanceValidation(departmentLevelCriteria.MeasurementDefinitionId))
            {
                ModelState.AddModelError("", ErrorMessage);
            }
            if (ModelState.IsValid)
            {
               
                
                var entity = GetMappedObject(departmentLevelCriteria);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted performance measurement definition information of evaluation criteria" + departmentLevelCriteria.EvaluationCriteria);
                #endregion
                db.MeasurementDefinitions.Attach(entity);
                db.MeasurementDefinitions.Remove(entity);
                db.SaveChanges();
              
            }

            return Json(new[] { departmentLevelCriteria }.ToDataSourceResult(request, ModelState));
        }

        private MeasurementDefinition GetMappedObject(MeasurementDefinition departmentLevelCriteria)
        {
            var entity = new MeasurementDefinition
            {
                MeasurementDefinitionId = departmentLevelCriteria.MeasurementDefinitionId,
                EmployeeId = departmentLevelCriteria.EmployeeId,
                OrgStuructureId = departmentLevelCriteria.OrgStuructureId,
                EvaluationCriteria = departmentLevelCriteria.EvaluationCriteria,
                LookupId = departmentLevelCriteria.LookupId,
                CriteriaCategoryId = departmentLevelCriteria.CriteriaCategoryId,
                IsMainCompanyLevel = departmentLevelCriteria.IsMainCompanyLevel,
                EvalutionType = departmentLevelCriteria.EvalutionType,
                DefnPercentageofWeight = departmentLevelCriteria.DefnPercentageofWeight,
                Result = departmentLevelCriteria.Result,
                Role = departmentLevelCriteria.Role,
                Remark = departmentLevelCriteria.Remark,
                OrgBranchName = departmentLevelCriteria.OrgBranchName,
                DateCreated = departmentLevelCriteria.DateCreated,
                LastModified = departmentLevelCriteria.LastModified,
                CreatedBy = departmentLevelCriteria.CreatedBy,
                IsDepartmentLevel = departmentLevelCriteria.IsDepartmentLevel,
                ModifiedBy = departmentLevelCriteria.ModifiedBy
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
