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

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class EstimationTemplateCustomWorkItemDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult CustomWorkItemDetails_Read([DataSourceRequest]DataSourceRequest request, int workItemIdRead)
        {
            IQueryable<EstimationTemplateCustomWorkItemDetail> customWorkItemDetails = db.EstimationTemplateCustomWorkItemDetails
                .Where(x => x.ParentId == workItemIdRead);
            DataSourceResult result = customWorkItemDetails.ToDataSourceResult(request, content => new
            {
                EstimationTemplateCustomWorkItemDetailId = content.EstimationTemplateCustomWorkItemDetailId,
                ParentId = content.ParentId,
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
        private bool ItemIsRegistered(EstimationTemplateCustomWorkItemDetail model, int workItemId, ControllerActionType actionType) {
            var itemIsRegistered = false;
            if(actionType == ControllerActionType.Create)
            {
                var temp = db.EstimationTemplateCustomWorkItemDetails.FirstOrDefault(i => i.ProjectEstimationWorkItemId == model.ProjectEstimationWorkItemId && i.ParentId == workItemId);
                if (temp != null)
                {
                    itemIsRegistered = true;
                }
            }
            else if(actionType == ControllerActionType.Update)
            {
                var temp = db.EstimationTemplateCustomWorkItemDetails
                    .FirstOrDefault(i => i.ProjectEstimationWorkItemId == model.ProjectEstimationWorkItemId && i.ParentId == workItemId && i.EstimationTemplateCustomWorkItemDetailId != model.EstimationTemplateCustomWorkItemDetailId);
                if (temp != null)
                {
                    itemIsRegistered = true;
                }
            }
            return itemIsRegistered;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemDetail_Create([DataSourceRequest]DataSourceRequest request, EstimationTemplateCustomWorkItemDetail content, int workItemIdCreate)
        {
            if (ModelState.IsValid)
            {
                if (ItemIsRegistered(content, workItemIdCreate, ControllerActionType.Create)) {
                    ModelState.AddModelError("", "This Item Exists already!");
                }
                else
                {
                    var entity = new EstimationTemplateCustomWorkItemDetail
                    {
                        EstimationTemplateCustomWorkItemDetailId = content.EstimationTemplateCustomWorkItemDetailId,
                        ParentId = workItemIdCreate,
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

                    db.EstimationTemplateCustomWorkItemDetails.Add(entity);
                    db.SaveChanges();
                    content.EstimationTemplateCustomWorkItemDetailId = entity.EstimationTemplateCustomWorkItemDetailId;

                    //ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
                    ProjectEstimationWorkItemController.UpdateWorkItemCost(workItemIdCreate);
                }
               
            }

            return Json(new[] { content }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemDetail_Update([DataSourceRequest]DataSourceRequest request, EstimationTemplateCustomWorkItemDetail content)
        {
            if (ModelState.IsValid)
            {
                if (ItemIsRegistered(content, content.ParentId, ControllerActionType.Update))
                {
                    ModelState.AddModelError("", "This Item Exists already!");
                }
                var entity = new EstimationTemplateCustomWorkItemDetail
                {
                    EstimationTemplateCustomWorkItemDetailId = content.EstimationTemplateCustomWorkItemDetailId,
                    ParentId = content.ParentId,
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

                db.EstimationTemplateCustomWorkItemDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
                ProjectEstimationWorkItemController.UpdateWorkItemCost(entity.ParentId);
            }

            return Json(new[] { content }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemDetail_Destroy([DataSourceRequest]DataSourceRequest request, EstimationTemplateCustomWorkItemDetail content)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationTemplateCustomWorkItemDetail
                {
                    EstimationTemplateCustomWorkItemDetailId = content.EstimationTemplateCustomWorkItemDetailId,
                    ParentId = content.ParentId,
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

                db.EstimationTemplateCustomWorkItemDetails.Attach(entity);
                db.EstimationTemplateCustomWorkItemDetails.Remove(entity);
                db.SaveChanges();

                //ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
                ProjectEstimationWorkItemController.UpdateWorkItemCost(entity.ParentId);
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