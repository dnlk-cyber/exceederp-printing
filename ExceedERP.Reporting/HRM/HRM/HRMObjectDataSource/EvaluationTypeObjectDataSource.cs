using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EvaluationTypeObjectDataSource : EmployeePerformanceVM
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerformanceVM> GetEvaluationType(string empId,string yearId, string quarterId, string criteriaId, string businessUnit)
        {

            var empPerformance = new List<EmployeePerformanceVM>();
            
            if (empId != null)
            {
                int.TryParse(empId, out int employeeId);
                int.TryParse(yearId, out int fiscalYearId);
                int.TryParse(quarterId, out int quartId);
                int.TryParse(criteriaId, out int evaluationCriteriaId);
                var employeePerformances = db.EmployeePerformances.Where(x => x.EmployeeId == employeeId/*&& x.GlFiscalYearId == fiscalYearId*/ && x.IsApproved).ToList();

                var GlFiscalYearIds = db.QuarterLengths.Where(x => x.QuarterLengthId == fiscalYearId).Select(x => x.GlFiscalYearId);

                if (GlFiscalYearIds != null)
                {
                    employeePerformances = employeePerformances.Where(x => GlFiscalYearIds.Contains(x.GlFiscalYearId)).ToList();

                }
                if (quarterId != null)
                {
                    employeePerformances = employeePerformances.Where(x => x.GlPeriodId == quartId).ToList();

                }
                if (criteriaId != null)
                {
                    employeePerformances = employeePerformances.Where(x => x.EvaluationCriteriaTypeId == evaluationCriteriaId).ToList();
                }
                if (businessUnit != null)
                {
                    int.TryParse(businessUnit, out int businessUnitId);
                    employeePerformances = employeePerformances.Where(x => x.BusinessUnitId == businessUnitId).ToList();
                }

                foreach(var employeePerformance in employeePerformances)
                {
                    
                        //var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeePerformance.EmployeeId);
                        //if (person != null)
                        //{
                        //    empPerformance.FullName = person.FullNameaEnglish;
                        //}

                        //var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employeePerformance.EmployeeId && x.isCurrent);
                        //if (place != null)
                        //{
                        //    var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                        //    if (branch != null)
                        //    {
                        //        empPerformance.Branch = branch.Name;
                        //    }

                        //    var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        //    if (mainCompany != null)
                        //    {
                        //        empPerformance.Department = mainCompany.Name;
                        //    }

                        //    var look = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
                        //    if (look != null)
                        //    {
                        //        empPerformance.Jobtitle = look.Description;
                        //    }
                        //}

                        //var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == employeePerformance.GlFiscalYearId);
                        //if (year != null)
                        //{
                        //    empPerformance.Year = year.Name;
                        //}

                        //var period = db.Quarters.FirstOrDefault(x => x.QuarterId == employeePerformance.GlPeriodId);
                        //if (period != null)
                        //{
                        //    empPerformance.Period = period.Name;
                        //}

                        var evaluationCriteria = db.MeasurementDefinitions.Where(x => x.MeasurementDefinitionId == employeePerformance.EvaluationCriteriaTypeId).ToList();
                    var crateriaList = new EmployeePerformanceVM();
                    foreach (var criteria in evaluationCriteria)
                    {


                            crateriaList.EvaluationCriteriaName = criteria.EvaluationCriteria;
                            crateriaList.Role = criteria.Role.ToString();
                            crateriaList.Weight = criteria.DefnPercentageofWeight;
                            crateriaList.Result = GetPerformanceScore(employeePerformance.EmployeePerformanceId);
                            var look = db.Lookups.FirstOrDefault(x => x.LookupId == criteria.LookupId);
                            if (look != null)
                            {
                                crateriaList.EvaluationType = look.Description;
                            }

                            var employeeMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformance.EmployeePerformanceId).ToList();

                            crateriaList.EmployeeMeasurmentList = new List<EmployeeMeasurementVM>();

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

                            crateriaList.EmployeeMeasurmentList.Add(employeeMeasure);
                            }
                      
                    }
                    empPerformance.Add(crateriaList);
                }
              //  var employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == employeeId && x.GlPeriodId == quartId && x.IsApproved);

                //if(criteriaId != null)
                //{
                //     employeePerformance = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == employeeId && x.GlPeriodId == quartId && x.EvaluationCriteriaTypeId == evaluationCriteriaId && x.IsApproved);
                //}

                //if (employeePerformance != null)
                //{
                //    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeePerformance.EmployeeId);
                //    if (person != null)
                //    {
                //        empPerformance.FullName = person.FullNameaEnglish;
                //    }

                //    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employeePerformance.EmployeeId && x.isCurrent);
                //    if (place != null)
                //    {
                //        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                //        if (branch != null)
                //        {
                //            empPerformance.Branch = branch.Name;
                //        }

                //        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                //        if (mainCompany != null)
                //        {
                //            empPerformance.Department = mainCompany.Name;
                //        }

                //        var look = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
                //        if (look != null)
                //        {
                //            empPerformance.Jobtitle = look.Description;
                //        }
                //    }

                //    var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == employeePerformance.GlFiscalYearId);
                //    if (year != null)
                //    {
                //        empPerformance.Year = year.Name;
                //    }

                //    var period = db.Quarters.FirstOrDefault(x => x.QuarterId == employeePerformance.GlPeriodId);
                //    if (period != null)
                //    {
                //        empPerformance.Period = period.Name;
                //    }

                //    var evaluationCriteria = db.MeasurementDefinitions.FirstOrDefault(x => x.MeasurementDefinitionId == employeePerformance.EvaluationCriteriaTypeId);
                //    if (evaluationCriteria != null)
                //    {
                //        empPerformance.EvaluationCriteriaName = evaluationCriteria.EvaluationCriteria;
                //        empPerformance.Role = evaluationCriteria.Role.ToString();
                //        empPerformance.Weight = evaluationCriteria.DefnPercentageofWeight;
                //        empPerformance.Result = GetPerformanceScore(employeePerformance.EmployeePerformanceId);
                //        var look = db.Lookups.FirstOrDefault(x => x.LookupId == evaluationCriteria.LookupId);
                //        if (look != null)
                //        {
                //            empPerformance.EvaluationType = look.Description;
                //        }
                //    }

                //    var employeeMeasurments = db.EmployeeMeasurement.Where(x => x.EmployeePerformanceId == employeePerformance.EmployeePerformanceId).ToList();

                //    empPerformance.EmployeeMeasurmentList = new List<EmployeeMeasurementVM>();

                //    foreach (var empMeaur in employeeMeasurments)
                //    {
                //        var employeeMeasure = new EmployeeMeasurementVM();

                //        var measur = db.Measurements.FirstOrDefault(x => x.MeasurementId == empMeaur.MeasurmentId);
                //        if (measur != null)
                //        {
                //            employeeMeasure.MeasurmentName = measur.MeasurementName;
                //            employeeMeasure.Weight = measur.MesPercentageofWeight;
                //        }

                //        employeeMeasure.Result = GetEmployeeMeasurment(empMeaur.EmpMeasurementId);

                //        var employeePerformanceScores = db.EmployeePerformanceScores.Where(x => x.EmployeeMeasurementId == empMeaur.EmpMeasurementId).ToList();

                //        employeeMeasure.EmployeePerformanceScoreVMList = new List<EmployeePerformanceScoreVM>();

                //        foreach (var perfAction in employeePerformanceScores)
                //        {
                //            var performanceScore = new EmployeePerformanceScoreVM();

                //            var performanceAction = db.PerformanceActions.FirstOrDefault(x => x.ActionId == perfAction.PerformanceActionId);
                //            if (performanceAction != null)
                //            {
                //                performanceScore.ActionName = performanceAction.ActionName;
                //                performanceScore.Weight = performanceAction.AcPercentageofWeight;
                //            }

                //            performanceScore.Score = perfAction.Score;

                //            employeeMeasure.EmployeePerformanceScoreVMList.Add(performanceScore);
                //        }

                //        empPerformance.EmployeeMeasurmentList.Add(employeeMeasure);
                //    }

                //}
            }

            return empPerformance;
        }
       public GlobalEmployeePerformanceTotalVM GetGlobalObject(string empId, string yearId, string quarterId)
        {
            var empPerformance = new GlobalEmployeePerformanceTotalVM();
            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            //var empPerformance = new EmployeePerformanceTotalVM();

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


            if (empId != null&& yearId != null)
            {
                int.TryParse(empId, out int employeeId);
                int.TryParse(yearId, out int FiscalYearId);
                int.TryParse(quarterId, out int quartId);
                var GlFiscalYearId = db.QuarterLengths.Where(x => x.QuarterLengthId == FiscalYearId).Select(x => x.GlFiscalYearId).FirstOrDefault();

              //  var GlFiscalYearId = db.QuarterLengths.Where(x => x.QuarterLengthId == FiscalYearId).Select(x => x.GlFiscalYearId);
              //  var employeePerformance = db.EmployeePerformances.Where(x => x.EmployeeId == employeeId && x.GlPeriodId == quartId && x.IsApproved);
                var employeePerformances = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == employeeId && x.GlFiscalYearId == FiscalYearId && x.IsApproved);
                if (GlFiscalYearId != 0)
                {
                    employeePerformances = db.EmployeePerformances.Where(x=>x.GlFiscalYearId== GlFiscalYearId && x.EmployeeId == employeeId && x.IsApproved).FirstOrDefault();
                }
                if (quarterId != null)
                {
                    employeePerformances = db.EmployeePerformances.FirstOrDefault(x => x.EmployeeId == employeeId &&  x.GlPeriodId == quartId && x.IsApproved);
                }
                if (employeePerformances != null)
                {


                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeePerformances.EmployeeId);
                    if (person != null)
                    {
                        empPerformance.FullName = person.FullNameaEnglish;
                    }

                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employeePerformances.EmployeeId && x.isCurrent);
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

                    var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == employeePerformances.GlFiscalYearId);
                    if (year != null)
                    {
                        empPerformance.Year = year.Name;
                    }
                    if (quarterId != null)
                    {
                        var period = db.Quarters.FirstOrDefault(x => x.QuarterId == employeePerformances.GlPeriodId);
                        if (period != null)
                        {
                            empPerformance.Period = period.Name;
                        }
                    }
                    else
                    {
                        var employeePerformance = db.EmployeePerformances.Where(x => x.GlFiscalYearId == GlFiscalYearId && x.EmployeeId == employeeId && x.IsApproved).DistinctByField(x => x.GlPeriodId).OrderBy(x=>x.GlPeriodId).ToList();
                        foreach (var per in employeePerformance)
                        {
                            var period = db.Quarters.FirstOrDefault(x => x.QuarterId == per.GlPeriodId);
                            if (period != null)
                            {
                                empPerformance.Period += period.Name + " - ";
                            }
                        }
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