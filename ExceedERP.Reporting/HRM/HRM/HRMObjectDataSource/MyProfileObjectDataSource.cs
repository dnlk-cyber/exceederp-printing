using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System.Data.Entity;
using System.Windows.Forms;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class MyProfileObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationContext userdetail = new ApplicationContext();
        string cultureName = GlobalHelper.GetCurrentCulture();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PromotedEmployee> GetPromotedEmployee(string id)
        {
            if (!int.TryParse(id, out int employeeId))
                return new List<PromotedEmployee>();

            var datePref = db.DatePreferenceSettings.AsNoTracking().FirstOrDefault();
            bool useEthDate = datePref?.UseEthiopianDate ?? true;

            var lookups = db.Lookups.AsNoTracking().ToList();
            var jobTitles = lookups
                .Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.Jobtitle_External_Experience
                         || x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE
                         || x.LookupType == Core.Domain.Common.LookUpTypes.OLD_JOB_TITLE)
                .ToDictionary(x => x.LookupId);

            var branches = db.Branch.AsNoTracking().ToDictionary(x => x.BranchId);

            var educations = db.Education.AsNoTracking()
                .Where(e => e.EmployeeId == employeeId).ToList();

            var experiences = db.Experiences.AsNoTracking()
                .Where(e => e.EmployeeId == employeeId).ToList();

            var placements = db.Placements.AsNoTracking()
                .Where(p => p.EmployeeId == employeeId).ToList();

            var histories = db.PlacementHistories.OrderByDescending(x=>x.To).AsNoTracking()
                .Where(h => h.EmployeeId == employeeId).ToList();

            var employee = (from e in db.Person_Employee
                            join p in db.Placements on e.EmployeeId equals p.EmployeeId
                            where e.EmployeeId == employeeId && e.IsActive && p.isCurrent
                            select new { e, p }).FirstOrDefault();

            if (employee == null)
                return new List<PromotedEmployee>();

            var empPerformance = db.EmployeePerformanceSummary.AsNoTracking()
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                .FirstOrDefault();

            var leaveEntitles = db.LeavesEntitle.AsNoTracking()
                .Where(x => x.EmployeeId == employeeId && !x.IsExpired)
                .OrderByDescending(x => x.GlFiscalYearId)
                .ToList();

            var fiscalYears = db.GLFiscalYears.AsNoTracking()
                .ToDictionary(x => x.GlFiscalYearId);

            int totalInternalDays = 0;
            int totalExternalDays = 0;
            int totalExperienceDays = 0;

            var result = new PromotedEmployee
            {
                No = 1,
                ApplicantName = $"{employee.e.FirstNameEnglish} {employee.e.MiddleNameEnglish} {employee.e.LastNameEnglish}",
                DateOfBirth = employee.e.DateOfBirth.ToString("dd.MM.yyyy"),
                Age = employee.e.DateOfBirth != null ? employee.e.DateOfBirth.GetAge().ToString() : "0",
                Perfromance = (empPerformance?.Result ?? 0).ToString(),
                RecruitmentType = HrHelper.GetEmployeeStatus(employee.e.Status),
                LeaveBalance = new List<LeaveBalanceViewModel> (),
                Educations = new List<EmployeeEducation> (),
                Expriance = new List<CurrentEmployeeExpriance> (),
                OtherExpriance = new List<OtherEmployeeExpriance> ()
            };

            // Leave Balance
            foreach (var le in leaveEntitles)
            {
                if (fiscalYears.TryGetValue(le.GlFiscalYearId, out var fy))
                {
                    result.LeaveBalance.Add(new LeaveBalanceViewModel
                    {
                        Year = useEthDate ? HrHelper.GetECDate(fy.DateTo).ToString() : fy.DateTo.Year.ToString(),
                        Balance = le.Balance
                    });
                }
            }

            // Education
            foreach (var e in educations.OrderByDescending(X=>X.To))
            {
                result.Educations.Add(new EmployeeEducation
                {
                    From = useEthDate ? e.FromEAmharic : e.From?.ToString("dd.MM.yyyy"),
                    To = useEthDate ? e.ToEAmharic : e.To.ToString("dd.MM.yyyy"),
                    OrgName = e.OrganizationName,
                    EduField = lookups.FirstOrDefault(l => l.LookupId == e.EduField)?.Description,
                    EduLevel = lookups.FirstOrDefault(l => l.LookupId == e.EduLevel)?.Description
                });
            }

            // External Experience
            foreach (var ex in experiences.Where(x => x.From != null && x.To != null))
            {
                var days = (ex.To - ex.From).Days;
                totalExternalDays += days;
                totalExperienceDays += days;

                jobTitles.TryGetValue(ex.LookupId??0 , out var jt);

                result.OtherExpriance.Add(new OtherEmployeeExpriance
                {
                    From = useEthDate ? ex.FromExAmharic : ex.From.ToString("dd.MM.yyyy"),
                    To = useEthDate ? ex.ToExAmharic : ex.To.ToString("dd.MM.yyyy"),
                    CompanyName = ex.Organozation,
                    JobTitleName = jt?.Description ?? ex.Responsibility,
                    ExpExtSingle = GetLocalizedExperienceText(days)
                });
            }

            // Internal Experience
            var sortedPlacements = placements.OrderByDescending(x => x.AssignmentDate).ToList();

            for (int i = 0; i < sortedPlacements.Count; i++)
            {
                var current = sortedPlacements[i];
                var nextDate = (i == 0) ? DateTime.Now : sortedPlacements[i - 1].AssignmentDate;

                var days = Math.Abs((nextDate - current.AssignmentDate).Days);
                totalInternalDays += days;
                totalExperienceDays += days;

                jobTitles.TryGetValue(current.LookupId, out var jt);

                result.Expriance.Add(new CurrentEmployeeExpriance
                {
                    From = useEthDate ? current.AssignmentDateAmharic : current.AssignmentDate.ToString("dd.MM.yyyy"),
                    To = useEthDate ? XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                                   : DateTime.Now.ToString("dd.MM.yyyy"),
                    JobTitleName = jt?.Description ?? current.JobTitleName ?? "-",
                    IntExpSingle = GetLocalizedExperienceText(days)
                });
            }

            // Histories
            foreach (var h in histories)
            {
                var days = (h.To - h.From).Days;
                totalInternalDays += days;
                totalExperienceDays += days;

                jobTitles.TryGetValue(h.LookupId, out var jt);

                result.Expriance.Add(new CurrentEmployeeExpriance
                {
                    From = useEthDate ? h.FromAmharic : h.From.ToString("dd.MM.yyyy"),
                    To = useEthDate ? h.ToAmharic : h.To.ToString("dd.MM.yyyy"),
                    JobTitleName = jt?.Description ?? h.JobTitle ?? "-",
                    IntExpSingle = GetLocalizedExperienceText(days)
                });
            }

            // Branch & Salary
            var activePlacement = placements.FirstOrDefault(x => x.isCurrent);
            if (activePlacement != null && branches.TryGetValue(activePlacement.BranchId, out var branch))
            {
                result.Branch = branch?.Name ?? branch?.AmharicName;
                result.CurrentSalary = activePlacement.Salary;
            }

            result.ExtExp = GetLocalizedExperienceText(totalExternalDays);
            result.IntExp = GetLocalizedExperienceText(totalInternalDays);
            result.TotalYear = GetLocalizedExperienceText(totalExperienceDays);

            return new List<PromotedEmployee> { result };
        }

        public List<PromotedEmployee> GetPromotedEmployeeExisted(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out int employeeId))
                return new List<PromotedEmployee>();

            // Cache lookup and static tables once
            var lookups = db.Lookups.AsNoTracking().ToList();
            var branches = db.Branch.AsNoTracking().ToDictionary(z=>z.BranchId);
            var educations = db.Education.AsNoTracking().Where(e => e.EmployeeId == employeeId).ToList();
            var experiences = db.Experiences.AsNoTracking().Where(e => e.EmployeeId == employeeId).ToList();
            var placements = db.Placements.AsNoTracking().Where(p => p.EmployeeId == employeeId).ToList();
            var placementHistories = db.PlacementHistories.AsNoTracking().Where(h => h.EmployeeId == employeeId).ToList();

            // Employee and placement info
            var employees = (from e in db.Person_Employee
                             join p in db.Placements on e.EmployeeId equals p.EmployeeId
                             where e.IsActive && p.isCurrent && e.EmployeeId == employeeId
                             select new { e, p }).ToList();

            // Leave entitlements & fiscal years
            var leaveEntitles = db.LeavesEntitle
                .AsNoTracking()
                .Where(x => x.EmployeeId == employeeId && !x.IsExpired)
                .OrderByDescending(x => x.GlFiscalYearId)
                .ToList();
            var fiscalYears = db.GLFiscalYears.AsNoTracking().ToDictionary(x => x.GlFiscalYearId);

            // Performance
            var empPerformance = db.EmployeePerformanceSummary
                .AsNoTracking()
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.EmployeePerformanceSummaryId)
                .FirstOrDefault();

            var promotedEmployees = new List<PromotedEmployee>();
            int totalInternalDays = 0, totalExternalDays = 0, totalExperienceDays = 0;
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            var jobTitles = lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.Jobtitle_External_Experience
                                       || x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE
                                       || x.LookupType == Core.Domain.Common.LookUpTypes.OLD_JOB_TITLE).ToList();

            foreach (var r in employees)
            {
               
             
                var promoted = new PromotedEmployee
                {
                    LeaveBalance = new List<LeaveBalanceViewModel>(),
                    Educations = new List<EmployeeEducation>(),
                    OtherExpriance = new List<OtherEmployeeExpriance>(),
                    Expriance = new List<CurrentEmployeeExpriance>(),
                    No = promotedEmployees.Count + 1,
                    ApplicantName = $"{r.e.FirstNameEnglish} {r.e.MiddleNameEnglish} {r.e.LastNameEnglish}",
                    DateOfBirth = r.e.DateOfBirth.ToString("dd.MM.yyyy"),
                    Age = r.e.DateOfBirth.GetAge().ToString(),
                    Perfromance = (empPerformance?.Result ?? 0).ToString(),
                    RecruitmentType = HrHelper.GetEmployeeStatus(r.e.Status)
                };

                // --- LEAVE BALANCE ---
                foreach (var le in leaveEntitles)
                {
                    if (fiscalYears.TryGetValue(le.GlFiscalYearId, out var fy))
                    {
                        promoted.LeaveBalance.Add(new LeaveBalanceViewModel
                        {
                            Year = datePreference ? HrHelper.GetECDate(fy.DateTo).ToString(): fy.DateTo.Year.ToString(),
                            Balance = le.Balance
                        });
                    }
                }

                // --- EDUCATION ---
                foreach (var e in educations.OrderByDescending(x => x.From).ThenByDescending(x => x.To))
                {
                    promoted.Educations.Add(new EmployeeEducation
                    {
                        From = datePreference?e?.FromEAmharic:e.From.HasValue?e.From.Value.ToString("dd.MM.yyyy"):null,
                        To = datePreference?e?.ToEAmharic:e.To.ToString("dd.MM.yyyy"),
                        OrgName = e?.OrganizationName,
                        EduField = lookups?.FirstOrDefault(l => l.LookupId == e.EduField)?.Description,
                        EduLevel = lookups?.FirstOrDefault(l => l.LookupId == e.EduLevel)?.Description
                    });
                }

                // --- EXTERNAL EXPERIENCE ---
                foreach (var ex in experiences.OrderByDescending(x => x.From))
                {
                    var durationDays = (ex.To - ex.From).Days;
                    totalExternalDays += durationDays;
                    totalExperienceDays += durationDays;
                    var jobTitle = jobTitles.FirstOrDefault(x => x.LookupId == ex.LookupId);

                    var parts = XAPI.Common.GetConcatenatedYearMonthDay(ex.From, ex.To).Split('-');
                    promoted.OtherExpriance.Add(new OtherEmployeeExpriance
                    {
                        From = datePreference? ex.FromExAmharic:ex.From.ToString("dd.MM.yyyy"),
                        To = datePreference ? ex.ToExAmharic:ex.To.ToString("dd.MM.yyyy"),
                        CompanyName = ex.Organozation,
                        JobTitleName = jobTitle?.Description??ex.Responsibility,
                        Year =  XAPI.Common.GetYearMonthDay(ex.From, ex.To),
                        DYear =  Convert.ToInt32(parts[0]),
                        DMonth = Convert.ToInt32(parts[1]),
                        ExpExtSingle = GetLocalizedExperienceText(durationDays)
                    });
                }

                promoted.OtherYear = promoted.OtherExpriance.Sum(x => x.DYear);
                promoted.OtherMonth = promoted.OtherExpriance.Sum(x => x.DMonth);
                int otherYear = promoted.OtherYear;
                int otherMonth = promoted.OtherMonth;
                NormalizeMonths(ref otherYear, ref otherMonth);

                promoted.ExtExp = GetLocalizedExperienceText(totalExternalDays);
                // --- INTERNAL EXPERIENCE (Placements + Histories) ---
                ComputeInternalExperience(promoted, placements, placementHistories, jobTitles, ref totalInternalDays, ref totalExperienceDays);

                promoted.IntExp = GetLocalizedExperienceText(totalInternalDays);

                // --- BRANCH & SALARY ---
                var activePlacement = placements.FirstOrDefault(x => x.isCurrent);
                if (activePlacement != null)
                {
                    branches.TryGetValue(activePlacement.BranchId, out var branch);
                    promoted.CurrentSalary = activePlacement.Salary;
                    promoted.Branch =branch?.Name??branch?.AmharicName;
                }

                // --- TOTAL EXPERIENCE ---
                var totalYear = promoted.DYear + promoted.OtherYear;
                var totalMonth = promoted.DMonth + promoted.OtherMonth;
                NormalizeMonths(ref totalYear, ref totalMonth);

                promoted.TotalMonth = totalMonth.ToString();
                promoted.TotalYear = GetLocalizedExperienceText(totalExperienceDays);

                promotedEmployees.Add(promoted);
            }

            return promotedEmployees.OrderBy(x => x.No).ToList();
        }

        private void NormalizeMonths(ref int years, ref int months)
        {
            if (months >= 12)
            {
                years += months / 12;
                months %= 12;
            }
        }

        private string GetLocalizedExperienceText(int days)
        {
            switch (cultureName.ToLower())
            {
                case "am":
                case "am-et":
                    return EmployeeHelper.GetDaystoWordsAmharic(days);
                default:
                    return EmployeeHelper.GetDaystoWords(days);
            }
        }

        private void ComputeInternalExperience(PromotedEmployee promoted, List<Placement> placements,  List<PlacementHistory> histories, List<Lookup> jobTitles,
            ref int totalInternalDays, ref int totalExperienceDays)
        {

            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;

            var sortedPlacements = placements.OrderByDescending(x => x.PlacementId).ToList();

            for (int i = 0; i < sortedPlacements.Count; i++)
            {
                var loockpId = sortedPlacements[i].LookupId;
                var current = sortedPlacements[i];
                var nextDate = (i == 0) ? DateTime.Now : sortedPlacements[i - 1].AssignmentDate;
                var days = Math.Abs((nextDate - current.AssignmentDate).Days);
                totalInternalDays += days;
                totalExperienceDays += days;
                var jobTitle = jobTitles.FirstOrDefault(x => x.LookupId == loockpId);
                promoted.Expriance.Add(new CurrentEmployeeExpriance
                {
                    From = datePreference ? current.AssignmentDateAmharic : current.AssignmentDate.ToString("dd.MM.yyyy"),
                    To = (i == 0)? datePreference
                                        ? XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                                        : DateTime.Now.ToString("dd.MM.yyyy")
                                    : datePreference
                                        ? sortedPlacements[i - 1].AssignmentDateAmharic
                                        : sortedPlacements[i - 1].AssignmentDate.ToString("dd.MM.yyyy"),
                    JobTitleName =  jobTitle.Description?? current.JobTitleName ??"-",
                    IntExpSingle = GetLocalizedExperienceText(days)
                });
            }

            // Add histories
            foreach (var h in histories.OrderByDescending(x => x.PlacementHistoryId))
            {
                var days = (h.To - h.From).Days;
                totalInternalDays += days;
                totalExperienceDays += days;
                var jobTitle = jobTitles.FirstOrDefault(x => x.LookupId == h.LookupId);

                promoted.Expriance.Add(new CurrentEmployeeExpriance
                {
                    From = datePreference ? h.FromAmharic : h.From.ToString("dd.MM.yyyy"),
                    To = datePreference ? h.ToAmharic : h.To.ToString("dd.MM.yyyy"),
                    JobTitleName = jobTitle.Description ?? h.JobTitle ?? "-",
                    IntExpSingle = GetLocalizedExperienceText(days)
                });
            }

            promoted.DYear = promoted.Expriance.Sum(x => x.DYear);
            promoted.DMonth = promoted.Expriance.Sum(x => x.DMonth);
            int proYear = promoted.DYear;
            int proMonth = promoted.DMonth;

            NormalizeMonths(ref proYear, ref proMonth);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}