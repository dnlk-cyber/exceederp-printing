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

namespace ExceedERP.Web.Areas.Production.Controllers.Settings
{
    public class InputMaterialApplicationRateController : Controller
    {
        // GET: Production/InputMaterialApplicationRate
        ExceedDbContext db;
        public InputMaterialApplicationRateController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            ViewData["ItemCategorys"] = db.ItemCategorys.ToList();
            ViewData["Items"] = db.InventoryItems.ToList();
            return View();
        }
        public ActionResult RateHeaders_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InputMaterialApplicationRate> rates = db.InputMaterialApplicationRates;
            DataSourceResult result = rates.ToDataSourceResult(request, rateHeader => new
            {
                InputMaterialApplicationRateId = rateHeader.InputMaterialApplicationRateId,
                ItemCode = rateHeader.ItemCode,
                ItemCategoryCode = rateHeader.ItemCategoryCode,
                ApplicationPlace = rateHeader.ApplicationPlace,

                OrgBranchName = rateHeader.OrgBranchName,
                DateCreated = rateHeader.DateCreated,
                LastModified = rateHeader.LastModified,
                CreatedBy = rateHeader.CreatedBy,
                ModifiedBy = rateHeader.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(InputMaterialApplicationRate model) {
            return db.InputMaterialApplicationRates
                .FirstOrDefault(mr => mr.ItemCode == model.ItemCode && 
                                mr.ApplicationPlace == model.ApplicationPlace && 
                                mr.InputMaterialApplicationRateId != model.InputMaterialApplicationRateId)
                                != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RateHeader_Create([DataSourceRequest]DataSourceRequest request, InputMaterialApplicationRate rateHeader)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(rateHeader))
                {
                    ModelState.AddModelError("", "This item exists already!");
                }
                else
                {
                    var entity = new InputMaterialApplicationRate
                    {
                        InputMaterialApplicationRateId = rateHeader.InputMaterialApplicationRateId,
                        ItemCode = rateHeader.ItemCode,
                        ItemCategoryCode = rateHeader.ItemCategoryCode,
                        ApplicationPlace = rateHeader.ApplicationPlace,

                        OrgBranchName = rateHeader.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = rateHeader.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = rateHeader.ModifiedBy,
                    };

                    db.InputMaterialApplicationRates.Add(entity);
                    db.SaveChanges();

                    rateHeader.InputMaterialApplicationRateId = entity.InputMaterialApplicationRateId;
                }
            }

            return Json(new[] { rateHeader }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RateHeader_Update([DataSourceRequest]DataSourceRequest request, InputMaterialApplicationRate rateHeader)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(rateHeader))
                {
                    ModelState.AddModelError("", "This item exists already!");
                }
                else
                {
                    var entity = new InputMaterialApplicationRate
                    {
                        InputMaterialApplicationRateId = rateHeader.InputMaterialApplicationRateId,
                        ItemCode = rateHeader.ItemCode,
                        ItemCategoryCode = rateHeader.ItemCategoryCode,
                        ApplicationPlace = rateHeader.ApplicationPlace,

                        OrgBranchName = rateHeader.OrgBranchName,
                        DateCreated = rateHeader.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = rateHeader.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.InputMaterialApplicationRates.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }

            return Json(new[] { rateHeader }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RateHeader_Destroy([DataSourceRequest]DataSourceRequest request, InputMaterialApplicationRate rateHeader)
        {
            if (ModelState.IsValid)
            {
                var entity = new InputMaterialApplicationRate
                {
                    InputMaterialApplicationRateId = rateHeader.InputMaterialApplicationRateId,
                    ItemCode = rateHeader.ItemCode,
                    ItemCategoryCode = rateHeader.ItemCategoryCode,
                    ApplicationPlace = rateHeader.ApplicationPlace,

                    OrgBranchName = rateHeader.OrgBranchName,
                    DateCreated = rateHeader.DateCreated,
                    LastModified = rateHeader.LastModified,
                    CreatedBy = rateHeader.CreatedBy,
                    ModifiedBy = rateHeader.ModifiedBy,

                };

                db.InputMaterialApplicationRates.Attach(entity);
                db.InputMaterialApplicationRates.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { rateHeader }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}