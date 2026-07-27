using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;
using ExceedERP.Reporting.Finance.ViewModel.GAAP;
using ExceedERP.Reporting.Finance.ViewModel.IFRS;

namespace ExceedERP.Reporting.Finance.DataSource.GAAP
{
    
        [DataObject]
        public class CashFlowProjectionObjectDataSource
        {
            readonly ExceedDbContext _db = new ExceedDbContext();
            private readonly ReportHelper _reportHelper = new ReportHelper();
            readonly AccountHelper _accountHelper = new AccountHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
            public List<CashFlowProjectionViewModel> GetCashFlowProjection(string fiscalYear)
            {
                List<CashFlowProjectionViewModel> cashFlowList = new List<CashFlowProjectionViewModel>();
                var cashFlow = _db.GaapCashFlows.AsNoTracking().FirstOrDefault();
              
               

            if (cashFlow != null && !String.IsNullOrEmpty(fiscalYear))
                {
                    int  fYearId = int.Parse(fiscalYear);
                    var fyearObj = _db.GLFiscalYears.Find(fYearId);
                    var glPeriods = _db.GLPeriods.AsNoTracking().Where(x => x.GlFiscalYearId== fYearId)
                        .OrderBy(x => x.Index).ToList();

                    CashFlowProjectionViewModel cashflowRecord =new CashFlowProjectionViewModel
                        {
                            BegEndLines = new BindingList<CashFlowProjectionLineVm>(),
                            Receipts = new BindingList<CashFlowProjectionLineVm>(),
                            PayOuts = new BindingList<CashFlowProjectionLineVm>(),
                            TotalCashAvailable= new BindingList<CashFlowProjectionLineVm>(),
                    };

                    if (glPeriods.Any())
                    {

                        cashflowRecord.Month1 = glPeriods[0]?.Name;
                        cashflowRecord.Month2 = glPeriods[1]?.Name;
                        cashflowRecord.Month3 = glPeriods[2]?.Name;
                        cashflowRecord.Month4 = glPeriods[3]?.Name;
                        cashflowRecord.Month5 = glPeriods[4]?.Name;
                        cashflowRecord.Month6 = glPeriods[5]?.Name;
                        cashflowRecord.Month7 = glPeriods[6]?.Name;
                        cashflowRecord.Month8 = glPeriods[7]?.Name;
                        cashflowRecord.Month9 = glPeriods[8]?.Name;
                        cashflowRecord.Month10 = glPeriods[9]?.Name;
                        cashflowRecord.Month11 = glPeriods[10]?.Name;
                        cashflowRecord.Month12 = glPeriods[11]?.Name;


                    #region receipt
                    foreach (var receipt in cashFlow.Receipts)
                    {
                       
                        #region  account range

                        var accountRange = receipt.GaapCashFlowReceiptRanges.ToList();
                        
                        #endregion

                        if (fyearObj != null)
                        {
                            var reportitem = new CashFlowProjectionLineVm
                            {
                                Description = receipt.Description
                            };
                          
                            #region period 1

                            int periodid= glPeriods.FirstOrDefault(x => x.Index == 1).GLPeriodId;
                            List<GLLedgerPostingLines> currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            decimal receiptBegTotal = 0;
                            decimal receiptStandardTotal = 0;
                            decimal receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal- receiptBegTotal;
                            reportitem.Beginning = receiptBegTotal;

                            reportitem.MonthOneValue = receiptStandardTotal;
                            reportitem.MonthOneEndValue = receiptEndTotal;
                            #endregion
                            #region period 2

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 2).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthTwoValue = receiptStandardTotal;
                            reportitem.MonthTwoEndValue = receiptEndTotal;
                            #endregion
                            #region period 3

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 3).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthThreeValue = receiptStandardTotal;
                            reportitem.MonthThreeEndValue = receiptEndTotal;
                            #endregion
                            #region period 4

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 4).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthFourValue = receiptStandardTotal;
                            reportitem.MonthFiveEndValue = receiptEndTotal;
                            #endregion
                            #region period 5

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 5).GLPeriodId;
                             currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }

                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthFiveValue = receiptStandardTotal;
                            reportitem.MonthFiveEndValue = receiptEndTotal;
                            #endregion
                            #region period 6

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 6).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthSixValue = receiptStandardTotal;
                            reportitem.MonthSixEndValue = receiptEndTotal;
                            #endregion
                            #region period 7

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 7).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthSevenValue = receiptStandardTotal;
                            reportitem.MonthSevenEndValue = receiptEndTotal;
                            #endregion
                            #region period 8

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 8).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthEightValue = receiptStandardTotal;
                            reportitem.MonthEightEndValue = receiptEndTotal;
                            #endregion
                            #region period 9

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 9).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthNineValue = receiptStandardTotal;
                            reportitem.MonthNineEndValue = receiptEndTotal;
                            #endregion
                            #region period 10

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 10).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthTenValue = receiptStandardTotal;
                            reportitem.MonthTenEndValue = receiptEndTotal;
                            #endregion
                            #region period 11

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 11).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthElevenValue = receiptStandardTotal;
                            reportitem.MonthElevenEndValue = receiptEndTotal;
                            #endregion
                            #region period 12

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 12).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            receiptBegTotal = 0;
                            receiptEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                receiptBegTotal = receiptBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                receiptEndTotal = receiptEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            receiptStandardTotal = receiptEndTotal - receiptBegTotal;
                            reportitem.MonthTwelveValue = receiptStandardTotal;
                            reportitem.MonthTwelveEndValue = receiptEndTotal;
                            #endregion

                            cashflowRecord.Receipts.Add(reportitem);
                        }

                       

                    }
                    #endregion
                    #region payout
                    foreach (var payOut in cashFlow.PayOuts)
                    {
                        #region  account range

                        var accountRange = payOut.GaapCashFlowPayOutRanges.ToList();
                        

                        #endregion

                        if (fyearObj != null)
                        {
                            var reportitem = new CashFlowProjectionLineVm
                            {
                                Description = payOut.Description
                            };

                            #region period 1

                            int periodid = glPeriods.FirstOrDefault(x => x.Index == 1).GLPeriodId;
                            var currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            decimal payOutBegTotal = 0;
                            decimal payOutStandardTotal = 0;
                            decimal payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }

                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.Beginning = payOutBegTotal;

                            reportitem.MonthOneValue = payOutStandardTotal;
                            #endregion
                            #region period 2

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 2).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthTwoValue = payOutStandardTotal;
                            #endregion
                            #region period 3

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 3).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthThreeValue = payOutStandardTotal;
                            #endregion
                            #region period 4

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 4).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthFourValue = payOutStandardTotal;
                            #endregion
                            #region period 5

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 5).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthFiveValue = payOutStandardTotal;
                            #endregion
                            #region period 6

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 6).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthSixValue = payOutStandardTotal;
                            #endregion
                            #region period 7

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 7).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthSevenValue = payOutStandardTotal;
                            #endregion
                            #region period 8

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 8).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthEightValue = payOutStandardTotal;
                            #endregion
                            #region period 9

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 9).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthNineValue = payOutStandardTotal;
                            #endregion
                            #region period 10

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 10).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthTenValue = payOutStandardTotal;
                            #endregion
                            #region period 11

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 11).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthElevenValue = payOutStandardTotal;
                            #endregion
                            #region period 12

                            periodid = glPeriods.FirstOrDefault(x => x.Index == 12).GLPeriodId;
                            currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, periodid);
                            payOutBegTotal = 0;
                            payOutEndTotal = 0;
                            foreach (var range in accountRange)
                            {
                                payOutBegTotal = payOutBegTotal + _reportHelper.GetAccountBegBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                                payOutEndTotal = payOutEndTotal + _reportHelper.GetAccountEndBalancesWithAllranges(currnetYearBalance, range.AccountFrom, range.AccountTo);
                            }
                            payOutStandardTotal = payOutEndTotal - payOutBegTotal;
                            reportitem.MonthTwelveValue = payOutStandardTotal;
                            #endregion

                            cashflowRecord.PayOuts.Add(reportitem);
                        }



                    }
                    #endregion
                    #region cash on hand

                        var monthOneValueBeg = cashflowRecord.Receipts.Sum(x => x.MonthOneEndValue) - cashflowRecord.Receipts.Sum(x => x.MonthOneValue);
                        var monthOneValueEnd = monthOneValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthOneValue) - cashflowRecord.PayOuts.Sum(x => x.MonthOneValue);

                        var monthTwoValueBeg = monthOneValueEnd;
                        var monthTwoValueEnd = monthTwoValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthTwoValue) - cashflowRecord.PayOuts.Sum(x => x.MonthTwoValue);

                        var monthThreeValueBeg = monthTwoValueEnd;
                        var monthThreeValueEnd = monthThreeValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthThreeValue) - cashflowRecord.PayOuts.Sum(x => x.MonthThreeValue);

                        var monthFourValueBeg = monthThreeValueEnd;
                        var monthFourValueEnd = monthFourValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthFourValue) - cashflowRecord.PayOuts.Sum(x => x.MonthFourValue);

                        var monthFiveValueBeg = monthFourValueEnd;
                        var monthFiveValueEnd = monthFiveValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthFiveValue) - cashflowRecord.PayOuts.Sum(x => x.MonthFiveValue);

                        var monthSixValueBeg = monthFiveValueEnd;
                        var monthSixValueEnd = monthSixValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthSixValue) - cashflowRecord.PayOuts.Sum(x => x.MonthSixValue);

                        var monthSevenValueBeg = monthSixValueEnd;
                        var monthSevenValueEnd = monthSevenValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthSevenValue) - cashflowRecord.PayOuts.Sum(x => x.MonthSevenValue);

                        var monthEightValueBeg = monthSevenValueEnd;
                        var monthEightValueEnd = monthEightValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthEightValue) - cashflowRecord.PayOuts.Sum(x => x.MonthEightValue);

                        var monthNineValueBeg = monthEightValueEnd;
                        var monthNineValueEnd = monthNineValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthNineValue) - cashflowRecord.PayOuts.Sum(x => x.MonthNineValue);

                        var monthTenValueBeg = monthNineValueEnd;
                        var monthTenValueEnd = monthTenValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthTenValue) - cashflowRecord.PayOuts.Sum(x => x.MonthTenValue);

                        var monthElevenValueBeg = monthTenValueEnd;
                        var monthElevenValueEnd = monthElevenValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthElevenValue) - cashflowRecord.PayOuts.Sum(x => x.MonthElevenValue);

                        var monthTwelveValueBeg = monthElevenValueEnd;
                        var monthTwelveValueEnd = monthTwelveValueBeg+cashflowRecord.Receipts.Sum(x => x.MonthTwelveValue) - cashflowRecord.PayOuts.Sum(x => x.MonthTwelveValue);

                     var begend = new CashFlowProjectionLineVm
                    {
                        Description = "Cash on hand (beginning of month)",
                        MonthOneValue = monthOneValueBeg,
                        MonthTwoValue = monthTwoValueBeg,
                        MonthThreeValue = monthThreeValueBeg,
                        MonthFourValue = monthFourValueBeg,
                        MonthFiveValue = monthFiveValueBeg,
                        MonthSixValue = monthSixValueBeg,
                        MonthSevenValue = monthSevenValueBeg,
                        MonthEightValue = monthEightValueBeg,
                        MonthNineValue = monthNineValueBeg,
                        MonthTenValue = monthTenValueBeg,
                        MonthElevenValue = monthElevenValueBeg,
                        MonthTwelveValue = monthTwelveValueBeg

                    };
                    cashflowRecord.BegEndLines.Add(begend);
                   
                   
                     begend = new CashFlowProjectionLineVm
                    {
                        Description = "Cash Position (end of month)",
                        MonthOneValue = monthOneValueEnd,
                        MonthTwoValue = monthTwoValueEnd,
                        MonthThreeValue = monthThreeValueEnd,
                        MonthFourValue = monthFourValueEnd,
                        MonthFiveValue = monthFiveValueEnd,
                        MonthSixValue = monthSixValueEnd,
                        MonthSevenValue = monthSevenValueEnd,
                        MonthEightValue = monthEightValueEnd,
                        MonthNineValue = monthNineValueEnd,
                        MonthTenValue = monthTenValueEnd,
                        MonthElevenValue = monthElevenValueEnd,
                        MonthTwelveValue = monthTwelveValueEnd

                     };
                    cashflowRecord.BegEndLines.Add(begend);
                    #endregion
                    #region total cash available

                    var cashavailable = new CashFlowProjectionLineVm
                    {
                        Description = "TOTAL CASH AVAILABLE",
                        MonthOneValue = monthOneValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthOneValue),
                        MonthTwoValue = monthTwoValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthTwoValue),
                        MonthThreeValue = monthThreeValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthThreeValue),
                        MonthFourValue = monthFourValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthFourValue),
                        MonthFiveValue = monthFiveValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthFiveValue),
                        MonthSixValue = monthSixValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthSixValue),
                        MonthSevenValue = monthSevenValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthSevenValue),
                        MonthEightValue = monthEightValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthEightValue),
                        MonthNineValue = monthNineValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthNineValue),
                        MonthTenValue = monthTenValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthTenValue),
                        MonthElevenValue = monthElevenValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthElevenValue),
                        MonthTwelveValue = monthTwelveValueBeg + cashflowRecord.Receipts.Sum(x => x.MonthTwelveValue)

                    };
                    cashflowRecord.TotalCashAvailable.Add(cashavailable);
                    
                    #endregion
                    cashFlowList.Add(cashflowRecord);

                    }
                }


                return cashFlowList;
            }
        }
    }