using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel.IFRS;
using Type = ExceedERP.Reporting.Finance.ViewModel.IFRS.Type;

namespace ExceedERP.Reporting.Finance.DataSource.IFRS
{
    [DataObject]
    public class ScoiByProjectOds : ScoiReportVeiwModel
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ReportHelper _reportHelper = new ReportHelper();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NoteVeiwModel> ScoiReportNotes(string fiscalyear, string periodFrom, string periodTo, string project, string segmentType, string segmentValue, string costcenter, string subaccount)
        {
            List<NoteVeiwModel> list = new List<NoteVeiwModel>();
            var notes = _db.IfrsStatementOfComprehensiveIncomes.AsNoTracking().FirstOrDefault();
            var continuingOperationRanges = _db.IfrsScoiContinuingOperationRanges.AsNoTracking().ToList();
            var discontinuedOperationRanges = _db.IfrsScoiDiscontinuedOperationRanges.ToList();
            var otherComprehensiveIncomeRanges = _db.IfrsScoiOtherComprehensiveIncomeRanges.AsNoTracking().ToList();

            if (notes != null && !String.IsNullOrEmpty(fiscalyear))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));
                var prevYearEndBalance = new List<GLLedgerPostingLines>();
                var prevYearBegBalance = new List<GLLedgerPostingLines>();
                var currentYearEndBalance = new List<GLLedgerPostingLines>();
                var currentYearBegBalance = new List<GLLedgerPostingLines>();


                if (fyearObj != null)
                {
                    int currentPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : int.Parse(periodTo);
                    int currentPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : int.Parse(periodFrom);
                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                    currentYearEndBalance = _reportHelper.GetAccountEndBalance(fyearObj.GlFiscalYearId, currentPeriodTo);
                    currentYearBegBalance = _reportHelper.GetAccountBegBalance(fyearObj.GlFiscalYearId, currentPeriodFrom);
                    if (prevfyearObj != null)
                    {
                        int previousPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodTo, prevfyearObj.GlFiscalYearId);
                        int previousPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodFrom, prevfyearObj.GlFiscalYearId);
                        prevYearEndBalance = _reportHelper.GetAccountEndBalance(prevfyearObj.GlFiscalYearId, previousPeriodTo);
                        prevYearBegBalance = _reportHelper.GetAccountBegBalance(prevfyearObj.GlFiscalYearId, previousPeriodFrom);
                    }
                }


                #region filter by segment

                if (!String.IsNullOrEmpty(project))
                {
                    currentYearEndBalance = currentYearEndBalance
                          .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();
                    prevYearEndBalance = prevYearEndBalance
                        .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();

                    currentYearBegBalance = currentYearBegBalance
                        .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();
                    prevYearBegBalance = prevYearBegBalance
                        .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();
                }

                if (!String.IsNullOrEmpty(costcenter))
                {
                    currentYearEndBalance = currentYearEndBalance
                          .Where(x => x.GLLedgerPosting.CostCenterSegment == costcenter).ToList();
                    prevYearEndBalance = prevYearEndBalance
                        .Where(x => x.GLLedgerPosting.CostCenterSegment == costcenter).ToList();

                    currentYearBegBalance = currentYearBegBalance
                        .Where(x => x.GLLedgerPosting.CostCenterSegment == costcenter).ToList();
                    prevYearBegBalance = prevYearBegBalance
                        .Where(x => x.GLLedgerPosting.CostCenterSegment == costcenter).ToList();
                }

                if (!String.IsNullOrEmpty(subaccount))
                {
                    currentYearEndBalance = currentYearEndBalance
                          .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount).ToList();
                    prevYearEndBalance = prevYearEndBalance
                        .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount).ToList();

                    currentYearBegBalance = currentYearBegBalance
                        .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount).ToList();
                    prevYearBegBalance = prevYearBegBalance
                        .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount).ToList();
                }

                if (!String.IsNullOrEmpty(segmentType) && !String.IsNullOrEmpty(segmentValue))
                {
                    
                    var setup = _db.GlSegmentSetups.FirstOrDefault(x => x.Name == segmentType);
                    int index = setup?.Sequence ?? 0;
                    switch (index)
                    {
                        case 1:                              

                            currentYearEndBalance = currentYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment1 == segmentValue).ToList();
                            prevYearEndBalance = prevYearEndBalance
                                .Where(x => x.GLLedgerPosting.Segment1 == segmentValue).ToList();

                            currentYearBegBalance = currentYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment1 == segmentValue).ToList();
                            prevYearBegBalance = prevYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment1 == segmentValue).ToList();
                            break;

                        case 2:
                            currentYearEndBalance = currentYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment2 == segmentValue).ToList();
                            prevYearEndBalance = prevYearEndBalance
                                .Where(x => x.GLLedgerPosting.Segment2 == segmentValue).ToList();

                            currentYearBegBalance = currentYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment2 == segmentValue).ToList();
                            prevYearBegBalance = prevYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment2 == segmentValue).ToList();
                            break;

                        case 3:
                            currentYearEndBalance = currentYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment3 == segmentValue).ToList();
                            prevYearEndBalance = prevYearEndBalance
                                .Where(x => x.GLLedgerPosting.Segment3 == segmentValue).ToList();

                            currentYearBegBalance = currentYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment3 == segmentValue).ToList();
                            prevYearBegBalance = prevYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment3 == segmentValue).ToList();
                            break;

                        case 4:
                            currentYearEndBalance = currentYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment4 == segmentValue).ToList();
                            prevYearEndBalance = prevYearEndBalance
                                .Where(x => x.GLLedgerPosting.Segment4 == segmentValue).ToList();

                            currentYearBegBalance = currentYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment4 == segmentValue).ToList();
                            prevYearBegBalance = prevYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment4 == segmentValue).ToList();
                            break;

                        case 5:
                            currentYearEndBalance = currentYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment5 == segmentValue).ToList();
                            prevYearEndBalance = prevYearEndBalance
                                .Where(x => x.GLLedgerPosting.Segment5 == segmentValue).ToList();

                            currentYearBegBalance = currentYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment5 == segmentValue).ToList();
                            prevYearBegBalance = prevYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment5 == segmentValue).ToList();
                            break;

                        case 6:
                            currentYearEndBalance = currentYearEndBalance
                                .Where(x => x.GLLedgerPosting.Segment6 == segmentValue).ToList();
                            prevYearEndBalance = prevYearEndBalance
                                .Where(x => x.GLLedgerPosting.Segment6 == segmentValue).ToList();

                            currentYearBegBalance = currentYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment6 == segmentValue).ToList();
                            prevYearBegBalance = prevYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment6 == segmentValue).ToList();
                            break;
                        case 7:
                            currentYearEndBalance = currentYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment7 == segmentValue).ToList();
                            prevYearEndBalance = prevYearEndBalance
                                .Where(x => x.GLLedgerPosting.Segment7 == segmentValue).ToList();

                            currentYearBegBalance = currentYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment7 == segmentValue).ToList();
                            prevYearBegBalance = prevYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment7 == segmentValue).ToList();
                            break;
                        case 8:
                            currentYearEndBalance = currentYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment8 == segmentValue).ToList();
                            prevYearEndBalance = prevYearEndBalance
                                .Where(x => x.GLLedgerPosting.Segment8 == segmentValue).ToList();

                            currentYearBegBalance = currentYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment8 == segmentValue).ToList();
                            prevYearBegBalance = prevYearBegBalance
                                .Where(x => x.GLLedgerPosting.Segment8 == segmentValue).ToList();
                            break;
                    }
                   
                }
                #endregion

                #region continuinng
                foreach (var con in notes.ContinuingOperations)
                {
                    var connote = new NoteVeiwModel
                    {
                        Description = con.Description,
                        Type = Type.Continuing,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<NoteDetailViewModel>()
                    };
                    //get range
                    var range = continuingOperationRanges
                        .Where(x => x.IfrsScoiContinuingOperationId == con.IfrsScoiContinuingOperationId).ToList();
                    foreach (var r in range)
                    {
                        string acctFrom = _accountHelper.GetNaturalAccountSegment(r.AccountFrom);
                        string acctTo = _accountHelper.GetNaturalAccountSegment(r.AccountTo);
                        var accountRange = _db.GLChartOfAccountAccounts
                            .Where(x => String.Compare(x.Values.ToUpper(), acctFrom.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), acctTo.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        foreach (var accRange in accountRange)
                        {
                            var detail = new NoteDetailViewModel
                            {
                                AccountDesc = accRange.Description,
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(prevYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                                                      - _reportHelper.GetAccountBegBalancesByAccountWithAllranges(prevYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(currentYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                                                     - _reportHelper.GetAccountBegBalancesByAccountWithAllranges(currentYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                            };
                            connote.NoteDetailsList.Add(detail);
                        }
                    }

                    list.Add(connote);

                }
                #endregion
                #region discontinued
                foreach (var con in notes.DiscontinuedOperations)
                {
                    var connote = new NoteVeiwModel
                    {
                        Description = con.Description,
                        Type = Type.Continuing,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<NoteDetailViewModel>()
                    };
                    //get range
                    var range = discontinuedOperationRanges
                        .Where(x => x.IfrsScoiDiscontinuedOperationId == con.IfrsScoiDiscontinuedOperationId).ToList();
                    foreach (var r in range)
                    {
                        string acctFrom = _accountHelper.GetNaturalAccountSegment(r.AccountFrom);
                        string acctTo = _accountHelper.GetNaturalAccountSegment(r.AccountTo);
                        var accountRange = _db.GLChartOfAccountAccounts
                            .Where(x => String.Compare(x.Values.ToUpper(), acctFrom, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), acctTo, StringComparison.Ordinal) <= 0)
                            .ToList();
                        foreach (var accRange in accountRange)
                        {
                            var detail = new NoteDetailViewModel
                            {
                                AccountDesc = accRange.Description,
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(prevYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                                                      - _reportHelper.GetAccountBegBalancesByAccountWithAllranges(prevYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(currentYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                                                     - _reportHelper.GetAccountBegBalancesByAccountWithAllranges(currentYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                            };
                            connote.NoteDetailsList.Add(detail);
                        }
                    }

                    list.Add(connote);

                }
                #endregion
                #region discontinued
                foreach (var con in notes.OtherComprehensiveIncomes)
                {
                    var connote = new NoteVeiwModel
                    {
                        Description = con.Description,
                        Type = Type.Continuing,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<NoteDetailViewModel>()
                    };
                    //get range
                    var range = otherComprehensiveIncomeRanges
                        .Where(x => x.IfrsScoiOtherComprehensiveIncomeId == con.IfrsScoiOtherComprehensiveIncomeId).ToList();
                    foreach (var r in range)
                    {
                        string acctFrom = _accountHelper.GetNaturalAccountSegment(r.AccountFrom);
                        string acctTo = _accountHelper.GetNaturalAccountSegment(r.AccountTo);
                        var accountRange = _db.GLChartOfAccountAccounts
                            .Where(x => String.Compare(x.Values.ToUpper(), acctFrom, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), acctTo, StringComparison.Ordinal) <= 0)
                            .ToList();
                        foreach (var accRange in accountRange)
                        {
                            var detail = new NoteDetailViewModel
                            {
                                AccountDesc = accRange.Description,
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(prevYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                                                      - _reportHelper.GetAccountBegBalancesByAccountWithAllranges(prevYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(currentYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                                                     - _reportHelper.GetAccountBegBalancesByAccountWithAllranges(currentYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                            };
                            connote.NoteDetailsList.Add(detail);
                        }
                    }

                    list.Add(connote);

                }
                #endregion
            }

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ScoiReportVeiwModel ScoiByProjectReport(string fiscalyear,string periodFrom, string periodTo, string project, string segmentType, string segmentValue, string costcenter, string subaccount)
        {
            ScoiReportVeiwModel scoiReport = new ScoiReportVeiwModel();            

            var scoiRules = _db.IfrsStatementOfComprehensiveIncomes.AsNoTracking().FirstOrDefault();                

            if (scoiRules != null && !String.IsNullOrEmpty(fiscalyear))
            {
                #region continuing operation variables

                decimal currentYearsumRevenue1 = 0;
                decimal currentYearsumRevenue2 = 0;
                decimal currentYearsumRevenue3 = 0;

                decimal currentYearsumExpense1 = 0;
                decimal currentYearsumExpense2 = 0;
                decimal currentYearsumExpense3 = 0;

                decimal previosYearsumRevenue1 = 0;
                decimal previosYearsumRevenue2 = 0;
                decimal previosYearsumRevenue3 = 0;

                decimal previosYearsumExpense1 = 0;
                decimal previosYearsumExpense2 = 0;
                decimal previosYearsumExpense3 = 0;

                decimal currentNetProfit = 0;
                decimal previousNetProfit = 0;

                #endregion

                #region discontinued operation variables

                decimal currentdiscontinuedSum = 0;
                decimal prevediscontinuedSum = 0;

                #endregion

                #region other income variables

                decimal currentOtherIncomeSum = 0;
                decimal preveOtherIncomeSum = 0;

                decimal currentTotalComprehensiveIncomeOfTheYear = 0;
                decimal preveTotalComprehensiveIncomeOfTheYear = 0;

                #endregion

                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));
               

                if (fyearObj != null)
                {
                    var prevYearEndBalance = new List<GLLedgerPostingLines>();
                    var prevYearBegBalance = new List<GLLedgerPostingLines>();
                    var currentYearEndBalance = new List<GLLedgerPostingLines>();
                    var currentYearBegBalance = new List<GLLedgerPostingLines>();

                    int currentPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : int.Parse(periodTo);
                    int currentPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : int.Parse(periodFrom);
                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                    currentYearEndBalance = _reportHelper.GetAccountEndBalance(fyearObj.GlFiscalYearId, currentPeriodTo);
                    currentYearBegBalance = _reportHelper.GetAccountBegBalance(fyearObj.GlFiscalYearId, currentPeriodFrom);
                    if (prevfyearObj != null)
                    {
                        int previousPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodTo, prevfyearObj.GlFiscalYearId);
                        int previousPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodFrom, prevfyearObj.GlFiscalYearId);
                        prevYearEndBalance = _reportHelper.GetAccountEndBalance(prevfyearObj.GlFiscalYearId, previousPeriodTo);
                        prevYearBegBalance = _reportHelper.GetAccountBegBalance(prevfyearObj.GlFiscalYearId, previousPeriodFrom);
                    }


                    #region filter by segment
                    if (!String.IsNullOrEmpty(project))
                    {
                        currentYearEndBalance = currentYearEndBalance
                                      .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();
                        prevYearEndBalance = prevYearEndBalance
                            .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();

                        currentYearBegBalance = currentYearBegBalance
                            .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();
                        prevYearBegBalance = prevYearBegBalance
                            .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();
                    }
                    if (!String.IsNullOrEmpty(costcenter))
                    {
                        currentYearEndBalance = currentYearEndBalance
                                      .Where(x => x.GLLedgerPosting.CostCenterSegment == costcenter).ToList();
                        prevYearEndBalance = prevYearEndBalance
                            .Where(x => x.GLLedgerPosting.CostCenterSegment == costcenter).ToList();

                        currentYearBegBalance = currentYearBegBalance
                            .Where(x => x.GLLedgerPosting.CostCenterSegment == costcenter).ToList();
                        prevYearBegBalance = prevYearBegBalance
                            .Where(x => x.GLLedgerPosting.CostCenterSegment == costcenter).ToList();
                    }
                    if (!String.IsNullOrEmpty(subaccount))
                    {
                        currentYearEndBalance = currentYearEndBalance
                                      .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount).ToList();
                        prevYearEndBalance = prevYearEndBalance
                            .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount).ToList();

                        currentYearBegBalance = currentYearBegBalance
                            .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount).ToList();
                        prevYearBegBalance = prevYearBegBalance
                            .Where(x => x.GLLedgerPosting.SubAccountSegment == subaccount).ToList();
                    }

                    if (!String.IsNullOrEmpty(segmentType) && !String.IsNullOrEmpty(segmentValue))
                    {

                        var setup = _db.GlSegmentSetups.FirstOrDefault(x => x.Name == segmentType);
                        int index = setup?.Sequence ?? 0;
                        switch (index)
                        {
                            case 1:

                                currentYearEndBalance = currentYearEndBalance
                                        .Where(x => x.GLLedgerPosting.Segment1 == segmentValue).ToList();
                                prevYearEndBalance = prevYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment1 == segmentValue).ToList();

                                currentYearBegBalance = currentYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment1 == segmentValue).ToList();
                                prevYearBegBalance = prevYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment1 == segmentValue).ToList();
                                break;

                            case 2:
                                currentYearEndBalance = currentYearEndBalance
                                        .Where(x => x.GLLedgerPosting.Segment2 == segmentValue).ToList();
                                prevYearEndBalance = prevYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment2 == segmentValue).ToList();

                                currentYearBegBalance = currentYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment2 == segmentValue).ToList();
                                prevYearBegBalance = prevYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment2 == segmentValue).ToList();
                                break;

                            case 3:
                                currentYearEndBalance = currentYearEndBalance
                                        .Where(x => x.GLLedgerPosting.Segment3 == segmentValue).ToList();
                                prevYearEndBalance = prevYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment3 == segmentValue).ToList();

                                currentYearBegBalance = currentYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment3 == segmentValue).ToList();
                                prevYearBegBalance = prevYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment3 == segmentValue).ToList();
                                break;

                            case 4:
                                currentYearEndBalance = currentYearEndBalance
                                        .Where(x => x.GLLedgerPosting.Segment4 == segmentValue).ToList();
                                prevYearEndBalance = prevYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment4 == segmentValue).ToList();

                                currentYearBegBalance = currentYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment4 == segmentValue).ToList();
                                prevYearBegBalance = prevYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment4 == segmentValue).ToList();
                                break;

                            case 5:
                                currentYearEndBalance = currentYearEndBalance
                                        .Where(x => x.GLLedgerPosting.Segment5 == segmentValue).ToList();
                                prevYearEndBalance = prevYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment5 == segmentValue).ToList();

                                currentYearBegBalance = currentYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment5 == segmentValue).ToList();
                                prevYearBegBalance = prevYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment5 == segmentValue).ToList();
                                break;

                            case 6:
                                currentYearEndBalance = currentYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment6 == segmentValue).ToList();
                                prevYearEndBalance = prevYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment6 == segmentValue).ToList();

                                currentYearBegBalance = currentYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment6 == segmentValue).ToList();
                                prevYearBegBalance = prevYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment6 == segmentValue).ToList();
                                break;
                            case 7:
                                currentYearEndBalance = currentYearEndBalance
                                        .Where(x => x.GLLedgerPosting.Segment7 == segmentValue).ToList();
                                prevYearEndBalance = prevYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment7 == segmentValue).ToList();

                                currentYearBegBalance = currentYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment7 == segmentValue).ToList();
                                prevYearBegBalance = prevYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment7 == segmentValue).ToList();
                                break;
                            case 8:
                                currentYearEndBalance = currentYearEndBalance
                                        .Where(x => x.GLLedgerPosting.Segment8 == segmentValue).ToList();
                                prevYearEndBalance = prevYearEndBalance
                                    .Where(x => x.GLLedgerPosting.Segment8 == segmentValue).ToList();

                                currentYearBegBalance = currentYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment8 == segmentValue).ToList();
                                prevYearBegBalance = prevYearBegBalance
                                    .Where(x => x.GLLedgerPosting.Segment8 == segmentValue).ToList();
                                break;
                        }

                    }
                    #endregion


                    #region continuing operation

                    decimal currentYearrevenueTotal = 0;
                    decimal prevYearrevenueTotal = 0;

                    decimal currentYearexpenseTotal = 0;
                    decimal prevYearexpenseTotal = 0;
                    var reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Continuing Operations",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    #region revenue 1

                    var revenue1 = scoiRules.ContinuingOperations
                        .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Revenue1).ToList();

                    foreach (var revenue in revenue1)
                    {
                        currentYearrevenueTotal = 0;
                        prevYearrevenueTotal = 0;

                        #region  account range

                        var accountRange = revenue.IfrsScoiContinuingOperationRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearrevenueTotal = currentYearrevenueTotal +
                                                      _reportHelper.GetAccountEndBalancesWithAllranges(currentYearEndBalance, range.AccountFrom, range.AccountTo)
                                                      - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearrevenueTotal = prevYearrevenueTotal
                                    + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                     - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion
                            }
                        }

                        #endregion


                        currentYearsumRevenue1 = currentYearsumRevenue1 + currentYearrevenueTotal;
                        previosYearsumRevenue1 = previosYearsumRevenue1 + prevYearrevenueTotal;

                        reportitem = new ScoiReportItemVeiwModel
                        {
                            Notes = revenue.NoteNumber,
                            CategoryDescription = revenue.Description,
                            CurrentYearBalance = currentYearrevenueTotal,
                            PreviousYearBalance = prevYearrevenueTotal,
                            IsHeader = false
                        };
                        scoiReport.ScoiReportItems.Add(reportitem);
                    }

                    #endregion

                    #region expense 1

                    var expense1 = scoiRules.ContinuingOperations
                        .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Expense1).ToList();
                    foreach (var expense in expense1)
                    {
                        currentYearexpenseTotal = 0;
                        prevYearexpenseTotal = 0;
                        #region  account range

                        var accountRange = expense.IfrsScoiContinuingOperationRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearexpenseTotal = currentYearexpenseTotal
                                                      + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearEndBalance, range.AccountFrom, range.AccountTo)
                                                      - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearexpenseTotal = prevYearexpenseTotal
                                    + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion
                            }

                        }

                        #endregion


                        currentYearsumExpense1 = currentYearsumExpense1 + currentYearexpenseTotal;
                        previosYearsumExpense1 = previosYearsumExpense1 + prevYearexpenseTotal;

                        reportitem = new ScoiReportItemVeiwModel
                        {
                            Notes = expense.NoteNumber,
                            CategoryDescription = expense.Description,
                            CurrentYearBalance = currentYearexpenseTotal,
                            PreviousYearBalance = prevYearexpenseTotal,
                            IsHeader = false
                        };
                        scoiReport.ScoiReportItems.Add(reportitem);
                    }

                    #endregion

                    var currentGrossProfit = currentYearsumRevenue1 - currentYearsumExpense1;
                    var previousGrossProfit = previosYearsumRevenue1 - previosYearsumExpense1;
                    reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Gross Profit",
                        CurrentYearBalance = currentGrossProfit,
                        PreviousYearBalance = previousGrossProfit,
                        IsHeader = true
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    #region revenue 2

                    var revenue2 = scoiRules.ContinuingOperations
                        .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Revenue2).ToList();
                    foreach (var revenue in revenue2)
                    {
                        currentYearrevenueTotal = 0;
                        prevYearrevenueTotal = 0;
                        #region  account range

                        var accountRange = revenue.IfrsScoiContinuingOperationRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearrevenueTotal = currentYearrevenueTotal
                                                     + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearEndBalance, range.AccountFrom, range.AccountTo)
                                                      - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearrevenueTotal = prevYearrevenueTotal
                                    + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion
                            }
                        }

                        #endregion

                        currentYearsumRevenue2 = currentYearsumRevenue2 + currentYearrevenueTotal;
                        previosYearsumRevenue2 = previosYearsumRevenue2 + prevYearrevenueTotal;

                        reportitem = new ScoiReportItemVeiwModel
                        {
                            Notes = revenue.NoteNumber,
                            CategoryDescription = revenue.Description,
                            CurrentYearBalance = currentYearrevenueTotal,
                            PreviousYearBalance = prevYearrevenueTotal,
                            IsHeader = false
                        };
                        scoiReport.ScoiReportItems.Add(reportitem);
                    }

                    #endregion

                    #region expense 2

                    var expense2 = scoiRules.ContinuingOperations
                        .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Expense2).ToList();
                    foreach (var expense in expense2)
                    {
                        currentYearexpenseTotal = 0;
                        prevYearexpenseTotal = 0;

                        #region  account range

                        var accountRange = expense.IfrsScoiContinuingOperationRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearexpenseTotal = currentYearexpenseTotal
                                                    + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearEndBalance, range.AccountFrom, range.AccountTo)
                                                    - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearexpenseTotal = prevYearexpenseTotal
                                    + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion
                            }
                        }

                        #endregion


                        currentYearsumExpense2 = currentYearsumExpense2 + currentYearexpenseTotal;
                        previosYearsumExpense2 = previosYearsumExpense2 + prevYearexpenseTotal;

                        reportitem = new ScoiReportItemVeiwModel
                        {
                            Notes = expense.NoteNumber,
                            CategoryDescription = expense.Description,
                            CurrentYearBalance = currentYearexpenseTotal,
                            PreviousYearBalance = prevYearexpenseTotal,
                            IsHeader = false
                        };
                        scoiReport.ScoiReportItems.Add(reportitem);
                    }

                    #endregion

                    var currentOperatingProfit = currentGrossProfit + currentYearsumRevenue2 - currentYearsumExpense2;
                    var previousOperatingProfit = previousGrossProfit + previosYearsumRevenue2 - previosYearsumExpense2;
                    reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Operating Profit/(Loss)",
                        CurrentYearBalance = currentOperatingProfit,
                        PreviousYearBalance = previousOperatingProfit,
                        IsHeader = true
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    #region revenue 3

                    var revenue3 = scoiRules.ContinuingOperations
                        .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Revenue3).ToList();
                    foreach (var revenue in revenue3)
                    {
                        currentYearrevenueTotal = 0;
                        prevYearrevenueTotal = 0;
                        #region  account range

                        var accountRange = revenue.IfrsScoiContinuingOperationRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearrevenueTotal = currentYearrevenueTotal
                                                      + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearEndBalance, range.AccountFrom, range.AccountTo)
                                                      - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearrevenueTotal = prevYearrevenueTotal
                                    + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion
                            }
                        }

                        #endregion

                        currentYearsumRevenue3 = currentYearsumRevenue3 + currentYearrevenueTotal;
                        previosYearsumRevenue3 = previosYearsumRevenue3 + prevYearrevenueTotal;

                        reportitem = new ScoiReportItemVeiwModel
                        {
                            Notes = revenue.NoteNumber,
                            CategoryDescription = revenue.Description,
                            CurrentYearBalance = currentYearrevenueTotal,
                            PreviousYearBalance = prevYearrevenueTotal,
                            IsHeader = false
                        };
                        scoiReport.ScoiReportItems.Add(reportitem);
                    }

                    #endregion

                    #region expense 3

                    var expense3 =
                        scoiRules.ContinuingOperations.Where(x => x.Category == IfrsContinuedOpRevenueExpense.Expense3)
                            .ToList();
                    foreach (var expense in expense3)
                    {
                        currentYearexpenseTotal = 0;
                        prevYearexpenseTotal = 0;
                        #region  account range

                        var accountRange = expense.IfrsScoiContinuingOperationRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearexpenseTotal = currentYearexpenseTotal
                                                     + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearEndBalance, range.AccountFrom, range.AccountTo)
                                                      - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearexpenseTotal = prevYearexpenseTotal
                                    + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo); ;

                                #endregion
                            }
                        }

                        #endregion
                        currentYearsumExpense3 = currentYearsumExpense3 + currentYearexpenseTotal;
                        previosYearsumExpense3 = previosYearsumExpense3 + prevYearexpenseTotal;

                        reportitem = new ScoiReportItemVeiwModel
                        {
                            Notes = expense.NoteNumber,
                            CategoryDescription = expense.Description,
                            CurrentYearBalance = currentYearexpenseTotal,
                            PreviousYearBalance = prevYearexpenseTotal,
                            IsHeader = false
                        };
                        scoiReport.ScoiReportItems.Add(reportitem);
                    }

                    #endregion

                    var currentProfitBeforeTax =
                        currentOperatingProfit + currentYearsumRevenue3 - currentYearsumExpense3;
                    var previousProfitBeforeTax =
                        previousOperatingProfit + previosYearsumRevenue3 - previosYearsumExpense3;

                    reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Profit/(Loss) Before Tax",
                        CurrentYearBalance = currentProfitBeforeTax,
                        PreviousYearBalance = previousProfitBeforeTax,
                        IsHeader = true
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    var profittaxRate = _db.FinanceSettings.FirstOrDefault();
                    decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;

                    decimal currentTax = currentProfitBeforeTax * rate;
                    decimal previousTax = previousProfitBeforeTax * rate;
                    currentNetProfit = currentProfitBeforeTax - currentTax;
                    previousNetProfit = previousProfitBeforeTax - previousTax;

                    reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Taxation",
                        CurrentYearBalance = currentTax,
                        PreviousYearBalance = previousTax,
                        IsHeader = false
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Net Profit/(Loss) from Continuing Operations",
                        CurrentYearBalance = (currentNetProfit),
                        PreviousYearBalance = (previousNetProfit),
                        IsHeader = false
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    #endregion

                    #region discontinued ops

                    reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Discontinued Operations",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    #region discontinued

                    var discontinueds = scoiRules.DiscontinuedOperations.ToList();

                    foreach (var discontinued in discontinueds)
                    {
                        decimal currentYeardiscontinuedTotal = 0;
                        decimal prevYeardiscontinuedTotal = 0;
                        #region  account range

                        var accountRange = discontinued.IfrsScoiDiscontinuedOperationRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYeardiscontinuedTotal = currentYeardiscontinuedTotal
                                + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearEndBalance, range.AccountFrom, range.AccountTo)
                                - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYeardiscontinuedTotal = prevYeardiscontinuedTotal
                                    + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo); ;

                                #endregion
                            }
                        }

                        #endregion

                        currentdiscontinuedSum = currentdiscontinuedSum + currentYeardiscontinuedTotal;
                        prevediscontinuedSum = prevediscontinuedSum + prevYeardiscontinuedTotal;

                        reportitem = new ScoiReportItemVeiwModel
                        {
                            Notes = discontinued.NoteNumber,
                            CategoryDescription = discontinued.Description,
                            CurrentYearBalance = currentYeardiscontinuedTotal,
                            PreviousYearBalance = prevYeardiscontinuedTotal,
                            IsHeader = false
                        };
                        scoiReport.ScoiReportItems.Add(reportitem);
                    }

                    #endregion

                    var currentNetProfitforTheyear = currentNetProfit - currentdiscontinuedSum;
                    var previousNetProfitforTheyear = previousNetProfit - prevediscontinuedSum;
                    reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Net Profit/(loss) for the year ",
                        CurrentYearBalance = currentNetProfitforTheyear,
                        PreviousYearBalance = previousNetProfitforTheyear,
                        IsHeader = true
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    #endregion

                    #region other comprehensive income

                    reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Other Comprehensive income ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    #region other comprehensive

                    var othercomprehensives = scoiRules.OtherComprehensiveIncomes.ToList();

                    foreach (var othercomprehensive in othercomprehensives)
                    {
                        decimal currentYearOthercomprehensiveTotal = 0;
                        decimal prevYearOthercomprehensiveTotal = 0;
                        #region  account range

                        var accountRange = othercomprehensive.IfrsScoiOtherComprehensiveIncomeRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearOthercomprehensiveTotal = currentYearOthercomprehensiveTotal
                                        + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearEndBalance, range.AccountFrom, range.AccountTo)
                                        - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearOthercomprehensiveTotal = prevYearOthercomprehensiveTotal
                                    + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion
                            }
                        }

                        #endregion

                        currentOtherIncomeSum = currentOtherIncomeSum + currentYearOthercomprehensiveTotal;
                        preveOtherIncomeSum = preveOtherIncomeSum + prevYearOthercomprehensiveTotal;

                        reportitem = new ScoiReportItemVeiwModel
                        {
                            Notes = othercomprehensive.NoteNumber,
                            CategoryDescription = othercomprehensive.Description,
                            CurrentYearBalance = currentYearOthercomprehensiveTotal,
                            PreviousYearBalance = prevYearOthercomprehensiveTotal,
                            IsHeader = false
                        };
                        scoiReport.ScoiReportItems.Add(reportitem);
                    }

                    #endregion

                    currentTotalComprehensiveIncomeOfTheYear = currentNetProfitforTheyear + currentOtherIncomeSum;
                    preveTotalComprehensiveIncomeOfTheYear = previousNetProfitforTheyear + preveOtherIncomeSum;
                    reportitem = new ScoiReportItemVeiwModel
                    {
                        CategoryDescription = "Total Comprehensive income/(loss) for the year",
                        CurrentYearBalance = currentTotalComprehensiveIncomeOfTheYear,
                        PreviousYearBalance = preveTotalComprehensiveIncomeOfTheYear,
                        IsHeader = true
                    };
                    scoiReport.ScoiReportItems.Add(reportitem);

                    #endregion
                }




                return scoiReport;
            }
            

            return scoiReport;
        }
        
    }
}
