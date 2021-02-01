using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class BonusBankPaymentListObjectDataSource : List<BonusBankPaymentList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BonusBankPaymentList> GetBonusBankPaymentListsByYear(string fyear)
        {
            List<BonusBankPaymentList> list = new List<BonusBankPaymentList> { };
            BonusBankPaymentList EmployeeInfo;

            if (!String.IsNullOrEmpty(fyear))
            {
                var bonusdata = db.Bonuses.Where(x => x.GlFiscalYearId.ToString() == fyear).ToList();
                decimal totalAmount = 0;
                foreach (Bonus bonus in bonusdata)
                {
                    EmployeeInfo = new BonusBankPaymentList();
                    EmployeeInfo.EmpID = bonus.PersonEmployee.EmployeeCode;
                    EmployeeInfo.EmpFullName = bonus.PersonEmployee.FirstNameEnglish + " " + bonus.PersonEmployee.MiddleNameEnglish + " " + bonus.PersonEmployee.LastNameEnglish;
                    EmployeeInfo.NetEarning = bonus.TotalBonus - bonus.TotalIncomeTax;
                    EmployeeInfo.BankAccount = db.BankReferences.Where(x => x.ReferenceType == "Employee" && x.Reference == bonus.EmployeeId.ToString()).Select(x => x.AccountNumber).FirstOrDefault();
                    list.Add(EmployeeInfo);

                }
                return list;
            }
            return list;
        }
    }
}