using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.DataSource;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class OvertimeHoursListObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OvertimeHoursList> GetOvertimeHoursLists(string period, string branchId, string user, string bizUnitId)
        {
            List<OvertimeHoursList> list = new List<OvertimeHoursList> { };
            OvertimeHoursList EmployeeInfo;
            var placments = db.Placements.OrderByDescending(x => x.AssignmentDate).ToList();

            //List<EmployeeIdView> employees = ListOfEmployeeIdView.GetEmployees();
           // var PeriodId = db.GLPeriods.Where(p => p. == period).FirstOrDefault();

            if (!String.IsNullOrEmpty(period))
            {
                if (placments.Any())
                {
                    int.TryParse(period, out int periodId);
                    var OtDatas = db.OverTimes.Where(x => x.GLPeriodId == periodId).ToList();
                    foreach (OverTime otdata in OtDatas)
                    {
                        db = new ExceedDbContext();
                        var id = otdata.Employee.EmployeeCode;

                        var emmployeebarnch =
                            placments
                                .FirstOrDefault(x => x.EmployeeId == otdata.EmployeeId && x.isCurrent);
                        if (branchId != null)
                        {
                            List<string> branchs = branchId.Split('-').ToList();
                            emmployeebarnch =
                                placments
                                    .FirstOrDefault(x => x.EmployeeId == otdata.EmployeeId && branchs.Contains(x.BranchId.ToString()) && x.isCurrent);

                        }
                        if (emmployeebarnch != null)
                        {

                            EmployeeInfo = new OvertimeHoursList();
                            EmployeeInfo.EmpID = id;
                            var firstOrDefault =
                                db.Person_Employee.FirstOrDefault(o => o.EmployeeId == otdata.EmployeeId);
                            if (firstOrDefault != null)
                            {
                                EmployeeInfo.FullName = firstOrDefault.FirstNameEnglish+" "+firstOrDefault.MiddleNameEnglish+" "+firstOrDefault.LastNameEnglish;
                            }
                               
                            EmployeeInfo.NormalHours = otdata.NormalHours;
                            EmployeeInfo.NightHours = otdata.NightHours;
                            EmployeeInfo.HolidayHours = otdata.HolidayHours;
                            EmployeeInfo.RestdayHours = otdata.RestdayHours;
                            list.Add(EmployeeInfo);
                        }
                    }
                }
                return list;

            }
            return list;
        }
        public void Dispose()
        {
            db.Dispose();
        }


    }
}