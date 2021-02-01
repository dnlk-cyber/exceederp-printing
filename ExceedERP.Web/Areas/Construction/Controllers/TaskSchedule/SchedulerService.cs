using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using ExceedERP.Helpers.Construction;
//using System.Globalization;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public class TaskService
    {
        private ExceedDbContext db;
        private DependencyService depservice;
        public TaskService(ExceedDbContext context)
        {
            db = context;
            depservice = new DependencyService();
        }

        public TaskService()
            : this(new ExceedDbContext())
        {
            depservice = new DependencyService();
        }

        public virtual IQueryable<TaskViewModel> GetAll()
        {
            var dependencyService = new DependencyService();
            return db.Tasks.ToList().Select(task => new TaskViewModel
            {
                TaskID = task.ID,
                Title = task.Title,
                Version = task.Version,

                //WorkItemDurationId = task.WorkItemDurationId,
                EstimatedDuration = task.EstimatedDuration,

                //TaskCode = string.IsNullOrEmpty(task.TaskCode) ? "T00" + task.ID : task.TaskCode,
                Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),
                //Start = task.Start,
                //End = task.End,
                BudgetCost = task.BudgetCost,
                DirectCost = task.DirectCost,
                EquipmentCost = task.EquipmentCost,
                LabourCost = task.LabourCost,
                MaterialCost = task.MaterialCost,
                TotalQuantityOfWork = task.TotalQuantityOfWork,

                ParentID = task.ParentID,
                PercentComplete = task.PercentComplete,
                OrderId = task.OrderID,
                Expanded = task.Expanded,
                Summary = task.Summary,
                
                WorkItemDurationId = task.WorkItemDurationId,
                WorkCategoryId = task.WorkCategoryId,
                ConfigurationId = task.ConfigurationId,
                SubProjectTypeId = task.SubProjectTypeId,
                BlockId = task.BlockId,
                ProjectInformationId = task.WorkItemDuration.ProjectInformationId,
                ProjectSpecificId = task.ProjectSpecificId,

                WorkCategory = task.WorkCategory,
                StructureyType = task.StructureyType,
                BlockName = task.BlockName,
                SubProjectTypeName = task.SubProjectTypeName,                //ActualDuration = BusinessDaysCalculator.Calculate(task.Start, task.End),
                ActualDuration = task.ActualDuration,
                //RemainingDuration = BusinessDaysCalculator.Calculate(task.Start > DateTime.Now ? task.Start : DateTime.Now, task.End),
                Predecessors = dependencyService.GetTaskPredecessors(task.ID),

                ExecutedQuantityTillPrevVersion = task.ExecutedQuantityTillPrevVersion,
                ExecutedQuantityThisVersion = task.ExecutedQuantityThisVersion,
                ExecutedQuantityToDate = task.ExecutedQuantityToDate,
                ExecutedAmountTillPrevVersion = task.ExecutedAmountTillPrevVersion,
                ExecutedAmountThisVersion = task.ExecutedAmountThisVersion,
                ExecutedAmountToDate = task.ExecutedAmountToDate,
            }).AsQueryable();
        }
        // list of tasks for the diagram wiget
        public virtual List<DiagramViewModel> GetDiagramTasks()
        {

            return db.Tasks.ToList().Select(task => new DiagramViewModel
            {
                TaskID = task.ID,
                Title = task.Title,
            }).ToList();
        }

        public virtual IQueryable<TaskViewModel> GetTasks(int projectId)
        {
            var dependencyService = new DependencyService();

            return db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == projectId).ToList().Select(task => new TaskViewModel
            {
                TaskID = task.ID,
                Title = task.Title,
                Version = task.Version,

                //WorkItemDurationId = task.WorkItemDurationId,
                EstimatedDuration = task.EstimatedDuration,
                ActualDuration = task.ActualDuration,


                //TaskCode = string.IsNullOrEmpty(task.TaskCode) ? "T00" + task.ID : task.TaskCode,
                Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),

                BudgetCost = task.BudgetCost,
                DirectCost = task.DirectCost,
                EquipmentCost = task.EquipmentCost,
                LabourCost = task.LabourCost,
                MaterialCost = task.MaterialCost,
                TotalQuantityOfWork = task.TotalQuantityOfWork,

                BlockId = task.BlockId,
                WorkItemDurationId = task.WorkItemDurationId,
                WorkCategoryId = task.WorkCategoryId,
                ConfigurationId = task.ConfigurationId,
                SubProjectTypeId = task.SubProjectTypeId,

                ProjectSpecificId = task.ProjectSpecificId,
                ProjectInformationId = task.WorkItemDuration.ProjectInformationId,
                ParentID = task.ParentID,
                PercentComplete = task.PercentComplete,
                OrderId = task.OrderID,
                Expanded = task.Expanded,
                Summary = task.Summary,
                Predecessors = dependencyService.GetTaskPredecessors(task.ID),

                WorkCategory = task.WorkCategory,
                StructureyType = task.StructureyType,
                BlockName = task.BlockName,
                SubProjectTypeName = task.SubProjectTypeName,

                ExecutedQuantityTillPrevVersion = task.ExecutedQuantityTillPrevVersion,
                ExecutedQuantityThisVersion = task.ExecutedQuantityThisVersion,
                ExecutedQuantityToDate = task.ExecutedQuantityToDate,
                ExecutedAmountTillPrevVersion = task.ExecutedAmountTillPrevVersion,
                ExecutedAmountThisVersion = task.ExecutedAmountThisVersion,
                ExecutedAmountToDate = task.ExecutedAmountToDate,
            }).AsQueryable();
        }
        public virtual IQueryable<TaskViewModel> GetFilteredTasks(int projectId, int version, int blockTypeId, int blockId, int strTypeId, int categoryId)
        {
            List<Task> listToReturn = new List<Task>();

            var dependencyService = new DependencyService();

            var allTasks = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == projectId && t.Version == version).ToList();
            if(blockTypeId != 0)
            {
                if(blockId != 0)
                {
                    if(strTypeId != 0)
                    {
                        if(categoryId != 0)
                        {
                            listToReturn = allTasks.Where(t => t.SubProjectTypeId == blockTypeId && t.BlockId == blockId && t.ConfigurationId == strTypeId && t.WorkCategoryId == categoryId).ToList();

                        }
                        else
                        {
                            listToReturn = allTasks.Where(t => t.SubProjectTypeId == blockTypeId && t.BlockId == blockId && t.ConfigurationId == strTypeId).ToList();

                        }
                    }
                    else
                    {
                        listToReturn = allTasks.Where(t => t.SubProjectTypeId == blockTypeId && t.BlockId == blockId).ToList();
                    }
                }
                else
                {
                    listToReturn = allTasks.Where(t => t.SubProjectTypeId == blockTypeId).ToList();
                }
            }
            else
            {
                listToReturn = allTasks;
            }
            return listToReturn.Select(task => new TaskViewModel
            {
                TaskID = task.ID,
                Title = task.Title,
                Version = task.Version,

                //WorkItemDurationId = task.WorkItemDurationId,
                EstimatedDuration = task.EstimatedDuration,
                ActualDuration = task.ActualDuration,


                //TaskCode = string.IsNullOrEmpty(task.TaskCode) ? "T00" + task.ID : task.TaskCode,
                Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),

                BudgetCost = task.BudgetCost,
                DirectCost = task.DirectCost,
                EquipmentCost = task.EquipmentCost,
                LabourCost = task.LabourCost,
                MaterialCost = task.MaterialCost,
                TotalQuantityOfWork = task.TotalQuantityOfWork,

                BlockId = task.BlockId,
                WorkItemDurationId = task.WorkItemDurationId,
                WorkCategoryId = task.WorkCategoryId,
                ConfigurationId = task.ConfigurationId,
                SubProjectTypeId = task.SubProjectTypeId,

                ProjectSpecificId = task.ProjectSpecificId,
                ProjectInformationId = task.WorkItemDuration.ProjectInformationId,
                ParentID = task.ParentID,
                PercentComplete = task.PercentComplete,
                OrderId = task.OrderID,
                Expanded = task.Expanded,
                Summary = task.Summary,
                Predecessors = dependencyService.GetTaskPredecessors(task.ID),

                WorkCategory = task.WorkCategory,
                StructureyType = task.StructureyType,
                BlockName = task.BlockName,
                SubProjectTypeName = task.SubProjectTypeName,

                ExecutedQuantityTillPrevVersion = task.ExecutedQuantityTillPrevVersion,
                ExecutedQuantityThisVersion = task.ExecutedQuantityThisVersion,
                ExecutedQuantityToDate = task.ExecutedQuantityToDate,
                ExecutedAmountTillPrevVersion = task.ExecutedAmountTillPrevVersion,
                ExecutedAmountThisVersion = task.ExecutedAmountThisVersion,
                ExecutedAmountToDate = task.ExecutedAmountToDate,
            }).AsQueryable();
        }
        public virtual IQueryable<TaskViewModel> GetTaskPredecessors()
        {
            return db.Tasks.ToList().Select(task => new TaskViewModel
            {
                TaskID = task.ID,
                Title = task.Title
                //Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                //End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),
                //ParentID = task.ParentID,
                //PercentComplete = task.PercentComplete,
                //OrderId = task.OrderID,
                //Expanded = task.Expanded,
                //Summary = task.Summary,
                //ProjectWorkBreakDownId = task.ProjectWorkBreakDownId,
                ////ProjectWorkBreakDown = task.ProjectWorkBreakDown,
                //ProjectInformationId = task.ProjectWorkBreakDown.ProjectInformationId,
                //OriginalDuration = BusinessDaysCalculator.Calculate(task.Start, task.End),
                //RemainingDuration = BusinessDaysCalculator.Calculate(DateTime.Now, task.End),
                //ProjectWorkBreakDownCode = task.ProjectWorkBreakDown.WBSCode
            }).AsQueryable();
        }

        //public JsonResult GetAllCandidatePredecessors()
        //{
        //    var items = db.Tasks;
        //    //var items = db.ProjectInformation .Where (X=> X.ProjectStatus == ProjectStatus.APPROVED || X.ProjectStatus == ProjectStatus.STARTED);
        //    return Json(items.Select(p => new { TaskID = p.ID, Title = p.Title})).AsQueryable();
        //}

        public virtual IQueryable<TaskViewModel> GetTasksByProjectId(int projectId)
        {

            return db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == projectId).ToList().Select(task => new TaskViewModel
            {
                TaskID = task.ID,
                Title = task.Title,
                ProjectSpecificId = task.ProjectSpecificId,
                Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),
                ParentID = task.ParentID,
                PercentComplete = task.PercentComplete,
                OrderId = task.OrderID,
                Expanded = task.Expanded,
                Summary = task.Summary,
                //WorkItemDurationId = task.WorkItemDurationId,
                WorkItemDurationId = task.WorkItemDurationId,

            }).AsQueryable();
        }
        public virtual IQueryable<TaskViewModel> GetChildTasks(int? taskId)
        {
            return db.Tasks.Where(t => t.ParentID == taskId).ToList().Select(task => new TaskViewModel
            {
                TaskID = task.ID,
                Title = task.Title,
                Version = task.Version,

                WorkItemDurationId = task.WorkItemDurationId,
                EstimatedDuration = task.EstimatedDuration,
                ActualDuration = task.ActualDuration,


                Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),

                BudgetCost = task.BudgetCost,
                DirectCost = task.DirectCost,
                EquipmentCost = task.EquipmentCost,
                LabourCost = task.LabourCost,
                MaterialCost = task.MaterialCost,
                TotalQuantityOfWork = task.TotalQuantityOfWork,
                BlockId = task.BlockId,

                ParentID = task.ParentID,
                PercentComplete = task.PercentComplete,
                OrderId = task.OrderID,
                Expanded = task.Expanded,
                Summary = task.Summary,
                ProjectInformationId = task.WorkItemDuration.ProjectInformationId,
                ProjectSpecificId = task.ProjectSpecificId,

                WorkCategory = task.WorkCategory,
                StructureyType = task.StructureyType,
                BlockName = task.BlockName,
                SubProjectTypeName = task.SubProjectTypeName,

            }).AsQueryable();
        }
        public virtual void Insert(TaskViewModel task, ModelStateDictionary modelState, int? selectedTask)
        {
            //int targetProjectWorkBreakdownId = 0;
            var defaultProjectWorkBreakdown = db.ProjectWorkBreakDown.FirstOrDefault();
            var defaultWorkItemDuration = db.WorkItemDuration.FirstOrDefault();
            if (selectedTask != null)
            {

                //if (selectedTask == -1)
                //{
                //    targetProjectWorkBreakdownId = defaultProjectWorkBreakdown.ProjectWorkBreakDownId;
                //}
                //else
                //{
                //    //var foundProjectWorkBreakdown = db.ProjectWorkBreakDown.Find(selectedTask);
                //    targetProjectWorkBreakdownId = db.Tasks.Find(selectedTask).ProjectWorkBreakDownId;
                //}
            }
            if (ValidateModel(task, modelState))
            {
                if (string.IsNullOrEmpty(task.Title))
                {
                    task.Title = "";
                }
                var newTask = new Task
                {
                    ID = task.TaskID,
                    //WorkItemDurationId = task.WorkItemDurationId,
                    //WorkItemDurationId = defaultWorkItemDuration.WorkItemDurationId,
                    WorkItemDurationId = task.WorkItemDurationId,
                    //WorkItemDuration = defaultWorkItemDuration,
                    OrderID = task.OrderId,
                    ProjectSpecificId = task.ProjectSpecificId,
                    Title = defaultWorkItemDuration.WorkDescription,
                    //Title = task.Title,
                    Start = task.Start,
                    End = task.End,
                    Expanded = task.Expanded,
                    ParentID = task.ParentID,
                    PercentComplete = task.PercentComplete,
                    // ProjectWorkBreakDownId = targetProjectWorkBreakdownId,
                    //ProjectWorkBreakDown = db.ProjectWorkBreakDown.Find(targetProjectWorkBreakdownId),
                    Summary = task.Summary
                };
                var entity = task.ToEntity();

                db.Tasks.Add(newTask);
                db.SaveChanges();

                task.TaskID = entity.ID;
            }
        }
        public virtual void InsertPerProjectBreakdown(TaskViewModel task, ModelStateDictionary modelState, int pwbIdInsert)
        {
            if (ValidateModel(task, modelState))
            {
                ProjectWorkBreakDown pwb = db.ProjectWorkBreakDown.Find(pwbIdInsert);
                if (string.IsNullOrEmpty(task.Title))
                {
                    task.Title = "temp task";
                }
                var newTask = new Task();
                var selectedParentId = task.ParentID;
                if (selectedParentId == null)
                {
                    newTask = new Task
                    {
                        ID = task.TaskID,
                        //OrderID = this.SetWorkItemOrderInSchedule(task.WorkItemDurationId),
                        Title = task.Title,
                        ProjectSpecificId = task.ProjectSpecificId,
                        Start = task.Start,
                        End = task.End,
                        Expanded = task.Expanded,
                        ParentID = task.ParentID,
                        PercentComplete = task.PercentComplete,
                        //ProjectWorkBreakDownId = pwbIdInsert,
                        //ProjectWorkBreakDown = pwb,
                        Version = 1,
                        Summary = task.Summary,

                        //WorkItemDurationId = task.WorkItemDurationId,
                        WorkItemDurationId = task.WorkItemDurationId,

                    };
                }
                else
                {
                    var parentTask = db.Tasks.Find(selectedParentId);
                    if (parentTask != null)
                    {
                        newTask = new Task
                        {
                            ID = task.TaskID,
                            // OrderID = this.SetWorkItemOrderInSchedule(task.WorkItemDurationId),
                            Title = task.Title,
                            ProjectSpecificId = task.ProjectSpecificId,
                            Start = parentTask.Start,
                            End = parentTask.End,
                            Expanded = task.Expanded,
                            ParentID = task.ParentID,
                            PercentComplete = task.PercentComplete,
                            //ProjectWorkBreakDownId = pwbIdInsert,
                            //ProjectWorkBreakDown = pwb,
                            Version = 1,
                            Summary = task.Summary,
                            //WorkItemDurationId = task.WorkItemDurationId,
                            WorkItemDurationId = task.WorkItemDurationId,

                        };
                        parentTask.Summary = true;
                        parentTask.Expanded = true;
                    }
                }

                //task.ProjectWorkBreakDownId = pwbIdInsert;
                var entity = task.ToEntity();

                db.Tasks.Add(newTask);
                db.SaveChanges();

                task.TaskID = entity.ID;
            }
        }
        public virtual void CreateSubActivity(TaskViewModel task, ModelStateDictionary modelState, int parentId)
        {
            var parentTask = db.Tasks.Find(parentId);
            if (parentTask != null)
            {
                var hasValidationErrors = false;

                var entity = task.ToEntity();
                entity.OrderID = parentTask.OrderID + 1;
                entity.ParentID = parentId;

                entity.End = entity.Start.AddDays((int)entity.ActualDuration);


                entity.WorkItemDurationId = parentTask.WorkItemDurationId;
                entity.WorkCategoryId = parentTask.WorkCategoryId;
                entity.ConfigurationId = parentTask.ConfigurationId;
                entity.BlockId = parentTask.BlockId;
                entity.SubProjectTypeId = parentTask.SubProjectTypeId;

                entity.SubProjectTypeName = parentTask.SubProjectTypeName;
                entity.BlockName = parentTask.BlockName;
                entity.StructureyType = parentTask.StructureyType;
                entity.WorkCategory = parentTask.WorkCategory;

                //entity.TotalQuantityOfWork = childWI.TotalQuantityOfWork;
                //entity.BudgetCost = ProjectEstimationHelper.GetWorkItemBudgetedCost(childWI.WorkItemPerProjectId);
                //entity.DirectCost = childWI.DirectCost;
                //entity.MaterialCost = childWI.MaterialCost;
                //entity.EquipmentCost = childWI.EquipmentOutput;
                //entity.LabourCost = childWI.LabourOutPut;
                entity.Version = parentTask.Version;
                entity.IsSubActivityCreatedInSchedule = true;
                //entity.ActualDuration = childTaskToAdd.EstimatedDuration;

                if (entity.Start < parentTask.Start || entity.Start > parentTask.End)
                {
                    modelState.AddModelError("", "Start Date Must be within range");
                    hasValidationErrors = true;
                }

                if (!hasValidationErrors)
                {
                    db.Tasks.Add(entity);
                    db.SaveChanges();

                    if (entity.End > parentTask.End)
                    {
                        parentTask.End = entity.End;
                    }
                    parentTask.Summary = true;
                    parentTask.Expanded = true;
                    db.Tasks.Attach(parentTask);
                    db.Entry(parentTask).State = EntityState.Modified;
                    db.SaveChanges();

                    if (depservice.TaskHasParent(parentTask.ID))
                    {
                        depservice.UpdateParentTaskWhenChildTaskDatesChange(parentTask.ParentID ?? 0);
                    }
                    if (depservice.TaskHasSuccessor(parentTask.ID))
                    {
                        depservice.UpdateTaskSuccssorWhenTaskIsModified(parentTask.ID, modelState);
                    }

                }


                //if (depservice.TaskHasChildren(entity.ID))
                //{
                //    depservice.UpdateChildTasksWhenParentTaskDateChange(entity.ID);
                //}

                //rearange ordering
                var tasksWithHigherOrder = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == parentTask.WorkItemDuration.ProjectInformationId &&
                t.OrderID >= parentTask.OrderID).ToList();
                if (tasksWithHigherOrder.Any())
                {
                    foreach (Task t in tasksWithHigherOrder)
                    {
                        t.OrderID = t.OrderID + 1;

                        db.Tasks.Attach(t);
                        db.Entry(t).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

        }
        public virtual TaskViewModel Update(TaskViewModel task, ModelStateDictionary modelState)
        {
            if (ValidateModel(task, modelState))
            {
                //WorkItemDuration duration = new WorkItemDuration();
                //if (task.WorkItemDurationId != null)
                //{
                //    var workItemDuration = db.WorkItemDuration.Find(task.WorkItemDurationId);
                //    if (workItemDuration != null)
                //    {
                //        duration = workItemDuration;
                //    }

                //}
                //task.Title = string.IsNullOrEmpty(duration.WorkDescription) ? task.Title : duration.WorkDescription;
                //var existingTasks = db.Tasks.Where(t => t.WorkItemDurationId == task.WorkItemDurationId).ToList();
                //if (existingTasks != null && existingTasks.Count() > 0)
                //{
                //    modelState.AddModelError("name_exists", "this work item name already used");
                //}
                //else
                //{
                //    task.Title = duration.WorkDescription;
                //}

                if (string.IsNullOrEmpty(task.Title))
                {
                    task.Title = "";
                }

                var entity = task.ToEntity();
                var workItem = db.WorkItemDuration.Find(task.WorkItemDurationId);
                if(workItem != null)
                {
                  entity.End =  BusinessDaysCalculator.GetCalculatedEndDate(entity.Start, false, entity.ActualDuration, workItem.ProjectInformationId);
                }
                //entity.End = entity.Start.AddDays((int)entity.ActualDuration);

                db.Tasks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                if (depservice.TaskHasParent(entity.ID))
                {
                    depservice.UpdateParentTaskWhenChildTaskDatesChange(entity.ParentID ?? 0);
                }
                if (depservice.TaskHasChildren(entity.ID))
                {
                    depservice.UpdateChildTasksWhenParentTaskDateChange(entity.ID);
                }
                if (depservice.TaskHasSuccessor(entity.ID))
                {
                    depservice.UpdateTaskSuccssorWhenTaskIsModified(entity.ID, modelState);
                }
                return new TaskViewModel
                {
                    TaskID = entity.ID,
                    Title = entity.Title,
                    Version = entity.Version,

                    //WorkItemDurationId = task.WorkItemDurationId,
                    EstimatedDuration = entity.EstimatedDuration,

                    //TaskCode = string.IsNullOrEmpty(task.TaskCode) ? "T00" + task.ID : task.TaskCode,
                    Start = DateTime.SpecifyKind(entity.Start, DateTimeKind.Utc),
                    End = DateTime.SpecifyKind(entity.End, DateTimeKind.Utc),
                    //Start = task.Start,
                    //End = task.End,
                    BudgetCost = entity.BudgetCost,
                    DirectCost = entity.DirectCost,
                    EquipmentCost = entity.EquipmentCost,
                    LabourCost = entity.LabourCost,
                    MaterialCost = entity.MaterialCost,
                    TotalQuantityOfWork = entity.TotalQuantityOfWork,

                    ParentID = entity.ParentID,
                    PercentComplete = entity.PercentComplete,
                    OrderId = entity.OrderID,
                    Expanded = entity.Expanded,
                    Summary = entity.Summary,

                    WorkItemDurationId = entity.WorkItemDurationId,
                    WorkCategoryId = entity.WorkCategoryId,
                    ConfigurationId = entity.ConfigurationId,
                    SubProjectTypeId = entity.SubProjectTypeId,
                    BlockId = entity.BlockId,
                    ProjectInformationId = entity.WorkItemDuration.ProjectInformationId,
                    ProjectSpecificId = entity.ProjectSpecificId,

                    WorkCategory = entity.WorkCategory,
                    StructureyType = entity.StructureyType,
                    BlockName = entity.BlockName,
                    SubProjectTypeName = entity.SubProjectTypeName,                //ActualDuration = BusinessDaysCalculator.Calculate(task.Start, task.End),
                    ActualDuration = entity.ActualDuration,
                    //RemainingDuration = BusinessDaysCalculator.Calculate(task.Start > DateTime.Now ? task.Start : DateTime.Now, task.End),
                    Predecessors = depservice.GetTaskPredecessors(entity.ID),

                    ExecutedQuantityTillPrevVersion = entity.ExecutedQuantityTillPrevVersion,
                    ExecutedQuantityThisVersion = entity.ExecutedQuantityThisVersion,
                    ExecutedQuantityToDate = entity.ExecutedQuantityToDate,
                    ExecutedAmountTillPrevVersion = entity.ExecutedAmountTillPrevVersion,
                    ExecutedAmountThisVersion = entity.ExecutedAmountThisVersion,
                    ExecutedAmountToDate = entity.ExecutedAmountToDate,
                };
            }
            return task;
        }

        public virtual void Delete(TaskViewModel task, ModelStateDictionary modelState)
        {
            var entity = task.ToEntity();
            db.Tasks.Attach(entity);

            Delete(entity);

            db.Tasks.Remove(entity);
            db.SaveChanges();
        }

        public virtual IQueryable<ProjectInformation> GetAllProjects()
        {
            //IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.AsQueryable();//.Where (x=> x.ProjectStatus == ProjectStatus .STARTED );
            return db.ProjectInformation.ToList().Select(projectInformation => new ProjectInformation
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                ProjectType = projectInformation.ProjectType,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                ProjectStatus = projectInformation.ProjectStatus,

                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
            }).AsQueryable();
        }
        private void Delete(Task task)
        {
            var childTasks = db.Tasks.Where(t => t.ParentID == task.ID);

            foreach (var childTask in childTasks)
            {
                Delete(childTask);
                db.Tasks.Remove(childTask);
            }
        }

        private bool ValidateModel(TaskViewModel task, ModelStateDictionary modelState)
        {
            if (task.Start > task.End)
            {
                modelState.AddModelError("errors", "End date must be greater or equal to Start date.");
                return false;
            }

            return true;
        }

        public int CalculateDifference(DateTime date1, DateTime date2)
        {
            var daysCount = 0;
            if (date1 < date2)
            {
                TimeSpan ts = date2 - date1;
                daysCount = (int)ts.TotalDays;
            }
            return daysCount;
        }
        private decimal GetEstimatedDuration(int? id)
        {
            var duration = new decimal();
            if (id != null)
            {
                var temp = db.WorkItemDuration.Find(id);
                if (temp != null)
                {
                    duration = temp.EstimatedDuration;
                    //if (temp.WorkItemPerProject.IsSubCategory)
                    //{
                    //    var childWorkItemDuration =
                    //        db.WorkItemDuration.Where(wd => wd.WorkItemPerProject.ParentId == temp.WorkItemPerProjectId)
                    //        .ToList();
                    //    if (childWorkItemDuration.Any())
                    //    {
                    //        duration = childWorkItemDuration.Max(wd => wd.EstimatedDuration);
                    //    }
                    //}
                }
            }

            return duration;
        }
        private int GetTaskBlockId(Task record)
        {
            var id = 0;
            //var durationWorkItem = db.WorkItemDuration.FirstOrDefault(wd => wd.WorkItemDurationId == record.WorkItemDurationId);
            //if (durationWorkItem != null)
            //{
            //    var workItem =
            //            db.WorkItemPerProject
            //            .FirstOrDefault(wi => wi.WorkItemPerProjectId == durationWorkItem.WorkItemPerProjectId);
            //    if (workItem != null)
            //    {
            //        if (workItem.IsSubCategory)
            //        {
            //            id = workItem.BlockId ?? 0;
            //        }
            //        else
            //        {
            //            if (workItem.BlockId == null)
            //            {
            //                // find the parent and use that block id
            //                var parent = db.WorkItemPerProject.FirstOrDefault(w => w.WorkItemPerProjectId == workItem.ParentId);
            //                if (parent != null)
            //                {
            //                    id = parent.BlockId ?? 0;
            //                }
            //            }
            //            else
            //            {
            //                id = (int)workItem.BlockId;
            //            }
            //        }
            //    }
            //}
            return id;
        }
        private int SetWorkItemOrderInSchedule(int? workItemDurationId)
        {
            int order = 0;
            //if (workItemDurationId != null)
            //{
            //    var currentWorkDuration = db.WorkItemDuration.Find(workItemDurationId);
            //    if (currentWorkDuration != null)
            //    {
            //        var currentProject = db.ProjectInformation.Find(currentWorkDuration.WorkItemPerProject.ProjectInformationId);
            //        if (currentProject != null)
            //        {
            //            var availableTasks = db.Tasks.Where(t => t.ProjectWorkBreakDown.ProjectInformationId == currentProject.ProjectInformationId);
            //            if (availableTasks.Any())
            //            {
            //                order = availableTasks.Max(task => task.OrderID) + 1;
            //            }
            //        }
            //    }
            //}
            return order;
        }

        public void PrepareScheduleBreakDown()
        {
            var allScheduleTasks = db.Tasks.ToList();
            if (allScheduleTasks.Any())
            {
                foreach (Task task in allScheduleTasks)
                {
                    DateTime start = task.Start;
                    DateTime end = task.End;

                    var periodsInTimeRange = db.ConstructionPeriod.Where(p => p.DateFrom <= start && p.DateTo >= start ||
                                                                              p.DateFrom >= start && p.DateTo <= end ||
                                                                              p.DateFrom <= end && p.DateTo >= end
                                                                        ).ToList();
                    if (periodsInTimeRange.Any() && task.WorkItemDuration != null)
                    {
                        var numOfPeriods = periodsInTimeRange.Count;
                        var workItem = task.WorkItemDuration;

                        foreach (ConstructionPeriod period in periodsInTimeRange)
                        {
                            var subPeriods = db.ConstructionSubPeriod.Where(sp => sp.ConstructionPeriodId == period.ConstructionPeriodId).ToList();
                            var existingItem = db.TaskDetail.FirstOrDefault(td => td.ID == task.ID && td.ConstructionPeriodId == period.ConstructionPeriodId);
                            if (existingItem == null)
                            {
                                var entity = new TaskDetail
                                {
                                    ConstructionPeriodId = period.ConstructionPeriodId,
                                    ID = task.ID,
                                    PlannedQuantityOfWork = task.TotalQuantityOfWork / numOfPeriods,
                                    //PlannedMaterialCost = task.MaterialCost / numOfPeriods,
                                    //PlannedEquipmentCost = task.EquipmentCost / numOfPeriods,
                                    //PlannedLaborCost = task.LabourCost / numOfPeriods,
                                };
                                entity.PlannedMaterialCost = entity.PlannedQuantityOfWork * workItem.MaterialCost;
                                entity.PlannedEquipmentCost = entity.PlannedQuantityOfWork * workItem.EquipmentCost;
                                entity.PlannedLaborCost = entity.PlannedQuantityOfWork * workItem.LabourCost;
                                entity.PlannedProfitCost = entity.PlannedQuantityOfWork * workItem.ProfitCost;
                                entity.PlannedOverHeadCost = entity.PlannedQuantityOfWork * workItem.OverHeadCost;

                                entity.PlannedDirectCost = entity.PlannedMaterialCost + entity.PlannedEquipmentCost + entity.PlannedLaborCost;
                                entity.TotalCost = entity.PlannedDirectCost + entity.PlannedProfitCost + entity.PlannedOverHeadCost;

                                db.TaskDetail.Add(entity);
                                db.SaveChanges();

                                #region further break down
                                if (subPeriods.Any())
                                {
                                    var numOfSubPeriods = subPeriods.Count;
                                    foreach (ConstructionSubPeriod subPeriod in subPeriods)
                                    {
                                        var existingSubPeriodData = db.TaskDetailBreakDown
                                            .FirstOrDefault(b => b.TaskDetailId == entity.TaskDetailId &&
                                            b.ConstructionSubPeriodId == subPeriod.ConstructionSubPeriodId);
                                        if (existingSubPeriodData == null)
                                        {
                                            var subPeriodDataToAdd = new TaskDetailBreakDown
                                            {
                                                TaskDetailId = entity.TaskDetailId,
                                                ConstructionSubPeriodId = subPeriod.ConstructionSubPeriodId,
                                                PlannedQuantityOfWork = entity.PlannedQuantityOfWork / numOfSubPeriods,
                                                //PlannedMaterialCost = entity.PlannedMaterialCost / numOfSubPeriods,
                                                //PlannedEquipmentCost = entity.PlannedEquipmentCost / numOfSubPeriods,
                                                //PlannedLaborCost = entity.PlannedLaborCost / numOfSubPeriods,

                                            };
                                            subPeriodDataToAdd.PlannedMaterialCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.MaterialCost;
                                            subPeriodDataToAdd.PlannedLaborCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.LabourCost;
                                            subPeriodDataToAdd.PlannedEquipmentCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.EquipmentCost;
                                            subPeriodDataToAdd.PlannedProfitCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.ProfitCost;
                                            subPeriodDataToAdd.PlannedOverHeadCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.OverHeadCost;

                                            subPeriodDataToAdd.PlannedDirectCost = subPeriodDataToAdd.PlannedMaterialCost + subPeriodDataToAdd.PlannedEquipmentCost + subPeriodDataToAdd.PlannedLaborCost;
                                            subPeriodDataToAdd.TotalCost = subPeriodDataToAdd.PlannedDirectCost + subPeriodDataToAdd.PlannedProfitCost + subPeriodDataToAdd.PlannedOverHeadCost;

                                            db.TaskDetailBreakDown.Add(subPeriodDataToAdd);
                                            db.SaveChanges();
                                        }
                                    }
                                } 
                                #endregion
                            }

                        }
                    }
                }
            }
        }
        public void UpdateTaskData()
        {
            var tasks = db.Tasks.ToList();
            if (tasks.Any())
            {
                foreach (Task task in tasks)
                {
                    task.BlockId = this.GetTaskBlockId(task);
                    //task.EstimatedDuration = this.GetEstimatedDuration(task.WorkItemDurationId);
                    task.ActualDuration = task.EstimatedDuration;

                    db.Tasks.Attach(task);
                    db.Entry(task).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}