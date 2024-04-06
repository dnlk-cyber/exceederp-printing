using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using XAPI;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeBSCPerforObjectDataSource : List<EmployeeBscPerfromanceVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeBscPerfromanceVM> GetEmployeeBSCPerformance(string branchId, string orgStructureId, string year, string status,string BusinessUnit,string rankName)
        {
            List<EmployeeBscPerfromanceVM> employeePerformanceList = new List<EmployeeBscPerfromanceVM>();

            int.TryParse(year, out int yearId);

            decimal totalResult = 0;
            decimal totalWeight = 0;

            var placements = db.Placements.Where(x => x.isCurrent).ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId).OrderByDescending(x => x.AssignmentDate).ToList();
            }

            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_FromPlacement(placements, orgId).OrderByDescending(x => x.AssignmentDate).ToList();
            }


            var performances = db.EmployeePerformanceScores.ToList();

            foreach (var place in placements)
            {
                totalResult = 0;
                totalWeight = 0;

                var employeePerformance = new EmployeeBscPerfromanceVM();

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

                var title = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
                if (title != null)
                {
                    employeePerformance.JobTitle = title.Description;
                }

                var quarterLength = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId == yearId);

                if (quarterLength != null)
                {
                    var quarters = db.Quarters.Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId).ToList();

                    employeePerformance.EvaluationPeiordList = new List<EvaluationPeiordVM>();

                    foreach (var quart in quarters)
                    {
                        var evaluationPeiord = new EvaluationPeiordVM();

                        evaluationPeiord.Period = quart.Name;

                        var employeePerf = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == place.EmployeeId && x.GlPeriodId == quart.QuarterId);

                        if (employeePerf != null)
                        {
                            var employeeMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerf.EmployeePerformanceId).ToList();

                            evaluationPeiord.EvaluationCriteriaList = new List<EvaluationCriteriaVM>();

                            foreach (var em in employeeMeasurments)
                            {
                                var criteria = new EvaluationCriteriaVM();

                                var measurment = db.Measurements.FirstOrDefault(x => x.MeasurementId == em.MeasurmentId);
                                if (measurment != null)
                                {
                                    criteria.Measurment = measurment.MeasurementName;
                                    criteria.Score = GetMeasurmentScore(em.EmpMeasurementId);
                                    criteria.Weight = em.Weight;
                                    totalWeight = totalWeight + em.Weight;
                                }

                                evaluationPeiord.EvaluationCriteriaList.Add(criteria);
                            }

                            if (evaluationPeiord.EvaluationCriteriaList.Any())
                            {
                                evaluationPeiord.CriteriaTotalScore = evaluationPeiord.EvaluationCriteriaList.Sum(x => x.Score);
                            }
                        }


                        employeePerformance.EvaluationPeiordList.Add(evaluationPeiord);

                    }

                    if (employeePerformance.EvaluationPeiordList.Any())
                    {
                        employeePerformance.TotalScoreInThePeriod = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalScore);
                        totalResult = totalResult + employeePerformance.TotalScoreInThePeriod;
                    }

                }

                employeePerformance.Total = totalResult;

                if(totalWeight > 0)
                {
                    employeePerformance.Percent = (totalResult * 100) / totalWeight;

                    var rank = db.PerformanceRank.FirstOrDefault(x => x.Minimum <= employeePerformance.Percent && x.Maximum >= employeePerformance.Percent);
                    if (rank != null)
                    {
                        employeePerformance.Rank = rank.Description;
                    }
                }
                
                if (status == "Reviewed" && totalResult > 0)
                {
                    employeePerformanceList.Add(employeePerformance);
                }
                else if (status == "Not Reviewed" && totalResult == 0)
                {
                    employeePerformanceList.Add(employeePerformance);
                }
                else if (status == null)
                {
                    employeePerformanceList.Add(employeePerformance);
                }

            }
            if (!string.IsNullOrEmpty(rankName))
            {
                employeePerformanceList = employeePerformanceList.Where(x => x.Rank == rankName).ToList();
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


    }
}