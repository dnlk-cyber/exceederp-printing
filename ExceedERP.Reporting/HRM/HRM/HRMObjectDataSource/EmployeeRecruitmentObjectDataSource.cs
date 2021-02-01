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
    public class EmployeeRecruitmentObjectDataSource : List<EmployeeRecruitment>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeRecruitment> GetEmployeeRecruitment(string dept, DateTime from, DateTime to, string branchId, string assignment)
        {
            List<EmployeeRecruitment> EmployeeInfoList = new List<EmployeeRecruitment>();
            EmployeeRecruitment EmployeeInfo = new EmployeeRecruitment();
            db = new ExceedDbContext();
            var branches = db.Branch.ToList();
            db = new ExceedDbContext();
            var jobs = db.Lookups.ToList();
            db = new ExceedDbContext();
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            List<Person_Employee> employees;
            if (branchId != null && dept != null)
            {
                employees = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(dept)).ToList();
                if (from != to)
                {
                    employees = employees.Where(x => x.HiredDate >= from && x.HiredDate <= to).ToList();
                }
            }
            else if (branchId != null && dept == null)
            {
                employees = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
                if (from != to)
                {
                    employees = employees.Where(x => x.HiredDate >= from && x.HiredDate <= to).ToList();
                }
            }
            else if (branchId == null && dept != null)
            {
                employees = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(dept)).ToList();
                if (from != to)
                {
                    employees = employees.Where(x => x.HiredDate >= from && x.HiredDate <= to).ToList();
                }
            }
            else
            {
                var place = db.Placements.Where(x=>x.isCurrent).Select(x=>x.EmployeeId).ToList();
                employees = db.Person_Employee.Where(x => x.IsActive && place.Contains(x.EmployeeId)).ToList();
                if(from != to)
                {
                    employees = employees.Where(x => x.HiredDate >= from && x.HiredDate <= to).ToList();
                }
              

            }
            if (dept != null)
            {
                db = new ExceedDbContext();
                var placements = db.Placements.Where(x => x.OrgStructureId.ToString() == dept && x.isCurrent).ToList();
                AssignmentType assigne = AssignmentType.NEW;
                if(assignment == "NEW")
                {
                    assigne = AssignmentType.NEW;
                }
                else if(assignment == "PROMOTION")
                {
                    assigne = AssignmentType.PROMOTION;
                }
                else if (assignment == "YEARLY_PROMOTION")
                {
                    assigne = AssignmentType.YEARLY_PROMOTION;
                }
                else if (assignment == "TRANSFER")
                {
                    assigne = AssignmentType.TRANSFER;
                }
                else if (assignment == "DEMOTION")
                {
                    assigne = AssignmentType.DEMOTION;
                }
                else if (assignment == "RECOMENDATION")
                {
                    assigne = AssignmentType.RECOMENDATION;
                }
                else if (assignment == "ASSIGNMENT")
                {
                    assigne = AssignmentType.ASSIGNMENT;
                }
                else if (assignment == "STRUCTURE_CHANGE")
                {
                    assigne = AssignmentType.STRUCTURE_CHANGE;
                }
                if(assignment != null)
                {
                    placements = placements.Where(x => x.AssignmentType == assigne).ToList();
                }
                if (placements.Any())
                {
                    foreach (var l in placements)
                    {
                        db = new ExceedDbContext();

                        var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId && x.IsActive).ToList();

                        if (emplist.Any())
                        {
                            foreach (var e in emplist)
                            {
                                DateTime hireDate = new DateTime(e.HiredDate.Year, e.HiredDate.Month, e.HiredDate.Day);
                                if (hireDate >= dateOne && hireDate <= dateTow)
                                {
                                    EmployeeInfo = new EmployeeRecruitment();
                                    EmployeeInfo.EmployeeId = e.EmployeeCode;
                                    //EmployeeInfo.Sex = e.Gender;
                                    if (e.Gender == Gender.MALE)
                                    {
                                        EmployeeInfo.Sex = "ወንድ";
                                    }
                                    else
                                    {
                                        EmployeeInfo.Sex = "ሴት";
                                    }
                                    EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                    //EmployeeInfo.HireDate = e.HiredDate.ToShortDateString();
                                    EmployeeInfo.HireDate = e.HiredDateAmharic;
                                    EmployeeInfo.Branch =
                                        branches.Where(x => x.BranchId == l.BranchId).FirstOrDefault().AmharicName;
                                    EmployeeInfo.JobTitle =
                                        jobs.Where(x => x.LookupId == l.LookupId).FirstOrDefault().Description;

                                    EmployeeInfo.Salary = l.Salary;
                                    var deptt =
                                        db.MainCompanyStructures.Where(
                                                x => x.OrgStructureId == l.OrgStructureId)
                                            .FirstOrDefault();
                                    if (deptt != null)
                                    {
                                        EmployeeInfo.Departement = deptt.Name;
                                    }
                                    EmployeeInfo.AssignmentType = l.AssignmentType.ToString();

                                    EmployeeInfoList.Add(EmployeeInfo);
                                }
                            }

                        }
                    }
                }
            }
            else
            {

                db = new ExceedDbContext();
                var placements = db.Placements.Where(x=>x.isCurrent).ToList();

                AssignmentType assigne = AssignmentType.NEW;
                if (assignment == "NEW")
                {
                    assigne = AssignmentType.NEW;
                }
                else if (assignment == "PROMOTION")
                {
                    assigne = AssignmentType.PROMOTION;
                }
                else if (assignment == "YEARLY_PROMOTION")
                {
                    assigne = AssignmentType.YEARLY_PROMOTION;
                }
                else if (assignment == "TRANSFER")
                {
                    assigne = AssignmentType.TRANSFER;
                }
                else if (assignment == "DEMOTION")
                {
                    assigne = AssignmentType.DEMOTION;
                }
                else if (assignment == "RECOMENDATION")
                {
                    assigne = AssignmentType.RECOMENDATION;
                }
                else if (assignment == "ASSIGNMENT")
                {
                    assigne = AssignmentType.ASSIGNMENT;
                }
                else if (assignment == "STRUCTURE_CHANGE")
                {
                    assigne = AssignmentType.STRUCTURE_CHANGE;
                }
                if (assignment != null)
                {
                    placements = placements.Where(x => x.AssignmentType == assigne).ToList();
                }

                if (placements.Any())
                {
                    foreach (var l in placements)
                    {
                        db = new ExceedDbContext();

                        var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId && x.IsActive).ToList();

                        if (emplist.Any())
                        {
                            foreach (var e in emplist)
                            {

                                EmployeeInfo = new EmployeeRecruitment();
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
                                EmployeeInfo.Branch =
                                    branches.Where(x => x.BranchId == l.BranchId).FirstOrDefault().AmharicName;
                                EmployeeInfo.JobTitle =
                                    jobs.Where(x => x.LookupId == l.LookupId).FirstOrDefault().Description;

                                EmployeeInfo.Salary = l.Salary;
                                //EmployeeInfo.HireDate = e.HiredDate.ToShortDateString();
                                EmployeeInfo.HireDate = e.HiredDateAmharic;
                                var deptt =
                                    db.MainCompanyStructures.Where(
                                            x => x.OrgStructureId == l.OrgStructureId)
                                        .FirstOrDefault();
                                if (deptt != null)
                                {
                                    EmployeeInfo.Departement = deptt.Name;
                                }

                                EmployeeInfo.AssignmentType = l.AssignmentType.ToString();

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