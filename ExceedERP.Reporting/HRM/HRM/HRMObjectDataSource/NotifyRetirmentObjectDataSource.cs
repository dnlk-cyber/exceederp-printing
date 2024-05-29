using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class NotifyRetirmentObjectDataSource : List<EmployeeRetirment>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeRetirment> GetNotifyRetirmentSummary(string value, string branchId, string orgStructureId)
        {
            List<EmployeeRetirment> list = new List<EmployeeRetirment>();
            EmployeeRetirment employee = new EmployeeRetirment();
            db = new ExceedDbContext();
            List<Person_Employee> emplist = db.Person_Employee.ToList();

            var placements = db.Placements.ToList();

            if(branchId != null)
            {
                int.TryParse(branchId, out int braId);
                var empIds = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId).Select(x=>x.EmployeeId).ToList();
                emplist = emplist.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
            if(orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                var empIds = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId).Select(x => x.EmployeeId).ToList();
                emplist = emplist.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }

            if (value != null)
            {
                int retirementAge = 0;
                var validator = db.Configurations.Where(x => x.Attribute == "RETIREMENT_AGE").FirstOrDefault();
                if (validator != null)
                {
                    retirementAge = Convert.ToInt32(validator.CurrentValue);

                    foreach (var emp in emplist)
                    {
                        employee = new EmployeeRetirment();
                        int x = (DateTime.Now - emp.DateOfBirth).Days / 365;
                        int y = retirementAge - x;
                        if (y <= Convert.ToInt32(value))
                        {
                            employee.FullName = emp.FullNameAmharic;
                            employee.IdNumber = emp.EmployeeCode;

                            if (emp.Gender == Gender.MALE)
                            {
                                employee.Sex = "ወንድ";
                            }
                            else
                            {
                                employee.Sex = "ሴት";
                            }

                            var retireYear = emp.DateOfBirth.AddYears(60);

                            employee.RemainingDays = retireYear.Subtract(DateTime.Today).TotalDays.ToString();

                            DateTime date = emp.DateOfBirth.AddYears(retirementAge);

                            employee.RetirmentDate = XAPI.EthiopicDateTime.GetEthiopicDate(date.Day, date.Month, date.Year);
                            employee.BirthDate = emp.DateOfBirthAm;
                            list.Add(employee);
                        }
                    }
                }
            }
            return list;
        }
    }
}