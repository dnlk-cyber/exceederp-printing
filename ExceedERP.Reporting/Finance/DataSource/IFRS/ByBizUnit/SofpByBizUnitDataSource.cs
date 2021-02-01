using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel.GAAP;
using ExceedERP.Reporting.Finance.ViewModel.IFRS;
using Type = ExceedERP.Reporting.Finance.ViewModel.IFRS.Type;

namespace ExceedERP.Reporting.Finance.DataSource.IFRS.ByBizUnit
{
    [DataObject]
    public class SofpByBizUnitDataSource : SofpReportVeiwModel
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ReportHelper _reportHelper = new ReportHelper();
        private readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
        //private readonly ClosingHelper _closingHelper = new ClosingHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NoteVeiwModel> SofpReportNotes(string fiscalyear, string period, string bizUnitCode)
        {
            List<NoteVeiwModel> list = new List<NoteVeiwModel>();
            var notes = _db.IfrsStatementOfFinancialPositions.FirstOrDefault();
            var assetRanges = _db.IfrsSofpAssetRanges.ToList();
            var equityAndLiablityRanges = _db.IfrsSofpEquityAndLiablityRanges.ToList();
           
            if (notes != null && !String.IsNullOrEmpty(fiscalyear) && !String.IsNullOrEmpty(bizUnitCode))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));
                var prevYearBalance = new List<GLLedgerPostingLines>();
                var currnetYearBalance = new List<GLLedgerPostingLines>();


                if (fyearObj != null)
                {
                    int currentPeriod = String.IsNullOrEmpty(period) ? 0 : int.Parse(period);
                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                    currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, currentPeriod);
                    if (prevfyearObj != null)
                    {
                        int previousPeriod = String.IsNullOrEmpty(period) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriod, prevfyearObj.GlFiscalYearId);
                        prevYearBalance = _reportHelper.GetAccountBalance(prevfyearObj.GlFiscalYearId, previousPeriod);
                    }
                }

                //var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));
                //var prevYearBalance = new List<GLLedgerPostingLines>();
                //var prevYearBegBalance = new List<GLLedgerPostingLines>();
                //var currentYearBalance = new List<GLLedgerPostingLines>();
                //var currentYearBegBalance = new List<GLLedgerPostingLines>();


                //if (fyearObj != null)
                //{
                //    int currentPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : int.Parse(periodTo);
                //    int currentPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : int.Parse(periodFrom);
                //    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                //    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                //    currentYearBalance = _reportHelper.GetAccountEndBalance(fyearObj.GlFiscalYearId, currentPeriodTo);
                //    currentYearBegBalance = _reportHelper.GetAccountBegBalance(fyearObj.GlFiscalYearId, currentPeriodFrom);
                //    if (prevfyearObj != null)
                //    {
                //        int previousPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodTo, prevfyearObj.GlFiscalYearId);
                //        int previousPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodFrom, prevfyearObj.GlFiscalYearId);
                //        prevYearBalance = _reportHelper.GetAccountEndBalance(prevfyearObj.GlFiscalYearId, previousPeriodTo);
                //        prevYearBegBalance = _reportHelper.GetAccountBegBalance(prevfyearObj.GlFiscalYearId, previousPeriodFrom);
                //    }
                //}

                #region asset
                foreach (var con in notes.Asstets)
                {
                    var connote = new NoteVeiwModel
                    {
                        Description = con.Description,
                        Type = Type.Asset,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<NoteDetailViewModel>()
                    };
                    //get range
                    var range = assetRanges
                        .Where(x => x.IfrsSofpAssetId == con.IfrsSofpAssetId).ToList();
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
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(prevYearBalance, accRange.Values,bizUnitCode),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(currnetYearBalance, accRange.Values,bizUnitCode)
                                //PreviousYearBalance = _reportHelper.GetAccountEndBalancesByAccountAndBizUnitWithAllranges(prevYearBalance, r.AccountFrom, r.AccountTo, accRange.Values, bizUnitCode)
                                //                      - _reportHelper.GetAccountBegBalancesByAccountAndBizUnitWithAllranges(prevYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values, bizUnitCode),
                                //CurrentYearBalance = _reportHelper.GetAccountEndBalancesByAccountAndBizUnitWithAllranges(currentYearBalance, r.AccountFrom, r.AccountTo, accRange.Values, bizUnitCode)
                                //                     - _reportHelper.GetAccountBegBalancesByAccountAndBizUnitWithAllranges(currentYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values, bizUnitCode)
                            };
                            connote.NoteDetailsList.Add(detail);
                        }
                    }

                    list.Add(connote);

                }
                #endregion
                #region equityAndLiablityRanges
                foreach (var con in notes.EquityAndLiablity)
                {
                    var connote = new NoteVeiwModel
                    {
                        Description = con.Description,
                        Type = Type.Liability,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<NoteDetailViewModel>()
                    };
                    //get range
                    var range = equityAndLiablityRanges
                        .Where(x => x.IfrsSofpEquityAndLiablityId == con.IfrsSofpEquityAndLiablityId).ToList();
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
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(prevYearBalance, accRange.Values,bizUnitCode),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(currnetYearBalance, accRange.Values,bizUnitCode)
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
        public SofpReportVeiwModel SofpReport(string fiscalyear, string period, string bizUnitCode)
        {
            SofpReportVeiwModel sofpReport = new SofpReportVeiwModel();

            if (!String.IsNullOrEmpty(fiscalyear) && !String.IsNullOrEmpty(bizUnitCode))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));

                var sofpRules = _db.IfrsStatementOfFinancialPositions.FirstOrDefault();

                var prevYearBalance = new List<GLLedgerPostingLines>();
                var currnetYearBalance = new List<GLLedgerPostingLines>();

                decimal previousyearEarning = 0;
                decimal currentyearEarning = 0;

                #region assets variables

                decimal currentYearSumCurrentAsset = 0;
                decimal currentYearSumNonCurrentAsset = 0;

                decimal previosYearSumCurrentAsset = 0;
                decimal previosYearSumNonCurrentAsset = 0;

                decimal currentYearSumTotalAsset = 0;
                decimal previosYearSumTotalAsset = 0;

                #endregion

                #region equity and liability variables

                decimal currentYearSumCapitalAndReserves = 0;
                decimal previousYearSumCapitalAndReserves = 0;

                #endregion

                #region liability variables

                decimal currentYearSumCurrentLiability = 0;
                decimal currentYearSumNonCurrentLiability = 0;

                decimal previosYearSumCurrentLiability = 0;
                decimal previosYearSumNonCurrentLiability = 0;

                decimal currentYearSumTotalLiability = 0;
                decimal previosYearSumTotalLiability = 0;

                #endregion

                if (fyearObj != null && sofpRules != null)
                {
                    int currentPeriod = String.IsNullOrEmpty(period) ? 0 : int.Parse(period);
                    int previousPeriod = 0;
                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);
                    
                    currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, currentPeriod);
                    if (prevfyearObj != null)
                    {
                        previousPeriod = String.IsNullOrEmpty(period) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriod, prevfyearObj.GlFiscalYearId);
                        prevYearBalance = _reportHelper.GetAccountBalance(prevfyearObj.GlFiscalYearId, previousPeriod);
                    }
                    #region asset

                    decimal currentYearNonCurrentAssetTotal = 0;
                    decimal prevYearNonCurrentAssetTotal = 0;

                    decimal currentYearCurrentAssetTotal = 0;
                    decimal prevYearCurrentAssetTotal = 0;

                    #region asset header

                    var reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "ASSETS",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #region non current asset header

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Non Current Assets ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #region non current asset

                    var nonCurrentAssets = sofpRules.Asstets
                        .Where(x => x.Category == IfrsSofpAssetTypes.NonCurrentAsset).ToList();


                    List<string> accountValues;
                    foreach (var nonCurrentAsset in nonCurrentAssets)
                    {
                        currentYearNonCurrentAssetTotal = 0;
                        prevYearNonCurrentAssetTotal = 0;


                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = nonCurrentAsset.IfrsSofpAssetRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            var accountValuesRange = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo,
                                GlSegmentTypes.Account);
                            if (accountValuesRange != null)
                                accountValues.AddRange(accountValuesRange);
                        }

                        #endregion

                        foreach (var account in accountValues)
                        {
                            #region current year

                            currentYearNonCurrentAssetTotal =
                                currentYearNonCurrentAssetTotal +
                                _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(currnetYearBalance, account, bizUnitCode);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearNonCurrentAssetTotal =
                                    prevYearNonCurrentAssetTotal +
                                    _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(prevYearBalance, account, bizUnitCode);

                                #endregion
                            }


                        }
                        currentYearSumNonCurrentAsset = currentYearSumNonCurrentAsset + currentYearNonCurrentAssetTotal;
                        previosYearSumNonCurrentAsset = previosYearSumNonCurrentAsset + prevYearNonCurrentAssetTotal;

                        reportitem = new SofpReportItemVeiwModel
                        {
                            Notes = nonCurrentAsset.NoteNumber,
                            CategoryDescription = nonCurrentAsset.Description,
                            CurrentYearBalance = currentYearNonCurrentAssetTotal,
                            PreviousYearBalance = prevYearNonCurrentAssetTotal,
                            IsHeader = false
                        };
                        sofpReport.SofpReportItems.Add(reportitem);
                    }

                    #region sum non currrent asset

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Total Non Current Assets ",
                        CurrentYearBalance = currentYearSumNonCurrentAsset,
                        PreviousYearBalance = previosYearSumNonCurrentAsset,
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region current asset header

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Current Assets ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #region  current asset

                    var currentAssets = sofpRules.Asstets.Where(x => x.Category == IfrsSofpAssetTypes.CurrentAsset)
                        .ToList();

                    foreach (var currentAsset in currentAssets)
                    {
                        currentYearCurrentAssetTotal = 0;
                        prevYearCurrentAssetTotal = 0;


                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = currentAsset.IfrsSofpAssetRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            var accountValuesRange = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo,
                                GlSegmentTypes.Account);
                            if (accountValuesRange != null)
                                accountValues.AddRange(accountValuesRange);
                        }

                        #endregion

                        foreach (var account in accountValues)
                        {
                            #region current year

                            currentYearCurrentAssetTotal =
                                currentYearCurrentAssetTotal +
                                _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(currnetYearBalance, account, bizUnitCode);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearCurrentAssetTotal =
                                    prevYearCurrentAssetTotal +
                                    _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(prevYearBalance, account, bizUnitCode);

                                #endregion
                            }


                        }
                        currentYearSumCurrentAsset = currentYearSumCurrentAsset + currentYearCurrentAssetTotal;
                        previosYearSumCurrentAsset = previosYearSumCurrentAsset + prevYearCurrentAssetTotal;

                        reportitem = new SofpReportItemVeiwModel
                        {
                            Notes = currentAsset.NoteNumber,
                            CategoryDescription = currentAsset.Description,
                            CurrentYearBalance = currentYearCurrentAssetTotal,
                            PreviousYearBalance = prevYearCurrentAssetTotal,
                            IsHeader = false
                        };
                        sofpReport.SofpReportItems.Add(reportitem);
                    }

                    #region sum non currrent asset

                    reportitem = new SofpReportItemVeiwModel();
                    reportitem.CategoryDescription = "Total Current Assets ";
                    reportitem.CurrentYearBalance = currentYearSumCurrentAsset;
                    reportitem.PreviousYearBalance = previosYearSumCurrentAsset;
                    reportitem.IsHeader = true;
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region total asstet

                    reportitem = new SofpReportItemVeiwModel();
                    reportitem.CategoryDescription = "Total Assets ";
                    reportitem.CurrentYearBalance = (currentYearSumCurrentAsset + currentYearSumNonCurrentAsset);
                    reportitem.PreviousYearBalance = (previosYearSumCurrentAsset + previosYearSumNonCurrentAsset);
                    reportitem.IsHeader = true;
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region  equity and liability

                    decimal currentYearCapitalandReserveTotal = 0;
                    decimal prevYearCapitalandReserveTotal = 0;

                    #region equity and liability header

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Equity and Liablities",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #region Capital and Reserves header

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Capital and Reserves",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #region capital and Reserves

                    var capitalAndReserves = sofpRules.EquityAndLiablity
                        .Where(x => x.Category == IfrsSofpEquityAndLiablityTypes.CapitalAndReserves).ToList();

                    foreach (var capitalAndReserve in capitalAndReserves)
                    {
                        currentYearCapitalandReserveTotal = 0;
                        prevYearCapitalandReserveTotal = 0;


                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = capitalAndReserve.IfrsSofpEquityAndLiablityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            var accountValuesRange = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo,
                                GlSegmentTypes.Account);
                            if (accountValuesRange != null)
                                accountValues.AddRange(accountValuesRange);
                        }

                        #endregion

                        foreach (var account in accountValues)
                        {
                            #region current year

                            currentYearCapitalandReserveTotal =
                                currentYearCapitalandReserveTotal +
                                _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(currnetYearBalance, account, bizUnitCode);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearCapitalandReserveTotal =
                                    prevYearCapitalandReserveTotal +
                                    _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(prevYearBalance, account, bizUnitCode);

                                #endregion
                            }


                        }
                        currentYearSumCapitalAndReserves =
                            currentYearSumCapitalAndReserves + currentYearCapitalandReserveTotal;
                        previousYearSumCapitalAndReserves =
                            previousYearSumCapitalAndReserves + prevYearCapitalandReserveTotal;

                        reportitem = new SofpReportItemVeiwModel
                        {
                            Notes = capitalAndReserve.NoteNumber,
                            CategoryDescription = capitalAndReserve.Description,
                            CurrentYearBalance = currentYearCapitalandReserveTotal,
                            PreviousYearBalance = prevYearCapitalandReserveTotal,
                            IsHeader = false
                        };
                        sofpReport.SofpReportItems.Add(reportitem);
                    }

                    #region current year earning

                    if (prevfyearObj != null)
                        previousyearEarning = _reportHelper.GetEarningByBusinessUnit(bizUnitCode, previousPeriod);
                    if (fyearObj != null)
                        currentyearEarning = _reportHelper.GetEarningByBusinessUnit(bizUnitCode, currentPeriod);

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Current Year Earning ",
                        CurrentYearBalance = currentyearEarning,
                        PreviousYearBalance = previousyearEarning,
                        IsHeader = false
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #region sum Capital and Reserves
                    currentYearSumCapitalAndReserves = currentYearSumCapitalAndReserves + currentyearEarning;
                    previousYearSumCapitalAndReserves = previousYearSumCapitalAndReserves + previousyearEarning;

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Total Capital and Reserves ",
                        CurrentYearBalance = currentYearSumCapitalAndReserves,
                        PreviousYearBalance = previousYearSumCapitalAndReserves,
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #endregion

                    #region liability

                    decimal currentYearNonCurrentLiabilityTotal = 0;
                    decimal prevYearNonCurrentLiabilityTotal = 0;

                    decimal currentYearCurrentLiabilityTotal = 0;
                    decimal prevYearCurrentLiabilityTotal = 0;


                    #region non current Liability header

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Non Current Liabilities ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #region non current Liability

                    var nonCurrentLiabilities = sofpRules.EquityAndLiablity
                        .Where(x => x.Category == IfrsSofpEquityAndLiablityTypes.NonCurrentLiability).ToList();

                    foreach (var nonCurrentLiability in nonCurrentLiabilities)
                    {
                        currentYearNonCurrentLiabilityTotal = 0;
                        prevYearNonCurrentLiabilityTotal = 0;


                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = nonCurrentLiability.IfrsSofpEquityAndLiablityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            var accountValuesRange = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo,
                                GlSegmentTypes.Account);
                            if (accountValuesRange != null)
                                accountValues.AddRange(accountValuesRange);
                        }

                        #endregion

                        foreach (var account in accountValues)
                        {
                            #region current year

                            currentYearNonCurrentLiabilityTotal =
                                currentYearNonCurrentLiabilityTotal +
                                _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(currnetYearBalance, account, bizUnitCode);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearNonCurrentLiabilityTotal =
                                    prevYearNonCurrentLiabilityTotal +
                                    _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(prevYearBalance, account, bizUnitCode);

                                #endregion
                            }


                        }
                        currentYearSumNonCurrentLiability =
                            currentYearSumNonCurrentLiability + currentYearNonCurrentLiabilityTotal;
                        previosYearSumNonCurrentLiability =
                            previosYearSumNonCurrentLiability + prevYearNonCurrentLiabilityTotal;

                        reportitem =
                            new SofpReportItemVeiwModel
                            {
                                Notes = nonCurrentLiability.NoteNumber,
                                CategoryDescription = nonCurrentLiability.Description,
                                CurrentYearBalance = currentYearNonCurrentLiabilityTotal,
                                PreviousYearBalance = prevYearNonCurrentLiabilityTotal,
                                IsHeader = false
                            };
                        sofpReport.SofpReportItems.Add(reportitem);
                    }

                    #region sum non currrent liability

                    reportitem = new SofpReportItemVeiwModel();
                    reportitem.CategoryDescription = "Total Non Current Liablities ";
                    reportitem.CurrentYearBalance = currentYearSumNonCurrentLiability;
                    reportitem.PreviousYearBalance = previosYearSumNonCurrentLiability;
                    reportitem.IsHeader = true;
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region current liability header

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Current Liablities",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #region  current Liablity

                    var currentLiablities = sofpRules.EquityAndLiablity
                        .Where(x => x.Category == IfrsSofpEquityAndLiablityTypes.CurrentLiability).ToList();

                    foreach (var currentLiablity in currentLiablities)
                    {
                        currentYearCurrentLiabilityTotal = 0;
                        prevYearCurrentLiabilityTotal = 0;


                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = currentLiablity.IfrsSofpEquityAndLiablityRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            var accountValuesRange = _accountHelper.GeAccountRange(range.AccountFrom, range.AccountTo,
                                GlSegmentTypes.Account);
                            if (accountValuesRange != null)
                                accountValues.AddRange(accountValuesRange);

                        }

                        #endregion

                        foreach (var account in accountValues)
                        {
                            #region current year

                            currentYearCurrentLiabilityTotal =
                                currentYearCurrentLiabilityTotal +
                                _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(currnetYearBalance, account, bizUnitCode);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearCurrentLiabilityTotal =
                                    prevYearCurrentLiabilityTotal +
                                    _reportHelper.GetAccountEndBalancesbyAccountAndBizUnit(prevYearBalance, account, bizUnitCode);

                                #endregion
                            }


                        }
                        currentYearSumCurrentLiability =
                            currentYearSumCurrentLiability + currentYearCurrentLiabilityTotal;
                        previosYearSumCurrentLiability = previosYearSumCurrentLiability + prevYearCurrentLiabilityTotal;

                        reportitem = new SofpReportItemVeiwModel
                        {
                            Notes = currentLiablity.NoteNumber,
                            CategoryDescription = currentLiablity.Description,
                            CurrentYearBalance = currentYearCurrentLiabilityTotal,
                            PreviousYearBalance = prevYearCurrentLiabilityTotal,
                            IsHeader = false
                        };
                        sofpReport.SofpReportItems.Add(reportitem);
                    }


                    #region  currrent year profit tax
                    decimal previousYearProfitTax = 0;
                    decimal currentYearProfitTax = 0;

                    if (FinanceHelper.CalculateProfitTax())
                    {

                        if (prevfyearObj != null)
                            previousYearProfitTax = _reportHelper.GetProfitTax(previousyearEarning);

                        if (fyearObj != null)
                            currentYearProfitTax = _reportHelper.GetProfitTax(currentyearEarning);


                        reportitem = new SofpReportItemVeiwModel
                        {
                            CategoryDescription = "Current Year Profit Tax",
                            CurrentYearBalance = currentYearProfitTax,
                            PreviousYearBalance = previousYearProfitTax,
                            IsHeader = false
                        };
                        sofpReport.SofpReportItems.Add(reportitem);
                    }

                    #endregion
                    #region sum currrent Liablities
                    currentYearSumCurrentLiability = currentYearSumCurrentLiability + currentYearProfitTax;
                    previosYearSumCurrentLiability = previosYearSumCurrentLiability + previousYearProfitTax;
                    reportitem = new SofpReportItemVeiwModel();
                    reportitem.CategoryDescription = "Total Current Liablities ";
                    reportitem.CurrentYearBalance = currentYearSumCurrentLiability;
                    reportitem.PreviousYearBalance = previosYearSumCurrentLiability;
                    reportitem.IsHeader = true;
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region total Liablities

                    reportitem = new SofpReportItemVeiwModel();
                    reportitem.CategoryDescription = "Total Liablities";
                    reportitem.CurrentYearBalance = (currentYearSumCurrentLiability + currentYearSumNonCurrentLiability);
                    reportitem.PreviousYearBalance = (previosYearSumCurrentLiability + previosYearSumNonCurrentLiability);
                    reportitem.IsHeader = true;
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #region total Liablities

                    reportitem = new SofpReportItemVeiwModel
                    {
                        CategoryDescription = "Total Equity and Liabilities ",
                        CurrentYearBalance = (currentYearSumCurrentLiability + currentYearSumNonCurrentLiability +
                                              currentYearSumCapitalAndReserves),
                        PreviousYearBalance = (previosYearSumCurrentLiability + previosYearSumNonCurrentLiability +
                                               previousYearSumCapitalAndReserves),
                        IsHeader = true
                    };
                    sofpReport.SofpReportItems.Add(reportitem);

                    #endregion

                    #endregion
                }
            }

            return sofpReport;
        }
    }
}
