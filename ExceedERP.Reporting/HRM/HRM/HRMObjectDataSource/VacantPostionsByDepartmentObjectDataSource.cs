using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;


namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class VacantPostionsByDepartmentObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<VacantPostionByDept> GetVacantPostionByDept(string org, string branchId,string businessUnit, string status)
        {
            VacantPostionByDept vacantPostions = new VacantPostionByDept();
            List<VacantPostionByDept> vacantPostionsList = new List<VacantPostionByDept>();
            vacantPostions = new VacantPostionByDept();
            vacantPostions.vacantDetail = new List<VacantDetail>();
            VacantDetail VacantDetail = new VacantDetail();

            List<Person_Employee> employees;

          //  employees = HrHelper.PersonEmployeesOneBranch(branchId, org, "");

            var currentStaff = db.Placements/*.Where(x => x.isCurrent)*/.ToList();
            //var currentOrgStId = currentStaff.Select(x => x.OrgStructureId);
            var toBepostions = db.JobTitleMapping.ToList();
            //var departmentList = from m in db.MainCompanyStructures
            //                     join j in toBepostions
            //                         on m.OrgStructureId equals j.OrgStructureId

            //                     select (new { m.OrgStructureId, m.Name });
            var filtered = db.MainCompanyStructures/*.Where(x=>currentOrgStId.Contains(x.OrgStructureId))*/.ToList();
           // var branch = db.Branch.ToList();

            if (!string.IsNullOrEmpty(status) &&status.ToLower() =="active")
            {
                currentStaff = currentStaff.Where(x => x.isCurrent).ToList();
                toBepostions = toBepostions.Where(x => x.IsActive).ToList();
                filtered = filtered.Where(x => x.IsActive).ToList();

            }
            if (!string.IsNullOrEmpty(status) &&status.ToLower() =="inactive")
            {
                currentStaff = currentStaff.Where(x => !x.isCurrent).ToList();
                toBepostions = toBepostions.Where(x => !x.IsActive).ToList();
                filtered = filtered.Where(x => !x.IsActive).ToList();


            }
            if (!string.IsNullOrEmpty(businessUnit))
            {
                int.TryParse(businessUnit, out int businesUnitId);
                toBepostions = toBepostions.Where(x => x.BusinessUnitId == businesUnitId).ToList();
                currentStaff = currentStaff.Where(x => x.BusinessUnitId==businesUnitId).ToList();
            }
            if (!string.IsNullOrEmpty(org ))
            {
                int.TryParse(org, out int orgId);
                filtered = filtered.Where(x => x.OrgStructureId == orgId).ToList();
                toBepostions = toBepostions.Where(x => x.OrgStructureId == orgId).ToList();
                currentStaff = currentStaff.Where(x => x.OrgStructureId == orgId).ToList();

            }
            if (!string.IsNullOrEmpty(branchId))
            {
                int.TryParse(branchId, out int branId);
                toBepostions = toBepostions.Where(x => x.BranchId == branId).ToList();
                currentStaff = currentStaff.Where(x =>   x.BranchId == branId).ToList();

            }
            var placOrgIds = currentStaff.Select(x => x.OrgStructureId).Distinct();
            var orgIds = toBepostions.Where(x=>placOrgIds.Contains(x.OrgStructureId)).Select(x => x.OrgStructureId).ToList();
            filtered = filtered.Where(x => orgIds.Contains(x.OrgStructureId)).Distinct().ToList();

            foreach (var filter in filtered)
            {
                vacantPostions = new VacantPostionByDept();
                vacantPostions.vacantDetail = new List<VacantDetail>();

                vacantPostions.Department = filter.Name;
                var toBe = toBepostions.Where(x => x.OrgStructureId == filter.OrgStructureId).ToList();
                var currentPlacement = currentStaff.Where(x => x.OrgStructureId == filter.OrgStructureId).ToList();
                foreach (var to in toBe)
                {
                    VacantDetail = new VacantDetail();
                    VacantDetail.Jobtitle = to.Lookup.Description;
                    VacantDetail.Shouldbe = to.Quntity;// toBe.Where(x=>x.LookupId==to.LookupId).Count();
                    VacantDetail.Current = currentPlacement.Where(x => x.LookupId == to.LookupId).Count();
                    VacantDetail.Vacant = VacantDetail.Shouldbe - VacantDetail.Current;
                    vacantPostions.vacantDetail.Add(VacantDetail);
                }


                vacantPostionsList.Add(vacantPostions);
            }

            return vacantPostionsList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}