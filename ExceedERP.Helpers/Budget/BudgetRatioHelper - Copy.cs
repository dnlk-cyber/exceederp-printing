using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
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
        public string IndustryAverage { get; set; }
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

        #region Ratio Per Quarter
        public List<RatioModel> ActivityRatios(int fiscalYear, int periodfrom, int periodto)
        {

            ExceedDbContext _db = new ExceedDbContext();
            BudgetReportHelper _ReportHelper = new BudgetReportHelper();
            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();
            BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
            List<RatioModel> RatioList = new List<RatioModel>();
            string Type = "ACTIVITY RATIO";
            decimal currentBudgetPlan = decimal.Zero;



            List<GLLedgerPostingLines> QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);



            decimal InventoryBalance = decimal.Zero;
            decimal COSTOFGOODSOLD = decimal.Zero;
            decimal RECEIVABLE_TURN_OVERBalance = decimal.Zero;
            decimal SALESBalance = decimal.Zero;
            decimal FIXED_ASSETSBalance = decimal.Zero;
            decimal RECEIVABLESBalance = decimal.Zero;
            decimal SALESONCREDITBalance = decimal.Zero;



            var INVENTORYRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.INVENTORY).FirstOrDefault();
            var COSTOFGOODSOLDRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.COST_OF_GOOD_SOLD).FirstOrDefault();


            if (INVENTORYRanges != null && COSTOFGOODSOLDRanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(INVENTORYRanges, fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(COSTOFGOODSOLDRanges , fiscalYear, periodfrom, periodto);
            }


            if (INVENTORYRanges != null)
            {
                InventoryBalance = _ReportHelper.GetAccountRangeBalance(INVENTORYRanges , QuarterBalance);
            }

            if (COSTOFGOODSOLDRanges != null)
            {
                COSTOFGOODSOLD = _ReportHelper.GetAccountRangeBalance(COSTOFGOODSOLDRanges , QuarterBalance);
            }

            decimal Inventory_TurnOver = COSTOFGOODSOLD / InventoryBalance.DivideByZeroHelper();




            var SALESONCREDITRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.SALES_ON_CREDIT).FirstOrDefault();
            var RECEIVABLESRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.RECEIVABLES).FirstOrDefault();




            if (SALESONCREDITRanges != null)
            {
                SALESONCREDITBalance = _ReportHelper.GetAccountRangeBalance(SALESONCREDITRanges , QuarterBalance);
            }

            if (RECEIVABLESRanges != null)
            {
                RECEIVABLESBalance = _ReportHelper.GetAccountRangeBalance(RECEIVABLESRanges , QuarterBalance);
            }

            decimal Receivables_TurnOver = SALESONCREDITBalance / RECEIVABLESBalance.DivideByZeroHelper();

            var Inventory_TurnOvers = new RatioModel
            {
                Type = Type,
                Budget = currentBudgetPlan,
                Name = "Inventory TurnOver",
                Actual = Inventory_TurnOver

            };

            RatioList.Add(Inventory_TurnOvers);


            var SALESRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.SALES).FirstOrDefault();
            var FIXED_ASSETSRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.FIXED_ASSETS).FirstOrDefault();

            if (SALESONCREDITRanges != null && RECEIVABLESRanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(SALESONCREDITRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(RECEIVABLESRanges , fiscalYear, periodfrom, periodto);
            }
            if (SALESRanges != null)
            {
                SALESBalance = _ReportHelper.GetAccountRangeBalance(SALESRanges , QuarterBalance);
            }

            if (FIXED_ASSETSRanges != null)
            {
                FIXED_ASSETSBalance = _ReportHelper.GetAccountRangeBalance(FIXED_ASSETSRanges , QuarterBalance);
            }

            decimal Fixed_Asset_Turnover = SALESBalance / FIXED_ASSETSBalance.DivideByZeroHelper();




            decimal DaysInYear = 365;
            var RECEIVABLE_TURN_OVERRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.RECEIVABLE_TURN_OVER).FirstOrDefault();



            if (RECEIVABLE_TURN_OVERRanges != null)
            {
                RECEIVABLE_TURN_OVERBalance = _ReportHelper.GetAccountRangeBalance(RECEIVABLE_TURN_OVERRanges , QuarterBalance);
            }

            decimal Average_Collection_Period = DaysInYear / RECEIVABLE_TURN_OVERBalance.DivideByZeroHelper();

            var Receivables_TurnOverRatio = new RatioModel
            {
                Type = Type,
                Budget = currentBudgetPlan,
                Name = "Receivables TurnOver",

                Actual = Receivables_TurnOver

            };

            currentBudgetPlan = decimal.Zero; RatioList.Add(Receivables_TurnOverRatio);

            if (FIXED_ASSETSRanges != null && SALESRanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(SALESRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(FIXED_ASSETSRanges , fiscalYear, periodfrom, periodto);

            }

            var Fixed_Asset_Turnovers = new RatioModel
            {
                Type = Type,
                Budget = currentBudgetPlan,
                Name = "Fixed Asset Turnover",
                Actual = Fixed_Asset_Turnover

            };

            RatioList.Add(Fixed_Asset_Turnovers);


            var Asset_TurnOver = new RatioModel
            {
                Type = Type,
                Budget = currentBudgetPlan,
                Name = "Asset TurnOver",
                Actual = Fixed_Asset_Turnover

            };
            RatioList.Add(Asset_TurnOver);

            if (RECEIVABLE_TURN_OVERRanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(RECEIVABLE_TURN_OVERRanges , fiscalYear, periodfrom, periodto);
            }

            var Average_Collection_Periods = new RatioModel
            {
                Type = Type,
                Budget = currentBudgetPlan,
                Name = "Average Collection Period",
                Actual = Average_Collection_Period

            };

            RatioList.Add(Average_Collection_Periods);

            return RatioList;
        }




        public List<RatioModel> LiquidityRatios(int fiscalYear, int periodfrom, int periodto)
        {

            ExceedDbContext _db = new ExceedDbContext();
            BudgetReportHelper _ReportHelper = new BudgetReportHelper();
            FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();
            List<RatioModel> RatioList = new List<RatioModel>();

            string Type = "LIQUIDITY RATIO";
            decimal currentBudgetPlan = decimal.Zero;


            List<GLLedgerPostingLines> QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);


            decimal SALESRangesBalance = decimal.Zero;
            decimal CASHBalance = decimal.Zero;
            decimal SHORT_TERM_MARKETABLE_SECURITIESBalance = decimal.Zero;
            decimal CURRENT_LIABLITIEBalance = decimal.Zero;
            decimal INVENTORYBalance = decimal.Zero;
            decimal CURRENTLIABLITIEBalance = decimal.Zero;
            decimal CURRENTASSETBalance = decimal.Zero;
            decimal CURRENT_LIABLITIESBalance = decimal.Zero;
            decimal CURRENT_ASSETBalance = decimal.Zero;



            var CURRENT_ASSETRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.CURRENT_ASSET).FirstOrDefault();
            var CURRENT_LIABLITIESRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.CURRENT_LIABLITIES).FirstOrDefault();


            if (CURRENT_ASSETRanges != null)
            {
                CURRENT_ASSETBalance = _ReportHelper.GetAccountRangeBalance(CURRENT_ASSETRanges , QuarterBalance);
            }

            if (CURRENT_LIABLITIESRanges != null)
            {
                CURRENT_LIABLITIESBalance = _ReportHelper.GetAccountRangeBalance(CURRENT_LIABLITIESRanges , QuarterBalance);
            }

            decimal CurrentRatio = CURRENT_ASSETBalance / CURRENT_LIABLITIESBalance.DivideByZeroHelper();







            var CURRENTASSETRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.CURRENT_ASSET).FirstOrDefault();
            var CURRENTLIABLITIERanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.CURRENT_LIABLITIES).FirstOrDefault();
            var INVENTORYRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.INVENTORY).FirstOrDefault();





            if (CURRENTASSETRanges != null)
            {
                CURRENTASSETBalance = _ReportHelper.GetAccountRangeBalance(CURRENTASSETRanges , QuarterBalance);
            }

            if (CURRENTLIABLITIERanges != null)
            {
                CURRENTLIABLITIEBalance = _ReportHelper.GetAccountRangeBalance(CURRENTLIABLITIERanges , QuarterBalance);
            }

            if (INVENTORYRanges != null)
            {
                INVENTORYBalance = _ReportHelper.GetAccountRangeBalance(INVENTORYRanges , QuarterBalance);
            }

            decimal Quick_Ratio = (CURRENTASSETBalance - INVENTORYBalance) / CURRENTLIABLITIEBalance.DivideByZeroHelper();


            var CASHRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.CASH).FirstOrDefault();
            var SHORT_TERM_MARKETABLE_SECURITIESRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.SHORT_TERM_MARKETABLE_SECURITIES).FirstOrDefault();
            var CURRENT_LIABLITIERanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.CURRENT_LIABLITIES).FirstOrDefault();



            if (CURRENT_LIABLITIERanges != null)
            {
                CURRENT_LIABLITIEBalance = _ReportHelper.GetAccountRangeBalance(CURRENT_LIABLITIERanges , QuarterBalance);
            }

            if (SHORT_TERM_MARKETABLE_SECURITIESRanges != null)
            {
                SHORT_TERM_MARKETABLE_SECURITIESBalance = _ReportHelper.GetAccountRangeBalance(SHORT_TERM_MARKETABLE_SECURITIESRanges , QuarterBalance);
            }

            if (CASHRanges != null)
            {
                CASHBalance = _ReportHelper.GetAccountRangeBalance(CASHRanges , QuarterBalance);
            }

            decimal Cash_Ratio = (CASHBalance + SHORT_TERM_MARKETABLE_SECURITIESBalance) / CURRENT_LIABLITIEBalance.DivideByZeroHelper();





            var SALESRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.SALES).FirstOrDefault();

            if (SALESRanges != null)
            {
                SALESRangesBalance = _ReportHelper.GetAccountRangeBalance(SALESRanges , QuarterBalance);
            }

            decimal Net_Working_Capital_Ratios = (CURRENT_ASSETBalance - CURRENTLIABLITIEBalance) / SALESRangesBalance.DivideByZeroHelper();



            if (CURRENT_LIABLITIESRanges != null && CURRENT_ASSETRanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(CURRENT_LIABLITIESRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(CURRENT_ASSETRanges , fiscalYear, periodfrom, periodto);
            }

            var Ratio = new RatioModel
            {
                Name = "Current Ratio",
                Type = Type,
                Budget = currentBudgetPlan,
                Actual = CurrentRatio
            };
            currentBudgetPlan = decimal.Zero; RatioList.Add(Ratio);

            if (CURRENTASSETRanges != null && INVENTORYRanges != null && CURRENTLIABLITIERanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(CURRENTASSETRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(CURRENTLIABLITIERanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(INVENTORYRanges, fiscalYear, periodfrom, periodto);
            }

            Ratio = new RatioModel
            {
                Name = "Quick Ratio",
                Type = Type,
                Budget = currentBudgetPlan,
                Actual = Quick_Ratio
            };
            currentBudgetPlan = decimal.Zero; RatioList.Add(Ratio);

            if (SHORT_TERM_MARKETABLE_SECURITIESRanges != null && CURRENT_LIABLITIERanges != null && CASHRanges != null)
            {

                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(CASHRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(SHORT_TERM_MARKETABLE_SECURITIESRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(CURRENT_LIABLITIERanges , fiscalYear, periodfrom, periodto);

            }


            var CashRatio = new RatioModel
            {
                Type = Type,
                Budget = currentBudgetPlan,
                Name = "Cash Ratio",
                Actual = Cash_Ratio,
            };
            currentBudgetPlan = decimal.Zero; RatioList.Add(CashRatio);


            if (SALESRanges != null && CURRENTLIABLITIERanges != null && CURRENT_ASSETRanges != null)
            {


                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(SALESRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(CURRENTLIABLITIERanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(CURRENT_ASSETRanges , fiscalYear, periodfrom, periodto);

            }



            var Net_Working_Capital_Ratio = new RatioModel
            {
                Type = Type,
                Budget = currentBudgetPlan,
                Name = "Net Working Capital Ratio",
                Actual = Net_Working_Capital_Ratios,


            };
            currentBudgetPlan = decimal.Zero; RatioList.Add(Net_Working_Capital_Ratio);

            return RatioList;
        }



        public List<RatioModel> CapitalStructureRatios(int fiscalYear, int periodfrom, int periodto)
        {

            ExceedDbContext _db = new ExceedDbContext();
            BudgetReportHelper _ReportHelper = new BudgetReportHelper();
            FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();
            var RatioList = new List<RatioModel>();
            string Type = "CAPITAL STRUCTURE RATIO";
            decimal currentBudgetPlan = decimal.Zero;


            List<GLLedgerPostingLines> QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);



            decimal TOTAL_SHAREHOLDERS_EQUITYBalance = decimal.Zero;
            decimal TOTAL_DEBTBalance = decimal.Zero;
            decimal TOTAL_LIABLITIESBalance = decimal.Zero;
            decimal TOTAL_ASSETBalance = decimal.Zero;






            var TOTAL_ASSETSRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.TOTAL_ASSETS).FirstOrDefault();
            var TOTAL_LIABLITIESRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.TOTAL_LIABLITIES).FirstOrDefault();



            if (TOTAL_ASSETSRanges != null)
            {
                TOTAL_ASSETBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_ASSETSRanges , QuarterBalance);
            }

            if (TOTAL_LIABLITIESRanges != null)
            {
                TOTAL_LIABLITIESBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_LIABLITIESRanges , QuarterBalance);
            }

            decimal Debt_To_Assets_Ratio = TOTAL_ASSETBalance / TOTAL_LIABLITIESBalance.DivideByZeroHelper();







            var TOTAL_DEBTRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.TOTAL_DEBT).FirstOrDefault();
            var TOTAL_SHAREHOLDERS_EQUITYRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.TOTAL_SHAREHOLDERS_EQUITY).FirstOrDefault();

            if (TOTAL_DEBTRanges != null)
            {
                TOTAL_DEBTBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_DEBTRanges, QuarterBalance);
            }

            if (TOTAL_SHAREHOLDERS_EQUITYRanges != null)
            {
                TOTAL_SHAREHOLDERS_EQUITYBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_SHAREHOLDERS_EQUITYRanges, QuarterBalance);
            }

            decimal Debt_To_Equity_Ratio = TOTAL_DEBTBalance / TOTAL_SHAREHOLDERS_EQUITYBalance.DivideByZeroHelper();


            if (TOTAL_LIABLITIESRanges != null && TOTAL_ASSETSRanges != null)
            {


                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(TOTAL_ASSETSRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(TOTAL_LIABLITIESRanges , fiscalYear, periodfrom, periodto);
            }

            var Debt_To_Assets = new RatioModel
            {
                Type = Type,
                Budget = currentBudgetPlan,
                Name = "Debt To Assets Ratio",
                Actual = Debt_To_Assets_Ratio

            };

            currentBudgetPlan = decimal.Zero; RatioList.Add(Debt_To_Assets);

            if (TOTAL_SHAREHOLDERS_EQUITYRanges != null && TOTAL_DEBTRanges != null)
            {

                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(TOTAL_DEBTRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(TOTAL_SHAREHOLDERS_EQUITYRanges , fiscalYear, periodfrom, periodto);
            }



            var Debt_To_Equity_Ratios = new RatioModel
            {
                Type = Type,
                Budget = currentBudgetPlan,
                Name = "Debt To Equity Ratio",
                Actual = Debt_To_Equity_Ratio

            };

            currentBudgetPlan = decimal.Zero; RatioList.Add(Debt_To_Equity_Ratios);
            return RatioList;
        }

        public List<RatioModel> ProfitabilityRatios(int fiscalYear, int periodfrom, int periodto)
        {

            ExceedDbContext _db = new ExceedDbContext();
            BudgetReportHelper _ReportHelper = new BudgetReportHelper();
            FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();
            var RatioList = new List<RatioModel>();



            List<GLLedgerPostingLines> QuarterBalance = _BudgetGLJournalHelper.GetGLPostingByPeriodRange(fiscalYear, periodfrom, periodto);

            string Type = "PROFITABILITY RATIO";
            decimal currentBudgetPlan = decimal.Zero;


            decimal NET_INCOMEBalance = decimal.Zero;
            decimal OPERATING_INCOMEBalance = decimal.Zero;
            decimal GROSS_INCOMEBalance = decimal.Zero;
            decimal SALESBalance = decimal.Zero;
            decimal TOTAL_ASSETSBalance = decimal.Zero;
            decimal TOTAL_SHAREHOLDERS_EQUITYBalance = decimal.Zero;
            decimal COST_OF_SERVICESBalance = decimal.Zero;









            var GROSS_INCOMERanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.GROSS_INCOME).FirstOrDefault();
            var SALESRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.SALES).FirstOrDefault();



            // #region get balance

            if (SALESRanges != null)
            {
                SALESBalance = _ReportHelper.GetAccountRangeBalance(SALESRanges, QuarterBalance);
            }

            if (GROSS_INCOMERanges != null)
            {
                GROSS_INCOMEBalance = _ReportHelper.GetAccountRangeBalance(GROSS_INCOMERanges, QuarterBalance);
            }

            decimal Gross_Profit_MarginTotal = GROSS_INCOMEBalance / SALESBalance.DivideByZeroHelper();
            if (SALESRanges != null && GROSS_INCOMERanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(GROSS_INCOMERanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(SALESRanges , fiscalYear, periodfrom, periodto);
            }
            var Gross_Profit_Margin_Ratio = new RatioModel
            {
                Type = Type,
                Name = "Gross Profit Margin",
                Budget = currentBudgetPlan,
                Actual = Gross_Profit_MarginTotal

            };

            currentBudgetPlan = decimal.Zero; RatioList.Add(Gross_Profit_Margin_Ratio);



            var OPERATING_INCOMERanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.OPERATING_INCOME).FirstOrDefault();



            if (OPERATING_INCOMERanges != null)
            {
                OPERATING_INCOMEBalance = _ReportHelper.GetAccountRangeBalance(OPERATING_INCOMERanges , QuarterBalance);
            }

            decimal Operating_Profit_MarginTotal = OPERATING_INCOMEBalance / SALESBalance.DivideByZeroHelper();

            if (SALESRanges != null && OPERATING_INCOMERanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(OPERATING_INCOMERanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(SALESRanges , fiscalYear, periodfrom, periodto);
            }

            var Operating_Profit_Margin_Ratio = new RatioModel
            {
                Type = Type,
                Name = "Operating Profit Margin",
                Budget = currentBudgetPlan,
                Actual = Operating_Profit_MarginTotal

            };
            currentBudgetPlan = decimal.Zero;
            RatioList.Add(Operating_Profit_Margin_Ratio);


            var NET_INCOMERanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.NET_INCOME).FirstOrDefault();

            if (NET_INCOMERanges != null)
            {
                NET_INCOMEBalance = _ReportHelper.GetAccountRangeBalance(NET_INCOMERanges , QuarterBalance);
            }

            decimal Net_Profit_MarginTotal = NET_INCOMEBalance / SALESBalance.DivideByZeroHelper();
            if (SALESRanges != null && NET_INCOMERanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(NET_INCOMERanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(SALESRanges , fiscalYear, periodfrom, periodto);
            }
            var Net_Profit_Margin_Ratio = new RatioModel
            {
                Type = Type,
                Name = "Net Profit Margin",
                Budget = currentBudgetPlan,
                Actual = Net_Profit_MarginTotal

            };
            currentBudgetPlan = decimal.Zero;
            RatioList.Add(Net_Profit_Margin_Ratio);
            //   _db.SaveChanges();



            var TOTAL_ASSETSRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.TOTAL_ASSETS).FirstOrDefault();




            if (TOTAL_ASSETSRanges != null)
            {
                TOTAL_ASSETSBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_ASSETSRanges , QuarterBalance);
            }

            decimal Return_On_AssetsTotal = NET_INCOMEBalance / TOTAL_ASSETSBalance.DivideByZeroHelper();

            if (NET_INCOMERanges != null && TOTAL_ASSETSRanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(TOTAL_ASSETSRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(NET_INCOMERanges , fiscalYear, periodfrom, periodto);
            }
            var Return_On_Assets_Ratio = new RatioModel
            {
                Type = Type,
                Name = "Return On Assets",
                Budget = currentBudgetPlan,
                Actual = Return_On_AssetsTotal

            };
            currentBudgetPlan = decimal.Zero;
            RatioList.Add(Return_On_Assets_Ratio);



            var TOTAL_SHAREHOLDERS_EQUITYRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.TOTAL_SHAREHOLDERS_EQUITY).FirstOrDefault();



            if (TOTAL_SHAREHOLDERS_EQUITYRanges != null)
            {
                TOTAL_SHAREHOLDERS_EQUITYBalance = _ReportHelper.GetAccountRangeBalance(TOTAL_SHAREHOLDERS_EQUITYRanges , QuarterBalance);
            }

            decimal Return_On_EquityTotal = NET_INCOMEBalance / TOTAL_SHAREHOLDERS_EQUITYBalance.DivideByZeroHelper();
            if (NET_INCOMERanges != null && TOTAL_SHAREHOLDERS_EQUITYRanges != null)
            {
                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(TOTAL_SHAREHOLDERS_EQUITYRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(NET_INCOMERanges , fiscalYear, periodfrom, periodto);

            }
            var entity = new RatioModel
            {
                Type = Type,
                Name = "Return On Equity",
                Budget = currentBudgetPlan,
                Actual = Return_On_EquityTotal

            };
            currentBudgetPlan = decimal.Zero;

            RatioList.Add(entity);




            var COST_OF_SERVICESRanges = _db.FinanceDashboards.Where(x => x.Category == Core.Domain.Dashboard.FinanceDashboradCategory.COST_OF_SERVICES).FirstOrDefault();




            if (COST_OF_SERVICESRanges != null)
            {
                COST_OF_SERVICESBalance = _ReportHelper.GetAccountRangeBalance(COST_OF_SERVICESRanges , QuarterBalance);
            }

            decimal Cost_Of_Goods_SoldTotal = COST_OF_SERVICESBalance / SALESBalance.DivideByZeroHelper();
            if (SALESRanges != null && COST_OF_SERVICESRanges != null)
            {

                currentBudgetPlan = budgetReportHelper.GetQuarterBudgetPlanByAccountRange(COST_OF_SERVICESRanges , fiscalYear, periodfrom, periodto) + budgetReportHelper.GetQuarterBudgetPlanByAccountRange(SALESRanges , fiscalYear, periodfrom, periodto);
            }
            var Cost_Of_Goods_Sold_Ratio = new RatioModel
            {
                Type = Type,
                Name = "Cost Of Goods Sold",
                Budget = currentBudgetPlan,
                Actual = Cost_Of_Goods_SoldTotal

            };
            currentBudgetPlan = decimal.Zero;

            RatioList.Add(Cost_Of_Goods_Sold_Ratio);


            return RatioList;

        }
        #endregion

    


    }

}
