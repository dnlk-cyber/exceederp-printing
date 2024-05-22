using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using XAPI;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePerforNotEvaluatedEmployeeObjectDataSource : List<EmployeePerfromanceVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePerfromanceVM> GetEmployeePerformance(string branchId, string orgStructureId, string year, string status, string empId, string businessUnit)
        {
            List<EmployeePerfromanceVM> employeePerformanceList = new List<EmployeePerfromanceVM>();

            int.TryParse(year, out int yearId);

            var placements = db.Placements.Where(x => x.isCurrent).ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = placements.Where(x => x.BranchId == braId).OrderByDescending(x => x.AssignmentDate).ToList();
            }

            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = placements.Where(x => x.OrgStructureId == orgId).ToList();
            }


            if (empId != null)
            {
                int.TryParse(empId, out int employeeId);
                placements = placements.Where(x => x.EmployeeId == employeeId).ToList();
            }
            if (businessUnit != null)
            {
                int.TryParse(businessUnit, out int businessUnitId);
                placements = placements.Where(x => x.BusinessUnitId == businessUnitId).ToList();
            }
            var employeePerformances = db.EmployeePerformances.Where(x=>x.IsSecondLevelApproved).ToList();
            
            if (placements.Any() && employeePerformances.Any())
            {
                var employeeIds = employeePerformances.Select(x => x.EmployeeId);
                 placements = placements.Where(x => !employeeIds.Contains(x.EmployeeId)).ToList();

            }
           
            foreach (var place in placements)
            {

                var empPerformance = new EmployeePerfromanceVM();
                var employeePerformance = db.EmployeePerformances.Where(x=>x.EmployeeId==place.EmployeeId).Where(x => !x.IsSecondLevelApproved).FirstOrDefault();
                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                if (emp != null&& employeePerformance !=null)
                {
                    empPerformance.EmployeeId = emp.EmployeeCode;
                    empPerformance.FullName = emp.FullNameaEnglish;
                    empPerformance.Sex = emp.Gender.ToString();
                    empPerformance.Status = "In-Process";
                }
                else if (emp != null&& employeePerformance ==null)
                {
                    empPerformance.EmployeeId = emp.EmployeeCode;
                    empPerformance.FullName = emp.FullNameaEnglish;
                    empPerformance.Sex = emp.Gender.ToString();
                    empPerformance.Status = "Not-Evaluated";
                }

                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                if (mainCompany != null)
                {
                    empPerformance.Department = mainCompany.Name;
                }
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                if (branch != null)
                {
                    empPerformance.Branch = branch.Name;
                }

                
                employeePerformanceList.Add(empPerformance);
               
            }

            return employeePerformanceList;
        }

    }
}