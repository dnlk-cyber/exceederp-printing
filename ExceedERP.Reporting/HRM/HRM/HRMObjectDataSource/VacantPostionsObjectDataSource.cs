using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System.Threading;
using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class VacantPostionsObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public VacantPostions GetVacantPostions(string branchId, string org, string title)
        {
            VacantPostions VacantPostions = new VacantPostions();
            List<Person_Employee> employees;

            VacantPostions = new VacantPostions();
            //var cultureName = Thread.CurrentThread.CurrentCulture.Name;
            var cultureName = GlobalHelper.GetCurrentCulture();
            employees = HrHelper.PersonEmployeesOneBranch(branchId, org, "");
          

            var toBepostions = db.JobTitleMapping.ToList();
            var currentStaff = db.Placements.Where(x => x.isCurrent).ToList();

            if(branchId != null)
            {
                int.TryParse(branchId, out int braId);
                currentStaff = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(currentStaff, braId);
                toBepostions = toBepostions.Where(x=>x.BranchId == braId).ToList();
            }
            if(org != null)
            {
                int.TryParse(org, out int orgId);
                currentStaff = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(currentStaff, orgId);
                toBepostions = toBepostions.Where(x=>x.OrgStructureId == orgId).ToList();
            }
            if(title != null)
            {
                int.TryParse(title, out int lookupId);
                currentStaff = EmployeeHelper.GetEmployees_ByJobTitle_FromPlacement(currentStaff, lookupId);
                toBepostions = toBepostions.Where(x=>x.LookupId == lookupId).ToList();
            }

            VacantPostions.ToBeStaff = toBepostions.Sum(x=>x.Quntity);

            var males = EmployeeHelper.Get_Male_EmployeesFrom_Placement(currentStaff).Select(x => x.EmployeeId);
            var females = EmployeeHelper.Get_Female_EmployeesFrom_Placement(currentStaff).Select(x=>x.EmployeeId);

            VacantPostions.NumberOfMaleEmployee = currentStaff.Where(x => males.Contains(x.EmployeeId)).Count();
            VacantPostions.NumberOfFemaleEmployee = currentStaff.Where(x => females.Contains(x.EmployeeId)).Count();
            VacantPostions.Status = "";          

            VacantPostions.TotalEmployee = VacantPostions.NumberOfFemaleEmployee + VacantPostions.NumberOfMaleEmployee;
            VacantPostions.VacantPos = VacantPostions.ToBeStaff - VacantPostions.TotalEmployee;
            if(VacantPostions.VacantPos < 0&&(cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et"))
            {
                VacantPostions.Status = "ከተፈቀደው በላይ የተመደበ";
            }
            if (VacantPostions.VacantPos < 0 && (cultureName.ToLower() == "om" ))
            {
                VacantPostions.Status = "ol kaa’ameera";
            }
            if (VacantPostions.VacantPos < 0 && (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us"))
            {
                VacantPostions.Status = "over Placed";
            }
            return VacantPostions;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}