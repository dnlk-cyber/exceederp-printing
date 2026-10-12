using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRAdmin, HrmConstants.HRPerformanceUser)]
    public class EmployeePerformanceCorporateAndSectorLevelsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            ViewData["fiscalYearList"] = CacheData.GetFiscalYearCaches().Select(
              i => new
              {
                  Name = i.Name,
                  GlFiscalYearId = i.Id,
              }).AsQueryable();

            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
              i => new
              {
                  FullNameAmharic = i.AmharicName,
                  FullNameaEnglish = i.Name,
                  EmployeeId = i.Id,
                  EmployeeCode = i.Code,
              }).AsQueryable();
            ViewData["branches"] = CacheData.GetBranchCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BranchId = i.Id,
               }).AsQueryable(); ;

            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BusinessUnitId = i.Id,
               }).AsQueryable(); ; ;
            ViewData["workunits"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["jobTitle"] = CacheData.GetJobTitlesCaches().Select(
              i => new
              {
                  Description = i.Name,
                  LookupId = i.Id,
              }).AsQueryable();
            ViewData["Period"] = db.Quarters.Select(x => new { x.QuarterId, x.Name }).ToList();

            var measurments = db.MeasurementDefinitions.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + p.Lookup.Description + " - " + p.Role.ToString() + " " + p.DefnPercentageofWeight

            }).ToList();
            ViewData["EvalutionCrateria"] = measurments;
            ViewData["Measurments"] = db.Measurements.ToList();
            ViewData["PerformanceActions"] = db.PerformanceActions.ToList();

        }
        public string GetEvaluationTypeName(int lookupId)
        {
            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == lookupId);
            if (lookup != null)
            {
                return lookup.Description;
            }
            return "";
        }
        private decimal GetWeight(int id)
        {
            var weight = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == id).ToList();
            if (weight.Count() > 0)
            {
                var updatePerformance = db.EmployeePerformances.Find(id);
                if (updatePerformance != null)
                {
                    updatePerformance.Weight = weight.Sum(x => x.Weight);
                    db.Entry(updatePerformance).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return weight.Sum(x => x.Weight);
            }
            return 0;
        }
        public decimal GetEvaluationResult(int id)
        {
            decimal result = 0;


            var empMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == id).ToList();
            foreach (var em in empMeasurments)
            {
                var weighAndResult = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();
                if (weighAndResult.Count() > 0)
                {

                    result += weighAndResult.Sum(x => x.Score);

                }
                else
                {
                    result += 0;
                }



            }


            var updatePerformance = db.EmployeePerformances.Find(id);
            if (updatePerformance != null)
            {
                updatePerformance.Result = result;
                db.Entry(updatePerformance).State = EntityState.Modified;
                db.SaveChanges();
            }
            return result;
        }
        public decimal GetEvaluationWeight(int id)
        {
            decimal Weight = 0;


            var empMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == id).ToList();
            foreach (var em in empMeasurments)
            {
                var weights = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();
                if (weights.Any())
                {

                    Weight += weights.Sum(x => x.Weight);

                }
               



            }


            var updatePerformance = db.EmployeePerformances.Find(id);
            if (updatePerformance != null)
            {
                updatePerformance.Weight = Weight;
                db.Entry(updatePerformance).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Weight;
        }
        public ActionResult EmployeePerformanceCorporateAndSectorLevels_Read([DataSourceRequest] DataSourceRequest request, int? fiscalyearId)
        {
            var employeePerformances = db.EmployeePerformances
                .Where(x => x.IsSecondLevelApproved);

            if (fiscalyearId.HasValue)
            {
                employeePerformances = employeePerformances
                    .Where(x => x.GlFiscalYearId == fiscalyearId.Value);
            }

            var latestPerformances = employeePerformances
                .GroupBy(x => new { x.EmployeeId, x.GlFiscalYearId, x.GlPeriodId })
                .Select(g => g.OrderByDescending(x => x.EmployeePerformanceId).FirstOrDefault())
                .AsQueryable();

            var result = latestPerformances.Select(x => new
            {
                x.EmployeePerformanceId,
                x.EmployeeId,
                x.GlFiscalYearId,
                x.GlPeriodId,
                x.BusinessUnitId,
                x.OrgStructureId,
                x.ResponsibilityLevel,
                x.BranchId,
                x.JobTitleId,
                x.IsCurrent,
                x.IsHRApproved,
                x.DateCreated,
                x.ImmediateSuppurvisorApprovalDate,
                x.SecondLevelApprovalDate,
                x.FinalApprovalDate,
                x.IsHRLevelEvalution,
                x.Date,
                x.Remark
            }).ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EmployeePerformanceCorporateAndSectorLevels_OnePerson_Read([DataSourceRequest] DataSourceRequest request, int employeeId, int fiscalyearId, int quarterId)
        {
            var employeePerformancesQuery = db.EmployeePerformances
                .Where(x => x.EmployeeId == employeeId
                            && x.GlFiscalYearId == fiscalyearId
                            && x.GlPeriodId == quarterId
                            && x.IsApproved
                            && (x.IsAcceptedByEmployee || x.IsRejectedByEmployee)
                            && x.IsSecondLevelApproved)
                .Select(employeePerformance => new
                {
                    employeePerformance.EmployeePerformanceId,
                    employeePerformance.EmployeeId,
                    employeePerformance.GlFiscalYearId,
                    employeePerformance.GlPeriodId,
                    employeePerformance.EvaluationCriteriaTypeId,
                    employeePerformance.BusinessUnitId,
                    employeePerformance.OrgStructureId,
                    employeePerformance.ResponsibilityLevel,
                    employeePerformance.BranchId,
                    employeePerformance.JobTitleId,
                    employeePerformance.IsCurrent,
                    employeePerformance.IsHRLevelEvalution,
                    employeePerformance.Date,
                    employeePerformance.DateCreated,
                    employeePerformance.ImmediateSuppurvisorApprovalDate,
                    employeePerformance.SecondLevelApprovalDate,
                    employeePerformance.FinalApprovalDate,
                    employeePerformance.EmployeeRemark,
                    employeePerformance.Remark
                })
                .AsNoTracking()
                .ToList(); // Materialize now to avoid multiple DB hits

            // Now batch compute the additional fields (avoids N+1 queries)
            var resultList = employeePerformancesQuery.Select(ep => new
            {
                ep.EmployeePerformanceId,
                ep.EmployeeId,
                ep.GlFiscalYearId,
                ep.GlPeriodId,
                ep.EvaluationCriteriaTypeId,
                ep.BusinessUnitId,
                ep.OrgStructureId,
                ep.ResponsibilityLevel,
                ep.BranchId,
                ep.JobTitleId,
                ep.IsCurrent,
                ep.IsHRLevelEvalution,
                ep.Date,
                ep.DateCreated,
                ep.ImmediateSuppurvisorApprovalDate,
                ep.SecondLevelApprovalDate,
                ep.FinalApprovalDate,
                ep.EmployeeRemark,
                ep.Remark,
                Result = db.EmployeeMeasurement.Where(m => m.EmployeePerformanceId == ep.EmployeePerformanceId)
                                                .SelectMany(m => db.EmployeePerformanceScores
                                                    .Where(s => s.EmployeeMeasurementId == m.EmpMeasurementId)
                                                    .Select(s => s.Score))
                                                .DefaultIfEmpty(0)
                                                .Sum(),
                Weight = db.EmployeeMeasurement.Where(m => m.EmployeePerformanceId == ep.EmployeePerformanceId)
                                    .SelectMany(m => db.EmployeePerformanceScores
                                        .Where(s => s.EmployeeMeasurementId == m.EmpMeasurementId)
                                        .Select(s => s.Weight))
                                    .DefaultIfEmpty(0)
                                    .Sum(),
                //Result = GetEvaluationResult(ep.EmployeePerformanceId),
                //Weight = GetEvaluationWeight(ep.EmployeePerformanceId) // Lightweight version with no db update
            });

            return Json(resultList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmployeePerformanceCorporateAndSectorLevels_OnePerson_Destroy([DataSourceRequest]DataSourceRequest request, EmployeePerformance employeePerformance,int employeePerformanceId)
        {
            ModelState.Clear();
            var employeePerformanceNew = db.EmployeePerformances.Find(employeePerformance.EmployeePerformanceId);
            if (employeePerformanceNew != null)
            {
                if (ModelState.IsValid)
                {
                    // Delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted performance Performance " + employeePerformance.EmployeeId);

                    // Delete related EmployeeMeasurements and EmployeePerformanceScores
                    var employeeMeasurements = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformanceNew.EmployeePerformanceId).ToList();
                    foreach (var employeeMeasurement in employeeMeasurements)
                    {
                        // Delete related EmployeePerformanceScores
                        var actions = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == employeeMeasurement.EmpMeasurementId).ToList();
                        foreach (var action in actions)
                        {
                            db.EmployeePerformanceScores.Remove(action);
                        }

                        db.EmployeeMeasurement.Remove(employeeMeasurement);
                    }

                    // Delete the EmployeePerformance entity
                    db.EmployeePerformances.Remove(employeePerformanceNew);
                    db.SaveChanges();
                }
            }

            return Json(new[] { employeePerformanceNew }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult EmployeeMeasurements_Read([DataSourceRequest] DataSourceRequest request, int performanceId)
        {
            

            IQueryable<EmployeeMeasurement> measurements = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == performanceId);

            DataSourceResult result = measurements.ToDataSourceResult(request, measur => new
            {
                EmpMeasurementId = measur.EmpMeasurementId,
                EmployeePerformanceId = measur.EmployeePerformanceId,
                MeasurmentId = measur.MeasurmentId,
                Weight = measur.Weight,
                Result = GetResult(measur.EmpMeasurementId),
                Remark = measur.Remark
            });

            return Json(result);
        }
        public double GetResult(int employeeMeasurmentId)
        {
            var employeePerformanceScore = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == employeeMeasurmentId).ToList();

            if (employeePerformanceScore.Any())
            {
                return Convert.ToDouble(employeePerformanceScore.Sum(x => x.Score));
            }

            return 0;
        }
        ////////////////
        public ActionResult PerformanceActions_Read([DataSourceRequest] DataSourceRequest request, string empMeasurId)
        {
            int.TryParse(empMeasurId, out int employeeMeasurementId);

            IQueryable<EmployeePerformanceScore> actions = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == employeeMeasurementId);

            DataSourceResult result = actions.ToDataSourceResult(request,
                measur => new
                {
                    EmployeePerformanceScoreId = measur.EmployeePerformanceScoreId,
                    PerformanceActionId = measur.PerformanceActionId,
                    EmployeeMeasurementId = measur.EmployeeMeasurementId,
                    Weight = measur.Weight,
                    Score = measur.Score,
                    //IsChecked = measur.IsChecked,
                    Remark = measur.Remark,

                });
            return Json(result);

        }
        public ActionResult PerformanceActions_Update([DataSourceRequest] DataSourceRequest request, EmployeePerformanceScore employeePerformanceScore)
        {
            if (employeePerformanceScore.Score > employeePerformanceScore.Weight)
            {
                ModelState.AddModelError("", "Score can't exceed the weight");
                return Json(new[] { employeePerformanceScore }.ToDataSourceResult(request, ModelState));

            }

            var allowedScores = db.PerformanceAllowedScores.Select(x => x.ScoreValue).OrderBy(x => x).ToList();

            if (allowedScores.Any() && !allowedScores.Contains(employeePerformanceScore.Score))
            {
                var allowedValuesText = string.Join(", ", allowedScores);

                ModelState.AddModelError("Score",
                    $"Invalid score. Allowed values are: {allowedValuesText}. " +
                    "Please contact HR to modify the evaluation score configuration if needed.");

                return Json(new[] { employeePerformanceScore }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {

                var entity = new EmployeePerformanceScore
                {
                    EmployeePerformanceScoreId = employeePerformanceScore.EmployeePerformanceScoreId,
                    PerformanceActionId = employeePerformanceScore.PerformanceActionId,
                    EmployeeMeasurementId = employeePerformanceScore.EmployeeMeasurementId,
                    Weight = employeePerformanceScore.Weight,
                    Score = employeePerformanceScore.Score,
                    //.IsChecked = employeePerformanceScore.IsChecked,
                    Remark = employeePerformanceScore.Remark,
                };

                db.EmployeePerformanceScores.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                // UserHelper.OperationLog(Request.UserHostAddress, "Employee Performance  ", "Success", User.Identity.Name, User.Identity.Name + " update Employee performance of employee id " + entity.EmployeeId, Modules.HRM);

                #endregion

            }

            return Json(new[] { employeePerformanceScore }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PerformanceActions_Destroy([DataSourceRequest] DataSourceRequest request, EmployeePerformanceScore employeePerformanceScore)
        {
            if (employeePerformanceScore.Score > employeePerformanceScore.Weight)
            {
                ModelState.AddModelError("", "Score can't exceed the weight");
            }
            var performanceActions = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == employeePerformanceScore.EmployeeMeasurementId).ToList();
            var employeePerformaceId = performanceActions.Any() ? db.EmployeeMeasurement.Find(performanceActions.FirstOrDefault()?.EmployeeMeasurementId)?.EmployeePerformanceId : null;
            var employeePerformace = employeePerformaceId !=null? db.EmployeePerformances.Find(employeePerformaceId):null;
            var criteria = employeePerformace !=null? db.MeasurementDefinitions.Find(employeePerformace.EvaluationCriteriaTypeId):null;
            
            if (performanceActions.Count() == 1 && criteria !=null && (criteria.Lookup.EvalutionType == EvaluationType.Corporate_Result
                                                                        || criteria.Lookup.EvalutionType == EvaluationType.Companylevel
                                                                        || criteria.Lookup.EvalutionType == EvaluationType.Sector_Result
                                                                        || criteria.Lookup.EvalutionType == EvaluationType.MainDepartmentLevel
                                                                        || criteria.Lookup.EvalutionType == EvaluationType.Project_Manager))
            {
                
                if (ModelState.IsValid)
                {
                    var entity = db.EmployeePerformanceScores.Find(employeePerformanceScore.EmployeePerformanceScoreId);
                    if (entity != null)
                    {
                        // Update entity properties
                        entity.PerformanceActionId = employeePerformanceScore.PerformanceActionId;
                        entity.Weight = employeePerformanceScore.Weight;
                        entity.Score = employeePerformanceScore.Score;
                        entity.Remark = employeePerformanceScore.Remark;

                        // Set entity state to modified
                        db.Entry(entity).State = EntityState.Modified;

                        // Remove related entities if necessary
                        var employeeMeasurement = db.EmployeeMeasurement.Find(entity.EmployeeMeasurementId);
                        if (employeeMeasurement != null)
                        {
                            db.EmployeeMeasurement.Remove(employeeMeasurement);
                        }

                        var employeePerformance = db.EmployeePerformances.Find(employeeMeasurement?.EmployeePerformanceId);
                        if (employeePerformance != null)
                        {
                            db.EmployeePerformances.Remove(employeePerformance);
                        }
                        db.EmployeePerformanceScores.Remove(entity);
                        db.SaveChanges();

                        #region operation log
                        // Log operation
                        #endregion
                    }
                }
            }
           

            return Json(new[] { employeePerformanceScore }.ToDataSourceResult(request, ModelState));
        }
        ///////////////
        public JsonResult GetFiscalYearFromQuarterLength()
        {

            var fiscalYear = db.GLFiscalYears.OrderByDescending(x => x.GlFiscalYearId).ToList();

            return Json(fiscalYear.Select(p =>
            new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodOrQuarterList(string GlFiscalYearId)
        {
            List<Quarter> quarterList = new List<Quarter>();
            var quarterLength = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId.ToString() == GlFiscalYearId &&!x.IsClosed);
            if (quarterLength != null)
            {
                var quarter = db.Quarters.Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId&&!x.IsClosed).ToList();
                quarterList.AddRange(quarter);
            }

            return Json(quarterList.Select(p => new
            {
                QuarterId = p.QuarterId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public string ApproveEvaluation(int year, int period, int employee)
        {
            var employeePerformances = db.EmployeePerformances.Where(x => x.GlFiscalYearId == year && x.GlPeriodId == period && x.EmployeeId == employee&&x.IsApproved&&x.IsAcceptedByEmployee&&x.IsSecondLevelApproved).ToList();
            var status = "false";
            foreach ( var employeePerformance in employeePerformances)
            {
                employeePerformance.IsHRApproved = true;
                employeePerformance.FinalApprovalDate = DateTime.Now;
                employeePerformance.FinalApprover = User.Identity.Name;
                db.Entry(employeePerformance).State = EntityState.Modified;
                db.SaveChanges();
                status= "true";
            }
            if (employee != 0)
            {
                //var bossId = EmployeeHelper.GetSupervisor(placement.EmployeeId);
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(employee),
                    Title = " HR has approved your Performance ",
                    Message = $" you have performance has been approved by HR Approver",
                    URL = "/HRM/EmployeePerformanceAllSummaries/"

                };

                Notify.NotifyUser(notif);
            }
            return status;
        }
        public string CheckTheWeight(int evaluationCriteriaId, decimal resultValue)
        {
            // Get all measurement IDs tied to the definition
            var measureIds = db.Measurements
                .Where(x => x.MeasurementDefinitionId == evaluationCriteriaId)
                .Select(x => x.MeasurementId)
                .ToList();

            if (!measureIds.Any())
                return "No measurements found for the given evaluation criteria.";

            // Sum the weight of actions linked to these measurements
            decimal totalWeight = db.PerformanceActions
                .Where(x => x.MeasurementId != null && measureIds.Contains(x.MeasurementId.Value))
                .Sum(x => (decimal?)x.AcPercentageofWeight) ?? 0;

            if (resultValue > totalWeight)
            {
                return $"Maximum weight is {totalWeight}. You enter a value of {totalWeight} and below {totalWeight} only.";
            }

            return $"Your maximum allowed weight is {totalWeight}.";
        }
        public JsonResult GenerateEmployeeForPerofmanceEvaluation(int fiscalYearId, int periodId, int evaluationCriteriaId, string code, string branchId, string orgId, string lookupId, decimal resultValue)
        {
            var measureDefinition = db.MeasurementDefinitions
                .AsNoTracking()
                .FirstOrDefault(x => x.MeasurementDefinitionId == evaluationCriteriaId);
            if (measureDefinition == null)
            {
                return Json(new { Message = "Criteria must not be null", Status = "not-OK" }, JsonRequestBehavior.AllowGet);
            }
            var allowedScores = db.PerformanceAllowedScores.Select(x => x.ScoreValue).OrderBy(x => x).ToList();

            if (allowedScores.Any() && !allowedScores.Contains(resultValue))
            {
                var allowedValuesText = string.Join(", ", allowedScores);

                return Json(new { Message = $"Invalid score. Allowed values are: {allowedValuesText}. " +"Please contact HR to modify the evaluation score configuration if needed.",
                    Status = "not-OK" }, JsonRequestBehavior.AllowGet);

            }
            var lookup = db.Lookups.AsNoTracking().FirstOrDefault(x => x.LookupId == measureDefinition.LookupId);
            var measurementsDef = db.Measurements
                .Where(x => x.MeasurementDefinitionId == evaluationCriteriaId)
                .ToList();
            var actionsByMeasurement = db.PerformanceActions
                .Where(x => x.MeasurementId != null)
                .GroupBy(x => x.MeasurementId)
                .ToDictionary(g => g.Key, g => g.ToList());

            int.TryParse(orgId, out int orgstructureId);
            int.TryParse(lookupId, out int jobTitleId);
            int.TryParse(branchId, out int branchParsedId);

            var empPerformancesQuery = db.EmployeePerformances
                .Where(x => x.GlFiscalYearId == fiscalYearId &&
                            x.GlPeriodId == periodId &&
                            x.IsSecondLevelApproved &&
                            x.ResponsibilityLevel == measureDefinition.Role);

            if (!string.IsNullOrEmpty(code))
                empPerformancesQuery = empPerformancesQuery.Where(x => x.BusinessUnitId.ToString() == code);

            if (!string.IsNullOrEmpty(orgId))
                empPerformancesQuery = empPerformancesQuery.Where(x => x.OrgStructureId == orgstructureId);

            if (!string.IsNullOrEmpty(lookupId))
                empPerformancesQuery = empPerformancesQuery.Where(x => x.JobTitleId == jobTitleId);

            if (!string.IsNullOrEmpty(branchId))
                empPerformancesQuery = empPerformancesQuery.Where(x => x.BranchId == branchParsedId);

            var empPerformances = empPerformancesQuery.ToList();

            if (!empPerformances.Any())
            {
                return Json(new { Message = "Performance doesn't have any record", Status = "not-OK" }, JsonRequestBehavior.AllowGet);
            }

            var employeeIds = empPerformances.Select(x => x.EmployeeId).Distinct().ToList();

            var placements = db.Placements
                .Where(x => x.isCurrent && employeeIds.Contains(x.EmployeeId) && x.ResponsibilityLevel == measureDefinition.Role)
                .ToList();

            var existingPerformances = db.EmployeePerformances
                .Where(x => employeeIds.Contains(x.EmployeeId) &&
                            x.GlFiscalYearId == fiscalYearId &&
                            x.GlPeriodId == periodId &&
                            x.EvaluationCriteriaTypeId == evaluationCriteriaId &&
                            x.IsSecondLevelApproved)
                .ToList();

            var now = DateTime.Now;
            var createdBy = User.Identity.Name;

            var newPerformances = new List<EmployeePerformance>();
            var newMeasurements = new List<EmployeeMeasurement>();
            var newScores = new List<EmployeePerformanceScore>();
            var distctDmpPerformance = empPerformances.DistinctByField(x => x.EmployeeId).ToList();
            foreach (var empPerformance in distctDmpPerformance)
            {
                var placement = placements.FirstOrDefault(x => x.EmployeeId == empPerformance.EmployeeId);
                if (placement == null) continue;

                var existing = existingPerformances.FirstOrDefault(x => x.EmployeeId == empPerformance.EmployeeId);

                if (existing == null)
                {
                    var entity = new EmployeePerformance
                    {
                        EmployeeId = empPerformance.EmployeeId,
                        GlFiscalYearId = fiscalYearId,
                        GlPeriodId = periodId,
                        EvaluationCriteriaTypeId = evaluationCriteriaId,
                        BusinessUnitId = placement.BusinessUnitId,
                        OrgStructureId = placement.OrgStructureId,
                        BranchId = placement.BranchId,
                        JobTitleId = placement.LookupId,
                        IsCurrent = placement.isCurrent,
                        CriteriaCategoryId = measureDefinition.CriteriaCategoryId,
                        DirectBossId = placement.HeadId ?? 0,
                        ResponsibilityLevel = placement.ResponsibilityLevel,
                        Date = now,
                        DateCreated = now,
                        LastModified = now,
                        CreatedBy = createdBy,
                        ModifiedBy = createdBy,
                        Comment = createdBy,
                        Remark = createdBy,
                        EmployeeRemark = createdBy,
                        DeadLine = 0,
                        IsAcceptedByEmployee = true,
                        IsApproved = true,
                        IsSecondLevelApproved = true,
                        IsImmediateSuppurvisorApproved = true,
                        IsSectorApplied = lookup?.EvalutionType == EvaluationType.Sector_Result,
                        IsCorporateApplied = lookup?.EvalutionType == EvaluationType.Corporate_Result,
                        IsHRApproved = false,
                        IsHRLevelEvalution = true,
                        IsRejectedByEmployee = false,
                        Weight = 0,
                        Result = resultValue
                    };

                    db.EmployeePerformances.Add(entity);
                    db.SaveChanges();

                    var totalWeight = 0m;

                    foreach (var meas in measurementsDef)
                    {
                        var empMeasure = new EmployeeMeasurement
                        {
                            MeasurmentId = meas.MeasurementId,
                            EmployeePerformanceId = entity.EmployeePerformanceId,
                            Weight = meas.MesPercentageofWeight,
                            Result = resultValue
                        };

                        db.EmployeeMeasurement.Add(empMeasure);
                        db.SaveChanges();


                        if (actionsByMeasurement.TryGetValue(meas.MeasurementId, out var perfActions))
                        {
                            foreach (var act in perfActions)
                            {
                                var perfScore = new EmployeePerformanceScore
                                {
                                    PerformanceActionId = act.ActionId,
                                    EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                                    Weight = act.AcPercentageofWeight,
                                    Score = resultValue,
                                    Remark = ""
                                };

                                db.EmployeePerformanceScores.Add(perfScore);
                                totalWeight = act.AcPercentageofWeight;

                            }
                        }
                    }

                    entity.Weight = totalWeight;
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    existing.BusinessUnitId = placement.BusinessUnitId;
                    existing.OrgStructureId = placement.OrgStructureId;
                    existing.BranchId = placement.BranchId;
                    existing.JobTitleId = placement.LookupId;
                    existing.IsCurrent = placement.isCurrent;
                    existing.ResponsibilityLevel = placement.ResponsibilityLevel;
                    existing.IsAcceptedByEmployee = true;
                    existing.IsApproved = true;
                    existing.IsSecondLevelApproved = true;
                    existing.IsImmediateSuppurvisorApproved = true;
                    existing.IsSectorApplied = lookup?.EvalutionType == EvaluationType.Sector_Result;
                    existing.IsCorporateApplied = lookup?.EvalutionType == EvaluationType.Corporate_Result;
                    existing.Result = resultValue;

                    var measures = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == existing.EmployeePerformanceId).ToList();

                    if (!measures.Any())
                    {
                        foreach (var meas in measurementsDef)
                        {
                            var empMeasure = new EmployeeMeasurement
                            {
                                MeasurmentId = meas.MeasurementId,
                                EmployeePerformanceId = existing.EmployeePerformanceId,
                                Weight = meas.MesPercentageofWeight,
                                Result = meas.MesPercentageofWeight >= resultValue ? resultValue : meas.MesPercentageofWeight
                            };

                            db.EmployeeMeasurement.Add(empMeasure);
                            db.SaveChanges();

                            if (actionsByMeasurement.TryGetValue(meas.MeasurementId, out var perfActions))
                            {
                                foreach (var act in perfActions)
                                {
                                    var score = new EmployeePerformanceScore
                                    {
                                        PerformanceActionId = act.ActionId,
                                        EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                                        Weight = act.AcPercentageofWeight,
                                        Score = act.AcPercentageofWeight >= resultValue? resultValue: act.AcPercentageofWeight,
                                        Remark = ""
                                    };

                                    db.EmployeePerformanceScores.Add(score);
                                    existing.Weight = act.AcPercentageofWeight;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var meas in measures)
                        {
                            meas.Result = resultValue;
                            var perfActions = db.EmployeePerformanceScores
                                .Where(x => x.EmployeeMeasurementId == meas.EmpMeasurementId)
                                .ToList();

                            foreach (var score in perfActions)
                            {
                                var act = db.PerformanceActions.FirstOrDefault(x => x.ActionId == score.PerformanceActionId);
                                score.Score = resultValue;
                                if (act != null) score.Weight = act.AcPercentageofWeight;
                                db.Entry(score).State = EntityState.Modified;
                                existing.Weight = score.Weight;

                            }

                            db.Entry(meas).State = EntityState.Modified;
                        }
                    }

                    

                    db.Entry(existing).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new { Message = "Evaluation result has been successfully applied!", Status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GenerateEmployeeForPerofmanceEvaluationExisted(int fiscalYearId, int periodId, int evaluationCriteriaId, string code, string branchId, string orgId,  string lookupId, decimal resultValue)
        {
            var empPerformances = db.EmployeePerformances
                .Where(x => x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.IsSecondLevelApproved).ToList();
            var measureDefinition = db.MeasurementDefinitions.FirstOrDefault(x=>x.MeasurementDefinitionId==evaluationCriteriaId);
            if(measureDefinition == null)
            {
                return Json(new { Message = "critria must not null", Status = "not-OK" }, JsonRequestBehavior.AllowGet);

            }
            if (empPerformances.Any())
            {
                empPerformances = empPerformances
                    .Where(x => x.ResponsibilityLevel == measureDefinition.Role).ToList();
            }
            var lookup = db.Lookups.Find(measureDefinition.LookupId);

            if (!string.IsNullOrEmpty(code) && empPerformances.Any())
            {
                empPerformances = empPerformances
                    .Where(x => x.BusinessUnitId.ToString() == code).ToList();
            }

            if (!string.IsNullOrEmpty(orgId) && empPerformances.Any())
            {
                int.TryParse(orgId, out int orgstructureId);
                empPerformances = empPerformances.Where(x => x.OrgStructureId == orgstructureId).ToList();
            }

            if (!string.IsNullOrEmpty(lookupId) && empPerformances.Any())
            {
                int.TryParse(lookupId, out int lookUpId);
                empPerformances = empPerformances
                    .Where(x => x.JobTitleId == lookUpId).ToList();
            }

            if (!string.IsNullOrEmpty(branchId) && empPerformances.Any())
            {
                int.TryParse(branchId, out int BranchId);
                empPerformances = empPerformances
                    .Where(x => x.BranchId == BranchId).ToList();
            }

            if (!empPerformances.Any())
            {
                return Json(new { Message = "Performance doesn't have any record", Status = "not-OK" }, JsonRequestBehavior.AllowGet);
            }

            foreach (var empPerformance in empPerformances)
            {
                var placement = db.Placements.Where(x => x.isCurrent).Where(x => x.EmployeeId == empPerformance.EmployeeId&&x.ResponsibilityLevel==measureDefinition.Role).FirstOrDefault();

                var Performance = db.EmployeePerformances
                    .FirstOrDefault(x =>x.EmployeeId==empPerformance.EmployeeId 
                                        && x.GlFiscalYearId == fiscalYearId 
                                        && x.GlPeriodId == periodId 
                                        && x.EvaluationCriteriaTypeId == evaluationCriteriaId
                                        && x.IsSecondLevelApproved);
                if (Performance == null && placement != null)
                {
                    var categorId = db.MeasurementDefinitions.Find(evaluationCriteriaId);
                    var entity = new EmployeePerformance
                    {
                        EmployeeId = empPerformance.EmployeeId,
                        GlFiscalYearId = fiscalYearId,
                        GlPeriodId = periodId,
                        EvaluationCriteriaTypeId = evaluationCriteriaId,
                        BusinessUnitId = placement.BusinessUnitId,
                        OrgStructureId = placement.OrgStructureId,
                        BranchId = placement.BranchId,
                        JobTitleId = placement.LookupId,
                        IsCurrent = placement.isCurrent,
                        CriteriaCategoryId = categorId != null ? categorId.CriteriaCategoryId:0,
                        DirectBossId = placement.HeadId !=null?(int)placement.HeadId:0,
                        ResponsibilityLevel = placement.ResponsibilityLevel,
                        Date = DateTime.Now,
                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name,
                        Comment = User.Identity.Name,
                        Remark = User.Identity.Name,
                        EmployeeRemark = User.Identity.Name,
                        DeadLine = 0,
                        IsAcceptedByEmployee = true,
                        IsApproved = true,
                        IsSecondLevelApproved = true,
                        IsImmediateSuppurvisorApproved = true,
                        IsSectorApplied = lookup?.EvalutionType==EvaluationType.Sector_Result?true:false,
                        IsCorporateApplied = lookup?.EvalutionType == EvaluationType.Corporate_Result ? true : false,

                        IsHRApproved = false,
                        IsHRLevelEvalution = true,
                        IsRejectedByEmployee = false,
                        Weight = 0,
                        Result = resultValue
                    };

                    db.EmployeePerformances.Add(entity);
                    db.SaveChanges();

                    var measurements = db.Measurements.Where(x => x.MeasurementDefinitionId == entity.EvaluationCriteriaTypeId).ToList();

                    foreach (var meas in measurements)
                    {
                        var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == entity.EmployeePerformanceId);

                        if (checkExit == null)
                        {
                            var empMeasure = new EmployeeMeasurement();

                            empMeasure.MeasurmentId = meas.MeasurementId;
                            empMeasure.EmployeePerformanceId = entity.EmployeePerformanceId;
                            empMeasure.Weight = meas.MesPercentageofWeight;
                            empMeasure.Result = resultValue;

                            db.EmployeeMeasurement.Add(empMeasure);
                            db.SaveChanges();

                            var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId == empMeasure.MeasurmentId).ToList();

                            foreach (var per in performanceActions)
                            {
                                var performanceActionScore = new EmployeePerformanceScore
                                {
                                    PerformanceActionId = per.ActionId,
                                    EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                                    Weight = per.AcPercentageofWeight,
                                    Score = resultValue,
                                    Remark = "",
                                };

                                db.EmployeePerformanceScores.Add(performanceActionScore);
                                db.SaveChanges();
                            }
                        }
                       
                    }
                    // Update weight
                    entity.Weight = db.EmployeeMeasurement.Where(x=>x.EmployeePerformanceId==entity.EmployeePerformanceId).Sum(x=>x.Weight);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (Performance != null && placement != null)
                {
                    
                    Performance.BusinessUnitId = placement.BusinessUnitId;
                    Performance.OrgStructureId = placement.OrgStructureId;
                    Performance.BranchId = placement.BranchId;
                    Performance.JobTitleId = placement.LookupId;
                    Performance.IsCurrent = placement.isCurrent;
                    Performance.ResponsibilityLevel = placement.ResponsibilityLevel;
                    Performance.IsAcceptedByEmployee = true;
                    Performance.IsApproved = true;
                    Performance.IsSecondLevelApproved = true;
                    Performance.IsImmediateSuppurvisorApproved = true;
                    Performance.IsSectorApplied = lookup?.EvalutionType == EvaluationType.Sector_Result ? true : false;
                    Performance.IsCorporateApplied = lookup?.EvalutionType == EvaluationType.Corporate_Result ? true : false;
                  
                    Performance.IsImmediateSuppurvisorApproved = true;
                    Performance.Result = resultValue;
                    db.Entry(Performance).State = EntityState.Modified;
                    db.SaveChanges();
                    var measurements = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId).ToList();
                    if (measurements.Any())
                    {
                        foreach (var meas in measurements)
                        {
                            var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId);
                            if (checkExit != null)
                            {
                                var actions = db.PerformanceActions.Where(x => x.MeasurementId == checkExit.EmpMeasurementId).ToList();

                                var empMeasure = new EmployeeMeasurement();
                                checkExit.Weight = actions.Any()?actions.Sum(x=>x.AcPercentageofWeight):meas.Weight;
                                checkExit.Result = resultValue;
                                db.Entry(checkExit).State = EntityState.Modified;
                                db.SaveChanges();
                                var performanceActions = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == meas.EmpMeasurementId).ToList();
                                if (performanceActions.Any())
                                {
                                    foreach (var per in performanceActions)
                                    {
                                        var action = db.PerformanceActions.Where(x => x.ActionId == per.PerformanceActionId).FirstOrDefault();

                                        per.Weight = action !=null
                                            &&action.AcPercentageofWeight !=per.Weight?action.AcPercentageofWeight:per.Weight;
                                        per.Score = resultValue;
                                        db.Entry(per).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    var actiondetailss = db.PerformanceActions.Where(x => x.MeasurementId == meas.MeasurmentId).ToList();
                                    foreach (var action in actiondetailss)
                                    {
                                        var performanceActionScore = new EmployeePerformanceScore
                                        {
                                            PerformanceActionId = action.ActionId,
                                            EmployeeMeasurementId = checkExit.EmpMeasurementId,
                                            Weight = action.AcPercentageofWeight,
                                            Score = resultValue,
                                            Remark = "",
                                            Comment = "HR",
                                        };

                                        db.EmployeePerformanceScores.Add(performanceActionScore);
                                        db.SaveChanges();

                                    }
                                }

                            }
                        }
                        Performance.Weight = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId).Sum(x => x.Weight);
                        db.Entry(Performance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var measurementss = db.Measurements.Where(x => x.MeasurementDefinitionId == Performance.EvaluationCriteriaTypeId).ToList();

                        foreach (var meas in measurementss)
                        {
                            var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == Performance.EmployeePerformanceId);

                            if (checkExit == null)
                            {
                                var empMeasure = new EmployeeMeasurement();

                                empMeasure.MeasurmentId = meas.MeasurementId;
                                empMeasure.EmployeePerformanceId = Performance.EmployeePerformanceId;
                                empMeasure.Weight = meas.MesPercentageofWeight;
                                empMeasure.Result = resultValue;

                                db.EmployeeMeasurement.Add(empMeasure);
                                db.SaveChanges();

                                var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId == empMeasure.MeasurmentId).ToList();

                                foreach (var per in performanceActions)
                                {
                                    var performanceActionScore = new EmployeePerformanceScore
                                    {
                                        PerformanceActionId = per.ActionId,
                                        EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                                        Weight = per.AcPercentageofWeight,
                                        Score = resultValue,
                                        Remark = "",
                                    };

                                    db.EmployeePerformanceScores.Add(performanceActionScore);
                                    db.SaveChanges();
                                }
                            }

                        }
                        // Update weight
                        Performance.Weight = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId).Sum(x => x.Weight);
                        db.Entry(Performance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            var message = "Evaluation result has been successfully applied!";

            return Json(new { Message = message, Status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RefreshEmployeeToEvalutaeForAnEvaluted(int fiscalYearId, int periodId, int evaluationCriteriaId, string code, string branchId, string orgId,  string lookupId, decimal resultValue)
        {
            var empPerformances = db.EmployeePerformances
                .Where(x => x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.IsSecondLevelApproved).ToList();
            var measureDefinition = db.MeasurementDefinitions.FirstOrDefault(x=>x.MeasurementDefinitionId==evaluationCriteriaId);
            if(measureDefinition != null && empPerformances.Any())
            {
                empPerformances = empPerformances
                    .Where(x => x.ResponsibilityLevel == measureDefinition.Role).ToList();
            }
            if (!string.IsNullOrEmpty(code) && empPerformances.Any())
            {
                empPerformances = empPerformances
                    .Where(x => x.BusinessUnitId.ToString() == code).ToList();
            }

            if (!string.IsNullOrEmpty(orgId) && empPerformances.Any())
            {
                int.TryParse(orgId, out int orgstructureId);
                empPerformances = empPerformances.Where(x => x.OrgStructureId == orgstructureId).ToList();
            }

            if (!string.IsNullOrEmpty(lookupId) && empPerformances.Any())
            {
                int.TryParse(lookupId, out int lookUpId);
                empPerformances = empPerformances
                    .Where(x => x.JobTitleId == lookUpId).ToList();
            }

            if (!string.IsNullOrEmpty(branchId) && empPerformances.Any())
            {
                int.TryParse(branchId, out int BranchId);
                empPerformances = empPerformances
                    .Where(x => x.BranchId == BranchId).ToList();
            }

            if (!empPerformances.Any())
            {
                return Json(new { Message = "Performance doesn't have any record", Status = "not-OK" }, JsonRequestBehavior.AllowGet);
            }

            foreach (var empPerformance in empPerformances)
            {
                var placement = db.Placements.Where(x => x.isCurrent).Where(x => x.EmployeeId == empPerformance.EmployeeId).FirstOrDefault();

                var Performance = db.EmployeePerformances.FirstOrDefault(x =>x.EmployeeId==empPerformance.EmployeeId && x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.EvaluationCriteriaTypeId == evaluationCriteriaId && x.IsSecondLevelApproved);
                if (Performance == null && placement != null)
                {
                    var categorId = db.MeasurementDefinitions.Find(evaluationCriteriaId);
                    var entity = new EmployeePerformance
                    {
                        EmployeeId = empPerformance.EmployeeId,
                        GlFiscalYearId = fiscalYearId,
                        GlPeriodId = periodId,
                        EvaluationCriteriaTypeId = evaluationCriteriaId,
                        BusinessUnitId = placement.BusinessUnitId,
                        OrgStructureId = placement.OrgStructureId,
                        BranchId = placement.BranchId,
                        JobTitleId = placement.LookupId,
                        IsCurrent = placement.isCurrent,
                        CriteriaCategoryId = categorId != null ? categorId.CriteriaCategoryId:0,
                        DirectBossId = placement.HeadId !=null?(int)placement.HeadId:0,
                        ResponsibilityLevel = placement.ResponsibilityLevel,
                        Date = DateTime.Now,
                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name,
                        Comment = User.Identity.Name,
                        Remark = User.Identity.Name,
                        EmployeeRemark = User.Identity.Name,
                        DeadLine = 0,
                        IsAcceptedByEmployee = true,
                        IsApproved = true,
                        IsSecondLevelApproved = true,
                        IsImmediateSuppurvisorApproved = true,
                        IsSectorApplied = true,
                        IsCorporateApplied = true,
                        IsHRApproved = false,
                        IsHRLevelEvalution = true,
                        IsRejectedByEmployee = false,
                        Weight = 0,
                        Result = resultValue
                    };

                    db.EmployeePerformances.Add(entity);
                    db.SaveChanges();

                    var measurements = db.Measurements.Where(x => x.MeasurementDefinitionId == entity.EvaluationCriteriaTypeId).ToList();

                    foreach (var meas in measurements)
                    {
                        var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == entity.EmployeePerformanceId);

                        if (checkExit == null)
                        {
                            var empMeasure = new EmployeeMeasurement();

                            empMeasure.MeasurmentId = meas.MeasurementId;
                            empMeasure.EmployeePerformanceId = entity.EmployeePerformanceId;
                            empMeasure.Weight = meas.MesPercentageofWeight;
                            empMeasure.Result = resultValue;

                            db.EmployeeMeasurement.Add(empMeasure);
                            db.SaveChanges();

                            var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId == empMeasure.MeasurmentId).ToList();

                            foreach (var per in performanceActions)
                            {
                                var performanceActionScore = new EmployeePerformanceScore
                                {
                                    PerformanceActionId = per.ActionId,
                                    EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                                    Weight = per.AcPercentageofWeight,
                                    Score = resultValue,
                                    Remark = "",
                                };

                                db.EmployeePerformanceScores.Add(performanceActionScore);
                                db.SaveChanges();
                            }
                        }
                       
                    }
                    // Update weight
                    entity.Weight = db.EmployeeMeasurement.Where(x=>x.EmployeePerformanceId==entity.EmployeePerformanceId).Sum(x=>x.Weight);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (Performance != null && placement != null)
                {
                    
                    Performance.BusinessUnitId = placement.BusinessUnitId;
                    Performance.OrgStructureId = placement.OrgStructureId;
                    Performance.BranchId = placement.BranchId;
                    Performance.JobTitleId = placement.LookupId;
                    Performance.IsCurrent = placement.isCurrent;
                    Performance.ResponsibilityLevel = placement.ResponsibilityLevel;
                    Performance.IsAcceptedByEmployee = true;
                    Performance.IsApproved = true;
                    Performance.IsSecondLevelApproved = true;
                    Performance.IsImmediateSuppurvisorApproved = true;
                    Performance.Result = resultValue;
                    db.Entry(Performance).State = EntityState.Modified;
                    db.SaveChanges();
                    var measurements = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId).ToList();
                    if (measurements.Any())
                    {
                        foreach (var meas in measurements)
                        {
                            var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId);
                            if (checkExit != null)
                            {
                                var actions = db.PerformanceActions.Where(x => x.MeasurementId == checkExit.EmpMeasurementId).ToList();

                                var empMeasure = new EmployeeMeasurement();
                                checkExit.Weight = actions.Any()?actions.Sum(x=>x.AcPercentageofWeight):meas.Weight;
                                checkExit.Result = resultValue;
                                db.Entry(checkExit).State = EntityState.Modified;
                                db.SaveChanges();
                                var performanceActions = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == meas.EmpMeasurementId).ToList();
                                if (performanceActions.Any())
                                {
                                    foreach (var per in performanceActions)
                                    {
                                        var action = db.PerformanceActions.Where(x => x.ActionId == per.PerformanceActionId).FirstOrDefault();

                                        per.Weight = action !=null
                                            &&action.AcPercentageofWeight !=per.Weight?action.AcPercentageofWeight:per.Weight;
                                        per.Score = resultValue;
                                        db.Entry(per).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    var actiondetailss = db.PerformanceActions.Where(x => x.MeasurementId == meas.MeasurmentId).ToList();
                                    foreach (var action in actiondetailss)
                                    {
                                        var performanceActionScore = new EmployeePerformanceScore
                                        {
                                            PerformanceActionId = action.ActionId,
                                            EmployeeMeasurementId = checkExit.EmpMeasurementId,
                                            Weight = action.AcPercentageofWeight,
                                            Score = resultValue,
                                            Remark = "",
                                            Comment = "HR",
                                        };

                                        db.EmployeePerformanceScores.Add(performanceActionScore);
                                        db.SaveChanges();

                                    }
                                }

                            }
                        }
                        Performance.Weight = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId).Sum(x => x.Weight);
                        db.Entry(Performance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var measurementss = db.Measurements.Where(x => x.MeasurementDefinitionId == Performance.EvaluationCriteriaTypeId).ToList();

                        foreach (var meas in measurementss)
                        {
                            var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == Performance.EmployeePerformanceId);

                            if (checkExit == null)
                            {
                                var empMeasure = new EmployeeMeasurement();

                                empMeasure.MeasurmentId = meas.MeasurementId;
                                empMeasure.EmployeePerformanceId = Performance.EmployeePerformanceId;
                                empMeasure.Weight = meas.MesPercentageofWeight;
                                empMeasure.Result = resultValue;

                                db.EmployeeMeasurement.Add(empMeasure);
                                db.SaveChanges();

                                var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId == empMeasure.MeasurmentId).ToList();

                                foreach (var per in performanceActions)
                                {
                                    var performanceActionScore = new EmployeePerformanceScore
                                    {
                                        PerformanceActionId = per.ActionId,
                                        EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                                        Weight = per.AcPercentageofWeight,
                                        Score = resultValue,
                                        Remark = "",
                                    };

                                    db.EmployeePerformanceScores.Add(performanceActionScore);
                                    db.SaveChanges();
                                }
                            }

                        }
                        // Update weight
                        Performance.Weight = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId).Sum(x => x.Weight);
                        db.Entry(Performance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            var message = "Evaluation result has been successfully applied!";

            return Json(new { Message = message, Status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RefreshEmployeeToEvaluateForUnEvaluated(int fiscalYearId, int periodId, int evaluationCriteriaId, string code, string branchId, string orgId, string lookupId)
        {
            var measureDefinition = db.MeasurementDefinitions
                .AsNoTracking()
                .FirstOrDefault(x => x.MeasurementDefinitionId == evaluationCriteriaId);
            if (measureDefinition == null)
            {
                return Json(new { Message = "Criteria must not be null", Status = "not-OK" }, JsonRequestBehavior.AllowGet);
            }

            var lookup = db.Lookups.AsNoTracking().FirstOrDefault(x => x.LookupId == measureDefinition.LookupId);
            var measurementsDef = db.Measurements
                .Where(x => x.MeasurementDefinitionId == evaluationCriteriaId)
                .ToList();
            var actionsByMeasurement = db.PerformanceActions
                .Where(x => x.MeasurementId != null)
                .GroupBy(x => x.MeasurementId)
                .ToDictionary(g => g.Key, g => g.ToList());

            int.TryParse(orgId, out int orgstructureId);
            int.TryParse(lookupId, out int jobTitleId);
            int.TryParse(branchId, out int branchParsedId);

            var empPerformancesQuery = db.EmployeePerformances
                .Where(x => x.GlFiscalYearId == fiscalYearId &&
                            x.GlPeriodId == periodId &&
                            x.IsSecondLevelApproved &&
                            x.ResponsibilityLevel == measureDefinition.Role);

            var empPerformancesEvaluated = db.EmployeePerformances
                .Where(x => x.GlFiscalYearId == fiscalYearId &&
                            x.GlPeriodId == periodId &&
                            x.EvaluationCriteriaTypeId == evaluationCriteriaId &&
                            x.IsSecondLevelApproved &&
                            x.ResponsibilityLevel == measureDefinition.Role);

            if (!string.IsNullOrEmpty(code))
            {
                empPerformancesQuery = empPerformancesQuery.Where(x => x.BusinessUnitId.ToString() == code);
                empPerformancesEvaluated = empPerformancesEvaluated.Where(x => x.BusinessUnitId.ToString() == code);

            }

            if (!string.IsNullOrEmpty(orgId))
            {
                empPerformancesQuery = empPerformancesQuery.Where(x => x.OrgStructureId == orgstructureId);
                empPerformancesEvaluated = empPerformancesEvaluated.Where(x => x.OrgStructureId == orgstructureId);

            }

            if (!string.IsNullOrEmpty(lookupId))
            {
                empPerformancesQuery = empPerformancesQuery.Where(x => x.JobTitleId == jobTitleId);
                empPerformancesEvaluated = empPerformancesEvaluated.Where(x => x.JobTitleId == jobTitleId);

            }

            if (!string.IsNullOrEmpty(branchId))
            {
                empPerformancesQuery = empPerformancesQuery.Where(x => x.BranchId == branchParsedId);
                empPerformancesEvaluated = empPerformancesEvaluated.Where(x => x.BranchId == branchParsedId);

            }

            var empPerformances = empPerformancesQuery.ToList();
            if (!empPerformances.Any())
            {
                return Json(new { Message = "Performance doesn't have any record", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
            if (!empPerformancesEvaluated.Any())
            {
                return Json(new { Message = "No Evaluated Employee  in this Record", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
            
            var employeeIds = empPerformances.Select(x => x.EmployeeId)
                                    .Distinct()
                                    .ToList();
            // Ensure there's at least one evaluated performance
            var perform = empPerformancesEvaluated.FirstOrDefault();
            if (perform == null)
            {
                return Json(new { Message = "No evaluated performance found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            // Find measurement linked to the performance
            var empMeasu = db.EmployeeMeasurement
                .FirstOrDefault(x => x.EmployeePerformanceId == perform.EmployeePerformanceId);
            if (empMeasu == null)
            {
                return Json(new { Message = "No measurement found for the evaluated performance.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
            // Find score
            var scoreEntity = db.EmployeePerformanceScores
                .FirstOrDefault(x => x.EmployeeMeasurementId == empMeasu.EmpMeasurementId);

            if (scoreEntity == null || scoreEntity.Score == 0)
            {
                return Json(new { Message = "Result can't be null or zero.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            decimal resultValue = scoreEntity.Score;

            var placements = db.Placements
                .Where(x => x.isCurrent && employeeIds.Contains(x.EmployeeId) && x.ResponsibilityLevel == measureDefinition.Role)
                .ToList();

            var existingPerformances = db.EmployeePerformances
                .Where(x => employeeIds.Contains(x.EmployeeId) &&
                            x.GlFiscalYearId == fiscalYearId &&
                            x.GlPeriodId == periodId &&
                            x.EvaluationCriteriaTypeId == evaluationCriteriaId &&
                            x.IsSecondLevelApproved)
                .ToList();

            var now = DateTime.Now;
            var createdBy = User.Identity.Name;

            var newPerformances = new List<EmployeePerformance>();
            var newMeasurements = new List<EmployeeMeasurement>();
            var newScores = new List<EmployeePerformanceScore>();
            var distctDmpPerformance = empPerformances.DistinctByField(x => x.EmployeeId).ToList();

            foreach (var empPerformance in distctDmpPerformance)
            {
                var placement = placements.FirstOrDefault(x => x.EmployeeId == empPerformance.EmployeeId);
                if (placement == null) continue;

                var existing = existingPerformances.FirstOrDefault(x => x.EmployeeId == empPerformance.EmployeeId);

                if (existing == null)
                {
                    var entity = new EmployeePerformance
                    {
                        EmployeeId = empPerformance.EmployeeId,
                        GlFiscalYearId = fiscalYearId,
                        GlPeriodId = periodId,
                        EvaluationCriteriaTypeId = evaluationCriteriaId,
                        BusinessUnitId = placement.BusinessUnitId,
                        OrgStructureId = placement.OrgStructureId,
                        BranchId = placement.BranchId,
                        JobTitleId = placement.LookupId,
                        IsCurrent = placement.isCurrent,
                        CriteriaCategoryId = measureDefinition.CriteriaCategoryId,
                        DirectBossId = placement.HeadId ?? 0,
                        ResponsibilityLevel = placement.ResponsibilityLevel,
                        Date = now,
                        DateCreated = now,
                        LastModified = now,
                        CreatedBy = createdBy,
                        ModifiedBy = createdBy,
                        Comment = createdBy,
                        Remark = createdBy,
                        EmployeeRemark = createdBy,
                        DeadLine = 0,
                        IsAcceptedByEmployee = true,
                        IsApproved = true,
                        IsSecondLevelApproved = true,
                        IsImmediateSuppurvisorApproved = true,
                        IsSectorApplied = lookup?.EvalutionType == EvaluationType.Sector_Result,
                        IsCorporateApplied = lookup?.EvalutionType == EvaluationType.Corporate_Result,
                        IsHRApproved = false,
                        IsHRLevelEvalution = true,
                        IsRejectedByEmployee = false,
                        Weight = 0,
                        Result = resultValue
                    };

                    db.EmployeePerformances.Add(entity);
                    db.SaveChanges();

                    var totalWeight = 0m;

                    foreach (var meas in measurementsDef)
                    {
                        var empMeasure = new EmployeeMeasurement
                        {
                            MeasurmentId = meas.MeasurementId,
                            EmployeePerformanceId = entity.EmployeePerformanceId,
                            Weight = meas.MesPercentageofWeight,
                            Result = resultValue
                        };

                        db.EmployeeMeasurement.Add(empMeasure);
                        db.SaveChanges();

                        totalWeight = meas.MesPercentageofWeight;

                        if (actionsByMeasurement.TryGetValue(meas.MeasurementId, out var perfActions))
                        {
                            foreach (var act in perfActions)
                            {
                                var perfScore = new EmployeePerformanceScore
                                {
                                    PerformanceActionId = act.ActionId,
                                    EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                                    Weight = act.AcPercentageofWeight,
                                    Score = resultValue,
                                    Remark = ""
                                };

                                db.EmployeePerformanceScores.Add(perfScore);
                                totalWeight = act.AcPercentageofWeight; 
                            }
                        }
                    }

                    entity.Weight = totalWeight;
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    existing.BusinessUnitId = placement.BusinessUnitId;
                    existing.OrgStructureId = placement.OrgStructureId;
                    existing.BranchId = placement.BranchId;
                    existing.JobTitleId = placement.LookupId;
                    existing.IsCurrent = placement.isCurrent;
                    existing.ResponsibilityLevel = placement.ResponsibilityLevel;
                    existing.IsAcceptedByEmployee = true;
                    existing.IsApproved = true;
                    existing.IsSecondLevelApproved = true;
                    existing.IsImmediateSuppurvisorApproved = true;
                    existing.IsSectorApplied = lookup?.EvalutionType == EvaluationType.Sector_Result;
                    existing.IsCorporateApplied = lookup?.EvalutionType == EvaluationType.Corporate_Result;
                    existing.Result = resultValue;

                    var measures = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == existing.EmployeePerformanceId).ToList();

                    if (!measures.Any())
                    {
                        foreach (var meas in measurementsDef)
                        {
                            var empMeasure = new EmployeeMeasurement
                            {
                                MeasurmentId = meas.MeasurementId,
                                EmployeePerformanceId = existing.EmployeePerformanceId,
                                Weight = meas.MesPercentageofWeight,
                                Result = resultValue
                            };

                            db.EmployeeMeasurement.Add(empMeasure);
                            db.SaveChanges();

                            if (actionsByMeasurement.TryGetValue(meas.MeasurementId, out var perfActions))
                            {
                                foreach (var act in perfActions)
                                {
                                    var score = new EmployeePerformanceScore
                                    {
                                        PerformanceActionId = act.ActionId,
                                        EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                                        Weight = act.AcPercentageofWeight,
                                        Score = resultValue,
                                        Remark = ""
                                    };

                                    db.EmployeePerformanceScores.Add(score);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var meas in measures)
                        {
                            meas.Result = resultValue;
                            var perfActions = db.EmployeePerformanceScores
                                .Where(x => x.EmployeeMeasurementId == meas.EmpMeasurementId)
                                .ToList();

                            foreach (var score in perfActions)
                            {
                                var act = db.PerformanceActions.FirstOrDefault(x => x.ActionId == score.PerformanceActionId);
                                score.Score = resultValue;
                                if (act != null) score.Weight = act.AcPercentageofWeight;
                                db.Entry(score).State = EntityState.Modified;
                                existing.Weight = score.Weight;

                            }

                            db.Entry(meas).State = EntityState.Modified;
                        }
                    }

                

                    db.Entry(existing).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new { Message = "Evaluation result has been successfully applied!", Status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RefreshEmployeeToEvaluateForUnEvaluatedExisted(
                                           int fiscalYearId, 
                                           int periodId, 
                                           int evaluationCriteriaId,
                                           string code,
                                           string branchId, 
                                           string orgId, 
                                           string lookupId)
        {
            try
            {
                var measureDefinition = db.MeasurementDefinitions
               .AsNoTracking()
               .FirstOrDefault(x => x.MeasurementDefinitionId == evaluationCriteriaId);
                if (measureDefinition == null)
                {
                    return Json(new { Message = "Criteria must not be null", Status = "not-OK" }, JsonRequestBehavior.AllowGet);
                }
                // 1️⃣ Get all evaluated employees for the given criteria
                var evaluatedEmployees = db.EmployeePerformances
                    .Where(x => x.GlFiscalYearId == fiscalYearId
                                && x.GlPeriodId == periodId
                                && x.IsSecondLevelApproved
                                && x.EvaluationCriteriaTypeId == evaluationCriteriaId)
                    .Select(x => new { x.EmployeeId,
                                       x.BranchId,
                                       x.BusinessUnitId,
                                       x.OrgStructureId,
                                       x.ResponsibilityLevel,
                                       x.Result })  // Select EmployeeId and Result
                    .ToList();

                if (!evaluatedEmployees.Any())
                {
                    return Json(new { Message = "No evaluated employees found for this criteria.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
                }

                // 2️⃣ Find unevaluated employees who have other approved records
                var evalatedEmployeeIds = evaluatedEmployees.Select(x => x.EmployeeId).ToList();
                var unevaluatedEmployees = db.EmployeePerformances
                    .Where(x => x.GlFiscalYearId == fiscalYearId
                                && x.GlPeriodId == periodId
                                && x.IsSecondLevelApproved 
                                && x.EvaluationCriteriaTypeId != evaluationCriteriaId // Different criteria
                                && !evalatedEmployeeIds.Contains(x.EmployeeId))
                                .ToList();
                if (!string.IsNullOrEmpty(code) && unevaluatedEmployees.Any())
                {
                    int.TryParse(code, out int cc);
                    unevaluatedEmployees = unevaluatedEmployees
                        .Where(x => x.BusinessUnitId == cc).ToList();
                    evaluatedEmployees = evaluatedEmployees
                        .Where(x => x.BusinessUnitId == cc).ToList();
                }

                if (!string.IsNullOrEmpty(orgId) && unevaluatedEmployees.Any())
                {
                    int.TryParse(orgId, out int orgstructureId);
                    unevaluatedEmployees = unevaluatedEmployees.Where(x => x.OrgStructureId == orgstructureId).ToList();
                    evaluatedEmployees = evaluatedEmployees.Where(x => x.OrgStructureId == orgstructureId).ToList();
                }
                if (!string.IsNullOrEmpty(branchId) && unevaluatedEmployees.Any())
                {
                    int.TryParse(branchId, out int BranchId);
                    unevaluatedEmployees = unevaluatedEmployees
                        .Where(x => x.BranchId == BranchId).ToList();
                    evaluatedEmployees = evaluatedEmployees
                        .Where(x => x.BranchId == BranchId).ToList();
                }
                if (!evaluatedEmployees.Any())
                {
                    return Json(new { Message = "No evaluated employees found for this criteria.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
                }
                if (!unevaluatedEmployees.Any())
                {
                    return Json(new { Message = "No eligible unevaluated employees found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
                }
                var unevaluatedEmployeeIds = unevaluatedEmployees
                .Select(x => x.EmployeeId)
                .Distinct().ToList();
                // 3️⃣ Copy evaluation data to unevaluated employees
                foreach (var empId in unevaluatedEmployeeIds)
                {
                    var placement = db.Placements.Where(x => x.EmployeeId == empId).OrderByDescending(x => x.PlacementId).FirstOrDefault();
                    if (placement == null) continue;

                    // Get the result value from the previously evaluated employee
                    var resultValue = evaluatedEmployees
                        .Where(x=>x.ResponsibilityLevel ==placement.ResponsibilityLevel)
                        .FirstOrDefault()?.Result;

                    if (resultValue == null)
                    {
                        continue; // Skip if no result value found
                    }

                    var newEval = new EmployeePerformance
                    {
                        EmployeeId = empId,
                        GlFiscalYearId = fiscalYearId,
                        GlPeriodId = periodId,
                        EvaluationCriteriaTypeId = evaluationCriteriaId,
                        BusinessUnitId = placement.BusinessUnitId,
                        OrgStructureId = placement.OrgStructureId,
                        BranchId = placement.BranchId,
                        JobTitleId = placement.LookupId,
                        IsCurrent = placement.isCurrent,
                        CriteriaCategoryId = 0,
                        DirectBossId = placement.HeadId != null ? (int)placement.HeadId : 0,
                        ResponsibilityLevel = placement.ResponsibilityLevel,
                        Date = DateTime.Now,
                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name,
                        Comment = "Auto-generated based on previous evaluations",
                        Remark = "Auto-generated",
                        EmployeeRemark = "Auto-generated",
                        DeadLine = 0,
                        IsAcceptedByEmployee = true,
                        IsApproved = true,
                        IsSecondLevelApproved = true,
                        IsImmediateSuppurvisorApproved = true,
                        IsSectorApplied = true,
                        IsCorporateApplied = true,
                        IsHRApproved = false,
                        IsHRLevelEvalution = true,
                        IsRejectedByEmployee = false,
                        Weight = 0,
                        Result = resultValue.Value // Use the result value from the evaluated employee
                    };

                    db.EmployeePerformances.Add(newEval);
                    db.SaveChanges();

                    // 4️⃣ Assign Measurements
                    var measurements = db.Measurements
                        .Where(x => x.MeasurementDefinitionId == newEval.EvaluationCriteriaTypeId)
                        .ToList();

                    foreach (var meas in measurements)
                    {
                        var empMeasure = new EmployeeMeasurement
                        {
                            MeasurmentId = meas.MeasurementId,
                            EmployeePerformanceId = newEval.EmployeePerformanceId,
                            Weight = meas.MesPercentageofWeight,
                            Result = resultValue.Value
                        };

                        db.EmployeeMeasurement.Add(empMeasure);
                        db.SaveChanges();

                        // Assign Performance Actions
                        var performanceActions = db.PerformanceActions
                            .Where(x => x.MeasurementId == empMeasure.MeasurmentId)
                            .ToList();

                        foreach (var action in performanceActions)
                        {
                            var performanceActionScore = new EmployeePerformanceScore
                            {
                                PerformanceActionId = action.ActionId,
                                EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                                Weight = action.AcPercentageofWeight,
                                Score = resultValue.Value,
                                Remark = ""
                            };

                            db.EmployeePerformanceScores.Add(performanceActionScore);
                            db.SaveChanges();
                        }
                    }

                    // 5️⃣ Update Weight
                    newEval.Weight = db.EmployeeMeasurement
                        .Where(x => x.EmployeePerformanceId == newEval.EmployeePerformanceId)
                        .Sum(x => x.Weight);

                    db.Entry(newEval).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new { Message = "Evaluation copied successfully.", Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = "An error occurred: " + ex.Message, Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
        }

        public List<int> GetActiveEmployeeFromCurrentUserBranch(string user)
        {
            List<int> employeeIdLists = new List<int>();

            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(user);

            var placements = db.Placements.Where(x => mybraches.Contains(x.BranchId) && x.isCurrent).ToList();
            foreach (var place in placements)
            {
                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId && x.IsActive);
                if (emp != null)
                {
                    employeeIdLists.Add(place.EmployeeId);
                }

            }

            return employeeIdLists;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

