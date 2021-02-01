using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;

namespace ExceedERP.Web.Areas.Construction.Controllers.Tracking.DelayAnalysis
{
    [AuthorizeRoles(ConstructionConstants.ProjectTracking)]
    public class DelayAnalysisController : Controller
    {
        private DelayAnalysisTaskService taskService;
        private DelayAnalysisTaskDependecyService dependencyService;
        
        public DelayAnalysisController()
        {
            taskService = new DelayAnalysisTaskService();
            dependencyService = new DelayAnalysisTaskDependecyService();
            
        }

        protected override void Dispose(bool disposing)
        {
            taskService.Dispose();
            dependencyService.Dispose();

            base.Dispose(disposing);
        }
        public ActionResult Index()
        {
           taskService.CopyTasksFromSchedule();
            return View();
        }
        //private void CopyTasksFromSchedule()
        //{
        //    var availableTasks = db.Tasks.OrderBy(t => t.ID).ToList();
        //    foreach (Task task in availableTasks)
        //    {
        //        var existingDelayTask = db.DelayAnalysisTasks.Where(dat => dat.ID == task.ID).FirstOrDefault();
        //        if (existingDelayTask == null)
        //        {
        //            var newDelayTask = new DelayAnalysisTask
        //            {
        //                ID = task.ID,
        //                ParentID = null,
        //                Start = task.Start,
        //                End = task.End,
        //                Title = task.Title,
        //                Summary = task.Summary,
        //                Expanded = task.Expanded,
        //                PercentComplete = task.PercentComplete,
        //                OrderID = task.OrderID,
        //                DelayDuration = 0

        //            };
        //            db.DelayAnalysisTasks.Add(newDelayTask);
        //            db.SaveChanges();
        //        }
        //    }
        //    UpdateTaskHeirarchy();
        //}
        //private void UpdateTaskHeirarchy() {
        //    var availableTasks = db.Tasks.ToList();
        //    foreach (Task task in availableTasks)
        //    {
        //        var targetTask = db.DelayAnalysisTasks.Where(dat => dat.ID == task.ID).FirstOrDefault();
        //        if (targetTask != null)
        //        {
                    
        //            var rootTaskParent = task.ParentID;
        //            if(rootTaskParent != null)
        //            {
        //                var currentTaskParent = db.DelayAnalysisTasks.Where(tt => tt.ID == rootTaskParent).FirstOrDefault();
        //                targetTask.ParentID = currentTaskParent.DelayAnalysisTaskId;

        //                db.DelayAnalysisTasks.Attach(targetTask);
        //                db.Entry(targetTask).State = System.Data.Entity.EntityState.Modified;
        //                db.SaveChanges();

        //            }
                    
        //        }
        //    }
        //    CopyTaskDependency();
        //}
        //private void CopyTaskDependency()
        //{
        //    var availableDependencies = db.Dependencies.ToList();
        //    foreach (Dependency dependency in availableDependencies)
        //    {
        //        var existingDependency = db.DelayTaskDependencies.Where(dep => dep.ID == dependency.ID).FirstOrDefault();
        //        if (existingDependency == null)
        //        {
        //            var newDepandency = new DelayTaskDependency
        //            {
        //                ID = dependency.ID,
        //                PredecessorID = this.GetEquivalentPredecessorid(dependency.PredecessorID),
        //                SuccessorID = this.GetEquivalentSuccessorId(dependency.SuccessorID),
        //                Type = dependency.Type,
        //            };
        //            if( newDepandency.PredecessorID > 0 && newDepandency.SuccessorID > 0)
        //            {
        //                db.DelayTaskDependencies.Add(newDepandency);
        //                db.SaveChanges();
        //            }
                    
        //        }
        //    }
        //}
        
        
        public virtual JsonResult DelayAnalysisTasks_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(taskService.GetAll().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public virtual JsonResult DelayAnalysisTask_Update([DataSourceRequest]DataSourceRequest request, DelayAnalysisTaskViewModel task) {
            if (ModelState.IsValid)
            {
                taskService.Update(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));

        }

        public virtual JsonResult ReadDependencies([DataSourceRequest] DataSourceRequest request)
        {
            return Json(dependencyService.GetAll().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult CreateDependency([DataSourceRequest] DataSourceRequest request, DelayTaskDependencyViewModel dependency)
        {
            if (ModelState.IsValid)
            {
                dependencyService.Insert(dependency, ModelState);
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }
        public virtual JsonResult UpdateDependency([DataSourceRequest] DataSourceRequest request, DelayTaskDependencyViewModel dependency)
        {
            if (ModelState.IsValid)
            {
                dependencyService.Update(dependency, ModelState);
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }
        public virtual JsonResult DestroyDependency([DataSourceRequest] DataSourceRequest request, DelayTaskDependencyViewModel dependency)
        {
            if (ModelState.IsValid)
            {
                dependencyService.Delete(dependency, ModelState);
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }
    }
}