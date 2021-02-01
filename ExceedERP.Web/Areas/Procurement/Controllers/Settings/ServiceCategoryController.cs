using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Settings
{

    [AuthorizeRoles(FinanceConstants.AccountPayableAdmin)]
    public class ServiceCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ServiceCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ServiceCategory> servicecategories = db.ServiceCategories;
            DataSourceResult result = servicecategories.ToDataSourceResult(request, serviceCategory => new {
                ServiceCategoryCode = serviceCategory.ServiceCategoryCode,
                Name = serviceCategory.Name,
                Description = serviceCategory.Description,
                AccountCode = serviceCategory.AccountCode,
                IssueAccountCode = serviceCategory.IssueAccountCode,
                HaveParent = serviceCategory.HaveParent,
                ParentName = serviceCategory.ParentName,
                Abbreviation = serviceCategory.Abbreviation,
                DateCreated = serviceCategory.DateCreated,
                LastModified = serviceCategory.LastModified,
                CreatedBy = serviceCategory.CreatedBy,
                ModifiedBy = serviceCategory.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCategories_Create([DataSourceRequest]DataSourceRequest request, ServiceCategory serviceCategory)
        {
            #region validation

            bool valid = true;
            if (!String.IsNullOrEmpty(serviceCategory.AccountCode) && !String.IsNullOrEmpty(serviceCategory.ServiceCategoryCode))
            {
                var accountSegment = serviceCategory.AccountCode.Split('-')[0];
                if (!serviceCategory.ServiceCategoryCode.StartsWith(accountSegment))
                {
                    ModelState.AddModelError("Category code", serviceCategory.ServiceCategoryCode + " and account code prefix " + accountSegment + "  don't match.");
                    valid = false;
                }

            }
            else
            {
                ModelState.AddModelError("Category code and account code", "Please fill category code and account code.");
                valid = false;
            }

            var exists = db.ServiceCategories.Where(x => x.ServiceCategoryCode == serviceCategory.ServiceCategoryCode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Category code", serviceCategory.ServiceCategoryCode + " already exists in the system.");
                valid = false;
            }

            var accountcodeexists =
                db.ServiceCategories.Where(x => x.ServiceCategoryCode != serviceCategory.ServiceCategoryCode)
                    .Where(x => x.AccountCode == serviceCategory.AccountCode).ToList();

            if (accountcodeexists.Any())
            {
                ModelState.AddModelError("Account code", serviceCategory.AccountCode + " already exists in the system.");
                valid = false;
            }

            var nameexists = db.ServiceCategories.Where(x => x.ServiceCategoryCode != serviceCategory.ServiceCategoryCode)
                .Where(x => x.Name == serviceCategory.Name).ToList();

            if (nameexists.Any())
            {
                ModelState.AddModelError("Category Name", serviceCategory.Name + " already exists in the system.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ServiceCategory
                {
                    ServiceCategoryCode = serviceCategory.ServiceCategoryCode,
                    Name = serviceCategory.Name,
                    Description = serviceCategory.Description,
                    AccountCode = serviceCategory.AccountCode,
                    IssueAccountCode = serviceCategory.IssueAccountCode,
                    HaveParent = serviceCategory.HaveParent,
                    ParentName = serviceCategory.ParentName,
                    Abbreviation = serviceCategory.Abbreviation,
                    DateCreated = serviceCategory.DateCreated,
                    LastModified = serviceCategory.LastModified,
                    CreatedBy = serviceCategory.CreatedBy,
                    ModifiedBy = serviceCategory.ModifiedBy
                };

                db.ServiceCategories.Add(entity);
                db.SaveChanges();
                serviceCategory.ServiceCategoryCode = entity.ServiceCategoryCode;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Service category setup", "Success", User.Identity.Name, $"created category {entity.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { serviceCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCategories_Update([DataSourceRequest]DataSourceRequest request, ServiceCategory serviceCategory)
        {
            #region validation

            bool valid = true;
            if (!String.IsNullOrEmpty(serviceCategory.AccountCode) && !String.IsNullOrEmpty(serviceCategory.ServiceCategoryCode))
            {
                var accountSegment = serviceCategory.AccountCode.Split('-')[0];
                if (!serviceCategory.ServiceCategoryCode.StartsWith(accountSegment))
                {
                    ModelState.AddModelError("Category code", serviceCategory.ServiceCategoryCode + " and account code prefix " + accountSegment + "  don't match.");
                    valid = false;
                }

            }
            else
            {
                ModelState.AddModelError("Category code and account code", "Please fill category code and account code.");
                valid = false;
            }

            //var exists = db.ServiceCategories.Where(x => x.ServiceCategoryCode == serviceCategory.ServiceCategoryCode).Where(x => x.ServiceCategoryCode == serviceCategory.ServiceCategoryCode).ToList();

            //if (exists.Any())
            //{
            //    ModelState.AddModelError("Category code", serviceCategory.ServiceCategoryCode + " already exists in the system.");
            //    valid = false;
            //}

            var accountcodeexists =
                db.ServiceCategories.Where(x => x.ServiceCategoryCode != serviceCategory.ServiceCategoryCode)
                    .Where(x => x.AccountCode == serviceCategory.AccountCode).ToList();

            if (accountcodeexists.Any())
            {
                ModelState.AddModelError("Account code", serviceCategory.AccountCode + " already exists in the system.");
                valid = false;
            }

            var nameexists = db.ServiceCategories.Where(x => x.ServiceCategoryCode != serviceCategory.ServiceCategoryCode)
                .Where(x => x.Name == serviceCategory.Name).ToList();

            if (nameexists.Any())
            {
                ModelState.AddModelError("Category Name", serviceCategory.Name + " already exists in the system.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ServiceCategory
                {
                    ServiceCategoryCode = serviceCategory.ServiceCategoryCode,
                    Name = serviceCategory.Name,
                    Description = serviceCategory.Description,
                    AccountCode = serviceCategory.AccountCode,
                    IssueAccountCode = serviceCategory.IssueAccountCode,
                    HaveParent = serviceCategory.HaveParent,
                    ParentName = serviceCategory.ParentName,
                    Abbreviation = serviceCategory.Abbreviation,
                    DateCreated = serviceCategory.DateCreated,
                    LastModified = serviceCategory.LastModified,
                    CreatedBy = serviceCategory.CreatedBy,
                    ModifiedBy = serviceCategory.ModifiedBy
                };

                db.ServiceCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Service category setup", "Success", User.Identity.Name, $"updated category {entity.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { serviceCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCategories_Destroy([DataSourceRequest]DataSourceRequest request, ServiceCategory serviceCategory)
        {
            #region validation

            bool valid = true;
            var categories = db.ServiceCategories.Where(x => x.ParentName == serviceCategory.ServiceCategoryCode).ToList();

            if (categories.Any())
            {
                ModelState.AddModelError("Category code", serviceCategory.ServiceCategoryCode + " Cannot be deleted ,it has child categories under it.");
                valid = false;
            }

            var itemCategories =
                db.ServiceItems.Where(x => x.ServiceCategoryCode == serviceCategory.ServiceCategoryCode).ToList();

            if (itemCategories.Any())
            {
                ModelState.AddModelError("Category Items",
                    serviceCategory.ServiceCategoryCode + "  Cannot be deleted ,it has items under it.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ServiceCategory
                {
                    ServiceCategoryCode = serviceCategory.ServiceCategoryCode,
                    Name = serviceCategory.Name,
                    Description = serviceCategory.Description,
                    AccountCode = serviceCategory.AccountCode,
                    IssueAccountCode = serviceCategory.IssueAccountCode,
                    HaveParent = serviceCategory.HaveParent,
                    ParentName = serviceCategory.ParentName,
                    Abbreviation = serviceCategory.Abbreviation,
                    DateCreated = serviceCategory.DateCreated,
                    LastModified = serviceCategory.LastModified,
                    CreatedBy = serviceCategory.CreatedBy,
                    ModifiedBy = serviceCategory.ModifiedBy
                };

                db.ServiceCategories.Attach(entity);
                db.ServiceCategories.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted service category " + serviceCategory.Name);
                #endregion
            }

            return Json(new[] { serviceCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
