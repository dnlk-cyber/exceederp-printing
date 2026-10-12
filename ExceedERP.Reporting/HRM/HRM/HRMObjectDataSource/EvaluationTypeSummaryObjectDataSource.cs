using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EvaluationTypeSummaryObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public GlobalEmployeePerformanceTotalVM GetEvaluationTypeSummary(string empId, string yearId, string quarterId, string criteriaId)
        {
            var empPerformance = new GlobalEmployeePerformanceTotalVM();

            //var EvaluationCriteriaListFinal = new List<EmployeePerformanceEvaluationCriteriaTotalVM>();
            int.TryParse(empId, out int employeeId);
            int.TryParse(yearId, out int fiscalYearId);
            int.TryParse(quarterId, out int quartId);
            int.TryParse(criteriaId, out int evaluationCriteriaId);
            if (empId != null&& yearId != null)
            {

               var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId == employeeId && x.IsApproved).ToList();
                var GlFiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == fiscalYearId).Select(x => x.GlFiscalYearId);
                if (GlFiscalYearIds != null)
                {
                    employeePerformances = employeePerformances.Where(x => GlFiscalYearIds.Contains( x.GlFiscalYearId )).ToList();
                }
                if (quarterId != null)
                {
                    employeePerformances = employeePerformances.Where(x => x.GlPeriodId == quartId).ToList();

                }
                if (criteriaId != null)
                {
                    employeePerformances = employeePerformances.Where(x => x.EvaluationCriteriaTypeId == evaluationCriteriaId).ToList();
                }
                var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == fiscalYearId);
                if (year != null)
                {
                    empPerformance.Year = year.Name;

                }
                if (!string.IsNullOrEmpty(quarterId))
                {
                    int.TryParse(quarterId, out int QuarterId);
                    var period = db.Quarters.Find(QuarterId);
                    if (period != null)
                    {
                        empPerformance.Period = period.Name;
                    }
                }
                var empPerform = employeePerformances.FirstOrDefault();
                if (empPerform != null)
                {
                    
                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empPerform.EmployeeId);
                    if (employee != null)
                    {
                        empPerformance.FullName = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                        ? $"{employee.FirstName} {employee.MiddleName} {employee.LastName}"
                                        : $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}";
                    }

                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == empPerform.EmployeeId && x.isCurrent);
                    if (place != null)
                    {
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                        if (branch != null)
                        {
                            empPerformance.Branch = branch.Name;
                        }

                        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (mainCompany != null)
                        {
                            empPerformance.Department = mainCompany.Name;
                        }

                        var look = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
                        if (look != null)
                        {
                            empPerformance.Jobtitle = look.Description;
                        }
                        var head = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.HeadId);
                        if (head != null)
                        {
                            empPerformance.ImmidiateApprover = head.FullNameaEnglish;
                        }
                        var secondLevelApprover = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empPerform.DirectBossId);
                        if (secondLevelApprover != null)
                        {
                            empPerformance.SecondLevelApproverApprover = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                        ? $"{secondLevelApprover.FirstName} {secondLevelApprover.MiddleName} {secondLevelApprover.LastName}"
                                        : $"{secondLevelApprover.FirstNameEnglish} {secondLevelApprover.MiddleNameEnglish} {secondLevelApprover.LastNameEnglish}";
                        }

                    }

                   
                   


                }
                empPerformance.EmployeePerformanceList = new List<EmployeePerformanceEvaluationCriteriaTotalVM>();
                foreach (var employeePerformance in employeePerformances)
                {

                   var evaluationCriteria = db.MeasurementDefinitions.Find(employeePerformance.EvaluationCriteriaTypeId);
                   if(evaluationCriteria !=null)
                   {

                        var crateriaList = new EmployeePerformanceEvaluationCriteriaTotalVM();
                        crateriaList.EvaluationCriteriaName = evaluationCriteria.EvaluationCriteria;
                        crateriaList.Role = evaluationCriteria.Role.ToString();
                        crateriaList.Weight = evaluationCriteria.DefnPercentageofWeight;
                        crateriaList.Result = GetPerformanceScore(employeePerformance.EmployeePerformanceId);
                        var look = db.Lookups.FirstOrDefault(x => x.LookupId == evaluationCriteria.LookupId);
                        if (look != null)
                        {
                            crateriaList.EvaluationType = look.Description;
                        }
                        empPerformance.EmployeePerformanceList.Add(crateriaList);
                      

                    }


                }
            }
           
            return empPerformance;

        }

        
        public decimal GetPerformanceScore(int empPerformanceId)
        {
            decimal totalScore = 0;

            var empMeasurs = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == empPerformanceId).ToList();

            foreach (var em in empMeasurs)
            {
                var scores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId);

                if (scores.Any())
                {
                    totalScore = totalScore + scores.Sum(x => x.Score);
                }
            }

            return totalScore;
        }
        public void Dispose()
        {
            db.Dispose();
        }


    }
}