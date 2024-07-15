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

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRAdmin, HrmConstants.HRPerformanceUser)]
    public class EmployeePerformanceCorporateAndSectorLevelsController : BaseController
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
                Name = p.EvaluationCriteria + " - " /*+ GetEvaluationTypeName(p.LookupId) + " - " */+ p.Role.ToString()
            }).ToList();
            ViewData["EvalutionCrateria"] = measurments;
            ViewData["jobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();

            // ViewData["branch"] = db.Branch.ToList();
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


        public ActionResult EmployeePerformanceCorporateAndSectorLevels_Read([DataSourceRequest] DataSourceRequest request)
        {
            //var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var subordinates = EmployeeHelper.GetSubordinates(id);
            //List<int> employees = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);

            //var employeePerformances = db.EmployeePerformances.Where(x=>x.IsApproved).GroupBy(x => new
            //{
            //    x.EmployeeId,
            //    x.GlFiscalYearId,
            //    x.GlPeriodId,
            //}).Select(g => new { g.Key.EmployeeId, g.Key.GlFiscalYearId, g.Key.GlPeriodId}).OrderByDescending(x=>x.GlFiscalYearId).ToList();    
            var employeePerformances = db.EmployeePerformances
                                .Where(x => x.IsSecondLevelApproved).DistinctBy(x => new
                                {
                                    x.EmployeeId,
                                    x.GlFiscalYearId,
                                    x.GlPeriodId
                                }).OrderByDescending(x => x.GlFiscalYearId).ToList();
            //IQueryable<EmployeePerformance> employeePerformances = db.EmployeePerformances;
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
                JobTitleId = employeePerformance.JobTitleId,
                IsCurrent = employeePerformance.IsCurrent,
                IsHRApproved = employeePerformance.IsHRApproved,
                IsHRLevelEvalution = employeePerformance.IsHRLevelEvalution,
                Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
                Weight = GetWeight(employeePerformance.EmployeePerformanceId),
                //Result = employeePerformance.Result,
                ////Result = GetResult(employeePerformance.EmpMeasurementId),
                //Remark = employeePerformance.Remark

            });

            return Json(result);
        }
        public ActionResult EmployeePerformanceCorporateAndSectorLevels_OnePerson_Read([DataSourceRequest] DataSourceRequest request,int employeeId,int fiscalyearId, int quarterId)
        {
            List<int> employees = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
            //var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var subordinates = EmployeeHelper.GetSubordinates(id);
            var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId==employeeId&&x.GlFiscalYearId==fiscalyearId&&x.GlPeriodId==quarterId&x.IsApproved&&(x.IsAcceptedByEmployee||x.IsRejectedByEmployee)&&x.IsSecondLevelApproved).ToList();     
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
                Result = GetEvaluationResult(employeePerformance.EmployeePerformanceId),
                Weight = GetWeight(employeePerformance.EmployeePerformanceId),
                Remark = employeePerformance.Remark
            });

            return Json(result);
        }
        //public ActionResult EmployeePerformanceCorporateAndSectorLevels_OnePerson_Destroy([DataSourceRequest] DataSourceRequest request,int employeeId,int fiscalyearId, int quarterId)
        //{
        //    List<int> employees = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
        //    //var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //    //var subordinates = EmployeeHelper.GetSubordinates(id);
        //    var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId==employeeId&&x.GlFiscalYearId==fiscalyearId&&x.GlPeriodId==quarterId&x.IsApproved&&(x.IsAcceptedByEmployee||x.IsRejectedByEmployee)&&x.IsSecondLevelApproved).ToList();     
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
        //        IsHRLevelEvalution = employeePerformance.IsHRLevelEvalution,
        //        Result = employeePerformance.Result,
        //        Weight = employeePerformance.Weight,
        //        //Result = GetEvaluationWeightandResult(employeePerformance.EmployeePerformanceId),
        //        //Weight = GetWeight(employeePerformance.EmployeePerformanceId),
        //        Remark = employeePerformance.Remark
        //    });

        //    return Json(result);
        //}

        public ActionResult EmployeePerformanceCorporateAndSectorLevels_OnePerson_Destroy([DataSourceRequest]DataSourceRequest request, EmployeePerformance employeePerformance,int employeePerformanceId)
        {
            ModelState.Clear();
            var employeePerformanceNew = db.EmployeePerformances.Find(employeePerformance.EmployeePerformanceId);
            if (employeePerformanceNew != null)
            {
                if (ModelState.IsValid)
                {
                    // Delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted performance Performance " + employeePerformance.EmployeeId);

                    // Delete related EmployeeMeasurements and EmployeePerformanceScores
                    var employeeMeasurements = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformanceNew.EmployeePerformanceId).ToList();
                    foreach (var employeeMeasurement in employeeMeasurements)
                    {
                        // Delete related EmployeePerformanceScores
                        var actions = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == employeeMeasurement.EmpMeasurementId).ToList();
                        foreach (var action in actions)
                        {
                            db.EmployeePerformanceScores.Remove(action);
                        }

                        db.EmployeeMeasurement.Remove(employeeMeasurement);
                    }

                    // Delete the EmployeePerformance entity
                    db.EmployeePerformances.Remove(employeePerformanceNew);
                    db.SaveChanges();
                }
            }

            return Json(new[] { employeePerformanceNew }.ToDataSourceResult(request, ModelState));
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
        public ActionResult PerformanceActions_Destroy([DataSourceRequest] DataSourceRequest request, EmployeePerformanceScore employeePerformanceScore)
        {
            if (employeePerformanceScore.Score > employeePerformanceScore.Weight)
            {
                ModelState.AddModelError("", "Score can't exceed the weight");
            }
            var performanceActions = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == employeePerformanceScore.EmployeeMeasurementId).ToList();
            var employeePerformaceId = performanceActions.Any() ? db.EmployeeMeasurement.Find(performanceActions.FirstOrDefault()?.EmployeeMeasurementId)?.EmployeePerformanceId : null;
            var employeePerformace = employeePerformaceId !=null? db.EmployeePerformances.Find(employeePerformaceId):null;
            var criteria = employeePerformace !=null? db.MeasurementDefinitions.Find(employeePerformace.EvaluationCriteriaTypeId):null;
            
            if (performanceActions.Count() == 1 && criteria !=null && (criteria.Lookup.EvalutionType == EvaluationType.Corporate_Result
                                                                        || criteria.Lookup.EvalutionType == EvaluationType.Companylevel
                                                                        || criteria.Lookup.EvalutionType == EvaluationType.Sector_Result
                                                                        || criteria.Lookup.EvalutionType == EvaluationType.MainDepartmentLevel
                                                                        || criteria.Lookup.EvalutionType == EvaluationType.Project_Manager))
            {
                //if (ModelState.IsValid)
                //{

                //    var entity = new EmployeePerformanceScore
                //    {
                //        EmployeePerformanceScoreId = employeePerformanceScore.EmployeePerformanceScoreId,
                //        PerformanceActionId = employeePerformanceScore.PerformanceActionId,
                //        EmployeeMeasurementId = employeePerformanceScore.EmployeeMeasurementId,
                //        Weight = employeePerformanceScore.Weight,
                //        Score = employeePerformanceScore.Score,
                //        Remark = employeePerformanceScore.Remark,
                //    };

                //    db.EmployeePerformanceScores.Attach(entity);
                //    db.EmployeePerformanceScores.Remove(entity);
                //    db.Entry(entity).State = EntityState.Modified;
                //    var employeeMeasurement = db.EmployeeMeasurement.Find(employeePerformanceScore.EmployeeMeasurementId);
                //    if(employeeMeasurement != null)
                //    {
                //        db.EmployeeMeasurement.Attach(employeeMeasurement);
                //        db.EmployeeMeasurement.Remove(employeeMeasurement);
                //        db.Entry(entity).State = EntityState.Modified;
                //        var employeePerformance = db.EmployeePerformances.Find(employeeMeasurement.EmployeePerformanceId);
                //        if(employeePerformance != null)
                //        {
                //            db.EmployeePerformances.Attach(employeePerformance);
                //            db.EmployeePerformances.Remove(employeePerformance);
                //            db.Entry(entity).State = EntityState.Modified;
                //        }


                //    }

                //    db.SaveChanges();

                //    #region operation log

                //    // UserHelper.OperationLog(Request.UserHostAddress, "Employee Performance  ", "Success", User.Identity.Name, User.Identity.Name + " update Employee performance of employee id " + entity.EmployeeId, Modules.HRM);

                //    #endregion

                //}
                if (ModelState.IsValid)
                {
                    var entity = db.EmployeePerformanceScores.Find(employeePerformanceScore.EmployeePerformanceScoreId);
                    if (entity != null)
                    {
                        // Update entity properties
                        entity.PerformanceActionId = employeePerformanceScore.PerformanceActionId;
                        entity.Weight = employeePerformanceScore.Weight;
                        entity.Score = employeePerformanceScore.Score;
                        entity.Remark = employeePerformanceScore.Remark;

                        // Set entity state to modified
                        db.Entry(entity).State = EntityState.Modified;

                        // Remove related entities if necessary
                        var employeeMeasurement = db.EmployeeMeasurement.Find(entity.EmployeeMeasurementId);
                        if (employeeMeasurement != null)
                        {
                            db.EmployeeMeasurement.Remove(employeeMeasurement);
                        }

                        var employeePerformance = db.EmployeePerformances.Find(employeeMeasurement?.EmployeePerformanceId);
                        if (employeePerformance != null)
                        {
                            db.EmployeePerformances.Remove(employeePerformance);
                        }
                        db.EmployeePerformanceScores.Remove(entity);
                        db.SaveChanges();

                        #region operation log
                        // Log operation
                        #endregion
                    }
                }
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
            var employeePerformances = db.EmployeePerformances.Where(x => x.GlFiscalYearId == year && x.GlPeriodId == period && x.EmployeeId == employee&&x.IsApproved&&x.IsAcceptedByEmployee&&x.IsSecondLevelApproved).ToList();
            var status = "false";
            foreach ( var employeePerformance in employeePerformances)
            {
                employeePerformance.IsHRApproved = true;

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
                    Title = " HR has approved your Performance ",
                    Message = $" you have performance has been approved by HR Approver",
                    URL = "/HRM/EmployeePerformanceAllSummaries/"

                };

                Notify.NotifyUser(notif);
            }
            return status;
        }
        public string CheckTheWeight(int evaluationCriteriaId, decimal resultValue)
        {
            decimal weight = 0;
               
            var empMeasurment = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == evaluationCriteriaId);
            if ( empMeasurment !=null)
            {
               
                weight = empMeasurment.DefnPercentageofWeight;
                if(resultValue > empMeasurment.DefnPercentageofWeight)
                {
                    return "Maximum weight is " + weight + " enter below " + weight;
                }
                return "your Maximum weight is "+weight;

            }
            return "record not found";


        }
        //public JsonResult GenerateEmployeeForPerofmanceEvaluationCopy(int fiscalYearId, int periodId, int evaluationCriteriaId, string code,string branchId, string orgId,string lookupId,  decimal resultValue)
        //{
        //    int iworkunit = 0;
        //    int jobTitle = 0;
        //    int kbranch = 0;
        //    if(!string.IsNullOrEmpty(code))
        //    {

        //        var empPerformances = db.EmployeePerformances.Where(x => x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.BusinessUnitId.ToString()== code).Where(x=>x.IsSecondLevelApproved).ToList();
        //        var measureDefinition = db.MeasurementDefinitions.Find(evaluationCriteriaId);
        //        if (measureDefinition != null && empPerformances.Any())
        //        {


        //            empPerformances = empPerformances.Where(x=>x.ResponsibilityLevel==measureDefinition.Role).ToList();
                    
        //        }
        //        if (!string.IsNullOrEmpty(code)&&empPerformances.Any())
        //        {
        //            empPerformances = empPerformances.Where(x => x.BusinessUnitId.ToString() == code).ToList();

        //        }
        //        if (empPerformances.Any()&& !string.IsNullOrEmpty(branchId))
        //        {
        //            int.TryParse(branchId, out int BranchId);
        //            empPerformances = empPerformances.Where(x => x.BranchId == BranchId).ToList();
        //            if (!empPerformances.Any())
        //            {
        //                var formattedData13 = new
        //                {
        //                    Message = "performance doesn't have any record on this Branch",
        //                    Status = "not-OK"
        //                };
        //                return Json(formattedData13, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                kbranch++;
        //            }
        //        }
        //        if (empPerformances.Any()&& !string.IsNullOrEmpty(orgId))
        //        {
        //            int.TryParse(orgId, out int orgstructureId);
        //            empPerformances = empPerformances.Where(x => x.OrgStructureId == orgstructureId).ToList();
        //            if (!empPerformances.Any())
        //            {
        //                var formattedData13 = new
        //                {
        //                    Message = "performance doesn't have any record on this work Unit",
        //                    Status = "not-OK"
        //                };
        //                return Json(formattedData13, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                iworkunit++;
        //            }
        //        }
        //        if (empPerformances.Any()&& !string.IsNullOrEmpty(lookupId))
        //        {
        //            int.TryParse(lookupId, out int lookUpId);
        //            HashSet<int> employeeIds = new HashSet<int>(db.Placements.Where(x => x.isCurrent)
        //                                           .Where(x => x.LookupId == lookUpId).Select(x => x.EmployeeId));
        //            empPerformances = empPerformances.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //            if (!empPerformances.Any())
        //            {
        //                var formattedData13 = new
        //                {
        //                    Message = "performance doesn't have any record on this Job Title",
        //                    Status = "not-OK"
        //                };
        //                return Json(formattedData13, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                jobTitle++;
        //            }
        //        }
                
        //        foreach (var empPerformance in empPerformances)
        //        {
        //            var placement = db.Placements.Where(x => x.isCurrent).Where(x => x.EmployeeId == empPerformance.EmployeeId).FirstOrDefault();
        //            int.TryParse(code, out int bussinessUnitId);
        //            var Performance = db.EmployeePerformances.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.EvaluationCriteriaTypeId == evaluationCriteriaId && x.IsSecondLevelApproved);
        //            if (Performance == null)
        //            {

        //                var entity = new EmployeePerformance
        //                {
        //                    EmployeeId = empPerformance.EmployeeId,
        //                    GlFiscalYearId = fiscalYearId,
        //                    GlPeriodId = periodId,
        //                    EvaluationCriteriaTypeId = evaluationCriteriaId,
        //                    BusinessUnitId = bussinessUnitId,
        //                    OrgStructureId =placement !=null?placement.OrgStructureId: 0,
        //                    BranchId =placement !=null?placement.BranchId: 0,
        //                    IsAcceptedByEmployee = true,
        //                    IsApproved = true,
        //                    IsSecondLevelApproved = true,
        //                    IsImmediateSuppurvisorApproved = true,
        //                    IsSectorApplied = true,
        //                    Weight = 0,
        //                    Result = resultValue
        //                };

        //                db.EmployeePerformances.Add(entity);
        //                db.SaveChanges();

        //                var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == entity.EvaluationCriteriaTypeId).ToList();

        //                foreach (var meas in measurments)
        //                {
        //                    var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == entity.EmployeePerformanceId);

        //                    if (checkExit == null)
        //                    {
        //                        var empMeasure = new EmployeeMeasurement();

        //                        empMeasure.MeasurmentId = meas.MeasurementId;
        //                        empMeasure.EmployeePerformanceId = entity.EmployeePerformanceId;
        //                        empMeasure.Weight = meas.MesPercentageofWeight;
        //                        empMeasure.Result = resultValue;

        //                        db.EmployeeMeasurement.Add(empMeasure);
        //                        db.SaveChanges();
        //                        //update weight
        //                        entity.Weight = meas.MesPercentageofWeight;
        //                        db.Entry(entity).State = EntityState.Modified;
        //                        db.SaveChanges();
        //                        var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId == empMeasure.MeasurmentId).ToList();

        //                        foreach (var per in performanceActions)
        //                        {
        //                            var performanceActionScore = new EmployeePerformanceScore
        //                            {
        //                                PerformanceActionId = per.ActionId,
        //                                EmployeeMeasurementId = empMeasure.EmpMeasurementId,
        //                                Weight = per.AcPercentageofWeight,
        //                                Score = resultValue,
        //                                Remark = "",
        //                            };

        //                            db.EmployeePerformanceScores.Add(performanceActionScore);
        //                            db.SaveChanges();
        //                        }
        //                    }
        //                }
        //            }


                   

                   

        //        }
        //        if (iworkunit > 0)
        //        {
        //            var formattedData111 = new
        //            {
        //                Message = "Department Level  is applied",
        //                Status = "OK"
        //            };
        //            return Json(formattedData111, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var formattedData = new
        //            {
        //                Message = "Sector Level  is applied",
        //                Status = "OK"
        //            };
        //        }
               
              
        //    }
            
        //    else if(string.IsNullOrEmpty(code))
        //    {
                
        //        var empPerformances = db.EmployeePerformances.Where(x => x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId &&x.IsSecondLevelApproved).ToList();
        //        var measureDefinition = db.MeasurementDefinitions.Find(evaluationCriteriaId);
        //        if (measureDefinition != null && empPerformances.Any())
        //        {
        //            var employeeIds = db.Placements.Where(x => x.ResponsibilityLevel == measureDefinition.Role).Select(x => x.EmployeeId).ToList();
        //            if (employeeIds.Any())
        //            {
        //                empPerformances = empPerformances.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //            }
        //        }
        //        if (empPerformances.Any() &&  !string.IsNullOrEmpty(orgId))
        //        {
        //            int.TryParse(orgId, out int orgstructureId);
        //            empPerformances = empPerformances.Where(x => x.OrgStructureId == orgstructureId).ToList();
        //            if (!empPerformances.Any())
        //            {
        //                var formattedData13 = new
        //                {
        //                    Message = "performance doesn't have any record on this work Unit",
        //                    Status = "not-OK"
        //                };
        //                return Json(formattedData13, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                iworkunit++;
        //            }
        //        }
        //        if (empPerformances.Any() && !string.IsNullOrEmpty(lookupId))
        //        {
        //            int.TryParse(lookupId, out int lookUpId);
        //            HashSet<int> employeeIds = new HashSet<int>(db.Placements.Where(x => x.isCurrent)
        //                                           .Where(x => x.LookupId == lookUpId).Select(x => x.EmployeeId));
        //            empPerformances = empPerformances.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //            if (!empPerformances.Any())
        //            {
        //                var formattedData13 = new
        //                {
        //                    Message = "performance doesn't have any record on this Job Title",
        //                    Status = "not-OK"
        //                };
        //                return Json(formattedData13, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                jobTitle++;
        //            }
        //        }
        //        if (!empPerformances.Any())
        //        {
        //            var formattedData13 = new
        //            {
        //                Message = "performance doesn't have any record",
        //                Status = "not-OK"
        //            };
        //            return Json(formattedData13, JsonRequestBehavior.AllowGet);
        //        }
        //        foreach (var empPerformance in empPerformances)
        //        {
        //            var placement = db.Placements.Where(x => x.isCurrent).Where(x => x.EmployeeId == empPerformance.EmployeeId).FirstOrDefault();

        //            var Performance = db.EmployeePerformances.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId&&x.EvaluationCriteriaTypeId==evaluationCriteriaId &&x.IsSecondLevelApproved);
        //            if(Performance == null)
        //            {
        //                var entity = new EmployeePerformance
        //                {
        //                    EmployeeId = empPerformance.EmployeeId,
        //                    GlFiscalYearId = fiscalYearId,
        //                    GlPeriodId = periodId,
        //                    EvaluationCriteriaTypeId = evaluationCriteriaId,
        //                    OrgStructureId = placement != null ? placement.OrgStructureId : 0,
        //                    BranchId = placement != null ? placement.BranchId : 0,
        //                    BusinessUnitId = placement != null ? placement.BusinessUnitId : 0,
        //                    IsCorporateApplied = true,
        //                    IsAcceptedByEmployee = true,
        //                    IsApproved = true,
        //                    IsSecondLevelApproved = true,
        //                    IsImmediateSuppurvisorApproved = true,
        //                    IsHRLevelEvalution = true,
        //                    Weight = 0,
        //                    Result = resultValue
        //                };

        //                db.EmployeePerformances.Add(entity);
        //                db.SaveChanges();

        //                var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == entity.EvaluationCriteriaTypeId).ToList();

        //                foreach (var meas in measurments)
        //                {
        //                    var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == entity.EmployeePerformanceId);

        //                    if (checkExit == null)
        //                    {
        //                        var empMeasure = new EmployeeMeasurement();

        //                        empMeasure.MeasurmentId = meas.MeasurementId;
        //                        empMeasure.EmployeePerformanceId = entity.EmployeePerformanceId;
        //                        empMeasure.Weight = meas.MesPercentageofWeight;
        //                        empMeasure.Result = resultValue;

        //                        db.EmployeeMeasurement.Add(empMeasure);
        //                        db.SaveChanges();
        //                        //update weight
        //                        entity.Weight = meas.MesPercentageofWeight;
        //                        db.Entry(entity).State = EntityState.Modified;
        //                        db.SaveChanges();
        //                        var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId == empMeasure.MeasurmentId).ToList();

        //                        foreach (var per in performanceActions)
        //                        {
        //                            var performanceActionScore = new EmployeePerformanceScore
        //                            {
        //                                PerformanceActionId = per.ActionId,
        //                                EmployeeMeasurementId = empMeasure.EmpMeasurementId,
        //                                Weight = per.AcPercentageofWeight,
        //                                Score = resultValue,
        //                                Remark = "",
        //                            };

        //                            db.EmployeePerformanceScores.Add(performanceActionScore);
        //                            db.SaveChanges();
        //                        }

        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Performance.Result = resultValue;
        //                db.Entry(Performance).State = EntityState.Modified;
        //                db.SaveChanges();
        //                var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == Performance.EvaluationCriteriaTypeId).ToList();

        //                foreach (var meas in measurments)
        //                {
        //                    var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == Performance.EmployeePerformanceId);

        //                    if (checkExit == null)
        //                    {
        //                        var empMeasure = new EmployeeMeasurement();

        //                        empMeasure.Weight = meas.MesPercentageofWeight;
        //                        empMeasure.Result = resultValue;

        //                        db.Entry(empMeasure).State = EntityState.Modified;
        //                        db.SaveChanges();
        //                        //update weight
        //                        Performance.Weight = meas.MesPercentageofWeight;
        //                        db.Entry(Performance).State = EntityState.Modified;
        //                        db.SaveChanges();
        //                        var performanceActions = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == empMeasure.MeasurmentId).ToList();

        //                        foreach (var per in performanceActions)
        //                        {

        //                            per.Score = resultValue;
        //                            db.Entry(per).State = EntityState.Modified;
        //                            db.SaveChanges();
        //                        }

        //                    }
        //                }
        //            }



        //        }
        //        if (iworkunit > 0)
        //        {
        //            var formattedData111 = new
        //            {
        //                Message = "Department Level  is applied",
        //                Status = "OK"
        //            };
        //            return Json(formattedData111, JsonRequestBehavior.AllowGet);
        //        }
        //        if (jobTitle > 0)
        //        {
        //            var formattedData111 = new
        //            {
        //                Message = "Project maneger  Evalution  is applied",
        //                Status = "OK"
        //            };
        //            return Json(formattedData111, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var formattedData = new
        //            {
        //                Message = "Corporate Level  is applied",
        //                Status = "OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }
               

        //    }
        //    var formattedData1 = new
        //    {
        //        Message = "performance dosn't have record",
        //        Status = "not-OK"
        //    };
        //    return Json(formattedData1, JsonRequestBehavior.AllowGet);

        //}
        public JsonResult GenerateEmployeeForPerofmanceEvaluation(int fiscalYearId, int periodId, int evaluationCriteriaId, string code, string branchId, string orgId,  string lookupId, decimal resultValue)
        {
            var empPerformances = db.EmployeePerformances
                .Where(x => x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.IsSecondLevelApproved).ToList();
            var measureDefinition = db.MeasurementDefinitions.FirstOrDefault(x=>x.MeasurementDefinitionId==evaluationCriteriaId);
            if(measureDefinition != null && empPerformances.Any())
            {
                empPerformances = empPerformances
                    .Where(x => x.ResponsibilityLevel == measureDefinition.Role).ToList();
            }
            if (!string.IsNullOrEmpty(code) && empPerformances.Any())
            {
                empPerformances = empPerformances
                    .Where(x => x.BusinessUnitId.ToString() == code).ToList();
            }

            if (!string.IsNullOrEmpty(orgId) && empPerformances.Any())
            {
                int.TryParse(orgId, out int orgstructureId);
                empPerformances = empPerformances.Where(x => x.OrgStructureId == orgstructureId).ToList();
            }

            if (!string.IsNullOrEmpty(lookupId) && empPerformances.Any())
            {
                int.TryParse(lookupId, out int lookUpId);
                empPerformances = empPerformances
                    .Where(x => x.JobTitleId == lookUpId).ToList();
            }

            if (!string.IsNullOrEmpty(branchId) && empPerformances.Any())
            {
                int.TryParse(branchId, out int BranchId);
                empPerformances = empPerformances
                    .Where(x => x.BranchId == BranchId).ToList();
            }

            if (!empPerformances.Any())
            {
                return Json(new { Message = "Performance doesn't have any record", Status = "not-OK" }, JsonRequestBehavior.AllowGet);
            }

            foreach (var empPerformance in empPerformances)
            {
                var placement = db.Placements.Where(x => x.isCurrent).Where(x => x.EmployeeId == empPerformance.EmployeeId).FirstOrDefault();

                var Performance = db.EmployeePerformances.FirstOrDefault(x =>x.EmployeeId==empPerformance.EmployeeId && x.GlFiscalYearId == fiscalYearId && x.GlPeriodId == periodId && x.EvaluationCriteriaTypeId == evaluationCriteriaId && x.IsSecondLevelApproved);
                if (Performance == null && placement != null)
                {
                    var categorId = db.MeasurementDefinitions.Find(evaluationCriteriaId);
                    var entity = new EmployeePerformance
                    {
                        EmployeeId = empPerformance.EmployeeId,
                        GlFiscalYearId = fiscalYearId,
                        GlPeriodId = periodId,
                        EvaluationCriteriaTypeId = evaluationCriteriaId,
                        BusinessUnitId = placement.BusinessUnitId,
                        OrgStructureId = placement.OrgStructureId,
                        BranchId = placement.BranchId,
                        JobTitleId = placement.LookupId,
                        IsCurrent = placement.isCurrent,
                        CriteriaCategoryId = categorId != null ? categorId.CriteriaCategoryId:0,
                        DirectBossId = placement.HeadId !=null?(int)placement.HeadId:0,
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
                        IsAcceptedByEmployee = true,
                        IsApproved = true,
                        IsSecondLevelApproved = true,
                        IsImmediateSuppurvisorApproved = true,
                        IsSectorApplied = false,
                        IsCorporateApplied = false,
                        IsHRApproved = false,
                        IsHRLevelEvalution = true,
                        IsRejectedByEmployee = false,
                        Weight = 0,
                        Result = resultValue
                    };

                    db.EmployeePerformances.Add(entity);
                    db.SaveChanges();

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
                            empMeasure.Result = resultValue;

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
                                    Score = resultValue,
                                    Remark = "",
                                };

                                db.EmployeePerformanceScores.Add(performanceActionScore);
                                db.SaveChanges();
                            }
                        }
                       
                    }
                    // Update weight
                    entity.Weight = db.EmployeeMeasurement.Where(x=>x.EmployeePerformanceId==entity.EmployeePerformanceId).Sum(x=>x.Weight);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (Performance != null && placement != null)
                {
                    
                    Performance.BusinessUnitId = placement.BusinessUnitId;
                    Performance.OrgStructureId = placement.OrgStructureId;
                    Performance.BranchId = placement.BranchId;
                    Performance.JobTitleId = placement.LookupId;
                    Performance.IsCurrent = placement.isCurrent;
                    Performance.ResponsibilityLevel = placement.ResponsibilityLevel;
                    Performance.Result = resultValue;
                    db.Entry(Performance).State = EntityState.Modified;
                    db.SaveChanges();
                    var measurements = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId).ToList();
                    if (measurements.Any())
                    {
                        foreach (var meas in measurements)
                        {
                            var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId);
                            if (checkExit != null)
                            {
                                var empMeasure = new EmployeeMeasurement();
                                checkExit.Weight = meas.Weight;
                                checkExit.Result = resultValue;
                                db.Entry(checkExit).State = EntityState.Modified;
                                db.SaveChanges();
                                var performanceActions = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == meas.EmpMeasurementId).ToList();
                                if (performanceActions.Any())
                                {
                                    foreach (var per in performanceActions)
                                    {
                                        per.Score = resultValue;
                                        db.Entry(per).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    var actions = db.PerformanceActions.Where(x => x.MeasurementId == meas.MeasurmentId).ToList();
                                    foreach (var action in actions)
                                    {
                                        var performanceActionScore = new EmployeePerformanceScore
                                        {
                                            PerformanceActionId = action.ActionId,
                                            EmployeeMeasurementId = checkExit.EmpMeasurementId,
                                            Weight = action.AcPercentageofWeight,
                                            Score = resultValue,
                                            Remark = "",
                                            Comment = "HR",
                                        };

                                        db.EmployeePerformanceScores.Add(performanceActionScore);
                                        db.SaveChanges();

                                    }
                                }

                            }
                        }
                        Performance.Weight = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId).Sum(x => x.Weight);
                        db.Entry(Performance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var measurementss = db.Measurements.Where(x => x.MeasurementDefinitionId == Performance.EvaluationCriteriaTypeId).ToList();

                        foreach (var meas in measurementss)
                        {
                            var checkExit = db.EmployeeMeasurement.FirstOrDefault(x => x.MeasurmentId == meas.MeasurementId && x.EmployeePerformanceId == Performance.EmployeePerformanceId);

                            if (checkExit == null)
                            {
                                var empMeasure = new EmployeeMeasurement();

                                empMeasure.MeasurmentId = meas.MeasurementId;
                                empMeasure.EmployeePerformanceId = Performance.EmployeePerformanceId;
                                empMeasure.Weight = meas.MesPercentageofWeight;
                                empMeasure.Result = resultValue;

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
                                        Score = resultValue,
                                        Remark = "",
                                    };

                                    db.EmployeePerformanceScores.Add(performanceActionScore);
                                    db.SaveChanges();
                                }
                            }

                        }
                        // Update weight
                        Performance.Weight = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == Performance.EmployeePerformanceId).Sum(x => x.Weight);
                        db.Entry(Performance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            var message = "Evaluation result has been successfully applied!";

            return Json(new { Message = message, Status = "OK" }, JsonRequestBehavior.AllowGet);
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

