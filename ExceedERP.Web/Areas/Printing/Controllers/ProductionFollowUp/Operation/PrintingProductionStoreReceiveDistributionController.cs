
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class PrintingProductionStoreReceiveDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Distribution_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PrintingProductionFinishedGoodsStoreReceiveDistribution> distributions = db.PrintingProductionFinishedGoodsStoreReceiveDistributions.Where(d => d.FinishedGoodsStoreReceiveDistributionId == id).OrderByDescending(d => d.Debit);
            DataSourceResult result = distributions.ToDataSourceResult(request, distribution => new
            {
                FinishedGoodsStoreReceiveDistributionId = distribution.FinishedGoodsStoreReceiveDistributionId,
                ProductionStoreReceiveId = distribution.FinishedGoodsStoreReceiveId,
                FinishedGoodsStoreReceiveId = distribution.ReceiveItemIDTrackBack,
                AccountCode = distribution.AccountCode,
                AccountDesc = distribution.AccountDesc,
                Credit = distribution.Credit,
                Debit = distribution.Debit,


                OrgBranchName = distribution.OrgBranchName,
                DateCreated = distribution.DateCreated,
                LastModified = distribution.LastModified,
                CreatedBy = distribution.CreatedBy,
                ModifiedBy = distribution.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Create([DataSourceRequest]DataSourceRequest request, PrintingProductionFinishedGoodsStoreReceiveDistribution distribution, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductionFinishedGoodsStoreReceiveDistribution
                {
                    FinishedGoodsStoreReceiveDistributionId = distribution.FinishedGoodsStoreReceiveDistributionId,
                    FinishedGoodsStoreReceiveId =id,
                    ReceiveItemIDTrackBack = distribution.ReceiveItemIDTrackBack,
                    AccountCode = distribution.AccountCode,
                    AccountDesc = distribution.AccountDesc,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    OrgBranchName = distribution.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = distribution.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = distribution.ModifiedBy
                };
                

                db.PrintingProductionFinishedGoodsStoreReceiveDistributions.Add(entity);
                db.SaveChanges();
                distribution.FinishedGoodsStoreReceiveDistributionId = entity.FinishedGoodsStoreReceiveDistributionId;
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Update([DataSourceRequest]DataSourceRequest request, PrintingProductionFinishedGoodsStoreReceiveDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductionFinishedGoodsStoreReceiveDistribution
                {
                    FinishedGoodsStoreReceiveDistributionId = distribution.FinishedGoodsStoreReceiveDistributionId,
                    FinishedGoodsStoreReceiveId = distribution.FinishedGoodsStoreReceiveId,
                    ReceiveItemIDTrackBack = distribution.ReceiveItemIDTrackBack,
                    AccountCode = distribution.AccountCode,
                    AccountDesc = distribution.AccountDesc,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    OrgBranchName = distribution.OrgBranchName,
                    DateCreated = distribution.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = distribution.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                

                db.PrintingProductionFinishedGoodsStoreReceiveDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Destroy([DataSourceRequest]DataSourceRequest request, PrintingProductionFinishedGoodsStoreReceiveDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductionFinishedGoodsStoreReceiveDistribution
                {
                    FinishedGoodsStoreReceiveDistributionId = distribution.FinishedGoodsStoreReceiveDistributionId,
                    FinishedGoodsStoreReceiveId = distribution.FinishedGoodsStoreReceiveId,
                    ReceiveItemIDTrackBack = distribution.ReceiveItemIDTrackBack,
                    AccountCode = distribution.AccountCode,
                    AccountDesc = distribution.AccountDesc,
                    Credit = distribution.Credit,
                    Debit = distribution.Debit,

                    OrgBranchName = distribution.OrgBranchName,
                    DateCreated = distribution.DateCreated,
                    LastModified = distribution.LastModified,
                    CreatedBy = distribution.CreatedBy,
                    ModifiedBy = distribution.ModifiedBy
                };

                db.PrintingProductionFinishedGoodsStoreReceiveDistributions.Attach(entity);
                db.PrintingProductionFinishedGoodsStoreReceiveDistributions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}