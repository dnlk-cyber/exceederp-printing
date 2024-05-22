using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using XAPI;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerfromanceLeaveDispilineBonusSummaryObjectDataSource : List<EmployeePerfromanceLeaveDispilineBonusSummaryVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceLeaveDispilineBonusSummaryVM> GetEmployeeBSCPerformance(string branchId, string orgStructureId, string year,string quarterId, string status,string BusinessUnit,string rankName)
        {
            List<EmployeePerfromanceLeaveDispilineBonusSummaryVM> employeePerformanceList = new List<EmployeePerfromanceLeaveDispilineBonusSummaryVM>();


            decimal totalResult = 0;
            decimal totalWeight = 0;

          
            if (!string.IsNullOrEmpty(year))
            {
                int.TryParse(year, out int yearId);

                var EmpPerformances = db.EmployeePerformances.Where(x => x.IsSecondLevelApproved).ToList();
                var GlFiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == yearId).Select(x => x.GlFiscalYearId);
                if (GlFiscalYearIds.Any())
                {
                    EmpPerformances = EmpPerformances.Where(x => GlFiscalYearIds.Contains(x.GlFiscalYearId)).ToList();
                }
                if (quarterId != null)
                {
                    int.TryParse(quarterId, out int quartId);
                    EmpPerformances = EmpPerformances.Where(x => x.GlPeriodId == quartId).ToList();

                }
                if (branchId != null)
                {
                    int.TryParse(branchId, out int braId);
                    EmpPerformances = EmpPerformances.Where(x => x.BranchId == braId).ToList();
                }
                if (orgStructureId != null)
                {
                    int.TryParse(orgStructureId, out int orgId);
                    EmpPerformances = EmpPerformances.Where(x => x.OrgStructureId == orgId).ToList();
                }

                if (BusinessUnit != null)
                {
                    int.TryParse(BusinessUnit, out int businessUnitId);
                    EmpPerformances = EmpPerformances.Where(x => x.BusinessUnitId == businessUnitId).ToList();
                }
                if (!string.IsNullOrEmpty(status))
                {
                    EmpPerformances = EmpPerformances.Where(x => x.ResponsibilityLevel.ToString() == status).ToList();

                }

                var uniqueEmployeePerformances = EmpPerformances.DistinctByField(x => x.EmployeeId).ToList();

                foreach (var place in uniqueEmployeePerformances)
                {
                    

                    var employeePerformance = new EmployeePerfromanceLeaveDispilineBonusSummaryVM();

                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                    if (person != null)
                    {
                        employeePerformance.EmployeeId = person.EmployeeCode;

                        employeePerformance.Sex = person.Gender.ToString();
                        employeePerformance.FullName = person.FullNameaEnglish;
                    }

                    var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                    if (deptt != null)
                    {
                        employeePerformance.Departement = deptt.Name;
                    }

                    var title = db.Lookups.FirstOrDefault(x => x.LookupId == place.JobTitleId);
                    if (title != null)
                    {
                        employeePerformance.JobTitle = title.Description;
                    }
                    var leaves = db.Leaves.Where(x => x.EmployeeId == place.EmployeeId)
                                                            .Where(x => x.GlFiscalYearId == place.GlFiscalYearId).ToList();
                    employeePerformance.LeaveTaken= leaves.Any()? leaves.Sum(x=>x.Amount):0;
                    var months =db.GLFiscalYears
                         .Where(x => x.GlFiscalYearId == place.GlFiscalYearId).ToList();
                    employeePerformance.NumberOfMonthsServed = months.Any()? months.Sum(x => (DateTime.Now.Year - x.DateFrom.Year) * 12 + DateTime.Now.Month - x.DateFrom.Month):0;
                    var disciplines = db.DisciplinaryCases.Where(x => x.EmployeeId == place.EmployeeId
                                                                 && x.TransgressionDate.Year == DateTime.Now.Year).ToList();
                    employeePerformance.DisplineDays = disciplines.Any()? disciplines.Count():0;

                    employeePerformance.Bonus = 0;
                    employeePerformance.Level = "";
                    employeePerformance.Remark = place.Remark;
                    var EmpPerformanceQuarters = EmpPerformances.Where(x=>x.EmployeeId == place.EmployeeId).DistinctByField(x=>x.GlPeriodId);

                    employeePerformance.EvaluationPeiordList = new List<EvaluationLeaveDispilineBonusPeiordVM>();

                    foreach (var quart in EmpPerformanceQuarters)
                    {
                        var evaluationPeiord = new EvaluationLeaveDispilineBonusPeiordVM();
                        var quarter = db.Quarters.Find(quart.GlPeriodId);
                        if (quarter != null)
                        {
                            evaluationPeiord.Period = quarter.Name;
                        }
                        evaluationPeiord.EvaluationCriteriaList = new List<EvaluationLeaveDispilineCriteriaVM>();
                        
                        var employeePerfs = EmpPerformances.Where(x => x.EmployeeId == place.EmployeeId && x.GlPeriodId == quart.GlPeriodId);

                        foreach (var employeePerf in employeePerfs)
                        {
                            var criterias = new EvaluationLeaveDispilineCriteriaVM();
                            var criteria = db.MeasurementDefinitions.Find(employeePerf.EvaluationCriteriaTypeId);
                            if (criteria != null)
                            {
                                criterias.Measurment = criteria.EvaluationCriteria + "-" + criteria.Role.ToString();
                                criterias.Weight = GetPerformanceWeight(employeePerf.EmployeePerformanceId);
                                criterias.Score = GetPerformanceScore(employeePerf.EmployeePerformanceId);
                                evaluationPeiord.EvaluationCriteriaList.Add(criterias);
                            }
                                
                        }
                        if (evaluationPeiord.EvaluationCriteriaList.Any())
                        {
                            evaluationPeiord.CriteriaTotalScore = evaluationPeiord.EvaluationCriteriaList.Sum(x => x.Score);
                            evaluationPeiord.CriteriaTotalWeight = evaluationPeiord.EvaluationCriteriaList.Sum(x => x.Weight);
                        }

                        employeePerformance.EvaluationPeiordList.Add(evaluationPeiord);

                    }

                    if (employeePerformance.EvaluationPeiordList.Any())
                    {
                        employeePerformance.TotalScoreInThePeriod = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalScore);
                        totalResult = totalResult + employeePerformance.TotalScoreInThePeriod;
                    }
                    if (employeePerformance.EvaluationPeiordList.Any())
                    {
                        employeePerformance.Total = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalScore);
                        employeePerformance.TotalWeight = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalWeight);
                        employeePerformance.Percent = employeePerformance.TotalWeight>0? (employeePerformance.Total * 100) / employeePerformance.TotalWeight:0;
                        var rank = db.PerformanceRank.FirstOrDefault(x => x.Minimum <= employeePerformance.Percent && x.Maximum >= employeePerformance.Percent);
                        if (rank != null)
                        {
                            employeePerformance.Rank = rank.Description;
                        }
                    }
                    
                    employeePerformanceList.Add(employeePerformance);
                    

                }
                if (!string.IsNullOrEmpty(rankName))
                {
                    employeePerformanceList = employeePerformanceList.Where(x => x.Rank == rankName).ToList();
                }
            }
            return employeePerformanceList;
        }

        public decimal GetMeasurmentScore(int empMeasurmentId)
        {
            var empScores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == empMeasurmentId);

            if (empScores.Any())
            {
                return empScores.Sum(x => x.Score);
            }

            return 0;
        }
        public decimal GetPerformanceScore(int empPerformanceId)
        {
            decimal totalScore = 0;

            var empMeasurs = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == empPerformanceId).ToList();

            foreach (var em in empMeasurs)
            {
                var scores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();

                if (scores.Any())
                {
                    totalScore = totalScore + scores.Sum(x => x.Score);
                }
            }

            return totalScore;
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

    }
}