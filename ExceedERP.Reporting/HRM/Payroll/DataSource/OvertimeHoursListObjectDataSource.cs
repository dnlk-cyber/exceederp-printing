using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class OvertimeHoursListObjectDataSource : List<OvertimeHoursList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OvertimeHoursList> GetOvertimeHoursLists(string period, string branchId, string user)
        {
            List<OvertimeHoursList> list = new List<OvertimeHoursList> { };
            OvertimeHoursList EmployeeInfo;
            var placments = db.Placements.OrderByDescending(x => x.AssignmentDate).ToList();

            List<EmployeeIdView> employees = ListOfEmployeeIdView.GetEmployees();
            var PeriodId = db.GLPeriods.Where(p => p.Name == period).FirstOrDefault();

            if (!String.IsNullOrEmpty(period))
            {
                if (placments.Any())
                {
                    var OtDatas = db.OverTimes.Where(x => x.GLPeriodId == PeriodId.GLPeriodId).ToList();
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
                                employees.FirstOrDefault(o => o.PersonId == otdata.EmployeeId);
                            if (firstOrDefault != null)
                                EmployeeInfo.FullName = firstOrDefault.FullName;
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

        //  [DataObjectMethod(DataObjectMethodType.Select)]
        //public List<OvertimeHoursList> GetOvertimeContractHours(string period)
        //{
        //    List<OvertimeHoursList> list = new List<OvertimeHoursList> { };
        //    OvertimeHoursList EmployeeInfo;

        //    if (!String.IsNullOrEmpty(period))
        //    {
        //        var OtDatas = db.EmpPayrollContractOvertimeTwoes.Where(x => x.PeriodName == period).ToList();
        //        foreach (EmpPayrollContractOvertimeTwo otdata in OtDatas)
        //        {
        //            EmployeeInfo = new OvertimeHoursList();
        //            EmployeeInfo.EmployeeId = otdata.EmployeeIdContract;
        //            EmployeeInfo.FullName = otdata.EmpFullNameContract;
        //            EmployeeInfo.NormalHours = otdata.NormalHours;
        //            EmployeeInfo.NightHours = otdata.NightHours;
        //            EmployeeInfo.HolidayHours = otdata.HolidayHours;
        //            EmployeeInfo.RestdayHours = otdata.RestdayHours;
        //            list.Add(EmployeeInfo);
        //        }
        //        return list;
        //    }
        //    return list;
        //}
    }
}