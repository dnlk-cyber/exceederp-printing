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
    public class PayrollDetailObjectDataSource : List<PayrollDetailViewmodel>
    {

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PayrollDetailViewmodel> GetDetailReportData(string period, string fromCostCenter, string toCostCenter, string branchId, string workunit, string payment, string user,string bank)
        {
            List<PayrollDetailViewmodel> list = new List<PayrollDetailViewmodel> { };
            PayrollDetailViewmodel EmployeeInfo;

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
                    var Palacmenets = db.Placements.OrderByDescending(x => x.AssignmentDate);
                    var lookups = db.Lookups;

                    if (workunit != null)
                    {
                        var costcenters =
                            db.BranchCostCenter.Where(x => x.OrgStructureId.ToString() == workunit).FirstOrDefault();
                        if (costcenters != null)
                        {


                            payrolldatas =
                                payrolldatas.Where(x => x.CostCenterAccountCode == costcenters.Values).ToList();
                        }
                    }
                    if (payment == "CASH")
                    {
                        payrolldatas = payrolldatas.Where(x => x.BankAccount == null).ToList();
                    }
                    else
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
                    foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                    {
                        db = new ExceedDbContext();
                        var id = payrolldata.Employee.EmployeeCode;

                        EmployeeInfo = new PayrollDetailViewmodel();
                        EmployeeInfo.EmpID = id;
                        EmployeeInfo.EmpFullName = payrolldata.FullName;
                        var place =
                            Palacmenets.Where(x => x.EmployeeId == payrolldata.EmployeeId && x.isCurrent)
                                .FirstOrDefault();


                        if (place != null)
                        {
                            var dept = lookups.Where(x => x.LookupId == place.Department).FirstOrDefault();
                            if (dept != null)
                            {
                                EmployeeInfo.Department = dept.Description;
                                EmployeeInfo.WorkUnitName = dept.Description;
                            }
                        }
                        EmployeeInfo.AccountCode = payrolldata.CostCenterAccountCode;
                        EmployeeInfo.WorkingDays = Convert.ToInt32(Math.Round(30 - payrolldata.DaysAbsent, 2));
                        EmployeeInfo.AccountDesc = payrolldata.CostCenterAccountDesc;
                        EmployeeInfo.BasicSalary = payrolldata.BasicSalary;
                        EmployeeInfo.TotalAbsent = payrolldata.AbsentDeduction;
                        EmployeeInfo.Overtime = payrolldata.TotalOvertime;

                        EmployeeInfo.TotalTaxableAllowance = payrolldata.TaxableAllowance;
                        EmployeeInfo.TotalNonTaxableAllowance = payrolldata.NonTaxableAllowance;

                        EmployeeInfo.OtherAddition = payrolldata.TotalTaxAdditions;
                        EmployeeInfo.TotalTaxablePay = EmployeeInfo.BasicSalary
                                                       + EmployeeInfo.Overtime
                                                       + EmployeeInfo.TotalTaxableAllowance
                                                       + payrolldata.TotalTaxAdditions;

                        EmployeeInfo.TotalNonTaxablePay =
                            +EmployeeInfo.TotalNonTaxableAllowance + payrolldata.TotalNonTaxAdditions;


                        EmployeeInfo.IncomeTax = payrolldata.IncomeTax;
                        EmployeeInfo.Pension = payrolldata.PensionFromEmployee;
                        EmployeeInfo.Provident = payrolldata.ProvidentFromEmployee;
                        EmployeeInfo.ProvidentPension = payrolldata.PensionFromEmployee +
                                                        payrolldata.ProvidentFromEmployee;
                        EmployeeInfo.Loan = payrolldata.TotalLoan;
                        var deductions =
                            db.PayrollProcessDeductions.Where(
                                x =>
                                    x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId &&
                                    x.Name == "CREDIT").FirstOrDefault();
                        if (deductions != null)
                        {
                            EmployeeInfo.SavingCredit = deductions.Amount;
                        }

                        EmployeeInfo.SportFee = payrolldata.SportFee;
                        EmployeeInfo.Advance = payrolldata.TotalAdvance;
                        EmployeeInfo.LabourUnion = payrolldata.LabourUnionContribution;
                        EmployeeInfo.NonTaxAddition = payrolldata.TotalNonTaxAdditions;
                        EmployeeInfo.OtherDeduction =
                            payrolldata.TotalLongTermDeductions
                            + payrolldata.TotalRegularContribution
                            + payrolldata.TotalFixedContribution

                            + payrolldata.TotalPenality
                            + EmployeeInfo.Advance;



                        EmployeeInfo.TotalDeduction = EmployeeInfo.OtherDeduction + EmployeeInfo.IncomeTax +
                                                      EmployeeInfo.Pension + EmployeeInfo.Provident +
                                                      EmployeeInfo.TotalAbsent + EmployeeInfo.Loan +
                                                      EmployeeInfo.SportFee + EmployeeInfo.SavingCredit +
                                                      EmployeeInfo.LabourUnion;
                        EmployeeInfo.NetPay = payrolldata.NetEarning;
                        var bankAccount =
                            db.BankReferences.Where(
                                    x => x.Reference == EmployeeInfo.EmpID && x.ReferenceType == "Employee")
                                .FirstOrDefault();
                        if (bankAccount != null)
                            EmployeeInfo.BankAccount = bankAccount.AccountNumber;
                        list.Add(EmployeeInfo);
                    }
                    return list;
                }
            }

            return list;
        }

    }
}