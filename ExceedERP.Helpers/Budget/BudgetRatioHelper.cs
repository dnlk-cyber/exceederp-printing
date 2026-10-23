using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.Budget
{

    #region Model

    public class AccountRangeSummaryModel
    {
        public AccountRangeSummaryModel()
        {
            fiscalYear = new List<string>();
            firstvalue = new List<RatioModel>();
            secondvalue = new List<RatioModel>();
            thirdvalue = new List<RatioModel>();
            fourthvalue = new List<RatioModel>();

        }
        public List<string> fiscalYear { get; set; }
        public List<RatioModel> firstvalue { get; set; }
        public List<RatioModel> secondvalue { get; set; }
        public List<RatioModel> thirdvalue { get; set; }
        public List<RatioModel> fourthvalue { get; set; }

    }


    public class RatioSummaryModel
    {
        public decimal fiscalYear { get; set; }
        public decimal firstQ1value { get; set; }
        public decimal firstQ2value { get; set; }
        public decimal firstQ3value { get; set; }
        public decimal firstQ4value { get; set; }


        public decimal secondQ1value { get; set; }
        public decimal secondQ2value { get; set; }
        public decimal secondQ3value { get; set; }
        public decimal secondQ4value { get; set; }

        public decimal thirdQ1value { get; set; }
        public decimal thirdQ2value { get; set; }
        public decimal thirdQ3value { get; set; }
        public decimal thirdQ4value { get; set; }


        public decimal fourthQ1value { get; set; }
        public decimal fourthQ2value { get; set; }
        public decimal fourthQ3value { get; set; }
        public decimal fourthQ4value { get; set; }


    }


    public class RatioModel
    {

        public string Type { get; set; }
        public string Name { get; set; }
        public string FiscalYear { get; set; }
        public decimal Actual { get; set; }
        public decimal Budget { get; set; }
        public float IndustryAverage { get; set; }
        public string Remark { get; set; }
    }

    public class RatioAnnualSummaryModel
    {
        public RatioAnnualSummaryModel()
        {
            FirstQuarterActual = new List<RatioModel>();
            SecondQuarterActual = new List<RatioModel>();
            ThirdQuarterActual = new List<RatioModel>();
            FourthQuarterActual = new List<RatioModel>();
            IndustryAverage = new List<RatioModel>();
            Remark = new List<RatioModel>();
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public decimal CurrentBudget { get; set; }
        public List<RatioModel> FirstQuarterActual { get; set; }
        public List<RatioModel> SecondQuarterActual { get; set; }
        public List<RatioModel> ThirdQuarterActual { get; set; }
        public List<RatioModel> FourthQuarterActual { get; set; }
        public List<RatioModel> IndustryAverage { get; set; }
        public List<RatioModel> Remark { get; set; }


    }
    #endregion



    public class BudgetRatioHelper
    {
        private readonly ReportHelper reportHelper;
        public BudgetRatioHelper()
        {
            reportHelper = new ReportHelper();
        }

        #region Ratio Per Quarter
        public List<RatioModel> ActivityRatios(int fiscalYear, int periodfrom, int periodto, List<FinancialRatio> list, List<GLLedgerPostingLines> balance = null)
        {

            using (ExceedDbContext _db = new ExceedDbContext())
            {
                BudgetReportHelper _ReportHelper = new BudgetReportHelper();
                BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
                List<RatioModel> RatioList = new List<RatioModel>();
                string Type = "ACTIVITY RATIO";

                List<GLLedgerPostingLines> QuarterBalance = new List<GLLedgerPostingLines>();
                if (balance == null || balance.Any())
                    QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);

                foreach (var k in list)
                {
                    switch (k.Name)
                    {
                        case "INVENTORY_TURN_OVER_RATIO":

                            decimal InventoryBalance = decimal.Zero;
                            decimal COSTOFGOODSOLD = decimal.Zero;
                            var INVENTORYRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.INVENTORY).FirstOrDefault();
                            var COSTOFGOODSOLDRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.COST_OF_GOOD_SOLD).FirstOrDefault();

                            if (INVENTORYRanges != null)
                            {
                                InventoryBalance = _ReportHelper.GetAccountRangeBalance(INVENTORYRanges, QuarterBalance);
                            }

                            if (COSTOFGOODSOLDRanges != null)
                            {
                                COSTOFGOODSOLD = _ReportHelper.GetAccountRangeBalance(COSTOFGOODSOLDRanges, QuarterBalance);
                            }

                            decimal Inventory_TurnOver = COSTOFGOODSOLD / InventoryBalance.DivideByZeroHelper();

                            var Inventory_TurnOvers = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Inventory Turn Over",
                                Actual = Inventory_TurnOver,
                                IndustryAverage = k.Average

                            };

                            RatioList.Add(Inventory_TurnOvers);
                            break;
                        case "ACCOUNT_RECEIVABLE_DAYS":

                            var accountReceivable = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Account Receivable Days",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };

                            RatioList.Add(accountReceivable);
                            break;
                        case "ACCOUNT_PAYABLE_DAYS":

                            var accountPayable = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Account Payable Days",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };

                            RatioList.Add(accountPayable);
                            break;
                        case "STOCK_DAYS":

                            var stockDays = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Stock Days",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };

                            RatioList.Add(stockDays);

                            break;
                    }
                }

                return RatioList;
            }
        }
        public List<RatioModel> LiquidityRatios(int fiscalYear, int periodfrom, int periodto, List<FinancialRatio> list, List<GLLedgerPostingLines> balance = null)
        {

            using (ExceedDbContext _db = new ExceedDbContext())
            {
                BudgetReportHelper _ReportHelper = new BudgetReportHelper();
                BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
                List<RatioModel> RatioList = new List<RatioModel>();

                string Type = "LIQUIDITY RATIO";
                List<GLLedgerPostingLines> QuarterBalance = new List<GLLedgerPostingLines>();
                if (balance == null || balance.Any())
                    QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);
                foreach (var k in list)
                {
                    switch (k.Name)
                    {
                        case "CURRENT_RATIO":
                            decimal CURRENT_LIABLITIESBalance = decimal.Zero;
                            decimal CURRENT_ASSETBalance = decimal.Zero;
                            var CURRENT_ASSETRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.CURRENT_ASSET).FirstOrDefault();
                            var CURRENT_LIABLITIESRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.CURRENT_LIABLITIES).FirstOrDefault();


                            if (CURRENT_ASSETRanges != null)
                            {
                                CURRENT_ASSETBalance = _ReportHelper.GetAccountRangeBalance(CURRENT_ASSETRanges, QuarterBalance);
                            }

                            if (CURRENT_LIABLITIESRanges != null)
                            {
                                CURRENT_LIABLITIESBalance = _ReportHelper.GetAccountRangeBalance(CURRENT_LIABLITIESRanges, QuarterBalance);
                            }

                            decimal CurrentRatio = CURRENT_ASSETBalance / CURRENT_LIABLITIESBalance.DivideByZeroHelper();

                            var current = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Current Ratio",
                                Actual = CurrentRatio,
                                IndustryAverage = k.Average

                            };

                            RatioList.Add(current);
                            break;
                        case "QUICK_RATIO":

                            decimal INVENTORYBalance = decimal.Zero;
                            decimal CURRENTLIABLITIEBalance = decimal.Zero;
                            decimal CURRENTASSETBalance = decimal.Zero;

                            var CURRENTASSETRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.CURRENT_ASSET).FirstOrDefault();
                            var CURRENTLIABLITIERanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.CURRENT_LIABLITIES).FirstOrDefault();
                            var INVENTORYRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.INVENTORY).FirstOrDefault();


                            if (CURRENTASSETRanges != null)
                            {
                                CURRENTASSETBalance = _ReportHelper.GetAccountRangeBalance(CURRENTASSETRanges, QuarterBalance);
                            }

                            if (CURRENTLIABLITIERanges != null)
                            {
                                CURRENTLIABLITIEBalance = _ReportHelper.GetAccountRangeBalance(CURRENTLIABLITIERanges, QuarterBalance);
                            }

                            if (INVENTORYRanges != null)
                            {
                                INVENTORYBalance = _ReportHelper.GetAccountRangeBalance(INVENTORYRanges, QuarterBalance);
                            }

                            decimal Quick_Ratio = (CURRENTASSETBalance - INVENTORYBalance) / CURRENTLIABLITIEBalance.DivideByZeroHelper();

                            var quick = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Quick Ratio",
                                Actual = Quick_Ratio,
                                IndustryAverage = k.Average

                            };

                            RatioList.Add(quick);
                            break;
                        case "LIQUIDITY_RATIO":

                            decimal liquidAssetBalance = decimal.Zero;
                            decimal depositBalance = decimal.Zero;

                            var liquidASSETRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.LIQUID_ASSETS).FirstOrDefault();
                            var TotalDepositRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.TOTAL_DEPOSITS).FirstOrDefault();


                            if (liquidASSETRanges != null)
                            {
                                liquidAssetBalance = _ReportHelper.GetAccountRangeBalance(liquidASSETRanges, QuarterBalance);
                            }

                            if (TotalDepositRanges != null)
                            {
                                depositBalance = _ReportHelper.GetAccountRangeBalance(TotalDepositRanges, QuarterBalance);
                            }

                            decimal liquidity_ratio = (liquidAssetBalance) / depositBalance.DivideByZeroHelper();

                            var liquidity = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Liquidity Ratio",
                                Actual = liquidity_ratio,
                                IndustryAverage = k.Average

                            };

                            RatioList.Add(liquidity);
                            break;
                    }
                }

                return RatioList;
            }
        }
        public List<RatioModel> EfficiencyRatios(int fiscalYear, int periodfrom, int periodto, List<FinancialRatio> list, List<GLLedgerPostingLines> balance = null)
        {

            using (ExceedDbContext _db = new ExceedDbContext())
            {
                BudgetReportHelper _ReportHelper = new BudgetReportHelper();
                BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
                var RatioList = new List<RatioModel>();
                string Type = "EFFICIENCY RATIO";

                List<GLLedgerPostingLines> QuarterBalance = new List<GLLedgerPostingLines>();
                if (balance == null || balance.Any())
                    QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);
                foreach (var k in list)
                {
                    switch (k.Name)
                    {
                        case "ASSET_TURN_OVER_RATIO":
                            decimal TOTAL_CurAssetBalance = decimal.Zero;
                            decimal TOTAL_FixeAssetBalance = decimal.Zero;
                            decimal SaleBalance = decimal.Zero;

                            var TOTAL_CuAssetRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.CURRENT_ASSET).FirstOrDefault();
                            var TOTAL_FixeAssetRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.FIXED_ASSETS).FirstOrDefault();
                            var SaleRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.SALES).FirstOrDefault();

                            if (TOTAL_CuAssetRanges != null)
                            {
                                TOTAL_CurAssetBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_CuAssetRanges, QuarterBalance);
                            }

                            if (TOTAL_FixeAssetRanges != null)
                            {
                                TOTAL_FixeAssetBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_FixeAssetRanges, QuarterBalance);
                            }

                            if (SaleRanges != null)
                            {
                                SaleBalance = _ReportHelper.GetAccountRangeBalance(SaleRanges, QuarterBalance);
                            }

                            decimal Asset_Turn_Over_Ratio = SaleBalance / (TOTAL_CurAssetBalance + TOTAL_FixeAssetBalance).DivideByZeroHelper();

                            var Asset_Turn_Over_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Asset Turn Over Ratio",
                                Actual = Asset_Turn_Over_Ratio

                            };
                            RatioList.Add(Asset_Turn_Over_Ratios);
                            break;
                        case "FIXEDASSET_TURN_OVER_RATIO":
                            decimal TOTAL_FixedAssetBalance = decimal.Zero;
                            decimal TOTAL_SaleBalance = decimal.Zero;

                            var TOTAL_FixedAssetRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.FIXED_ASSETS).FirstOrDefault();
                            var TOTAL_SaleRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.SALES).FirstOrDefault();

                            if (TOTAL_FixedAssetRanges != null)
                            {
                                TOTAL_FixedAssetBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_FixedAssetRanges, QuarterBalance);
                            }

                            if (TOTAL_SaleRanges != null)
                            {
                                TOTAL_SaleBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_SaleRanges, QuarterBalance);
                            }

                            decimal Fixed_Asset_Turn_Over_Ratio = TOTAL_SaleBalance / TOTAL_FixedAssetBalance.DivideByZeroHelper();

                            var Fixed_Asset_Turn_Over_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Fixed Asset Turn Over Ratio",
                                Actual = Fixed_Asset_Turn_Over_Ratio

                            };
                            RatioList.Add(Fixed_Asset_Turn_Over_Ratios);

                            break;
                        case "WORKING_CAPITAL_SALES_RATIO":
                            decimal TOTAL_CurreAssetBalance = decimal.Zero;
                            decimal TOTAL_CurrLiabilityBalance = decimal.Zero;
                            decimal TOTAL_SalesBalance = decimal.Zero;

                            var TOTAL_CurrAssetRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.CURRENT_ASSET).FirstOrDefault();
                            var TOTAL_CurrLiabilityRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.CURRENT_LIABLITIES).FirstOrDefault();
                            var TOTAL_SalesRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.SALES).FirstOrDefault();

                            if (TOTAL_CurrAssetRanges != null)
                            {
                                TOTAL_CurreAssetBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_CurrAssetRanges, QuarterBalance);
                            }

                            if (TOTAL_CurrLiabilityRanges != null)
                            {
                                TOTAL_CurrLiabilityBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_CurrLiabilityRanges, QuarterBalance);
                            }

                            if (TOTAL_SalesRanges != null)
                            {
                                TOTAL_SalesBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_SalesRanges, QuarterBalance);
                            }

                            decimal Capital_Sales_Ratio = ((TOTAL_CurreAssetBalance - TOTAL_CurrLiabilityBalance) / (TOTAL_SalesBalance).DivideByZeroHelper()) * 100;

                            var Capital_Sales_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Working Capital to Sales Ratio %",
                                Actual = Capital_Sales_Ratio

                            };
                            RatioList.Add(Capital_Sales_Ratios);

                            break;
                        case "COST_OF_GOOD_SOLD":
                            var costofGoodSold = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Cost of good sold",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(costofGoodSold);

                            break;
                        case "ACCOUNT_PAYABLE_TURN_OVER_RATIO":
                            var accountPayableRatio = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Account payable turn over ration",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(accountPayableRatio);

                            break;
                        case "ACCOUNT_RECEIVABLE_TURN_OVER_RATIO":

                            var accountReceivableRatio = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Account receivable turn over ration",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(accountReceivableRatio);
                            break;
                    }
                }

                return RatioList;
            }
        }
        public List<RatioModel> ProfitabilityRatios(int fiscalYear, int periodfrom, int periodto, List<FinancialRatio> list, List<GLLedgerPostingLines> balance = null)
        {

            using (ExceedDbContext _db = new ExceedDbContext())
            {
                BudgetReportHelper _ReportHelper = new BudgetReportHelper();
                BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
                var RatioList = new List<RatioModel>();
                string Type = "PROFITABILITY RATIO";

                List<GLLedgerPostingLines> QuarterBalance = new List<GLLedgerPostingLines>();
                if (balance == null || balance.Any())
                    QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);
                foreach (var k in list)
                {
                    switch (k.Name)
                    {
                        case "GROSS_PROFIT":
                            var grossPrfoit = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Gross profit",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(grossPrfoit);
                            break;
                        case "OVERHEAD_RATIO":
                            decimal TOTAL_OverheadBalance = decimal.Zero;
                            decimal TOTAL_SaleBalance = decimal.Zero;

                            var TOTAL_OverheadRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.OVERHEAD).FirstOrDefault();
                            var TOTAL_SaleRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.SALES).FirstOrDefault();

                            if (TOTAL_OverheadRanges != null)
                            {
                                TOTAL_OverheadBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_OverheadRanges, QuarterBalance);
                            }

                            if (TOTAL_SaleRanges != null)
                            {
                                TOTAL_SaleBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_SaleRanges, QuarterBalance);
                            }

                            decimal Overhead_Ratio = (TOTAL_OverheadBalance / TOTAL_SaleBalance.DivideByZeroHelper()) * 100;

                            var Overhead_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Overhead Ratio",
                                Actual = Overhead_Ratio,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(Overhead_Ratios);

                            break;
                        case "RETURN_ON_SALES":
                            decimal TOTAL_OperatingIncomeBalance = decimal.Zero;
                            decimal TOTAL_SalesBalance = decimal.Zero;

                            var TOTAL_OperatingIncRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.OPERATING_INCOME).FirstOrDefault();
                            var TOTAL_SalesRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.SALES).FirstOrDefault();

                            if (TOTAL_OperatingIncRanges != null)
                            {
                                TOTAL_OperatingIncomeBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_OperatingIncRanges, QuarterBalance);
                            }

                            if (TOTAL_SalesRanges != null)
                            {
                                TOTAL_SalesBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_SalesRanges, QuarterBalance);
                            }

                            decimal Return_Sales_Ratio = (TOTAL_OperatingIncomeBalance / TOTAL_SalesBalance.DivideByZeroHelper()) * 100;

                            var Return_Sales_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Return on Sales Ratio %",
                                Actual = Return_Sales_Ratio,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(Return_Sales_Ratios);

                            break;
                        case "NET_PROFIT_RATIO":

                            var netPrfofitRation = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Net profit ratio",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(netPrfofitRation);
                            break;
                        case "RETURN_ON_CAPITAL_EMPLOYED":
                            decimal OperatingIncomeBalance = decimal.Zero;
                            decimal TOTAL_FixedAssetBalance = decimal.Zero;

                            var OperatingIncRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.OPERATING_INCOME).FirstOrDefault();
                            var TOTAL_FixedAssetRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.FIXED_ASSETS).FirstOrDefault();

                            if (OperatingIncRanges != null)
                            {
                                OperatingIncomeBalance = _ReportHelper.GetAccountRangeBalance(OperatingIncRanges, QuarterBalance);
                            }

                            if (TOTAL_FixedAssetRanges != null)
                            {
                                TOTAL_FixedAssetBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_FixedAssetRanges, QuarterBalance);
                            }

                            decimal Return_Capital_Ratio = (OperatingIncomeBalance / TOTAL_FixedAssetBalance.DivideByZeroHelper()) * 100;

                            var Return_Capital_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Return on Capital Ratio %",
                                Actual = Return_Capital_Ratio,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(Return_Capital_Ratios);

                            break;

                    }
                }

                return RatioList;
            }

        }
        public List<RatioModel> InvestorRatios(int fiscalYear, int periodfrom, int periodto, List<FinancialRatio> list, List<GLLedgerPostingLines> balance = null)
        {

            using (ExceedDbContext _db = new ExceedDbContext())
            {
                BudgetReportHelper _ReportHelper = new BudgetReportHelper();
                BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
                var RatioList = new List<RatioModel>();
                string Type = "INVESTOR RATIO";
                List<GLLedgerPostingLines> QuarterBalance = new List<GLLedgerPostingLines>();
                if (balance == null || balance.Any())
                    QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);


                decimal Net_income = reportHelper.GetNetEarningByPeriodRange(periodfrom, periodto);

                foreach (var k in list)
                {
                    switch (k.Name)
                    {
                        case "RETURN_ON_EQUITY":


                            decimal TOTAL_Equity = reportHelper.GetAverageEquityByPeriodRange(periodfrom, periodto);

                            decimal return_on_equity = (Net_income / TOTAL_Equity.DivideByZeroHelper());

                            var returnOnEquity = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Return on equity",
                                Actual = return_on_equity,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(returnOnEquity);
                            break;
                        case "RETURN_ON_INVESTMENT":

                            decimal TOTAL_Investment = 0;
                            var InvestmentRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.INVESTMENT).FirstOrDefault();

                            if (InvestmentRanges != null)
                            {
                                TOTAL_Investment = _ReportHelper.GetAccountRangeBalance(InvestmentRanges, QuarterBalance);
                            }

                            decimal return_on_investment = (Net_income / TOTAL_Investment.DivideByZeroHelper());
                            var returnOnInvestment = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Return on investment",
                                Actual = return_on_investment,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(returnOnInvestment);
                            break;
                        case "MARKET_VALUE":

                            var marketValue = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Market Value",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(marketValue);
                            break;
                        case "MARKET_CAPITALIZATION":

                            var marketCapitalization = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Market Capitalization",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(marketCapitalization);
                            break;
                        case "EARNINGS_PER_SHARE":

                            decimal TOTAL_Shares = 5;

                            decimal earning_per_share = (Net_income / TOTAL_Shares.DivideByZeroHelper());
                            var earningPerShare = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Earning per share",
                                Actual = earning_per_share,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(earningPerShare);

                            break;
                        case "DIVIDEND_YIELD":
                            var dividendYield = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Dividend yield",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(dividendYield);

                            break;
                        case "PRICE_TO_BOOK_RATIO":

                            var priceToBook = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Price to book ratio",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(priceToBook);
                            break;
                        case "PRICE_TO_EARNING_RATIO":

                            var priceToEarning = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Price to earning ratio",
                                Actual = 0,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(priceToEarning);
                            break;
                        case "EQUITY_RATIO":
                            decimal ShareholderBalance = decimal.Zero;
                            decimal TOTAL_AssetBalance = decimal.Zero;

                            var ShareholderEquityRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.TOTAL_SHAREHOLDERS_EQUITY).FirstOrDefault();
                            var TOTAL_AssetRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.TOTAL_ASSETS).FirstOrDefault();

                            if (ShareholderEquityRanges != null)
                            {
                                ShareholderBalance = _ReportHelper.GetAccountRangeBalance(ShareholderEquityRanges, QuarterBalance);
                            }

                            if (TOTAL_AssetRanges != null)
                            {
                                TOTAL_AssetBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_AssetRanges, QuarterBalance);
                            }

                            decimal Return_Capital_Ratio = (ShareholderBalance / TOTAL_AssetBalance.DivideByZeroHelper()) * 100;

                            var Return_Capital_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Equity Ratio %",
                                Actual = Return_Capital_Ratio,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(Return_Capital_Ratios);

                            break;
                    }
                }

                return RatioList;
            }
        }
        public List<RatioModel> LeverageRatios(int fiscalYear, int periodfrom, int periodto, List<FinancialRatio> list, List<GLLedgerPostingLines> balance = null)
        {

            using (ExceedDbContext _db = new ExceedDbContext())
            {
                BudgetReportHelper _ReportHelper = new BudgetReportHelper();
                BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
                var RatioList = new List<RatioModel>();

                string Type = "LEVERAGE RATIO";

                List<GLLedgerPostingLines> QuarterBalance = new List<GLLedgerPostingLines>();
                if (balance == null || balance.Any())
                    QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);

                foreach (var k in list)
                {
                    switch (k.Name)
                    {
                        case "GEARING_RATIO":
                            decimal TOTAL_EQUITYBalance = decimal.Zero;
                            decimal TOTAL_DebtBalance = decimal.Zero;

                            var TOTAL_DebtRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.TOTAL_DEBT).FirstOrDefault();
                            var TOTAL_EQUITYRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.TOTAL_SHAREHOLDERS_EQUITY).FirstOrDefault();

                            if (TOTAL_DebtRanges != null)
                            {
                                TOTAL_DebtBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_DebtRanges, QuarterBalance);
                            }

                            if (TOTAL_EQUITYRanges != null)
                            {
                                TOTAL_EQUITYBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_EQUITYRanges, QuarterBalance);
                            }

                            decimal Gearing_Ratio = TOTAL_DebtBalance / (TOTAL_DebtBalance + TOTAL_EQUITYBalance).DivideByZeroHelper();

                            var Gearing_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Gearing Ratio",
                                Actual = Gearing_Ratio,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(Gearing_Ratios);
                            break;
                        case "DEBT_EQUITY_RATIO":
                            decimal TOTAL_SHAREHOLDERS_EQUITYBalance = decimal.Zero;
                            decimal TOTAL_DEBTBalance = decimal.Zero;

                            var TOTAL_DEBTRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.TOTAL_DEBT).FirstOrDefault();
                            var TOTAL_SHAREHOLDERS_EQUITYRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.TOTAL_SHAREHOLDERS_EQUITY).FirstOrDefault();

                            if (TOTAL_DEBTRanges != null)
                            {
                                TOTAL_DEBTBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_DEBTRanges, QuarterBalance);
                            }

                            if (TOTAL_SHAREHOLDERS_EQUITYRanges != null)
                            {
                                TOTAL_SHAREHOLDERS_EQUITYBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_SHAREHOLDERS_EQUITYRanges, QuarterBalance);
                            }

                            decimal Debt_To_Equity_Ratio = TOTAL_DEBTBalance / TOTAL_SHAREHOLDERS_EQUITYBalance.DivideByZeroHelper();

                            var Debt_To_Equity_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Debt To Equity Ratio",
                                Actual = Debt_To_Equity_Ratio,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(Debt_To_Equity_Ratios);
                            break;
                        case "INTEREST_COVERAGE_RATIO":
                            decimal TOTAL_OperatingIncomeBalance = decimal.Zero;
                            decimal TOTAL_InterestBalance = decimal.Zero;

                            var TOTAL_InterestRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.INTEREST).FirstOrDefault();
                            var TOTAL_OperatingIncomeRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.OPERATING_INCOME).FirstOrDefault();

                            if (TOTAL_InterestRanges != null)
                            {
                                TOTAL_InterestBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_InterestRanges, QuarterBalance);
                            }

                            if (TOTAL_OperatingIncomeRanges != null)
                            {
                                TOTAL_OperatingIncomeBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_OperatingIncomeRanges, QuarterBalance);
                            }

                            decimal Interest_Ratio = TOTAL_OperatingIncomeBalance / TOTAL_InterestBalance.DivideByZeroHelper();

                            var Interest_Coverage_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Interest Coverage Ratio",
                                Actual = Interest_Ratio

                            };
                            RatioList.Add(Interest_Coverage_Ratios);

                            break;
                        case "DEBT_RATIO":
                            decimal TOTAL_ASSETBalance = decimal.Zero;
                            decimal TOTAL_DBalance = decimal.Zero;

                            var TOTAL_DRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.TOTAL_DEBT).FirstOrDefault();
                            var TOTAL_ASSETRanges = _db.FinanceDashboards.Where(x => x.Category == FinanceDashboradCategory.TOTAL_ASSETS).FirstOrDefault();

                            if (TOTAL_DRanges != null)
                            {
                                TOTAL_DBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_DRanges, QuarterBalance);
                            }

                            if (TOTAL_ASSETRanges != null)
                            {
                                TOTAL_ASSETBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_ASSETRanges, QuarterBalance);
                            }

                            decimal Debt_Ratio = TOTAL_DBalance / TOTAL_ASSETBalance.DivideByZeroHelper();

                            var Debt_Ratios = new RatioModel
                            {
                                Type = Type,
                                Budget = 0,
                                Name = "Debt Ratio",
                                Actual = Debt_Ratio,
                                IndustryAverage = k.Average

                            };
                            RatioList.Add(Debt_Ratios);

                            break;
                    }
                }

                return RatioList;
            }

        }

        public IEnumerable<RatioModel> CapitalStructureRatios(int fiscalYear, int periodFrom, int periodTo)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RatioModel> ActivityRatios(int fiscalYear, int periodFrom, int periodTo)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RatioModel> LiquidityRatios(int fiscalYear, int periodFrom, int periodTo)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RatioModel> ProfitabilityRatios(int fiscalYear, int periodFrom, int periodTo)
        {
            throw new NotImplementedException();
        }
        #endregion




    }

}
