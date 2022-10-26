using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

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
    public class OrganizationCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["OrganizationCategoryList"] = db.OrganizationCategories;
            return View();
        }

        public ActionResult OrganizationCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OrganizationCategory> organizationcategories = db.OrganizationCategories;
            DataSourceResult result = organizationcategories.ToDataSourceResult(request, organizationCategory => new
            {
                OrganizationCategoryID = organizationCategory.OrganizationCategoryID,
                Description = organizationCategory.Description,
                Index = organizationCategory.Index,
                Parent = organizationCategory.Parent,
                IsActive = organizationCategory.IsActive,
                CreatedBy = organizationCategory.CreatedBy,
                DateCreated = organizationCategory.DateCreated,
                LastModified = organizationCategory.LastModified,
                ModifiedBy = organizationCategory.ModifiedBy,
                OrgBranchName = organizationCategory.OrgBranchName,
                Remark = organizationCategory.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OrganizationCategories_Create([DataSourceRequest]DataSourceRequest request, OrganizationCategory organizationCategory)
        {
            List<OrganizationCategory> organizationCategoryList = db.OrganizationCategories.Where(ocl => ocl.Index == organizationCategory.Index).ToList();
            List<OrganizationCategory> organizationCategoryLists = db.OrganizationCategories.Where(ocl => ocl.Description == organizationCategory.Description).ToList();

            if (organizationCategoryList.Any())
            {
                ModelState.AddModelError("Invalid Index Value", "Please readjust index value!");
            }
            else if (organizationCategoryLists.Any())
            {
                ModelState.AddModelError("Invalid Index Value", "Category already exist!");
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new OrganizationCategory
                    {
                        Description = organizationCategory.Description,
                        Index = organizationCategory.Index,
                        Parent = organizationCategory.Parent,
                        IsActive = organizationCategory.IsActive,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = organizationCategory.Remark
                    };

                    db.OrganizationCategories.Add(entity);
                    db.SaveChanges();
                    organizationCategory.OrganizationCategoryID = entity.OrganizationCategoryID;
                }

            }
            return Json(new[] { organizationCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OrganizationCategories_Update([DataSourceRequest]DataSourceRequest request, OrganizationCategory organizationCategory)
        {
            List<OrganizationCategory> organizationCategoryList = db.OrganizationCategories.Where(ocl => ocl.Index == organizationCategory.Index).ToList();
            List<OrganizationCategory> organizationCategoryLists = db.OrganizationCategories.Where(ocl => ocl.Description == organizationCategory.Description).ToList();

            if (organizationCategoryList.Any())
            {
                ModelState.AddModelError("Invalid Index Value", "Please readjust index value!");
            }
            else if (organizationCategoryLists.Any())
            {
                ModelState.AddModelError("Invalid Index Value", "Category already exist!");
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new OrganizationCategory
                    {
                        OrganizationCategoryID = organizationCategory.OrganizationCategoryID,
                        Description = organizationCategory.Description,
                        Index = organizationCategory.Index,
                        Parent = organizationCategory.Parent,
                        IsActive = organizationCategory.IsActive,
                        Remark = organizationCategory.Remark
                    };

                    db.OrganizationCategories.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            return Json(new[] { organizationCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OrganizationCategories_Destroy([DataSourceRequest]DataSourceRequest request, OrganizationCategory organizationCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new OrganizationCategory
                {
                    OrganizationCategoryID = organizationCategory.OrganizationCategoryID,
                    Description = organizationCategory.Description,
                    Index = organizationCategory.Index,
                    Parent = organizationCategory.Parent,
                    IsActive = organizationCategory.IsActive,
                    Remark = organizationCategory.Remark
                };

                db.OrganizationCategories.Attach(entity);
                db.OrganizationCategories.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Organization Category number " + organizationCategory.OrganizationCategoryID);
      
            }

            return Json(new[] { organizationCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
