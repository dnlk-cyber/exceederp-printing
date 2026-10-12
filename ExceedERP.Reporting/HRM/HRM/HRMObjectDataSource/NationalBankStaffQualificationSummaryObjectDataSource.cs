using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class NationalBankStaffQualificationSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NationalBankStaffQualificationSummaryVM> GetEmployeeEduLevelSummary(string branchId, string orgStructureId, string businessUnit, string status, string eduLevel = "")
        {
            var result = new List<NationalBankStaffQualificationSummaryVM>();

            var organization = db.Companies.AsNoTracking().FirstOrDefault(x => x.Category == "0");
            var fyearId = _fiscalYearHelper.GetCurrentFYearId();
            var fiscalYear = db.GLFiscalYears.Find(fyearId);

            //  Education Levels
            var eduLevelsQuery = db.Lookups
                .AsNoTracking()
                .Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL);

            if (!string.IsNullOrWhiteSpace(eduLevel) && int.TryParse(eduLevel, out int lookupId))
                eduLevelsQuery = eduLevelsQuery.Where(x => x.LookupId == lookupId);

            var eduLevels = eduLevelsQuery
                .Select(x => new { x.LookupId, x.Description, x.Index })
                .ToList();

            if (!eduLevels.Any())
                return result;

            //  Base employee query (stay IQueryable)
            var employeesQuery = db.Person_Employee.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(branchId) && int.TryParse(branchId, out int braId))
            {
                employeesQuery = from e in employeesQuery
                                 join p in db.Placements on e.EmployeeId equals p.EmployeeId
                                 where p.BranchId == braId
                                 select e;
            }

            if (!string.IsNullOrWhiteSpace(businessUnit) && int.TryParse(businessUnit, out int businessUnitId))
            {
                employeesQuery = from e in employeesQuery
                                 join p in db.Placements on e.EmployeeId equals p.EmployeeId
                                 where p.BusinessUnitId == businessUnitId
                                 select e;
            }

            if (!string.IsNullOrWhiteSpace(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var structureIds = db.MainCompanyStructures
                    .Where(x => x.Parent == orgId)
                    .Select(x => x.OrgStructureId);

                employeesQuery = from e in employeesQuery
                                 join p in db.Placements on e.EmployeeId equals p.EmployeeId
                                 where structureIds.Contains(p.OrgStructureId)
                                 select e;
            }


            if (status == "Active")
                employeesQuery = employeesQuery.Where(x => x.IsActive);
            else if (status == "InActive")
                employeesQuery = employeesQuery.Where(x => !x.IsActive);

            //  Project only needed fields
            var employees = employeesQuery
                .Select(e => new
                {
                    e.EmployeeId,
                    e.Gender,
                    e.Status
                }).ToList();



            if (!employees.Any())
                return result;

            var employeeIds = employees.Select(e => e.EmployeeId).ToList();

            //  Get highest education per employee (FILTERED!)
            var highestEdu = db.Education
                .Where(e => employeeIds.Contains(e.EmployeeId))
                .Join(db.Lookups,
                    edu => edu.EduLevel,
                    lookup => lookup.LookupId,
                    (edu, lookup) => new
                    {
                        edu.EmployeeId,
                        edu.EduLevel,
                        lookup.Index
                    })
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.Index).FirstOrDefault())
                .ToList();

            //  Convert employee info to dictionary (fast lookup)
            var empDict = employees.ToDictionary(x => x.EmployeeId);

            var comp = new NationalBankStaffQualificationSummaryVM
            {
                InstitutionName = organization?.TradeName ?? organization?.AmharicName?? "Dedebit Credit & Saving Instition.",
                PeriodEnding = fiscalYear?.DateTo != null? $"{fiscalYear.DateTo:MMMM dd, yyyy} G.C / {HrHelper.GetECDate(fiscalYear.DateTo)} E.C": "",
                EducationLevels = new List<EducationLevelRowVM>()
            };

            //  Group once instead of filtering repeatedly
            var grouped = highestEdu
                .GroupBy(x => x.EduLevel)
                .ToDictionary(g => g.Key, g => g.Select(x => x.EmployeeId).ToList());

            foreach (var level in eduLevels.OrderByDescending(x => x.Index))
            {
                grouped.TryGetValue(level.LookupId, out var ids);
                if (ids == null)
                    continue;
               
                int male = 0, female = 0, permanent = 0, temporary = 0, others = 0;

                foreach (var id in ids)
                {
                    if (!empDict.TryGetValue(id, out var emp)) continue;

                    if (emp.Gender == Gender.MALE) male++;
                    else if (emp.Gender == Gender.FEMALE) female++;

                    if (emp.Status == EmployeeStatus.PERMANENT) permanent++;
                    else if (emp.Status == EmployeeStatus.CONTRACT) temporary++;
                    else others++; // FIXED logic
                }

                if (ids.Any())
                {
                    comp.EducationLevels.Add(new EducationLevelRowVM
                    {
                        EducationLevel = level.Description,
                        Male = male,
                        Female = female,
                        Permanent = permanent,
                        Temporary = temporary,
                        Others = others
                    });
                }
            }

            result.Add(comp);
            return result;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }

    
}