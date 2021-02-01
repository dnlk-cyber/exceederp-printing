using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class DepartmentLateComingObjectDataSource : List<LateComing>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LateComing> GetDepartmentLateComing(string period, string dept, string user)
        {
            List<LateComing> list = new List<LateComing> { };
            LateComing EmployeeInfo;
            var glPeriod = db.GLPeriods.FirstOrDefault(x => x.Name == period);

            var payroll = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == period);
            var employees = db.Person_Employee.ToList();
            List<string> emloyeeList = GetUserBranchEmployees(Convert.ToInt32(dept), null);
            foreach (var e in emloyeeList)
            {
                EmployeeInfo = new LateComing();

                var employee = employees.FirstOrDefault(x => x.EmployeeId.ToString() == e);
                if (employee != null)
                {
                    EmployeeInfo.EmpID = employee.EmployeeCode;
                    EmployeeInfo.FullName = employee.FirstName + " " + employee.MiddleName + " " +
                                            employee.LastName;
                }
                if (glPeriod != null)
                {

                    var lateComings = db.LateComingSummaries.Where(x => x.GLPeriodId == glPeriod.GLPeriodId).ToList();

                    if (lateComings.Any())
                    {

                        var employeelateComings = lateComings.Where(x => x.EmployeeId.ToString() == e).ToList();
                        if (employeelateComings.Any())
                        {
                            EmployeeInfo.TotalDays = employeelateComings.Sum(x => x.TotalDays);
                            EmployeeInfo.TotalHour = employeelateComings.Sum(x => x.TotalHours);
                        }
                    }
                }
                list.Add(EmployeeInfo);
            }

            return list;
        }
        public List<string> GetUserBranchEmployees(int dept, List<string> branchId = null)
        {
            var placments = from p in db.Placements
                            join
                                e in db.Person_Employee
                                on p.EmployeeId equals e.EmployeeId
                            where p.isCurrent && e.IsActive && p.OrgStructureId == dept
                            select (new
                            {
                                EmployeeId = p.EmployeeId,
                                p.BranchId
                            });
            if (branchId != null)
            {
                placments = placments.Where(x => branchId.Contains(x.BranchId.ToString()));
            }

            if (placments.Any())
            {
                var listOfEmployees = placments.Select(x => x.EmployeeId.ToString()).ToList();
                return listOfEmployees;
            }
            return new List<string>();
        }

    }
}