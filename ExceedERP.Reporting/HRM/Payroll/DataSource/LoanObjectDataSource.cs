using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class LoanObjectDataSource : List<LoanReport>
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IOrderedEnumerable<LoanReport> GetLoan(string branchId, string period, string user)
        {
            List<LoanReport> list = new List<LoanReport> { };
            int glPeriodId = 0;
            var periodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period);
            if (periodObj != null)
            {
                glPeriodId = periodObj.GLPeriodId;
            }

            #region new

            var payrolViewModels = _db.PayrollProcessViewModel.Where(x => x.GLPeriodId == glPeriodId).AsQueryable();

            if (!String.IsNullOrEmpty(branchId))
            {
                int did = int.Parse(branchId);
                payrolViewModels = payrolViewModels.Where(x => x.BranchId == did);
            }

            var payrolViewModelIds = payrolViewModels.Select(x => x.PayrollProcessViewModelId).ToList();

            var payrolLoanViewModels = _db.PayrollProcessLoans.Where(x => payrolViewModelIds.Contains(x.PayrollProcessViewModelId)).ToList();

            foreach (var payrollProcessLoan in payrolLoanViewModels)
            {
                var loanInfo = _db.Loans.Find(payrollProcessLoan.LoanId);
                if (loanInfo != null)
                {
                    var employeeInfo = _db.Person_Employee.Find(loanInfo.EmployeeId);
                    if (employeeInfo != null)
                    {
                        var employeePayrolViewModelIds = _db.PayrollProcessViewModel.Where(x => x.EmployeeId == employeeInfo.EmployeeId)
                            .Select(x => x.PayrollProcessViewModelId).ToList();

                        var employeePayrolLoanViewModels = _db.PayrollProcessLoans
                            .Where(x=>x.LoanId== payrollProcessLoan.LoanId)
                            .Where(x => employeePayrolViewModelIds.Contains(x.PayrollProcessViewModelId)).ToList();

                        int remainingMonths = loanInfo.Months-employeePayrolLoanViewModels.Count;

                        string startingMonth = "";
                        if (employeePayrolLoanViewModels.Any())

                        {
                            int minLoanProcessViewId =
                                employeePayrolLoanViewModels.Min(x => x.PayrollProcessViewModelId);

                            var minviewObj = _db.PayrollProcessViewModel.Find(minLoanProcessViewId);
                            startingMonth = minviewObj?.PeriodName;
                        }

                        var loan = new LoanReport
                        {
                            EmployeeCode = employeeInfo.EmployeeCode,
                            Balance = payrollProcessLoan.Outstanding,
                            MonthlyAmount = loanInfo.MonthlyAmount,
                            Month = loanInfo.Months,
                            StartingMonth =startingMonth,
                            RemainingMonths = remainingMonths,
                            LoanAmount = loanInfo.LoanAmount,
                            FullName = employeeInfo.FirstNameEnglish + " " + employeeInfo.MiddleNameEnglish + " " + employeeInfo.LastNameEnglish,
                            Description = loanInfo.Description,
                            IsActive = loanInfo.IsActive?"Yes":"No"
                        };
                        list.Add(loan);
                    }
                   
                }
                else
                {
                    var payrolViewModel =_db.PayrollProcessViewModel.Find(payrollProcessLoan.PayrollProcessViewModelId);
                    if (payrolViewModel != null)
                    {
                        var employeeInfo = _db.Person_Employee.Find(payrolViewModel.EmployeeId);
                        if (employeeInfo != null)
                        {
                            var loan = new LoanReport
                            {
                                EmployeeCode = employeeInfo.EmployeeCode,
                                Balance = payrollProcessLoan.Outstanding,
                                MonthlyAmount = payrollProcessLoan.Amount,
                                Month = 0,
                                LoanAmount =0,
                                FullName = employeeInfo.FirstNameEnglish + " " + employeeInfo.MiddleNameEnglish + " " + employeeInfo.LastNameEnglish,
                                Description = payrollProcessLoan.Name,
                                IsActive = "NA"
                            };
                            list.Add(loan);
                        }
                    }
                }
                
            }

            return list.OrderBy(x=>x.FullName);
            #endregion

            var employee = from e in _db.Person_Employee
                join p in _db.Placements
                    on e.EmployeeId equals p.EmployeeId
                join l in _db.Loans
                    on e.EmployeeId equals l.EmployeeId
                where l.GLPeriodId == glPeriodId
                select (new
                {
                    e.EmployeeId,
                    e.FirstNameEnglish,
                    e.MiddleNameEnglish,
                    e.LastNameEnglish,
                    e.EmployeeCode,
                    l.LoanAmount,
                    l.MonthlyAmount,
                    l.Months,
                    l.Balance,
                    l.Description,
                    l.IsActive,
                    p.BranchId
                });
            var loanList = employee.ToList();

            if (branchId != null)
            {
                List<string> branchs = branchId.Split('-').ToList();
                loanList = loanList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
            }
            else if (!UserHelper.CheckUserRole(user, "All_Branch_Payroll_Report"))
            {
                List<string> branchs = UserHelper.GetUserBranchId(user);
                loanList = loanList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
            }
            else if (UserHelper.CheckUserRole(user, "All_Branch_Payroll_Report"))
            {
                //no need to filter since he priviledge for all
                //List<string> branchs = UserHelper.GetUserBranchId(user);
                //loanList = loanList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
            }
          
                

           
        }
    }
}