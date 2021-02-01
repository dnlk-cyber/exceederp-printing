using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class EmployeePaySlipObjectDataSource : List<PaySlip>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PaySlip> GetLoggedinEmployeePaySlipReport(string period, string employee)
        {
            List<PaySlip> list = new List<PaySlip> { };
            PaySlip EmployeeInfo = new PaySlip();
            db = new ExceedDbContext();
            if (period == "") return list;
            var perioddetail = db.GLPeriods.Where(x => x.Name == period).FirstOrDefault();
            var bankReference = db.BankReferences.Where(x => x.ReferenceType == "Employee" && x.Type == BankAccountType.PAYROLL).ToList();
            //int branch = Convert.ToInt32(branchId);
            //var context = new ApplicationExceedDbContext();

            //var allUsers = context.Users.ToList();

            //var name = allUsers.Where(X => X.UserName == User.Identity.Name).FirstOrDefault();
            //if (name != null)
            //{
            //    employee = name.EmployeeId;
            //}
            if (employee != null)
            {
                var personEmployee = db.Person_Employee.Where(x => x.EmployeeCode == employee && x.IsActive)
                    .FirstOrDefault();
                if (personEmployee != null)
                {
                    var idd = personEmployee
                        .EmployeeId;
                    var company = db.Companies.Where(c => c.Category == "0").FirstOrDefault();
                    bool isbasicAddes = false;
                    List<PayrollProcessViewModel> payrolldatas = new List<PayrollProcessViewModel>();

                    var placments =
                        db.Placements.Where(x => x.EmployeeId == personEmployee.EmployeeId && x.isCurrent).FirstOrDefault();
                    var bank = bankReference.Where(x => x.Reference == personEmployee.EmployeeId.ToString()).FirstOrDefault();
                    payrolldatas = db.PayrollProcessViewModel.Where(x => x.PeriodName == period && x.EmployeeId == idd).ToList();

                    EmployeeInfo.PayrollProcessAllowances = new List<PayrollProcessAllowances>();
                    EmployeeInfo.PayrollProcessAdvances = new List<PayrollProcessAdvances>();
                    EmployeeInfo.PayrollProcessDeductions = new List<PayrollProcessDeductions>();
                    foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                    {
                        db = new ExceedDbContext();
                        var id =
                            db.Person_Employee.Where(x => x.EmployeeId == payrolldata.EmployeeId)
                                .FirstOrDefault()
                                .EmployeeCode;
                        EmployeeInfo = new PaySlip();
                        {
                            EmployeeInfo.Deparatment = placments.MainCompanyStructure.Name;
                        }
                        if (bankReference != null)
                        {
                            EmployeeInfo.Deparatment = placments.MainCompanyStructure.Name;
                        }
                        if (bank != null)
                        {
                            EmployeeInfo.BranchName = bank.Bank.Name;
                        }
                        EmployeeInfo.Company = company.TradeName + " " + "PAY SLIP";
                        EmployeeInfo.PayrollPeriodName = period;
                        EmployeeInfo.EmpID = id;
                        EmployeeInfo.EmpFullName = payrolldata.FullName;
                        EmployeeInfo.BasicSalary = payrolldata.BasicSalary;
                        EmployeeInfo.Overtime = payrolldata.TotalOvertime;
                        EmployeeInfo.PayrollPeriodName = payrolldata.PeriodName;
                        EmployeeInfo.CreditAssociation = payrolldata.AssocationContribution;
                        EmployeeInfo.LabourUnion = payrolldata.LabourUnionContribution;
                        EmployeeInfo.NetPay = Math.Round(payrolldata.NetEarning, 2);
                        EmployeeInfo.ServiceCharge = payrolldata.ServiceCharge;
                        EmployeeInfo.IncomeTax = payrolldata.IncomeTax;
                        EmployeeInfo.WorkingDays = Math.Round(30 - payrolldata.DaysAbsent, 2);
                        var additions =
                            db.PayrollProcessAdditions.Where(
                                    x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                .ToList();
                        EmployeeInfo.PayrollProcessAdditions = new List<PayrollProcessAdditions>();

                        foreach (PayrollProcessAdditions add in additions)
                        {
                            if (EmployeeInfo.PayrollProcessAdditions == null)
                            {
                                EmployeeInfo.PayrollProcessAdditions = new List<PayrollProcessAdditions>();

                                EmployeeInfo.PayrollProcessAdditions.Add(add);
                            }
                            else
                            {
                                EmployeeInfo.PayrollProcessAdditions.Add(add);
                            }
                        }

                        var Allowances =
                            db.PayrollProcessAllowances.Where(
                                x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId).ToList();
                        foreach (PayrollProcessAllowances add in Allowances)
                        {
                            if (EmployeeInfo.PayrollProcessAllowances == null)
                            {
                                EmployeeInfo.PayrollProcessAllowances = new List<PayrollProcessAllowances>();
                                EmployeeInfo.PayrollProcessAllowances.Add(add);
                            }
                            else
                            {
                                EmployeeInfo.PayrollProcessAllowances.Add(add);
                            }
                        }
                        var Advances =
                            db.PayrollProcessAdvances.Where(
                                x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId).ToList();
                        foreach (PayrollProcessAdvances add in Advances)
                        {
                            if (EmployeeInfo.PayrollProcessAdvances == null)
                            {
                                EmployeeInfo.PayrollProcessAdvances = new List<PayrollProcessAdvances>();
                                EmployeeInfo.PayrollProcessAdvances.Add(add);
                            }
                            else
                            {
                                EmployeeInfo.PayrollProcessAdvances.Add(add);
                            }
                        }
                        var Loans =
                            db.PayrollProcessLoans.Where(
                                x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId).ToList();
                        foreach (PayrollProcessLoans add in Loans)
                        {
                            if (EmployeeInfo.PayrollProcessLoans == null)
                            {
                                EmployeeInfo.PayrollProcessLoans = new List<PayrollProcessLoans>();
                                EmployeeInfo.PayrollProcessLoans.Add(add);
                            }
                            else
                            {
                                EmployeeInfo.PayrollProcessLoans.Add(add);
                            }
                        }
                        EmployeeInfo.PayrollProcessDeductions = new List<PayrollProcessDeductions>();


                        var Deductions =
                            db.PayrollProcessDeductions.Where(
                                    x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                .OrderBy(x => x.Index)
                                .ToList();
                        foreach (PayrollProcessDeductions add in Deductions)
                        {
                            if (EmployeeInfo.PayrollProcessDeductions == null)
                            {
                                EmployeeInfo.PayrollProcessDeductions = new List<PayrollProcessDeductions>();
                                if (add.Remark == "Discipline")
                                {
                                    add.Name = "Discipline Penalty";
                                }
                                EmployeeInfo.PayrollProcessDeductions.Add(add);
                            }
                            else
                            {
                                if (add.Remark == "Discipline")
                                {
                                    add.Name = "Discipline Penalty";
                                }
                                EmployeeInfo.PayrollProcessDeductions.Add(add);
                            }

                        }

                        var FixedContribution =
                            db.PayrollProcessFixedContribution.Where(
                                x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId).ToList();
                        foreach (PayrollProcessFixedContribution add in FixedContribution)
                        {
                            if (EmployeeInfo.PayrollProcessFixedContribution == null)
                            {
                                EmployeeInfo.PayrollProcessFixedContribution = new List<PayrollProcessFixedContribution>();
                                EmployeeInfo.PayrollProcessFixedContribution.Add(add);
                            }
                            else
                            {
                                EmployeeInfo.PayrollProcessFixedContribution.Add(add);
                            }
                        }
                        var TermDeductions =
                            db.PayrollProcessLongTermDeductions.Where(
                                x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId).ToList();
                        foreach (PayrollProcessLongTermDeductions add in TermDeductions)
                        {
                            if (EmployeeInfo.PayrollProcessLongTermDeductions == null)
                            {
                                EmployeeInfo.PayrollProcessLongTermDeductions = new List<PayrollProcessLongTermDeductions>();

                                EmployeeInfo.PayrollProcessLongTermDeductions.Add(add);
                            }
                            else
                            {
                                EmployeeInfo.PayrollProcessLongTermDeductions.Add(add);
                            }
                        }
                        var RegularContribution =
                            db.PayrollProcessRegularContribution.Where(
                                x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId).ToList();
                        foreach (PayrollProcessRegularContribution add in RegularContribution)
                        {
                            if (EmployeeInfo.PayrollProcessRegularContribution == null)
                            {
                                EmployeeInfo.PayrollProcessRegularContribution =
                                    new List<PayrollProcessRegularContribution>();

                                EmployeeInfo.PayrollProcessRegularContribution.Add(add);
                            }
                            else
                            {
                                EmployeeInfo.PayrollProcessRegularContribution.Add(add);
                            }
                        }
                        EmployeeInfo.FiscalYear = GetFiscalYearName(payrolldata.GLPeriodId);
                        EmployeeInfo.TotalDeduction = payrolldata.TotalDeductions;
                        EmployeeInfo.PensionContribution = payrolldata.TotalPension;
                        EmployeeInfo.ProvidentContribution = payrolldata.TotalProvident;

                        list.Add(EmployeeInfo);
                    }
                }
            }

            return list;



        }
        public string GetFiscalYearName(int glPeriodId)
        {
            var period = db.GLPeriods.Where(x => x.GLPeriodId == glPeriodId).FirstOrDefault();
            if (period != null)
            {
                return period.GlFiscalYear.Name;
            }
            return "";
        }
    }
}