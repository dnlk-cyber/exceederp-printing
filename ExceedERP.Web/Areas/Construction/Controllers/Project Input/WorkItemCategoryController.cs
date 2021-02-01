using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    public class WorkItemCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var categories = db.ItemCategorys.AsQueryable();
            ViewData["Categories"] = categories;
         
           var lis = db.Configurations .Where( x=> x.Category ==ConstructionSettingCategories .STRUCTURE_TYPE.ToString( ) ) .ToList() ;
           ViewData["StructureType"] = lis;

            return View();
        }
        public class StructureType
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }
        public ActionResult Lookups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Lookup> lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.WORK_ITEM);
            DataSourceResult result = lookups.ToDataSourceResult(request, lookup => new {
                LookupId = lookup.LookupId,
                Code = lookup.Code,
                Category = lookup.Category,
                Index = lookup.Index,
                LookupType = lookup.LookupType,
                Description = lookup.Description,
                IsDefault = lookup.IsDefault,
                IsActive = lookup.IsActive,
                Remark = lookup.Remark,
                DateCreated = lookup.DateCreated,
                LastModified = lookup.LastModified,
                CreatedBy = lookup.CreatedBy,
                ModifiedBy = lookup.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Create([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            if (ModelState.IsValid)
            {
                var entity = new Lookup
                {
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType = LookUpTypes.WORK_ITEM,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = true,
                    Remark = lookup.Remark,
                    DateCreated = System.DateTime.Now,
                    LastModified = lookup.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = lookup.ModifiedBy 
                };

                db.Lookups.Add(entity);
                db.SaveChanges();
                lookup.LookupId = entity.LookupId;
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Update([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            if (ModelState.IsValid)
            {
                var entity = new Lookup
                {
                    LookupId = lookup.LookupId,
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType = LookUpTypes.WORK_ITEM,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = lookup.IsActive,
                    Remark = lookup.Remark,
                    DateCreated = lookup.DateCreated,
                    LastModified = System.DateTime.Now,
                    CreatedBy = lookup.CreatedBy,
                    ModifiedBy = User.Identity.Name  
                };

                db.Lookups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Destroy([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            if (ModelState.IsValid)
            {
                var entity = new Lookup
                {
                    LookupId = lookup.LookupId,
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType = LookUpTypes.WORK_ITEM,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = lookup.IsActive,
                    Remark = lookup.Remark,
                    DateCreated = lookup.DateCreated,
                    LastModified = lookup.LastModified,
                    CreatedBy = lookup.CreatedBy,
                    ModifiedBy = lookup.ModifiedBy 
                };

                db.Lookups.Attach(entity);
                db.Lookups.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
