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

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,
                    HrmConstants.HRDepartmentHead, 
                    HrmConstants.HRPerformanceUser, 
                    HrmConstants.HRPerformanceReport)]

    public class EmployeePerformanceSummariesController : BaseController
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
            

            ViewData["Period"] = db.Quarters.Select(x=>new { x.QuarterId,x.Name}).ToList();
            var  measurments = db.MeasurementDefinitions.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + p.Lookup.Description + " - " + p.Role.ToString() + " " + p.DefnPercentageofWeight
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
        //public decimal GetEvaluationWeightandResult(int id)
        //{
        //    decimal result = 0;


        //    var empMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == id).ToList();
        //    foreach (var em in empMeasurments)
        //    {
        //        var weighAndResult =db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();
        //        if (weighAndResult.Count() > 0)
        //        {

        //            result += weighAndResult.Sum(x => x.Score);

        //        }
        //        else
        //        {
        //            result += 0;
        //        }



        //    }



        //    return result;
        //}
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

        public ActionResult EmployeePerformanceSummaries_Read([DataSourceRequest] DataSourceRequest request)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
           // var subordinates = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(id);
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);

            var employeePerformances = db.EmployeePerformances
                                .Where(x=>x.IsAcceptedByEmployee||x.IsRejectedByEmployee)
                                .Where(x => x.IsApproved 
                                && subordinates.Contains(x.EmployeeId) 
                                &&x.EmployeeId!=id).DistinctBy(x => new
                                {
                                    x.EmployeeId,
                                    x.GlFiscalYearId,
                                    x.GlPeriodId
                                }).OrderByDescending(x => x.GlFiscalYearId).ToList();
            DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
            {
                EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
                EmployeeId = employeePerformance.EmployeeId,
                GlFiscalYearId = employeePerformance.GlFiscalYearId,
                GlPeriodId = employeePerformance.GlPeriodId,
                OrgStructureId = employeePerformance.OrgStructureId,
                IsCurrent = employeePerformance.IsCurrent,
                BranchId = employeePerformance.BranchId,
                BusinessUnitId = employeePerformance.BusinessUnitId,
                Date = employeePerformance.Date,
                DateCreated = employeePerformance.DateCreated,
                CreatedBy = employeePerformance.CreatedBy,
                ModifiedBy = employeePerformance.ModifiedBy,
                LastModified = employeePerformance.LastModified,
                IsApproved = employeePerformance.IsApproved,
                IsRejectedByEmployee = employeePerformance.IsRejectedByEmployee,
                IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
                IsImmediateSuppurvisorApproved = GetHRLevelApprovals(employeePerformance.EmployeeId, employeePerformance.GlFiscalYearId, employeePerformance.GlPeriodId),
                IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
                IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
                ReasonFor2ndRejection = employeePerformance.ReasonFor2ndRejection,
                ImmediateSuppurvisorApprovalDate = employeePerformance.ImmediateSuppurvisorApprovalDate,
                SecondLevelApprovalDate = employeePerformance.SecondLevelApprovalDate,
                FinalApprovalDate = employeePerformance.FinalApprovalDate,
                EvalutionType = employeePerformance.EvalutionType,
                //IsImmediateSuppurvisorApproved = employeePerformance.IsImmediateSuppurvisorApproved,
                Weight = GetWeight(employeePerformance.EmployeePerformanceId),
                Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
                //AreAllHRLevelApproved = ,
                Remark = employeePerformance.EmployeeRemark
            });

            return Json(result);
        }
        public ActionResult EmployeePerformanceSummaries_OnePerson_Read([DataSourceRequest] DataSourceRequest request,int employeeId,int fiscalyearId, int quarterId)
        {
            //List<int> employees = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var subordinates = EmployeeHelper.GetSubordinates(id);
            var employeePerformances = db.EmployeePerformances
                                                    .Where(x => x.EmployeeId == employeeId 
                                                    && x.GlFiscalYearId == fiscalyearId 
                                                    && x.GlPeriodId == quarterId 
                                                    && x.IsApproved 
                                                    && (x.IsAcceptedByEmployee || x.IsRejectedByEmployee)
                                                  ).ToList();

            //var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId==employeeId
            //                                                            &&x.GlFiscalYearId==fiscalyearId
            //                                                            &&x.GlPeriodId==quarterId
            //                                                            &&(x.IsRejectedByEmployee||x.IsRejectedByEmployee)
            //                                                            &&x.IsApproved).ToList();     
            //IQueryable<EmployeePerformance> employeePerformances = db.EmployeePerformances;
            DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
            {
                EmployeePerformanceId = employeePerformance.EmployeePerformanceId,
                EmployeeId = employeePerformance.EmployeeId,
                GlFiscalYearId = employeePerformance.GlFiscalYearId,
                GlPeriodId = employeePerformance.GlPeriodId,
                EvaluationCriteriaTypeId = employeePerformance.EvaluationCriteriaTypeId,
                BusinessUnitId = employeePerformance.BusinessUnitId,
                OrgStructureId = employeePerformance.OrgStructureId,
                ResponsibilityLevel = employeePerformance.ResponsibilityLevel,
                BranchId = employeePerformance.BranchId,
                JobTitleId = employeePerformance.JobTitleId,
                IsCurrent = employeePerformance.IsCurrent,
                IsHRLevelEvalution = employeePerformance.IsHRLevelEvalution,
                // Result = employeePerformance.Result,
                //Weight = employeePerformance.Weight,
                EvalutionType = employeePerformance.EvalutionType,
                Date = employeePerformance.Date,
                DateCreated = employeePerformance.DateCreated,
                ImmediateSuppurvisorApprovalDate = employeePerformance.ImmediateSuppurvisorApprovalDate,
                SecondLevelApprovalDate = employeePerformance.SecondLevelApprovalDate,
                FinalApprovalDate = employeePerformance.FinalApprovalDate,
                Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
                Weight = GetWeight(employeePerformance.EmployeePerformanceId),              
                IsApproved = employeePerformance.IsApproved,
                IsRejectedByEmployee = employeePerformance.IsRejectedByEmployee,
                IsAcceptedByEmployee = employeePerformance.IsAcceptedByEmployee,
                IsImmediateSuppurvisorApproved = employeePerformance.IsImmediateSuppurvisorApproved,
                IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
                IsSecondLevelRejected = employeePerformance.IsSecondLevelRejected,
                ReasonFor2ndRejection = employeePerformance.ReasonFor2ndRejection,
                
                Remark = employeePerformance.EmployeeRemark
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
        private void AveragingEvaluationCopyyy(IList<EmployeePerformance> employeePerformances)
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

            //// Step 6: Save the Averaged EmployeeMeasurement Records
            //var masterId = employeeMeasurements.First().EmployeePerformanceId;
            //var measurements = employeeMeasurements.Where(x => x.EmployeePerformanceId == masterId).ToList();
            //foreach (var meas in measurements)
            //{

            //    var empMeasure = new EmployeeMeasurement
            //    {
            //        MeasurmentId = meas.MeasurmentId,
            //        EmployeePerformanceId = entity.EmployeePerformanceId,
            //        Weight = meas.Weight,
            //        Result = meas.Result
            //    };

            //    db.EmployeeMeasurement.Add(empMeasure);
            //    db.SaveChanges();
            //    var PerformanceScores = employeePerformanceScores.Where(x => x.EmployeeMeasurementId == meas.EmpMeasurementId);
            //    // Step 7: Save the Averaged EmployeePerformanceScores
            //    //foreach (var avgScore in avgScores.Where(x => x.PerformanceActionId == meas.MeasurmentId))
            //    foreach (var avgScore in PerformanceScores)
            //    {
            //        var PerformanceScoresResults = employeePerformanceScores.Where(x => x.Weight == avgScore.Weight).ToList();

            //        var performanceActionScore = new EmployeePerformanceScore
            //        {
            //            PerformanceActionId = avgScore.PerformanceActionId,
            //            EmployeeMeasurementId = empMeasure.EmpMeasurementId,
            //            Weight = avgScore.Weight,
            //            Score = PerformanceScoresResults.Any()? PerformanceScoresResults.Average(x=>x.Score):0,
            //            Remark = "Averaged Score"
            //        };

            //        db.EmployeePerformanceScores.Add(performanceActionScore);
            //    }
            //}
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
        public string ApproveEvaluation(int year, int period, int employee,int empPerformanceId)
        {
            var status = "false";
            var performance = db.EmployeePeerToPeerAndInternalCustomerPerformanceSettings.FirstOrDefault();
            var employeePerformances = db.EmployeePerformances
                                               .Where(x => x.GlFiscalYearId == year
                                                   && x.GlPeriodId == period
                                                   && x.EmployeeId == employee
                                                   && x.IsApproved
                                                   && (x.IsAcceptedByEmployee
                                                       || x.IsRejectedByEmployee)
                                                   && !x.IsImmediateSuppurvisorApproved)
                                                   .ToList();
            if (performance !=null 
                && (performance.IsInternalCustomerHasMultipleEvaluator
                || performance.IsPeerToPeerHasMultipleEvaluator))
            {
                var employeePeerToPeerPerformances = db.EmployeePerformances
                                                          .Where(x => x.GlFiscalYearId == year
                                                                && x.GlPeriodId == period
                                                                && x.EmployeeId == employee
                                                                && x.EvalutionType == "PeerToPeer"
                                                                && x.IsApproved
                                                                && !x.IsAveraged
                                                                && (x.IsAcceptedByEmployee
                                                                    || x.IsRejectedByEmployee)
                                                                && !x.IsImmediateSuppurvisorApproved)
                                                                .ToList();
                if(employeePeerToPeerPerformances.Any()&& employeePeerToPeerPerformances.Count()>1)
                    AveragingEvaluationCopyyy(employeePeerToPeerPerformances);
                    //AveragingEvaluation(employeePeerToPeerPerformances);

                var employeeInternalCustomerPerformances = db.EmployeePerformances
                                                        .Where(x => x.GlFiscalYearId == year
                                                                     && x.GlPeriodId == period
                                                                     && x.EmployeeId == employee
                                                                     && x.EvalutionType == "InternalCustomer"
                                                                     && x.IsApproved
                                                                     && !x.IsAveraged
                                                                     && (x.IsAcceptedByEmployee
                                                                         || x.IsRejectedByEmployee)
                                                                     && !x.IsImmediateSuppurvisorApproved)
                                                                     .ToList();

                if(employeeInternalCustomerPerformances.Any()&& employeeInternalCustomerPerformances.Count()>1)
                    AveragingEvaluationCopyyy(employeeInternalCustomerPerformances);
                    //AveragingEvaluation(employeeInternalCustomerPerformances);

                employeePerformances = employeePerformances.ToList();
            }
            
            if (db.EmployeePerformances
                        .Where(x => x.GlFiscalYearId == year 
                                     && x.GlPeriodId == period 
                                     && x.EmployeeId == employee 
                                     && x.IsApproved 
                                     && !x.IsAcceptedByEmployee
                                     && !x.IsRejectedByEmployee)
                                     .Any())
            {
                return status;
            }
            var employeePerformancess = db.EmployeePerformances
                                                        .Where(x => x.GlFiscalYearId == year
                                                                     && x.GlPeriodId == period
                                                                     && x.EmployeeId == employee
                                                                     && x.IsApproved
                                                                     && !x.IsAveraged
                                                                     && (x.IsAcceptedByEmployee
                                                                         || x.IsRejectedByEmployee)
                                                                     && !x.IsImmediateSuppurvisorApproved)
                                                                     .ToList();
            foreach ( var employeePerformance in employeePerformancess)
            {
               
                employeePerformance.IsImmediateSuppurvisorApproved = true;
                employeePerformance.ImmediateSuppurvisorApprovalDate = DateTime.Now;
                employeePerformance.ImmediateSuppurvisorApprover = User.Identity.Name;
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
                    Title = "Immediate Suppurvisor has approved your Performance ",
                    Message = $" you have performance has been approved by your Immediate Suppurvisor",
                    URL = "/HRM/EmployeePerformanceSummaries/"
                };

                Notify.NotifyUser(notif);
            }
            return status;
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

