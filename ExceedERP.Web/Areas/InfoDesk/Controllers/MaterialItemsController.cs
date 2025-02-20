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
            return View();
        }

        public ActionResult MaterialItems_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MaterialItems> materialitems = db.MaterialItems;
            DataSourceResult result = materialitems.ToDataSourceResult(request, materialItems => new {
                MaterialItemsId = materialItems.MaterialItemsId,
                ItemName = materialItems.ItemName,
                Remark = materialItems.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialItems_Create([DataSourceRequest]DataSourceRequest request, MaterialItems materialItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialItems
                {
                    ItemName = materialItems.ItemName,
                    Remark = materialItems.Remark
                };

                db.MaterialItems.Add(entity);
                db.SaveChanges();
                materialItems.MaterialItemsId = entity.MaterialItemsId;
            }

            return Json(new[] { materialItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialItems_Update([DataSourceRequest]DataSourceRequest request, MaterialItems materialItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialItems
                {
                    MaterialItemsId = materialItems.MaterialItemsId,
                    ItemName = materialItems.ItemName,
                    Remark = materialItems.Remark
                };

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
                var entity = new MaterialItems
                {
                    MaterialItemsId = materialItems.MaterialItemsId,
                    ItemName = materialItems.ItemName,
                    Remark = materialItems.Remark
                };

                db.MaterialItems.Attach(entity);
                db.MaterialItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { materialItems }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
