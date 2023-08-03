using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Common
{
    public static class DocApprovalHelper
    {

        public static ApprovalHierarchy HierarchyType(ApprovalDocumentType doctype)
        {
            ExceedDbContext db = new ExceedDbContext();
            var rule = db.DocumentApprovalRules.FirstOrDefault(x => x.DocumentType == doctype);
            if (rule != null)
                return rule.HierarchyType;
            else
                return ApprovalHierarchy.Placement_Hierarchy;
        }
        public static bool CanOwnerApprove(ApprovalDocumentType doctype)
        {
            ExceedDbContext db = new ExceedDbContext();
            var rule=db.DocumentApprovalRules.FirstOrDefault(x => x.DocumentType == doctype);
            if (rule != null)
            {
                if (rule.OwnerCanApprove)
                    return true;
                return false;
            }

            return false;
        }

        public static int GetJobInformation( int empid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

            if (placement != null)
                return placement.LookupId;
            return 0;
        }

        public static int GetEmployeeWithJobTitle(int titleId,int branchid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var placements = db.Placements.Where(x => x.isCurrent && x.LookupId == titleId).ToList();

            if (placements.Any())
            {
                var specificBranch = placements.Where(x => x.BranchId == branchid).FirstOrDefault();
                if (specificBranch != null)
                {
                    return specificBranch.EmployeeId;
                }
                else
                {
                    return placements.FirstOrDefault().EmployeeId;
                }
            }
            return 0;
        }

        //new
        public static int GetEmployeeBranch(int empid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

            if (placement != null)
                return placement.BranchId;
            return 0;
        }

        public static AssignApprovalGroupAssignment GetApprovalAssignmentForJob(int job, ApprovalDocumentType doctype,int branchid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var approvalassignment = db.AssignApprovalGroupAssignments
                .Where(x=>x.DocumentType==doctype)
                .Where(x=>x.ApprovalGroup.OpertatingUnit==branchid)//new
                .Where(x => x.AssignApprovalGroup.OpertatingUnit == branchid)//new
                .FirstOrDefault(x => x.AssignApprovalGroup.Job == job);
            return approvalassignment;
        }

        public static ApprovalGroup GetApprovalGroupForApprovalAssignment(AssignApprovalGroupAssignment approvalassignment)
        {
            ExceedDbContext db = new ExceedDbContext();
            var approvalGroup = db.ApprovalGroups.Find(approvalassignment.ApprovalGroupId);
            return approvalGroup;
        }

        public static decimal GetApprovalGroupAmountLimit(ApprovalGroup approvalgroup)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal limit = 0;
            var amountlimit =
                db.ApprovalGroupRules.FirstOrDefault(x => x.ApprovalGroupId == approvalgroup.ApprovalGroupId);
            if (amountlimit != null)
                limit = amountlimit.AmountLimit;
            return limit;
        }

        public static string GetApprovalGroupAcctLimit(ApprovalGroup approvalgroup)
        {
            ExceedDbContext db = new ExceedDbContext();
            string lower = "";
            string higher = "";
            var accountlimit =db.ApprovalGroupRules.FirstOrDefault(x => x.ApprovalGroupId == approvalgroup.ApprovalGroupId);
            if (accountlimit != null)
            {
                lower = accountlimit.LowValue;
                higher = accountlimit.HighValue;
            }
            return lower;
        }

        public static bool IsApprovalGroupWithinAmountLimit(ApprovalGroup approvalgroup,decimal docAmount)
        {
            decimal ruleAmount = GetApprovalGroupAmountLimit(approvalgroup);
            return ruleAmount>=docAmount;
        }

        public static bool IsApprovalGroupWithinAccountLimit(ApprovalGroup approvalgroup, string acct)
        {
            string range = GetApprovalGroupAcctLimit(approvalgroup);
            //logic to be updated
            return true;
        }

        public static ForwardMethod GetRoutingMethod(ApprovalDocumentType doctype)
        {
            ExceedDbContext db = new ExceedDbContext();
            var docuapprovalrule = db.DocumentApprovalRules.FirstOrDefault(x => x.DocumentType == doctype);
            if (docuapprovalrule != null)
                return docuapprovalrule.ForwardMethod;
            return ForwardMethod.Hierarchy;
        }   

        public static int GetNextSupervisor(int empid, int branchid)
        {
            ExceedDbContext db = new ExceedDbContext();
            int nextempid = 0;
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);
            if (placement != null)
            {
                if (placement.IsHead)
                {
                    var currentWorkunit = db.MainCompanyStructures.Find(placement.OrgStructureId);
                    var parentworkunit =
                        db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == currentWorkunit.Parent);
                    var headPlacement = db.Placements
                        .Where(x => x.BranchId == branchid)//new
                        .FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == parentworkunit.OrgStructureId);
                    if (headPlacement != null)
                        nextempid = headPlacement.EmployeeId;
                }
                else
                {
                    var head = db.Placements
                        .Where(x => x.BranchId == branchid)//new
                        .FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == placement.OrgStructureId);
                    if (head != null)
                    {
                        nextempid = head.EmployeeId;
                    }
                }
            }

            return nextempid;
        }

        //ver new
        public static bool IsEmployeeWithinLimit(int empId, ApprovalDocumentType doctype, decimal docAmount,int branchid )
        {
            ExceedDbContext db = new ExceedDbContext();
            AssignApprovalGroupAssignment assignment = null;
            var specifiEmpAssignment = db.AssignApprovalGroupAssignments
            .Where(x => x.DocumentType == doctype)
            .Where(x => x.ApprovalGroup.OpertatingUnit == branchid)//new
            .Where(x => x.AssignApprovalGroup.OpertatingUnit == branchid)//new
            .FirstOrDefault(x => x.AssignApprovalGroup.EmployeeId == empId);

            if(specifiEmpAssignment == null)
            {
                var placement = db.Placements.Where(x => x.EmployeeId == empId && x.isCurrent).FirstOrDefault();
                if (placement != null)
                {
                    var jobTitleAssignment = db.AssignApprovalGroupAssignments
                   .Where(x => x.DocumentType == doctype)
                   .Where(x => x.ApprovalGroup.OpertatingUnit == branchid)//new
                   .Where(x => x.AssignApprovalGroup.OpertatingUnit == branchid)//new
                   .FirstOrDefault(x => x.AssignApprovalGroup.Job == placement.LookupId);
                    if (jobTitleAssignment != null)
                        assignment = jobTitleAssignment;
                }
            }
            else
            {
                assignment = specifiEmpAssignment;
            }

            if(assignment!=null)
            {
                var approvalGroup = GetApprovalGroupForApprovalAssignment(assignment);
                bool withinAmountLimit = IsApprovalGroupWithinAmountLimit(approvalGroup, docAmount);
                bool withinAccountLimit = IsApprovalGroupWithinAccountLimit(approvalGroup, "1000");

                return (withinAccountLimit && withinAmountLimit);
            }
            else
            {
                //can't even approve
            }

            return true;
        }

    }
}
