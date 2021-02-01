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
    public class ServiceItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }
        private void PopulateCategories()
        {
            var dataContext = new ExceedDbContext();
            var categories = dataContext.ServiceCategories;

            ViewData["categories"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }

        public ActionResult ServiceItems_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ServiceItem> serviceitems = db.ServiceItems;
            DataSourceResult result = serviceitems.ToDataSourceResult(request, serviceItem => new {
                ServiceCategoryCode = serviceItem.ServiceCategoryCode,
                ServiceItemCode = serviceItem.ServiceItemCode,
                Name = serviceItem.Name,
                Type = serviceItem.Type,
                UoMName = serviceItem.UoMName,
                Balance = serviceItem.Balance,
                AvgPrice = serviceItem.AvgPrice,
                HasTransaction = serviceItem.HasTransaction,
                Enable = serviceItem.Enable,
                DateCreated = serviceItem.DateCreated,
                LastModified = serviceItem.LastModified,
                CreatedBy = serviceItem.CreatedBy,
                ModifiedBy = serviceItem.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceItems_Create([DataSourceRequest]DataSourceRequest request, ServiceItem serviceItem)
        {
            #region validation

            bool valid = true;
            var exists = db.ServiceItems.Where(x => x.ServiceItemCode == serviceItem.ServiceItemCode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Item code",
                    serviceItem.ServiceItemCode + " already exists in the system.");
                valid = false;
            }

           


            var nameexists = db.ServiceItems.Where(x => x.ServiceItemCode != serviceItem.ServiceItemCode).Where(x => x.Name == serviceItem.Name).ToList();

            if (nameexists.Any())
            {
                ModelState.AddModelError("Item Name", serviceItem.Name + " already exists in the system.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ServiceItem
                {
                    ServiceCategoryCode = serviceItem.ServiceCategoryCode,
                    ServiceItemCode = serviceItem.ServiceItemCode,
                    Name = serviceItem.Name,
                    Type = serviceItem.Type,
                    UoMName = serviceItem.UoMName,
                    Balance = serviceItem.Balance,
                    AvgPrice = serviceItem.AvgPrice,
                    HasTransaction = serviceItem.HasTransaction,
                    Enable = serviceItem.Enable,
                    DateCreated = serviceItem.DateCreated,
                    LastModified = serviceItem.LastModified,
                    CreatedBy = serviceItem.CreatedBy,
                    ModifiedBy = serviceItem.ModifiedBy
                };

                db.ServiceItems.Add(entity);
                db.SaveChanges();
                serviceItem.ServiceItemCode = entity.ServiceItemCode;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Service item setup", "Success", User.Identity.Name, $"created service item {entity.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { serviceItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceItems_Update([DataSourceRequest]DataSourceRequest request, ServiceItem serviceItem)
        {
            #region validation

            bool valid = true;

            var nameexists = db.ServiceItems.Where(x => x.ServiceItemCode != serviceItem.ServiceItemCode)
                .Where(x => x.Name == serviceItem.Name).ToList();

            if (nameexists.Any())
            {
                ModelState.AddModelError("Item Name", serviceItem.Name + " already exists in the system.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ServiceItem
                {
                    ServiceCategoryCode = serviceItem.ServiceCategoryCode,
                    ServiceItemCode = serviceItem.ServiceItemCode,
                    Name = serviceItem.Name,
                    Type = serviceItem.Type,
                    UoMName = serviceItem.UoMName,
                    Balance = serviceItem.Balance,
                    AvgPrice = serviceItem.AvgPrice,
                    HasTransaction = serviceItem.HasTransaction,
                    Enable = serviceItem.Enable,
                    DateCreated = serviceItem.DateCreated,
                    LastModified = serviceItem.LastModified,
                    CreatedBy = serviceItem.CreatedBy,
                    ModifiedBy = serviceItem.ModifiedBy
                };

                db.ServiceItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Service item setup", "Success", User.Identity.Name, $"updated service item {entity.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { serviceItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceItems_Destroy([DataSourceRequest]DataSourceRequest request, ServiceItem serviceItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ServiceItem
                {
                    ServiceItemCode = serviceItem.ServiceItemCode,
                    Name = serviceItem.Name,
                    Type = serviceItem.Type,
                    Balance = serviceItem.Balance,
                    AvgPrice = serviceItem.AvgPrice,
                    HasTransaction = serviceItem.HasTransaction,
                    Enable = serviceItem.Enable,
                    DateCreated = serviceItem.DateCreated,
                    LastModified = serviceItem.LastModified,
                    CreatedBy = serviceItem.CreatedBy,
                    ModifiedBy = serviceItem.ModifiedBy
                };

                db.ServiceItems.Attach(entity);
                db.ServiceItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { serviceItem }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var item = db.ServiceItems.FirstOrDefault(p => p.ServiceItemCode == id);
                if (item != null && item.Enable)
                {
                    item.Enable = false;
                    #region disable log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Disable", "Success", User.Identity.Name, "Disabled service item  " + item.Name);
                    #endregion
                }
                else
                {
                    if (item != null)
                    {
                        item.Enable = true;
                        #region enable log
                        UserHelper.DeleteLog(Request.UserHostAddress, "Enable", "Success", User.Identity.Name, "Enabled service item  " + item.Name);
                        #endregion
                    }
                }
                if (item != null)
                {
                    db.ServiceItems.Attach(item);
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
