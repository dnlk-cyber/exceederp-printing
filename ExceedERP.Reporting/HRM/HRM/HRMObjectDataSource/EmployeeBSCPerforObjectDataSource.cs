using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeBSCPerforObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeBscPerfromanceVM> GetEmployeeBSCPerformance(string branchId, string orgStructureId,string year, string quarterId, string status, string BusinessUnit, string rankName)
        {
            var employeePerformanceList = new List<EmployeeBscPerfromanceVM>();
            decimal totalResult = 0;

            if (string.IsNullOrEmpty(year))
                return employeePerformanceList;

            int.TryParse(year, out int yearId);

            // Step 1: Filter early
            var fiscalYearIds = db.QuarterLengths
                .Where(x => x.QuarterLengthId == yearId)
                .Select(x => x.GlFiscalYearId)
                .ToList();

            var query = db.EmployeePerformances
                .Where(x => x.IsSecondLevelApproved);

            if (fiscalYearIds.Any())
                query = query.Where(x => fiscalYearIds.Contains(x.GlFiscalYearId));

            if (int.TryParse(quarterId, out int quartId))
                query = query.Where(x => x.GlPeriodId == quartId);

            if (int.TryParse(branchId, out int braId))
                query = query.Where(x => x.BranchId == braId);

            if (int.TryParse(orgStructureId, out int orgId))
                query = query.Where(x => x.OrgStructureId == orgId);

            if (int.TryParse(BusinessUnit, out int businessUnitId))
                query = query.Where(x => x.BusinessUnitId == businessUnitId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.ResponsibilityLevel.ToString() == status);

            // Step 2: Materialize only filtered data once
            var empPerformances = query.ToList();

            if (!empPerformances.Any())
                return employeePerformanceList;

            // Step 3: Preload all related data at once
            var employeeIds = empPerformances.Select(x => x.EmployeeId).Distinct().ToList();
            var orgIds = empPerformances.Select(x => x.OrgStructureId).Distinct().ToList();
            var titleIds = empPerformances.Select(x => x.JobTitleId).Distinct().ToList();
            var glPeriodIds = empPerformances.Select(x => x.GlPeriodId).Distinct().ToList();
            var perfIds = empPerformances.Select(x => x.EmployeePerformanceId).ToList();

            var persons = db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToDictionary(x => x.EmployeeId, x => x);

            var departments = db.MainCompanyStructures
                .Where(x => orgIds.Contains(x.OrgStructureId))
                .ToDictionary(x => x.OrgStructureId, x => x.Name);

            var titles = db.Lookups
                .Where(x => titleIds.Contains(x.LookupId))
                .ToDictionary(x => x.LookupId, x => x.Description);

            var quarters = db.Quarters
                .Where(x => glPeriodIds.Contains(x.QuarterId))
                .ToDictionary(x => x.QuarterId, x => x.Name);

            var perfRanks = db.PerformanceRank.ToList();

            // Preload measurements and scores for all performances
            var allMeasurements = db.EmployeeMeasurement
                .Where(x => perfIds.Contains(x.EmployeePerformanceId))
                .ToList();

            var measureIds = allMeasurements.Select(x => x.EmpMeasurementId).ToList();
            var allScores = db.EmployeePerformanceScores
                .Where(x => measureIds.Contains(x.EmployeeMeasurementId))
                .ToList();

            var allCriteria = db.MeasurementDefinitions
                .ToDictionary(x => x.MeasurementDefinitionId, x => x);

            // Step 4: Group data by Employee
            var grouped = empPerformances.GroupBy(x => x.EmployeeId);

            foreach (var group in grouped)
            {
                var place = group.First();
                var employeePerformance = new EmployeeBscPerfromanceVM
                {
                    EvaluationPeiordList = new List<EvaluationPeiordVM>()
                };

                // Person info
                if (persons.TryGetValue(place.EmployeeId, out var person))
                {
                    employeePerformance.EmployeeId = person.EmployeeCode;
                    employeePerformance.Sex = person.Gender.ToString();
                    employeePerformance.FullName = person.FullNameaEnglish;
                }

                if (departments.TryGetValue(place.OrgStructureId, out var deptName))
                    employeePerformance.Departement = deptName;

                if (titles.TryGetValue(place.JobTitleId, out var title))
                    employeePerformance.JobTitle = title;

                // Step 5: Loop by quarter
                var perQuarter = group.GroupBy(x => x.GlPeriodId).OrderBy(x => x.Key);
                foreach (var quarterGroup in perQuarter)
                {
                    var evaluationPeriod = new EvaluationPeiordVM
                    {
                        EvaluationCriteriaList = new List<EvaluationCriteriaVM>(),
                        Period = quarters.TryGetValue(quarterGroup.Key, out var qName) ? qName : ""
                    };

                    // Each performance in that period
                    foreach (var perf in quarterGroup)
                    {
                        if (!allMeasurements.Any(m => m.EmployeePerformanceId == perf.EmployeePerformanceId))
                            continue;

                        var relatedMeasures = allMeasurements
                            .Where(m => m.EmployeePerformanceId == perf.EmployeePerformanceId)
                            .Select(m => new
                            {
                                m.EmpMeasurementId,
                                Scores = allScores.Where(s => s.EmployeeMeasurementId == m.EmpMeasurementId).ToList()
                            });

                        foreach (var measure in relatedMeasures)
                        {
                            var criteriaDef = allCriteria.ContainsKey(perf.EvaluationCriteriaTypeId)
                                ? allCriteria[perf.EvaluationCriteriaTypeId]
                                : null;

                            if (criteriaDef == null) continue;

                            var vm = new EvaluationCriteriaVM
                            {
                                Measurment = $"{criteriaDef.EvaluationCriteria}-{criteriaDef.Role}",
                                Weight = measure.Scores.Sum(s => s.Weight),
                                Score = measure.Scores.Sum(s => s.Score)
                            };
                            evaluationPeriod.EvaluationCriteriaList.Add(vm);
                        }
                    }

                    if (evaluationPeriod.EvaluationCriteriaList.Any())
                    {
                        evaluationPeriod.CriteriaTotalScore = evaluationPeriod.EvaluationCriteriaList.Sum(x => x.Score);
                        evaluationPeriod.CriteriaTotalWeight = evaluationPeriod.EvaluationCriteriaList.Sum(x => x.Weight);
                        employeePerformance.EvaluationPeiordList.Add(evaluationPeriod);
                    }
                }

                if (employeePerformance.EvaluationPeiordList.Any())
                {
                    employeePerformance.Total = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalScore);
                    employeePerformance.TotalWeight = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalWeight);
                    employeePerformance.Percent = employeePerformance.TotalWeight > 0
                        ? (employeePerformance.Total * 100) / employeePerformance.TotalWeight
                        : 0;

                    var rank = perfRanks.FirstOrDefault(x =>
                        x.Minimum <= employeePerformance.Percent && x.Maximum >= employeePerformance.Percent);

                    if (rank != null)
                        employeePerformance.Rank = rank.Description;

                    totalResult += employeePerformance.Total;
                }

                employeePerformanceList.Add(employeePerformance);
            }

            if (!string.IsNullOrEmpty(rankName))
                employeePerformanceList = employeePerformanceList
                    .Where(x => x.Rank == rankName)
                    .ToList();

            return employeePerformanceList;
        }

        //public List<EmployeeBscPerfromanceVM> GetEmployeeBSCPerformance(string branchId, string orgStructureId, string year,string quarterId, string status,string BusinessUnit,string rankName)
        //{
        //    List<EmployeeBscPerfromanceVM> employeePerformanceList = new List<EmployeeBscPerfromanceVM>();


        //    decimal totalResult = 0;

        //    if (!string.IsNullOrEmpty(year))
        //    {
        //        int.TryParse(year, out int yearId);

        //        var EmpPerformances = db.EmployeePerformances.Where(x => x.IsSecondLevelApproved).ToList();
        //        var GlFiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == yearId).Select(x => x.GlFiscalYearId);
        //        if (GlFiscalYearIds.Any())
        //        {
        //            EmpPerformances = EmpPerformances.Where(x => GlFiscalYearIds.Contains(x.GlFiscalYearId)).ToList();
        //        }
        //        if (quarterId != null)
        //        {
        //            int.TryParse(quarterId, out int quartId);
        //            EmpPerformances = EmpPerformances.Where(x => x.GlPeriodId == quartId).ToList();

        //        }
        //        if (branchId != null)
        //        {
        //            int.TryParse(branchId, out int braId);
        //            EmpPerformances = EmpPerformances.Where(x => x.BranchId == braId).ToList();
        //        }
        //        if (orgStructureId != null)
        //        {
        //            int.TryParse(orgStructureId, out int orgId);
        //            EmpPerformances = EmpPerformances.Where(x => x.OrgStructureId == orgId).ToList();
        //        }

        //        if (BusinessUnit != null)
        //        {
        //            int.TryParse(BusinessUnit, out int businessUnitId);
        //            EmpPerformances = EmpPerformances.Where(x => x.BusinessUnitId == businessUnitId).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(status))
        //        {
        //            EmpPerformances = EmpPerformances.Where(x => x.ResponsibilityLevel.ToString() == status).ToList();

        //        }

        //        var uniqueEmployeePerformances = EmpPerformances.DistinctByField(x => x.EmployeeId).ToList();

        //        foreach (var place in uniqueEmployeePerformances)
        //        {


        //            var employeePerformance = new EmployeeBscPerfromanceVM();

        //            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
        //            if (person != null)
        //            {
        //                employeePerformance.EmployeeId = person.EmployeeCode;

        //                employeePerformance.Sex = person.Gender.ToString();
        //                employeePerformance.FullName = person.FullNameaEnglish;
        //            }

        //            var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
        //            if (deptt != null)
        //            {
        //                employeePerformance.Departement = deptt.Name;
        //            }

        //            var title = db.Lookups.FirstOrDefault(x => x.LookupId == place.JobTitleId);
        //            if (title != null)
        //            {
        //                employeePerformance.JobTitle = title.Description;
        //            }

        //            var EmpPerformanceQuarters = EmpPerformances.Where(x=>x.EmployeeId == place.EmployeeId).DistinctByField(x=>x.GlPeriodId).OrderBy(x=>x.GlPeriodId);

        //            employeePerformance.EvaluationPeiordList = new List<EvaluationPeiordVM>();

        //            foreach (var quart in EmpPerformanceQuarters)
        //            {
        //                    var evaluationPeiord = new EvaluationPeiordVM();
        //                    var quarter = db.Quarters.Find(quart.GlPeriodId);
        //                    if (quarter != null)
        //                    {
        //                        evaluationPeiord.Period = quarter.Name;
        //                    }
        //                    evaluationPeiord.EvaluationCriteriaList = new List<EvaluationCriteriaVM>();

        //                    var employeePerfs = EmpPerformances.Where(x => x.EmployeeId == place.EmployeeId && x.GlPeriodId == quart.GlPeriodId);

        //                    foreach (var employeePerf in employeePerfs)
        //                    {
        //                        var criterias = new EvaluationCriteriaVM();
        //                        var criteria = db.MeasurementDefinitions.Find(employeePerf.EvaluationCriteriaTypeId);
        //                        if (criteria != null)
        //                        {
        //                            criterias.Measurment = criteria.EvaluationCriteria + "-" + criteria.Role.ToString();
        //                            criterias.Weight = GetPerformanceWeight(employeePerf.EmployeePerformanceId);
        //                            criterias.Score = GetPerformanceScore(employeePerf.EmployeePerformanceId);
        //                            evaluationPeiord.EvaluationCriteriaList.Add(criterias);
        //                        }

        //                    }
        //                    if (evaluationPeiord.EvaluationCriteriaList.Any())
        //                    {
        //                        evaluationPeiord.CriteriaTotalScore = evaluationPeiord.EvaluationCriteriaList.Sum(x => x.Score);
        //                        evaluationPeiord.CriteriaTotalWeight = evaluationPeiord.EvaluationCriteriaList.Sum(x => x.Weight);
        //                    }

        //                    employeePerformance.EvaluationPeiordList.Add(evaluationPeiord);


        //            }

        //            if (employeePerformance.EvaluationPeiordList.Any())
        //            {
        //                employeePerformance.TotalScoreInThePeriod = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalScore);
        //                totalResult = totalResult + employeePerformance.TotalScoreInThePeriod;
        //            }
        //            if (employeePerformance.EvaluationPeiordList.Any())
        //            {
        //                employeePerformance.Total = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalScore);
        //                employeePerformance.TotalWeight = employeePerformance.EvaluationPeiordList.Sum(x => x.CriteriaTotalWeight);
        //                employeePerformance.Percent = employeePerformance.TotalWeight>0? (employeePerformance.Total * 100) / employeePerformance.TotalWeight:0;
        //                var rank = db.PerformanceRank.FirstOrDefault(x => x.Minimum <= employeePerformance.Percent && x.Maximum >= employeePerformance.Percent);
        //                if (rank != null)
        //                {
        //                    employeePerformance.Rank = rank.Description;
        //                }

        //            }

        //            employeePerformanceList.Add(employeePerformance);

        //        }
        //        if (!string.IsNullOrEmpty(rankName))
        //        {
        //            employeePerformanceList = employeePerformanceList.Where(x => x.Rank == rankName).ToList();
        //        }
        //    }
        //    return employeePerformanceList;
        //}


        private decimal GetPerformanceScore(int empPerformanceId)
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
        private decimal GetPerformanceWeight(int empPerformanceId)
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
        public void Dispose()
        {
            db.Dispose();
        }
    }
}