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
    public class IncomeTaxObjectDataSource : List<IncomeTaxList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<IncomeTaxList> GetMonthlyIncomeTaxReport(string period, string branchId, string status, string user)
        {
            List<IncomeTaxList> list = new List<IncomeTaxList> { };
            IncomeTaxList EmployeeInfo;

            var identifications = db.Identifications.Where(x => x.Type == IdentificationType.TIN).ToList();

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
                        db = new ExceedDbContext();
                        EmployeeInfo = new IncomeTaxList();
                        var personEmployee = db.Person_Employee
                            .FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId);
                        if (personEmployee != null)
                        {


                            EmployeeInfo.EmpID = personEmployee.EmployeeCode;
                            EmployeeInfo.Status = personEmployee.Status.ToString();
                        }

                        EmployeeInfo.TotalTaxable = payrolldata.BasicSalary
                            + payrolldata.TotalOvertime 
                            + payrolldata.TaxableAllowance
                            + payrolldata.TotalAdditions;
                        EmployeeInfo.NetPay = payrolldata.NetEarning;
                        EmployeeInfo.FullName = payrolldata.FullName;
                        EmployeeInfo.BasicSalary = payrolldata.BasicSalary;
                        EmployeeInfo.TotalOvertime = payrolldata.TotalOvertime;
                        EmployeeInfo.TaxableAllowance = payrolldata.TaxableAllowance;
                        EmployeeInfo.TotalAdditions = payrolldata.TotalAdditions;
                        EmployeeInfo.IncomeTax = payrolldata.IncomeTax;
                        if (identifications.Any())
                        {
                            var tin =
                                identifications.FirstOrDefault(x => x.Reference == payrolldata.EmployeeId.ToString() &&
                                        x.ReferenceType == "Employee");
                            if (tin != null)
                            {
                                EmployeeInfo.TIN = tin.IdNumber;
                            }

                        }
                        list.Add(EmployeeInfo);
                    }
                }
                return list;
            }

            return list;
        }
    }
}