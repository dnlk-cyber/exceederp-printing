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

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,HrmConstants.HRAdmin, HrmConstants.HRPerformanceUser)]
    public class EmployeePerformanceAllSummariesController : BaseController
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
                  FullNameAmharic = i.Code+"-"+i.AmharicName,
                  FullNameaEnglish = i.Code + "-" + i.Name,
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
            ViewData["Period"] = db.Quarters.Select(x=>new { x.QuarterId,x.Name}).ToList();
            var  measurments = db.MeasurementDefinitions.Select(p => new
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
            var lookup = db.Lookups.Find(lookupId);
            if (lookup != null)
            {
                return lookup.Description;
            }
            return "";
        }
        private decimal GetWeight(int id)
        {
            var weight = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == id).ToList();
            if (weight.Any())
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
        public ActionResult UpdateDirectBoss()
        {
            var placements = db.Placements
                .Where(x => x.isCurrent && x.IsHead)
                .ToList();
            foreach (var place in placements)
            {
                //var subordinates = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(place.EmployeeId).Where(x => x != place.EmployeeId);
                var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(place.EmployeeId).Where(x => x != place.EmployeeId);
                foreach (var empId in subordinates)
                {
                    var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId == empId).ToList();
                    foreach (var employeePerformance in employeePerformances)
                    {
                        employeePerformance.DirectBossId = place.EmployeeId;
                        db.Entry(employeePerformance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return View("Success");
        }
        public ActionResult UpdatePerformance()
        {
            var empPerformances = db.EmployeePerformances.ToList();
            foreach (var empPerformance in empPerformances)
            {
                var placement = db.Placements.Where(x => x.isCurrent).Where(x => x.EmployeeId == empPerformance.EmployeeId).FirstOrDefault();
                if (placement != null)
                {
                    
                    empPerformance.OrgStructureId = placement.OrgStructureId;
                    empPerformance.BranchId = placement.BranchId;
                    empPerformance.DirectBossId = placement.HeadId??0;
                    empPerformance.BusinessUnitId = placement.BusinessUnitId;
                    empPerformance.ResponsibilityLevel = placement.ResponsibilityLevel;
                    empPerformance.JobTitleId = placement.LookupId;
                    empPerformance.IsCurrent = placement.isCurrent;
                    db.Entry(empPerformance).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var placementss = db.Placements.Where(x => x.EmployeeId == empPerformance.EmployeeId).OrderByDescending(x=>x.PlacementId).FirstOrDefault();
                    if (placementss != null)
                    {

                        empPerformance.OrgStructureId = placementss.OrgStructureId;
                        empPerformance.BranchId = placementss.BranchId;
                        empPerformance.DirectBossId = placementss.HeadId ?? 0;
                        empPerformance.BusinessUnitId = placementss.BusinessUnitId;
                        empPerformance.ResponsibilityLevel = placementss.ResponsibilityLevel;
                        empPerformance.JobTitleId = placementss.LookupId;
                        empPerformance.IsCurrent = placementss.isCurrent;
                        db.Entry(empPerformance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return View("Success");
        }
        public JsonResult RefreshPerformanceData(int yearId)
        {
            // Step 1: Load performances for the year
            var empPerformances = db.EmployeePerformances
                                    .Where(x => x.GlFiscalYearId == yearId)
                                    .ToList();

            if (!empPerformances.Any())
            {
                return Json(new
                {
                    Message = "No employee performance records found for the selected year.",
                    Status = "NotFound"
                }, JsonRequestBehavior.AllowGet);
            }

            // Step 2: Get relevant employee IDs
            var employeeIds = empPerformances.Select(x => x.EmployeeId).Distinct().ToList();

            // Step 3: Load only relevant placements
            var placements = db.Placements
                               .Where(x => employeeIds.Contains(x.EmployeeId))
                               .ToList();

            // Step 4: Organize placements by employeeId
            var placementMap = placements
                .GroupBy(p => p.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(p => p.isCurrent)  // true first
                          .ThenByDescending(p => p.PlacementId) // then most recent
                          .FirstOrDefault()
                );

            // Step 5: Update each performance record with the best placement
            foreach (var empPerformance in empPerformances)
            {
                if (placementMap.TryGetValue(empPerformance.EmployeeId, out var placement) && placement != null)
                {
                    empPerformance.OrgStructureId = placement.OrgStructureId;
                    empPerformance.BranchId = placement.BranchId;
                    empPerformance.DirectBossId = placement.HeadId ?? 0;
                    empPerformance.BusinessUnitId = placement.BusinessUnitId;
                    empPerformance.ResponsibilityLevel = placement.ResponsibilityLevel;
                    empPerformance.JobTitleId = placement.LookupId;
                    empPerformance.IsCurrent = placement.isCurrent;

                    db.Entry(empPerformance).State = EntityState.Modified;
                }
            }

            db.SaveChanges();

            return Json(new
            {
                Message = "Employee performance records have been updated successfully.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }
        private bool GetHRLevelApprovals(int empId, int fiscalyearId, int periodId)
        {
            var records = db.EmployeePerformances
                .Where(x => x.EmployeeId == empId && x.GlFiscalYearId == fiscalyearId && x.GlPeriodId == periodId&&x.IsSecondLevelApproved);

            // Return true only if there are matching records and all are approved
            return records.Any() && records.All(x => x.IsHRApproved);
        }
        public ActionResult EmployeePerformanceAllSummaries_Read([DataSourceRequest] DataSourceRequest request, string fiscalYear)
        {
            int? fiscalYearId = null;

            if (int.TryParse(fiscalYear, out int parsedYear))
            {
                fiscalYearId = parsedYear;
            }

            // Step 1: Batch calculate HR approvals and store in dictionary
            var approvalQuery = db.EmployeePerformances
                .Where(x => x.IsSecondLevelApproved);

            if (fiscalYearId.HasValue)
            {
                approvalQuery = approvalQuery.Where(x => x.GlFiscalYearId == fiscalYearId.Value);
            }

            var approvals = approvalQuery
                .GroupBy(x => new { x.EmployeeId, x.GlFiscalYearId, x.GlPeriodId })
                .Select(g => new
                {
                    g.Key.EmployeeId,
                    g.Key.GlFiscalYearId,
                    g.Key.GlPeriodId,
                    AreAllHRApproved = g.All(x => x.IsHRApproved)
                })
                .ToList()
                .ToDictionary(
                    x => (x.EmployeeId, x.GlFiscalYearId, x.GlPeriodId),
                    x => x.AreAllHRApproved
                );

            // Step 2: Query only distinct/most recent approved records
            var performanceQuery = db.EmployeePerformances
                .Where(x => x.IsSecondLevelApproved);

            if (fiscalYearId.HasValue)
            {
                performanceQuery = performanceQuery.Where(x => x.GlFiscalYearId == fiscalYearId.Value);
            }

            var employeePerformances = performanceQuery
                .GroupBy(x => new { x.EmployeeId, x.GlFiscalYearId, x.GlPeriodId })
                .Select(g => g.OrderByDescending(x => x.GlFiscalYearId).FirstOrDefault())
                .ToList()
                .Select(employeePerformance => new
                {
                    EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
                    EmployeeId = employeePerformance.EmployeeId,
                    GlFiscalYearId = employeePerformance.GlFiscalYearId,
                    GlPeriodId = employeePerformance.GlPeriodId,
                    BusinessUnitId = employeePerformance.BusinessUnitId,
                    OrgStructureId = employeePerformance.OrgStructureId,
                    ResponsibilityLevel = employeePerformance.ResponsibilityLevel,
                    BranchId = employeePerformance.BranchId,
                    IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
                    IsImmediateSuppurvisorApproved = employeePerformance.IsImmediateSuppurvisorApproved,
                    IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
                    IsApproved = employeePerformance.IsApproved,
                    IsHRRejected = employeePerformance.IsHRRejected,
                    IsHRApproved = employeePerformance.IsHRApproved,
                    IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
                    IsCorporateApplied = employeePerformance.IsCorporateApplied,
                    IsSectorApplied = employeePerformance.IsSectorApplied,
                    DateCreated = employeePerformance.DateCreated,
                    ImmediateSuppurvisorApprovalDate = employeePerformance.ImmediateSuppurvisorApprovalDate,
                    SecondLevelApprovalDate = employeePerformance.SecondLevelApprovalDate,
                    FinalApprovalDate = employeePerformance.FinalApprovalDate,
                    Date = employeePerformance.Date,
                    IsCurrent = employeePerformance.IsCurrent,
                    ReasonFor2ndRejection = employeePerformance.ReasonFor2ndRejection,
                    Comment = employeePerformance.Comment,
                    ReasonForHRRejection = employeePerformance.ReasonForHRRejection,
                    EmployeeRemark = employeePerformance.EmployeeRemark,
                    Remark = employeePerformance.Remark,

                    AreAllHRLevelApproved = approvals.TryGetValue(
                        (employeePerformance.EmployeeId, employeePerformance.GlFiscalYearId, employeePerformance.GlPeriodId),
                        out var approved) ? approved : false
                });

            // Step 3: Apply Kendo filtering, sorting, paging
            var result = employeePerformances.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmployeePerformanceSummaries_OnePerson_Read([DataSourceRequest] DataSourceRequest request,
         int employeeId, int fiscalyearId, int quarterId)
        {
            var query = db.EmployeePerformances
                .Where(x => x.EmployeeId == employeeId
                            && x.GlFiscalYearId == fiscalyearId
                            && x.GlPeriodId == quarterId
                            && x.IsApproved
                            && (x.IsAcceptedByEmployee||x.IsRejectedByEmployee)
                            && x.IsSecondLevelApproved)
                .Select(employeePerformance => new
                {
                    EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
                    EmployeeId = employeePerformance.EmployeeId,
                    GlFiscalYearId = employeePerformance.GlFiscalYearId,
                    GlPeriodId = employeePerformance.GlPeriodId,
                    EvaluationCriteriaTypeId = employeePerformance.EvaluationCriteriaTypeId,
                    OrgStructureId = employeePerformance.OrgStructureId,
                    BranchId = employeePerformance.BranchId,
                    BusinessUnitId = employeePerformance.BusinessUnitId,
                    ResponsibilityLevel = employeePerformance.ResponsibilityLevel,
                    IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
                    IsImmediateSuppurvisorApproved = employeePerformance.IsImmediateSuppurvisorApproved,
                    IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
                    IsApproved = employeePerformance.IsApproved,
                    IsHRApproved = employeePerformance.IsHRApproved,
                    IsCurrent = employeePerformance.IsCurrent,
                    IsCorporateApplied = employeePerformance.IsCorporateApplied,
                    IsSectorApplied = employeePerformance.IsSectorApplied,
                    DateCreated = employeePerformance.DateCreated,
                    Date = employeePerformance.Date,
                    ImmediateSuppurvisorApprovalDate = employeePerformance.ImmediateSuppurvisorApprovalDate,
                    SecondLevelApprovalDate = employeePerformance.SecondLevelApprovalDate,
                    FinalApprovalDate = employeePerformance.FinalApprovalDate,
                    IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
                    IsHRRejected = employeePerformance.IsHRRejected,
                    ReasonForHRRejection = employeePerformance.ReasonForHRRejection,
                    ReasonFor2ndRejection = employeePerformance.ReasonFor2ndRejection,
                    Comment = employeePerformance.Comment,
                    EmployeeRemark = employeePerformance.EmployeeRemark,
                    Remark = employeePerformance.Remark,
                });

            // Materialize the result to avoid executing GetWeight/GetEvaluationResult on the database side
            var projectedList = query.ToList();

            // Call methods only once per record — consider batch fetching if hitting DB
            var result = projectedList
                .Select(e => new
                {
                    e.EmployeePerformanceId,
                    e.EmployeeId,
                    e.GlFiscalYearId,
                    e.GlPeriodId,
                    e.EvaluationCriteriaTypeId,
                    e.OrgStructureId,
                    e.BranchId,
                    e.BusinessUnitId,
                    e.ResponsibilityLevel,
                    e.IsAcceptedByEmployee,
                    e.IsImmediateSuppurvisorApproved,
                    e.IsSecondLevelRejected,
                    e.IsApproved,
                    e.IsHRApproved,
                    e.IsCurrent,
                    e.IsCorporateApplied,
                    e.IsSectorApplied,
                    e.DateCreated,
                    e.Date,
                    e.ImmediateSuppurvisorApprovalDate,
                    e.SecondLevelApprovalDate,
                    e.FinalApprovalDate,
                    e.IsSecondLevelApproved,
                    e.IsHRRejected,
                    e.ReasonForHRRejection,
                    ReasonFor2ndRejection = e.ReasonFor2ndRejection,
                    Comment = e.Comment,
                    EmployeeRemark = e.EmployeeRemark,
                    e.Remark,
                    Result = GetEvaluationWeightandResult(e.EmployeePerformanceId),
                    Weight = GetWeight(e.EmployeePerformanceId)
                })
                .ToDataSourceResult(request);

            return Json(result);
        }

        //public ActionResult EmployeePerformanceAllSummaries_Read([DataSourceRequest] DataSourceRequest request)
        //{

        //    var employeePerformances = db.EmployeePerformances
        //                        .Where(x => x.IsSecondLevelApproved).DistinctBy(x => new
        //                        {
        //                            x.EmployeeId,
        //                            x.GlFiscalYearId,
        //                            x.GlPeriodId,

        //                        }).OrderByDescending(x => x.GlFiscalYearId).ToList();

        //    DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
        //    {
        //        EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
        //        EmployeeId = employeePerformance.EmployeeId,
        //        GlFiscalYearId = employeePerformance.GlFiscalYearId,
        //        GlPeriodId = employeePerformance.GlPeriodId,
        //        BusinessUnitId = employeePerformance.BusinessUnitId,
        //        OrgStructureId = employeePerformance.OrgStructureId,
        //        ResponsibilityLevel = employeePerformance.ResponsibilityLevel,
        //        BranchId = employeePerformance.BranchId,
        //        IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
        //        IsImmediateSuppurvisorApproved = employeePerformance.IsImmediateSuppurvisorApproved,
        //        IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
        //        IsApproved = employeePerformance.IsApproved,
        //        IsHRApproved = employeePerformance.IsHRApproved,
        //        IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
        //        IsCorporateApplied = employeePerformance.IsCorporateApplied,
        //        IsSectorApplied = employeePerformance.IsSectorApplied,
        //        DateCreated = employeePerformance.DateCreated,
        //        ImmediateSuppurvisorApprovalDate = employeePerformance.ImmediateSuppurvisorApprovalDate,
        //        SecondLevelApprovalDate = employeePerformance.SecondLevelApprovalDate,
        //        FinalApprovalDate = employeePerformance.FinalApprovalDate,
        //        Date = employeePerformance.Date,
        //        IsCurrent = employeePerformance.IsCurrent,
        //        IsHRRejected = employeePerformance.IsHRRejected,
        //        ReasonForHRRejection = employeePerformance.ReasonForHRRejection,
        //        //Result = GetEvaluationWeightandResult(employeePerformance.EmployeePerformanceId),
        //        //Weight = GetWeight(employeePerformance.EmployeePerformanceId),
        //        AreAllHRLevelApproved = GetHRLevelApprovals(employeePerformance.EmployeeId, employeePerformance.GlFiscalYearId, employeePerformance.GlPeriodId),
        //        //Result = employeePerformance.Result,
        //        //Result = GetResult(employeePerformance.EmpMeasurementId),
        //        Remark = employeePerformance.Remark
        //    });

        //    return Json(result);
        //}
        //public ActionResult EmployeePerformanceSummaries_OnePerson_Read([DataSourceRequest] DataSourceRequest request,int employeeId,int fiscalyearId, int quarterId)
        //{
        //    List<int> employees = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
        //    //var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //    //var subordinates = EmployeeHelper.GetSubordinates(id);
        //    var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId==employeeId&&x.GlFiscalYearId==fiscalyearId&&x.GlPeriodId==quarterId&x.IsApproved&&x.IsAcceptedByEmployee&&x.IsSecondLevelApproved).ToList();     
        //    //IQueryable<EmployeePerformance> employeePerformances = db.EmployeePerformances;
        //    DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
        //    {
        //        EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
        //        EmployeeId = employeePerformance.EmployeeId,
        //        GlFiscalYearId = employeePerformance.GlFiscalYearId,
        //        GlPeriodId = employeePerformance.GlPeriodId,
        //        EvaluationCriteriaTypeId = employeePerformance.EvaluationCriteriaTypeId,
        //        OrgStructureId = employeePerformance.OrgStructureId,
        //        BranchId = employeePerformance.BranchId,
        //        BusinessUnitId = employeePerformance.BusinessUnitId,
        //        ResponsibilityLevel = employeePerformance.ResponsibilityLevel,
        //        IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
        //        IsImmediateSuppurvisorApproved = employeePerformance.IsImmediateSuppurvisorApproved,
        //        IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
        //        IsApproved = employeePerformance.IsApproved,
        //        IsHRApproved = employeePerformance.IsHRApproved,
        //        IsCurrent = employeePerformance.IsCurrent,
        //        IsCorporateApplied = employeePerformance.IsCorporateApplied,
        //        IsSectorApplied = employeePerformance.IsSectorApplied,
        //        DateCreated = employeePerformance.DateCreated,
        //        Date = employeePerformance.Date,
        //        ImmediateSuppurvisorApprovalDate = employeePerformance.ImmediateSuppurvisorApprovalDate,
        //        SecondLevelApprovalDate = employeePerformance.SecondLevelApprovalDate,
        //        FinalApprovalDate = employeePerformance.FinalApprovalDate,
        //        IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
        //        IsHRRejected = employeePerformance.IsHRRejected,
        //        ReasonForHRRejection = employeePerformance.ReasonForHRRejection,
        //        Result = GetEvaluationWeightandResult(employeePerformance.EmployeePerformanceId),
        //        Weight = GetWeight(employeePerformance.EmployeePerformanceId),
        //        //AreAllHRLevelApproved = GetHRLevelApprovals(employeePerformance.EmployeeId, employeePerformance.GlFiscalYearId, employeePerformance.GlPeriodId),

        //        Remark = employeePerformance.Remark
        //    });

        //    return Json(result);
        //}
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

           
            var allowedScores = db.EmployeePerformanceScores.Select(x => x.Score).OrderBy(x => x) .ToList();

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
        ///////////////
        
        public JsonResult GetFiscalYear()
        {

            var fiscalYear = db.GLFiscalYears.OrderByDescending(x => x.GlFiscalYearId).ToList();

            return Json(fiscalYear.Select(p =>
            new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodList(string GlFiscalYearId)
        {
            List<Quarter> quarterList = new List<Quarter>();
            var quarterLength = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId.ToString() == GlFiscalYearId);
            if (quarterLength != null)
            {
                var quarter = db.Quarters.Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId).ToList();
                quarterList.AddRange(quarter);
            }

            return Json(quarterList.Select(p => new
            {
                QuarterId = p.QuarterId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApproveEvaluation(int year, int period, int employee)
        {
            // Fetch all employee performances in one query
            var employeePerformances = db.EmployeePerformances
                .Where(x => x.GlFiscalYearId == year 
                            && x.GlPeriodId == period 
                            && x.EmployeeId == employee
                            && x.IsSecondLevelApproved)
                                .ToList();

            // Check if there are any employee performances
            if ( !employeePerformances.Any())
            {
                return Json(new
                {
                    Message = "No employee performance records found for the given criteria.",
                    Status = "Not-OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // Calculate total performance weight, ensuring that GetPerformanceWeight is not null
            decimal weight = employeePerformances
                .Sum(performance => GetPerformanceWeight(performance.EmployeePerformanceId));

            var performanceEvaluation = db.PerformanceEvaluationSettings.FirstOrDefault(x => x.IsActive);
            // Default required weight = 100
            decimal requiredWeight = 100m;

            // If setting exists and has a valid TotalWeight, use it
            if (performanceEvaluation?.TotalWeight > 0)
            {
                requiredWeight = performanceEvaluation.TotalWeight.Value;
            }
            // Determine if validation should run
            bool shouldValidate = performanceEvaluation?.EnforceTotalWeightValidation ?? true;
            if (shouldValidate && weight != requiredWeight)
            {
                return Json(new
                {
                    Message = $"The total evaluation weight is {weight}. It must equal {requiredWeight}.",
                    Status = "Not-OK"
                }, JsonRequestBehavior.AllowGet);
            }
            // Approve all evaluations in memory and save them in a single batch
            foreach (var employeePerformance in employeePerformances)
            {
                employeePerformance.IsHRApproved = true;
                employeePerformance.FinalApprovalDate = DateTime.Now;
                employeePerformance.FinalApprover = User.Identity.Name;
                db.Entry(employeePerformance).State = EntityState.Modified;
            }
            db.SaveChanges(); // Save changes once after all modifications

            // Send notification if employee ID is not 0
            if (employee != 0)
            {
                var notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(employee),
                    Title = "Performance Approved by HR",
                    Message = "Your performance evaluation has been approved by HR.",
                    URL = "/HRM/EmployeePerformanceAllSummaries/"
                };

                Notify.NotifyUser(notif);
            }

            // Return success message
           
            return Json(new
            {
                Message = "All evaluations have been successfully approved..",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }

        public decimal GetPerformanceWeight(int empPerformanceId)
        {
            decimal totalweight = 0;

            var empMeasurs = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == empPerformanceId).ToList();

            foreach (var em in empMeasurs)
            {
                var scores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();

                if (scores.Any())
                {
                    totalweight += scores.Sum(x => x.Weight);
                }
            }

            return totalweight;
        }

        public decimal GetEvaluationWeightandResult(int id)
        {
            decimal result = 0;

               
            var empMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == id).ToList();
            foreach (var em in empMeasurments)
            {
                var weighAndResult =db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();
                if (weighAndResult.Any())
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
        public int GenerateEmployeeForPerofmanceEvaluation(int empId, int fiscalYearId, int periodId, int evaluationCriteriaId)
        {
            var empPerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == empId && x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.EvaluationCriteriaTypeId == evaluationCriteriaId);
            if (empPerformance == null)
            {
                var entity = new EmployeePerformance
                {
                    EmployeeId = empId,
                    GlFiscalYearId = fiscalYearId,
                    GlPeriodId = periodId,
                    EvaluationCriteriaTypeId = evaluationCriteriaId,
                    Weight = 0,
                    Result = 0
                };

                db.EmployeePerformances.Add(entity);
                db.SaveChanges();

                var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == entity.EvaluationCriteriaTypeId).ToList();

                foreach (var meas in measurments)
                {
                    var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == entity.EmployeePerformanceId);

                    if (checkExit == null)
                    {
                        var empMeasure = new EmployeeMeasurement();

                        empMeasure.MeasurmentId = meas.MeasurementId;
                        empMeasure.EmployeePerformanceId = entity.EmployeePerformanceId;
                        empMeasure.Weight = meas.MesPercentageofWeight;
                        empMeasure.Result = 0;

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
                                Score = 0,
                                Remark = "",
                            };

                            db.EmployeePerformanceScores.Add(performanceActionScore);
                            db.SaveChanges();
                        }
                    }
                }

                return entity.EmployeePerformanceId;
                //var returnvalue = new
                //{
                //    EmployeePerformanceId = entity.EmployeePerformanceId
                //};

                //return Json(returnvalue, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return empPerformance.EmployeePerformanceId;
                //var returnvalue1 = new
                //{
                //    EmployeePerformanceId = empPerformance.EmployeePerformanceId
                //};

                //return Json(returnvalue1, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult HRRejectionWithReason_Create(int yearId, int periodId, int employeeId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return Json(new { success = false, message = "Rejection reason is required." });
            }

            var employeePerformances = db.EmployeePerformances.Where(x =>
                x.GlFiscalYearId == yearId &&
                x.GlPeriodId == periodId &&
                x.EmployeeId == employeeId &&
                x.IsApproved &&
                x.IsImmediateSuppurvisorApproved &&
                x.IsSecondLevelApproved).ToList();

            if (!employeePerformances.Any())
            {
                return Json(new { success = false, message = "No matching performance records found for rejection." });
            }

            foreach (var performance in employeePerformances)
            {
                performance.IsSecondLevelApproved = false;
                performance.IsImmediateSuppurvisorApproved = false;
                performance.IsHRRejected = true;
                performance.IsHRApproved = false;
                performance.ReasonForHRRejection = reason;
                performance.FinalApprovalDate = DateTime.Now;
                performance.FinalApprover = User.Identity.Name;
                db.Entry(performance).State = EntityState.Modified;
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Performance evaluation successfully rejected." });
        }


        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee Performance Summary");
            #endregion
            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee Performance Summary");
            #endregion
            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

