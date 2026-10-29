﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Planning;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin,
        PlanningRoles.Planning_User,
        HrmConstants.HRDepartmentHead,
        HrmConstants.HREmployeeSelfService)]
    public class BSCPlanSelfCaresController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly PlanningApprovalHelper auth;

        private const string ConcurrencyMessage =
            "The plan was changed by another user while you were working on it. Refresh the page and try again.";

        public BSCPlanSelfCaresController()
        {
            auth = new PlanningApprovalHelper(db);
        }

       private bool GetHasChid(int id,int OthWorkId)
       {

            var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == id).FirstOrDefault();
            if (placement != null)
            {
                var orgstructures = db.MainCompanyStructures
                                   .Where(x => x.Parent == id && x.OrgStructureId != id)
                                   .ToList();
                var Recordorgstructures = (orgstructures.Count() > 0) ? orgstructures.Where(x => x.OrgStructureId == OthWorkId).FirstOrDefault() : null;
                if (Recordorgstructures !=null)
                {
                    return true;

                }
                return false;
            }
            return false;
        }
        public ActionResult BSCPlanSelfCares_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            var placement = GetCurrentPlacement();
            if (placement == null && !PlanningSelfCareHelper.CanViewAllWorkUnits(User))
                return Json(Enumerable.Empty<BSCPlan>().ToDataSourceResult(request));

            var bscplans = GetVisiblePlans(id, placement);

            var branchIds = bscplans.Where(x => x.BranchId.HasValue).Select(x => x.BranchId.Value).Distinct().ToList();
            var branches = db.Branch.Where(x => branchIds.Contains(x.BranchId)).ToDictionary(x => x.BranchId, x => x.Name);

            foreach (var bscplan in bscplans)
            {
                bscplan.ApprovalStatus = GetEffectiveApprovalStatus(bscplan);
                var approvalAction = string.Empty;
                PlanApprovalStatus nextStatus;
                bscplan.CanApprove = placement != null &&
                                     TryGetNextApprovalStatus(bscplan, placement, out nextStatus, out approvalAction);
                bscplan.ApprovalAction = approvalAction;
                bscplan.CanCopy = placement != null && CanCurrentHeadAggregate(bscplan, placement);
                bscplan.CanEdit = placement != null && IsRecordOwner(bscplan, placement) && IsEditableStatus(bscplan.ApprovalStatus);
                bscplan.CanDelete = placement != null && IsRecordOwner(bscplan, placement) && bscplan.ApprovalStatus == PlanApprovalStatus.Draft;
                bscplan.CanRequestRevision = bscplan.CanApprove && IsAwaitingApproval(bscplan.ApprovalStatus);
                bscplan.CanAddFeedback = bscplan.CanRequestRevision;
                bscplan.HasChild = placement != null && placement.IsHead && db.MainCompanyStructures.Any(x =>
                    x.Parent == placement.OrgStructureId && x.OrgStructureId == bscplan.OrgStructureId);
                bscplan.ApprovalStatusText = GetApprovalStatusText(bscplan.ApprovalStatus);
                bscplan.BranchName = bscplan.BranchId.HasValue && branches.ContainsKey(bscplan.BranchId.Value)
                    ? branches[bscplan.BranchId.Value]
                    : string.Empty;
            }

            DataSourceResult result = bscplans.ToDataSourceResult(request, bSCPlan => new {
                BSCPlanId = bSCPlan.BSCPlanId,
                ObjectiveId = bSCPlan.ObjectiveId,
                OrgStructureId = bSCPlan.OrgStructureId,
                BranchId = bSCPlan.BranchId,
                BranchName = bSCPlan.BranchName,
                ApprovalStatus = bSCPlan.ApprovalStatus,
                ApprovalStatusText = bSCPlan.ApprovalStatusText,
                ApprovalAction = bSCPlan.ApprovalAction,
                CanApprove = bSCPlan.CanApprove,
                CanCopy = bSCPlan.CanCopy,
                CanEdit = bSCPlan.CanEdit,
                CanDelete = bSCPlan.CanDelete,
                CanRequestRevision = bSCPlan.CanRequestRevision,
                CanAddFeedback = bSCPlan.CanAddFeedback,
                IsFirstLevelApproved = bSCPlan.IsFirstLevelApproved,
                IsSecondLevel = bSCPlan.IsSecondLevel,
                IsThirdLevel = bSCPlan.IsThirdLevel,
                IsCopied = bSCPlan.IsCopied,
                HasChild = bSCPlan.HasChild,
                SubmittedBy = bSCPlan.SubmittedBy,
                SubmittedTime = bSCPlan.SubmittedTime,
                BranchApprovedBy = bSCPlan.BranchApprovedBy,
                BranchApprovedTime = bSCPlan.BranchApprovedTime,
                HeadOfficeApprovedBy = bSCPlan.HeadOfficeApprovedBy,
                HeadOfficeApprovedTime = bSCPlan.HeadOfficeApprovedTime,
                PlanId = bSCPlan.PlanId
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult BSCPlanSelfCares_Create([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan,int id)
        {
            var placement = GetCurrentPlacement();
            if (placement == null)
                ModelState.AddModelError(string.Empty, "Your user is not linked to a current employee placement.");
            else if (!IsDepartmentPlacement(placement))
                ModelState.AddModelError(string.Empty, "Only department users can create a departmental plan. Branch plans are created by aggregating approved department plans.");

            if (placement != null)
            {
                bSCPlan.OrgStructureId = placement.OrgStructureId;
                bSCPlan.BranchId = placement.BranchId;
            }

            var exists = placement != null && db.BSCPlans.Any(s => s.ObjectiveId == bSCPlan.ObjectiveId &&
                s.PlanId == id && s.OrgStructureId == placement.OrgStructureId && s.BranchId == placement.BranchId);
            if (exists)
            {
                ModelState.AddModelError("exists", "This objective is already registered for your branch and work unit.");
            }
            if (ModelState.IsValid)
            {
                bSCPlan.PlanId = id;
                bSCPlan.ApprovalStatus = PlanApprovalStatus.Draft;
                var entity = GetMappedObject(bSCPlan);
                db.BSCPlans.Add(entity);
                db.SaveChanges();
                bSCPlan.BSCPlanId = entity.BSCPlanId;
                PopulatethePlanningDetails(bSCPlan.ObjectiveId, entity.BSCPlanId,id);
                // PopulateThePeriod(entity.BSCPlanId, id);
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Bsc Actuals", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                #endregion

            }
            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult BSCPlanSelfCares_Update([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan)
        {
            var placement = GetCurrentPlacement();
            var entity = db.BSCPlans.Find(bSCPlan.BSCPlanId);

            if (placement == null || entity == null || !IsRecordOwner(entity, placement))
                ModelState.AddModelError(string.Empty, "You can update only plans owned by your current branch and work unit.");
            else if (!IsEditableStatus(GetEffectiveApprovalStatus(entity)))
                ModelState.AddModelError(string.Empty, "Only a draft or a plan returned for revision can be edited.");
            else if (db.BSCPlans.Any(x => x.BSCPlanId != entity.BSCPlanId && x.PlanId == entity.PlanId &&
                                         x.ObjectiveId == bSCPlan.ObjectiveId && x.OrgStructureId == entity.OrgStructureId &&
                                         x.BranchId == entity.BranchId))
                ModelState.AddModelError("ObjectiveId", "This objective is already registered for your branch and work unit.");

            if (ModelState.IsValid)
            {
                entity.ObjectiveId = bSCPlan.ObjectiveId;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "BSCPlan", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult BSCPlanSelfCares_Destroy([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan)
        {
            var placement = GetCurrentPlacement();
            var persistedPlan = db.BSCPlans.Find(bSCPlan.BSCPlanId);
            if (placement == null || persistedPlan == null || !IsRecordOwner(persistedPlan, placement))
                ModelState.AddModelError(string.Empty, "You can delete only plans owned by your current branch and work unit.");
            else if (GetEffectiveApprovalStatus(persistedPlan) != PlanApprovalStatus.Draft)
                ModelState.AddModelError(string.Empty, "A submitted plan cannot be deleted.");

            var planDetails = db.PlanningDetails.Where(x => x.BSCPlanId == bSCPlan.BSCPlanId).ToList();
            if (planDetails.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                #region delete log
                var objective = db.Objectives.Find(bSCPlan.ObjectiveId);
                var delete = objective.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion

                db.BSCPlans.Remove(persistedPlan);
                db.SaveChanges();
            }

            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }
        private BSCPlan GetMappedObject(BSCPlan bSCPlan)
        {
            // Approval state is never accepted from the request; a new plan always starts as a draft.
            var entity = new BSCPlan
            {
                ObjectiveId = bSCPlan.ObjectiveId,
                OrgStructureId = bSCPlan.OrgStructureId,
                BranchId = bSCPlan.BranchId,
                IsFirstLevelApproved = false,
                IsSecondLevel = false,
                IsThirdLevel = false,
                IsCopied = false,
                ApprovalStatus = PlanApprovalStatus.Draft,
                SubmittedBy = null,
                SubmittedTime = null,
                BranchApprovedBy = null,
                BranchApprovedTime = null,
                HeadOfficeApprovedBy = null,
                HeadOfficeApprovedTime = null,
                PlanId = bSCPlan.PlanId
            };
            return entity;
        }
        private int PopulatethePlanningDetails(int objectiveId, int bscId,int planId)
        {
            var indicators = db.Indicators.Where(x=>x.IsActive).Where(x=>x.ObjectiveId==objectiveId).ToList();
            foreach(var indcator in indicators)
            {
               
                var entity = new PlanningDetail
                {
                    IndicatorId = indcator.IndicatorId,
                    BSCPlanId = bscId,
                    KPIType = indcator.KPIType,
                    ParentId = indcator.ParentId,
                    GlobalIndicatorId = indcator.GlobalIndicatorId,
                    HasSubIndicator = indcator.HasSubIndicator,
                    Unit = indcator.Unit,
                    Baseline = 0,
                    Target = 0,
                    Plan = 0,


                };
                db.PlanningDetails.Add(entity);
                db.SaveChanges();
                var fiscalYear = db.Plans.Find(planId);
                if (fiscalYear != null)
                {

                    var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).ToList();
                    foreach (var period in periods)
                    {
                        
                        var entity2 = new MonthlyPlan
                        {
                            GLPeriodId = period.GLPeriodId,
                            PlanningDetailId = entity.PlanningDetailId,
                            Value = 0,

                        };
                        db.MonthlyPlans.Add(entity2);
                        db.SaveChanges();

                    }

                }
               
                    
                
            }
            return 0;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User, HrmConstants.HRDepartmentHead, HrmConstants.HREmployeeSelfService)]
        public JsonResult ApproveBScPlan(int id, string feedback)
        {
            feedback = (feedback ?? string.Empty).Trim();
            if (feedback.Length > 2000)
                return Json(new { Message = "Feedback cannot exceed 2000 characters.", Status = "NOT_OK" });

            var record = db.BSCPlans.Find(id);
            var placement = GetCurrentPlacement();
            if (record == null || placement == null)
                return Json(new { Message = "The plan or your current placement was not found.", Status = "NOT_OK" });

            record.ApprovalStatus = GetEffectiveApprovalStatus(record);
            if (!record.BranchId.HasValue && IsRecordOwner(record, placement))
                record.BranchId = placement.BranchId;

            PlanApprovalStatus nextStatus;
            string approvalAction;
            if (!TryGetNextApprovalStatus(record, placement, out nextStatus, out approvalAction))
                return Json(new { Message = "You are not the authorized approver for the plan's current hierarchy level.", Status = "NOT_OK" });

            var previousStatus = record.ApprovalStatus;
            var now = DateTime.Now;
            if ((nextStatus == PlanApprovalStatus.DepartmentSubmitted ||
                 nextStatus == PlanApprovalStatus.BranchApproved ||
                 nextStatus == PlanApprovalStatus.HeadOfficeApproved) && !record.SubmittedTime.HasValue)
            {
                record.SubmittedBy = User.Identity.Name;
                record.SubmittedTime = now;
                record.IsFirstLevelApproved = true;
            }
            if ((nextStatus == PlanApprovalStatus.BranchApproved ||
                 nextStatus == PlanApprovalStatus.HeadOfficeApproved) && !record.BranchApprovedTime.HasValue)
            {
                record.BranchApprovedBy = User.Identity.Name;
                record.BranchApprovedTime = now;
                record.IsSecondLevel = true;
            }
            if (nextStatus == PlanApprovalStatus.HeadOfficeApproved)
            {
                record.HeadOfficeApprovedBy = User.Identity.Name;
                record.HeadOfficeApprovedTime = now;
                record.IsThirdLevel = true;
            }

            record.ApprovalStatus = nextStatus;
            db.PlanApprovalFeedbacks.Add(new PlanApprovalFeedback
            {
                BSCPlanId = record.BSCPlanId,
                Action = previousStatus == PlanApprovalStatus.RevisionRequested
                    ? PlanApprovalAction.Resubmitted
                    : previousStatus == PlanApprovalStatus.Draft
                        ? PlanApprovalAction.Submitted
                        : PlanApprovalAction.Approved,
                FromStatus = previousStatus,
                ToStatus = nextStatus,
                Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback,
                ActionBy = User.Identity.Name,
                ActionTime = now
            });
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { Message = ConcurrencyMessage, Status = "NOT_OK" });
            }

            UserHelper.OperationLog(Request.UserHostAddress, "BSCPlan Approval", "Success", User.Identity.Name,
                approvalAction + " by " + User.Identity.Name, Modules.Planning);

            return Json(new
            {
                Message = approvalAction + " completed successfully.",
                Status = "OK",
                ApprovalStatus = GetApprovalStatusText(nextStatus)
            });
        }

        public ActionResult PlanApprovalFeedbacks_Read([DataSourceRequest] DataSourceRequest request, int bscPlanId)
        {
            var record = db.BSCPlans.Find(bscPlanId);
            var placement = GetCurrentPlacement();
            if (record == null || placement == null || !CanViewPlan(record, placement))
                return Json(Enumerable.Empty<PlanApprovalFeedback>().ToDataSourceResult(request));

            var feedbacks = db.PlanApprovalFeedbacks.AsNoTracking()
                .Where(x => x.BSCPlanId == bscPlanId)
                .OrderByDescending(x => x.ActionTime)
                .ToList()
                .Select(x => new
                {
                    x.PlanApprovalFeedbackId,
                    x.BSCPlanId,
                    ActionText = GetApprovalActionText(x.Action),
                    FromStatusText = GetApprovalStatusText(x.FromStatus),
                    ToStatusText = GetApprovalStatusText(x.ToStatus),
                    x.Feedback,
                    x.ActionBy,
                    x.ActionTime
                });

            return Json(feedbacks.ToDataSourceResult(request));
        }

        public ActionResult PlanFeedbacks_Read([DataSourceRequest] DataSourceRequest request, int planId)
        {
            var placement = GetCurrentPlacement();
            if (placement == null)
                return Json(Enumerable.Empty<PlanApprovalFeedback>().ToDataSourceResult(request));

            var visiblePlans = GetVisiblePlans(planId, placement);
            var bscPlanIds = visiblePlans.Select(x => x.BSCPlanId).ToList();
            if (!bscPlanIds.Any())
                return Json(Enumerable.Empty<PlanApprovalFeedback>().ToDataSourceResult(request));

            var orgStructureIds = visiblePlans.Select(x => x.OrgStructureId).Distinct().ToList();
            var workUnits = db.MainCompanyStructures.AsNoTracking()
                .Where(x => orgStructureIds.Contains(x.OrgStructureId))
                .ToDictionary(x => x.OrgStructureId, x => x.Name);

            var branchIds = visiblePlans.Where(x => x.BranchId.HasValue).Select(x => x.BranchId.Value).Distinct().ToList();
            var branches = db.Branch.AsNoTracking()
                .Where(x => branchIds.Contains(x.BranchId))
                .ToDictionary(x => x.BranchId, x => x.Name);

            var objectiveIds = visiblePlans.Select(x => x.ObjectiveId).Distinct().ToList();
            var objectives = db.Objectives.AsNoTracking()
                .Where(x => objectiveIds.Contains(x.ObjectiveId))
                .ToDictionary(x => x.ObjectiveId, x => x.Name);

            var departmentPlans = visiblePlans
                .GroupBy(x => x.BSCPlanId)
                .ToDictionary(g => g.Key, g => g.First());

            var planFeedbacks = db.PlanApprovalFeedbacks.AsNoTracking()
                .Where(x => bscPlanIds.Contains(x.BSCPlanId))
                .OrderByDescending(x => x.ActionTime)
                .ToList()
                .Select(x =>
                {
                    var departmentPlan = departmentPlans[x.BSCPlanId];
                    return new
                    {
                        x.PlanApprovalFeedbackId,
                        x.BSCPlanId,
                        WorkUnitName = workUnits.ContainsKey(departmentPlan.OrgStructureId)
                            ? workUnits[departmentPlan.OrgStructureId]
                            : string.Empty,
                        BranchName = departmentPlan.BranchId.HasValue && branches.ContainsKey(departmentPlan.BranchId.Value)
                            ? branches[departmentPlan.BranchId.Value]
                            : string.Empty,
                        ObjectiveName = objectives.ContainsKey(departmentPlan.ObjectiveId)
                            ? objectives[departmentPlan.ObjectiveId]
                            : string.Empty,
                        ActionText = GetApprovalActionText(x.Action),
                        FromStatusText = GetApprovalStatusText(x.FromStatus),
                        ToStatusText = GetApprovalStatusText(x.ToStatus),
                        x.Feedback,
                        x.ActionBy,
                        x.ActionTime
                    };
                });

            return Json(planFeedbacks.ToDataSourceResult(request));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User, HrmConstants.HRDepartmentHead, HrmConstants.HREmployeeSelfService)]
        public JsonResult RequestPlanRevision(int id, string feedback)
        {
            feedback = (feedback ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(feedback))
                return Json(new { Message = "Feedback is required when requesting a revision.", Status = "NOT_OK" });
            if (feedback.Length > 2000)
                return Json(new { Message = "Feedback cannot exceed 2000 characters.", Status = "NOT_OK" });

            var record = db.BSCPlans.Find(id);
            var placement = GetCurrentPlacement();
            if (record == null || placement == null)
                return Json(new { Message = "The plan or your current placement was not found.", Status = "NOT_OK" });

            record.ApprovalStatus = GetEffectiveApprovalStatus(record);
            if (!IsAwaitingApproval(record.ApprovalStatus))
                return Json(new { Message = "Only a submitted plan awaiting your approval can be returned for revision.", Status = "NOT_OK" });

            PlanApprovalStatus ignoredStatus;
            string ignoredAction;
            if (!TryGetNextApprovalStatus(record, placement, out ignoredStatus, out ignoredAction))
                return Json(new { Message = "You are not the authorized approver for the plan's current hierarchy level.", Status = "NOT_OK" });

            var previousStatus = record.ApprovalStatus;
            var now = DateTime.Now;
            db.PlanApprovalFeedbacks.Add(new PlanApprovalFeedback
            {
                BSCPlanId = record.BSCPlanId,
                Action = PlanApprovalAction.RevisionRequested,
                FromStatus = previousStatus,
                ToStatus = PlanApprovalStatus.RevisionRequested,
                Feedback = feedback,
                ActionBy = User.Identity.Name,
                ActionTime = now
            });

            ResetApprovalTracking(record);
            record.ApprovalStatus = PlanApprovalStatus.RevisionRequested;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { Message = ConcurrencyMessage, Status = "NOT_OK" });
            }

            UserHelper.OperationLog(Request.UserHostAddress, "BSCPlan Revision", "Success", User.Identity.Name,
                "Revision requested by " + User.Identity.Name, Modules.Planning);

            return Json(new
            {
                Message = "The plan was returned for revision.",
                Status = "OK",
                ApprovalStatus = GetApprovalStatusText(record.ApprovalStatus)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User, HrmConstants.HRDepartmentHead, HrmConstants.HREmployeeSelfService)]
        public JsonResult AddPlanFeedback(int id, string feedback)
        {
            feedback = (feedback ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(feedback))
                return Json(new { Message = "Feedback is required.", Status = "NOT_OK" });
            if (feedback.Length > 2000)
                return Json(new { Message = "Feedback cannot exceed 2000 characters.", Status = "NOT_OK" });

            var record = db.BSCPlans.Find(id);
            var placement = GetCurrentPlacement();
            if (record == null || placement == null)
                return Json(new { Message = "The plan or your current placement was not found.", Status = "NOT_OK" });

            record.ApprovalStatus = GetEffectiveApprovalStatus(record);
            if (!IsAwaitingApproval(record.ApprovalStatus))
                return Json(new { Message = "Feedback can be added only while the plan is awaiting your approval.", Status = "NOT_OK" });

            PlanApprovalStatus ignoredStatus;
            string ignoredAction;
            if (!TryGetNextApprovalStatus(record, placement, out ignoredStatus, out ignoredAction))
                return Json(new { Message = "You are not the authorized approver for the plan's current hierarchy level.", Status = "NOT_OK" });

            db.PlanApprovalFeedbacks.Add(new PlanApprovalFeedback
            {
                BSCPlanId = record.BSCPlanId,
                Action = PlanApprovalAction.FeedbackAdded,
                FromStatus = record.ApprovalStatus,
                ToStatus = record.ApprovalStatus,
                Feedback = feedback,
                ActionBy = User.Identity.Name,
                ActionTime = DateTime.Now
            });
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { Message = ConcurrencyMessage, Status = "NOT_OK" });
            }

            UserHelper.OperationLog(Request.UserHostAddress, "BSCPlan Feedback", "Success", User.Identity.Name,
                "Feedback added by " + User.Identity.Name, Modules.Planning);

            return Json(new { Message = "Feedback added successfully.", Status = "OK" });
        }

        private Placement GetCurrentPlacement()
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            return auth.GetCurrentPlacement(employeeId);
        }

        private static PlanApprovalStatus GetEffectiveApprovalStatus(BSCPlan plan)
        {
            return PlanningApprovalHelper.GetEffectiveApprovalStatus(plan);
        }

        private static string GetApprovalStatusText(PlanApprovalStatus status)
        {
            return PlanningApprovalHelper.GetApprovalStatusText(status);
        }

        private static string GetApprovalActionText(PlanApprovalAction action)
        {
            return PlanningApprovalHelper.GetApprovalActionText(action);
        }

        private static bool IsEditableStatus(PlanApprovalStatus status)
        {
            return PlanningApprovalHelper.IsEditableStatus(status);
        }

        private static bool IsAwaitingApproval(PlanApprovalStatus status)
        {
            return PlanningApprovalHelper.IsAwaitingApproval(status);
        }

        private static bool IsRecordOwner(BSCPlan record, Placement placement)
        {
            return PlanningApprovalHelper.IsRecordOwner(record, placement);
        }

        private List<BSCPlan> GetVisiblePlans(int planId, Placement placement)
        {
            if (PlanningSelfCareHelper.CanViewAllWorkUnits(User))
                return db.BSCPlans.Where(x => x.PlanId == planId).ToList();

            return auth.GetVisiblePlans(planId, placement);
        }

        private bool IsDepartmentPlacement(Placement placement)
        {
            return auth.IsDepartmentPlacement(placement);
        }

        private bool IsBranchPlacement(Placement placement)
        {
            return auth.IsBranchPlacement(placement);
        }

        private bool IsHeadOfficePlacement(Placement placement)
        {
            return auth.IsHeadOfficePlacement(placement);
        }

        private bool TryGetNextApprovalStatus(BSCPlan record, Placement placement,
            out PlanApprovalStatus nextStatus, out string approvalAction)
        {
            return auth.TryGetNextApprovalStatus(record, placement, out nextStatus, out approvalAction);
        }

        private bool CanCurrentHeadAggregate(BSCPlan record, Placement placement)
        {
            return auth.CanCurrentHeadAggregate(record, placement);
        }

        private bool CanViewPlan(BSCPlan record, Placement placement)
        {
            return auth.CanViewPlan(record, placement);
        }

        private static void ResetApprovalTracking(BSCPlan record)
        {
            PlanningApprovalHelper.ResetApprovalTracking(record);
        }
        [NonAction]
        public JsonResult CopyBScPlan1(int bscPlanId)
        {
            var recordToBeCopied = db.BSCPlans.Where(x=>!x.IsCopied&&x.IsFirstLevelApproved).Where(x=>x.BSCPlanId==bscPlanId).FirstOrDefault();
            if (recordToBeCopied != null)
            {
                
               
                var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var placement = db.Placements.Where(x => x.isCurrent &&x.EmployeeId==employeeId && x.IsHead ).FirstOrDefault();
                if (placement != null)
                {
                    var orgstructures = db.MainCompanyStructures
                                    .Where(x => x.Parent == placement.OrgStructureId).ToList();
                    var Recordorgstructures =(orgstructures.Any())? orgstructures.Where(x => x.OrgStructureId == recordToBeCopied.OrgStructureId).FirstOrDefault():null;
                    if (Recordorgstructures !=null)
                    {
                        var bscplan = db.BSCPlans.Where(x => x.OrgStructureId == placement.OrgStructureId).Where(x=>x.PlanId== recordToBeCopied.PlanId).FirstOrDefault();
                        if (bscplan != null)
                        {
                            var planningDetails = db.PlanningDetails.Where(x => x.BSCPlanId == recordToBeCopied.BSCPlanId).ToList();
                            foreach (var planningDetail in planningDetails)
                            {

                                var PlanningDetailUpper = db.PlanningDetails.Where(x =>  x.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                                        &&x.HasSubIndicator==planningDetail.HasSubIndicator
                                                                                          && x.KPIType == planningDetail.KPIType
                                                                                          && x.BSCPlanId==bscplan.BSCPlanId).FirstOrDefault();

                                if(PlanningDetailUpper != null)
                                {
                                    //var lowwerdetail = db.PlanningDetails.Where(x => x.IndicatorId == planningDetail.IndicatorId && x.BSCPlanId == bscplan.BSCPlanId);
                                    PlanningDetailUpper.Plan += planningDetail.Plan;         //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Plan): planningDetail.Plan;
                                    PlanningDetailUpper.Target += planningDetail.Target;     //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Target): planningDetail.Target;
                                    PlanningDetailUpper.Baseline += planningDetail.Target;   //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Baseline): planningDetail.Baseline;
                                    PlanningDetailUpper.KPIType = planningDetail.KPIType;
                                    PlanningDetailUpper.ParentId = planningDetail.ParentId;
                                    PlanningDetailUpper.GlobalIndicatorId = planningDetail.GlobalIndicatorId;
                                    PlanningDetailUpper.IndicatorId = planningDetail.IndicatorId;
                                    PlanningDetailUpper.HasSubIndicator = planningDetail.HasSubIndicator;
                                    PlanningDetailUpper.BSCPlanId = bscplan.BSCPlanId;
                                    db.Entry(PlanningDetailUpper).State = EntityState.Modified;
                                    //db.SaveChanges();
                                    var existdMothlyPlanToCopieds = db.MonthlyPlans.Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId
                                                                                                  &&x.GetDetail.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                                                 && x.GetDetail.HasSubIndicator == planningDetail.HasSubIndicator
                                                                                                && x.GetDetail.KPIType == planningDetail.KPIType
                                                                                            ).ToList();
                                    
                                    foreach (var monthlyplanToCopied in existdMothlyPlanToCopieds)
                                    {
                                        var monthlyplan = db.MonthlyPlans.FirstOrDefault(x => x.GLPeriodId == monthlyplanToCopied.GLPeriodId
                                                                                         &&x.PlanningDetailId== PlanningDetailUpper.PlanningDetailId
                                                                                         && x.GetDetail.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                                         && x.GetDetail.HasSubIndicator == planningDetail.HasSubIndicator
                                                                                          && x.GetDetail.KPIType == planningDetail.KPIType);
                                        if(monthlyplan != null)
                                        {

                                           // var existdMothlyPlanUppersCopy = existdMothlyPlanUppers.Where(x => x.GLPeriodId == periodId);
                                            //monthlyplan.Value = existdMothlyPlanUppersCopy.Count() > 0 ? existdMothlyPlanUppersCopy.Sum(x => x.Value) : 0;
                                            monthlyplan.Value += monthlyplanToCopied.Value;
                                            db.Entry(monthlyplan).State = EntityState.Modified;
                                           // db.SaveChanges();
                                            //var monthlyPlanIds = existdMothlyPlanUppersCopy.Select(x => x.MonthlyPlanId);
                                            var weeklyPlan = db.WeeklyPlans.FirstOrDefault(x => x.MonthlyPlanId == monthlyplan.MonthlyPlanId);
                                            if(weeklyPlan != null)
                                            {

                                                var weeklyPlans = db.WeeklyPlans.Where(x =>x.MonthlyPlanId== monthlyplanToCopied.MonthlyPlanId).ToList();
                                                weeklyPlan.WeekOne += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                                                weeklyPlan.WeekTwo += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                                                weeklyPlan.WeekThree += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                                                weeklyPlan.WeekFour += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekFour) : weeklyPlan.WeekFour;
                                                db.Entry(weeklyPlan).State = EntityState.Modified;
                                               // db.SaveChanges();

                                            }
                                            db.SaveChanges();

                                        }
                                        else
                                        {
                                            var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == bscplan.GetPlan.GlFiscalYearId).ToList();
                                            foreach (var period in periods)
                                            {

                                                var monthlyPlans = db.MonthlyPlans.Where(x => x.GLPeriodId == period.GLPeriodId)
                                                                                .Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId)
                                                                                .ToList();
                                                var entity2 = new MonthlyPlan
                                                {
                                                    GLPeriodId = period.GLPeriodId,
                                                    ActualValue = 0,
                                                    PlanningDetailId = monthlyplanToCopied.PlanningDetailId,
                                                    Value = monthlyPlans.Count() > 0 ? monthlyPlans.Sum(x => x.Value) : 0,

                                                };
                                                db.MonthlyPlans.Add(entity2);
                                                db.SaveChanges();

                                                var monthlyPlanIds = monthlyPlans.Select(x => x.MonthlyPlanId).ToList();
                                                // var weeklyPlan = db.WeeklyPlans.FirstOrDefault(x => monthlyPlanIds.Contains(x.MonthlyPlanId));
                                                WeeklyPlan weeklyPlan = new WeeklyPlan();
                                                var weeklyPlans = db.WeeklyPlans.Where(x => monthlyPlanIds.Contains(x.MonthlyPlanId)).ToList();
                                                weeklyPlan.WeekOne = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                                                weeklyPlan.WeekTwo = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                                                weeklyPlan.WeekThree = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                                                weeklyPlan.WeekFour = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekFour) : weeklyPlan.WeekFour;
                                                weeklyPlan.ActualWeekOne = 0;
                                                weeklyPlan.ActualWeekTwo = 0;
                                                weeklyPlan.ActualWeekThree = 0;
                                                weeklyPlan.ActualWeekFour = 0;
                                                weeklyPlan.MonthlyPlanId = entity2.MonthlyPlanId;
                                                db.WeeklyPlans.Add(weeklyPlan);
                                                // db.Entry(weeklyPlan).State = EntityState.Modified;
                                                db.SaveChanges();


                                            }
                                        }
                                    }
                                }
                                else
                                {

                                    

                                    var entity = new PlanningDetail
                                    {
                                        IndicatorId = planningDetail.IndicatorId,
                                        GlobalIndicatorId = planningDetail.GlobalIndicatorId,
                                        BSCPlanId = bscplan.BSCPlanId,
                                        KPIType = planningDetail.KPIType,
                                        ParentId = planningDetail.ParentId,
                                        HasSubIndicator = planningDetail.HasSubIndicator,
                                        Unit = planningDetail.Unit,
                                        Baseline = planningDetail.Baseline,
                                        Target = planningDetail.Target,
                                        Plan = planningDetail.Plan,


                                    };
                                    db.PlanningDetails.Add(entity);
                                    db.SaveChanges();
                                    var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == bscplan.GetPlan.GlFiscalYearId).ToList();
                                    foreach (var period in periods)
                                    {

                                        var monthlyPlans = db.MonthlyPlans.Where(x => x.GLPeriodId == period.GLPeriodId)
                                                                        .Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId).ToList();
                                        if (monthlyPlans.Any())
                                        {
                                            var entity2 = new MonthlyPlan
                                            {
                                                GLPeriodId = period.GLPeriodId,
                                                PlanningDetailId = entity.PlanningDetailId,
                                                Value = monthlyPlans.Count() > 0 ? monthlyPlans.Sum(x => x.Value) : 0,
                                                ActualValue = 0
                                            };
                                            db.MonthlyPlans.Add(entity2);
                                            db.SaveChanges();
                                            var monthlyPlanIds = monthlyPlans.Select(x => x.MonthlyPlanId);
                                            WeeklyPlan weeklyPlan = new WeeklyPlan();
                                            var weeklyPlans = db.WeeklyPlans.Where(x => monthlyPlanIds.Contains(x.MonthlyPlanId)).ToList();
                                            weeklyPlan.WeekOne = weeklyPlans.Any()  ? weeklyPlans.Sum(x => x.WeekOne) : 0;
                                            weeklyPlan.WeekTwo = weeklyPlans.Any() ? weeklyPlans.Sum(x => x.WeekTwo) : 0;
                                            weeklyPlan.WeekThree = weeklyPlans.Any()? weeklyPlans.Sum(x => x.WeekThree) : 0;
                                            weeklyPlan.WeekFour = weeklyPlans.Any()? weeklyPlans.Sum(x => x.WeekFour) : 0;
                                            weeklyPlan.ActualWeekOne = 0;
                                            weeklyPlan.ActualWeekTwo = 0;
                                            weeklyPlan.ActualWeekThree = 0;
                                            weeklyPlan.ActualWeekFour = 0;
                                            weeklyPlan.MonthlyPlanId = entity2.MonthlyPlanId;
                                            db.WeeklyPlans.Add(weeklyPlan);
                                            db.SaveChanges();
                                        }
                                       
                                        

                                    }





                                    
                                }
                            }
                            recordToBeCopied.IsCopied = true;
                            db.Entry(recordToBeCopied).State = EntityState.Modified;
                            db.SaveChanges();
                            var formattedData14 = new
                            {
                                Message = "copied SuccessFully",
                                Status = "okay"
                            };
                            return Json(formattedData14, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var entity11 = new BSCPlan
                            {
                                OrgStructureId = placement.OrgStructureId,
                                ObjectiveId = recordToBeCopied.ObjectiveId,
                                PlanId = recordToBeCopied.PlanId,
                                IsCopied = true,
                                IsFirstLevelApproved = true,
                                IsSecondLevel = true,
                                IsThirdLevel = false,


                            };
                            db.BSCPlans.Add(entity11);
                            db.SaveChanges();
                            var indicators = db.PlanningDetails.Where(x => x.BSCPlanId == recordToBeCopied.BSCPlanId).ToList();

                            foreach (var indcator in indicators)
                            {

                                var entity = new PlanningDetail
                                {
                                    IndicatorId = indcator.IndicatorId,
                                    BSCPlanId = entity11.BSCPlanId,
                                    KPIType = indcator.KPIType,
                                    ParentId = indcator.ParentId,
                                    GlobalIndicatorId = indcator.GlobalIndicatorId,
                                    HasSubIndicator = indcator.HasSubIndicator,
                                    Unit = indcator.Unit,
                                    Baseline = indcator.Baseline,
                                    Target = indcator.Target,
                                    Plan = indcator.Plan,
                                    
                                };
                                db.PlanningDetails.Add(entity);
                                db.SaveChanges();
                                var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == recordToBeCopied.GetPlan.GlFiscalYearId).ToList();
                                foreach (var period in periods)
                                {

                                    var monthlyPlans = db.MonthlyPlans.Where(x => x.GLPeriodId == period.GLPeriodId)
                                                                    .Where(x => x.PlanningDetailId == indcator.PlanningDetailId)
                                                                    .ToList();
                                    var entity2 = new MonthlyPlan
                                    {
                                        GLPeriodId = period.GLPeriodId,
                                        PlanningDetailId = entity.PlanningDetailId,
                                        Value = monthlyPlans.Count() > 0 ? monthlyPlans.Sum(x => x.Value) : 0,
                                        ActualValue = 0,

                                    };
                                    db.MonthlyPlans.Add(entity2);
                                    db.SaveChanges();

                                    var monthlyPlanIds = monthlyPlans.Select(x => x.MonthlyPlanId);
                                    var weeklyPlan = db.WeeklyPlans.FirstOrDefault(x => monthlyPlanIds.Contains( x.MonthlyPlanId ));
                                    if (weeklyPlan != null)
                                    {
                                        var weeklyPlans = db.WeeklyPlans.Where(x => monthlyPlanIds.Contains(x.MonthlyPlanId)).ToList();
                                        weeklyPlan.WeekOne = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                                        weeklyPlan.WeekTwo = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                                        weeklyPlan.WeekThree = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                                        weeklyPlan.WeekFour = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekFour) : weeklyPlan.WeekThree;
                                        db.Entry(weeklyPlan).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }





                            }
                            recordToBeCopied.IsSecondLevel = true;
                            recordToBeCopied.IsCopied = true;
                            db.Entry(recordToBeCopied).State = EntityState.Modified;
                            db.SaveChanges();
                            var formattedData14 = new
                            {
                                Message = "copied SuccessFully",
                                Status = "okay"
                            };
                            return Json(formattedData14, JsonRequestBehavior.AllowGet);
                        }
                        
                    }
                    else
                    {
                        var formattedData13 = new
                        {
                            Message = "you can not copy b/c This is your own dept Or no  Work Unit  is  under you",
                            Status = "okay"
                        };
                        return Json(formattedData13, JsonRequestBehavior.AllowGet);

                    }

                }
                else
                {
                    var formattedData12 = new
                    {
                        Message = "your are not Department Head.",
                        Status = "okay"
                    };
                    return Json(formattedData12, JsonRequestBehavior.AllowGet);

                }
               
            }

            var formattedData = new
            {
                Message = "Record is  Copied Once or it is not Approved.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User, HrmConstants.HRDepartmentHead)]
        public JsonResult CopyBScPlan(int bscPlanId)
        {
            var recordToBeCopied = db.BSCPlans.FirstOrDefault(x => x.BSCPlanId == bscPlanId && !x.IsCopied);
            var placement = GetCurrentPlacement();

            if (recordToBeCopied == null || placement == null)
            {
                return Json(new { Message = "The plan is already copied, missing, or your placement was not found.", Status = "NOT_OK" });
            }

            recordToBeCopied.ApprovalStatus = GetEffectiveApprovalStatus(recordToBeCopied);
            if (!CanCurrentHeadAggregate(recordToBeCopied, placement))
            {
                return Json(new { Message = "You are not authorized to aggregate this plan at its current hierarchy level.", Status = "NOT_OK" });
            }

            var bscplan = db.BSCPlans.FirstOrDefault(x => x.PlanId == recordToBeCopied.PlanId &&
                x.ObjectiveId == recordToBeCopied.ObjectiveId && x.OrgStructureId == placement.OrgStructureId &&
                x.BranchId == placement.BranchId);

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (bscplan == null)
                    {
                        bscplan = new BSCPlan
                        {
                            OrgStructureId = placement.OrgStructureId,
                            BranchId = placement.BranchId,
                            ObjectiveId = recordToBeCopied.ObjectiveId,
                            PlanId = recordToBeCopied.PlanId,
                            ApprovalStatus = PlanApprovalStatus.Draft
                        };
                        db.BSCPlans.Add(bscplan);
                        db.SaveChanges();
                    }

                    CopyPlanningDetailsAndMonthlyPlans(recordToBeCopied.BSCPlanId, bscplan.BSCPlanId);
                    recordToBeCopied.IsCopied = true;

                    db.PlanApprovalFeedbacks.Add(new PlanApprovalFeedback
                    {
                        BSCPlanId = recordToBeCopied.BSCPlanId,
                        Action = PlanApprovalAction.Aggregated,
                        FromStatus = recordToBeCopied.ApprovalStatus,
                        ToStatus = recordToBeCopied.ApprovalStatus,
                        Feedback = "Aggregated into plan #" + bscplan.BSCPlanId + ".",
                        ActionBy = User.Identity.Name,
                        ActionTime = DateTime.Now
                    });
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    transaction.Rollback();
                    return Json(new { Message = ConcurrencyMessage, Status = "NOT_OK" });
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return Json(new { Message = "The aggregation failed and no changes were saved.", Status = "NOT_OK" });
                }
            }

            UserHelper.OperationLog(Request.UserHostAddress, "BSCPlan Aggregation", "Success", User.Identity.Name,
                "Aggregated by " + User.Identity.Name, Modules.Planning);

            return Json(new { Message = "Plan aggregated successfully into your branch/work-unit plan.", Status = "OK" });
        }

        private void CopyPlanningDetailsAndMonthlyPlans(int sourceBSCPlanId, int targetBSCPlanId)
        {
            var planningDetails = db.PlanningDetails.Where(x => x.BSCPlanId == sourceBSCPlanId).ToList();

            foreach (var planningDetail in planningDetails)
            {
                // Copy PlanningDetail
                var existInDesitination = db.PlanningDetails.Where(x => x.BSCPlanId == targetBSCPlanId
                                                    && x.IndicatorId == planningDetail.IndicatorId
                                                    && x.ParentId == planningDetail.ParentId
                                                    && x.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                    && x.HasSubIndicator == planningDetail.HasSubIndicator
                                                    &&x.KPIType ==planningDetail.KPIType).FirstOrDefault();
                if (existInDesitination == null)
                {
                    var newPlanningDetail = new PlanningDetail
                    {
                        IndicatorId = planningDetail.IndicatorId,
                        GlobalIndicatorId = planningDetail.GlobalIndicatorId,
                        BSCPlanId = targetBSCPlanId,
                        KPIType = planningDetail.KPIType,
                        ParentId = planningDetail.ParentId,
                        HasSubIndicator = planningDetail.HasSubIndicator,
                        Unit = planningDetail.Unit,
                        Baseline = planningDetail.Baseline,
                        Target = planningDetail.Target,
                        Plan = planningDetail.Plan
                    };
                    db.PlanningDetails.Add(newPlanningDetail);
                    db.SaveChanges();

                    // Copy MonthlyPlans
                    var monthlyPlans = db.MonthlyPlans.Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId).ToList();
                    foreach (var monthlyPlan in monthlyPlans)
                    {
                        var newMonthlyPlan = new MonthlyPlan
                        {
                            GLPeriodId = monthlyPlan.GLPeriodId,
                            PlanningDetailId = newPlanningDetail.PlanningDetailId,
                            Value = monthlyPlan.Value,
                            ActualValue = monthlyPlan.ActualValue
                        };
                        db.MonthlyPlans.Add(newMonthlyPlan);
                        db.SaveChanges();

                        // Copy WeeklyPlans
                        var weeklyPlans = db.WeeklyPlans.Where(x => x.MonthlyPlanId == monthlyPlan.MonthlyPlanId).ToList();
                        foreach (var weeklyPlan in weeklyPlans)
                        {
                            var newWeeklyPlan = new WeeklyPlan
                            {
                                MonthlyPlanId = newMonthlyPlan.MonthlyPlanId,
                                WeekOne = weeklyPlan.WeekOne,
                                WeekTwo = weeklyPlan.WeekTwo,
                                WeekThree = weeklyPlan.WeekThree,
                                WeekFour = weeklyPlan.WeekFour,
                                ActualWeekOne = weeklyPlan.ActualWeekOne,
                                ActualWeekTwo = weeklyPlan.ActualWeekTwo,
                                ActualWeekThree = weeklyPlan.ActualWeekThree,
                                ActualWeekFour = weeklyPlan.ActualWeekFour
                            };
                            db.WeeklyPlans.Add(newWeeklyPlan);
                        }
                    }

                }
                else
                {
                    existInDesitination.Plan += planningDetail.Plan;         //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Plan): planningDetail.Plan;
                    existInDesitination.Target += planningDetail.Target;     //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Target): planningDetail.Target;
                    existInDesitination.Baseline += planningDetail.Baseline;
                    existInDesitination.KPIType = planningDetail.KPIType;
                    existInDesitination.ParentId = planningDetail.ParentId;
                    existInDesitination.GlobalIndicatorId = planningDetail.GlobalIndicatorId;
                    existInDesitination.IndicatorId = planningDetail.IndicatorId;
                    existInDesitination.HasSubIndicator = planningDetail.HasSubIndicator;
                    existInDesitination.BSCPlanId = targetBSCPlanId;
                    db.Entry(existInDesitination).State = EntityState.Modified;
                    var existdMothlyPlanToCopieds = db.MonthlyPlans.Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId
                                                                                  && x.GetDetail.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                                 && x.GetDetail.HasSubIndicator == planningDetail.HasSubIndicator
                                                                                && x.GetDetail.KPIType == planningDetail.KPIType
                                                                            ).ToList();

                    foreach (var monthlyplanToCopied in existdMothlyPlanToCopieds)
                    {
                        var monthlyplan = db.MonthlyPlans.FirstOrDefault(x => x.GLPeriodId == monthlyplanToCopied.GLPeriodId
                                                                         && x.PlanningDetailId == existInDesitination.PlanningDetailId
                                                                         && x.GetDetail.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                         && x.GetDetail.HasSubIndicator == planningDetail.HasSubIndicator
                                                                          && x.GetDetail.KPIType == planningDetail.KPIType);

                        if (monthlyplan != null)
                        {

                            
                            monthlyplan.Value += monthlyplanToCopied.Value;
                            db.Entry(monthlyplan).State = EntityState.Modified;
                            var weeklyPlan = db.WeeklyPlans.FirstOrDefault(x => x.MonthlyPlanId == monthlyplan.MonthlyPlanId);
                            if (weeklyPlan != null)
                            {

                                var weeklyPlans = db.WeeklyPlans.Where(x => x.MonthlyPlanId == monthlyplanToCopied.MonthlyPlanId).ToList();
                                weeklyPlan.WeekOne += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                                weeklyPlan.WeekTwo += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                                weeklyPlan.WeekThree += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                                weeklyPlan.WeekFour += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekFour) : weeklyPlan.WeekFour;
                                db.Entry(weeklyPlan).State = EntityState.Modified;
                                // db.SaveChanges();

                            }
                            db.SaveChanges();

                        }
                        else
                        {
                                                        
                            var entity2 = new MonthlyPlan
                            {
                                GLPeriodId = monthlyplanToCopied.GLPeriodId,
                                PlanningDetailId = existInDesitination.PlanningDetailId,
                                Value = monthlyplanToCopied.Value,
                                ActualValue = monthlyplanToCopied.ActualValue,

                            };
                            db.MonthlyPlans.Add(entity2);
                            db.SaveChanges();

                            var weeklyPlanToBecopied = db.WeeklyPlans.Where(x =>x.MonthlyPlanId== monthlyplanToCopied.MonthlyPlanId).ToList();
                            WeeklyPlan weeklyPlan = new WeeklyPlan();
                            weeklyPlan.WeekOne = weeklyPlanToBecopied.Count() > 0 ? weeklyPlanToBecopied.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                            weeklyPlan.WeekTwo = weeklyPlanToBecopied.Count() > 0 ? weeklyPlanToBecopied.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                            weeklyPlan.WeekThree = weeklyPlanToBecopied.Count() > 0 ? weeklyPlanToBecopied.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                            weeklyPlan.WeekFour = weeklyPlanToBecopied.Count() > 0 ? weeklyPlanToBecopied.Sum(x => x.WeekFour) : weeklyPlan.WeekFour;
                            weeklyPlan.ActualWeekOne = 0;
                            weeklyPlan.ActualWeekTwo = 0;
                            weeklyPlan.ActualWeekThree = 0;
                            weeklyPlan.ActualWeekFour = 0;
                            weeklyPlan.MonthlyPlanId = entity2.MonthlyPlanId;
                            db.WeeklyPlans.Add(weeklyPlan);
                            db.SaveChanges();


                          
                        }
                    }
                }
               
            }
            db.SaveChanges();
        }
    //    private void CopyPlanningDetailsAndMonthlyPlans1(int sourceBSCPlanId, int targetBSCPlanId)
    //    {
    //        var planningDetails = db.PlanningDetails
    //            .Include(x => x.MonthlyPlans)
    //            .Where(x => x.BSCPlanId == sourceBSCPlanId)
    //            .ToList();

    //        foreach (var planningDetail in planningDetails)
    //        {
    //            var existingDestination = db.PlanningDetails.FirstOrDefault(x =>
    //                x.BSCPlanId == targetBSCPlanId &&
    //                x.GlobalIndicatorId == planningDetail.GlobalIndicatorId &&
    //                x.HasSubIndicator == planningDetail.HasSubIndicator &&
    //                x.KPIType == planningDetail.KPIType);

    //            if (existingDestination == null)
    //            {
    //                Copy PlanningDetail
    //                var newPlanningDetail = new PlanningDetail
    //                {
    //                     Populate properties (e.g., IndicatorId, GlobalIndicatorId, etc.)
    //                     Example:
    //                     IndicatorId = planningDetail.IndicatorId,
    //                     GlobalIndicatorId = planningDetail.GlobalIndicatorId,
    //                     BSCPlanId = targetBSCPlanId,
    //                     ...
    //                };
    //            db.PlanningDetails.Add(newPlanningDetail);

    //            Copy MonthlyPlans
    //                foreach (var monthlyPlan in planningDetail.MonthlyPlans)
    //            {
    //                var newMonthlyPlan = new MonthlyPlan
    //                    {
    //                         Populate properties (e.g., GLPeriodId, Value, ActualValue, etc.)
    //                         Example:
    //                         GLPeriodId = monthlyPlan.GLPeriodId,
    //                         Value = monthlyPlan.Value,
    //                         ActualValue = monthlyPlan.ActualValue,
    //                         ...
    //                    };
    //            newPlanningDetail.MonthlyPlans.Add(newMonthlyPlan);

    //            Copy WeeklyPlans
    //                    foreach (var weeklyPlan in monthlyPlan.WeeklyPlans)
    //            {
    //                var newWeeklyPlan = new WeeklyPlan
    //                        {
    //                             Populate properties (e.g., WeekOne, WeekTwo, etc.)
    //                             Example:
    //                             WeekOne = weeklyPlan.WeekOne,
    //                             WeekTwo = weeklyPlan.WeekTwo,
    //                             ...
    //                        };
    //            newMonthlyPlan.WeeklyPlans.Add(newWeeklyPlan);
    //        }
    //    }
    //}
    //            else
    //            {
    //                 Update existing PlanningDetail
    //                existingDestination.Plan += planningDetail.Plan;
    //existingDestination.Target += planningDetail.Target;
    //                existingDestination.Baseline += planningDetail.Target;

    //                 Update MonthlyPlans
    //                foreach (var monthlyPlan in planningDetail.MonthlyPlans)
    //                {
    //                    var existingMonthlyPlan = existingDestination.MonthlyPlans
    //                        .FirstOrDefault(x => x.GLPeriodId == monthlyPlan.GLPeriodId);
    //                    if (existingMonthlyPlan != null)
    //                    {
    //                        existingMonthlyPlan.Value += monthlyPlan.Value;

    //                         Update WeeklyPlans
    //                        foreach (var weeklyPlan in monthlyPlan.WeeklyPlans)
    //                        {
    //                            var existingWeeklyPlan = existingMonthlyPlan.WeeklyPlans
    //                                .FirstOrDefault(x => /* Check condition */);
    //                            if (existingWeeklyPlan != null)
    //                            {
    //                                 Update existing weekly plan
    //                            }
    //                            else
    //                            {
    //                                 Create new weekly plan
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                         Create new monthly plan and weekly plans
    //                    }
    //                }
    //            }
    //        }

    //         Save changes to the database
    //        db.SaveChanges();
    //    }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
