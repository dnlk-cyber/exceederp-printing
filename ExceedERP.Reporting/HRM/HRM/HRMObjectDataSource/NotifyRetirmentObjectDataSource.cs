using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class NotifyRetirmentObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeRetirment> GetNotifyRetirmentSummary(DateTime? from, DateTime? to, string branchId, string orgStructureId)
        {
            List<EmployeeRetirment> list = new List<EmployeeRetirment>();
            db = new ExceedDbContext();
            var emplist = db.Person_Employee.Where(x => x.IsActive).ToList();
            // Filter by branchId
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                var empIds = db.Placements.Where(x=>x.BranchId== braId&&x.isCurrent).Select(x => x.EmployeeId);
                emplist = emplist.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
            // Filter by orgStructureId
            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var empIds = db.Placements.Where(x => x.OrgStructureId == orgId && x.isCurrent).Select(x => x.EmployeeId);
                emplist = emplist.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
            // Get retirement age configuration
            var validator = db.Configurations.FirstOrDefault(x => x.Attribute == "RETIREMENT_AGE");
            if (validator == null)
            {
                return new List<EmployeeRetirment>(); // No retirement age configured
            }
            int retirementAge = Convert.ToInt32(validator.CurrentValue);
            // Filter by retirement date range
            DateTime fromDate = from ?? DateTime.Now;
            DateTime toDate = to ?? DateTime.Now.AddDays(10);
            emplist = emplist.Where(x=>!x.IsReHired).Where(x =>
                (retirementAge - (fromDate - x.DateOfBirth).TotalDays / 365) > 0 &&
                (retirementAge - (toDate - x.DateOfBirth).TotalDays / 365) < 0).ToList();

            foreach (var emp in emplist)
            {
               var  employee = new EmployeeRetirment();
                employee.FullName = emp.FullNameAmharic;
                employee.IdNumber = emp.EmployeeCode;
                if (emp.Gender == Gender.MALE)
                {
                    //employee.Sex = "ወንድ";
                    employee.Sex = HrHelper.GetGender(emp.Gender);
                }
                else
                {
                    //employee.Sex = "ሴት";
                    employee.Sex = HrHelper.GetGender(emp.Gender);
                    
                }
                var retireYear = emp.DateOfBirth.AddYears(retirementAge);
                employee.RemainingDays = Math.Round(retireYear.Subtract(DateTime.Now).TotalDays,2).ToString();
                DateTime retirementDate = emp.DateOfBirth.AddYears(retirementAge);
                employee.RetirmentDate = XAPI.EthiopicDateTime.GetEthiopicDate(retirementDate.Day, retirementDate.Month, retirementDate.Year);
                employee.BirthDate = emp.DateOfBirthAm;
                list.Add(employee);

            }
            return list;
            // Generate the retirement summary
            //var retirementList = emplist.ToList().Select(emp =>
            //{
            //    var employee = new EmployeeRetirment
            //    {
            //        FullName = emp.FullNameAmharic,
            //        IdNumber = emp.EmployeeCode,
            //        Sex = HrHelper.GetGender(emp.Gender),
            //        BirthDate = emp.DateOfBirthAm
            //    };

            //    DateTime retirementDate = emp.DateOfBirth.AddYears(retirementAge);
            //    employee.RemainingDays = retirementDate.Subtract(DateTime.Today).TotalDays.ToString();
            //    employee.RetirmentDate = XAPI.EthiopicDateTime.GetEthiopicDate(retirementDate.Day, retirementDate.Month, retirementDate.Year);

            //    return employee;
            //}).ToList();

            //return retirementList;


        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}