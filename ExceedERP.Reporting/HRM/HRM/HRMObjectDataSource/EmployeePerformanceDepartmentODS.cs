using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
//using ExceedERP.Web.Areas.HRM.Controllers;
using XAPI;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerformanceDepartmentODS : List<EmployeePerfromanceVM>
    {
        //HRMInputs input = new HRMInputs();
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceVM> GetEmployeePerformance(string empId, string id, string year, string businessUnit)
        {
            List<EmployeePerfromanceVM> employeePerformanceList = new List<EmployeePerfromanceVM>();

            decimal totalResult = 0;

            if (!string.IsNullOrEmpty(year))
            {


                int.TryParse(year, out int yearId);

                var employeePerformances = db.EmployeePerformances.Where(x=>x.GlFiscalYearId==yearId).Where(x => x.IsSecondLevelApproved).ToList();

                if (id != null)
                {
                    int.TryParse(id, out int orgStructureId);

                    employeePerformances = employeePerformances.Where(x=>x.OrgStructureId==orgStructureId).ToList();
                    
                }
                if (empId != null)
                {
                    int.TryParse(empId, out int employeeId);
                    employeePerformances = employeePerformances.Where(x => x.EmployeeId == employeeId).ToList();
                }

                if (businessUnit != null)
                {
                    int.TryParse(businessUnit, out int businessUnitId);
                    employeePerformances = employeePerformances.Where(x => x.BusinessUnitId == businessUnitId).ToList();
                }
                var uniqEmpPerformances = employeePerformances.DistinctByField(x => x.EmployeeId).ToList();
                foreach (var place in uniqEmpPerformances)
                {
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

                    var singleEmployeeRecords = employeePerformances.Where(x => x.EmployeeId == place.EmployeeId).ToList();
                    if (singleEmployeeRecords.Any())
                    {
                        empPerformance.PerformanceResults = new List<PerformanceResultVM>();


                        foreach (var quar in singleEmployeeRecords)
                        {
                            var quarter = new PerformanceResultVM();
                            var criteria = db.MeasurementDefinitions.Find(quar.EvaluationCriteriaTypeId);
                            if (criteria != null)
                            {
                                var quarters = db.Quarters.Find( place.GlPeriodId);
                                if (quarters != null)
                                {
                                    quarter.Period = quarters.Name;
                                }
                                quarter.Criteria = criteria.EvaluationCriteria+"-"+criteria.EvalutionType.ToString()+"-"+criteria.Role.ToString();
                                quarter.Score = GetPerformanceScore(quar.EmployeePerformanceId);
                                totalResult = totalResult + quarter.Score;
                                empPerformance.PerformanceResults.Add(quarter);
                            }



                        }
                        employeePerformanceList.Add(empPerformance);
                    }

                    
                }
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
    }
}