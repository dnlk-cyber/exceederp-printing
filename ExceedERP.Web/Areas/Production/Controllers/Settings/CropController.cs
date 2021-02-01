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
    public class CropController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public CropController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name}).ToList();
            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode}).ToList();
            return View();
        }
       
        public ActionResult Crops_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Crop> crops = db.Crops;
            DataSourceResult result = crops.ToDataSourceResult(request, crop => new CropViewModel
            {
                CropId = crop.CropId,
                Name = crop.Name,
                ItemCode = crop.ItemCode,
                ItemCategoryCode = crop.ItemCategoryCode,
                UoM = InventoryHelper.GetUnitOfMeasurment(crop.ItemCode),
                Description = crop.Description,

                OrgBranchName = crop.OrgBranchName,
                DateCreated = crop.DateCreated,
                LastModified = crop.LastModified,
                CreatedBy = crop.CreatedBy,
                ModifiedBy = crop.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Crop_Create([DataSourceRequest]DataSourceRequest request, CropViewModel crop)
        {
            if (ModelState.IsValid)
            {
                var entity = new Crop
                {
                    Name = ProductionSettingHelper.GetInventoryItemName(crop.ItemCode),
                    ItemCode = crop.ItemCode,
                    ItemCategoryCode = crop.ItemCategoryCode,
                    Description = crop.Description,

                    OrgBranchName = crop.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = crop.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = crop.ModifiedBy,

                };

                db.Crops.Add(entity);
                db.SaveChanges();

                crop.CropId = entity.CropId;

            }

            return Json(new[] { crop }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Crop_Update([DataSourceRequest]DataSourceRequest request, CropViewModel crop)
        {
            if (ModelState.IsValid)
            {
                var entity = new Crop
                {
                    CropId = crop.CropId,
                    Name = ProductionSettingHelper.GetInventoryItemName(crop.ItemCode),
                    ItemCode = crop.ItemCode,
                    ItemCategoryCode = crop.ItemCategoryCode,
                    Description = crop.Description,

                    OrgBranchName = crop.OrgBranchName,
                    DateCreated = crop.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = crop.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.Crops.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { crop }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Crop_Destroy([DataSourceRequest]DataSourceRequest request, CropViewModel crop)
        {
            if (ModelState.IsValid)
            {
                var entity = new Crop
                {
                    CropId = crop.CropId,
                    Name = crop.Name,
                    ItemCode = crop.ItemCode,
                    ItemCategoryCode = crop.ItemCategoryCode,
                    Description = crop.Description,

                    OrgBranchName = crop.OrgBranchName,
                    DateCreated = crop.DateCreated,
                    LastModified = crop.LastModified,
                    CreatedBy = crop.CreatedBy,
                    ModifiedBy = crop.ModifiedBy,

                };

                db.Crops.Attach(entity);
                db.Crops.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { crop }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}