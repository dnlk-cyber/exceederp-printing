using System;
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
    public class EmployeePerformanceSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public EmployeePerformanceHeaderVM GetEmployeePerformanceSummary(string empId,string yearId, string quarterId, string criteriaId)
        {

            var empPerformance = new EmployeePerformanceHeaderVM();
            if (!string.IsNullOrEmpty(empId) && !string.IsNullOrEmpty(yearId))
            {
                int.TryParse(empId, out int employeeId);
                int.TryParse(yearId, out int fiscalYearId);
                int.TryParse(quarterId, out int quartId);
                var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId == employeeId && x.IsSecondLevelApproved).ToList();
                var GlFiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == fiscalYearId).Select(x => x.GlFiscalYearId);
                if (GlFiscalYearIds != null)
                {
                    employeePerformances = employeePerformances.Where(x => GlFiscalYearIds.Contains(x.GlFiscalYearId)).ToList();
                }
                if (quarterId != null)
                {
                    employeePerformances = employeePerformances.Where(x => x.GlPeriodId == quartId).ToList();

                }
                if (!string.IsNullOrEmpty(criteriaId))
                {
                    int.TryParse(criteriaId, out int evaluationCriteriaId);
                    employeePerformances = employeePerformances.Where(x => x.EvaluationCriteriaTypeId == evaluationCriteriaId).ToList();
                }
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
                var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId);
                {
                    if (place != null)
                    {

                        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                        if (person != null)
                        {
                            empPerformance.FullName = System.Net.WebUtility.HtmlEncode(person.FullNameaEnglish);
                        }
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                        if (branch != null)
                        {
                            empPerformance.Branch = System.Net.WebUtility.HtmlEncode(branch.Name);
                        }

                        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (mainCompany != null)
                        {
                            empPerformance.Department = System.Net.WebUtility.HtmlEncode(mainCompany.Name);
                        }

                        var look = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
                        if (look != null)
                        {
                            empPerformance.Jobtitle = System.Net.WebUtility.HtmlEncode(look.Description);
                        }
                        var businessUnitt = db.BusinessUnits.Find(place.BusinessUnitId);
                        if(businessUnitt != null)
                        {
                            empPerformance.BussinessUnit = businessUnitt.Name;
                        }
                        var head = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.HeadId);
                        if (head != null)
                        {
                            empPerformance.ImmidiateApprover = head.FullNameaEnglish;
                        }
                        var perform = employeePerformances.Where(x => x.EmployeeId == employeeId && x.DirectBossId !=0).FirstOrDefault();
                        if (perform != null)
                        {
                            var secondLevelApprover = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == perform.DirectBossId);
                            if (secondLevelApprover != null)
                            {
                                empPerformance.SecondLevelApproverApprover = secondLevelApprover.FullNameaEnglish;
                                empPerformance.SecondLevelApproverDate = perform.SecondLevelApprovalDate.HasValue?perform.SecondLevelApprovalDate.Value.ToString("dd.MM.yyyy"):null;
                            }
                        }
                    }

                    var year = db.GLFiscalYears.Find(fiscalYearId);
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
                }
                empPerformance.EmployeePerformanceLists = new List<EmployeePerformanceVM>();

                foreach (var employeePerformance in employeePerformances)
                {

                    var crateriaList = new EmployeePerformanceVM();

                    var period = db.Quarters.FirstOrDefault(x => x.QuarterId == employeePerformance.GlPeriodId);
                    if (period != null)
                    {
                        crateriaList.Period = period.Name;
                    }
                    var evaluationCriteria = db.MeasurementDefinitions.Where(x => x.MeasurementDefinitionId == employeePerformance.EvaluationCriteriaTypeId).FirstOrDefault();
                    if (evaluationCriteria != null )
                    {                  
                        crateriaList.EvaluationCriteriaName = evaluationCriteria.EvaluationCriteria;
                        crateriaList.Role = evaluationCriteria.Role.ToString();
                        crateriaList.Weight = evaluationCriteria.DefnPercentageofWeight;

                    }
                    crateriaList.Result = GetPerformanceScore(employeePerformance.EmployeePerformanceId);
                    var employeeMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformance.EmployeePerformanceId).ToList();
                    crateriaList.EmployeeMeasurmentList = new List<EmployeeMeasurementVM>();
                    foreach (var empMeaur in employeeMeasurments)
                    {
                        var employeeMeasure = new EmployeeMeasurementVM();
                        var measurement = db.Measurements.FirstOrDefault(x => x.MeasurementId == empMeaur.MeasurmentId);
                        if (measurement != null)
                        {
                            employeeMeasure.MeasurmentName = measurement.MeasurementName;
                            employeeMeasure.Weight = measurement.MesPercentageofWeight;
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
                        crateriaList.EmployeeMeasurmentList.Add(employeeMeasure);

                    }
                    empPerformance.EmployeePerformanceLists.Add(crateriaList);
                }
             
            }

            return empPerformance;
        }

        private decimal GetEmployeeMeasurment(int empMeasurmentId)
        {
            var performanceScores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == empMeasurmentId);

            if (performanceScores.Any())
            {
                return performanceScores.Sum(x => x.Score);
            }

            return 0;
        }
        private decimal GetPerformanceScore(int empPerformanceId)
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