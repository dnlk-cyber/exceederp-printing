using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.Letters.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.Letters.DataSource.AffanOromo
{
    [DataObject]
    public class RecruitmentLetterAffanOromoObjectDatasource: IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public RecruitmentLetterVM GetRecruitmentLetter(string empId, string vacancyId)
        {
            RecruitmentLetterVM EmployeeInfo = new RecruitmentLetterVM();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(vacancyId, out int vacantId);
                int.TryParse(empId, out int employeeId);
                var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
                if (employee != null)
                {
                    EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
                    EmployeeInfo.FullName =  employee.FullNameaEnglish;

                    var placement = _db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId);
                    if (placement != null)
                    {
                        var businessUnit = _db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == placement.BusinessUnitId);
                        if (businessUnit != null)
                        {
                            EmployeeInfo.BusinessUnit = businessUnit.Name;
                        }
                        var rank = _db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
                        if (rank != null)
                        {
                            EmployeeInfo.Grade = rank.RankName;
                        }
                        var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
                        if (lookup != null)
                        {
                            EmployeeInfo.JobTitle = lookup.Description;
                        }
                        EmployeeInfo.Salary = placement.Salary;
                        var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (department != null)
                        {
                            //EmployeeInfo.wo = department.Name;
                        }
                        var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                        if (branch != null)
                        {
                            EmployeeInfo.Branch = branch.Name;
                        }
                        //EmployeeInfo.AssignmentDate = placement.AssignmentDate.ToString("dd.MM.yyyy");
                        ////EmployeeInfo.AssignmentDate = placement.AssignmentDate;
                        //EmployeeInfo.AssignmentDateAmh = placement.AssignmentDateAmharic;
                        EmployeeInfo.Salary = placement.Salary;
                        var probation = _db.ProbationsSettings.FirstOrDefault();
                        //EmployeeInfo.Probation = probation != null ? probation.PromotionDays / 30 : 0;
                        //EmployeeInfo.ProbationInword = XAPI.NumberExtensions.toWordsAmharic((double)EmployeeInfo.Probation);
                        EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);

                    }
                   
                        double sumOfExp = DateTime.Now.Subtract(employee.HiredDate).TotalDays;
                        var empExps = _db.Experiences.Where(x => x.EmployeeId == employee.EmployeeId).ToList();
                        foreach (var ex in empExps)
                        {
                            sumOfExp = sumOfExp + ex.To.Subtract(ex.From).TotalDays;
                        }
                    

                    decimal expYear = Math.Round((decimal)sumOfExp / 365m,2);
                    EmployeeInfo.WorkExperiance = expYear;
                    var educations = _db.Education.Where(x => x.EmployeeId == employeeId).ToList();
                    if (educations.Any())
                    {
                        var levelIds = educations.Select(x => x.EduLevel);
                        var fieldIds = educations.Select(x => x.EduField);
                        foreach (var level in levelIds)
                        {
                            var lookup = _db.Lookups.Find(level);
                            if (lookup != null)
                            {
                                EmployeeInfo.EducationLevel += lookup.Description + ", ";
                            }
                        }
                        foreach (var field in fieldIds)
                        {
                            var lookup = _db.Lookups.Find(field);
                            if (lookup != null)
                            {
                                EmployeeInfo.EducationField += lookup.Description + ", ";
                            }
                        }
                    }
                    int.TryParse(vacancyId, out int vId);
                    var vacancy = _db.VaccancyPlans.Find(vId);
                    if(vacancy != null)
                    {
                        EmployeeInfo.VacancyCode = vacancy.VaccancyCode;
                        EmployeeInfo.VacancyDate = $"{vacancy.From:dd.MM.yyyy} ({vacancy.FromVacAm})-{vacancy.To:dd.MM.yyyy}({vacancy.ToVacAm})";

                    }

                }
            }

            return EmployeeInfo;
           
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Vacancy> GetVacancyCode()
        {
            return _db.VaccancyPlans.Select(x => new Vacancy
            {
                VacancyId = x.VacancyId,
                VaccancyCode = x.VaccancyCode

            }).ToList();

        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }

    //[DataObject]
    //public class ContractHirement1ObjectDatasource : ContractHirementVM
    //{
    //    private ExceedDbContext _db = new ExceedDbContext();
    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public ContractHirementVM GetContractHirement(string empId)
    //    {

    //        ContractHirementVM EmployeeInfo = new ContractHirementVM();
    //        if (empId != null)
    //        {
    //            int.TryParse(empId, out int employeeId);
    //            var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
    //            if (employee != null)
    //            {
    //                EmployeeInfo = new ContractHirementVM();
    //                EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
    //                EmployeeInfo.FullName = employee.EmployeeCode+"-"+employee.FullNameAmharic;
    //                var placement = _db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId 
    //                                                                    && x.EmployementType == EmployeeStatus.CONTRACT);
    //                if (placement != null)
    //                {
    //                    var rank = _db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
    //                    if (rank != null)
    //                    {
    //                        EmployeeInfo.Rank = rank.RankName;
    //                    }
    //                    var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
    //                    if (lookup != null)
    //                    {
    //                        EmployeeInfo.JobTitle = lookup.Description;
    //                    }
    //                    EmployeeInfo.Salary = placement.Salary;
    //                    var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
    //                    if (department != null)
    //                    {
    //                        EmployeeInfo.WorkUnit = department.Name;
    //                    }
    //                    var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
    //                    if (branch != null)
    //                    {
    //                        EmployeeInfo.Branch = branch.Name;
    //                    }
    //                    EmployeeInfo.AssignmentDate = placement.AssignmentDate;
    //                    EmployeeInfo.AssignmentDateAmh = placement.AssignmentDateAmharic;
    //                    EmployeeInfo.Salary = placement.Salary;
    //                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);

    //                }

    //            }
    //        }
    //        return EmployeeInfo;
    //    }

    //}
}
