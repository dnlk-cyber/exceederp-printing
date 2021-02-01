using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{
   
    public class GoodSellingLendingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private DistributionHelper _distributionHelper = new DistributionHelper();
        private InventoryTransferHelper _transferHelper=new InventoryTransferHelper();
       

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

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;



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

            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult GoodSellingAndLendings_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<GoodSellingAndLending> goodsellingandlendings = db.GoodSellingAndLendings.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = goodsellingandlendings.ToDataSourceResult(request, goodSellingAndLending => new {
                GoodSellingAndLendingID = goodSellingAndLending.GoodSellingAndLendingID,
                SellOrLend = goodSellingAndLending.SellOrLend,
                StoreCode = goodSellingAndLending.StoreCode,
                Buyer = goodSellingAndLending.Buyer,
                BuyerAddress = goodSellingAndLending.BuyerAddress,
                Date = goodSellingAndLending.Date,
                Status = goodSellingAndLending.Status,
                Remark = goodSellingAndLending.Remark,
                IsVoid = goodSellingAndLending.IsVoid,
                VoidBy = goodSellingAndLending.VoidBy,
                VoidTime = goodSellingAndLending.VoidTime,
                IsOnlineApproved = goodSellingAndLending.IsOnlineApproved,
                OnlineApprovedBy = goodSellingAndLending.OnlineApprovedBy,
                OnlineApprovedTime = goodSellingAndLending.OnlineApprovedTime,
                IsSendForApproval = goodSellingAndLending.IsSendForApproval,
                SendForApprovalBy = goodSellingAndLending.SendForApprovalBy,
                SendForApprovalTime = goodSellingAndLending.SendForApprovalTime,
                IsOnlineTransferred = goodSellingAndLending.IsOnlineTransferred,
                IsOnlineTransferredBy = goodSellingAndLending.IsOnlineTransferredBy,
                OnlineTransferredTime = goodSellingAndLending.OnlineTransferredTime,
                
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodSellingAndLendings_Create([DataSourceRequest]DataSourceRequest request, GoodSellingAndLending goodSellingAndLending)
        {
            if (ModelState.IsValid)
            {
                var entity = new GoodSellingAndLending
                {
                    SellOrLend = goodSellingAndLending.SellOrLend,
                    StoreCode = goodSellingAndLending.StoreCode,
                    Buyer = goodSellingAndLending.Buyer,
                    BuyerAddress = goodSellingAndLending.BuyerAddress,
                    Date = goodSellingAndLending.Date.Date,
                    Status = Enum.GetName(typeof(OperationStatuses) , 5),
                    Remark = goodSellingAndLending.Remark,
                    IsVoid = goodSellingAndLending.IsVoid,
                    VoidBy = goodSellingAndLending.VoidBy,
                    VoidTime = goodSellingAndLending.VoidTime,
                    IsOnlineApproved = goodSellingAndLending.IsOnlineApproved,
                    OnlineApprovedBy = goodSellingAndLending.OnlineApprovedBy,
                    OnlineApprovedTime = goodSellingAndLending.OnlineApprovedTime,
                    IsOnlineTransferred = goodSellingAndLending.IsOnlineTransferred,
                    IsOnlineTransferredBy = goodSellingAndLending.IsOnlineTransferredBy,
                    OnlineTransferredTime = goodSellingAndLending.OnlineTransferredTime,
                    DateCreated = DateTime.Now, 
                    CreatedBy = User.Identity.Name,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(goodSellingAndLending.StoreCode).ToString()
                };

                db.GoodSellingAndLendings.Add(entity);
                db.SaveChanges();
                goodSellingAndLending.GoodSellingAndLendingID = entity.GoodSellingAndLendingID;


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good selling", "Success", User.Identity.Name, $"created good selling {entity.GoodSellingAndLendingID} ", Modules.Inventory);
                #endregion

            }

            return Json(new[] { goodSellingAndLending }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodSellingAndLendings_Update([DataSourceRequest]DataSourceRequest request, GoodSellingAndLending goodSellingAndLending)
        {
            if (ModelState.IsValid)
            {
                var entity = new GoodSellingAndLending
                {
                    GoodSellingAndLendingID = goodSellingAndLending.GoodSellingAndLendingID,
                    SellOrLend = goodSellingAndLending.SellOrLend,
                    StoreCode = goodSellingAndLending.StoreCode,
                    Buyer = goodSellingAndLending.Buyer,
                    BuyerAddress = goodSellingAndLending.BuyerAddress,
                    Date = goodSellingAndLending.Date,
                    Status = goodSellingAndLending.Status,
                    Remark = goodSellingAndLending.Remark,
                    IsVoid = goodSellingAndLending.IsVoid,
                    VoidBy = goodSellingAndLending.VoidBy,
                    VoidTime = goodSellingAndLending.VoidTime,
                    IsOnlineApproved = goodSellingAndLending.IsOnlineApproved,
                    OnlineApprovedBy = goodSellingAndLending.OnlineApprovedBy,
                    OnlineApprovedTime = goodSellingAndLending.OnlineApprovedTime,
                    IsOnlineTransferred = goodSellingAndLending.IsOnlineTransferred,
                    IsOnlineTransferredBy = goodSellingAndLending.IsOnlineTransferredBy,
                    OnlineTransferredTime = goodSellingAndLending.OnlineTransferredTime,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.GoodSellingAndLendings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good selling", "Success", User.Identity.Name, $"updated good selling {entity.GoodSellingAndLendingID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { goodSellingAndLending }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodSellingAndLendings_Destroy([DataSourceRequest]DataSourceRequest request, GoodSellingAndLending goodSellingAndLending)
        {
            if (ModelState.IsValid)
            {
                var entity = new GoodSellingAndLending
                {
                    GoodSellingAndLendingID = goodSellingAndLending.GoodSellingAndLendingID,
                    SellOrLend = goodSellingAndLending.SellOrLend,
                    StoreCode = goodSellingAndLending.StoreCode,
                    Buyer = goodSellingAndLending.Buyer,
                    BuyerAddress = goodSellingAndLending.BuyerAddress,
                    Date = goodSellingAndLending.Date,
                    Status = goodSellingAndLending.Status,
                    Remark = goodSellingAndLending.Remark,
                    IsVoid = goodSellingAndLending.IsVoid,
                    VoidBy = goodSellingAndLending.VoidBy,
                    VoidTime = goodSellingAndLending.VoidTime,
                    IsOnlineApproved = goodSellingAndLending.IsOnlineApproved,
                    OnlineApprovedBy = goodSellingAndLending.OnlineApprovedBy,
                    OnlineApprovedTime = goodSellingAndLending.OnlineApprovedTime,
                    IsOnlineTransferred = goodSellingAndLending.IsOnlineTransferred,
                    IsOnlineTransferredBy = goodSellingAndLending.IsOnlineTransferredBy,
                    OnlineTransferredTime = goodSellingAndLending.OnlineTransferredTime,
                };

                db.GoodSellingAndLendings.Attach(entity);
                db.GoodSellingAndLendings.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted inventory sales number " + goodSellingAndLending.GoodSellingAndLendingID + " " + goodSellingAndLending.Buyer);
                #endregion
            }

            return Json(new[] { goodSellingAndLending }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.GoodSellingAndLendings.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.GoodSellingAndLendingItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.GoodSellingAndLendings.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good selling", "Success", User.Identity.Name, $"send for approval selling {id} ", Modules.Inventory);
                #endregion
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

            else if (record.GoodSellingAndLendingItems.Count == 0)
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

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = db.GoodSellingAndLendings.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                InventoryTransferHelper transferHelper = new InventoryTransferHelper();
                string response = transferHelper.GoodSelling(myid, description, User?.Identity?.Name ?? "");
                if (response != "SUCCESS")
                {
                    var formattedData = new
                    {
                        Message = response,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "Successfuly Transferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }


            }
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record cannot be transfered before validation.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult GenerateDistribution(int id)
        {

            var record = db.GoodSellingAndLendings.Find(id);
            if (record != null && !record.IsOnlineTransferred)
            {
                _distributionHelper.UpdateGoodSellingDistribution(record);
                var formattedData1 = new
                {
                    Message = "Complete.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            var formattedData = new
            {
                Message = "Record not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
