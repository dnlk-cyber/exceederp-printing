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
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    //[AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class EmployeePerformanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Measurments"] = db.Measurements.ToList();
            ViewData["PerformanceActions"] = db.PerformanceActions.ToList();
            ViewData["CriteriaCategory"] = db.CriteriaCategories.Select(x => new { x.CriteriaCategoryId, x.Name }).ToList();


            return View();
        }

      
        public ActionResult EmployeeMeasurements_Read([DataSourceRequest] DataSourceRequest request, string empId, string fiscalYearId, string periodId, string evaluationCriteriaId,int criteriaCategoryId)
        {
            int.TryParse(empId, out int employeeId);
            int.TryParse(fiscalYearId, out int glFiscalYearId);
            int.TryParse(periodId, out int glPeriodId);
            int.TryParse(evaluationCriteriaId, out int evalCriteriaId);

            int employeePerformanceId = 0;

            if (employeeId != 0 && glFiscalYearId != 0 && glPeriodId != 0 && evalCriteriaId != 0)
            {
                employeePerformanceId = GenerateEmployeeForPerofmanceEvaluation(employeeId, glFiscalYearId, glPeriodId, evalCriteriaId,criteriaCategoryId);
            }

            IQueryable<EmployeeMeasurement> measurements = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformanceId);

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
            }

            //var criteriaType = db.EvaluationCriteriaActionTypes.FirstOrDefault(x => x.IsActive);
            //if (criteriaType != null && criteriaType.PerformanceCriteriaActionType == PerformanceCriteriaActionType.Open)
            //{
            //    //var revertOther = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == employeePerformanceScore.EmployeeMeasurementId && x.EmployeePerformanceScoreId != employeePerformanceScore.EmployeePerformanceScoreId).ToList();
            //    //foreach (var rev in revertOther)
            //    //{
            //        //rev.IsChecked = false;
            //        //rev.Score = 0;

            //        db.Entry(rev).State = EntityState.Modified;
            //        db.SaveChanges();
            ////    }
            //}

            if (ModelState.IsValid)
            {

                var entity = new EmployeePerformanceScore
                {
                    EmployeePerformanceScoreId = employeePerformanceScore.EmployeePerformanceScoreId,
                    PerformanceActionId = employeePerformanceScore.PerformanceActionId,
                    EmployeeMeasurementId = employeePerformanceScore.EmployeeMeasurementId,
                    Weight = employeePerformanceScore.Weight,
                    Score = employeePerformanceScore.Score,
                    // IsChecked = employeePerformanceScore.IsChecked,
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
        //reports 
        public ActionResult PerformanceSummary()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Performance_Summary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult EmployeePerformanceTotalSummary()
        {
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Employee_Performance_Total_Summary_Report, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult PerformanceSummaryDepartment()
        {
            ApplicationDbContext adb = new ApplicationDbContext();
            string username = User.Identity.Name;
            var user = adb.Users.FirstOrDefault(x => x.UserName == username);

            if (user != null)
            {
                ViewBag.id = user.EmployeeId;
            }

            return View();
        }

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
        public string ApproveEvaluation(int year, int period, int employee, int evaluationCriteria, int criteriaCategoryId)
        {
            var employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.GlFiscalYearId == year && x.GlPeriodId == period && x.EmployeeId == employee && x.EvaluationCriteriaTypeId == evaluationCriteria&&x.CriteriaCategoryId==criteriaCategoryId);

            if (employeePerformance != null)
            {
                employeePerformance.IsApproved = true;

                db.Entry(employeePerformance).State = EntityState.Modified;
                db.SaveChanges();

                return "true";
            }

            return "false";
        }
        //public JsonResult GetEvaluationWeightandResult(int evaluationCriteriaId, int employeeId, int yearId, int periodId)
        //{
        //    decimal result = 0;
        //    string approval = "Not Submitted";

        //    var employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == employeeId && x.GlFiscalYearId == yearId && x.GlPeriodId == periodId && x.EvaluationCriteriaTypeId == evaluationCriteriaId);
        //    if (employeePerformance != null)
        //    {
        //        if (employeePerformance.IsApproved)
        //        {
        //            approval = "Submitted";
        //        }
        //        else
        //        {
        //            approval = "Not Submitted";
        //        }

        //        var empMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformance.EmployeePerformanceId).ToList();

        //        foreach (var em in empMeasurments)
        //        {
        //            var score = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId);

        //            if (score.Any())
        //            {
        //                result = result + score.Sum(x => x.Score);

        //            }
        //        }
        //    }

        //    var ev = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == evaluationCriteriaId);
        //    if (ev != null)
        //    {
        //        var eval = new
        //        {
        //            Weight = ev.DefnPercentageofWeight,
        //            Result = result,
        //            Approval = approval
        //        };

        //        return Json(eval);
        //    }

        //    var eval1 = new
        //    {
        //        Weight = 0,
        //        Result = result,
        //        Approval = approval
        //    };

        //    return Json(eval1);
        //}

        public JsonResult GetEvaluationWeightandResult(int evaluationCriteriaId, int employeeId, int yearId, int periodId)
        {
            decimal result = 0;
            string approval = "Not Submitted";

            var employeePerformance = db.EmployeePerformances
                .FirstOrDefault(x => x.EmployeeId == employeeId &&
                                     x.GlFiscalYearId == yearId &&
                                     x.GlPeriodId == periodId &&
                                     x.EvaluationCriteriaTypeId == evaluationCriteriaId);

            if (employeePerformance != null)
            {
                approval = employeePerformance.IsApproved ? "Submitted" : "Not Submitted";

                var empMeasurements = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformance.EmployeePerformanceId).ToList();

                result = empMeasurements.SelectMany(em => db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId)).Sum(x => x.Score);
            }
        
            var ev = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == evaluationCriteriaId);

            var eval = new
            {
                Weight = ev?.DefnPercentageofWeight ?? 0,
                Result = result,
                Approval = approval
            };

            return Json(eval);
        }

        public int GenerateEmployeeForPerofmanceEvaluation(int empId, int fiscalYearId, int periodId, int evaluationCriteriaId,int criteriaCategoryId)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var placement = db.Placements.Where(x => x.EmployeeId == empId).Where(x => x.isCurrent).FirstOrDefault();
            var empPerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == empId && x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.EvaluationCriteriaTypeId == evaluationCriteriaId&&x.CriteriaCategoryId==criteriaCategoryId);
            if (empPerformance == null && placement != null)
            {
                var setting = db.PerformanceSettings.FirstOrDefault();
                var deadLine = setting != null ? setting.Value:0;
                var entity = new EmployeePerformance
                {
                    EmployeeId = empId,
                    DirectBossId= EmployeeHelper.GetSupervisor(id),
                    GlFiscalYearId = fiscalYearId,
                    GlPeriodId = periodId,
                    BranchId = placement.BranchId,
                    BusinessUnitId = placement.BusinessUnitId,
                    OrgStructureId = placement.OrgStructureId,
                    ResponsibilityLevel = placement.ResponsibilityLevel,
                    JobTitleId = placement.LookupId,
                    DeadLine = deadLine,
                    Date = DateTime.Now,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    EvaluationCriteriaTypeId = evaluationCriteriaId,
                    CriteriaCategoryId =criteriaCategoryId, 
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
            else if(empPerformance != null)
            {

                var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == evaluationCriteriaId).ToList();

                foreach (var meas in measurments)
                {
                    var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == empPerformance.EmployeePerformanceId);

                    if (checkExit == null)
                    {
                        var empMeasure = new EmployeeMeasurement();

                        empMeasure.MeasurmentId = meas.MeasurementId;
                        empMeasure.EmployeePerformanceId = empPerformance.EmployeePerformanceId;
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

               // return entity.EmployeePerformanceId;
                return empPerformance.EmployeePerformanceId;
                //var returnvalue1 = new
                //{
                //    EmployeePerformanceId = empPerformance.EmployeePerformanceId
                //};

                //return Json(returnvalue1, JsonRequestBehavior.AllowGet);
            }

            return 0;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

