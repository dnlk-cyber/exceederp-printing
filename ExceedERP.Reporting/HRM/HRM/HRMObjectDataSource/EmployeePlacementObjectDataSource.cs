using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePlacementObjectDataSource : List<EmployeePlacementVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePlacementVM> GetEmployeePlacment(string EmpId ,string branchId, string ParentOrStructureId, string orgStructureId,DateTime? FromAssignmentDate,DateTime? toAssignmentDate, string status)
        {
            List<EmployeePlacementVM> list = new List<EmployeePlacementVM>();
            var placements = db.Placements.ToList();
          
            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                 placements = placements.Where(x => x.BranchId == braId).ToList();
            }
            if (ParentOrStructureId != null)
            {
                int.TryParse(ParentOrStructureId, out int orgId);
                placements = placements.Where(x => x.ParentOrgStructureId==orgId).ToList();
            }

            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = placements.Where(x => x.OrgStructureId==orgId).ToList();


            }

            if (status !=null)
            {
                if(status == "Active")
                {
                    placements = placements.Where(x => x.isCurrent).ToList();

                }
                if (status == "InActive")
                {
                    placements = placements.Where(x => !x.isCurrent).ToList();

                }

            }
            if (FromAssignmentDate.HasValue)
            {
                placements = placements.Where(x => x.AssignmentDate.Date>=FromAssignmentDate.Value.Date).ToList();

            }
            if (toAssignmentDate.HasValue)
            {
                placements = placements.Where(x => x.AssignmentDate.Date<=toAssignmentDate.Value.Date).ToList();

            }
            foreach (var place in placements)
            {
                var employeeplacement = new EmployeePlacementVM();
                employeeplacement.EmployeeId = place.EmployeeId;
                employeeplacement.EmployeeCode = place.EmployeeCode;
                var employee = db.Person_Employee.Find(place.EmployeeId);
                if (employee != null)
                {
                    employeeplacement.FullName = employee.FullNameaEnglish;
                    employeeplacement.HireDate = employee.HiredDate.ToString("dd-MM-yyyy") + "(" + employee.HiredDateAmharic + ")";

                }
                var branch = db.Branch.FirstOrDefault(x=>x.BranchId==place.BranchId);
                if (branch != null)
                {
                    employeeplacement.Branch = branch.Name;

                }
                var bussnessUnit = db.BusinessUnits.FirstOrDefault(x=>x.BusinessUnitId==place.BusinessUnitId);
                if (bussnessUnit != null)
                {
                    employeeplacement.BussinessUnit = bussnessUnit.Name;
                }
                var workUnit = db.MainCompanyStructures.FirstOrDefault(x => x.Parent == place.ParentOrgStructureId);
                if (workUnit != null)
                {
                    employeeplacement.ParentStructureName = workUnit.Name;

                }
                var orgstruc = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId==place.OrgStructureId);
                if (orgstruc != null)
                {
                    employeeplacement.Departement = orgstruc.Name;

                }
                employeeplacement.Boss = place.Boss;
                //var jobTitle = db.Lookups.Find(place.LookupId);
                //if (jobTitle != null)
                //{
                //    employeeplacement.JobTitle =jobTitle.Description

                //}
                employeeplacement.JobTitle = place.JobTitleName;
                employeeplacement.AssignmentDate = place.AssignmentDate;
                var rank = db.Ranks.FirstOrDefault(x=>x.RankId==place.RankId);
                if(rank != null)
                {
                    employeeplacement.Grade = rank.RankName;

                }
                var step = db.Steps.FirstOrDefault(x=>x.StepId==place.StepId);
                if(step !=null)
                {
                    employeeplacement.Step = step.StepName;
                }
                employeeplacement.Salary = place.Salary;
                employeeplacement.AssignmentType = place.AssignmentType.ToString();
                list.Add(employeeplacement);
            }
            list = list.OrderByDescending(x => x.AssignmentDate.Date).ThenByDescending(x => x.EmployeeId).ToList();

            return list ;
          
        }
    }
}