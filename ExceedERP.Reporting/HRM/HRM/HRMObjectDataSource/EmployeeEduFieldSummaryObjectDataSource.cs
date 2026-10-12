using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEduFieldSummaryObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeEduFieldSummary(string branchId, string orgStructureId, string status, string edufield = "")
        {
            var list = new List<EmployeeSummary>();

            // Fetch educational fields
            var eduFieldsQuery = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_FIELD).AsQueryable();

            if (!string.IsNullOrEmpty(edufield) && int.TryParse(edufield, out int eduFieldId))
            {
                eduFieldsQuery = eduFieldsQuery.Where(x => x.LookupId == eduFieldId);
            }
            var eduFields = eduFieldsQuery.ToList();

            // Fetch all necessary data in a single query
            var employeesQuery = db.Person_Employee.AsQueryable();
            var placementsQuery = db.Placements.AsQueryable();

            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                var branchEmpIds = placementsQuery.Where(x => x.BranchId == braId).Select(x => x.EmployeeId);
                employeesQuery = employeesQuery.Where(x => branchEmpIds.Contains(x.EmployeeId));
            }

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
                var placementEmpIds = placementsQuery.Where(x => structureIds.Contains(x.OrgStructureId)).Select(x => x.EmployeeId);
                employeesQuery = employeesQuery.Where(x => placementEmpIds.Contains(x.EmployeeId));
            }

            if (status == "Active")
            {
                employeesQuery = employeesQuery.Where(x => x.IsActive);
            }
            else if (status == "InActive")
            {
                employeesQuery = employeesQuery.Where(x => !x.IsActive);
            }

            var employees = employeesQuery.ToList();

            if (!employees.Any() || !eduFields.Any())
            {
                return list; // No employees or education fields to process
            }

            var maleEmpIds = employees.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
            var femaleEmpIds = employees.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

            foreach (var field in eduFields)
            {
                var distinctMaleEmployees = db.Education
                    .Where(x => x.EduField == field.LookupId && maleEmpIds.Contains(x.EmployeeId))
                    .GroupBy(x => x.EmployeeId)
                    .Select(g => g.FirstOrDefault())
                    .ToList();

                var distinctFemaleEmployees = db.Education
                    .Where(x => x.EduField == field.LookupId && femaleEmpIds.Contains(x.EmployeeId))
                    .GroupBy(x => x.EmployeeId)
                    .Select(g => g.FirstOrDefault())
                    .ToList();

                var employeeSummary = new EmployeeSummary
                {
                    Criteria = "Field",
                    SummaryName = field.Description,
                    NumberOfMaleEmployee = distinctMaleEmployees.Count,
                    NumberOfFemaleEmployee = distinctFemaleEmployees.Count,
                    TotalEmployee = distinctMaleEmployees.Count + distinctFemaleEmployees.Count
                };

                if (employeeSummary.TotalEmployee > 0)
                {
                    list.Add(employeeSummary);
                }
            }

            return list;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        //public List<EmployeeSummary> GetEmployeeEduFieldSummary(string branchId, string orgStructureId, string status, string edufield = "")
        //{

        //    List<EmployeeSummary> list = new List<EmployeeSummary>();
        //    EmployeeSummary employee = new EmployeeSummary();

        //    var eduFields = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_FIELD).ToList();

        //    // emplist = HrHelper.AllPersonEmployeesOneBranch(branchId, orgStructureId, "");
        //    var employees = db.Person_Employee.ToList();

        //    var placements = db.Placements.ToList();

        //    if (branchId != null)
        //    {
        //        int.TryParse(branchId, out int braId);
        //        // placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
        //        var branchEmpId = placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).ToList();
        //        employees = employees.Where(x => branchEmpId.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();
        //    }
        //    if (orgStructureId != null)
        //    {
        //        int.TryParse(orgStructureId, out int orgId);
        //        // placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
        //        var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
        //        placements = placements.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();
        //        var placemenEmpIds = placements.Where(x => structureIds.Contains(x.OrgStructureId)).Select(x => x.EmployeeId).ToList();
        //        employees = employees.Where(x => placemenEmpIds.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();

        //    }

        //    if (status == "Active")
        //    {
        //        // placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements);
        //        employees = employees.Where(x => x.IsActive).ToList();

        //    }
        //    if (status == "InActive")
        //    {
        //        //placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);
        //        employees = employees.Where(x => x.IsActive).ToList();

        //    }

        //    if (edufield != null)
        //    {
        //        int.TryParse(edufield, out int lookupId);
        //        eduFields = eduFields.Where(x => x.LookupId == lookupId).ToList();
        //    }


        //    if (eduFields.Any())
        //    {
        //        //var malesId = emplist.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
        //        //var femalesId = emplist.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

        //        // var malesId = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId).ToList();    
        //        // var femalesId = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId).ToList();  
        //        var mempIds = employees.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
        //        var fempIds = employees.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

        //        foreach (var l in eduFields)
        //        {
        //            employee = new EmployeeSummary();
        //            employee.Criteria = "Field";
        //            employee.SummaryName = l.Description;

        //            employee.Criteria = "Edu level";
        //            employee.SummaryName = l.Description;

        //            employee.NumberOfMaleEmployee = db.Education.Where(x => x.EduField == l.LookupId && mempIds.Contains(x.EmployeeId)).Count();
        //            employee.NumberOfFemaleEmployee = db.Education.Where(x => x.EduField == l.LookupId && fempIds.Contains(x.EmployeeId)).Count();
        //            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

        //            if(employee.TotalEmployee > 0)
        //            {
        //                list.Add(employee);
        //            }                 
        //        }
        //    }

        //    return list;
        //}
    }
}