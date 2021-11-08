using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.HRM.Payroll;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class MonthlySalaryPerPeriodObjectDataSource : List<MonthlySalaryPerPeriodVM>
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MonthlySalaryPerPeriodVM> GetPayrollBankSlipLists(string period, string type, string branchId, string user, string bank)
        {
            List<MonthlySalaryPerPeriodVM> list = new List<MonthlySalaryPerPeriodVM> { };
            MonthlySalaryPerPeriodVM EmployeeInfo;

            if (!String.IsNullOrEmpty(period))
            {
                var payrolldatas =
                    db.PayrollProcessViewModel.Where(x => x.PeriodName == period)
                        .ToList();


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
                    if (type == "CASH")
                    {
                        payrolldatas = payrolldatas.Where(x => x.BankAccount == null).ToList();
                    }
                    else if (type == "BANK")
                    {
                        payrolldatas = payrolldatas.Where(x => x.BankAccount != null).ToList();

                        if (!string.IsNullOrEmpty(bank))
                        {
                            int bankid = int.Parse(bank);
                            //get list of employee ids that use this bank
                            var bankEmployeeIds = db.BankReferences
                                .Where(x => x.BankId == bankid)
                                .Where(x => x.Type == BankAccountType.PAYROLL).Select(x => x.Reference).ToList();

                            //filter the employees

                            payrolldatas = payrolldatas.Where(x => bankEmployeeIds.Contains(x.EmployeeId.ToString())).ToList();
                        }

                    }
                    else
                    {
                        payrolldatas = payrolldatas.ToList();
                    }
                    foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                    {
                        db = new ExceedDbContext();
                        var personEmployee = db.Person_Employee
                            .FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId);
                        if (personEmployee != null)
                        {
                            var id =
                                personEmployee
                                    .EmployeeCode;
                            var paymentAccount =
                                payrolldatas.FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId);
                            EmployeeInfo = new MonthlySalaryPerPeriodVM();
                            EmployeeInfo.EmpID = id;
                            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId);
                            if (person != null)
                            {
                                EmployeeInfo.EmpFullName = person.FullNameAmharic;
                            }

                            EmployeeInfo.NetEarning = payrolldata.NetEarning;

                            if (paymentAccount != null && paymentAccount.BankAccount != null)
                            {
                                EmployeeInfo.BankAccount = paymentAccount.BankAccount;

                            }
                            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId && x.isCurrent);
                            if (placement != null)
                            {

                                var rank = db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
                                if (rank != null)
                                {
                                    EmployeeInfo.Rank = rank.RankName;
                                }
                                var step = db.Steps.FirstOrDefault(x => x.StepId == placement.StepId);
                                if (step != null)
                                {
                                    EmployeeInfo.Step = step.StepName;
                                }
                                var look = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                                if (look != null)
                                {
                                    EmployeeInfo.JobTitle = look.Description;
                                }
                            }

                            var check = list.FirstOrDefault(x => x.EmpID == EmployeeInfo.EmpID);

                            if (check == null)
                            {
                                list.Add(EmployeeInfo);
                            }
                        }
                    }
                }
                return list;

            }

            return list;
        }
    }
}