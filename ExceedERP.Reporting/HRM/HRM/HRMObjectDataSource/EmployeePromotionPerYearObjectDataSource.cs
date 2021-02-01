using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePromotionPerYearObjectDataSource : List<EmployeePromotion>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePromotion> GetEmployeePromotionPerYer(string dept, string branchId)
        {
            List<EmployeePromotion> EmployeeInfoList = new List<EmployeePromotion>();
            EmployeePromotion EmployeeInfo = new EmployeePromotion();
            int PromotionPerYear = 0;
            int checkDiffrence = 0;
            db = new ExceedDbContext();
            var ranks = db.Steps.ToList();
            db = new ExceedDbContext();
            var jobs = db.Lookups.ToList();
            db = new ExceedDbContext();
            var salaries = db.Salaries.ToList();
            var configration = db.Configurations.Where(x => x.Reference == "YEARLY_PROMOTION_PER_YEAR" && x.Attribute == "Promotion").FirstOrDefault();
            if (configration != null)
            {
                PromotionPerYear = Convert.ToInt32(configration.CurrentValue);
            }
            List<Person_Employee> emplist;
            if (branchId != null && dept != null)
            {
                emplist = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
            }
            else if (branchId != null && dept == null)
            {
                emplist = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).Where(x => x.IsActive).ToList();
            }
            else if (branchId == null && dept != null)
            {
                emplist = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
            }
            else
            {

                emplist = db.Person_Employee.Where(x => x.IsActive).ToList();

            }
            if (dept != null)
            {
                db = new ExceedDbContext();





                if (emplist.Any())
                {
                    foreach (var e in emplist)
                    {
                        db = new ExceedDbContext();
                        var placements = db.Placements.Where(x => x.EmployeeId == e.EmployeeId && x.LookupId.ToString() == dept && x.isCurrent).FirstOrDefault();

                        if (placements != null)
                        {
                            checkDiffrence = DateTime.Now.Year - placements.AssignmentDate.Year;
                            if (checkDiffrence >= PromotionPerYear)
                            {

                                var previousPlacment = db.Placements.Where(x => x.Index == placements.Index - 1).FirstOrDefault();
                                EmployeeInfo = new EmployeePromotion();
                               // EmployeeInfo.Sex = e.Gender;
                                if (e.Gender==Gender.MALE)
                                {
                                    EmployeeInfo.Sex = "ወንድ";
                                }
                                else
                                {
                                    EmployeeInfo.Sex = "ሴት";
                                }
                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                EmployeeInfo.EmployeeId = e.EmployeeCode;
                                if (previousPlacment != null)
                                {
                                    EmployeeInfo.FromStep = ranks.Where(x => x.StepId == previousPlacment.StepId).FirstOrDefault().StepName;
                                }
                                var toStep = ranks.Where(x => x.StepId == placements.StepId).FirstOrDefault();
                                if (toStep != null)
                                {
                                    var nextStep = ranks.Where(x => x.StepSequence == toStep.StepSequence + 1).FirstOrDefault();
                                    if (nextStep != null)
                                    {
                                        EmployeeInfo.ToStep = nextStep.StepName;
                                        var salary = db.Salaries.Where(x => x.RankId == placements.RankId && x.StepId == nextStep.StepId).FirstOrDefault();
                                        if (salary != null)
                                        {
                                            EmployeeInfo.Salary = salary.SalaryAmount;
                                        }
                                        else
                                        {
                                            var JobTitlesalary = db.SalariesJobTitles.Where(x => x.RankId == placements.RankId && x.StepId == nextStep.StepId && x.LookupId == placements.LookupId).FirstOrDefault();
                                            if (JobTitlesalary != null)
                                            {
                                                EmployeeInfo.Salary = JobTitlesalary.Salary;
                                            }
                                        }
                                    }
                                }
                                EmployeeInfo.YearOfPromotion = placements.AssignmentDate.Year;

                                var deptt =
                                    db.MainCompanyStructures.Where(
                                            x => x.OrgStructureId == placements.OrgStructureId)
                                        .FirstOrDefault();
                                EmployeeInfo.JobTitle = jobs.Where(x => x.LookupId.ToString() == dept).FirstOrDefault().Description;
                                if (deptt != null)
                                {
                                    EmployeeInfo.Departement = deptt.Name;
                                }
                                EmployeeInfoList.Add(EmployeeInfo);

                            }

                        }
                    }
                }
            }
            else
            {
                db = new ExceedDbContext();




                if (emplist.Any())
                {
                    foreach (var e in emplist)
                    {
                        db = new ExceedDbContext();
                        var placements = db.Placements.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent).FirstOrDefault();
                        if (placements != null)
                        {
                            checkDiffrence = DateTime.Now.Year - placements.AssignmentDate.Year;
                            if (checkDiffrence >= PromotionPerYear)
                            {
                                var previousPlacment = db.Placements.Where(x => x.Index == placements.Index - 1).FirstOrDefault();
                                EmployeeInfo = new EmployeePromotion();
                               // EmployeeInfo.Sex = e.Gender;
                                if (e.Gender == Gender.MALE)
                                {
                                    EmployeeInfo.Sex = "ወንድ";
                                }
                                else
                                {
                                    EmployeeInfo.Sex = "ሴት";
                                }
                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                EmployeeInfo.EmployeeId = e.EmployeeCode;
                                if (previousPlacment != null)
                                {
                                    EmployeeInfo.FromStep = ranks.Where(x => x.StepId == previousPlacment.StepId).FirstOrDefault().StepName;
                                }
                                var toStep = ranks.Where(x => x.StepId == placements.StepId).FirstOrDefault();
                                if (toStep != null)
                                {
                                    var nextStep = ranks.Where(x => x.StepSequence == toStep.StepSequence + 1).FirstOrDefault();
                                    if (nextStep != null)
                                    {
                                        EmployeeInfo.ToStep = nextStep.StepName;
                                        var salary = db.Salaries.Where(x => x.RankId == placements.RankId && x.StepId == nextStep.StepId).FirstOrDefault();
                                        if (salary != null)
                                        {
                                            EmployeeInfo.Salary = salary.SalaryAmount;
                                        }
                                        else
                                        {
                                            var JobTitlesalary = db.SalariesJobTitles.Where(x => x.RankId == placements.RankId && x.StepId == nextStep.StepId && x.LookupId == placements.LookupId).FirstOrDefault();
                                            if (JobTitlesalary != null)
                                            {
                                                EmployeeInfo.Salary = JobTitlesalary.Salary;
                                            }
                                        }
                                    }
                                }
                                EmployeeInfo.YearOfPromotion = placements.AssignmentDate.Year;
                                EmployeeInfo.Salary = placements.Salary;
                                EmployeeInfo.JobTitle = jobs.Where(x => x.LookupId == placements.LookupId).FirstOrDefault().Description;
                                var deptt =
                                    db.MainCompanyStructures.Where(
                                            x => x.OrgStructureId == placements.OrgStructureId)
                                        .FirstOrDefault();
                                if (deptt != null)
                                {
                                    EmployeeInfo.Departement = deptt.Name;
                                }
                                EmployeeInfoList.Add(EmployeeInfo);
                            }


                        }
                    }
                }
            }
            return EmployeeInfoList;
        }
    }
}