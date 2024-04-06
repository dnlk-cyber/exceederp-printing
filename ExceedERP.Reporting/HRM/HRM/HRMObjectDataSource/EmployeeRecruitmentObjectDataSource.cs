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
    public class EmployeeRecruitmentObjectDataSource : List<EmployeeRecruitment>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeRecruitment> GetEmployeeRecruitment(string dept, DateTime from, DateTime to, string branchId, string assignment)
        {
            List<EmployeeRecruitment> EmployeeInfoList = new List<EmployeeRecruitment>();
            EmployeeRecruitment EmployeeInfo = new EmployeeRecruitment();

            DateTime dateTo = new DateTime(to.Year, to.Month, to.Day);
            DateTime dateFrom = new DateTime(from.Year, from.Month, from.Day);

            List<Placement> placements = db.Placements.ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (dept != null)
            {
                int.TryParse(dept, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            }
            if (assignment != null)
            {
                var assign = GetAssignemtType(assignment);
                placements = placements.Where(x => x.AssignmentType == assign).ToList();
            }

            if (from <= to)
            {
                placements = placements.Where(x => x.AssignmentDate >= dateFrom && x.AssignmentDate <= dateTo).ToList();
            }

            placements = placements.Where(x => x.isCurrent).ToList();

            foreach (var l in placements)
            {
                db = new ExceedDbContext();

                EmployeeInfo = new EmployeeRecruitment();

                var e = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == l.EmployeeId);

                if (e != null)
                {
                    EmployeeInfo.EmployeeId = e.EmployeeCode;
                    //EmployeeInfo.Sex = e.Gender;
                    EmployeeInfo.Sex = EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);
                    EmployeeInfo.FullName = e.FullNameAmharic;
                    //EmployeeInfo.HireDate = e.HiredDate.ToShortDateString();
                    EmployeeInfo.HireDate = e.HiredDateAmharic;
                }

                var place = db.Placements.Where(x => x.EmployeeId == l.EmployeeId && !x.isCurrent).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                if (place != null)
                {
                    var fromBranch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                    if (fromBranch != null)
                    {
                        if (l.AssignmentType == AssignmentType.TRANSFER)
                        {
                            EmployeeInfo.FromBranch = fromBranch.AmharicName;
                        }
                        else
                        {
                            EmployeeInfo.ToBranch = "--";
                        }
                    }
                }

                var branch = db.Branch.FirstOrDefault(x => x.BranchId == l.BranchId);
                if (branch != null)
                {
                    EmployeeInfo.Branch = branch.AmharicName;
                    if (l.AssignmentType == AssignmentType.TRANSFER)
                    {
                        EmployeeInfo.ToBranch = branch.AmharicName;
                    }
                    else
                    {
                        EmployeeInfo.ToBranch = "--";
                    }

                }
                var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == l.LookupId);
                if (jobTitle != null)
                {
                    EmployeeInfo.JobTitle = jobTitle.Description;
                }

                EmployeeInfo.Salary = l.Salary;
                var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == l.OrgStructureId);
                if (deptt != null)
                {
                    EmployeeInfo.Departement = deptt.Name;
                }

                EmployeeInfo.AssignmentType = l.AssignmentType.ToString();

                EmployeeInfoList.Add(EmployeeInfo);
            }


            return EmployeeInfoList;
        }

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
    }
}