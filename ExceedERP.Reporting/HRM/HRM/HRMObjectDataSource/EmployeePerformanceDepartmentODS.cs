using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
//using ExceedERP.Web.Areas.HRM.Controllers;
using XAPI;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
  
    [DataObject]
    public class EmployeePerformanceDepartmentODS : List<EmployeePerfromance>
    {
        //HRMInputs input = new HRMInputs();
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromance> GetEmployeePerformance( string year)
        {
            List<EmployeePerfromance> EmployeeInfoList = new List<EmployeePerfromance>();
            EmployeePerfromance EmployeeInfo = new EmployeePerfromance();
            DateTime dateTow = new DateTime(HrHelper.ReportDateTwo.Year, HrHelper.ReportDateTwo.Month, HrHelper.ReportDateTwo.Day);
            DateTime dateOne = new DateTime(HrHelper.ReportDateOne.Year, HrHelper.ReportDateOne.Month, HrHelper.ReportDateOne.Day);
            db = new ExceedDbContext();
            Result Results = new Result();
            var periods = from p in db.QuarterLengths
                          join f in db.GLFiscalYears.Where(x => x.GlFiscalYearId.ToString() == year)
                              on p.GlFiscalYearId equals f.GlFiscalYearId
                          join q in db.Quarters
                          on p.QuarterLengthId equals q.QuarterLengthId
                          where f.Status == FiscalYearStatus.Open
                          select (new { q });

            var period = periods.Select(x => x.q).ToList();
            List<Person_Employee> employees;

           // employees = HrHelper.PersonEmployeesOneBranch(branchId, orgStructureId, employeeType);
      
            employees = db.Person_Employee.ToList();

            {

                db = new ExceedDbContext();
                var placements = db.Placements.ToList();

                db = new ExceedDbContext();



                if (employees.Any())
                {
                    foreach (var e in employees)
                    {
                        EmployeeInfo = new EmployeePerfromance();
                        decimal result = 0;
                        db = new ExceedDbContext();
                        var id = e.EmployeeCode;

                        EmployeeInfo.Results = new List<Result>();


                        EmployeeInfo.EmployeeId = id;
                        EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);
                        //EmployeeInfo.Period = r.Name;
                        EmployeeInfo.FullName = HrHelper.GetEmployeeFullName(e.EmployeeId); ;
                        //EmployeeInfo.Total = result;

                        var palcmentOne =
                            placements.Where(x => x.EmployeeId == e.EmployeeId)
                                .OrderByDescending(x => x.AssignmentDate)
                                .FirstOrDefault();
                        if (palcmentOne != null)
                        {
                            var deptt =
                                db.MainCompanyStructures
                                    .FirstOrDefault(x => x.OrgStructureId == palcmentOne.OrgStructureId);
                            if (deptt != null)
                            {
                                EmployeeInfo.Departement = deptt.Name;
                            }


                        }
                        EmployeeInfo.From = HrHelper.ReportDateOneAm;
                        EmployeeInfo.To = HrHelper.ReportDateTwoAm;
                        if (palcmentOne != null) EmployeeInfo.branchName = palcmentOne.Branch.AmharicName;


                        if (period.Any())
                        {
                            foreach (var r in period)
                            {
                                result = 0;
                                Results = new Result();

                                var emplperforma =
                       db.EmployeeMeasurementDefinition.Where(
                           x => x.EmployeeId == e.EmployeeId && x.QuarterId == r.QuarterId).ToList();
                                if (emplperforma.Any())
                                {
                                    result = emplperforma.Sum(x => x.Result);

                                }
                                Results.Total = result;
                                Results.Period = r.AmharicName;
                                EmployeeInfo.Results.Add(Results);
                            }

                            // EmployeeInfo.Results= EmployeeInfo.Results.Sum(x => x.Total);


                        }
                        EmployeeInfoList.Add(EmployeeInfo);
                    }

                }
            }



            return EmployeeInfoList;
        }
    }
}