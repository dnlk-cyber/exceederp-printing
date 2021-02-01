using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.Ratio.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Finance.Ratio.DataSource
{

    [DataObject]
    public class RatioAnnualSummaryDataSource
    {
        private static readonly ExceedDbContext Db = new ExceedDbContext();
        private readonly ExceedDbContext db = new ExceedDbContext();


        public static string GetFiscalYearByIndex(string fiscalYearId, int offset)
        {

            string year = string.Empty;
            if (!string.IsNullOrWhiteSpace(fiscalYearId))
            {
                 try
                {


                    int fiscalYear = int.Parse(fiscalYearId);

                    var glyear = Db.GLFiscalYears.Find(fiscalYear);

                    if (glyear != null)
                    {

                        if (offset == 0)
                        {
                            return year = glyear.Name;
                        }
                        else
                        {
                            var prevGlyear = Db.GLFiscalYears.Where(x => x.DateFrom < glyear.DateFrom ).OrderByDescending(y => y.DateFrom);
                            offset = offset - 1;
                            var prevYear = prevGlyear.Skip(offset).Take(1).Single();

                            if (prevYear != null)
                            {
                                return year = prevYear.Name;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    return year;
                }
            }
            return year;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RatioAnnualSummaryViewModel> GetRatioSummary(string fiscalYear)
        {
            List<RatioAnnualSummaryViewModel> ratioList = new List<RatioAnnualSummaryViewModel>();
            BudgetReportHelper budgetReportHelper = new BudgetReportHelper();
            FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            BudgetGLJournalHelper _BudgetGLJournalHelper = new BudgetGLJournalHelper();
            int FiscalYear = _FiscalYearHelper.GetCurrentFYearId();


            if (!string.IsNullOrWhiteSpace(fiscalYear))
            {
                FiscalYear = int.Parse(fiscalYear);

            }


            if (!string.IsNullOrWhiteSpace(fiscalYear))
            {
                var summaryData = budgetReportHelper.GetRatioSummary(FiscalYear);



                List<RatioModel> distinctTypes = summaryData.firstQ1ratioModels.DistinctByField(x => x.Type).ToList();
                List<RatioModel> distinctNames = summaryData.firstQ1ratioModels.DistinctByField(x => x.Name).ToList();

                foreach (var t in distinctTypes)
                {
                    List<RatioModel> distinctNamesByType = distinctNames.Where(x => x.Type == t.Type).ToList();



                    var ratio = new RatioAnnualSummaryViewModel
                    {
                        Type = t.Type,


                    };
                    ratioList.Add(ratio);


                    foreach (var data in distinctNamesByType)
                    {
                        decimal currentBudget = decimal.Zero;



                        currentBudget += summaryData.firstQ1ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget + summaryData.firstQ2ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget + summaryData.firstQ3ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget + summaryData.firstQ4ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget;

                        currentBudget += summaryData.secondQ1ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget + summaryData.secondQ2ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget + summaryData.secondQ3ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget + summaryData.secondQ4ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget;

                        currentBudget += summaryData.thirdQ1ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget + summaryData.thirdQ2ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget + summaryData.thirdQ3ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Budget + summaryData.thirdQ4ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().GetDefault().Budget;



                        var subratio = new RatioAnnualSummaryViewModel
                        {
                            Name = data.Name,
                            currentBudget = currentBudget,
                            firstQ1 = summaryData.firstQ1ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,
                            firstQ2 = summaryData.firstQ2ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,
                            firstQ3 = summaryData.firstQ3ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,
                            firstQ4 = summaryData.firstQ4ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,



                            secondQ1 = summaryData.secondQ1ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,
                            secondQ2 = summaryData.secondQ2ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,
                            secondQ3 = summaryData.secondQ3ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,
                            secondQ4 = summaryData.secondQ4ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,



                            thirdQ1 = summaryData.thirdQ1ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,
                            thirdQ2 = summaryData.thirdQ2ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,
                            thirdQ3 = summaryData.thirdQ3ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual,
                            thirdQ4 = summaryData.thirdQ4ratioModels.Where(x => x.Name == data.Name).FirstOrDefault().GetDefault().Actual
                        };


                        ratioList.Add(subratio);

                    }

                }







            }



            return ratioList;




        }
    }
}