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
                var socieRules = _db.IfrsStatementOfChangesInEquities.ToList();
                var yearBegBalance = new List<GLLedgerPostingLines>();
                var yearEndBalance = new List<GLLedgerPostingLines>();



                #region prev and current fiscal year ending balance

                decimal beginningPaidUpCapital = 0;
                decimal beginningLegalReserve = 0;
                decimal beginningRevaluationReserve = 0;
                decimal beginningOtherReserve = 0;
                decimal beginningRetainedEarnings = 0;
                decimal beginningTotal = 0;

                decimal endingPaidUpCapital = 0;
                decimal endingLegalReserve = 0;
                decimal endingRevaluationReserve = 0;
                decimal endingOtherReserve = 0;
                decimal endingRetainedEarnings = 0;
                decimal endingYearTotal = 0;

                #endregion
              

                #region prior period adjustment

                decimal priorPeriodAdjustmentRetainedEarnings = 0;
                decimal priorPeriodAdjustmentTotal = 0;

                #endregion

                #region restated balance of prev year

                decimal priorPeriodRestatedBalanceRetainedEarnings = 0;
                decimal priorPeriodRestatedBalanceTotal = 0;

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


                        }

                        #endregion
                        additionalInvestmentPaidUpCapital = endingPaidUpCapital - beginningPaidUpCapital;
                        additionalInvestmentTotal = additionalInvestmentPaidUpCapital;
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


                        }

                        #endregion
                        transferToLegalReserveLegalReserve = endingLegalReserve - beginningLegalReserve;
                        transferToLegalReserveRetainedEarnings = -transferToLegalReserveLegalReserve;//minus
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


                        }



                        #endregion
                        otherComprehensiveIncomeRevaluationReserve = endingRevaluationReserve - beginningRevaluationReserve;
                        totalComprehensiveIncomeRevaluationReserve = endingRevaluationReserve - beginningRevaluationReserve;
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


                        }

                        #endregion
                        otherComprehensiveIncomeOtherReserve = endingOtherReserve - beginningOtherReserve;
                        totalComprehensiveIncomeOtherReserve = endingOtherReserve - beginningOtherReserve;
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


                        }

                        #endregion

                    }

                    #endregion

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
                        PaidInCapital = beginningPaidUpCapital,
                        LegalReserve = beginningLegalReserve,
                        RevaluationReserve = beginningRevaluationReserve,
                        OtherReserve = beginningOtherReserve,
                        RetainedEarning = beginningRetainedEarnings,
                        Total = beginningTotal
                    };
                    list.Add(reportitem);
                    #endregion
                    #region adjustment
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Prior Period Adjustment ",
                        Notes = "",
                        PaidInCapital = 0,
                        LegalReserve = 0,
                        RevaluationReserve = 0,
                        OtherReserve = 0,
                        RetainedEarning = priorPeriodAdjustmentRetainedEarnings,
                        Total = priorPeriodAdjustmentTotal
                    };
                    list.Add(reportitem);
                    #endregion
                    #region restated
                    priorPeriodRestatedBalanceRetainedEarnings = beginningRetainedEarnings + priorPeriodAdjustmentRetainedEarnings;
                    priorPeriodRestatedBalanceTotal = beginningTotal + priorPeriodAdjustmentTotal;
                    reportitem = new SocieVeiwModel
                    {
                        CategoryDescription = $"Restated Balance at Beginning of the year {_db.GLPeriods.Find(begnningPeriod).DateFrom.ToShortDateString()}",
                        Notes = "",
                        PaidInCapital = 0,
                        LegalReserve = 0,
                        RevaluationReserve = 0,
                        OtherReserve = 0,
                        RetainedEarning = priorPeriodRestatedBalanceRetainedEarnings,
                        Total = priorPeriodRestatedBalanceTotal
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
                        Total = priorPeriodRestatedBalanceTotal
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
                        PaidInCapital = 0,
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
