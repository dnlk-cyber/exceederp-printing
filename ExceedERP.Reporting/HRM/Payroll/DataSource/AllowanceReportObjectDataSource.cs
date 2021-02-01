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
    public class AllowanceReportObjectDataSource : List<AllowanceReport>
    {

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AllowanceReport> GetDetailReportData(string period, string costcenter,
            string branchId, string user)
        {
            List<AllowanceReport> list = new List<AllowanceReport> { };
            AllowanceReport EmployeeInfo;
            AllowanceTaxable allowanceTaxable;
            AllowanceNonTax allowanceNonTax;
            bool check;

            if (!String.IsNullOrEmpty(period))
            {
                var payrolldatas =
                    db.PayrollProcessViewModel.Where(x => x.PeriodName == period).ToList();

                if (string.IsNullOrEmpty(costcenter))
                {
                    payrolldatas = payrolldatas.Where(x =>x.CostCenterAccountCode==costcenter).ToList();
                }

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
                    foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                    {
                        db = new ExceedDbContext();
                        check = false;
                        var id =
                            db.Person_Employee
                                .FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId)
                                .EmployeeCode;
                        EmployeeInfo = new AllowanceReport();
                        EmployeeInfo.AllowanceNonTaxList = new List<AllowanceNonTax>();
                        EmployeeInfo.AllowanceTaxableList = new List<AllowanceTaxable>();
                        EmployeeInfo.EmpID = id;
                        EmployeeInfo.EmpFullName = payrolldata.FullName;


                        EmployeeInfo.AccountCode = payrolldata.CostCenterAccountCode;
                        EmployeeInfo.AccountDesc = payrolldata.CostCenterAccountDesc;
                        EmployeeInfo.BasicSalary = payrolldata.BasicSalary;
                        allowanceTaxable = new AllowanceTaxable();
                        allowanceNonTax = new AllowanceNonTax();
                        var allowances =
                            db.PayrollProcessAllowances.Where(
                                x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId).ToList();
                        foreach (var a in allowances)
                        {

                            if (a.AllowanceType == AllowanceType.TRANSPORT)
                            {

                                allowanceTaxable.Transport += a.Taxable;
                                allowanceNonTax.TransportN += a.NonTaxable;
                            }
                            if (a.AllowanceType == AllowanceType.HOME)
                            {

                                allowanceTaxable.HomeTaxable += a.Taxable;
                                allowanceNonTax.HomeNonTax += a.NonTaxable;
                            }
                            if (a.AllowanceType == AllowanceType.TELEPHONE)
                            {

                                allowanceTaxable.TelephoneTax += a.Taxable;
                                allowanceNonTax.TelephoneNontax += a.NonTaxable;
                            }
                            if (a.AllowanceType == AllowanceType.DESERT)
                            {

                                allowanceTaxable.Desert += a.Taxable;
                                allowanceNonTax.DesertN += a.NonTaxable;
                            }
                            if (a.AllowanceType == AllowanceType.PROFESSIONAL)
                            {

                                allowanceTaxable.Professional += a.Taxable;
                                allowanceNonTax.ProfessionalN += a.NonTaxable;
                            }
                            if (a.AllowanceType == AllowanceType.RESPONSIBILITY)
                            {

                                allowanceTaxable.Responsiblity += a.Taxable;
                                allowanceNonTax.ResponsiblityN += a.NonTaxable;
                            }
                            if (a.AllowanceType == AllowanceType.PROJECT)
                            {

                                allowanceTaxable.Project += a.Taxable;
                                allowanceNonTax.ProjectN += a.NonTaxable;
                            }
                            if (a.AllowanceType == AllowanceType.POSITION)
                            {

                                allowanceTaxable.PositionTax += a.Taxable;
                                allowanceNonTax.PositionTaxN += a.NonTaxable;
                            }

                        }

                        if ((allowanceNonTax.DesertN != 0 || allowanceNonTax.HomeNonTax != 0 ||
                             allowanceNonTax.PositionTaxN != 0 || allowanceNonTax.ProfessionalN != 0 ||
                             allowanceNonTax.ProjectN != 0 || allowanceNonTax.ResponsiblityN != 0 ||
                             allowanceNonTax.TelephoneNontax != 0 || allowanceNonTax.TransportN != 0) ||
                            (allowanceTaxable.Desert != 0 || allowanceTaxable.HomeTaxable != 0 ||
                             allowanceTaxable.PositionTax != 0 || allowanceTaxable.Professional != 0 ||
                             allowanceTaxable.Project != 0 || allowanceTaxable.Responsiblity != 0 ||
                             allowanceTaxable.TelephoneTax != 0 || allowanceTaxable.Transport != 0))
                        {
                            check = true;
                            EmployeeInfo.AllowanceNonTaxList.Add(allowanceNonTax);
                            EmployeeInfo.AllowanceTaxableList.Add(allowanceTaxable);
                        }
                        if (check)
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