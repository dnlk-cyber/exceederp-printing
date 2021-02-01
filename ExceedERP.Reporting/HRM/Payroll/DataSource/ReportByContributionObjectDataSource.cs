using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class ReportByContributionObjectDataSource : List<ReportByContributionTypeList>
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ReportByContributionTypeList> ReportByContributionTypeListData(string period, string type, string source, string branchId, string user)
        {
            List<ReportByContributionTypeList> list = new List<ReportByContributionTypeList> { };
            ReportByContributionTypeList EmployeeInfo;

            var employees =
                db.Person_Employee.ToList();


            {

                if ((!String.IsNullOrEmpty(period)) && (!String.IsNullOrEmpty(type)) &&
                    (!String.IsNullOrEmpty(source)))
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
                        decimal totalAmount = 0;
                        foreach (PayrollProcessViewModel payrolldata in payrolldatas)
                        {
                            EmployeeInfo = new ReportByContributionTypeList
                            {
                                Period = period,
                                BasicSalary = payrolldata.BasicSalary
                            };
                            var firstOrDefault = db.Branch.FirstOrDefault(x => payrolldata.BranchId == x.BranchId);
                            if (firstOrDefault != null)
                            {
                                var branchobj = firstOrDefault.Name;
                                EmployeeInfo.Branch = branchobj;
                            }

                            EmployeeInfo.ContributionName = type;
                            EmployeeInfo.Source = source;
                            var personEmployee =
                                employees.FirstOrDefault(x => x.EmployeeId == payrolldata.EmployeeId);

                            if (personEmployee != null)
                            {
                                var id =
                                    personEmployee
                                        .EmployeeCode;
                                EmployeeInfo.EmpID = id;
                            }
                            EmployeeInfo.FullName = payrolldata.FullName;
                            if (source == "Deduction")
                            {
                                var contribObject =
                                    db.PayrollProcessDeductions.Where(
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .Where(x => x.Name == type)
                                        .ToList();
                                if (contribObject.Any())
                                    EmployeeInfo.ContributionAmount = contribObject.Sum(x => x.Amount);
                                else
                                    EmployeeInfo.ContributionAmount = contribObject.Sum(x => x.Amount);
                            }
                            else if (source == "Long Term Deduction")
                            {
                                var contribObject =
                                    db.PayrollProcessLongTermDeductions.Where(
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .Where(x => x.Name == type)
                                        .ToList();
                                if (contribObject.Any())
                                    EmployeeInfo.ContributionAmount = contribObject.Sum(x => x.Amount);
                                else
                                    EmployeeInfo.ContributionAmount = contribObject.Sum(x => x.Amount);
                            }
                            else if (source == "Fixed Contribution")
                            {
                                var contribObject =
                                    db.PayrollProcessFixedContribution.Where(
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .Where(x => x.Name == type)
                                        .ToList();
                                if (contribObject.Any())
                                    EmployeeInfo.ContributionAmount = contribObject.Sum(x => x.Amount);
                                else
                                    EmployeeInfo.ContributionAmount = contribObject.Sum(x => x.Amount);
                                var fixedObject =
                                    db.FixedContributions.Where(
                                            x =>
                                                x.EmployeeId == payrolldata.EmployeeId &&
                                                x.ContributionType.Description == type &&
                                                x.GLPeriodId == payrolldata.GLPeriodId)
                                        .ToList();
                                if (fixedObject.Any())
                                {
                                    EmployeeInfo.Balance = fixedObject.Sum(x => x.Balance);

                                }
                                else
                                    EmployeeInfo.Balance = 0;
                            }
                            else if (source == "Regular Contribution")
                            {
                                var contribObject =
                                    db.PayrollProcessRegularContribution.Where(
                                            x => x.PayrollProcessViewModelId == payrolldata.PayrollProcessViewModelId)
                                        .Where(x => x.Name == type)
                                        .ToList();
                                var reguralObject =
                                    db.RegularContributions.Where(
                                            x =>
                                                x.EmployeeId == payrolldata.EmployeeId &&
                                                x.ContributionType.Description == type &&
                                                x.GLPeriodId == payrolldata.GLPeriodId)
                                        .ToList();
                                if (contribObject.Any())
                                {
                                    EmployeeInfo.ContributionAmount = contribObject.Sum(x => x.Amount);

                                }
                                else
                                    EmployeeInfo.ContributionAmount = 0;
                                if (reguralObject.Any())
                                {

                                    EmployeeInfo.Balance = reguralObject.Sum(x => x.Amount);
                                }
                                else
                                    EmployeeInfo.Balance = 0;
                            }
                            else
                            {
                                EmployeeInfo.ContributionAmount = 0;
                            }
                            if (EmployeeInfo.ContributionAmount > 0)
                            {
                                list.Add(EmployeeInfo);
                            }
                            totalAmount = totalAmount + EmployeeInfo.ContributionAmount;
                        }
                        return list;
                    }
                }
            }
            return list;
        }
    }
}