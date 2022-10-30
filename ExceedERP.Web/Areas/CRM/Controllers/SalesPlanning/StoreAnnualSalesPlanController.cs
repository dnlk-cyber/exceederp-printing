using System;
using System.Collections.Generic;
//using System.Data
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers.SalesPlanning
{
    [AuthorizeRoles(CRMRoles.SalesPlanApprover, CRMRoles.SalesPlanUser)]
    public class StoreAnnualSalesPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StoreAnnualPlans_Read([DataSourceRequest]DataSourceRequest request, int branchPlanId)
        {
            List<StoreAnnualSalesPlan> storePlans = db.StoreAnnualSalesPlans.Where(sp => sp.BranchAnnualSalesPlanId == branchPlanId).ToList();
            //if (!storePlans.Any())
            //{
            //    var headerRecord = db.BranchAnnualSalesPlans.Find(branchPlanId);
            //    if (headerRecord != null)
            //    {
            //        //var stores = db.InventoryStores.Where(s => s.BranchId == headerRecord.BranchId).ToList();
            //        var stores = db.SalesStores.Where(s => s.InventoryStore.BranchId == headerRecord.BranchId).ToList();

            //        if (stores.Any())
            //        {
            //            var ratio = ((decimal)100) / stores.Count;
            //            var qty = headerRecord.Quantity * ratio / 100;
            //            var value = qty * headerRecord.AnnualSalesPlan.AveragePrice;
            //            foreach(SalesStore store in stores)
            //            {
            //                var record = new StoreAnnualSalesPlan
            //                {
            //                    //StoreCode = store.StoreCode,
            //                    SalesStoreId = store.SalesStoreId,
            //                    BranchAnnualSalesPlanId = headerRecord.BranchAnnualSalesPlanId,
            //                    Ratio = ratio,
            //                    AnnualQuantity = qty,
            //                    AnnualValue = value,

            //                    CreatedBy = User.Identity.Name,
            //                    DateCreated = DateTime.Now,

            //                };
            //                db.StoreAnnualSalesPlans.Add(record);
            //                db.SaveChanges();

            //                storePlans.Add(record);
            //            }
            //        }
            //    }
            //}
            DataSourceResult result = storePlans.AsQueryable().ToDataSourceResult(request, storePlan => new
            {
                StoreAnnualSalesPlanId = storePlan.StoreAnnualSalesPlanId,
                BranchAnnualSalesPlanId = storePlan.BranchAnnualSalesPlanId,
                //StoreCode = storePlan.StoreCode,
                SalesStoreId = storePlan.SalesStoreId,
                Ratio = storePlan.Ratio,
                AnnualQuantity = storePlan.AnnualQuantity,
                AnnualValue = storePlan.AnnualValue,

                IsReadonly = storePlan.BranchAnnualSalesPlan.AnnualSalesPlan.IsOnlineApproved,

                OrgBranchName = storePlan.OrgBranchName,
                DateCreated = storePlan.DateCreated,
                LastModified = storePlan.LastModified,
                CreatedBy = storePlan.CreatedBy,
                ModifiedBy = storePlan.ModifiedBy
            });

            return Json(result);
        }
        private bool DuplicateDataExists(StoreAnnualSalesPlan storeAnnualSalesPlan, int headerId)
        {
            return db.StoreAnnualSalesPlans.Any(p => p.BranchAnnualSalesPlanId == headerId && p.SalesStoreId == storeAnnualSalesPlan.SalesStoreId && p.StoreAnnualSalesPlanId != storeAnnualSalesPlan.StoreAnnualSalesPlanId);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreAnnualPlan_Create([DataSourceRequest]DataSourceRequest request, StoreAnnualSalesPlan storePlan, int branchPlanId)
        {
            if(DuplicateDataExists(storePlan, branchPlanId))
            {
                ModelState.AddModelError("", "Plan for this store exists already.");
            }
            if (ModelState.IsValid)
            {
                var entity = new StoreAnnualSalesPlan
                {
                    StoreAnnualSalesPlanId = storePlan.StoreAnnualSalesPlanId,
                    BranchAnnualSalesPlanId = branchPlanId,
                    //StoreCode = storePlan.StoreCode,
                    SalesStoreId = storePlan.SalesStoreId,
                    Ratio = storePlan.Ratio,
                    AnnualQuantity = storePlan.AnnualQuantity,
                    AnnualValue = storePlan.AnnualValue,

                    OrgBranchName = storePlan.OrgBranchName,
                    DateCreated = storePlan.DateCreated,
                    LastModified = storePlan.LastModified,
                    CreatedBy = storePlan.CreatedBy,
                    ModifiedBy = storePlan.ModifiedBy
                };
                var headerBranchPlan = db.BranchAnnualSalesPlans.Find(entity.BranchAnnualSalesPlanId);
                if (headerBranchPlan != null)
                {
                    entity.AnnualQuantity = headerBranchPlan.Quantity * entity.Ratio / 100;
                    entity.AnnualValue = headerBranchPlan.Value * entity.Ratio / 100;
                }

                db.StoreAnnualSalesPlans.Add(entity);
                db.SaveChanges();
                storePlan.StoreAnnualSalesPlanId = entity.StoreAnnualSalesPlanId;

                CreateStoreMonthlyPlans(entity);
            }

            return Json(new[] { storePlan }.ToDataSourceResult(request, ModelState));
        }

        private void CreateStoreMonthlyPlans(StoreAnnualSalesPlan headerRecord)
        {
            var year = db.GLFiscalYears.Find(headerRecord.BranchAnnualSalesPlan.AnnualSalesPlan.GlFiscalYearId);
            if (year != null)
            {
                var crmPeriods = db.CRMPeriods.Where(s => s.DateFrom >= year.DateFrom && s.DateFrom <= year.DateTo).ToList();
                //var num = crmPeriods.Any() ? crmPeriods.Count : 0;
                //var qty = num > 0 ? headerRecord.AnnualQuantity / num : 0;
                //var price = headerRecord.BranchAnnualSalesPlan.AnnualSalesPlan.AveragePrice;
                foreach (var period in crmPeriods)
                {
                    var record = new StoreMonthlySalesPlan
                    {
                        CRMPeriodId = period.CRMPeriodId,

                        StoreAnnualSalesPlanId = headerRecord.StoreAnnualSalesPlanId,
                        Quantity = headerRecord.AnnualQuantity/12,
                        Value = headerRecord.AnnualValue/12,


                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };
                    db.StoreMonthlySalesPlans.Add(record);
                }
                db.SaveChanges();

            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreAnnualPlan_Update([DataSourceRequest]DataSourceRequest request, StoreAnnualSalesPlan storePlan)
        {
            if (DuplicateDataExists(storePlan, storePlan.BranchAnnualSalesPlanId))
            {
                ModelState.AddModelError("", "Plan for this store exists already.");
            }
            if (ModelState.IsValid)
            {
                var entity = new StoreAnnualSalesPlan
                {
                    StoreAnnualSalesPlanId = storePlan.StoreAnnualSalesPlanId,
                    BranchAnnualSalesPlanId = storePlan.BranchAnnualSalesPlanId,
                    //StoreCode = storePlan.StoreCode,
                    SalesStoreId = storePlan.SalesStoreId,
                    Ratio = storePlan.Ratio,
                    AnnualQuantity = storePlan.AnnualQuantity,
                    AnnualValue = storePlan.AnnualValue,

                    OrgBranchName = storePlan.OrgBranchName,
                    DateCreated = storePlan.DateCreated,
                    LastModified = storePlan.LastModified,
                    CreatedBy = storePlan.CreatedBy,
                    ModifiedBy = storePlan.ModifiedBy
                };
                var headerBranchPlan = db.BranchAnnualSalesPlans.Find(entity.BranchAnnualSalesPlanId);
                if (headerBranchPlan != null)
                {
                    entity.AnnualQuantity = headerBranchPlan.Quantity * entity.Ratio / 100;
                    entity.AnnualValue = headerBranchPlan.Value * entity.Ratio / 100;
                }
                db.StoreAnnualSalesPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                new CRMSalesPlanHelper().UpdateStoreMonthlyPlanOnStoreAnnualPlanUpdate(entity);
            }

            return Json(new[] { storePlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreAnnualPlan_Destroy([DataSourceRequest]DataSourceRequest request, StoreAnnualSalesPlan storePlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreAnnualSalesPlan
                {
                    StoreAnnualSalesPlanId = storePlan.StoreAnnualSalesPlanId,
                    BranchAnnualSalesPlanId = storePlan.BranchAnnualSalesPlanId,
                    //StoreCode = storePlan.StoreCode,
                    SalesStoreId = storePlan.SalesStoreId,
                    Ratio = storePlan.Ratio,
                    AnnualQuantity = storePlan.AnnualQuantity,
                    AnnualValue = storePlan.AnnualValue,

                    OrgBranchName = storePlan.OrgBranchName,
                    DateCreated = storePlan.DateCreated,
                    LastModified = storePlan.LastModified,
                    CreatedBy = storePlan.CreatedBy,
                    ModifiedBy = storePlan.ModifiedBy
                };

                db.StoreAnnualSalesPlans.Attach(entity);
                db.StoreAnnualSalesPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { storePlan }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
