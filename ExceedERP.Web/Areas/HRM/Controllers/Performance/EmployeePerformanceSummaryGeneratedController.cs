using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.Web.Helpers;
using Microsoft.Ajax.Utilities;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,HrmConstants.HRAdmin, HrmConstants.HRPerformanceUser)]
    public class EmployeePerformanceSummaryGeneratedController : BaseController
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
           
            ViewData["Period"] = db.Quarters.Select(x => new { x.QuarterId, x.Name }).ToList();

            

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
                    var placementss = db.Placements.Where(x => x.EmployeeId == empPerformance.EmployeeId).FirstOrDefault();
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
            // Get performance summaries for the given year
            var empPerformances = db.EmployeePerformanceSummary
                .Where(x => x.GlFiscalYearId == yearId)
                .ToList();

            if (!empPerformances.Any())
            {
                return Json(new
                {
                    Message = "No performance records found for the selected year.",
                    Status = "NoData"
                }, JsonRequestBehavior.AllowGet);
            }

            // Get employee IDs from performance summaries
            var employeeIds = empPerformances.Select(x => x.EmployeeId).Distinct().ToList();

            // Get placements only for those employees
            var placements = db.Placements
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .OrderByDescending(x => x.isCurrent)
                .ThenByDescending(x => x.PlacementId)
                .ToList();

            // Group placements by EmployeeId for quick lookup
            var placementLookup = placements
                .GroupBy(p => p.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault(x => x.isCurrent) ?? g.FirstOrDefault()
                );

            // Update performance summaries
            foreach (var empPerformance in empPerformances)
            {
                if (placementLookup.TryGetValue(empPerformance.EmployeeId, out var placement) && placement != null)
                {
                    empPerformance.OrgStructureId = placement.OrgStructureId;
                    empPerformance.BranchId = placement.BranchId;
                    empPerformance.BusinessUnitId = placement.BusinessUnitId;
                    empPerformance.ResponsibilityLevel = placement.ResponsibilityLevel;

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
        public ActionResult EmployeePerformanceSummary_Read([DataSourceRequest] DataSourceRequest request)
        {

            var employeePerformances = db.EmployeePerformanceSummary.ToList();

            DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
            {
                EmployeePerformanceSummaryId = employeePerformance.EmployeePerformanceSummaryId,
                EmployeeId = employeePerformance.EmployeeId,
                GlFiscalYearId = employeePerformance.GlFiscalYearId,
                GlPeriodId = employeePerformance.GlPeriodId,
                BranchId = employeePerformance.BranchId,
                BusinessUnitId = employeePerformance.BusinessUnitId,
                ResponsibilityLevel = employeePerformance.ResponsibilityLevel,
                OrgStructureId = employeePerformance.OrgStructureId,
                Result = employeePerformance.Result,
                Weight = employeePerformance.Weight,
                TotalDays = employeePerformance.TotalDays,
                Bonus = employeePerformance.Bonus,
                CreatedBy = employeePerformance.CreatedBy,
                DateCreated = employeePerformance.DateCreated,
                ModifiedBy = employeePerformance.ModifiedBy,
                LastModified = employeePerformance.LastModified,
                Remark = employeePerformance.Remark
            });

            return Json(result);
        }
        ////////////////
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

        public decimal GetPerformanceResult(int id)
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
        public JsonResult GenerateEmployeePerformanceSummary(string empId, int fiscalYearId, string branchId, string code)
        {
            var query = db.EmployeePerformances.Where(x => x.GlFiscalYearId == fiscalYearId && x.IsHRApproved);

            if (int.TryParse(empId, out int employeeId))
            {
                query = query.Where(x => x.EmployeeId == employeeId);
            }
            if (int.TryParse(branchId, out int branchIdParsed))
            {
                query = query.Where(x => x.BranchId == branchIdParsed);
            }
            if (int.TryParse(code, out int businessUnitId))
            {
                query = query.Where(x => x.BusinessUnitId == businessUnitId);
            }

            var employeePerformances = query.OrderBy(x => x.EmployeePerformanceId).ToList();

            if (!employeePerformances.Any())
            {
                return Json(new
                {
                    Message = "No employee performance records have been approved by HR for this Fiscal year. Please ensure that records are submitted and approved before proceeding.",
                    Status = "Not-OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // Weight/score totals per evaluation, computed in one query instead of per-row queries
            var totals = (from m in db.EmployeeMeasurement
                          join s in db.EmployeePerformanceScores on m.EmpMeasurementId equals s.EmployeeMeasurementId
                          join p in query on m.EmployeePerformanceId equals p.EmployeePerformanceId
                          group s by m.EmployeePerformanceId into g
                          select new { PerformanceId = g.Key, Weight = g.Sum(x => x.Weight), Score = g.Sum(x => x.Score) })
                         .ToDictionary(x => x.PerformanceId);

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId);

            var persons = db.Person_Employee
                .Where(p => query.Any(q => q.EmployeeId == p.EmployeeId))
                .Select(p => new { p.EmployeeId, p.EmployeeCode, p.HiredDate })
                .ToDictionary(p => p.EmployeeId);

            var existingSummaries = db.EmployeePerformanceSummary
                .Where(s => s.GlFiscalYearId == fiscalYearId && query.Any(q => q.EmployeeId == s.EmployeeId))
                .ToList()
                .GroupBy(s => new { s.EmployeeId, s.GlPeriodId })
                .ToDictionary(g => g.Key, g => g.First());

            var now = DateTime.Now;
            var userName = User.Identity.Name;
            var ranks = db.PerformanceRank.OrderBy(x => x.Minimum).ToList();
            int generated = 0;
            var skipped = new List<string>();

            foreach (var employeeGroup in employeePerformances.GroupBy(x => x.EmployeeId))
            {
                var evaluations = employeeGroup.ToList();
                var first = evaluations[0];
                persons.TryGetValue(first.EmployeeId, out var person);
                var label = person?.EmployeeCode ?? first.EmployeeId.ToString();

                foreach (var evaluation in evaluations)
                {
                    var result = totals.TryGetValue(evaluation.EmployeePerformanceId, out var t) ? t.Score : 0m;
                    if (evaluation.Result != result)
                    {
                        evaluation.Result = result;
                    }
                }

                // Sum per period, then average across periods
                var periodTotals = evaluations
                    .GroupBy(x => x.GlPeriodId)
                    .Select(g => new
                    {
                        Weight = g.Sum(p => totals.TryGetValue(p.EmployeePerformanceId, out var t) ? t.Weight : 0m),
                        Score = g.Sum(p => totals.TryGetValue(p.EmployeePerformanceId, out var t) ? t.Score : 0m)
                    })
                    .ToList();

                decimal weight = periodTotals.Average(g => g.Weight);
                decimal score = periodTotals.Average(g => g.Score);

                decimal daysServed = 0;
                if (fiscalYear != null)
                {
                    var startDate = person != null && fiscalYear.DateFrom < person.HiredDate ? person.HiredDate : fiscalYear.DateFrom;
                    daysServed = Math.Min((decimal)(fiscalYear.DateTo - startDate).TotalDays, 360m);
                }

                if (score > 100 && weight != 0)
                {
                    score = Math.Round(score * 100 / weight, 2);
                }
                if (weight > 100)
                {
                    weight = 100;
                }

                if (weight != 100)
                {
                    skipped.Add($"{label}: total weight is {weight:0.##} (must be 100)");
                    continue;
                }
                if (score <= 0)
                {
                    skipped.Add($"{label}: total score is {score:0.##} (must be greater than 0)");
                    continue;
                }

                var bonus = CalculateBonus(daysServed, score, ranks);

                if (!existingSummaries.TryGetValue(new { first.EmployeeId, first.GlPeriodId }, out var summary))
                {
                    db.EmployeePerformanceSummary.Add(new EmployeePerformanceSummary
                    {
                        EmployeeId = first.EmployeeId,
                        GlFiscalYearId = first.GlFiscalYearId,
                        GlPeriodId = first.GlPeriodId,
                        OrgStructureId = first.OrgStructureId,
                        BranchId = first.BranchId,
                        BusinessUnitId = first.BusinessUnitId,
                        ResponsibilityLevel = first.ResponsibilityLevel,
                        CreatedBy = userName,
                        DateCreated = now,
                        Weight = weight,
                        Result = Math.Round(score, 2),
                        TotalDays = daysServed,
                        Bonus = bonus
                    });
                }
                else
                {
                    summary.OrgStructureId = first.OrgStructureId;
                    summary.BranchId = first.BranchId;
                    summary.BusinessUnitId = first.BusinessUnitId;
                    summary.Weight = weight;
                    summary.ResponsibilityLevel = first.ResponsibilityLevel;
                    summary.Result = Math.Round(score, 2);
                    summary.TotalDays = daysServed;
                    summary.Bonus = bonus;
                    summary.LastModified = now;
                    summary.ModifiedBy = userName;
                }
                generated++;
            }

            db.SaveChanges();

            var message = $"{generated} employee performance summaries generated/updated.";
            if (skipped.Count > 0)
            {
                message += $"\n\n{skipped.Count} approved employee(s) skipped:\n" + string.Join("\n", skipped.Take(25));
                if (skipped.Count > 25)
                {
                    message += $"\n... and {skipped.Count - 25} more";
                }
            }

            return Json(new
            {
                Message = message,
                Status = generated > 0 ? "OK" : "Not-OK",
                Generated = generated,
                Skipped = skipped
            }, JsonRequestBehavior.AllowGet);
        }

        // BonusRate in Performance Rank is a percentage; the bonus is pro-rated by days served over 360.
        private static decimal CalculateBonus(decimal daysServed, decimal score, IEnumerable<PerformanceRank> ranks)
        {
            const decimal minimumDaysForBonus = 180m;
            if (daysServed < minimumDaysForBonus)
            {
                return 0m;
            }

            var rank = ranks.FirstOrDefault(x => x.Minimum <= score && x.Maximum >= score);
            return rank == null ? 0m : rank.BonusRate * (daysServed / 360m);
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

