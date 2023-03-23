using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class PayrollSummaryObjectDataSource : List<PayrollSummaryViewmodel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PayrollSummaryViewmodel> GetSummaryReportData(string period, string branchId, string user)
        {
            List<PayrollSummaryViewmodel> list = new List<PayrollSummaryViewmodel> { };
            PayrollSummaryViewmodel EmployeeInfo;

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
                    var costcenters = db.GLChartOfAccountCostCenters.ToList();
                    foreach (GLChartOfAccountCostCenter costcenter in costcenters)
                    {
                        var payrolldataSummary =
                            payrolldatas.Where(x => x.CostCenterAccountCode == costcenter.Values).ToList();

                        if (payrolldataSummary.Any())
                        {
                            EmployeeInfo = new PayrollSummaryViewmodel
                            {
                                AccountCode = costcenter.Values,
                                AccountDesc = costcenter.Description,
                                BasicSalary = payrolldataSummary.Sum(x => x.BasicSalary),
                                TotalAbsent = payrolldataSummary.Sum(x => x.AbsentDeduction),
                                Overtime = payrolldataSummary.Sum(x => x.TotalOvertime),
                                TotalPenality = payrolldataSummary.Sum(x => x.TotalPenality)
                            };
                            EmployeeInfo.Overtime = payrolldataSummary.Sum(x => x.TotalOvertime);
                            EmployeeInfo.TotalTaxableAllowance = payrolldataSummary.Sum(x => x.TaxableAllowance);
                            EmployeeInfo.TotalNonTaxableAllowance =
                                payrolldataSummary.Sum(x => x.NonTaxableAllowance);


                            EmployeeInfo.OtherAddition = payrolldataSummary.Sum(x => x.TotalAdditions);
                            EmployeeInfo.TotalTaxablePay = EmployeeInfo.BasicSalary
                                                           + EmployeeInfo.Overtime
                                                           + EmployeeInfo.TotalTaxableAllowance


                                                           + payrolldataSummary.Sum(x => x.TotalTaxAdditions);
                            ;

                            EmployeeInfo.TotalNonTaxablePay = EmployeeInfo.TotalNonTaxableAllowance +
                                                              payrolldataSummary.Sum(x => x.TotalNonTaxAdditions);

                            EmployeeInfo.IncomeTax = payrolldataSummary.Sum(x => x.IncomeTax);
                            EmployeeInfo.Pension = payrolldataSummary.Sum(x => x.PensionFromEmployee);
                            EmployeeInfo.Provident = payrolldataSummary.Sum(x => x.ProvidentFromEmployee);
                            EmployeeInfo.Loan = payrolldataSummary.Sum(x => x.TotalLoan);
                            EmployeeInfo.Saving = payrolldataSummary.Sum(x => x.AssocationContribution);
                            EmployeeInfo.SportFee = payrolldataSummary.Sum(x => x.SportFee);
                            EmployeeInfo.Advance = payrolldataSummary.Sum(x => x.TotalAdvance);
                            EmployeeInfo.LabourUnion = payrolldataSummary.Sum(x => x.LabourUnionContribution);
                            EmployeeInfo.ProvidentPension = EmployeeInfo.Provident + EmployeeInfo.Pension;
                            EmployeeInfo.OtherDeduction = 
                                                          + payrolldataSummary.Sum(x => x.TotalLongTermDeductions)
                                                          + payrolldataSummary.Sum(x => x.TotalRegularContribution)
                                                          + payrolldataSummary.Sum(x => x.TotalFixedContribution)                                                        
                                                          + EmployeeInfo.Advance
                                                          + EmployeeInfo.TotalPenality; 


                            EmployeeInfo.TotalDeduction = EmployeeInfo.OtherDeduction +
                                                          EmployeeInfo.IncomeTax +
                                                          EmployeeInfo.Pension +
                                                          EmployeeInfo.TotalAbsent +
                                                          EmployeeInfo.Provident+
                                                          EmployeeInfo.Loan +
                                                          EmployeeInfo.Saving +
                                                          EmployeeInfo.SportFee+
                                                          EmployeeInfo.LabourUnion;

                            EmployeeInfo.NetPay = payrolldataSummary.Sum(x => x.NetEarning);
                            list.Add(EmployeeInfo);
                        }
                    }
                }
            }
            else
            {

                var payrolldatas = db.PayrollProcessViewModel.ToList();
                //var costcenters = db.PayrollCostCenters.ToList();
                var costcenters = db.GLChartOfAccountCostCenters.ToList();
                foreach (GLChartOfAccountCostCenter costcenter in costcenters)
                {
                    var payrolldataSummary =
                        payrolldatas.Where(x => x.CostCenterAccountCode == costcenter.Values).ToList();
                    EmployeeInfo = new PayrollSummaryViewmodel();
                    EmployeeInfo.AccountCode = costcenter.Values;
                    EmployeeInfo.AccountDesc = costcenter.Description;
                    EmployeeInfo.BasicSalary = payrolldataSummary.Sum(x => x.BasicSalary);
                    EmployeeInfo.TotalAbsent = payrolldataSummary.Sum(x => x.AbsentDeduction);
                    EmployeeInfo.Overtime = payrolldataSummary.Sum(x => x.TotalOvertime);
                    EmployeeInfo.TotalTaxableAllowance = payrolldataSummary.Sum(x => x.TaxableAllowance);
                    EmployeeInfo.TotalNonTaxableAllowance = payrolldataSummary.Sum(x => x.NonTaxableAllowance);

                    //EmployeeInfo.TotalTaxablePositionAllowance = payrolldataSummary.Sum(x => x.TaxablePositionAllowance);
                    //EmployeeInfo.TotalNonTaxablePositionAllowance = payrolldataSummary.Sum(x => x.NonTaxablePositionAllowance);

                    //EmployeeInfo.TotalTaxableTopUpAllowance = payrolldataSummary.Sum(x => x.TaxableTopUpAllowance);
                    //EmployeeInfo.TotalNonTaxableTopUpAllowance = payrolldataSummary.Sum(x => x.NonTaxableTopUpAllowance);

                    //EmployeeInfo.TotalTaxableProfessionalAllowance = payrolldataSummary.Sum(x => x.TaxableProfessionalAllowance);
                    //EmployeeInfo.TotalNonTaxableProfessionalAllowance = payrolldataSummary.Sum(x => x.NonTaxableProfessionalAllowance);

                    //EmployeeInfo.TotalTaxableTransportAllowance = payrolldataSummary.Sum(x => x.TaxableTransportAllowance);
                    //EmployeeInfo.TotalNonTaxableTransportAllowance = payrolldataSummary.Sum(x => x.NonTaxableTransportAllowance);

                    //EmployeeInfo.TotalTaxableHouseAllowance = payrolldataSummary.Sum(x => x.TaxableHouseAllowance);
                    //EmployeeInfo.TotalNonTaxableHouseAllowance = payrolldataSummary.Sum(x => x.NonTaxableHouseAllowance);


                    EmployeeInfo.OtherAddition = payrolldataSummary.Sum(x => x.TotalAdditions);
                    EmployeeInfo.TotalTaxablePay = EmployeeInfo.BasicSalary
                                                   + EmployeeInfo.Overtime
                                                   + EmployeeInfo.TotalTaxableAllowance

                                                   //+ EmployeeInfo.TotalTaxablePositionAllowance
                                                   //+ EmployeeInfo.TotalTaxableTopUpAllowance
                                                   //+ EmployeeInfo.TotalTaxableProfessionalAllowance
                                                   //+ EmployeeInfo.TotalTaxableTransportAllowance
                                                   //+ EmployeeInfo.TotalTaxableHouseAllowance
                                                   + EmployeeInfo.OtherAddition;

                    EmployeeInfo.TotalNonTaxablePay = EmployeeInfo.TotalNonTaxableAllowance;
                    //+EmployeeInfo.TotalNonTaxablePositionAllowance
                    //+ EmployeeInfo.TotalNonTaxableTopUpAllowance
                    //+ EmployeeInfo.TotalNonTaxableProfessionalAllowance
                    //+ EmployeeInfo.TotalNonTaxableTransportAllowance
                    //+ EmployeeInfo.TotalNonTaxableHouseAllowance;

                    EmployeeInfo.IncomeTax = payrolldataSummary.Sum(x => x.IncomeTax);
                    EmployeeInfo.Pension = payrolldataSummary.Sum(x => x.PensionFromEmployee);
                    EmployeeInfo.Provident = payrolldataSummary.Sum(x => x.ProvidentFromEmployee);
                    EmployeeInfo.Loan = payrolldataSummary.Sum(x => x.TotalLoan);
                    EmployeeInfo.SavingCredit = payrolldataSummary.Sum(x => x.AssocationContribution);
                    EmployeeInfo.Advance = payrolldataSummary.Sum(x => x.TotalAdvance);
                    EmployeeInfo.LabourUnion = payrolldataSummary.Sum(x => x.LabourUnionContribution);

                    EmployeeInfo.OtherDeduction = payrolldataSummary.Sum(x => x.TotalDeductions)
                                                  + payrolldataSummary.Sum(x => x.TotalLongTermDeductions)
                                                  + payrolldataSummary.Sum(x => x.TotalRegularContribution)
                                                  + payrolldataSummary.Sum(x => x.TotalFixedContribution)
                                                  + EmployeeInfo.Loan
                                                  + EmployeeInfo.SavingCredit
                                                  + EmployeeInfo.Advance
                                                  + EmployeeInfo.LabourUnion;


                    EmployeeInfo.TotalDeduction = EmployeeInfo.OtherDeduction + EmployeeInfo.IncomeTax +
                                                  EmployeeInfo.Pension + EmployeeInfo.TotalAbsent +
                                                  EmployeeInfo.Provident;
                    EmployeeInfo.NetPay = payrolldataSummary.Sum(x => x.NetEarning);
                    list.Add(EmployeeInfo);
                }
            }



            return list;
        }
    }
}