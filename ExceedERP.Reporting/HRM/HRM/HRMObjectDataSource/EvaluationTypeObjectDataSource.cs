using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EvaluationTypeObjectDataSource : EmployeePerformanceVM
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public EmployeePerformanceVM GetEvaluationType(string empId, string quarterId, string criteriaId)
        {

            var empPerformance = new EmployeePerformanceVM();
            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (Organization != null)
            {
                empPerformance.Name = Organization.TradeName;
                empPerformance.AmharicName = Organization.AmharicName;
                empPerformance.Abbreviation = Organization.Abbreviation;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
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
                    db.Attachments.Where(
                        a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
                if (logo != null)
                {
                    empPerformance.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }


            if (empId != null)
            {
                int.TryParse(empId, out int employeeId);
                int.TryParse(quarterId, out int quartId);
                int.TryParse(criteriaId, out int evaluationCriteriaId);

                var employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == employeeId && x.GlPeriodId == quartId && x.EvaluationCriteriaTypeId == evaluationCriteriaId && x.IsApproved);

                if (employeePerformance != null)
                {
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeePerformance.EmployeeId);
                    if (person != null)
                    {
                        empPerformance.FullName = person.FullNameaEnglish;
                    }

                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employeePerformance.EmployeeId && x.isCurrent);
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
            }

            return empPerformance;
        }

        public decimal GetEmployeeMeasurment(int empMeasurmentId)
        {
            var performanceScores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == empMeasurmentId);

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
                var scores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == em.EmpMeasurementId);

                if (scores.Any())
                {
                    totalScore = totalScore + scores.Sum(x => x.Score);
                }
            }

            return totalScore;
        }

    }


}