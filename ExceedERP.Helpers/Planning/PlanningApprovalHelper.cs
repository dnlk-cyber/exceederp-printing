using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Planning
{
    /// <summary>
    /// Single source of truth for the Department -> Branch -> Head Office plan approval hierarchy.
    /// Every controller that reads or writes plan data must authorise through this helper.
    /// </summary>
    public class PlanningApprovalHelper
    {
        private readonly ExceedDbContext db;

        public PlanningApprovalHelper(ExceedDbContext context)
        {
            db = context;
        }

        public Placement GetCurrentPlacement(int employeeId)
        {
            return db.Placements.AsNoTracking().FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId);
        }

        public static PlanApprovalStatus GetEffectiveApprovalStatus(BSCPlan plan)
        {
            if (plan.ApprovalStatus != PlanApprovalStatus.Draft)
                return plan.ApprovalStatus;
            if (plan.IsThirdLevel)
                return PlanApprovalStatus.HeadOfficeApproved;
            if (plan.IsSecondLevel)
                return PlanApprovalStatus.BranchApproved;
            if (plan.IsFirstLevelApproved)
                return PlanApprovalStatus.DepartmentSubmitted;
            return PlanApprovalStatus.Draft;
        }

        public static string GetApprovalStatusText(PlanApprovalStatus status)
        {
            switch (status)
            {
                case PlanApprovalStatus.DepartmentSubmitted:
                    return "Submitted to Higher-Level Head";
                case PlanApprovalStatus.WorkUnitApproved:
                    return "Work Unit Approved";
                case PlanApprovalStatus.BranchApproved:
                    return "Branch Approved";
                case PlanApprovalStatus.HeadOfficeApproved:
                    return "Head Office Approved";
                case PlanApprovalStatus.RevisionRequested:
                    return "Revision Requested";
                default:
                    return "Draft";
            }
        }

        public static string GetApprovalActionText(PlanApprovalAction action)
        {
            switch (action)
            {
                case PlanApprovalAction.Approved:
                    return "Approved";
                case PlanApprovalAction.RevisionRequested:
                    return "Revision Requested";
                case PlanApprovalAction.Resubmitted:
                    return "Resubmitted";
                case PlanApprovalAction.FeedbackAdded:
                    return "Feedback Added";
                case PlanApprovalAction.Aggregated:
                    return "Aggregated";
                default:
                    return "Submitted";
            }
        }

        public static bool IsEditableStatus(PlanApprovalStatus status)
        {
            return status == PlanApprovalStatus.Draft || status == PlanApprovalStatus.RevisionRequested;
        }

        public static bool IsAwaitingApproval(PlanApprovalStatus status)
        {
            return status == PlanApprovalStatus.DepartmentSubmitted || status == PlanApprovalStatus.BranchApproved;
        }

        public static bool IsRecordOwner(BSCPlan record, Placement placement)
        {
            return record.OrgStructureId == placement.OrgStructureId &&
                   (!record.BranchId.HasValue || record.BranchId.Value == placement.BranchId);
        }

        public static void ResetApprovalTracking(BSCPlan record)
        {
            record.IsFirstLevelApproved = false;
            record.IsSecondLevel = false;
            record.IsThirdLevel = false;
            record.SubmittedBy = null;
            record.SubmittedTime = null;
            record.BranchApprovedBy = null;
            record.BranchApprovedTime = null;
            record.HeadOfficeApprovedBy = null;
            record.HeadOfficeApprovedTime = null;
        }

        public bool IsDepartmentPlacement(Placement placement)
        {
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            return orgStructure != null && orgStructure.Parent > 0;
        }

        public bool IsBranchPlacement(Placement placement)
        {
            if (!placement.IsHead)
                return false;

            var branch = db.Branch.Find(placement.BranchId);
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            return branch != null && branch.ParentBranchId.HasValue &&
                   orgStructure != null && orgStructure.Parent <= 0;
        }

        public bool IsHeadOfficePlacement(Placement placement)
        {
            if (!placement.IsHead)
                return false;

            var branch = db.Branch.Find(placement.BranchId);
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            return branch != null && !branch.ParentBranchId.HasValue &&
                   orgStructure != null && orgStructure.Parent <= 0;
        }

        public List<BSCPlan> GetVisiblePlans(int planId, Placement placement)
        {
            return GetVisiblePlans(db.BSCPlans.Where(x => x.PlanId == planId), placement);
        }

        public List<int> GetVisiblePlanIds(Placement placement)
        {
            return GetVisiblePlans(db.BSCPlans, placement).Select(x => x.PlanId).Distinct().ToList();
        }

        private List<BSCPlan> GetVisiblePlans(IEnumerable<BSCPlan> plans, Placement placement)
        {
            var candidates = plans.ToList();

            var childOrgStructureIds = placement.IsHead
                ? db.MainCompanyStructures
                    .Where(x => x.Parent == placement.OrgStructureId && x.OrgStructureId != placement.OrgStructureId)
                    .Select(x => x.OrgStructureId)
                    .ToList()
                : new List<int>();

            if (IsHeadOfficePlacement(placement))
            {
                var childBranchIds = db.Branch
                    .Where(x => x.ParentBranchId == placement.BranchId)
                    .Select(x => x.BranchId)
                    .ToList();

                return candidates.Where(x =>
                {
                    var status = GetEffectiveApprovalStatus(x);
                    if (x.BranchId == placement.BranchId && childOrgStructureIds.Contains(x.OrgStructureId))
                        return status != PlanApprovalStatus.Draft && status != PlanApprovalStatus.RevisionRequested;

                    return x.BranchId.HasValue &&
                           childBranchIds.Contains(x.BranchId.Value) &&
                           x.OrgStructureId == placement.OrgStructureId &&
                           (status == PlanApprovalStatus.BranchApproved ||
                            status == PlanApprovalStatus.HeadOfficeApproved);
                }).ToList();
            }

            if (placement.IsHead)
            {
                return candidates.Where(x =>
                {
                    if (IsRecordOwner(x, placement))
                        return true;

                    if (x.BranchId != placement.BranchId)
                        return false;

                    var status = GetEffectiveApprovalStatus(x);
                    return childOrgStructureIds.Contains(x.OrgStructureId) &&
                           status != PlanApprovalStatus.Draft &&
                           status != PlanApprovalStatus.RevisionRequested;
                }).ToList();
            }

            return candidates
                .Where(x => IsRecordOwner(x, placement))
                .ToList();
        }

        public bool CanViewPlan(BSCPlan record, Placement placement)
        {
            if (placement.IsHead && record.BranchId == placement.BranchId &&
                record.OrgStructureId != placement.OrgStructureId)
            {
                var targetOrg = db.MainCompanyStructures.Find(record.OrgStructureId);
                var status = GetEffectiveApprovalStatus(record);
                if (targetOrg != null && targetOrg.Parent == placement.OrgStructureId &&
                    status != PlanApprovalStatus.Draft && status != PlanApprovalStatus.RevisionRequested)
                    return true;
            }

            if (IsHeadOfficePlacement(placement))
            {
                if (!record.BranchId.HasValue || record.OrgStructureId != placement.OrgStructureId)
                    return false;

                var targetBranch = db.Branch.Find(record.BranchId.Value);
                var status = GetEffectiveApprovalStatus(record);
                return targetBranch != null &&
                       targetBranch.ParentBranchId == placement.BranchId &&
                       (status == PlanApprovalStatus.BranchApproved ||
                        status == PlanApprovalStatus.HeadOfficeApproved);
            }

            if (IsBranchPlacement(placement))
            {
                if (record.BranchId != placement.BranchId)
                    return false;
                if (record.OrgStructureId == placement.OrgStructureId)
                    return true;

                var targetOrg = db.MainCompanyStructures.Find(record.OrgStructureId);
                var status = GetEffectiveApprovalStatus(record);
                return targetOrg != null &&
                       targetOrg.Parent == placement.OrgStructureId &&
                       status != PlanApprovalStatus.Draft &&
                       status != PlanApprovalStatus.RevisionRequested;
            }

            return IsRecordOwner(record, placement);
        }

        /// <summary>
        /// A plan's figures (details, monthly and weekly values) may only be changed by the
        /// owning work unit, and only while the plan is a draft or has been returned for revision.
        /// </summary>
        public bool CanEditFigures(BSCPlan record, Placement placement)
        {
            return record != null && placement != null &&
                   IsRecordOwner(record, placement) &&
                   IsEditableStatus(GetEffectiveApprovalStatus(record));
        }

        public bool TryGetNextApprovalStatus(BSCPlan record, Placement placement,
            out PlanApprovalStatus nextStatus, out string approvalAction)
        {
            nextStatus = record.ApprovalStatus;
            approvalAction = string.Empty;
            if (!placement.IsHead)
                return false;

            var status = GetEffectiveApprovalStatus(record);
            var targetBranchId = record.BranchId ?? (IsRecordOwner(record, placement) ? placement.BranchId : 0);
            var targetBranch = db.Branch.Find(targetBranchId);
            var targetOrg = db.MainCompanyStructures.Find(record.OrgStructureId);
            var approverOrg = db.MainCompanyStructures.Find(placement.OrgStructureId);
            if (targetBranch == null || targetOrg == null || approverOrg == null)
                return false;

            if (IsEditableStatus(status) && IsRecordOwner(record, placement))
            {
                if (targetOrg.Parent <= 0 && IsBranchPlacement(placement))
                {
                    nextStatus = PlanApprovalStatus.BranchApproved;
                    approvalAction = "Submit branch plan to Head Office";
                }
                else if (targetOrg.Parent > 0 && IsDepartmentPlacement(placement))
                {
                    nextStatus = PlanApprovalStatus.DepartmentSubmitted;
                    approvalAction = "Submit plan to higher-level head";
                }
                else
                {
                    return false;
                }
                return true;
            }

            if (status == PlanApprovalStatus.DepartmentSubmitted &&
                targetBranchId == placement.BranchId &&
                record.OrgStructureId != placement.OrgStructureId &&
                targetOrg.Parent == placement.OrgStructureId)
            {
                var isBranchApproval = IsBranchPlacement(placement);
                nextStatus = isBranchApproval
                    ? PlanApprovalStatus.BranchApproved
                    : PlanApprovalStatus.WorkUnitApproved;
                approvalAction = isBranchApproval
                    ? "Approve department plan at Branch"
                    : "Approve lower-level work-unit plan";
                return true;
            }

            if (status == PlanApprovalStatus.BranchApproved &&
                IsHeadOfficePlacement(placement) &&
                targetBranch.ParentBranchId == placement.BranchId &&
                record.OrgStructureId == placement.OrgStructureId &&
                targetOrg.Parent <= 0 && approverOrg.Parent <= 0)
            {
                nextStatus = PlanApprovalStatus.HeadOfficeApproved;
                approvalAction = "Approve plan at Head Office";
                return true;
            }

            return false;
        }

        public bool CanCurrentHeadAggregate(BSCPlan record, Placement placement)
        {
            if (!IsBranchPlacement(placement) || record.IsCopied ||
                GetEffectiveApprovalStatus(record) != PlanApprovalStatus.BranchApproved)
                return false;

            var targetBranchId = record.BranchId ?? 0;
            var targetOrg = db.MainCompanyStructures.Find(record.OrgStructureId);
            if (targetOrg == null)
                return false;

            return targetBranchId == placement.BranchId &&
                   targetOrg.Parent == placement.OrgStructureId;
        }

        public BSCPlan GetPlanOfDetail(int planningDetailId)
        {
            return db.PlanningDetails
                .Where(x => x.PlanningDetailId == planningDetailId)
                .Select(x => x.GetBSCPlan)
                .FirstOrDefault();
        }

        public BSCPlan GetPlanOfMonthlyPlan(int monthlyPlanId)
        {
            return db.MonthlyPlans
                .Where(x => x.MonthlyPlanId == monthlyPlanId)
                .Select(x => x.GetDetail.GetBSCPlan)
                .FirstOrDefault();
        }
    }
}
