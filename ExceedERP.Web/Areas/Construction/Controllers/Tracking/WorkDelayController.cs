using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Tracking
{
    [AuthorizeRoles(ConstructionConstants.ProjectTracking)]
    public class WorkDelayController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            //ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();

            var workItems = db.Tasks.ToList();
            ViewData["WorkItems"] = workItems;

            var delayTypes = db.DelayTypes.ToList();
            ViewData["DalayTypes"] = delayTypes;
            return View();
        }


        public ActionResult ProjectInformation_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.STARTED || x.ProjectStatus == ProjectStatus.APPROVED);
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                ProjectTypeId = projectInformation.ProjectType,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,

                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
                ProjectStatus = projectInformation.ProjectStatus
            });

            return Json(result);
        }
        public ActionResult WorkDelay_Read([DataSourceRequest]DataSourceRequest request, int projectIdRead, int revisionId)
        {
            var currentVersion = new ScheduleRevision();
            var temp = db.ScheduleRevisions.Find(revisionId);
            if (temp != null)
            {
                currentVersion = temp;
            }

            IQueryable<WorkDelay> workDelays = db.WorkDelay.Where(wd => wd.Task.WorkItemDuration.ProjectInformationId == projectIdRead && wd.Task.Version == currentVersion.Version);
            DataSourceResult result = workDelays.ToDataSourceResult(request, delay => new
            {
                WorkDelayId = delay.WorkDelayId,
                ID = delay.ID,
                PlannedDuration = delay.PlannedDuration,
                Order = delay.Order,
                Description = delay.Description,
                DelayTypeId = delay.DelayTypeId,
                Start = delay.Start,
                End = delay.End,
                Duration = delay.Duration,

                OrgBranchName = delay.OrgBranchName,
                DateCreated = delay.DateCreated,
                LastModified = delay.LastModified,
                CreatedBy = delay.CreatedBy,
                ModifiedBy = delay.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkDelay_Create([DataSourceRequest]DataSourceRequest request, WorkDelay delay)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkDelay
                {
                    //WorkDelayId = delay.WorkDelayId,
                    ID = delay.ID,
                    PlannedDuration = DelayAnalysisHelper.GetPlannedDurationOfWorkItem(delay.ID),
                    Order = this.CalculateDelayOrder(delay.ID),
                    Description = delay.Description,
                    DelayTypeId = delay.DelayTypeId,
                    Start = delay.Start,
                    End = delay.End,
                    Duration = DelayAnalysisHelper.GetDelayDurtion(delay),

                    OrgBranchName = delay.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = delay.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = delay.ModifiedBy
                };

                db.WorkDelay.Add(entity);
                db.SaveChanges();
                delay.WorkDelayId = entity.WorkDelayId;

                //DelayAnalysisHelper.UpdateDelayAnalysisTask(entity.DelayAnalysisTaskId);
                //DelayAnalysisHelper.UpdateSuccessorsDates(entity.DelayAnalysisTaskId);
            }

            return Json(new[] { delay }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkDelay_Update([DataSourceRequest]DataSourceRequest request, WorkDelay delay)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkDelay
                {
                    WorkDelayId = delay.WorkDelayId,
                    ID = delay.ID,
                    PlannedDuration = DelayAnalysisHelper.GetPlannedDurationOfWorkItem(delay.ID),
                    Order = delay.Order,
                    Description = delay.Description,
                    DelayTypeId = delay.DelayTypeId,
                    Start = delay.Start,
                    End = delay.End,
                    Duration = DelayAnalysisHelper.GetDelayDurtion(delay),

                    OrgBranchName = delay.OrgBranchName,
                    DateCreated = delay.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = delay.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                db.WorkDelay.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //DelayAnalysisHelper.UpdateDelayAnalysisTask(entity.DelayAnalysisTaskId);

            }

            return Json(new[] { delay }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkDelay_Destroy([DataSourceRequest]DataSourceRequest request, WorkDelay delay)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkDelay
                {
                    WorkDelayId = delay.WorkDelayId,
                    ID = delay.ID,
                    PlannedDuration = delay.PlannedDuration,
                    Order = delay.Order,
                    Description = delay.Description,
                    DelayTypeId = delay.DelayTypeId,
                    Start = delay.Start,
                    End = delay.End,
                    Duration = delay.Duration,

                    OrgBranchName = delay.OrgBranchName,
                    DateCreated = delay.DateCreated,
                    LastModified = delay.LastModified,
                    CreatedBy = delay.CreatedBy,
                    ModifiedBy = delay.ModifiedBy
                };

                db.WorkDelay.Attach(entity);
                db.WorkDelay.Remove(entity);
                db.SaveChanges();

                //DelayAnalysisHelper.UpdateDelayAnalysisTask(entity.DelayAnalysisTaskId);

            }

            return Json(new[] { delay }.ToDataSourceResult(request, ModelState));
        }

        private int CalculateDelayOrder(int taskId)
        {
            var delayOrder = 1;
            //var currentDelayAnalysisTask = db.DelayAnalysisTasks.Find(delayAnalysisTaskId);
            var currentTask = db.Tasks.Find(taskId);
            if (currentTask != null)
            {
                var allDelays = db.WorkDelay
                    .Where(wd => wd.Task.WorkItemDuration.ProjectInformationId == currentTask
                                     .WorkItemDuration.ProjectInformationId && wd.Task.Version ==
                                 currentTask.Version).ToList();
                if (allDelays.Any())
                {
                    // if there are previous delays, add one to the latest one
                    delayOrder = allDelays.Max(d => d.Order) + 1;
                }
            }
            return delayOrder;
        }

        public void AddDelayToDelayAnalysisTask(int delayId)
        {
            

            var currentDelay = db.WorkDelay.Find(delayId);
            if (currentDelay != null)
            {
                //var existingDelayAnalysisTask = db.DelayAnalysisTasks
                //    .Where(dt => dt.RootTask.ProjectWorkBreakDown.ProjectInformationId ==
                //                 currentDelay.Task.ProjectWorkBreakDown.ProjectInformationId).ToList();
                ////var existing
                //if (existingDelayAnalysisTask.Any())
                //{
                //    db.DelayAnalysisTasks.RemoveRange(existingDelayAnalysisTask);
                //    db.SaveChanges();
                //}

                // if there are prior delays
                // they should be added first
                var priorDelays = db.WorkDelay.Where(d => d.Order < currentDelay.Order && d.ID == currentDelay.ID)
                    .ToList();
                if (priorDelays.Any())
                {
                    foreach (WorkDelay priorDelay in priorDelays)
                    {
                        DelayAnalysisHelper.UpdateDelayAnalysisTask(priorDelay.ID, priorDelay.Order);
                    }
                }

                // add current delay 
                DelayAnalysisHelper.UpdateDelayAnalysisTask(currentDelay.ID, currentDelay.Order);
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}