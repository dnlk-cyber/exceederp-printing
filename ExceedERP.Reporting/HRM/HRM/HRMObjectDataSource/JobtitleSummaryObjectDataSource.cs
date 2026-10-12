using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class JobtitleSummaryObjectDataSource: IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobTitleSummaryVM> GetJobTitleSummary(string empId,string businessUnit, string jobTitle)
        {
            var placementsQuery = db.Placements
                .Where(x => x.isCurrent)
                .AsQueryable();

            if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out int employeeId))
                placementsQuery = placementsQuery.Where(x => x.EmployeeId == employeeId);

            if (!string.IsNullOrEmpty(jobTitle) && int.TryParse(jobTitle, out int lookUpId))
                placementsQuery = placementsQuery.Where(x => x.LookupId == lookUpId);

            if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int businessUnitId))
                placementsQuery = placementsQuery.Where(x => x.BusinessUnitId == businessUnitId);

            var rawData = (
                from p in placementsQuery
                join e in db.Person_Employee on p.EmployeeId equals e.EmployeeId
                join jt in db.Lookups on p.LookupId equals jt.LookupId
                select new
                {
                    e.EmployeeCode,
                    e.FirstName,
                    e.MiddleName,
                    e.LastName,
                    e.FirstNameEnglish,
                    e.MiddleNameEnglish,
                    e.LastNameEnglish,
                    jt.Description,
                    jt.DescriptionEng,
                    e.SubCity,
                    e.PhoneNo,
                    e.City,
                    e.Wereda
                }
            ).ToList();

            var culture = cultureName?.ToLower();
            bool isAmharic = culture == "am" || culture == "am-et";
            var result = rawData.Select(r => new JobTitleSummaryVM
            {
                Code = r.EmployeeCode,
                FullName = isAmharic
                            ? $"{r.FirstName} {r.MiddleName} {r.LastName}"
                            : $"{r.FirstNameEnglish} {r.MiddleNameEnglish} {r.LastNameEnglish}",
                JobTitle = r.Description ?? r.DescriptionEng,
                Address = r.SubCity,
                Tel = r.PhoneNo,
                City = r.City,
                Email = r.Wereda,
                WebSite = r.SubCity,
                Fax = ""
            }).ToList();

            return result;
        }



        public void Dispose()
        {
            db.Dispose();
        }
    }
}