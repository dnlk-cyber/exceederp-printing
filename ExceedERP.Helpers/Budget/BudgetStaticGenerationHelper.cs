using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Budget
{

    public class BudgetStaticGenerationHelper

    {



        public async Task<bool> GenrateAccountCodeByRange(AccountCodeRangeViewModel Range, OtherSpecifcationViewModel OtherSpecifcation, BudgetGLAccountViewModel BudgetGLAccount)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                List<BudgetPlan> BudgetPlans = new List<BudgetPlan>();
                List<BudgetPlanMonthly> BudgetPlanMonthlys = new List<BudgetPlanMonthly>();
                AccountHelper AccountHelper = new AccountHelper();
                try
                {
                    if (Range != null)
                    {
                        string subfromSegment = Range.BudgetCategory.FromAccountCode.Split('-')[3];
                        string subtoSegment = Range.BudgetCategory.ToAccountCode.Split('-')[3];


                        string costfromSegment = Range.BudgetCategory.FromAccountCode.Split('-')[2];
                        string costtoSegment = Range.BudgetCategory.ToAccountCode.Split('-')[2];

                        string comapnyfromSegment = Range.BudgetCategory.FromAccountCode.Split('-')[1];
                        string companytoSegment = Range.BudgetCategory.ToAccountCode.Split('-')[1];


                        string acountfromSegment = Range.BudgetCategory.FromAccountCode.Split('-')[0];
                        string accounttoSegment = Range.BudgetCategory.ToAccountCode.Split('-')[0];



                        List<GLChartOfAccountAccount> Accounts = BudgetGLAccount.GlChartOfAccountAccounts.Where(x => x.Enable && x.AllowBudgeting).Where(x => x.Values.ToUpper().CompareTo(acountfromSegment) >= 0)
                           .Where(x => x.Values.ToUpper().CompareTo(accounttoSegment) <= 0)
                           .ToList();
                        List<GLChartOfAccountSegment> CostCenterAccounts = BudgetGLAccount.GlChartOfAccountCostCenters.Where(x => x.Enable && x.AllowBudgeting).Where(x => x.Values.ToUpper().CompareTo(costfromSegment) >= 0)
                               .Where(x => x.Values.ToUpper().CompareTo(costtoSegment) <= 0)
                               .ToList();
                        List<GLChartOfAccountSegment> LocationAccounts = BudgetGLAccount.GlChartOfAccountLocations.Where(x => x.Enable && x.AllowBudgeting).Where(x => x.Values.ToUpper().CompareTo(comapnyfromSegment) >= 0)
                              .Where(x => x.Values.ToUpper().CompareTo(companytoSegment) <= 0)
                              .ToList();

                        List<GLChartOfAccountSegment> SubAccounts = BudgetGLAccount.GlChartOfAccountSubAccounts.Where(x => x.Enable && x.AllowBudgeting).Where(x => x.Values.ToUpper().CompareTo(subfromSegment) >= 0)
                               .Where(x => x.Values.ToUpper().CompareTo(subtoSegment) <= 0)
                               .ToList();
                        string Currency = string.Empty;

                        if (OtherSpecifcation.BudgetCurrency != null)
                        {
                            Currency = OtherSpecifcation.BudgetCurrency.Name;
                        }

                        var CostCenter = BudgetGLAccount.GlChartOfAccountCostCenters.FirstOrDefault();
                        var TotalDivder = OtherSpecifcation.Branches.Count * Accounts.Count * LocationAccounts.Count * CostCenterAccounts.Count * SubAccounts.Count;
                        var Quota = Range.PlanCategory.GrandTotal / BudgetPlanHelper.DefalutIntZeroHelper(TotalDivder);
                        foreach (var Branch in OtherSpecifcation.Branches)
                        {
                            foreach (var Account in Accounts)
                            {
                                foreach (var LocationAccount in LocationAccounts)
                                {
                                    foreach (var CostCenterAccount in CostCenterAccounts)
                                    {

                                        foreach (var SubAccount in SubAccounts)
                                        {


                                            string AccCode = Account.Values + "-" + LocationAccount.Values + "-" + CostCenterAccount.Values + "-" + SubAccount.Values;
                                            int costCenterId = 0;
                                            int.TryParse(CostCenterAccount.Values, out costCenterId);

                                            BudgetPlan Plan = new BudgetPlan();
                                            if (Branch.BranchTypes == BranchTypes.HEAD_OFFICE)
                                            {

                                                Plan = new BudgetPlan()
                                                {


                                                    PlanCategoryId = Range.PlanCategory.PlanCategoryID,
                                                    AccountCode = AccCode,
                                                    BranchId = Branch.BranchId,
                                                    AccountDesc = AccountHelper.GetAccountDesc(AccCode),
                                                    CurrentYearlyBudget = 0,
                                                    TotalYearlyBudget = Quota,
                                                    Unit = OtherSpecifcation.BudgetCurrency?.FinSetupCurrencyID ?? 0
                                                };
                                                db.BudgetPlans.Add(Plan);
                                                await db.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                Plan = new BudgetPlan()
                                                {
                                                    PlanCategoryId = Range.PlanCategory.PlanCategoryID,
                                                    AccountCode = AccCode,
                                                    BranchId = Branch.BranchId,
                                                    AccountDesc = AccountHelper.GetAccountDesc(AccCode),
                                                    CurrentYearlyBudget = 0,
                                                    TotalYearlyBudget = Quota,
                                                    Unit = OtherSpecifcation.BudgetCurrency?.FinSetupCurrencyID ?? 0
                                                };
                                                db.BudgetPlans.Add(Plan);
                                                await db.SaveChangesAsync();
                                            }


                                            if (Range.GLPeriods.Any())
                                            {
                                                BudgetPlanMonthlys = new List<BudgetPlanMonthly>();
                                                var Months = Range.GLPeriods;
                                                int intervals = Months.Count;
                                                decimal monthBuget = Plan.TotalYearlyBudget / intervals;

                                                foreach (var Month in Months)
                                                {

                                                    var budgetpermonth = new BudgetPlanMonthly
                                                    {
                                                        GLPeriodId = Month.GLPeriodId,
                                                        MonthlyBudget = monthBuget,
                                                        CurrentBudget = monthBuget,
                                                        BudgetDefineID = Plan.BudgetPlanID,
                                                        DateCreated = DateTime.Now,

                                                    };

                                                    BudgetPlanMonthlys.Add(budgetpermonth);


                                                }
                                                db.BudgetPlanMonthlys.AddRange(BudgetPlanMonthlys);
                                                await db.SaveChangesAsync();
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
                return true;
            }
        }

    }


}
