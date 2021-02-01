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

        public static string GetJobInformation( int empid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

            if (placement != null)
                return placement.LookupId.ToString();
            return "";
        }

        public static AssignApprovalGroupAssignment GetApprovalAssignmentForJob(string job, ApprovalDocumentType doctype)
        {
            ExceedDbContext db = new ExceedDbContext();
            var approvalassignment = db.AssignApprovalGroupAssignments.Where(x=>x.DocumentType==doctype).FirstOrDefault(x => x.AssignApprovalGroup.Job == job);
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

        public static int GetNextSupervisor(int empid)
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
                    var headPlacement = db.Placements.FirstOrDefault(x =>
                        x.isCurrent && x.IsHead && x.OrgStructureId == parentworkunit.OrgStructureId);
                    if (headPlacement != null)
                        nextempid = headPlacement.EmployeeId;
                }
                else
                {
                    var head = db.Placements.FirstOrDefault(x =>x.isCurrent && x.IsHead && x.OrgStructureId == placement.OrgStructureId);
                    if (head != null)
                    {
                        nextempid = head.EmployeeId;
                    }
                }
            }

            return nextempid;
        }
    }
}
