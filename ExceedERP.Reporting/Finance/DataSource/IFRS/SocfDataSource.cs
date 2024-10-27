using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel.IFRS;

namespace ExceedERP.Reporting.Finance.DataSource.IFRS
{
    [DataObject]
    public class SocfDataSource : SocfReportVeiwModel
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ReportHelper _reportHelper = new ReportHelper();
        private readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
        private readonly ClosingHelper _closingHelper = new ClosingHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NoteVeiwModel> SocfReportNotes(string fiscalyear, string period)
        {
            List<NoteVeiwModel> list = new List<NoteVeiwModel>();
            var notes = _db.IfrsStatementOfCashFlows.FirstOrDefault();
            var operatingActivityRanges = _db.IfrsSocfOperatingActivityRanges.AsNoTracking().ToList();
            var investingActivityRanges = _db.IfrsSocfInvestingActivityRanges.AsNoTracking().ToList();
            var financingActivityRanges = _db.IfrsSocfFinancingActivityRanges.AsNoTracking().ToList();

            if (notes != null && !String.IsNullOrEmpty(fiscalyear))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));

                var currnetYearBegBalance = new List<GLLedgerPostingLines>();
                var currnetYearEndBalance = new List<GLLedgerPostingLines>();

                var prevYearBegBalance = new List<GLLedgerPostingLines>();
                var prevYearEndBalance = new List<GLLedgerPostingLines>();

                if (fyearObj != null)
                {
                    int currentEndPeriod = String.IsNullOrEmpty(period) ? 0 : int.Parse(period);
                    int currentBegPeriod =  _fiscalYearHelper.GetFiscalYearFirstPeriod(fyearObj.GlFiscalYearId);

                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                    currnetYearBegBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, currentBegPeriod);
                    currnetYearEndBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, currentEndPeriod);
                    if (prevfyearObj != null)
                    {
                        int previousEndPeriod = String.IsNullOrEmpty(period) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentEndPeriod, prevfyearObj.GlFiscalYearId);
                        int previousBegPeriod = _fiscalYearHelper.GetFiscalYearFirstPeriod(prevfyearObj.GlFiscalYearId);

                        prevYearBegBalance = _reportHelper.GetAccountBalance(prevfyearObj.GlFiscalYearId, previousBegPeriod);
                        prevYearEndBalance = _reportHelper.GetAccountBalance(prevfyearObj.GlFiscalYearId, previousEndPeriod);
                    }
                }


                #region operating
                foreach (var con in notes.OperatingActivities)
                {
                    var connote = new NoteVeiwModel
                    {
                        Description = con.Description,
                        Type = ViewModel.IFRS.Type.Operating,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<NoteDetailViewModel>()
                    };
                    //get range
                    var range = operatingActivityRanges
                        .Where(x => x.IfrsSocfOperatingActivityId == con.IfrsSocfOperatingActivityId).ToList();
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
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(prevYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(currnetYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                            };
                            connote.NoteDetailsList.Add(detail);
                        }
                    }

                    list.Add(connote);

                }
                #endregion
                #region investing
                foreach (var con in notes.InvestingActivities)
                {
                    var connote = new NoteVeiwModel
                    {
                        Description = con.Description,
                        Type = ViewModel.IFRS.Type.Investing,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<NoteDetailViewModel>()
                    };
                    //get range
                    var range = investingActivityRanges
                        .Where(x => x.IfrsSocfInvestingActivityId == con.IfrsSocfInvestingActivityId).ToList();
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
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(prevYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(currnetYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                            };
                            connote.NoteDetailsList.Add(detail);
                        }
                    }

                    list.Add(connote);

                }
                #endregion
                #region financing
                foreach (var con in notes.FinancingActivities)
                {
                    var connote = new NoteVeiwModel
                    {
                        Description = con.Description,
                        Type = ViewModel.IFRS.Type.Financing,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<NoteDetailViewModel>()
                    };
                    //get range
                    var range = financingActivityRanges
                        .Where(x => x.IfrsSocfFinancingActivityId == con.IfrsSocfFinancingActivityId).ToList();
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
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(prevYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(currnetYearEndBalance, r.AccountFrom, r.AccountTo, accRange.Values)
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
        public SocfReportVeiwModel SocfReport(string fiscalyear, string period)
        {
            SocfReportVeiwModel socfReport = new SocfReportVeiwModel();
            if (!String.IsNullOrEmpty(fiscalyear))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));

                var socfRules = _db.IfrsStatementOfCashFlows.AsNoTracking().FirstOrDefault();

                var currnetYearBegBalance = new List<GLLedgerPostingLines>();
                var currnetYearEndBalance = new List<GLLedgerPostingLines>();

                var prevYearBegBalance = new List<GLLedgerPostingLines>();
                var prevYearEndBalance = new List<GLLedgerPostingLines>();


                #region update earning  ledger--current year

                _closingHelper.UpdateEarningForAYear(fyearObj.GlFiscalYearId);

                #endregion

                #region operating Activities variables

                decimal currentProfitBeforeTax = 0;
                decimal previousProfitBeforeTax = 0;

                decimal currentYearsumOperatingRevenue = 0;

                decimal currentYearsumOperatingExpense = 0;

                decimal previosYearsumOperatingRevenue = 0;

                decimal previosYearsumOperatingExpense = 0;

                decimal currentYearSumAdjustment = 0;
                decimal previosYearSumAdjustment = 0;

                decimal currentYearSumIncreaseDecrease = 0;
                decimal previosYearSumIncreaseDecrease = 0;

                decimal currentCashGeneratedFromOperating = 0;
                decimal previousCashGeneratedFromOperating = 0;

                decimal currentYearSumNet = 0;
                decimal previosYearSumNet = 0;

                decimal currentNetCashOperating = 0;
                decimal previousNetCashOperating = 0;

                #endregion

                #region Investing activities variables

                decimal currentYearsumInvestingAdd = 0;

                decimal currentYearsumInvestingDeduct = 0;

                decimal previosYearsumInvestingAdd = 0;

                decimal previosYearsumInvestingDeduct = 0;

                decimal currentNetCashInvesting = 0;
                decimal previousNetCashInvesting = 0;

                #endregion

                #region financing variables

                decimal currentYearsumFinancingRevenue = 0;

                decimal currentYearsumFinancingExpense = 0;

                decimal previosYearsumFinancingRevenue = 0;

                decimal previosYearsumFinancingExpense = 0;

                decimal currentNetCashFinancing = 0;
                decimal previousNetCashFinancing = 0;

                #endregion

                #region net movement, beginning and ending variables              

                decimal currentNetMovement = 0;
                decimal previousNetMovement = 0;

                decimal currentBeginning = 0;
                decimal previousBeginning = 0;

                decimal currentEnding = 0;
                decimal previousEnding = 0;

                #endregion

                if (fyearObj != null && socfRules != null)
                {
                    int currentEndPeriod = String.IsNullOrEmpty(period) ? 0 : int.Parse(period);
                    int currentBegPeriod = _fiscalYearHelper.GetFiscalYearFirstPeriod(fyearObj.GlFiscalYearId);

                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                    currnetYearBegBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, currentBegPeriod);
                    currnetYearEndBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, currentEndPeriod);

                    int previousEndPeriod = 0;

                    if (prevfyearObj != null)
                    {
                        #region update earning  ledger--prev year

                        _closingHelper.UpdateEarningForAYear(prevfyearObj.GlFiscalYearId);

                        #endregion

                        previousEndPeriod = String.IsNullOrEmpty(period) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentEndPeriod, prevfyearObj.GlFiscalYearId);
                        int previousBegPeriod = _fiscalYearHelper.GetFiscalYearFirstPeriod(prevfyearObj.GlFiscalYearId);

                        prevYearBegBalance = _reportHelper.GetAccountBalance(prevfyearObj.GlFiscalYearId, previousBegPeriod);
                        prevYearEndBalance = _reportHelper.GetAccountBalance(prevfyearObj.GlFiscalYearId, previousEndPeriod);
                    }

                    #region operating activity

                    #region operating header

                    var reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Cash Flow From Operating Activities ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    socfReport.SocfReportItems.Add(reportitem);


                    #endregion

                    #region operating -profit before tax

                    //var profitBeforeTaxOperatings = socfRules.OperatingActivities
                    //    .Where(x => x.Category == IfrsCashFlowTypes.ProfitBeforeTax).ToList();


                    //foreach (var revenueOperating in profitBeforeTaxOperatings)
                    //{
                    //    decimal currentYearRevenueTotal = 0;
                    //    decimal prevYearRevenueTotal = 0;


                    //    #region  account range

                    //    var accountRange = revenueOperating.IfrsSocfOperatingActivityRanges.ToList();
                    //    foreach (var range in accountRange)
                    //    {

                    //        #region current year

                    //        currentYearRevenueTotal =
                    //            currentYearRevenueTotal +
                    //            _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);

                    //        #endregion

                    //        if (prevfyearObj != null)
                    //        {
                    //            #region previos year

                    //            prevYearRevenueTotal =
                    //                prevYearRevenueTotal +
                    //                _reportHelper.GetAccountEndBalancesWithAllranges(prevYearBalance, range.AccountFrom, range.AccountTo);

                    //            #endregion
                    //        }
                    //    }



                    //    #endregion


                    //    currentProfitBeforeTax = currentProfitBeforeTax + currentYearRevenueTotal;
                    //    previousProfitBeforeTax = previousProfitBeforeTax + prevYearRevenueTotal;


                    //}

                    #region current year earning

                    if (prevfyearObj != null)
                        previousProfitBeforeTax = _closingHelper.GetCurrentEarning(prevfyearObj.GlFiscalYearId, previousEndPeriod);
                    if (fyearObj != null)
                        currentProfitBeforeTax = _closingHelper.GetCurrentEarning(fyearObj.GlFiscalYearId, currentEndPeriod);
                   

                    #endregion

                    reportitem = new SocfReportItemVeiwModel
                    {
                        Notes = "", /*profitBeforeTaxOperatings.FirstOrDefault()?.NoteNumber*/
                        CategoryDescription = "Profit Before Taxation",
                        CurrentYearBalance = currentProfitBeforeTax,
                        PreviousYearBalance = previousProfitBeforeTax,
                        IsHeader = false
                    };
                    socfReport.SocfReportItems.Add(reportitem);
                    #endregion
                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Adjustment for:",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #region Adjustment -operating

                    var adjustmentOperatings = socfRules.OperatingActivities
                        .Where(x => x.Category == IfrsCashFlowTypes.Adjustment).ToList();

                   
                    foreach (var adjustment in adjustmentOperatings)
                    {
                        decimal currentYearAdjustmentTotal = 0;
                        decimal prevYearAdjustmentTotal = 0;                        

                        #region  account range

                        var accountRange = adjustment.IfrsSocfOperatingActivityRanges.ToList();
                        foreach (var range in accountRange)
                        {

                            #region current year

                            currentYearAdjustmentTotal =
                                currentYearAdjustmentTotal +
                                (_reportHelper.GetAccountEndBalancesWithAllranges(currnetYearEndBalance, range.AccountFrom, range.AccountTo)
                                - _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBegBalance, range.AccountFrom, range.AccountTo));

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearAdjustmentTotal =
                                    prevYearAdjustmentTotal +
                                    (_reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo));

                                #endregion
                            }
                        }



                        #endregion

                        if (adjustment.Rule == IfrsCashFlowRule.ADD)
                        {
                            currentYearSumAdjustment = currentYearSumAdjustment + currentYearAdjustmentTotal;
                            previosYearSumAdjustment = previosYearSumAdjustment + prevYearAdjustmentTotal;
                            
                            reportitem = new SocfReportItemVeiwModel
                            {
                                Notes = adjustment.NoteNumber,
                                CategoryDescription = adjustment.Description,
                                CurrentYearBalance = currentYearAdjustmentTotal,
                                PreviousYearBalance = prevYearAdjustmentTotal,
                                IsHeader = false
                            };
                            socfReport.SocfReportItems.Add(reportitem);
                        }
                        else if (adjustment.Rule == IfrsCashFlowRule.DEDUCT)
                        {
                            currentYearSumAdjustment = currentYearSumAdjustment - currentYearAdjustmentTotal;
                            previosYearSumAdjustment = previosYearSumAdjustment - prevYearAdjustmentTotal;

                            reportitem = new SocfReportItemVeiwModel
                            {
                                Notes = adjustment.NoteNumber,
                                CategoryDescription = adjustment.Description,
                                CurrentYearBalance = (-currentYearAdjustmentTotal),
                                PreviousYearBalance = (-prevYearAdjustmentTotal),
                                IsHeader = false
                            };
                            socfReport.SocfReportItems.Add(reportitem);
                        }

                       
                    }

                    #endregion

                    #region increase decrease operating

                    var increaseDecreases = socfRules.OperatingActivities
                        .Where(x => x.Category == IfrsCashFlowTypes.Increase_Decrease).ToList();
                    foreach (var increaseDecrease in increaseDecreases)
                    {
                        decimal currentYearIncDecTotal = 0;
                        decimal prevYearIncDecTotal = 0;


                        #region  account range

                        var accountRange = increaseDecrease.IfrsSocfOperatingActivityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearIncDecTotal =
                                currentYearIncDecTotal +
                                (_reportHelper.GetAccountEndBalancesWithAllranges(currnetYearEndBalance, range.AccountFrom, range.AccountTo)
                                - _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBegBalance, range.AccountFrom, range.AccountTo));

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearIncDecTotal =
                                    prevYearIncDecTotal +
                                    (_reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo));

                                #endregion
                            }

                        }

                        #endregion

                        if (increaseDecrease.Rule == IfrsCashFlowRule.ADD)
                        {
                            currentYearSumIncreaseDecrease = currentYearSumIncreaseDecrease + currentYearIncDecTotal;
                            previosYearSumIncreaseDecrease = previosYearSumIncreaseDecrease + prevYearIncDecTotal;

                            reportitem = new SocfReportItemVeiwModel
                            {
                                Notes = increaseDecrease.NoteNumber,
                                CategoryDescription = increaseDecrease.Description,
                                CurrentYearBalance = currentYearIncDecTotal,
                                PreviousYearBalance = prevYearIncDecTotal,
                                IsHeader = false
                            };
                            socfReport.SocfReportItems.Add(reportitem);
                        }
                        else if (increaseDecrease.Rule == IfrsCashFlowRule.DEDUCT)
                        {
                            currentYearSumIncreaseDecrease = currentYearSumIncreaseDecrease - currentYearIncDecTotal;
                            previosYearSumIncreaseDecrease = previosYearSumIncreaseDecrease - prevYearIncDecTotal;

                            reportitem = new SocfReportItemVeiwModel
                            {
                                Notes = increaseDecrease.NoteNumber,
                                CategoryDescription = increaseDecrease.Description,
                                CurrentYearBalance = (-currentYearIncDecTotal),
                                PreviousYearBalance = (-prevYearIncDecTotal),
                                IsHeader = false
                            };
                            socfReport.SocfReportItems.Add(reportitem);
                        }

                        //if (increaseDecrease.Rule == IfrsCashFlowRule.ADD_INCREASE)
                        //{
                        //    if (currentYearIncDecTotal > prevYearIncDecTotal)
                        //    {
                        //        currentYearIncDecTotal = currentYearIncDecTotal - prevYearIncDecTotal;
                        //    }
                        //    else
                        //    {
                        //        currentYearIncDecTotal = currentYearIncDecTotal - prevYearIncDecTotal;
                        //    }

                        //    if (prevYearIncDecTotal > prevYearTwoIncDecTotal)
                        //    {
                        //        prevYearIncDecTotal = prevYearIncDecTotal - prevYearTwoIncDecTotal;
                        //    }
                        //    else
                        //    {
                        //        prevYearIncDecTotal = -prevYearIncDecTotal - prevYearTwoIncDecTotal;
                        //    }
                        //}

                        //else if (increaseDecrease.Rule == IfrsCashFlowRule.ADD_DECREASE)
                        //{
                        //    if (currentYearIncDecTotal < prevYearIncDecTotal)
                        //    {
                        //        currentYearIncDecTotal = -(currentYearIncDecTotal - prevYearIncDecTotal);
                        //    }
                        //    else
                        //    {
                        //        currentYearIncDecTotal = -(currentYearIncDecTotal - prevYearIncDecTotal);
                        //    }

                        //    if (prevYearIncDecTotal < prevYearTwoIncDecTotal)
                        //    {
                        //        prevYearIncDecTotal = -(prevYearIncDecTotal - prevYearTwoIncDecTotal);
                        //    }
                        //    else
                        //    {
                        //        prevYearIncDecTotal = -(prevYearIncDecTotal - prevYearTwoIncDecTotal);
                        //    }
                        //}

                        //else if (increaseDecrease.Rule == IfrsCashFlowRule.DEDUCT_INCREASE)
                        //{
                        //    if (currentYearIncDecTotal > prevYearIncDecTotal)
                        //    {

                        //        currentYearIncDecTotal = -(currentYearIncDecTotal - prevYearIncDecTotal);
                        //    }
                        //    else
                        //    {
                        //        currentYearIncDecTotal = currentYearIncDecTotal - prevYearIncDecTotal;
                        //    }

                        //    if (prevYearIncDecTotal > prevYearTwoIncDecTotal)
                        //    {
                        //        prevYearIncDecTotal = -(prevYearIncDecTotal - prevYearTwoIncDecTotal);
                        //    }
                        //    else
                        //    {
                        //        prevYearIncDecTotal = prevYearIncDecTotal - prevYearTwoIncDecTotal;
                        //    }
                        //}

                        //else if (increaseDecrease.Rule == IfrsCashFlowRule.DEDUCT_DECREASE)
                        //{
                        //    if (currentYearIncDecTotal < prevYearIncDecTotal)
                        //    {

                        //        currentYearIncDecTotal = -(currentYearIncDecTotal - prevYearIncDecTotal);
                        //    }
                        //    else
                        //    {
                        //        currentYearIncDecTotal = currentYearIncDecTotal - prevYearIncDecTotal;
                        //    }

                        //    if (prevYearIncDecTotal < prevYearTwoIncDecTotal)
                        //    {
                        //        prevYearIncDecTotal = -(prevYearIncDecTotal - prevYearTwoIncDecTotal);
                        //    }
                        //    else
                        //    {
                        //        prevYearIncDecTotal = prevYearIncDecTotal - prevYearTwoIncDecTotal;
                        //    }
                        //}

                        
                    }

                    #endregion

                    #region cash generated from operating activities

                    currentCashGeneratedFromOperating = currentProfitBeforeTax + currentYearSumAdjustment+currentYearSumIncreaseDecrease;
                    previousCashGeneratedFromOperating = previousProfitBeforeTax + previosYearSumAdjustment+previosYearSumIncreaseDecrease;
                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Cash Generated from Operating Activities",
                        CurrentYearBalance = currentCashGeneratedFromOperating,
                        PreviousYearBalance = previousCashGeneratedFromOperating,
                        IsHeader = false
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #endregion

                    #region Net -operating

                    var netOperatings = socfRules.OperatingActivities
                        .Where(x => x.Category == IfrsCashFlowTypes.Net).ToList();


                    foreach (var netOp in netOperatings)
                    {
                        decimal currentYearNetTotal = 0;
                        decimal prevYearNetTotal = 0;

                        #region  account range

                        var accountRange = netOp.IfrsSocfOperatingActivityRanges.ToList();
                        foreach (var range in accountRange)
                        {

                            #region current year

                            currentYearNetTotal =
                                currentYearNetTotal +
                                (_reportHelper.GetAccountEndBalancesWithAllranges(currnetYearEndBalance, range.AccountFrom, range.AccountTo)-
                                _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBegBalance, range.AccountFrom, range.AccountTo));

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearNetTotal =
                                    prevYearNetTotal +
                                    (_reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo));

                                #endregion
                            }
                        }



                        #endregion

                        if (netOp.Rule == IfrsCashFlowRule.ADD)
                        {
                            currentYearSumNet = currentYearSumNet + currentYearNetTotal;
                            previosYearSumNet = previosYearSumNet + prevYearNetTotal;

                            reportitem = new SocfReportItemVeiwModel
                            {
                                Notes = netOp.NoteNumber,
                                CategoryDescription = netOp.Description,
                                CurrentYearBalance = currentYearNetTotal,
                                PreviousYearBalance = prevYearNetTotal,
                                IsHeader = false
                            };
                            socfReport.SocfReportItems.Add(reportitem);
                        }
                        else if (netOp.Rule == IfrsCashFlowRule.DEDUCT)
                        {
                            currentYearSumNet = currentYearSumNet - currentYearNetTotal;
                            previosYearSumNet = previosYearSumNet - prevYearNetTotal;

                            reportitem = new SocfReportItemVeiwModel
                            {
                                Notes = netOp.NoteNumber,
                                CategoryDescription = netOp.Description,
                                CurrentYearBalance = (-currentYearNetTotal),
                                PreviousYearBalance = (-prevYearNetTotal),
                                IsHeader = false
                            };
                            socfReport.SocfReportItems.Add(reportitem);
                        }

                       
                    }

                    #endregion

                    #region net operating

                    currentNetCashOperating = currentCashGeneratedFromOperating + currentYearSumNet;
                    previousNetCashOperating = previousCashGeneratedFromOperating + previosYearSumNet;

                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Net cash from operating activities",
                        CurrentYearBalance = currentNetCashOperating,
                        PreviousYearBalance = previousNetCashOperating,
                        IsHeader = true
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region investing activity

                    #region investing header

                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Cash Flow From Investing Activities ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #endregion

                    #region investing-ADD

                    var addInvestings = socfRules.InvestingActivities
                        .Where(x => x.Rule ==IfrsCashFlowRule.ADD).ToList();

                    foreach (var addInvesting in addInvestings)
                    {
                        decimal currentYearAddTotal = 0;
                        decimal prevYearAddTotal = 0;                        

                        #region  account range

                        var accountRange = addInvesting.IfrsSocfInvestingActivityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearAddTotal =
                                currentYearAddTotal +
                                (_reportHelper.GetAccountEndBalancesWithAllranges(currnetYearEndBalance, range.AccountFrom, range.AccountTo)
                                - _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBegBalance, range.AccountFrom, range.AccountTo));

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearAddTotal =
                                    prevYearAddTotal +
                                    (_reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo));

                                #endregion
                            }
                        }

                        #endregion
                        
                        currentYearsumInvestingAdd = currentYearsumInvestingAdd + currentYearAddTotal;
                        previosYearsumInvestingAdd = previosYearsumInvestingAdd + prevYearAddTotal;

                        reportitem = new SocfReportItemVeiwModel
                        {
                            Notes = addInvesting.NoteNumber,
                            CategoryDescription = addInvesting.Description,
                            CurrentYearBalance = currentYearAddTotal,
                            PreviousYearBalance = prevYearAddTotal,
                            IsHeader = false
                        };
                        socfReport.SocfReportItems.Add(reportitem);
                    }

                    #endregion

                    #region investing -DEDUCT

                    var deductInvestings = socfRules.InvestingActivities
                        .Where(x => x.Rule == IfrsCashFlowRule.DEDUCT).ToList();
                    foreach (var deductInvesting in deductInvestings)
                    {
                        decimal currentYearDeductTotal = 0;
                        decimal prevYearDeductTotal = 0;                        

                        #region  account range

                        var accountRange = deductInvesting.IfrsSocfInvestingActivityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearDeductTotal =
                                currentYearDeductTotal +
                                (_reportHelper.GetAccountEndBalancesWithAllranges(currnetYearEndBalance, range.AccountFrom, range.AccountTo)
                                - _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBegBalance, range.AccountFrom, range.AccountTo));

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearDeductTotal =
                                    prevYearDeductTotal +
                                    (_reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo));

                                #endregion
                            }
                        }

                        #endregion
                       
                        currentYearsumInvestingDeduct = currentYearsumInvestingDeduct - currentYearDeductTotal;
                        previosYearsumInvestingDeduct = previosYearsumInvestingDeduct - prevYearDeductTotal;

                        reportitem = new SocfReportItemVeiwModel
                        {
                            Notes = deductInvesting.NoteNumber,
                            CategoryDescription = deductInvesting.Description,
                            CurrentYearBalance = (-currentYearDeductTotal),
                            PreviousYearBalance = (-prevYearDeductTotal),
                            IsHeader = false
                        };
                        socfReport.SocfReportItems.Add(reportitem);
                    }

                    #endregion

                    #region net investing

                    currentNetCashInvesting = currentYearsumInvestingAdd - currentYearsumInvestingDeduct;
                    previousNetCashInvesting = previosYearsumInvestingAdd - previosYearsumInvestingDeduct;
                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Net Cash from Investing Activities",
                        CurrentYearBalance = currentNetCashInvesting,
                        PreviousYearBalance = previousNetCashInvesting,
                        IsHeader = true
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region financing activity;

                    #region financing header

                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Cash Flow From Financing Activities ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #endregion

                    #region financing ADD

                    var addFinancings = socfRules.FinancingActivities
                        .Where(x => x.Rule == IfrsCashFlowRule.ADD).ToList();

                    foreach (var addFinancing in addFinancings)
                    {
                        decimal currentYearAddTotal = 0;
                        decimal prevYearAddTotal = 0;                       

                        #region  account range

                        var accountRange = addFinancing.IfrsSocfFinancingActivityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearAddTotal =
                                currentYearAddTotal +
                                (_reportHelper.GetAccountEndBalancesWithAllranges(currnetYearEndBalance, range.AccountFrom, range.AccountTo)
                                - _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBegBalance, range.AccountFrom, range.AccountTo));

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearAddTotal =
                                    prevYearAddTotal +
                                    (_reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo));

                                #endregion
                            }
                        }

                        #endregion

                        
                        currentYearsumFinancingRevenue = currentYearsumFinancingRevenue + currentYearAddTotal;
                        previosYearsumFinancingRevenue = previosYearsumFinancingRevenue + prevYearAddTotal;

                        reportitem = new SocfReportItemVeiwModel
                        {
                            Notes = addFinancing.NoteNumber,
                            CategoryDescription = addFinancing.Description,
                            CurrentYearBalance = currentYearAddTotal,
                            PreviousYearBalance = prevYearAddTotal,
                            IsHeader = false
                        };
                        socfReport.SocfReportItems.Add(reportitem);
                    }

                    #endregion

                    #region financing DEDUCT

                    var deductFinancings = socfRules.FinancingActivities
                        .Where(x => x.Rule == IfrsCashFlowRule.DEDUCT).ToList();
                    foreach (var deductFinancing in deductFinancings)
                    {
                        decimal currentYearDeductTotal = 0;
                        decimal prevYearDeductTotal = 0;                       

                        #region  account range

                        var accountRange = deductFinancing.IfrsSocfFinancingActivityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearDeductTotal =
                                currentYearDeductTotal +
                                (_reportHelper.GetAccountEndBalancesWithAllranges(currnetYearEndBalance, range.AccountFrom, range.AccountTo)
                                - _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBegBalance, range.AccountFrom, range.AccountTo));

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearDeductTotal =
                                    prevYearDeductTotal +
                                    (_reportHelper.GetAccountEndBalancesWithAllranges(prevYearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo));

                                #endregion
                            }
                        }

                        #endregion

                        
                        currentYearsumFinancingExpense = currentYearsumFinancingExpense -currentYearDeductTotal;
                        previosYearsumFinancingExpense = previosYearsumFinancingExpense - prevYearDeductTotal;

                        reportitem = new SocfReportItemVeiwModel
                        {
                            Notes = deductFinancing.NoteNumber,
                            CategoryDescription = deductFinancing.Description,
                            CurrentYearBalance = (-currentYearDeductTotal),
                            PreviousYearBalance = (-prevYearDeductTotal),
                            IsHeader = false
                        };
                        socfReport.SocfReportItems.Add(reportitem);
                    }

                    #endregion

                    #region net financing

                    currentNetCashFinancing = currentYearsumFinancingRevenue - currentYearsumFinancingExpense;
                    previousNetCashFinancing = previosYearsumFinancingRevenue - previosYearsumFinancingExpense;
                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Net Cash from Financing Activities",
                        CurrentYearBalance = currentNetCashFinancing,
                        PreviousYearBalance = previousNetCashFinancing,
                        IsHeader = true
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region net movement

                    currentNetMovement = currentNetCashOperating + currentNetCashInvesting + currentNetCashFinancing;
                    previousNetMovement = previousNetCashOperating + previousNetCashInvesting + previousNetCashFinancing;

                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Movement in Cash and Cash Equivalents ",
                        CurrentYearBalance = currentNetMovement,                            
                        PreviousYearBalance = previousNetMovement,
                        IsHeader = false
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #endregion

                    #region beginning

                    #region  cash and euivalent account range

                    var cashAccountRanges = socfRules.IfrsCashAndCashEquivalentRanges.ToList();
                    foreach (var range in cashAccountRanges)
                    {
                        #region current year

                        currentBeginning =
                            currentBeginning +
                            _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBegBalance, range.AccountFrom, range.AccountTo);

                        #endregion

                        if (prevfyearObj != null)
                        {
                            #region previos year

                            previousBeginning =
                                previousBeginning +
                                _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion
                        }
                    }

                    #endregion

                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Cash and Cash Equivalents at the beginning of the year ",
                        CurrentYearBalance = currentBeginning,
                        PreviousYearBalance = previousBeginning,
                        IsHeader = false
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #endregion
                    #region ending

                    currentEnding = currentBeginning + currentNetMovement;
                    previousEnding = previousBeginning + previousNetMovement;
                    reportitem = new SocfReportItemVeiwModel
                    {
                        CategoryDescription = "Cash and Cash Equivalents at the end of the year",
                        CurrentYearBalance =currentEnding,
                        PreviousYearBalance =previousEnding,
                        IsHeader = false
                    };
                    socfReport.SocfReportItems.Add(reportitem);

                    #endregion

                }
            }

            return socfReport;

        }
    }
}
