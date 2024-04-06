using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class SeverancePayableObjectDataSource : List<LeavePayableVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public SeverancePayableVM GetLeavePayable(string id)
        {
            SeverancePayableVM severancePayable = new SeverancePayableVM();
            int.TryParse(id, out int payableId);
          

            var payable = db.SeverancePayables.Find(payableId);
            if(payable != null)
            {
                var fiscalYear = db.GLFiscalYears.Find(payable.GlFiscalYearId);
                if(fiscalYear != null)
                {
                    severancePayable.FiscalYear = fiscalYear.Name;
                    
                }
                severancePayable.SeverancePayablesDetails = new List<SeverancePayableDetailVM>();
                var PayableDetails = db.SeverancePayableDetails.Where(x => x.SeverancePayableId == payableId).DistinctByField(x=>x.EmployeeId).ToList();
                foreach (var payableDetail in PayableDetails)
                {
                    var payableList = new SeverancePayableDetailVM();
                    var employee = db.Person_Employee.Find(payableDetail.EmployeeId);
                    if(employee != null)
                    {
                        payableList.EmployeeFullName =   employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;

                    }
                    payableList.Basicalary = payableDetail.BasicSalary;
                    payableList.NomberOfYears = payableDetail.NumberOFYears;
                    payableList.GrossComposition = payableDetail.GrossComponsation;
                    payableList.Total = payableDetail.NoneTaxableTotal;
                    payableList.IncomeTax = payableDetail.TaxWithheldIncomeTax;
                    payableList.TotalIncomeTax = payableDetail.TaxWithheldComTax;
                    payableList.Deduction = payableDetail.SpecialSalaryDeduction;
                    payableList.TotalDeuduction = payableDetail.TotalDeductions;
                    payableList.NetPay = payableDetail.NetPay;
                    severancePayable.SeverancePayablesDetails.Add(payableList);



                }
               
            }

            return severancePayable;


        }

    }
}