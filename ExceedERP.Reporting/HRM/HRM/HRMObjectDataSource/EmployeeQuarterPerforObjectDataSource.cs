using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeQuarterPerforObjectDataSource : List<EmployeeQuarterPerfromance>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeQuarterPerfromance> GetEmployeeQuarterPerformance(string fiscalYear, string quarterId  , string branchId, string orgStructureId, string businessUnitId,  string status)
        {
            List<EmployeeQuarterPerfromance> employeePerformanceList = new List<EmployeeQuarterPerfromance>();

            int.TryParse(fiscalYear, out int yearId);

            decimal totalResult = 0;

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

            if (businessUnitId != null)
            {
                int.TryParse(businessUnitId, out int businesUnitId);
                EmpPerformances = EmpPerformances.Where(x => x.BusinessUnitId == businesUnitId).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                EmpPerformances = EmpPerformances.Where(x => x.ResponsibilityLevel.ToString() == status).ToList();

            }

            var uniqueEmployeePerformances = EmpPerformances.DistinctByField(x => x.EmployeeId).ToList();

            //if (empId != null)
            //{
            //    int.TryParse(empId, out int employeeId);
            //    placements = placements.Where(x => x.EmployeeId == employeeId).ToList();
            //}

            foreach (var place in uniqueEmployeePerformances)
            {
                totalResult = 0;

                var empPerformance = new EmployeeQuarterPerfromance();

                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                if (emp != null)
                {
                    empPerformance.EmployeeId = emp.EmployeeCode;
                    empPerformance.FullName = emp.FullNameaEnglish;
                    empPerformance.Sex = emp.Gender.ToString();
                }

                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                if (mainCompany != null)
                {
                    empPerformance.Department = mainCompany.Name;
                }
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                if (branch != null)
                {
                    empPerformance.Branch = branch.Name;
                }
                empPerformance.QurterList = new List<QurterList>();

                var EmpPerformancequarterLengths = EmpPerformances.Where(x => x.EmployeeId == place.EmployeeId).DistinctByField(x=>x.GlPeriodId);

                foreach (var empLength in  EmpPerformancequarterLengths)
                {

                    var quarters = EmpPerformances.Where(x => x.EmployeeId == empLength.EmployeeId &&x.GlPeriodId==empLength.GlPeriodId).ToList();

                    foreach (var quar in quarters)
                    {
                        var quarter = new QurterList();
                        var perod = db.Quarters.Find(quar.GlPeriodId); 
                        if(perod != null)
                        {
                            quarter.PeriodName = perod.Name;

                        }
                        var measureDefinition = db.MeasurementDefinitions.Find(quar.EvaluationCriteriaTypeId);
                        if(measureDefinition != null)
                        {
                            quarter.Criateria = measureDefinition.EvaluationCriteria;

                        }

                       
                        quarter.Weight = GetPerformanceWeight(quar.EmployeePerformanceId);
                        quarter.Score = GetPerformanceScore(quar.EmployeePerformanceId);
                        empPerformance.QurterList.Add(quarter);
                    }
                   


                }
                if (empPerformance.QurterList.Any())
                {
                    empPerformance.Total = empPerformance.QurterList.Sum(x => x.Score);
                    empPerformance.Totalweight = empPerformance.QurterList.Sum(x => x.Weight);
                    empPerformance.TotaOutOf100 = empPerformance.Totalweight > 0 ? (empPerformance.Total * 100) / empPerformance.Totalweight : 0;
                }
                empPerformance.Role = place.ResponsibilityLevel.ToString();
                employeePerformanceList.Add(empPerformance);
                
            }

            return employeePerformanceList;
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
            decimal totalWeight = 0;

            var empMeasurs = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == empPerformanceId).ToList();

            foreach (var em in empMeasurs)
            {
                var scores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId).ToList();

                if (scores.Any())
                {
                    totalWeight +=  scores.Sum(x => x.Weight);
                }
            }

            return totalWeight;
        }
    }
}