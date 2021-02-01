using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Web.Helpers;


namespace ExceedERP.Web.Areas.CRM.Controllers
{
      [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class SalesStoreController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            ViewData["InventoryStore"] = db.InventoryStores.ToList();
            return View();
        }

        public ActionResult SalesStores_Read([DataSourceRequest]DataSourceRequest request )
        {
            IQueryable<SalesStore> SalesStores = db.SalesStores;
            DataSourceResult result = SalesStores.ToDataSourceResult(request, SalesStore => new
            {
                SalesStoreId = SalesStore.SalesStoreId,
                Location = SalesStore.Location,
               StoreCode =  SalesStore.StoreCode,
                Description= SalesStore.Description,
                CreatedBy = SalesStore.CreatedBy,
                DateCreated = SalesStore.DateCreated,
                LastModified = SalesStore.LastModified,
                ModifiedBy = SalesStore.ModifiedBy,
                OrgBranchName = SalesStore.OrgBranchName,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesStores_Create([DataSourceRequest]DataSourceRequest request, SalesStore SalesStore)
        {
            if (ModelState.IsValid)
            {
                var MappedStore = db.SalesStores.Where(x => x.StoreCode == SalesStore.StoreCode).ToList();
                if(MappedStore.Any())
                {
                    ModelState.AddModelError("AlreadyMapped", " Store is Already Mapped !");
                    return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
                }
                var Store = db.InventoryStores.Where(x => x.StoreCode == SalesStore.StoreCode).FirstOrDefault();
                if (Store != null)
                {
                    SalesStore.Location = Store.Branch.Name;

                    var entity = new SalesStore
                    {

                        SalesStoreId = SalesStore.SalesStoreId,
                        Location = SalesStore.Location,
                        StoreCode = SalesStore.StoreCode,
                        Description = SalesStore.Description,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        InventoryStore = Store
                    };

                    db.SalesStores.Add(entity);
                    db.SaveChanges();
                    SalesStore.SalesStoreId = entity.SalesStoreId;
                }
            }

            return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesStores_Update([DataSourceRequest]DataSourceRequest request, SalesStore SalesStore)
        {
          
            if (ModelState.IsValid)
            {
                var MappedStore = db.SalesStores.Where(x => x.StoreCode == SalesStore.StoreCode).ToList();
                if (MappedStore.Any())
                {
                    ModelState.AddModelError("AlreadyMapped", " Store is Already Mapped !");
                    return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
                }
                
                var Store = db.InventoryStores.Where(x => x.StoreCode == SalesStore.StoreCode).FirstOrDefault();
                if (Store != null)
                {
                    SalesStore.Location = Store.Branch.Name;
                }
                var entity = new SalesStore
                {
                    SalesStoreId = SalesStore.SalesStoreId,
                    Location = SalesStore.Location,
                    StoreCode = SalesStore.StoreCode,
                      Description = SalesStore.Description,
                };

                db.SalesStores.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesStores_Destroy([DataSourceRequest]DataSourceRequest request, SalesStore SalesStore)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesStore
                {
                    SalesStoreId = SalesStore.SalesStoreId,

                };

                db.SalesStores.Attach(entity);
                db.SalesStores.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Sales Store number " + SalesStore.SalesStoreId);

            }

            return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}