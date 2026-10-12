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
    public class EmployeeSummaryByBussinessUnitObjectDataSource:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummaryByBussinessUnitVM> GetEmployeeSummaryByBussinessUnit(string branchId, string businessUnitId, string orgStructureId, DateTime? fromAsssignmentDate, DateTime? toAssignmentDAte, string job, string status,DateTime?fromHireDate,DateTime?toHireDate)
        {
            var list = new List<EmployeeSummaryByBussinessUnitVM>();
            var empPlacementsQueries = db.Placements.ToList();
            var employeesQuery = db.Person_Employee.ToList();

            // Filter employees and placements by status
            if (status == "Active")
            {
                employeesQuery = employeesQuery.Where(x => x.IsActive).ToList();
                empPlacementsQueries = empPlacementsQueries.Where(x => x.isCurrent).ToList();
            }
            else if (status == "InActive")
            {
                employeesQuery = employeesQuery.Where(x => !x.IsActive).ToList();
                empPlacementsQueries = empPlacementsQueries.Where(x => !x.isCurrent).ToList();
            }
            if (fromAsssignmentDate.HasValue)
            {
                empPlacementsQueries = empPlacementsQueries.Where(x => x.AssignmentDate>=fromAsssignmentDate.Value).ToList();

            }
            if (toAssignmentDAte.HasValue)
            {
                empPlacementsQueries = empPlacementsQueries.Where(x => x.AssignmentDate<=toAssignmentDAte.Value).ToList();

            }
            if (fromHireDate.HasValue)
            {
                employeesQuery = employeesQuery.Where(x => x.HiredDate>=fromHireDate.Value).ToList();
                HashSet<int> empIds = new HashSet<int>(employeesQuery.Select(x => x.EmployeeId));
                empPlacementsQueries = empPlacementsQueries.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            }
            if (toHireDate.HasValue)
            {
                employeesQuery = employeesQuery.Where(x => x.HiredDate<=toHireDate.Value).ToList();
                HashSet<int> empIds = new HashSet<int>(employeesQuery.Select(x => x.EmployeeId));
                empPlacementsQueries = empPlacementsQueries.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            }
            // Filter placements by branch, business unit, and organizational structure
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                empPlacementsQueries = empPlacementsQueries.Where(x => x.BranchId == braId).ToList();
            }
            if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int bUnitId))
            {
                empPlacementsQueries = empPlacementsQueries.Where(x => x.BusinessUnitId == bUnitId).ToList();
            }
            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
                empPlacementsQueries = empPlacementsQueries.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();
            }
            if (!string.IsNullOrEmpty(job) && int.TryParse(job, out int lookUpId))
            {
                empPlacementsQueries = empPlacementsQueries.Where(x => x.LookupId == lookUpId).ToList();
            }
           
            // Get distinct business units
            var businessUnitPlacements = empPlacementsQueries.Select(x => x.BusinessUnitId).Distinct().ToList();

            foreach (var businessUnitPlacement in businessUnitPlacements)
            {
                var summary = new EmployeeSummaryByBussinessUnitVM();
                var businessUnit = db.BusinessUnits.Find(businessUnitPlacement);

                if (businessUnit != null)
                {
                    summary.BussinesUnit = businessUnit.Name;

                    var permanentEmployeeIds = empPlacementsQueries.Where(x => x.BusinessUnitId == businessUnitPlacement && x.EmployementType == EmployeeStatus.PERMANENT).Select(x => x.EmployeeId).Distinct().ToList();
                    var permanentEmployees = employeesQuery.Where(x => permanentEmployeeIds.Contains(x.EmployeeId));
                    summary.PermanentFemale = permanentEmployees.Count(e => e.Gender == Gender.FEMALE);
                    summary.PermanentMale = permanentEmployees.Count(e => e.Gender == Gender.MALE);
                    summary.TotalPermanentEmployee = summary.PermanentFemale + summary.PermanentMale;

                    var contractEmployeeIds = empPlacementsQueries.Where(x => x.BusinessUnitId == businessUnitPlacement && x.EmployementType == EmployeeStatus.CONTRACT).Select(x => x.EmployeeId).Distinct().ToList();
                    var contractEmployees = employeesQuery.Where(x => contractEmployeeIds.Contains(x.EmployeeId));
                    summary.ContractFemale = contractEmployees.Count(e => e.Gender == Gender.FEMALE);
                    summary.ContractMale = contractEmployees.Count(e => e.Gender == Gender.MALE);
                    summary.TotalContaractEmployee = summary.ContractFemale + summary.ContractMale;

                    var otherEmployeeIds = empPlacementsQueries.Where(x => x.BusinessUnitId == businessUnitPlacement && x.EmployementType != EmployeeStatus.PERMANENT && x.EmployementType != EmployeeStatus.CONTRACT).Select(x => x.EmployeeId).Distinct().ToList();
                    var otherEmployees = employeesQuery.Where(x => otherEmployeeIds.Contains(x.EmployeeId));
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

        //public List<EmployeeSummaryByBussinessUnitVM> GetEmployeeSummaryByBussinessUnit(string branchId, string businessUnitId, string orgStructureId, string job, string status)
        //{
        //    var list = new List<EmployeeSummaryByBussinessUnitVM>();
        //    var empPlacementsQueries = db.Placements.AsQueryable();
        //    var employeesQuery = db.Person_Employee.AsQueryable();

        //    // Filter employees and placements by status
        //    if (status == "Active")
        //    {
        //        employeesQuery = employeesQuery.Where(x => x.IsActive);
        //        empPlacementsQueries = empPlacementsQueries.Where(x => x.isCurrent);
        //    }
        //    else if (status == "InActive")
        //    {
        //        employeesQuery = employeesQuery.Where(x => !x.IsActive);
        //        empPlacementsQueries = empPlacementsQueries.Where(x => !x.isCurrent);
        //    }

        //    // Filter placements by branch, business unit, and organizational structure
        //    if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
        //    {
        //        empPlacementsQueries = empPlacementsQueries.Where(x => x.BranchId == braId);
        //    }
        //    if (!string.IsNullOrEmpty(businessUnitId) && int.TryParse(businessUnitId, out int bUnitId))
        //    {
        //        empPlacementsQueries = empPlacementsQueries.Where(x => x.BusinessUnitId == bUnitId);
        //    }
        //    if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
        //    {
        //        var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
        //        empPlacementsQueries = empPlacementsQueries.Where(x => structureIds.Contains(x.OrgStructureId));
        //    }
        //    if (!string.IsNullOrEmpty(job) && int.TryParse(job, out int lookUpId))
        //    {
        //        empPlacementsQueries = empPlacementsQueries.Where(x => x.LookupId == lookUpId);
        //    }
        //    // Join employees with placements
        //    var PlacementBybussinessUnits = empPlacementsQueries/*.Select(x => x.BusinessUnitId)*/.DistinctByField(x=>x.BusinessUnitId).ToList();
        //    foreach(var PlacementBybussinessUnit in PlacementBybussinessUnits)
        //    {
        //        var permanentEmployeeIds = empPlacementsQueries.Where(x=>x.BusinessUnitId== PlacementBybussinessUnit.BusinessUnitId
        //                                                                &&x.EmployementType== EmployeeStatus.PERMANENT)
        //                                                                .Select(x => x.EmployeeId).Distinct().ToList();
        //        var permannetemployees = employeesQuery.Where(x => permanentEmployeeIds.Contains(x.EmployeeId));
        //        var contractEmployeeIds = empPlacementsQueries.Where(x=>x.BusinessUnitId== PlacementBybussinessUnit.BusinessUnitId
        //                                                                &&x.EmployementType== EmployeeStatus.CONTRACT)
        //                                                                .Select(x => x.EmployeeId).Distinct().ToList();
        //        var contractEmployees = employeesQuery.Where(x => permanentEmployeeIds.Contains(x.EmployeeId));
        //        var ottherEmployeeIds = empPlacementsQueries.Where(x=>x.BusinessUnitId== PlacementBybussinessUnit.BusinessUnitId
        //                                                                &&x.EmployementType != EmployeeStatus.PERMANENT
        //                                                                &&x.EmployementType != EmployeeStatus.CONTRACT)
        //                                                                .Select(x => x.EmployeeId).Distinct().ToList();
        //        var otherEmployees = employeesQuery.Where(x => permanentEmployeeIds.Contains(x.EmployeeId));

        //        var summary = new EmployeeSummaryByBussinessUnitVM();
        //        var bussinessUnit = db.BusinessUnits.Find(PlacementBybussinessUnit.BusinessUnitId);
        //        if(bussinessUnit != null)
        //        {
        //            summary.BussinesUnit = bussinessUnit.Name;
        //            summary.PermanentFemale = permannetemployees.Count(e => e.Gender == Gender.FEMALE);
        //            summary.PermanentMale = permannetemployees.Count(e => e.Gender == Gender.MALE);
        //            summary.TotalPermanentEmployee = summary.PermanentFemale+ summary.PermanentMale;
        //            summary.ContractFemale = permannetemployees.Count(e => e.Gender == Gender.FEMALE);
        //            summary.ContractMale = permannetemployees.Count(e => e.Gender == Gender.MALE);
        //            summary.TotalContaractEmployee = summary.ContractFemale+ summary.ContractMale;
        //            summary.OtherFemale = permannetemployees.Count(e => e.Gender == Gender.FEMALE);
        //            summary.OtherMale = permannetemployees.Count(e => e.Gender == Gender.MALE);
        //            summary.TotalOtherEmployee = summary.OtherMale+ summary.OtherFemale;
        //            summary.TotalFemale = summary.OtherFemale + summary.ContractFemale+ summary.PermanentFemale;
        //            summary.TotalMale = summary.PermanentMale + summary.ContractMale+ summary.OtherMale;
        //            summary.TotalEmployee = summary.TotalFemale + summary.TotalMale;
        //            list.Add(summary)

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