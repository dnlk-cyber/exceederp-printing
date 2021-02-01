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
        public List<LeaveSummary> GetLeaveSummary(string periodN, string branchId, string orgStructureId, string id, string status, object[] dep)
        {
            List<LeaveSummary> summaries = new List<LeaveSummary>();
            LeaveSummary sum = new LeaveSummary();
            List<LeaveSummaryList> LeaveSummaryList = new List<LeaveSummaryList>();
            LeaveSummaryList LeaveSummaryone = new LeaveSummaryList();
            List<Person_Employee> employees;
            List<int> placements = new List<int>();
            if (branchId != null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetAllEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).ToList();
            }
            if (branchId != null /*&& orgStructureId == null*/)
            {
                employees = EmployeeHelper.GetAllEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetAllEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();
            }
            else
            {
                employees = db.Person_Employee.ToList();
            }
            if (status == "Active")
            {
                employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                employees = db.Person_Employee.Where(x => !x.IsActive).ToList();
            }
            if (dep.Any())
            {
                foreach (var d in dep.ToList())
                {
                    List<int> placement = new List<int>(); ;
                    if (status == "Active")
                    {
                        placement = db.Placements.Where(x => x.OrgStructureId.ToString() == d.ToString() && x.isCurrent).Select(x => x.EmployeeId).ToList();
                    }
                    else
                    {
                        placement = db.Placements.Where(x => x.OrgStructureId.ToString() == d.ToString()).Select(x => x.EmployeeId).ToList();
                    }
                    if (placement.Any())
                    {
                        placements.AddRange(placement);
                    }
                }

                employees = employees.Where(x => placements.Contains(x.EmployeeId)).ToList();

            }


            if (periodN != null)
            {
                int period = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == periodN).GlFiscalYearId;
                var entitlew = db.LeavesEntitle.Where(x => x.GlFiscalYearId == period).ToList();
                var taken = db.Leaves.Where(x => x.GlFiscalYearId == period).ToList();
                var paidIncash = db.LeavePaid.Where(x => x.GlFiscalYearId == period).ToList();


                //var tranferedList = db.LeaveTransfer.Where(x => x.To == period && x.Check).ToList();
                var periodName = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
                //var requestStatus = db.RequestesStatus.Where(x => x.ReferenceType == "LeaveTransfer").ToList();
                foreach (var e in employees)
                {
                    sum = new LeaveSummary();
                    sum.LeaveSummaryLists = new List<LeaveSummaryList>();
                    LeaveSummaryone = new LeaveSummaryList();
                    sum.ID = e.EmployeeCode;
                    sum.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                    if (periodName != null)
                    {
                        LeaveSummaryone.LeavePeriod = periodName.Name;

                        //LeaveSummaryone.TransferedFrom = Convert.ToDecimal(tranferedList.Where(x => x.EmployeeId == e.EmployeeId).Sum(a => a.Amount));

                        var firstOrDefault = entitlew.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
                        if (firstOrDefault != null)
                        {

                            LeaveSummaryone.Entitlment = firstOrDefault.Amount;

                            LeaveSummaryone.TotalAmountTaken = taken.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.Amount);
                            LeaveSummaryone.PainInCash = Convert.ToDecimal(paidIncash.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.Amount));
                            LeaveSummaryone.Balance = firstOrDefault.Balance;// (LeaveSummaryone.Entitlment + LeaveSummaryone.TransferedFrom) - LeaveSummaryone.TotalAmountTaken - LeaveSummaryone.PainInCash;

                        }
                        LeaveSummaryList.Add(LeaveSummaryone);
                        sum.LeaveSummaryLists.Add(LeaveSummaryone);

                        summaries.Add(sum);
                    }
                    Placement placement = null;
                    if(status == "Active")
                    {
                        placement = db.Placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);
                    }
                    else
                    {
                        var places = db.Placements.Where(x=>x.EmployeeId == e.EmployeeId).OrderByDescending(x=>x.PlacementId).ToList();
                        if(places != null)
                        {
                            placement = places.FirstOrDefault();
                        }
                    }
                    if (placement != null)
                    {
                        var stracture = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (stracture != null)
                        {
                            sum.Department = stracture.Name;
                        }

                    }
                }
            }
            else
            {
                var periods = db.GLFiscalYears.ToList();
                foreach (var e in employees)
                {
                    sum = new LeaveSummary();
                    sum.LeaveSummaryLists = new List<LeaveSummaryList>();
                    sum.ID = e.EmployeeCode;
                    sum.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;

                    Placement placement = null;
                    if (status == "Active")
                    {
                        placement = db.Placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);
                    }
                    else
                    {
                        var places = db.Placements.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.PlacementId).ToList();
                        if (places != null)
                        {
                            placement = places.FirstOrDefault();
                        }
                    }
                    if (placement != null)
                    {
                        var stracture = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (stracture != null)
                        {
                            sum.Department = stracture.Name;
                        }

                    }

                    var filterPeriods = db.LeavesEntitle.DistinctByField(x => x.GlFiscalYearId).Where(x => !x.IsExpired).ToList();
                    foreach (var filter in filterPeriods)
                    {
                        var period = periods.FirstOrDefault(x => x.GlFiscalYearId == filter.GlFiscalYearId);

                        //foreach (var period in periods)
                        //{

                        var entitlew = db.LeavesEntitle.Where(x => x.GlFiscalYearId == period.GlFiscalYearId).ToList();
                        var taken = db.Leaves.Where(x => x.GlFiscalYearId == period.GlFiscalYearId).ToList();
                        var paidIncash = db.LeavePaid.Where(x => x.GlFiscalYearId == period.GlFiscalYearId).ToList();


                        //var tranferedList = db.LeaveTransfer.Where(x => x.To == period.GlFiscalYearId && x.Check).ToList();
                        var periodName = db.GLFiscalYears.Where(x => x.GlFiscalYearId == period.GlFiscalYearId).FirstOrDefault();
                        //var requestStatus = db.RequestesStatus.Where(x => x.ReferenceType == "LeaveTransfer").ToList();

                        LeaveSummaryone = new LeaveSummaryList();

                        if (periodName != null)
                        {
                            LeaveSummaryone.LeavePeriod = periodName.Name;

                            //LeaveSummaryone.TransferedFrom = Convert.ToDecimal(db.LeavesEntitle.Where(x => x.EmployeeId == e.EmployeeId && !x.IsExpired).Sum(a => a.Amount));
                            var totalBalance = entitlew.Where(x => x.EmployeeId == e.EmployeeId && !x.IsExpired && x.GlFiscalYearId == period.GlFiscalYearId).Sum(x => x.Amount);
                            var firstOrDefault = entitlew.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId);
                            //if (firstOrDefault != null)


                            LeaveSummaryone.TotalAmountTaken = taken.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.Amount);
                            LeaveSummaryone.PainInCash = Convert.ToDecimal(paidIncash.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.Amount));
                            if (firstOrDefault != null)
                            {

                                LeaveSummaryone.Balance = firstOrDefault.Balance;// (totalBalance - LeaveSummaryone.TotalAmountTaken - LeaveSummaryone.PainInCash);
                                LeaveSummaryone.Entitlment = firstOrDefault.Amount; //LeaveSummaryone.Balance + LeaveSummaryone.TotalAmountTaken + LeaveSummaryone.PainInCash;
                            }
                            LeaveSummaryList.Add(LeaveSummaryone);
                            sum.LeaveSummaryLists.Add(LeaveSummaryone);
                        }
                        //}
                    }
                    summaries.Add(sum);
                }
            }


            return summaries;
        }
    }
}