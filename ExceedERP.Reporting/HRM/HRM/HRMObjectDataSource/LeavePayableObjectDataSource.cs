using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeavePayableObjectDataSource : List<LeavePayableVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public LeavePayableVM GetLeavePayable(string id)
        {
            LeavePayableVM leavepayable = new LeavePayableVM();
            int.TryParse(id, out int leavePayableId);
            string fiscalYear1 = "";
            string fiscalYear2 = "";

            var payable = db.LeavePayables.Find(leavePayableId);
            if(payable != null)
            {
                var fiscalYear = db.GLFiscalYears.Find(payable.GlFiscalYearId);
                if(fiscalYear != null)
                {
                    leavepayable.FiscalYear = fiscalYear.Name;
                    var glfiscal = db.GLFiscalYears.Where(x => x.DateTo < fiscalYear.DateTo).OrderByDescending(x => x.DateTo).Select(x => x.GlFiscalYearId).Take(2).ToArray();
                    int fiscalyear1 = glfiscal[0];
                    int fiscalyear2 = glfiscal[1];
                    var year1 = db.GLFiscalYears.Find(fiscalyear1);
                    if(year1 != null)
                    {
                        fiscalYear1 = year1.Name;
                    }
                    var year2 = db.GLFiscalYears.Find(fiscalyear2);
                    if(year2 != null)
                    {
                        fiscalYear2 = year2.Name;
                    }
                }
                leavepayable.LeavePayableDetails = new List<LeavePayableDetailVM>();
                var PayableDetails = db.LeavePayableDetails.Where(x => x.LeavePayableId == leavePayableId).DistinctByField(x=>x.EmployeeId).ToList();
                foreach (var payableDetail in PayableDetails)
                {
                    var payableList = new LeavePayableDetailVM();
                    var employee = db.Person_Employee.Find(payableDetail.EmployeeId);
                    if(employee != null)
                    {
                        payableList.EmployeeFullName =   employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;

                    }
                    payableList.FiscalYear1 = fiscalYear1;
                    payableList.FiscalYear2 = fiscalYear2;
                    payableList.Year1 = payableDetail.Year1;
                    payableList.Year2 = payableDetail.Year2;
                    payableList.TotalDays = payableDetail.PayableDays;
                    payableList.Payable = payableDetail.Balance;
                    leavepayable.LeavePayableDetails.Add(payableList);



                }
               
            }

            return leavepayable;


        }

    }
}