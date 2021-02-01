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
    public class PenstionListObjectDataSource : List<PenstionList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PenstionList> GetMonthlyPensionReport(string period, string branchId, string status, string user)
        {
            List<PenstionList> list = new List<PenstionList> { };
            PenstionList EmployeeInfo;
            var identifications = db.Identifications.Where(x => x.Type == IdentificationType.PENSION_NO).ToList();

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
                    if (status != null)
                    {
                        var selected = from p in payrolldatas
                            join e in db.Person_Employee
                                on p.EmployeeId equals e.EmployeeId
                            where e.Status.ToString() == status
                            select (new
                            {
                                p
                            });
                        payrolldatas = selected.Select(x => x.p).ToList();
                    }

                    foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                    {
                        EmployeeInfo = new PenstionList();
                        db = new ExceedDbContext();
                        var personEmployee = db.Person_Employee
                            .FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId);
                        if (personEmployee != null)
                        {
                            var id =
                                personEmployee
                                    .EmployeeCode;

                            EmployeeInfo.EmpID = id;
                            EmployeeInfo.Status = personEmployee.Status.ToString();
                        }
                        EmployeeInfo.FullName = payrolldata.FullName;
                        EmployeeInfo.BasicSalary = payrolldata.BasicSalary;
                        EmployeeInfo.PensionFromEmployee = payrolldata.PensionFromEmployee;
                        EmployeeInfo.PensionFromCompany = payrolldata.PensionFromCompany;
                        EmployeeInfo.TotalPension = payrolldata.TotalPension;

                        EmployeeInfo.Pension = payrolldata.PensionNumber;
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