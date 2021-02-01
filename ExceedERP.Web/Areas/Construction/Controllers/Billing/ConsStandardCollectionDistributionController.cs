using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class ConsStandardCollectionDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult CollectionDistribution_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ConsStandardCollectionDistribution> distributions = db.ConsStandardCollectionDistribution.Where(d => d.ConsARStandardCollectionId == id).OrderBy(d => d.Credit);
            DataSourceResult result = distributions.ToDataSourceResult(request, distribution => new
            {
                ConsARStandardCollectionId = distribution.ConsARStandardCollectionId,
                ConsStandardCollectionDistributionId = distribution.ConsStandardCollectionDistributionId,
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
        public ActionResult CollectionDistribution_Create([DataSourceRequest]DataSourceRequest request, ConsStandardCollectionDistribution distribution, int id)
        {
            if (ModelState.IsValid)
            {
                
                var entity = new ConsStandardCollectionDistribution
                {
                    ConsStandardCollectionDistributionId = distribution.ConsStandardCollectionDistributionId,
                    ConsARStandardCollectionId = id,
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
                
                db.ConsStandardCollectionDistribution.Add(entity);
                db.SaveChanges();
                distribution.ConsStandardCollectionDistributionId = entity.ConsStandardCollectionDistributionId;
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CollectionDistribution_Update([DataSourceRequest]DataSourceRequest request, ConsStandardCollectionDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsStandardCollectionDistribution
                {
                    ConsStandardCollectionDistributionId = distribution.ConsStandardCollectionDistributionId,
                    ConsARStandardCollectionId = distribution.ConsARStandardCollectionId,
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
                
                db.ConsStandardCollectionDistribution.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CollectionDistribution_Destroy([DataSourceRequest]DataSourceRequest request, ConsStandardCollectionDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsStandardCollectionDistribution
                {
                    ConsStandardCollectionDistributionId = distribution.ConsStandardCollectionDistributionId,
                    ConsARStandardCollectionId = distribution.ConsARStandardCollectionId,
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

                db.ConsStandardCollectionDistribution.Attach(entity);
                db.ConsStandardCollectionDistribution.Remove(entity);
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