using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Finance.GL;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Finance;

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
        public FinSetupCurrency BudgetCurrency { get; set; }
    }

    public class BudgetGLAccountViewModel
    {
        public List<GLChartOfAccountAccount> GlChartOfAccountAccounts { get; set; }
        public List<GLChartOfAccountSegment> GlChartOfAccountCostCenters { get; set; }
        public List<GLChartOfAccountSegment> GlChartOfAccountLocations { get; set; }
        public List<GLChartOfAccountSegment> GlChartOfAccountSubAccounts { get; set; }
    }

    #endregion


    public class BudgetGenerationHelper
    {
        //private readonly BudgetStaticGenerationHelper _staticGenerationHelper = new BudgetStaticGenerationHelper();
        private readonly BudgetDynamicGenerationHelper _dynamicGenerationHelper = new BudgetDynamicGenerationHelper();


        public async Task<bool> GenrateAccountCodeByRange(AccountCodeRangeViewModel range,
            OtherSpecifcationViewModel otherSpecifcation, BudgetGLAccountViewModel budgetGlAccount)
        {
            //using (ExceedDbContext db = new ExceedDbContext())
            //{
                //AccountHelper accountHelper = new AccountHelper();

                return await _dynamicGenerationHelper.GenrateAccountCodeByRange(range, otherSpecifcation,
                   budgetGlAccount);

                //budgetGlAccount = new BudgetGLAccountViewModel();

                //var glChartOfAccountAccounts =
                //    db.GLChartOfAccountAccounts.Where(x => x.Enable && x.AllowBudgeting).ToList();

                //var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
                //var glChartOfAccountLocations =
                //    db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId)
                //    .Where(x => x.Enable && x.AllowBudgeting).ToList();

                //var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                //var glChartOfAccountCostCenters =
                //    db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId)
                //    .Where(x => x.Enable && x.AllowBudgeting).ToList();

                //var subAccountSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
                //var glChartOfAccountSubAccounts =
                //    db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId)
                //    .Where(x => x.Enable && x.AllowBudgeting).ToList();

                //budgetGlAccount.GlChartOfAccountAccounts = glChartOfAccountAccounts;
                //budgetGlAccount.GlChartOfAccountCostCenters = glChartOfAccountCostCenters;
                //budgetGlAccount.GlChartOfAccountLocations = glChartOfAccountLocations;
                //budgetGlAccount.GlChartOfAccountSubAccounts = glChartOfAccountSubAccounts;
                //return await _staticGenerationHelper.GenrateAccountCodeByRange(range, otherSpecifcation, budgetGlAccount);
           // }
        }
    }
}