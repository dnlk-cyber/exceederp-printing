using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class MyEmployeePerformanceODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerformanceVM> GetMyPerformance(string id, string quarterId, string criteriaId)
        {
            List<EmployeePerformanceVM> employeePerformanceList = new List<EmployeePerformanceVM>();
            var empPerformance = new EmployeePerformanceVM();

            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                empPerformance.Name = organization.TradeName;
                empPerformance.AmharicName = organization.AmharicName;
                empPerformance.Abbreviation = organization.Abbreviation;

                var address =
               db.Addresses.Where(
                       a =>
                       a.Reference == organization.MainCompanyId.ToString() &&
                       a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {
                        if (a.Category == AddressType.KIFLEKETEMA)
                        {
                            empPerformance.Address = "Subcity: " + a.Value + " ";
                        }
                        if (a.Category == AddressType.KEBELE)
                        {
                            empPerformance.Address += "Kebele: " + a.Value + " ";
                        }
                        if (a.Category == AddressType.CITYPROVINCE)
                        {
                            if (empPerformance.Address == null)
                            {
                                empPerformance.Address += a.Value + " -Ethiopia";
                            }
                        }
                    }
                }

                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    empPerformance.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            empPerformance.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);


            if (id != null)
            {
                int.TryParse(id, out int empId);
                int.TryParse(quarterId, out int quartId);
                int.TryParse(criteriaId, out int evaluationCriteriaId);

                var employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == empId && x.GlPeriodId == quartId && x.EvaluationCriteriaTypeId == evaluationCriteriaId && x.IsApproved);

                if (employeePerformance != null)
                {
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeePerformance.EmployeeId);
                    if (person != null)
                    {
                        empPerformance.FullName = person.FullNameaEnglish;
                    }

                    var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == employeePerformance.GlFiscalYearId);
                    if (year != null)
                    {
                        empPerformance.Year = year.Name;
                    }

                    var period = db.Quarters.FirstOrDefault(x => x.QuarterId == employeePerformance.GlPeriodId);
                    if (period != null)
                    {
                        empPerformance.Period = period.Name;
                    }

                    var evaluationCriteria = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == employeePerformance.EvaluationCriteriaTypeId);
                    if (evaluationCriteria != null)
                    {
                        empPerformance.EvaluationCriteriaName = evaluationCriteria.EvaluationCriteria;
                        empPerformance.Role = evaluationCriteria.Role.ToString();
                        empPerformance.Weight = evaluationCriteria.DefnPercentageofWeight;
                        empPerformance.Result = GetPerformanceScore(employeePerformance.EmployeePerformanceId);
                        var look = db.Lookups.FirstOrDefault(x => x.LookupId == evaluationCriteria.LookupId);
                        if (look != null)
                        {
                            empPerformance.EvaluationType = look.Description;
                        }
                    }

                    var employeeMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformance.EmployeePerformanceId).ToList();

                    empPerformance.EmployeeMeasurmentList = new List<EmployeeMeasurementVM>();

                    foreach (var empMeaur in employeeMeasurments)
                    {
                        var employeeMeasure = new EmployeeMeasurementVM();

                        var measur = db.Measurements.FirstOrDefault(x => x.MeasurementId == empMeaur.MeasurmentId);
                        if (measur != null)
                        {
                            employeeMeasure.MeasurmentName = measur.MeasurementName;
                            employeeMeasure.Weight = measur.MesPercentageofWeight;
                        }

                        employeeMeasure.Result = GetEmployeeMeasurment(empMeaur.EmpMeasurementId);

                        var employeePerformanceScores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == empMeaur.EmpMeasurementId).ToList();

                        employeeMeasure.EmployeePerformanceScoreVMList = new List<EmployeePerformanceScoreVM>();

                        foreach (var perfAction in employeePerformanceScores)
                        {
                            var performanceScore = new EmployeePerformanceScoreVM();

                            var performanceAction = db.PerformanceActions.FirstOrDefault(x => x.ActionId == perfAction.PerformanceActionId);
                            if (performanceAction != null)
                            {
                                performanceScore.ActionName = performanceAction.ActionName;
                                performanceScore.Weight = performanceAction.AcPercentageofWeight;
                            }

                            performanceScore.Score = perfAction.Score;
                           
                            employeeMeasure.EmployeePerformanceScoreVMList.Add(performanceScore);
                        }

                        empPerformance.EmployeeMeasurmentList.Add(employeeMeasure);
                    }                    
                }
                employeePerformanceList.Add(empPerformance);
            }

            return employeePerformanceList;
        }

        public decimal GetEmployeeMeasurment(int empMeasurmentId)
        {
            var performanceScores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == empMeasurmentId).ToList();

            if (performanceScores.Any())
            {
                return performanceScores.Sum(x => x.Score);
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

    }
}
