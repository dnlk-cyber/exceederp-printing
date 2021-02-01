using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class AdvanceObjectDataSource : List<AdvanceReport>
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IOrderedEnumerable<AdvanceReport> GetAdvance(string branchId, string period, string user)
        {
            List<AdvanceReport> list = new List<AdvanceReport> { };
            AdvanceReport EmployeeInfo;

            int glPeriodId = 0;
            var periodObj = _db.GLPeriods.FirstOrDefault(x => x.Name == period);
            if (periodObj != null)
            {
                glPeriodId = periodObj.GLPeriodId;
            }

            #region new

            var payrolViewModelIds = _db.PayrollProcessViewModel.Where(x => x.GLPeriodId == glPeriodId).AsQueryable();

            if (branchId != null)
            {
                List<string> branchs = branchId.Split('-').ToList();
                payrolViewModelIds = payrolViewModelIds.Where(x => branchs.Contains(x.BranchId.ToString())).AsQueryable();
            }
            else if (UserHelper.CheckUserRole(user, "All_Branch_Payroll_Report"))
            {
                List<string> branchs = UserHelper.GetUserBranchId(user);
                payrolViewModelIds = payrolViewModelIds.Where(x => branchs.Contains(x.BranchId.ToString())).AsQueryable();
            }
            else
            {
                return list.OrderBy(x => x.FullName);
            }

            List<int> selectedViewIds = payrolViewModelIds.Select(x => x.PayrollProcessViewModelId).ToList();

            var payrolAdvancesViewModels = _db.PayrollProcessAdvances
                                            .Where(x => selectedViewIds
                                            .Contains(x.PayrollProcessViewModelId)).ToList();
           

            foreach (var payrollProcess in payrolAdvancesViewModels)
            {
                var advanceInfo = _db.Advances.Find(payrollProcess.AdvanceId);
                if (advanceInfo != null)
                {
                    var employeeInfo = _db.Person_Employee.Find(advanceInfo.EmployeeId);
                    if (employeeInfo != null)
                    {
                        var employeePayrolViewModelIds = _db.PayrollProcessViewModel.Where(x => x.EmployeeId == employeeInfo.EmployeeId)
                            .Select(x => x.PayrollProcessViewModelId).ToList();

                        var employeePayrolLoanViewModels = _db.PayrollProcessLoans
                            .Where(x => x.LoanId == payrollProcess.AdvanceId)
                            .Where(x => employeePayrolViewModelIds.Contains(x.PayrollProcessViewModelId)).ToList();

                        int remainingMonths = advanceInfo.Months -employeePayrolLoanViewModels.Count();

                        string startingMonth = "";
                        if (employeePayrolLoanViewModels.Any())

                        {
                            int minLoanProcessViewId =
                                employeePayrolLoanViewModels.Min(x => x.PayrollProcessViewModelId);

                            var minviewObj = _db.PayrollProcessViewModel.Find(minLoanProcessViewId);
                            startingMonth = minviewObj?.PeriodName;
                        }                    

                        var advance = new AdvanceReport
                        {
                            EmployeeCode = employeeInfo.EmployeeCode,
                            Balance = payrollProcess.Outstanding,
                            MonthlyAmount = advanceInfo.MonthlyAmount,
                            Month = advanceInfo.Months,
                            StartingMonth = startingMonth,
                            RemainingMonths = remainingMonths,
                            AdvanceAmount = advanceInfo.AdvanceAmount,
                            FullName = employeeInfo.FirstNameEnglish + " " + employeeInfo.MiddleNameEnglish + " " + employeeInfo.LastNameEnglish,
                            Description = advanceInfo.Description,
                            IsActive = advanceInfo.IsActive ? "Yes" : "No"
                        };
                        list.Add(advance);
                    }

                }
                else
                {
                    var payrolViewModel = _db.PayrollProcessViewModel.Find(payrollProcess.PayrollProcessViewModelId);
                    if (payrolViewModel != null)
                    {
                        var employeeInfo = _db.Person_Employee.Find(payrolViewModel.EmployeeId);
                        if (employeeInfo != null)
                        {
                            var advance = new AdvanceReport
                            {
                                EmployeeCode = employeeInfo.EmployeeCode,
                                Balance = payrollProcess.Outstanding,
                                MonthlyAmount = payrollProcess.Amount,
                                Month = 0,
                                AdvanceAmount = 0,
                                FullName = employeeInfo.FirstNameEnglish + " " + employeeInfo.MiddleNameEnglish + " " + employeeInfo.LastNameEnglish,
                                Description = payrollProcess.Name,
                                IsActive = "NA"
                            };
                            list.Add(advance);
                        }
                    }
                }

            }

            return list.OrderBy(x => x.FullName);
            #endregion

            var employee = from e in _db.Person_Employee
                join p in _db.Placements
                    on e.EmployeeId equals p.EmployeeId
                join l in _db.Advances
                    on e.EmployeeId equals l.EmployeeId
                where p.isCurrent && e.IsActive && l.GLPeriodId == glPeriodId
                select (new
                {
                    e.EmployeeId,
                    e.FirstNameEnglish,
                    e.MiddleNameEnglish,
                    e.LastNameEnglish,
                    e.EmployeeCode,
                    l.AdvanceAmount,
                    l.MonthlyAmount,
                    l.Months,
                    l.Balance,
                    l.Description,
                    l.IsActive,
                    p.BranchId
                });
            var laonList = employee.ToList();
            if (branchId != null)
            {
                List<string> branchs = branchId.Split('-').ToList();
                laonList = laonList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
            }
            else if (UserHelper.CheckUserRole(user, "All_Branch_Payroll_Report"))
            {
                List<string> branchs = UserHelper.GetUserBranchId(user);
                laonList = laonList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
            }
            else
            {
                laonList = null;
            }
            if (laonList != null)
                foreach (var l in laonList)
                {
                    EmployeeInfo = new AdvanceReport();
                    EmployeeInfo.EmployeeCode = l.EmployeeCode;
                    EmployeeInfo.Balance = l.Balance;
                    EmployeeInfo.MonthlyAmount = l.MonthlyAmount;
                    EmployeeInfo.Month = l.Months;
                    EmployeeInfo.AdvanceAmount = l.AdvanceAmount;
                    EmployeeInfo.FullName = l.FirstNameEnglish + " " + l.MiddleNameEnglish + " " + l.LastNameEnglish;
                    EmployeeInfo.Description = l.Description;
                    EmployeeInfo.IsActive = l.IsActive.ToString();
                    list.Add(EmployeeInfo);
                }

           // return list;
            
        }

    }
}