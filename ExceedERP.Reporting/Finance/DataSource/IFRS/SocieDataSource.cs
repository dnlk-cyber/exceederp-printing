using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;
using ExceedERP.Reporting.Finance.ViewModel.IFRS;
using Type = System.Type;

namespace ExceedERP.Reporting.Finance.DataSource.IFRS
{
    [DataObject]
    public class SocieDataSource : List<SocieVeiwModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ReportHelper _reportHelper = new ReportHelper();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SocieVeiwModel> SocieReport(string fiscalyear, string fromPeriod,string toPeriod)
        {
            List<SocieVeiwModel> list = new List<SocieVeiwModel> ();


            if (!String.IsNullOrEmpty(fiscalyear))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));
                int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                var socieRules = _db.IfrsStatementOfChangesInEquities.AsNoTracking().ToList();
                var yearBegBalance = new List<GLLedgerPostingLines>();
                var yearEndBalance = new List<GLLedgerPostingLines>();
                var prevyearEndBalance = new List<GLLedgerPostingLines>();



                #region prev and current fiscal year ending balance

                decimal prevYearEndPaidUpCapital = 0;
                decimal prevYearEndLegalReserve = 0;
                decimal prevYearEndRevaluationReserve = 0;
                decimal prevYearEndOtherReserve = 0;
                decimal prevYearEndRetainedEarnings = 0;
                decimal prevYearEndDividends = 0;
                decimal prevYearEndTotal = 0;

                decimal beginningPaidUpCapital = 0;
                decimal beginningLegalReserve = 0;
                decimal beginningRevaluationReserve = 0;
                decimal beginningOtherReserve = 0;
                decimal beginningRetainedEarnings = 0;
                decimal beginningDividends = 0;
                decimal beginningTotal = 0;

                decimal endingPaidUpCapital = 0;
                decimal endingLegalReserve = 0;
                decimal endingRevaluationReserve = 0;
                decimal endingOtherReserve = 0;
                decimal endingRetainedEarnings = 0;
                decimal endingDividends = 0;
                decimal endingYearTotal = 0;

                #endregion
                #region prior period adjustment

                decimal priorPeriodAdjustmentPaidUpCapital = 0;
                decimal priorPeriodAdjustmentLegalReserve = 0;
                decimal priorPeriodAdjustmentRevaluationReserve = 0;
                decimal priorPeriodAdjustmentOtherReserve = 0;
                decimal priorPeriodAdjustmentDividends = 0;
                decimal priorPeriodAdjustmentRetainedEarnings = 0;
                decimal priorPeriodAdjustmentTotal = 0;

                #endregion
              
                #region profit of the year

                decimal profitOfTheYearRetainedEarnings = 0;

                #endregion

                #region other comprehensive income

                decimal otherComprehensiveIncomeRevaluationReserve = 0;
                decimal otherComprehensiveIncomeOtherReserve = 0;
                decimal otherComprehensiveIncomeTotal = 0;

                #endregion

                #region total comprehensive income

                decimal totalComprehensiveIncomeRevaluationReserve = 0;
                decimal totalComprehensiveIncomeOtherReserve = 0;
                decimal totalComprehensiveIncomeRetainedEarnings = 0;
                decimal totalComprehensiveIncomeTotal = 0;

                #endregion

                #region transfer to legal reserve

                decimal transferToLegalReserveLegalReserve = 0;
                decimal transferToLegalReserveRetainedEarnings = 0;

                #endregion

                #region additional investment

                decimal additionalInvestmentPaidUpCapital = 0;
                decimal additionalInvestmentTotal = 0;

                #endregion

                #region distributed profit

                decimal distributedProfitRetainedEarnings = 0;
                decimal distributedProfitTotal = 0;

                #endregion

                if (fyearObj != null && socieRules.Any())
                {
                    //new
                    int endPeriod = String.IsNullOrEmpty(toPeriod) ? _fiscalYearHelper.GetFiscalYearLastPeriod(fyearObj.GlFiscalYearId) : int.Parse(toPeriod);
                    int begnningPeriod = String.IsNullOrEmpty(fromPeriod) ? _fiscalYearHelper.GetFiscalYearFirstPeriod(fyearObj.GlFiscalYearId): int.Parse(fromPeriod);

                    yearBegBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, begnningPeriod);
                    yearEndBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, endPeriod);

                    int previousEndPeriod = 0;
                    if (prevfyearObj!=null)
                    {
                        previousEndPeriod =  _fiscalYearHelper.GetFiscalYearLastPeriod(prevfyearObj.GlFiscalYearId);
                        prevyearEndBalance = _reportHelper.GetAccountBalance(prevfyearObj.GlFiscalYearId, previousEndPeriod);
                    }

                   
                    #region get data
                    #region begining and ending year balance



                    #region paid up capital

                    var paidUpCapitals = socieRules.FirstOrDefault(x =>
                        x.Category == IfrsStatementOfChangesInEquityTypes.PaidInCapital);
                    if (paidUpCapitals != null)
                    {
                        #region  get balance

                        var accountRange = paidUpCapitals.IfrsStatementOfChangesInEquityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            beginningPaidUpCapital = beginningPaidUpCapital +
                                                 _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                            endingPaidUpCapital = endingPaidUpCapital +
                                                    _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo);

                            prevYearEndPaidUpCapital = prevYearEndPaidUpCapital +
                                                    _reportHelper.GetAccountEndBalancesWithAllranges(prevyearEndBalance, range.AccountFrom, range.AccountTo);
                        }

                        #endregion
                        additionalInvestmentPaidUpCapital = endingPaidUpCapital - beginningPaidUpCapital;
                        additionalInvestmentTotal = additionalInvestmentPaidUpCapital;

                        priorPeriodAdjustmentPaidUpCapital = beginningPaidUpCapital - prevYearEndPaidUpCapital;
                        
                    }

                    #endregion
                    #region legal reserve

                    var legalReserve = socieRules.FirstOrDefault(x =>
                        x.Category == IfrsStatementOfChangesInEquityTypes.LegalReserve);
                    if (legalReserve != null)
                    {
                        #region  get balance

                        var accountRange = legalReserve.IfrsStatementOfChangesInEquityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            beginningLegalReserve = beginningLegalReserve +
                                                    _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);
                            endingLegalReserve = endingLegalReserve +
                                                   _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo);

                            prevYearEndLegalReserve = prevYearEndLegalReserve +
                                                    _reportHelper.GetAccountEndBalancesWithAllranges(prevyearEndBalance, range.AccountFrom, range.AccountTo);
                        }

                        #endregion
                        transferToLegalReserveLegalReserve = endingLegalReserve - beginningLegalReserve;
                        transferToLegalReserveRetainedEarnings = -transferToLegalReserveLegalReserve;//minus

                        priorPeriodAdjustmentLegalReserve = beginningLegalReserve - prevYearEndLegalReserve;
                    }

                    #endregion
                    #region revaluation reserve

                    var revaluationReserve = socieRules.FirstOrDefault(x =>
                        x.Category == IfrsStatementOfChangesInEquityTypes.RevaluationReserve);
                    if (revaluationReserve != null)
                    {
                        #region  get balance

                        var accountRange = revaluationReserve.IfrsStatementOfChangesInEquityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            beginningRevaluationReserve = beginningRevaluationReserve +
                                                   _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                            endingRevaluationReserve = endingRevaluationReserve +
                                                         _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo);

                            prevYearEndRevaluationReserve = prevYearEndRevaluationReserve +
                                                    _reportHelper.GetAccountEndBalancesWithAllranges(prevyearEndBalance, range.AccountFrom, range.AccountTo);
                        }



                        #endregion
                        otherComprehensiveIncomeRevaluationReserve = endingRevaluationReserve - beginningRevaluationReserve;
                        totalComprehensiveIncomeRevaluationReserve = endingRevaluationReserve - beginningRevaluationReserve;

                        priorPeriodAdjustmentRevaluationReserve = beginningRevaluationReserve - prevYearEndRevaluationReserve;
                    }

                    #endregion|
                    #region other reserve

                    var otherReserve = socieRules.FirstOrDefault(x =>
                        x.Category == IfrsStatementOfChangesInEquityTypes.OtherReserve);
                    if (otherReserve != null)
                    {
                        #region  get balance

                        var accountRange = otherReserve.IfrsStatementOfChangesInEquityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            beginningOtherReserve = beginningOtherReserve +
                                                         _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                            endingOtherReserve = endingOtherReserve +
                                                   _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo);

                            prevYearEndOtherReserve = prevYearEndOtherReserve +
                                                    _reportHelper.GetAccountEndBalancesWithAllranges(prevyearEndBalance, range.AccountFrom, range.AccountTo);

                        }

                        #endregion
                        otherComprehensiveIncomeOtherReserve = endingOtherReserve - beginningOtherReserve;
                        totalComprehensiveIncomeOtherReserve = endingOtherReserve - beginningOtherReserve;

                        priorPeriodAdjustmentOtherReserve = beginningOtherReserve - prevYearEndOtherReserve;
                    }

                    #endregion
                    #region retained earning

                    var retainedEarning = socieRules.FirstOrDefault(x =>
                        x.Category == IfrsStatementOfChangesInEquityTypes.RetainedEarnings);
                    if (retainedEarning != null)
                    {
                        #region  get balance

                        var accountRange = retainedEarning.IfrsStatementOfChangesInEquityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            beginningRetainedEarnings = beginningRetainedEarnings +
                                                   _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                            endingRetainedEarnings = endingRetainedEarnings +
                                                       _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo);

                            prevYearEndRetainedEarnings = prevYearEndRetainedEarnings +
                                                    _reportHelper.GetAccountEndBalancesWithAllranges(prevyearEndBalance, range.AccountFrom, range.AccountTo);
                        }

                        #endregion

                        priorPeriodAdjustmentRetainedEarnings = beginningRetainedEarnings - prevYearEndRetainedEarnings;

                    }

                    #endregion
                    #region distributed dividends

                    var dividends = socieRules.FirstOrDefault(x =>
                        x.Category == IfrsStatementOfChangesInEquityTypes.Dividends);
                    if (dividends != null)
                    {
                        #region  get balance

                        var accountRange = dividends.IfrsStatementOfChangesInEquityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            beginningDividends = beginningDividends +
                                                   _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                            endingDividends = endingDividends +
                                                       _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo);


                        }

                        #endregion

                        distributedProfitRetainedEarnings=-(endingDividends-beginningDividends);
                        distributedProfitTotal = distributedProfitRetainedEarnings;
                    }

                    #endregion

                    #region profit of the year exclusive of other comprehensive 
                    #region continuing operation variables

                    decimal currentYearsumRevenue1 = 0;
                    decimal currentYearsumRevenue2 = 0;
                    decimal currentYearsumRevenue3 = 0;

                    decimal currentYearsumExpense1 = 0;
                    decimal currentYearsumExpense2 = 0;
                    decimal currentYearsumExpense3 = 0;

                    decimal currentNetProfit = 0;

                    #endregion
                    #region discontinued operation variables

                    decimal currentdiscontinuedSum = 0;

                    #endregion
                    var scoiRules = _db.IfrsStatementOfComprehensiveIncomes.AsNoTracking().FirstOrDefault();
                    if (scoiRules != null)
                    {
                        #region continuing operation

                        decimal currentYearrevenueTotal = 0;
                        decimal currentYearexpenseTotal = 0;


                        #region revenue 1

                        var revenue1 = scoiRules.ContinuingOperations
                            .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Revenue1).ToList();

                        foreach (var revenue in revenue1)
                        {
                            currentYearrevenueTotal = 0;

                            #region  account range

                            var accountRange = revenue.IfrsScoiContinuingOperationRanges.ToList();
                            foreach (var range in accountRange)
                            {
                                #region current year

                                currentYearrevenueTotal = currentYearrevenueTotal +
                                                          _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo)
                                                          - _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion


                            }

                            #endregion


                            currentYearsumRevenue1 = currentYearsumRevenue1 + currentYearrevenueTotal;
                            
                        }

                        #endregion

                        #region expense 1

                        var expense1 = scoiRules.ContinuingOperations
                            .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Expense1).ToList();
                        foreach (var expense in expense1)
                        {
                            currentYearexpenseTotal = 0;
                            #region  account range

                            var accountRange = expense.IfrsScoiContinuingOperationRanges.ToList();
                            foreach (var range in accountRange)
                            {
                                #region current year

                                currentYearexpenseTotal = currentYearexpenseTotal
                                                          + _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo)
                                                          - _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion



                            }

                            #endregion


                            currentYearsumExpense1 = currentYearsumExpense1 + currentYearexpenseTotal;
                        }

                        #endregion

                        var currentGrossProfit = currentYearsumRevenue1 - currentYearsumExpense1;

                        #region revenue 2

                        var revenue2 = scoiRules.ContinuingOperations
                            .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Revenue2).ToList();
                        foreach (var revenue in revenue2)
                        {
                            currentYearrevenueTotal = 0;
                            #region  account range

                            var accountRange = revenue.IfrsScoiContinuingOperationRanges.ToList();
                            foreach (var range in accountRange)
                            {
                                #region current year

                                currentYearrevenueTotal = currentYearrevenueTotal
                                                         + _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo)
                                                          - _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion


                            }

                            #endregion

                            currentYearsumRevenue2 = currentYearsumRevenue2 + currentYearrevenueTotal;
                        }

                        #endregion

                        #region expense 2

                        var expense2 = scoiRules.ContinuingOperations
                            .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Expense2).ToList();
                        foreach (var expense in expense2)
                        {
                            currentYearexpenseTotal = 0;

                            #region  account range

                            var accountRange = expense.IfrsScoiContinuingOperationRanges.ToList();
                            foreach (var range in accountRange)
                            {
                                #region current year

                                currentYearexpenseTotal = currentYearexpenseTotal
                                                        + _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo)
                                                        - _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion


                            }

                            #endregion


                            currentYearsumExpense2 = currentYearsumExpense2 + currentYearexpenseTotal;
                        }

                        #endregion

                        var currentOperatingProfit = currentGrossProfit + currentYearsumRevenue2 - currentYearsumExpense2;


                        #region revenue 3

                        var revenue3 = scoiRules.ContinuingOperations
                            .Where(x => x.Category == IfrsContinuedOpRevenueExpense.Revenue3).ToList();
                        foreach (var revenue in revenue3)
                        {
                            currentYearrevenueTotal = 0;
                            #region  account range

                            var accountRange = revenue.IfrsScoiContinuingOperationRanges.ToList();
                            foreach (var range in accountRange)
                            {
                                #region current year

                                currentYearrevenueTotal = currentYearrevenueTotal
                                                          + _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo)
                                                          - _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion


                            }

                            #endregion

                            currentYearsumRevenue3 = currentYearsumRevenue3 + currentYearrevenueTotal;
                        }

                        #endregion

                        #region expense 3

                        var expense3 =
                            scoiRules.ContinuingOperations.Where(x => x.Category == IfrsContinuedOpRevenueExpense.Expense3)
                                .ToList();
                        foreach (var expense in expense3)
                        {
                            currentYearexpenseTotal = 0;
                            #region  account range

                            var accountRange = expense.IfrsScoiContinuingOperationRanges.ToList();
                            foreach (var range in accountRange)
                            {
                                #region current year

                                currentYearexpenseTotal = currentYearexpenseTotal
                                                         + _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo)
                                                          - _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion


                            }

                            #endregion
                            currentYearsumExpense3 = currentYearsumExpense3 + currentYearexpenseTotal;

                        }

                        #endregion

                        var currentProfitBeforeTax =
                            currentOperatingProfit + currentYearsumRevenue3 - currentYearsumExpense3;


                        var profittaxRate = _db.FinanceSettings.FirstOrDefault();
                        decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;

                        decimal currentTax = currentProfitBeforeTax * rate;
                        currentNetProfit = currentProfitBeforeTax - currentTax;

                        #endregion
                        #region discontinued ops

                        #region discontinued

                        var discontinueds = scoiRules.DiscontinuedOperations.ToList();

                        foreach (var discontinued in discontinueds)
                        {
                            decimal currentYeardiscontinuedTotal = 0;
                            #region  account range

                            var accountRange = discontinued.IfrsScoiDiscontinuedOperationRanges.ToList();
                            foreach (var range in accountRange)
                            {
                                #region current year

                                currentYeardiscontinuedTotal = currentYeardiscontinuedTotal
                                    + _reportHelper.GetAccountEndBalancesWithAllranges(yearEndBalance, range.AccountFrom, range.AccountTo)
                                    - _reportHelper.GetAccountBegBalancesWithAllranges(yearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion


                            }

                            #endregion

                            currentdiscontinuedSum = currentdiscontinuedSum + currentYeardiscontinuedTotal;
                        }

                        #endregion

                        decimal currentNetProfitforTheyear = currentNetProfit - currentdiscontinuedSum;

                        profitOfTheYearRetainedEarnings = currentNetProfitforTheyear;

                        #endregion

                    }
                    #endregion

                    #region check beginning period
                    var begperiod = _db.GlBeginningPeriods.AsNoTracking().FirstOrDefault();
                    if (begperiod != null)
                    {
                        var begPer = _db.GLPeriods.Find(begperiod.GLPeriodId);
                        var lastYearEndPeriod = _db.GLPeriods.Find(previousEndPeriod);

                        if (begPer.DateFrom > lastYearEndPeriod.DateFrom)
                        {
                            priorPeriodAdjustmentPaidUpCapital = 0;
                            priorPeriodAdjustmentLegalReserve = 0;
                            priorPeriodAdjustmentRevaluationReserve = 0;
                            priorPeriodAdjustmentOtherReserve = 0;
                            priorPeriodAdjustmentRetainedEarnings=0;

                            prevYearEndPaidUpCapital = beginningPaidUpCapital;
                            prevYearEndLegalReserve = beginningLegalReserve;
                            prevYearEndRevaluationReserve = beginningRevaluationReserve;
                            prevYearEndOtherReserve = beginningOtherReserve;
                            prevYearEndRetainedEarnings= beginningRetainedEarnings;
                        }
                    }
                    #endregion

                    prevYearEndTotal = prevYearEndPaidUpCapital
                                   + prevYearEndLegalReserve
                                   + prevYearEndRevaluationReserve
                                   + prevYearEndOtherReserve
                                   + prevYearEndRetainedEarnings;

                    priorPeriodAdjustmentTotal = priorPeriodAdjustmentPaidUpCapital
                                   + priorPeriodAdjustmentLegalReserve
                                   + priorPeriodAdjustmentRevaluationReserve
                                   + priorPeriodAdjustmentOtherReserve
                                   + priorPeriodAdjustmentRetainedEarnings;

                    beginningTotal = beginningPaidUpCapital
                                   + beginningLegalReserve
                                   + beginningRevaluationReserve
                                   + beginningOtherReserve
                                   + beginningRetainedEarnings;
                    //display at end of lines
                    endingYearTotal = endingPaidUpCapital
                                       + endingLegalReserve
                                       + endingRevaluationReserve
                                       + endingOtherReserve
                                       + endingRetainedEarnings;

                    #endregion
                    #endregion

                   
                    #region display data
                    //balance  of prev year
                    #region beginning
                    var reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Balance at Beginning of the year {_db.GLPeriods.Find(begnningPeriod).DateFrom.ToShortDateString()}",
                        Notes = "",
                        PaidInCapital = prevYearEndPaidUpCapital,
                        LegalReserve = prevYearEndLegalReserve,
                        RevaluationReserve = prevYearEndRevaluationReserve,
                        OtherReserve = prevYearEndOtherReserve,
                        RetainedEarning = prevYearEndRetainedEarnings,
                        Total = prevYearEndTotal
                    };
                    list.Add(reportitem);
                    #endregion
                    #region adjustment
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Prior Period Adjustment ",
                        Notes = "",
                        PaidInCapital = priorPeriodAdjustmentPaidUpCapital,
                        LegalReserve = priorPeriodAdjustmentLegalReserve,
                        RevaluationReserve = priorPeriodAdjustmentRevaluationReserve,
                        OtherReserve = priorPeriodAdjustmentOtherReserve,
                        RetainedEarning = priorPeriodAdjustmentRetainedEarnings,
                        Total = priorPeriodAdjustmentTotal
                    };
                    list.Add(reportitem);
                    #endregion
                    #region restated
                    //priorPeriodRestatedBalanceRetainedEarnings = beginningRetainedEarnings + priorPeriodAdjustmentRetainedEarnings;
                    //priorPeriodRestatedBalanceTotal = beginningTotal + priorPeriodAdjustmentTotal;
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Restated Balance at Beginning of the year {_db.GLPeriods.Find(begnningPeriod).DateFrom.ToShortDateString()}",
                        Notes = "",
                        PaidInCapital = beginningPaidUpCapital,
                        LegalReserve = beginningLegalReserve,
                        RevaluationReserve = beginningRevaluationReserve,
                        OtherReserve = beginningOtherReserve,
                        RetainedEarning = beginningRetainedEarnings,
                        Total = beginningTotal
                    };
                    list.Add(reportitem);
                    #endregion
                    #region profit
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Profit for the year ",
                        Notes = "",
                        PaidInCapital = 0,
                        LegalReserve = 0,
                        RevaluationReserve = 0,
                        OtherReserve = 0,
                        RetainedEarning = profitOfTheYearRetainedEarnings,
                        Total = profitOfTheYearRetainedEarnings
                    };
                    list.Add(reportitem);
                    #endregion
                    #region other comprehensive
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Other Comprehensive Income for the year ",
                        Notes = "",
                        PaidInCapital = 0,
                        LegalReserve = 0,
                        RevaluationReserve = otherComprehensiveIncomeRevaluationReserve,
                        OtherReserve = otherComprehensiveIncomeOtherReserve,
                        RetainedEarning = 0,
                        Total = otherComprehensiveIncomeTotal
                    };
                    list.Add(reportitem);
                    #endregion
                    #region total comprehensive
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Total Comprehensive Income  ",
                        Notes = "",
                        PaidInCapital = 0,
                        LegalReserve = 0,
                        RevaluationReserve = totalComprehensiveIncomeRevaluationReserve,
                        OtherReserve = totalComprehensiveIncomeOtherReserve,
                        RetainedEarning = totalComprehensiveIncomeRetainedEarnings,
                        Total = totalComprehensiveIncomeTotal
                    };
                    list.Add(reportitem);
                    #endregion
                    #region transfer to legal
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Transfer to Legal Reserve ",
                        Notes = "",
                        PaidInCapital = 0,
                        LegalReserve = transferToLegalReserveLegalReserve,
                        RevaluationReserve = 0,
                        OtherReserve = 0,
                        RetainedEarning = transferToLegalReserveRetainedEarnings,
                        Total = 0
                    };
                    list.Add(reportitem);
                    #endregion
                    #region additional investment
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Additional Investment ",
                        Notes = "",
                        PaidInCapital = additionalInvestmentPaidUpCapital,
                        LegalReserve = 0,
                        RevaluationReserve = 0,
                        OtherReserve = 0,
                        RetainedEarning = 0,
                        Total = additionalInvestmentTotal
                    };
                    list.Add(reportitem);
                    #endregion
                    #region dividend
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Distributed/Dividends Profit",
                        Notes = "",
                        PaidInCapital = 0,
                        LegalReserve = 0,
                        RevaluationReserve = 0,
                        OtherReserve = 0,
                        RetainedEarning = distributedProfitRetainedEarnings,
                        Total = distributedProfitTotal
                    };
                    list.Add(reportitem);

                    
                    #endregion
                    #region ending
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Balance at End of the year {_db.GLPeriods.Find(begnningPeriod).DateTo.ToShortDateString()}",
                        Notes = "",
                        PaidInCapital = endingPaidUpCapital,
                        LegalReserve = endingLegalReserve,
                        RevaluationReserve = endingRevaluationReserve,
                        OtherReserve = endingOtherReserve,
                        RetainedEarning = endingRetainedEarnings,
                        Total = endingYearTotal
                    };
                    list.Add(reportitem);
                    #endregion
                    #endregion


                }
            }

            return list;
        }
    }
}
