using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class ProvidentListObjectDataSource : List<PenstionList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PenstionList> GetMonthlyProvidentReport(string period, string branchId, string user)
        {
            List<PenstionList> list = new List<PenstionList> { };
            PenstionList EmployeeInfo;
            var identifications = db.Identifications.Where(x => x.Type == IdentificationType.PROVIDENT_FUND).ToList();

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
                {
                    foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                    {
                        db = new ExceedDbContext();
                        var id =
                            db.Person_Employee.Where(x => x.EmployeeId == payrolldata.EmployeeId)
                                .FirstOrDefault()
                                .EmployeeCode;
                        EmployeeInfo = new PenstionList();
                        EmployeeInfo.EmpID = id;
                        EmployeeInfo.FullName = payrolldata.FullName;
                        EmployeeInfo.BasicSalary = payrolldata.BasicSalary;
                        EmployeeInfo.PensionFromEmployee = payrolldata.ProvidentFromEmployee;
                        EmployeeInfo.PensionFromCompany = payrolldata.ProvidentFromCompany;
                        EmployeeInfo.TotalPension = payrolldata.TotalProvident;

                        EmployeeInfo.Pension = payrolldata.ProvidentNumber;
                        if (EmployeeInfo.TotalPension > 0)
                        {
                            list.Add(EmployeeInfo);
                        }
                    }
                    return list;
                }
            }
            return list;
        }
    }
}