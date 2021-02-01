using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    [AuthorizeRoles(ConstructionConstants.SchedulingUser)]
    public class SchedulerController : Controller
    {
        private ExceedDbContext db;
        private TaskService taskService;
        private DependencyService dependencyService;
        private AssignmentService assignmentService;
        private ResourceService resourceService;
        //public int projectId;
        //public int version;
        public SchedulerController()
        {
            db = new ExceedDbContext();
            taskService = new TaskService();
            dependencyService = new DependencyService();
            assignmentService = new AssignmentService();
            resourceService = new ResourceService();

            //this.projectId = 0;
            //this.version = 0;
        }

        protected override void Dispose(bool disposing)
        {
            taskService.Dispose();
            dependencyService.Dispose();

            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            ViewData["Periods"] = db.ConstructionPeriod.Select(p => new { p.ConstructionPeriodId, p.Name}).ToList();

            ViewData["SubPeriods"] = db.ConstructionSubPeriod.Select(p => new { p.Name, p.ConstructionSubPeriodId}).ToList();


            //this.PrepareScheduleBreakDown();

            return View();
        }

        public ActionResult Resources()
        {
            return View();
        }
        public ActionResult Diagram()
        {
            return View();
        }
        public ActionResult RunBackground()
        {
            //this.UpdateTaskData();

            this.PrepareScheduleBreakDown();
            return Redirect("Index");
        }
        public ActionResult UpdateExistingTasksProjectSpecificId()
        {
            SchedulingHelper.UpdateExistingTasksProjectSpecificId();
            return Redirect("Index");
        }
        //public  void SetSelectedProjectId(int value) {
        //    ProjectId = value;
        //}
        public ActionResult ChangeProjectAndVersion(int projectId, int version)
        {
            //this.projectId = projectId;
            //this.version = version;
            return Redirect("ReadProjectTasksOfSpecificVerion");
        }
        //public  int GetSelectedProjectId()
        //{
        //    return ProjectId;
        //}
        public virtual JsonResult ReadAllProjects([DataSourceRequest] DataSourceRequest request)
        {
            return Json(taskService.GetAllProjects().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        public virtual LargeJsonResult ReadTasks([DataSourceRequest] DataSourceRequest request)
        {
            //return Json(taskService.GetAll().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            //return Json((taskService.GetTasks(this.projectId).Where(t => t.Version == this.version)).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            var collection = taskService.GetAll().ToDataSourceResult(request);

            return new LargeJsonResult
            {
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                Data = collection

            };
        }
        public virtual ActionResult ReadProjectTasksOfSpecificVerion([DataSourceRequest] DataSourceRequest request, int projectId, int version)
        {
            //return Json((taskService.GetTasks(projectId).Where(t => t.Version == version)).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            DataSourceResult collection = (taskService.GetTasks(projectId).Where(t => t.Version == version).OrderBy(t => t.OrderId)).ToDataSourceResult(request);

            return new LargeJsonResult
            {
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                Data = collection

            };
        }
        public virtual ActionResult ReadFilteredTasks([DataSourceRequest] DataSourceRequest request, int projectId, int version, int blockTypeId, int blockId, int strTypeId, int categoryId, int pageNumber, int pageSize)
        {
            request.Page = pageNumber;
            request.PageSize = pageSize;
            //return Json((taskService.GetTasks(projectId).Where(t => t.Version == version)).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            DataSourceResult collection = 
                (taskService.GetFilteredTasks(projectId, version, blockTypeId, blockId, strTypeId, categoryId)
                    .OrderBy(t => t.OrderId)
                ).ToDataSourceResult(request);

            return new LargeJsonResult
            {
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                Data = collection

            };
        }
        public virtual JsonResult ReadTasksPerProject([DataSourceRequest] DataSourceRequest request, int projectId)
        {
            return Json(taskService.GetTasks(projectId).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public LargeJsonResult ReadAllTasksForExport([DataSourceRequest] DataSourceRequest request, int projectId)
        {
            var model = taskService.GetTasks(projectId).ToDataSourceResult(request);

            return new LargeJsonResult
            {
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                Data = model

            };
        }
        public virtual JsonResult ReadTasksPerProjectAndVersion([DataSourceRequest] DataSourceRequest request, int projectId, int versionNo)
        {
            return Json((taskService.GetTasks(projectId).Where(t => t.Version == versionNo)).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReadTasksToPreceed([DataSourceRequest] DataSourceRequest request)
        {
            return Json(taskService.GetTaskPredecessors().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAllCandidatePredecessors([DataSourceRequest] DataSourceRequest request, int? crntTaskId)
        {
            var db = new ExceedDbContext();

            var items = db.Tasks.AsQueryable();
            if (crntTaskId != null)
            {
                var targetTask = db.Tasks.Find(crntTaskId);
                if (targetTask != null)
                {
                    var targetProject = db.ProjectInformation.Find(targetTask.WorkItemDuration.ProjectInformationId);

                    items = items.Where(task => task.WorkItemDuration.ProjectInformationId == targetProject.ProjectInformationId);
                }

            }
            return Json(items.Select(p => new { TaskID = p.ID, Title = p.Title }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReadPredecessors([DataSourceRequest] DataSourceRequest request, int? theCurrentTaskId, int? activeProjectId)
        {
            return Json(dependencyService.GetPredecessors(theCurrentTaskId, activeProjectId).ToDataSourceResult(request));
        }
        public virtual JsonResult GetChildTasks([DataSourceRequest] DataSourceRequest request, int? theCurrentTaskId)
        {
            return Json(taskService.GetChildTasks(theCurrentTaskId).ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult CreateTask([DataSourceRequest] DataSourceRequest request, TaskViewModel task, int selectedTask)
        {
            if (ModelState.IsValid)
            {
                taskService.Insert(task, ModelState, selectedTask);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult CreateTaskPerProjectBd([DataSourceRequest] DataSourceRequest request, TaskViewModel task, int pwbIdCreate)
        {
            if (ModelState.IsValid)
            {
                taskService.InsertPerProjectBreakdown(task, ModelState, pwbIdCreate);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }
        public virtual JsonResult CreateSubActivity([DataSourceRequest] DataSourceRequest request, TaskViewModel task, int theCurrentTaskId)
        {
            if (ModelState.IsValid)
            {
                taskService.CreateSubActivity(task, ModelState, theCurrentTaskId);
            }
            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }
        public virtual JsonResult UpdateTask([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                var updatedTask = taskService.Update(task, ModelState);

                return Json(new[] { updatedTask }.ToDataSourceResult(request, ModelState));
            }
            //task.End = task.Start.AddDays((int)task.ActualDuration);
            //if(task.Work)
            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }
        public virtual JsonResult DestroyTask([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                taskService.Delete(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        #region Task_Dependencies
        public virtual JsonResult ReadDependencies([DataSourceRequest] DataSourceRequest request, int? taskId)
        {
            if (taskId == null)
            {
                return Json(dependencyService.GetAll().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(dependencyService.GetDependencies(taskId).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

        }
        public virtual JsonResult ReadPredessors([DataSourceRequest] DataSourceRequest request, int? taskId)
        {
            if (taskId == null)
            {
                return null;
            }
            else
            {
                return Json(dependencyService.GetPredecessors(taskId).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }
        public virtual JsonResult ReadSuccessors([DataSourceRequest] DataSourceRequest request, int? taskId)
        {
            if (taskId == null)
            {
                return null;
            }
            else
            {
                return Json(dependencyService.GetSuccessors(taskId).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
        }
        public virtual JsonResult DestroyDependency([DataSourceRequest] DataSourceRequest request, DependencyViewModel dependency)
        {
            if (ModelState.IsValid)
            {
                dependencyService.Delete(dependency, ModelState);
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult CreateDependency([DataSourceRequest] DataSourceRequest request, DependencyViewModel dependency, int? tId)
        {
            if (ModelState.IsValid)
            {
                dependencyService.Insert(dependency, ModelState, tId);
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult CreateTaskDependency([DataSourceRequest] DataSourceRequest request, DependencyViewModel dependency, int? successorId)
        {
            if (ModelState.IsValid)
            {
                dependencyService.InsertTaskPredessor(dependency, ModelState, (int)successorId);
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult CreateTaskPredecessor([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PredecssorViewModel>   predecessors, int targetId)
        {
            if (ModelState.IsValid && predecessors != null)
            {
                foreach (var predecessor in predecessors)
                {
                    dependencyService.UpdateGanttTaskPredecessor(predecessor, ModelState, targetId);
                }
            }
            return Json(predecessors.ToDataSourceResult(request, ModelState));
        }

        #region temporary solution
        public virtual JsonResult AddPredecessor([DataSourceRequest] DataSourceRequest request, DependencyViewModel depVM, int taskId)
        {
            PredecssorViewModel predecessor = new PredecssorViewModel
            {
                Id = depVM.PredecessorID,
                Name = depVM.predecessorTaskname,
                Type = depVM.Type,
                LagTime = depVM.LagTime,
                IsSelected = true
            };
            if (ModelState.IsValid)
            {
                dependencyService.UpdateGanttTaskPredecessor(predecessor, ModelState, taskId);
            }
            return Json(new[] { depVM }.ToDataSourceResult(request, ModelState));
        }
        public virtual JsonResult UpdatePredecessor([DataSourceRequest] DataSourceRequest request, DependencyViewModel depVM, int taskId)
        {
            PredecssorViewModel predecessor = new PredecssorViewModel
            {
                Id = depVM.PredecessorID,
                Name = depVM.predecessorTaskname,
                Type = depVM.Type,
                LagTime = depVM.LagTime,
                IsSelected = true
            };
            if (ModelState.IsValid)
            {
                dependencyService.UpdateGanttTaskPredecessor(predecessor, ModelState, taskId);
            }
            return Json(new[] { depVM }.ToDataSourceResult(request, ModelState));
        }
        public virtual JsonResult DeletePredecessor([DataSourceRequest] DataSourceRequest request, DependencyViewModel depVM, int taskId)
        {
            PredecssorViewModel predecessor = new PredecssorViewModel
            {
                Id = depVM.PredecessorID,
                Name = depVM.predecessorTaskname,
                Type = depVM.Type,
                LagTime = depVM.LagTime,
                IsSelected = false
            };
            if (ModelState.IsValid)
            {
                dependencyService.UpdateGanttTaskPredecessor(predecessor, ModelState, taskId);
            }
            return Json(new[] { depVM }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        public virtual JsonResult UpdateDependency([DataSourceRequest] DataSourceRequest request, DependencyViewModel dependency)
        {
            if (ModelState.IsValid)
            {
                dependencyService.Update(dependency, ModelState);
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }
        #endregion


        #region Task_Resources
        public virtual JsonResult ReadResources([DataSourceRequest] DataSourceRequest request)
        {
            return Json(resourceService.GetAll().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult ReadTaskResources([DataSourceRequest] DataSourceRequest request, int tskId)
        {
            return Json(resourceService.GetTaskResources(tskId).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult CreateResource([DataSourceRequest] DataSourceRequest request, ResourceViewModel resource)
        {
            if (ModelState.IsValid)
            {
                resourceService.Insert(resource);
            }
            return Json(new[] { resource }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult UpdateResource([DataSourceRequest] DataSourceRequest request, ResourceViewModel resource)
        {
            if (ModelState.IsValid)
            {
                resourceService.Update(resource);
            }
            return Json(new[] { resource }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult DestroyResource([DataSourceRequest] DataSourceRequest request, ResourceViewModel resource)
        {
            if (ModelState.IsValid)
            {
                resourceService.Delete(resource);
            }
            return Json(new[] { resource }.ToDataSourceResult(request, ModelState));
        }



        public virtual JsonResult ReadAssignments([DataSourceRequest] DataSourceRequest request)
        {
            return Json(assignmentService.GetAll().ToDataSourceResult(request));
        }

        public virtual JsonResult DestroyAssignment([DataSourceRequest] DataSourceRequest request, ResourceAssignmentViewModel assignment)
        {
            if (ModelState.IsValid)
            {
                assignmentService.Delete(assignment);
            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult CreateAssignment([DataSourceRequest] DataSourceRequest request, ResourceAssignmentViewModel assignment)
        {
            if (ModelState.IsValid)
            {
                assignmentService.Insert(assignment);
            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult UpdateAssignment([DataSourceRequest] DataSourceRequest request, ResourceAssignmentViewModel assignment)
        {
            if (ModelState.IsValid)
            {
                assignmentService.Update(assignment);
            }

            return Json(new[] { assignment }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        private void PrepareScheduleBreakDown()
        {
            taskService.PrepareScheduleBreakDown();
            #region MyRegion
            //var allScheduleTasks = db.Tasks.ToList();
            //if (allScheduleTasks.Any())
            //{
            //    foreach(Task task in allScheduleTasks)
            //    {
            //        DateTime start = task.Start;
            //        DateTime end = task.End;

            //        var periodsInTimeRange = db.ConstructionPeriod.Where(p => p.DateFrom <= start && p.DateTo >= start ||
            //                                                                  p.DateFrom >= start && p.DateTo <= end ||
            //                                                                  p.DateFrom <= end && p.DateTo >= end
            //                                                            ).ToList();
            //        if (periodsInTimeRange.Any())
            //        {
            //            var numOfPeriods = periodsInTimeRange.Count;


            //            foreach(ConstructionPeriod period in periodsInTimeRange)
            //            {
            //                var existingItem = db.TaskDetail.FirstOrDefault(td => td.ID == task.ID && td.ConstructionPeriodId == period.ConstructionPeriodId);
            //                if(existingItem == null)
            //                {
            //                    var entity = new TaskDetail
            //                    {
            //                        ConstructionPeriodId = period.ConstructionPeriodId,
            //                        ID = task.ID,
            //                        PlannedQuantityOfWork = task.TotalQuantityOfWork / numOfPeriods,
            //                        PlannedMaterialCost = task.MaterialCost / numOfPeriods,
            //                        PlannedEquipmentCost = task.EquipmentCost / numOfPeriods,
            //                        PlannedLaborCost = task.LabourCost / numOfPeriods,

            //                    };
            //                    entity.PlannedDirectCost = entity.PlannedMaterialCost + entity.PlannedEquipmentCost + entity.PlannedLaborCost;
            //                    entity.PlannedProfitCost = ConsSettingHelper.GetCurrentProfitRate() * entity.PlannedDirectCost / 100;
            //                    entity.PlannedOverHeadCost = ConsSettingHelper.GetCurrentOverHeadRate() * entity.PlannedDirectCost / 100;
            //                    entity.TotalCost = entity.PlannedDirectCost + entity.PlannedProfitCost + entity.PlannedOverHeadCost;

            //                    db.TaskDetail.Add(entity);
            //                    db.SaveChanges();
            //                }

            //            }
            //        }
            //    }
            //} 
            #endregion
        }
        private void UpdateTaskData()
        {
            taskService.UpdateTaskData();
        }
        public DateTime GetTaskEndDate(DateTime start, decimal duration)
        {
            var endDate = start.AddDays((double)duration);
            return endDate;
        }
    }
}