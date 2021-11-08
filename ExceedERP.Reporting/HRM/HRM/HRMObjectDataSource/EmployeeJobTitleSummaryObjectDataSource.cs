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
    public class EmployeeJobTitleSummaryObjectDataSource : List<EmployeeSummary>
    {

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetEmployeeJobTitleSummary(string branchId, string orgStructureId, string job, string status)
        {

            List<EmployeeSummary> list = new List<EmployeeSummary>();

            var employee = new EmployeeSummary();

            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();

            int index = 0;

            var empPlacement = db.Placements.ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                empPlacement = empPlacement.Where(x => x.BranchId == braId).ToList();
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                // empPlacement = empPlacement.Where(x => x.OrgStructureId == orgId).ToList();
                empPlacement = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(empPlacement, orgId);
                /// and here filter jobtitles under this department

                var lookupIds = empPlacement.Select(x => x.LookupId).ToList();
                lookups = lookups.Where(x => lookupIds.Contains(x.LookupId)).ToList();

            }
            if (job != null)
            {
                int.TryParse(job, out int lookUpId);
                empPlacement = empPlacement.Where(x => x.LookupId == lookUpId).ToList();
               
                /// filter by jobtitle here 
                lookups = lookups.Where(x => x.LookupId == lookUpId).ToList();
            }

            var males = db.Person_Employee.Where(x => x.Gender == Gender.MALE).ToList();
            var females = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).ToList();

            var malesId = males.Select(x => x.EmployeeId).ToList();
            var femalesId = females.Select(x => x.EmployeeId).ToList();


            if (status == "Active")
            {
                malesId = males.Where(x => x.IsActive).Select(x => x.EmployeeId).ToList();
                femalesId = females.Where(x => x.IsActive).Select(x => x.EmployeeId).ToList();
                empPlacement = empPlacement.Where(x => x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
            }
            if (status == "InActive")
            {
                malesId = males.Where(x => !x.IsActive).Select(x => x.EmployeeId).ToList();
                femalesId = females.Where(x => !x.IsActive).Select(x => x.EmployeeId).ToList();
                empPlacement = empPlacement.Where(x => !x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
            }


            empPlacement = empPlacement.DistinctByField(x => x.EmployeeId).ToList();


            foreach (var look in lookups)
            {
                employee = new EmployeeSummary();

                employee.index = index;
                employee.SummaryName = look.Description;
                employee.NumberOfFemaleEmployee = empPlacement.Where(x => femalesId.Contains(x.EmployeeId) && x.LookupId == look.LookupId).Count();
                employee.NumberOfMaleEmployee = empPlacement.Where(x => malesId.Contains(x.EmployeeId) && x.LookupId == look.LookupId).Count();
                employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;

                list.Add(employee);
                index++;
            }


            employee = new EmployeeSummary();

            var countM = 0;
            var countF = 0;

            foreach (var id in malesId)
            {
                var place = db.Placements.Where(x => x.EmployeeId == id);
                if (place == null)
                {
                    countM = countM + 1;
                }
            }

            foreach (var id in femalesId)
            {
                var place = db.Placements.FirstOrDefault(x => x.EmployeeId == id);
                if (place == null)
                {
                    countF = countF + 1;
                }
            }

            employee.SummaryName = "ያልተመደቡ ";
            employee.NumberOfMaleEmployee = countM;
            employee.NumberOfFemaleEmployee = countF;
            employee.TotalEmployee = employee.NumberOfMaleEmployee + employee.NumberOfFemaleEmployee;

            employee.index = index++;

            list.Add(employee);

            return list;
        }


    }
}