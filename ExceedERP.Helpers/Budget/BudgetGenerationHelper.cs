using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Budget
{
    #region ViewModels

    public class AccountCodeRangeViewModel
    {
        public PlanCategory PlanCategory { get; set; }
        public BudgetCategory BudgetCategory { get; set; }
        public GlFiscalYear GlFiscalYear { get; set; }
        public List<GLPeriod> GLPeriods { get; set; }
    }

    public class OtherSpecifcationViewModel
    {
        public List<Branch> Branches { get; set; }
        public BudgetCurrency BudgetCurrency { get; set; }
    }

    public class BudgetGLAccountViewModel
    {
        public List<GLChartOfAccountAccount> GlChartOfAccountAccounts { get; set; }
        public List<GLChartOfAccountCostCenter> GlChartOfAccountCostCenters { get; set; }
        public List<GLChartOfAccountLocation> GlChartOfAccountLocations { get; set; }
        public List<GLChartOfAccountSubAccount> GlChartOfAccountSubAccounts { get; set; }
    }

    #endregion


    public class BudgetGenerationHelper
    {
        private readonly BudgetStaticGenerationHelper _staticGenerationHelper = new BudgetStaticGenerationHelper();
        private readonly BudgetDynamicGenerationHelper _dynamicGenerationHelper = new BudgetDynamicGenerationHelper();


        public async Task<bool> GenrateAccountCodeByRange(AccountCodeRangeViewModel range,
            OtherSpecifcationViewModel otherSpecifcation, BudgetGLAccountViewModel budgetGlAccount)
        {
            ExceedDbContext db = new ExceedDbContext();
            AccountHelper accountHelper = new AccountHelper();

            if (accountHelper.IsDynamicCoA())
            {
                return await _dynamicGenerationHelper.GenrateAccountCodeByRange(range, otherSpecifcation,
                    budgetGlAccount);
            }

            budgetGlAccount = new BudgetGLAccountViewModel();

            var glChartOfAccountAccounts =
                db.GLChartOfAccountAccounts.Where(x => x.Enable && x.AllowBudgeting).ToList();
            var glChartOfAccountLocations =
                db.GLChartOfAccountLocations.Where(x => x.Enable && x.AllowBudgeting).ToList();
            var glChartOfAccountCostCenters =
                db.GLChartOfAccountCostCenters.Where(x => x.Enable && x.AllowBudgeting).ToList();
            var glChartOfAccountSubAccounts =
                db.GLChartOfAccountSubAccounts.Where(x => x.Enable && x.AllowBudgeting).ToList();

            budgetGlAccount.GlChartOfAccountAccounts = glChartOfAccountAccounts;
            budgetGlAccount.GlChartOfAccountCostCenters = glChartOfAccountCostCenters;
            budgetGlAccount.GlChartOfAccountLocations = glChartOfAccountLocations;
            budgetGlAccount.GlChartOfAccountSubAccounts = glChartOfAccountSubAccounts;
            return await _staticGenerationHelper.GenrateAccountCodeByRange(range, otherSpecifcation, budgetGlAccount);
        }
    }
}