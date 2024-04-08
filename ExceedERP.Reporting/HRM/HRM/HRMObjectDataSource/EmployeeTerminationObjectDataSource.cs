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
    public class EmployeeTerminationObjectDataSource : List<EmployeeTermination>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeTermination> GetEmployeeTermination(string reason, DateTime? from, DateTime? to, string branchId, string orgStructureId)
        {
            List<EmployeeTermination> EmployeeInfoList = new List<EmployeeTermination>();
            EmployeeTermination EmployeeInfo = new EmployeeTermination();

            var terminations = db.Terminations.Where(x=>x.IsOnlineApproved).ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                var placements = db.Placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).ToList();
                terminations = terminations.Where(x => placements.Contains(x.EmployeeId)).ToList();
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                var placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(db.Placements.ToList(), orgId).Select(x => x.EmployeeId).ToList();
                terminations = terminations.Where(x => placements.Contains(x.EmployeeId)).ToList();
            }
            if (reason != null)
            {
                int.TryParse(reason, out int lookUpId);
                terminations = terminations.Where(x => x.LookupId == lookUpId).ToList();
            }
            if (from != null && to == null)
            {
                terminations = terminations.Where(x => x.TerminationDate >= from.Value).ToList();
            }
            if (from == null && to != null)
            {
                terminations = terminations.Where(x => x.TerminationDate <= to.Value).ToList();
            }
            if (from != null && to != null)
            {
                terminations = terminations.Where(x => x.TerminationDate >= from.Value && x.TerminationDate <= to.Value).ToList();
            }

            foreach (var l in terminations)
            {
                db = new ExceedDbContext();

                var e = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == l.EmployeeId);

                if (e != null)
                {
                    EmployeeInfo = new EmployeeTermination();

                    EmployeeInfo.EmployeeId = e.EmployeeCode;

                    EmployeeInfo.Sex = EmployeeInfo.Sex = HrHelper.GetLocalizedGender(e.Gender);
                    EmployeeInfo.FullName = e.FullNameAmharic;

                    var look = db.Lookups.FirstOrDefault(x => x.LookupId == l.LookupId);
                    if (look != null)
                    {
                        EmployeeInfo.Reason = look.Description;
                    }

                    EmployeeInfo.TerminationDate = l.TerminationDateAm;// l.TerminationDate.ToShortDateString();
                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);

                    if (place != null)
                    {
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                        if (branch != null)
                        {
                            EmployeeInfo.Branch = branch.Name;
                        }

                        var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
                        if (jobTitle != null)
                        {
                            EmployeeInfo.JobTitle = jobTitle.Description;
                        }

                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (deptt != null)
                        {
                            EmployeeInfo.Departement = deptt.Name;
                        }
                    }

                    EmployeeInfoList.Add(EmployeeInfo);

                }
            }

            return EmployeeInfoList;
        }
    }
}