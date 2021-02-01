using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public class TaskDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult TaskDetail_Read([DataSourceRequest]DataSourceRequest request, int theCurrentTaskId)
        {
            IQueryable<TaskDetail> taskDetails = db.TaskDetail.Where(d => d.ID == theCurrentTaskId);

            DataSourceResult result = taskDetails.ToDataSourceResult(request, taskDetail => new
            {
                TaskDetailId = taskDetail.TaskDetailId,
                ID = taskDetail.ID,
                PlannedQuantityOfWork = taskDetail.PlannedQuantityOfWork,
                ConstructionPeriodId = taskDetail.ConstructionPeriodId,
                PlannedMaterialCost = taskDetail.PlannedMaterialCost,
                PlannedEquipmentCost = taskDetail.PlannedEquipmentCost,
                PlannedLaborCost = taskDetail.PlannedLaborCost,
                PlannedDirectCost = taskDetail.PlannedDirectCost,
                PlannedProfitCost = taskDetail.PlannedProfitCost,
                PlannedOverHeadCost = taskDetail.PlannedOverHeadCost,
                TotalCost = taskDetail.TotalCost,

                OrgBranchName = taskDetail.OrgBranchName,
                DateCreated = taskDetail.DateCreated,
                LastModified = taskDetail.LastModified,
                CreatedBy = taskDetail.CreatedBy,
                ModifiedBy = taskDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskDetail_Create([DataSourceRequest]DataSourceRequest request, TaskDetail taskDetail, int theCurrentTaskId)
        {
            if (ModelState.IsValid)
            {
                var rootTask = db.Tasks.Find(theCurrentTaskId);
                var entity = new TaskDetail
                {
                    //TaskDetailId = taskDetail.TaskDetailId,
                    ID = theCurrentTaskId,
                    ConstructionPeriodId = taskDetail.ConstructionPeriodId,
                    PlannedQuantityOfWork = taskDetail.PlannedQuantityOfWork,
                    //PlannedMaterialCost = taskDetail.PlannedMaterialCost,
                    //PlannedEquipmentCost = taskDetail.PlannedEquipmentCost,
                    //PlannedLaborCost = taskDetail.PlannedLaborCost,
                    //PlannedDirectCost = taskDetail.PlannedDirectCost,
                    //PlannedProfitCost = taskDetail.PlannedProfitCost,
                    //PlannedOverHeadCost = taskDetail.PlannedOverHeadCost,
                    //TotalCost = taskDetail.TotalCost,

                    OrgBranchName = taskDetail.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = taskDetail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = taskDetail.ModifiedBy
                };
                if (rootTask != null)
                {
                    var durationItem = rootTask.WorkItemDuration;
                    if (durationItem != null)
                    {
                        var workItem = db.WorkItemPerProject.Find(durationItem.WorkItemPerProjectId);
                        if (workItem != null)
                        {
                            var quantity = entity.PlannedQuantityOfWork;

                            entity.PlannedMaterialCost = quantity * durationItem.MaterialCost;
                            entity.PlannedLaborCost = quantity * durationItem.LabourCost;
                            entity.PlannedEquipmentCost = quantity * durationItem.EquipmentCost;
                            entity.PlannedDirectCost = entity.PlannedMaterialCost + entity.PlannedLaborCost + entity.PlannedEquipmentCost;
                            entity.PlannedProfitCost = (workItem.ProfitMargin / 100) * entity.PlannedDirectCost;
                            entity.PlannedOverHeadCost = (workItem.OverheadRate / 100) * entity.PlannedDirectCost;

                            entity.TotalCost = entity.PlannedDirectCost + entity.PlannedProfitCost + entity.PlannedOverHeadCost;
                        }
                    }
                }

                db.TaskDetail.Add(entity);
                db.SaveChanges();
                taskDetail.TaskDetailId = entity.TaskDetailId;

                taskDetail.PlannedDirectCost = entity.PlannedDirectCost;
                taskDetail.TotalCost = entity.TotalCost;
            }

            return Json(new[] { taskDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskDetail_Update([DataSourceRequest]DataSourceRequest request, TaskDetail taskDetail)
        {
            if (ModelState.IsValid)
            {
                var rootTask = db.Tasks.Find(taskDetail.ID);
                var entity = new TaskDetail
                {
                    TaskDetailId = taskDetail.TaskDetailId,
                    ID = taskDetail.ID,
                    ConstructionPeriodId = taskDetail.ConstructionPeriodId,
                    PlannedQuantityOfWork = taskDetail.PlannedQuantityOfWork,
                    PlannedMaterialCost = taskDetail.PlannedMaterialCost,
                    PlannedEquipmentCost = taskDetail.PlannedEquipmentCost,
                    PlannedLaborCost = taskDetail.PlannedLaborCost,
                    PlannedDirectCost = taskDetail.PlannedDirectCost,
                    PlannedProfitCost = taskDetail.PlannedProfitCost,
                    PlannedOverHeadCost = taskDetail.PlannedOverHeadCost,
                    TotalCost = taskDetail.TotalCost,

                    OrgBranchName = taskDetail.OrgBranchName,
                    DateCreated = taskDetail.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = taskDetail.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };
                if (rootTask != null)
                {
                    var durationItem = rootTask.WorkItemDuration;
                    if (durationItem != null)
                    {
                        var workItem = db.WorkItemPerProject.Find(durationItem.WorkItemPerProjectId);
                        if (workItem != null)
                        {
                            var quantity = entity.PlannedQuantityOfWork;

                            entity.PlannedMaterialCost = quantity * workItem.MaterialCostPerUnit;
                            entity.PlannedLaborCost = quantity * workItem.LaborCostPerUnit;
                            entity.PlannedEquipmentCost = quantity * workItem.EquipmentCostPerUnit;
                            entity.PlannedDirectCost = entity.PlannedMaterialCost + entity.PlannedLaborCost + entity.PlannedEquipmentCost;
                            entity.PlannedProfitCost = (workItem.ProfitMargin / 100) * entity.PlannedDirectCost;
                            entity.PlannedOverHeadCost = (workItem.OverheadRate / 100) * entity.PlannedDirectCost;

                            entity.TotalCost = entity.PlannedDirectCost + entity.PlannedProfitCost + entity.PlannedOverHeadCost;
                        }
                    }
                }

                db.TaskDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                taskDetail.PlannedOverHeadCost = entity.PlannedOverHeadCost;
                taskDetail.PlannedProfitCost = entity.PlannedProfitCost;
                taskDetail.PlannedDirectCost = entity.PlannedDirectCost;
                taskDetail.TotalCost = entity.TotalCost;
            }

            return Json(new[] { taskDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TaskDetail_Destroy([DataSourceRequest]DataSourceRequest request, TaskDetail taskDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new TaskDetail
                {
                    TaskDetailId = taskDetail.TaskDetailId,
                    ID = taskDetail.ID,
                    ConstructionPeriodId = taskDetail.ConstructionPeriodId,
                    PlannedQuantityOfWork = taskDetail.PlannedQuantityOfWork,
                    PlannedMaterialCost = taskDetail.PlannedMaterialCost,
                    PlannedEquipmentCost = taskDetail.PlannedEquipmentCost,
                    PlannedLaborCost = taskDetail.PlannedLaborCost,
                    PlannedDirectCost = taskDetail.PlannedDirectCost,
                    PlannedProfitCost = taskDetail.PlannedProfitCost,
                    PlannedOverHeadCost = taskDetail.PlannedOverHeadCost,
                    TotalCost = taskDetail.TotalCost,

                    OrgBranchName = taskDetail.OrgBranchName,
                    DateCreated = taskDetail.DateCreated,
                    LastModified = taskDetail.LastModified,
                    CreatedBy = taskDetail.CreatedBy,
                    ModifiedBy = taskDetail.ModifiedBy
                };

                db.TaskDetail.Attach(entity);
                db.TaskDetail.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { taskDetail }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult ResetTaskCostBreakDown(int taskId)
        {
            var currentTask = db.Tasks.Find(taskId);
            if (currentTask == null)
            {
                var dt = new
                {
                    Message = "Work item (task) not found",
                    Status = "Error"
                };
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
            else
            {

                DateTime start = currentTask.Start;
                DateTime end = currentTask.End;

                var periodsInTimeRange = db.ConstructionPeriod.Where(p => p.DateFrom <= start && p.DateTo >= start ||
                                                                          p.DateFrom >= start && p.DateTo <= end ||
                                                                          p.DateFrom <= end && p.DateTo >= end
                                                                    ).ToList();
                if (periodsInTimeRange.Any() && currentTask.WorkItemDuration != null)
                {
                    var numOfPeriods = periodsInTimeRange.Count;
                    var workItem = currentTask.WorkItemDuration;

                    var existingItems = db.TaskDetail.Where(td => td.ID == currentTask.ID).ToList();
                    if (existingItems.Any())
                    {
                        db.TaskDetail.RemoveRange(existingItems);
                        db.SaveChanges();
                    }
                    foreach (ConstructionPeriod period in periodsInTimeRange)
                    {
                        var subPeriods = db.ConstructionSubPeriod.Where(sp => sp.ConstructionPeriodId == period.ConstructionPeriodId).ToList();
                        //var existingItem = db.TaskDetail.FirstOrDefault(td => td.ID == currentTask.ID && td.ConstructionPeriodId == period.ConstructionPeriodId);

                        var entity = new TaskDetail
                        {
                            ConstructionPeriodId = period.ConstructionPeriodId,
                            ID = currentTask.ID,
                            PlannedQuantityOfWork = currentTask.TotalQuantityOfWork / numOfPeriods,
                            //PlannedMaterialCost = currentTask.MaterialCost / numOfPeriods,
                            //PlannedEquipmentCost = currentTask.EquipmentCost / numOfPeriods,
                            //PlannedLaborCost = currentTask.LabourCost / numOfPeriods,

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
                        //}
                    }
                }

                var dt = new
                {
                    Message = "Operation Completed Successfully",
                    Status = "Ok"
                };
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}