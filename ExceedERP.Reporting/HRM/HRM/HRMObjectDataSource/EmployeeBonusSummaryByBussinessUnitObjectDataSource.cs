using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeBonusSummaryByBussinessUnitObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeBonusSummaryByBussinessUnitVM> GetEmployeeBonusSummary(
                                                                            string empId, string branchId, string orgStructureId, string year,
                                                                            string quarterId, string status, string BusinessUnit, object[] jobTitles, string rankName)
        {
            var result = new List<EmployeeBonusSummaryByBussinessUnitVM>();

            if (string.IsNullOrEmpty(year))
                return result;

            int.TryParse(year, out int yearId);

            // 1️⃣ Preload all needed data into dictionaries
            var empPerformances = db.EmployeePerformances
                .Where(x => x.IsHRApproved)
                .ToList();

            HashSet<int> fiscalYearIds = new HashSet<int>( db.QuarterLengths
                .Where(x => x.QuarterLengthId == yearId)
                .Select(x => x.GlFiscalYearId));

            empPerformances = empPerformances
                .Where(x => fiscalYearIds.Contains(x.GlFiscalYearId))
                .ToList();

            if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int empIdInt))
                empPerformances = empPerformances.Where(x => x.EmployeeId == empIdInt).ToList();

            if (!string.IsNullOrEmpty(quarterId) && int.TryParse(quarterId, out int qId))
                empPerformances = empPerformances.Where(x => x.GlPeriodId == qId).ToList();

            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int bId))
                empPerformances = empPerformances.Where(x => x.BranchId == bId).ToList();

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int oId))
                empPerformances = empPerformances.Where(x => x.OrgStructureId == oId).ToList();

            if (!string.IsNullOrEmpty(BusinessUnit) && int.TryParse(BusinessUnit, out int buId))
                empPerformances = empPerformances.Where(x => x.BusinessUnitId == buId).ToList();

            if (jobTitles != null)
            {
                HashSet<int> jobIds = new HashSet<int>( jobTitles.Select(Convert.ToInt32));
                empPerformances = empPerformances.Where(x => jobIds.Contains(x.JobTitleId)).ToList();
            }

            if (!string.IsNullOrEmpty(status))
                empPerformances = empPerformances.Where(x => x.ResponsibilityLevel.ToString() == status).ToList();

            // 2️⃣ Load related data once into dictionaries
            var persons = db.Person_Employee.ToDictionary(x => x.EmployeeId);
            var departments = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
            var titles = db.Lookups.ToDictionary(x => x.LookupId);
            var fiscalYears = db.GLFiscalYears.ToDictionary(x => x.GlFiscalYearId);
            var quarters = db.Quarters.ToDictionary(x => x.QuarterId);

            // 3️⃣ Preload all measurements & scores into dictionaries
            var empMeasurements = db.EmployeeMeasurement.ToList();
            var measurementScores = db.EmployeePerformanceScores.ToList();

            var scoresByPerfId = empMeasurements
                .GroupBy(m => m.EmployeePerformanceId)
                .ToDictionary(
                    g => g.Key,
                    g => measurementScores
                        .Where(s => g.Select(m => m.EmpMeasurementId).Contains(s.EmployeeMeasurementId))
                        .ToList()
                );

            // 4️⃣ Process unique employees
            var ranks = PerformanceReportLookup.LoadRanks(db);
            foreach (var place in empPerformances.GroupBy(x => x.EmployeeId).Select(g => g.First()))
            {
                var vm = new EmployeeBonusSummaryByBussinessUnitVM();

                // Fast O(1) dictionary lookups
                if (persons.TryGetValue(place.EmployeeId, out var person))
                {
                    vm.EmployeeId = person.EmployeeCode;
                    vm.FullName = $"{person.EmployeeCode}-{person.FullNameaEnglish}";
                }

                if (departments.TryGetValue(place.OrgStructureId, out var dept))
                    vm.Departement = dept.Name;

                if (titles.TryGetValue(place.JobTitleId, out var title))
                    vm.JobTitle = title.Description;

                if (fiscalYears.TryGetValue(place.GlFiscalYearId, out var fy) && person != null)
                {
                    DateTime startDate = fy.DateFrom > person.HiredDate ? person.HiredDate : fy.DateFrom;
                    vm.DatesServedInThisFiscalYear = (decimal)(fy.DateTo - startDate).TotalDays;
                }

                // Example: Calculate scores directly from preloaded dictionary
                decimal corpScore = 0, corpWeight = 0;
                if (scoresByPerfId.TryGetValue(place.EmployeePerformanceId, out var perfScores))
                {
                    corpScore = perfScores.Sum(s => s.Score);
                    corpWeight = perfScores.Sum(s => s.Weight);
                }

                // ... repeat for sector, direct boss, job title with pre-filtered IDs

                vm.TotalScoreOutOf100 = corpScore; // + other scores
                vm.Bonus = PerformanceReportLookup.CalculateBonus(ranks, vm.DatesServedInThisFiscalYear, vm.TotalScoreOutOf100);

                result.Add(vm);
            }

            return result;
        }
       

        //public List<EmployeeBonusSummaryByBussinessUnitVM> GetEmployeeBonusSummary(string empId,string branchId, string orgStructureId, string year,string quarterId, string status,string BusinessUnit,object[] jobTitles, string rankName)
        //{
        //    List<EmployeeBonusSummaryByBussinessUnitVM> employeePerformanceList = new List<EmployeeBonusSummaryByBussinessUnitVM>();


        //    if (!string.IsNullOrEmpty(year))
        //    {
        //        int.TryParse(year, out int yearId);

        //        var EmpPerformances = db.EmployeePerformances.Where(x => x.IsHRApproved).ToList();
        //        var GlFiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == yearId).Select(x => x.GlFiscalYearId);
        //        if (GlFiscalYearIds.Any()&& EmpPerformances.Any())
        //        {
        //            EmpPerformances = EmpPerformances.Where(x => GlFiscalYearIds.Contains(x.GlFiscalYearId)).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(empId))
        //        {
        //            int.TryParse(empId, out int employeeId);
        //            EmpPerformances = EmpPerformances.Where(x => x.EmployeeId == employeeId).ToList();

        //        }
        //        if (!string.IsNullOrEmpty(quarterId))
        //        {
        //            int.TryParse(quarterId, out int quartId);
        //            EmpPerformances = EmpPerformances.Where(x => x.GlPeriodId == quartId).ToList();

        //        }
        //        if (!string.IsNullOrEmpty(branchId))
        //        {
        //            int.TryParse(branchId, out int braId);
        //            EmpPerformances = EmpPerformances.Where(x => x.BranchId == braId).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(orgStructureId))
        //        {
        //            int.TryParse(orgStructureId, out int orgId);
        //            EmpPerformances = EmpPerformances.Where(x => x.OrgStructureId == orgId).ToList();
        //        }

        //        if (!string.IsNullOrEmpty(BusinessUnit))
        //        {
        //            int.TryParse(BusinessUnit, out int businessUnitId);
        //            EmpPerformances = EmpPerformances.Where(x => x.BusinessUnitId == businessUnitId).ToList();
        //        }
        //        if (jobTitles != null)
        //        {
        //            List<int> jIds = jobTitles.Select(pp => Convert.ToInt32(pp)).ToList();
        //            if(jIds.Any())
        //                EmpPerformances = EmpPerformances.Where(x => jIds.Contains(x.JobTitleId)).ToList();

        //        }
        //        if (!string.IsNullOrEmpty(status))
        //        {
        //            EmpPerformances = EmpPerformances.Where(x => x.ResponsibilityLevel.ToString() == status).ToList();

        //        }

        //        var uniqueEmployeePerformances = EmpPerformances.DistinctByField(x => x.EmployeeId).ToList();

        //        foreach (var place in uniqueEmployeePerformances)
        //        {


        //            var employeePerformance = new EmployeeBonusSummaryByBussinessUnitVM();
        //            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
        //            if (person != null)
        //            {
        //                employeePerformance.EmployeeId = person.EmployeeCode;
        //                employeePerformance.FullName = person.EmployeeCode+"-"+person.FullNameaEnglish;
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
        //            var fiscalyear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == place.GlFiscalYearId);
        //            if (fiscalyear != null&&person !=null)
        //            {
        //                DateTime startDate = fiscalyear.DateFrom > person.HiredDate ? person.HiredDate : fiscalyear.DateFrom;
        //                employeePerformance.DatesServedInThisFiscalYear = (decimal)(fiscalyear.DateTo- startDate).TotalDays;
        //            }
        //            var EmpPerformanceQuarters = EmpPerformances
        //                                           .Where(x=>x.EmployeeId == place.EmployeeId)
        //                                                 .DistinctByField(x=>x.GlPeriodId)
        //                                                 .OrderBy(x=>x.GlPeriodId);

        //            employeePerformance.EvaluationPeiordBonusSummaries = new List<EvaluationPeiordBusinessUnitBonusSummaryVM>();
        //            foreach (var quart in EmpPerformanceQuarters)
        //            {
        //                decimal corpWeight = 0;
        //                decimal corpScore = 0;
        //                decimal sectorWeight = 0;
        //                decimal SectorScore = 0;
        //                decimal directWeight = 0;
        //                decimal directScore = 0;
        //                decimal jobTitleWeight = 0;
        //                decimal jobTitleScore = 0;
        //                var evaluationPeiord = new EvaluationPeiordBusinessUnitBonusSummaryVM();
        //                var quarter = db.Quarters.Find(quart.GlPeriodId);
        //                if (quarter != null)
        //                {
        //                        evaluationPeiord.Period = quarter.Name;
        //                }

        //                var lookUpIds = db.Lookups.Where(x => x.EvalutionType == EvaluationType.Corporate_Result
        //                                                && x.LookupType == LookUpTypes.EVALUATION_TYPE).Select(x => x.LookupId);
        //                var criteriaIds = db.MeasurementDefinitions.Where(x => lookUpIds.Contains(x.LookupId))
        //                                                 .Select(x => x.MeasurementDefinitionId);
        //                var CorpBScPerformances = db.EmployeePerformances
        //                                        .Where(x => criteriaIds.Contains(x.EvaluationCriteriaTypeId) 
        //                                                  && x.IsHRApproved
        //                                                  && x.GlPeriodId == quart.GlPeriodId
        //                                                  && x.EmployeeId == place.EmployeeId
        //                                                  && x.GlFiscalYearId == quart.GlFiscalYearId).ToList();

        //                foreach (var employeePerf in CorpBScPerformances)
        //                {

        //                    corpWeight += GetPerformanceWeight(employeePerf.EmployeePerformanceId);
        //                    corpScore += GetPerformanceScore(employeePerf.EmployeePerformanceId);

        //                }
        //                evaluationPeiord.AverageCorpBScOutOf20 = corpWeight != 0 ? (corpScore * 20) / corpWeight : 0;

        //                lookUpIds = db.Lookups.Where(x => x.EvalutionType == EvaluationType.Sector_Result
        //                                                 && x.LookupType == LookUpTypes.EVALUATION_TYPE)
        //                                                 .Select(x => x.LookupId);
        //                var sectorCriteriaIds = db.MeasurementDefinitions.Where(x => lookUpIds.Contains(x.LookupId))
        //                                        .Select(x => x.MeasurementDefinitionId);
        //                var SectorPerformances = db.EmployeePerformances
        //                                                .Where(x => sectorCriteriaIds.Contains(x.EvaluationCriteriaTypeId)
        //                                                  && x.GlPeriodId == quart.GlPeriodId
        //                                                  && x.EmployeeId == place.EmployeeId
        //                                                   && x.IsHRApproved
        //                                                  && x.GlFiscalYearId == quart.GlFiscalYearId).ToList();
        //                foreach (var employeePerf in SectorPerformances)
        //                {

        //                    sectorWeight += GetPerformanceWeight(employeePerf.EmployeePerformanceId);
        //                    SectorScore += GetPerformanceScore(employeePerf.EmployeePerformanceId);

        //                }
        //                evaluationPeiord.AverageSectorBScOutOf30 = sectorWeight != 0 ? SectorScore * 30 / sectorWeight : 0;

        //                lookUpIds = db.Lookups.Where(x => x.EvalutionType == EvaluationType.DirectBoss
        //                                                && x.LookupType == LookUpTypes.EVALUATION_TYPE).Select(x => x.LookupId);
        //                var directCriteriaIds = db.MeasurementDefinitions.Where(x => lookUpIds.Contains(x.LookupId)).Select(x => x.MeasurementDefinitionId);
        //                //var directCriteriaIds = db.MeasurementDefinitions.Where(x => x.EvalutionType == EvaluationType.DirectBoss).Select(x => x.MeasurementDefinitionId);
        //                var directPerformances = db.EmployeePerformances
        //                                            .Where(x => directCriteriaIds.Contains(x.EvaluationCriteriaTypeId)
        //                                                   && x.GlPeriodId == quart.GlPeriodId
        //                                                   && x.EmployeeId == place.EmployeeId
        //                                                   && x.IsHRApproved
        //                                                   && x.GlFiscalYearId == quart.GlFiscalYearId).ToList();
        //                foreach (var employeePerf in SectorPerformances)
        //                {

        //                    directWeight += GetPerformanceWeight(employeePerf.EmployeePerformanceId);
        //                    directScore += GetPerformanceScore(employeePerf.EmployeePerformanceId);

        //                }
        //                evaluationPeiord.EvaluationResultOf20 = directWeight != 0 ? directScore * 20 / directWeight : 0;

        //                lookUpIds = db.Lookups.Where(x => x.EvalutionType == EvaluationType.MainDepartmentLevel
        //                                                && x.LookupType == LookUpTypes.EVALUATION_TYPE)
        //                                                .Select(x => x.LookupId);
        //                var jobtitleCriteriaIds = db.MeasurementDefinitions.Where(x => lookUpIds.Contains(x.LookupId)).Select(x => x.MeasurementDefinitionId);
        //                var jobtitlePerformances = db.EmployeePerformances
        //                                                .Where(x => jobtitleCriteriaIds.Contains(x.EvaluationCriteriaTypeId)
        //                                                   && x.GlPeriodId == quart.GlPeriodId
        //                                                   && x.EmployeeId == place.EmployeeId
        //                                                   && x.IsHRApproved
        //                                                   && x.GlFiscalYearId == quart.GlFiscalYearId).ToList();

        //                foreach (var employeePerf in jobtitlePerformances)
        //                {

        //                    jobTitleWeight += GetPerformanceWeight(employeePerf.EmployeePerformanceId);
        //                    jobTitleScore += GetPerformanceScore(employeePerf.EmployeePerformanceId);

        //                }
        //                evaluationPeiord.AverageJobTitleBScOutOf30 = jobTitleWeight != 0 ? jobTitleScore * 30 / jobTitleWeight : 0;
        //                employeePerformance.EvaluationPeiordBonusSummaries.Add(evaluationPeiord);
        //            }
        //            if (employeePerformance.EvaluationPeiordBonusSummaries.Any())
        //            {
        //                employeePerformance.AverageCorpBScOutOf20 = employeePerformance.EvaluationPeiordBonusSummaries
        //                    .Where(x => x.AverageCorpBScOutOf20 != 0)
        //                    .Select(x => x.AverageCorpBScOutOf20)
        //                    .DefaultIfEmpty(0) // Provide a default value if the list is empty
        //                    .Average();

        //                employeePerformance.AverageSectorBScOutOf30 = employeePerformance.EvaluationPeiordBonusSummaries
        //                    .Where(x => x.AverageSectorBScOutOf30 != 0)
        //                    .Select(x => x.AverageSectorBScOutOf30)
        //                    .DefaultIfEmpty(0)
        //                    .Average();

        //                employeePerformance.AverageOf20 = employeePerformance.EvaluationPeiordBonusSummaries
        //                    .Where(x => x.EvaluationResultOf20 != 0)
        //                    .Select(x => x.EvaluationResultOf20)
        //                    .DefaultIfEmpty(0)
        //                    .Average();

        //                employeePerformance.AverageJobTitleBScOutOf30 = employeePerformance.EvaluationPeiordBonusSummaries
        //                    .Where(x => x.AverageJobTitleBScOutOf30 != 0)
        //                    .Select(x => x.AverageJobTitleBScOutOf30)
        //                    .DefaultIfEmpty(0)
        //                    .Average();
        //            }


        //            employeePerformance.TotalScoreOutOf100 = employeePerformance.AverageCorpBScOutOf20 
        //                                                        + employeePerformance.AverageSectorBScOutOf30
        //                                                        + employeePerformance .AverageJobTitleBScOutOf30
        //                                                        + employeePerformance.AverageOf20;
        //            if (fiscalyear != null && person != null)
        //            {
        //                DateTime startDate = fiscalyear.DateFrom < person.HiredDate ? person.HiredDate : fiscalyear.DateFrom;
        //                employeePerformance.DatesServedInThisFiscalYear = (decimal)(fiscalyear.DateTo - startDate).TotalDays;
        //                // Adjust DatesServedInThisFiscalYear based on the value of pagume
        //                if (employeePerformance.DatesServedInThisFiscalYear >= 311 && employeePerformance.DatesServedInThisFiscalYear <= 315)
        //                {
        //                    employeePerformance.DatesServedInThisFiscalYear -= (int)(employeePerformance.DatesServedInThisFiscalYear - 311);
        //                }
        //                else if (employeePerformance.DatesServedInThisFiscalYear > 315)
        //                {
        //                    employeePerformance.DatesServedInThisFiscalYear -= 4;
        //                }

        //            }
        //            // Ensure DatesServedInThisFiscalYear does not exceed 360 days
        //            if (employeePerformance.DatesServedInThisFiscalYear > 360)
        //            {
        //                employeePerformance.DatesServedInThisFiscalYear = 360;
        //            }
        //            employeePerformance.Bonus = CalculateResult(employeePerformance.DatesServedInThisFiscalYear, employeePerformance.TotalScoreOutOf100);
        //            employeePerformanceList.Add(employeePerformance);

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