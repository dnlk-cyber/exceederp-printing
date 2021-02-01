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
    public class EmployeeSalaryObjectDataSource : List<EmployeeJob>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeJob> GetEmployeeSalary(int min, int max, string branchId, string orgStructureId,DateTime from, DateTime to, bool active, bool terminated)
        {
            List<EmployeeJob> EmployeeInfoList = new List<EmployeeJob>();
            EmployeeJob EmployeeInfo = new EmployeeJob();
            db = new ExceedDbContext();
            var ranks = db.Ranks.ToList();
            db = new ExceedDbContext();
            var jobs = db.Lookups.ToList();
            db = new ExceedDbContext();
            var salaries = db.Salaries.ToList();
            List<Person_Employee> employees = db.Person_Employee.ToList();
            //if (branchId != null && orgStructureId != null)
            //{
            //    employees = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).ToList();
            //}
            //if (branchId != null && orgStructureId == null)
            //{
            //    employees = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            //}
            //if (branchId == null && orgStructureId != null)
            //{
            //    employees = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();
            //}
            //else
            //{

            //    employees = db.Person_Employee.Where(x => x.IsActive).ToList();

            //}

          
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            DateTime dateTwo = new DateTime(to.Year, to.Month, to.Day);

            db = new ExceedDbContext();
            var placements = db.Placements.OrderByDescending(x=>x.PlacementId).ToList();
            if(active == true && terminated == false)
            {
                employees = employees.Where(x => x.IsActive).ToList();
                if (branchId != null)
                {
                    var places = placements.Where(x=>x.BranchId.ToString() == branchId && x.isCurrent).ToList().Select(x=>x.EmployeeId);
                     employees = employees.Where(x=>places.Contains(x.EmployeeId)).ToList();
                }
                if (orgStructureId != null)
                {
                    var places = placements.Where(x => x.OrgStructureId.ToString() == orgStructureId && x.isCurrent).ToList().Select(x => x.EmployeeId);
                    employees = employees.Where(x => places.Contains(x.EmployeeId)).ToList();
                }
                //if (dateOne < dateTwo)
                //{
                //    var places = placements.Where(x =>x.isCurrent && x.AssignmentDate >= dateOne && x.AssignmentDate <= dateTwo).Select(x=>x.EmployeeId).ToList();
                //    employees = employees.Where(x=>places.Contains(x.EmployeeId)).ToList();
                //}
                if (dateOne < dateTwo)
                {
                    var terminations = db.Terminations.Where(x => x.TerminationDate >= dateOne && x.TerminationDate <= dateTwo).ToList()?.Select(x => x.EmployeeId);
                    var places = placements.Where(x => !terminations.Contains(x.EmployeeId) && x.AssignmentDate >= dateOne && x.AssignmentDate <= dateTwo).Select(x => x.EmployeeId).ToList();
                    employees = db.Person_Employee.Where(x => places.Contains(x.EmployeeId)).ToList();
                }

            }
            if(terminated == true && active == false)
            {
                 employees = employees.Where(x => !x.IsActive).ToList();

                if (branchId != null)
                {
                    var places = db.Placements.OrderByDescending(x=>x.PlacementId).Where(x => x.BranchId.ToString() == branchId && !x.isCurrent).ToList().Select(x => x.EmployeeId).Distinct();
                    employees = employees.Where(x => places.Contains(x.EmployeeId)).ToList();
                }
                if (orgStructureId != null)
                {
                    var places = db.Placements.OrderByDescending(x => x.PlacementId).Where(x => x.OrgStructureId.ToString() == orgStructureId && !x.isCurrent).ToList().Select(x => x.EmployeeId).Distinct();
                    employees = employees.Where(x => places.Contains(x.EmployeeId)).ToList();
                }

                if (dateOne < dateTwo)
                {
                    var terminations = db.Terminations.Where(x => x.TerminationDate >= dateOne && x.TerminationDate <= dateTwo).ToList()?.Select(x => x.EmployeeId);
                    var places = placements.Where(x => terminations.Contains(x.EmployeeId) && !x.isCurrent).Select(x=>x.EmployeeId).ToList();
                    employees = employees.Where(x=>places.Contains(x.EmployeeId)).ToList();
                }
            }
            if(active == true && terminated == true)
            {
                if (branchId != null)
                {
                    var places = placements.Where(x => x.BranchId.ToString() == branchId).ToList().Select(x => x.EmployeeId);
                    employees = employees.Where(x => places.Contains(x.EmployeeId)).ToList();
                }
                if (orgStructureId != null)
                {
                    var places = placements.Where(x => x.OrgStructureId.ToString() == orgStructureId).ToList().Select(x => x.EmployeeId);
                    employees = employees.Where(x => places.Contains(x.EmployeeId)).ToList();
                }
            }
            if (active == false && terminated == false)
            {
                if (branchId != null)
                {
                    var places = placements.Where(x => x.BranchId.ToString() == branchId).ToList().Select(x => x.EmployeeId);
                    employees = employees.Where(x => places.Contains(x.EmployeeId)).ToList();
                }
                if (orgStructureId != null)
                {
                    var places = placements.Where(x => x.OrgStructureId.ToString() == orgStructureId).ToList().Select(x => x.EmployeeId);
                    employees = employees.Where(x => places.Contains(x.EmployeeId)).ToList();
                }
            }

            placements = placements.DistinctByField(x=>x.EmployeeId).ToList();


            if (placements.Any())
            {
                foreach (var l in placements)
                {
                    db = new ExceedDbContext();
                    if (l.Salary >= min && l.Salary <= max)
                    {
                        var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId).ToList();

                        if (emplist.Any())
                        {
                            foreach (var e in emplist)
                            {
                                EmployeeInfo = new EmployeeJob();
                                EmployeeInfo.EmployeeId = e.EmployeeCode;
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

                                EmployeeInfo.Rank = ranks.Where(x => x.RankId == l.RankId).FirstOrDefault().RankName;
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
                                EmployeeInfoList.Add(EmployeeInfo);
                            }
                        }
                    }
                    else if (min == 0 && max == 0)
                    {
                        var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId).ToList();

                        if (emplist.Any())
                        {
                            foreach (var e in emplist)
                            {
                                EmployeeInfo = new EmployeeJob();
                                EmployeeInfo.EmployeeId = e.EmployeeCode;
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

                                EmployeeInfo.Rank = ranks.Where(x => x.RankId == l.RankId).FirstOrDefault().RankName;
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
                                EmployeeInfoList.Add(EmployeeInfo);
                            }
                        }
                    }

                }
            }


            return EmployeeInfoList.OrderBy(x => x.EmployeeId).ToList();
        }
    }
}