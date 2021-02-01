using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class ItemCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext appdb = new ApplicationDbContext();

        public ActionResult Index()
        {
            PopulateStoreAndEmployee();
            return View();
        }
        private void PopulateStoreAndEmployee()
        {

            var users = appdb.Users.AsQueryable();
            var selectedUser = users.Select(i => new {
                    Name = i.UserName,
                    Code = i.Id
                })
                .AsQueryable();

            ViewData["users"] = selectedUser;

        }

        public ActionResult ItemCategorys_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ItemCategory> itemcategorys = db.ItemCategorys;
            DataSourceResult result = itemcategorys.ToDataSourceResult(request, itemCategory => new
            {
                ItemCategoryCode = itemCategory.ItemCategoryCode,
                Name = itemCategory.Name,
                Abbreviation = itemCategory.Abbreviation,
                Description = itemCategory.Description,
                HaveParent = itemCategory.HaveParent,
                ParentName = itemCategory.ParentName,
                AccountCode = itemCategory.AccountCode,
                IssueAccountCode = itemCategory.IssueAccountCode,
                IsFixedAsset = itemCategory.IsFixedAsset,
                CostType = itemCategory.CostType
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemCategorys_Create([DataSourceRequest]DataSourceRequest request, ItemCategory itemCategory)
        {
            #region validation

            bool valid = true;
            if (!String.IsNullOrEmpty(itemCategory.AccountCode) && !String.IsNullOrEmpty(itemCategory.ItemCategoryCode))
            {
                //var accountSegment = itemCategory.Description.Split('-')[0];
                //if (!itemCategory.ItemCategoryCode.StartsWith(accountSegment)&& !itemCategory.HaveParent)
                //{
                //    ModelState.AddModelError("Category code", itemCategory.ItemCategoryCode + " and account code prefix " + accountSegment + "  don't match.");
                //    valid = false;
                //}

            }
            else
            {
                ModelState.AddModelError("Category code and account code", "Please fill category code and account code.");
                valid = false;
            }

            var exists = db.ItemCategorys.Where(x => x.ItemCategoryCode == itemCategory.ItemCategoryCode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Category code", itemCategory.ItemCategoryCode + " already exists in the system.");
                valid = false;
            }

            var accountcodeexists =
                db.ItemCategorys.Where(x => x.ItemCategoryCode != itemCategory.ItemCategoryCode)
                    .Where(x => x.AccountCode == itemCategory.AccountCode &&  !itemCategory.HaveParent).ToList();

            if (accountcodeexists.Any())
            {
                ModelState.AddModelError("Account code", itemCategory.AccountCode + " already exists in the system.");
                valid = false;
            }

            var nameexists = db.ItemCategorys.Where(x => x.ItemCategoryCode != itemCategory.ItemCategoryCode)
                .Where(x => x.Name == itemCategory.Name).ToList();

            if (nameexists.Any())
            {
                ModelState.AddModelError("Category Name", itemCategory.Name + " already exists in the system.");
                valid = false;
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new ItemCategory
                    {

                        ItemCategoryCode = itemCategory.ItemCategoryCode,
                        Name = itemCategory.Name,
                        HaveParent = itemCategory.HaveParent,
                        Abbreviation = itemCategory.Abbreviation,
                        ParentName = itemCategory.ParentName,
                        Description = itemCategory.Description,
                        IssueAccountCode = itemCategory.IssueAccountCode,
                        AccountCode = itemCategory.AccountCode,
                        IsFixedAsset = itemCategory.IsFixedAsset,
                        CostType = itemCategory.CostType,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.ItemCategorys.Add(entity);
                    db.SaveChanges();
                    itemCategory.ItemCategoryCode = entity.ItemCategoryCode;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "inventory item category", "Success", User.Identity.Name, $"created category {entity.ItemCategoryCode} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { itemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemCategorys_Update([DataSourceRequest]DataSourceRequest request, ItemCategory itemCategory)
        {
            #region validation

            bool valid = true;
            if (!String.IsNullOrEmpty(itemCategory.AccountCode) && !String.IsNullOrEmpty(itemCategory.ItemCategoryCode))
            {
                //var accountSegment = itemCategory.Description.Split('-')[0];
                //if (!itemCategory.ItemCategoryCode.StartsWith(accountSegment) && !itemCategory.HaveParent)
                //{
                //    ModelState.AddModelError("Category code", itemCategory.ItemCategoryCode + " and account code prefix " + accountSegment + "  don't match.");
                //    valid = false;
                //}

            }
            else
            {
                ModelState.AddModelError("Category code and account code", "Please fill category code and account code.");
                valid = false;
            }

            //var exists = db.ItemCategorys.Where(x => x.ItemCategoryCode == itemCategory.ItemCategoryCode).Where(x => x.ItemCategoryCode == itemCategory.ItemCategoryCode).ToList();

            //if (exists.Any())
            //{
            //    ModelState.AddModelError("Category code", itemCategory.ItemCategoryCode + " already exists in the system.");
            //    valid = false;
            //}

            var accountcodeexists =
                db.ItemCategorys.Where(x => x.ItemCategoryCode != itemCategory.ItemCategoryCode)
                    .Where(x => x.AccountCode == itemCategory.AccountCode && !itemCategory.HaveParent).ToList();

            if (accountcodeexists.Any())
            {
                ModelState.AddModelError("Account code", itemCategory.AccountCode + " already exists in the system.");
                valid = false;
            }

            var nameexists = db.ItemCategorys.Where(x => x.ItemCategoryCode != itemCategory.ItemCategoryCode)
                .Where(x => x.Name == itemCategory.Name).ToList();

            if (nameexists.Any())
            {
                ModelState.AddModelError("Category Name", itemCategory.Name + " already exists in the system.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new ItemCategory
                    {
                        ItemCategoryCode = itemCategory.ItemCategoryCode,

                        Name = itemCategory.Name,
                        HaveParent = itemCategory.HaveParent,
                        Abbreviation = itemCategory.Abbreviation,
                        ParentName = itemCategory.ParentName,
                        Description = itemCategory.Description,
                        AccountCode = itemCategory.AccountCode,
                        IssueAccountCode = itemCategory.IssueAccountCode,
                        IsFixedAsset = itemCategory.IsFixedAsset,
                        CostType = itemCategory.CostType,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    db.ItemCategorys.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "inventory item category", "Success", User.Identity.Name, $"updated category {entity.ItemCategoryCode} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { itemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemCategorys_Destroy([DataSourceRequest]DataSourceRequest request, ItemCategory itemCategory)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var categories = db.ItemCategorys.Where(x => x.ParentName == itemCategory.ItemCategoryCode).ToList();

                if (categories.Any())
                {
                    ModelState.AddModelError("Category code", itemCategory.ItemCategoryCode + " Cannot be deleted ,it has child categories under it.");
                    valid = false;
                }

                var itemCategories =
                    db.InventoryItems.Where(x => x.ItemCategoryCode == itemCategory.ItemCategoryCode).ToList();

                if (itemCategories.Any())
                {
                    ModelState.AddModelError("Category Items",
                        itemCategory.ItemCategoryCode + "  Cannot be deleted ,it has items under it.");
                    valid = false;
                }


                #endregion

                if (valid)
                {
                    var entity = new ItemCategory
                    {
                        ItemCategoryCode = itemCategory.ItemCategoryCode,
                        HaveParent = itemCategory.HaveParent,
                        Abbreviation = itemCategory.Abbreviation,
                        ParentName = itemCategory.ParentName,
                        IssueAccountCode = itemCategory.IssueAccountCode,
                        Name = itemCategory.Name,
                        Description = itemCategory.Description,
                        AccountCode = itemCategory.AccountCode,
                        IsFixedAsset = itemCategory.IsFixedAsset
                    };

                    db.ItemCategorys.Attach(entity);
                    db.ItemCategorys.Remove(entity);
                    db.SaveChanges();
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted item category " + itemCategory.Name );
                    #endregion
                }
            }

            return Json(new[] { itemCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
