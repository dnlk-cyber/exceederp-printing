using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    public class PriceAdjustmentWeightItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult WeightItems_Read([DataSourceRequest]DataSourceRequest request, int idRead)
        {
            IQueryable<PriceAdjustmentWeightItem> items = db.PriceAdjustmentWeightItems.Where(itm => itm.PriceAdjustmentWeightId == idRead);
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                PriceAdjustmentWeightId = item.PriceAdjustmentWeightId,
                PriceAdjustmentWeightItemId = item.PriceAdjustmentWeightItemId,
                ProjectRepresentativeElementId = item.ProjectRepresentativeElementId,
                ItemName = item.ItemName,
                ItemCode = item.ItemCode,
                FleetsID = item.FleetsID,
                ProjectEstimationIndexLabourId = item.ProjectEstimationIndexLabourId,
                Amount = item.Amount,
                ApprovedAmount = item.ApprovedAmount,

                OrgBranchName = item.OrgBranchName,
                DateCreated = item.DateCreated,
                LastModified = item.LastModified,
                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeightItem_Create([DataSourceRequest]DataSourceRequest request, PriceAdjustmentWeightItem item, int idCreate)
        {
            if (ModelState.IsValid)
            {

                var entity = new PriceAdjustmentWeightItem
                {
                    PriceAdjustmentWeightId = item.PriceAdjustmentWeightId,
                    PriceAdjustmentWeightItemId = idCreate,
                    ProjectRepresentativeElementId = item.ProjectRepresentativeElementId,
                    ItemName = item.ItemName,
                    ItemCode = item.ItemCode,
                    FleetsID = item.FleetsID,
                    ProjectEstimationIndexLabourId = item.ProjectEstimationIndexLabourId,
                    Amount = item.Amount,
                    ApprovedAmount = item.ApprovedAmount,
                    
                    OrgBranchName = item.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = item.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = item.ModifiedBy
                };

                db.PriceAdjustmentWeightItems.Add(entity);
                db.SaveChanges();

                item.PriceAdjustmentWeightItemId = entity.PriceAdjustmentWeightItemId;

            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeightItem_Update([DataSourceRequest]DataSourceRequest request, PriceAdjustmentWeightItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceAdjustmentWeightItem
                {
                    PriceAdjustmentWeightId = item.PriceAdjustmentWeightId,
                    PriceAdjustmentWeightItemId = item.PriceAdjustmentWeightItemId,
                    ProjectRepresentativeElementId = item.ProjectRepresentativeElementId,
                    ItemName = item.ItemName,
                    ItemCode = item.ItemCode,
                    FleetsID = item.FleetsID,
                    ProjectEstimationIndexLabourId = item.ProjectEstimationIndexLabourId,
                    Amount = item.Amount,
                    ApprovedAmount = item.ApprovedAmount,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.PriceAdjustmentWeightItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeightItem_Destroy([DataSourceRequest]DataSourceRequest request, PriceAdjustmentWeightItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceAdjustmentWeightItem
                {
                    PriceAdjustmentWeightId = item.PriceAdjustmentWeightId,
                    PriceAdjustmentWeightItemId = item.PriceAdjustmentWeightItemId,
                    ProjectRepresentativeElementId = item.ProjectRepresentativeElementId,
                    ItemName = item.ItemName,
                    ItemCode = item.ItemCode,
                    FleetsID = item.FleetsID,
                    ProjectEstimationIndexLabourId = item.ProjectEstimationIndexLabourId,
                    Amount = item.Amount,
                    ApprovedAmount = item.ApprovedAmount,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy
                };

                db.PriceAdjustmentWeightItems.Attach(entity);
                db.PriceAdjustmentWeightItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}