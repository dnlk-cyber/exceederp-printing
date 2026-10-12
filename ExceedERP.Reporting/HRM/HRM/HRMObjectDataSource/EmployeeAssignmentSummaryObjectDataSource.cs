using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeAssignmentSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AssignmentSummaryVM> GetEmployeeRecruitmentSummary(string dept, DateTime? from, DateTime? to,
                                                                        string branchId,string businessUnit, string assignment, bool isCurrent)
        {
            var query = db.Placements.AsQueryable();

            // Branch Filter
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                query = query.Where(x => x.BranchId == braId);
            }

            // Department Filter
            if (!string.IsNullOrEmpty(dept) && int.TryParse(dept, out int orgId))
            {
                query = query.Where(x => x.OrgStructureId == orgId);
            }
               
            // Business Unit Filter
            if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int businessUnitId))
            {
                query = query.Where(x => x.BusinessUnitId == businessUnitId);
            }

            //  AssignmentType Filter (SAFE)
            if (!string.IsNullOrEmpty(assignment) &&
                Enum.TryParse<AssignmentType>(assignment, true, out var assignmentEnum))
            {
                query = query.Where(x => x.AssignmentType == assignmentEnum);
            }

            //  Date Filters
            if (from.HasValue)
            {
                query = query.Where(x => x.AssignmentDate >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(x => x.AssignmentDate <= to.Value);
            }

            // Current Filter
            if (isCurrent)
            {
                query = query.Where(x => x.isCurrent);
            }

            //  JOIN with Employee table ONLY (no need for others)
            var result = query
                .Join(db.Person_Employee,
                      p => p.EmployeeId,
                      e => e.EmployeeId,
                      (p, e) => new
                      {
                          p.AssignmentType,
                          Gender = e.Gender
                      })
                .AsEnumerable() // switch to memory for grouping safely
                .GroupBy(x => x.AssignmentType)
                .Select(g => new AssignmentSummaryVM
                {
                    AssignmentType = g.Key.ToString(),

                    Male = g.Count(x => x.Gender == Gender.MALE),    
                    Female = g.Count(x => x.Gender == Gender.FEMALE)   

                }).OrderBy(x => x.AssignmentType).ToList();

            return result;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}