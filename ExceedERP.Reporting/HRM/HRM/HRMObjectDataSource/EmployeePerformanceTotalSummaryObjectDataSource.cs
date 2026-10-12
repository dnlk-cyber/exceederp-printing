using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerformanceTotalSummaryObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public GlobalEmployeePerformanceTotalVM GetEmployeePerformanceGradSummary(string empId, string yearId, string quarterId, string criteriaId)
        {
            var empPerformance = new GlobalEmployeePerformanceTotalVM();

            int.TryParse(empId, out int employeeId);
            int.TryParse(yearId, out int fiscalYearId);
            int.TryParse(quarterId, out int quartId);
            int.TryParse(criteriaId, out int evaluationCriteriaId);
            if (empId != null&& yearId != null)
            {

               var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId == employeeId && x.IsApproved&&x.IsSecondLevelApproved).ToList();
                var GlFiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == fiscalYearId).Select(x => x.GlFiscalYearId);
                if (GlFiscalYearIds.Any())
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
                


               
                var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId && x.isCurrent);
                if (place != null)
                {
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                    if (person != null)
                    {
                        // HTML-encode since this value is embedded raw into an HtmlTextBox template
                        empPerformance.FullName = WebUtility.HtmlEncode(person.FullNameaEnglish);
                    }

                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                    if (branch != null)
                    {
                        empPerformance.Branch = WebUtility.HtmlEncode(branch.Name);
                    }

                    var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                    if (mainCompany != null)
                    {
                        empPerformance.Department = WebUtility.HtmlEncode(mainCompany.Name);
                    }

                    var look = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
                    if (look != null)
                    {
                        empPerformance.Jobtitle = WebUtility.HtmlEncode(look.Description);
                    }
                    var head = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.HeadId);
                    if (head != null)
                    {
                        empPerformance.ImmidiateApprover = head.FullNameaEnglish;
                    }
                    var dirctboss = employeePerformances.Where(x => x.DirectBossId != 0).FirstOrDefault();
                    if(dirctboss != null)
                    {
                        var secondLevelApprover = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == dirctboss.DirectBossId);
                        if (secondLevelApprover != null)
                        {
                            empPerformance.SecondLevelApproverApprover = secondLevelApprover.FullNameaEnglish;
                        }
                        empPerformance.Role = WebUtility.HtmlEncode(dirctboss.ResponsibilityLevel.ToString());
                    }
                  
                }
                var employeeComent = new StringBuilder();

                empPerformance.EmployeePerformanceList = new List<EmployeePerformanceEvaluationCriteriaTotalVM>();
                foreach (var employeePerformance in employeePerformances)
                {
                   

                    var evaluationCriteria = db.MeasurementDefinitions.Find(employeePerformance.EvaluationCriteriaTypeId);
                   if(evaluationCriteria !=null)
                   {

                        var crateriaList = new EmployeePerformanceEvaluationCriteriaTotalVM();
                        var quarter = db.Quarters.Find(employeePerformance.GlPeriodId);
                        if(quarter != null)
                        {
                            crateriaList.Period = quarter.Name;
                        }
                        crateriaList.EvaluationCriteriaName = evaluationCriteria.EvaluationCriteria;
                        crateriaList.Role = evaluationCriteria.Role.ToString();
                        crateriaList.Weight = evaluationCriteria.DefnPercentageofWeight;
                        crateriaList.Result = GetPerformanceScore(employeePerformance.EmployeePerformanceId);
                        var look = db.Lookups.FirstOrDefault(x => x.LookupId == evaluationCriteria.LookupId);
                        if (look != null)
                        {
                            crateriaList.EvaluationType = look.Description;
                            // HTML-encode since EmployeeComent is embedded raw into an HtmlTextBox template
                            if (!string.IsNullOrWhiteSpace(employeePerformance.EmployeeRemark))
                            {
                                employeeComent.AppendLine($"🧑‍💼{WebUtility.HtmlEncode(look.Description)} : {WebUtility.HtmlEncode(employeePerformance.EmployeeRemark)}");
                                employeeComent.AppendLine();
                            }

                            if (!string.IsNullOrWhiteSpace(employeePerformance.Remark))
                            {
                                employeeComent.AppendLine($"👨‍🏫{WebUtility.HtmlEncode(look.Description)} : {WebUtility.HtmlEncode(employeePerformance.Remark)}");
                                employeeComent.AppendLine();
                            }
                        }
                        empPerformance.EmployeePerformanceList.Add(crateriaList);

                   }


                }
                empPerformance.EmployeeComent = employeeComent.ToString();

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