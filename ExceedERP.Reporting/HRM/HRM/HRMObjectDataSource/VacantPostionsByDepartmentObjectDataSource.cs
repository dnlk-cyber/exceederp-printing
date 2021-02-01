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
    public class VacantPostionsByDepartmentObjectDataSource : List<VacantPostionByDept>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<VacantPostionByDept> GetVacantPostionByDept(string org, string branchId)
        {
            VacantPostionByDept vacantPostions = new VacantPostionByDept();
            List<VacantPostionByDept> vacantPostionsList = new List<VacantPostionByDept>();
            vacantPostions = new VacantPostionByDept();
            vacantPostions.vacantDetail = new List<VacantDetail>();
            VacantDetail VacantDetail = new VacantDetail();
            List<Person_Employee> employees;

            employees = HrHelper.PersonEmployeesOneBranch(branchId, org, "");
            var toBepostions = db.JobTitleMapping.ToList();
            var currentStaff = db.Placements.Where(x => x.isCurrent).ToList();
            var departmentList = from m in db.MainCompanyStructures
                                 join j in toBepostions
                                     on m.OrgStructureId equals j.OrgStructureId

                                 select (new { m.OrgStructureId, m.Name });
            var filtered = db.MainCompanyStructures.ToList();
            var branch = db.Branch.ToList();
            if (org != null)
            {

                filtered = filtered.Where(x => x.OrgStructureId.ToString() == org).ToList();

            }


            foreach (var filter in filtered)
            {
                vacantPostions = new VacantPostionByDept();
                vacantPostions.vacantDetail = new List<VacantDetail>();

                vacantPostions.Department = filter.Name;
                var toBe = toBepostions.Where(x => x.OrgStructureId == filter.OrgStructureId).ToList();
                var currentPlacement = currentStaff.Where(x => x.OrgStructureId == filter.OrgStructureId).ToList();

                if (branchId != null)
                {
                    toBe = toBepostions.Where(x => x.OrgStructureId == filter.OrgStructureId && x.BranchId.ToString() == branchId).ToList();
                    currentPlacement = currentStaff.Where(x => x.OrgStructureId == filter.OrgStructureId && x.BranchId.ToString() == branchId).ToList();
                }
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

            #region comment
            //foreach (var d in filtered)
            //{
            //    vacantPostions = new VacantPostionByDept();
            //    vacantPostions.vacantDetail = new List<VacantDetail>();

            //    vacantPostions.Department = d.Name;
            //    var toBe = toBepostions.Where(x => x.OrgStructureId == d.OrgStructureId).ToList();
            //    foreach (var t in toBe)
            //    {
            //        VacantDetail = new VacantDetail();
            //        currentStaff = currentStaff.Where(x => x.LookupId == t.LookupId && x.isCurrent).ToList();

            //        VacantDetail.Jobtitle = t.Lookup.Description;
            //        if (toBepostions.Any())
            //        {
            //            VacantDetail.Shouldbe += t.Quntity;
            //        }
            //        if (currentStaff.Any())
            //        {                       
            //            foreach (var c in currentStaff)
            //            {
            //                var emp =
            //                    employees.FirstOrDefault(x => x.EmployeeId == c.EmployeeId);
            //                if (emp != null)
            //                {
            //                    VacantDetail.Current += 1;
            //                }

            //            }
            //        }
            //        VacantDetail.Vacant = VacantDetail.Shouldbe - VacantDetail.Current;
            //        vacantPostions.vacantDetail.Add(VacantDetail);

            //    }
            //    vacantPostionsList.Add(vacantPostions);
            //}

            #endregion

            return vacantPostionsList;
        }
    }
}