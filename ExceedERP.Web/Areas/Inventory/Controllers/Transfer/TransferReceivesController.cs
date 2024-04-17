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

    public class TransferReceivesController : Controller
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
        public ActionResult ItemTransferReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //List<string> myStores = db.InventoryStores.Where(x => mybraches.Contains(x.BranchId.ToString())).Select(x => x.StoreCode).ToList();
            //IQueryable<ItemTransfer> itemtransferreceives = db.ItemTransfers.Where(st => st.IsOnlineApproved && !st.IsVoid).Where(x => myStores.Contains(x.ToStore));

            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<ItemTransferReceive> itemtransferreceives = db.ItemTransferReceive.Where(x => userStores.Contains(x.ToStore));

            DataSourceResult result = itemtransferreceives.ToDataSourceResult(request, itemTransferReceive => new
            {

                ItemTransferReceiveId = itemTransferReceive.ItemTransferReceiveId,
                ItemTransferID = itemTransferReceive.ItemTransferID,
                DocumentRef = itemTransferReceive.DocumentRef,
                ReceiveDate = itemTransferReceive.ReceiveDate,
                FromStore = itemTransferReceive.FromStore,
                ToStore = itemTransferReceive.ToStore,
                DeliveredBy = itemTransferReceive.DeliveredBy,
                PlateNo = itemTransferReceive.PlateNo,
                SupplierName = itemTransferReceive.SupplierName,
                DriverType = itemTransferReceive.DriverType,
                Status = itemTransferReceive.Status,
                Remark = itemTransferReceive.Remark,
                GLPeriodId = itemTransferReceive.GLPeriodId,
                BranchId = itemTransferReceive.BranchId,
                BusinessUnitId = itemTransferReceive.BusinessUnitId,
                IsVoid = itemTransferReceive.IsVoid,
                VoidBy = itemTransferReceive.VoidBy,
                VoidTime = itemTransferReceive.VoidTime,
                IsOnlineApproved = itemTransferReceive.IsOnlineApproved,
                OnlineApprovedBy = itemTransferReceive.OnlineApprovedBy,
                OnlineApprovedTime = itemTransferReceive.OnlineApprovedTime,
                IsSendForApproval = itemTransferReceive.IsSendForApproval,
                SendForApprovalBy = itemTransferReceive.SendForApprovalBy,
                SendForApprovalTime = itemTransferReceive.SendForApprovalTime,
                IsOnlineTransferred = itemTransferReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = itemTransferReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = itemTransferReceive.OnlineTransferredTime,


                DateCreated = itemTransferReceive.DateCreated,
                LastModified = itemTransferReceive.LastModified,
                CreatedBy = itemTransferReceive.CreatedBy,
                ModifiedBy = itemTransferReceive.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferReceives_Create([DataSourceRequest]DataSourceRequest request, ItemTransferReceive itemTransferReceive)
        {
            #region validations

            var transfer = db.ItemTransfers.Find(itemTransferReceive.ItemTransferID);
            itemTransferReceive.BusinessUnitId = transfer.BusinessUnitId;
            if (itemTransferReceive.GLPeriodId != 0)
            {
                var inventoryPeriod = db.InventoryPeriods.Find(itemTransferReceive.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    var openPeriod = db.InventoryStorePeriodStatuses
                        .Where(x => x.InventoryPeriodId == itemTransferReceive.GLPeriodId).FirstOrDefault(x => x.StoreCode == itemTransferReceive.ToStore);

                    if (openPeriod != null && openPeriod.Status != FiscalYearStatus.Open)
                        ModelState.AddModelError("open", "The selected period is not open for this store.");

                    if (!((itemTransferReceive.ReceiveDate.Date >= inventoryPeriod.DateFrom) &&
                          (itemTransferReceive.ReceiveDate.Date <= inventoryPeriod.DateTo)))
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
            if (itemTransferReceive.ReceiveDate > DateTime.Now)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (!String.IsNullOrEmpty(itemTransferReceive.DocumentRef))
            {
                var exists = db.ItemTransferReceive.Where(x => x.DocumentRef == itemTransferReceive.DocumentRef && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", itemTransferReceive.DocumentRef + " already exists in the system.");
                }
            }

            #endregion
            ModelState.Remove("BranchId");
            if (ModelState.IsValid)
            {
                if (transfer != null)
                {
                    var entity = new ItemTransferReceive
                    {
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(transfer.ToStore),
                        BusinessUnitId=itemTransferReceive.BusinessUnitId,
                        ItemTransferID = itemTransferReceive.ItemTransferID,
                        ReceiveDate = itemTransferReceive.ReceiveDate.Date,
                        FromStore = transfer.FromStore,
                        ToStore = transfer.ToStore,
                        DeliveredBy = itemTransferReceive.DeliveredBy,
                        PlateNo = itemTransferReceive.PlateNo,
                        SupplierName = itemTransferReceive.SupplierName,
                        DriverType = itemTransferReceive.DriverType,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = itemTransferReceive.Remark,
                        GLPeriodId = itemTransferReceive.GLPeriodId,
                        IsVoid = itemTransferReceive.IsVoid,
                        VoidBy = itemTransferReceive.VoidBy,
                        VoidTime = itemTransferReceive.VoidTime,
                        IsOnlineApproved = itemTransferReceive.IsOnlineApproved,
                        OnlineApprovedBy = itemTransferReceive.OnlineApprovedBy,
                        OnlineApprovedTime = itemTransferReceive.OnlineApprovedTime,
                        IsOnlineTransferred = itemTransferReceive.IsOnlineTransferred,
                        IsOnlineTransferredBy = itemTransferReceive.IsOnlineTransferredBy,
                        OnlineTransferredTime = itemTransferReceive.OnlineTransferredTime,
                        DocumentRef = itemTransferReceive.DocumentRef,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(transfer.ToStore).ToString()
                    };

                    db.ItemTransferReceive.Add(entity);
                    db.SaveChanges();
                    itemTransferReceive.ItemTransferReceiveId = entity.ItemTransferReceiveId;
                    TransferredItem(entity);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "transfer receive", "Success", User.Identity.Name, $"created transfer receive {entity.ItemTransferReceiveId} ", Modules.Inventory);
                    #endregion

                }
            }

            return Json(new[] { itemTransferReceive }.ToDataSourceResult(request, ModelState));
        }
        
        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferReceives_Update([DataSourceRequest]DataSourceRequest request, ItemTransferReceive itemTransferReceive)
        {
            #region validations

            var transfer = db.ItemTransfers.Find(itemTransferReceive.ItemTransferID);
            itemTransferReceive.BusinessUnitId = transfer.BusinessUnitId;

            if (itemTransferReceive.GLPeriodId != 0)
            {
                var inventoryPeriod = db.InventoryPeriods.Find(itemTransferReceive.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    var openPeriod = db.InventoryStorePeriodStatuses
                        .Where(x => x.InventoryPeriodId == itemTransferReceive.GLPeriodId).FirstOrDefault(x => x.StoreCode == itemTransferReceive.ToStore);

                    if (openPeriod != null && openPeriod.Status != FiscalYearStatus.Open)
                        ModelState.AddModelError("open", "The selected period is not open for this store.");

                    if (!((itemTransferReceive.ReceiveDate.Date >= inventoryPeriod.DateFrom) &&
                          (itemTransferReceive.ReceiveDate.Date <= inventoryPeriod.DateTo)))
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
            if (itemTransferReceive.ReceiveDate > DateTime.Now)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (!String.IsNullOrEmpty(itemTransferReceive.DocumentRef))
            {
                var exists = db.ItemTransferReceive.Where(x=>x.ItemTransferReceiveId!=itemTransferReceive.ItemTransferReceiveId).Where(x => x.DocumentRef == itemTransferReceive.DocumentRef && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", itemTransferReceive.DocumentRef + " already exists in the system.");
                }
            }

            #endregion
            ModelState.Remove("BranchId");
            if (ModelState.IsValid)
            {
                if (transfer != null)
                {
                    var entity = new ItemTransferReceive
                    {
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(transfer.ToStore),
                        BusinessUnitId=itemTransferReceive.BusinessUnitId,
                        ItemTransferReceiveId = itemTransferReceive.ItemTransferReceiveId,
                        ItemTransferID = itemTransferReceive.ItemTransferID,
                        DocumentRef = itemTransferReceive.DocumentRef,
                        ReceiveDate = itemTransferReceive.ReceiveDate,
                        FromStore = itemTransferReceive.FromStore,
                        ToStore = itemTransferReceive.ToStore,
                        DeliveredBy = itemTransferReceive.DeliveredBy,
                        PlateNo = itemTransferReceive.PlateNo,
                        SupplierName = itemTransferReceive.SupplierName,
                        DriverType = itemTransferReceive.DriverType,
                        Status = itemTransferReceive.Status,
                        Remark = itemTransferReceive.Remark,
                        GLPeriodId = itemTransferReceive.GLPeriodId,
                        IsVoid = itemTransferReceive.IsVoid,
                        VoidBy = itemTransferReceive.VoidBy,
                        VoidTime = itemTransferReceive.VoidTime,
                        IsOnlineApproved = itemTransferReceive.IsOnlineApproved,
                        OnlineApprovedBy = itemTransferReceive.OnlineApprovedBy,
                        OnlineApprovedTime = itemTransferReceive.OnlineApprovedTime,
                        IsOnlineTransferred = itemTransferReceive.IsOnlineTransferred,
                        IsOnlineTransferredBy = itemTransferReceive.IsOnlineTransferredBy,
                        OnlineTransferredTime = itemTransferReceive.OnlineTransferredTime,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now,

                        
                };

                    db.ItemTransferReceive.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "transfer receive", "Success", User.Identity.Name, $"updated transfer receive {entity.ItemTransferReceiveId} ", Modules.Inventory);
                    #endregion
                }

                db.SaveChanges();

                //_distributionHelper.UpdateTransferSendDistribution(entity);
            }

            return Json(new[] { itemTransferReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferReceives_Destroy([DataSourceRequest]DataSourceRequest request, ItemTransferReceive itemTransfer)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferReceive
                {
                    ItemTransferID = itemTransfer.ItemTransferID,
                    ItemTransferReceiveId = itemTransfer.ItemTransferReceiveId,
                    ReceiveDate = itemTransfer.ReceiveDate,
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

                db.ItemTransferReceive.Attach(entity);
                db.ItemTransferReceive.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted item transfer receive number " + itemTransfer.ItemTransferReceiveId + " " + itemTransfer.DocumentRef);
                #endregion
            }

            return Json(new[] { itemTransfer }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.ItemTransferReceive.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ItemTransferItemReceive.Count > 0)
            {
                #region validate qty,price and tax

                foreach (var item in record.ItemTransferItemReceive)
                {
                    if (item.ReceivedQuantity <= 0 || item.UPrice <= 0)
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
                db.ItemTransferReceive.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer receive", "Success", User.Identity.Name, $"send for approval transfer receive {id} ", Modules.Inventory);
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

            else if (record.ItemTransferItemReceive.Count == 0)
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
            var record = db.ItemTransferReceive.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                transferHelper = new InventoryTransferHelper();
                string response = transferHelper.TransferReceive(myid, description, User?.Identity?.Name ?? "");
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

            var record = db.ItemTransferReceive.Find(id);
            if (record != null && !record.IsOnlineTransferred)
            {
                _distributionHelper.UpdateTransferReceiveDistribution(record);
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
            var record = db.ItemTransferReceive.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ItemTransferItemReceive.Count > 0)
            {
                bool valid = true;
                #region validate

                foreach (var i in record.ItemTransferItemReceive)
                {
                    #region stock

                    var storeItem = db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode).FirstOrDefault(x => x.StoreCode == record.ToStore);
                    if (storeItem == null)
                    {
                        valid = false;
                        string message = "Item: " + i.InventoryItem.Name + " is not available in store: " + record.ToStore;
                        ModelState.AddModelError("store", message);

                        var formattedData2 = new
                        {
                            Message = message,
                            Status = "OK"
                        };
                        return Json(formattedData2, JsonRequestBehavior.AllowGet);

                    }
                    if (storeItem.Balance - i.ReceivedQuantity < 0)
                    {
                        valid = false;
                        string message ="There is no balance in system to void for item " + i.ItemCode +"-"+i.InventoryItem.Name;
                        var formattedData2 = new
                        {
                            Message = message,
                            Status = "OK"
                        };
                        return Json(formattedData2, JsonRequestBehavior.AllowGet);
                    }

                    #endregion
                    
                }
                #endregion

                if (valid)
                {
                    record.Status = Enum.GetName(typeof(OperationStatuses), 12);
                    record.IsVoid = true;
                    record.VoidBy = User.Identity.Name;
                    record.VoidTime = DateTime.Now;
                    db.ItemTransferReceive.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    //db.SaveChanges();

                    #region update stock based on the receive


                    foreach (var i in record.ItemTransferItemReceive)
                    {
                        string itemCode = i.ItemCode;

                        InventoryItem stockItem = db.InventoryItems.Find(itemCode);
                        int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.ToStore);

                        #region stock

                        BranchItemAssignment branchItem =
                            db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
                                .FirstOrDefault(x => x.ItemCode == i.ItemCode);
                        StoreItemAssignment storeItem =
                            db.StoreItemAssignments.FirstOrDefault(
                                x => x.StoreCode == record.ToStore && x.ItemCode == itemCode);

                        if (storeItem != null)
                        {
                            storeItem.Balance = storeItem.Balance - i.ReceivedQuantity;
                            db.Entry(storeItem).State = EntityState.Modified;
                            //db.SaveChanges();

                            #region update bin card

                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = "Void Transfer Receive " + record.ItemTransferReceiveId,
                                Reference = record.DocumentRef + " " + record.Remark,
                                Date = record.ReceiveDate,
                                IssuedQTY = i.ReceivedQuantity,
                                IssuedUPrice = i.UPrice,
                                IssuedTPrice = i.TPrice,
                                BalanceQTY = storeItem.Balance,
                                BalanceUPrice = i.UPrice,
                                BalanceTPrice = i.UPrice * storeItem.Balance,
                                //new    
                                StoreCode = record.FromStore,
                                GLPeriodId = record.GLPeriodId,
                                TransactionType = BinCardTransactionTypes.TransferReceive,
                                StatusType = BinCardTransactionStatus.Void,
                                TransactionOrder = BinCardTransactionOrder.VoidTransferReceive
                            };
                            db.BinCardLines.Add(binCardLine);
                            //db.SaveChanges();

                            #endregion

                            #endregion
                            var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateSigleItemBinCardTransactions(storeItem.StoreItemAssignmentID, record.ReceiveDate));

                        }
                        if (branchItem != null)
                        {
                            branchItem.Balance = branchItem.Balance - i.ShippedQuantity;
                            db.Entry(branchItem).State = EntityState.Modified;
                            //db.SaveChanges();

                        }
                        if (stockItem != null)
                        {
                            stockItem.Balance = stockItem.Balance - i.ShippedQuantity;
                            db.Entry(stockItem).State = EntityState.Modified;
                            //db.SaveChanges();
                        }

                        #endregion
                    }

                    db.SaveChanges();
                    #endregion

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "transfer receive", "Success", User.Identity.Name, $"void transfer receive {id} ", Modules.Inventory);
                    #endregion

                    var formattedData = new
                    {
                        Message = "Void Successful.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
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

            if (record.ItemTransferItemReceive.Count == 0)
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

        private void TransferredItem(ItemTransferReceive record)
        {
            var itemTransfer = db.ItemTransfers.Find(record.ItemTransferID);
            var items = db.ItemTransferItems.Where(x => x.ItemTransferID == record.ItemTransferID).ToList();

            if (itemTransfer != null)
                foreach (var sentItem in items)
                {
                    decimal totalQty = sentItem.ShippedQuantity;
                    decimal receivedQty = 0;
                    decimal remainingQty = 0;

                    var alreadyReceivedQty = db.ItemTransferItemReceive
                        .Where(x => x.ItemTransferReceive.IsOnlineApproved)
                         .Where(x => x.ItemTransferReceive.IsVoid==false)
                        .Where(x => x.ItemCode == sentItem.ItemCode)
                        .Where(x => x.ItemTransferReceive.ItemTransferID == record.ItemTransferID).ToList();
                    if (alreadyReceivedQty != null)
                        receivedQty = alreadyReceivedQty.Sum(x => x.ReceivedQuantity);
                    remainingQty = totalQty - receivedQty;

                    if (remainingQty>0)
                    {
                        var itemReceive = new ItemTransferItemReceive
                        {
                            ItemTransferReceiveId = record.ItemTransferReceiveId,
                            ItemCategoryCode = sentItem.ItemCategoryCode,
                            ItemCode = sentItem.ItemCode,
                            RequestedQuantity = sentItem.RequestedQuantity,
                            UPrice = sentItem.UPrice,
                            PartOrCodeNumber = sentItem.PartOrCodeNumber,
                            ShippedQuantity = sentItem.ShippedQuantity,
                            TPrice = sentItem.ShippedQuantity * sentItem.UPrice,
                            ReceivedQuantity = remainingQty,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name
                        };

                        db.ItemTransferItemReceive.Add(itemReceive);
                        db.SaveChanges();
                    }
                }
        }

    }
}
