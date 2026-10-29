using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Planning;
using System.Collections.Generic;
using System;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User, 
        HrmConstants.HRDepartmentHead, HrmConstants.HREmployeeSelfService)]
    public class BSCPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly PlanningApprovalHelper auth;

        public BSCPlansController()
        {
            auth = new PlanningApprovalHelper(db);
        }

       
        public ActionResult BSCPlans_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            var placement = GetCurrentPlacement();
            var bscplans = db.BSCPlans.Where(b => b.PlanId == id).ToList();
            foreach (var bscplan in bscplans)
            {
                bscplan.ApprovalStatus = GetEffectiveApprovalStatus(bscplan);
                PlanApprovalStatus nextStatus;
                string approvalAction;
                bscplan.CanApprove = placement != null &&
                    auth.TryGetNextApprovalStatus(bscplan, placement, out nextStatus, out approvalAction);
                bscplan.ApprovalStatusText = GetApprovalStatusText(bscplan.ApprovalStatus);
            }
            DataSourceResult result = bscplans.ToDataSourceResult(request, bSCPlan => new {
                BSCPlanId = bSCPlan.BSCPlanId,
                ObjectiveId = bSCPlan.ObjectiveId,
                OrgStructureId = bSCPlan.OrgStructureId,
                BranchId = bSCPlan.BranchId,
                ApprovalStatus = bSCPlan.ApprovalStatus,
                ApprovalStatusText = bSCPlan.ApprovalStatusText,
                CanApprove = bSCPlan.CanApprove,
                SubmittedBy = bSCPlan.SubmittedBy,
                SubmittedTime = bSCPlan.SubmittedTime,
                BranchApprovedBy = bSCPlan.BranchApprovedBy,
                BranchApprovedTime = bSCPlan.BranchApprovedTime,
                HeadOfficeApprovedBy = bSCPlan.HeadOfficeApprovedBy,
                HeadOfficeApprovedTime = bSCPlan.HeadOfficeApprovedTime,
                IsFirstLevelApproved = bSCPlan.IsFirstLevelApproved,
                IsSecondLevel = bSCPlan.IsSecondLevel,
                IsThirdLevel = bSCPlan.IsThirdLevel,
                IsCopied = bSCPlan.IsCopied,
                PlanId = bSCPlan.PlanId
            });

            return Json(result);
        }

        public ActionResult PlanApprovalFeedbacks_Read(
            [DataSourceRequest] DataSourceRequest request,
            int bscPlanId)
        {
            if (!db.BSCPlans.Any(x => x.BSCPlanId == bscPlanId))
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
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
            PlanApprovalStatus nextStatus;
            string approvalAction;
            if (!auth.TryGetNextApprovalStatus(record, placement, out nextStatus, out approvalAction))
                return Json(new { Message = "You are not the authorized approver for the plan's current hierarchy level.", Status = "NOT_OK" });

            var previousStatus = record.ApprovalStatus;
            var now = DateTime.Now;
            if (!record.SubmittedTime.HasValue)
            {
                record.SubmittedBy = User.Identity.Name;
                record.SubmittedTime = now;
                record.IsFirstLevelApproved = true;
            }
            if (nextStatus == PlanApprovalStatus.BranchApproved || nextStatus == PlanApprovalStatus.HeadOfficeApproved)
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
                return Json(new { Message = "The plan was changed by another user while you were working on it. Refresh the page and try again.", Status = "NOT_OK" });
            }

            return Json(new
            {
                Message = approvalAction + " completed successfully.",
                Status = "OK",
                ApprovalStatus = GetApprovalStatusText(nextStatus)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
        public JsonResult AddPlanFeedback(int id, string feedback)
        {
            feedback = (feedback ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(feedback))
                return Json(new { Message = "Feedback is required.", Status = "NOT_OK" });
            if (feedback.Length > 2000)
                return Json(new { Message = "Feedback cannot exceed 2000 characters.", Status = "NOT_OK" });

            var record = db.BSCPlans.Find(id);
            if (record == null)
                return Json(new { Message = "The selected plan was not found.", Status = "NOT_OK" });

            var status = GetEffectiveApprovalStatus(record);
            if (!PlanningApprovalHelper.IsAwaitingApproval(status))
                return Json(new { Message = "Feedback can be added only while the plan is awaiting approval.", Status = "NOT_OK" });

            var placement = GetCurrentPlacement();
            PlanApprovalStatus nextStatus;
            string approvalAction;
            if (placement == null || !auth.TryGetNextApprovalStatus(record, placement, out nextStatus, out approvalAction))
                return Json(new { Message = "You are not the authorized approver for the plan's current hierarchy level.", Status = "NOT_OK" });

            PlanningApprovalHelper.ResetApprovalTracking(record);
            record.ApprovalStatus = PlanApprovalStatus.RevisionRequested;
            db.PlanApprovalFeedbacks.Add(new PlanApprovalFeedback
            {
                BSCPlanId = record.BSCPlanId,
                Action = PlanApprovalAction.RevisionRequested,
                FromStatus = status,
                ToStatus = PlanApprovalStatus.RevisionRequested,
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
                return Json(new { Message = "The plan was changed by another user while you were working on it. Refresh the page and try again.", Status = "NOT_OK" });
            }

            UserHelper.OperationLog(
                Request.UserHostAddress,
                "BSCPlan Feedback",
                "Success",
                User.Identity.Name,
                "Feedback added by " + User.Identity.Name,
                Modules.Planning);

            return Json(new { Message = "Feedback sent and the plan was returned for revision.", Status = "OK", ApprovalStatus = GetApprovalStatusText(record.ApprovalStatus) });
        }

        private Placement GetCurrentPlacement()
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            return auth.GetCurrentPlacement(employeeId);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCPlans_Create([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan,int id)
        {
            var exists = db.BSCPlans.Any(s => s.ObjectiveId == bSCPlan.ObjectiveId&& s.OrgStructureId ==bSCPlan.OrgStructureId && s.PlanId==id);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered");
            }
            if (ModelState.IsValid)
            {
                bSCPlan.PlanId = id;
                bSCPlan.ApprovalStatus = PlanApprovalStatus.Draft;
                bSCPlan.IsFirstLevelApproved = false;
                bSCPlan.IsSecondLevel = false;
                bSCPlan.IsThirdLevel = false;
                bSCPlan.SubmittedBy = null;
                bSCPlan.SubmittedTime = null;
                bSCPlan.BranchApprovedBy = null;
                bSCPlan.BranchApprovedTime = null;
                bSCPlan.HeadOfficeApprovedBy = null;
                bSCPlan.HeadOfficeApprovedTime = null;
                var entity = GetMappedObject(bSCPlan);
                db.BSCPlans.Add(entity);
                db.SaveChanges();
                bSCPlan.BSCPlanId = entity.BSCPlanId;
                PopulatethePlanningDetails(bSCPlan.OrgStructureId, bSCPlan.ObjectiveId, entity.BSCPlanId,id);
                // PopulateThePeriod(entity.BSCPlanId, id);
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Bsc Actuals", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                #endregion


            }
            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCPlans_Update([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan)
        {
            var entity = db.BSCPlans.Find(bSCPlan.BSCPlanId);
            if (entity == null)
                ModelState.AddModelError(string.Empty, "The plan was not found.");
            else if (entity.IsApprovalLocked)
                ModelState.AddModelError(string.Empty, "Only a draft or a plan returned for revision can be edited.");

            var exists = entity != null && db.BSCPlans.Any(s => s.BSCPlanId != entity.BSCPlanId &&
                s.ObjectiveId == bSCPlan.ObjectiveId && s.OrgStructureId == bSCPlan.OrgStructureId &&
                s.PlanId == entity.PlanId && s.BranchId == entity.BranchId);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered");
            }
            if (ModelState.IsValid)
            {
                entity.ObjectiveId = bSCPlan.ObjectiveId;
                entity.OrgStructureId = bSCPlan.OrgStructureId;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "BSCPlan", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCPlans_Destroy([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan)
        {
            var entity = db.BSCPlans.Find(bSCPlan.BSCPlanId);
            if (entity == null)
                ModelState.AddModelError(string.Empty, "The plan was not found.");
            else if (GetEffectiveApprovalStatus(entity) != PlanApprovalStatus.Draft)
                ModelState.AddModelError(string.Empty, "A submitted plan cannot be deleted.");

            var planDetails = db.PlanningDetails.Where(x => x.BSCPlanId == bSCPlan.BSCPlanId).ToList();
            if (planDetails.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                #region delete log
                var objective = db.Objectives.Find(entity.ObjectiveId);
                var delete = objective.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion
                db.BSCPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }
        private BSCPlan GetMappedObject(BSCPlan bSCPlan)
        {
            var entity = new BSCPlan
            {
                BSCPlanId = bSCPlan.BSCPlanId,
                ObjectiveId = bSCPlan.ObjectiveId,
                OrgStructureId = bSCPlan.OrgStructureId,
                BranchId = bSCPlan.BranchId,
                IsFirstLevelApproved = bSCPlan.IsFirstLevelApproved,
                IsSecondLevel = bSCPlan.IsSecondLevel,
                IsThirdLevel = bSCPlan.IsThirdLevel,
                IsCopied = bSCPlan.IsCopied,
                ApprovalStatus = bSCPlan.ApprovalStatus,
                SubmittedBy = bSCPlan.SubmittedBy,
                SubmittedTime = bSCPlan.SubmittedTime,
                BranchApprovedBy = bSCPlan.BranchApprovedBy,
                BranchApprovedTime = bSCPlan.BranchApprovedTime,
                HeadOfficeApprovedBy = bSCPlan.HeadOfficeApprovedBy,
                HeadOfficeApprovedTime = bSCPlan.HeadOfficeApprovedTime,
                PlanId = bSCPlan.PlanId
            };
            return entity;
        }

        private static PlanApprovalStatus GetEffectiveApprovalStatus(BSCPlan plan)
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

        private static string GetApprovalStatusText(PlanApprovalStatus status)
        {
            switch (status)
            {
                case PlanApprovalStatus.DepartmentSubmitted:
                    return "Submitted to Branch";
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

        private static string GetApprovalActionText(PlanApprovalAction action)
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
                default:
                    return "Submitted";
            }
        }
        private int PopulatethePlanningDetails(int orgStructureId, int objectiveId, int bscId, int planId)
        {
            // Get all indicators in one query
            var indicators = db.Indicators
                                    .Where(x => x.IsActive 
                                               && x.ObjectiveId == objectiveId).ToList();

            // Get fiscal year and periods in advance
            var fiscalYear = db.Plans
                .Where(p => p.PlanId == planId)
                .Select(p => new { p.GlFiscalYearId })
                .FirstOrDefault();

            List<GLPeriod> periods = null;
            if (fiscalYear != null)
            {
                periods = db.GLPeriods
                    .Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId)
                    .ToList();
            }

            // Use transaction for bulk operations
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var indicator in indicators)
                    {
                        // Create PlanningDetail
                        var planningDetail = new PlanningDetail
                        {
                            IndicatorId = indicator.IndicatorId,
                            BSCPlanId = bscId,
                            KPIType = indicator.KPIType,
                            ParentId = indicator.ParentId,
                            GlobalIndicatorId = indicator.GlobalIndicatorId,
                            HasSubIndicator = indicator.HasSubIndicator,
                            Unit = indicator.Unit,
                            Baseline = 0,
                            Target = 0,
                            Plan = 0
                        };

                        db.PlanningDetails.Add(planningDetail);
                        db.SaveChanges(); // Need to save to get ID for monthly plans

                        // Create MonthlyPlans if periods exist
                        if (periods != null && periods.Any())
                        {
                            var monthlyPlans = periods.Select(period => new MonthlyPlan
                            {
                                GLPeriodId = period.GLPeriodId,
                                PlanningDetailId = planningDetail.PlanningDetailId,
                                Value = 0,
                                ActualValue = 0
                            }).ToList();

                            db.MonthlyPlans.AddRange(monthlyPlans);
                            db.SaveChanges();
                        }
                    }

                    transaction.Commit();
                    return 1; // Return success code instead of 0
                }
                catch
                {
                    transaction.Rollback();
                    return -1; // Return error code
                }
            }
        }
        //private int PopulatethePlanningDetails(int orgStructureId,int objectiveId, int bscId,int planId)
        //{
        //    var indicators = db.Indicators.Where(x=>x.IsActive).Where(x => x.ObjectiveId == objectiveId).ToList();
        //    foreach(var indcator in indicators)
        //    {
               
        //        var entity = new PlanningDetail
        //        {
        //            IndicatorId = indcator.IndicatorId,
        //            BSCPlanId = bscId,
        //            KPIType = indcator.KPIType,
        //            ParentId = indcator.ParentId,
        //            GlobalIndicatorId = indcator.GlobalIndicatorId,
        //            HasSubIndicator = indcator.HasSubIndicator,
        //            Unit = indcator.Unit,
        //            Baseline = 0,
        //            Target = 0,
        //            Plan = 0,


        //        };
        //        db.PlanningDetails.Add(entity);
        //        db.SaveChanges();
               
        //        var fiscalYear = db.Plans.Find(planId);
        //        if (fiscalYear != null)
        //        {

        //            var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).ToList();
        //            foreach (var period in periods)
        //            {


        //                var entity2 = new MonthlyPlan
        //                {
        //                    GLPeriodId = period.GLPeriodId,
        //                    PlanningDetailId = entity.PlanningDetailId,
        //                    Value = 0,
        //                    ActualValue = 0,

        //                };
        //                db.MonthlyPlans.Add(entity2);
        //                db.SaveChanges();

        //            }

        //        }
               
                    
                
        //    }
        //    return 0;
        //}
        public JsonResult RefreshToAddNewAddedItems(int id)
        {
            int count = 0;
            var record = db.BSCPlans.Find(id);
            if (record != null)
            {
                if (record.IsApprovalLocked)
                {
                    return Json(new
                    {
                        Message = "Only a draft or a plan returned for revision can be refreshed.",
                        Status = "NOT_OK"
                    }, JsonRequestBehavior.AllowGet);
                }

                #region validate item Addition

                var indicators = db.Indicators.Where(x=>x.IsActive).Where(x => x.ObjectiveId == record.ObjectiveId).ToList();
                foreach (var indcator in indicators)
                {
                    var exist = db.PlanningDetails.Where(x => x.IndicatorId == indcator.IndicatorId && x.BSCPlanId == id).FirstOrDefault();
                    if (exist == null)
                    {


                        var entity = new PlanningDetail
                        {
                            IndicatorId = indcator.IndicatorId,
                            BSCPlanId = record.BSCPlanId,
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

                        //var fiscalYear = db.Plans.Where(x=>x.PlanId==record.GetPlan.PlanId).FirstOrDefault();
                        //if (fiscalYear != null)
                        //{

                        var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == record.GetPlan.GlFiscalYearId).ToList();
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

                       // }
                        count++;
                    }
                    else if (exist != null)
                    {

                        exist.IndicatorId = indcator.IndicatorId;
                        exist.BSCPlanId = record.BSCPlanId;
                        exist.KPIType = indcator.KPIType;
                        exist.ParentId = indcator.ParentId;
                        exist.GlobalIndicatorId = indcator.GlobalIndicatorId;
                        exist.HasSubIndicator = indcator.HasSubIndicator;
                        exist.Unit = indcator.Unit;
                        db.Entry(exist).State = EntityState.Modified;
                        db.SaveChanges();
                        count++;
                    }

                }
                if (count > 0)
                {
                    var formattedData = new
                    {
                        Message = " Item Updated Successfully .",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = " there no Updated  Item",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            var formattedData12 = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
            return Json(formattedData12, JsonRequestBehavior.AllowGet);
        }

       
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
