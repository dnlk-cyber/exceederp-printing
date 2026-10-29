using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class PMERController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        #region Views

        // PMER Main Dashboard
        public ActionResult Index()
        {
            ViewBag.TotalStrategicPlans = db.StrategicPlans.Count();
            ViewBag.TotalStrategicGoals = db.StrategicGoals.Count();
            ViewBag.TotalCorporateKPIs = db.GlobalKPIs.Count(x => x.StandardKPI != null);
            ViewBag.TotalDepartmentPlans = db.BSCPlans.Count();
            ViewBag.TotalApprovedPlans = db.BSCPlans.Count(x => x.ApprovalStatus == PlanApprovalStatus.BranchApproved || x.ApprovalStatus == PlanApprovalStatus.HeadOfficeApproved);

            // Alignment calculation
            int totalPlans = db.BSCPlans.Count();
            int alignedPlans = db.BSCPlans.Count(x => x.ApprovalStatus != PlanApprovalStatus.Draft);
            decimal alignmentRate = totalPlans > 0 ? Math.Round(((decimal)alignedPlans / totalPlans) * 100m, 1) : 100m;
            ViewBag.AlignmentRate = alignmentRate;

            return View();
        }

        // Pillar 1: Strategic Plan
        public ActionResult StrategicPlan()
        {
            PopulateLookups();
            return View();
        }

        // Pillar 2: Annual Plan & Cascading
        public ActionResult AnnualPlan()
        {
            PopulateLookups();
            return View();
        }

        // Pillar 3: Actual Plan & Execution Tracking
        public ActionResult ActualPlan()
        {
            PopulateLookups();
            return View();
        }

        // Pillar 4: PMER Corporate Reports & Governance
        public ActionResult Reports(string reportType = "Strategic")
        {
            PopulateLookups();
            ViewBag.SelectedReportType = reportType;

            string title = "PMER Corporate Strategic Plan Report";
            string typeName = "ExceedERP.Reporting.Planning.Design.StrategicPlanReport, ExceedERP.Reporting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

            switch (reportType)
            {
                case "KPI":
                    title = "PMER Corporate KPI Progress Tracker Report";
                    typeName = "ExceedERP.Reporting.Planning.Design.KpiProgressTrackerReport, ExceedERP.Reporting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    break;
                case "Quarterly":
                    title = "PMER Quarterly Plan vs Actual Report";
                    typeName = "ExceedERP.Reporting.Planning.Design.PlanVsActualQuarterlyReport, ExceedERP.Reporting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    break;
                case "Annual":
                    title = "PMER Annual Operational Plan Report";
                    typeName = "ExceedERP.Reporting.Planning.Design.PlanReport, ExceedERP.Reporting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    break;
                case "Executive":
                    title = "PMER Executive Summary Report";
                    typeName = "ExceedERP.Reporting.Planning.Design.ExcutiveSummaryReport, ExceedERP.Reporting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    break;
                default:
                    title = "PMER Corporate Strategic Plan Report";
                    typeName = "ExceedERP.Reporting.Planning.Design.StrategicPlanReport, ExceedERP.Reporting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    break;
            }

            ViewBag.FormTitle = title;
            ViewBag.FormNumber = string.Empty;
            ViewBag.ReportTypeName = typeName;
            return View();
        }

        #endregion

        #region Lookups

        private void PopulateLookups()
        {
            ViewData["GlFiscalYears"] = db.GLFiscalYears
                .OrderBy(x => x.DateFrom)
                .Select(x => new
                {
                    x.GlFiscalYearId,
                    x.Name
                })
                .ToList();

            ViewData["departmentLists"] = db.MainCompanyStructures
                .Select(x => new
                {
                    x.OrgStructureId,
                    x.Name
                })
                .ToList();

            ViewData["StrategicPlans"] = db.StrategicPlans
                .Select(x => new
                {
                    x.StrategicPlanId,
                    x.Description
                })
                .ToList();
        }

        #endregion

        #region Pillar 1: Strategic Plan & 5-Year Implementation Schedule Endpoints

        public ActionResult StrategicPlans_Read([DataSourceRequest] DataSourceRequest request)
        {
            var strategicPlans = db.StrategicPlans;
            var result = strategicPlans.ToDataSourceResult(request, strategicPlan => new
            {
                strategicPlan.StrategicPlanId,
                strategicPlan.FromGlFiscalYearId,
                strategicPlan.ToGlFiscalYearId,
                strategicPlan.StrategicObjective,
                strategicPlan.TotalBudget,
                strategicPlan.Description,
                strategicPlan.DateCreated,
                strategicPlan.CreatedBy,
                strategicPlan.LastModified,
                strategicPlan.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StrategicPlans_Create([DataSourceRequest] DataSourceRequest request, StrategicPlan strategicPlan)
        {
            if (ModelState.IsValid)
            {
                strategicPlan.DateCreated = DateTime.Now.Date;
                strategicPlan.CreatedBy = User.Identity.Name;

                db.StrategicPlans.Add(strategicPlan);
                db.SaveChanges();
            }

            return Json(new[] { strategicPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StrategicPlans_Update([DataSourceRequest] DataSourceRequest request, StrategicPlan strategicPlan)
        {
            if (ModelState.IsValid)
            {
                var existing = db.StrategicPlans.Find(strategicPlan.StrategicPlanId);
                if (existing != null)
                {
                    existing.StrategicObjective = strategicPlan.StrategicObjective;
                    existing.Description = strategicPlan.Description;
                    existing.TotalBudget = strategicPlan.TotalBudget;
                    existing.FromGlFiscalYearId = strategicPlan.FromGlFiscalYearId;
                    existing.ToGlFiscalYearId = strategicPlan.ToGlFiscalYearId;
                    existing.LastModified = DateTime.Now.Date;
                    existing.ModifiedBy = User.Identity.Name;
                    db.SaveChanges();
                }
            }

            return Json(new[] { strategicPlan }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ImplementationSchedule_Read([DataSourceRequest] DataSourceRequest request, int? strategicPlanId)
        {
            var query = db.StrategicGoals.AsQueryable();
            if (strategicPlanId.HasValue && strategicPlanId.Value > 0)
            {
                query = query.Where(x => x.StrategicPlanId == strategicPlanId.Value);
            }

            var result = query.OrderBy(x => x.Code).ToDataSourceResult(request, goal => new
            {
                goal.StrategicGoalId,
                goal.StrategicPlanId,
                goal.Code,
                goal.StrategicObjective,
                goal.Name,
                goal.Description,
                goal.ResponsibleBody,
                goal.UOM,
                goal.Baseline,
                goal.Target2015EFY,
                goal.Target2016EFY,
                goal.Target2017EFY,
                goal.Target2018EFY,
                goal.Target2019EFY,
                goal.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImplementationSchedule_Update([DataSourceRequest] DataSourceRequest request, StrategicGoal goal)
        {
            if (ModelState.IsValid)
            {
                var existing = db.StrategicGoals.Find(goal.StrategicGoalId);
                if (existing != null)
                {
                    existing.Code = goal.Code;
                    existing.StrategicObjective = goal.StrategicObjective;
                    existing.Name = goal.Name;
                    existing.Description = goal.Description;
                    existing.ResponsibleBody = goal.ResponsibleBody;
                    existing.UOM = goal.UOM;
                    existing.Baseline = goal.Baseline;
                    existing.Target2015EFY = goal.Target2015EFY;
                    existing.Target2016EFY = goal.Target2016EFY;
                    existing.Target2017EFY = goal.Target2017EFY;
                    existing.Target2018EFY = goal.Target2018EFY;
                    existing.Target2019EFY = goal.Target2019EFY;
                    existing.Remark = goal.Remark;
                    existing.LastModified = DateTime.Now;
                    existing.ModifiedBy = User.Identity.Name;
                    db.SaveChanges();
                }
            }

            return Json(new[] { goal }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Pillar 2: Corporate KPIs & Cascading Endpoints

        public ActionResult CorporateKPIs_Read([DataSourceRequest] DataSourceRequest request)
        {
            var kpis = db.GlobalKPIs.Where(x => x.StandardKPI != null || x.KeyResultArea != null);
            var result = kpis.ToDataSourceResult(request, k => new
            {
                k.GlobalKPIId,
                k.StrategicObjective,
                k.KeyResultArea,
                k.AnnualObjective,
                k.StandardKPI,
                k.MeasurementFormula,
                k.AnnualTargetText,
                k.ThresholdText,
                k.IsActive
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CorporateKPIs_Create([DataSourceRequest] DataSourceRequest request, GlobalKPI kpi)
        {
            if (ModelState.IsValid)
            {
                kpi.KPIName = kpi.StandardKPI ?? kpi.KeyResultArea ?? "PMER KPI";
                kpi.IsActive = true;
                kpi.DateCreated = DateTime.Now;
                kpi.CreatedBy = User.Identity.Name;
                db.GlobalKPIs.Add(kpi);
                db.SaveChanges();
            }

            return Json(new[] { kpi }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CorporateKPIs_Update([DataSourceRequest] DataSourceRequest request, GlobalKPI kpi)
        {
            if (ModelState.IsValid)
            {
                var existing = db.GlobalKPIs.Find(kpi.GlobalKPIId);
                if (existing != null)
                {
                    existing.StrategicObjective = kpi.StrategicObjective;
                    existing.KeyResultArea = kpi.KeyResultArea;
                    existing.AnnualObjective = kpi.AnnualObjective;
                    existing.StandardKPI = kpi.StandardKPI;
                    existing.MeasurementFormula = kpi.MeasurementFormula;
                    existing.AnnualTargetText = kpi.AnnualTargetText;
                    existing.ThresholdText = kpi.ThresholdText;
                    existing.IsActive = kpi.IsActive;
                    existing.LastModified = DateTime.Now;
                    existing.ModifiedBy = User.Identity.Name;
                    db.SaveChanges();
                }
            }

            return Json(new[] { kpi }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DepartmentAlignment_Read([DataSourceRequest] DataSourceRequest request)
        {
            var orgs = db.MainCompanyStructures.ToList();
            var plans = db.BSCPlans.ToList();

            var data = orgs.Select(o =>
            {
                var unitPlans = plans.Where(p => p.OrgStructureId == o.OrgStructureId).ToList();
                int count = unitPlans.Count;
                int approved = unitPlans.Count(p => p.ApprovalStatus == PlanApprovalStatus.BranchApproved || p.ApprovalStatus == PlanApprovalStatus.HeadOfficeApproved);
                string status = count == 0 ? "Not Started" : (approved == count ? "100% Aligned" : "Pending Alignment");
                decimal rate = count > 0 ? Math.Round(((decimal)approved / count) * 100m, 1) : 0m;

                return new
                {
                    OrgStructureId = o.OrgStructureId,
                    DepartmentName = o.Name,
                    TotalPlans = count,
                    ApprovedPlans = approved,
                    AlignmentRate = rate,
                    Status = status,
                    ThresholdMet = rate >= 90m
                };
            }).ToList();

            return Json(data.ToDataSourceResult(request));
        }

        #endregion

        #region Pillar 3: Operational Validation & Actuals Endpoints

        public ActionResult OperationalValidation_Read([DataSourceRequest] DataSourceRequest request)
        {
            var plans = db.BSCPlans
                .Include(x => x.Objective)
                .Include(x => x.GetPlan)
                .ToList();

            var orgMap = db.MainCompanyStructures.ToDictionary(k => k.OrgStructureId, v => v.Name);

            var data = plans.Select(p => new
            {
                p.BSCPlanId,
                PlanDescription = p.GetPlan != null ? p.GetPlan.Description : "Annual Plan",
                ObjectiveName = p.Objective != null ? p.Objective.Name : "N/A",
                WorkUnit = orgMap.ContainsKey(p.OrgStructureId) ? orgMap[p.OrgStructureId] : "N/A",
                p.ApprovalStatus,
                StatusText = p.ApprovalStatus.ToString(),
                IsValidated = p.ApprovalStatus == PlanApprovalStatus.BranchApproved || p.ApprovalStatus == PlanApprovalStatus.HeadOfficeApproved,
                p.SubmittedBy,
                p.SubmittedTime
            }).ToList();

            return Json(data.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult ValidateOperationalPlan(int bscPlanId)
        {
            var plan = db.BSCPlans.Find(bscPlanId);
            if (plan != null)
            {
                plan.ApprovalStatus = PlanApprovalStatus.BranchApproved;
                plan.BranchApprovedBy = User.Identity.Name;
                plan.BranchApprovedTime = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, message = "Plan validated and approved for execution." });
            }
            return Json(new { success = false, message = "Plan not found." });
        }

        #endregion

        #region Pillar 4: Reports Data Endpoints

        public ActionResult PMERReportsData_Read([DataSourceRequest] DataSourceRequest request, string reportType)
        {
            var reports = new List<object>();

            if (reportType == "Alignment")
            {
                int totalDepts = db.MainCompanyStructures.Count();
                int alignedDepts = db.MainCompanyStructures.Count(d => db.BSCPlans.Any(p => p.OrgStructureId == d.OrgStructureId && (p.ApprovalStatus == PlanApprovalStatus.BranchApproved || p.ApprovalStatus == PlanApprovalStatus.HeadOfficeApproved)));
                decimal rate = totalDepts > 0 ? Math.Round(((decimal)alignedDepts / totalDepts) * 100m, 1) : 100m;

                reports.Add(new
                {
                    ReportCode = "REP-ALIGN-01",
                    Title = "Strategic Goal Alignment Report (Semi-Annual)",
                    Frequency = "Semi-Annually (2 Reports)",
                    Target = "100% Alignment",
                    Threshold = "≥ 90% Alignment",
                    CurrentAchievement = rate + "%",
                    Status = rate >= 90m ? "Meets Threshold" : "Under Threshold",
                    SubmissionStatus = "Verified & Complete"
                });
            }
            else if (reportType == "Monthly")
            {
                for (int m = 1; m <= 12; m++)
                {
                    reports.Add(new
                    {
                        ReportCode = $"REP-MON-{m:D2}",
                        Title = $"Month {m} Corporate Performance Monitoring Report",
                        Frequency = "Monthly",
                        Target = "Delivered to BOD",
                        Threshold = "100% On-Time",
                        CurrentAchievement = m <= 3 ? "Submitted & Reviewed" : "Scheduled",
                        Status = m <= 3 ? "Completed" : "Pending",
                        SubmissionStatus = m <= 3 ? "On Time" : "Scheduled"
                    });
                }
            }
            else if (reportType == "Quarterly")
            {
                for (int q = 1; q <= 4; q++)
                {
                    reports.Add(new
                    {
                        ReportCode = $"REP-QTR-{q:D2}",
                        Title = $"Quarter {q} Comprehensive Institutional Evaluation Report",
                        Frequency = "Quarterly",
                        Target = "1 Evaluation Report",
                        Threshold = "4 Reports Annually",
                        CurrentAchievement = q == 1 ? "Completed & Endorsed" : "In Progress",
                        Status = q == 1 ? "Endorsed by AB" : "Pending Quarter Close",
                        SubmissionStatus = q == 1 ? "Published" : "Scheduled"
                    });
                }
            }

            return Json(reports.ToDataSourceResult(request));
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
