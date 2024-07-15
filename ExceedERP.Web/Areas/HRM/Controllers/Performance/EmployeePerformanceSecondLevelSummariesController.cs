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
using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,
                      HrmConstants.HRPerformanceUser,
                      HrmConstants.HRDepartmentHead)]
    public class EmployeePerformanceSecondLevelSummariesController : BaseController
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
            ViewData["Measurments"] = db.Measurements.ToList();
            ViewData["PerformanceActions"] = db.PerformanceActions.ToList();
            ViewData["EmployeeList"] = db.Person_Employee.Select(x => new {
                x.EmployeeId,
                FullNameAmharic = x.EmployeeCode + "-" + x.FirstName + " " + x.MiddleName + " " + x.LastName
            }).AsQueryable();
            // var list = inputs.GetEmployees(User.Identity.Name);

            ViewData["Period"] = db.Quarters.Select(x=>new { x.QuarterId,x.Name}).ToList();
            var  measurments = db.MeasurementDefinitions.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " /*+ GetEvaluationTypeName(p.LookupId) + " - " */+ p.Role.ToString()
            }).ToList();
            ViewData["EvalutionCrateria"] = measurments;
            ViewData["branch"] = db.Branch.ToList();

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

        public ActionResult EmployeePerformanceSecondLevelSummaries_Read([DataSourceRequest] DataSourceRequest request)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var subordinates = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(id);
            var subordinates = EmployeeHelper.GetSubordinatesOfSubortinates_FilteredByHead(id);
            var employeePerformances = db.EmployeePerformances.Where(x=> subordinates.Contains(x.DirectBossId)||subordinates.Contains(x.EmployeeId)||subordinates.Contains((int)x.SecondLevelApproverId))
                                .Where(x => x.IsApproved &&x.IsImmediateSuppurvisorApproved&&x.IsAcceptedByEmployee||x.IsRejectedByEmployee).DistinctBy(x => new
                                {
                                    x.EmployeeId,
                                    x.GlFiscalYearId,
                                    x.GlPeriodId
                                }).OrderByDescending(x => x.GlFiscalYearId).ToList();
            DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
            {
                EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
                EmployeeId = employeePerformance.EmployeeId,
                DirectBossId = employeePerformance.DirectBossId,
                GlFiscalYearId = employeePerformance.GlFiscalYearId,
                GlPeriodId = employeePerformance.GlPeriodId,
                OrgStructureId = employeePerformance.OrgStructureId,
                BranchId = employeePerformance.BranchId,
                BusinessUnitId = employeePerformance.BusinessUnitId,
                IsCurrent = employeePerformance.IsCurrent,
                IsHRApproved = employeePerformance.IsHRApproved,
                IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
                IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
                ReasonFor2ndRejection = employeePerformance.ReasonFor2ndRejection,
                Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
                Weight = GetWeight(employeePerformance.EmployeePerformanceId),
                //Result = employeePerformance.Result,
                ////Result = GetResult(employeePerformance.EmpMeasurementId),
                //Remark = employeePerformance.Remark
            });

            return Json(result);
        }
        public ActionResult EmployeePerformanceSecondLevelSummaries_OnePerson_Read([DataSourceRequest] DataSourceRequest request,int employeeId,int fiscalyearId, int quarterId)
        {
            //List<int> employees = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
            //var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var subordinates = EmployeeHelper.GetSubordinates(id);
            var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId==employeeId&&x.GlFiscalYearId==fiscalyearId&&x.GlPeriodId==quarterId&x.IsApproved&&(x.IsAcceptedByEmployee||x.IsRejectedByEmployee)&&x.IsImmediateSuppurvisorApproved).ToList();     
            //IQueryable<EmployeePerformance> employeePerformances = db.EmployeePerformances;
            DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
            {
                EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
                EmployeeId = employeePerformance.EmployeeId,
                DirectBossId = employeePerformance.DirectBossId,
                GlFiscalYearId = employeePerformance.GlFiscalYearId,
                GlPeriodId = employeePerformance.GlPeriodId,
                OrgStructureId = employeePerformance.OrgStructureId,
                BranchId = employeePerformance.BranchId,
                BusinessUnitId = employeePerformance.BusinessUnitId,
                IsCurrent = employeePerformance.IsCurrent,
                EvaluationCriteriaTypeId = employeePerformance.EvaluationCriteriaTypeId,
                ReasonFor2ndRejection = employeePerformance.ReasonFor2ndRejection,
                IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
                //Weight = employeePerformance.Weight,
                //Result = employeePerformance.Result,
                Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
                Weight = GetWeight(employeePerformance.EmployeePerformanceId),
                Remark = employeePerformance.Remark
            });

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
                    Comment = measur.Comment,
                    Weight = measur.Weight,
                    Score = measur.Score,
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

            
            var before = db.EmployeePerformanceScores.Find(employeePerformanceScore.EmployeePerformanceScoreId);
            if(before != null)
            {
                before.Comment += "The previeus result was " + before.Score.ToString() + " " + employeePerformanceScore.Comment;
                before.Score = employeePerformanceScore.Score;
                before.Remark = employeePerformanceScore.Remark;
                db.Entry(before).State = EntityState.Modified;
                db.SaveChanges();
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
                    Comment = employeePerformanceScore.Comment,
                    Remark = employeePerformanceScore.Remark,
                };

                db.EmployeePerformanceScores.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                 UserHelper.OperationLog(Request.UserHostAddress, "Employee Performance Score ", "Success", User.Identity.Name, User.Identity.Name + "Update Employee Performance Score" + entity.EmployeePerformanceScoreId, Modules.HRM);

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
        public string ApproveEvaluation(int year, int period, int employee)
        {
            var employeePerformances = db.EmployeePerformances.Where(x => x.GlFiscalYearId == year && x.GlPeriodId == period && x.EmployeeId == employee&&x.IsApproved&&x.IsImmediateSuppurvisorApproved).ToList();
            var status = "false";
            foreach ( var employeePerformance in employeePerformances)
            {
                employeePerformance.IsSecondLevelApproved = true;

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
                    Title = " Suppurvisor has approved your Performance ",
                    Message = $" you have performance has been approved by your Second Level Approver",
                    URL = "/HRM/EmployeePerformanceSecondLevelSummaries/"

                };

                Notify.NotifyUser(notif);
            }
            return status;
        }
        public string RejectionReason_Create(int yearId, int periodId, int employeeId, string reason)
        {
            var employeePerformances = db.EmployeePerformances.Where(x => x.GlFiscalYearId == yearId
                                                                          && x.GlPeriodId == periodId 
                                                                          && x.EmployeeId == employeeId 
                                                                          && x.IsApproved
                                                                          &&x.IsImmediateSuppurvisorApproved 
                                                                          && !x.IsSecondLevelApproved).ToList();
            var status = "false";
            foreach (var employeePerformance in employeePerformances)
            {
                employeePerformance.IsSecondLevelRejected = true;
                employeePerformance.IsSecondLevelApproved = false;
                employeePerformance.ReasonFor2ndRejection = reason;

                db.Entry(employeePerformance).State = EntityState.Modified;
                db.SaveChanges();
                status = "true";

            }

            return status;
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

            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);

            var placements = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent).ToList();
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

