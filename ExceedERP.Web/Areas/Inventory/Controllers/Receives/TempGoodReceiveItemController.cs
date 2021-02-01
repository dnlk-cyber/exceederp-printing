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

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{
    
    public class TempGoodReceiveItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult TempGoodReceiveItems_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<TempGoodReceiveItem> tempgoodreceiveItems = db.TempGoodReceiveItems.Where(tgi => tgi.TempGoodReceiveID == id);
            DataSourceResult result = tempgoodreceiveItems.ToDataSourceResult(request, tempGoodReceiveItem => new {
                TempGoodReceiveID = tempGoodReceiveItem.TempGoodReceiveID,
                TempGoodReceiveItemID = tempGoodReceiveItem.TempGoodReceiveItemID,
                ItemCategoryCode = tempGoodReceiveItem.ItemCategoryCode,
                ItemCode = tempGoodReceiveItem.ItemCode,
                Quantity = tempGoodReceiveItem.Quantity,
                Remark = tempGoodReceiveItem.Remark,
                OrgBranchName = tempGoodReceiveItem.OrgBranchName,
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TempGoodReceiveItems_Create([DataSourceRequest]DataSourceRequest request, TempGoodReceiveItem tempGoodReceiveItem , int id )
        {
            if (ModelState.IsValid)
            {
                var entity = new TempGoodReceiveItem
                {
                    TempGoodReceiveID =  id,
                    ItemCategoryCode = tempGoodReceiveItem.ItemCategoryCode,
                    ItemCode = tempGoodReceiveItem.ItemCode,
                    Quantity = tempGoodReceiveItem.Quantity,
                    Remark = tempGoodReceiveItem.Remark,
                    OrgBranchName = tempGoodReceiveItem.OrgBranchName,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                };

                db.TempGoodReceiveItems.Add(entity);
                db.SaveChanges();
                tempGoodReceiveItem.TempGoodReceiveItemID = entity.TempGoodReceiveItemID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "temp good receive item", "Success", User.Identity.Name, $"created temp good receive {entity.ItemCode} under record {entity.TempGoodReceiveID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { tempGoodReceiveItem }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TempGoodReceiveItems_Update([DataSourceRequest]DataSourceRequest request, TempGoodReceiveItem tempGoodReceiveItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new TempGoodReceiveItem
                {
                    TempGoodReceiveID = tempGoodReceiveItem.TempGoodReceiveID,
                    TempGoodReceiveItemID = tempGoodReceiveItem.TempGoodReceiveItemID,
                    ItemCategoryCode = tempGoodReceiveItem.ItemCategoryCode,
                    ItemCode = tempGoodReceiveItem.ItemCode,
                    Quantity = tempGoodReceiveItem.Quantity,
                    Remark = tempGoodReceiveItem.Remark,
                    OrgBranchName = tempGoodReceiveItem.OrgBranchName,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                db.TempGoodReceiveItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "temp good receive item", "Success", User.Identity.Name, $"created temp good receive {entity.ItemCode} under record {entity.TempGoodReceiveID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { tempGoodReceiveItem }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TempGoodReceiveItems_Destroy([DataSourceRequest]DataSourceRequest request, TempGoodReceiveItem tempGoodReceiveItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new TempGoodReceiveItem
                {
                    TempGoodReceiveItemID = tempGoodReceiveItem.TempGoodReceiveItemID,
                    ItemCategoryCode = tempGoodReceiveItem.ItemCategoryCode,
                    ItemCode = tempGoodReceiveItem.ItemCode,
                    Quantity = tempGoodReceiveItem.Quantity,
                    Remark = tempGoodReceiveItem.Remark,
                    OrgBranchName = tempGoodReceiveItem.OrgBranchName,
                };

                db.TempGoodReceiveItems.Attach(entity);
                db.TempGoodReceiveItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted temporary good receive item " + tempGoodReceiveItem.TempGoodReceiveID + " " + tempGoodReceiveItem.InventoryItem.Name);
                #endregion
            }

            return Json(new[] { tempGoodReceiveItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
