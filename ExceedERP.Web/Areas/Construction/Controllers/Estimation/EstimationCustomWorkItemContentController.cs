using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class EstimationCustomWorkItemContentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //public ActionResult Index(string name)
        //{

        //    return View();
        //}

        public ActionResult CustomWorkItemContent_Read([DataSourceRequest]DataSourceRequest request, int workItemIdRead)
        {
            IQueryable<EstimationCustomWorkItemContent> customWorkItemContents = db.EstimationCustomWorkItemContents
                .Where(x => x.ProjectEstimationCustomWorkItemId == workItemIdRead);
            DataSourceResult result = customWorkItemContents.ToDataSourceResult(request, content => new
            {
                EstimationCustomWorkItemContentId = content.EstimationCustomWorkItemContentId,
                ProjectEstimationCustomWorkItemId = content.ProjectEstimationCustomWorkItemId,
                ProjectEstimationWorkItemId = content.ProjectEstimationWorkItemId,
                QuantityPerUnit = content.QuantityPerUnit,
                UnitCost = content.UnitCost,
                UnitPrice = content.UnitPrice,

                OrgBranchName = content.OrgBranchName,
                DateCreated = content.DateCreated,
                LastModified = content.LastModified,
                CreatedBy = content.CreatedBy,
                ModifiedBy = content.ModifiedBy,
            });

            return Json(result);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemContent_Create([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemContent content, int workItemIdCreate)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationCustomWorkItemContent
                {
                    EstimationCustomWorkItemContentId = content.EstimationCustomWorkItemContentId,
                    ProjectEstimationCustomWorkItemId = workItemIdCreate,
                    ProjectEstimationWorkItemId = content.ProjectEstimationWorkItemId,
                    QuantityPerUnit = content.QuantityPerUnit,
                    UnitCost = ProjectEstimationHelper.GetEstimationWorkItemDirectCost(content.ProjectEstimationWorkItemId),
                    UnitPrice = content.UnitPrice,

                    OrgBranchName = content.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = content.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = content.ModifiedBy,
                };
                entity.UnitPrice = entity.QuantityPerUnit * entity.UnitCost;

                db.EstimationCustomWorkItemContents.Add(entity);
                db.SaveChanges();
                content.ProjectEstimationCustomWorkItemId = entity.ProjectEstimationCustomWorkItemId;

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
            }

            return Json(new[] { content }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemContent_Update([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemContent content)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationCustomWorkItemContent
                {
                    EstimationCustomWorkItemContentId = content.EstimationCustomWorkItemContentId,
                    ProjectEstimationCustomWorkItemId = content.ProjectEstimationCustomWorkItemId,
                    ProjectEstimationWorkItemId = content.ProjectEstimationWorkItemId,
                    QuantityPerUnit = content.QuantityPerUnit,
                    UnitCost = ProjectEstimationHelper.GetEstimationWorkItemDirectCost(content.ProjectEstimationWorkItemId),
                    UnitPrice = content.UnitPrice,


                    OrgBranchName = content.OrgBranchName,
                    DateCreated = content.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = content.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };
                entity.UnitPrice = entity.QuantityPerUnit * entity.UnitCost;

                db.EstimationCustomWorkItemContents.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);

            }

            return Json(new[] { content }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItemContent_Destroy([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemContent content)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationCustomWorkItemContent
                {
                    EstimationCustomWorkItemContentId = content.EstimationCustomWorkItemContentId,
                    ProjectEstimationCustomWorkItemId = content.ProjectEstimationCustomWorkItemId,
                    ProjectEstimationWorkItemId = content.ProjectEstimationWorkItemId,
                    QuantityPerUnit = content.QuantityPerUnit,
                    UnitCost = content.UnitCost,
                    UnitPrice = content.UnitPrice,

                    OrgBranchName = content.OrgBranchName,
                    DateCreated = content.DateCreated,
                    LastModified = content.LastModified,
                    CreatedBy = content.CreatedBy,
                    ModifiedBy = content.ModifiedBy,
                };

                db.EstimationCustomWorkItemContents.Attach(entity);
                db.EstimationCustomWorkItemContents.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
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