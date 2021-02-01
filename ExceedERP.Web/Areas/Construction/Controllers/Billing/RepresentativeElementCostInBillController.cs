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

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class RepresentativeElementCostInBillController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult RepresentativeElements_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<RepresentativeElementCostInBill> elements = db.RepresentativeElementCostInBill.Where(e => e.BoQOfExecutedWorkId == id);
            DataSourceResult result = elements.ToDataSourceResult(request, element => new {
                RepresentativeElementCostInBillId = element.RepresentativeElementCostInBillId,
                ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                BoQOfExecutedWorkId = element.BoQOfExecutedWorkId,
                ItemCode = element.ItemCode,
                FleetsID = element.FleetsID,
                ProjectEstimationIndexLabourId = element.ProjectEstimationIndexLabourId,
                ItemName = element.ItemName,
                Unit = element.Unit,
                BasicPrice = element.BasicPrice,
                CurrentPrice = element.CurrentPrice,
                BidderProposedWeight = element.BidderProposedWeight,

                OrgBranchName = element.OrgBranchName,
                DateCreated = element.DateCreated,
                LastModified = element.LastModified,
                CreatedBy = element.CreatedBy,
                ModifiedBy = element.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RepresentativeElement_Create([DataSourceRequest]DataSourceRequest request, RepresentativeElementCostInBill element, int id)
        {
            if (ModelState.IsValid)
            {

                var entity = new RepresentativeElementCostInBill
                {
                    //RepresentativeElementCostInBillId = element.RepresentativeElementCostInBillId,
                    ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                    BoQOfExecutedWorkId = id,
                    ItemCode = element.ItemCode,
                    FleetsID = element.FleetsID,
                    ProjectEstimationIndexLabourId = element.ProjectEstimationIndexLabourId,
                    ItemName = element.ItemName,
                    Unit = element.Unit,
                    BasicPrice = element.BasicPrice,
                    CurrentPrice = element.CurrentPrice,
                    BidderProposedWeight = element.BidderProposedWeight,

                    OrgBranchName = element.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = element.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = element.ModifiedBy
                };

                db.RepresentativeElementCostInBill.Add(entity);
                db.SaveChanges();

                element.RepresentativeElementCostInBillId = entity.RepresentativeElementCostInBillId;
                element.Unit = entity.Unit;
                element.BasicPrice = entity.BasicPrice;
            }

            return Json(new[] { element }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RepresentativeElement_Update([DataSourceRequest]DataSourceRequest request, RepresentativeElementCostInBill element)
        {
            if (ModelState.IsValid)
            {
                var entity = new RepresentativeElementCostInBill
                {
                    RepresentativeElementCostInBillId = element.RepresentativeElementCostInBillId,
                    ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                    BoQOfExecutedWorkId = element.BoQOfExecutedWorkId,
                    ItemCode = element.ItemCode,
                    FleetsID = element.FleetsID,
                    ProjectEstimationIndexLabourId = element.ProjectEstimationIndexLabourId,
                    ItemName = element.ItemName,
                    Unit = element.Unit,
                    BasicPrice = element.BasicPrice,
                    CurrentPrice = element.CurrentPrice,
                    BidderProposedWeight = element.BidderProposedWeight,

                    OrgBranchName = element.OrgBranchName,
                    DateCreated = element.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = element.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.RepresentativeElementCostInBill.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { element }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RepresentativeElement_Destroy([DataSourceRequest]DataSourceRequest request, RepresentativeElementCostInBill element)
        {
            if (ModelState.IsValid)
            {
                var entity = new RepresentativeElementCostInBill
                {
                    RepresentativeElementCostInBillId = element.RepresentativeElementCostInBillId,
                    ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                    BoQOfExecutedWorkId = element.BoQOfExecutedWorkId,
                    ItemCode = element.ItemCode,
                    FleetsID = element.FleetsID,
                    ProjectEstimationIndexLabourId = element.ProjectEstimationIndexLabourId,
                    ItemName = element.ItemName,
                    Unit = element.Unit,
                    BasicPrice = element.BasicPrice,
                    CurrentPrice = element.CurrentPrice,
                    BidderProposedWeight = element.BidderProposedWeight,

                    OrgBranchName = element.OrgBranchName,
                    DateCreated = element.DateCreated,
                    LastModified = element.LastModified,
                    CreatedBy = element.CreatedBy,
                    ModifiedBy = element.ModifiedBy
                };

                db.RepresentativeElementCostInBill.Attach(entity);
                db.RepresentativeElementCostInBill.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { element }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}