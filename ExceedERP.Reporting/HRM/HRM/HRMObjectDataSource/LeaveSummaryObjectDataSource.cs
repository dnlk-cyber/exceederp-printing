using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeaveSummaryObjectDataSource : List<LeaveSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeaveSummary> GetLeaveSummary(string periodN, string branchId, /*string orgStructureId,*/ string id, string status, object[] dep, bool allDepartment)
        {
            List<LeaveSummary> summaries = new List<LeaveSummary>();
            var sum = new LeaveSummary();

            var LeaveSummaryone = new LeaveSummaryList();

            var placements = db.Placements.ToList();

            if (id != null)
            {
                int.TryParse(id, out int empId);
                placements = placements.Where(x => x.EmployeeId == empId).ToList();
            }

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }

            if (status == "Active")
            {
                placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements).OrderByDescending(x => x.AssignmentDate).ToList();
            }
            if (status == "InActive")
            {
                placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements).OrderByDescending(x => x.AssignmentDate).ToList();
            }

            if (!allDepartment)
            {
                if (dep.Any())
                {
                    List<int> orgIds = new List<int>();

                    foreach (var d in dep.ToList())
                    {
                        orgIds.Add(Convert.ToInt32(d.ToString()));
                    }

                    placements = placements.Where(x => orgIds.Contains(x.OrgStructureId)).ToList();
                }
            }

            var periods = db.GLFiscalYears.ToList();

            if (periodN != null)
            {
                int.TryParse(periodN, out int GLFiscalYearId);
                periods = periods.Where(x => x.GlFiscalYearId == GLFiscalYearId).ToList();
            }

            var placementsDistinicted = placements.DistinctByField(x => x.EmployeeId).ToList();

            foreach (var e in placementsDistinicted)
            {
                sum = new LeaveSummary();
                sum.LeaveSummaryLists = new List<LeaveSummaryList>();

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
                if (person != null)
                {
                    sum.ID = person.EmployeeCode;
                    sum.FullName = person.FullNameAmharic;
                }

                var activePeriods = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == e.EmployeeId).Select(x => x.GlFiscalYearId).ToList();

                var empActivePeriods = periods.Where(x => activePeriods.Contains(x.GlFiscalYearId)).ToList();

                foreach (var period in empActivePeriods)
                {
                    LeaveSummaryone = new LeaveSummaryList();

                    LeaveSummaryone.LeavePeriod = period.Name;

                    var leaveTaken = db.Leaves.Where(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId).ToList();
                    var paidIncash = db.LeavePaid.Where(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId).ToList();
                    var leaveEntitlments = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId);

                    LeaveSummaryone.TotalAmountTaken = leaveTaken.Sum(x => x.Amount);
                    LeaveSummaryone.PainInCash = Convert.ToDecimal(paidIncash.Sum(x => x.Amount));

                    if (leaveEntitlments != null)
                    {
                        LeaveSummaryone.Balance = leaveEntitlments.Balance;
                        LeaveSummaryone.Entitlment = leaveEntitlments.Amount;

                        LeaveSummaryone.PainInCash = e.Salary / 26 * leaveEntitlments.Balance;
                    }

                    sum.LeaveSummaryLists.Add(LeaveSummaryone);
                }
                if (!sum.LeaveSummaryLists.Any())
                {
                    LeaveSummaryone = new LeaveSummaryList();

                    LeaveSummaryone.LeavePeriod = " ";
                    LeaveSummaryone.TotalAmountTaken = 0;
                    LeaveSummaryone.PainInCash = 0;
                    LeaveSummaryone.Balance = 0;
                    LeaveSummaryone.Entitlment = 0;
                }

                if (e.isCurrent)
                {
                    var stracture = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == e.OrgStructureId);
                    if (stracture != null)
                    {
                        sum.Department = stracture.Name;
                    }
                }
                else
                {
                    var place = placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
                    if (place != null)
                    {
                        var stracture = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (stracture != null)
                        {
                            sum.Department = stracture.Name;
                        }
                    }
                }

                summaries.Add(sum);
            }

            return summaries;
        }
    }
}