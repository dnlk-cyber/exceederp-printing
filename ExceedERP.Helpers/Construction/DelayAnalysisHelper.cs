using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ExceedERP.Helpers.Construction
{
    public static class DelayAnalysisHelper
    {
        private static ExceedDbContext db = new ExceedDbContext();
        
        /// <summary>
        /// get the 'as-planned' duration of a given work item,
        /// which is from the schedule data
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static int GetPlannedDurationOfWorkItem(int taskId)
        {
            //var db = new ExceedDbContext();
            var result = 0;
            //DelayAnalysisTask task = new DelayAnalysisTask();
            //var currentWorkItem = db.DelayAnalysisTasks.Find(delayAnalysisTaskId);
            //if (currentWorkItem != null)
            //{
            //    task = currentWorkItem;
            //}
            var task = db.Tasks.Find(taskId);
            if (task != null)
            {
                result = BusinessDaysCalculator.Calculate(task.Start, task.End);
            }
            return result;
        }
        /// <summary>
        /// get the delay duration from the delay start and delay end
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int GetDelayDurtion(WorkDelay model)
        {
            return BusinessDaysCalculator.Calculate(model.Start, model.End);

        }

        /// <summary>
        /// update the delay analysis task when delay information is added, modified or deleted
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="delayOrder"></param>
        public static void UpdateDelayAnalysisTask( int taskId, int delayOrder)
        {
            //ExceedDbContext db = new ExceedDbContext();
            ExceedERP.Core.Domain.Construction.Task scheduleTask = db.Tasks.Find(taskId);
            if (scheduleTask != null)
            {
                var currentDelayAnalysisTask = db.DelayAnalysisTasks.FirstOrDefault(dt => dt.ID == taskId);
                if (currentDelayAnalysisTask != null)
                {
                    var allDelays = db.WorkDelay.Where(wd => wd.ID == taskId && wd.Order <= delayOrder).ToList();
                    if (allDelays.Any())
                    {
                        currentDelayAnalysisTask.HasDelay = true;

                        var concatnatedDelayDurationValue = "";
                        if (allDelays.Count == 1)
                        {
                            var delay = allDelays.FirstOrDefault();
                            if (delay != null)
                                concatnatedDelayDurationValue = " " + delay.Duration;
                        }
                        else if (allDelays.Count > 1)
                        {
                            concatnatedDelayDurationValue = "" + allDelays.FirstOrDefault().Duration + "+";
                            var allDelayArray = allDelays.ToArray();

                            for (int i = 1; i < allDelays.Count() - 1; i++)
                            {
                                concatnatedDelayDurationValue = concatnatedDelayDurationValue + allDelayArray[i].Duration + "+";
                            }
                            concatnatedDelayDurationValue = concatnatedDelayDurationValue + allDelayArray[allDelays.Count() - 1].Duration;
                        }
                        currentDelayAnalysisTask.DelayDuration = concatnatedDelayDurationValue;

                        // add sum of the delay durations to find the new end date of the task
                        var totalDelayDuration = allDelays.Sum(d => d.Duration);
                        currentDelayAnalysisTask.End = currentDelayAnalysisTask.RootTask.End.AddDays(totalDelayDuration);

                        db.DelayAnalysisTasks.Attach(currentDelayAnalysisTask);
                        db.Entry(currentDelayAnalysisTask).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        // update successors dates accordingly
                        UpdateSuccessorsDates(currentDelayAnalysisTask.DelayAnalysisTaskId);
                    }
                    else
                    {
                        currentDelayAnalysisTask.HasDelay = false;
                        //task.DelayDuration = "";
                        db.DelayAnalysisTasks.Attach(currentDelayAnalysisTask);
                        db.Entry(currentDelayAnalysisTask).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                
            }
        }

        /// <summary>
        /// This is used to change the dates of successor tasks when a predessor task
        /// has delay
        /// </summary>
        /// <param name="delayAnalysisTaskId"></param>
        public static void UpdateSuccessorsDates(int delayAnalysisTaskId)
        {
            var db = new ExceedDbContext();
            var currentTask = db.DelayAnalysisTasks.Find(delayAnalysisTaskId);
            if (currentTask != null)
            {
                var successors = db.DelayTaskDependencies.Where(d => d.PredecessorID == delayAnalysisTaskId && d.Type == (int)DependencyType.FinishStart).ToList();
                if (successors.Any())
                {
                    foreach (DelayTaskDependency dependency in successors)
                    {
                        var taskSuccessor = db.DelayAnalysisTasks.Find(dependency.SuccessorID);
                        if (taskSuccessor != null)
                        {
                            var predessorEnd = currentTask.End;
                            var succssorStart = taskSuccessor.Start;
                            if (predessorEnd > succssorStart)
                            {
                                // change the successor tasks start date to at least be the end
                                // date of the predecessor's 
                                TimeSpan span = taskSuccessor.End - taskSuccessor.Start;
                                taskSuccessor.Start = predessorEnd.AddDays(1);
                                taskSuccessor.End = taskSuccessor.Start.Add(span);
                                db.DelayAnalysisTasks.Attach(taskSuccessor);
                                db.Entry(taskSuccessor).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                // Change parent task End and Start dates accordingly
                                SyncParentTaskDates(taskSuccessor.DelayAnalysisTaskId);
                                // see if task has child tasks
                                if (HasChildTask(taskSuccessor.DelayAnalysisTaskId))
                                {
                                    SyncChidTaskDatesWithParentTask(taskSuccessor.DelayAnalysisTaskId);
                                }
                                // check if the successor have successors of its own
                                var successorsSuccessor = db.DelayTaskDependencies.Where(dep => dep.PredecessorID == taskSuccessor.DelayAnalysisTaskId && dep.Type == (int)DependencyType.FinishStart).ToList();
                                if (successorsSuccessor.Any())
                                {
                                    UpdateSuccessorsDates(taskSuccessor.DelayAnalysisTaskId);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// update the dates of a parent task when its child tasks keep growing
        /// </summary>
        /// <param name="delayAnalysisTaskId"></param>
        private static void SyncParentTaskDates(int delayAnalysisTaskId)
        {
            var db = new ExceedDbContext();
            var currentTask = db.DelayAnalysisTasks.Find(delayAnalysisTaskId);
            if (currentTask != null)
            {
                var parentTask = db.DelayAnalysisTasks.Find(currentTask.ParentID);

                if (parentTask != null)
                {
                    var childTasks = db.DelayAnalysisTasks.Where(d => d.ParentID == parentTask.DelayAnalysisTaskId).ToList();
                    if (childTasks.Any())
                    {
                        var maxDate = childTasks.Max(tt => tt.End);
                        var minDate = childTasks.Min(tt => tt.Start);

                        parentTask.End = maxDate;
                        parentTask.Start = minDate;

                        db.DelayAnalysisTasks.Attach(parentTask);
                        db.Entry(parentTask).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                    }
                }
            }
        }
        /// <summary>
        /// change the dates of child tasks when the parent task is changed
        /// </summary>
        /// <param name="modelId"></param>
        private static void SyncChidTaskDatesWithParentTask(int modelId)
        {
            var db = new ExceedDbContext();
            //if (HasChildTask(modelId))
            //{
            var currentTask = db.DelayAnalysisTasks.Find(modelId);
            if (currentTask != null)
            {
                var childTasks = db.DelayAnalysisTasks.Where(d => d.ParentID == currentTask.DelayAnalysisTaskId).ToList();
                if (childTasks.Any())
                {
                    foreach (DelayAnalysisTask childTask in childTasks)
                    {
                        if(childTask.Start < currentTask.Start)
                        {
                            var currentStartDateOfChildTask = childTask.Start;
                            var currentEndDateOfChildTask = childTask.End;
                            TimeSpan span = currentEndDateOfChildTask - currentStartDateOfChildTask;
                            childTask.Start = currentTask.Start;
                            childTask.End = childTask.Start.Add(span);

                            db.DelayAnalysisTasks.Attach(childTask);
                            db.Entry(childTask).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            UpdateSuccessorsDates(childTask.DelayAnalysisTaskId);
                            if (HasChildTask(childTask.DelayAnalysisTaskId))
                            {
                                SyncChidTaskDatesWithParentTask(childTask.DelayAnalysisTaskId);
                            }

                        }
                    }
                }
            }
            //}
        }

        /// <summary>
        /// checks if a given task has child tasks of its own
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static bool HasChildTask(int modelId)
        {
            var db = new ExceedDbContext();
            var hasChildren = false;
            var allChildTasks = db.DelayAnalysisTasks.Where(t => t.ParentID == modelId).ToList();
            if (allChildTasks.Any())
            {
                hasChildren = true;
            }
            return hasChildren;
        }
    }
}
