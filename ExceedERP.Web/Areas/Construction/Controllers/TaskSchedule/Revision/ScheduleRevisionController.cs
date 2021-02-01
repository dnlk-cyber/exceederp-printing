using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule.Revision
{
    public class ScheduleRevisionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ScheduleRevision_Read([DataSourceRequest]DataSourceRequest request, int projectIdRead)
        {
            IQueryable<ScheduleRevision> revisions = db.ScheduleRevisions.Where(sr => sr.ProjectInformationId == projectIdRead);
            DataSourceResult result = revisions.ToDataSourceResult(request, revision => new
            {
                ScheduleRevisionId = revision.ScheduleRevisionId,
                ProjectInformationId = revision.ProjectInformationId,
                Version = revision.Version,
                ScheduleStatus = revision.ScheduleStatus,
                DateOfApproval = revision.DateOfApproval,

                OrgBranchName = revision.OrgBranchName,
                DateCreated = revision.DateCreated,
                LastModified = revision.LastModified,
                CreatedBy = revision.CreatedBy,
                ModifiedBy = revision.ModifiedBy ?? " ",

                Status = revision.Status,
                IsSendForApproval = revision.IsSendForApproval,
                SendForApprovalBy = revision.SendForApprovalBy,
                SendForApprovalTime = revision.SendForApprovalTime,
                IsOnlineApproved = revision.IsOnlineApproved,
                OnlineApprovedBy = revision.OnlineApprovedBy,
                OnlineApprovedTime = revision.OnlineApprovedTime,
                IsOnlineTransferred = revision.IsOnlineTransferred,
                IsOnlineTransferredBy = revision.IsOnlineTransferredBy,
                OnlineTransferredTime = revision.OnlineTransferredTime,
                IsVoid = revision.IsVoid,
                VoidBy = revision.VoidBy,
                VoidTime = revision.VoidTime,
            });

            return Json(result);
        }

        private bool PreviousVersionNotApproved(int projectId)
        {
            var previousNotApproved = false;
            var versions = db.ScheduleRevisions.Where(v => v.ProjectInformationId == projectId).ToList();
            if (versions.Any())
            {
                var lastVerion = versions.Max(v => v.Version);
                var latest = versions.FirstOrDefault(v => v.Version == lastVerion);
                if (latest.ScheduleStatus != ProjectScheduleStatus.Approved)
                {
                    previousNotApproved = true;
                }
            }
            return previousNotApproved;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScheduleRevision_Create([DataSourceRequest]DataSourceRequest request, ScheduleRevision revision, int projectIdCreate)
        {
            if (ModelState.IsValid)
            {
                if (PreviousVersionNotApproved(projectIdCreate))
                {
                    ModelState.AddModelError("", "Previous Version Need to be approved First");
                }
                else
                {
                    var entity = new ScheduleRevision
                    {
                        ProjectInformationId = projectIdCreate,
                        Version = this.SetScheduleVersion(projectIdCreate),
                        ScheduleStatus = revision.ScheduleStatus,
                        DateOfApproval = revision.DateOfApproval,

                        OrgBranchName = revision.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = revision.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = revision.ModifiedBy,

                        Status = OperationStatuses.New.ToString(),
                        IsSendForApproval = revision.IsSendForApproval,
                        SendForApprovalBy = revision.SendForApprovalBy,
                        SendForApprovalTime = revision.SendForApprovalTime,
                        IsOnlineApproved = revision.IsOnlineApproved,
                        OnlineApprovedBy = revision.OnlineApprovedBy,
                        OnlineApprovedTime = revision.OnlineApprovedTime,
                        IsOnlineTransferred = revision.IsOnlineTransferred,
                        IsOnlineTransferredBy = revision.IsOnlineTransferredBy,
                        OnlineTransferredTime = revision.OnlineTransferredTime,
                        IsVoid = revision.IsVoid,
                        VoidBy = revision.VoidBy,
                        VoidTime = revision.VoidTime,

                    };

                    db.ScheduleRevisions.Add(entity);
                    db.SaveChanges();
                    revision.ScheduleRevisionId = entity.ScheduleRevisionId;

                    this.CopyTasksFromPreviosVersion(entity);
                }

            }

            return Json(new[] { revision }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScheduleRevision_Update([DataSourceRequest]DataSourceRequest request, ScheduleRevision revision)
        {
            if (ModelState.IsValid)
            {
                var entity = new ScheduleRevision
                {
                    ScheduleRevisionId = revision.ScheduleRevisionId,
                    ProjectInformationId = revision.ProjectInformationId,
                    Version = revision.Version,
                    ScheduleStatus = revision.ScheduleStatus,
                    DateOfApproval = revision.DateOfApproval,

                    OrgBranchName = revision.OrgBranchName,
                    DateCreated = revision.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = revision.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                    Status = revision.Status,
                    IsSendForApproval = revision.IsSendForApproval,
                    SendForApprovalBy = revision.SendForApprovalBy,
                    SendForApprovalTime = revision.SendForApprovalTime,
                    IsOnlineApproved = revision.IsOnlineApproved,
                    OnlineApprovedBy = revision.OnlineApprovedBy,
                    OnlineApprovedTime = revision.OnlineApprovedTime,
                    IsOnlineTransferred = revision.IsOnlineTransferred,
                    IsOnlineTransferredBy = revision.IsOnlineTransferredBy,
                    OnlineTransferredTime = revision.OnlineTransferredTime,
                    IsVoid = revision.IsVoid,
                    VoidBy = revision.VoidBy,
                    VoidTime = revision.VoidTime,
                };

                db.ScheduleRevisions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { revision }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScheduleRevision_Destroy([DataSourceRequest]DataSourceRequest request, ScheduleRevision revision)
        {
            if (ModelState.IsValid)
            {
                var entity = new ScheduleRevision
                {
                    ScheduleRevisionId = revision.ScheduleRevisionId,
                    ProjectInformationId = revision.ProjectInformationId,
                    Version = revision.Version,
                    ScheduleStatus = revision.ScheduleStatus,
                    DateOfApproval = revision.DateOfApproval,

                    OrgBranchName = revision.OrgBranchName,
                    DateCreated = revision.DateCreated,
                    LastModified = revision.LastModified,
                    CreatedBy = revision.CreatedBy,
                    ModifiedBy = revision.ModifiedBy,

                    Status = revision.Status,
                    IsSendForApproval = revision.IsSendForApproval,
                    SendForApprovalBy = revision.SendForApprovalBy,
                    SendForApprovalTime = revision.SendForApprovalTime,
                    IsOnlineApproved = revision.IsOnlineApproved,
                    OnlineApprovedBy = revision.OnlineApprovedBy,
                    OnlineApprovedTime = revision.OnlineApprovedTime,
                    IsOnlineTransferred = revision.IsOnlineTransferred,
                    IsOnlineTransferredBy = revision.IsOnlineTransferredBy,
                    OnlineTransferredTime = revision.OnlineTransferredTime,
                    IsVoid = revision.IsVoid,
                    VoidBy = revision.VoidBy,
                    VoidTime = revision.VoidTime,
                };

                db.ScheduleRevisions.Attach(entity);
                db.ScheduleRevisions.Remove(entity);
                db.SaveChanges();
                this.RemoveTasksOfCurretVersion(entity);
            }

            return Json(new[] { revision }.ToDataSourceResult(request, ModelState));
        }
        private int SetScheduleVersion(int projectId)
        {
            int counter = 0;
            var availableRevisions = db.ScheduleRevisions.Where(r => r.ProjectInformationId == projectId).ToList();
            // this is the first revion of the project - excluding advance payment
            if (!availableRevisions.Any())
            {
                counter = 1;
            }
            else
            {
                counter = availableRevisions.Max(r => r.Version) + 1;
            }
            return counter;
        }
        private void RemoveTasksOfCurretVersion(ScheduleRevision model)
        {
            var tasks = db.Tasks.Where(t => t.Version == model.Version && t.WorkItemDuration.ProjectInformationId == model.ProjectInformationId).ToList();
            if (tasks.Any())
            {
                db.Tasks.RemoveRange(tasks);
                db.SaveChanges();
            }
        }
        private void CopyTasksFromPreviosVersion(ScheduleRevision model)
        {
            var allTasksUnderProject = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == model.ProjectInformationId).ToList();
            var tempIds = new HashSet<int?>(allTasksUnderProject.Select(t => t.WorkItemDurationId));
            var tempNonNull = tempIds.Where(id => id != null).ToList();
            var itemIds = new HashSet<int>();
            foreach (var id in tempNonNull)
            {
                itemIds.Add((int)id);
            }

            #region add variationa and supplementary work items(that didn't exist in the previous version)
            //var variationAndSupplementary = db.WorkItemPerProject
            //                                   .Where(w => (w.IsVariationWorkItem || w.IsSupplementaryWorkItem) &&
            //                                    w.ProjectInformationId == model.ProjectInformationId &&
            //                                    !workItemIds.Contains(w.WorkItemPerProjectId)
            //                               ).ToList();
            var variationAndSupplementary = db.WorkItemDuration
                                               .Where(w => (w.Source == DurationItemSource.Estimation) &&
                                                w.ProjectInformationId == model.ProjectInformationId &&
                                                !itemIds.Contains(w.WorkItemDurationId)
                                           ).ToList();
            if (variationAndSupplementary.Any())
            {
                SchedulingHelper.AddVariationAndSupplementaryWorkToSchedule(variationAndSupplementary, model.ProjectInformationId, model);

            }
            #endregion
            #region add tasks from previous version
            var previosVersionTasks = allTasksUnderProject.Where(t => t.Version == (model.Version - 1)).ToList();
            if (previosVersionTasks.Any())
            {
                foreach (Task task in previosVersionTasks)
                {
                    var taskOfNextVersion = new Task
                    {
                        Version = task.Version + 1,
                        PreviousVersionTaskId = task.ID,
                        //WorkItemDurationId = task.WorkItemDurationId,
                        WorkItemDurationId = task.WorkItemDurationId,
                        OrderID = task.OrderID,
                        ProjectSpecificId = task.ProjectSpecificId,
                        Title = task.Title,
                        Start = task.Start,
                        End = task.End,
                        Expanded = task.Expanded,
                        ParentID = task.ParentID,
                        PercentComplete = task.PercentComplete,
                        Summary = task.Summary,

                        WorkCategoryId = task.WorkCategoryId,
                        ConfigurationId = task.ConfigurationId,
                        SubProjectTypeId = task.SubProjectTypeId,
                        BlockId = task.BlockId,

                        WorkCategory = task.WorkCategory,
                        StructureyType = task.StructureyType,
                        BlockName = task.BlockName,
                        SubProjectTypeName = task.SubProjectTypeName,                //ActualDuration = BusinessDaysCalculator.Calculate(task.Start, task.End),
                        ActualDuration = task.ActualDuration,
                        //RemainingDuration = BusinessDaysCalculator.Calculate(task.Start > DateTime.Now ? task.Start : DateTime.Now, task.End),
                        TotalQuantityOfWork = task.WorkItemDuration.Quantity - task.ExecutedQuantityToDate,
                        //TotalQuantityOfWork = (task.WorkItemPerProject.TotalQuantityOfWork + task.WorkItemPerProject.AddedQuantityOfWork) - task.WorkItemPerProject.OmittedQuantityOfWork - task.ExecutedQuantityToDate,

                        ExecutedQuantityTillPrevVersion = task.ExecutedQuantityToDate,
                        ExecutedQuantityThisVersion = 0,
                        ExecutedQuantityToDate = task.ExecutedQuantityToDate,

                        //ExecutedAmountTillPrevVersion = task.ExecutedAmountTillPrevVersion,
                        //ExecutedAmountThisVersion = task.ExecutedAmountThisVersion,
                        //ExecutedAmountToDate = task.ExecutedAmountToDate,
                    };
                    taskOfNextVersion.ExecutedAmountTillPrevVersion = taskOfNextVersion.ExecutedQuantityTillPrevVersion * task.WorkItemDuration.TotalUnitRate;
                    taskOfNextVersion.ExecutedAmountToDate = taskOfNextVersion.ExecutedQuantityToDate * task.WorkItemDuration.TotalUnitRate;

                    // check the quantity of work remaining ..
                    // and only copy tasks with remaining anount > 0
                    if (task.WorkItemDuration.HasSubWorkItems)
                    {
                        db.Tasks.Add(taskOfNextVersion);
                        db.SaveChanges();
                    }
                    else
                    {
                        if (taskOfNextVersion.TotalQuantityOfWork > 0)
                        {
                            db.Tasks.Add(taskOfNextVersion);
                            db.SaveChanges();
                        }
                    }

                }
            }
            #endregion
            this.CopyTaskHeirarchy(model);
        }

        private void CopyTaskHeirarchy(ScheduleRevision model)
        {
            var previosVersionTasks = db.Tasks.Where(t => t.Version == (model.Version - 1) &&
                                        model.ProjectInformationId == t.WorkItemDuration.ProjectInformationId).ToList();

            if (previosVersionTasks.Any())
            {
                foreach (Task previousTask in previosVersionTasks)
                {
                    if (previousTask.ParentID != null)
                    {
                        var previousParent = db.Tasks.Find(previousTask.ParentID);
                        if (previousParent != null)
                        {
                            var currentTask = db.Tasks.Where(t => t.PreviousVersionTaskId == previousTask.ID).FirstOrDefault();
                            var currentParent = db.Tasks.Where(t => t.PreviousVersionTaskId == previousParent.ID).FirstOrDefault();
                            if (currentParent != null && currentTask != null)
                            {
                                currentTask.ParentID = currentParent.ID;

                                db.Tasks.Attach(currentTask);
                                db.Entry(currentTask).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                    }
                }
            }
            this.CopyTaskDependecy(model);
        }
        private void CopyTaskDependecy(ScheduleRevision model)
        {
            var previosVersionTasks = db.Tasks.Where(t => t.Version == (model.Version - 1) &&
                                        model.ProjectInformationId == t.WorkItemDuration.ProjectInformationId).ToList();

            if (previosVersionTasks.Any())
            {
                foreach (Task previousTask in previosVersionTasks)
                {
                    var availableDependecies = db.Dependencies.Where(d => d.PredecessorID == previousTask.ID || d.SuccessorID == previousTask.ID).ToList();
                    if (availableDependecies.Any())
                    {
                        foreach (Dependency dependecy in availableDependecies)
                        {
                            var previousPredecessor = db.Tasks.Find(dependecy.PredecessorID);
                            var previousSuccessor = db.Tasks.Find(dependecy.SuccessorID);
                            if (previousPredecessor != null && previousSuccessor != null)
                            {
                                var currentPredecesor = db.Tasks.Where(t => t.PreviousVersionTaskId == previousPredecessor.ID).FirstOrDefault();
                                var currentSuccessor = db.Tasks.Where(t => t.PreviousVersionTaskId == previousSuccessor.ID).FirstOrDefault();
                                if (currentPredecesor != null && currentSuccessor != null)
                                {
                                    var entity = new Dependency
                                    {
                                        PredecessorID = currentPredecesor.ID,
                                        SuccessorID = currentSuccessor.ID,
                                        Type = dependecy.Type
                                    };
                                    // check if this dependency is already available
                                    var temp = db.Dependencies.Where(d => d.PredecessorID == entity.PredecessorID && d.SuccessorID == entity.SuccessorID && d.Type == entity.Type).FirstOrDefault();
                                    if (temp == null)
                                    {
                                        db.Dependencies.Add(entity);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.CopyTaskResources(model);
        }
        private void CopyTaskResources(ScheduleRevision model)
        {
            var previosVersionTasks = db.Tasks.Where(t => t.Version == (model.Version - 1) &&
                                        model.ProjectInformationId == t.WorkItemDuration.ProjectInformationId).ToList();
            if (previosVersionTasks.Any())
            {
                foreach (Task previousTask in previosVersionTasks)
                {
                    var resourceAssignments = db.ResourceAssignments.Where(ra => ra.TaskID == previousTask.ID).ToList();
                    if (resourceAssignments.Any())
                    {
                        foreach (ResourceAssignment assignment in resourceAssignments)
                        {
                            var currentTask = db.Tasks.Where(t => t.PreviousVersionTaskId == previousTask.ID).FirstOrDefault();
                            if (currentTask != null)
                            {
                                var entity = new ResourceAssignment
                                {
                                    TaskID = currentTask.ID,
                                    ResourceID = assignment.ResourceID,
                                    Units = assignment.Units
                                };
                                db.ResourceAssignments.Add(entity);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

        }

        public JsonResult SendForApproval(int id)
        {
            var record = db.ScheduleRevisions.Find(id);

            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                var tasksInThisVerion = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == record.ProjectInformationId && t.Version == record.Version).ToList();
                if (tasksInThisVerion.Any())
                {
                    #region 

                    //foreach (var item in record.WorkInProgressItems)
                    //{
                    //    if (item.UnitPrice <= 0 || item.GLTaxRateID == 0)
                    //    {
                    //        var formattedData3 = new
                    //        {
                    //            Message = "Please fill unit price and tax information for all items.",
                    //            Status = "NOT_OK"
                    //        };
                    //        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    #endregion

                    record.ScheduleStatus = ProjectScheduleStatus.WaitingApproval;
                    record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                    record.IsSendForApproval = true;
                    record.SendForApprovalBy = User.Identity.Name;
                    record.SendForApprovalTime = DateTime.Now;
                    db.ScheduleRevisions.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully Sent.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "This Version is Empty",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            //if (record.WorkInProgressItems.Count == 0)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Please add line items first.",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}