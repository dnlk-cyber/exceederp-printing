using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Dashboard;
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

            #region Init

            ExceedDbContext db = new ExceedDbContext();
            BudgetGLJournalHelper budgetGLJournalHelper = new BudgetGLJournalHelper();
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
                    firstQ1ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3)));
                    firstQ2ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6)));
                    firstQ3ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9)));
                    firstQ4ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12)));


                    firstQ1ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3)));
                    firstQ2ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6)));
                    firstQ3ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9)));
                    firstQ4ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12)));

                    firstQ1ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3)));
                    firstQ2ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6)));
                    firstQ3ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9)));
                    firstQ4ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12)));


                    firstQ1ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 3)));
                    firstQ2ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 6)));
                    firstQ3ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 9)));
                    firstQ4ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[0].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[0].GlFiscalYearId, 12)));


                }






                if (f == 1)
                {
                    secondQ1ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3)));
                    secondQ2ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6)));
                    secondQ3ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9)));
                    secondQ4ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12)));


                    secondQ1ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3)));
                    secondQ2ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6)));
                    secondQ3ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9)));
                    secondQ4ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12)));

                    secondQ1ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3)));
                    secondQ2ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6)));
                    secondQ3ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9)));
                    secondQ4ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12)));


                    secondQ1ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 3)));
                    secondQ2ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 6)));
                    secondQ3ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 9)));
                    secondQ4ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[1].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[1].GlFiscalYearId, 12)));


                }






                if (f == 2)
                {
                    thirdQ1ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3)));
                    thirdQ2ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6)));
                    thirdQ3ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9)));
                    thirdQ4ratioModels.AddRange(budgetRatioHelper.ActivityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12)));


                    thirdQ1ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3)));
                    thirdQ2ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6)));
                    thirdQ3ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9)));
                    thirdQ4ratioModels.AddRange(budgetRatioHelper.CapitalStructureRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12)));

                    thirdQ1ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3)));
                    thirdQ2ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6)));
                    thirdQ3ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9)));
                    thirdQ4ratioModels.AddRange(budgetRatioHelper.LiquidityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12)));


                    thirdQ1ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 1), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 3)));
                    thirdQ2ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 4), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 6)));
                    thirdQ3ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 7), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 9)));
                    thirdQ4ratioModels.AddRange(budgetRatioHelper.ProfitabilityRatios(fiscalyears[2].GlFiscalYearId, reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 10), reportHelper.GetPeriodQuarterIndex(fiscalyears[2].GlFiscalYearId, 12)));


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
        public List<BudgetPlanMonthly> GetQuarterPlanBalance(GlFiscalYear fiscalYear, BudgetEnums.BudgetQuarter budgetQuarter)
        {
            ExceedDbContext _db = new ExceedDbContext();
            ReportHelper reportHelper = new ReportHelper();
            List<BudgetPlanMonthly> budgetPlanMonthlies = new List<BudgetPlanMonthly>();

            var thisYearPlans = _db.PlanCategorys.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId && x.IsOnlineApproved).ToList();
            foreach (var thisYearPlan in thisYearPlans)
            {
                var budgetPlans = _db.BudgetPlans.Where(x => x.BudgetCategoryID == thisYearPlan.PlanCategoryID).ToList();
                foreach (var budgetPlan in  budgetPlans)
                {
                    List<BudgetPlanMonthly> monthlys = new List<BudgetPlanMonthly>();
                    int startIndex = 0;
                    int endIndex = 0;
                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.First))
                    {
                        startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 1);
                        endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 3);
                    }

                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.Second))
                    {
                        startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 4);
                        endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 6);
                    }

                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.Third))
                    {
                        startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 7);
                        endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear.GlFiscalYearId, 9);
                    }

                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.Fourth))
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

        public List<BudgetPlanMonthly> GetQuarterPlanBalance(int fiscalYear, BudgetEnums.BudgetQuarter budgetQuarter)
        {
            ExceedDbContext _db = new ExceedDbContext();
            ReportHelper reportHelper = new ReportHelper();
            List<BudgetPlanMonthly> budgetPlanMonthlies = new List<BudgetPlanMonthly>();

            var thisYearPlans = _db.PlanCategorys.Where(x => x.GlFiscalYearId == fiscalYear).ToList();
            foreach (var thisYearPlan in thisYearPlans)
            {
                var budgetPlans = _db.BudgetPlans.Where(x => x.BudgetCategoryID == thisYearPlan.PlanCategoryID).ToList();
             
                foreach (var budgetPlan in  budgetPlans)
                {
                    List<BudgetPlanMonthly> monthlys = new List<BudgetPlanMonthly>();
                    int startIndex = 0;
                    int endIndex = 0;
                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.First))
                    {
                        startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 1);
                        endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 3);
                    }

                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.Second))
                    {
                        startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 4);
                        endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 6);
                    }

                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.Third))
                    {
                        startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 7);
                        endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 9);
                    }

                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.Fourth))
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

        public decimal GetQuarterBudgetPlanByAccountRange(FinanceDashboard ranges, int fiscalYearId, int fromPeriod, int toPeriod)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal totalBudget = decimal.Zero;
            var thisYearPlans = db.PlanCategorys.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();

            if (ranges != null && ranges.FinanceDashboardRanges != null)
            {
                foreach (var thisYearPlan in thisYearPlans)
                {
                    var budgetPlans = db.BudgetPlans.Where(x => x.BudgetCategoryID == thisYearPlan.PlanCategoryID).ToList();
             
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

            ExceedDbContext db = new ExceedDbContext();
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
                    var budgetPlans = db.BudgetPlans.Where(x => x.BudgetCategoryID == thisYearPlan.PlanCategoryID).ToList();
             
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
        public decimal GetQuarterPlanByAccountRange(List<BudgetPlanMonthly> budgetPlanMonthlies, string fromAccount, string toAccount)
        {

            ExceedDbContext db = new ExceedDbContext();

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



            return budgetPlanMonthlies.Where(x => x.AccountCode.GetSegment(0).ToUpper().CompareTo(seg1From.ToUpper()) >= 0)
                            .Where(x => x.AccountCode.GetSegment(0).ToString().ToUpper().CompareTo(seg1To.ToUpper()) <= 0)

                            .Where(x => x.AccountCode.GetSegment(1).ToUpper().CompareTo(seg2From.ToUpper()) >= 0)
                            .Where(x => x.AccountCode.GetSegment(1).ToUpper().CompareTo(seg2To.ToUpper()) <= 0)

                            .Where(x => x.AccountCode.GetSegment(2).ToUpper().CompareTo(seg3From.ToUpper()) >= 0)
                            .Where(x => x.AccountCode.GetSegment(2).ToUpper().CompareTo(seg3To.ToUpper()) <= 0)

                            .Where(x => x.AccountCode.GetSegment(3).ToUpper().CompareTo(seg4From.ToUpper()) >= 0)
                            .Where(x => x.AccountCode.GetSegment(3).ToUpper().CompareTo(seg4To.ToUpper()) <= 0)

                            .Where(x => x.AccountCode.GetSegment(4).ToUpper().CompareTo(seg5From.ToUpper()) >= 0)
                            .Where(x => x.AccountCode.GetSegment(4).ToUpper().CompareTo(seg5To.ToUpper()) <= 0)
                            .Sum(x => x.CurrentBudget);


        }

        public List<BudgetPlanMonthly> GetQuarterPlanBalanceByBranch(int fiscalYear, int branchId, BudgetEnums.BudgetQuarter budgetQuarter)
        {
            ExceedDbContext _db = new ExceedDbContext();
            ReportHelper reportHelper = new ReportHelper();
            List<BudgetPlanMonthly> budgetPlanMonthlies = new List<BudgetPlanMonthly>();

            var thisYearPlans = _db.PlanCategorys.Where(x => x.GlFiscalYearId == fiscalYear && x.IsOnlineApproved).ToList();
            foreach (var thisYearPlan in thisYearPlans)
            {
                var budgetPlans = _db.BudgetPlans.Where(x => x.BudgetCategoryID == thisYearPlan.PlanCategoryID && x.BranchId == branchId).ToList();

                foreach (var budgetPlan in budgetPlans)
                {
                    List<BudgetPlanMonthly> monthlys = new List<BudgetPlanMonthly>();
                    int startIndex = 0;
                    int endIndex = 0;
                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.First))
                    {
                        startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 1);
                        endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 3);
                    }

                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.Second))
                    {
                        startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 4);
                        endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 6);
                    }

                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.Third))
                    {
                        startIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 7);
                        endIndex = reportHelper.GetPeriodQuarterIndex(fiscalYear, 9);
                    }

                    if (budgetQuarter.Equals(ExceedERP.Core.Domain.Budget.BudgetEnums.BudgetQuarter.Fourth))
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
