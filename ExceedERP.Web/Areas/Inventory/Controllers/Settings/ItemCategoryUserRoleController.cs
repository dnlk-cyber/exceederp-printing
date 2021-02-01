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
    
    public class ItemCategoryUserRoleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult ItemCategoryUserRoles_Read(string id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ItemCategoryUserRole> itemcategoryuserroles = db.ItemCategoryUserRoles.Where(x=>x.ItemCategoryCode==id);
            DataSourceResult result = itemcategoryuserroles.ToDataSourceResult(request, itemCategoryUserRole => new {
                ItemCategoryUserRoleId = itemCategoryUserRole.ItemCategoryUserRoleId,
                ItemCategoryCode = itemCategoryUserRole.ItemCategoryCode,
                RoleName = itemCategoryUserRole.RoleName,
                Remark = itemCategoryUserRole.Remark,
                DateCreated = itemCategoryUserRole.DateCreated,
                LastModified = itemCategoryUserRole.LastModified,
                CreatedBy = itemCategoryUserRole.CreatedBy,
                ModifiedBy = itemCategoryUserRole.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemCategoryUserRoles_Create(string id,[DataSourceRequest]DataSourceRequest request, ItemCategoryUserRole itemCategoryUserRole)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemCategoryUserRole
                {
                    ItemCategoryCode = id,
                    RoleName = itemCategoryUserRole.RoleName,
                    Remark = itemCategoryUserRole.Remark,
                    DateCreated = itemCategoryUserRole.DateCreated,
                    LastModified = itemCategoryUserRole.LastModified,
                    CreatedBy = itemCategoryUserRole.CreatedBy,
                    ModifiedBy = itemCategoryUserRole.ModifiedBy
                };

                db.ItemCategoryUserRoles.Add(entity);
                db.SaveChanges();
                itemCategoryUserRole.ItemCategoryUserRoleId = entity.ItemCategoryUserRoleId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "inventory item category user mapping", "Success", User.Identity.Name, $"created category {entity.ItemCategoryCode} and user {entity.RoleName} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { itemCategoryUserRole }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemCategoryUserRoles_Update([DataSourceRequest]DataSourceRequest request, ItemCategoryUserRole itemCategoryUserRole)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemCategoryUserRole
                {
                    ItemCategoryUserRoleId = itemCategoryUserRole.ItemCategoryUserRoleId,
                    ItemCategoryCode = itemCategoryUserRole.ItemCategoryCode,
                    RoleName = itemCategoryUserRole.RoleName,
                    Remark = itemCategoryUserRole.Remark,
                    DateCreated = itemCategoryUserRole.DateCreated,
                    LastModified = itemCategoryUserRole.LastModified,
                    CreatedBy = itemCategoryUserRole.CreatedBy,
                    ModifiedBy = itemCategoryUserRole.ModifiedBy
                };

                db.ItemCategoryUserRoles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "inventory item category user mapping", "Success", User.Identity.Name, $"updated category {entity.ItemCategoryCode} and user {entity.RoleName} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { itemCategoryUserRole }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemCategoryUserRoles_Destroy([DataSourceRequest]DataSourceRequest request, ItemCategoryUserRole itemCategoryUserRole)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemCategoryUserRole
                {
                    ItemCategoryUserRoleId = itemCategoryUserRole.ItemCategoryUserRoleId,
                    ItemCategoryCode = itemCategoryUserRole.ItemCategoryCode,
                    RoleName = itemCategoryUserRole.RoleName,
                    Remark = itemCategoryUserRole.Remark,
                    DateCreated = itemCategoryUserRole.DateCreated,
                    LastModified = itemCategoryUserRole.LastModified,
                    CreatedBy = itemCategoryUserRole.CreatedBy,
                    ModifiedBy = itemCategoryUserRole.ModifiedBy
                };

                db.ItemCategoryUserRoles.Attach(entity);
                db.ItemCategoryUserRoles.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted item category {itemCategoryUserRole.ItemCategoryCode} and user {itemCategoryUserRole.RoleName}");
                #endregion
            }

            return Json(new[] { itemCategoryUserRole }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
