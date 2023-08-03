using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.Finance
{

    
    public class PayrollHelper
    {
        public List<EmployeeIdView> GetEmployees(int branchid, int businessUnitId, string all = "")
        {
            ExceedDbContext db = new ExceedDbContext();
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employees = from e in db.Person_Employee
                            join p in db.Placements
                                on e.EmployeeId equals p.EmployeeId
                            where e.IsActive && p.isCurrent && !p.IsPend
                           // where e.EmployeeCode== "OCC-000180"
                            select (new
                            {
                                e.EmployeeId,
                                e.IsActive,
                                e.FirstNameEnglish,
                                e.MiddleNameEnglish,
                                e.LastNameEnglish,
                                e.DateOfBirth,
                                e.HiredDate,
                                e.Status,
                                p.BranchId,
                                p.BusinessUnitId,
                                p.Salary,
                                p.AssignmentType,
                                p.AssignmentDate,
                                e.EmployeeTypeId
                            });
            var employeeList = employees.ToList();
            if (all == "all")
            {
                var allbranch = db.Branch.Select(x => x.BranchId).ToList();
                employeeList = employeeList.Where(x => allbranch.Contains(x.BranchId)).ToList();
            }
            else
            {
                employeeList = employeeList.Where(x => x.BranchId == branchid).ToList();
            }

            if (GlobalHelper.IsPayrollModebranchAndBusinessUnit() && businessUnitId != 0)
            {
                employeeList = employeeList.Where(x => x.BusinessUnitId == businessUnitId).ToList();
            }
            foreach (var emp in employeeList)
            {

                if (emp.Salary >= 0)
                {
                    EmployeeIdView = new EmployeeIdView
                    {
                        PersonId = emp.EmployeeId,
                        IsActive = emp.IsActive,
                        FullName = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish,
                        Age = (DateTime.Now.Year - emp.DateOfBirth.Year),
                        PlacmentDate = emp.AssignmentDate,
                        BasicSalary=emp.Salary,
                        HiredDate = emp.HiredDate,
                        AssignmentType = emp.AssignmentType,
                        DateOfBirth = emp.DateOfBirth,
                        IsMilitary = emp.EmployeeTypeId == 2 ? true : false,
                        BusinessUnitId = emp.BusinessUnitId,
                        BranchId = emp.BranchId,
                        Status = emp.Status
                    };
                    EmployeeIdViews.Add(EmployeeIdView);
                }
            }


            return EmployeeIdViews;
        }
    }
    
}
