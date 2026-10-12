using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,
                   HrmConstants.HRPerformanceUser,
                   HrmConstants.HRAdmin,
                   HrmConstants.HRDepartmentHead,
                   HrmConstants.HRPerformanceReport,
                   HrmConstants.HREmployeeSelfService)]
    public class DepartmentDefineMeasurmentController : BaseController
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

            // Check for existing performance actions
            bool hasActions = db.PerformanceActions.Any(x => x.MeasurementDefinitionId == id);
            if (hasActions)
            {
                ErrorMessage = "You cannot delete this Measurement Definition. Please delete the related Performance Actions first.";
                return false;
            }

            // Check for employee performance records
            bool hasPerformance = db.EmployeePerformances.Any(x => x.EvaluationCriteriaTypeId == id);
            if (hasPerformance)
            {
                ErrorMessage = "You cannot delete this Measurement Definition. Employees have already been evaluated using this criteria.";
                return false;
            }

            // Check for measurements
            bool hasMeasurements = db.Measurements.Any(x => x.MeasurementDefinitionId == id);
            if (hasMeasurements)
            {
                ErrorMessage = "You cannot delete this Measurement Definition. Please delete the related Measurements first.";
                return false;
            }

            

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
        public ActionResult DepartmentDefineMeasurment_Read([DataSourceRequest]DataSourceRequest request,string lookupId)
        {
            var user = User.Identity.Name;
            var orgId = UserInformationHelper.GetEmployeeDepartmentId(user);
            IQueryable<MeasurementDefinition> departmentLevelCriterias = db.MeasurementDefinitions
                                                                .Where(x=>x.OrgStuructureId==orgId||x.CreatedBy==user)
                                                                .Where(x=>x.IsDepartmentLevel);
            if (!string.IsNullOrEmpty(lookupId) && int.TryParse(lookupId, out int lookId))
            {
                departmentLevelCriterias = departmentLevelCriterias
                    .Where(x => x.LookupId == lookId);
            }
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
        public ActionResult DepartmentDefineMeasurment_Destroy([DataSourceRequest] DataSourceRequest request, MeasurementDefinition departmentLevelCriteria)
        {
            if (!CheckPerformanceValidation(departmentLevelCriteria.MeasurementDefinitionId))
            {
                ModelState.AddModelError("", ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(departmentLevelCriteria);

                // Log deletion
                UserHelper.DeleteLog(
                    Request.UserHostAddress,
                    "Delete",
                    "Success",
                    User.Identity.Name,
                    $"Deleted performance measurement definition: {departmentLevelCriteria.EvaluationCriteria}");

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

        public JsonResult AddCriteriaPerResponsiblityLevel(string evalutionTypeId)
        {
            var user = User.Identity.Name;
            var orgId = UserInformationHelper.GetEmployeeDepartmentId(user);

            // Validate LookupId input
            if (!int.TryParse(evalutionTypeId, out int lookUpId))
            {
                return Json(new { Message = "Invalid Evaluation Type ID", status = "NOT-Ok" });
            }

            // Check if lookup exists
            var lookUp = db.Lookups.Find(lookUpId);
            if (lookUp == null)
            {
                return Json(new { Message = "Evaluation Type not found", status = "NOT-Ok" });
            }

            // Disallowed Evaluation Types
            var disallowedTypes = new[]
            {
                EvaluationType.Sector_Result,
                EvaluationType.Corporate_Result,
                EvaluationType.Companylevel,
                EvaluationType.Project_Manager

            };

            if (disallowedTypes.Contains(lookUp.EvalutionType))
            {
                return Json(new { Message = "You cannot add criteria for this Evaluation Type.", status = "NOT-Ok" });
            }
            var sourceDefinition = db.MeasurementDefinitions.Where(x => x.IsDepartmentLevel).FirstOrDefault(x => x.LookupId == lookUpId);
            if (sourceDefinition == null)
            {
                return Json(new { Message = "Please select Evaluation Type", status = "NOT-Ok" });
            }

            // Get all existing roles for this LookupId
            var existingRoles = db.MeasurementDefinitions
                                  .Where(x => x.IsDepartmentLevel)
                                  .Where(x => x.LookupId == lookUpId)
                                  .Select(x => x.Role)
                                  .ToList().Distinct();

            // Loop through all enum values except already existing
            foreach (ResponsibilityLevel level in Enum.GetValues(typeof(ResponsibilityLevel)))
            {
                if (level == ResponsibilityLevel.NOTSELECTED || existingRoles.Contains(level))
                    continue;

                var newDefinition = new MeasurementDefinition
                {
                    EvaluationCriteria = sourceDefinition.EvaluationCriteria,
                    DefnPercentageofWeight = sourceDefinition.DefnPercentageofWeight,
                    LookupId = sourceDefinition.LookupId,
                    OrgStuructureId = orgId,
                    CriteriaCategoryId = sourceDefinition.CriteriaCategoryId,
                    EvalutionType = sourceDefinition.EvalutionType,
                    Result = sourceDefinition.Result,
                    Role = level,
                    Remark = sourceDefinition.Remark,
                    EmployeeId = sourceDefinition.EmployeeId,
                    IsMainCompanyLevel = sourceDefinition.IsMainCompanyLevel,
                    DateCreated = DateTime.Now,
                    LastModified = sourceDefinition.LastModified,
                    CreatedBy = User.Identity.Name,
                    IsDepartmentLevel = true,
                    ModifiedBy = sourceDefinition.ModifiedBy
                };

                db.MeasurementDefinitions.Add(newDefinition);
                db.SaveChanges(); // Save to get ID

                // Clone Measurements
                var sourceMeasurements = db.Measurements
                                           .Where(x => x.MeasurementDefinitionId == sourceDefinition.MeasurementDefinitionId)
                                           .ToList();

                foreach (var measurement in sourceMeasurements)
                {
                    var newMeasurement = new Measurement
                    {
                        MeasurementName = measurement.MeasurementName,
                        MesPercentageofWeight = measurement.MesPercentageofWeight,
                        MeasurementDefinitionId = newDefinition.MeasurementDefinitionId,
                        Remark = measurement.Remark,
                        Result = measurement.Result
                    };

                    db.Measurements.Add(newMeasurement);
                    db.SaveChanges();

                    // Clone Performance Actions
                    var sourceActions = db.PerformanceActions
                                          .Where(x => x.MeasurementId == measurement.MeasurementId)
                                          .ToList();

                    foreach (var action in sourceActions)
                    {
                        var newAction = new PerformanceAction
                        {
                            ActionName = action.ActionName,
                            AcPercentageofWeight = action.AcPercentageofWeight,
                            Result = action.AcPercentageofWeight,
                            MeasurementDefinitionId = newDefinition.MeasurementDefinitionId,
                            MeasurementId = newMeasurement.MeasurementId,
                            Remark = action.Remark
                        };

                        db.PerformanceActions.Add(newAction);
                    }

                    db.SaveChanges();
                }
            }

            return Json(new { Message = "Successfully created for other roles", status = "Ok" });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
