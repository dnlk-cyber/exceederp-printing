using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeReligionObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeReligionVM> GetEmployeeReligion(string empId, string branchId, string religion, string status)
        {
            var employees = db.Person_Employee
                              .AsNoTracking()
                              .AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int branId))
                employees = employees.Where(x => x.BranchId == branId);

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Active")
                    employees = employees.Where(x => x.IsActive);
                else if (status == "InActive")
                    employees = employees.Where(x => !x.IsActive);
            }

            if (!string.IsNullOrEmpty(religion))
            {
                // Better if Religion is enum/int in DB
                employees = employees.Where(x => x.Religion.ToString() == religion);
            }

            var result = (from emp in employees
                          join b in db.Branch.AsNoTracking()
                              on emp.BranchId equals b.BranchId into branchJoin
                          from branch in branchJoin.DefaultIfEmpty()
                          select new EmployeeReligionVM
                          {
                              EmployeeCode = emp.EmployeeCode,

                              FullName = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                  ? (emp.FirstName + " " + emp.MiddleName + " " + emp.LastName)
                                  : (emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish),

                              Branch = branch != null ? branch.Name : null,

                              Religion = emp.Religion.ToString()
                          })
                          .ToList();

            return result;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}