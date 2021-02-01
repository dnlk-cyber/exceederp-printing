using System;
using System.Collections.Generic;
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
    [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
    public class StoreItemBalanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }

        private void PopulateCategories()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var stores = db.InventoryStores;
            ViewData["Stores"] = stores;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }

       
        public ActionResult  StoreItemBalances_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> myStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<StoreItemAssignment> storeItemassignments = db.StoreItemAssignments.Where(x => myStores.Contains(x.StoreCode));
            DataSourceResult result = storeItemassignments.ToDataSourceResult(request, storeItemAssignment => new {
                StoreItemAssignmentID = storeItemAssignment.StoreItemAssignmentID,
                StoreCode = storeItemAssignment.StoreCode,
                ItemCategoryCode = storeItemAssignment.ItemCategoryCode,
                ItemCode = storeItemAssignment.ItemCode,
                ReOrderPoint = storeItemAssignment.ReOrderPoint,
                MinPoint = storeItemAssignment.MinPoint,
                MaxPoint = storeItemAssignment.MaxPoint,
                Balance = storeItemAssignment.Balance,
                AvgPrice = storeItemAssignment.AvgPrice
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult  StoreItemBalances_Create([DataSourceRequest]DataSourceRequest request, StoreItemAssignment storeItemAssignment)
        {
             
             
            if (ModelState.IsValid)
            {
                  #region validation

                    bool valid = true;
                    var exists = db.StoreItemAssignments.Where(x => x.StoreCode == storeItemAssignment.StoreCode)
                                                          .Where(x => x.ItemCode == storeItemAssignment.ItemCode).ToList();

                    if (exists.Any())
                    {
                        ModelState.AddModelError("Store Item",
                            storeItemAssignment.StoreCode + " and " + storeItemAssignment.ItemCode + " already exists in the system.");
                        valid = false;
                    }

                    #endregion

                if (valid)
                {

                    var entity = new StoreItemAssignment
                    {
                        StoreCode = storeItemAssignment.StoreCode,
                        ItemCategoryCode = storeItemAssignment.ItemCategoryCode,
                        ItemCode = storeItemAssignment.ItemCode,
                        ReOrderPoint = storeItemAssignment.ReOrderPoint,
                        MinPoint = storeItemAssignment.MinPoint,
                        MaxPoint = storeItemAssignment.MaxPoint,
                        Balance = storeItemAssignment.Balance,
                        AvgPrice = storeItemAssignment.AvgPrice,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.StoreItemAssignments.Add(entity);
                    db.SaveChanges();
                    storeItemAssignment.StoreItemAssignmentID = entity.StoreItemAssignmentID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store item balance", "Success", User.Identity.Name, $"created store item balance {entity.ItemCode} at store {entity.StoreCode}  ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { storeItemAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult  StoreItemBalances_Update([DataSourceRequest]DataSourceRequest request, StoreItemAssignment storeItemAssignment)
        {
            if (ModelState.IsValid)
            {
                 #region validation

                    bool valid = true;
                    var exists = db.StoreItemAssignments.Where(x=>x.StoreItemAssignmentID==storeItemAssignment.StoreItemAssignmentID)
                                                         .Where(x => x.StoreCode == storeItemAssignment.StoreCode)
                                                          .Where(x=>x.ItemCode==storeItemAssignment.ItemCode).ToList();

                    if (exists.Any())
                    {
                        ModelState.AddModelError("Store Item",
                            storeItemAssignment.StoreCode + " and " + storeItemAssignment.ItemCode+ " already exists in the system.");
                        valid = false;
                    }

                    #endregion

                if (valid)
                {
                    var entity = new StoreItemAssignment
                    {
                        StoreItemAssignmentID = storeItemAssignment.StoreItemAssignmentID,
                        StoreCode = storeItemAssignment.StoreCode,
                        ItemCategoryCode = storeItemAssignment.ItemCategoryCode,
                        ItemCode = storeItemAssignment.ItemCode,
                        ReOrderPoint = storeItemAssignment.ReOrderPoint,
                        MinPoint = storeItemAssignment.MinPoint,
                        MaxPoint = storeItemAssignment.MaxPoint,
                        Balance = storeItemAssignment.Balance,
                        AvgPrice = storeItemAssignment.AvgPrice,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now

                    };

                    db.StoreItemAssignments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store item balance", "Success", User.Identity.Name, $"updated store item balance {entity.ItemCode} at store {entity.StoreCode}  ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { storeItemAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult  StoreItemBalances_Destroy([DataSourceRequest]DataSourceRequest request, StoreItemAssignment storeItemAssignment)
        {
            if (ModelState.IsValid)
            {
                #region validation

                    bool valid = true;
                    valid = false;
                    #endregion

                if (valid)
                {
                    var entity = new StoreItemAssignment
                    {
                        StoreItemAssignmentID = storeItemAssignment.StoreItemAssignmentID,
                        StoreCode = storeItemAssignment.StoreCode,
                        ItemCategoryCode = storeItemAssignment.ItemCategoryCode,
                        ItemCode = storeItemAssignment.ItemCode,
                        ReOrderPoint = storeItemAssignment.ReOrderPoint,
                        MinPoint = storeItemAssignment.MinPoint,
                        MaxPoint = storeItemAssignment.MaxPoint,
                        Balance = storeItemAssignment.Balance,
                        AvgPrice = storeItemAssignment.AvgPrice
                    };

                    db.StoreItemAssignments.Attach(entity);
                    db.StoreItemAssignments.Remove(entity);
                    db.SaveChanges();
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted store item " + storeItemAssignment.StoreCode + " " + storeItemAssignment.InventoryItem.Name);
                    #endregion
                }
            }

            return Json(new[] { storeItemAssignment }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing); 
        }
       
    }
}
