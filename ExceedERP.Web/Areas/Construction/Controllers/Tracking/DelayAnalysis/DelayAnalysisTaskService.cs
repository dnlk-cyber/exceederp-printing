using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Tracking.DelayAnalysis
{
    public class DelayAnalysisTaskService
    {
        private ExceedDbContext db;

        public DelayAnalysisTaskService(ExceedDbContext context)
        {
            db = context;
        }

        public DelayAnalysisTaskService()
            : this(new ExceedDbContext())
        {
        }
        public virtual IQueryable<DelayAnalysisTaskViewModel> GetAll()
        {
            return db.DelayAnalysisTasks.ToList().Select(task => new DelayAnalysisTaskViewModel
            {
                TaskID = task.DelayAnalysisTaskId,
                ParentID = task.ParentID,
                RootTaskId = task.ID,
                Title = task.Title,
                Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),
                PercentComplete = task.PercentComplete,
                OrderId = task.OrderID,
                Expanded = task.Expanded,
                Summary = task.Summary,
                ProjectInformationId = task.RootTask.WorkItemDuration.ProjectInformationId,
                PlannedDuration = BusinessDaysCalculator.Calculate(task.Start, task.End),
                DelayDuration = string.IsNullOrEmpty(task.DelayDuration) ? "" : task.DelayDuration,
                HasDelay = task.HasDelay,
                TaskVersion = task.RootTask.Version
            }).AsQueryable();
        }

        public virtual void Update(DelayAnalysisTaskViewModel task, ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                var entity = task.ToEntity();
                db.DelayAnalysisTasks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
            }
        }
        public void CopyTasksFromSchedule()
        {
            #region clear existing data
            var existingDelayAnalysisTasks = db.DelayAnalysisTasks.ToList();
            var existingDependency = db.DelayTaskDependencies.ToList();
            if (existingDelayAnalysisTasks.Any())
            {
                db.DelayAnalysisTasks.RemoveRange(existingDelayAnalysisTasks);
                db.SaveChanges();
            }
            if (existingDependency.Any())
            {
                db.DelayTaskDependencies.RemoveRange(existingDependency);
                db.SaveChanges();
            }
            #endregion
            var availableTasks = db.Tasks.OrderBy(t => t.ID).ToList();
            foreach (Task task in availableTasks)
            {
                //var existingDelayTask = db.DelayAnalysisTasks.FirstOrDefault(dat => dat.ID == task.ID);
                //if (existingDelayTask == null)
                //{
                var newDelayTask = new DelayAnalysisTask
                {
                    ID = task.ID,
                    ParentID = null,
                    Start = task.Start,
                    End = task.End,
                    Title = task.Title,
                    Summary = task.Summary,
                    Expanded = task.Expanded,
                    PercentComplete = task.PercentComplete,
                    OrderID = task.OrderID,
                    DelayDuration = null,
                    HasDelay = false

                };
                db.DelayAnalysisTasks.Add(newDelayTask);
                db.SaveChanges();
                //}
            }
            UpdateTaskHeirarchy();
        }
        private void UpdateTaskHeirarchy()
        {
            var availableTasks = db.Tasks.ToList();
            foreach (Task task in availableTasks)
            {
                var targetTask = db.DelayAnalysisTasks.Where(dat => dat.ID == task.ID).FirstOrDefault();
                if (targetTask != null)
                {

                    var rootTaskParent = task.ParentID;
                    if (rootTaskParent != null)
                    {
                        var currentTaskParent = db.DelayAnalysisTasks.Where(tt => tt.ID == rootTaskParent).FirstOrDefault();
                        targetTask.ParentID = currentTaskParent.DelayAnalysisTaskId;

                        db.DelayAnalysisTasks.Attach(targetTask);
                        db.Entry(targetTask).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                    }

                }
            }
            CopyTaskDependency();
        }
        private void CopyTaskDependency()
        {
            var availableDependencies = db.Dependencies.ToList();
            foreach (Dependency dependency in availableDependencies)
            {
                var existingDependency = db.DelayTaskDependencies.Where(dep => dep.ID == dependency.ID).FirstOrDefault();
                if (existingDependency == null)
                {
                    var newDepandency = new DelayTaskDependency
                    {
                        ID = dependency.ID,
                        PredecessorID = this.GetEquivalentPredecessorid(dependency.PredecessorID),
                        SuccessorID = this.GetEquivalentSuccessorId(dependency.SuccessorID),
                        Type = dependency.Type,
                    };
                    if (newDepandency.PredecessorID > 0 && newDepandency.SuccessorID > 0)
                    {
                        db.DelayTaskDependencies.Add(newDepandency);
                        db.SaveChanges();
                    }

                }
            }
        }
        private int GetEquivalentPredecessorid(int rootPredessorId)
        {
            var equivalentTask = db.DelayAnalysisTasks.Where(t => t.ID == rootPredessorId).FirstOrDefault();
            int val = 0;
            if (equivalentTask != null)
            {
                val = equivalentTask.DelayAnalysisTaskId;
            }
            return val;
        }
        private int GetEquivalentSuccessorId(int rootSuccessorId)
        {
            var equivalentTask = db.DelayAnalysisTasks.Where(t => t.ID == rootSuccessorId).FirstOrDefault();
            int val = 0;
            if (equivalentTask != null)
            {
                val = equivalentTask.DelayAnalysisTaskId;
            }
            return val;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}