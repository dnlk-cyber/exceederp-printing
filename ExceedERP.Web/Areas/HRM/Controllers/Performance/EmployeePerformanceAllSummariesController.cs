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
            ViewData["EmployeeList"] = db.Person_Employee.Select(x=>new { x.EmployeeId,
                              FullNameAmharic = x.EmployeeCode+"-"+x.FirstName+" "+x.MiddleName+" "+x.LastName}).AsQueryable();
            var branches = db.Branch.Select(x => new { x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            var workunits = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
            ViewData["workunits"] = workunits;
            ViewData["Period"] = db.Quarters.Select(x=>new { x.QuarterId,x.Name}).ToList();
            var  measurments = db.MeasurementDefinitions.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + p.Lookup.Description + " - " + p.Role.ToString() + " " + p.DefnPercentageofWeight

            }).ToList();
            ViewData["EvalutionCrateria"] = measurments;
           // ViewData["branch"] = db.Branch.ToList();
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
        private bool GetHRLevelApprovals(int empId, int fiscalyearId, int periodId)
        {
            var records = db.EmployeePerformances
                .Where(x => x.EmployeeId == empId && x.GlFiscalYearId == fiscalyearId && x.GlPeriodId == periodId&&x.IsSecondLevelApproved);

            // Return true only if there are matching records and all are approved
            return records.Any() && records.All(x => x.IsHRApproved);
        }
        //public ActionResult EmployeePerformanceAllSummaries_Read([DataSourceRequest] DataSourceRequest request)
        //{

        //    var employeePerformances = db.EmployeePerformances
        //                         .Where(x => x.IsSecondLevelApproved)
        //                         .DistinctBy(x => new
        //                         {
        //                             x.EmployeeId,
        //                             x.GlFiscalYearId,
        //                             x.GlPeriodId,
        //                         })
        //                         .OrderByDescending(x => x.GlFiscalYearId)
        //                         .ToList();

        //    // Determine if all `IsHRApproved` are true for each grouped record
        //    var data = employeePerformances.GroupBy(performance => new
        //    {
        //        performance.EmployeeId,
        //        performance.GlFiscalYearId,
        //        performance.GlPeriodId
        //    })
        //    .Select(group => new
        //    {
        //        EmployeePerformanceId = group.First().EmployeePerformanceId,
        //        EmployeeId = group.Key.EmployeeId,
        //        GlFiscalYearId = group.Key.GlFiscalYearId,
        //        GlPeriodId = group.Key.GlPeriodId,
        //        BusinessUnitId = group.First().BusinessUnitId,
        //        OrgStructureId = group.First().OrgStructureId,
        //        ResponsibilityLevel = group.First().ResponsibilityLevel,
        //        BranchId = group.First().BranchId,
        //        IsAcceptedByEmployee = group.All(x => x.IsAcceptedByEmployee),
        //        IsImmediateSuppurvisorApproved = group.All(x => x.IsImmediateSuppurvisorApproved),
        //        IsSecondLevelRejected = group.All(x => x.IsSecondLevelRejected),
        //        IsApproved = group.All(x => x.IsApproved),
        //        IsHRApproved = group.All(x => x.IsHRApproved), // True only if all are true
        //        IsSecondLevelApproved = group.All(x => x.IsSecondLevelApproved),
        //        IsCurrent = group.All(x => x.IsCurrent),
        //        Result = GetEvaluationWeightandResult(group.First().EmployeePerformanceId),
        //        Weight = GetWeight(group.First().EmployeePerformanceId),
        //        AreAllHRLevelApproved = GetHRLevelApprovals(group.Key.EmployeeId, group.Key.GlFiscalYearId, group.Key.GlPeriodId),
        //    }).ToList();

        //    // Convert to DataSourceResult
        //    DataSourceResult result = data.ToDataSourceResult(request);

        //    return Json(result);
        //}
        public ActionResult EmployeePerformanceAllSummaries_Read([DataSourceRequest] DataSourceRequest request)
        {

            var employeePerformances = db.EmployeePerformances
                                .Where(x => x.IsSecondLevelApproved).DistinctBy(x => new
                                {
                                    x.EmployeeId,
                                    x.GlFiscalYearId,
                                    x.GlPeriodId,

                                }).OrderByDescending(x => x.GlFiscalYearId).ToList();

            DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
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
                IsHRApproved = employeePerformance.IsHRApproved,
                IsHRRejected = employeePerformance.IsHRRejected,
                IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
                IsCurrent = employeePerformance.IsCurrent,
                Result = GetEvaluationWeightandResult(employeePerformance.EmployeePerformanceId),
                Weight = GetWeight(employeePerformance.EmployeePerformanceId),
                AreAllHRLevelApproved = GetHRLevelApprovals(employeePerformance.EmployeeId, employeePerformance.GlFiscalYearId, employeePerformance.GlPeriodId),
                //Result = employeePerformance.Result,
                //Result = GetResult(employeePerformance.EmpMeasurementId),
                Remark = employeePerformance.Remark
            });

            return Json(result);
        }
        public ActionResult EmployeePerformanceSummaries_OnePerson_Read([DataSourceRequest] DataSourceRequest request,int employeeId,int fiscalyearId, int quarterId)
        {
            List<int> employees = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
            //var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var subordinates = EmployeeHelper.GetSubordinates(id);
            var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId==employeeId&&x.GlFiscalYearId==fiscalyearId&&x.GlPeriodId==quarterId&x.IsApproved&&x.IsAcceptedByEmployee&&x.IsSecondLevelApproved).ToList();     
            //IQueryable<EmployeePerformance> employeePerformances = db.EmployeePerformances;
            DataSourceResult result = employeePerformances.ToDataSourceResult(request, employeePerformance => new
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
                IsSecondLevelApproved = employeePerformance.IsSecondLevelApproved,
                Result = GetEvaluationWeightandResult(employeePerformance.EmployeePerformanceId),
                Weight = GetWeight(employeePerformance.EmployeePerformanceId),
                AreAllHRLevelApproved = GetHRLevelApprovals(employeePerformance.EmployeeId, employeePerformance.GlFiscalYearId, employeePerformance.GlPeriodId),

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
                var errorMessage = new
                {
                    Message = "No employee performance records found for the given criteria.",
                    Status = "Not-OK"
                };
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
            }

            // Calculate total performance weight, ensuring that GetPerformanceWeight is not null
            decimal weight = employeePerformances
                .Sum(performance => GetPerformanceWeight(performance.EmployeePerformanceId));

            // Check if the total weight is 100
            if (weight != 100)
            {
                var errorMessage = new
                {
                    Message = $"The total evaluation weight is {weight}. Please adjust the weight to sum up to 100%.",
                    Status = "Not-OK"
                };
                return Json(errorMessage, JsonRequestBehavior.AllowGet);
            }

            // Approve all evaluations in memory and save them in a single batch
            foreach (var employeePerformance in employeePerformances)
            {
                employeePerformance.IsHRApproved = true;
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
            var successMessage = new
            {
                Message = "All evaluations have been successfully approved.",
                Status = "OK"
            };
            return Json(successMessage, JsonRequestBehavior.AllowGet);
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

