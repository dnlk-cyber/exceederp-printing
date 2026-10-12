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
    public class LeavePlanObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeavePlanList> GetLeavePlan(string empId,string period, string dept, string branchId,DateTime? from, DateTime? to)
        {
            List<LeavePlanList> list = new List<LeavePlanList>();
            LeavePlanList employee = new LeavePlanList();
            db = new ExceedDbContext();
            var periodList = db.GLFiscalYears;
            List<Person_Employee> empList;//= db.Person_Employee.Where(x => x.IsActive).ToList();
            var placments = db.Placements.Where(x => x.isCurrent);
            var departments = db.MainCompanyStructures;


            #region 
            var leaveLists = db.LeavePlans.ToList();
            if (branchId != null && dept != null)
            {
                empList = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
                var empIds = empList.Select(x => x.EmployeeId).ToList();
                leaveLists = leaveLists.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
            if (branchId != null)
            {
                empList = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).Where(x => x.IsActive).ToList();
                var empIds = empList.Select(x => x.EmployeeId).ToList();
                leaveLists = leaveLists.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
           if ( dept != null)
           {
                empList = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(dept)).Where(x => x.IsActive).ToList();
                var empIds = empList.Select(x => x.EmployeeId).ToList();
                leaveLists = leaveLists.Where(x => empIds.Contains(x.EmployeeId)).ToList();

           }
           if(empId != null)
           {
                int.TryParse(empId, out int emppId);
                leaveLists = leaveLists.Where(x => x.EmployeeId==emppId).ToList();

           }
           if (period != null && dept != null)
           {
                var periods = periodList.FirstOrDefault(x => x.GlFiscalYearId.ToString() == period);
                
                leaveLists = leaveLists.Where(
                    x => x.GlFiscalYearId.ToString() == period).ToList();
                if (leaveLists.Any())
                {
                    db = new ExceedDbContext();
                    int.TryParse(period, out int fiscalyearId);
                    var employees = leaveLists.GroupBy(x => new { x.EmployeeId, x.GlFiscalYearId })
                                            .Select(g => new {
                                                EmployeeId = g.Key.EmployeeId,
                                                GlFiscalYearId = g.Key.GlFiscalYearId,
                                                //LeaveAmount = g.Sum(s => s.LeaveAmount),
                                            }).ToList();
                    foreach (var employeePlan in employees)
                    {
                        employee = new LeavePlanList();
                        var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeePlan.EmployeeId);
                        if (emp != null)
                        {
                            employee.Id = emp.EmployeeCode;
                            employee.FullName = emp.FullNameaEnglish;
                        }
                        var place = placments.FirstOrDefault(x => x.EmployeeId == employeePlan.EmployeeId && x.isCurrent);
                        if (place != null)
                        {
                            var department = db.MainCompanyStructures.Find(place.OrgStructureId);
                            employee.Department = department!=null? department.Name :"";
                        }
                        var fiscalyear = db.GLFiscalYears.Find(employeePlan.GlFiscalYearId);
                        employee.LeavePeriod = fiscalyear != null ? fiscalyear.Name : "";
                        employee.LeavePlanDetails = new List<LeavePlanDetailList>();
                        leaveLists = leaveLists.Where(x => x.EmployeeId == employeePlan.EmployeeId).ToList();
                        foreach (var t in leaveLists)
                        {
                            employee.LeavePeriod = t.GlFiscalYear.Name;
                            var detail = new LeavePlanDetailList();
                            detail.Amount = t.LeaveAmount;
                            detail.From = t.From.ToShortDateString();
                            detail.To = t.To.ToShortDateString();
                            detail.Remark = t.Remark;
                            employee.LeavePlanDetails.Add(detail);
                        }
                            
                     list.Add(employee);
                        

                    }
                }
            }
            else
            {
                
                if (leaveLists.Any())
                {
                    db = new ExceedDbContext();
                    var employees = leaveLists.GroupBy(x=> new { x.EmployeeId,x.GlFiscalYearId})
                        .Select(g =>new { EmployeeId=g.Key.EmployeeId,
                                        GlFiscalYearId = g.Key.GlFiscalYearId,
                                       // LeaveAmount =g.Sum(s=>s.LeaveAmount),
                            }).ToList();
                    foreach (var employeePlan in employees)
                    {
                        employee = new LeavePlanList();
                        var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeePlan.EmployeeId);
                        if (emp != null)
                        {
                            employee.Id = emp.EmployeeCode;
                            employee.FullName = emp.FullNameaEnglish;
                        }
                        var place = placments.FirstOrDefault(x => x.EmployeeId == employeePlan.EmployeeId && x.isCurrent);
                        if (place != null)
                        {
                            var department = db.MainCompanyStructures.Find(place.OrgStructureId);
                            employee.Department = department != null ? department.Name : "";

                        }
                        var fiscalyear = db.GLFiscalYears.Find(employeePlan.GlFiscalYearId);
                        employee.LeavePeriod = fiscalyear != null ? fiscalyear.Name : "";
                        employee.LeavePlanDetails = new List<LeavePlanDetailList>();
                        leaveLists = leaveLists.Where(x => x.EmployeeId == employeePlan.EmployeeId).ToList();
                        foreach (var t in leaveLists)
                        {
                            var department = db.MainCompanyStructures.Find(place.OrgStructureId);
                            employee.Department = department != null ? department.Name : ""; var detail = new LeavePlanDetailList();
                            detail.Amount = t.LeaveAmount;
                            detail.From = t.From.ToShortDateString();
                            detail.To = t.To.ToShortDateString();
                            detail.Remark = t.Remark;
                            employee.LeavePlanDetails.Add(detail);
                        }
                            
                        list.Add(employee);
                            

                    }
                }
               
            }
            #endregion
            return list.OrderBy(x => x.Id).ToList();
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}