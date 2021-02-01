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
    public class DynamicColumnObjectDataSource : List<DynamicColumn>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DynamicColumn> GetAdvance(string period, string branchId, string workunit, string payment, string user)
        {
            List<DynamicColumn> list = new List<DynamicColumn>();
            DynamicColumn one = new DynamicColumn();
            var lookups = db.Lookups.ToList();
            var contributionNames =
                db.ContributionTypes.Where(x => x.ContributionCategory == ContributionCategory.FIXED).ToList();
            var regularcontributionNames =
                db.ContributionTypes.Where(x => x.ContributionCategory == ContributionCategory.REGULAR).ToList();
            //var contributions = db.ContributionTypes.ToList();
            if (!String.IsNullOrEmpty(period))
            {
                var payrolldatas =
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
                    if (workunit != null)
                    {
                        //var costcenters =
                        //    db.BranchCostCenter.FirstOrDefault(x => x.OrgStructureId.ToString() == workunit);
                        //if (costcenters != null)
                        //{

                        //    payrolldatas =
                        //        payrolldatas.Where(x => x.CostCenterAccountCode == costcenters.Values).ToList();
                        //}

                        payrolldatas =
                            payrolldatas.Where(x => x.CostCenterAccountCode == workunit).ToList();
                    }

                    if (payment == "CASH")
                    {
                        payrolldatas = payrolldatas.Where(x => x.BankAccount == null).ToList();
                    }
                    else if (payment == "BANK")
                    {
                        payrolldatas = payrolldatas.Where(x => x.BankAccount != null).ToList();

                    }
                    else
                    {
                        payrolldatas = payrolldatas.ToList();
                    }
                    int index = 0;
                    foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                    {
                        index++;
                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Cost Center";
                        one.cellContent = payrolldata.CostCenterAccountCode;
                        one.values = 0;
                        list.Add(one);

                        db = new ExceedDbContext();
                        var id = payrolldata.Employee.EmployeeCode;

                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Employee Id";
                        one.cellContent = id;
                        one.values = 0;
                        list.Add(one);
                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Full Name";
                        one.cellContent = payrolldata.FullName;
                        one.values = 0;
                        list.Add(one);




                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Working Days";
                        one.cellContent = Convert.ToInt32(Math.Round(30 - payrolldata.DaysAbsent, 2)).ToString();
                        one.values = 30 - payrolldata.DaysAbsent;
                        list.Add(one);
                        //one = new DynamicColumn();
                        //one.rowNumber = index;
                        //one.columnName = "AccDescription";
                        //one.cellContent = payrolldata.CostCenterAccountDesc;
                        //list.Add(one);
                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Basic Salary";
                        one.cellContent = payrolldata.BasicSalary.ToString("0.##");
                        one.values = payrolldata.BasicSalary;
                        list.Add(one);

                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Absent";
                        one.cellContent = payrolldata.AbsentDeduction.ToString("0.##");
                        one.values = payrolldata.AbsentDeduction;
                        list.Add(one);

                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Over Time";
                        one.cellContent = payrolldata.TotalOvertime.ToString("0.##");
                        one.values = payrolldata.TotalOvertime;
                        list.Add(one);


                        #region  Taxable Allowances

                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "T-House Allowance";
                        one.cellContent = payrolldata.TaxableHouseAllowance.ToString("0.##");
                        one.values = payrolldata.TaxableHouseAllowance;
                        list.Add(one);
                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "T-Position Allowance";
                        one.cellContent = payrolldata.TaxablePositionAllowance.ToString("0.##");
                        one.values = payrolldata.TaxablePositionAllowance;
                        list.Add(one);
                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "T-Professional Allowance";
                        one.cellContent = payrolldata.TaxableProfessionalAllowance.ToString("0.##");
                        one.values = payrolldata.TaxableProfessionalAllowance;
                        list.Add(one);
                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "T-Project Allowance";
                        one.cellContent = payrolldata.TaxableProjectAllowance.ToString("0.##");
                        one.values = payrolldata.TaxableProjectAllowance;
                        list.Add(one);

                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "T-Responsiblity Allowance";
                        one.cellContent = payrolldata.TaxableResponsiblityAllowance.ToString("0.##");
                        one.values = payrolldata.TaxableResponsiblityAllowance;
                        list.Add(one);

                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "T-Telephone Allowance";
                        one.cellContent = payrolldata.TaxableTelephoneAllowance.ToString("0.##");
                        one.values = payrolldata.TaxableTelephoneAllowance;
                        list.Add(one);

                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "T-TopUp Allowance";
                        one.cellContent = payrolldata.TaxableTopUpAllowance.ToString("0.##");
                        one.values = payrolldata.TaxableTopUpAllowance;
                        list.Add(one);


                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "T-Transport Allowance";
                        one.cellContent = payrolldata.TaxableTransportAllowance.ToString("0.##");
                        one.values = payrolldata.TaxableTransportAllowance;
                        list.Add(one);
                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "T-Desert Allowance";
                        one.cellContent = payrolldata.TaxableDesertAllowance.ToString("0.##");
                        one.values = payrolldata.TaxableDesertAllowance;
                        list.Add(one);
                        //one = new DynamicColumn();

                        //one.rowNumber = index;
                        //one.columnName = "NT-Transport Allowance";
                        //one.cellContent = payrolldata.NonTaxableAllowance.ToString("0.##");
                        //one.values = payrolldata.NonTaxableTransportAllowance;
                        //list.Add(one);

                        #endregion

                        //one = new DynamicColumn();

                        //one.rowNumber = index;
                        //one.columnName = "Taxable Allowance";
                        //one.cellContent = payrolldata.TaxableAllowance.ToString("0.##");
                        //one.values = payrolldata.TaxableAllowance;
                        //list.Add(one);

                        var additions = db.Additions
                            .Where(x => x.GLPeriodId == payrolldata.GLPeriodId)
                            .Where(a => a.AdditionTaxType == AdditionTaxType.TAXABLE);

                        foreach (var a in additions)
                        {
                            var payrolladditions =
                                db.PayrollProcessAdditions.FirstOrDefault(
                                    x =>
                                        x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId &&
                                        x.AdditionId == a.LookupId);

                            if (payrolladditions != null)
                            {
                                var exist = list
                                    .Where(x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                    .Where(x => x.columnName == payrolladditions.Name);
                                if (!exist.Any())
                                {
                                    one = new DynamicColumn();
                                    one.rowNumber = index;
                                    one.PayrollProcessViewModelId = payrolldata.PayrollProcessViewModelId;
                                    one.columnName = payrolladditions.Name;
                                    one.cellContent = (payrolladditions.Amount).ToString("0.##");
                                    one.values = payrolladditions.Amount;
                                    list.Add(one);
                                }

                            }
                            else
                            {
                                string lookupname = lookups.FirstOrDefault(x => x.LookupId == a.LookupId)?.Description;
                                var exist = list
                                    .Where(x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                    .Where(x => x.columnName == lookupname);
                                if (!exist.Any())
                                {
                                    one = new DynamicColumn();
                                    one.rowNumber = index;
                                    one.PayrollProcessViewModelId = payrolldata.PayrollProcessViewModelId;
                                    var name = lookups.FirstOrDefault(x => x.LookupId == a.LookupId);
                                    if (name != null) one.columnName = name.Description;
                                    one.cellContent = "0.00";
                                    one.values = 0;
                                    list.Add(one);
                                    //saveCredit = deductions.Amount;
                                }
                            }
                        }

                        //one = new DynamicColumn();
                        //one.rowNumber = index;
                        //one.columnName = " Taxable Additions";
                        //one.cellContent = payrolldata.TotalTaxAdditions.ToString("0.##");
                        //one.values = payrolldata.TotalTaxAdditions;
                        //list.Add(one);

                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Total Taxable Pay";
                        var totalTaxPay = (payrolldata.BasicSalary
                                           + payrolldata.TotalOvertime
                                           + payrolldata.TaxableAllowance
                                           + payrolldata.TotalTaxAdditions);
                        one.cellContent = totalTaxPay.ToString("0.##");

                        one.values = totalTaxPay;
                        list.Add(one);

                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Income Tax";
                        one.cellContent = (payrolldata.IncomeTax).ToString("0.##");
                        one.values = payrolldata.IncomeTax;
                        list.Add(one);


                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Provident/Pension Employee";
                        var providentEMp = payrolldata.PensionFromEmployee + payrolldata.ProvidentFromEmployee;
                        one.cellContent = providentEMp.ToString("0.##");
                        one.values = providentEMp;
                        list.Add(one);


                        decimal saveCredit = 0;
                        var deductions =
                            db.PayrollProcessDeductions.Where(
                                x =>
                                    x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId &&
                                    x.Name == "CREDIT").FirstOrDefault();
                        if (deductions != null)
                        {
                            one = new DynamicColumn();
                            one.rowNumber = index;
                            one.columnName = "Saving";
                            one.cellContent = (deductions.Amount).ToString("0.##");
                            one.values = deductions.Amount;
                            list.Add(one);
                            saveCredit = deductions.Amount;
                        }
                        else
                        {
                            one = new DynamicColumn();
                            one.rowNumber = index;
                            one.columnName = "Saving";
                            one.cellContent = "0.00";
                            one.values = 0;
                            list.Add(one);
                            //saveCredit = deductions.Amount;
                        }

                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Loan";
                        one.cellContent = (payrolldata.TotalLoan).ToString("0.##");
                        one.values = payrolldata.TotalLoan;
                        list.Add(one);



                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Sport Fee";
                        one.cellContent = (payrolldata.SportFee).ToString("0.##");
                        one.values = payrolldata.SportFee;
                        list.Add(one);

                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Advance";
                        one.cellContent = (payrolldata.TotalAdvance).ToString("0.##");
                        one.values = payrolldata.TotalAdvance;
                        list.Add(one);

                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Labour Union";
                        one.cellContent = (payrolldata.LabourUnionContribution).ToString("0.##");
                        one.values = payrolldata.LabourUnionContribution;
                        list.Add(one);









                        foreach (var c in contributionNames)
                        {

                            var fixedContributions =
                                db.PayrollProcessFixedContribution.Where(
                                    x =>
                                        x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId &&
                                        c.Description == x.Name).FirstOrDefault();
                            if (fixedContributions != null)
                            {

                                one = new DynamicColumn();
                                one.rowNumber = index;
                                one.columnName = fixedContributions.Name;
                                one.cellContent = (fixedContributions.Amount).ToString("0.##");
                                one.values = fixedContributions.Amount;
                                list.Add(one);


                            }
                            else
                            {
                                one = new DynamicColumn();
                                one.rowNumber = index;
                                one.columnName = c.Description;
                                one.cellContent = "0.00";
                                one.values = 0;
                                list.Add(one);

                            }
                        }


                        foreach (var c in regularcontributionNames)
                        {
                            var regularContributions =
                                db.PayrollProcessRegularContribution.Where(
                                    x =>
                                        x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId &&
                                        x.Name == c.Description).FirstOrDefault();
                            if (regularContributions != null)
                            {

                                one = new DynamicColumn();
                                one.rowNumber = index;
                                one.columnName = regularContributions.Name;
                                one.cellContent = (regularContributions.Amount).ToString("0.##");
                                one.values = regularContributions.Amount;
                                list.Add(one);


                            }
                            else
                            {

                                one = new DynamicColumn();
                                one.rowNumber = index;
                                one.columnName = c.Description;
                                one.cellContent = "0.00";
                                one.values = 0;
                                list.Add(one);
                            }
                        }
                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Penality";
                        one.cellContent = (payrolldata.TotalPenality).ToString("0.##");
                        one.values = payrolldata.TotalPenality;
                        list.Add(one);

                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Total Deduction";
                        var totaldesuction = (payrolldata.TotalLongTermDeductions
                                              + payrolldata.TotalRegularContribution
                                              + payrolldata.TotalFixedContribution

                                              + payrolldata.TotalPenality
                                              + payrolldata.TotalAdvance + payrolldata.IncomeTax +
                                              payrolldata.PensionFromEmployee + payrolldata.ProvidentFromEmployee +
                                              payrolldata.AbsentDeduction + payrolldata.TotalLoan +
                                              payrolldata.SportFee +
                                              saveCredit + payrolldata.LabourUnionContribution);
                        one.cellContent = totaldesuction.ToString("0.##");
                        one.values = totaldesuction;
                        list.Add(one);

                        #region Non Taxable Allowances

                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "NT-House Allowance";
                        one.cellContent = payrolldata.NonTaxableHouseAllowance.ToString("0.##");
                        one.values = payrolldata.NonTaxableHouseAllowance;
                        list.Add(one);
                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "NT-Position Allowance";
                        one.cellContent = payrolldata.NonTaxablePositionAllowance.ToString("0.##");
                        one.values = payrolldata.NonTaxablePositionAllowance;
                        list.Add(one);
                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "NT-Professional Allowance";
                        one.cellContent = payrolldata.NonTaxableProfessionalAllowance.ToString("0.##");
                        one.values = payrolldata.NonTaxableProfessionalAllowance;
                        list.Add(one);
                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "NT-Project Allowance";
                        one.cellContent = payrolldata.NonTaxableProjectAllowance.ToString("0.##");
                        one.values = payrolldata.NonTaxableProjectAllowance;
                        list.Add(one);

                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "NT-Responsiblity Allowance";
                        one.cellContent = payrolldata.NonTaxableResponsiblityAllowance.ToString("0.##");
                        one.values = payrolldata.NonTaxableResponsiblityAllowance;
                        list.Add(one);

                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "NT-Telephone Allowance";
                        one.cellContent = payrolldata.NonTaxableTelephoneAllowance.ToString("0.##");
                        one.values = payrolldata.NonTaxableTelephoneAllowance;
                        list.Add(one);

                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "NT-TopUp Allowance";
                        one.cellContent = payrolldata.NonTaxableTopUpAllowance.ToString("0.##");
                        one.values = payrolldata.NonTaxableTopUpAllowance;
                        list.Add(one);


                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "NT-Transport Allowance";
                        one.cellContent = payrolldata.NonTaxableTransportAllowance.ToString("0.##");
                        one.values = payrolldata.NonTaxableTransportAllowance;
                        list.Add(one);
                        one = new DynamicColumn();

                        one.rowNumber = index;
                        one.columnName = "NT-Desert Allowance";
                        one.cellContent = payrolldata.NonTaxableDesertAllowance.ToString("0.##");
                        one.values = payrolldata.NonTaxableDesertAllowance;
                        list.Add(one);

                        #endregion

                        //one = new DynamicColumn();
                        //one.rowNumber = index;
                        //one.columnName = "Non Taxable Allowance";
                        //one.cellContent = payrolldata.NonTaxableAllowance.ToString("0.##");
                        //one.values = payrolldata.NonTaxableAllowance;
                        //list.Add(one);
                        var nonTaxAdditions =
                            db.Additions.Where(x=>x.GLPeriodId==payrolldata.GLPeriodId).Where(a => a.AdditionTaxType == AdditionTaxType.NON_TAXABLE);

                        foreach (var a in nonTaxAdditions)
                        {
                            var payrolladditions =
                                db.PayrollProcessAdditions.FirstOrDefault(
                                    x =>
                                        x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId &&
                                        x.AdditionId == a.LookupId);

                            if (payrolladditions != null)
                            {
                                one = new DynamicColumn();
                                one.rowNumber = index;
                                one.columnName = payrolladditions.Name;
                                one.cellContent = (payrolladditions.Amount).ToString("0.##");
                                one.values = payrolladditions.Amount;
                                list.Add(one);

                            }
                            else
                            {
                                one = new DynamicColumn();
                                one.rowNumber = index;
                                var name = lookups.FirstOrDefault(x => x.LookupId == a.LookupId);
                                if (name != null) one.columnName = name.Description;
                                one.cellContent = "0.00";
                                one.values = 0;
                                list.Add(one);
                                //saveCredit = deductions.Amount;
                            }
                        }
                        //one = new DynamicColumn();
                        //one.rowNumber = index;
                        //one.columnName = "Non Tax Addition";
                        //one.cellContent = (payrolldata.TotalNonTaxAdditions).ToString("0.##");
                        //one.values = payrolldata.TotalNonTaxAdditions;
                        //list.Add(one);
                        one = new DynamicColumn();
                        one.rowNumber = index;
                        one.columnName = "Net Pay";
                        one.cellContent = (payrolldata.NetEarning).ToString("0.##");
                        one.values = payrolldata.NetEarning;
                        list.Add(one);



                    }

                }





                return UnRemovedDetail(list);
            }

            return UnRemovedDetail(list);
        }
        public List<DynamicColumn> UnRemovedDetail(List<DynamicColumn> list)
        {
            var removedList = new List<DynamicColumn>();
            List<string> columnNames = list.Select(x => x.columnName).Distinct().ToList();
            foreach (var c in columnNames)
            {
                var check = list.Where(y => y.columnName == c);

                var dynamicColumns= check.ToList();
                if (dynamicColumns.Sum(x => x.values) > 0 ||c == "Cost Center" || c == "Full Name" || c == "Employee Id")
                {
                    removedList.AddRange(dynamicColumns);
                }
            }



            return removedList;
        }
    }
}