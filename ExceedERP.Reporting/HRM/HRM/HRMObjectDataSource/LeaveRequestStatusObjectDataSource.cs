using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeaveRequestStatusObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeaveRequestStatusVM> GetLeaveRequestStatus(string empId, string leaveType, string periodId, 
                                                                string branchId, string businessUnitId, string orgStructureId, string status,
                                                                DateTime? from, DateTime? to, object[] jobtitle)
        {
            var resultList = new List<LeaveRequestStatusVM>();

            //  Prepare base query
            var leaveQuery = db.LeaveRequest.AsNoTracking().AsQueryable();

            //  Apply filters safely (TryParse avoids exceptions)
            if (int.TryParse(empId, out int employeeId))
                leaveQuery = leaveQuery.Where(x => x.EmployeeId == employeeId);

            if (Enum.TryParse<LeaveType>(leaveType, out var leaveTypeEnum))
                leaveQuery = leaveQuery.Where(x => x.Type == leaveTypeEnum);

            if (int.TryParse(periodId, out int fiscalId))
                leaveQuery = leaveQuery.Where(x => x.GlFiscalYearId == fiscalId);

            if (from.HasValue)
                leaveQuery = leaveQuery.Where(x => x.From >= from.Value);

            if (to.HasValue)
                leaveQuery = leaveQuery.Where(x => x.To <= to.Value);

            // Placement filtering (branch/org)
            var placementQuery = db.Placements.AsNoTracking().AsQueryable();

            if (int.TryParse(branchId, out int bId))
                leaveQuery = leaveQuery.Where(p => p.BranchId == bId);

            if (int.TryParse(orgStructureId, out int oId))
                placementQuery = placementQuery.Where(p => p.OrgStructureId == oId);
            if (int.TryParse(businessUnitId, out int bUId))
                placementQuery = placementQuery.Where(p => p.BusinessUnitId == bUId);

            if (jobtitle != null && jobtitle.Any())
            {
                var jobtitles = jobtitle
                    .Where(b => b != null)
                    .Select(b => Convert.ToInt32(b))
                    .ToList();

                leaveQuery = leaveQuery.Where(x => jobtitles.Contains(x.LookupId??0));
            }

            // Join directly instead of Contains()
            leaveQuery =
                from l in leaveQuery
                join p in placementQuery on l.EmployeeId equals p.EmployeeId
                select l;

            //  Filter by status (if provided)
            if (!string.IsNullOrEmpty(status)&&Enum.TryParse<RequestStatus>(status, out var statusEnum))
            {
                var validRequestIds = db.RequestesStatus
                    .AsNoTracking()
                    .Where(r => r.Status == statusEnum)
                    .Select(r => r.RequestId);

                leaveQuery = leaveQuery.Where(l => validRequestIds.Contains(l.LeaveRequestId));
            }

            //  Materialize
            var leaveList = leaveQuery.ToList();
            if (!leaveList.Any())
                return resultList;

            //  Prefetch related data for these employees
            var empIds = leaveList.Select(l => l.EmployeeId).Distinct().ToList();
            var leaveRequestIds = leaveList.Select(l => l.LeaveRequestId).ToList();

            var personDict = db.Person_Employee.AsNoTracking().Where(p => empIds.Contains(p.EmployeeId)).ToDictionary(p => p.EmployeeId);
           
            var placementDict = db.Placements.AsNoTracking()
                                            .Where(p => empIds.Contains(p.EmployeeId))
                                            .GroupBy(p => p.EmployeeId)
                                            .Select(g => new
                                            {
                                                EmployeeId = g.Key,
                                                Placement = g
                                                    .OrderByDescending(p => p.isCurrent) // true first
                                                    .ThenByDescending(p => p.AssignmentDate) // latest next  
                                                    .FirstOrDefault()
                                            }).ToDictionary(x => x.EmployeeId, x => x.Placement);

            var orgDict = db.MainCompanyStructures.AsNoTracking()
                .ToDictionary(o => o.OrgStructureId);

            var lookupDict = db.Lookups.AsNoTracking()
                .ToDictionary(l => l.LookupId);

            var leavePlans = db.LeavePlans.AsNoTracking()
                .Where(p => empIds.Contains(p.EmployeeId))
                .ToList();

            var requestStatuses = db.RequestesStatus.AsNoTracking()
                .Where(r => leaveRequestIds.Contains(r.RequestId))
                .ToList();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;

            //  Build result per employee
            foreach (var group in leaveList.GroupBy(x => x.EmployeeId))
            {
                int emp = group.Key;
                personDict.TryGetValue(emp, out var person);
                placementDict.TryGetValue(emp, out var placement);

                var item = new LeaveRequestStatusVM
                {
                    ApplicantName = person != null ? $"{person.EmployeeCode}-{person.FullNameaEnglish}" : " ",
                    Department = (placement != null && orgDict.TryGetValue(placement.OrgStructureId, out var dept)) ? dept.Name : " ",
                    JobTitle = (placement != null && lookupDict.TryGetValue(placement.LookupId, out var job)) ? (job.Description ?? job.DescriptionEng) : " ",
                    LeaveRequestStatus = new List<RequestesStatusVM>()
                };

                // Loop leave requests per employee
                foreach (var leave in group)
                {
                    var leaveVM = new RequestesStatusVM
                    {
                        Period = leave.GlFiscalYear?.Name,
                        Type = leave.Type.ToString(),
                        ActionType = leave.LeaveActionTypes.ToString(),
                        Day = leave.Day.ToString(),
                        From = datePreference? leave.FromAmharic:leave.From.ToString("dd.MM.yyyy"),
                        To = datePreference ? leave.ToAmharic : leave.To.ToString("dd.MM.yyyy"),
                        RequestAmount = leave.LeaveBalance,
                        LeaveDays = leave.LeaveDays
                    };

                    // Match Leave Plan if exists
                    if (leave.Type == LeaveType.ANNUAL || leave.Type == LeaveType.DayOff)
                    {
                        var plan = leavePlans.FirstOrDefault(x =>
                            x.EmployeeId == leave.EmployeeId &&
                            x.GlFiscalYearId == leave.GlFiscalYearId &&
                            x.From <= leave.From && x.To >= leave.To);

                        if (plan != null)
                        {
                            leaveVM.FromPlan = datePreference ? plan.FromAmharic : plan.From.ToString("dd.MM.yyyy");
                            leaveVM.ToPlan = datePreference ? plan.ToAmharic : plan.To.ToString("dd.MM.yyyy"); 
                            leaveVM.PlannedLeaveDays = plan.LeaveAmount;
                            leaveVM.Variation = Math.Abs(plan.LeaveAmount - leave.LeaveDays);
                        }
                    }

                    // 🔹 Add request status details
                    var statuses = requestStatuses
                        .Where(s => s.RequestId == leave.LeaveRequestId)
                        .ToList();

                    if (Enum.TryParse<RequestStatus>(status, out var filterStatus))
                        statuses = statuses.Where(s => s.Status == filterStatus).ToList();

                    leaveVM.RequestStatusDetail = statuses.Select(s =>
                    {
                        personDict.TryGetValue(s.ApproverId ?? 0, out var approver);
                        placementDict.TryGetValue(s.ApproverId ?? 0, out var approverPlacement);
                        string approverDept = (approverPlacement != null && orgDict.TryGetValue(approverPlacement.OrgStructureId, out var org))
                            ? org.Name : " ";

                        return new RequestesStatusDetailVM
                        {
                            Date = datePreference ? s.DateReqStatAm : s.Date.ToString("dd.MM.yyyy") ,
                            Status = s.Status.ToString(),
                            AproverName = $"{approver?.FirstNameEnglish} {approver?.MiddleNameEnglish} {approver?.LastNameEnglish} ",
                            ApproverDepartmentName = approverDept
                        };
                    }).ToList();

                    item.LeaveRequestStatus.Add(leaveVM);
                }

                resultList.Add(item);
            }

            return resultList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobTitleMV> GetJobTitleParams(object[] jobtitle)
        {
            var resultList = new List<JobTitleMV>();

            // Load lookup dictionary only once
            var lookupDict = db.Lookups
                .AsNoTracking()
                .ToDictionary(l => l.LookupId);

            // Validate input
            if (jobtitle == null || !jobtitle.Any())
                return resultList;

            // Convert input job title IDs
            var jobTitles = jobtitle
                .Where(x => x != null)
                .Select(x => Convert.ToInt32(x))
                .ToList();

            // Collect job titles
            var titles = new List<string>();

            foreach (var jobTit in jobTitles)
            {
                if (lookupDict.TryGetValue(jobTit, out var job))
                {
                    titles.Add(job.Description ?? job.DescriptionEng);
                }
            }

            // Create one row with comma-separated titles
            resultList.Add(new JobTitleMV
            {
                JobTitle = string.Join(", ", titles)
            });

            return resultList;
        }


        public void Dispose()
        {
            db.Dispose();
        }
        public class JobTitleMV
        {
            public string JobTitle { get; set; }
        }
    }

    
}