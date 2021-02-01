using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.Issues
{
    public class ProductionStoreReceiveDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Distribution_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductionStoreReceiveDistribution> distributions = db.ProductionStoreReceiveDistributions.Where(d => d.ProductionStoreReceiveId == id).OrderByDescending(d => d.Debit);
            DataSourceResult result = distributions.ToDataSourceResult(request, distribution => new
            {
                ProductionStoreReceiveDistributionId = distribution.ProductionStoreReceiveDistributionId,
                ProductionStoreReceiveId = distribution.ProductionStoreReceiveId,
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
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Create([DataSourceRequest]DataSourceRequest request, ProductionStoreReceiveDistribution distribution, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionStoreReceiveDistribution
                {
                    ProductionStoreReceiveDistributionId = distribution.ProductionStoreReceiveDistributionId,
                    ProductionStoreReceiveId =id,
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
                

                db.ProductionStoreReceiveDistributions.Add(entity);
                db.SaveChanges();
                distribution.ProductionStoreReceiveDistributionId = entity.ProductionStoreReceiveDistributionId;
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Update([DataSourceRequest]DataSourceRequest request, ProductionStoreReceiveDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionStoreReceiveDistribution
                {
                    ProductionStoreReceiveDistributionId = distribution.ProductionStoreReceiveDistributionId,
                    ProductionStoreReceiveId = distribution.ProductionStoreReceiveId,
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
                

                db.ProductionStoreReceiveDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Destroy([DataSourceRequest]DataSourceRequest request, ProductionStoreReceiveDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionStoreReceiveDistribution
                {
                    ProductionStoreReceiveDistributionId = distribution.ProductionStoreReceiveDistributionId,
                    ProductionStoreReceiveId = distribution.ProductionStoreReceiveId,
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

                db.ProductionStoreReceiveDistributions.Attach(entity);
                db.ProductionStoreReceiveDistributions.Remove(entity);
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