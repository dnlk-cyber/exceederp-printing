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
    public class GenericProjectCustomWorkItemDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult CustomWorkItemDetails_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            IQueryable<GenericProjectCustomWorkItemDetail> customWorkItemDetails = db.GenericProjectCustomWorkItemDetails
                .Where(x => x.GenericWorkItemPerProjectId == workItemId);
            DataSourceResult result = customWorkItemDetails.ToDataSourceResult(request, content => new
            {
                GenericProjectCustomWorkItemDetailId = content.GenericProjectCustomWorkItemDetailId,
                GenericWorkItemPerProjectId = content.GenericWorkItemPerProjectId,
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
        private bool ItemIsRegistered(GenericProjectCustomWorkItemDetail model, int workItemId, ControllerActionType actionType) {
            var itemIsRegistered = false;
            if(actionType == ControllerActionType.Create)
            {
                var temp = db.GenericProjectCustomWorkItemDetails.FirstOrDefault(i => i.ProjectEstimationWorkItemId == model.ProjectEstimationWorkItemId && i.GenericWorkItemPerProjectId == workItemId);
                if (temp != null)
                {
                    itemIsRegistered = true;
                }
            }
            else if(actionType == ControllerActionType.Update)
            {
                var temp = db.GenericProjectCustomWorkItemDetails
                    .FirstOrDefault(i => i.ProjectEstimationWorkItemId == model.ProjectEstimationWorkItemId && i.GenericWorkItemPerProjectId == workItemId && i.GenericProjectCustomWorkItemDetailId != model.GenericProjectCustomWorkItemDetailId);
                if (temp != null)
                {
                    itemIsRegistered = true;
                }
            }
            return itemIsRegistered;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemDetail_Create([DataSourceRequest]DataSourceRequest request, GenericProjectCustomWorkItemDetail content, int workItemId)
        {
            if (ModelState.IsValid)
            {
                if (ItemIsRegistered(content, workItemId, ControllerActionType.Create)) {
                    ModelState.AddModelError("", "This Item Exists already!");
                }
                else
                {
                    var entity = new GenericProjectCustomWorkItemDetail
                    {
                        GenericProjectCustomWorkItemDetailId = content.GenericProjectCustomWorkItemDetailId,
                        GenericWorkItemPerProjectId = workItemId,
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

                    db.GenericProjectCustomWorkItemDetails.Add(entity);
                    db.SaveChanges();
                    content.GenericProjectCustomWorkItemDetailId = entity.GenericProjectCustomWorkItemDetailId;

                    ProjectEstimationHelper.UpdateGenericWorkItem(workItemId);
                    //ProjectEstimationWorkItemController.UpdateWorkItemCost(workItemIdCreate);
                }

            }

            return Json(new[] { content }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemDetail_Update([DataSourceRequest]DataSourceRequest request, GenericProjectCustomWorkItemDetail content)
        {
            if (ModelState.IsValid)
            {
                if (ItemIsRegistered(content, content.GenericWorkItemPerProjectId, ControllerActionType.Update))
                {
                    ModelState.AddModelError("", "This Item Exists already!");
                }
                var entity = new GenericProjectCustomWorkItemDetail
                {
                    GenericProjectCustomWorkItemDetailId = content.GenericProjectCustomWorkItemDetailId,
                    GenericWorkItemPerProjectId = content.GenericWorkItemPerProjectId,
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

                db.GenericProjectCustomWorkItemDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);
                //ProjectEstimationWorkItemController.UpdateWorkItemCost(entity.ParentId);
            }

            return Json(new[] { content }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemDetail_Destroy([DataSourceRequest]DataSourceRequest request, GenericProjectCustomWorkItemDetail content)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectCustomWorkItemDetail
                {
                    GenericProjectCustomWorkItemDetailId = content.GenericProjectCustomWorkItemDetailId,
                    GenericWorkItemPerProjectId = content.GenericWorkItemPerProjectId,
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

                db.GenericProjectCustomWorkItemDetails.Attach(entity);
                db.GenericProjectCustomWorkItemDetails.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);
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