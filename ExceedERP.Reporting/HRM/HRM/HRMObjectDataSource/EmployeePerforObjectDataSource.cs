using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using XAPI;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerforObjectDataSource : List<EmployeePerfromanceVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceVM> GetEmployeePerformance(string branchId, string orgStructureId, string year, string quarterId, string status, string empId,string businessUnit)
        {
            List<EmployeePerfromanceVM> employeePerformanceList = new List<EmployeePerfromanceVM>();


            decimal totalResult = 0;
            if (!string.IsNullOrEmpty(year))
            {
                int.TryParse(year, out int yearId);

                var EmpPerformances = db.EmployeePerformances.Where(x => x.IsSecondLevelApproved).ToList();
                var GlFiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == yearId).Select(x => x.GlFiscalYearId);
                if (GlFiscalYearIds.Any())
                {
                    EmpPerformances = EmpPerformances.Where(x => GlFiscalYearIds.Contains(x.GlFiscalYearId)).ToList();
                }
                if (!string.IsNullOrEmpty(quarterId))
                {
                    int.TryParse(quarterId, out int quartId);

                    EmpPerformances = EmpPerformances.Where(x => x.GlPeriodId == quartId).ToList();

                }
                if (branchId != null)
                {
                    int.TryParse(branchId, out int braId);
                    EmpPerformances = EmpPerformances.Where(x=>x.BranchId==braId).ToList();
                }
                if (orgStructureId != null)
                {
                    int.TryParse(orgStructureId, out int orgId);
                    EmpPerformances = EmpPerformances.Where(x=>x.OrgStructureId==orgId).ToList();
                }
                if (empId != null)
                {
                    int.TryParse(empId, out int employeeId);
                    EmpPerformances = EmpPerformances.Where(x => x.EmployeeId == employeeId).ToList();
                }
                if (businessUnit != null)
                {
                    int.TryParse(businessUnit, out int businessUnitId);
                    EmpPerformances = EmpPerformances.Where(x => x.BusinessUnitId == businessUnitId).ToList();
                }
                if (!string.IsNullOrEmpty(status))
                {
                    EmpPerformances = EmpPerformances.Where(x => x.ResponsibilityLevel.ToString() == status).ToList();
                }
                var uniqueEmployeePerformances = EmpPerformances.DistinctByField(x => x.EmployeeId).ToList();
                foreach (var place in uniqueEmployeePerformances)
                {
                    totalResult = 0;

                    var empPerformance = new EmployeePerfromanceVM();

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
                    var performances = EmpPerformances.Where(x => x.EmployeeId == place.EmployeeId).DistinctByField(x => x.EvaluationCriteriaTypeId).ToList();
                    empPerformance.PerformanceResults = new List<PerformanceResultVM>();
                    foreach (var performance in  performances)
                    {
                        var quarter = new PerformanceResultVM();
                        var criteria = db.MeasurementDefinitions.Find(performance.EvaluationCriteriaTypeId);
                        if(criteria != null)
                        {
                            var quar = db.Quarters.Find(performance.GlPeriodId);
                            if(quar !=null)
                            {
                                quarter.Period = quar.Name;
                            }
                            quarter.Criteria = criteria.EvaluationCriteria+"-"+criteria.Role.ToString();
                            quarter.Weight = GetPerformanceWeight(performance.EmployeePerformanceId);
                            quarter.Score = GetPerformanceScore(performance.EmployeePerformanceId);
                            totalResult = totalResult + quarter.Score;
                            empPerformance.PerformanceResults.Add(quarter);
                        }
                          
                    }
                    
                    employeePerformanceList.Add(empPerformance);

                    
                }
            }

            return employeePerformanceList;
        }

        public  decimal GetPerformanceScore(int empPerformanceId)
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
        public  decimal GetPerformanceWeight(int empPerformanceId)
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