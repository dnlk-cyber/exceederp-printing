using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.BudgetAndCashFlow
{
    public class RawProductPurchaseController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public RawProductPurchaseController()
        {
            db = new ExceedDbContext();
        }
       
        public ActionResult Purchases_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RawProductPurchase> purchases = db.RawProductPurchases;
            DataSourceResult result = purchases.ToDataSourceResult(request, purchase => new 
            {
                RawProductPurchaseId = purchase.RawProductPurchaseId,
                BranchId = purchase.BranchId,
                GlFiscalYearId = purchase.GlFiscalYearId,                

                OrgBranchName = purchase.OrgBranchName,
                DateCreated = purchase.DateCreated,
                LastModified = purchase.LastModified,
                CreatedBy = purchase.CreatedBy,
                ModifiedBy = purchase.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(RawProductPurchase record)
        {
            return db.RawProductPurchases
                .FirstOrDefault(i => i.BranchId == record.BranchId && i.GlFiscalYearId == record.GlFiscalYearId && i.RawProductPurchaseId != record.RawProductPurchaseId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Create([DataSourceRequest]DataSourceRequest request, RawProductPurchase purchase)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(purchase))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new RawProductPurchase
                    {
                        RawProductPurchaseId = purchase.RawProductPurchaseId,
                        BranchId = purchase.BranchId,
                        GlFiscalYearId = purchase.GlFiscalYearId,

                        OrgBranchName = purchase.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = purchase.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = purchase.ModifiedBy,
                    };

                    db.RawProductPurchases.Add(entity);
                    db.SaveChanges();

                    purchase.RawProductPurchaseId = entity.RawProductPurchaseId;

                    //this.CreateMonthlyCashFlowItems(entity);
                }
            }

            return Json(new[] { purchase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Update([DataSourceRequest]DataSourceRequest request, RawProductPurchase purchase)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(purchase))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new RawProductPurchase
                    {
                        RawProductPurchaseId = purchase.RawProductPurchaseId,
                        BranchId = purchase.BranchId,
                        GlFiscalYearId = purchase.GlFiscalYearId,

                        OrgBranchName = purchase.OrgBranchName,
                        DateCreated = purchase.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = purchase.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.RawProductPurchases.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { purchase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Destroy([DataSourceRequest]DataSourceRequest request, RawProductPurchase purchase)
        {
            if (ModelState.IsValid)
            {
                var entity = new RawProductPurchase
                {
                    RawProductPurchaseId = purchase.RawProductPurchaseId,
                    BranchId = purchase.BranchId,
                    GlFiscalYearId = purchase.GlFiscalYearId,

                    OrgBranchName = purchase.OrgBranchName,
                    DateCreated = purchase.DateCreated,
                    LastModified = purchase.LastModified,
                    CreatedBy = purchase.CreatedBy,
                    ModifiedBy = purchase.ModifiedBy,

                };

                db.RawProductPurchases.Attach(entity);
                db.RawProductPurchases.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { purchase }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}