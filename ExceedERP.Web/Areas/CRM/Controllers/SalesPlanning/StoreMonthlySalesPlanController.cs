using System.Collections.Generic;
//using System.Data
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers.SalesPlanning
{
    [AuthorizeRoles(CRMRoles.SalesPlanApprover, CRMRoles.SalesPlanUser)]
    public class StoreMonthlySalesPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StorePlans_Read([DataSourceRequest]DataSourceRequest request, int annualStorePlanId)
        {
            List<StoreMonthlySalesPlan> storePlans = db.StoreMonthlySalesPlans.Where(sp => sp.StoreAnnualSalesPlanId == annualStorePlanId).ToList();
            if (!storePlans.Any())
            {
                var headerRecord = db.StoreAnnualSalesPlans.Find(annualStorePlanId);
                if (headerRecord != null)
                {
                    var year = db.GLFiscalYears.Find(headerRecord.BranchAnnualSalesPlan.AnnualSalesPlan.GlFiscalYearId);
                    if(year != null)
                    {
                        var crmPeriods = db.CRMPeriods.Where(s => s.DateFrom >= year.DateFrom && s.DateFrom <= year.DateTo).ToList();
                        var num = crmPeriods.Any() ? crmPeriods.Count : 0;
                        var qty = num > 0 ? headerRecord.AnnualQuantity / num : 0;
                        var price = headerRecord.BranchAnnualSalesPlan.AnnualSalesPlan.AveragePrice;
                        foreach (var period in crmPeriods)
                        {
                            var record = new StoreMonthlySalesPlan
                            {
                                CRMPeriodId = period.CRMPeriodId,

                                StoreAnnualSalesPlanId = headerRecord.StoreAnnualSalesPlanId,
                                Quantity = qty,
                                Value = qty * price,
                                

                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name
                            };
                            storePlans.Add(record);
                            db.StoreMonthlySalesPlans.Add(record);
                        }
                        db.SaveChanges();

                    }
                    
                }
            }
            DataSourceResult result = storePlans.AsQueryable().ToDataSourceResult(request, storePlan => new
            {
                StoreMonthlySalesPlanId = storePlan.StoreMonthlySalesPlanId,
                StoreAnnualSalesPlanId = storePlan.StoreAnnualSalesPlanId,
                CRMPeriodId = storePlan.CRMPeriodId,
                Quantity = storePlan.Quantity,
                Value = storePlan.Value,

                OrgBranchName = storePlan.OrgBranchName,
                DateCreated = storePlan.DateCreated,
                LastModified = storePlan.LastModified,
                CreatedBy = storePlan.CreatedBy,
                ModifiedBy = storePlan.ModifiedBy
            });

            return Json(result);
        }
        private bool DuplicateDataExists(StoreMonthlySalesPlan storePlan, int storeAnnualPlanId)
        {
            return db.StoreMonthlySalesPlans.Any(p => p.CRMPeriodId == storePlan.CRMPeriodId && p.StoreAnnualSalesPlanId == storeAnnualPlanId && p.StoreMonthlySalesPlanId == storePlan.StoreMonthlySalesPlanId);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StorePlan_Create([DataSourceRequest]DataSourceRequest request, StoreMonthlySalesPlan storePlan, int annualStorePlanId)
        {
            if(DuplicateDataExists(storePlan, annualStorePlanId))
            {
                ModelState.AddModelError("", "Plan for this store and period exists already");
            }
            if (ModelState.IsValid)
            {
                var entity = new StoreMonthlySalesPlan
                {
                    StoreMonthlySalesPlanId = storePlan.StoreMonthlySalesPlanId,
                    StoreAnnualSalesPlanId = annualStorePlanId,
                    CRMPeriodId = storePlan.CRMPeriodId,
                    Quantity = storePlan.Quantity,
                    Value = storePlan.Value,

                    OrgBranchName = storePlan.OrgBranchName,
                    DateCreated = storePlan.DateCreated,
                    LastModified = storePlan.LastModified,
                    CreatedBy = storePlan.CreatedBy,
                    ModifiedBy = storePlan.ModifiedBy
                };
                //var headerBranchPlan = db.BranchAnnualSalesPlans.Find(entity.BranchAnnualSalesPlanId);
                //if (headerBranchPlan != null)
                //{
                //    entity.AnnualQuantity = headerBranchPlan.Quantity * entity.Ratio / 100;
                //    entity.AnnualValue = entity.AnnualQuantity * headerBranchPlan.AnnualSalesPlan.AveragePrice;
                //}
                db.StoreMonthlySalesPlans.Add(entity);
                db.SaveChanges();
                storePlan.StoreAnnualSalesPlanId = entity.StoreAnnualSalesPlanId;
            }

            return Json(new[] { storePlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StorePlan_Update([DataSourceRequest]DataSourceRequest request, StoreMonthlySalesPlan storePlan)
        {
            if (DuplicateDataExists(storePlan, storePlan.StoreAnnualSalesPlanId))
            {
                ModelState.AddModelError("", "Plan for this store and period exists already");
              }
            if (ModelState.IsValid)
            {
                var entity = new StoreMonthlySalesPlan
                {
                    StoreMonthlySalesPlanId = storePlan.StoreMonthlySalesPlanId,
                    StoreAnnualSalesPlanId = storePlan.StoreAnnualSalesPlanId,
                    CRMPeriodId = storePlan.CRMPeriodId,
                    Quantity = storePlan.Quantity,
                    Value = storePlan.Value,

                    OrgBranchName = storePlan.OrgBranchName,
                    DateCreated = storePlan.DateCreated,
                    LastModified = storePlan.LastModified,
                    CreatedBy = storePlan.CreatedBy,
                    ModifiedBy = storePlan.ModifiedBy
                };
                //var headerBranchPlan = db.BranchAnnualSalesPlans.Find(entity.BranchAnnualSalesPlanId);
                //if (headerBranchPlan != null)
                //{
                //    entity.AnnualQuantity = headerBranchPlan.Quantity * entity.Ratio / 100;
                //    entity.AnnualValue = entity.AnnualQuantity * headerBranchPlan.AnnualSalesPlan.AveragePrice;
                //}
                db.StoreMonthlySalesPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { storePlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StorePlan_Destroy([DataSourceRequest]DataSourceRequest request, StoreMonthlySalesPlan storePlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreMonthlySalesPlan
                {
                    StoreMonthlySalesPlanId = storePlan.StoreMonthlySalesPlanId,
                    StoreAnnualSalesPlanId = storePlan.StoreAnnualSalesPlanId,
                    CRMPeriodId = storePlan.CRMPeriodId,
                    Quantity = storePlan.Quantity,
                    Value = storePlan.Value,

                    OrgBranchName = storePlan.OrgBranchName,
                    DateCreated = storePlan.DateCreated,
                    LastModified = storePlan.LastModified,
                    CreatedBy = storePlan.CreatedBy,
                    ModifiedBy = storePlan.ModifiedBy
                };

                db.StoreMonthlySalesPlans.Attach(entity);
                db.StoreMonthlySalesPlans.Remove(entity);
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
