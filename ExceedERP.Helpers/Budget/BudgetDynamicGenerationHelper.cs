using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ExceedERP.Helpers.Budget;

namespace ExceedERP.Helpers.Budget
{
    public class BudgetDynamicGenerationHelper
    {
        private readonly AccountHelper _accountHelper = new AccountHelper();

        public async Task<bool> GenrateAccountCodeByRange(AccountCodeRangeViewModel range,
            OtherSpecifcationViewModel otherSpecifcation, BudgetGLAccountViewModel budgetGlAccount)
        {
            ExceedDbContext db = new ExceedDbContext();

            int accountLength = _accountHelper.GetAccountCombLength();

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

            var firstSegmentType = db.GlSegmentSetups.FirstOrDefault(x => x.Sequence == 1);
            var secondSegmentType = db.GlSegmentSetups.FirstOrDefault(x => x.Sequence == 2);
            var thirdSegmentType = db.GlSegmentSetups.FirstOrDefault(x => x.Sequence == 3);
            var fourthSegmentType = db.GlSegmentSetups.FirstOrDefault(x => x.Sequence == 4);
            var fivthSegmentType = db.GlSegmentSetups.FirstOrDefault(x => x.Sequence == 5);

            var segmentOneRange = new List<string>();
            var segmentTwoRange = new List<string>();
            var segmentThirdRange = new List<string>();
            var segmentFourthRange = new List<string>();
            var segmentFivthRange = new List<string>();
            string currentAccountCode = string.Empty;
            decimal quota = 0;
            int totalQuota = 0;
            switch (accountLength)
            {
                case 1:
                    seg1From = range.BudgetCategory.FromAccountCode.Split('-')[0];
                    seg1To = range.BudgetCategory.ToAccountCode.Split('-')[0];

                    if (firstSegmentType != null && firstSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentOneRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentOneRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == firstSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    totalQuota = otherSpecifcation.Branches.Count * segmentOneRange.Count;
                    quota = range.PlanCategory.GrandTotal /
                            BudgetPlanHelper.DefalutIntZeroHelper(totalQuota);
                    foreach (var branch in otherSpecifcation.Branches)
                    {
                        foreach (var segment1 in segmentOneRange)
                        {
                            currentAccountCode = segment1;
                            await GenerateBudgetPlan(range, otherSpecifcation, currentAccountCode, branch, quota);
                        }
                    }

                    break;

                case 2:
                    seg1From = range.BudgetCategory.FromAccountCode.Split('-')[0];
                    seg1To = range.BudgetCategory.ToAccountCode.Split('-')[0];
                    seg2From = range.BudgetCategory.FromAccountCode.Split('-')[1];
                    seg2To = range.BudgetCategory.ToAccountCode.Split('-')[1];


                    if (firstSegmentType != null && firstSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentOneRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentOneRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == firstSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (secondSegmentType != null &&
                        secondSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentTwoRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentTwoRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == secondSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    totalQuota = otherSpecifcation.Branches.Count * segmentOneRange.Count *
                                 segmentTwoRange.Count;
                    quota = range.PlanCategory.GrandTotal /
                            BudgetPlanHelper.DefalutIntZeroHelper(totalQuota);
                    foreach (var branch in otherSpecifcation.Branches)
                    {
                        foreach (var segment1 in segmentOneRange)
                        {
                            foreach (var segment2 in segmentTwoRange)
                            {
                                currentAccountCode = segment1 + "-" + segment2;
                                await GenerateBudgetPlan(range, otherSpecifcation, currentAccountCode, branch, quota);
                            }
                        }
                    }

                    break;
                case 3:
                    seg1From = range.BudgetCategory.FromAccountCode.Split('-')[0];
                    seg1To = range.BudgetCategory.ToAccountCode.Split('-')[0];
                    seg2From = range.BudgetCategory.FromAccountCode.Split('-')[1];
                    seg2To = range.BudgetCategory.ToAccountCode.Split('-')[1];
                    seg3From = range.BudgetCategory.FromAccountCode.Split('-')[2];
                    seg3To = range.BudgetCategory.ToAccountCode.Split('-')[2];


                    if (firstSegmentType != null && firstSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentOneRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentOneRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == firstSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (secondSegmentType != null &&
                        secondSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentTwoRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentTwoRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == secondSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (thirdSegmentType != null && thirdSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentThirdRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentThirdRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == thirdSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }


                    totalQuota = otherSpecifcation.Branches.Count * segmentOneRange.Count *
                                 segmentTwoRange.Count * segmentThirdRange.Count;
                    quota = range.PlanCategory.GrandTotal /
                            BudgetPlanHelper.DefalutIntZeroHelper(totalQuota);
                    foreach (var branch in otherSpecifcation.Branches)
                    {
                        foreach (var segment1 in segmentOneRange)
                        {
                            foreach (var segment2 in segmentTwoRange)
                            {
                                foreach (var segment3 in segmentThirdRange)
                                {
                                    currentAccountCode = segment1 + "-" + segment2 + "-" + segment3;

                                    await GenerateBudgetPlan(range, otherSpecifcation, currentAccountCode, branch,
                                        quota);
                                }
                            }
                        }
                    }

                    break;
                case 4:
                    seg1From = range.BudgetCategory.FromAccountCode.Split('-')[0];
                    seg1To = range.BudgetCategory.ToAccountCode.Split('-')[0];
                    seg2From = range.BudgetCategory.FromAccountCode.Split('-')[1];
                    seg2To = range.BudgetCategory.ToAccountCode.Split('-')[1];
                    seg3From = range.BudgetCategory.FromAccountCode.Split('-')[2];
                    seg3To = range.BudgetCategory.ToAccountCode.Split('-')[2];
                    seg4From = range.BudgetCategory.FromAccountCode.Split('-')[3];
                    seg4To = range.BudgetCategory.ToAccountCode.Split('-')[3];


                    if (firstSegmentType != null && firstSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentOneRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentOneRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == firstSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (secondSegmentType != null &&
                        secondSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentTwoRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentTwoRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == secondSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (thirdSegmentType != null && thirdSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentThirdRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentThirdRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == thirdSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (fourthSegmentType != null &&
                        fourthSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentFourthRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg4From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg4To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentFourthRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == fourthSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg4From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg4To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    totalQuota =
                        otherSpecifcation.Branches.Count * segmentOneRange.Count *
                        segmentTwoRange.Count * segmentThirdRange.Count * segmentFourthRange.Count;
                    quota = range.PlanCategory.GrandTotal /
                            BudgetPlanHelper.DefalutIntZeroHelper(totalQuota);
                    foreach (var branch in otherSpecifcation.Branches)
                    {
                        foreach (var segment1 in segmentOneRange)
                        {
                            foreach (var segment2 in segmentTwoRange)
                            {
                                foreach (var segment3 in segmentThirdRange)
                                {
                                    foreach (var segment4 in segmentFourthRange)
                                    {
                                        currentAccountCode =
                                            segment1 + "-" + segment2 + "-" + segment3 + "-" + segment4;


                                        await GenerateBudgetPlan(range, otherSpecifcation, currentAccountCode, branch,
                                            quota);
                                    }
                                }
                            }
                        }
                    }

                    break;
                case 5:
                    seg1From = range.BudgetCategory.FromAccountCode.Split('-')[0];
                    seg1To = range.BudgetCategory.ToAccountCode.Split('-')[0];
                    seg2From = range.BudgetCategory.FromAccountCode.Split('-')[1];
                    seg2To = range.BudgetCategory.ToAccountCode.Split('-')[1];
                    seg3From = range.BudgetCategory.FromAccountCode.Split('-')[2];
                    seg3To = range.BudgetCategory.ToAccountCode.Split('-')[2];
                    seg4From = range.BudgetCategory.FromAccountCode.Split('-')[3];
                    seg4To = range.BudgetCategory.ToAccountCode.Split('-')[3];
                    seg5From = range.BudgetCategory.FromAccountCode.Split('-')[4];
                    seg5To = range.BudgetCategory.ToAccountCode.Split('-')[4];


                    if (firstSegmentType != null && firstSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentOneRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentOneRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == firstSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg1To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (secondSegmentType != null &&
                        secondSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentTwoRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentTwoRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == secondSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg2To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (thirdSegmentType != null && thirdSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentThirdRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentThirdRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == thirdSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg3To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (fourthSegmentType != null &&
                        fourthSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentFourthRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg4From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg4To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentFourthRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == fourthSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg4From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg4To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    if (fivthSegmentType != null && fivthSegmentType.SegmentType == DynamicSegmentTypes.NaturalAccount)
                    {
                        segmentFivthRange = db.GLChartOfAccountAccounts
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg5From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg5To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }
                    else
                    {
                        segmentFivthRange = db.GlChartOfAccountSegments
                            .Where(x => x.GLSegmentSetupId == fivthSegmentType.GLSegmentSetupId)
                            .Where(x => x.Enable && x.AllowBudgeting)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg5From, StringComparison.Ordinal) >= 0)
                            .Where(x => String.Compare(x.Values.ToUpper(), seg5To, StringComparison.Ordinal) <= 0)
                            .Select(x => x.Values)
                            .ToList();
                    }

                    totalQuota = otherSpecifcation.Branches.Count * segmentOneRange.Count * segmentTwoRange.Count *
                                 segmentThirdRange.Count * segmentFourthRange.Count * segmentFivthRange.Count;
                    quota = range.PlanCategory.GrandTotal / BudgetPlanHelper.DefalutIntZeroHelper(totalQuota);

                    foreach (var branch in otherSpecifcation.Branches)
                    {
                        foreach (var segment1 in segmentOneRange)
                        {
                            foreach (var segment2 in segmentTwoRange)
                            {
                                foreach (var segment3 in segmentThirdRange)
                                {
                                    foreach (var segment4 in segmentFourthRange)
                                    {
                                        foreach (var segment5 in segmentFivthRange)
                                        {
                                            currentAccountCode =
                                                segment1 + "-" + segment2 + "-" + segment3 + "-" + segment4 + "-" +
                                                segment5;

                                            await GenerateBudgetPlan(range, otherSpecifcation, currentAccountCode,
                                                branch, quota);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;
            }

            return true;
        }


        public async Task<bool> GenerateBudgetPlan(AccountCodeRangeViewModel range,
            OtherSpecifcationViewModel otherSpecifcation, string accountCode, Branch branch, decimal quota)
        {
            ExceedDbContext db = new ExceedDbContext();
            int costCenterIndex = new DynamicAccountHelper().GetCostCenterAccountSequenceIndex();

            BudgetPlan plan = new BudgetPlan();
            if (branch.BranchTypes == BranchTypes.HEAD_OFFICE)
            {
                plan = new BudgetPlan()
                {
                    BudgetCategoryID = range.PlanCategory.PlanCategoryID,
                    AccountCode = accountCode,
                    BranchId = branch.BranchId,

                    AccountDesc = _accountHelper.GetAccountDesc(accountCode),
                    CurrentYearlyBudget = 0,
                    TotalYearlyBudget = quota,
                    Unit = otherSpecifcation.BudgetCurrency.BudgetCurrencyId
                };
                
                if (costCenterIndex >= 0)
                {
                    var glSetupCostCenter = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);

                    var costCenterValue = accountCode.Split('-')[costCenterIndex];
                    var currentCostCenter = db.GlChartOfAccountSegments
                    .Where(x => x.GLSegmentSetupId == glSetupCostCenter.GLSegmentSetupId)
                    .FirstOrDefault(x => x.Values == costCenterValue);

                    if (currentCostCenter != null)
                    {
                        int costCenterId = 0;
                        int.TryParse(currentCostCenter.Values, out costCenterId);

                        plan.GLChartOfAccountCostCenterId = costCenterId;
                        plan.WorkUnitName = currentCostCenter.Description;
                    }

                }

                db.BudgetPlans.Add(plan);
                await db.SaveChangesAsync();
            }
            else
            {
                plan = new BudgetPlan()
                {
                    BudgetCategoryID = range.PlanCategory.PlanCategoryID,
                    AccountCode = accountCode,
                    BranchId = branch.BranchId,
                    AccountDesc = _accountHelper.GetAccountDesc(accountCode),
                    CurrentYearlyBudget = 0,
                    TotalYearlyBudget = quota,
                    Unit = otherSpecifcation.BudgetCurrency.BudgetCurrencyId
                };

              
                if (costCenterIndex >= 0)
                {
                    var glSetupCostCenter = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);

                    var costCenterValue = accountCode.Split('-')[costCenterIndex];
                    var currentCostCenter = db.GlChartOfAccountSegments
                    .Where(x => x.GLSegmentSetupId == glSetupCostCenter.GLSegmentSetupId)
                    .FirstOrDefault(x => x.Values == costCenterValue);

                    if (currentCostCenter != null)
                    {
                        int costCenterId = 0;
                        int.TryParse(currentCostCenter.Values, out costCenterId);

                        plan.GLChartOfAccountCostCenterId = costCenterId;
                        plan.WorkUnitName = currentCostCenter.Description;
                    }

                }
                db.BudgetPlans.Add(plan);
                await db.SaveChangesAsync();
            }


            if (range.GLPeriods.Any())
            {
                var budgetPlanMonthlys = new List<BudgetPlanMonthly>();
                var months = range.GLPeriods;
                int intervals = months.Count;
                decimal monthBuget = quota / intervals;

                foreach (var month in months)
                {
                    string Currency = string.Empty;
                    if (otherSpecifcation != null && otherSpecifcation.BudgetCurrency != null)
                    {
                        Currency = otherSpecifcation.BudgetCurrency.Name;
                    }
                    var budgetPlanMonthly = new BudgetPlanMonthly
                    {
                        AccountCode = plan.AccountCode,
                        AccountDesc = plan.AccountDesc,
                        GLPeriodId = month.GLPeriodId,
                        MonthlyBudget = monthBuget,
                        CurrentBudget = monthBuget,
                        BudgetDefineID = plan.BudgetPlanID,
                        DateCreated = DateTime.Now,
                        Unit = Currency,
                    };


                    budgetPlanMonthlys.Add(budgetPlanMonthly);

                }

                db.BudgetPlanMonthlys.AddRange(budgetPlanMonthlys);
                await db.SaveChangesAsync();
            }

            return true;
        }
    }
}