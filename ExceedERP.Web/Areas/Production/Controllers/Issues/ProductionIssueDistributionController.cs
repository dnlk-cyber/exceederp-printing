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
    public class ProductionIssueDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Distribution_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductionIssueDistribution> distributions = db.ProductionIssueDistributions.Where(d => d.ProductionIssueId == id).OrderByDescending(d => d.Debit);
            DataSourceResult result = distributions.ToDataSourceResult(request, distribution => new
            {
                ProductionIssueDistributionId = distribution.ProductionIssueDistributionId,
                ProductionIssueId = distribution.ProductionIssueId,
                IssueItemIDTrackBack = distribution.IssueItemIDTrackBack,
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
        public ActionResult Distribution_Create([DataSourceRequest]DataSourceRequest request, ProductionIssueDistribution distribution, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionIssueDistribution
                {
                    ProductionIssueDistributionId = distribution.ProductionIssueDistributionId,
                    ProductionIssueId = id,
                    IssueItemIDTrackBack = distribution.IssueItemIDTrackBack,
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
                

                db.ProductionIssueDistributions.Add(entity);
                db.SaveChanges();
                distribution.ProductionIssueDistributionId = entity.ProductionIssueDistributionId;
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Update([DataSourceRequest]DataSourceRequest request, ProductionIssueDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionIssueDistribution
                {
                    ProductionIssueDistributionId = distribution.ProductionIssueDistributionId,
                    ProductionIssueId = distribution.ProductionIssueId,
                    IssueItemIDTrackBack = distribution.IssueItemIDTrackBack,
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
                

                db.ProductionIssueDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Destroy([DataSourceRequest]DataSourceRequest request, ProductionIssueDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionIssueDistribution
                {
                    ProductionIssueDistributionId = distribution.ProductionIssueDistributionId,
                    ProductionIssueId = distribution.ProductionIssueId,
                    IssueItemIDTrackBack = distribution.IssueItemIDTrackBack,
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

                db.ProductionIssueDistributions.Attach(entity);
                db.ProductionIssueDistributions.Remove(entity);
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