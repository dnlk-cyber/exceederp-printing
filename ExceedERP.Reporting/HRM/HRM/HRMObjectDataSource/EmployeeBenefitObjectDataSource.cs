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
    public class EmployeeBenefitObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeBenefitVM> GetEmployeeBenefit(string employee,string fiscalYear,string branchId, string orgStructureId, string businessUnit, string jobTitle)
        {
            int.TryParse(fiscalYear, out int fiscalyearId);
            int.TryParse(employee, out int employeeId);
            int.TryParse(branchId, out int braId);
            int.TryParse(orgStructureId, out int orgId);
            int.TryParse(businessUnit, out int busId);
            int.TryParse(jobTitle, out int jobTitleId);

            var employeeBenefitVms = new List<EmployeeBenefitVM>();

            // Preload data
            //var employeeBenefits = db.EmployeeBenefits
            //    .Where(eb =>
            //        (fiscalyearId == 0 || eb.GlFiscalYearId == fiscalyearId) &&
            //        (employeeId == 0 || eb.EmployeeId == employeeId) &&
            //        (braId == 0 || eb.BranchId == braId) &&
            //        (orgId == 0 || eb.OrgStructureId == orgId) &&
            //        (busId == 0 || eb.BusinessUnitId == busId) &&
            //        (jobTitleId == 0 || eb.LookupId == jobTitleId))
            //    .ToList();

            //var employeeIds = employeeBenefits.Select(eb => eb.EmployeeId).Distinct().ToList();
            //var orgIds = employeeBenefits.Select(eb => eb.OrgStructureId).Distinct().ToList();
            //var busIds = employeeBenefits.Select(eb => eb.BusinessUnitId).Distinct().ToList();
            //var branchIds = employeeBenefits.Select(eb => eb.BranchId).Distinct().ToList();
            //var lookupIds = employeeBenefits.Select(eb => eb.LookupId).Distinct().ToList();

            //var persons = db.Person_Employee.Where(p => employeeIds.Contains(p.EmployeeId)).ToDictionary(p => p.EmployeeId);
            //var orgDict = db.MainCompanyStructures.Where(o => orgIds.Contains(o.OrgStructureId)).ToDictionary(o => o.OrgStructureId);
            //var busDict = db.BusinessUnits.Where(b => busIds.Contains(b.BusinessUnitId)).ToDictionary(b => b.BusinessUnitId);
            //var branchDict = db.Branch.Where(b => branchIds.Contains(b.BranchId)).ToDictionary(b => b.BranchId);
            //var lookups = db.Lookups.Where(l => lookupIds.Contains(l.LookupId)).ToDictionary(l => l.LookupId);

            //var benefitIds = employeeBenefits.Select(eb => eb.EmployeeBenefitId).ToList();
            //var benefitDetails = db.EmployeeBenefitDetails
            //    .Where(d => benefitIds.Contains(d.EmployeeBenefitId))
            //    .ToList();

            //var allLookupIds = benefitDetails.Select(d => d.BenefitTypeId)
            //    .Concat(benefitDetails.Select(d => d.UnitofMeasurement))
            //    .Distinct()
            //    .ToList();

            //var detailLookups = db.Lookups
            //    .Where(l => allLookupIds.Contains(l.LookupId))
            //    .ToDictionary(l => l.LookupId);

            //var groupedDetails = benefitDetails.GroupBy(d => d.EmployeeBenefitId)
            //    .ToDictionary(g => g.Key, g => g.ToList());

          

            //foreach (var eb in employeeBenefits)
            //{
            //    if (!persons.TryGetValue(eb.EmployeeId, out var person)) continue;

            //    employeeBenefitVms.Add(new EmployeeBenefitVM
            //    {
            //        EmployeeCode = person.EmployeeCode,
            //        EmployeeFullName = $"{person.FirstNameEnglish} {person.MiddleNameEnglish} {person.LastNameEnglish}",
            //        Branch = branchDict.TryGetValue(eb.BranchId, out var bran) ? bran.Name : null,
            //        BusinessUnit = busDict.TryGetValue(eb.BusinessUnitId, out var bus) ? bus.Name : null,
            //        OrgStructure = orgDict.TryGetValue(eb.OrgStructureId, out var org) ? org.Name : null,
            //        JobTitle = lookups.TryGetValue(eb.LookupId, out var title) ? title.Description : null,
            //        EffectiveFrom = eb.EffectiveFrom,
            //        EffectiveTo = eb.EffectiveTo,
            //        FiscalYear = eb.FiscalYear?.Name,
            //        Details = groupedDetails.TryGetValue(eb.EmployeeBenefitId, out var details) ?
            //                  details.Select(d => new EmployeeBenefitEachVM
            //                  {
            //                      BenefitType = detailLookups.TryGetValue(d.BenefitTypeId, out var bt) ? bt.Description : "",
            //                      UnitofMeasurement = detailLookups.TryGetValue(d.UnitofMeasurement, out var uom) ? uom.Description : null,
            //                      Quantity = d.Quantity,
            //                      UtilizedQuantity = d.UtilizedQuantity,
            //                      Remark = d.Remark,
            //                      IsUtilized = d.IsUtilized,
            //                  }).ToList() : new List<EmployeeBenefitEachVM>()
            //    });
            //}

            return employeeBenefitVms;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}