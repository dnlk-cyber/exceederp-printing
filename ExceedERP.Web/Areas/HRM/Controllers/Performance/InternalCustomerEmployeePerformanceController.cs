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

    [AuthorizeRoles(HrmConstants.HRDepartmentHead,
        HrmConstants.HRPerformanceUser,
        HrmConstants.HRPerformanceAdmin,
        HrmConstants.HRDepartmentHead,
        HrmConstants.HREmployeeSelfService)]
    public class InternalCustomerEmployeePerformanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Measurments"] = db.Measurements.ToList();
            ViewData["PerformanceActions"] = db.PerformanceActions.ToList();

            return View();
        }

        public ActionResult InternalCustomerEmployeeMeasurements_Read([DataSourceRequest] DataSourceRequest request, string empId, string fiscalYearId, string periodId, string evaluationCriteriaId)
        {
            int.TryParse(empId, out int employeeId);
            int.TryParse(fiscalYearId, out int glFiscalYearId);
            int.TryParse(periodId, out int glPeriodId);
            int.TryParse(evaluationCriteriaId, out int evalCriteriaId);

            int employeePerformanceId = 0;

            if (employeeId != 0 && glFiscalYearId != 0 && glPeriodId != 0 && evalCriteriaId != 0)
            {
                employeePerformanceId = GenerateEmployeeForInternalCustomerPerofmanceEvaluation(employeeId, glFiscalYearId, glPeriodId, evalCriteriaId);
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

        public ActionResult InternalCustomerPerformanceActions_Read([DataSourceRequest] DataSourceRequest request, string empMeasurId)
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
        public ActionResult InternalCustomerPerformanceActions_Update([DataSourceRequest] DataSourceRequest request, EmployeePerformanceScore employeePerformanceScore)
        {
            if (employeePerformanceScore.Score > employeePerformanceScore.Weight)
            {
                ModelState.AddModelError("", "Score can't exceed the weight");
            }

            //var criteriaType = db.EvaluationCriteriaActionTypes.FirstOrDefault(x => x.IsActive);
            //if (criteriaType != null && criteriaType.PerformanceCriteriaActionType == PerformanceCriteriaActionType.Open)
            //{
            //    var revertOther = db.PeerToPeerEmployeePerformanceScores.Where(x => x.PeerEmployeeMeasurementId == employeePerformanceScore.PeerEmployeeMeasurementId && x.PeerToPeerEmployeePerformanceScoreId != employeePerformanceScore.PeerToPeerEmployeePerformanceScoreId).ToList();
            //    foreach (var rev in revertOther)
            //    {
            //        //rev.IsChecked = false;
            //        rev.Score = 0;

            //        db.Entry(rev).State = EntityState.Modified;
            //        db.SaveChanges();
            //    }
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

                 UserHelper.OperationLog(Request.UserHostAddress, "Employee Performance  ", "Success", User.Identity.Name, User.Identity.Name + " update Employee performance of employee id " + entity.EmployeeMeasurementId, Modules.HRM);

                #endregion

            }

            return Json(new[] { employeePerformanceScore }.ToDataSourceResult(request, ModelState));
        }
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
        public string ApproveEvaluation(int year, int period, int employee, int evaluationCriteria)
        {
            var employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.GlFiscalYearId == year && x.GlPeriodId == period && x.EmployeeId == employee && x.EvaluationCriteriaTypeId == evaluationCriteria);

            if (employeePerformance != null)
            {
                employeePerformance.IsApproved = true;

                db.Entry(employeePerformance).State = EntityState.Modified;
                db.SaveChanges();

                return "true";
            }

            return "false";
        }
        public JsonResult GetEvaluationWeightandResult(int evaluationCriteriaId, int employeeId, int yearId, int periodId)
        {
            decimal result = 0;
            string approval = "Not Submitted";

            var employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == employeeId && x.GlFiscalYearId == yearId && x.GlPeriodId == periodId && x.EvaluationCriteriaTypeId == evaluationCriteriaId);
            if (employeePerformance != null)
            {
                if (employeePerformance.IsApproved)
                {
                    approval = "Submitted";
                }
                else
                {
                    approval = "Not Submitted";
                }

                var empMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformance.EmployeePerformanceId).ToList();

                foreach (var em in empMeasurments)
                {
                    var score = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId);

                    if (score.Any())
                    {
                        result = result + score.Sum(x => x.Score);
                    }
                }
            }

            var ev = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == evaluationCriteriaId);
            if (ev != null)
            {
                var eval = new
                {
                    Weight = ev.DefnPercentageofWeight,
                    Result = result,
                    Approval = approval
                };

                return Json(eval);
            }

            var eval1 = new
            {
                Weight = 0,
                Result = result,
                Approval = approval
            };

            return Json(eval1);
        }
        public int GenerateEmployeeForInternalCustomerPerofmanceEvaluation(int empId, int fiscalYearId, int periodId, int evaluationCriteriaId)
        {

            var Performance = db.EmployeePerformances
                .Where(x => x.GlFiscalYearId == fiscalYearId
                        && x.EmployeeId == empId
                        && x.EvaluationCriteriaTypeId == evaluationCriteriaId
                        && x.GlPeriodId == periodId).FirstOrDefault();
            var category = db.MeasurementDefinitions.Find(evaluationCriteriaId);
            
            var placement = db.Placements.Where(x => x.isCurrent)
                                         .Where(x => x.EmployeeId == empId)
                                         .FirstOrDefault();

            if (Performance == null && placement != null)
            {
                var entity = new EmployeePerformance
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
                    CriteriaCategoryId = category != null ? category.CriteriaCategoryId : 0,
                    DirectBossId = placement.HeadId != null ? (int)placement.HeadId : 0,
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
                    IsAcceptedByEmployee = false,
                    IsApproved = false,
                    IsSecondLevelApproved = false,
                    IsImmediateSuppurvisorApproved = false,
                    IsSectorApplied = false,
                    IsCorporateApplied = false,
                    IsHRApproved = false,
                    IsHRLevelEvalution = false,
                    IsRejectedByEmployee = false,
                    IsSecondLevelRejected = false,
                    Grade = null,
                    ReasonFor2ndRejection = null,
                    Weight = 0,
                    Result = 0,
                };

                db.EmployeePerformances.Add(entity);
                db.SaveChanges();
                Performance.EmployeePerformanceId = entity.EmployeePerformanceId;

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
                // Update weight
                entity.Weight = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == entity.EmployeePerformanceId).Sum(x => x.Weight);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return Performance.EmployeePerformanceId;

            }

            else if (Performance != null && placement != null)
            {

                Performance.BusinessUnitId = placement.BusinessUnitId;
                Performance.OrgStructureId = placement.OrgStructureId;
                Performance.BranchId = placement.BranchId;
                Performance.JobTitleId = placement.LookupId;
                Performance.IsCurrent = placement.isCurrent;
                //  Performance.DirectBossId = placement.HeadId != null ? (int)placement.HeadId : 0,
                Performance.DirectBossId = placement.HeadId ?? 0;
                Performance.ResponsibilityLevel = placement.ResponsibilityLevel;
                Performance.Result = Performance.Result;
                Performance.ModifiedBy = User.Identity.Name;
                Performance.LastModified = DateTime.Now;
                db.Entry(Performance).State = EntityState.Modified;
                db.SaveChanges();
                var measurements = db.Measurements.Where(x => x.MeasurementDefinitionId == Performance.EvaluationCriteriaTypeId).ToList();
                foreach (var meas in measurements)
                {
                    var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == Performance.EmployeePerformanceId);

                    if (checkExit == null)
                    {
                        var empMeasure = new EmployeeMeasurement();

                        empMeasure.MeasurmentId = meas.MeasurementId;
                        empMeasure.EmployeePerformanceId = Performance.EmployeePerformanceId;
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
                    else
                    {
                        checkExit.MeasurmentId = meas.MeasurementId;
                        checkExit.EmployeePerformanceId = Performance.EmployeePerformanceId;
                        checkExit.Weight = meas.MesPercentageofWeight;
                        checkExit.Result = checkExit.Result;
                        db.Entry(checkExit).State = EntityState.Modified;
                        db.SaveChanges();
                        var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId == checkExit.MeasurmentId).ToList();

                        foreach (var per in performanceActions)
                        {
                            var check = db.EmployeePerformanceScores.FirstOrDefault(x => x.PerformanceActionId == per.ActionId && x.EmployeeMeasurementId == checkExit.EmpMeasurementId);
                            if (check == null)
                            {
                                var performanceActionScore = new EmployeePerformanceScore
                                {
                                    PerformanceActionId = per.ActionId,
                                    EmployeeMeasurementId = checkExit.EmpMeasurementId,
                                    Weight = per.AcPercentageofWeight,
                                    Score = 0,
                                    Remark = "",
                                };

                                db.EmployeePerformanceScores.Add(performanceActionScore);
                                db.SaveChanges();
                            }
                            else
                            {
                                check.Weight = per.AcPercentageofWeight;
                                check.Score = check.Score;
                                db.Entry(check).State = EntityState.Modified;
                                db.SaveChanges();

                            }

                        }

                    }


                }
                db.SaveChanges();
                return Performance.EmployeePerformanceId;

            }

            return 0;
        }

        //public int GenerateEmployeeForInternalCustomerPerofmanceEvaluation(int empId, int fiscalYearId, int periodId, int evaluationCriteriaId)
        //{
        //    var empPerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == empId && x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.EvaluationCriteriaTypeId == evaluationCriteriaId);
        //    if (empPerformance == null)
        //    {
        //        var entity = new EmployeePerformance
        //        {
        //            EmployeeId = empId,
        //            GlFiscalYearId = fiscalYearId,
        //            GlPeriodId = periodId,
        //            EvaluationCriteriaTypeId = evaluationCriteriaId,
        //            Weight = 0,
        //            Result = 0
        //        };

        //        db.EmployeePerformances.Add(entity);
        //        db.SaveChanges();

        //        var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == entity.EvaluationCriteriaTypeId).ToList();

        //        foreach (var meas in measurments)
        //        {
        //            var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == entity.EmployeePerformanceId);

        //            if (checkExit == null)
        //            {
        //                var empMeasure = new EmployeeMeasurement();

        //                empMeasure.MeasurmentId = meas.MeasurementId;
        //                empMeasure.EmployeePerformanceId = entity.EmployeePerformanceId;
        //                empMeasure.Weight = meas.MesPercentageofWeight;
        //                empMeasure.Result = 0;

        //                db.EmployeeMeasurement.Add(empMeasure);
        //                db.SaveChanges();

        //                var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId == empMeasure.MeasurmentId).ToList();

        //                foreach (var per in performanceActions)
        //                {
        //                    var performanceActionScore = new EmployeePerformanceScore
        //                    {
        //                        PerformanceActionId = per.ActionId,
        //                        EmployeeMeasurementId = empMeasure.EmpMeasurementId,
        //                        Weight = per.AcPercentageofWeight,
        //                        Score = 0,
        //                        Remark = "",
        //                    };

        //                    db.EmployeePerformanceScores.Add(performanceActionScore);
        //                    db.SaveChanges();
        //                }
        //            }
        //        }

        //        return entity.EmployeePerformanceId;
        //        //var returnvalue = new
        //        //{
        //        //    EmployeePerformanceId = entity.EmployeePerformanceId
        //        //};

        //        //return Json(returnvalue, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return empPerformance.EmployeePerformanceId;
        //        //var returnvalue1 = new
        //        //{
        //        //    EmployeePerformanceId = empPerformance.EmployeePerformanceId
        //        //};

        //        //return Json(returnvalue1, JsonRequestBehavior.AllowGet);
        //    }

        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

