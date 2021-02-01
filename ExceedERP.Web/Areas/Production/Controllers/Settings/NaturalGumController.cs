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
    public class NaturalGumController : Controller
    {
        // GET: Production/NaturalGum
        ExceedDbContext db;
        public NaturalGumController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name }).ToList();
            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode }).ToList();
            return View();
        }
        public ActionResult Gums_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<NaturalGum> gums = db.NaturalGums;
            DataSourceResult result = gums.ToDataSourceResult(request, gum => new NaturalGumViewModel
            {
                NaturalGumId = gum.NaturalGumId,
                Name = gum.Name,
                ItemCode = gum.ItemCode,
                ItemCategoryCode = gum.ItemCategoryCode,
                UoM = InventoryHelper.GetUnitOfMeasurment(gum.ItemCode),
                Description = gum.Description,

                OrgBranchName = gum.OrgBranchName,
                DateCreated = gum.DateCreated,
                LastModified = gum.LastModified,
                CreatedBy = gum.CreatedBy,
                ModifiedBy = gum.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Gum_Create([DataSourceRequest]DataSourceRequest request, NaturalGumViewModel gum)
        {
            if (ModelState.IsValid)
            {
                var entity = new NaturalGum
                {
                    Name = ProductionSettingHelper.GetInventoryItemName(gum.ItemCode),
                    ItemCode = gum.ItemCode,
                    ItemCategoryCode = gum.ItemCategoryCode,
                    Description = gum.Description,

                    OrgBranchName = gum.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = gum.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gum.ModifiedBy,

                };

                db.NaturalGums.Add(entity);
                db.SaveChanges();

                gum.NaturalGumId = entity.NaturalGumId;

            }

            return Json(new[] { gum }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Gum_Update([DataSourceRequest]DataSourceRequest request, NaturalGumViewModel gum)
        {
            if (ModelState.IsValid)
            {
                var entity = new NaturalGum
                {
                    NaturalGumId = gum.NaturalGumId,
                    Name = ProductionSettingHelper.GetInventoryItemName(gum.ItemCode),
                    ItemCode = gum.ItemCode,
                    ItemCategoryCode = gum.ItemCategoryCode,
                    Description = gum.Description,

                    OrgBranchName = gum.OrgBranchName,
                    DateCreated = gum.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = gum.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.NaturalGums.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { gum }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Gum_Destroy([DataSourceRequest]DataSourceRequest request, NaturalGumViewModel gum)
        {
            if (ModelState.IsValid)
            {
                var entity = new NaturalGum
                {
                    NaturalGumId = gum.NaturalGumId,
                    Name = gum.Name,
                    ItemCode = gum.ItemCode,
                    ItemCategoryCode = gum.ItemCategoryCode,
                    Description = gum.Description,

                    OrgBranchName = gum.OrgBranchName,
                    DateCreated = gum.DateCreated,
                    LastModified = gum.LastModified,
                    CreatedBy = gum.CreatedBy,
                    ModifiedBy = gum.ModifiedBy,

                };

                db.NaturalGums.Attach(entity);
                db.NaturalGums.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { gum }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}