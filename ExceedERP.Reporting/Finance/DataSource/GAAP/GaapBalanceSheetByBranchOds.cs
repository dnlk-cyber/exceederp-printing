
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel.GAAP;

namespace ExceedERP.Reporting.Finance.DataSource.GAAP
{
    [DataObject]
    public class GaapBalanceSheetByBranchOds : BalanceSheetReportVeiwModel
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ReportHelper _reportHelper = new ReportHelper();
        readonly AccountHelper _accountHelper = new AccountHelper();
        //private ClosingHelper _closingHelper = new ClosingHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GaapNoteVeiwModel> GaapBalanceSheetReportNotes(string fiscalyear, string period,string project)
        {
            List<GaapNoteVeiwModel> list = new List<GaapNoteVeiwModel>();
            var notes = _db.GapBalanceSheets.FirstOrDefault(x => x.ReportType == GaapReportType.GAAPOne);
            var assetRanges = _db.GaapBalanceSheetAssetRanges.ToList();
            var liabilityRanges = _db.GaapBalanceSheetLiabilityRanges.ToList();
            var capitalRanges = _db.GaapBalanceSheetCapitalRanges.ToList();

            if (!String.IsNullOrEmpty(fiscalyear) && !String.IsNullOrEmpty(project))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));
                var prevYearBalance = new List<GLLedgerPostingLines>();
                var currnetYearBalance = new List<GLLedgerPostingLines>();

                //new
              
                

                if (fyearObj != null)
                {
                    int currentPeriod = String.IsNullOrEmpty(period) ? 0 : int.Parse(period);
                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                    currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId,currentPeriod);
                    if (prevfyearObj != null)
                    {
                        int previousPeriod = String.IsNullOrEmpty(period) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriod, prevfyearObj.GlFiscalYearId);
                        prevYearBalance = _reportHelper.GetAccountBalance(prevfyearObj.GlFiscalYearId,previousPeriod);
                    }
                }
                #region filter by project

                prevYearBalance = prevYearBalance
                   .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();
                currnetYearBalance = currnetYearBalance
                    .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();

                #endregion


                #region asset
                foreach (var con in notes.Assets)
                {
                    var connote = new GaapNoteVeiwModel
                    {
                        Description = con.Description,
                        Type = NoteType.Asset,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<GaapNoteDetailViewModel>()
                    };
                    //get range
                    var range = assetRanges
                        .Where(x => x.GaapBalanceSheetAssetId == con.GaapBalanceSheetAssetId).ToList();
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
                            var detail = new GaapNoteDetailViewModel
                            {
                                AccountDesc = accRange.Description,
                                PreviousYearBalance = _reportHelper.GetAccountEndBalances(prevYearBalance, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalances(currnetYearBalance, accRange.Values)
                            };
                            connote.NoteDetailsList.Add(detail);
                        }
                    }

                    list.Add(connote);

                }
                #endregion
                #region capital
                foreach (var con in notes.Capitals)
                {
                    var connote = new GaapNoteVeiwModel
                    {
                        Description = con.Description,
                        Type = NoteType.Capital,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<GaapNoteDetailViewModel>()
                    };
                    //get range
                    var range = capitalRanges
                        .Where(x => x.GaapBalanceSheetCapitalId == con.GaapBalanceSheetCapitalId).ToList();
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
                            var detail = new GaapNoteDetailViewModel
                            {
                                AccountDesc = accRange.Description,
                                PreviousYearBalance = _reportHelper.GetAccountEndBalances(prevYearBalance, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalances(currnetYearBalance, accRange.Values)
                            };
                            connote.NoteDetailsList.Add(detail);
                        }
                    }

                    list.Add(connote);

                }
                #endregion
                #region liability
                foreach (var con in notes.Liabilities)
                {
                    var connote = new GaapNoteVeiwModel
                    {
                        Description = con.Description,
                        Type = NoteType.Liability,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<GaapNoteDetailViewModel>()
                    };
                    //get range
                    var range = liabilityRanges
                        .Where(x => x.GaapBalanceSheetLiabilityId == con.GaapBalanceSheetLiabilityId).ToList();
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
                            var detail = new GaapNoteDetailViewModel
                            {
                                AccountDesc = accRange.Description,
                                PreviousYearBalance = _reportHelper.GetAccountEndBalances(prevYearBalance, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalances(currnetYearBalance, accRange.Values)
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
        public BalanceSheetReportVeiwModel GaapBalanceSheetReport(string fiscalyear, string period, string project)
        {
            BalanceSheetReportVeiwModel sofpReport = new BalanceSheetReportVeiwModel();

            if (!String.IsNullOrEmpty(fiscalyear) && !String.IsNullOrEmpty(project))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));

                var gaapBalanceSheetRules = _db.GapBalanceSheets.FirstOrDefault(x => x.ReportType == GaapReportType.GAAPOne);

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

                if (fyearObj != null && gaapBalanceSheetRules != null)
                {

                    ClosingHelper closingHelper = new ClosingHelper();

                    #region update earning  ledger--current year

                    closingHelper.UpdateEarningForAYear(fyearObj.GlFiscalYearId);

                    #endregion
                    int currentPeriod = String.IsNullOrEmpty(period) ? 0 : int.Parse(period);
                    int previousPeriod = 0;
                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);
                    
                    currnetYearBalance = _reportHelper.GetAccountBalance(fyearObj.GlFiscalYearId, currentPeriod);
                    if (prevfyearObj != null)
                    {
                        previousPeriod = String.IsNullOrEmpty(period) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriod, prevfyearObj.GlFiscalYearId);
                        prevYearBalance = _reportHelper.GetAccountBalance(prevfyearObj.GlFiscalYearId, previousPeriod);
                        #region update earning  ledger--prev year

                        closingHelper.UpdateEarningForAYear(prevfyearObj.GlFiscalYearId);

                        #endregion
                    }

                    #region filter by project

                    prevYearBalance = prevYearBalance
                       .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();
                    currnetYearBalance = currnetYearBalance
                        .Where(x => x.GLLedgerPosting.BranchSegment == project).ToList();

                    #endregion

                    #region asset

                    decimal currentYearNonCurrentAssetTotal = 0;
                    decimal prevYearNonCurrentAssetTotal = 0;

                    decimal currentYearCurrentAssetTotal = 0;
                    decimal prevYearCurrentAssetTotal = 0;

                    #region asset header

                    var reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "ASSETS",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #region non current asset header

                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Non Current Assets ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                     sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #region non current asset

                    var nonCurrentAssets = gaapBalanceSheetRules.Assets
                        .Where(x => x.Category == GaapAssetTypes.NonCurrentAsset).ToList();


                    List<string> accountValues;
                    foreach (var nonCurrentAsset in nonCurrentAssets)
                    {
                        currentYearNonCurrentAssetTotal = 0;
                        prevYearNonCurrentAssetTotal = 0;


                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = nonCurrentAsset.GaapBalanceSheetAssetRanges.ToList();
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
                                _reportHelper.GetAccountEndBalances(currnetYearBalance, account);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearNonCurrentAssetTotal =
                                    prevYearNonCurrentAssetTotal +
                                    _reportHelper.GetAccountEndBalances(prevYearBalance, account);

                                #endregion
                            }


                        }
                        currentYearSumNonCurrentAsset = currentYearSumNonCurrentAsset + currentYearNonCurrentAssetTotal;
                        previosYearSumNonCurrentAsset = previosYearSumNonCurrentAsset + prevYearNonCurrentAssetTotal;

                        reportitem = new BalanceSheetReportItemVeiwModel
                        {
                            Notes = nonCurrentAsset.NoteNumber,
                            CategoryDescription = nonCurrentAsset.Description,
                            CurrentYearBalance = currentYearNonCurrentAssetTotal,
                            PreviousYearBalance = prevYearNonCurrentAssetTotal,
                            IsHeader = false
                        };
                        sofpReport.BalanceSheetReportItems.Add(reportitem);
                    }

                    #region sum non currrent asset

                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Total Non Current Assets ",
                        CurrentYearBalance = currentYearSumNonCurrentAsset,
                        PreviousYearBalance = previosYearSumNonCurrentAsset,
                        IsHeader = true
                    };
                    sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region current asset header

                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Current Assets ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                     sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #region  current asset

                    var currentAssets = gaapBalanceSheetRules.Assets.Where(x => x.Category == GaapAssetTypes.CurrentAsset)
                        .ToList();

                    foreach (var currentAsset in currentAssets)
                    {
                        currentYearCurrentAssetTotal = 0;
                        prevYearCurrentAssetTotal = 0;


                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = currentAsset.GaapBalanceSheetAssetRanges.ToList();
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
                                _reportHelper.GetAccountEndBalances(currnetYearBalance, account);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearCurrentAssetTotal =
                                    prevYearCurrentAssetTotal +
                                    _reportHelper.GetAccountEndBalances(prevYearBalance, account);

                                #endregion
                            }


                        }
                        currentYearSumCurrentAsset = currentYearSumCurrentAsset + currentYearCurrentAssetTotal;
                        previosYearSumCurrentAsset = previosYearSumCurrentAsset + prevYearCurrentAssetTotal;

                        reportitem = new BalanceSheetReportItemVeiwModel
                        {
                            Notes = currentAsset.NoteNumber,
                            CategoryDescription = currentAsset.Description,
                            CurrentYearBalance = currentYearCurrentAssetTotal,
                            PreviousYearBalance = prevYearCurrentAssetTotal,
                            IsHeader = false
                        };
                        sofpReport.BalanceSheetReportItems.Add(reportitem);
                    }

                    #region sum non currrent asset

                    reportitem = new BalanceSheetReportItemVeiwModel();
                    reportitem.CategoryDescription = "Total Current Assets ";
                    reportitem.CurrentYearBalance = currentYearSumCurrentAsset;
                    reportitem.PreviousYearBalance = previosYearSumCurrentAsset;
                    reportitem.IsHeader = true;
                     sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region total asstet

                    reportitem = new BalanceSheetReportItemVeiwModel();
                    reportitem.CategoryDescription = "Total Assets ";
                    reportitem.CurrentYearBalance = (currentYearSumCurrentAsset + currentYearSumNonCurrentAsset);
                    reportitem.PreviousYearBalance = (previosYearSumCurrentAsset + previosYearSumNonCurrentAsset);
                    reportitem.IsHeader = true;
                     sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region  equity and liability-capital

                    decimal currentYearCapitalandReserveTotal = 0;
                    decimal prevYearCapitalandReserveTotal = 0;

                    #region equity and liability header

                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Equity and Liablities",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                     sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #region Capital and Reserves header

                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Capital and Reserves",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                     sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #region capital and Reserves--capital

                    var capitalAndReserves = gaapBalanceSheetRules.Capitals
                        .Where(x => x.Category == GaapEquityTypes.CapitalAndReserves).ToList();

                    foreach (var capitalAndReserve in capitalAndReserves)
                    {
                        currentYearCapitalandReserveTotal = 0;
                        prevYearCapitalandReserveTotal = 0;

                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = capitalAndReserve.GaapBalanceSheetCapitalRanges.ToList();
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
                                _reportHelper.GetAccountEndBalances(currnetYearBalance, account);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearCapitalandReserveTotal =
                                    prevYearCapitalandReserveTotal +
                                    _reportHelper.GetAccountEndBalances(prevYearBalance, account);

                                #endregion
                            }


                        }
                        currentYearSumCapitalAndReserves =
                            currentYearSumCapitalAndReserves + currentYearCapitalandReserveTotal;
                        previousYearSumCapitalAndReserves =
                            previousYearSumCapitalAndReserves + prevYearCapitalandReserveTotal;

                        reportitem = new BalanceSheetReportItemVeiwModel
                        {
                            Notes = capitalAndReserve.NoteNumber,
                            CategoryDescription = capitalAndReserve.Description,
                            CurrentYearBalance = currentYearCapitalandReserveTotal,
                            PreviousYearBalance = prevYearCapitalandReserveTotal,
                            IsHeader = false
                        };
                        sofpReport.BalanceSheetReportItems.Add(reportitem);
                    }

                    #region current year earning

                   
                    if (prevfyearObj != null)
                        previousyearEarning = _reportHelper.GetEarningByBranch(project, previousPeriod);
                    if (fyearObj != null)
                        currentyearEarning = _reportHelper.GetEarningByBranch(project, currentPeriod);

                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Current Year Earning ",
                        CurrentYearBalance = currentyearEarning,
                        PreviousYearBalance = previousyearEarning,
                        IsHeader = false
                    };
                    sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #region sum Capital and Reserves

                    currentYearSumCapitalAndReserves=currentYearSumCapitalAndReserves + currentyearEarning;
                    previousYearSumCapitalAndReserves = previousYearSumCapitalAndReserves + previousyearEarning;
                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Total Capital and Reserves ",
                        CurrentYearBalance = currentYearSumCapitalAndReserves,
                        PreviousYearBalance = previousYearSumCapitalAndReserves,
                        IsHeader = true
                    };
                    sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #endregion

                    #region liability

                    decimal currentYearNonCurrentLiabilityTotal = 0;
                    decimal prevYearNonCurrentLiabilityTotal = 0;

                    decimal currentYearCurrentLiabilityTotal = 0;
                    decimal prevYearCurrentLiabilityTotal = 0;


                    #region non current Liability header

                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Non Current Liabilities ",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                     sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #region non current Liability

                    var nonCurrentLiabilities = gaapBalanceSheetRules.Liabilities
                        .Where(x => x.Category == GaapLiablityTypes.NonCurrentLiability).ToList();

                    foreach (var nonCurrentLiability in nonCurrentLiabilities)
                    {
                        currentYearNonCurrentLiabilityTotal = 0;
                        prevYearNonCurrentLiabilityTotal = 0;


                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = nonCurrentLiability.GaapBalanceSheetLiabilityRanges.ToList();
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
                                _reportHelper.GetAccountEndBalances(currnetYearBalance, account);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearNonCurrentLiabilityTotal =
                                    prevYearNonCurrentLiabilityTotal +
                                    _reportHelper.GetAccountEndBalances(prevYearBalance, account);

                                #endregion
                            }


                        }
                        currentYearSumNonCurrentLiability =
                            currentYearSumNonCurrentLiability + currentYearNonCurrentLiabilityTotal;
                        previosYearSumNonCurrentLiability =
                            previosYearSumNonCurrentLiability + prevYearNonCurrentLiabilityTotal;

                        reportitem =
                            new BalanceSheetReportItemVeiwModel
                            {
                                Notes = nonCurrentLiability.NoteNumber,
                                CategoryDescription = nonCurrentLiability.Description,
                                CurrentYearBalance = currentYearNonCurrentLiabilityTotal,
                                PreviousYearBalance = prevYearNonCurrentLiabilityTotal,
                                IsHeader = false
                            };
                        sofpReport.BalanceSheetReportItems.Add(reportitem);
                    }

                    #region sum non currrent liability

                    reportitem =new BalanceSheetReportItemVeiwModel
                        {
                            CategoryDescription = "Total Non Current Liablities ",
                            CurrentYearBalance = currentYearSumNonCurrentLiability,
                            PreviousYearBalance = previosYearSumNonCurrentLiability,
                            IsHeader = true
                        };
                    sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region current liability header

                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Current Liablities",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                     sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #region  current Liablity

                    var currentLiablities = gaapBalanceSheetRules.Liabilities
                        .Where(x => x.Category == GaapLiablityTypes.CurrentLiability).ToList();

                    foreach (var currentLiablity in currentLiablities)
                    {
                        currentYearCurrentLiabilityTotal = 0;
                        prevYearCurrentLiabilityTotal = 0;


                        accountValues = new List<string>();

                        #region  account range

                        var accountRange = currentLiablity.GaapBalanceSheetLiabilityRanges.ToList();
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
                                _reportHelper.GetAccountEndBalances(currnetYearBalance, account);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearCurrentLiabilityTotal =
                                    prevYearCurrentLiabilityTotal +
                                    _reportHelper.GetAccountEndBalances(prevYearBalance, account);

                                #endregion
                            }


                        }
                        currentYearSumCurrentLiability =currentYearSumCurrentLiability + currentYearCurrentLiabilityTotal;
                        previosYearSumCurrentLiability = previosYearSumCurrentLiability + prevYearCurrentLiabilityTotal;

                        reportitem = new BalanceSheetReportItemVeiwModel
                        {
                            Notes = currentLiablity.NoteNumber,
                            CategoryDescription = currentLiablity.Description,
                            CurrentYearBalance = currentYearCurrentLiabilityTotal,
                            PreviousYearBalance = prevYearCurrentLiabilityTotal,
                            IsHeader = false
                        };
                        sofpReport.BalanceSheetReportItems.Add(reportitem);
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


                        reportitem = new BalanceSheetReportItemVeiwModel
                        {
                            CategoryDescription = "Current Year Profit Tax",
                            CurrentYearBalance = currentYearProfitTax,
                            PreviousYearBalance = previousYearProfitTax,
                            IsHeader = false
                        };
                        sofpReport.BalanceSheetReportItems.Add(reportitem);
                    }

                    #endregion

                    #region sum currrent Liablities

                    currentYearSumCurrentLiability = currentYearSumCurrentLiability + currentYearProfitTax;
                    previosYearSumCurrentLiability=previosYearSumCurrentLiability + previousYearProfitTax;
                    reportitem =new BalanceSheetReportItemVeiwModel
                        {
                            CategoryDescription = "Total Current Liablities ",
                            CurrentYearBalance = currentYearSumCurrentLiability,
                            PreviousYearBalance = previosYearSumCurrentLiability,
                            IsHeader = true
                        };
                    sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #endregion

                    #region total Liablities

                    reportitem = new BalanceSheetReportItemVeiwModel
                    {
                        CategoryDescription = "Total Liablities",
                        CurrentYearBalance = (currentYearSumCurrentLiability + currentYearSumNonCurrentLiability),
                        PreviousYearBalance = (previosYearSumCurrentLiability + previosYearSumNonCurrentLiability),
                        IsHeader = true
                    };
                    sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #region total  Equity and Liabilities

                    reportitem =
                        new BalanceSheetReportItemVeiwModel
                        {
                            CategoryDescription = "Total Equity and Liabilities ",
                            CurrentYearBalance = (currentYearSumCurrentLiability + currentYearSumNonCurrentLiability +
                                                  currentYearSumCapitalAndReserves),
                            PreviousYearBalance = (previosYearSumCurrentLiability + previosYearSumNonCurrentLiability +
                                                   previousYearSumCapitalAndReserves),
                            IsHeader = true
                        };
                    sofpReport.BalanceSheetReportItems.Add(reportitem);

                    #endregion

                    #endregion
                }
            }

            return sofpReport;
        }

        
    }
}
