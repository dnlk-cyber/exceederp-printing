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
    public class PaySlipObjectDataSource : List<PaySlip>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PaySlip> GetEmployeePaySlipReport(string period, string costCenter, string empStatus, string employee, string branchId, string workUnit, string jobtitle, string user,string addlength)
        {
            List<PaySlip> list = new List<PaySlip> { };
            PaySlip EmployeeInfo = new PaySlip();
            db = new ExceedDbContext();
            if (period == "") return list;

            var perioddetail = db.GLPeriods.FirstOrDefault(x => x.Name == period);
            var bankReference =
                db.BankReferences.Where(x => x.ReferenceType == "Employee" && x.Type == BankAccountType.PAYROLL)
                    .ToList();
            if (employee != null)
            {
                var personEmployee = db.Person_Employee
                    .FirstOrDefault(x => x.EmployeeCode == employee && x.IsActive);

                if (personEmployee != null)
                {
                    var placments =
                        db.Placements
                            .FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId && x.isCurrent);
                    var bank =
                        bankReference.FirstOrDefault(x => x.Reference == personEmployee.EmployeeId.ToString());
                    var idd = personEmployee
                        .EmployeeId;
                    var company = db.Companies.FirstOrDefault(c => c.Category == "0");
                    bool isbasicAddes = false;
                    List<PayrollProcessViewModel> payrolldatas = new List<PayrollProcessViewModel>();


                    payrolldatas =
                        db.PayrollProcessViewModel.Where(
                                x =>
                                    x.PeriodName == period && x.EmployeeId == idd)
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
                            if (perioddetail != null)
                            {
                                EmployeeInfo.FiscalYear = GetFiscalYearName(perioddetail.GLPeriodId);
                            }
                            if (placments != null)
                            {
                                //EmployeeInfo.Deparatment = placments.MainCompanyStructure.Name;
                                var str = db.MainCompanyStructures.Find(placments.OrgStructureId);
                                if (str != null)
                                {
                                    EmployeeInfo.Deparatment = str.Name;
                                }
                            }
                            if (bankReference != null)
                            {
                                //EmployeeInfo.Deparatment = placments.MainCompanyStructure.Name;
                                var str = db.MainCompanyStructures.Find(placments.OrgStructureId);
                                if (str != null)
                                {
                                    EmployeeInfo.Deparatment = str.Name;
                                }
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
                                    EmployeeInfo.PayrollProcessFixedContribution =
                                        new List<PayrollProcessFixedContribution>();
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
                                    EmployeeInfo.PayrollProcessLongTermDeductions =
                                        new List<PayrollProcessLongTermDeductions>();

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
                            EmployeeInfo.TotalDeduction = payrolldata.TotalDeductions;
                            EmployeeInfo.PensionContribution = payrolldata.TotalPension;
                            EmployeeInfo.ProvidentContribution = payrolldata.TotalProvident;
                            EmployeeInfo.FiscalYear = GetFiscalYearName(payrolldata.GLPeriodId);
                            list.Add(EmployeeInfo);
                        }
                    }
                }
            }
            else
            {
                var personEmployee = db.Person_Employee.Where(x => x.IsActive)
                    .ToList();
                if (personEmployee.Any())
                {
                    list = new List<PaySlip> { };               
                    var company = db.Companies.FirstOrDefault(c => c.Category == "0");
                    List<PayrollProcessViewModel> payrolldatas = new List<PayrollProcessViewModel>();


                    payrolldatas =
                        db.PayrollProcessViewModel.Where(
                            x => x.PeriodName == period).ToList();
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

                        if (!String.IsNullOrEmpty(workUnit))
                        {
                            var employeesInWorkUnit =
                                db.Placements.Where(x=>x.isCurrent)
                                    .Where(x => x.OrgStructureId.ToString() == workUnit).Select(x=>x.EmployeeId);
                            payrolldatas =
                                payrolldatas.Where(x => employeesInWorkUnit.Contains(x.EmployeeId)).ToList();
                        }

                        if (!String.IsNullOrEmpty(jobtitle))
                        {
                            var employeesWithJobtitle =
                                db.Placements.Where(x => x.isCurrent)
                                    .Where(x => x.LookupId.ToString() == jobtitle).Select(x => x.EmployeeId);
                            payrolldatas =
                                payrolldatas.Where(x => employeesWithJobtitle.Contains(x.EmployeeId)).ToList();
                        }

                        if (!String.IsNullOrEmpty(costCenter))
                        {
                              payrolldatas =
                                payrolldatas.Where(x => x.CostCenterAccountCode==costCenter).ToList();
                        }

                        if (payrolldatas.Any())
                        {
                            EmployeeInfo.PayrollProcessAllowances = new List<PayrollProcessAllowances>();
                            EmployeeInfo.PayrollProcessAdvances = new List<PayrollProcessAdvances>();
                            EmployeeInfo.PayrollProcessDeductions = new List<PayrollProcessDeductions>();
                            foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                            {
                                db = new ExceedDbContext();
                                var id = payrolldata.Employee.EmployeeCode;
                                var placments =
                                    db.Placements
                                        .FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId && x.isCurrent);
                                var bank =
                                    bankReference
                                        .FirstOrDefault(x => x.Reference == payrolldata.EmployeeId.ToString());
                                EmployeeInfo = new PaySlip();
                                if (perioddetail != null)
                                {
                                    EmployeeInfo.FiscalYear = GetFiscalYearName(perioddetail.GLPeriodId);
                                }
                                if (placments != null)
                                {
                                    //EmployeeInfo.Deparatment = placments.MainCompanyStructure.Name;
                                    var str = db.MainCompanyStructures.Find(placments.OrgStructureId);
                                    if (str != null)
                                    {
                                        EmployeeInfo.Deparatment = str.Name;
                                    }
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
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .ToList();
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
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .ToList();
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
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .ToList();
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
                                        EmployeeInfo.PayrollProcessDeductions.Add(add);
                                    }
                                    else
                                    {
                                        EmployeeInfo.PayrollProcessDeductions.Add(add);
                                    }

                                }

                                var FixedContribution =
                                    db.PayrollProcessFixedContribution.Where(
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .ToList();
                                foreach (PayrollProcessFixedContribution add in FixedContribution)
                                {
                                    if (EmployeeInfo.PayrollProcessFixedContribution == null)
                                    {
                                        EmployeeInfo.PayrollProcessFixedContribution =
                                            new List<PayrollProcessFixedContribution>();
                                        EmployeeInfo.PayrollProcessFixedContribution.Add(add);
                                    }
                                    else
                                    {
                                        EmployeeInfo.PayrollProcessFixedContribution.Add(add);
                                    }
                                }
                                var TermDeductions =
                                    db.PayrollProcessLongTermDeductions.Where(
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .ToList();
                                foreach (PayrollProcessLongTermDeductions add in TermDeductions)
                                {
                                    if (EmployeeInfo.PayrollProcessLongTermDeductions == null)
                                    {
                                        EmployeeInfo.PayrollProcessLongTermDeductions =
                                            new List<PayrollProcessLongTermDeductions>();

                                        EmployeeInfo.PayrollProcessLongTermDeductions.Add(add);
                                    }
                                    else
                                    {
                                        EmployeeInfo.PayrollProcessLongTermDeductions.Add(add);
                                    }
                                }
                                var RegularContribution =
                                    db.PayrollProcessRegularContribution.Where(
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .ToList();
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
                                EmployeeInfo.TotalDeduction = payrolldata.TotalDeductions;
                                EmployeeInfo.PensionContribution = payrolldata.TotalPension;
                                EmployeeInfo.ProvidentContribution = payrolldata.TotalProvident;
                                EmployeeInfo.FiscalYear = GetFiscalYearName(payrolldata.GLPeriodId);
                                var check = list.Where(x => x.EmpID == EmployeeInfo.EmpID).FirstOrDefault();
                                if (check == null)
                                {

                                    list.Add(EmployeeInfo);
                                }

                            }
                        }
                       

                    }
                }
            }

            if (!String.IsNullOrEmpty(addlength) && addlength == "True")
            {
                #region add empty content to make it same length
                int maxAddition = list.Max(x => x.PayrollProcessAdditions.Count);
                int maxDeduction = list.Max(x => x.PayrollProcessDeductions.Count);
                int max = maxAddition > maxDeduction ? maxAddition : maxDeduction;

                foreach (var item in list)
                {
                    if (item.PayrollProcessAdditions.Count < max)
                    {
                        int diff = max - item.PayrollProcessAdditions.Count;
                        for (int i = 0; i < diff; i++)
                        {
                            var r = new PayrollProcessAdditions
                            {
                                Name = "..."
                            };
                            item.PayrollProcessAdditions.Add(r);
                        }
                    }
                    if (item.PayrollProcessDeductions.Count < max)
                    {
                        int diff = max - item.PayrollProcessDeductions.Count;
                        for (int i = 0; i < diff; i++)
                        {
                            var r = new PayrollProcessDeductions
                            {
                                Name = "..."
                            };
                            item.PayrollProcessDeductions.Add(r);
                        }
                    }
                }
                #endregion
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