using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.Finance.GL;
using System.Text;
using System.Data;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeaveSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        private  readonly string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeaveSummary> GetLeaveSummary(object[] periodN, string branchId,string businessUnit, string id, string status, object[] dep, bool allDepartment)
        {

            bool datePreference = HrHelper.GetDatePreference();

            var placements = db.Placements.AsQueryable();

            // Apply filters efficiently
            if (!string.IsNullOrEmpty(id)&&int.TryParse(id, out int empId))
                placements = placements.Where(x => x.EmployeeId == empId);

            if (!string.IsNullOrEmpty(branchId)&&int.TryParse(branchId, out int braId))
                placements = placements.Where(x => x.BranchId == braId);
            if (!string.IsNullOrEmpty(businessUnit) &&int.TryParse(businessUnit, out int businessUnitId))
                placements = placements.Where(x => x.BusinessUnitId == businessUnitId);

            if (!string.IsNullOrEmpty(status))
            {
                placements = status == "Active"
                    ? placements.Where(x => x.isCurrent)
                    : status == "InActive"
                        ? placements.Where(x => !x.isCurrent)
                        : placements;
            }

            if (!allDepartment && dep?.Any() == true)
            {
                var orgIds = dep.Select(d => Convert.ToInt32(d.ToString())).ToList();
                placements = placements.Where(x => orgIds.Contains(x.OrgStructureId));
            }

            var placementList = placements.Where(x=>x.isCurrent)
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.OrderByDescending(p => p.PlacementId).FirstOrDefault())
                .ToList();

            var employeeIds = placementList.Select(x => x.EmployeeId).ToList();

            var persons = db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToDictionary(x => x.EmployeeId);

            var structures = db.MainCompanyStructures
                .ToDictionary(x => x.OrgStructureId);

             var jobTitles = db.Lookups
                                .Where(x=>x.LookupType ==LookUpTypes.Jobtitle_External_Experience
                                                || x.LookupType == LookUpTypes.JOB_TITLE
                                                ||x.LookupType == LookUpTypes.OLD_JOB_TITLE)
                                 .ToDictionary(x => x.LookupId);

            var periodsQuery = db.GLFiscalYears.AsQueryable();
            if (periodN?.Any() == true)
            {
                var periodIds = periodN.Select(p => Convert.ToInt32(p)).ToList();
                periodsQuery = periodsQuery.Where(x => periodIds.Contains(x.GlFiscalYearId));
            }

            var allPeriods = periodsQuery.ToList();
            var entitlements = db.LeavesEntitle
                .Where(x => employeeIds.Contains(x.EmployeeId) && !x.IsExpired)
                .ToList();

            var entByEmp = entitlements
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var leaves = db.LeaveRequest
                .Where(x => employeeIds.Contains(x.EmployeeId) && x.IsOnlineApproved && !x.IsVoid)
                .ToList();

            var leavesByEmp = leaves
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var paidCash = db.LeavePaid
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToList();

            var paidByEmp = paidCash
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var summaries = new List<LeaveSummary>();

            foreach (var placement in placementList)
            {
                var empIdLoop = placement.EmployeeId;
                var summary = new LeaveSummary { LeaveSummaryLists = new List<LeaveSummaryList>() };

                if (persons.TryGetValue(empIdLoop, out var person))
                {
                    summary.ID = person.EmployeeCode;
                    summary.FullName = person.FullNameAmharic??person.FullNameaEnglish;
                    summary.HireDate = datePreference
                        ? person.HiredDateAmharic
                        : person.HiredDate.ToString("dd.MM.yyyy");
                }

                if (structures.TryGetValue(placement.OrgStructureId, out var structName))
                    summary.Department = structName?.Name;

                if (jobTitles.TryGetValue(placement.LookupId, out var jobTitle))
                    summary.Position = jobTitle?.Description??jobTitle?.DescriptionEng;

                summary.Salary = placement.Salary;

                var empPeriods = entByEmp.ContainsKey(empIdLoop)
                    ? allPeriods.Where(p => entByEmp[empIdLoop].Any(e => e.GlFiscalYearId == p.GlFiscalYearId)).ToList(): new List<GlFiscalYear>();

                foreach (var period in empPeriods.OrderByDescending(p => p.GlFiscalYearId))
                {
                    var leaveSummary = new LeaveSummaryList
                    {
                        LeavePeriod = period.Name
                    };

                    leaveSummary.TotalAmountTaken = leavesByEmp.TryGetValue(empIdLoop, out var takenLeaves)
                        ? takenLeaves.Where(x => x.GlFiscalYearId == period.GlFiscalYearId).Sum(x => x.LeaveDays): 0;

                    leaveSummary.PainInCash = paidByEmp.TryGetValue(empIdLoop, out var paid)
                        ? Convert.ToDecimal(paid.Where(x => x.GlFiscalYearId == period.GlFiscalYearId).Sum(x => x.Amount)): 0;

                    var entitlement = entByEmp[empIdLoop].FirstOrDefault(x => x.GlFiscalYearId == period.GlFiscalYearId);
                    if (entitlement != null)
                    {
                        leaveSummary.Balance = entitlement.Balance;
                        leaveSummary.Entitlment = entitlement.Amount;
                        leaveSummary.PainInCash = (placement.Salary / 26) * entitlement.Balance;
                    }

                    summary.LeaveSummaryLists.Add(leaveSummary);
                }

                if (!summary.LeaveSummaryLists.Any())
                {
                    summary.LeaveSummaryLists.Add(new LeaveSummaryList
                    {
                        LeavePeriod = " ",
                        TotalAmountTaken = 0,
                        PainInCash = 0,
                        Balance = 0,
                        Entitlment = 0
                    });
                }

                summaries.Add(summary);
            }

            return summaries;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public string GetAllperiods(object[] periodN)
        {
            if (periodN?.Any() != true)
                return string.Empty;

            var periodIds = periodN.Select(p => Convert.ToInt32(p)).ToList();
            var names = periodIds
                .Select(id => db.GLFiscalYears.Find(id)?.Name)
                .Where(name => !string.IsNullOrEmpty(name));

            return  string.Join(", ", names) ;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetAllFiscalYears(object[] periodN)
        {
            if (periodN == null || periodN.Length == 0)
                return new List<string>();

            var periodIds = periodN
                .Where(p => int.TryParse(p?.ToString(), out _))
                .Select(p => Convert.ToInt32(p))
                .ToList();

            if (!periodIds.Any())
                return new List<string>();

            var names = db.GLFiscalYears
                          .Where(g => periodIds.Contains(g.GlFiscalYearId))
                          .Select(g => g.Name)
                          .Where(name => !string.IsNullOrEmpty(name))
                          .ToList();

            // Return as single comma-separated item
            return new List<string>
            {
                string.Join(", ", names)
            };

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public DataTable GetAllFiscalYearss(object[] periodN)
        {
            DataTable table = new DataTable();
            table.Columns.Add("FiscalYears", typeof(string));

            if (periodN == null || periodN.Length == 0)
            {
                table.Rows.Add(string.Empty);
                return table;
            }

            var periodIds = periodN
                .Where(p => int.TryParse(p?.ToString(), out _))
                .Select(p => Convert.ToInt32(p))
                .ToList();

            if (!periodIds.Any())
            {
                table.Rows.Add(string.Empty);
                return table;
            }

            var names = db.GLFiscalYears
                          .Where(g => periodIds.Contains(g.GlFiscalYearId))
                          .Select(g => g.Name)
                          .Where(name => !string.IsNullOrEmpty(name))
                          .ToList();

            // Concatenate into one string
            string concatenated = string.Join(" ", names);

            // Put into one row, one column
            table.Rows.Add(concatenated);

            return table;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetAllPeriods(object[] periodN)
        {
            if (periodN == null || periodN.Length == 0)
                return new List<string>();

            var periodIds = periodN
                .Where(p => int.TryParse(p?.ToString(), out _))
                .Select(p => Convert.ToInt32(p))
                .ToList();

            if (!periodIds.Any())
                return new List<string>();

            // Fetch in one query (better performance than Find in a loop)
            var names = db.GLPeriods
                          .Where(g => periodIds.Contains(g.GLPeriodId))
                          .Select(g => g.Name)
                          .Where(name => !string.IsNullOrEmpty(name))
                          .ToList();

            return names;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }


    //public class LeaveSummaryObjectDataSource : List<LeaveSummary>
    //{
    //    private ExceedDbContext db = new ExceedDbContext();

    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public List<LeaveSummary> GetLeaveSummary(object[] periodN, string branchId, string id, string status, object[] dep, bool allDepartment)
    //    {
    //        List<LeaveSummary> summaries = new List<LeaveSummary>();
    //        var sum = new LeaveSummary();
    //        var LeaveSummaryone = new LeaveSummaryList();

    //        // Ensure placements is not null
    //        var placements = db.Placements.AsQueryable() ;

    //        if (!string.IsNullOrEmpty(id))
    //        {
    //            int.TryParse(id, out int empId);
    //            placements = placements.Where(x => x.EmployeeId == empId);
    //        }

    //        if (!string.IsNullOrEmpty(branchId))
    //        {
    //            int.TryParse(branchId, out int braId);
    //            placements = placements.Where(x=>x.BranchId==braId);
    //        }

    //        if (!string.IsNullOrEmpty(status))
    //        {
    //            if (status == "Active")
    //            {
    //                placements = placements.Where(x => x.isCurrent);
    //            }
    //            if (status == "InActive")
    //            {
    //                placements = placements.Where(x => !x.isCurrent);
    //            }
    //        }

    //        if (!allDepartment && dep?.Any() == true)
    //        {
    //            List<int> orgIds = dep.Select(d => Convert.ToInt32(d.ToString())).ToList();
    //            placements = placements.Where(x => orgIds.Contains(x.OrgStructureId));
    //        }

    //        // Ensure periods is not null
    //        var periods = db.GLFiscalYears?.ToList() ?? new List<GlFiscalYear>();

    //        if (periodN?.Any() == true)
    //        {
    //            List<int> ids = periodN.Select(b => Convert.ToInt32(b)).ToList();
    //            periods = periods.Where(x => ids.Contains(x.GlFiscalYearId)).ToList();
    //        }

    //        var placementsDistinicted = placements.DistinctByField(x => x.EmployeeId).ToList();

    //        foreach (var e in placementsDistinicted)
    //        {
    //            sum = new LeaveSummary { LeaveSummaryLists = new List<LeaveSummaryList>() };

    //            var person = db.Person_Employee?.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
    //            if (person != null)
    //            {
    //                sum.ID = person.EmployeeCode;
    //                sum.FullName = person.FullNameAmharic;
    //            }
    //            LeaveSummaryone.Salary = placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == e.EmployeeId)?.Salary ?? 0;
    //            var activePeriods = db.LeavesEntitle?.Where(x => !x.IsExpired && x.EmployeeId == e.EmployeeId).Select(x => x.GlFiscalYearId).ToList() ?? new List<int>();

    //            var empActivePeriods = periods.Where(x => activePeriods.Contains(x.GlFiscalYearId)).OrderByDescending(X=>X.GlFiscalYearId).ToList();

    //            foreach (var period in empActivePeriods)
    //            {
    //                LeaveSummaryone = new LeaveSummaryList
    //                {
    //                    LeavePeriod = period.Name
    //                };
    //                //var leaveTaken = db.Leaves?.Where(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId).ToList() ?? new List<Leave>();
    //                var leaveTaken = db.LeaveRequest.Where(x => x.EmployeeId == e.EmployeeId
    //                                                        && x.GlFiscalYearId == period.GlFiscalYearId
    //                                                        &&x.IsOnlineApproved 
    //                                                        &&!x.IsVoid).ToList() ?? new List<LeaveRequest>();
    //                var paidIncash = db.LeavePaid?.Where(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId).ToList() ?? new List<LeavePaid>();
    //                var leaveEntitlments = db.LeavesEntitle?.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId);

    //                LeaveSummaryone.TotalAmountTaken = leaveTaken.Sum(x => x.LeaveDays);
    //                LeaveSummaryone.PainInCash = Convert.ToDecimal(paidIncash.Sum(x => x.Amount));
    //                //LeaveSummaryone.Salary = placements.Any()?(decimal) placements.FirstOrDefault(x=>x.isCurrent&&x.EmployeeId==e.EmployeeId)?.Salary:0;
    //                if (leaveEntitlments != null)
    //                {
    //                    LeaveSummaryone.Balance = leaveEntitlments.Balance;
    //                    LeaveSummaryone.Entitlment = leaveEntitlments.Amount;

    //                    LeaveSummaryone.PainInCash = e.Salary / 26 * leaveEntitlments.Balance;
    //                }

    //                sum.LeaveSummaryLists.Add(LeaveSummaryone);
    //            }

    //            if (!sum.LeaveSummaryLists.Any())
    //            {
    //                LeaveSummaryone = new LeaveSummaryList
    //                {
    //                    LeavePeriod = " ",
    //                    TotalAmountTaken = 0,
    //                    PainInCash = 0,
    //                    Balance = 0,
    //                    Entitlment = 0
    //                };
    //                sum.LeaveSummaryLists.Add(LeaveSummaryone);
    //            }

    //            if (e.isCurrent)
    //            {
    //                var stracture = db.MainCompanyStructures?.FirstOrDefault(x => x.OrgStructureId == e.OrgStructureId);
    //                if (stracture != null)
    //                {
    //                    sum.Department = stracture.Name;
    //                    sum.Salary = e.Salary;
    //                }
    //            }
    //            else
    //            {
    //                var place = placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
    //                if (place != null)
    //                {
    //                    var stracture = db.MainCompanyStructures?.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
    //                    if (stracture != null)
    //                    {
    //                        sum.Department = stracture.Name;
    //                        sum.Salary = place.Salary;
    //                    }
    //                }
    //            }

    //            summaries.Add(sum);
    //        }

    //        return summaries;
    //    }
    //public List<LeaveSummary> GetLeaveSummary(object[] periodN, string branchId, /*string orgStructureId,*/ string id, string status, object[] dep, bool allDepartment)
    //{
    //    List<LeaveSummary> summaries = new List<LeaveSummary>();
    //    var sum = new LeaveSummary();

    //    var LeaveSummaryone = new LeaveSummaryList();

    //    var placements = db.Placements.ToList();

    //    if (!string.IsNullOrEmpty(id))
    //    {
    //        int.TryParse(id, out int empId);
    //        placements = placements.Where(x => x.EmployeeId == empId).ToList();
    //    }

    //    if (!string.IsNullOrEmpty(branchId))
    //    {
    //        int.TryParse(branchId, out int braId);
    //        placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
    //    }
    //    if (!string.IsNullOrEmpty(status))
    //    {
    //        if (status == "Active")
    //        {
    //            placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements).OrderByDescending(x => x.AssignmentDate).ToList();
    //        }
    //        if (status == "InActive")
    //        {
    //            placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements).OrderByDescending(x => x.AssignmentDate).ToList();
    //        }

    //    }

    //    if (!allDepartment)
    //    {
    //        if (dep.Any())
    //        {
    //            List<int> orgIds = new List<int>();

    //            foreach (var d in dep.ToList())
    //            {
    //                orgIds.Add(Convert.ToInt32(d.ToString()));
    //            }

    //            placements = placements.Where(x => orgIds.Contains(x.OrgStructureId)).ToList();
    //        }
    //    }

    //    var periods = db.GLFiscalYears.ToList();
    //    var periodNs = periodN.ToList();
    //    if (periodNs.Any())
    //    {
    //        List<int> ids = new List<int>();
    //        foreach (var b in periodNs)
    //        {
    //            int filt =  Convert.ToInt32(b);
    //            ids.Add(filt);
    //        }
    //        periods = periods.Where(x => ids.Contains(x.GlFiscalYearId)).ToList();

    //    }
    //    //if (periodN != null)
    //    //{
    //    //    int.TryParse(periodN, out int GLFiscalYearId);
    //    //    periods = periods.Where(x => x.GlFiscalYearId == GLFiscalYearId).ToList();
    //    //}

    //    var placementsDistinicted = placements.DistinctByField(x => x.EmployeeId).ToList();

    //    foreach (var e in placementsDistinicted)
    //    {
    //        sum = new LeaveSummary();
    //        sum.LeaveSummaryLists = new List<LeaveSummaryList>();

    //        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
    //        if (person != null)
    //        {
    //            sum.ID = person.EmployeeCode;
    //            sum.FullName = person.FullNameAmharic;
    //        }

    //        var activePeriods = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == e.EmployeeId).Select(x => x.GlFiscalYearId).ToList();

    //        var empActivePeriods = periods.Where(x => activePeriods.Contains(x.GlFiscalYearId)).ToList();

    //        foreach (var period in empActivePeriods)
    //        {
    //            LeaveSummaryone = new LeaveSummaryList();

    //            LeaveSummaryone.LeavePeriod = period.Name;

    //            var leaveTaken = db.Leaves.Where(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId).ToList();
    //            var paidIncash = db.LeavePaid.Where(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId).ToList();
    //            var leaveEntitlments = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId);

    //            LeaveSummaryone.TotalAmountTaken = leaveTaken.Sum(x => x.Amount);
    //            LeaveSummaryone.PainInCash = Convert.ToDecimal(paidIncash.Sum(x => x.Amount));

    //            if (leaveEntitlments != null)
    //            {
    //                LeaveSummaryone.Balance = leaveEntitlments.Balance;
    //                LeaveSummaryone.Entitlment = leaveEntitlments.Amount;

    //                LeaveSummaryone.PainInCash = e.Salary / 26 * leaveEntitlments.Balance;
    //            }

    //            sum.LeaveSummaryLists.Add(LeaveSummaryone);
    //        }
    //        if (!sum.LeaveSummaryLists.Any())
    //        {
    //            LeaveSummaryone = new LeaveSummaryList();

    //            LeaveSummaryone.LeavePeriod = " ";
    //            LeaveSummaryone.TotalAmountTaken = 0;
    //            LeaveSummaryone.PainInCash = 0;
    //            LeaveSummaryone.Balance = 0;
    //            LeaveSummaryone.Entitlment = 0;
    //        }

    //        if (e.isCurrent)
    //        {
    //            var stracture = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == e.OrgStructureId);
    //            if (stracture != null)
    //            {
    //                sum.Department = stracture.Name;
    //                sum.Salary = e.Salary;
    //            }
    //        }
    //        else
    //        {
    //            var place = placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
    //            if (place != null)
    //            {
    //                var stracture = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
    //                if (stracture != null)
    //                {
    //                    sum.Department = stracture.Name;
    //                    sum.Salary = place.Salary;
    //                }
    //            }
    //        }

    //        summaries.Add(sum);
    //    }

    //    return summaries;
    //}
}
