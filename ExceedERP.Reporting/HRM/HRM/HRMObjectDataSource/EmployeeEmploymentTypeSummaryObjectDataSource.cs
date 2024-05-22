using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEmploymentTypeSummaryObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeTypeSummary(string branchId, object[] orgStructureIds, string status, string source, string empStatus = "")
        {
            var list = GetEmployees(branchId, orgStructureIds, status);

            return list.OrderBy(x => x.SummaryName).ToList();
        }

        public List<EmployeeSummary> GetEmployees(string branchId, object[] orgStructureIds, string status)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();
           
            var placements = db.Placements.ToList();
            //var employeeIds = new HashSet<int>(placementsss.Select(x=>x.EmployeeId));
            //var place = db.Placements.Where(x=>employeeIds.Contains(x.EmployeeId)).ToList();
            var employees = db.Person_Employee.ToList();
            if (!string.IsNullOrEmpty(branchId))
            {
                int.TryParse(branchId, out int braId);
                // placementsss = placementsss.Where(x => x.BranchId == braId).ToList();
                var branchEmpId = placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).ToList();
                employees = employees.Where(x => branchEmpId.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();
            }
           
            if (orgStructureIds !=null)
            {
                List<int> orgStructIds = new List<int>();
                var orgIds = orgStructureIds.ToList();

                foreach (var orgid in orgIds)
                {
                    string orgstId = orgid.ToString();
                    int.TryParse(orgstId, out int orgId);
                    orgStructIds.Add(orgId);
                }
                if (orgStructIds.Any())
                {
                    //int.TryParse(orgStructureId, out int orgId);
                    // var emIds = place.Where(x => x.OrgStructureId == orgId).Select(x => x.EmployeeId).ToList();
                    // placementsss = placementsss.Where(x => x.OrgStructureId == orgId).ToList();
                    var structureIds = db.MainCompanyStructures.Where(x => orgStructIds.Contains(x.Parent)).Select(x => x.OrgStructureId).ToList();
                    placements = placements.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();
                    var placemenEmpIds = placements.Where(x => structureIds.Contains(x.OrgStructureId)).Select(x => x.EmployeeId).ToList();
                    employees = employees.Where(x => placemenEmpIds.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();

                }

            }
            //if (!string.IsNullOrEmpty(orgStructureId))
            //{
            //    //int.TryParse(orgStructureId, out int orgId);
            //    // var emIds = place.Where(x => x.OrgStructureId == orgId).Select(x => x.EmployeeId).ToList();
            //    // placementsss = placementsss.Where(x => x.OrgStructureId == orgId).ToList();
            //    var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
            //    placements = placements.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();
            //    var placemenEmpIds = placements.Where(x => structureIds.Contains(x.OrgStructureId)).Select(x => x.EmployeeId).ToList();
            //    employees = employees.Where(x => placemenEmpIds.Contains(x.EmployeeId)).DistinctByField(x => x.EmployeeId).ToList();

            //}

            if (status == "Active")
            {
                //placementsss = placementsss.Where(x => x.isCurrent).ToList();
                employees = employees.Where(x => x.IsActive).ToList();

            }
            if (status == "InActive")
            {
                // placementsss = placementsss.Where(x => !x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
                employees = employees.Where(x => !x.IsActive).ToList();

            }

            var males = db.Person_Employee.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
            var females = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();

            var allTypes = Enum.GetValues(typeof(EmployeeStatus))
                           .Cast<EmployeeStatus>()
                           .ToList().Select(s => new { Text = s.ToString(), Value = s.ToString() });

            //foreach (var a in allTypes)
            //{
            //    if (a.Value.ToUpper() == "PERMANENT")
            //    {

            //        employees = employees.Where(x => x.Status == EmployeeStatus.PERMANENT).ToList();
            //        //var emplistByStatus = placementsss.Where(x => x.Status.ToString() == a.Value).ToList();
            //       // placementsss = placementsss.Where(x => a.Value.Contains(x.EmployementType.ToString())).ToList();
            //        var employee = new EmployeeSummary();
            //        employee.Criteria = "Employement Type";
            //       // employee.SummaryName = GetStatus(a.Value.ToString());
            //        employee.SummaryName = "PERMANENT";
            //        //employee.NumberOfFemaleEmployee = emplistByStatus.Where(x=>x.Gender == Gender.FEMALE).Count();
            //        //employee.NumberOfMaleEmployee = emplistByStatus.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.NumberOfFemaleEmployee = employees.Where(x => x.Gender == Gender.FEMALE).Count();
            //        employee.NumberOfMaleEmployee = employees.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

            //        list.Add(employee);
            //    }
            //    if (a.Value.ToUpper() == "CONTRACT")
            //    {

            //        //employees = employees.Where(x => a.Value.Contains(x.Status.ToString())).ToList();
            //        employees = employees.Where(x => x.Status== EmployeeStatus.CONTRACT).ToList();

            //        //var emplistByStatus = placementsss.Where(x => x.Status.ToString() == a.Value).ToList();
            //        // placementsss = placementsss.Where(x => a.Value.Contains(x.EmployementType.ToString())).ToList();
            //        var employee = new EmployeeSummary();
            //        employee.Criteria = "Employement Type";
            //        // employee.SummaryName = GetStatus(a.Value.ToString());
            //        employee.SummaryName = "CONTRACT";
            //        //employee.NumberOfFemaleEmployee = emplistByStatus.Where(x=>x.Gender == Gender.FEMALE).Count();
            //        //employee.NumberOfMaleEmployee = emplistByStatus.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.NumberOfFemaleEmployee = employees.Where(x => x.Gender == Gender.FEMALE).Count();
            //        employee.NumberOfMaleEmployee = employees.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

            //        list.Add(employee);
            //    }
            //    if (a.Value.ToUpper() == "PROBATION")
            //    {

            //        employees = employees.Where(x => x.Status == EmployeeStatus.PROBATION).ToList();
            //        //employees = employees.Where(x => a.Value.ToUpper().Contains(x.Status.ToString().ToUpper())).ToList();
            //        //var emplistByStatus = placementsss.Where(x => x.Status.ToString() == a.Value).ToList();
            //        // placementsss = placementsss.Where(x => a.Value.Contains(x.EmployementType.ToString())).ToList();
            //        var employee = new EmployeeSummary();
            //        employee.Criteria = "Employement Type";
            //        // employee.SummaryName = GetStatus(a.Value.ToString());
            //        employee.SummaryName = "PROBATION";
            //        //employee.NumberOfFemaleEmployee = emplistByStatus.Where(x=>x.Gender == Gender.FEMALE).Count();
            //        //employee.NumberOfMaleEmployee = emplistByStatus.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.NumberOfFemaleEmployee = employees.Where(x => x.Gender == Gender.FEMALE).Count();
            //        employee.NumberOfMaleEmployee = employees.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

            //        list.Add(employee);
            //    }
            //    if (a.Value.ToUpper() == "FREELANCER")
            //    {
            //        employees = employees.Where(x => x.Status == EmployeeStatus.FREELANCER).ToList();
            //        //employees = employees.Where(x => a.Value.ToUpper().Contains(x.Status.ToString().ToUpper())).ToList();
            //        //var emplistByStatus = placementsss.Where(x => x.Status.ToString() == a.Value).ToList();
            //        // placementsss = placementsss.Where(x => a.Value.Contains(x.EmployementType.ToString())).ToList();
            //        var employee = new EmployeeSummary();
            //        employee.Criteria = "Employement Type";
            //        // employee.SummaryName = GetStatus(a.Value.ToString());
            //        employee.SummaryName = "FREELANCER";
            //        //employee.NumberOfFemaleEmployee = emplistByStatus.Where(x=>x.Gender == Gender.FEMALE).Count();
            //        //employee.NumberOfMaleEmployee = emplistByStatus.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.NumberOfFemaleEmployee = employees.Where(x => x.Gender == Gender.FEMALE).Count();
            //        employee.NumberOfMaleEmployee = employees.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

            //        list.Add(employee);
            //    }
            //    if (a.Value.ToUpper() == "NO_STATUS")
            //    {
            //        employees = employees.Where(x => x.Status == EmployeeStatus.NO_STATUS).ToList();
            //        // employees = employees.Where(x => a.Value.ToUpper().Contains(x.Status.ToString().ToUpper())).ToList();
            //        //var emplistByStatus = placementsss.Where(x => x.Status.ToString() == a.Value).ToList();
            //        // placementsss = placementsss.Where(x => a.Value.Contains(x.EmployementType.ToString())).ToList();
            //        var employee = new EmployeeSummary();
            //        employee.Criteria = "Employement Type";
            //        // employee.SummaryName = GetStatus(a.Value.ToString());
            //        employee.SummaryName = "NO_STATUS";
            //        //employee.NumberOfFemaleEmployee = emplistByStatus.Where(x=>x.Gender == Gender.FEMALE).Count();
            //        //employee.NumberOfMaleEmployee = emplistByStatus.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.NumberOfFemaleEmployee = employees.Where(x => x.Gender == Gender.FEMALE).Count();
            //        employee.NumberOfMaleEmployee = employees.Where(x => x.Gender == Gender.MALE).Count();
            //        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

            //        list.Add(employee);
            //    }

            //}
            foreach (var a in allTypes)
            {
                var emplistByStatus = employees.Where(x => x.Status.ToString() == a.Value).DistinctByField(x => x.EmployeeId).ToList();

                var employee = new EmployeeSummary();

                employee.Criteria = "Employement Type";

                employee.SummaryName = GetStatus(a.Value.ToString());
                employee.NumberOfFemaleEmployee = emplistByStatus.Where(x => females.Contains(x.EmployeeId)).Count();
                employee.NumberOfMaleEmployee = emplistByStatus.Where(x => males.Contains(x.EmployeeId)).Count();
                employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

                list.Add(employee);

            }
            return list;
        }
        public string GetStatus(string empStatus)
        {
            switch (empStatus)
            {
                case "PERMANENT":
                    return "Permanent";

                case "CONTRACT":
                    return "Contract";

                case "PROBATION":
                    return "Probation";
                case "FREELANCER":
                    return "Freelancer";
                case "NO_STATUS":
                    return "No Status";

                case "SPECIAL":
                    return "Special";


            }
            return empStatus;
        }

    }

    public class EmployeesVm
    {
        public int EmployeeId { get; set; }
        public Gender Gender { get; set; }
        public EmployeeStatus EmployeeStatus { get; set; }
        public int BranchId { get; set; }
        public int OrgStructureId { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }
    }
}