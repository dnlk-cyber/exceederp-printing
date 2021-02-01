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
    public class RawProductFinishedGoodMappingController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public RawProductFinishedGoodMappingController()
        {
            db = new ExceedDbContext();
        }
       
        public ActionResult Mappings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RawProductFinishedGoodMapping> mappings = db.RawProductFinishedGoodMapping;
            DataSourceResult result = mappings.ToDataSourceResult(request, mapping => new RawProductFinishedGoodMapping
            {
                RawProductFinishedGoodMappingId = mapping.RawProductFinishedGoodMappingId,
                RawItemCode = mapping.RawItemCode,
                CleanedItemCode = mapping.CleanedItemCode,
                ByproductItemCode = mapping.ByproductItemCode,
                ItemCategoryCode = mapping.ItemCategoryCode,
                Remark = mapping.Remark,

                OrgBranchName = mapping.OrgBranchName,
                DateCreated = mapping.DateCreated,
                LastModified = mapping.LastModified,
                CreatedBy = mapping.CreatedBy,
                ModifiedBy = mapping.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Mapping_Create([DataSourceRequest]DataSourceRequest request, RawProductFinishedGoodMapping mapping)
        {
            if (ModelState.IsValid)
            {
                var entity = new RawProductFinishedGoodMapping
                {
                    RawProductFinishedGoodMappingId = mapping.RawProductFinishedGoodMappingId,
                    RawItemCode = mapping.RawItemCode,
                    CleanedItemCode = mapping.CleanedItemCode,
                    ByproductItemCode = mapping.ByproductItemCode,
                    ItemCategoryCode = mapping.ItemCategoryCode,
                    Remark = mapping.Remark,

                    OrgBranchName = mapping.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = mapping.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = mapping.ModifiedBy,

                };

                db.RawProductFinishedGoodMapping.Add(entity);
                db.SaveChanges();

                mapping.RawProductFinishedGoodMappingId = entity.RawProductFinishedGoodMappingId;

            }

            return Json(new[] { mapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Mapping_Update([DataSourceRequest]DataSourceRequest request, RawProductFinishedGoodMapping mapping)
        {
            if (ModelState.IsValid)
            {
                var entity = new RawProductFinishedGoodMapping
                {
                    RawProductFinishedGoodMappingId = mapping.RawProductFinishedGoodMappingId,
                    RawItemCode = mapping.RawItemCode,
                    CleanedItemCode = mapping.CleanedItemCode,
                    ByproductItemCode = mapping.ByproductItemCode,
                    ItemCategoryCode = mapping.ItemCategoryCode,
                    Remark = mapping.Remark,

                    OrgBranchName = mapping.OrgBranchName,
                    DateCreated = mapping.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = mapping.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.RawProductFinishedGoodMapping.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { mapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Mapping_Destroy([DataSourceRequest]DataSourceRequest request, RawProductFinishedGoodMapping mapping)
        {
            if (ModelState.IsValid)
            {
                var entity = new RawProductFinishedGoodMapping
                {
                    RawProductFinishedGoodMappingId = mapping.RawProductFinishedGoodMappingId,
                    RawItemCode = mapping.RawItemCode,
                    CleanedItemCode = mapping.CleanedItemCode,
                    ByproductItemCode = mapping.ByproductItemCode,
                    ItemCategoryCode = mapping.ItemCategoryCode,
                    Remark = mapping.Remark,

                    OrgBranchName = mapping.OrgBranchName,
                    DateCreated = mapping.DateCreated,
                    LastModified = mapping.LastModified,
                    CreatedBy = mapping.CreatedBy,
                    ModifiedBy = mapping.ModifiedBy,

                };

                db.RawProductFinishedGoodMapping.Attach(entity);
                db.RawProductFinishedGoodMapping.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { mapping }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}