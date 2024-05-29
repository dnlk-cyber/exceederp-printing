using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeavePlanObjectDataSource : List<LeavePlanList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeavePlanList> GetLeavePlan(string period, string dept, string branchId)
        {
            List<LeavePlanList> list = new List<LeavePlanList>();
            LeavePlanList employee = new LeavePlanList();
            db = new ExceedDbContext();
            var periodList = db.GLFiscalYears;
            List<Person_Employee> empList;//= db.Person_Employee.Where(x => x.IsActive).ToList();
            var placments = db.Placements.Where(x => x.isCurrent);
            var departments = db.MainCompanyStructures;


            #region 

            if (branchId != null && dept != null)
            {
                empList = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
            }
            else if (branchId != null && dept == null)
            {
                empList = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).Where(x => x.IsActive).ToList();
            }
            else if (branchId == null && dept != null)
            {
                empList = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
            }
            else
            {

                empList = db.Person_Employee.Where(x => x.IsActive).ToList();

            }

            if (period != null && dept != null)
            {
                var periods = periodList.FirstOrDefault(x => x.GlFiscalYearId.ToString() == period);
                var leaveList = db.LeavePlans.ToList();
                leaveList = leaveList.Where(
                    x => x.GlFiscalYearId.ToString() == period).ToList();
                employee = new LeavePlanList();
                if (leaveList.Any())
                {
                    db = new ExceedDbContext();


                    foreach (var t in leaveList)
                    {
                        var emp = empList.FirstOrDefault(x => x.EmployeeId == t.EmployeeId);
                        if (emp != null)
                        {
                            var place = placments.FirstOrDefault(x => x.EmployeeId == t.EmployeeId && x.isCurrent);
                            if (place != null)
                            {
                                employee = new LeavePlanList();

                                employee.Id = emp.EmployeeCode;
                                employee.FullName = emp.FullNameAmharic;//emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                                employee.LeavePeriod = periods.Name;
                                employee.Amount = t.LeaveAmount;
                                employee.From = t.From.ToShortDateString();
                                employee.To = t.To.ToShortDateString();
                                employee.Remark = t.Remark;
                                var firstOrDefaul =
                                    departments
                                        .FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                                if (firstOrDefaul != null)
                                    employee.Department = firstOrDefaul.Name;

                            }
                            list.Add(employee);
                        }

                    }
                }
            }
            else
            {
                var periods = periodList.Select(x => x.GlFiscalYearId).ToList();
                var leaveList = db.LeavePlans.ToList();//.Where(x => periods.Contains(x.GlFiscalYearId)).ToList();

                employee = new LeavePlanList();
                if (leaveList.Any())
                {
                    db = new ExceedDbContext();


                    foreach (var t in leaveList)
                    {
                        var emp = empList.FirstOrDefault(x => x.EmployeeId == t.EmployeeId);
                        if (emp != null)
                        {
                            var place = placments.FirstOrDefault(x => x.EmployeeId == t.EmployeeId && x.isCurrent);
                            if (place != null)
                            {
                                employee = new LeavePlanList();
                                employee.Id = emp.EmployeeCode;
                                employee.FullName = emp.FullNameAmharic;//emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                                employee.LeavePeriod = t.GlFiscalYear.Name;
                                employee.Amount = t.LeaveAmount;
                                employee.From = t.From.ToShortDateString();
                                employee.To = t.To.ToShortDateString();
                                employee.Remark = t.Remark;
                                var firstOrDefaul =
                                    departments
                                        .FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                                if (firstOrDefaul != null)
                                    employee.Department = firstOrDefaul.Name;

                            }
                            list.Add(employee);
                        }

                    }
                }
            }
            #endregion
            return list.OrderBy(x => x.Id).ToList();
        }
    }
}