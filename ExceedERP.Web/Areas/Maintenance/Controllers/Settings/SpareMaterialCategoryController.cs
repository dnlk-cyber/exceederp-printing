using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    //[AuthorizeRoles(MaintenanceRoles.MaintenanceAdmin)]  
    public class SpareMaterialCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Category_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SpareMaterialCategory> categories = db.SpareMaterialCategory;
            DataSourceResult result = categories.ToDataSourceResult(request, category => new
            {
                SpareMaterialCategoryId = category.SpareMaterialCategoryId,
                Description = category.Description,
                Remark = category.Remark,

                DateCreated = category.DateCreated,
                LastModified = category.LastModified,
                CreatedBy = category.CreatedBy,
                ModifiedBy = category.ModifiedBy
            });

            return Json(result);
        }
        private bool CategoryExists(SpareMaterialCategory model)
        {
            return db.SpareMaterialCategory.FirstOrDefault(me => me.Description == model.Description && me.SpareMaterialCategoryId != model.SpareMaterialCategoryId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Category_Create([DataSourceRequest]DataSourceRequest request, SpareMaterialCategory category)
        {
            ModelState.Remove("SpareMaterialCategoryId");
            if (ModelState.IsValid)
            {
                if (CategoryExists(category))
                {
                    ModelState.AddModelError("SpareMaterialCategortyId", "Duplicate category not allowed!");
                }
                else
                {
                    var entity = new SpareMaterialCategory
                    {
                        SpareMaterialCategoryId = Guid.NewGuid(),
                        Description = category.Description,
                        Remark = category.Remark,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.SpareMaterialCategory.Add(entity);
                    db.SaveChanges();
                    category.SpareMaterialCategoryId = entity.SpareMaterialCategoryId;

                }
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Category_Update([DataSourceRequest]DataSourceRequest request, SpareMaterialCategory category)
        {
            ModelState.Remove("SpareMaterialCategoryId");
            if (ModelState.IsValid)
            {
                if (CategoryExists(category))
                {
                    ModelState.AddModelError("SpareMaterialCategoryId", "Duplicate category not allowed!");
                }
                else
                {
                    var entity = new SpareMaterialCategory
                    {
                        SpareMaterialCategoryId = category.SpareMaterialCategoryId,
                        Description = category.Description,
                        Remark = category.Remark,

                        DateCreated = category.DateCreated,
                        CreatedBy = category.CreatedBy,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };
                    db.SpareMaterialCategory.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Category_Destroy([DataSourceRequest]DataSourceRequest request, SpareMaterialCategory category)
        {
            if (ModelState.IsValid)
            {
                var entity = new SpareMaterialCategory
                {
                    SpareMaterialCategoryId = category.SpareMaterialCategoryId,
                    Description = category.Description,
                    Remark = category.Remark,

                    DateCreated = category.DateCreated,
                    LastModified = category.LastModified,
                    CreatedBy = category.CreatedBy,
                    ModifiedBy = category.ModifiedBy
                };

                db.SpareMaterialCategory.Attach(entity);
                db.SpareMaterialCategory.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}