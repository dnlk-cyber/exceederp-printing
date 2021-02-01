using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class StoreController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateBranchs();
            return View();
        }
        private void PopulateBranchs()
        {
            var branches = db.Branch;
            ViewData["branches"] = branches;

            var businessUnits = db.BusinessUnits;
            ViewData["businessUnits"] = businessUnits;           
        }

        public ActionResult Stores_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InventoryStore> stores = db.InventoryStores;
            DataSourceResult result = stores.ToDataSourceResult(request, store => new {
                StoreCode = store.StoreCode,
                Name = store.Name,
                BranchId = store.BranchId,
                BusinessUnitId = store.BusinessUnitId,
                Location = store.Location,
                Enable = store.Enable,
                IsTemp = store.IsTemp,
                Remark = store.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Stores_Create([DataSourceRequest]DataSourceRequest request, InventoryStore store)
        {
            if (ModelState.IsValid)
            {
                  #region validation

                    bool valid = true;
                    var exists =
                        db.InventoryStores.Where(x => x.StoreCode == store.StoreCode).ToList();

                    if (exists.Any())
                    {
                        ModelState.AddModelError("Store Code",
                            store.StoreCode + " already exists in the system.");
                        valid = false;
                    }

                    var nameexists =
                        db.InventoryStores.Where(x => x.StoreCode != store.StoreCode)
                                           .Where(x=>x.Name==store.Name).ToList();

                    if (nameexists.Any())
                    {
                        ModelState.AddModelError("Store Name",
                            store.Name + " already exists in the system.");
                        valid = false;
                    }


                    #endregion

                if (valid)
                {

                    var entity = new InventoryStore
                    {

                        Name = store.Name,
                        StoreCode = store.StoreCode,
                        BranchId = store.BranchId,
                        BusinessUnitId = store.BusinessUnitId,
                        Location = store.Location,
                        Enable = store.Enable,
                        IsTemp = store.IsTemp,
                        Remark = store.Remark,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.InventoryStores.Add(entity);
                    db.SaveChanges();
                    store.StoreCode = entity.StoreCode;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store ", "Success", User.Identity.Name, $"created store {entity.StoreCode} with name {entity.Name}  ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { store }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Stores_Update([DataSourceRequest]DataSourceRequest request, InventoryStore store)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var nameexists =
                    db.InventoryStores.Where(x => x.StoreCode != store.StoreCode)
                                       .Where(x => x.Name == store.Name).ToList();

                if (nameexists.Any())
                {
                    ModelState.AddModelError("Store Name",
                        store.Name + " already exists in the system.");
                    valid = false;
                }


                #endregion

                if (valid)
                {

                    var entity = new InventoryStore
                    {
                        StoreCode = store.StoreCode,
                        Name = store.Name,
                        BranchId = store.BranchId,
                        BusinessUnitId = store.BusinessUnitId,
                        Location = store.Location,
                        Enable = store.Enable,
                        IsTemp = store.IsTemp,
                        Remark = store.Remark,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    db.InventoryStores.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store ", "Success", User.Identity.Name, $"updated store {entity.StoreCode} with name {entity.Name}  ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { store }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Stores_Destroy([DataSourceRequest]DataSourceRequest request, InventoryStore store)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;

                ModelState.AddModelError("Store Delete",store.StoreCode + "cannot be deleted.");
                valid = false;

                #endregion

                if (valid)
                {
                    var entity = new InventoryStore
                    {
                        StoreCode = store.StoreCode,

                        Name = store.Name,
                        BranchId = store.BranchId,
                        BusinessUnitId = store.BusinessUnitId,
                        Location = store.Location,
                        Enable = store.Enable,
                        IsTemp = store.IsTemp,
                        Remark = store.Remark
                    };

                    db.InventoryStores.Attach(entity);
                    db.InventoryStores.Remove(entity);
                    db.SaveChanges();
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted store " + store.StoreCode);
                    #endregion
                }
            }

            return Json(new[] { store }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var store = db.InventoryStores.FirstOrDefault(p => p.StoreCode == id);
                if (store != null && store.Enable)
                {
                    store.Enable = false;
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Disable", "Success", User.Identity.Name, "Disabled store  " + store.StoreCode);
                    #endregion
                }
                else
                {
                    if (store != null)
                    {
                        store.Enable = true;

                        #region delete log
                        UserHelper.DeleteLog(Request.UserHostAddress, "Enable", "Success", User.Identity.Name,"Enabled store  " + store.StoreCode);
                        #endregion
                    }

                }
                if (store != null)
                {
                    db.InventoryStores.Attach(store);
                    db.Entry(store).State = EntityState.Modified;
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
