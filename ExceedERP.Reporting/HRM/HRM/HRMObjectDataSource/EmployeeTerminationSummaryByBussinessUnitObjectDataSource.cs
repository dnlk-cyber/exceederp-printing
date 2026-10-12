using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeTerminationSummaryByBussinessUnitDataSource:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeTerminationSummaryByBussinessUnitVM> GetEmployeeTerminationSummaryByBussinessUnit(string branchId, string businessUnitId, string orgStructureId, string job)
        {
            var list = new List<EmployeeTerminationSummaryByBussinessUnitVM>();
            var empPlacementsQueries = db.Placements.Where(x => !x.isCurrent).ToList();
            var employeesQuery = db.Person_Employee.Where(x => !x.IsActive).ToList();
            var terminations = db.Terminations.Where(x => x.IsOnlineApproved).ToList();

            // Filter placements by branch, business unit, and organizational structure
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                empPlacementsQueries = empPlacementsQueries.Where(x => x.BranchId == braId).ToList();
                var employeeIds = empPlacementsQueries.Select(x => x.EmployeeId).Distinct().ToList();
                terminations = terminations.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            }
            if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int bUnitId))
            {
                empPlacementsQueries = empPlacementsQueries.Where(x => x.BusinessUnitId == bUnitId).ToList();
                var employeeIds = empPlacementsQueries.Select(x => x.EmployeeId).Distinct().ToList();
                terminations = terminations.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            }
            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
                empPlacementsQueries = empPlacementsQueries.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();
                var employeeIds = empPlacementsQueries.Select(x => x.EmployeeId).Distinct().ToList();
                terminations = terminations.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            }
            if (!string.IsNullOrEmpty(job) && int.TryParse(job, out int lookUpId))
            {
                empPlacementsQueries = empPlacementsQueries.Where(x => x.LookupId == lookUpId).ToList();
                var employeeIds = empPlacementsQueries.Select(x => x.EmployeeId).Distinct().ToList();
                terminations = terminations.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            }

            var terminatedEmployeeIds = terminations.Select(x => x.EmployeeId).Distinct().ToList();
            empPlacementsQueries = empPlacementsQueries.Where(x => terminatedEmployeeIds.Contains(x.EmployeeId)).ToList().DistinctByField(x => x.EmployeeId).ToList();
            var businessUnitPlacements = empPlacementsQueries.Select(x => x.BusinessUnitId).Distinct().ToList();

            foreach (var businessUnitPlacement in businessUnitPlacements)
            {
                var summary = new EmployeeTerminationSummaryByBussinessUnitVM();
                var businessUnit = db.BusinessUnits.Find(businessUnitPlacement);

                if (businessUnit != null)
                {
                    summary.BussinesUnit = businessUnit.Name;

                    var permanentEmployeeIds = empPlacementsQueries.Where(x => x.BusinessUnitId == businessUnitPlacement && x.EmployementType == EmployeeStatus.PERMANENT).Select(x => x.EmployeeId).Distinct().ToList();
                    var permanentEmployees = employeesQuery.Where(x => permanentEmployeeIds.Contains(x.EmployeeId)).ToList();
                    summary.PermanentFemale = permanentEmployees.Count(e => e.Gender == Gender.FEMALE);
                    summary.PermanentMale = permanentEmployees.Count(e => e.Gender == Gender.MALE);
                    summary.TotalPermanentEmployee = summary.PermanentFemale + summary.PermanentMale;

                    var contractEmployeeIds = empPlacementsQueries.Where(x => x.BusinessUnitId == businessUnitPlacement && x.EmployementType == EmployeeStatus.CONTRACT).Select(x => x.EmployeeId).Distinct().ToList();
                    var contractEmployees = employeesQuery.Where(x => contractEmployeeIds.Contains(x.EmployeeId)).ToList();
                    summary.ContractFemale = contractEmployees.Count(e => e.Gender == Gender.FEMALE);
                    summary.ContractMale = contractEmployees.Count(e => e.Gender == Gender.MALE);
                    summary.TotalContaractEmployee = summary.ContractFemale + summary.ContractMale;

                    var otherEmployeeIds = empPlacementsQueries.Where(x => x.BusinessUnitId == businessUnitPlacement && x.EmployementType != EmployeeStatus.PERMANENT && x.EmployementType != EmployeeStatus.CONTRACT).Select(x => x.EmployeeId).Distinct().ToList();
                    var otherEmployees = employeesQuery.Where(x => otherEmployeeIds.Contains(x.EmployeeId)).ToList();
                    summary.OtherFemale = otherEmployees.Count(e => e.Gender == Gender.FEMALE);
                    summary.OtherMale = otherEmployees.Count(e => e.Gender == Gender.MALE);
                    summary.TotalOtherEmployee = summary.OtherFemale + summary.OtherMale;

                    summary.TotalFemale = summary.PermanentFemale + summary.ContractFemale + summary.OtherFemale;
                    summary.TotalMale = summary.PermanentMale + summary.ContractMale + summary.OtherMale;
                    summary.TotalEmployee = summary.TotalFemale + summary.TotalMale;

                    list.Add(summary);
                }
            }

            return list;
        }

        //public List<EmployeeTerminationSummaryByBussinessUnitVM> GetEmployeeTeminationSummaryByBussinessUnit(string branchId, string businessUnitId, string orgStructureId, string job, string status)
        //{
        //    var list = new List<EmployeeTerminationSummaryByBussinessUnitVM>();
        //    var empPlacementsQueries = db.Placements.Where(x=>!x.isCurrent).ToList();
        //    var employeesQuery = db.Person_Employee.Where(x=>!x.IsActive).ToList();
        //    var termination = db.Terminations.Where(x=>x.IsOnlineApproved).ToList();
        //    // Filter placements by branch, business unit, and organizational structure
        //    if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
        //    {

        //        empPlacementsQueries = empPlacementsQueries.Where(x => x.BranchId == braId).ToList();
        //        var employeeIds = empPlacementsQueries.Select(x=>x.EmployeeId).Distinct();
        //        termination = termination.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();

        //    }
        //    if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int bUnitId))
        //    {
        //        empPlacementsQueries = empPlacementsQueries.Where(x => x.BusinessUnitId == bUnitId).ToList();
        //        var employeeIds = empPlacementsQueries.Select(x => x.EmployeeId).Distinct();
        //        termination = termination.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
        //    {
        //        var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
        //        empPlacementsQueries = empPlacementsQueries.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();
        //        var employeeIds = empPlacementsQueries.Select(x => x.EmployeeId).Distinct();
        //        termination = termination.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(job) && int.TryParse(job, out int lookUpId))
        //    {
        //        empPlacementsQueries = empPlacementsQueries.Where(x => x.LookupId == lookUpId).ToList();
        //        var employeeIds = empPlacementsQueries.Select(x => x.EmployeeId).Distinct();
        //        termination = termination.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //    }

        //    var terminateEmployeeIds = termination.Select(x => x.EmployeeId);
        //    empPlacementsQueries = empPlacementsQueries.Where(x => terminateEmployeeIds.Contains(x.EmployeeId)).ToList().DistinctByField(x=>x.EmployeeId).ToList();
        //    var businessUnitPlacements = empPlacementsQueries.Where(x=> terminateEmployeeIds.Contains(x.EmployeeId)).Select(x => x.BusinessUnitId).Distinct().ToList();

        //    foreach (var businessUnitPlacement in businessUnitPlacements)
        //    {
        //        var summary = new EmployeeTerminationSummaryByBussinessUnitVM();
        //        var businessUnit = db.BusinessUnits.Find(businessUnitPlacement);

        //        if (businessUnit != null)
        //        {
        //            summary.BussinesUnit = businessUnit.Name;

        //            var permanentEmployeeIds = empPlacementsQueries.Where(x => x.BusinessUnitId == businessUnitPlacement && x.EmployementType == EmployeeStatus.PERMANENT).Select(x => x.EmployeeId).Distinct().ToList();
        //            var permanentEmployees = employeesQuery.Where(x => permanentEmployeeIds.Contains(x.EmployeeId));
        //            summary.PermanentFemale = permanentEmployees.Count(e => e.Gender == Gender.FEMALE);
        //            summary.PermanentMale = permanentEmployees.Count(e => e.Gender == Gender.MALE);
        //            summary.TotalPermanentEmployee = summary.PermanentFemale + summary.PermanentMale;

        //            var contractEmployeeIds = empPlacementsQueries.Where(x => x.BusinessUnitId == businessUnitPlacement && x.EmployementType == EmployeeStatus.CONTRACT).Select(x => x.EmployeeId).Distinct().ToList();
        //            var contractEmployees = employeesQuery.Where(x => contractEmployeeIds.Contains(x.EmployeeId));
        //            summary.ContractFemale = contractEmployees.Count(e => e.Gender == Gender.FEMALE);
        //            summary.ContractMale = contractEmployees.Count(e => e.Gender == Gender.MALE);
        //            summary.TotalContaractEmployee = summary.ContractFemale + summary.ContractMale;

        //            var otherEmployeeIds = empPlacementsQueries.Where(x => x.BusinessUnitId == businessUnitPlacement && x.EmployementType != EmployeeStatus.PERMANENT && x.EmployementType != EmployeeStatus.CONTRACT).Select(x => x.EmployeeId).Distinct().ToList();
        //            var otherEmployees = employeesQuery.Where(x => otherEmployeeIds.Contains(x.EmployeeId));
        //            summary.OtherFemale = otherEmployees.Count(e => e.Gender == Gender.FEMALE);
        //            summary.OtherMale = otherEmployees.Count(e => e.Gender == Gender.MALE);
        //            summary.TotalOtherEmployee = summary.OtherFemale + summary.OtherMale;

        //            summary.TotalFemale = summary.PermanentFemale + summary.ContractFemale + summary.OtherFemale;
        //            summary.TotalMale = summary.PermanentMale + summary.ContractMale + summary.OtherMale;
        //            summary.TotalEmployee = summary.TotalFemale + summary.TotalMale;

        //            list.Add(summary);
        //        }
        //    }

        //    return list;
        //}
        public void Dispose()
        {
            db.Dispose();
        }

    }
}