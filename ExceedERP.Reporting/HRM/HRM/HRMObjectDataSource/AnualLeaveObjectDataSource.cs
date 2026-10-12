using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class AnualLeaveObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AnnualLeaveVM> GetAnualLeave(string empId, string branch, string leaveType, string periodId,
                                                 string orgStructureId, string status, DateTime? from, DateTime? to)
        {
            // Start with queryable (deferred execution)
            var leaveEntitlesQuery = db.LeavesEntitle.Where(x => !x.IsExpired).AsQueryable();
            var leaveRequestsQuery = db.LeaveRequest.Where(x => x.IsOnlineApproved).AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int employeeId))
            {
                leaveEntitlesQuery = leaveEntitlesQuery.Where(x => x.EmployeeId == employeeId);
                leaveRequestsQuery = leaveRequestsQuery.Where(x => x.EmployeeId==employeeId);

            }

            if (!string.IsNullOrEmpty(branch) && int.TryParse(branch, out int branchId))
                leaveEntitlesQuery = leaveEntitlesQuery.Where(x => x.BranchId == branchId);

            if (!string.IsNullOrEmpty(leaveType))
            {
                leaveRequestsQuery = leaveRequestsQuery.Where(x => x.Type.ToString() == leaveType);
                var empIds = leaveRequestsQuery.Select(x => x.EmployeeId);
                leaveEntitlesQuery = leaveEntitlesQuery.Where(x => empIds.Contains(x.EmployeeId));
            }

            if (int.TryParse(periodId, out int glFiscalYearId))
                leaveEntitlesQuery = leaveEntitlesQuery.Where(x => x.GlFiscalYearId == glFiscalYearId);

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var employeeIdsInOrg = db.Placements.Where(x => x.OrgStructureId == orgId && x.isCurrent)
                                                             .Select(x => x.EmployeeId);
                leaveEntitlesQuery = leaveEntitlesQuery.Where(x => employeeIdsInOrg.Contains(x.EmployeeId));
            }

            if (!string.IsNullOrEmpty(status))
            {
                var requestIds = db.RequestesStatus
                                   .Where(x => x.Status.ToString() == status)
                                   .Select(x => x.RequestId);
                leaveRequestsQuery = leaveRequestsQuery.Where(x => requestIds.Contains(x.LeaveRequestId));
                var empIds = leaveRequestsQuery.Select(x => x.EmployeeId);
                leaveEntitlesQuery = leaveEntitlesQuery.Where(x => empIds.Contains(x.EmployeeId));
            }

            if (from.HasValue)
            {
                leaveRequestsQuery = leaveRequestsQuery.Where(x => x.From.Date >= from.Value.Date);
                var empIds = leaveRequestsQuery.Select(x => x.EmployeeId);
                leaveEntitlesQuery = leaveEntitlesQuery.Where(x => empIds.Contains(x.EmployeeId));
            }

            if (to.HasValue)
            {
                leaveRequestsQuery = leaveRequestsQuery.Where(x => x.To.Date <= to.Value.Date);
                var empIds = leaveRequestsQuery.Select(x => x.EmployeeId);
                leaveEntitlesQuery = leaveEntitlesQuery.Where(x => empIds.Contains(x.EmployeeId));
            }

            // Materialize queries
            var leaveEntitles = leaveEntitlesQuery.ToList();
            var leaveRequests = leaveRequestsQuery.ToList();

            if (!leaveEntitles.Any()) return new List<AnnualLeaveVM>();

            // ✅ Dictionaries with GroupBy (avoids duplicate key crash)
            var fiscalYearDict = db.GLFiscalYears
                .GroupBy(x => x.GlFiscalYearId)
                .ToDictionary(g => g.Key, g => g.First().Name);
            var employeeDict = db.Person_Employee.GroupBy(x => x.EmployeeId)
                                            .ToDictionary(
                                                g => g.Key,
                                                g => new EmployeeInfo
                                                {
                                                    EmployeeCode = g.First().EmployeeCode,
                                                    FullName = $"{g.First().FirstNameEnglish} {g.First().MiddleNameEnglish} {g.First().LastNameEnglish}"
                                                }
                                            );


            var jobTitleDict = db.Lookups
                .GroupBy(x => x.LookupId)
                .ToDictionary(g => g.Key, g => g.First().Description);

            var departmentDict = db.MainCompanyStructures
                .GroupBy(x => x.OrgStructureId)
                .ToDictionary(g => g.Key, g => g.First().Name);

            var placementDict = db.Placements
                .Where(x => x.isCurrent)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.First());

            var requestStatuses = db.RequestesStatus.ToList();

            var result = new List<AnnualLeaveVM>();

            // Group entitlements per employee
            foreach (var employeeLeave in leaveEntitles.GroupBy(x => x.EmployeeId).Select(g => g.First()))
            {
                Placement placement;
                if (!placementDict.TryGetValue(employeeLeave.EmployeeId, out placement))
                    continue;

                string empName = "";
                string code = "";
                if (employeeDict.TryGetValue(placement.EmployeeId, out var empInfo))
                {
                    code = empInfo.EmployeeCode;
                    empName = empInfo.FullName;
                }


                string title = "";
                jobTitleDict.TryGetValue(placement.LookupId, out title);

                string dept = "";
                departmentDict.TryGetValue(placement.OrgStructureId, out dept);

                var annualLeave = new AnnualLeaveVM
                {
                    EmployeeCode = code,
                    EmployeeName = empName,
                    Title = title,
                    Department = dept,
                    Remark = employeeLeave.Remark,
                    RequesteLeaves = new List<LeaveRequestVM>(),
                    LeaveBalances = new List<LeaveBalanceVM>()
                };

                var empLeaveRequests = leaveRequests
                    .Where(x => x.IsOnlineApproved
                             && x.EmployeeId == employeeLeave.EmployeeId
                             && leaveEntitles.Any(e => e.GlFiscalYearId == x.GlFiscalYearId))
                    .ToList();

                foreach (var leave in empLeaveRequests)
                {
                    var approvers = requestStatuses.Where(x => x.RequestId == leave.LeaveRequestId).ToList();

                    string depApprover = "";
                    var depEntry = approvers.FirstOrDefault(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                    if (depEntry?.ApproverDepartment != null &&
                        departmentDict.TryGetValue((int)depEntry.ApproverDepartment, out var depName))
                    {
                        depApprover = depName;
                    }

                    string hrApprover = "";
                    var hrEntry = approvers.FirstOrDefault(x => x.Status == RequestStatus.APPROVE);
                    if (hrEntry?.ApproverId != null &&
                        employeeDict.TryGetValue((int)hrEntry.ApproverId, out var employee))
                    {
                        hrApprover = employee.FullName;
                    }

                    annualLeave.RequesteLeaves.Add(new LeaveRequestVM
                    {
                        LeaveType = leave.Type.ToString(),
                        FromAm = leave.FromAmharic,
                        ToAm = leave.ToAmharic,
                        LeaveDays = leave.LeaveDays,
                        ApproverDepartment = depApprover,
                        Approver = hrApprover
                    });
                }

                annualLeave.LeaveDays = empLeaveRequests.Sum(x => x.LeaveDays);

                // Last 3 fiscal years
                var recentEntitles = leaveEntitles
                    .Where(x => x.EmployeeId == employeeLeave.EmployeeId)
                    .OrderByDescending(x => x.GlFiscalYearId)
                    .Take(3)
                    .ToList();

                if (recentEntitles.Count > 0)
                {
                    string fy1;
                    fiscalYearDict.TryGetValue(recentEntitles[0].GlFiscalYearId, out fy1);
                    annualLeave.FiscalYear1 = fy1;
                    annualLeave.Balance1 = recentEntitles[0]?.Balance ?? 0;
                }
                if (recentEntitles.Count > 1)
                {
                    string fy2;
                    fiscalYearDict.TryGetValue(recentEntitles[1].GlFiscalYearId, out fy2);
                    annualLeave.FiscalYear2 = fy2;
                    annualLeave.Balance2 = recentEntitles[1]?.Balance??0 ;
                }
                if (recentEntitles.Count > 2)
                {
                    string fy3;
                    fiscalYearDict.TryGetValue(recentEntitles[2].GlFiscalYearId, out fy3);
                    annualLeave.FiscalYear3 = fy3;
                    annualLeave.Balance3 = recentEntitles[2]?.Balance ?? 0;
                }

                annualLeave.TotlalAmount = recentEntitles.Sum(x => x.Balance);

                foreach (var ent in recentEntitles)
                {
                    string fy;
                    fiscalYearDict.TryGetValue(ent.GlFiscalYearId, out fy);
                    annualLeave.LeaveBalances.Add(new LeaveBalanceVM
                    {
                        FiscalYear = fy,
                        Balance = ent?.Balance??0
                    });
                }

                result.Add(annualLeave);
            }

            return result;
        }
        public void Dispose()
        {
            db.Dispose();
        }
        public class EmployeeInfo
        {
            public string EmployeeCode { get; set; }
            public string FullName { get; set; }
        }

        //public List<AnnualLeaveVM> GetAnualLeave()
        //{
        //    List<AnnualLeaveVM> annualLeaves = new List<AnnualLeaveVM>();

        //    var leaveEntitles = db.LeavesEntitle.Where(x => !x.IsExpired).ToList();

        //    var fiscalYearHeaders = leaveEntitles
        //        .Select(x => GetFiscalYearName(x.GlFiscalYearId))
        //        .Distinct()
        //        .OrderBy(x => x)
        //        .ToList();

        //    var anualLeaveByEmployees = leaveEntitles
        //        .GroupBy(x => x.EmployeeId)
        //        .Select(g => g.First())
        //        .ToList();

        //    foreach (var employeeLeave in anualLeaveByEmployees)
        //    {
        //        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeLeave.EmployeeId && x.isCurrent);
        //        if (placement == null) continue;

        //        var annualLeavePerEmployee = new AnnualLeaveVM
        //        {
        //            EmployeeName = GetFullName(placement.EmployeeId),
        //            Title = GetFullJobtitleName(placement.LookupId),
        //            Department = GetDepartmentName(placement.OrgStructureId),
        //            Remark = employeeLeave.Remark,
        //            RequesteLeaves = new List<LeaveRequestVM>(),
        //            LeaveBalances = new List<LeaveBalanceVM>(),
        //            DynamicHeaders = fiscalYearHeaders // Add all fiscal years dynamically
        //        };

        //        var leaveRequests = db.LeaveRequest
        //            .Where(x => x.IsOnlineApproved && x.EmployeeId == employeeLeave.EmployeeId)
        //            .ToList();

        //        foreach (var leave in leaveRequests)
        //        {
        //            var approvers = db.RequestesStatus
        //                .Where(x => x.RequestId == leave.LeaveRequestId)
        //                .ToList();

        //            string depApprover = approvers.FirstOrDefault(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL)?.ApproverDepartment != null
        //                ? GetDepartmentName((int)approvers.FirstOrDefault(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL).ApproverDepartment)
        //                : "";

        //            string hrApprover = approvers.FirstOrDefault(x => x.Status == RequestStatus.APPROVE)?.ApproverId != null
        //                ? GetFullName((int)approvers.FirstOrDefault(x => x.Status == RequestStatus.APPROVE).ApproverId)
        //                : "";

        //            annualLeavePerEmployee.RequesteLeaves.Add(new LeaveRequestVM
        //            {
        //                LeaveType = leave.Type.ToString(),
        //                FromAm = leave.FromAmharic,
        //                ToAm = leave.ToAmharic,
        //                LeaveDays = leave.LeaveDays,
        //                ApproverDepartment = depApprover,
        //                Approver = hrApprover
        //            });
        //        }

        //        annualLeavePerEmployee.LeaveDays = leaveRequests.Any() ? leaveRequests.Sum(x => x.LeaveDays) : 0;

        //        var employeeLeaveEntitles = leaveEntitles
        //            .Where(x => x.EmployeeId == employeeLeave.EmployeeId)
        //            .ToList();

        //        var leaveBalances = fiscalYearHeaders
        //            .Select(fiscalYear => new LeaveBalanceVM
        //            {
        //                FiscalYear = fiscalYear,
        //                Balance = employeeLeaveEntitles
        //                    .Where(x => GetFiscalYearName(x.GlFiscalYearId) == fiscalYear)
        //                    .Sum(x => x.Balance)
        //            })
        //            .ToList();

        //        annualLeavePerEmployee.LeaveBalances.AddRange(leaveBalances);
        //        annualLeavePerEmployee.TotlalAmount = leaveBalances.Sum(x => x.Balance);

        //        annualLeaves.Add(annualLeavePerEmployee);
        //    }

        //    return annualLeaves;
        //}

        public List<AnnualLeaveVM> GetAnualLeaveExisted( string empId, string branch, string leaveType, string periodId, string orgStructureId, string status, DateTime? from, DateTime? to)
        {
            List<AnnualLeaveVM> annualLeaves = new List<AnnualLeaveVM>();

            var leaveEntitles = db.LeavesEntitle.Where(x => !x.IsExpired).ToList();
            var leaveRequests = db.LeaveRequest.Where(x => x.IsOnlineApproved).ToList();
            //HashSet<int> periodIds = periodNs != null
            //    ? new HashSet<int>(periodNs.Select(pp => Convert.ToInt32(pp)))
            //    : new HashSet<int>();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                leaveEntitles = leaveEntitles.Where(x => x.EmployeeId == employeeId).ToList();
            }
            
            if (!string.IsNullOrEmpty(branch))
            {
                int.TryParse(branch, out int branchId);
                leaveEntitles = leaveEntitles.Where(x => x.BranchId == branchId).ToList();
            }
            if (!string.IsNullOrEmpty(leaveType))
            {
                leaveRequests = leaveRequests.Where(x => x.Type.ToString() == leaveType).ToList();
                var empIds = leaveRequests.Select(x => x.EmployeeId).ToList();
                leaveEntitles = leaveEntitles.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            }
            if (!string.IsNullOrEmpty(periodId ))
            {
                int.TryParse(periodId, out int glfisicalYearId);
                leaveEntitles = leaveEntitles.Where(x => x.GlFiscalYearId == glfisicalYearId).ToList();
            }
            if (!string.IsNullOrEmpty(orgStructureId)&& leaveEntitles.Any())
            {
                int.TryParse(orgStructureId, out int orgId);
                var placements = db.Placements.Where(x=>x.OrgStructureId==orgId&&x.isCurrent);
                var employeIds = placements.Select(x => x.EmployeeId).ToList();
                leaveEntitles = leaveEntitles.Where(x => employeIds.Contains( x.EmployeeId)).ToList();

            }
            if (!string.IsNullOrEmpty(status) && leaveEntitles.Any() && leaveEntitles.Any())
            {
                var requstIds = db.RequestesStatus.Where(x => x.Status.ToString() == status).Select(x => x.RequestId);
                leaveRequests = leaveRequests.Where(x => requstIds.Contains(x.LeaveRequestId)).ToList();
                var empIds = leaveRequests.Select(x => x.EmployeeId).ToList();
                leaveEntitles = leaveEntitles.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            }
            if (from.HasValue && leaveEntitles.Any() && leaveEntitles.Any())
            {
                leaveRequests = leaveRequests.Where(x => x.From.Date >= from.Value.Date).ToList();
                var empIds = leaveRequests.Select(x => x.EmployeeId).ToList();
                leaveEntitles = leaveEntitles.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            }
            else if (to.HasValue && leaveEntitles.Any() && leaveEntitles.Any())
            {
                leaveRequests = leaveRequests.Where(x => x.To.Date <= to.Value.Date).ToList();
                var empIds = leaveRequests.Select(x => x.EmployeeId).ToList();
                leaveEntitles = leaveEntitles.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            }


            var anualLeaveByEmployees = leaveEntitles.GroupBy(x => x.EmployeeId).Select(g => g.First()).ToList();
            //  
            var fiscalYearIds = new HashSet<int>(leaveEntitles.Select(x => x.GlFiscalYearId));

            foreach (var employeeLeave in anualLeaveByEmployees)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeLeave.EmployeeId && x.isCurrent);
                if (placement == null)
                    continue;

                var annualLeavePerEmployee = new AnnualLeaveVM
                {
                    EmployeeName = GetFullName(placement.EmployeeId),
                    Title = GetFullJobtitleName(placement.LookupId),
                    Department = GetDepartmentName(placement.OrgStructureId),
                    Remark = employeeLeave.Remark,
                    RequesteLeaves = new List<LeaveRequestVM>(),
                    LeaveBalances = new List<LeaveBalanceVM>()

                };

                 leaveRequests = leaveRequests
                    .Where(x => x.IsOnlineApproved && x.EmployeeId == employeeLeave.EmployeeId && fiscalYearIds.Contains(x.GlFiscalYearId)).ToList();

                foreach (var leave in leaveRequests)
                {

                    var approvers = db.RequestesStatus
                                        .Where(x => x.RequestId == leave.LeaveRequestId)
                                        .ToList();

                    string depApprover = "";
                    var depApproverEntry = approvers.FirstOrDefault(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                    if (depApproverEntry?.ApproverDepartment != null)
                    {
                        depApprover = GetDepartmentName((int)depApproverEntry.ApproverDepartment);
                    }

                    string hrApprover = "";
                    var hrApproverEntry = approvers.FirstOrDefault(x => x.Status == RequestStatus.APPROVE);
                    if (hrApproverEntry?.ApproverId != null)
                    {
                        hrApprover = GetFullName((int)hrApproverEntry.ApproverId);
                    }
                    annualLeavePerEmployee.RequesteLeaves.Add(new LeaveRequestVM
                    {
                        LeaveType = leave.Type.ToString(),
                        FromAm = leave.FromAmharic,
                        ToAm = leave.ToAmharic,
                        LeaveDays = leave.LeaveDays,
                        ApproverDepartment = depApprover,
                        Approver = hrApprover,


                    });

                }

                annualLeavePerEmployee.LeaveDays = leaveRequests.Any() ? leaveRequests.Sum(x => x.LeaveDays) : 0;

                //var employeeLeaveEntitles = leaveEntitles
                //    .Where(x => x.EmployeeId == employeeLeave.EmployeeId)
                //    .OrderByDescending(x => x.GlFiscalYearId).ToList();


                //annualLeavePerEmployee.FiscalYear1 = GetFiscalYearName(employeeLeaveEntitles[0].GlFiscalYearId);
                //annualLeavePerEmployee.Balance1 = employeeLeaveEntitles[0].Balance;
                //annualLeavePerEmployee.FiscalYear2 = GetFiscalYearName(employeeLeaveEntitles[1].GlFiscalYearId);
                //annualLeavePerEmployee.Balance2 = employeeLeaveEntitles[1].Balance;
                //annualLeavePerEmployee.FiscalYear3 = GetFiscalYearName(employeeLeaveEntitles[2].GlFiscalYearId);
                //annualLeavePerEmployee.Balance3 = employeeLeaveEntitles[2].Balance;
                //annualLeavePerEmployee.TotlalAmount = employeeLeaveEntitles.Any() ? employeeLeaveEntitles.Sum(x => x.Balance) : 0;
               // Retrieve the top 3 recent fiscal year entries for the employee

               var employeeLeaveEntitles = leaveEntitles
                   .Where(x => x.EmployeeId == employeeLeave.EmployeeId)
                   .OrderByDescending(x => x.GlFiscalYearId)
                   .Take(3)
                   .ToList();

                // Ensure there are entries before accessing the array
                if (employeeLeaveEntitles.Count > 0)
                {
                    annualLeavePerEmployee.FiscalYear1 = GetFiscalYearName(employeeLeaveEntitles.ElementAtOrDefault(0)?.GlFiscalYearId ?? 0);
                    annualLeavePerEmployee.Balance1 = employeeLeaveEntitles.ElementAtOrDefault(0)?.Balance ?? 0;
                }
                if (employeeLeaveEntitles.Count > 1)
                {
                    annualLeavePerEmployee.FiscalYear2 = GetFiscalYearName(employeeLeaveEntitles.ElementAtOrDefault(1)?.GlFiscalYearId ?? 0);
                    annualLeavePerEmployee.Balance2 = employeeLeaveEntitles.ElementAtOrDefault(1)?.Balance ?? 0;
                }
                if (employeeLeaveEntitles.Count > 2)
                {
                    annualLeavePerEmployee.FiscalYear3 = GetFiscalYearName(employeeLeaveEntitles.ElementAtOrDefault(2)?.GlFiscalYearId ?? 0);
                    annualLeavePerEmployee.Balance3 = employeeLeaveEntitles.ElementAtOrDefault(2)?.Balance ?? 0;
                }

                //Calculate the total balance
                annualLeavePerEmployee.TotlalAmount = employeeLeaveEntitles.Any()? employeeLeaveEntitles.Sum(x => x.Balance):0;

                foreach (var leaveEntitle in employeeLeaveEntitles)
                {
                    annualLeavePerEmployee.LeaveBalances.Add(new LeaveBalanceVM
                    {
                        FiscalYear = GetFiscalYearName(leaveEntitle.GlFiscalYearId),
                        Balance = leaveEntitle.Balance
                    });
                }
                annualLeaves.Add(annualLeavePerEmployee);
            }

            return annualLeaves;
        }
        private static string GetFiscalYearName(int id)
        {
            using (var db = new ExceedDbContext())
            {
                return db.GLFiscalYears.Find(id)?.Name;
            }
        }

        private static string GetFullName(int id)
        {
            using (var db = new ExceedDbContext())
            {
                var employee = db.Person_Employee.Find(id);
                return employee != null
                    ? $"{employee.EmployeeCode} - {employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}"
                    : null;
            }
        }

        private static string GetFullJobtitleName(int id)
        {
            using (var db = new ExceedDbContext())
            {
                return db.Lookups.Find(id)?.Description;
            }
        }

        private static string GetDepartmentName(int id)
        {
            using (var db = new ExceedDbContext())
            {
                return db.MainCompanyStructures.Find(id)?.Name;
            }
        }
       

    }


}
