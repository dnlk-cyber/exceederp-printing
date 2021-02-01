using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Budget.GAAP.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Reporting.Budget.GAAP.DataSource
{

    [DataObject]
    public class ProfitLossSheetDataSource : List<ProfitLossSheetViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();
        AccountHelper accountHelper = new AccountHelper();
    
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProfitLossSheetViewModel> GetProfitLosss(string Year)
        {
            BudgetGLJournalHelper BudgetGLJournalHelper = new BudgetGLJournalHelper();
            var ProfitLoss = new List<ProfitLossSheetViewModel>();
            var JournalDetails = new List<GLRecordJournalDetail>();

            var FiscalYear = db.GLFiscalYears.Where(x => x.GlFiscalYearId.ToString() == Year).FirstOrDefault();
            var ProfitLossStatements = db.ProfitLossStatements.Where(x => x.GlFiscalYearId == FiscalYear.GlFiscalYearId).ToList();
            var Journal = db.GLRecordJournalDetails.Where(x => x.GLRecordJournal.FiscalYear == FiscalYear.Name).ToList();

            foreach (var ProfitLossStatement in ProfitLossStatements)
            {
                decimal FirstQuarter = decimal.Zero;
                decimal SecondQuarter = decimal.Zero;
                decimal ThirdQuarter = decimal.Zero;
                decimal FourthQuarter = decimal.Zero;

                decimal PlanFirstQuarter = decimal.Zero;
                decimal PlanSecondQuarter = decimal.Zero;
                decimal PlanThirdQuarter = decimal.Zero;
                decimal PlanFourthQuarter = decimal.Zero;

                decimal PercentageFirstQuarter = decimal.Zero;
                decimal PercentageSecondQuarter = decimal.Zero;
                decimal PercentageThirdQuarter = decimal.Zero;
                decimal PercentageFourthQuarter = decimal.Zero;

                decimal ActualTotal = decimal.Zero;
                decimal PlanTotal = decimal.Zero;
                decimal PercentageTotal = decimal.Zero;

                Journal = Journal.Where(x => x.AccountType.ToString() == ProfitLossStatement.ProfitLossStatementCategory.ProfitLostType.ToString()).ToList();

                var AccountCodes = accountHelper.GeAccountRange(ProfitLossStatement.FromAccCode, ProfitLossStatement.ToAccCode, GlSegmentTypes.Account);

                foreach (var AccountCode in AccountCodes)
                {
                    var JournalDetail = Journal.Where(x => x.AccountCode == AccountCode).ToList();
                    JournalDetails.AddRange(JournalDetail);

                }

                FirstQuarter = BudgetGLJournalHelper.GetQuarterBalance(JournalDetails, FiscalYear, BudgetEnums.BudgetQuarter.First);
                SecondQuarter = BudgetGLJournalHelper.GetQuarterBalance(JournalDetails, FiscalYear, BudgetEnums.BudgetQuarter.Second);
                ThirdQuarter = BudgetGLJournalHelper.GetQuarterBalance(JournalDetails, FiscalYear, BudgetEnums.BudgetQuarter.Third);
                FourthQuarter = BudgetGLJournalHelper.GetQuarterBalance(JournalDetails, FiscalYear, BudgetEnums.BudgetQuarter.Fourth);

                PlanFirstQuarter = ProfitLossStatement.First;
                PlanSecondQuarter = ProfitLossStatement.Second;
                PlanThirdQuarter = ProfitLossStatement.Third;
                PlanFourthQuarter = ProfitLossStatement.Fourth;

                PercentageFirstQuarter = ((FirstQuarter - PlanFirstQuarter) / PlanFirstQuarter) * 100;
                PercentageSecondQuarter = ((SecondQuarter - PlanSecondQuarter) / PlanSecondQuarter) * 100;
                PercentageThirdQuarter = ((ThirdQuarter - PlanThirdQuarter) / PlanThirdQuarter) * 100;
                PercentageFourthQuarter = ((FourthQuarter - PlanFourthQuarter) / PlanFourthQuarter) * 100;

                ActualTotal = FirstQuarter + SecondQuarter + ThirdQuarter + FourthQuarter;
                PlanTotal = PlanFirstQuarter + PlanFourthQuarter + PlanSecondQuarter + PlanThirdQuarter;
                PercentageTotal = (PercentageFirstQuarter + PercentageFourthQuarter + PercentageSecondQuarter + PercentageThirdQuarter) / 4;


                ProfitLoss.Add(new ProfitLossSheetViewModel
                {
                    AccountDesc = ProfitLossStatement.ProfitLossStatementCategory.Name,
                    AccountType = ProfitLossStatement.ProfitLossStatementCategory.ProfitLostType.ToString(),
                    PlanFirst = PlanFirstQuarter,
                    PlanSecond = PlanSecondQuarter,
                    PlanThird = PlanThirdQuarter,
                    PlanFourth = PlanFourthQuarter,
                    DiffrenceFirst = PercentageFirstQuarter,
                    DiffrenceSecond = PercentageSecondQuarter,
                    DiffrenceThird = PercentageThirdQuarter,
                    DiffrenceFourth = PercentageFourthQuarter,
                    AccountCode = ProfitLossStatement.Name,
                    First = FirstQuarter,
                    Fourth = FourthQuarter,
                    Second = SecondQuarter,
                    Third = ThirdQuarter,
                    PercentageTotal = PercentageTotal,
                    ActualTotal = ActualTotal,
                    PlanTotal = PlanTotal
                });

            }
            return ProfitLoss;
        }
    }
}
