using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjectCustomWorkItemDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult CustomWorkItemDetails_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            IQueryable<ProjectCustomWorkItemDetail> customWorkItemDetails = db.ProjectCustomWorkItemDetails
                .Where(x => x.WorkItemPerProjectId == workItemId);
            DataSourceResult result = customWorkItemDetails.ToDataSourceResult(request, content => new
            {
                ProjectCustomWorkItemDetailId = content.ProjectCustomWorkItemDetailId,
                WorkItemPerProjectId = content.WorkItemPerProjectId,
                ProjectEstimationWorkItemId = content.ProjectEstimationWorkItemId,
                QuantityPerUnit = content.QuantityPerUnit,
                UnitCost = content.UnitCost,
                TotalCost = content.TotalCost,

                OrgBranchName = content.OrgBranchName,
                DateCreated = content.DateCreated,
                LastModified = content.LastModified,
                CreatedBy = content.CreatedBy,
                ModifiedBy = content.ModifiedBy,
            });

            return Json(result);

        }
        private bool ItemIsRegistered(ProjectCustomWorkItemDetail model, int workItemId)
        {
            var itemIsRegistered = false;

            var temp = db.ProjectCustomWorkItemDetails
                .FirstOrDefault(i => i.ProjectEstimationWorkItemId == model.ProjectEstimationWorkItemId && i.WorkItemPerProjectId == workItemId && i.WorkItemPerProjectId != model.WorkItemPerProjectId);
            if (temp != null)
            {
                itemIsRegistered = true;
            }
            return itemIsRegistered;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemDetail_Create([DataSourceRequest]DataSourceRequest request, ProjectCustomWorkItemDetail content, int workItemId)
        {
            if (ModelState.IsValid)
            {
                if (ItemIsRegistered(content, workItemId))
                {
                    ModelState.AddModelError("", "This Item Exists already!");
                }
                else
                {
                    var entity = new ProjectCustomWorkItemDetail
                    {
                        ProjectCustomWorkItemDetailId = content.ProjectCustomWorkItemDetailId,
                        WorkItemPerProjectId = workItemId,
                        ProjectEstimationWorkItemId = content.ProjectEstimationWorkItemId,
                        QuantityPerUnit = content.QuantityPerUnit,
                        UnitCost = ProjectEstimationHelper.GetEstimationWorkItemDirectCost(content.ProjectEstimationWorkItemId),
                        TotalCost = content.TotalCost,

                        OrgBranchName = content.OrgBranchName,
                        DateCreated = DateTime.Now.Date,
                        LastModified = content.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = content.ModifiedBy,
                    };
                    entity.TotalCost = entity.QuantityPerUnit * entity.UnitCost;

                    db.ProjectCustomWorkItemDetails.Add(entity);
                    db.SaveChanges();
                    content.ProjectCustomWorkItemDetailId = entity.ProjectCustomWorkItemDetailId;

                    //ProjectEstimationHelper.UpdateGenericWorkItem(workItemId);
                    //ProjectEstimationWorkItemController.UpdateWorkItemCost(workItemIdCreate);
                }

            }

            return Json(new[] { content }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemDetail_Update([DataSourceRequest]DataSourceRequest request, ProjectCustomWorkItemDetail content)
        {
            if (ModelState.IsValid)
            {
                if (ItemIsRegistered(content, content.WorkItemPerProjectId))
                {
                    ModelState.AddModelError("", "This Item Exists already!");
                }
                var entity = new ProjectCustomWorkItemDetail
                {
                    ProjectCustomWorkItemDetailId = content.ProjectCustomWorkItemDetailId,
                    WorkItemPerProjectId = content.WorkItemPerProjectId,
                    ProjectEstimationWorkItemId = content.ProjectEstimationWorkItemId,
                    QuantityPerUnit = content.QuantityPerUnit,
                    UnitCost = ProjectEstimationHelper.GetEstimationWorkItemDirectCost(content.ProjectEstimationWorkItemId),
                    TotalCost = content.TotalCost,


                    OrgBranchName = content.OrgBranchName,
                    DateCreated = content.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = content.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };
                entity.TotalCost = entity.QuantityPerUnit * entity.UnitCost;

                db.ProjectCustomWorkItemDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);
                //ProjectEstimationWorkItemController.UpdateWorkItemCost(entity.ParentId);
            }

            return Json(new[] { content }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemDetail_Destroy([DataSourceRequest]DataSourceRequest request, ProjectCustomWorkItemDetail content)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCustomWorkItemDetail
                {
                    ProjectCustomWorkItemDetailId = content.ProjectCustomWorkItemDetailId,
                    WorkItemPerProjectId = content.WorkItemPerProjectId,
                    ProjectEstimationWorkItemId = content.ProjectEstimationWorkItemId,
                    QuantityPerUnit = content.QuantityPerUnit,
                    UnitCost = ProjectEstimationHelper.GetEstimationWorkItemDirectCost(content.ProjectEstimationWorkItemId),
                    TotalCost = content.TotalCost,

                    OrgBranchName = content.OrgBranchName,
                    DateCreated = content.DateCreated,
                    LastModified = content.LastModified,
                    CreatedBy = content.CreatedBy,
                    ModifiedBy = content.ModifiedBy,
                };

                db.ProjectCustomWorkItemDetails.Attach(entity);
                db.ProjectCustomWorkItemDetails.Remove(entity);
                db.SaveChanges();

                //ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);
                //ProjectEstimationWorkItemController.UpdateWorkItemCost(entity.ParentId);
            }

            return Json(new[] { content }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}