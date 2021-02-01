using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public class ProjectWorkBreakDownController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectWorkBreakDown_Read([DataSourceRequest]DataSourceRequest request, int idOne)
        {

            IQueryable<ProjectWorkBreakDown> projectworkbreakdowns = db.ProjectWorkBreakDown.Where(proj => proj.ProjectInformationId == idOne);
            DataSourceResult result = projectworkbreakdowns.ToDataSourceResult(request, projectWorkBreakDown => new
            {
                ProjectWorkBreakDownId = projectWorkBreakDown.ProjectWorkBreakDownId,
                WBSCode = projectWorkBreakDown.WBSCode,
                Description = projectWorkBreakDown.Description,
                ConfigurationId = projectWorkBreakDown.ConfigurationId,
                //OriginalDuration = GetWBSDuration(projectWorkBreakDown.ProjectWorkBreakDownId),
                RemainingDuration = projectWorkBreakDown.RemainingDuration,
                ProjectInformationId = projectWorkBreakDown.ProjectInformationId,
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
               
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectWorkBreakDown_Create([DataSourceRequest]DataSourceRequest request, ProjectWorkBreakDown projectWorkBreakDown, int idTwo)
        {
            ProjectInformation targetProjectInformation = db.ProjectInformation.Find(idTwo);
            if (targetProjectInformation != null)
            {

                if (ModelState.IsValid)
                {
                    var entity = new ProjectWorkBreakDown
                    {
                        WBSCode = projectWorkBreakDown.WBSCode,
                        Description = projectWorkBreakDown.Description,
                        ProjectInformationId = targetProjectInformation.ProjectInformationId,
                        ConfigurationId = projectWorkBreakDown.ConfigurationId,
                        OriginalDuration = projectWorkBreakDown.OriginalDuration,
                        RemainingDuration = projectWorkBreakDown.RemainingDuration,
                        Project = targetProjectInformation
                    };

                    db.ProjectWorkBreakDown.Add(entity);
                    db.SaveChanges();
                    projectWorkBreakDown.ProjectWorkBreakDownId = entity.ProjectWorkBreakDownId;
                }

            }
            return Json(new[] { projectWorkBreakDown }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectWorkBreakDown_Update([DataSourceRequest]DataSourceRequest request, ProjectWorkBreakDown projectWorkBreakDown)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectWorkBreakDown
                {
                    ProjectWorkBreakDownId = projectWorkBreakDown.ProjectWorkBreakDownId,
                    WBSCode = projectWorkBreakDown.WBSCode,
                    Description = projectWorkBreakDown.Description,
                    ProjectInformationId = projectWorkBreakDown.ProjectInformationId,
                    ConfigurationId = projectWorkBreakDown.ConfigurationId,
                    OriginalDuration = projectWorkBreakDown.OriginalDuration,
                    RemainingDuration = projectWorkBreakDown.RemainingDuration,
                };

                db.ProjectWorkBreakDown.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectWorkBreakDown }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectWorkBreakDown_Destroy([DataSourceRequest]DataSourceRequest request, ProjectWorkBreakDown projectWorkBreakDown)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectWorkBreakDown
                {
                    ProjectWorkBreakDownId = projectWorkBreakDown.ProjectWorkBreakDownId,
                    WBSCode = projectWorkBreakDown.WBSCode,
                    Description = projectWorkBreakDown.Description,
                    ProjectInformationId = projectWorkBreakDown.ProjectInformationId,
                    ConfigurationId = projectWorkBreakDown.ConfigurationId,
                    OriginalDuration = projectWorkBreakDown.OriginalDuration,
                    RemainingDuration = projectWorkBreakDown.RemainingDuration,
                };

                db.ProjectWorkBreakDown.Attach(entity);
                db.ProjectWorkBreakDown.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectWorkBreakDown }.ToDataSourceResult(request, ModelState));
        }

        //private int GetWBSDuration(int wbsId)
        //{
        //    ////var t1 = from t in db.Tasks
        //    ////         where t.ProjectWorkBreakDownId == wbsId
        //    ////         orderby t.Start ascending
        //    ////         select new { ID = t.ID, Start = t.Start, End = t.End};
        //    ////var earlyTaskStartDate = t1.AsQueryable().FirstOrDefault();

        //    ////var t2 = from tt in db.Tasks
        //    ////         where tt.ProjectWorkBreakDownId == wbsId
        //    ////         orderby tt.End descending
        //    ////         select new { ID = tt.ID, Start = tt.Start, End = tt.End };

        //    //var t1 = (from t in db.Tasks
        //    //         where t.ProjectWorkBreakDownId == wbsId
        //    //         //orderby t.Start ascending
        //    //         select Convert.ToDateTime(t.Start)).Distinct().OrderByDescending(tsk => tsk.Start).FirstOrDefault();
        //    ////var earlyTaskStartDate = t1.FirstOrDefault();

        //    //var t2 = (from tt in db.Tasks
        //    //         where tt.ProjectWorkBreakDownId == wbsId
        //    //         //orderby tt.End descending
        //    //         select new { ID = tt.ID, Start = tt.Start, End = tt.End }).Distinct().OrderByDescending(ts => ts.End).FirstOrDefault();
        //    ////var task2 = t2.FirstOrDefault();
        //    ////var lateTaskEndDate = t2.AsQueryable();
        //    ////var date2 = lateTaskEndDate.
        //    ////var task1 = db.Tasks.Where(tsk => tsk.ProjectWorkBreakDownId == wbsId).OrderByDescending(t => t.Start).LastOrDefault();
        //    ////var task1 = listOne.Last();

        //    //// var task2 = db.Tasks.Where(tsk => tsk.ProjectWorkBreakDownId == wbsId).OrderByDescending(t => t.End).FirstOrDefault();
        //    ////var task2 = listTwo.First();

        //    var totalDays = 0;
        //    var taskList = db.Tasks.Where(task => task.ProjectWorkBreakDownId == wbsId).AsQueryable();
        //    if (taskList.Count() != 0)
        //    {
        //        var largestDate = DateTime.Now;
        //        var smallestDate = DateTime.Now;

        //        var tsk = taskList.FirstOrDefault();
        //        if (tsk != null)
        //        {
        //            smallestDate = tsk.Start;
        //            largestDate = tsk.End;

        //            foreach (var t in taskList)
        //            {
        //                if (t.Start < smallestDate)
        //                {
        //                    smallestDate = t.Start;
        //                }
        //            }
        //            foreach (var t in taskList)
        //            {
        //                if (t.End > largestDate)
        //                {
        //                    largestDate = t.End;
        //                }
        //            }
        //        }
        //        totalDays = BusinessDaysCalculator.Calculate(smallestDate, largestDate);
        //    }
        //    else
        //    {
        //        totalDays = 0;
        //    }
        //    return totalDays;

        //}
        public void ImportWorkItemToSchedule(int workkItemId, int currentWBSId) {
            var currentWorkItem = db.WorkItemPerProject.FirstOrDefault(w => w.WorkItemPerProjectId == workkItemId);
            if(currentWorkItem != null)
            {
                #region Add schedule version 1.0, if its the first time
                var firstVersionSchedule = db.ScheduleRevisions.FirstOrDefault(r => r.ProjectInformationId == currentWorkItem.ProjectInformationId);
                if (firstVersionSchedule == null)
                {
                    // add  shedule version 1.0
                    var firstVersion = new ScheduleRevision
                    {
                        ProjectInformationId = currentWorkItem.ProjectInformationId,
                        Version = 1,
                        ScheduleStatus = ProjectScheduleStatus.Editing,
                        OrgBranchName = null,
                        DateOfApproval = null,
                        CreatedBy = null,
                        ModifiedBy = null,
                        DateCreated = null,
                        LastModified = null,

                    };
                    db.ScheduleRevisions.Add(firstVersion);
                    db.SaveChanges();
                }
                #endregion 
                //var workItemDuration = db.WorkItemDuration.FirstOrDefault(wid => wid.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId);
                //if(workItemDuration != null)
                //{
                //    var taskToInsert = new TaskViewModel();
                   
                //    var todaysDate = DateTime.Now;
                //    taskToInsert.Start = todaysDate;
                //    taskToInsert.End = todaysDate.AddDays((int)workItemDuration.EstimatedDuration);
                //    taskToInsert.WorkItemDurationId = workItemDuration.WorkItemDurationId;
                //    taskToInsert.Title = workItemDuration.WorkDescription;
                //    taskToInsert.PercentComplete = 0;
                //    //taskToInsert.OrderId = 0;
                //    taskToInsert.Summary = workItemDuration.WorkItemPerProject.IsSubCategory ? true : false;
                //    taskToInsert.Expanded = true;
                //    taskToInsert.ProjectWorkBreakDownId = currentWBSId;
                //    //taskToInsert.ProjectWorkBreakDown = targetWBS;

                //    taskToInsert.WorkItemDurationId = workItemDuration.WorkItemDurationId;

                //    taskToInsert.EstimatedDuration = workItemDuration.EstimatedDuration;
                //    taskToInsert.TotalQuantityOfWork = workItemDuration.WorkItemPerProject.TotalQuantityOfWork;
                //    taskToInsert.BudgetCost = ProjectEstimationHelper.GetWorkItemBudgetedCost(workItemDuration.WorkItemPerProjectId);
                //    taskToInsert.DirectCost = workItemDuration.WorkItemPerProject.DirectCost;
                //    taskToInsert.MaterialCost = workItemDuration.WorkItemPerProject.MaterialCost;
                //    taskToInsert.EquipmentCost = workItemDuration.WorkItemPerProject.EquipmentOutput;
                //    taskToInsert.LabourCost = workItemDuration.WorkItemPerProject.LabourOutPut;
                //    taskToInsert.OrderId = this.SetWorkItemOrderInSchedule(currentWorkItem);
                //    taskToInsert.Version = 1;
                //    //taskToInsert.OriginalDuration = 1;
                //    //taskToInsert.RemainingDuration = 1;

                //    var entity = taskToInsert.ToEntity();
                //    var tempDb = new ExceedDbContext();
                //    tempDb.Tasks.Add(entity);
                //    tempDb.SaveChanges();
                //}
            }
        }
        private int SetWorkItemOrderInSchedule(WorkItemPerProject model)
        {
            int order = 0;
            var currentProject = db.ProjectInformation.Find(model.ProjectInformationId);
            if (currentProject != null)
            {
                var availableTasks = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == currentProject.ProjectInformationId);
                if (availableTasks.Any())
                {
                    order = availableTasks.Max(task => task.OrderID) + 1;
                }
            }
            return order;
        }
        private void CheckAndChangeZeroVersionTask()
        {
            var tasks = db.Tasks.Where(t => t.Version == 0).ToList();
            if (tasks.Any())
            {
                foreach(var t in tasks)
                {
                    t.Version = 1;

                    db.Tasks.Attach(t);
                    db.Entry(t).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
