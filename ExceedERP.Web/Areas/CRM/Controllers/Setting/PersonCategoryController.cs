using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class PersonCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["PersonCategoryList"] = db.PersonCategories.AsQueryable();
            return View();
        }

        public ActionResult PersonCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PersonCategory> personcategories = db.PersonCategories;
            DataSourceResult result = personcategories.ToDataSourceResult(request, personCategory => new {
                CategoryID = personCategory.PersonCategoryID,
                Description = personCategory.Description,
                Index = personCategory.Index,
                Parent = personCategory.Parent,
                IsActive = personCategory.IsActive,
                CreatedBy = personCategory.CreatedBy,
                DateCreated = personCategory.DateCreated,
                LastModified = personCategory.LastModified,
                ModifiedBy = personCategory.ModifiedBy,
                OrgBranchName = personCategory.OrgBranchName,
                Remark = personCategory.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PersonCategories_Create([DataSourceRequest]DataSourceRequest request, PersonCategory personCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new PersonCategory
                {
                   Description = personCategory.Description,
                    Index = personCategory.Index,
                    Parent = personCategory.Parent,
                    IsActive = personCategory.IsActive,
                   CreatedBy = User.Identity.Name,
                   DateCreated = DateTime.Now,
                    Remark = personCategory.Remark
                };

                db.PersonCategories.Add(entity);
                db.SaveChanges();
                personCategory.PersonCategoryID = entity.PersonCategoryID;
            }

            return Json(new[] { personCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PersonCategories_Update([DataSourceRequest]DataSourceRequest request, PersonCategory personCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new PersonCategory
                {
                    PersonCategoryID = personCategory.PersonCategoryID,
                    Description = personCategory.Description,
                    Index = personCategory.Index,
                    Parent = personCategory.Parent,
                    IsActive = personCategory.IsActive,
                    Remark = personCategory.Remark
                };

                db.PersonCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { personCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PersonCategories_Destroy([DataSourceRequest]DataSourceRequest request, PersonCategory personCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new PersonCategory
                {
                    PersonCategoryID = personCategory.PersonCategoryID,
                    Description = personCategory.Description,
                    Index = personCategory.Index,
                    Parent = personCategory.Parent,
                    IsActive = personCategory.IsActive,
                    Remark = personCategory.Remark
                };

                db.PersonCategories.Attach(entity);
                db.PersonCategories.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Person Category  number " + personCategory.PersonCategoryID);
      
            }

            return Json(new[] { personCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
