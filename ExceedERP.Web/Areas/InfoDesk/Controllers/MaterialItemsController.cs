using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.InfoDesk.Support;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class MaterialItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var materialItms = db.MaterialItems.ToList();
            var catecorIds = materialItms?.Select(x => x.CategoryId);
            //var catecorIds = materialItms?.Select(x => x.CategoryId);
            ViewData["category"] = db.ItemCategorys.ToList()
                                        .Where(x=>catecorIds.Contains(x.ItemCategoryCode))
                                            .Select(x=>new
                                            { Code=x.ItemCategoryCode,
                                                Name =$"{x.ItemCategoryCode}-{x.Name}"
                                            }).ToList();
            ViewData["inventoryItems"] = db.InventoryItems.ToList()
                                    //.Where(x => catecorIds.Contains(x.ItemCategoryCode))
                                            .Select(x => new
                                            {
                                                Code = x.ItemCode,
                                                Name = $"{x.ItemCode}-{x.Name}"
                                            }).ToList();
            return View();
        }

        public ActionResult MaterialItems_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MaterialItems> materialitems = db.MaterialItems;
            DataSourceResult result = materialitems.ToDataSourceResult(request, materialItem => new {
                MaterialItemsId = materialItem.MaterialItemsId,
                CategoryId = materialItem.CategoryId,
                InventoryItemId = materialItem.InventoryItemId,
                ItemName = materialItem.ItemName,
                Remark = materialItem.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialItems_Create([DataSourceRequest]DataSourceRequest request, MaterialItems materialItems)
        {
            var items = db.InventoryItems.FirstOrDefault(x=>x.ItemCode==materialItems.InventoryItemId);
            if(items != null)
            {
                materialItems.ItemName = $"{items.ItemCode} -{items.Name}";
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(materialItems);
                db.MaterialItems.Add(entity);
                db.SaveChanges();
                materialItems.MaterialItemsId = entity.MaterialItemsId;
            }

            return Json(new[] { materialItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialItems_Update([DataSourceRequest]DataSourceRequest request, MaterialItems materialItems)
        {
            var items = db.InventoryItems.FirstOrDefault(x => x.ItemCode == materialItems.InventoryItemId);
            if (items != null)
            {
                materialItems.ItemName =$"{items.ItemCode} -{items.Name}";
            }

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(materialItems);
                db.MaterialItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { materialItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialItems_Destroy([DataSourceRequest]DataSourceRequest request, MaterialItems materialItems)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(materialItems);
                db.MaterialItems.Attach(entity);
                db.MaterialItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { materialItems }.ToDataSourceResult(request, ModelState));
        }

        private MaterialItems GetMappedObject(MaterialItems materialItems)
        {
            var entity = new MaterialItems
            {
                MaterialItemsId = materialItems.MaterialItemsId,
                CategoryId = materialItems.CategoryId,
                InventoryItemId = materialItems.InventoryItemId,
                ItemName = materialItems.ItemName,
                Remark = materialItems.Remark
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
