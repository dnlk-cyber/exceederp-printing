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
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Transfer
{

    public class TransferSendController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly DistributionHelper _distributionHelper = new DistributionHelper();
        private InventoryTransferHelper transferHelper;

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }
        private void PopulateCategories()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            var stores = db.InventoryStores;
            var suppliers = db.ProcurementSuppliers;
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(i => new
            {
                Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                Code = i.EmployeeId
            }).AsQueryable();

            ViewData["suppliers"] = suppliers;
            ViewData["stores"] = stores;
            ViewData["defaultstore"] = stores.First();
            ViewData["personEmployees"] = selectedEmployee;
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult ItemTransfers_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //IQueryable<ItemTransfer> itemtransfers = db.ItemTransfers.Where(x => mybraches.Contains(x.OrgBranchName));

            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<ItemTransfer> itemtransfers = db.ItemTransfers.Where(x => userStores.Contains(x.FromStore));

            DataSourceResult result = itemtransfers.ToDataSourceResult(request, itemTransfer => new
            {
                ItemTransferID = itemTransfer.ItemTransferID,
                EmployeeId = itemTransfer.EmployeeId,
                StoreRequisitionID = itemTransfer.StoreRequisitionID,
                DocumentRef = itemTransfer.DocumentRef,
                TransferDate = itemTransfer.TransferDate,
                FromStore = itemTransfer.FromStore,
                ToStore = itemTransfer.ToStore,
                DeliveredBy = itemTransfer.DeliveredBy,
                PlateNo = itemTransfer.PlateNo,
                SupplierName = itemTransfer.SupplierName,
                DriverType = itemTransfer.DriverType,
                Status = itemTransfer.Status,
                Remark = itemTransfer.Remark,
                GLPeriodId = itemTransfer.GLPeriodId,
                BranchId = itemTransfer.BranchId,
                BusinessUnitId = itemTransfer.BusinessUnitId,
                IsVoid = itemTransfer.IsVoid,
                VoidBy = itemTransfer.VoidBy,
                VoidTime = itemTransfer.VoidTime,
                IsOnlineApproved = itemTransfer.IsOnlineApproved,
                OnlineApprovedBy = itemTransfer.OnlineApprovedBy,
                OnlineApprovedTime = itemTransfer.OnlineApprovedTime,
                IsSendForApproval = itemTransfer.IsSendForApproval,
                SendForApprovalBy = itemTransfer.SendForApprovalBy,
                SendForApprovalTime = itemTransfer.SendForApprovalTime,
                IsOnlineTransferred = itemTransfer.IsOnlineTransferred,
                IsOnlineTransferredBy = itemTransfer.IsOnlineTransferredBy,
                OnlineTransferredTime = itemTransfer.OnlineTransferredTime,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransfers_Create([DataSourceRequest]DataSourceRequest request, ItemTransfer itemTransfer)
        {
            var storeRequisition = db.StoreRequisitions.Find(itemTransfer.StoreRequisitionID);
            itemTransfer.BusinessUnitId = storeRequisition.BusinessUnitId;
            ModelState.Remove("FromStore");
            ModelState.Remove("ToStore");
            ModelState.Remove("BranchId");

            if (itemTransfer.GLPeriodId != 0)
            {
                var inventoryPeriod = db.InventoryPeriods.Find(itemTransfer.GLPeriodId);
                var openPeriod = db.InventoryStorePeriodStatuses
                    .Where(x => x.InventoryPeriodId == itemTransfer.GLPeriodId).FirstOrDefault(x => x.StoreCode == itemTransfer.FromStore);

                if(openPeriod != null && openPeriod.Status!=FiscalYearStatus.Open)
                    ModelState.AddModelError("open","The selected period is not open for this store.");

                if (inventoryPeriod != null)
                {
                    if (!((itemTransfer.TransferDate.Date >= inventoryPeriod.DateFrom) &&
                          (itemTransfer.TransferDate.Date <= inventoryPeriod.DateTo)))
                        ModelState.AddModelError("",
                            "Transfer Date should be within the period: " + inventoryPeriod.Name + "(From " +
                            inventoryPeriod.DateFrom.ToShortDateString() + " To " +
                            inventoryPeriod.DateTo.ToShortDateString() + ")");

                }
                else
                {
                    ModelState.AddModelError("", "The selected Period is not defined.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select period.");
            }
            if (itemTransfer.TransferDate > DateTime.Now)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (!String.IsNullOrEmpty(itemTransfer.DocumentRef))
            {
                var exists = db.ItemTransfers.Where(x => x.DocumentRef == itemTransfer.DocumentRef && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", itemTransfer.DocumentRef + " already exists in the system.");
                }
            }

            if (ModelState.IsValid)
            {
                if (storeRequisition != null)
                {
                    var entity = new ItemTransfer
                    {
                        StoreRequisitionID = itemTransfer.StoreRequisitionID,
                        EmployeeId = (int)storeRequisition.EmployeeId,
                        BranchId =ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeRequisition.StoreCode),
                        BusinessUnitId=itemTransfer.BusinessUnitId,
                        TransferDate = itemTransfer.TransferDate.Date,
                        FromStore = storeRequisition.StoreCode,
                        ToStore = storeRequisition.ToStoreCode,
                        DeliveredBy = itemTransfer.DeliveredBy,
                        PlateNo = itemTransfer.PlateNo,
                        SupplierName = itemTransfer.SupplierName,
                        DriverType = itemTransfer.DriverType,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = itemTransfer.Remark,
                        GLPeriodId = itemTransfer.GLPeriodId,
                        IsVoid = itemTransfer.IsVoid,
                        VoidBy = itemTransfer.VoidBy,
                        VoidTime = itemTransfer.VoidTime,
                        IsOnlineApproved = itemTransfer.IsOnlineApproved,
                        OnlineApprovedBy = itemTransfer.OnlineApprovedBy,
                        OnlineApprovedTime = itemTransfer.OnlineApprovedTime,
                        IsOnlineTransferred = itemTransfer.IsOnlineTransferred,
                        IsOnlineTransferredBy = itemTransfer.IsOnlineTransferredBy,
                        OnlineTransferredTime = itemTransfer.OnlineTransferredTime,
                        DocumentRef = itemTransfer.DocumentRef,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeRequisition.StoreCode).ToString()
                    };

                    db.ItemTransfers.Add(entity);
                    db.SaveChanges();
                    itemTransfer.ItemTransferID = entity.ItemTransferID;
                    RequestedItem(entity);
                   _distributionHelper.UpdateTransferSendDistribution(entity);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "transfer send", "Success", User.Identity.Name, $"created transfer send {entity.ItemTransferID} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { itemTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransfers_Update([DataSourceRequest]DataSourceRequest request, ItemTransfer itemTransfer)
        {

            var storeRequisition = db.StoreRequisitions.Find(itemTransfer.StoreRequisitionID);
            itemTransfer.BusinessUnitId = storeRequisition.BusinessUnitId;

            ModelState.Remove("FromStore");
            ModelState.Remove("ToStore");
            ModelState.Remove("BranchId");

            if (itemTransfer.GLPeriodId != 0)
            {
                var inventoryPeriod = db.InventoryPeriods.Find(itemTransfer.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    var openPeriod = db.InventoryStorePeriodStatuses
                        .Where(x => x.InventoryPeriodId == itemTransfer.GLPeriodId).FirstOrDefault(x => x.StoreCode == itemTransfer.FromStore);

                    if (openPeriod != null && openPeriod.Status != FiscalYearStatus.Open)
                        ModelState.AddModelError("open", "The selected period is not open for this store.");

                    if (!((itemTransfer.TransferDate.Date >= inventoryPeriod.DateFrom) &&
                          (itemTransfer.TransferDate.Date <= inventoryPeriod.DateTo)))
                        ModelState.AddModelError("",
                            "Transfer Date should be within the period: " + inventoryPeriod.Name + "(From " +
                            inventoryPeriod.DateFrom.ToShortDateString() + " To " +
                            inventoryPeriod.DateTo.ToShortDateString() + ")");

                }
                else
                {
                    ModelState.AddModelError("", "The selected Period is not defined.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select period.");
            }
            if (itemTransfer.TransferDate > DateTime.Now)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (!String.IsNullOrEmpty(itemTransfer.DocumentRef))
            {
                var exists = db.ItemTransfers.Where(x => x.ItemTransferID != itemTransfer.ItemTransferID).Where(x => x.DocumentRef == itemTransfer.DocumentRef && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", itemTransfer.DocumentRef + " already exists in the system.");
                }
            }

            if (ModelState.IsValid)
            {
                if (storeRequisition != null)
                {
                    var entity = new ItemTransfer
                    {
                        ItemTransferID = itemTransfer.ItemTransferID,
                        EmployeeId = (int)storeRequisition.EmployeeId,
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeRequisition.StoreCode),
                        BusinessUnitId=itemTransfer.BusinessUnitId,
                        StoreRequisitionID = itemTransfer.StoreRequisitionID,
                        TransferDate = itemTransfer.TransferDate,
                        FromStore = storeRequisition.StoreCode,
                        ToStore = storeRequisition.ToStoreCode,
                        DeliveredBy = itemTransfer.DeliveredBy,
                        PlateNo = itemTransfer.PlateNo,
                        SupplierName = itemTransfer.SupplierName,
                        DriverType = itemTransfer.DriverType,
                        Remark = itemTransfer.Remark,
                        GLPeriodId = itemTransfer.GLPeriodId,
                        IsVoid = itemTransfer.IsVoid,
                        VoidBy = itemTransfer.VoidBy,
                        VoidTime = itemTransfer.VoidTime,
                        IsOnlineApproved = itemTransfer.IsOnlineApproved,
                        OnlineApprovedBy = itemTransfer.OnlineApprovedBy,
                        OnlineApprovedTime = itemTransfer.OnlineApprovedTime,
                        IsOnlineTransferred = itemTransfer.IsOnlineTransferred,
                        IsOnlineTransferredBy = itemTransfer.IsOnlineTransferredBy,
                        OnlineTransferredTime = itemTransfer.OnlineTransferredTime,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now,
                        DocumentRef = itemTransfer.DocumentRef,
                        OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(itemTransfer.FromStore).ToString()
                    };

                    db.ItemTransfers.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "transfer send", "Success", User.Identity.Name, $"updated transfer send {entity.ItemTransferID} ", Modules.Inventory);
                    #endregion
                }

                db.SaveChanges();
            }

            return Json(new[] { itemTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransfers_Destroy([DataSourceRequest]DataSourceRequest request, ItemTransfer itemTransfer)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemTransfer
                {
                    ItemTransferID = itemTransfer.ItemTransferID,
                    EmployeeId = itemTransfer.EmployeeId,
                    StoreRequisitionID = itemTransfer.StoreRequisitionID,
                    TransferDate = itemTransfer.TransferDate,
                    FromStore = itemTransfer.FromStore,
                    ToStore = itemTransfer.ToStore,
                    DeliveredBy = itemTransfer.DeliveredBy,
                    PlateNo = itemTransfer.PlateNo,
                    SupplierName = itemTransfer.SupplierName,
                    DriverType = itemTransfer.DriverType,
                    Status = itemTransfer.Status,
                    Remark = itemTransfer.Remark,
                    IsVoid = itemTransfer.IsVoid,
                    VoidBy = itemTransfer.VoidBy,
                    VoidTime = itemTransfer.VoidTime,
                    IsOnlineApproved = itemTransfer.IsOnlineApproved,
                    OnlineApprovedBy = itemTransfer.OnlineApprovedBy,
                    OnlineApprovedTime = itemTransfer.OnlineApprovedTime,
                    IsOnlineTransferred = itemTransfer.IsOnlineTransferred,
                    IsOnlineTransferredBy = itemTransfer.IsOnlineTransferredBy,
                    OnlineTransferredTime = itemTransfer.OnlineTransferredTime,
                    DocumentRef = itemTransfer.DocumentRef
                };

                db.ItemTransfers.Attach(entity);
                db.ItemTransfers.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted item transfer number " + itemTransfer.ItemTransferID + " " + itemTransfer.DocumentRef);
                #endregion
            }

            return Json(new[] { itemTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public ActionResult SendForApproval(int id)
        {
            var record = db.ItemTransfers.Find(id);
            var items = db.ItemTransferItems.Where(x => x.ItemTransferID == id).ToList();
          
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && items.Any() )
            {
                #region validate qty,price and tax

                foreach (var item in items)
                {
                    if (item.ShippedQuantity <= 0 || item.UPrice <= 0)
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill quantity and unit price information for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.ItemTransfers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer send", "Success", User.Identity.Name, $"send for approval transfer send {id} ", Modules.Inventory);
                #endregion
            }
            return RedirectToAction("Index");
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = db.ItemTransfers.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                transferHelper = new InventoryTransferHelper();
                string response = transferHelper.TransferSend(myid, description, User?.Identity?.Name ?? "");
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

            var record = db.ItemTransfers.Find(id);
            if (record != null && !record.IsOnlineTransferred)
            {
               _distributionHelper.UpdateTransferSendDistribution(record);
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

        [AuthorizeRoles(InventoryConstants.PropertyVoid)]
        public JsonResult Void(int id)
        {
            var record = db.ItemTransfers.Find(id);
            if ((record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ItemTransferItem.Count > 0)
                ||
                (record != null && record.IsOnlineApproved && record.IsVoid && !record.IsOnlineTransferred && record.ItemTransferItem.Count>0&&record.Status== "ReceiverVoid"))
            {
                var receive = db.ItemTransferReceive
                    .Where(x => x.IsOnlineApproved)
                    .Where(x => x.IsVoid==false)
                    .Where(x => x.ItemTransferID == record.ItemTransferID).ToList();
                if(receive.Any())
                {
                    var formattedData3 = new
                    {
                        Message = "The transfer is already received,hence can't be void.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData3, JsonRequestBehavior.AllowGet);
                }
                record.Status = Enum.GetName(typeof(OperationStatuses), 11);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                db.ItemTransfers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                //db.SaveChanges();
                #region update stock based on the issue

                foreach (ItemTransferItem i in record.ItemTransferItem)
                {
                    string itemCode = i.ItemCode;

                    InventoryItem stockItem = db.InventoryItems.Find(itemCode);
                    int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.FromStore);
                    #region stock
                    BranchItemAssignment branchItem = db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId).FirstOrDefault(x => x.ItemCode == i.ItemCode);
                    StoreItemAssignment storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.StoreCode == record.FromStore && x.ItemCode == itemCode);

                    if (storeItem != null)
                    {
                        storeItem.Balance = storeItem.Balance + i.ShippedQuantity;
                        db.Entry(storeItem).State = EntityState.Modified;
                        //db.SaveChanges();
                        #region update bin card

                        #region bin card line
                        BinCardLine binCardLine = new BinCardLine
                        {
                            StoreItemAssignment = storeItem,
                            ReferenceDoc = "Void Transfer Send " + record.ItemTransferID,
                            Reference = record.DocumentRef + " " + record.Remark,
                            Date = record.TransferDate,
                            ReceivedQTY = i.ShippedQuantity,
                            ReceivedUPrice = i.UPrice,
                            ReceivedTPrice = i.TPrice,
                            BalanceQTY = storeItem.Balance,
                            BalanceUPrice = i.UPrice,
                            BalanceTPrice = i.UPrice * storeItem.Balance,
                            //new    
                            StoreCode = record.ToStore,
                            GLPeriodId = record.GLPeriodId,
                            TransactionType = BinCardTransactionTypes.TransferSend,
                            StatusType = BinCardTransactionStatus.Void,
                            TransactionOrder = BinCardTransactionOrder.VoidTransferSend
                        };
                        db.BinCardLines.Add(binCardLine);
                        //db.SaveChanges();

                        #endregion

                        #endregion
                        var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateSigleItemBinCardTransactions(storeItem.StoreItemAssignmentID, record.TransferDate));

                    }
                    if (branchItem != null)
                    {
                        branchItem.Balance = branchItem.Balance + i.ShippedQuantity;
                        db.Entry(branchItem).State = EntityState.Modified;
                       // db.SaveChanges();

                    }
                    if (stockItem != null)
                    {
                        stockItem.Balance = stockItem.Balance + i.ShippedQuantity;
                        db.Entry(stockItem).State = EntityState.Modified;
                        //db.SaveChanges();
                    }



                    #endregion
                }

                #endregion

                #region remove unapproved transfer receives
                var receives = db.ItemTransferReceive
                    .Where(x => x.IsOnlineApproved == false)
                    .Where(x => x.ItemTransferID == record.ItemTransferID)
                    .ToList();
                db.ItemTransferReceive.RemoveRange(receives);
                #endregion
                db.SaveChanges();
                CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer send", "Success", User.Identity.Name, $"void transfer send {id} ", Modules.Inventory);
                #endregion

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.ItemTransferItem.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

         private void CopyRecord(int id)
         {
             var record = db.ItemTransfers.Find(id);
             if (record != null)
             {
                 #region header
                 var entity = new ItemTransfer
                 {
                     StoreRequisitionID = record.StoreRequisitionID,
                     EmployeeId = record.EmployeeId,
                     BranchId = record.BranchId,
                     BusinessUnitId = record.BusinessUnitId,
                     TransferDate = record.TransferDate.Date,
                     FromStore = record.FromStore,
                     ToStore = record.ToStore,
                     DeliveredBy = record.DeliveredBy,
                     PlateNo = record.PlateNo,
                     SupplierName = record.SupplierName,
                     DriverType = record.DriverType,
                     Status = Enum.GetName(typeof(OperationStatuses), 5),
                     Remark = record.Remark,
                     GLPeriodId = record.GLPeriodId,                     
                     DocumentRef = record.DocumentRef,
                     CreatedBy = User.Identity.Name,
                     DateCreated = DateTime.Now,                     
                     OrgBranchName = record.OrgBranchName
                 };

                 db.ItemTransfers.Add(entity);
                 db.SaveChanges();
                 #endregion
                 foreach (var i in record.ItemTransferItem)
                 {
                     #region lines
                     var line = new ItemTransferItem
                     {
                         ItemTransferID = entity.ItemTransferID,
                         ItemCategoryCode = i.ItemCategoryCode,
                         ItemCode = i.ItemCode,
                         PartOrCodeNumber = i.PartOrCodeNumber,
                         RequestedQuantity = i.RequestedQuantity,
                         ShippedQuantity = i.ShippedQuantity,
                         UPrice = i.UPrice,
                         TPrice = i.TPrice,
                         ReceivedQuantity = 0,
                         Remark = i.Remark,
                         CreatedBy = User.Identity.Name,
                         DateCreated = DateTime.Now
                     };

                     db.ItemTransferItems.Add(line);
                     db.SaveChanges();

                     #endregion

                 }

             }

         }
        private void RequestedItem(ItemTransfer record)
        {
            StoreRequisition storeRequisition = db.StoreRequisitions.Find(record.StoreRequisitionID);

            if (storeRequisition != null)
                foreach (var requestedItem in storeRequisition.StoreRequisitionItem)
                {
                    decimal avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(requestedItem.ItemCode, storeRequisition.StoreCode);

                    var transferItem = new ItemTransferItem
                    {
                        ItemTransferID = record.ItemTransferID,
                        ItemCategoryCode = requestedItem.ItemCategoryCode,
                        ItemCode = requestedItem.ItemCode,
                        RequestedQuantity = requestedItem.ApprovedQuantity,
                        UPrice = avgPrice,
                        PartOrCodeNumber = requestedItem.PartNo,
                        TPrice = requestedItem.ApprovedQuantity * avgPrice,
                        ShippedQuantity = requestedItem.IssuedQuantity,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    //should get per store price

                    db.ItemTransferItems.Add(transferItem);
                    db.SaveChanges();
                }
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

       
    }
}
