using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Budget
{
    public  class BudgetRequestHelper
    {

        
      
        public  decimal GetAmount(BudgetRequestLine Line)
        {
            if (Line.BudgetRequestType == BudgetRequestType.Equipment_Purchase || Line.BudgetRequestType == BudgetRequestType.Material_Purchase || Line.BudgetRequestType == BudgetRequestType.Other_Expense || Line.BudgetRequestType == BudgetRequestType.Gas_Oil || Line.BudgetRequestType == BudgetRequestType.Other)
            {
                return Line.PurchasedRequestUnit * Line.PurchasedRequestUnitPrice;
            }
            if (Line.BudgetRequestType == BudgetRequestType.Equipment_Rent )
            {
                return Line.RentPerHour * Line.RentTotalHr * Line.PurchasedRequestUnit;
            }
            if (Line.BudgetRequestType == BudgetRequestType.SubContract_Aggrement)
            {
                return Line.PurchasedRequestUnitPrice;
            }
            if (Line.BudgetRequestType == BudgetRequestType.Staff_Monthly_Salary)
            {
                return Line.SalaryPerMonth + Line.ProffiessionalAllowance + Line.DisturbanceAllowance + Line.OverTime;
            }

            if (Line.BudgetRequestType == BudgetRequestType.Skilled_Daily_Labour)
            {
                return Line.DailySalary * Line.NoWorkingDay * Line.PurchasedRequestUnit ;
            }
            return decimal.Zero;
        }
   


        public  decimal GetBranchPeriodPlanAmount(BudgetRequestLine Line)
        {
           

            return decimal.Zero;
        }
    }
}
