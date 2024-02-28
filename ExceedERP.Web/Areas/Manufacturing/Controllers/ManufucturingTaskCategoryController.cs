using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class ManufucturingTaskCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManifucturingTaskCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ManifucturingTaskCategory> manifucturingtaskcategories = db.ManifucturingTaskCategories;
            DataSourceResult result = manifucturingtaskcategories.ToDataSourceResult(request, manifucturingTaskCategory => new {
                ManifucturingTaskCategoryId = manifucturingTaskCategory.ManifucturingTaskCategoryId,
                Type = manifucturingTaskCategory.Type,
                Name = manifucturingTaskCategory.Name,
                StandardHour = manifucturingTaskCategory.StandardHour,
                HourRate = manifucturingTaskCategory.HourRate
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManifucturingTaskCategories_Create([DataSourceRequest]DataSourceRequest request, ManifucturingTaskCategory manifucturingTaskCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManifucturingTaskCategory
                {
                    Type = manifucturingTaskCategory.Type,
                    Name = manifucturingTaskCategory.Name,
                    StandardHour = manifucturingTaskCategory.StandardHour,
                    HourRate = manifucturingTaskCategory.HourRate
                };

                db.ManifucturingTaskCategories.Add(entity);
                db.SaveChanges();
                manifucturingTaskCategory.ManifucturingTaskCategoryId = entity.ManifucturingTaskCategoryId;
            }

            return Json(new[] { manifucturingTaskCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManifucturingTaskCategories_Update([DataSourceRequest]DataSourceRequest request, ManifucturingTaskCategory manifucturingTaskCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManifucturingTaskCategory
                {
                    ManifucturingTaskCategoryId = manifucturingTaskCategory.ManifucturingTaskCategoryId,
                    Type = manifucturingTaskCategory.Type,
                    Name = manifucturingTaskCategory.Name,
                    StandardHour = manifucturingTaskCategory.StandardHour,
                    HourRate = manifucturingTaskCategory.HourRate
                };

                db.ManifucturingTaskCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { manifucturingTaskCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManifucturingTaskCategories_Destroy([DataSourceRequest]DataSourceRequest request, ManifucturingTaskCategory manifucturingTaskCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManifucturingTaskCategory
                {
                    ManifucturingTaskCategoryId = manifucturingTaskCategory.ManifucturingTaskCategoryId,
                    Type = manifucturingTaskCategory.Type,
                    Name = manifucturingTaskCategory.Name,
                    StandardHour = manifucturingTaskCategory.StandardHour,
                    HourRate = manifucturingTaskCategory.HourRate
                };

                db.ManifucturingTaskCategories.Attach(entity);
                db.ManifucturingTaskCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { manifucturingTaskCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
