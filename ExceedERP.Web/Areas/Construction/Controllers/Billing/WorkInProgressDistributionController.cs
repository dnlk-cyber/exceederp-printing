using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class WorkInProgressDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult WorkInProgressDistribution_Read([DataSourceRequest]DataSourceRequest request, int wipId)
        {
            IQueryable<WorkInProgressDistribution> workInProgressDistributions = db.WorkInProgressDistribution.Where(d => d.WorkInProgressId == wipId).OrderByDescending(d => d.Debit);
            DataSourceResult result = workInProgressDistributions.ToDataSourceResult(request, progressDistribution => new
            {
                WorkInProgressDistributionId = progressDistribution.WorkInProgressDistributionId,
                WorkInProgressId = progressDistribution.WorkInProgressId,
                WorkInProgressWorkItemId = progressDistribution.WorkInProgressWorkItemId,
                AccountCode = progressDistribution.AccountCode,
                AccountDesc = progressDistribution.AccountDesc,
                Credit = progressDistribution.Credit,
                Debit = progressDistribution.Debit,
                

                OrgBranchName = progressDistribution.OrgBranchName,
                DateCreated = progressDistribution.DateCreated,
                LastModified = progressDistribution.LastModified,
                CreatedBy = progressDistribution.CreatedBy,
                ModifiedBy = progressDistribution.ModifiedBy
            });

            return Json(result);
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkInProgressDistribution_Create([DataSourceRequest]DataSourceRequest request, WorkInProgressDistribution progressDistribution, int wipId)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(projectIdCreateAdvance))
                //{
                //    ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                //}
                //else
                //{
                var entity = new WorkInProgressDistribution
                {
                    WorkInProgressDistributionId = progressDistribution.WorkInProgressDistributionId,
                    WorkInProgressId = wipId,
                    WorkInProgressWorkItemId = progressDistribution.WorkInProgressWorkItemId,
                    AccountCode = progressDistribution.AccountCode,
                    AccountDesc = progressDistribution.AccountDesc,
                    Credit = progressDistribution.Credit,
                    Debit = progressDistribution.Debit,

                    OrgBranchName = progressDistribution.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = progressDistribution.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = progressDistribution.ModifiedBy
                };
                //entity.SubTotal = entity.Quantity * entity.UnitPrice;
                //entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                //entity.TotalAmount = entity.TaxAmount + entity.SubTotal;

                db.WorkInProgressDistribution.Add(entity);
                db.SaveChanges();
                progressDistribution.WorkInProgressDistributionId = entity.WorkInProgressDistributionId;

               

                //}
            }

            return Json(new[] { progressDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkInProgressDistribution_Update([DataSourceRequest]DataSourceRequest request, WorkInProgressDistribution progressDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkInProgressDistribution
                {
                    WorkInProgressDistributionId = progressDistribution.WorkInProgressDistributionId,
                    WorkInProgressId = progressDistribution.WorkInProgressId,
                    WorkInProgressWorkItemId = progressDistribution.WorkInProgressWorkItemId,
                    AccountCode = progressDistribution.AccountCode,
                    AccountDesc = progressDistribution.AccountDesc,
                    Credit = progressDistribution.Credit,
                    Debit = progressDistribution.Debit,

                    OrgBranchName = progressDistribution.OrgBranchName,
                    DateCreated = progressDistribution.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = progressDistribution.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                //entity.SubTotal = entity.Quantity * entity.UnitPrice;
                //entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                //entity.TotalAmount = entity.TaxAmount + entity.SubTotal;

                db.WorkInProgressDistribution.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                
            }

            return Json(new[] { progressDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkInProgressDistribution_Destroy([DataSourceRequest]DataSourceRequest request, WorkInProgressDistribution progressDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkInProgressDistribution
                {
                    WorkInProgressDistributionId = progressDistribution.WorkInProgressDistributionId,
                    WorkInProgressId = progressDistribution.WorkInProgressId,
                    WorkInProgressWorkItemId = progressDistribution.WorkInProgressWorkItemId,
                    AccountCode = progressDistribution.AccountCode,
                    AccountDesc = progressDistribution.AccountDesc,
                    Credit = progressDistribution.Credit,
                    Debit = progressDistribution.Debit,

                    OrgBranchName = progressDistribution.OrgBranchName,
                    DateCreated = progressDistribution.DateCreated,
                    LastModified = progressDistribution.LastModified,
                    CreatedBy = progressDistribution.CreatedBy,
                    ModifiedBy = progressDistribution.ModifiedBy
                };

                db.WorkInProgressDistribution.Attach(entity);
                db.WorkInProgressDistribution.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { progressDistribution }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}