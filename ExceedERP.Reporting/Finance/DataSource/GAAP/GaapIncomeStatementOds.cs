using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel.GAAP;

namespace ExceedERP.Reporting.Finance.DataSource.GAAP
{
    [DataObject]
    public class GaapIncomeStatementOds : IncomeStatementReportVeiwModel
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ReportHelper _reportHelper = new ReportHelper();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GaapNoteVeiwModel> IncomestatementReportNotes(string fiscalyear, string periodFrom, string periodTo)
        {
            List<GaapNoteVeiwModel> list = new List<GaapNoteVeiwModel>();
            var notes = _db.GaapIncomeStatements.FirstOrDefault(x => x.ReportType == GaapReportType.GAAPOne);
            var operationRevenueRanges = _db.GaapIncomeStatementOperationRevenueRanges.ToList();
            var operationExpenseRanges = _db.GaapIncomeStatementOperationExpenseRanges.ToList();

            if (notes != null && !String.IsNullOrEmpty(fiscalyear))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));
                var prevYearBalance = new List<GLLedgerPostingLines>();
                var prevYearBegBalance = new List<GLLedgerPostingLines>();
                var currentYearBalance = new List<GLLedgerPostingLines>();
                var currentYearBegBalance = new List<GLLedgerPostingLines>();


                if (fyearObj != null)
                {
                    int currentPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : int.Parse(periodTo);
                    int currentPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : int.Parse(periodFrom);
                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                    currentYearBalance = _reportHelper.GetAccountEndBalance(fyearObj.GlFiscalYearId, currentPeriodTo);
                    currentYearBegBalance = _reportHelper.GetAccountBegBalance(fyearObj.GlFiscalYearId, currentPeriodFrom);
                    if (prevfyearObj != null)
                    {
                        int previousPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodTo, prevfyearObj.GlFiscalYearId);
                        int previousPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodFrom, prevfyearObj.GlFiscalYearId);
                        prevYearBalance = _reportHelper.GetAccountEndBalance(prevfyearObj.GlFiscalYearId, previousPeriodTo);
                        prevYearBegBalance = _reportHelper.GetAccountBegBalance(prevfyearObj.GlFiscalYearId, previousPeriodFrom);
                    }
                }


                #region revenue
                foreach (var con in notes.Revenues)
                {
                    var connote = new GaapNoteVeiwModel
                    {
                        Description = con.Description,
                        Type = NoteType.Revenue,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<GaapNoteDetailViewModel>()
                    };
                    //get range
                    var range = operationRevenueRanges
                        .Where(x => x.GaapIncomeStatementOperationRevenueId == con.GaapIncomeStatementOperationRevenueId).ToList();
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
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(prevYearBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                                                      - _reportHelper.GetAccountBegBalancesByAccountWithAllranges(prevYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(currentYearBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                                                     - _reportHelper.GetAccountBegBalancesByAccountWithAllranges(currentYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                            };
                            connote.NoteDetailsList.Add(detail);
                        }
                    }

                    list.Add(connote);

                }
                #endregion
                #region expense
                foreach (var con in notes.Expenses)
                {
                    var connote = new GaapNoteVeiwModel
                    {
                        Description = con.Description,
                        Type = NoteType.Expense,
                        NoteNumber = con.NoteNumber,
                        NoteDetailsList = new List<GaapNoteDetailViewModel>()
                    };
                    //get range
                    var range = operationExpenseRanges
                        .Where(x => x.GaapIncomeStatementOperationExpenseId == con.GaapIncomeStatementOperationExpenseId).ToList();
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
                                PreviousYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(prevYearBalance, r.AccountFrom, r.AccountTo, accRange.Values)
                                                    - _reportHelper.GetAccountBegBalancesByAccountWithAllranges(prevYearBegBalance, r.AccountFrom, r.AccountTo, accRange.Values),
                                CurrentYearBalance = _reportHelper.GetAccountEndBalancesByAccountWithAllranges(currentYearBalance, r.AccountFrom, r.AccountTo, accRange.Values)
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
        public IncomeStatementReportVeiwModel IncomeStatementReport(string fiscalyear, string periodFrom, string periodTo)
        {
            IncomeStatementReportVeiwModel scoiReport = new IncomeStatementReportVeiwModel();

            if (!String.IsNullOrEmpty(fiscalyear))
            {
                var fyearObj = _db.GLFiscalYears.Find(int.Parse(fiscalyear));
                var gaapIncomeStatement = _db.GaapIncomeStatements.FirstOrDefault(x => x.ReportType == GaapReportType.GAAPOne);
                var prevYearBalance = new List<GLLedgerPostingLines>();
                var prevYearBegBalance = new List<GLLedgerPostingLines>();
                var currentYearBalance = new List<GLLedgerPostingLines>();
                var currentYearBegBalance = new List<GLLedgerPostingLines>();

                #region  operation revenue and expense variables

                decimal currentYearsumOperationRevenue = 0;

                decimal currentYearsumOperationExpense = 0;

                decimal previosYearsumOperationRevenue = 0;

                decimal previosYearsumOperationExpense = 0;

                decimal currentYearGrossProfit = 0;
                decimal previosYearGrossProfit = 0;

                #endregion

                #region non operation revenue and expense variables

                decimal currentYearsumNonOperationRevenue = 0;

                decimal currentYearsumNonOperationExpense = 0;

                decimal previosYearsumNonOperationRevenue = 0;

                decimal previosYearsumNonOperationExpense = 0;

                #endregion


                if (fyearObj != null && gaapIncomeStatement != null)
                {
                    int currentPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : int.Parse(periodTo);
                    int currentPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : int.Parse(periodFrom);
                    int prevyear = _fiscalYearHelper.GetPreviousFYearId(fyearObj.GlFiscalYearId);
                    var prevfyearObj = _db.GLFiscalYears.Find(prevyear);

                    currentYearBalance = _reportHelper.GetAccountEndBalance(fyearObj.GlFiscalYearId, currentPeriodTo);
                    currentYearBegBalance = _reportHelper.GetAccountBegBalance(fyearObj.GlFiscalYearId, currentPeriodFrom);
                    if (prevfyearObj != null)
                    {
                        int previousPeriodTo = String.IsNullOrEmpty(periodTo) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodTo, prevfyearObj.GlFiscalYearId);
                        int previousPeriodFrom = String.IsNullOrEmpty(periodFrom) ? 0 : _fiscalYearHelper.GetParallelPeriodInYear(currentPeriodFrom, prevfyearObj.GlFiscalYearId);
                        prevYearBalance =_reportHelper.GetAccountEndBalance(prevfyearObj.GlFiscalYearId, previousPeriodTo);
                        prevYearBegBalance = _reportHelper.GetAccountBegBalance(prevfyearObj.GlFiscalYearId, previousPeriodFrom);
                    }

                    #region  operation

                    decimal currentYearrevenueTotal = 0;
                    decimal prevYearrevenueTotal = 0;

                    decimal currentYearexpenseTotal = 0;
                    decimal prevYearexpenseTotal = 0;
                    var reportitem = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Operation Revenue",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem);

                    #region revenue 

                    var revenues = gaapIncomeStatement.Revenues
                        .Where(x => x.Category == IncomeStatementRevenueCategory.OperationalRevenue).ToList();

                    foreach (var revenue in revenues)
                    {
                        currentYearrevenueTotal = 0;
                        prevYearrevenueTotal = 0;

                        #region  account range

                        var accountRange = revenue.GaapIncomeStatementOperationRevenueRanges.ToList();
                        foreach (var range in accountRange)
                        {
                            #region current year

                            currentYearrevenueTotal = currentYearrevenueTotal 
                                +_reportHelper.GetAccountEndBalancesWithAllranges(currentYearBalance, range.AccountFrom, range.AccountTo)
                                -_reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearrevenueTotal =prevYearrevenueTotal
                                                      + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearBalance, range.AccountFrom, range.AccountTo)
                                                      - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion
                            }
                        }

                        #endregion

                      
                        currentYearsumOperationRevenue = currentYearsumOperationRevenue + currentYearrevenueTotal;
                        previosYearsumOperationRevenue = previosYearsumOperationRevenue + prevYearrevenueTotal;

                        reportitem = new IncomeStatementReportItemVeiwModel
                        {
                            Notes = revenue.NoteNumber,
                            CategoryDescription = revenue.Description,
                            CurrentYearBalance = currentYearrevenueTotal,
                            PreviousYearBalance = prevYearrevenueTotal,
                            IsHeader = false
                        };
                        scoiReport.IncomeStatementReportItems.Add(reportitem);
                    }

                    #endregion

                    var reportitem1 = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Total Operation Revenue",
                        CurrentYearBalance = currentYearsumOperationRevenue,
                        PreviousYearBalance = previosYearsumOperationRevenue,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem1);

                    var reportitem0 = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Operation Expenses",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem0);

                    #region expense 

                    var expenses = gaapIncomeStatement.Expenses
                        .Where(x => x.Category == IncomeStatementExpenseCategory.OperationalExpense).ToList();
                    foreach (var expense in expenses)
                    {
                        currentYearexpenseTotal = 0;
                        prevYearexpenseTotal = 0;


                        #region  account range

                        var accountRange = expense.GaapIncomeStatementOperationExpenseRanges.ToList();
                        foreach (var range in accountRange)
                        {
                           

                            #region current year

                            currentYearexpenseTotal = currentYearexpenseTotal 
                                                     + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearBalance, range.AccountFrom, range.AccountTo)
                                                     - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearexpenseTotal =prevYearexpenseTotal
                                                      + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearBalance, range.AccountFrom, range.AccountTo)
                                                      - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion
                            }
                        }

                        #endregion

                        
                        currentYearsumOperationExpense = currentYearsumOperationExpense + currentYearexpenseTotal;
                        previosYearsumOperationExpense = previosYearsumOperationExpense + prevYearexpenseTotal;

                        reportitem = new IncomeStatementReportItemVeiwModel
                        {
                            Notes = expense.NoteNumber,
                            CategoryDescription = expense.Description,
                            CurrentYearBalance = currentYearexpenseTotal,
                            PreviousYearBalance = prevYearexpenseTotal,
                            IsHeader = false
                        };
                        scoiReport.IncomeStatementReportItems.Add(reportitem);
                    }

                    #endregion

                    var reportitem2 = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Total Operation Expenses",
                        CurrentYearBalance = currentYearsumOperationExpense,
                        PreviousYearBalance = previosYearsumOperationExpense,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem2);

                    currentYearGrossProfit= currentYearsumOperationRevenue - currentYearsumOperationExpense;
                    previosYearGrossProfit = previosYearsumOperationRevenue - previosYearsumOperationExpense;
                    reportitem = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Gross Profit",
                        CurrentYearBalance = currentYearGrossProfit,
                        PreviousYearBalance = previosYearGrossProfit,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem);
                    #endregion
                    #region  non operation

                    
                    var reportitem3 = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Non Operation Revenue",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem3);

                    #region revenue 

                    var nonOperationRevenues = gaapIncomeStatement.Revenues
                        .Where(x => x.Category == IncomeStatementRevenueCategory.NonOperationalRevenue).ToList();

                    foreach (var revenue in nonOperationRevenues)
                    {
                        currentYearrevenueTotal = 0;
                        prevYearrevenueTotal = 0;

                        #region  account range

                        var accountRange = revenue.GaapIncomeStatementOperationRevenueRanges.ToList();
                        foreach (var range in accountRange)
                        {

                            #region current year

                            currentYearrevenueTotal = currentYearrevenueTotal
                                                      + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearBalance, range.AccountFrom, range.AccountTo)
                                                      - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearrevenueTotal =prevYearrevenueTotal 
                                                    + _reportHelper.GetAccountEndBalancesWithAllranges(prevYearBalance, range.AccountFrom, range.AccountTo)
                                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo);

                                #endregion
                            }
                        }

                        #endregion


                        currentYearsumNonOperationRevenue = currentYearsumNonOperationRevenue + currentYearrevenueTotal;
                        previosYearsumNonOperationRevenue = previosYearsumNonOperationRevenue + prevYearrevenueTotal;

                        reportitem = new IncomeStatementReportItemVeiwModel
                        {
                            Notes = revenue.NoteNumber,
                            CategoryDescription = revenue.Description,
                            CurrentYearBalance = currentYearrevenueTotal,
                            PreviousYearBalance = prevYearrevenueTotal,
                            IsHeader = false
                        };
                        scoiReport.IncomeStatementReportItems.Add(reportitem);
                    }

                    #endregion

                    var reportitem4 = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Total Non Operation Revenue",
                        CurrentYearBalance = currentYearsumNonOperationRevenue,
                        PreviousYearBalance = previosYearsumNonOperationRevenue,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem4);

                    var reportitem5 = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Non Operation Expenses",
                        CurrentYearBalance = 0,
                        PreviousYearBalance = 0,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem5);

                    #region expense 

                    var nonOperationExpenses = gaapIncomeStatement.Expenses
                        .Where(x => x.Category == IncomeStatementExpenseCategory.NonOperationalExpense).ToList();
                    foreach (var expense in nonOperationExpenses)
                    {
                        currentYearexpenseTotal = 0;
                        prevYearexpenseTotal = 0;

                        #region  account range

                        var accountRange = expense.GaapIncomeStatementOperationExpenseRanges.ToList();
                        foreach (var range in accountRange)
                        {

                            #region current year

                            currentYearexpenseTotal = currentYearexpenseTotal 
                                                        + _reportHelper.GetAccountEndBalancesWithAllranges(currentYearBalance, range.AccountFrom, range.AccountTo)
                                                        - _reportHelper.GetAccountBegBalancesWithAllranges(currentYearBegBalance, range.AccountFrom, range.AccountTo);

                            #endregion

                            if (prevfyearObj != null)
                            {
                                #region previos year

                                prevYearexpenseTotal =prevYearexpenseTotal 
                                                    +_reportHelper.GetAccountEndBalancesWithAllranges(prevYearBalance, range.AccountFrom, range.AccountTo)
                                                    - _reportHelper.GetAccountBegBalancesWithAllranges(prevYearBegBalance, range.AccountFrom, range.AccountTo); 

                                #endregion
                            }
                        }

                        #endregion


                        currentYearsumNonOperationExpense = currentYearsumNonOperationExpense + currentYearexpenseTotal;
                        previosYearsumNonOperationExpense = previosYearsumNonOperationExpense + prevYearexpenseTotal;

                        reportitem = new IncomeStatementReportItemVeiwModel
                        {
                            Notes = expense.NoteNumber,
                            CategoryDescription = expense.Description,
                            CurrentYearBalance = currentYearexpenseTotal,
                            PreviousYearBalance = prevYearexpenseTotal,
                            IsHeader = false
                        };
                        scoiReport.IncomeStatementReportItems.Add(reportitem);
                    }

                    #endregion

                    var reportitem6 = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Total Non Operation Expenses",
                        CurrentYearBalance = currentYearsumNonOperationExpense,
                        PreviousYearBalance = previosYearsumNonOperationExpense,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem6);

                    var currentTaxableIncome = currentYearGrossProfit + currentYearsumNonOperationRevenue - currentYearsumNonOperationExpense;
                    var previousTaxableIncome = previosYearGrossProfit + previosYearsumNonOperationRevenue - previosYearsumNonOperationExpense;

                    reportitem = new IncomeStatementReportItemVeiwModel
                    {
                        CategoryDescription = "Taxable Income",
                        CurrentYearBalance = currentTaxableIncome,
                        PreviousYearBalance = previousTaxableIncome,
                        IsHeader = true
                    };
                    scoiReport.IncomeStatementReportItems.Add(reportitem);

                    if (FinanceHelper.CalculateProfitTax())
                    {
                        var profittaxRate = _db.FinanceSettings.FirstOrDefault();
                        decimal rate = profittaxRate?.ProfitTaxCalculatedRate ?? 0;
                        decimal currentTax = currentTaxableIncome * rate;
                        decimal previousTax = previousTaxableIncome * rate;

                        reportitem = new IncomeStatementReportItemVeiwModel
                        {
                            CategoryDescription = "Profit Tax",
                            CurrentYearBalance = currentTax,
                            PreviousYearBalance = previousTax,
                            IsHeader = true
                        };
                        scoiReport.IncomeStatementReportItems.Add(reportitem);

                        var currentNetProfit = currentTaxableIncome - currentTax;
                        var previousNetProfit = previousTaxableIncome - previousTax;

                        reportitem = new IncomeStatementReportItemVeiwModel
                        {
                            CategoryDescription = "Profit After Tax",
                            CurrentYearBalance = currentNetProfit,
                            PreviousYearBalance = previousNetProfit,
                            IsHeader = true
                        };
                        scoiReport.IncomeStatementReportItems.Add(reportitem);
                    }

                    #endregion

                    return scoiReport;
                }
            }

            return scoiReport;
        }
        
    }
}
