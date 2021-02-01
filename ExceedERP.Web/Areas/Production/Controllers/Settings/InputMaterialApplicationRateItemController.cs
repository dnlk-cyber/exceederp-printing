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
    public class InputMaterialApplicationRateItemController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public InputMaterialApplicationRateItemController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult RateItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var parentItem = db.InputMaterialApplicationRates.Find(id);
            var unit = "unit";
            if(parentItem != null)
            {
                unit = InventoryHelper.GetUnitOfMeasurment(parentItem.ItemCode);
            }
            IQueryable<InputMaterialApplicationRateItem> rates = db.InputMaterialApplicationRateItems.Where(itm => itm.InputMaterialApplicationRateId == id);
            DataSourceResult result = rates.ToDataSourceResult(request, rateItem => new InputMaterialApplicationRateItemViewModel
            {
                InputMaterialApplicationRateItemId = rateItem.InputMaterialApplicationRateItemId,
                InputMaterialApplicationRateId = rateItem.InputMaterialApplicationRateId,
                ItemCode = rateItem.ItemCode,
                ItemCategoryCode = rateItem.ItemCategoryCode,
                StandardRate = rateItem.StandardRate,
                Remark = rateItem.Remark,
                HeaderUoM = unit,
                UoM = InventoryHelper.GetUnitOfMeasurment(rateItem.ItemCode),

                OrgBranchName = rateItem.OrgBranchName,
                DateCreated = rateItem.DateCreated,
                LastModified = rateItem.LastModified,
                CreatedBy = rateItem.CreatedBy,
                ModifiedBy = rateItem.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RateItem_Create([DataSourceRequest]DataSourceRequest request, InputMaterialApplicationRateItemViewModel rateItem, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new InputMaterialApplicationRateItem
                {
                    InputMaterialApplicationRateItemId = rateItem.InputMaterialApplicationRateItemId,
                    InputMaterialApplicationRateId = id,
                    ItemCode = rateItem.ItemCode,
                    ItemCategoryCode = rateItem.ItemCategoryCode,
                    StandardRate = rateItem.StandardRate,
                    Remark = rateItem.Remark,

                    OrgBranchName = rateItem.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = rateItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = rateItem.ModifiedBy,

                };

                db.InputMaterialApplicationRateItems.Add(entity);
                db.SaveChanges();

                rateItem.InputMaterialApplicationRateItemId = entity.InputMaterialApplicationRateItemId;

            }

            return Json(new[] { rateItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RateItem_Update([DataSourceRequest]DataSourceRequest request, InputMaterialApplicationRateItemViewModel rateItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new InputMaterialApplicationRateItem
                {
                    InputMaterialApplicationRateItemId = rateItem.InputMaterialApplicationRateItemId,
                    InputMaterialApplicationRateId = rateItem.InputMaterialApplicationRateId,
                    ItemCode = rateItem.ItemCode,
                    ItemCategoryCode = rateItem.ItemCategoryCode,
                    StandardRate = rateItem.StandardRate,
                    Remark = rateItem.Remark,

                    OrgBranchName = rateItem.OrgBranchName,
                    DateCreated = rateItem.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = rateItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.InputMaterialApplicationRateItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { rateItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RateItem_Destroy([DataSourceRequest]DataSourceRequest request, InputMaterialApplicationRateItemViewModel rateItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new InputMaterialApplicationRateItem
                {
                    InputMaterialApplicationRateItemId = rateItem.InputMaterialApplicationRateItemId,
                    InputMaterialApplicationRateId = rateItem.InputMaterialApplicationRateId,
                    ItemCode = rateItem.ItemCode,
                    ItemCategoryCode = rateItem.ItemCategoryCode,
                    StandardRate = rateItem.StandardRate,
                    Remark = rateItem.Remark,

                    OrgBranchName = rateItem.OrgBranchName,
                    DateCreated = rateItem.DateCreated,
                    LastModified = rateItem.LastModified,
                    CreatedBy = rateItem.CreatedBy,
                    ModifiedBy = rateItem.ModifiedBy,

                };

                db.InputMaterialApplicationRateItems.Attach(entity);
                db.InputMaterialApplicationRateItems.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { rateItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}