using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class MapWorkItemCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MapWorkItemCategory_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<MapWorkItemCategory> mapworkitemcategory = db.MapWorkItemCategory.Where( x=> x.LookupId ==id);
            DataSourceResult result = mapworkitemcategory.ToDataSourceResult(request, mapWorkItemCategory => new {
                MapWorkCategoryId = mapWorkItemCategory.MapWorkCategoryId,
                LookupId = mapWorkItemCategory.LookupId,
                ItemCategoryCode = mapWorkItemCategory.ItemCategoryCode
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MapWorkItemCategory_Create([DataSourceRequest]DataSourceRequest request, MapWorkItemCategory mapWorkItemCategory, int idd)
        {

            if (ModelState.IsValid)
            {
                var entity = new MapWorkItemCategory
                {
                    LookupId =idd,
                    ItemCategoryCode = mapWorkItemCategory.ItemCategoryCode
                };

                db.MapWorkItemCategory.Add(entity);
                db.SaveChanges();
                mapWorkItemCategory.MapWorkCategoryId = entity.MapWorkCategoryId;
            }

            return Json(new[] { mapWorkItemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MapWorkItemCategory_Update([DataSourceRequest]DataSourceRequest request, MapWorkItemCategory mapWorkItemCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new MapWorkItemCategory
                {
                    MapWorkCategoryId = mapWorkItemCategory.MapWorkCategoryId,
                    LookupId = mapWorkItemCategory.LookupId,
                    ItemCategoryCode = mapWorkItemCategory.ItemCategoryCode
                };

                db.MapWorkItemCategory.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { mapWorkItemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MapWorkItemCategory_Destroy([DataSourceRequest]DataSourceRequest request, MapWorkItemCategory mapWorkItemCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new MapWorkItemCategory
                {
                    MapWorkCategoryId = mapWorkItemCategory.MapWorkCategoryId,
                    LookupId = mapWorkItemCategory.LookupId,
                    ItemCategoryCode = mapWorkItemCategory.ItemCategoryCode
                };

                db.MapWorkItemCategory.Attach(entity);
                db.MapWorkItemCategory.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { mapWorkItemCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
