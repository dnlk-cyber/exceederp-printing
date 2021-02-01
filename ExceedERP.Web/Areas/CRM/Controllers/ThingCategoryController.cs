using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class ThingCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            List<ThingCategory> thingCategoryList = db.ThingCategories.ToList();
            ViewData["CategoryList"] = thingCategoryList;
            return View();

        }

        public ActionResult ThingCategories_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<ThingCategory> thingcategories = db.ThingCategories;
            DataSourceResult result = thingcategories.ToDataSourceResult(request, thingCategory => new
            {
                ThingCategoryID = thingCategory.ThingCategoryID,
                Description = thingCategory.Description,
                Index = thingCategory.Index,
                Parent = thingCategory.Parent,
                IsActive = thingCategory.IsActive,
                CreatedBy = thingCategory.CreatedBy,
                DateCreated = thingCategory.DateCreated,
                LastModified = thingCategory.LastModified,
                ModifiedBy = thingCategory.ModifiedBy,
                OrgBranchName = thingCategory.OrgBranchName,
                Remark = thingCategory.Remark
            });
            ViewData["CategoryList"] = db.ThingCategories;
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ThingCategories_Create([DataSourceRequest]DataSourceRequest request, ThingCategory thingCategory)
        {
            List<ThingCategory> thingCategoryList = db.ThingCategories.Where(tc => tc.Index == thingCategory.Index).ToList();
            List<ThingCategory> thingCategoryLists = db.ThingCategories.Where(tc => tc.Description == thingCategory.Description).ToList();
            if (thingCategoryList.Count > 0)
            {
                ModelState.AddModelError("DuplicateIndexEntry","Please readjust index value!");
            }
            else if (thingCategoryLists.Count > 0)
            {
                ModelState.AddModelError("DuplicateIndexEntry", "Category already exist!");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new ThingCategory
                    {
                        Description = thingCategory.Description,

                        Index = thingCategory.Index,
                        Parent = thingCategory.Parent,
                        IsActive = thingCategory.IsActive,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = thingCategory.Remark
                    };

                    db.ThingCategories.Add(entity);
                    db.SaveChanges();
                    thingCategory.ThingCategoryID = entity.ThingCategoryID;
                }
            }

            return Json(new[] { thingCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ThingCategories_Update([DataSourceRequest]DataSourceRequest request, ThingCategory thingCategory)
        {
            List<ThingCategory> thingCategoryList = db.ThingCategories.Where(tc => tc.Index == thingCategory.Index).ToList();
            List<ThingCategory> thingCategoryLists = db.ThingCategories.Where(tc => tc.Description == thingCategory.Description && tc.ThingCategoryID != thingCategory.ThingCategoryID).ToList();
            if (thingCategoryList.Count > 1)
            {
                ModelState.AddModelError("DuplicateIndexEntry", "Please readjust index value!");
            }
            else if (thingCategoryLists.Count > 0)
            {
                ModelState.AddModelError("DuplicateIndexEntry", "Category already exist!");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new ThingCategory
                    {
                        ThingCategoryID = thingCategory.ThingCategoryID,
                        Description = thingCategory.Description,
                        Index = thingCategory.Index,
                        Parent = thingCategory.Parent,
                        IsActive = thingCategory.IsActive,
                        Remark = thingCategory.Remark
                    };
                    db.Entry(entity).State = EntityState.Modified;
                  //  db.ThingCategories.Attach(entity);
                  
                    db.SaveChanges();
                }
            }

            return Json(new[] { thingCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ThingCategories_Destroy([DataSourceRequest]DataSourceRequest request, ThingCategory thingCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ThingCategory
                {
                    ThingCategoryID = thingCategory.ThingCategoryID,
                    Description = thingCategory.Description,
                    Index = thingCategory.Index,
                    Parent = thingCategory.Parent,
                    IsActive = thingCategory.IsActive,
                    Remark = thingCategory.Remark
                };

                db.ThingCategories.Attach(entity);
                db.ThingCategories.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Product Category number " + thingCategory.ThingCategoryID);
      
            }

            return Json(new[] { thingCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
