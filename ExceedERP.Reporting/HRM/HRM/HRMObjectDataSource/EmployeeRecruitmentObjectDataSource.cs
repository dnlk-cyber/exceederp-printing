using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeRecruitmentObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeRecruitment> GetEmployeeRecruitment(string dept, DateTime? from, DateTime? to, string branchId, string assignment, bool isCurrent)
        {
            var query = db.Placements.AsQueryable();

            if (!string.IsNullOrEmpty(branchId))
            {
                int.TryParse(branchId, out int braId);
                query = query.Where(x => x.BranchId == braId);
            }

            if (!string.IsNullOrEmpty(dept))
            {
                int.TryParse(dept, out int orgId);
                query = query.Where(x => x.OrgStructureId == orgId);
            }

            if (!string.IsNullOrEmpty(assignment))
            {
                query = query.Where(x => x.AssignmentType.ToString().ToLower() == assignment.ToLower());
            }

            if (from.HasValue)
            {
                query = query.Where(x => x.AssignmentDate >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(x => x.AssignmentDate <= to.Value);
            }

            if (isCurrent)
            {
                query = query.Where(x => x.isCurrent);
            }

            var placements = query.ToList();

            // Preload related data (avoid DB calls in loop)
            var employeeIds = placements.Select(x => x.EmployeeId).Distinct().ToList();
            var branchIds = placements.Select(x => x.BranchId).Distinct().ToList();
            var lookupIds = placements.Select(x => x.LookupId).Distinct().ToList();
            var deptIds = placements.Select(x => x.OrgStructureId).Distinct().ToList();
            var unitIds = placements.Select(x => x.BusinessUnitId).Distinct().ToList();

            var employees = db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToDictionary(x => x.EmployeeId);

            var branches = db.Branch
                .Where(x => branchIds.Contains(x.BranchId))
                .ToDictionary(x => x.BranchId);

            var jobTitles = db.Lookups
                .Where(x => lookupIds.Contains(x.LookupId))
                .ToDictionary(x => x.LookupId);

            var departments = db.MainCompanyStructures
                .Where(x => deptIds.Contains(x.OrgStructureId))
                .ToDictionary(x => x.OrgStructureId);
            var businessUnits = db.BusinessUnits
                .Where(x => unitIds.Contains(x.BusinessUnitId))
                .ToDictionary(x => x.BusinessUnitId);

            var result = new List<EmployeeRecruitment>();

            foreach (var l in placements)
            {
                var EmployeeInfo = new EmployeeRecruitment();

                if (employees.TryGetValue(l.EmployeeId, out var e))
                {
                    EmployeeInfo.EmployeeId = e.EmployeeCode;
                    EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);
                    EmployeeInfo.FullName = $"{e.FirstNameEnglish} {e.MiddleNameEnglish} {e.LastNameEnglish}";
                    EmployeeInfo.HireDate = e.HiredDateAmharic;
                }

                if (branches.TryGetValue(l.BranchId, out var branch))
                {
                    EmployeeInfo.Branch = branch.AmharicName;

                    if (l.AssignmentType == AssignmentType.TRANSFER)
                        EmployeeInfo.ToBranch = branch.AmharicName ?? branch.Name;
                    else
                        EmployeeInfo.ToBranch = "--";
                }

                if (jobTitles.TryGetValue(l.LookupId, out var jobTitle))
                {
                    EmployeeInfo.JobTitle = jobTitle.Description ?? jobTitle.DescriptionEng;
                }

                if (departments.TryGetValue(l.OrgStructureId, out var deptt))
                {
                    EmployeeInfo.Departement = deptt.Name;
                }
                if (businessUnits.TryGetValue(l.BusinessUnitId, out var unit))
                {
                    EmployeeInfo.BusinessUnit = unit.Name;
                }

                EmployeeInfo.Salary = l.Salary;
                EmployeeInfo.AssignmentType = l.AssignmentType.ToString();

                result.Add(EmployeeInfo);
            }

            return result;
        }
        //public List<EmployeeRecruitment> GetEmployeeRecruitment(string dept, DateTime? from, DateTime? to, string branchId, string assignment,bool isCurrent)
        //{
        //    List<EmployeeRecruitment> EmployeeInfoList = new List<EmployeeRecruitment>();
        //    EmployeeRecruitment EmployeeInfo = new EmployeeRecruitment();



        //    List<Placement> placements = db.Placements.ToList();

        //    if (!string.IsNullOrEmpty(branchId))
        //    {
        //        int.TryParse(branchId, out int braId);
        //        placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
        //    }
        //    if (!string.IsNullOrEmpty(dept))
        //    {
        //        int.TryParse(dept, out int orgId);
        //        placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
        //    }
        //    if (!string.IsNullOrEmpty(assignment))
        //    {
        //        placements = placements.Where(x => x.AssignmentType.ToString().ToLower() == assignment.ToLower()).ToList();
        //    }

        //    if (from.HasValue)
        //    {
        //        placements = placements.Where(x => x.AssignmentDate.Date.Date >= from.Value.Date).ToList();
        //    }
        //    if (to.HasValue)
        //    {
        //        placements = placements.Where(x =>  x.AssignmentDate.Date <= to.Value.Date).ToList();
        //    }
        //    if(isCurrent)
        //        placements = placements.Where(x => x.isCurrent).ToList();

        //    foreach (var l in placements)
        //    {
        //        db = new ExceedDbContext();

        //        EmployeeInfo = new EmployeeRecruitment();

        //        var e = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == l.EmployeeId);

        //        if (e != null)
        //        {
        //            EmployeeInfo.EmployeeId = e.EmployeeCode;
        //            //EmployeeInfo.Sex = e.Gender;
        //            EmployeeInfo.Sex = EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);
        //            EmployeeInfo.FullName = e.FullNameaEnglish;
        //            //EmployeeInfo.HireDate = e.HiredDate.ToShortDateString();
        //            EmployeeInfo.HireDate = e.HiredDateAmharic;
        //        }


        //        var fromBranch = db.Branch.FirstOrDefault(x => x.BranchId == l.BranchId);
        //        if (fromBranch != null)
        //        {
        //            if (l.AssignmentType == AssignmentType.TRANSFER)
        //            {
        //                EmployeeInfo.FromBranch = fromBranch.AmharicName;
        //            }
        //            else
        //            {
        //                EmployeeInfo.ToBranch = "--";
        //            }
        //        }


        //        var branch = db.Branch.FirstOrDefault(x => x.BranchId == l.BranchId);
        //        if (branch != null)
        //        {
        //            EmployeeInfo.Branch = branch.AmharicName;
        //            if (l.AssignmentType == AssignmentType.TRANSFER)
        //            {
        //                EmployeeInfo.ToBranch = branch.AmharicName??branch.Name;
        //            }
        //            else
        //            {
        //                EmployeeInfo.ToBranch = "--";
        //            }

        //        }
        //        var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == l.LookupId);
        //        if (jobTitle != null)
        //        {
        //            EmployeeInfo.JobTitle = jobTitle.Description??jobTitle.DescriptionEng;
        //        }

        //        EmployeeInfo.Salary = l.Salary;
        //        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == l.OrgStructureId);
        //        if (deptt != null)
        //        {
        //            EmployeeInfo.Departement = deptt.Name;
        //        }

        //        EmployeeInfo.AssignmentType = l.AssignmentType.ToString();

        //        EmployeeInfoList.Add(EmployeeInfo);
        //    }


        //    return EmployeeInfoList;
        //}

        public AssignmentType GetAssignemtType(string assignment)
        {
            AssignmentType assigne = AssignmentType.NEW;
            if (assignment == "NEW")
            {
                assigne = AssignmentType.NEW;
            }
            else if (assignment == "PROMOTION")
            {
                assigne = AssignmentType.PROMOTION;
            }
            else if (assignment == "YEARLY_PROMOTION")
            {
                assigne = AssignmentType.YEARLY_PROMOTION;
            }
            else if (assignment == "TRANSFER")
            {
                assigne = AssignmentType.TRANSFER;
            }
            else if (assignment == "DEMOTION")
            {
                assigne = AssignmentType.DEMOTION;
            }
            else if (assignment == "RECOMENDATION")
            {
                assigne = AssignmentType.RECOMENDATION;
            }
            else if (assignment == "ASSIGNMENT")
            {
                assigne = AssignmentType.ASSIGNMENT;
            }
            else if (assignment == "STRUCTURE_CHANGE")
            {
                assigne = AssignmentType.STRUCTURE_CHANGE;
            }


            return assigne;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}