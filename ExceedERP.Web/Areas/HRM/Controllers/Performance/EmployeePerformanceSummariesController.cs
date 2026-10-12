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
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,HrmConstants.HRDepartmentHead, 
                    HrmConstants.HRPerformanceUser, HrmConstants.HRPerformanceReport)]

    public class EmployeePerformanceSummariesController : BaseController
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
            ViewData["fiscalYearList"] = db.GLFiscalYears.AsQueryable();
            ViewData["Measurments"] = db.Measurements.AsQueryable();
            ViewData["PerformanceActions"] = db.PerformanceActions.AsQueryable();
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
               i => new
               {
                   FullNameAmharic = i.AmharicName,
                   FullNameaEnglish = i.Name,
                   EmployeeId = i.Id,
                   EmployeeCode = i.Code,
               }).AsQueryable();
            ViewData["Period"] = db.Quarters.Select(x=>new { x.QuarterId,x.Name}).ToList();
            var  measurments = db.MeasurementDefinitions.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + p.Lookup.Description + " - " + p.Role.ToString() + " " + p.DefnPercentageofWeight
            }).AsQueryable();
            ViewData["EvalutionCrateria"] = measurments;
            ViewData["branch"] = CacheData.GetBranchCaches().Select(
              i => new
              {
                  Name = i.Name,
                  BranchId = i.Id,
              }).AsQueryable(); ; ;

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
        public decimal GetEvaluationResult(int id)
        {
            // Get all measurements and scores in a single query
            return db.EmployeeMeasurement
                .Where(x => x.EmployeePerformanceId == id)
                .SelectMany(em => db.EmployeePerformanceScores
                    .Where(eps => eps.EmployeeMeasurementId == em.EmpMeasurementId)
                    .Select(eps => eps.Score))
                .DefaultIfEmpty(0)
                .Sum();
        }
        public decimal GetWeight(int id)
        {
            // Get all measurements and scores in a single query
            return  db.EmployeeMeasurement
                .Where(x => x.EmployeePerformanceId == id)
                .SelectMany(em => db.EmployeePerformanceScores
                    .Where(eps => eps.EmployeeMeasurementId == em.EmpMeasurementId)
                    .Select(eps => eps.Weight))
                .DefaultIfEmpty(0)
                .Sum();
        }
       
        private bool GetHRLevelApprovals(int empId, int fiscalyearId, int periodId)
        {
            var records = db.EmployeePerformances
                .Where(x => x.EmployeeId == empId 
                && x.GlFiscalYearId == fiscalyearId
                &&x.IsApproved
                &&!x.IsAveraged
                &&(x.IsAcceptedByEmployee 
                || x.IsRejectedByEmployee)
                && x.GlPeriodId == periodId);

            // Return true only if there are matching records and all are approved
            return records.Any() && records.All(x => x.IsImmediateSuppurvisorApproved);
        }

        //public ActionResult EmployeePerformanceSummaries_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //    var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);

        //    // Main query optimization
        //    var query = db.EmployeePerformances
        //        .Where(x => (x.IsAcceptedByEmployee || x.IsRejectedByEmployee))
        //        .Where(x => x.IsApproved && subordinates.Contains(x.EmployeeId) 
        //                && x.EmployeeId != id)
        //        .GroupBy(x => new { x.EmployeeId, x.GlFiscalYearId, x.GlPeriodId })
        //        .Select(g => g.OrderByDescending(x => x.DateCreated).FirstOrDefault())
        //        .OrderByDescending(x => x.GlFiscalYearId);

        //    // Projection optimization
        //    var result = query.ToDataSourceResult(request, employeePerformance => new
        //    {
        //        employeePerformance.EmployeePerformanceId,
        //        employeePerformance.EmployeeId,
        //        employeePerformance.GlFiscalYearId,
        //        employeePerformance.GlPeriodId,
        //        employeePerformance.OrgStructureId,
        //        employeePerformance.IsCurrent,
        //        employeePerformance.BranchId,
        //        employeePerformance.BusinessUnitId,
        //        employeePerformance.Date,
        //        employeePerformance.DateCreated,
        //        employeePerformance.CreatedBy,
        //        employeePerformance.ModifiedBy,
        //        employeePerformance.LastModified,
        //        employeePerformance.IsApproved,
        //        employeePerformance.IsRejectedByEmployee,
        //        employeePerformance.IsAcceptedByEmployee,
        //        IsImmediateSuppurvisorApproved = GetHRLevelApprovals(employeePerformance.EmployeeId, employeePerformance.GlFiscalYearId, employeePerformance.GlPeriodId),
        //        employeePerformance.IsSecondLevelApproved,
        //        employeePerformance.IsSecondLevelRejected,
        //        employeePerformance.ReasonFor2ndRejection,
        //        employeePerformance.ReasonForHRRejection,
        //        employeePerformance.ImmediateSuppurvisorApprovalDate,
        //        employeePerformance.SecondLevelApprovalDate,
        //        employeePerformance.FinalApprovalDate,
        //        employeePerformance.EvalutionType,
        //        employeePerformance.EmployeeRemark,
        //        //Weight = GetWeight(employeePerformance.EmployeePerformanceId),
        //        //Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
        //        Remark = employeePerformance.Remark
        //    });

        //    return Json(result);
        //}

        public ActionResult EmployeePerformanceSummaries_Read([DataSourceRequest] DataSourceRequest request)
        {

            db.Database.CommandTimeout = 120;
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);

            // Grouping query to aggregate and project distinct evaluation sets
            var query = db.EmployeePerformances
                .Where(x => (x.IsAcceptedByEmployee || x.IsRejectedByEmployee))
                .Where(x => x.IsApproved && subordinates.Contains(x.EmployeeId) && x.EmployeeId != id)
                .GroupBy(x => new { x.EmployeeId, x.GlFiscalYearId, x.GlPeriodId })
                .Select(g => new
                {
                    LatestRecord = g.OrderByDescending(x => x.DateCreated).FirstOrDefault(),

            // Item 607: Aggregate structured comments across all evaluations in the period (comma-separated)
            AggregatedSpecialAccomplishments = g.Where(x => !string.IsNullOrEmpty(x.SpecialAccomplishments))
                                                        .Select(x => x.SpecialAccomplishments),
                    AggregatedStrengths = g.Where(x => !string.IsNullOrEmpty(x.Strengths))
                                           .Select(x => x.Strengths),
                    AggregatedAreasNeedingImprovement = g.Where(x => !string.IsNullOrEmpty(x.AreasNeedingImprovement))
                                                        .Select(x => x.AreasNeedingImprovement),
                    AggregatedEmployeeRemarks = g.Where(x => !string.IsNullOrEmpty(x.EmployeeRemark))
                                                 .Select(x => x.EmployeeRemark)
                })
                .OrderByDescending(x => x.LatestRecord.GlFiscalYearId).ToList();

            // In-memory string formatting for comma-delimited strings compatible with EF / Kendo
            var result = query.ToDataSourceResult(request, x => new
            {
                x.LatestRecord.EmployeePerformanceId,
                x.LatestRecord.EmployeeId,
                x.LatestRecord.GlFiscalYearId,
                x.LatestRecord.GlPeriodId,
                x.LatestRecord.OrgStructureId,
                x.LatestRecord.IsCurrent,
                x.LatestRecord.BranchId,
                x.LatestRecord.BusinessUnitId,
                x.LatestRecord.Date,
                x.LatestRecord.DateCreated,
                x.LatestRecord.CreatedBy,
                x.LatestRecord.ModifiedBy,
                x.LatestRecord.LastModified,
                x.LatestRecord.IsApproved,
                x.LatestRecord.IsRejectedByEmployee,
                x.LatestRecord.IsAcceptedByEmployee,
                IsImmediateSuppurvisorApproved = GetHRLevelApprovals(x.LatestRecord.EmployeeId, x.LatestRecord.GlFiscalYearId, x.LatestRecord.GlPeriodId),
                x.LatestRecord.IsSecondLevelApproved,
                x.LatestRecord.IsSecondLevelRejected,
                x.LatestRecord.ReasonFor2ndRejection,
                x.LatestRecord.ReasonForHRRejection,
                x.LatestRecord.ImmediateSuppurvisorApprovalDate,
                x.LatestRecord.SecondLevelApprovalDate,
                x.LatestRecord.FinalApprovalDate,
                x.LatestRecord.EvalutionType,

                // Item 606 & 607 Fields (Comma Aggregated)
                SpecialAccomplishments = string.Join(", ", x.AggregatedSpecialAccomplishments.Distinct()),
                Strengths = string.Join(", ", x.AggregatedStrengths.Distinct()),
                AreasNeedingImprovement = string.Join(", ", x.AggregatedAreasNeedingImprovement.Distinct()),
                EmployeeRemark = string.Join(", ", x.AggregatedEmployeeRemarks.Distinct()),

                x.LatestRecord.InteractionCount,
                x.LatestRecord.IsEscalatedToHR,
                x.LatestRecord.EscalationDate,
                Remark = x.LatestRecord.Remark
            });

            return Json(result);
        }

        //public ActionResult EmployeePerformanceSummaries_OnePerson_Read_existed([DataSourceRequest] DataSourceRequest request, int employeeId, int fiscalyearId, int quarterId)
        //{
        //    var query = db.EmployeePerformances
        //        .Where(x => x.EmployeeId == employeeId
        //                 && x.GlFiscalYearId == fiscalyearId
        //                 && x.GlPeriodId == quarterId
        //                 && x.IsApproved
        //                 && (x.IsAcceptedByEmployee || x.IsRejectedByEmployee));

        //    var result = query.ToDataSourceResult(request, employeePerformance => new
        //    {
        //        employeePerformance.EmployeePerformanceId,
        //        employeePerformance.EmployeeId,
        //        employeePerformance.GlFiscalYearId,
        //        employeePerformance.GlPeriodId,
        //        employeePerformance.EvaluationCriteriaTypeId,
        //        employeePerformance.BusinessUnitId,
        //        employeePerformance.OrgStructureId,
        //        employeePerformance.ResponsibilityLevel,
        //        employeePerformance.BranchId,
        //        employeePerformance.JobTitleId,
        //        employeePerformance.IsCurrent,
        //        employeePerformance.IsHRLevelEvalution,
        //        employeePerformance.EvalutionType,
        //        employeePerformance.Date,
        //        employeePerformance.DateCreated,
        //        employeePerformance.CreatedBy,
        //        employeePerformance.ModifiedBy,
        //        employeePerformance.LastModified,
        //        employeePerformance.ImmediateSuppurvisorApprovalDate,
        //        employeePerformance.SecondLevelApprovalDate,
        //        employeePerformance.FinalApprovalDate,

        //        Result = db.EmployeeMeasurement
        //                   .Where(m => m.EmployeePerformanceId == employeePerformance.EmployeePerformanceId)
        //                   .SelectMany(m => db.EmployeePerformanceScores
        //                       .Where(s => s.EmployeeMeasurementId == m.EmpMeasurementId)
        //                       .Select(s => s.Score))
        //                   .DefaultIfEmpty(0)
        //                   .Sum(),

        //        Weight = db.EmployeeMeasurement
        //                   .Where(m => m.EmployeePerformanceId == employeePerformance.EmployeePerformanceId)
        //                   .SelectMany(m => db.EmployeePerformanceScores
        //                       .Where(s => s.EmployeeMeasurementId == m.EmpMeasurementId)
        //                       .Select(s => s.Weight))
        //                   .DefaultIfEmpty(0)
        //                   .Sum(),

        //        employeePerformance.IsApproved,
        //        employeePerformance.IsRejectedByEmployee,
        //        employeePerformance.IsAcceptedByEmployee,
        //        employeePerformance.IsImmediateSuppurvisorApproved,
        //        employeePerformance.IsSecondLevelApproved,
        //        employeePerformance.IsSecondLevelRejected,
        //        employeePerformance.ReasonFor2ndRejection,

        //        // Item 606 & 607 Direct Model Mappings
        //        employeePerformance.SpecialAccomplishments,
        //        employeePerformance.Strengths,
        //        employeePerformance.AreasNeedingImprovement,
        //        employeePerformance.EmployeeRemark,
        //        employeePerformance.InteractionCount,
        //        employeePerformance.IsEscalatedToHR,
        //        employeePerformance.EscalationDate,
        //        Remark = employeePerformance.Remark
        //    });

        //    return Json(result);
        //}
        //public ActionResult EmployeePerformanceSummaries_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //   // var subordinates = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(id);
        //    var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);

        //    var employeePerformances = db.EmployeePerformances
        //                        .Where(x=>x.IsAcceptedByEmployee||x.IsRejectedByEmployee)
        //                        .Where(x => x.IsApproved 
        //                        && subordinates.Contains(x.EmployeeId) 
        //                        &&x.EmployeeId!=id).DistinctBy(x => new
        //                        {
        //                            x.EmployeeId,
        //                            x.GlFiscalYearId,
        //                            x.GlPeriodId
        //                        }).OrderByDescending(x => x.GlFiscalYearId).ToList();
        //    DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
        //    {
        //        EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
        //        EmployeeId = employeePerformance.EmployeeId,
        //        GlFiscalYearId = employeePerformance.GlFiscalYearId,
        //        GlPeriodId = employeePerformance.GlPeriodId,
        //        OrgStructureId = employeePerformance.OrgStructureId,
        //        IsCurrent = employeePerformance.IsCurrent,
        //        BranchId = employeePerformance.BranchId,
        //        BusinessUnitId = employeePerformance.BusinessUnitId,
        //        Date = employeePerformance.Date,
        //        DateCreated = employeePerformance.DateCreated,
        //        CreatedBy = employeePerformance.CreatedBy,
        //        ModifiedBy = employeePerformance.ModifiedBy,
        //        LastModified = employeePerformance.LastModified,
        //        IsApproved = employeePerformance.IsApproved,
        //        IsRejectedByEmployee = employeePerformance.IsRejectedByEmployee,
        //        IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
        //        IsImmediateSuppurvisorApproved = GetHRLevelApprovals(employeePerformance.EmployeeId, employeePerformance.GlFiscalYearId, employeePerformance.GlPeriodId),
        //        IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
        //        IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
        //        ReasonFor2ndRejection = employeePerformance.ReasonFor2ndRejection,
        //        ImmediateSuppurvisorApprovalDate = employeePerformance.ImmediateSuppurvisorApprovalDate,
        //        SecondLevelApprovalDate = employeePerformance.SecondLevelApprovalDate,
        //        FinalApprovalDate = employeePerformance.FinalApprovalDate,
        //        EvalutionType = employeePerformance.EvalutionType,
        //        //IsImmediateSuppurvisorApproved = employeePerformance.IsImmediateSuppurvisorApproved,
        //        Weight = GetWeight(employeePerformance.EmployeePerformanceId),
        //        Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
        //        //AreAllHRLevelApproved = ,
        //        Remark = employeePerformance.EmployeeRemark
        //    });

        //    return Json(result);
        //}
        //public ActionResult EmployeePerformanceSummaries_OnePerson_Read([DataSourceRequest] DataSourceRequest request,int employeeId,int fiscalyearId, int quarterId)
        //{
        //    //List<int> employees = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
        //    var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //    //var subordinates = EmployeeHelper.GetSubordinates(id);
        //    var employeePerformances = db.EmployeePerformances
        //                                            .Where(x => x.EmployeeId == employeeId 
        //                                            && x.GlFiscalYearId == fiscalyearId 
        //                                            && x.GlPeriodId == quarterId 
        //                                            && x.IsApproved 
        //                                            && (x.IsAcceptedByEmployee || x.IsRejectedByEmployee)
        //                                          ).ToList();

        //    //var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId==employeeId
        //    //                                                            &&x.GlFiscalYearId==fiscalyearId
        //    //                                                            &&x.GlPeriodId==quarterId
        //    //                                                            &&(x.IsRejectedByEmployee||x.IsRejectedByEmployee)
        //    //                                                            &&x.IsApproved).ToList();     
        //    //IQueryable<EmployeePerformance> employeePerformances = db.EmployeePerformances;
        //    DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
        //    {
        //        EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
        //        EmployeeId = employeePerformance.EmployeeId,
        //        GlFiscalYearId = employeePerformance.GlFiscalYearId,
        //        GlPeriodId = employeePerformance.GlPeriodId,
        //        EvaluationCriteriaTypeId = employeePerformance.EvaluationCriteriaTypeId,
        //        BusinessUnitId = employeePerformance.BusinessUnitId,
        //        OrgStructureId = employeePerformance.OrgStructureId,
        //        ResponsibilityLevel = employeePerformance.ResponsibilityLevel,
        //        BranchId = employeePerformance.BranchId,
        //        JobTitleId = employeePerformance.JobTitleId,
        //        IsCurrent = employeePerformance.IsCurrent,
        //        IsHRLevelEvalution = employeePerformance.IsHRLevelEvalution,
        //        // Result = employeePerformance.Result,
        //        //Weight = employeePerformance.Weight,
        //        EvalutionType = employeePerformance.EvalutionType,
        //        Date = employeePerformance.Date,
        //        DateCreated = employeePerformance.DateCreated,
        //        ImmediateSuppurvisorApprovalDate = employeePerformance.ImmediateSuppurvisorApprovalDate,
        //        SecondLevelApprovalDate = employeePerformance.SecondLevelApprovalDate,
        //        FinalApprovalDate = employeePerformance.FinalApprovalDate,
        //        Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
        //        Weight = GetWeight(employeePerformance.EmployeePerformanceId),              
        //        IsApproved = employeePerformance.IsApproved,
        //        IsRejectedByEmployee = employeePerformance.IsRejectedByEmployee,
        //        IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
        //        IsImmediateSuppurvisorApproved = employeePerformance.IsImmediateSuppurvisorApproved,
        //        IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
        //        IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
        //        ReasonFor2ndRejection = employeePerformance.ReasonFor2ndRejection,

        //        Remark = employeePerformance.EmployeeRemark
        //    });

        //    return Json(result);
        //}
        //public ActionResult EmployeePerformanceSummaries_OnePerson_Read([DataSourceRequest] DataSourceRequest request, int employeeId, int fiscalyearId, int quarterId)
        //{

        //    var employeePerformances = db.EmployeePerformances
        //        .Where(x => x.EmployeeId == employeeId
        //                 && x.GlFiscalYearId == fiscalyearId
        //                 && x.GlPeriodId == quarterId
        //                 && x.IsApproved
        //                 && (x.IsAcceptedByEmployee || x.IsRejectedByEmployee)).ToList();
        //    DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
        //    {
        //        EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
        //        EmployeeId = employeePerformance.EmployeeId,
        //        DirectBossId = employeePerformance.DirectBossId,
        //        GlFiscalYearId = employeePerformance.GlFiscalYearId,
        //        GlPeriodId = employeePerformance.GlPeriodId,
        //        OrgStructureId = employeePerformance.OrgStructureId,
        //        BranchId = employeePerformance.BranchId,
        //        BusinessUnitId = employeePerformance.BusinessUnitId,
        //        IsApproved = employeePerformance.IsApproved,
        //        IsRejectedByEmployee = employeePerformance.IsRejectedByEmployee,
        //        IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
        //        IsCurrent = employeePerformance.IsCurrent,
        //        EvaluationCriteriaTypeId = employeePerformance.EvaluationCriteriaTypeId,
        //        ReasonFor2ndRejection = employeePerformance.ReasonFor2ndRejection,
        //        IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
        //        EvalutionType = employeePerformance.EvalutionType,
        //        Date = employeePerformance.Date,
        //        DateCreated = employeePerformance.DateCreated,
        //        ImmediateSuppurvisorApprovalDate = employeePerformance.ImmediateSuppurvisorApprovalDate,
        //        SecondLevelApprovalDate = employeePerformance.SecondLevelApprovalDate,
        //        FinalApprovalDate = employeePerformance.FinalApprovalDate,
        //        Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
        //        Weight = GetWeight(employeePerformance.EmployeePerformanceId),
        //        Remark = employeePerformance.Remark
        //    });

        //    return Json(result);
        //}
        public ActionResult EmployeePerformanceSummaries_OnePerson_Read([DataSourceRequest] DataSourceRequest request, int employeeId, int fiscalyearId, int quarterId)
        {
            var query = db.EmployeePerformances
                .Where(x => x.EmployeeId == employeeId
                         && x.GlFiscalYearId == fiscalyearId
                         && x.GlPeriodId == quarterId
                         && x.IsApproved
                         && (x.IsAcceptedByEmployee || x.IsRejectedByEmployee))
                .Select(ep => new
                {
                    ep.EmployeePerformanceId,
                    ep.EmployeeId,
                    ep.DirectBossId,
                    ep.GlFiscalYearId,
                    ep.GlPeriodId,
                    ep.OrgStructureId,
                    ep.BranchId,
                    ep.BusinessUnitId,
                    ep.IsApproved,
                    ep.IsRejectedByEmployee,
                    ep.IsAcceptedByEmployee,
                    ep.IsCurrent,
                    ep.EvaluationCriteriaTypeId,
                    ep.ReasonFor2ndRejection,
                    ep.ReasonForHRRejection,
                    ep.IsSecondLevelRejected,
                    ep.EvalutionType,
                    ep.Date,
                    ep.DateCreated,
                    ep.ImmediateSuppurvisorApprovalDate,
                    ep.SecondLevelApprovalDate,
                    ep.FinalApprovalDate,
                    ep.Remark,
                    ep.EmployeeRemark,
                    ep.EscalationDate,
                    ep.AreasNeedingImprovement,
                    ep.SpecialAccomplishments,
                    ep.Strengths,
                    // compute aggregates inline
                    Result = db.EmployeeMeasurement
                                       .Where(m => m.EmployeePerformanceId == ep.EmployeePerformanceId)
                                       .SelectMany(m => db.EmployeePerformanceScores
                                           .Where(s => s.EmployeeMeasurementId == m.EmpMeasurementId)
                                           .Select(s => (decimal?)s.Score))
                                       .DefaultIfEmpty(0)
                                       .Sum() ?? 0,

                    Weight = db.EmployeeMeasurement
                               .Where(m => m.EmployeePerformanceId == ep.EmployeePerformanceId)
                               .SelectMany(m => db.EmployeePerformanceScores
                                   .Where(s => s.EmployeeMeasurementId == m.EmpMeasurementId)
                                   .Select(s => (decimal?)s.Weight))
                               .DefaultIfEmpty(0)
                               .Sum() ?? 0
                });

            var result = query.ToDataSourceResult(request);
            return Json(result);
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
        private void AveragingEvaluationFinal(IList<EmployeePerformance> employeePerformances)
        {
            if (employeePerformances == null || !employeePerformances.Any())
                return;

            var criteriaIds = employeePerformances.Select(x => x.EvaluationCriteriaTypeId).Distinct().ToList();
            var employeeId = employeePerformances.First().EmployeeId;

            // Step 1: Delete previous averaged records
            var averagedPerformances = db.EmployeePerformances
                .Where(x => x.EmployeeId == employeeId &&
                            criteriaIds.Contains(x.EvaluationCriteriaTypeId) &&
                           (x.Remark == "Averaged Evaluation" ||
                            x.Comment == "Averaged Evaluation" ||
                            x.EmployeeRemark == "Averaged Evaluation")).ToList();

            if (averagedPerformances.Any())
            {
                db.EmployeePerformances.RemoveRange(averagedPerformances);
                db.SaveChanges();
            }

            // Step 2: Related Measurements and Scores
            var employeePerformanceIds = employeePerformances.Select(x => x.EmployeePerformanceId).ToList();
            var employeeMeasurements = db.EmployeeMeasurement
                                          .Where(x => employeePerformanceIds.Contains(x.EmployeePerformanceId))
                                          .ToList();

            var employeeMeasurementIds = employeeMeasurements.Select(x => x.EmpMeasurementId).ToList();
            var employeePerformanceScores = db.EmployeePerformanceScores
                                               .Where(x => employeeMeasurementIds.Contains(x.EmployeeMeasurementId))
                                               .ToList();

            // Step 3: Average Scores
            var avgScores = employeePerformanceScores
                .GroupBy(x => x.PerformanceActionId)
                .ToDictionary(g => g.Key, g => g.Average(x => x.Score));

            var avgMeasurements = employeeMeasurements
                .GroupBy(x => x.MeasurmentId)
                .Select(g => new
                {
                    MeasurmentId = g.Key,
                    AvgResult = g.Average(x => x.Result),
                    AvgWeight = g.Average(x => x.Weight)
                }).ToList();

            // Step 4: Create new averaged EmployeePerformance record
            var basePerf = employeePerformances.First();
            var entity = new EmployeePerformance
            {
                EmployeeId = basePerf.EmployeeId,
                GlFiscalYearId = basePerf.GlFiscalYearId,
                GlPeriodId = basePerf.GlPeriodId,
                EvaluationCriteriaTypeId = basePerf.EvaluationCriteriaTypeId,
                BusinessUnitId = basePerf.BusinessUnitId,
                OrgStructureId = basePerf.OrgStructureId,
                BranchId = basePerf.BranchId,
                JobTitleId = basePerf.JobTitleId,
                IsCurrent = basePerf.IsCurrent,
                CriteriaCategoryId = basePerf.CriteriaCategoryId,
                DirectBossId = basePerf.DirectBossId,
                ResponsibilityLevel = basePerf.ResponsibilityLevel,
                Date = DateTime.Now,
                DateCreated = DateTime.Now,
                LastModified = DateTime.Now,
                CreatedBy = User.Identity.Name,
                ModifiedBy = User.Identity.Name,
                Comment = "Averaged Evaluation",
                Remark = "Averaged Evaluation",
                EmployeeRemark = "Averaged Evaluation",
                DeadLine = 0,
                IsAcceptedByEmployee = true,
                IsApproved = true,
                IsSecondLevelApproved = false,
                IsImmediateSuppurvisorApproved = true,
                ImmediateSuppurvisorApprovalDate = DateTime.Now,
                ImmediateSuppurvisorApprover = User.Identity.Name,
                IsSectorApplied = false,
                IsCorporateApplied = false,
                IsHRApproved = false,
                IsHRLevelEvalution = false,
                IsRejectedByEmployee = false,
                IsSecondLevelRejected = false,
                IsAveraged = true,
                Grade = null,
                ReasonFor2ndRejection = null,
                Weight = avgMeasurements.Average(x => x.AvgWeight),
                Result = avgMeasurements.Average(x => x.AvgResult)
            };

            db.EmployeePerformances.Add(entity);
            db.SaveChanges();

            // Step 5: Add EmployeeMeasurement & Scores
            var measurements = db.Measurements
                .Where(x => x.MeasurementDefinitionId == entity.EvaluationCriteriaTypeId)
                .ToList();

            var newEmpMeasurements = new List<EmployeeMeasurement>();
            var newPerformanceScores = new List<EmployeePerformanceScore>();

            foreach (var meas in measurements)
            {
                var empMeasure = new EmployeeMeasurement
                {
                    MeasurmentId = meas.MeasurementId,
                    EmployeePerformanceId = entity.EmployeePerformanceId,
                    Weight = meas.MesPercentageofWeight,
                    Result = 0
                };

                newEmpMeasurements.Add(empMeasure);
            }

            db.EmployeeMeasurement.AddRange(newEmpMeasurements);
            db.SaveChanges();

            foreach (var empMeasure in newEmpMeasurements)
            {
                var relatedActions = db.PerformanceActions
                    .Where(x => x.MeasurementId == empMeasure.MeasurmentId)
                    .ToList();

                foreach (var action in relatedActions)
                {
                    avgScores.TryGetValue(action.ActionId, out var avgScore);

                    var score = new EmployeePerformanceScore
                    {
                        PerformanceActionId = action.ActionId,
                        EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                        Weight = action.AcPercentageofWeight,
                        Score = avgScore,
                        Remark = ""
                    };

                    newPerformanceScores.Add(score);
                }
            }

            db.EmployeePerformanceScores.AddRange(newPerformanceScores);
            db.SaveChanges();

            // Step 6: Mark original performances as averaged
            foreach (var performance in employeePerformances)
            {
                performance.IsAveraged = true;
                db.Entry(performance).State = EntityState.Modified;
            }

            db.SaveChanges();
        }

        private void AveragingEvaluationCopyyy(IList<EmployeePerformance> employeePerformances)
        {
            if (employeePerformances == null || !employeePerformances.Any())
                return; // Exit if the list is empty
            // remove if there is avarage value before 
            var criteriaIds = employeePerformances.Select(x => x.EvaluationCriteriaTypeId).Distinct();
            var employeeId = employeePerformances.FirstOrDefault()?.EmployeeId;
            var averagedPerformances = db.EmployeePerformances
                               .Where(x => criteriaIds.Contains(x.EvaluationCriteriaTypeId)&&x.EmployeeId==employeeId &&
                                          (x.Remark == "Averaged Evaluation" ||
                                           x.Comment == "Averaged Evaluation" ||
                                           x.EmployeeRemark == "Averaged Evaluation"))
                               .ToList();

            if (averagedPerformances.Any())
            {
                db.EmployeePerformances.RemoveRange(averagedPerformances);
                db.SaveChanges();
            }
            // Step 1: Get all related measurements
            var employeePerformanceIds = employeePerformances
                                         .Select(x => x.EmployeePerformanceId).ToList();
            var employeeMeasurements = db.EmployeeMeasurement
                                        .Where(x => employeePerformanceIds.Contains(x.EmployeePerformanceId))
                                        .ToList();

            // Step 2: Get all related performance scores
            var employeeMeasurementIds = employeeMeasurements.Select(x => x.EmpMeasurementId).ToList();
            var employeePerformanceScores = db.EmployeePerformanceScores
                                             .Where(x => employeeMeasurementIds.Contains(x.EmployeeMeasurementId))
                                             .ToList();

            // Step 3: Average Scores at EmployeePerformanceScores Level
            var avgScores = employeePerformanceScores
                .GroupBy(x => x.PerformanceActionId)
                .Select(g => new
                {
                    PerformanceActionId = g.Key,
                    AvgScore = g.Average(x => x.Score)
                }).ToList();

            // Step 4: Average Results at EmployeeMeasurement Level
            var avgMeasurements = employeeMeasurements
                .GroupBy(x => x.MeasurmentId)
                .Select(g => new
                {
                    MeasurmentId = g.Key,
                    AvgResult = g.Average(x => x.Result),
                    AvgWeight = g.Average(x => x.Weight)
                }).ToList();

            // Step 5: Create a new EmployeePerformance record (Averaged Data)
            var entity = new EmployeePerformance
            {
                EmployeeId = employeePerformances.First().EmployeeId,
                GlFiscalYearId = employeePerformances.First().GlFiscalYearId,
                GlPeriodId = employeePerformances.First().GlPeriodId,
                EvaluationCriteriaTypeId = employeePerformances.First().EvaluationCriteriaTypeId,
                BusinessUnitId = employeePerformances.First().BusinessUnitId,
                OrgStructureId = employeePerformances.First().OrgStructureId,
                BranchId = employeePerformances.First().BranchId,
                JobTitleId = employeePerformances.First().JobTitleId,
                IsCurrent = employeePerformances.First().IsCurrent,
                CriteriaCategoryId = employeePerformances.First().CriteriaCategoryId,
                DirectBossId = employeePerformances.First().DirectBossId,
                ResponsibilityLevel = employeePerformances.First().ResponsibilityLevel,
                Date = DateTime.Now,
                DateCreated = DateTime.Now,
                LastModified = DateTime.Now,
                CreatedBy = User.Identity.Name,
                ModifiedBy = User.Identity.Name,
                Comment = "Averaged Evaluation",
                Remark = "Averaged Evaluation",
                EmployeeRemark = "Averaged Evaluation",
                DeadLine = 0,
                IsAcceptedByEmployee = true,
                IsApproved = true,
                IsSecondLevelApproved = false,
                IsImmediateSuppurvisorApproved = true, // Set to true
                ImmediateSuppurvisorApprovalDate = DateTime.Now,
                ImmediateSuppurvisorApprover = User.Identity.Name,
                IsSectorApplied = false,
                IsCorporateApplied = false,
                IsHRApproved = false,
                IsHRLevelEvalution = false,
                IsRejectedByEmployee = false,
                IsSecondLevelRejected = false,
                IsAveraged = true,
                Grade = null,
                ReasonFor2ndRejection = null,
                Weight = avgMeasurements.Average(x => x.AvgWeight),
                Result = avgMeasurements.Average(x => x.AvgResult)
            };

            db.EmployeePerformances.Add(entity);
            db.SaveChanges(); // Save and get the ID
            var measurements = db.Measurements.Where(x => x.MeasurementDefinitionId == entity.EvaluationCriteriaTypeId).ToList();

            foreach (var meas in measurements)
            {

                var empMeasure = new EmployeeMeasurement();
                empMeasure.MeasurmentId = meas.MeasurementId;
                empMeasure.EmployeePerformanceId = entity.EmployeePerformanceId;
                empMeasure.Weight = meas.MesPercentageofWeight;
                empMeasure.Result = 0;
                db.EmployeeMeasurement.Add(empMeasure);
                db.SaveChanges();
                var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId == meas.MeasurementId).ToList();

                foreach (var per in performanceActions)
                {
                    var PerformanceScoresResults = employeePerformanceScores.Where(x => x.PerformanceActionId == per.ActionId).ToList();

                    var performanceActionScore = new EmployeePerformanceScore
                    {
                        PerformanceActionId = per.ActionId,
                        EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                        Weight = per.AcPercentageofWeight,
                        Score = PerformanceScoresResults.Any()? PerformanceScoresResults.Average(x=>x.Score):0,
                        Remark = "",
                    };

                    db.EmployeePerformanceScores.Add(performanceActionScore);
                    db.SaveChanges();
                }
                

            }

           
            foreach(var performance in employeePerformances)
            {
                performance.IsAveraged = true;
                db.Entry(performance).State = EntityState.Modified ;
            }
            db.SaveChanges(); // Final save
        }
        private void AveragingEvaluationCopy(IList<EmployeePerformance> employeePerformances)
        {
            if (employeePerformances == null || !employeePerformances.Any())
                return; // Exit if the list is empty

            // Step 1: Get all related measurements
            var employeePerformanceIds = employeePerformances
                                         .Select(x => x.EmployeePerformanceId).ToList();
            var employeeMeasurements = db.EmployeeMeasurement
                                        .Where(x => employeePerformanceIds.Contains(x.EmployeePerformanceId))
                                        .ToList();

            // Step 2: Get all related performance scores
            var employeeMeasurementIds = employeeMeasurements.Select(x => x.EmpMeasurementId).ToList();
            var employeePerformanceScores = db.EmployeePerformanceScores
                                             .Where(x => employeeMeasurementIds.Contains(x.EmployeeMeasurementId))
                                             .ToList();

            // Step 3: Average Scores at EmployeePerformanceScores Level
            var avgScores = employeePerformanceScores
                .GroupBy(x => x.PerformanceActionId)
                .Select(g => new
                {
                    PerformanceActionId = g.Key,
                    AvgScore = g.Average(x => x.Score)
                }).ToList();

            // Step 4: Average Results at EmployeeMeasurement Level
            var avgMeasurements = employeeMeasurements
                .GroupBy(x => x.MeasurmentId)
                .Select(g => new
                {
                    MeasurmentId = g.Key,
                    AvgResult = g.Average(x => x.Result),
                    AvgWeight = g.Average(x => x.Weight)
                }).ToList();

            // Step 5: Create a new EmployeePerformance record (Averaged Data)
            var entity = new EmployeePerformance
            {
                EmployeeId = employeePerformances.First().EmployeeId,
                GlFiscalYearId = employeePerformances.First().GlFiscalYearId,
                GlPeriodId = employeePerformances.First().GlPeriodId,
                EvaluationCriteriaTypeId = employeePerformances.First().EvaluationCriteriaTypeId,
                BusinessUnitId = employeePerformances.First().BusinessUnitId,
                OrgStructureId = employeePerformances.First().OrgStructureId,
                BranchId = employeePerformances.First().BranchId,
                JobTitleId = employeePerformances.First().JobTitleId,
                IsCurrent = employeePerformances.First().IsCurrent,
                CriteriaCategoryId = employeePerformances.First().CriteriaCategoryId,
                DirectBossId = employeePerformances.First().DirectBossId,
                ResponsibilityLevel = employeePerformances.First().ResponsibilityLevel,
                Date = DateTime.Now,
                DateCreated = DateTime.Now,
                LastModified = DateTime.Now,
                CreatedBy = User.Identity.Name,
                ModifiedBy = User.Identity.Name,
                Comment = "Averaged Evaluation",
                Remark = "Averaged Evaluation",
                EmployeeRemark = "Averaged Evaluation",
                DeadLine = 0,
                IsAcceptedByEmployee = true,
                IsApproved = true,
                IsSecondLevelApproved = false,
                IsImmediateSuppurvisorApproved = true, // Set to true
                ImmediateSuppurvisorApprovalDate = DateTime.Now,
                ImmediateSuppurvisorApprover = User.Identity.Name,
                IsSectorApplied = false,
                IsCorporateApplied = false,
                IsHRApproved = false,
                IsHRLevelEvalution = false,
                IsRejectedByEmployee = false,
                IsSecondLevelRejected = false,
                IsAveraged = true,
                Grade = null,
                ReasonFor2ndRejection = null,
                Weight = avgMeasurements.Average(x => x.AvgWeight),
                Result = avgMeasurements.Average(x => x.AvgResult)
            };

            db.EmployeePerformances.Add(entity);
            db.SaveChanges(); // Save and get the ID
            // Step 6: Save the Averaged EmployeeMeasurement Records
            foreach (var meas in avgMeasurements)
            {
                var empMeasure = new EmployeeMeasurement
                {
                    MeasurmentId = meas.MeasurmentId,
                    EmployeePerformanceId = entity.EmployeePerformanceId,
                    Weight = meas.AvgWeight,
                    Result = meas.AvgResult
                };

                db.EmployeeMeasurement.Add(empMeasure);
                db.SaveChanges();

                // Step 7: Save the Averaged EmployeePerformanceScores
                foreach (var avgScore in avgScores.Where(x => x.PerformanceActionId == meas.MeasurmentId))
                {
                    var performanceActionScore = new EmployeePerformanceScore
                    {
                        PerformanceActionId = avgScore.PerformanceActionId,
                        EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                        Weight = meas.AvgWeight,
                        Score = avgScore.AvgScore,
                        Remark = "Averaged Score"
                    };

                    db.EmployeePerformanceScores.Add(performanceActionScore);
                }
            }
            db.SaveChanges(); // Final save
        }
        private void AveragingEvaluation(IList<EmployeePerformance> employeePerformances)
        {
            if (employeePerformances == null || !employeePerformances.Any())
                return; // Exit if the list is empty

            // Step 1: Get all related measurements
            var employeePerformanceIds = employeePerformances.Select(x => x.EmployeePerformanceId).ToList();
            var employeeMeasurements = db.EmployeeMeasurement
                                        .Where(x => employeePerformanceIds.Contains(x.EmployeePerformanceId))
                                        .ToList();

            // Step 2: Get all related performance scores
            var employeeMeasurementIds = employeeMeasurements.Select(x => x.EmpMeasurementId).ToList();
            var employeePerformanceScores = db.EmployeePerformanceScores
                                             .Where(x => employeeMeasurementIds.Contains(x.EmployeeMeasurementId))
                                             .ToList();

            // Step 3: Average Scores at EmployeePerformanceScores Level
            var avgScores = employeePerformanceScores
                .GroupBy(x => 1)
                .Select(g => new
                {
                    PerformanceActionId = g.First().PerformanceActionId,
                    AvgScore = g.Average(x => x.Score),
                    AvgWeight = g.Average(x => x.Weight)
                }).FirstOrDefault();

            // Step 4: Average Results at EmployeeMeasurement Level
            var avgMeasurements = employeeMeasurements
                .GroupBy(x => 1)
                .Select(g => new
                {
                    MeasurmentId = g.First().MeasurmentId,
                    AvgResult = g.Average(x => x.Result),
                    AvgWeight = g.Average(x => x.Weight)
                }).FirstOrDefault();

            // Step 5: Create a new EmployeePerformance record (Averaged Data)
            var entity = new EmployeePerformance
            {
                EmployeeId = employeePerformances.First().EmployeeId,
                GlFiscalYearId = employeePerformances.First().GlFiscalYearId,
                GlPeriodId = employeePerformances.First().GlPeriodId,
                EvaluationCriteriaTypeId = employeePerformances.First().EvaluationCriteriaTypeId,
                BusinessUnitId = employeePerformances.First().BusinessUnitId,
                OrgStructureId = employeePerformances.First().OrgStructureId,
                BranchId = employeePerformances.First().BranchId,
                JobTitleId = employeePerformances.First().JobTitleId,
                IsCurrent = employeePerformances.First().IsCurrent,
                CriteriaCategoryId = employeePerformances.First().CriteriaCategoryId,
                DirectBossId = employeePerformances.First().DirectBossId,
                ResponsibilityLevel = employeePerformances.First().ResponsibilityLevel,
                Date = DateTime.Now,
                DateCreated = DateTime.Now,
                LastModified = DateTime.Now,
                CreatedBy = User.Identity.Name,
                ModifiedBy = User.Identity.Name,
                Comment = "Averaged Evaluation",
                Remark = "Averaged Evaluation",
                EmployeeRemark = "Averaged Evaluation",
                DeadLine = 0,
                IsAcceptedByEmployee = true,
                IsApproved = true,
                IsSecondLevelApproved = false,
                IsImmediateSuppurvisorApproved = true,
                ImmediateSuppurvisorApprovalDate = DateTime.Now,
                ImmediateSuppurvisorApprover = User.Identity.Name,
                IsSectorApplied = false,
                IsCorporateApplied = false,
                IsHRApproved = false,
                IsHRLevelEvalution = false,
                IsRejectedByEmployee = false,
                IsSecondLevelRejected = false,
                IsAveraged = true,
                Grade = null,
                ReasonFor2ndRejection = null,
                Weight = avgMeasurements?.AvgWeight ?? 0, // Use null-coalescing operator to avoid null reference
                Result = avgMeasurements?.AvgResult ?? 0
            };

            db.EmployeePerformances.Add(entity);
            db.SaveChanges(); // Save and get the ID

            // Step 6: Save the Averaged EmployeeMeasurement Record
            var empMeasure = new EmployeeMeasurement
            {
                MeasurmentId = avgMeasurements?.MeasurmentId ?? 0, // Use default value if null
                EmployeePerformanceId = entity.EmployeePerformanceId,
                Weight = avgScores?.AvgWeight ?? 0,
                Result = avgScores?.AvgScore ?? 0
            };

            db.EmployeeMeasurement.Add(empMeasure);
            db.SaveChanges();

            // Step 7: Save the Averaged EmployeePerformanceScores
            var performanceActionScore = new EmployeePerformanceScore
            {
                PerformanceActionId = avgScores?.PerformanceActionId ?? 0, // Use default value if null
                EmployeeMeasurementId = empMeasure.EmpMeasurementId,
                Weight = avgScores?.AvgWeight ?? 0,
                Score = avgScores?.AvgScore ?? 0,
                Remark = "Averaged Score"
            };

            db.EmployeePerformanceScores.Add(performanceActionScore);
            db.SaveChanges(); // Final save
        }
        [HttpPost]
        public JsonResult ApproveEvaluation(int year,int period,int employee,
    string remark,
    string specialAccomplishments,
    string strengths,
    string areasNeedingImprovement)
        {
            // Validate that the employee has completed their sign-off step
            bool hasUnresolvedEmployeeAction = db.EmployeePerformances.Any(x =>
                x.GlFiscalYearId == year &&
                x.GlPeriodId == period &&
                x.EmployeeId == employee &&
                x.IsApproved &&
                !x.IsAcceptedByEmployee &&
                !x.IsRejectedByEmployee);

            if (hasUnresolvedEmployeeAction)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Cannot approve: The employee must first accept or reject their evaluation."
                });
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Handle multi-evaluator averaging if configured
                    var settings = db.EmployeePeerToPeerAndInternalCustomerPerformanceSettings.FirstOrDefault();
                    if (settings != null && (settings.IsInternalCustomerHasMultipleEvaluator || settings.IsPeerToPeerHasMultipleEvaluator))
                    {
                        var multiEvaluatorItems = db.EmployeePerformances
                            .Where(x => x.GlFiscalYearId == year
                                     && x.GlPeriodId == period
                                     && x.EmployeeId == employee
                                     && x.IsApproved
                                     && !x.IsAveraged
                                     && !x.IsImmediateSuppurvisorApproved
                                     && (x.EvalutionType == "PeerToPeer" || x.EvalutionType == "InternalCustomer" || x.EmployeeRemark == "Averaged Evaluation"))
                            .ToList();

                        var groupedCriteria = multiEvaluatorItems
                            .GroupBy(x => new { x.EvalutionType, x.EvaluationCriteriaTypeId })
                            .Where(g => g.Count() > 1);

                        foreach (var group in groupedCriteria)
                        {
                            AveragingEvaluationFinal(group.ToList());
                        }
                    }

                    // Retrieve all pending records for Level 1 approval
                    var pendingEvaluations = db.EmployeePerformances
                        .Where(x => x.GlFiscalYearId == year
                                 && x.GlPeriodId == period
                                 && x.EmployeeId == employee
                                 && x.IsApproved
                                 && !x.IsAveraged
                                 && !x.IsImmediateSuppurvisorApproved)
                        .ToList();

                    if (!pendingEvaluations.Any())
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "No eligible records found pending Level 1 approval."
                        });
                    }

                    var approvalDate = DateTime.Now;
                    var approverName = User?.Identity?.Name ?? "Supervisor";

                    foreach (var evaluation in pendingEvaluations)
                    {
                        // Level 1 Supervisor Approval
                        evaluation.IsImmediateSuppurvisorApproved = true;
                        evaluation.ImmediateSuppurvisorApprovalDate = approvalDate;
                        evaluation.ImmediateSuppurvisorApprover = approverName;
                        evaluation.Remark = remark;

                        // Item 607: Structured Evaluator Feedback Areas
                        evaluation.SpecialAccomplishments = specialAccomplishments;
                        evaluation.Strengths = strengths;
                        evaluation.AreasNeedingImprovement = areasNeedingImprovement;

                        // Item 606: Clear prior 2nd-level rejection state on resubmission
                        evaluation.IsSecondLevelRejected = false;
                        evaluation.ReasonFor2ndRejection = null;

                        db.Entry(evaluation).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    // Send notification to the employee
                    if (employee != 0)
                    {
                        var targetUserId = UserInformationHelper.GetEmployeeUserId(employee);
                        if (!string.IsNullOrEmpty(targetUserId))
                        {
                            Notify.NotifyUser(new NotifModels
                            {
                                UserId = targetUserId,
                                Title = "Performance Evaluation Approved (Level 1)",
                                Message = "Your performance evaluation has been approved by your supervisor and submitted for Level 2 review.",
                                URL = "/HRM/EmployeePerformanceSummaries/"
                            });
                        }
                    }

                    return Json(new
                    {
                        Success = true,
                        Message = "Performance evaluation approved successfully and forwarded to Level 2."
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new
                    {
                        Success = false,
                        Message = "An error occurred while approving the evaluation: " + ex.Message
                    });
                }
            }
        }
        //public string ApproveEvaluation(int year, int period, int employee,int empPerformanceId, string remark)
        //{
        //    var status = "false";
        //    var performance = db.EmployeePeerToPeerAndInternalCustomerPerformanceSettings.FirstOrDefault();
        //    var employeePerformances = db.EmployeePerformances
        //                                       .Where(x => x.GlFiscalYearId == year
        //                                           && x.GlPeriodId == period
        //                                           && x.EmployeeId == employee
        //                                           && x.IsApproved
        //                                           && (x.IsAcceptedByEmployee
        //                                               || x.IsRejectedByEmployee)
        //                                           && !x.IsImmediateSuppurvisorApproved)
        //                                           .ToList();
        //    if (performance !=null 
        //        && (performance.IsInternalCustomerHasMultipleEvaluator
        //        || performance.IsPeerToPeerHasMultipleEvaluator))
        //    {
        //        var peerToPeerPerformances = db.EmployeePerformances
        //                                                  .Where(x => x.GlFiscalYearId == year
        //                                                        && x.GlPeriodId == period
        //                                                        && x.EmployeeId == employee
        //                                                        && (x.EvalutionType == "PeerToPeer"||x.EmployeeRemark== "Averaged Evaluation")
        //                                                        && x.IsApproved
        //                                                        && !x.IsAveraged
        //                                                        && (x.IsAcceptedByEmployee
        //                                                            || x.IsRejectedByEmployee)
        //                                                        && !x.IsImmediateSuppurvisorApproved)
        //                                                        .ToList();
        //        // Group by EvaluationCriteriaTypeId and average only groups with more than one entry
        //        var groupedByCriteria = peerToPeerPerformances
        //            .GroupBy(x => x.EvaluationCriteriaTypeId)
        //            .Where(g => g.Count() > 1);

        //        foreach (var group in groupedByCriteria)
        //        {
        //            AveragingEvaluationFinal(group.ToList());
        //        }


        //        var employeeInternalCustomerPerformances = db.EmployeePerformances
        //                                                .Where(x => x.GlFiscalYearId == year
        //                                                             && x.GlPeriodId == period
        //                                                             && x.EmployeeId == employee
        //                                                             && (x.EvalutionType == "InternalCustomer" || x.EmployeeRemark == "Averaged Evaluation")
        //                                                             && x.IsApproved
        //                                                             && !x.IsAveraged
        //                                                             && (x.IsAcceptedByEmployee
        //                                                                 || x.IsRejectedByEmployee)
        //                                                             && !x.IsImmediateSuppurvisorApproved)
        //                                                             .ToList();

        //        var groupedByInternalCriteria = employeeInternalCustomerPerformances
        //            .GroupBy(x => x.EvaluationCriteriaTypeId)
        //            .Where(g => g.Count() > 1);

        //        foreach (var group in groupedByInternalCriteria)
        //        {
        //            AveragingEvaluationFinal(group.ToList());
        //        }

        //        employeePerformances = employeePerformances.ToList();
        //    }

        //    if (db.EmployeePerformances
        //                .Where(x => x.GlFiscalYearId == year 
        //                             && x.GlPeriodId == period 
        //                             && x.EmployeeId == employee 
        //                             && x.IsApproved 
        //                             && !x.IsAcceptedByEmployee
        //                             && !x.IsRejectedByEmployee)
        //                             .Any())
        //    {
        //        return status;
        //    }
        //    var employeePerformancess = db.EmployeePerformances
        //                                                .Where(x => x.GlFiscalYearId == year
        //                                                             && x.GlPeriodId == period
        //                                                             && x.EmployeeId == employee
        //                                                             && x.IsApproved
        //                                                             && !x.IsAveraged
        //                                                             && (x.IsAcceptedByEmployee
        //                                                                 || x.IsRejectedByEmployee)
        //                                                             && !x.IsImmediateSuppurvisorApproved)
        //                                                             .ToList();
        //    foreach ( var employeePerformance in employeePerformancess)
        //    {

        //        employeePerformance.Remark = remark;
        //        employeePerformance.IsImmediateSuppurvisorApproved = true;
        //        employeePerformance.ImmediateSuppurvisorApprovalDate = DateTime.Now;
        //        employeePerformance.ImmediateSuppurvisorApprover = User.Identity.Name;
        //        db.Entry(employeePerformance).State = EntityState.Modified;
        //        db.SaveChanges();
        //        status= "true";
        //    }
        //    if (employee != 0)
        //    {
        //        //var bossId = EmployeeHelper.GetSupervisor(placement.EmployeeId);
        //        NotifModels notif = new NotifModels
        //        {
        //            UserId = UserInformationHelper.GetEmployeeUserId(employee),
        //            Title = "Immediate Suppurvisor has approved your Performance ",
        //            Message = $" you have performance has been approved by your Immediate Suppurvisor",
        //            URL = "/HRM/EmployeePerformanceSummaries/"
        //        };

        //        Notify.NotifyUser(notif);
        //    }
        //    return status;
        //}
        [HttpPost]
        public JsonResult RejectEvaluation(int year, int period, int employee, string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                return Json(new { Success = false, Message = "Rejection reason is required." }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Find records currently approved by L1 but awaiting/pending L2 action
                    var performances = db.EmployeePerformances
                        .Where(x => x.GlFiscalYearId == year
                                 && x.GlPeriodId == period
                                 && x.EmployeeId == employee
                                 && x.IsApproved
                                 && !x.IsAveraged
                                 && x.IsImmediateSuppurvisorApproved
                                 && !x.IsSecondLevelApproved)
                        .ToList();

                    if (!performances.Any())
                    {
                        return Json(new { Success = false, Message = "No eligible evaluations found to reject." }, JsonRequestBehavior.AllowGet);
                    }

                    var now = DateTime.Now;
                    var currentApprover = User?.Identity?.Name ?? "System";
                    bool isEscalated = false;

                    foreach (var perf in performances)
                    {
                        // Item 606: Track review cycles/interactions
                        perf.InteractionCount = (perf.InteractionCount ?? 0) + 1;

                        // Level 2 Rejection flags
                        perf.IsSecondLevelRejected = true;
                        perf.IsSecondLevelApproved = false;
                        perf.ReasonFor2ndRejection = rejectionReason;
                        perf.SecondLevelRejectionDate = now;
                        perf.SecondLevelRejector = currentApprover;

                        // Item 606: Return back to Level 1 Evaluator by revoking L1 approval
                        perf.IsImmediateSuppurvisorApproved = false;

                        // Item 606: Escalate to HR compliance review if interaction threshold (>= 3) is reached
                        if (perf.InteractionCount >= 3)
                        {
                            perf.IsEscalatedToHR = true;
                            perf.EscalationDate = now;
                            isEscalated = true;
                        }

                        db.Entry(perf).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    // --- Notifications ---
                    // 1. Notify Level 1 Supervisor that the review was returned
                    var supervisorUserId = performances.FirstOrDefault()?.ImmediateSuppurvisorApprover;
                    if (!string.IsNullOrEmpty(supervisorUserId))
                    {
                        Notify.NotifyUser(new NotifModels
                        {
                            UserId = supervisorUserId,
                            Title = isEscalated ? "Evaluation Escalated to HR" : "Performance Evaluation Returned (Level 2 Rejection)",
                            Message = isEscalated
                                ? $"Evaluation for employee #{employee} exceeded 3 review cycles and has been escalated to HR."
                                : $"Evaluation for employee #{employee} was rejected by the 2nd evaluator with reason: {rejectionReason}",
                            URL = "/HRM/EmployeePerformanceSummaries/"
                        });
                    }

                    // 2. Notify Employee
                    var empUserId = UserInformationHelper.GetEmployeeUserId(employee);
                    if (!string.IsNullOrEmpty(empUserId))
                    {
                        Notify.NotifyUser(new NotifModels
                        {
                            UserId = empUserId,
                            Title = "Performance Evaluation Update",
                            Message = isEscalated
                                ? "Your evaluation has been forwarded to HR for compliance review after multiple review cycles."
                                : "Your performance evaluation was returned to your supervisor for further review.",
                            URL = "/HRM/EmployeePerformanceSummaries/"
                        });
                    }

                    string responseMessage = isEscalated
                        ? "Evaluation rejected and escalated to HR compliance (3 interaction cycles reached)."
                        : "Evaluation rejected and returned to the 1st Evaluator for further review.";

                    return Json(new { Success = true, Message = responseMessage }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { Success = false, Message = "An error occurred while rejecting the evaluation: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
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

