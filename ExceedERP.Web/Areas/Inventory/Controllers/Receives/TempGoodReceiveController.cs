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

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{
   
    public class TempGoodReceiveController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {

            PopulateViewData();
            return View();

        }

         private void PopulateViewData()
         {
             var categories = db.ItemCategorys;
             var items = db.InventoryItems;
             var branches = db.Branch;
             var tax = db.GLTaxRates;
             var stores = db.InventoryStores;
             var receivetype = db.ReceiveTypes;

             ViewData["stores"] = stores;
             ViewData["tax"] = tax;
             ViewData["ReeceiveType"] = receivetype;
             ViewData["Branches"] = branches;
             var selecteditems = items.Select(
                 i => new
                 {
                     Name = i.ItemCode + "_" + i.Name,
                     ItemCode = i.ItemCode
                 }).ToList();
            ViewData["Category"] = categories;
             ViewData["defaultCategory"] = categories.FirstOrDefault();

         }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult TempGoodReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<TempGoodReceive> tempgoodreceives = db.TempGoodReceives.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = tempgoodreceives.ToDataSourceResult(request, tempGoodReceive => new
            {
                TempGoodReceiveID = tempGoodReceive.TempGoodReceiveID,
                BranchId = tempGoodReceive.BranchId,
                StoreCode = tempGoodReceive.StoreCode,
                DeliveredBy = tempGoodReceive.DeliveredBy,
                ReceivedBy = tempGoodReceive.ReceivedBy,
                ReceiveDate = tempGoodReceive.ReceiveDate,
                Status = tempGoodReceive.Status,
                Remark = tempGoodReceive.Remark,
                IsVoid = tempGoodReceive.IsVoid,
                VoidBy = tempGoodReceive.VoidBy,
                VoidTime = tempGoodReceive.VoidTime,
                IsOnlineApproved = tempGoodReceive.IsOnlineApproved,
                OnlineApprovedBy = tempGoodReceive.OnlineApprovedBy,
                OnlineApprovedTime = tempGoodReceive.OnlineApprovedTime,
                IsSendForApproval = tempGoodReceive.IsSendForApproval,
                SendForApprovalBy = tempGoodReceive.SendForApprovalBy,
                SendForApprovalTime = tempGoodReceive.SendForApprovalTime,
                IsOnlineTransferred = tempGoodReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = tempGoodReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = tempGoodReceive.OnlineTransferredTime,
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TempGoodReceives_Create([DataSourceRequest]DataSourceRequest request, TempGoodReceive tempGoodReceive)
        {
            if (tempGoodReceive.ReceiveDate > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }
            if (ModelState.IsValid)
            {
                var entity = new TempGoodReceive
                {
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(tempGoodReceive.StoreCode),
                    StoreCode = tempGoodReceive.StoreCode,
                    DeliveredBy = tempGoodReceive.DeliveredBy,
                    ReceivedBy = tempGoodReceive.ReceivedBy,
                    ReceiveDate = tempGoodReceive.ReceiveDate.Date,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = tempGoodReceive.Remark,
                    IsVoid = tempGoodReceive.IsVoid,
                    VoidBy = tempGoodReceive.VoidBy,
                    VoidTime = tempGoodReceive.VoidTime,
                    IsOnlineApproved = tempGoodReceive.IsOnlineApproved,
                    OnlineApprovedBy = tempGoodReceive.OnlineApprovedBy,
                    OnlineApprovedTime = tempGoodReceive.OnlineApprovedTime,
                    IsOnlineTransferred = tempGoodReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = tempGoodReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = tempGoodReceive.OnlineTransferredTime,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(tempGoodReceive.StoreCode).ToString()
                };

                db.TempGoodReceives.Add(entity);
                db.SaveChanges();
                tempGoodReceive.TempGoodReceiveID = entity.TempGoodReceiveID;


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "temp good receive", "Success", User.Identity.Name, $"updated temp good receive {entity.TempGoodReceiveID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { tempGoodReceive }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TempGoodReceives_Update([DataSourceRequest]DataSourceRequest request, TempGoodReceive tempGoodReceive)
        {
            if (tempGoodReceive.ReceiveDate > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }
            if (ModelState.IsValid)
            {
                var entity = new TempGoodReceive
                {
                    TempGoodReceiveID = tempGoodReceive.TempGoodReceiveID,
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(tempGoodReceive.StoreCode),
                    StoreCode = tempGoodReceive.StoreCode,
                    DeliveredBy = tempGoodReceive.DeliveredBy,
                    ReceivedBy = tempGoodReceive.ReceivedBy,
                    ReceiveDate = tempGoodReceive.ReceiveDate,
                    Status = tempGoodReceive.Status,
                    Remark = tempGoodReceive.Remark,
                    IsVoid = tempGoodReceive.IsVoid,
                    VoidBy = tempGoodReceive.VoidBy,
                    VoidTime = tempGoodReceive.VoidTime,
                    IsOnlineApproved = tempGoodReceive.IsOnlineApproved,
                    OnlineApprovedBy = tempGoodReceive.OnlineApprovedBy,
                    OnlineApprovedTime = tempGoodReceive.OnlineApprovedTime,
                    IsOnlineTransferred = tempGoodReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = tempGoodReceive.IsOnlineTransferredBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                    OnlineTransferredTime = tempGoodReceive.OnlineTransferredTime,
                };

                db.TempGoodReceives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "temp good receive", "Success", User.Identity.Name, $"updated temp good receive {entity.TempGoodReceiveID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { tempGoodReceive }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TempGoodReceives_Destroy([DataSourceRequest]DataSourceRequest request, TempGoodReceive tempGoodReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new TempGoodReceive
                {
                    TempGoodReceiveID = tempGoodReceive.TempGoodReceiveID,
                    BranchId = tempGoodReceive.BranchId,
                    StoreCode = tempGoodReceive.StoreCode,
                    DeliveredBy = tempGoodReceive.DeliveredBy,
                    ReceivedBy = tempGoodReceive.ReceivedBy,
                    ReceiveDate = tempGoodReceive.ReceiveDate,
                    Status = tempGoodReceive.Status,
                    Remark = tempGoodReceive.Remark,
                    IsVoid = tempGoodReceive.IsVoid,
                    VoidBy = tempGoodReceive.VoidBy,
                    VoidTime = tempGoodReceive.VoidTime,
                    IsOnlineApproved = tempGoodReceive.IsOnlineApproved,
                    OnlineApprovedBy = tempGoodReceive.OnlineApprovedBy,
                    OnlineApprovedTime = tempGoodReceive.OnlineApprovedTime,
                    IsOnlineTransferred = tempGoodReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = tempGoodReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = tempGoodReceive.OnlineTransferredTime,
                };

                db.TempGoodReceives.Attach(entity);
                db.TempGoodReceives.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted temporary good receive " + tempGoodReceive.TempGoodReceiveID);
                #endregion
            }

            return Json(new[] { tempGoodReceive }.ToDataSourceResult(request, ModelState));
        }

       
         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
         public JsonResult TempGoodReceives_SendForApproval(int id)
         {
             var record = db.TempGoodReceives.Find(id);
             if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.TempGoodReceiveItem.Count > 0)
             {
                 record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                 record.IsSendForApproval = true;
                 record.SendForApprovalBy = User.Identity.Name;
                 record.SendForApprovalTime = DateTime.Now;
                 db.TempGoodReceives.Attach(record);
                 db.Entry(record).State = EntityState.Modified;
                 db.SaveChanges();
                 var formattedData = new
                 {
                     Message = "Successfully Sent.",
                     Status = "OK"
                 };
                 return Json(formattedData, JsonRequestBehavior.AllowGet);
             }
             else if (record == null)
             {
                 var formattedData = new
                 {
                     Message = "The record is not found.",
                     Status = "NOT_OK"
                 };
                 return Json(formattedData, JsonRequestBehavior.AllowGet);
             }

             else if (record.TempGoodReceiveItem.Count == 0)
             {
                 var formattedData = new
                 {
                     Message = "Please add line items first.",
                     Status = "NOT_OK"
                 };
                 return Json(formattedData, JsonRequestBehavior.AllowGet);
             }
             else if (record.IsSendForApproval)
             {
                 var formattedData = new
                 {
                     Message = "The record is already sent for approval you cannot send again.",
                     Status = "NOT_OK"
                 };
                 return Json(formattedData, JsonRequestBehavior.AllowGet);
             }

             else if (record.IsOnlineApproved)
             {
                 var formattedData = new
                 {
                     Message = "The record is already approved.",
                     Status = "NOT_OK"
                 };
                 return Json(formattedData, JsonRequestBehavior.AllowGet);
             }
             return null;
         }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
