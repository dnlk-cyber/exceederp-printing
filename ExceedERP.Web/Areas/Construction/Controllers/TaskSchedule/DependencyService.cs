using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public class DependencyService
    {
        private ExceedDbContext db;

        public DependencyService(ExceedDbContext context)
        {
            db = context;
        }

        public DependencyService()
            : this(new ExceedDbContext())
        {
        }

        public virtual IQueryable<DependencyViewModel> GetAll()
        {
            return db.Dependencies.ToList().Select(dependency => new DependencyViewModel
            {
                DependencyID = dependency.ID,
                PredecessorID = dependency.PredecessorID,
                SuccessorID = dependency.SuccessorID,
                Type = (DependencyType)dependency.Type,
                // predecessorTaskname = db.Tasks.Find(dependency.PredecessorID).Title
            }).AsQueryable();
        }
        public virtual IQueryable<DependencyViewModel> GetDependencies(int? taskId)
        {
            if (taskId == null)
            {
                return this.GetAll();
            }
            return db.Dependencies.Where(d => d.SuccessorID == taskId).ToList().Select(dependency => new DependencyViewModel
            {
                DependencyID = dependency.ID,
                PredecessorID = dependency.PredecessorID,
                SuccessorID = dependency.SuccessorID,
                Type = (DependencyType)dependency.Type,
                predecessorTaskname = db.Tasks.Find(dependency.PredecessorID).Title
            }).AsQueryable();
        }
        public virtual IQueryable<DependencyViewModel> GetPredecessors(int? taskId)
        {
            if (taskId == null)
            {
                return this.GetAll();
            }
            return db.Dependencies.Where(d => d.SuccessorID == taskId).ToList().Select(dependency => new DependencyViewModel
            {
                DependencyID = dependency.ID,
                PredecessorID = dependency.PredecessorID,
                SuccessorID = dependency.SuccessorID,
                Type = (DependencyType)dependency.Type,
                LagTime = dependency.LagTime,
                predecessorTaskname = db.Tasks.Find(dependency.PredecessorID).Title
            }).AsQueryable();
        }
        public virtual IQueryable<DependencyViewModel> GetSuccessors(int? taskId)
        {
            if (taskId == null)
            {
                return this.GetAll();
            }
            return db.Dependencies.Where(d => d.PredecessorID == taskId).ToList().Select(dependency => new DependencyViewModel
            {
                DependencyID = dependency.ID,
                PredecessorID = dependency.PredecessorID,
                SuccessorID = dependency.SuccessorID,
                Type = (DependencyType)dependency.Type,
                LagTime = dependency.LagTime,
                SuccessorTaskname = db.Tasks.Find(dependency.SuccessorID).Title
            }).AsQueryable();
        }
        public virtual string GetTaskPredecessors(int taskId)
        {
            var predecessors = new List<int>();
            //var dependencies = db.Dependencies.Where(d => d.SuccessorID == taskId).AsQueryable();
            var projSpecIds = new HashSet<int>(
                (from t in db.Tasks
                 join dep in db.Dependencies
                 on t.ID equals dep.PredecessorID
                 where dep.SuccessorID == taskId
                 select new
                 {
                     ProjectSpecificId = t.ProjectSpecificId,
                 }).ToList()
                 .Select(i => i.ProjectSpecificId));
            predecessors.AddRange(projSpecIds);

            //if (dependencies.Any())
            //{
            //    foreach (var dp in dependencies)
            //    {
            //        predecessors.Add(dp.PredecessorID);
            //    }
            //}

            var concatnatedStr = " ";
            foreach (var predecessor in predecessors)
            {
                concatnatedStr = concatnatedStr + predecessor + " ";
            }
            return concatnatedStr;
        }
        public virtual IQueryable<PredecssorViewModel> GetPredecessors(int? myCurrentTaskId, int? pId)
        {
            var allTasks = db.Tasks.ToList();
            if (pId != null && allTasks.Any())
            {
                allTasks = allTasks.Where(t => t.WorkItemDuration.ProjectInformationId == pId).ToList();
            }
            if (myCurrentTaskId != null)
            {
                var selectedTask = db.Tasks.Find(myCurrentTaskId);
                // find the candidate tasks
                // which are  - the tasks within the project and that have
                // end date less than the start of the task in question
                if (selectedTask != null)
                {
                    //allTasks = allTasks.Where(t => t.Version == selectedTask.Version && t.End <= selectedTask.Start).OrderBy(t => t.OrderID).ToList();
                    allTasks = allTasks
                        .Where(t => t.ID != myCurrentTaskId &&
                            t.Version == selectedTask.Version &&
                            t.BlockId <= selectedTask.BlockId &&
                            t.ID != selectedTask.ParentID)
                        .OrderBy(t => t.OrderID)
                        .ToList();
                }
            }
            var listToReturn = new List<PredecssorViewModel>();
            foreach (var task in allTasks)
            {
                var dep = this.GetDependecyInfoOfPredecessorTask(myCurrentTaskId, task.ID);

                var record = new PredecssorViewModel
                {
                    Id = task.ID,
                    Name = task.Title,
                    IsSelected = dep != null ? true : false,
                    Type = dep != null ? (DependencyType)dep.Type : (DependencyType)5,
                    LagTime = dep != null ? dep.LagTime : 0,

                    ProjectSpecificId = task.ProjectSpecificId
                };
                listToReturn.Add(record);
            }
            //return allTasks.Select(task => new PredecssorViewModel
            //{
            //    Id = task.ID,
            //    Name = task.Title,
            //    IsSelected = IsAnExistingPredecessor(myCurrentTaskId, task.ID),
            //    Type = GetDependencyType(myCurrentTaskId, task.ID),
            //    LagTime = GetDependencyLagTime(myCurrentTaskId, task.ID),

            //    ProjectSpecificId = task.ProjectSpecificId

            //}).AsQueryable();
            return listToReturn.AsQueryable();
        }


        public virtual void Insert(DependencyViewModel dependency, ModelStateDictionary modelState, int? followerId)
        {
            var entity = new Dependency();
            if (followerId != null)
            {
                entity = new Dependency
                {
                    PredecessorID = dependency.PredecessorID,
                    SuccessorID = (int)followerId,
                    Type = (int)dependency.Type
                };
            }
            else
            {
                entity = dependency.ToEntity();
            }
            db.Dependencies.Add(entity);
            db.SaveChanges();

            dependency.DependencyID = entity.ID;
        }

        public virtual void InsertTaskPredessor(DependencyViewModel dependency, ModelStateDictionary modelState, int successor)
        {
            var entity = new Dependency
            {
                PredecessorID = dependency.PredecessorID,
                SuccessorID = successor,
                Type = (int)dependency.Type
            };

            db.Dependencies.Add(entity);
            db.SaveChanges();

            dependency.DependencyID = entity.ID;
        }
        public virtual void InsertGanttTaskPredecessor(PredecssorViewModel dpViewmodel, ModelStateDictionary modelState, int tskId)
        {
            var entity = new Dependency
            {
                PredecessorID = dpViewmodel.Id,
                SuccessorID = tskId,
                Type = (int)dpViewmodel.Type,
                LagTime = dpViewmodel.LagTime,
            };

            db.Dependencies.Add(entity);
            db.SaveChanges();

            this.UpdateScheduleBasedOnDependency(dpViewmodel, modelState, tskId);
            //dpViewmodel.Id = entity.ID;
        }

        public virtual void UpdateGanttTaskPredecessor(PredecssorViewModel dpViewmodel, ModelStateDictionary modelState, int tskId)
        {
            var depToEdit = db.Dependencies.Where(dp => dp.PredecessorID == dpViewmodel.Id && dp.SuccessorID == tskId).FirstOrDefault();

            if (depToEdit != null)
            {
                if (!dpViewmodel.IsSelected)
                {
                    this.DeletePredecessor(dpViewmodel, modelState, tskId);
                }
                else
                {
                    depToEdit.Type = (int)dpViewmodel.Type;
                    depToEdit.LagTime = dpViewmodel.LagTime;

                    db.Dependencies.Attach(depToEdit);
                    db.Entry(depToEdit).State = EntityState.Modified;
                    db.SaveChanges();
                }
                this.UpdateScheduleBasedOnDependency(dpViewmodel, modelState, tskId);

            }
            else
            {
                this.InsertGanttTaskPredecessor(dpViewmodel, modelState, tskId);
            }

            //var entity = new Dependency
            //{
            //    ID = depToEdit.ID,
            //    PredecessorID = dpViewmodel.Id,
            //    SuccessorID = tskId,
            //    Type = (int)dpViewmodel.Type
            //};


            //dpViewmodel.Id = entity.ID;
        }

        public virtual void DeletePredecessor(PredecssorViewModel dpViewmodel, ModelStateDictionary modelState, int tskId)
        {
            var depToDelete = db.Dependencies.Where(dp => dp.PredecessorID == dpViewmodel.Id && dp.SuccessorID == tskId).FirstOrDefault();
            //var entity = new Dependency
            //{
            //    ID = depToDelete.ID,
            //    PredecessorID = dpViewmodel.Id,
            //    SuccessorID = tskId,
            //    Type = (int)dpViewmodel.Type
            //};
            db.Dependencies.Attach(depToDelete);
            db.Dependencies.Remove(depToDelete);
            db.SaveChanges();
        }
        public virtual void Update(DependencyViewModel dependency, ModelStateDictionary modelState)
        {
            var entity = dependency.ToEntity();
            db.Dependencies.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public virtual void Delete(DependencyViewModel dependency, ModelStateDictionary modelState)
        {
            var entity = dependency.ToEntity();
            db.Dependencies.Attach(entity);
            db.Dependencies.Remove(entity);
            db.SaveChanges();
        }

        #region Methods for local use
        private Dependency GetDependecyInfoOfPredecessorTask(int? targetTaskId, int predId)
        {
            return db.Dependencies
                                .FirstOrDefault(dp => dp.SuccessorID == targetTaskId && dp.PredecessorID == predId);
        }
        public bool IsAnExistingPredecessor(int? targetTaskId, int predId)
        {
            var isPredecessor = false;
            if (targetTaskId != null)
            {
                //var allPredecessors = from d in db.Dependencies
                //                      join t in db.Tasks
                //                      on d.PredecessorID equals t.ID
                //                      where d.SuccessorID == targetTaskId
                //                      select new
                //                      {
                //                          ID = t.ID,
                //                          Title = t.Title
                //                      };
                var predecessorEntity = db.Dependencies
                                                .FirstOrDefault(dp => dp.SuccessorID == targetTaskId && dp.PredecessorID == predId);

                //foreach (var pd in allPredecessors)
                //{
                //    if (pd.ID == predId)
                //    {
                //        IsAPredessor = true;
                //    }
                //}
                if (predecessorEntity != null)
                {
                    // task is defined as a predessor already
                    isPredecessor = true;
                }

            }
            return isPredecessor;

        }
        public DependencyType GetDependencyType(int? targetTaskId, int predId)
        {
            var taskDependecy = 5;
            if (targetTaskId != null)
            {
                //var allPredecessors = from d in db.Dependencies
                //                      join t in db.Tasks
                //                      on d.PredecessorID equals t.ID
                //                      where d.SuccessorID == targetTaskId
                //                      select new
                //                      {
                //                          ID = t.ID,
                //                          Title = t.Title,
                //                          DepType = d.Type,
                //                          LagTime = d.LagTime,
                //                      };
                var predecessorEntity = db.Dependencies
                                                .FirstOrDefault(dp => dp.SuccessorID == targetTaskId && dp.PredecessorID == predId);


                //foreach (var pd in allPredecessors)
                //{
                //    if (pd.ID == predId)
                //    {
                //        //IsAPredessor = true;
                //        taskDependecy = pd.DepType;
                //    }
                //}
                if (predecessorEntity != null)
                {
                    taskDependecy = predecessorEntity.Type;
                }

            }
            return (DependencyType)taskDependecy;

        }
        public int GetDependencyLagTime(int? targetTaskId, int predId)
        {
            var lag = 0;
            if (targetTaskId != null)
            {
                //var allPredecessors = from d in db.Dependencies
                //                      join t in db.Tasks
                //                      on d.PredecessorID equals t.ID
                //                      where d.SuccessorID == targetTaskId
                //                      select new
                //                      {
                //                          ID = t.ID,
                //                          Title = t.Title,
                //                          DepType = d.Type,
                //                          LagTime = d.LagTime,
                //                      };
                var predecessorEntity = db.Dependencies
                                                .FirstOrDefault(dp => dp.SuccessorID == targetTaskId && dp.PredecessorID == predId);

                //foreach (var pd in allPredecessors)
                //{
                //    if (pd.ID == predId)
                //    {
                //        //IsAPredessor = true;
                //        lag = pd.LagTime;
                //    }
                //}
                if (predecessorEntity != null)
                {
                    lag = predecessorEntity.LagTime;
                }

            }
            return lag;

        }
        private void UpdateScheduleBasedOnDependency(PredecssorViewModel predessorViewModel, ModelStateDictionary modelState, int currentTaskId)
        {
            var currentTask = db.Tasks.Find(currentTaskId);
            var predessorTask = db.Tasks.Find(predessorViewModel.Id);
            if (currentTask != null && predessorTask != null)
            {
                // change the dates of the current task according the 
                // predessor and its type
                var dependency = predessorViewModel.Type;
                var lag = predessorViewModel.LagTime;
                var durationOfCurrentTask = currentTask.End - currentTask.Start;
                if (dependency == DependencyType.FinishStart)
                {
                    currentTask.Start = predessorTask.End.AddDays(lag);
                    currentTask.End = currentTask.Start.Add(durationOfCurrentTask);
                }
                else if (dependency == DependencyType.FinishFinish)
                {
                    currentTask.End = predessorTask.End.AddDays(predessorViewModel.LagTime);
                    currentTask.Start = currentTask.End.Subtract(durationOfCurrentTask);
                }
                else if (dependency == DependencyType.StartFinish)
                {
                    currentTask.End = predessorTask.Start.AddDays(predessorViewModel.LagTime);
                    currentTask.Start = currentTask.End.Subtract(durationOfCurrentTask);
                }
                else if (dependency == DependencyType.StartStart)
                {
                    currentTask.Start = predessorTask.Start.AddDays(predessorViewModel.LagTime);
                    currentTask.End = currentTask.Start.Add(durationOfCurrentTask);
                }

                db.Tasks.Attach(currentTask);
                db.Entry(currentTask).State = EntityState.Modified;
                db.SaveChanges();

                // if selected task is a child task check the parent task dates
                if (TaskHasParent(currentTask.ID))
                {
                    UpdateParentTaskWhenChildTaskDatesChange((int)currentTask.ParentID);
                }
                if (TaskHasChildren(currentTask.ID))
                {
                    UpdateChildTasksWhenParentTaskDateChange(currentTask.ID);
                }
                if (TaskHasSuccessor(currentTask.ID))
                {
                    UpdateTaskSuccssorWhenTaskIsModified(currentTask.ID, modelState);
                }
            }
        }
        public void UpdateParentTaskWhenChildTaskDatesChange(int id)
        {
            var task = db.Tasks.Find(id);
            if (task != null)
            {
                var childTasks = db.Tasks.Where(t => t.ParentID == task.ID).ToList();
                if (childTasks.Any())
                {
                    var maxEndDate = childTasks.Max(d => d.End);
                    var minStart = childTasks.Min(d => d.Start);
                    if (task.Start > minStart || task.End < maxEndDate)
                    {
                        //if (task.Start > minStart)
                        //{
                        //    task.Start = minStart;
                        //}
                        //else if (task.End < maxEndDate)
                        //{
                        //    task.End = maxEndDate;
                        //}
                        task.Start = minStart;
                        task.End = maxEndDate;

                        //task.ActualDuration = (task.End - task.Start).Days;
                        task.ActualDuration = BusinessDaysCalculator.Calculate(minStart, maxEndDate, false, task.WorkItemDuration.ProjectInformationId);

                        db.Tasks.Attach(task);
                        db.Entry(task).State = EntityState.Modified;
                        db.SaveChanges();
                        if (TaskHasParent(task.ID))
                        {
                            UpdateParentTaskWhenChildTaskDatesChange(task.ParentID ?? 0);
                        }
                        //if (TaskHasChildren(task.ID))
                        //{
                        //    UpdateChildTasksWhenParentTaskDateChange(task.ID);
                        //}
                        if (TaskHasSuccessor(task.ID))
                        {
                            UpdateTaskSuccssorWhenTaskIsModified(task.ID, null);
                        }
                    }
                }
            }
        }
        public void UpdateTaskSuccssorWhenTaskIsModified(int id, ModelStateDictionary modelState)
        {
            var currentTask = db.Tasks.Find(id);
            if (currentTask != null)
            {
                var successors = db.Dependencies.Where(d => d.PredecessorID == id).ToList();
                if (successors.Any())
                {
                    foreach (var successor in successors)
                    {
                        var depType = GetDependencyType(successor.SuccessorID, id);
                        var lagTime = GetDependencyLagTime(successor.SuccessorID, id);
                        var dpViewModel = new PredecssorViewModel
                        {
                            Id = id,
                            Name = currentTask.Title,
                            Type = depType,
                            LagTime = lagTime
                        };
                        UpdateScheduleBasedOnDependency(dpViewModel, modelState, successor.SuccessorID);
                    }
                }
            }
        }
        public void UpdateChildTasksWhenParentTaskDateChange(int id)
        {
            var currentTask = db.Tasks.Find(id);
            if (currentTask != null)
            {
                var childTasks = db.Tasks.Where(t => t.ParentID == id).OrderBy(t => t.OrderID).ToList();
                if (childTasks.Any())
                {
                    foreach (var child in childTasks)
                    {
                        var duration = child.End - child.Start;

                        child.Start = currentTask.Start;
                        child.End = BusinessDaysCalculator.GetCalculatedEndDate(child.Start, false, child.ActualDuration, child.WorkItemDuration.ProjectInformationId);
                        //child.Start.Add(duration);

                        db.Tasks.Attach(child);
                        db.Entry(child).State = EntityState.Modified;
                        db.SaveChanges();

                        if (TaskHasChildren(child.ID))
                        {
                            UpdateChildTasksWhenParentTaskDateChange(child.ID);
                        }
                        if (TaskHasSuccessor(child.ID))
                        {
                            UpdateTaskSuccssorWhenTaskIsModified(child.ID, null);
                        }

                    }
                    #region tried to maintain the exact arrangement of child tasks
                    //else
                    //{
                    //    var childTaskArray = childTasks.ToArray();
                    //    for (int i = 0; i < childTasks.Count(); i++)
                    //    {
                    //        var taskToModify = childTaskArray[i];
                    //        // capture the values before any changes are made to the
                    //        // task at the ith index
                    //        //var ithStartBeforeModification = taskToModify.Start;
                    //        //var ithEndBeforeModification = taskToModify.End;
                    //        var duration = taskToModify.End - taskToModify.Start;

                    //        if(i == 0)// first one
                    //        {
                    //            taskToModify.Start = currentTask.Start;
                    //            taskToModify.End = taskToModify.Start.Add(duration);

                    //            db.Tasks.Attach(taskToModify);
                    //            db.Entry(taskToModify).State = EntityState.Modified;
                    //            db.SaveChanges();
                    //        }
                    //        else // check the difference between the ith and i-1th
                    //        {
                    //            var prevTask = childTasks.FirstOrDefault(t => t.ID == childTaskArray[i - 1].ID);
                    //            if(prevTask != null)
                    //            {
                    //                var difference = taskToModify.Start - prevTask.End;

                    //                taskToModify.Start = childTaskArray[i - 1].End.Add(difference);
                    //                taskToModify.End = taskToModify.Start.Add(duration);

                    //                db.Tasks.Attach(taskToModify);
                    //                db.Entry(taskToModify).State = EntityState.Modified;
                    //                db.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //} 
                    #endregion
                }
            }
        }
        public bool TaskHasParent(int taskId)
        {
            var hasParent = false;
            var currentTask = db.Tasks.Find(taskId);
            if (currentTask != null)
            {
                var parent = db.Tasks.Find(currentTask.ParentID);
                if (parent != null)
                {
                    hasParent = true;
                }
            }
            return hasParent;
        }
        public bool TaskHasChildren(int taskId)
        {
            var hasChildren = false;
            var currentTask = db.Tasks.Find(taskId);
            if (currentTask != null)
            {
                var childTasks = db.Tasks.Where(t => t.ParentID == currentTask.ID).ToList();
                if (childTasks.Any())
                {
                    hasChildren = true;
                }
            }
            return hasChildren;
        }
        public bool TaskHasSuccessor(int taskId)
        {
            var hasSuccessors = false;
            var currentTask = db.Tasks.Find(taskId);
            if (currentTask != null)
            {
                var succssors = db.Dependencies.Where(d => d.PredecessorID == currentTask.ID).ToList();
                if (succssors.Any())
                {
                    hasSuccessors = true;
                }
            }
            return hasSuccessors;
        }
        #endregion
        public void Dispose()
        {
            db.Dispose();
        }
    }
}