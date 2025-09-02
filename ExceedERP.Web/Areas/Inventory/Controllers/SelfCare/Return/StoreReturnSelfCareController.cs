using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.SelfCare.Return
{

    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class StoreReturnSelfCareController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            //var categories = db.ItemCategorys;
            //var items = db.InventoryItems;
            //var stores = db.InventoryStores;
            //var branches = db.Branch;
            //var costCenter = db.BranchCostCenter;
            //var selectedcostCenter = costCenter.Select(
            //  i => new
            //  {
            //      Name = i.MainCompanyStructure.Name,
            //      Code = i.OrgStructureId
            //  }).AsQueryable();

            //var inventoryPeriods = db.InventoryPeriods;
            //var selectedinventoryPeriods = inventoryPeriods.Select(
            // i => new
            // {
            //     Name = i.Name,
            //     GLPeriodId = i.InventoryPeriodId
            // }).AsQueryable();
            ViewData["inventoryPeriods"] = CacheData.GetInventoryPeriodCaches().Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.Id
                }).AsQueryable();



            ViewData["costCenters"] = CacheData.GetWorkUnitCaches().Select(
                i => new
                {
                    Name = i.Name,
                    Code = i.Id
                }).AsQueryable();
            ViewData["stores"] = CacheData.GetStoreCaches().Select(
                i => new
                {
                    Name = i.Name,
                    StoreCode = i.Id
                }).AsQueryable();
            ViewData["Branches"] = CacheData.GetBranchCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BranchId = i.Id
               }).AsQueryable();
            //var selecteditems = items.Select(
            //    i => new
            //    {
            //        Name = i.ItemCode + "_" + i.Name,
            //        ItemCode = i.ItemCode
            //    }).ToList();
            ViewData["Items"] = CacheData.GetInventoryItemCaches().Select(
              i => new
              {
                  Name = i.Id + "_" + i.Name,
                  ItemCode = i.Id
              }).ToList();
            ViewData["Category"] = CacheData.GetInventoryCategoryCaches().Select(
                i => new
                {
                    Name = i.Id + "_" + i.Name,
                    ItemCategoryCode = i.Id
                }).ToList();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult StoreReturns_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<StoreReturn> storereturns = db.StoreReturns.Where(x => userStores.Contains(x.StoreCode));

            if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService)
                 && System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HRDepartmentHead))
            {

                storereturns = storereturns.Where(x => userworkunit.Contains(x.CreatedAtOrgStructureId)
                //|| x.CreatedAtOrgStructureId == 0//if they are not vis
                || x.ForwardedTo == User.Identity.Name
                || x.ForwardedFrom == User.Identity.Name
                || x.CreatedBy == User.Identity.Name);

            }
            else if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService))
            {
                storereturns = storereturns.Where(x => x.CreatedBy == User.Identity.Name);
            }
            DataSourceResult result = storereturns.ToDataSourceResult(request, storeReturn => new
            {
                StoreReturnID = storeReturn.StoreReturnID,
                BranchId = storeReturn.BranchId,
                BusinessUnitId = storeReturn.BusinessUnitId,
                CreatedAtOrgStructureId = storeReturn.CreatedAtOrgStructureId,
                Reference = storeReturn.Reference,
                StoreCode = storeReturn.StoreCode,
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

                IsDepartmentOnlineApproved = storeReturn.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = storeReturn.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = storeReturn.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = storeReturn.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = storeReturn.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = storeReturn.DepartmentSendForApprovalTime,

                ForwardedFrom = storeReturn.ForwardedFrom,
                ForwardedTo = storeReturn.ForwardedTo,
                IsFinalApproval = storeReturn.IsFinalApproval,
                CurrentApprovalStep = storeReturn.CurrentApprovalStep,

                DateCreated = storeReturn.DateCreated,
                CreatedBy = storeReturn.CreatedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturns_Create([DataSourceRequest]DataSourceRequest request, StoreReturn storeReturn)
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
            storeReturn.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);
            if (ModelState.IsValid)
            {
                var entity = new StoreReturn
                {
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeReturn.StoreCode),
                    BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                    Reference = storeReturn.Reference,
                    StoreCode = storeReturn.StoreCode,
                    GLPeriodId = storeReturn.GLPeriodId,
                    ReturnDate = storeReturn.ReturnDate.Date,
                    Reason = storeReturn.Reason,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    IsForOther = storeReturn.IsForOther,
                    OrgStructureId = storeReturn.OrgStructureId,
                    CreatedAtOrgStructureId = storeReturn.CreatedAtOrgStructureId,
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

                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = storeReturn.IsFinalApproval,
                    CurrentApprovalStep = storeReturn.CurrentApprovalStep,

                    IsDepartmentOnlineApproved = storeReturn.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = storeReturn.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = storeReturn.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = storeReturn.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = storeReturn.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = storeReturn.DepartmentSendForApprovalTime,

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
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeReturn.StoreCode),
                    BusinessUnitId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBusinessUnit(storeReturn.StoreCode),
                    Reference = storeReturn.Reference,
                    StoreCode = storeReturn.StoreCode,
                    GLPeriodId = storeReturn.GLPeriodId,
                    ReturnDate = storeReturn.ReturnDate,
                    Reason = storeReturn.Reason,
                    Status = storeReturn.Status,
                    IsForOther = storeReturn.IsForOther,
                    OrgStructureId = storeReturn.OrgStructureId,
                    CreatedAtOrgStructureId = storeReturn.CreatedAtOrgStructureId,
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

                    ForwardedFrom = storeReturn.ForwardedFrom,
                    ForwardedTo = storeReturn.ForwardedTo,
                    IsFinalApproval = storeReturn.IsFinalApproval,
                    CurrentApprovalStep = storeReturn.CurrentApprovalStep,

                    IsDepartmentOnlineApproved = storeReturn.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = storeReturn.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = storeReturn.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = storeReturn.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = storeReturn.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = storeReturn.DepartmentSendForApprovalTime,

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

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReturns_Destroy([DataSourceRequest]DataSourceRequest request, StoreReturn storeReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreReturn
                {
                    StoreReturnID = storeReturn.StoreReturnID,
                    BranchId = storeReturn.BranchId,
                    BusinessUnitId = storeReturn.BusinessUnitId,
                    Reference = storeReturn.Reference,
                    StoreCode = storeReturn.StoreCode,
                    ReturnDate = storeReturn.ReturnDate,
                    Reason = storeReturn.Reason,
                    Status = storeReturn.Status,
                    IsForOther = storeReturn.IsForOther,
                    OrgStructureId = storeReturn.OrgStructureId,
                    CreatedAtOrgStructureId = storeReturn.CreatedAtOrgStructureId,
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

        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = db.StoreReturns.Find(id);
            if (record != null && !record.IsDepartmentOnlineApproved && !record.IsDepartmentSendForApproval && record.StoreReturnItems.Count>0)
            {
                if (record.BusinessUnitId == 0 && GlobalHelper.IsMultipleBusinessUnit())
                {
                    record.BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(record.CreatedBy);
                }

                record.Status = Enum.GetName(typeof(OperationStatuses), 16);
                record.IsDepartmentSendForApproval = true;
                record.DepartmentSendForApprovalBy = User.Identity.Name;
                record.DepartmentSendForApprovalTime = DateTime.Now;
                db.StoreReturns.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store return", "Success", User.Identity.Name, $"send for department approval store return {id} ", Modules.Inventory);
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
                                Reference = record.Reference + " " + record.Remark,
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
                                TransactionOrder = BinCardTransactionOrder.VoidReturn
                            };

                            db.BinCardLines.Add(binCardLine);
                            // db.SaveChanges();

                            #endregion
                            var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateSigleItemBinCardTransactions(storeItem.StoreItemAssignmentID, record.ReturnDate));
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
