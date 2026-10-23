//using ExceedERP.Core.Domain.Budget;
//using ExceedERP.Core.Domain.Finance.GL;
//using System.Collections.Generic;
//using System.Linq;

//namespace ExceedERP.Helpers.Budget
//{
//    public class BudgetPlanHelper
//    {
//        public static int DefalutIntZeroHelper(int Value)
//        {
//            if (Value == 0)
//            {
//                return 1;
//            }
//            return Value;
//        }
//        public  void GetQuarterIndex(GlFiscalYear FiscalYear, BudgetEnums.BudgetQuarter BudgetQuarter, out int LastPeriodIndex, out int OffSet)
//        {

//            int OffSetIndex = 0;
//            int LastIndex = 0;

//            int PeriodRange = FiscalYear.GLPeriod.Count / 4;

//            if (BudgetQuarter == BudgetEnums.BudgetQuarter.First)
//            {
//                LastIndex = PeriodRange;
//                OffSetIndex = 0;
//            }
//            if (BudgetQuarter == BudgetEnums.BudgetQuarter.Second)
//            {
//                LastIndex = PeriodRange * 2;
//                OffSetIndex = LastIndex - PeriodRange;
//            }
//            if (BudgetQuarter == BudgetEnums.BudgetQuarter.Third)
//            {
//                LastIndex = PeriodRange * 3;
//                OffSetIndex = LastIndex - PeriodRange;
//            }
//            if (BudgetQuarter == BudgetEnums.BudgetQuarter.Fourth)
//            {
//                LastIndex = PeriodRange * 4;
//                OffSetIndex = LastIndex - PeriodRange;
//            }

//            OffSet = OffSetIndex;
//            LastPeriodIndex = LastIndex;

//        }
//        public  decimal GetQuarterPlanBalance(List<BudgetPlan> BudgetPlans, List<BudgetPlanMonthly> BudgetPlanMonthly)
//        {
//               decimal GrowthBalance = decimal.Zero;
               
//                 foreach (var Plan in BudgetPlans)
//                {

//                    Plan.BudgetPlanMonthly = BudgetPlanMonthly.Where(x => x.BudgetDefineID == Plan.BudgetPlanID).ToList();
//                     GrowthBalance = GrowthBalance + Plan.BudgetPlanMonthly.Sum(y => y.MonthlyBudget);
//                }
//               return GrowthBalance;
//        }

//    }
//}
