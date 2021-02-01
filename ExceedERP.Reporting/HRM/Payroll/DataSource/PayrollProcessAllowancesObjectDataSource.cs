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
    public class PayrollProcessAllowancesObjectDataSource : List<PayrollProcessAdditions>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PayrollProcessAdditions> GetAllowances(string period, string costCenter, string empStatus,
            string employee)
        {
            var status = EmployeeStatus.PERMANENT;
            List<PayrollProcessAdditions> list = new List<PayrollProcessAdditions>();
            PayrollProcessAdditions payrolldatas = new PayrollProcessAdditions();
            bool addSalary = false;
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
            var personEmployees = db.Person_Employee.ToList();
            if (employee != null)
            {
                var personEmployee = personEmployees
                    .FirstOrDefault(x => x.EmployeeCode == employee);


                if (personEmployee != null)
                {
                    var id =
                        personEmployee
                            .EmployeeId;

                    var payrollview =
                        db.PayrollProcessViewModel
                            .FirstOrDefault(x => x.EmployeeId == id && x.PeriodName == period);

                    if (!String.IsNullOrEmpty(period) && !String.IsNullOrEmpty(empStatus) &&
                        !String.IsNullOrEmpty(employee) || !String.IsNullOrEmpty(costCenter))
                    {

                        var payrollAllowances =
                            db.PayrollProcessAdditions.Where(
                                p =>
                                    (p.PayrollProcessViewModelId == payrollview.PayrollProcessViewModelId) &&
                                    (p.CostCenterAccountCode == costCenter));
                        if (payrollview != null)
                        {
                            payrolldatas.Name = "Basic Salary";
                            payrolldatas.Amount = payrollview.BasicSalary;
                            list.Add(payrolldatas);


                        }


                        foreach (PayrollProcessAdditions payrolldata in payrollAllowances)
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