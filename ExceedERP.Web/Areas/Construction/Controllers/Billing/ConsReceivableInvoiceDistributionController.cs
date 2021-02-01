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
    public class ConsReceivableInvoiceDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult InvoiceDistribution_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ConsReceivableInvoiceDistribution> distributions = db.ConsReceivableInvoiceDistribution.Where(d => d.ConsReceivableInvoiceId == id);
            DataSourceResult result = distributions.ToDataSourceResult(request, distribution => new
            {
                ConsReceivableInvoiceId = distribution.ConsReceivableInvoiceId,
                ConsReceivableInvoiceDistributionId = distribution.ConsReceivableInvoiceDistributionId,
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
        public ActionResult InvoiceDistribution_Create([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoiceDistribution distribution, int id)
        {
            if (ModelState.IsValid)
            {

                var entity = new ConsReceivableInvoiceDistribution
                {
                    ConsReceivableInvoiceDistributionId = distribution.ConsReceivableInvoiceDistributionId,
                    ConsReceivableInvoiceId = id,
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

                db.ConsReceivableInvoiceDistribution.Add(entity);
                db.SaveChanges();
                distribution.ConsReceivableInvoiceDistributionId = entity.ConsReceivableInvoiceDistributionId;
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvoiceDistribution_Update([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoiceDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsReceivableInvoiceDistribution
                {
                    ConsReceivableInvoiceDistributionId = distribution.ConsReceivableInvoiceDistributionId,
                    ConsReceivableInvoiceId = distribution.ConsReceivableInvoiceId,
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

                db.ConsReceivableInvoiceDistribution.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { distribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvoiceDistribution_Destroy([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoiceDistribution distribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsReceivableInvoiceDistribution
                {
                    ConsReceivableInvoiceDistributionId = distribution.ConsReceivableInvoiceDistributionId,
                    ConsReceivableInvoiceId = distribution.ConsReceivableInvoiceId,
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

                db.ConsReceivableInvoiceDistribution.Attach(entity);
                db.ConsReceivableInvoiceDistribution.Remove(entity);
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