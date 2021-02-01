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
    public class TerminationObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetTerminationSummary(DateTime from, DateTime to, string branchId, string orgStructureId)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            var termination = db.Terminations.ToList();
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            employee = new EmployeeSummary();
            List<Person_Employee> employees;
            if (branchId != null && orgStructureId != null)
            {
                //employees = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).ToList();

                var listE = new List<Person_Employee>();
                var allEmployee = db.Person_Employee.ToList();
                var placements = db.Placements.DistinctByField(x => x.EmployeeId).Where(x => x.BranchId.ToString() == branchId && x.OrgStructureId.ToString() == orgStructureId).ToList();
                if (placements.Any())
                {
                    var filterdE = from f in placements
                                  join e in allEmployee
                                  on f.EmployeeId equals e.EmployeeId
                                  where f.EmployeeId == e.EmployeeId
                                  select (new { e });

                    foreach (var p in filterdE)
                    {

                        listE.Add(p.e);

                    }
                }
                employees = listE;

            }
            else if (branchId != null && orgStructureId == null)
            {
                //employees = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).ToList();

                var listE = new List<Person_Employee>();
                var allEmployee = db.Person_Employee.ToList();
                var placements = db.Placements.DistinctByField(x=>x.EmployeeId).Where(x => x.BranchId.ToString() == branchId ).ToList();
                if (placements.Any())
                {
                    foreach (var p in placements)
                    {
                        var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                        if (check != null)
                        {
                            listE.Add(check);
                        }
                    }
                }
                employees = listE;
            }
            else if (branchId == null && orgStructureId != null)
            {
                //employees = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();

                var listE = new List<Person_Employee>();
                var allEmployee = db.Person_Employee.ToList();
                var placements = db.Placements.DistinctByField(x => x.EmployeeId).Where(x => x.OrgStructureId.ToString() == orgStructureId).ToList();
                if (placements.Any())
                {
                    foreach (var p in placements)
                    {
                        var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                        if (check != null)
                        {
                            listE.Add(check);
                        }
                    }
                }
                employees = listE;
            }
            else
            {

                employees = db.Person_Employee/*.Where(x => x.IsActive)*/.ToList();

            }
            if (termination.Count() > 0)
            {
                db = new ExceedDbContext();
                foreach (var t in termination)
                {
                    DateTime terminDate = new DateTime(t.TerminationDate.Year, t.TerminationDate.Month,
                        t.TerminationDate.Day);
                    if (terminDate >= dateOne && terminDate <= dateTow)
                    {

                        db = new ExceedDbContext();
                        var empList = employees.Where(x => x.EmployeeId == t.EmployeeId).ToList();
                        foreach (var e in empList)
                        {
                            // employee.Criteria = "Job Title";

                            // employee.SummaryName = Name.Description;
                            if (e.Gender == Gender.FEMALE)
                            {
                                employee.NumberOfFemaleEmployee += 1;

                            }
                            if (e.Gender == Gender.MALE)
                            {
                                employee.NumberOfMaleEmployee += 1;

                            }

                            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                        }
                    }
                }
                list.Add(employee);
            }

            return list;
        }
    }
}