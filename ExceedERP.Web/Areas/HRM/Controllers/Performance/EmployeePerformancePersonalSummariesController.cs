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
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HREmployeeSelfService)]
    public class EmployeePerformancePersonalSummariesController : BaseController
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
      
        private decimal GetWeight(int employeePerformanceId)
        {
            return db.EmployeePerformanceScores
                     .Where(x => db.EmployeeMeasurement
                                   .Any(em => em.EmpMeasurementId == x.EmployeeMeasurementId && em.EmployeePerformanceId == employeePerformanceId))
                     .Sum(x => (decimal?)x.Weight) ?? 0;
        }

        private decimal GetEvaluationResult(int employeePerformanceId)
        {
            return db.EmployeePerformanceScores
                     .Where(x => db.EmployeeMeasurement
                                   .Any(em => em.EmpMeasurementId == x.EmployeeMeasurementId && em.EmployeePerformanceId == employeePerformanceId))
                     .Sum(x => (decimal?)x.Score) ?? 0;
        }


        public ActionResult EmployeePerformancePersonal_Read([DataSourceRequest] DataSourceRequest request,int empId)
        {

            var query = from ep in db.EmployeePerformances
                        where ep.EmployeeId == empId &&
                              ep.IsHRApproved &&
                              !ep.IsPeerAndInternalCustomerHiddenToEmployee
                        orderby ep.GlFiscalYearId descending
                        select new
                        {
                            ep.EmployeePerformanceId,
                            ep.EmployeeId,
                            ep.GlFiscalYearId,
                            ep.GlPeriodId,
                            ep.IsApproved,
                            ep.OrgStructureId,
                            ep.BranchId,
                            ep.BusinessUnitId,
                            ep.EvaluationCriteriaTypeId,
                            ep.Date,
                            ep.DeadLine,
                            ep.IsHRApproved,
                            ep.IsAcceptedByEmployee,
                            ep.IsRejectedByEmployee,
                            ep.ReasonFor2ndRejection,
                            ep.ReasonForHRRejection,
                            ep.EmployeeRemark,
                            ep.Remark,
                            Result = db.EmployeePerformanceScores
                                       .Where(x => db.EmployeeMeasurement
                                                     .Any(em => em.EmpMeasurementId == x.EmployeeMeasurementId &&
                                                               em.EmployeePerformanceId == ep.EmployeePerformanceId))
                                       .Sum(x => (decimal?)x.Score) ?? 0,
                            Weight = db.EmployeePerformanceScores
                                       .Where(x => db.EmployeeMeasurement
                                                     .Any(em => em.EmpMeasurementId == x.EmployeeMeasurementId &&
                                                               em.EmployeePerformanceId == ep.EmployeePerformanceId))
                                       .Sum(x => (decimal?)x.Weight) ?? 0
                        };

            var result = query.ToDataSourceResult(request);
            return Json(result);
        }
        public ActionResult EmployeePerformancePersonalSummaries_Read([DataSourceRequest] DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var query = from ep in db.EmployeePerformances
                        where ep.EmployeeId == employeeId &&
                              ep.IsApproved &&
                              !ep.IsPeerAndInternalCustomerHiddenToEmployee
                        orderby ep.GlFiscalYearId descending
                        select new
                        {
                            ep.EmployeePerformanceId,
                            ep.EmployeeId,
                            ep.GlFiscalYearId,
                            ep.GlPeriodId,
                            ep.IsApproved,
                            ep.OrgStructureId,
                            ep.BranchId,
                            ep.BusinessUnitId,
                            ep.EvaluationCriteriaTypeId,
                            ep.Date,
                            ep.DeadLine,
                            ep.IsHRApproved,
                            ep.IsAcceptedByEmployee,
                            ep.IsRejectedByEmployee,
                            ep.ReasonFor2ndRejection,
                            ep.ReasonForHRRejection,
                            ep.EmployeeRemark,
                            ep.Remark,
                            Result = db.EmployeePerformanceScores
                                       .Where(x => db.EmployeeMeasurement
                                                     .Any(em => em.EmpMeasurementId == x.EmployeeMeasurementId &&
                                                               em.EmployeePerformanceId == ep.EmployeePerformanceId))
                                       .Sum(x => (decimal?)x.Score) ?? 0,
                            Weight = db.EmployeePerformanceScores
                                       .Where(x => db.EmployeeMeasurement
                                                     .Any(em => em.EmpMeasurementId == x.EmployeeMeasurementId &&
                                                               em.EmployeePerformanceId == ep.EmployeePerformanceId))
                                       .Sum(x => (decimal?)x.Weight) ?? 0
                        };

            var result = query.ToDataSourceResult(request);
            return Json(result);
        }
        public ActionResult EmployeePerformanceSummaries_OnePerson_Read([DataSourceRequest] DataSourceRequest request,int employeeId,int fiscalyearId, int quarterId)
        {
           
            var employeePerformances = db.EmployeePerformances.Where(x => !x.IsPeerAndInternalCustomerHiddenToEmployee)
                                                              .Where(x => x.EmployeeId==employeeId
                                                                          &&x.GlFiscalYearId==fiscalyearId
                                                                          &&x.GlPeriodId==quarterId
                                                                          &&x.IsApproved).ToList();     
            DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
            {
                EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
                EmployeeId = employeePerformance.EmployeeId,
                GlFiscalYearId = employeePerformance.GlFiscalYearId,
                GlPeriodId = employeePerformance.GlPeriodId,
                IsApproved = employeePerformance.IsApproved,
                DeadLine = employeePerformance.DeadLine,
                ReasonFor2ndRejection=employeePerformance.ReasonFor2ndRejection,
                ReasonForHRRejection=employeePerformance.ReasonForHRRejection,
                EmployeeRemark=employeePerformance.EmployeeRemark,
                EvaluationCriteriaTypeId = employeePerformance.EvaluationCriteriaTypeId,
                IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
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
                    Comment = measur.Comment,
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
                 //UserHelper.OperationLog(Request.UserHostAddress, "Employee Performance  ", "Success", User.Identity.Name, User.Identity.Name + " update Employee performance of employee id " + entity.EmployeeId, Modules.HRM);
                #endregion

            }

            return Json(new[] { employeePerformanceScore }.ToDataSourceResult(request, ModelState));
        }
        ///////////////
        
        public JsonResult GetFiscalYear()
        {

            var fiscalYear = db.GLFiscalYears.OrderByDescending(x=>x.GlFiscalYearId).ToList();

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
        //public string AcceptedEvaluation(int year, int period, int employee,string reason)
        public string AcceptedEvaluation(int employeePerformanceId, string reason)
        {
            var employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeePerformanceId == employeePerformanceId &&x.IsApproved);
            var status = "false";
            if (employeePerformance !=null )
            {
                employeePerformance.IsAcceptedByEmployee = true;
                employeePerformance.EmployeeRemark = reason;
                db.Entry(employeePerformance).State = EntityState.Modified;
                db.SaveChanges();

                status= "true";
            }

            return status;
        }
        //public string RejectionReason_Create(int yearId, int periodId, int employeeId,string reason)
        public string RejectionReason_Create(int employeePerformanceId, string reason)
        {
            var employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeePerformanceId == employeePerformanceId&&x.IsApproved);
            var status = "false";
            if(employeePerformance !=null)
            {
                employeePerformance.IsRejectedByEmployee = true;
                employeePerformance.EmployeeRemark = reason;

                db.Entry(employeePerformance).State = EntityState.Modified;
                db.SaveChanges();

                status= "true";
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

