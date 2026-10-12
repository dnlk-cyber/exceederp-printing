using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class LeaveUsageObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LeaveUsageVM> GetAnualLeave(string empId, string branch, string periodId)
        {
            var query = db.LeavesEntitle.AsNoTracking().Where(x => !x.IsExpired); 

            //  Filters
            if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int eId))
                query = query.Where(x => x.EmployeeId == eId);

            if (!string.IsNullOrEmpty(branch) && int.TryParse(branch, out int bId))
                query = query.Where(x => x.BranchId == bId);

            //  Load minimal data
            var rawData = query
                .Select(x => new
                {
                    x.EmployeeId,
                    x.Employee.EmployeeCode,
                    x.Employee.FirstName,
                    x.Employee.MiddleName,
                    x.Employee.LastName,
                    x.Employee.FirstNameEnglish,
                    x.Employee.MiddleNameEnglish,
                    x.Employee.LastNameEnglish,
                    x.Employee.Title,
                    x.GlFiscalYearId,
                    x.Balance
                })
                .ToList();

            //  Cross Tab
            var result = rawData
                .GroupBy(x => new
                {
                    x.EmployeeId,
                    x.EmployeeCode,
                    x.FirstName,
                    x.MiddleName,
                    x.LastName,
                    x.FirstNameEnglish,
                    x.MiddleNameEnglish,
                    x.LastNameEnglish,
                    x.Title
                })
               .SelectMany(g => g.Select(x => new LeaveUsageVM
               {
                   EmployeeCode = g.Key.EmployeeCode,
                   EmployeeName = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                        ? $"{g.Key.FirstName} {g.Key.MiddleName} {g.Key.LastName}"
                                        : $"{g.Key.FirstNameEnglish} {g.Key.MiddleNameEnglish} {g.Key.LastNameEnglish}",
                   Title = g.Key.Title.ToString(),
                   FiscalYear = x.GlFiscalYearId.ToString(),  //REQUIRED
                   Balance = x.Balance,
                   TotalAmount = g.Sum(z => z.Balance)

               })).ToList();

            return result;
        }
        public void Dispose()
        {
            db.Dispose();
        }
      

      
        private static string GetFiscalYearName(int id)
        {
            using (var db = new ExceedDbContext())
            {
                return db.GLFiscalYears.Find(id)?.Name;
            }
        }

        private static string GetFullName(int id)
        {
            using (var db = new ExceedDbContext())
            {
                var employee = db.Person_Employee.Find(id);
                return employee != null
                    ? $"{employee.EmployeeCode} - {employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}"
                    : null;
            }
        }

        private static string GetFullJobtitleName(int id)
        {
            using (var db = new ExceedDbContext())
            {
                return db.Lookups.Find(id)?.Description;
            }
        }

        private static string GetDepartmentName(int id)
        {
            using (var db = new ExceedDbContext())
            {
                return db.MainCompanyStructures.Find(id)?.Name;
            }
        }
       

    }


}
