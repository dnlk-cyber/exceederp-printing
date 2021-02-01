using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class PayrollProcessDeductionsObjectDataSource : List<PayrollProcessDeductions>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PayrollProcessDeductions> GetDeductions(string period, string costCenter, string empStatus, string employee)
        {
            var status = EmployeeStatus.PERMANENT;
            List<PayrollProcessDeductions> list = new List<PayrollProcessDeductions>();
            PayrollProcessDeductions EmployeeInfo = new PayrollProcessDeductions();
            switch (empStatus)
            {
                case "PERMANENT":
                    status = EmployeeStatus.PERMANENT;
                    break;
                case "CONTRACT":
                    status = EmployeeStatus.CONTRACT;
                    break;
                case "NO_STATUS":
                    status = EmployeeStatus.NO_STATUS;
                    break;
            }
            db = new ExceedDbContext();
            var personEmployee = db.Person_Employee.Where(x => x.EmployeeCode == employee)
                .FirstOrDefault();
            if (personEmployee != null)
            {
                var id =
                    personEmployee
                        .EmployeeId;

                var payrollview =
                    db.PayrollProcessViewModel.Where(x => x.EmployeeId == id && x.PeriodName == period)
                        .FirstOrDefault();
                if (!String.IsNullOrEmpty(period) && !String.IsNullOrEmpty(empStatus) &&
                    !String.IsNullOrEmpty(employee) || !String.IsNullOrEmpty(costCenter))
                {
                    if (payrollview != null)
                    {
                        EmployeeInfo.Name = "Income Tax";
                        EmployeeInfo.Amount = payrollview.IncomeTax;
                        list.Add(EmployeeInfo);
                        EmployeeInfo = new PayrollProcessDeductions();
                        EmployeeInfo.Name = "Pesnion";
                        EmployeeInfo.Amount = payrollview.PensionFromEmployee;
                        list.Add(EmployeeInfo);

                        var payrollDeduction =
                            db.PayrollProcessDeductions.Where(
                                p => (p.PayrollProcessViewModelId == payrollview.PayrollProcessViewModelId) && (p.CostCenterAccountDesc == costCenter));
                        foreach (PayrollProcessDeductions payrolldata in payrollDeduction)
                        {

                            list.Add(payrolldata);
                        }
                    }

                }

            }
            return list;
        }
    }
}