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

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class EmployeePerformanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        static int EvaluationType { get; set; }
        static int Employee { get; set; }
        static int Period { get; set; }
        static bool depLevel { get; set; }
        static string DefinationRemark { get; set; }
        static string MeasureRemark { get; set; }
        static string ActionRemark { get; set; }
        public ActionResult Index()
        {

            ViewData["EvaluationTypes"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.EVALUATION_TYPE).ToList();

            var periods = db.GLPeriods.ToList();
            ViewData["Periods"] = periods;
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            ViewData["Employee"] = db.Person_Employee.AsQueryable();
            ViewData["PerformanceActions"] = db.PerformanceActions.AsQueryable();
            return View();
        }
        public decimal GetPerformanceResult(int empId, string period, string lookupid, string evaluationLevel)
        {
            decimal result = 0;

            var measurementdefinitions = db.MeasurementDefinitions.Select(x => x.MeasurementDefinitionId);
            if (!String.IsNullOrEmpty(lookupid))
            {
                measurementdefinitions = db.MeasurementDefinitions.Where(x => x.LookupId.ToString() == lookupid).Select(x => x.MeasurementDefinitionId);
            }
            if(!String.IsNullOrEmpty(evaluationLevel))
            {
                if (evaluationLevel == "1")
                {
                    measurementdefinitions = db.MeasurementDefinitions.Where(x => x.LookupId.ToString() == lookupid && !x.IsDepartmentLevel).Select(x => x.MeasurementDefinitionId);
                }
                else if (evaluationLevel == "2")
                {
                    measurementdefinitions = db.MeasurementDefinitions.Where(x => x.LookupId.ToString() == lookupid && x.IsDepartmentLevel).Select(x => x.MeasurementDefinitionId);
                }
            }
            if (measurementdefinitions.Any())
            {
                var measurments = db.Measurements.Where(x => measurementdefinitions.Contains(x.MeasurementDefinitionId)).Select(x => x.MeasurementId).ToList();
                if (measurments.Any())
                {
                    var score = db.EmployeePerformanceScores.Where(x => measurments.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period).ToList();
                    if (!String.IsNullOrEmpty(evaluationLevel))
                    {
                        if (evaluationLevel == "1")
                        {
                            score = db.EmployeePerformanceScores.Where(x => measurments.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period && !x.IsDepartmentLevel).ToList();
                        }
                        else if (evaluationLevel == "2")
                        {
                            score = db.EmployeePerformanceScores.Where(x => measurments.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period && x.IsDepartmentLevel).ToList();
                        }
                    }
                    if (score.Any())
                    {
                        result = score.Sum(x => x.Result);
                    }
                }
            }

            return result;
        }
        public decimal GetPerformanceWeight(int empId, string period, string lookupid, string evaluationLevel)
        {
            decimal result = 0;
            bool isDepartmentLevel = false;

            var measurementdefinitions = db.MeasurementDefinitions.Select(x => x.MeasurementDefinitionId);
            if (!String.IsNullOrEmpty(lookupid))
            {
                measurementdefinitions = db.MeasurementDefinitions.Where(x => x.LookupId.ToString() == lookupid).Select(x => x.MeasurementDefinitionId);
            }
            if (!String.IsNullOrEmpty(evaluationLevel))
            {
                if (evaluationLevel == "1")
                {
                    measurementdefinitions = db.MeasurementDefinitions.Where(x => x.LookupId.ToString() == lookupid && !x.IsDepartmentLevel).Select(x => x.MeasurementDefinitionId);
                }
                else if (evaluationLevel == "2")
                {
                    measurementdefinitions = db.MeasurementDefinitions.Where(x => x.LookupId.ToString() == lookupid && x.IsDepartmentLevel).Select(x => x.MeasurementDefinitionId);
                }
            }
            if (measurementdefinitions.Any())
            {
                var measurments = db.Measurements.Where(x => measurementdefinitions.Contains(x.MeasurementDefinitionId)).Select(x => x.MeasurementId).ToList();
                if (measurments.Any())
                {
                    var score = db.EmployeePerformanceScores.Where(x => measurments.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period).ToList();
                    if (!String.IsNullOrEmpty(evaluationLevel))
                    {
                        if (evaluationLevel == "1")
                        {
                            score = db.EmployeePerformanceScores.Where(x => measurments.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period && !x.IsDepartmentLevel).ToList();
                        }
                        else if (evaluationLevel == "2")
                        {
                            score = db.EmployeePerformanceScores.Where(x => measurments.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period && x.IsDepartmentLevel).ToList();
                        }
                    }
                    if (score.Any())
                    {
                        result = score.Sum(x => x.EmpPerfPercentageofWeight);
                    }
                }
            }

            return result;
        }
        public string GetGrade(int empId, string period, string lookupid, string evaluationLevel)
        {
            string grade = "";
            var measurementdefinitions = db.MeasurementDefinitions.ToList();
            if (!String.IsNullOrEmpty(lookupid))
            {
                measurementdefinitions = measurementdefinitions.Where(x => x.LookupId.ToString() == lookupid).ToList();
            }
            if (!String.IsNullOrEmpty(evaluationLevel))
            {
                if (evaluationLevel == "1")
                {
                    measurementdefinitions = measurementdefinitions.Where(x => !x.IsDepartmentLevel).ToList();
                }
                else if (evaluationLevel == "2")
                {
                    measurementdefinitions = measurementdefinitions.Where(x => x.IsDepartmentLevel).ToList();
                }
            }
            if (measurementdefinitions.Any())
            {
                var measurDefintions = measurementdefinitions.Select(x => x.MeasurementDefinitionId);
                var measurments = db.Measurements.Where(x => measurDefintions.Contains(x.MeasurementDefinitionId)).Select(x => x.MeasurementId).ToList();
                if (measurments.Any())
                {
                    var score = db.EmployeePerformanceScores.Where(x => measurments.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period).ToList();
                    if (score.Any())
                    {
                        var result = score.Sum(x => x.Result);
                        var performanceRank = db.PerformanceRank.FirstOrDefault(x => x.Minimum <= result && x.Maximum >= result);
                        if (performanceRank != null)
                        {
                            grade = performanceRank.Description;
                        }
                    }
                }
            }

            return grade;
        }
        public decimal GetMeasurResult(int empId, string period, string lookupid, int measuDefId, int measurId, string evaluationLevel)
        {
            decimal result = 0;
            var measurementdefinition = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == measuDefId);
            if (!String.IsNullOrEmpty(evaluationLevel))
            {
                if (evaluationLevel == "1")
                {
                    measurementdefinition = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == measuDefId && !x.IsDepartmentLevel);
                }
                else if (evaluationLevel == "2")
                {
                    measurementdefinition = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == measuDefId && x.IsDepartmentLevel);
                }
            }
            if (measurementdefinition != null)
            {
                var measurment = db.Measurements.FirstOrDefault(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && x.MeasurementId == measurId);
                if (measurment != null)
                {
                    var score = db.EmployeePerformanceScores.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && x.MeasurementId == measurment.MeasurementId && x.EmployeeId == empId && x.QuarterId.ToString() == period).ToList();
                    if (!String.IsNullOrEmpty(evaluationLevel))
                    {
                        if (evaluationLevel == "1")
                        {
                            score = db.EmployeePerformanceScores.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && x.MeasurementId == measurment.MeasurementId && x.EmployeeId == empId && x.QuarterId.ToString() == period && !x.IsDepartmentLevel).ToList();
                        }
                        else if (evaluationLevel == "2")
                        {
                            score = db.EmployeePerformanceScores.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && x.MeasurementId == measurment.MeasurementId && x.EmployeeId == empId && x.QuarterId.ToString() == period && x.IsDepartmentLevel).ToList();
                        }
                    }
                    if (score.Any())
                    {
                        result = score.Sum(x => x.Result);
                    }
                }
            }

            return result;
        }
        public decimal GetMeasurWeight(int empId, string period, string lookupid, int measuDefId, int measurId)
        {
            decimal result = 0;
            var measurementdefinition = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == measuDefId);
            if (measurementdefinition != null)
            {
                var measurment = db.Measurements.FirstOrDefault(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && x.MeasurementId == measurId);
                if (measurment != null)
                {
                    var score = db.EmployeePerformanceScores.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && x.MeasurementId == measurment.MeasurementId && x.EmployeeId == empId && x.QuarterId.ToString() == period).ToList();
                    if (score.Any())
                    {
                        result = score.Sum(x => x.EmpPerfPercentageofWeight);
                    }
                }
            }

            return result;
        }
        public decimal GetMeasurDefinitionResult(int empId, string period, string lookupid, int measuDefId, string evaluationLevel)
        {
            decimal result = 0;
            var measurementdefinition = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == measuDefId);
       
            if (evaluationLevel == "1")
            {
                measurementdefinition = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == measuDefId && !x.IsDepartmentLevel);
            }
            else if (evaluationLevel == "2")
            {
                measurementdefinition = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == measuDefId && x.IsDepartmentLevel);
            }

            if (measurementdefinition != null)
            {
                var measurment = db.Measurements.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId).Select(x => x.MeasurementId).ToList();
                if (measurment.Any())
                {
                    var score = db.EmployeePerformanceScores.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && measurment.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period).ToList();
                    if (evaluationLevel == "1")
                    {
                        score = db.EmployeePerformanceScores.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && measurment.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period && !x.IsDepartmentLevel).ToList();
                    }
                    else if (evaluationLevel == "2")
                    {
                        score = db.EmployeePerformanceScores.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && measurment.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period && x.IsDepartmentLevel).ToList();
                    }
                    if (score.Any())
                    {
                        result = score.Sum(x => x.Result);
                    }
                }
            }

            return result;
        }
        public decimal GetMeasurDefinitionWeight(int empId, string period, string lookupid, int measuDefId)
        {
            decimal result = 0;
            var measurementdefinition = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == measuDefId);
            if (measurementdefinition != null)
            {
                var measurment = db.Measurements.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId).Select(x => x.MeasurementId).ToList();
                if (measurment.Any())
                {
                    var score = db.EmployeePerformanceScores.Where(x => x.MeasurementDefinitionId == measurementdefinition.MeasurementDefinitionId && measurment.Contains(x.MeasurementId.Value) && x.EmployeeId == empId && x.QuarterId.ToString() == period).ToList();
                    if (score.Any())
                    {
                        result = score.Sum(x => x.EmpPerfPercentageofWeight);
                    }
                }
            }

            return result;
        }
        public bool GetApproveStatus(int empId, string period)
        {
            var performanceScore = db.EmployeePerformanceScores.FirstOrDefault(x=>x.EmployeeId == empId && x.QuarterId.ToString() == period && x.IsApproved);
            if(performanceScore != null)
            {
                return true;
            }
            return false;
        }
        public ActionResult EmployeePerformance_Read([DataSourceRequest] DataSourceRequest request, string employee, string period, string LookupId, string Evaluation)
        {
            var logedEmp = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates(logedEmp);
            var employeePerformance = db.EmployeePerformances.Where(x => subordinates.Contains(x.EmployeeId)).ToList();
            if (!string.IsNullOrEmpty(employee))
            {
                employeePerformance = db.EmployeePerformances.Where(x => x.EmployeeId.ToString() == employee).ToList();
            }
            var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == PerformanceSelfEvaluation.Enable_Self_Evaluation.ToString());
            if (config == null)
            {
                subordinates.Remove(logedEmp);
                employeePerformance = employeePerformance.Where(x => x.EmployeeId != logedEmp).ToList();
            }
            #region Generate Performace 
            if (subordinates.Count() > employeePerformance.Count() && String.IsNullOrEmpty(employee))
            {
                foreach (var sub in subordinates)
                {
                    var empPerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == sub);
                    if (empPerformance == null)
                    {
                        var entity = new EmployeePerformance
                        {
                            EmployeeId = sub,
                            Weight = 0,
                            Result = 0
                        };
                        db.EmployeePerformances.Add(entity);
                        db.SaveChanges();
                    }
                }

                employeePerformance = db.EmployeePerformances.Where(x => subordinates.Contains(x.EmployeeId)).ToList();
            }
            if (!employeePerformance.Any() && !string.IsNullOrEmpty(employee))
            {
                var id = Convert.ToInt32(employee);
                var entity = new EmployeePerformance
                {
                    EmployeeId = id,
                    Weight = 0,
                    Result = 0
                };

                db.EmployeePerformances.Add(entity);
                db.SaveChanges();
            }
            #endregion

            DataSourceResult result = employeePerformance.ToDataSourceResult(request,
                emp => new
                {
                    EmployeePerformanceId = emp.EmployeePerformanceId,
                    EmployeeId = emp.EmployeeId,
                    Weight = GetPerformanceWeight(emp.EmployeeId, period, LookupId, Evaluation),
                    Result = GetPerformanceResult(emp.EmployeeId, period, LookupId, Evaluation),
                    Grade = GetGrade(emp.EmployeeId, period, LookupId, Evaluation),
                    IsApproved = GetApproveStatus(emp.EmployeeId,period),
                    Remark = emp.Remark,
                });
            return Json(result);

        }

        public ActionResult MeasurementDefinitions_Read([DataSourceRequest] DataSourceRequest request, string employee, string period, string LookupId, string Evaluation, int empId)
        {                 


            IQueryable<MeasurementDefinition> measurementdefinitions = db.MeasurementDefinitions;

            if(!String.IsNullOrEmpty(Evaluation))
            {
                if (Evaluation == "1")
                {
                    measurementdefinitions = measurementdefinitions.Where(x=>!x.IsDepartmentLevel);
                }
                else if (Evaluation == "2")
                {
                    measurementdefinitions = measurementdefinitions.Where(x => x.IsDepartmentLevel);
                }
            }

            if (!String.IsNullOrEmpty(LookupId))
            {
                measurementdefinitions = measurementdefinitions.Where(x => x.LookupId.ToString() == LookupId);
            }



            DataSourceResult result = measurementdefinitions.ToDataSourceResult(request,
                measurementDefinition => new
                {
                    MeasurementDefinitionId = measurementDefinition.MeasurementDefinitionId,
                    EvaluationCriteria = measurementDefinition.EvaluationCriteria,
                    LookupId = measurementDefinition.LookupId,
                    Role = measurementDefinition.Role,
                    EmployeeId = measurementDefinition.EmployeeId,
                    Result = GetMeasurDefinitionResult(empId, period, LookupId, measurementDefinition.MeasurementDefinitionId,Evaluation),
                    DefnPercentageofWeight = measurementDefinition.DefnPercentageofWeight,//GetMeasurDefinitionWeight(empId, period, LookupId, measurementDefinition.MeasurementDefinitionId),
                    Remark = measurementDefinition.Remark,
                    // IsApproved = IsPerformanceApproved(measurementDefinition.MeasurementDefinitionId),
                    CheckEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name)
                });
            return Json(result);

        }
        public ActionResult Measurements_Read([DataSourceRequest] DataSourceRequest request, string mdid, string period, string LookupId, int empId, string Evaluation)
        {

            IQueryable<Measurement> measurements = db.Measurements.Where(x => x.MeasurementDefinitionId.ToString() == mdid);

            DataSourceResult result = measurements.ToDataSourceResult(request,
                measur => new
                {
                    MeasurementId = measur.MeasurementId,
                    MeasurementDefinitionId = measur.MeasurementDefinitionId,
                    MeasurementName = measur.MeasurementName,
                    MesPercentageofWeight = measur.MesPercentageofWeight,//GetMeasurWeight(empId, period, LookupId, measur.MeasurementDefinitionId, measur.MeasurementId),
                    Result = GetMeasurResult(empId, period, LookupId, measur.MeasurementDefinitionId, measur.MeasurementId,Evaluation),
                    Remark = measur.Remark
                });
            return Json(result);

        }

        public ActionResult PerformanceActions_Read([DataSourceRequest] DataSourceRequest request, string employee, string period, string LookupId, string Evaluation, string mesid, string empId, int measurmentDefinitionId)
        {
            bool isDepartmentLevel = false;
           

            IQueryable<EmployeePerformanceScore> actions = db.EmployeePerformanceScores.Where(x => x.QuarterId.ToString() == period && x.EmployeeId.ToString() == empId && x.MeasurementId.ToString() == mesid && x.MeasurementDefinitionId == measurmentDefinitionId);
            if (Evaluation == "1")
            {
                Evaluation = EvaluationLevel.Global.ToString();
                isDepartmentLevel = false;
                actions = actions.Where(x => !x.IsDepartmentLevel);
            }
            else if (Evaluation == "2")
            {
                Evaluation = EvaluationLevel.Department.ToString();
                isDepartmentLevel = true;
                actions = actions.Where(x => x.IsDepartmentLevel);
            }
          
            if (!actions.Any())
            {
                if ((!String.IsNullOrEmpty(employee) || !String.IsNullOrEmpty(empId)) && !String.IsNullOrEmpty(period))
                {

                    var performanceActions = db.PerformanceActions.Where(x => x.MeasurementId.ToString() == mesid).ToList();
                    foreach (var pa in performanceActions)
                    {
                        var id = Convert.ToInt32(empId);
                        var quarter = Convert.ToInt32(period);
                        var entity = new EmployeePerformanceScore
                        {
                            ActionId = pa.ActionId,
                            EmployeeId = id,
                            QuarterId = quarter,
                            MeasurementId = pa.MeasurementId,
                            MeasurementDefinitionId = measurmentDefinitionId,
                            EmpPerfPercentageofWeight = pa.AcPercentageofWeight,
                            IsApproved = false,
                            IsDepartmentLevel = isDepartmentLevel
                        };
                        db.EmployeePerformanceScores.Add(entity);
                        db.SaveChanges();
                    }

                }
            }
            DataSourceResult result = actions.ToDataSourceResult(request,
                measur => new
                {
                    EmployeePerformanceScoreId = measur.EmployeePerformanceScoreId,
                    MeasurementId = measur.MeasurementId,
                    MeasurementDefinitionId = measur.MeasurementDefinitionId,
                    ActionId = measur.ActionId,
                    QuarterId = measur.QuarterId,
                    EmployeeId = measur.EmployeeId,
                    Result = measur.Result,
                    Remark = measur.Remark,
                    FiscalYearId = measur.FiscalYearId,
                    IsDepartmentLevel = measur.IsDepartmentLevel,
                    IsApproved = measur.IsApproved,
                    EmpPerfPercentageofWeight = measur.EmpPerfPercentageofWeight
                });
            return Json(result);

        }
        public ActionResult PerformanceActions_Update([DataSourceRequest] DataSourceRequest request, EmployeePerformanceScore employeePerformanceScore)
        {
            if (employeePerformanceScore.Result > employeePerformanceScore.EmpPerfPercentageofWeight)
            {
                ModelState.AddModelError("", "Result should less or equal to percentage weight");
            }
            if (ModelState.IsValid)
            {
                var entity = new EmployeePerformanceScore
                {
                    EmployeePerformanceScoreId = employeePerformanceScore.EmployeePerformanceScoreId,
                    ActionId = employeePerformanceScore.ActionId,
                    FiscalYearId = employeePerformanceScore.FiscalYearId,
                    MeasurementDefinitionId = employeePerformanceScore.MeasurementDefinitionId,
                    MeasurementId = employeePerformanceScore.MeasurementId,
                    QuarterId = employeePerformanceScore.QuarterId,
                    EmployeeId = employeePerformanceScore.EmployeeId,
                    Remark = employeePerformanceScore.Remark,
                    Result = employeePerformanceScore.Result,
                    IsDepartmentLevel = employeePerformanceScore.IsDepartmentLevel,
                    IsApproved = employeePerformanceScore.IsApproved,
                    EmpPerfPercentageofWeight = employeePerformanceScore.EmpPerfPercentageofWeight,
                };

                db.EmployeePerformanceScores.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Employee Performance  ", "Success", User.Identity.Name, User.Identity.Name + " update Employee performance of employee id " + entity.EmployeeId, Modules.HRM);

                #endregion

            }

            return Json(new[] { employeePerformanceScore }.ToDataSourceResult(request, ModelState));
        }
        private int GetActionId(int mesId, int perormanceAction, int emId, int period)
        {
            var performance =
                db.EmployeePerformanceAction.FirstOrDefault(
                    x =>
                        x.MeasurementId == mesId && x.QuarterId == period &&
                        x.EmployeeId == emId && x.ActionId == perormanceAction);
            if (performance != null)
            {
                return performance.EmpActionId;
            }
;
            return 0;
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
        public JsonResult GetPeriodListssss()
        {
            var currentFiscalYear = from gl in db.GLFiscalYears
                                    join ql in db.QuarterLengths
                                    on gl.GlFiscalYearId equals ql.GlFiscalYearId
                                    join q in db.Quarters
                                    on ql.QuarterLengthId equals q.QuarterLengthId

                                    where gl.Status == FiscalYearStatus.Open
                                    select (new { q });

            var periodList = currentFiscalYear.Select(x => x.q).ToList();
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<Quarter>();

            return Json(periodList.Select(p => new { QuarterId = p.QuarterId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployee(int employee)
        {
            Employee = employee;

            return Index();
        }
        public ActionResult GetPeriod(int period)
        {
            Period = period;

            return Index();
        }
        public ActionResult PerformanceSummary()
        {
            return View();
        }
        public ActionResult PerformanceSummaryDepartment()
        {
            return View();
        }
        public JsonResult GetLevel()
        {
            List<string> attributes = Enum.GetValues(typeof(EvaluationLevel))
                .Cast<EvaluationLevel>()
                .Select(v => v.ToString())
                .ToList();

            return Json(attributes.Select(s => new { Text = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCurrentLogUser()
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var person = db.Person_Employee.Where(x => x.EmployeeId == empId).ToList();

            return Json(person.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        private decimal GetPerformanceActionResult(int actionId, /*int defId,*/ int mesId)
        {
            try
            {
                db = new ExceedDbContext();
                var employeePerformanceAction =
                    db.EmployeePerformanceAction.Where(
                        x =>
                            x.ActionId == actionId && /*x.MeasurementDefinitionId == defId &&*/ x.MeasurementId == mesId &&
                            x.EmployeeId == Employee && x.QuarterId == Period).ToList();

                if (employeePerformanceAction.Any())
                {
                    var result =
                        employeePerformanceAction.Sum(x => x.Result);
                    ActionRemark = employeePerformanceAction.FirstOrDefault().Remark;
                    return result;
                }
            }
            catch (Exception)
            {

                //throw;
            }
            return 0;
        }
        public string Approve(int period)
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates(empId);
         
            var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == PerformanceSelfEvaluation.Enable_Self_Evaluation.ToString());
            if (config == null)
            {
                subordinates.Remove(empId);
            }

            var performanceScores = db.EmployeePerformanceScores.Where(x => x.QuarterId == period && subordinates.Contains(x.EmployeeId)).ToList();
            foreach (var per in performanceScores)
            {
                per.IsApproved = true;

                db.Entry(per).State = EntityState.Modified;
                db.SaveChanges();
            }

            return "true";
        }
        private string Criteria(int id)
        {
            var Defn = db.MeasurementDefinitions.Where(x => x.MeasurementDefinitionId == id).FirstOrDefault();
            if (Defn != null)
            {
                return Defn.EvaluationCriteria;
            }
            return "";
        }

        private ResponsibilityLevel Role(int id)
        {
            var Defn = db.MeasurementDefinitions.Where(x => x.MeasurementDefinitionId == id).FirstOrDefault();
            if (Defn != null)
            {
                return Defn.Role;
            }
            return ResponsibilityLevel.TOP_MANAGMENT;
        }

        private decimal GetActionSummaryResult(int defId, int mesId)
        {
            decimal result = 0;
            try
            {


                db = new ExceedDbContext();
                if (defId == 0)
                {
                    var firstOrDefaultt =
                        db.EmployeePerformanceAction.Where(
                            x => x.MeasurementId == mesId && x.EmployeeId == Employee && x.QuarterId == Period)
                            .ToList();
                    if (firstOrDefaultt.Any())
                    {
                        result = firstOrDefaultt.Sum(x => x.Result);


                    }
                }
                if (mesId == 0)
                {
                    var firstOrDefaultt =
                        db.EmployeePerformanceAction.Where(
                            x => x.MeasurementDefinitionId == defId && x.EmployeeId == Employee && x.QuarterId == Period)
                            .ToList();
                    if (firstOrDefaultt.Any())
                    {
                        result = firstOrDefaultt.Sum(x => x.Result);


                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

