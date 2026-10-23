using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System.Collections.Generic;
using System.Linq; 

namespace ExceedERP.Helpers.Budget
{

    #region Model
    public class RatioSummaryReportModel
    {

        public RatioSummaryReportModel()
        {
            firstQ1ratioModels = new List<RatioModel>();
            firstQ2ratioModels = new List<RatioModel>();
            firstQ3ratioModels = new List<RatioModel>();
            firstQ4ratioModels = new List<RatioModel>();




            secondQ1ratioModels = new List<RatioModel>();
            secondQ2ratioModels = new List<RatioModel>();
            secondQ3ratioModels = new List<RatioModel>();
            secondQ4ratioModels = new List<RatioModel>();



            thirdQ1ratioModels = new List<RatioModel>();
            thirdQ2ratioModels = new List<RatioModel>();
            thirdQ3ratioModels = new List<RatioModel>();
            thirdQ4ratioModels = new List<RatioModel>();
        }


 
        public List<RatioModel> firstQ1ratioModels { get; set; }
        public List<RatioModel> firstQ2ratioModels { get; set; }
        public List<RatioModel> firstQ3ratioModels { get; set; }
        public List<RatioModel> firstQ4ratioModels { get; set; }
        public List<RatioModel> secondQ1ratioModels { get; set; }
        public List<RatioModel> secondQ2ratioModels { get; set; }
        public List<RatioModel> secondQ3ratioModels { get; set; }
        public List<RatioModel> secondQ4ratioModels { get; set; }
        public List<RatioModel> thirdQ1ratioModels { get; set; }
        public List<RatioModel> thirdQ2ratioModels { get; set; }
        public List<RatioModel> thirdQ3ratioModels { get; set; }
        public List<RatioModel> thirdQ4ratioModels { get; set; }


    }

    #endregion

    public class BudgetReportHelper
    {
        


        public RatioSummaryReportModel GetRatioSummary(int fiscalYearId)
        {
            List<FinancialRatio> list =new List<FinancialRatio>();
            using (ExceedDbContext db = new ExceedDbContext())
            {

                var activeRatios = db.FinancialRatios.Where(x => x.Enable).ToList();

                var activity = activeRatios.Where(x => x.Category == RatioCategory.Activity).ToList();
                var efficiency = activeRatios.Where(x => x.Category == RatioCategory.Efficiency).ToList();
                var liquidity = activeRatios.Where(x => x.Category == RatioCategory.Liquidity).ToList();
                var profitability = activeRatios.Where(x => x.Category == RatioCategory.Profitability).ToList();
                var investor = activeRatios.Where(x => x.Category == RatioCategory.Investor).ToList();
                var leverage = activeRatios.Where(x => x.Category == RatioCategory.Leverage).ToList();

                #region Init

                BudgetRatioHelper budgetRatioHelper = new BudgetRatioHelper();
                ReportHelper reportHelper = new ReportHelper();
                RatioSummaryReportModel ratioSummaryReports = new RatioSummaryReportModel();


                List<RatioModel> firstQ1ratioModels = new List<RatioModel>();
                List<RatioModel> firstQ2ratioModels = new List<RatioModel>();
                List<RatioModel> firstQ3ratioModels = new List<RatioModel>();
                List<RatioModel> firstQ4ratioModels = new List<RatioModel>();




                List<RatioModel> secondQ1ratioModels = new List<RatioModel>();
                List<RatioModel> secondQ2ratioModels = new List<RatioModel>();
                List<RatioModel> secondQ3ratioModels = new List<RatioModel>();
                List<RatioModel> secondQ4ratioModels = new List<RatioModel>();



                List<RatioModel> thirdQ1ratioModels = new List<RatioModel>();
                List<RatioModel> thirdQ2ratioModels = new List<RatioModel>();
                List<RatioModel> thirdQ3ratioModels = new List<RatioModel>();
                List<RatioModel> thirdQ4ratioModels = new List<RatioModel>();




                List<RatioModel> fourthQ1ratioModels = new List<RatioModel>();
                List<RatioModel> fourthQ2ratioModels = new List<RatioModel>();
                List<RatioModel> fourthQ3ratioModels = new List<RatioModel>();
                List<RatioModel> fourthQ4ratioModels = new List<RatioModel>();


                #endregion

                var fiscalyears = db.GLFiscalYears.Where(x => x.GlFiscalYearId <= fiscalYearId).OrderByDescending(x => x.DateFrom).ToList();
                if (fiscalyears.Count >= 3)
                {
                    fiscalyears = fiscalyears.Take(3).ToList();
                }

                for (int f = 0; f < fiscalyears.Count; f++)
                {
                    if (f == 0)
                    {
                        firstQ1ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3), activity));
                        firstQ2ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6), activity));
                        firstQ3ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9), activity));
                        firstQ4ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12), activity));


                        firstQ1ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3), efficiency));
                        firstQ2ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6), efficiency));
                        firstQ3ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9), efficiency));
                        firstQ4ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12), efficiency));

                        firstQ1ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3), liquidity));
                        firstQ2ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6), liquidity));
                        firstQ3ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9), liquidity));
                        firstQ4ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12), liquidity));


                        firstQ1ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3), profitability));
                        firstQ2ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6), profitability));
                        firstQ3ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9), profitability));
                        firstQ4ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12), profitability));


                        firstQ1ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3), investor));
                        firstQ2ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6), investor));
                        firstQ3ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9), investor));
                        firstQ4ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12), investor));


                        firstQ1ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3), leverage));
                        firstQ2ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6), leverage));
                        firstQ3ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9), leverage));
                        firstQ4ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12), leverage));


                    }


                    if (f == 1)
                    {
                        secondQ1ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3), activity));
                        secondQ2ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6), activity));
                        secondQ3ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9), activity));
                        secondQ4ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12), activity));


                        secondQ1ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3), efficiency));
                        secondQ2ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6), efficiency));
                        secondQ3ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9), efficiency));
                        secondQ4ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12), efficiency));

                        secondQ1ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3), liquidity));
                        secondQ2ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6), liquidity));
                        secondQ3ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9), liquidity));
                        secondQ4ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12), liquidity));


                        secondQ1ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3), profitability));
                        secondQ2ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6), profitability));
                        secondQ3ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9), profitability));
                        secondQ4ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12), profitability));


                        secondQ1ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3), investor));
                        secondQ2ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6), investor));
                        secondQ3ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9), investor));
                        secondQ4ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12), investor));


                        secondQ1ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3), leverage));
                        secondQ2ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6), leverage));
                        secondQ3ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9), leverage));
                        secondQ4ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12), leverage));

                    }


                    if (f == 2)
                    {
                        thirdQ1ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3), activity));
                        thirdQ2ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6), activity));
                        thirdQ3ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9), activity));
                        thirdQ4ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12), activity));


                        thirdQ1ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3), efficiency));
                        thirdQ2ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6), efficiency));
                        thirdQ3ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9), efficiency));
                        thirdQ4ratioModels.AddRange(budgetRatioHelper.EfficiencyRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12), efficiency));

                        thirdQ1ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3), liquidity));
                        thirdQ2ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6), liquidity));
                        thirdQ3ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9), liquidity));
                        thirdQ4ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12), liquidity));


                        thirdQ1ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3), profitability));
                        thirdQ2ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6), profitability));
                        thirdQ3ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9), profitability));
                        thirdQ4ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12), profitability));


                        thirdQ1ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3), investor));
                        thirdQ2ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6), investor));
                        thirdQ3ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9), investor));
                        thirdQ4ratioModels.AddRange(budgetRatioHelper.InvestorRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12), investor));


                        thirdQ1ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3), leverage));
                        thirdQ2ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6), leverage));
                        thirdQ3ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9), leverage));
                        thirdQ4ratioModels.AddRange(budgetRatioHelper.LeverageRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12), leverage));


                    }





                }

                ratioSummaryReports.firstQ1ratioModels = firstQ1ratioModels;
                ratioSummaryReports.firstQ2ratioModels = firstQ2ratioModels;
                ratioSummaryReports.firstQ3ratioModels = firstQ3ratioModels;
                ratioSummaryReports.firstQ4ratioModels = firstQ4ratioModels;


                ratioSummaryReports.secondQ1ratioModels = secondQ1ratioModels;
                ratioSummaryReports.secondQ2ratioModels = secondQ2ratioModels;
                ratioSummaryReports.secondQ3ratioModels = secondQ3ratioModels;
                ratioSummaryReports.secondQ4ratioModels = secondQ4ratioModels;



                ratioSummaryReports.thirdQ1ratioModels = thirdQ1ratioModels;
                ratioSummaryReports.thirdQ2ratioModels = thirdQ2ratioModels;
                ratioSummaryReports.thirdQ3ratioModels = thirdQ3ratioModels;
                ratioSummaryReports.thirdQ4ratioModels = thirdQ4ratioModels;

                return ratioSummaryReports;
            }
        }
        public List<BudgetPlanMonthly> GetQuarterPlanBalance(GlFiscalYear fiscalYear, BudgetEnums.BudgetQuarter budgetQuarter)
        {
            using (ExceedDbContext _db = new ExceedDbContext())
            {
                ReportHelper reportHelper = new ReportHelper();
                List<BudgetPlanMonthly> budgetPlanMonthlies = new List<BudgetPlanMonthly>();

                var thisYearPlans = _db.PlanCategorys.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId && x.IsOnlineApproved).ToList();
                foreach (var thisYearPlan in thisYearPlans)
                {
                    var budgetPlans = _db.BudgetPlans.Where(x => x.PlanCategoryId == thisYearPlan.PlanCategoryID).ToList();
                    foreach (var budgetPlan in budgetPlans)
                    {
                        List<BudgetPlanMonthly> monthlys = new List<BudgetPlanMonthly>();
                        int startIndex = 0;
                        int endIndex = 0;
                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.First))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 1);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 3);
                        }

                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.Second))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 4);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 6);
                        }

                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.Third))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 7);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 9);
                        }

                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.Fourth))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 10);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 12);

                        }

                        monthlys = _db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == budgetPlan.BudgetPlanID && x.GLPeriodId >= startIndex && x.GLPeriodId <= endIndex).ToList();

                        budgetPlanMonthlies.AddRange(monthlys);
                    }
                }

                return budgetPlanMonthlies;
            }
        }

        public List<BudgetPlanMonthly> GetPeriodRangePlanBalance(int fiscalYearId, int fromPeriod, int toPeriod)
        {
            using (ExceedDbContext _db = new ExceedDbContext())
            {
                List<BudgetPlanMonthly> budgetPlanMonthlies = new List<BudgetPlanMonthly>();

                var periodFrom = _db.GLPeriods.Find(fromPeriod);
                var periodTo = _db.GLPeriods.Find(toPeriod);

                var coveredPeriodIds = _db.GLPeriods.AsNoTracking().Where(x => x.GlFiscalYearId == fiscalYearId)
                                                    .Where(x => x.DateFrom >= periodFrom.DateFrom)
                                                    .Where(x => x.DateTo <= periodTo.DateTo).Select(x => x.GLPeriodId).ToList();

                var monthlys = _db.BudgetPlanMonthlys
                          .Include("BudgetPlan")
                         .Where(x => coveredPeriodIds.Contains(x.GLPeriodId)).ToList();
                
                budgetPlanMonthlies.AddRange(monthlys);

                return budgetPlanMonthlies;
            }
        }
        public List<BudgetPlanMonthly> ParseQuarterBalanceFromPeriod(List<BudgetPlanMonthly> budgetPlanMonthlies, int fiscalYearId, BudgetEnums.BudgetQuarter quarter)
        {
            using (ExceedDbContext _db = new ExceedDbContext())
            {
                ReportHelper reportHelper = new ReportHelper();

                int startPeriodId = 0;
                int endPeriodId = 0;

                switch (quarter)

                {
                    case BudgetEnums.BudgetQuarter.First:
                        startPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalYearId, 1);
                        endPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalYearId, 3);
                        break;

                    case BudgetEnums.BudgetQuarter.Second:
                        startPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalYearId, 4);
                        endPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalYearId, 6);
                        break;

                    case BudgetEnums.BudgetQuarter.Third:
                        startPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalYearId, 7);
                        endPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalYearId, 9);
                        break;

                    case BudgetEnums.BudgetQuarter.Fourth:
                        startPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalYearId, 10);
                        endPeriodId = reportHelper.GetPeriodQuarterIndex(fiscalYearId, 12);
                        break;

                }

                var periodFrom = _db.GLPeriods.Find(startPeriodId);
                var periodTo = _db.GLPeriods.Find(endPeriodId);

                var coveredPeriodIds = _db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYearId)
                                                    .Where(x => x.DateFrom >= periodFrom.DateFrom)
                                                    .Where(x => x.DateTo <= periodTo.DateTo).Select(x => x.GLPeriodId).ToList();

                var monthlys = budgetPlanMonthlies.Where(x => coveredPeriodIds.Contains(x.GLPeriodId)).ToList();
                return monthlys;
            }
        }



        public List<BudgetPlanMonthly> GetQuarterPlanBalance(int fiscalYear, BudgetEnums.BudgetQuarter budgetQuarter)
        {
            using (ExceedDbContext _db = new ExceedDbContext())
            {
                ReportHelper reportHelper = new ReportHelper();
                List<BudgetPlanMonthly> budgetPlanMonthlies = new List<BudgetPlanMonthly>();

                var thisYearPlans = _db.PlanCategorys.Where(x => x.GlFiscalYearId == fiscalYear).ToList();
                foreach (var thisYearPlan in thisYearPlans)
                {
                    var budgetPlans = _db.BudgetPlans.Where(x => x.PlanCategoryId == thisYearPlan.PlanCategoryID).ToList();

                    foreach (var budgetPlan in budgetPlans)
                    {
                        List<BudgetPlanMonthly> monthlys = new List<BudgetPlanMonthly>();
                        int startIndex = 0;
                        int endIndex = 0;
                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.First))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 1);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 3);
                        }

                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.Second))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 4);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 6);
                        }

                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.Third))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 7);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 9);
                        }

                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.Fourth))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 10);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 12);

                        }

                        monthlys = _db.BudgetPlanMonthlys.Where(x => x.GLPeriodId >= startIndex && x.GLPeriodId <= endIndex && x.BudgetDefineID == budgetPlan.BudgetPlanID).ToList();

                        budgetPlanMonthlies.AddRange(monthlys);
                    }
                }

                return budgetPlanMonthlies;
            }
        }

        public decimal GetQuarterBudgetPlanByAccountRange(FinanceDashboard ranges, int fiscalYearId, int fromPeriod, int toPeriod)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                decimal totalBudget = decimal.Zero;
                var thisYearPlans = db.PlanCategorys.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();

                if (ranges != null && ranges.FinanceDashboardRanges != null)
                {
                    foreach (var thisYearPlan in thisYearPlans)
                    {
                        var budgetPlans = db.BudgetPlans.Where(x => x.PlanCategoryId == thisYearPlan.PlanCategoryID).ToList();

                        foreach (var budgetPlan in budgetPlans)
                        {
                            List<BudgetPlanMonthly> monthlyPlans = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == budgetPlan.BudgetPlanID && x.GLPeriodId >= fromPeriod && x.GLPeriodId <= toPeriod).ToList();


                            foreach (var range in ranges.FinanceDashboardRanges)
                            {
                                totalBudget += GetQuarterPlanByAccountRange(monthlyPlans, range.AccountFrom, range.AccountTo);
                            }
                        }
                    }
                }

                return totalBudget;
            }
        }
        public decimal GetAccountRangeBalance(FinanceDashboard Ranges, List<GLLedgerPostingLines> CurrentBalance)
        {
            decimal TotalBalance = decimal.Zero;
            ReportHelper _ReportHelper = new ReportHelper();

            if (Ranges != null && Ranges.FinanceDashboardRanges != null)
            {
                foreach (var Range in Ranges.FinanceDashboardRanges)
                {
                    TotalBalance += _ReportHelper.GetAccountEndBalancesWithAllranges(CurrentBalance, Range.AccountFrom, Range.AccountTo);
                }
            }
            return TotalBalance;
        }
        public decimal GetAnnualBudgetPlanByAccountRange(FinanceDashboard dashBoardRange, int fiscalYearId)
        {

            using (ExceedDbContext db = new ExceedDbContext())
            {
                decimal totalBudget = decimal.Zero;
                if (dashBoardRange != null)
                {
                    List<FinanceDashboardRange> dashRanges = new List<FinanceDashboardRange>();
                    if (dashBoardRange.FinanceDashboardRanges != null)
                    {
                        dashRanges = dashBoardRange.FinanceDashboardRanges.ToList();
                    }
                    var thisYearPlans = db.PlanCategorys.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();
                    foreach (var thisYearPlan in thisYearPlans)
                    {
                        var budgetPlans = db.BudgetPlans.Where(x => x.PlanCategoryId == thisYearPlan.PlanCategoryID).ToList();

                        foreach (var budgetPlan in budgetPlans)
                        {
                            List<BudgetPlanMonthly> monthlyPlans = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == budgetPlan.BudgetPlanID).ToList();


                            foreach (var range in dashRanges)
                            {
                                totalBudget += GetQuarterPlanByAccountRange(monthlyPlans, range.AccountFrom, range.AccountTo);
                            }
                        }
                    }
                }

                return totalBudget;
            }
        }
        public decimal GetQuarterPlanByAccountRange(List<BudgetPlanMonthly> budgetPlanMonthlies, string fromAccount, string toAccount)
        {

            using (ExceedDbContext db = new ExceedDbContext())
            {

                int accountLength = db.GlSegmentSetups.Count(x => x.Enabled);


                string seg1From = "";
                string seg1To = "";
                string seg2From = "";
                string seg2To = "";
                string seg3From = "";
                string seg3To = "";
                string seg4From = "";
                string seg4To = "";
                string seg5From = "";
                string seg5To = "";

                switch (accountLength)
                {
                    case 1:
                        seg1From = fromAccount.Split('-')[0];
                        seg1To = toAccount.Split('-')[0];
                        break;

                    case 2:
                        seg1From = fromAccount.Split('-')[0];
                        seg1To = toAccount.Split('-')[0];
                        seg2From = fromAccount.Split('-')[1];
                        seg2To = toAccount.Split('-')[1];
                        break;
                    case 3:
                        seg1From = fromAccount.Split('-')[0];
                        seg1To = toAccount.Split('-')[0];
                        seg2From = fromAccount.Split('-')[1];
                        seg2To = toAccount.Split('-')[1];
                        seg3From = fromAccount.Split('-')[2];
                        seg3To = toAccount.Split('-')[2];
                        break;
                    case 4:
                        seg1From = fromAccount.Split('-')[0];
                        seg1To = toAccount.Split('-')[0];
                        seg2From = fromAccount.Split('-')[1];
                        seg2To = toAccount.Split('-')[1];
                        seg3From = fromAccount.Split('-')[2];
                        seg3To = toAccount.Split('-')[2];
                        seg4From = fromAccount.Split('-')[3];
                        seg4To = toAccount.Split('-')[3];
                        break;
                    case 5:
                        seg1From = fromAccount.Split('-')[0];
                        seg1To = toAccount.Split('-')[0];
                        seg2From = fromAccount.Split('-')[1];
                        seg2To = toAccount.Split('-')[1];
                        seg3From = fromAccount.Split('-')[2];
                        seg3To = toAccount.Split('-')[2];
                        seg4From = fromAccount.Split('-')[3];
                        seg4To = toAccount.Split('-')[3];
                        seg5From = fromAccount.Split('-')[4];
                        seg5To = toAccount.Split('-')[4];
                        break;
                }



                return budgetPlanMonthlies.Where(x => x.BudgetPlan.AccountCode.GetSegment(0).ToUpper().CompareTo(seg1From.ToUpper()) >= 0)
                                .Where(x => x.BudgetPlan.AccountCode.GetSegment(0).ToString().ToUpper().CompareTo(seg1To.ToUpper()) <= 0)

                                .Where(x => x.BudgetPlan.AccountCode.GetSegment(1).ToUpper().CompareTo(seg2From.ToUpper()) >= 0)
                                .Where(x => x.BudgetPlan.AccountCode.GetSegment(1).ToUpper().CompareTo(seg2To.ToUpper()) <= 0)

                                .Where(x => x.BudgetPlan.AccountCode.GetSegment(2).ToUpper().CompareTo(seg3From.ToUpper()) >= 0)
                                .Where(x => x.BudgetPlan.AccountCode.GetSegment(2).ToUpper().CompareTo(seg3To.ToUpper()) <= 0)

                                .Where(x => x.BudgetPlan.AccountCode.GetSegment(3).ToUpper().CompareTo(seg4From.ToUpper()) >= 0)
                                .Where(x => x.BudgetPlan.AccountCode.GetSegment(3).ToUpper().CompareTo(seg4To.ToUpper()) <= 0)

                                .Where(x => x.BudgetPlan.AccountCode.GetSegment(4).ToUpper().CompareTo(seg5From.ToUpper()) >= 0)
                                .Where(x => x.BudgetPlan.AccountCode.GetSegment(4).ToUpper().CompareTo(seg5To.ToUpper()) <= 0)
                                .Sum(x => x.CurrentBudget);
            }


        }

        public List<BudgetPlanMonthly> GetQuarterPlanBalanceByBranch(int fiscalYear, int branchId, BudgetEnums.BudgetQuarter budgetQuarter)
        {
            using (ExceedDbContext _db = new ExceedDbContext())
            {
                ReportHelper reportHelper = new ReportHelper();
                List<BudgetPlanMonthly> budgetPlanMonthlies = new List<BudgetPlanMonthly>();

                var thisYearPlans = _db.PlanCategorys.Where(x => x.GlFiscalYearId == fiscalYear && x.IsOnlineApproved).ToList();
                foreach (var thisYearPlan in thisYearPlans)
                {
                    var budgetPlans = _db.BudgetPlans.Where(x => x.PlanCategoryId == thisYearPlan.PlanCategoryID && x.BranchId == branchId).ToList();

                    foreach (var budgetPlan in budgetPlans)
                    {
                        List<BudgetPlanMonthly> monthlys = new List<BudgetPlanMonthly>();
                        int startIndex = 0;
                        int endIndex = 0;
                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.First))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 1);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 3);
                        }

                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.Second))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 4);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 6);
                        }

                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.Third))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 7);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 9);
                        }

                        if (budgetQuarter.Equals(BudgetEnums.BudgetQuarter.Fourth))
                        {
                            startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 10);
                            endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 12);

                        }

                        monthlys = _db.BudgetPlanMonthlys.Where(x => x.GLPeriodId >= startIndex && x.GLPeriodId <= endIndex && x.BudgetDefineID == budgetPlan.BudgetPlanID).ToList();

                        budgetPlanMonthlies.AddRange(monthlys);
                    }
                }

                return budgetPlanMonthlies;
            }
        }
    }
}
