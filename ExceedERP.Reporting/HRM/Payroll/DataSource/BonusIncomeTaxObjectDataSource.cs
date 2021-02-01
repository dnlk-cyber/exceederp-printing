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
    public class BonusIncomeTaxObjectDataSource : List<BonusIncomeTaxList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BonusIncomeTaxList> GetBonusIncomeTaxReportData(string fyear)
        {
            List<BonusIncomeTaxList> list = new List<BonusIncomeTaxList> { };
            BonusIncomeTaxList EmployeeInfo;

            if (!String.IsNullOrEmpty(fyear))
            {
                var bonusdata = db.Bonuses.Where(x => x.GlFiscalYearId.ToString() == fyear).ToList();
                decimal totalAmount = 0;
                foreach (Bonus bonus in bonusdata)
                {
                    EmployeeInfo = new BonusIncomeTaxList();
                    EmployeeInfo.EmpID = bonus.PersonEmployee.EmployeeCode;
                    EmployeeInfo.FullName = bonus.PersonEmployee.FirstNameEnglish + " " + bonus.PersonEmployee.MiddleNameEnglish + " " + bonus.PersonEmployee.LastNameEnglish;
                    EmployeeInfo.IncomeTax = bonus.TotalIncomeTax;
                    EmployeeInfo.BasicSalary = bonus.TotalBonus;
                    list.Add(EmployeeInfo);

                }
                return list;

            }
            return list;
        }
    }
}