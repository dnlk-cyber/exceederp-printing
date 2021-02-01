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

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public class TaskDetailBreakDownController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Detail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TaskDetailBreakDown> taskDetails = db.TaskDetailBreakDown.Where(d => d.TaskDetailId == id);

            DataSourceResult result = taskDetails.ToDataSourceResult(request, taskDetail => new
            {
                TaskDetailBreakDownId = taskDetail.TaskDetailBreakDownId,
                TaskDetailId = taskDetail.TaskDetailId,
                ConstructionSubPeriodId = taskDetail.ConstructionSubPeriodId,
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
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Detail_Create([DataSourceRequest]DataSourceRequest request, TaskDetailBreakDown taskDetail, int id)
        {
            if (ModelState.IsValid)
            {
                var detail = db.TaskDetail.Find(id);
                //var rootTask = db.Tasks.Find(tas)
                var entity = new TaskDetailBreakDown
                {
                    TaskDetailId = id,
                    ConstructionSubPeriodId = taskDetail.ConstructionSubPeriodId,
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
                if (detail != null)
                {

                    var durationItem = detail.Task.WorkItemDuration;
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

                db.TaskDetailBreakDown.Add(entity);
                db.SaveChanges();
                taskDetail.TaskDetailId = entity.TaskDetailId;

                taskDetail.PlannedOverHeadCost = entity.PlannedOverHeadCost;
                taskDetail.PlannedProfitCost = entity.PlannedProfitCost;
                taskDetail.PlannedDirectCost = entity.PlannedDirectCost;
                taskDetail.TotalCost = entity.TotalCost;
            }

            return Json(new[] { taskDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Detail_Update([DataSourceRequest]DataSourceRequest request, TaskDetailBreakDown taskDetail)
        {
            if (ModelState.IsValid)
            {
                var detail = db.TaskDetail.Find(taskDetail.TaskDetailId);
                var entity = new TaskDetailBreakDown
                {

                    TaskDetailBreakDownId = taskDetail.TaskDetailBreakDownId,
                    TaskDetailId = taskDetail.TaskDetailId,
                    ConstructionSubPeriodId = taskDetail.ConstructionSubPeriodId,
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
                if (detail != null)
                {
                    var durationItem = detail.Task.WorkItemDuration;
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

                db.TaskDetailBreakDown.Attach(entity);
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
        public ActionResult Detail_Destroy([DataSourceRequest]DataSourceRequest request, TaskDetailBreakDown taskDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new TaskDetailBreakDown
                {

                    TaskDetailBreakDownId = taskDetail.TaskDetailBreakDownId,
                    TaskDetailId = taskDetail.TaskDetailId,
                    ConstructionSubPeriodId = taskDetail.ConstructionSubPeriodId,
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

                db.TaskDetailBreakDown.Attach(entity);
                db.TaskDetailBreakDown.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { taskDetail }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}