using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class CreditandLabourListObjectDataSource : List<CreditandLabourList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CreditandLabourList> GetMonthlyLabourAndCreditReport(string period, string branchId, string user)
        {
            List<CreditandLabourList> list = new List<CreditandLabourList> { };
            CreditandLabourList EmployeeInfo;

            if (!String.IsNullOrEmpty(period))
            {
                var payrolldatas =
                    db.PayrollProcessViewModel.Where(x => x.PeriodName == period).ToList();

                if (branchId != null)
                {
                    List<string> branchs = branchId.Split('-').ToList();
                    payrolldatas = payrolldatas.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
                }
                else if (UserHelper.CheckUserRole(user, "All_Branch_Payroll_Report"))
                {
                    List<string> branchs = UserHelper.GetUserBranchId(user);
                    payrolldatas = payrolldatas.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
                }
                else
                {
                    payrolldatas = null;
                }
                if (payrolldatas != null)
                    foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                    {
                        EmployeeInfo = new CreditandLabourList();
                        db = new ExceedDbContext();
                        var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId);
                        if (personEmployee != null)
                        {
                            var id = personEmployee.EmployeeCode;

                            EmployeeInfo.EmpID = id;
                        }
                        EmployeeInfo.FullName = payrolldata.FullName;
                        EmployeeInfo.BasicSalary = payrolldata.BasicSalary;
                        EmployeeInfo.CreditContribution = payrolldata.AssocationContribution;
                        EmployeeInfo.LabourUnionContribution = payrolldata.LabourUnionContribution;
                        EmployeeInfo.SportFee = payrolldata.SportFee;
                        list.Add(EmployeeInfo);
                    }
                return list;
            }

            return list;
        }
    }
}