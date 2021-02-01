using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class PayrollSlipHeaderObjectDataSource : PayrollSlipHeader
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PayrollSlipHeader GetSlipHeaders(string period, string costCenter, string empStatus, string employee)
        {
            PayrollSlipHeader seleted = new PayrollSlipHeader();
            db = new ExceedDbContext();
            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeCode == employee);

            if (personEmployee != null)
            {
                var id =
                    personEmployee
                        .EmployeeId;
                var company = db.Companies.FirstOrDefault(c => c.Category == "0");
                var employeeId = ListOfEmployeeIdView.GetEmployees().FirstOrDefault(x => x.PersonId == id);
                if (employeeId != null) seleted.EmpID = employeeId.EmployeeCode;
                if (company != null) seleted.Company = company.TradeName;
                var payroll =
                    db.PayrollProcessViewModel.FirstOrDefault(
                        w => w.EmployeeId == id || (w.CostCenterAccountDesc == costCenter));
                if (employeeId != null) seleted.FullName = employeeId.FullName;
                seleted.PeriodName = period;
                if (payroll != null) seleted.NetPay = payroll.NetEarning;
            }
            return seleted;
        }

    }
}