using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Suppliers
{
    [AuthorizeRoles(ProcurementConstants.ProcurementAdmin)]
    public class SupplierCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
    
        public ActionResult SupplierCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SupplierCategory> suppliercategories = db.SupplierCategories;
            DataSourceResult result = suppliercategories.ToDataSourceResult(request, supplierCategory => new {
                SupplierCategoryID = supplierCategory.SupplierCategoryID,
                Category = supplierCategory.Category,
                Description = supplierCategory.Description,
                
                OrgBranchName = supplierCategory.OrgBranchName,
                DateCreated = supplierCategory.DateCreated,
                LastModified = supplierCategory.LastModified,
                CreatedBy = supplierCategory.CreatedBy,
                ModifiedBy = supplierCategory.ModifiedBy
            });

            return Json(result);
        }
   
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierCategories_Create([DataSourceRequest]DataSourceRequest request, SupplierCategory supplierCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierCategory
                {
                    Category = supplierCategory.Category,
                    Description = supplierCategory.Description,
                    
                    OrgBranchName = supplierCategory.OrgBranchName,
                    DateCreated = supplierCategory.DateCreated,
                    LastModified = supplierCategory.LastModified,
                    CreatedBy = supplierCategory.CreatedBy,
                    ModifiedBy = supplierCategory.ModifiedBy
                };

                db.SupplierCategories.Add(entity);
                db.SaveChanges();
                supplierCategory.SupplierCategoryID = entity.SupplierCategoryID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier category setup", "Success", User.Identity.Name, $"created supplier category {entity.Category}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { supplierCategory }.ToDataSourceResult(request, ModelState));
        }
   
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierCategories_Update([DataSourceRequest]DataSourceRequest request, SupplierCategory supplierCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierCategory
                {
                    SupplierCategoryID = supplierCategory.SupplierCategoryID,
                    Category = supplierCategory.Category,
                    Description = supplierCategory.Description,
                    
                    OrgBranchName = supplierCategory.OrgBranchName,
                    DateCreated = supplierCategory.DateCreated,
                    LastModified = supplierCategory.LastModified,
                    CreatedBy = supplierCategory.CreatedBy,
                    ModifiedBy = supplierCategory.ModifiedBy
                };

                db.SupplierCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier category setup", "Success", User.Identity.Name, $"updated supplier category {entity.Category}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { supplierCategory }.ToDataSourceResult(request, ModelState));
        }
   
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierCategories_Destroy([DataSourceRequest]DataSourceRequest request, SupplierCategory supplierCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierCategory
                {
                    SupplierCategoryID = supplierCategory.SupplierCategoryID,
                    Category = supplierCategory.Category,
                    Description = supplierCategory.Description,
                    
                    OrgBranchName = supplierCategory.OrgBranchName,
                    DateCreated = supplierCategory.DateCreated,
                    LastModified = supplierCategory.LastModified,
                    CreatedBy = supplierCategory.CreatedBy,
                    ModifiedBy = supplierCategory.ModifiedBy
                };

                db.SupplierCategories.Attach(entity);
                db.SupplierCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplierCategory }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
