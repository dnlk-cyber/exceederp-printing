using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeTerminationObjectDataSource : List<EmployeeTermination>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeTermination> GetEmployeeTermination(string reason, DateTime from, DateTime to, string branchId, string orgStructureId)
        {
            List<EmployeeTermination> EmployeeInfoList = new List<EmployeeTermination>();
            EmployeeTermination EmployeeInfo = new EmployeeTermination();
            db = new ExceedDbContext();
            var branches = db.Branch.ToList();
            db = new ExceedDbContext();
            var jobs = db.Lookups.ToList();
            db = new ExceedDbContext();
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            var placements = db.Placements.ToList();
            List<Person_Employee> employees = db.Person_Employee.ToList();
           
            if (branchId != null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).ToList();
            }
            else if (branchId != null && orgStructureId == null)
            {
                employees = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();
            }
            else
            {

                employees = db.Person_Employee.ToList();

            }
            List<Termination> terminations = db.Terminations.ToList();
            if (from != null && to != null)
            {
                terminations = terminations.Where(x => x.TerminationDate >= from && x.TerminationDate <= to).ToList();               
            }
            if (reason != null)
            {
                db = new ExceedDbContext();
                terminations = terminations.Where(x => x.LookupId.ToString() == reason).ToList();
                if (terminations.Any())
                {
                    foreach (var l in terminations)
                    {
                        db = new ExceedDbContext();

                        var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId).ToList();

                        if (emplist.Any())
                        {
                            foreach (var e in emplist)
                            {
                                DateTime terminationDate = new DateTime(l.TerminationDate.Year, l.TerminationDate.Month,
                                    l.TerminationDate.Day);
                                if (terminationDate >= dateOne && terminationDate <= dateTow)
                                {
                                    EmployeeInfo = new EmployeeTermination();
                                    EmployeeInfo.EmployeeId = e.EmployeeCode;
                                    if (e.Gender == 0)
                                    {
                                        EmployeeInfo.Sex = "ወንድ";
                                    }
                                    else
                                    {
                                        EmployeeInfo.Sex = "ሴት";
                                    }
                                    EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                    EmployeeInfo.Reason =
                                        jobs.Where(x => x.LookupId == l.LookupId).FirstOrDefault().Description;
                                 

                                    EmployeeInfo.TerminationDate = l.TerminationDateAm;// l.TerminationDate.ToShortDateString();
                                    var place = placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
                                    if (place != null)
                                    {


                                        EmployeeInfo.Branch =
                                            branches.Where(x => x.BranchId == place.BranchId).FirstOrDefault().Name;
                                        EmployeeInfo.JobTitle =
                                            jobs.Where(x => x.LookupId == place.LookupId)
                                                .FirstOrDefault()
                                                .Description;


                                        var deptt =
                                            db.MainCompanyStructures.Where(
                                                    x => x.OrgStructureId == place.OrgStructureId)
                                                .FirstOrDefault();
                                        if (deptt != null)
                                        {
                                            EmployeeInfo.Departement = deptt.Name;
                                        }
                                    }
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
                terminations = terminations.ToList();
                if (terminations.Any())
                {
                    foreach (var l in terminations)
                    {
                        db = new ExceedDbContext();

                        var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId).ToList();

                        if (emplist.Any())
                        {
                            foreach (var e in emplist)
                            {

                                EmployeeInfo = new EmployeeTermination();
                                if (e.Gender == 0)
                                {
                                    EmployeeInfo.Sex = "ወንድ";
                                }
                                else
                                {
                                    EmployeeInfo.Sex = "ሴት";
                                }
                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                EmployeeInfo.EmployeeId = e.EmployeeCode;
                                var place = placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
                                if (place != null)
                                {


                                    EmployeeInfo.Branch =
                                        branches.Where(x => x.BranchId == place.BranchId).FirstOrDefault().Name;
                                    EmployeeInfo.JobTitle =
                                        jobs.Where(x => x.LookupId == place.LookupId).FirstOrDefault().Description;
                                    var deptt =
                                        db.MainCompanyStructures.Where(
                                                x => x.OrgStructureId == place.OrgStructureId)
                                            .FirstOrDefault();
                                    if (deptt != null)
                                    {
                                        EmployeeInfo.Departement = deptt.Name;
                                    }
                                }
                                EmployeeInfo.Reason =
                                    jobs.Where(x => x.LookupId == l.LookupId).FirstOrDefault().Description;
                                EmployeeInfo.TerminationDate = l.TerminationDateAm;// l.TerminationDate.ToShortDateString();
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