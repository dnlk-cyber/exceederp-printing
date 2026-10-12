using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeLeaveObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeLeave> GetEmployeeLeave(string fiscalYear,string branchId,
                                                     string businessUnit, string dept,
                                                      DateTime? from, DateTime? to,
                                                        string type, string status)
        {
            var employeeQuery = db.Person_Employee.AsQueryable();
            var placementQuery = db.Placements.AsQueryable();

            // Branch
            if (!string.IsNullOrWhiteSpace(branchId) && int.TryParse(branchId, out int branch))
            {
                employeeQuery = employeeQuery.Where(e =>
                    placementQuery.Any(p => p.EmployeeId == e.EmployeeId && p.BranchId == branch));
            }

            // Business Unit
            if (!string.IsNullOrWhiteSpace(businessUnit) && int.TryParse(businessUnit, out int bu))
            {
                employeeQuery = employeeQuery.Where(e =>
                    placementQuery.Any(p => p.EmployeeId == e.EmployeeId && p.BusinessUnitId == bu));
            }

            // Department
            if (!string.IsNullOrWhiteSpace(dept) && int.TryParse(dept, out int deptId))
            {
                employeeQuery = employeeQuery.Where(e =>
                    placementQuery.Any(p => p.EmployeeId == e.EmployeeId && p.OrgStructureId == deptId));
            }

            // Employee Status
            if (status == "Active")
                employeeQuery = employeeQuery.Where(x => x.IsActive);
            else if (status == "InActive")
                employeeQuery = employeeQuery.Where(x => !x.IsActive);

            var leaveQuery = db.LeaveRequest
                .Where(x => x.IsOnlineApproved && !x.IsVoid);

            if (!string.IsNullOrWhiteSpace(fiscalYear) &&
                int.TryParse(fiscalYear, out int year))
            {
                leaveQuery = leaveQuery.Where(x => x.GlFiscalYearId == year);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                leaveQuery = leaveQuery.Where(x => x.Type.ToString() == type);
            }

            if (from.HasValue)
            {
                var f = from.Value.Date;
                leaveQuery = leaveQuery.Where(x =>
                    (x.From <= f && x.To >= f) ||
                    x.From >= f);
            }

            if (to.HasValue)
            {
                var t = to.Value.Date;
                leaveQuery = leaveQuery.Where(x =>
                    (x.From <= t && x.To >= t) ||
                    x.To <= t);
            }

            var employeeIds = employeeQuery.Select(x => x.EmployeeId);

            leaveQuery = leaveQuery.Where(x => employeeIds.Contains(x.EmployeeId));

            var employees = employeeQuery.ToDictionary(x => x.EmployeeId);

            var entitlements = db.LeavesEntitle
                .Where(x => !x.IsExpired)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(
                    x => x.Key,
                    x => x.Sum(y => y.Balance));

            var placements = db.Placements
                .OrderByDescending(x => x.AssignmentDate)
                .ToList()
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault(x => x.isCurrent) ?? g.First());

            var departments = db.MainCompanyStructures
                .ToDictionary(x => x.OrgStructureId, x => x.Name);

            var fiscalYears = db.GLFiscalYears
                .ToDictionary(x => x.GlFiscalYearId, x => x.Name);

            var leaves = leaveQuery.ToList();

            var result = new List<EmployeeLeave>();

            foreach (var leave in leaves)
            {
                if (!employees.TryGetValue(leave.EmployeeId, out var emp))
                    continue;

                var item = new EmployeeLeave
                {
                    Sex = HrHelper.GetLocalizedGender(emp.Gender),
                    FullName = $"{emp.EmployeeCode}-{emp.FirstName} {emp.MiddleName} {emp.LastName}",
                    From = leave.FromAmharic,
                    To = leave.ToAmharic,
                    LeaveType = leave.Type.ToString(),
                    AmountTaken = leave.LeaveDays
                };

                if (leave.Day == LeaveDay.Full_Day)
                {
                    item.FullOrHalf = LeaveDay.Full_Day.ToString();
                }
                else
                {
                    item.FullOrHalf = leave.MorningOrAfternoon.ToString();
                }

                if (entitlements.TryGetValue(leave.EmployeeId, out var balance))
                    item.Balance = balance;

                if (placements.TryGetValue(leave.EmployeeId, out var placement))
                {
                    if (departments.TryGetValue(placement.OrgStructureId, out var deptName))
                    {
                        item.Departement = deptName;
                    }
                }

                if (fiscalYears.TryGetValue(leave.GlFiscalYearId, out var period))
                {
                    item.Period = period;
                }

                result.Add(item);
            }

            return result;
        }

        //[DataObjectMethod(DataObjectMethodType.Select)]
        //public List<EmployeeLeave> GetEmployeeLeave(string fiscalYear,string branchId,string businessUnit, string dept, DateTime? from, DateTime? to, string type, string status)
        //{
        //    List<EmployeeLeave> EmployeeInfoList = new List<EmployeeLeave>();
        //    EmployeeLeave EmployeeInfo = new EmployeeLeave();
        //    List<Person_Employee> employees = db.Person_Employee.ToList(); ;
        //    var placements = db.Placements.ToList();

        //    if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
        //    {
        //        var placementEmpIds = db.Placements.Where(x => x.BranchId == braId)
        //                                .Select(x => x.EmployeeId).ToList();
        //        employees = employees.Where(x => placementEmpIds.Contains(x.EmployeeId)).ToList();
        //    }

        //    if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int businessUnitId))
        //    {
        //        var placementEmpIds = db.Placements.Where(x => x.BusinessUnitId == businessUnitId)
        //                                .Select(x => x.EmployeeId).ToList();
        //        employees = employees.Where(x => placementEmpIds.Contains(x.EmployeeId)).ToList();
        //    }

        //    if (!string.IsNullOrEmpty(dept) && int.TryParse(dept, out int orgId))
        //    {
        //        var placementEmpIds = db.Placements
        //                                .Where(x => x.OrgStructureId == orgId)
        //                                .Select(x => x.EmployeeId)
        //                                .ToList();
        //        employees = employees.Where(x => placementEmpIds.Contains(x.EmployeeId)).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(status))
        //    {
        //        if (status == "Active")
        //        {
        //            employees = employees.Where(x => x.IsActive).ToList();
        //        }
        //        if (status == "InActive")
        //        {
        //            employees = employees.Where(x => !x.IsActive).ToList();
        //        }

        //    }
           
        //    var leaves = db.LeaveRequest.Where(x=>x.IsOnlineApproved&&!x.IsVoid).ToList();
        //    if (!string.IsNullOrEmpty(fiscalYear)
        //       && int.TryParse(fiscalYear, out int yearId))
        //    {

        //        leaves = leaves.Where(x => x.GlFiscalYearId == yearId).ToList();

        //    }
        //    if (!string.IsNullOrEmpty(type))
        //    {
        //        leaves = leaves.Where(x => x.Type.ToString() == type).ToList();
        //    }
        //    if (from.HasValue)
        //    {
        //        leaves = leaves
        //            .Where(x =>( x.From.Date <= from.Value.Date
        //                        &&x.To.Date>=from.Value.Date)
        //                        ||(x.From.Date>=from.Value.Date)).ToList();
        //    }
        //    if (to.HasValue)
        //    {
        //        leaves = leaves.Where(x => 
        //                                (x.From.Date <= to.Value.Date 
        //                                && x.To > to.Value.Date) 
        //                                ||x.To<=to.Value.Date).ToList();

        //    }
           
         
        //    foreach (var l in leaves)
        //    {
                
        //        var emplist = employees.FirstOrDefault(x => x.EmployeeId == l.EmployeeId);
        //        if (emplist != null)
        //        {

        //            EmployeeInfo = new EmployeeLeave();
        //            EmployeeInfo.Sex = HrHelper.GetLocalizedGender(emplist.Gender);
        //            EmployeeInfo.FullName =$"{emplist.EmployeeCode}-{ emplist.FirstName} { emplist.MiddleName} { emplist.LastName}";
        //            EmployeeInfo.From = l.FromAmharic;
        //            EmployeeInfo.To = l.ToAmharic;//l.To.ToShortDateString();
        //            //EmployeeInfo.EmployeeId = emplist.EmployeeCode;
        //            EmployeeInfo.LeaveType = l.Type.ToString();
        //            if (l.Day == LeaveDay.Full_Day)
        //            {
        //                EmployeeInfo.FullOrHalf = LeaveDay.Full_Day.ToString(); ;
        //            }
        //            else if (l.Day == LeaveDay.Half_Day)
        //            {
        //                if (l.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
        //                {
        //                    EmployeeInfo.FullOrHalf = MorningOrAfternoon.Afternoon.ToString();
        //                }
        //                else if (l.MorningOrAfternoon == MorningOrAfternoon.Morning)
        //                {
        //                    EmployeeInfo.FullOrHalf = MorningOrAfternoon.Morning.ToString();
        //                }
        //            }

        //            EmployeeInfo.AmountTaken = l.LeaveDays;

        //            var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == l.EmployeeId && !x.IsExpired).ToList();
        //            if (entitle.Any())
        //            {
        //                EmployeeInfo.Balance = entitle.Sum(x => x.Balance);
        //            }


        //            int? deptId = null; // Default to null instead of 0 to avoid confusion
        //            var placementsPerPer = db.Placements
        //                                .Where(x => x.EmployeeId == l.EmployeeId)
        //                                .OrderByDescending(x => x.AssignmentDate)
        //                                .ToList();

        //            // Try to find a current placement, if not, pick the latest one
        //            deptId = placementsPerPer.FirstOrDefault(x => x.isCurrent)?.OrgStructureId
        //                    ?? placementsPerPer.FirstOrDefault()?.OrgStructureId;

        //            if (deptId.HasValue)
        //            {
        //                var mainCompanyStructure = db.MainCompanyStructures
        //                                              .FirstOrDefault(x => x.OrgStructureId == deptId);

        //                if (mainCompanyStructure != null)
        //                {
        //                    EmployeeInfo.Departement = mainCompanyStructure.Name;
        //                }
        //            }
        //            var glPeriod = db.GLFiscalYears.Find(l.GlFiscalYearId);
        //            if(glPeriod != null)
        //            {
        //                EmployeeInfo.Period = glPeriod.Name;
        //            }
        //            EmployeeInfoList.Add(EmployeeInfo);
                    
        //        }
                   
        //    }
            
        //    return EmployeeInfoList;
        //}


        public void Dispose()
        {
            db.Dispose();
        }
    }
}