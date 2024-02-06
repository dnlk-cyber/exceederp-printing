using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation
{

   // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class ProductionStoreReturnController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // private readonly DistributionHelper _distributionHelper = new DistributionHelper();
        // private InventoryTransferHelper transferHelper;

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var stores = db.InventoryStores;
            var branches = db.Branch;
            var costCenter = db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;


            ViewData["costCenters"] = selectedcostCenter;
            ViewData["stores"] = stores;
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

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult StoreReturns_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //IQueryable<StoreReturn> storereturns = db.StoreReturns.Where(x => mybraches.Contains(x.OrgBranchName));

            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<StoreReturn> storereturns = db.StoreReturns.Where(q=>q.JobOrderId==id.ToString());

            DataSourceResult result = storereturns.ToDataSourceResult(request, storeReturn => new
            {
                StoreReturnID = storeReturn.StoreReturnID,
                Reference = storeReturn.Reference,
                StoreCode = storeReturn.StoreCode,
                JobOrderId = storeReturn.JobOrderId,
                IssueNumber = storeReturn.IssueNumber,
                ReturnDate = storeReturn.ReturnDate,
                Reason = storeReturn.Reason,
                Status = storeReturn.Status,
                IsForOther = storeReturn.IsForOther,
                OrgStructureId = storeReturn.OrgStructureId,
                ForBranchId = storeReturn.ForBranchId,
                ConfirmationLetter = storeReturn.ConfirmationLetter,
                Remark = storeReturn.Remark,
                GLPeriodId = storeReturn.GLPeriodId,
                IsVoid = storeReturn.IsVoid,
                VoidBy = storeReturn.VoidBy,
                VoidTime = storeReturn.VoidTime,
                IsOnlineApproved = storeReturn.IsOnlineApproved,
                OnlineApprovedBy = storeReturn.OnlineApprovedBy,
                OnlineApprovedTime = storeReturn.OnlineApprovedTime,
                IsOnlineTransferred = storeReturn.IsOnlineTransferred,
                IsOnlineTransferredBy = storeReturn.IsOnlineTransferredBy,
                OnlineTransferredTime = storeReturn.OnlineTransferredTime,
                IsSendForApproval = storeReturn.IsSendForApproval,
                SendForApprovalBy = storeReturn.SendForApprovalBy,
                SendForApprovalTime = storeReturn.SendForApprovalTime,
                DateCreated = storeReturn.DateCreated,
                CreatedBy = storeReturn.CreatedBy,
            });

            return Json(result,JsonRequestBehavior.AllowGet);
        }

       // [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturns_Create([DataSourceRequest]DataSourceRequest request, StoreReturn storeReturn,int id)
        {

            if (storeReturn.GLPeriodId != 0)
            {
                var inventoryPeriod = db.InventoryPeriods.Find(storeReturn.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    if (
                        !((storeReturn.ReturnDate.Date >= inventoryPeriod.DateFrom.Date) &&
                          (storeReturn.ReturnDate.Date <= inventoryPeriod.DateTo.Date)))
                        ModelState.AddModelError("",
                            "Return Date should be within the period: " + inventoryPeriod.Name + "(From " +
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
            if (storeReturn.ReturnDate > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }
            if (ModelState.IsValid)
            {
                var entity = new StoreReturn
                {
                    Reference = storeReturn.Reference,
                    JobOrderId = id.ToString(),
                    IssueNumber = storeReturn.IssueNumber,
                    StoreCode = storeReturn.StoreCode,
                    GLPeriodId = storeReturn.GLPeriodId,
                    ReturnDate = storeReturn.ReturnDate.Date,
                    Reason = storeReturn.Reason,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    IsForOther = storeReturn.IsForOther,
                    OrgStructureId = storeReturn.OrgStructureId,
                    ForBranchId = storeReturn.ForBranchId,
                    ConfirmationLetter = storeReturn.ConfirmationLetter,
                    Remark = storeReturn.Remark,
                    IsVoid = storeReturn.IsVoid,
                    VoidBy = storeReturn.VoidBy,
                    VoidTime = storeReturn.VoidTime,
                    IsOnlineApproved = storeReturn.IsOnlineApproved,
                    OnlineApprovedBy = storeReturn.OnlineApprovedBy,
                    OnlineApprovedTime = storeReturn.OnlineApprovedTime,
                    IsOnlineTransferred = storeReturn.IsOnlineTransferred,
                    IsOnlineTransferredBy = storeReturn.IsOnlineTransferredBy,
                    OnlineTransferredTime = storeReturn.OnlineTransferredTime,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeReturn.StoreCode).ToString()
                };

                db.StoreReturns.Add(entity);
                db.SaveChanges();
                storeReturn.StoreReturnID = entity.StoreReturnID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store return", "Success", User.Identity.Name, $"created store return {entity.StoreReturnID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeReturn }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturns_Update([DataSourceRequest]DataSourceRequest request, StoreReturn storeReturn)
        {

            if (storeReturn.GLPeriodId != 0)
            {
                var inventoryPeriod = db.InventoryPeriods.Find(storeReturn.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    if (
                        !((storeReturn.ReturnDate.Date >= inventoryPeriod.DateFrom.Date) &&
                          (storeReturn.ReturnDate.Date <= inventoryPeriod.DateTo.Date)))
                        ModelState.AddModelError("",
                            "Return Date should be within the period: " + inventoryPeriod.Name + "(From " +
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
            if (storeReturn.ReturnDate > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }
            if (ModelState.IsValid)
            {
                var entity = new StoreReturn
                {
                    StoreReturnID = storeReturn.StoreReturnID,
                    Reference = storeReturn.Reference,
                    IssueNumber = storeReturn.IssueNumber,
                    JobOrderId = storeReturn.JobOrderId,
                    StoreCode = storeReturn.StoreCode,
                    GLPeriodId = storeReturn.GLPeriodId,
                    ReturnDate = storeReturn.ReturnDate,
                    Reason = storeReturn.Reason,
                    Status = storeReturn.Status,
                    IsForOther = storeReturn.IsForOther,
                    OrgStructureId = storeReturn.OrgStructureId,
                    ForBranchId = storeReturn.ForBranchId,
                    ConfirmationLetter = storeReturn.ConfirmationLetter,
                    Remark = storeReturn.Remark,
                    IsVoid = storeReturn.IsVoid,
                    VoidBy = storeReturn.VoidBy,
                    VoidTime = storeReturn.VoidTime,
                    IsOnlineApproved = storeReturn.IsOnlineApproved,
                    OnlineApprovedBy = storeReturn.OnlineApprovedBy,
                    OnlineApprovedTime = storeReturn.OnlineApprovedTime,
                    IsOnlineTransferred = storeReturn.IsOnlineTransferred,
                    IsOnlineTransferredBy = storeReturn.IsOnlineTransferredBy,
                    OnlineTransferredTime = storeReturn.OnlineTransferredTime,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                db.StoreReturns.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store return", "Success", User.Identity.Name, $"updated store return {entity.StoreReturnID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeReturn }.ToDataSourceResult(request, ModelState));
        }

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturns_Destroy([DataSourceRequest]DataSourceRequest request, StoreReturn storeReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreReturn
                {
                    StoreReturnID = storeReturn.StoreReturnID,
                    Reference = storeReturn.Reference,
                    JobOrderId = storeReturn.JobOrderId,
                    IssueNumber = storeReturn.IssueNumber,
                    StoreCode = storeReturn.StoreCode,
                    ReturnDate = storeReturn.ReturnDate,
                    Reason = storeReturn.Reason,
                    Status = storeReturn.Status,
                    IsForOther = storeReturn.IsForOther,
                    OrgStructureId = storeReturn.OrgStructureId,
                    ForBranchId = storeReturn.ForBranchId,
                    ConfirmationLetter = storeReturn.ConfirmationLetter,
                    Remark = storeReturn.Remark,
                    IsVoid = storeReturn.IsVoid,
                    VoidBy = storeReturn.VoidBy,
                    VoidTime = storeReturn.VoidTime,
                    IsOnlineApproved = storeReturn.IsOnlineApproved,
                    OnlineApprovedBy = storeReturn.OnlineApprovedBy,
                    OnlineApprovedTime = storeReturn.OnlineApprovedTime,
                    IsOnlineTransferred = storeReturn.IsOnlineTransferred,
                    IsOnlineTransferredBy = storeReturn.IsOnlineTransferredBy,
                    OnlineTransferredTime = storeReturn.OnlineTransferredTime,
                };

                db.StoreReturns.Attach(entity);
                db.StoreReturns.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted store return " + storeReturn.StoreReturnID);
                #endregion
            }

            return Json(new[] { storeReturn }.ToDataSourceResult(request, ModelState));
        }


        //[AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        //public JsonResult TransferDistribution(string id, string description)
        //{
        //    int myid = int.Parse(id);
        //    var record = db.StoreReturns.Find(myid);

        //    if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
        //    {
        //        transferHelper = new InventoryTransferHelper();
        //        string response = transferHelper.StoreReturn(myid, description, User.Identity.Name);
        //        if (response != "SUCCESS")
        //        {
        //            var formattedData = new
        //            {
        //                Message = response,
        //                Status = "NOT_OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var formattedData = new
        //            {
        //                Message = "Successfuly Transferred.",
        //                Status = "NOT_OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }


        //    }
        //    if (record != null && !record.IsOnlineApproved)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record cannot be transfered before validation.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    if (record != null && record.IsOnlineTransferred)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "You cannot transfer this transaction, it is already transfered.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    return null;
        //}

        //[AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult GenerateDistribution(int id)
        {

            var record = db.StoreReturns.Find(id);
            if (record != null && !record.IsOnlineTransferred)
            {
              //  _distributionHelper.UpdateStoreReturnDistribution(record);
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

        public JsonResult SendForApproval(int id)
        {
            var record = db.StoreReturns.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.StoreReturnItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.StoreReturns.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store return", "Success", User.Identity.Name, $"send for approval store return {id} ", Modules.Inventory);
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

            else if (record.StoreReturnItems.Count == 0)
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

        [AuthorizeRoles(InventoryConstants.PropertyVoid)]
        public JsonResult Void(int id)
        {
            var record = db.StoreReturns.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.StoreReturnItems.Count > 0)
            {
                bool valid = true;
                #region validate

                foreach (var i in record.StoreReturnItems)
                {
                    #region stock

                    var storeItem = db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode).FirstOrDefault(x => x.StoreCode == record.StoreCode);
                    if (storeItem == null)
                    {
                        valid = false;
                        string message = "Item: " + i.InventoryItem.Name + " is not available in store: " + record.StoreCode;
                        ModelState.AddModelError("store", message);

                        var formattedData2 = new
                        {
                            Message = message,
                            Status = "OK"
                        };
                        return Json(formattedData2, JsonRequestBehavior.AllowGet);

                    }
                    if (storeItem.Balance - i.Quantity < 0)
                    {
                        valid = false;
                        string message = "There is no balance in system to void for item " + i.ItemCode + "-" + i.InventoryItem.Name;
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
                    record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                    record.IsVoid = true;
                    record.VoidBy = User.Identity.Name;
                    record.VoidTime = DateTime.Now;
                    db.StoreReturns.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    //db.SaveChanges();

                    #region reverse

                    foreach (var i in record.StoreReturnItems)
                    {
                        #region Store Item Balance

                        var storeItem =
                            db.StoreItemAssignments.FirstOrDefault(
                                x => x.StoreCode == record.StoreCode && x.ItemCode == i.ItemCode);
                        if (storeItem != null)
                        {
                            storeItem.Balance = storeItem.Balance - i.Quantity;
                            db.StoreItemAssignments.Attach(storeItem);
                            db.Entry(storeItem).State = EntityState.Modified;
                            //db.SaveChanges();

                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = "Void Return_" + record.StoreReturnID,
                                Date = record.ReturnDate,
                                IssuedQTY = i.Quantity,
                                IssuedUPrice = i.UnitPrice,
                                IssuedTPrice = i.UnitPrice * i.Quantity,
                                BalanceQTY = storeItem.Balance,
                                BalanceUPrice = i.UnitPrice,
                                BalanceTPrice = i.UnitPrice * storeItem.Balance,

                                //new    
                                StoreCode = record.StoreCode,
                                GLPeriodId = record.GLPeriodId,
                                TransactionType = BinCardTransactionTypes.Return,
                                StatusType = BinCardTransactionStatus.Void,
                            };

                            db.BinCardLines.Add(binCardLine);
                            // db.SaveChanges();

                            #endregion
                        }


                        #endregion

                        #region  branch Balance

                        var storeBranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode);
                        var branchItemAssignment =
                            db.BranchItemAssignments.Where(x => x.BranchId == storeBranch)
                                .FirstOrDefault(x => x.ItemCode == i.ItemCode);
                        if (branchItemAssignment != null)
                        {
                            branchItemAssignment.Balance = branchItemAssignment.Balance - i.Quantity;
                            db.Entry(branchItemAssignment).State = EntityState.Modified;
                            //db.SaveChanges();
                        }

                        #endregion

                        #region  Item Balance

                        var inventoryItem = db.InventoryItems.Find(i.ItemCode);
                        if (inventoryItem != null)
                        {
                            inventoryItem.Balance = inventoryItem.Balance - i.Quantity;
                            db.Entry(inventoryItem).State = EntityState.Modified;
                        }

                        #endregion
                    }

                    db.SaveChanges();
                    #endregion

                    CopyRecord(id);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store return", "Success", User.Identity.Name, $"void store return {id} ", Modules.Inventory);
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

            if (record.StoreReturnItems.Count == 0)
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
            var record = db.StoreReturns.Find(id);
            if (record != null)
            {
                #region header
                var entity = new StoreReturn
                {
                    Reference = record.Reference,
                    StoreCode = record.StoreCode,
                    ReturnDate = record.ReturnDate,
                    Reason = record.Reason,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = record.Remark,
                    //IsVoid = record.IsVoid,
                    //VoidBy = record.VoidBy,
                    //VoidTime = record.VoidTime,
                    //IsOnlineApproved = record.IsOnlineApproved,
                    //OnlineApprovedBy = record.OnlineApprovedBy,
                    //OnlineApprovedTime = record.OnlineApprovedTime,
                    //IsOnlineTransferred = record.IsOnlineTransferred,
                    //IsOnlineTransferredBy = record.IsOnlineTransferredBy,
                    //OnlineTransferredTime = record.OnlineTransferredTime,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = record.OrgBranchName
                };

                db.StoreReturns.Add(entity);
                db.SaveChanges();
                #endregion
                foreach (var i in record.StoreReturnItems)
                {
                    #region lines
                    var line = new StoreReturnItems
                    {
                        StoreReturnID = entity.StoreReturnID,
                        ItemCategoryCode = i.ItemCategoryCode,
                        ItemCode = i.ItemCode,
                        UnitPrice = i.UnitPrice,
                        Quantity = i.Quantity,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                    };

                    db.StoreReturnItemss.Add(line);
                    db.SaveChanges();

                    #endregion

                }

            }

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
