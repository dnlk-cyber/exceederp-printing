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

        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {

            var fiscalYearList = db.GLFiscalYears.ToList();
            ViewData["fiscalYearList"] = fiscalYearList;// != null ? periodList.ToList() : new List<GlFiscalYear>();
            ViewData["EmployeeList"] = db.Person_Employee.Select(x=>new { x.EmployeeId,
                              FullNameAmharic = x.EmployeeCode+"-"+x.FirstName+" "+x.MiddleName+" "+x.LastName}).AsQueryable();
           
            ViewData["Period"] = db.Quarters.Select(x=>new { x.QuarterId,x.Name}).ToList();
            var branches = db.Branch.Select(x => new { x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            var workunits = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
            ViewData["workunits"] = workunits;

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

            var fiscalYear = db.GLFiscalYears.ToList();

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
        public JsonResult GenerateEmployeePerformanceSummary(string empId, int fiscalYearId, int periodId,string branchId,string code)
        {
            decimal datesServedInThisFiscalYear = 0;
            decimal bonus = 0;
            var employeePerformances = db.EmployeePerformances.Where(x=>x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.IsHRApproved).ToList();

            if (!string.IsNullOrEmpty(empId)&&int.TryParse(empId,out int employeeId))
            {
                employeePerformances = employeePerformances.Where(x => x.EmployeeId == employeeId).ToList();
            }
            if (!string.IsNullOrEmpty(branchId)&&int.TryParse(branchId,out int brachIdParded))
            {
                employeePerformances = employeePerformances.Where(x => x.BranchId == brachIdParded).ToList();
            }
            if (!string.IsNullOrEmpty(code)&&int.TryParse(code,out int businessUnitId))
            {
                employeePerformances = employeePerformances.Where(x => x.BusinessUnitId == businessUnitId).ToList();
            }
            
            if (!employeePerformances.Any())
            {
                return Json(new
                {
                    Message = "No employee performance records have been approved by HR at this time. Please ensure that records are submitted and approved before proceeding.",
                    Status = "Not-OK"
                }, JsonRequestBehavior.AllowGet);
            }
            var distictemployeePerformances = employeePerformances
                               .Where(x => x.IsHRApproved).DistinctBy(x => new
                               {
                                   x.EmployeeId,
                                   x.GlFiscalYearId,
                                   x.GlPeriodId,
                               }).OrderByDescending(x => x.GlFiscalYearId).ToList();
            foreach (var empPerformance in distictemployeePerformances)
            {
                var person = db.Person_Employee.Find(empPerformance.EmployeeId);

                var allEvalutionsPerEmployee = employeePerformances
                                                        .Where(x => x.EmployeeId == empPerformance.EmployeeId
                                                        && x.GlFiscalYearId == fiscalYearId
                                                        && x.GlPeriodId == periodId ).ToList();

                decimal weight = allEvalutionsPerEmployee
               .Sum(performance => GetPerformanceWeight(performance.EmployeePerformanceId));
                // Calculate total performance score, ensuring that GetPerformanceWeight is not null
                decimal score = allEvalutionsPerEmployee
               .Sum(performance => GetPerformanceResult(performance.EmployeePerformanceId));
                var fiscalyear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == empPerformance.GlFiscalYearId);
                if (fiscalyear != null)
                {
                    if (fiscalyear != null && person != null)
                    {
                        DateTime startDate = fiscalyear.DateFrom < person.HiredDate ? person.HiredDate : fiscalyear.DateFrom;
                        datesServedInThisFiscalYear = (decimal)(fiscalyear.DateTo - startDate).TotalDays;
                    }
                    else
                    {
                        datesServedInThisFiscalYear = (decimal)(fiscalyear.DateTo - fiscalyear.DateFrom).TotalDays;

                    }
                   
                }
                if (datesServedInThisFiscalYear > 360)
                {
                    datesServedInThisFiscalYear = 360;
                }
               
                if (score > 100 && weight !=0)
                {
                    score = score * 100 / weight;
                }
                if (weight > 100)
                {
                    weight = 100;
                }
                // Check if the total weight is 100
                if (weight == 100 && score>0)
                {
                    bonus =  CalculateBonus(datesServedInThisFiscalYear, score)*100;
                    var employeesummary = db.EmployeePerformanceSummary
                                            .FirstOrDefault(x=>x.GlFiscalYearId==empPerformance.GlFiscalYearId
                                            &&x.GlPeriodId==empPerformance.GlPeriodId
                                            &&x.EmployeeId==empPerformance.EmployeeId);
                    if(employeesummary == null)
                    {
                        var entity = new EmployeePerformanceSummary
                        {
                            EmployeeId = empPerformance.EmployeeId,
                            GlFiscalYearId = empPerformance.GlFiscalYearId,
                            GlPeriodId = empPerformance.GlPeriodId,
                            OrgStructureId = empPerformance.OrgStructureId,
                            BranchId = empPerformance.BranchId,
                            BusinessUnitId = empPerformance.BusinessUnitId,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                            Weight = weight,
                            Result = Math.Round(score, 2),
                            TotalDays = datesServedInThisFiscalYear,
                            Bonus = bonus
                        };
                        db.EmployeePerformanceSummary.Add(entity);
                    }
                    else
                    {
                        employeesummary.OrgStructureId = empPerformance.OrgStructureId;
                        employeesummary.BranchId = empPerformance.BranchId;
                        employeesummary.BusinessUnitId = empPerformance.BusinessUnitId;
                        employeesummary.Weight = weight;
                        employeesummary.Result =Math.Round(score,2);
                        employeesummary.TotalDays = datesServedInThisFiscalYear;
                        employeesummary.Bonus = bonus;
                        employeesummary.LastModified = DateTime.Now;
                        employeesummary.ModifiedBy = User.Identity.Name;
                        db.Entry(employeesummary).State = EntityState.Modified;
                    }
                   
                    db.SaveChanges();
                }
             

            }
            return Json(new
            {
                Message = "Employee performance summaries have been successfully generated and updated.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }
        decimal CalculateBonus(decimal K6, decimal J6)
        {
            return (K6 >= 180)
                ? (J6 < 75 ? 0m
                    : (J6 <= 80 ? 0.8m * (K6 / 360m)
                        : (J6 <= 90 ? 0.9m * (K6 / 360m)
                            : 1m * (K6 / 360m))))
                : 0m;
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

