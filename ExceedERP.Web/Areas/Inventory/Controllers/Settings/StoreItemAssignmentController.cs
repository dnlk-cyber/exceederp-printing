using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
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

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
   
    public class StoreItemAssignmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyAdmin,InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyBincard)]
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
        }
        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyBincard)]
        public ActionResult StoreItemAssignments_Read([DataSourceRequest]DataSourceRequest request,string storecode,bool isMinValue,bool isMaxValue,
            bool isDangerValue,bool isSafetyValue, bool isReorderValue)
        {
            var storeItemAssignments = db.StoreItemAssignments.AsQueryable();

            if (!string.IsNullOrEmpty(storecode))
                storeItemAssignments = storeItemAssignments.Where(x => x.StoreCode == storecode).AsQueryable();            

            if (isMinValue)
                storeItemAssignments = storeItemAssignments.Where(x => x.Balance <= x.MinPoint).AsQueryable();

            if (isMaxValue)
                storeItemAssignments = storeItemAssignments.Where(x => x.Balance >= x.MaxPoint).AsQueryable();//overage check

            if (isDangerValue)
                storeItemAssignments = storeItemAssignments.Where(x => x.Balance <= x.DangerPoint).AsQueryable();

            if (isSafetyValue)
                storeItemAssignments = storeItemAssignments.Where(x => x.Balance <= x.SafetyPoint).AsQueryable();

            if (isReorderValue)
                storeItemAssignments = storeItemAssignments.Where(x => x.Balance <= x.ReOrderPoint).AsQueryable();

            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);

            if (System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.PropertyStoreKeeper)&& !System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.PropertyAdmin))
                storeItemAssignments = storeItemAssignments.Where(x => userStores.Contains(x.StoreCode)).AsQueryable();

            IQueryable<StoreItemAssignment> storeItemassignments = storeItemAssignments;
            DataSourceResult result = storeItemassignments.ToDataSourceResult(request, storeItemAssignment => new {
                StoreItemAssignmentID = storeItemAssignment.StoreItemAssignmentID,
                StoreCode = storeItemAssignment.StoreCode,
                ItemCategoryCode = storeItemAssignment.ItemCategoryCode,
                ItemCode = storeItemAssignment.ItemCode,
                ItemName = storeItemAssignment.ItemName,
                ItemLocation = storeItemAssignment.ItemLocation,
                ReOrderPoint = storeItemAssignment.ReOrderPoint,
                SafetyPoint = storeItemAssignment.SafetyPoint,
                DangerPoint = storeItemAssignment.DangerPoint,
                MinPoint = storeItemAssignment.MinPoint,
                MaxPoint = storeItemAssignment.MaxPoint,
                Balance = storeItemAssignment.Balance,
                AvgPrice = storeItemAssignment.AvgPrice,
                BegAvgPrice = storeItemAssignment.BegAvgPrice,
                BegBalance = storeItemAssignment.BegBalance,
                Type = storeItemAssignment.Type,
                CreatedBy = storeItemAssignment.CreatedBy,
                DateCreated = storeItemAssignment.DateCreated,
                ModifiedBy = storeItemAssignment.ModifiedBy,
                LastModified = storeItemAssignment.LastModified
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreItemAssignments_Create([DataSourceRequest]DataSourceRequest request, StoreItemAssignment storeItemAssignment)
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
                        ItemName = storeItemAssignment.ItemName,
                        ReOrderPoint = storeItemAssignment.ReOrderPoint,
                        SafetyPoint = storeItemAssignment.SafetyPoint,
                        DangerPoint = storeItemAssignment.DangerPoint,
                        MinPoint = storeItemAssignment.MinPoint,
                        MaxPoint = storeItemAssignment.MaxPoint,
                        Balance = storeItemAssignment.Balance,
                        AvgPrice = storeItemAssignment.AvgPrice,
                        ItemLocation = storeItemAssignment.ItemLocation,
                        BegAvgPrice = storeItemAssignment.BegAvgPrice,
                        BegBalance = storeItemAssignment.BegBalance,
                        Type = storeItemAssignment.Type,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.StoreItemAssignments.Add(entity);
                    db.SaveChanges();
                    storeItemAssignment.StoreItemAssignmentID = entity.StoreItemAssignmentID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store item ", "Success", User.Identity.Name, $"created store item {entity.ItemCode} at store {entity.StoreCode}  ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { storeItemAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreItemAssignments_Update([DataSourceRequest]DataSourceRequest request, StoreItemAssignment storeItemAssignment)
        {
            #region validation
            var exists = db.StoreItemAssignments.Where(x => x.StoreItemAssignmentID != storeItemAssignment.StoreItemAssignmentID)
                                                 .Where(x => x.StoreCode == storeItemAssignment.StoreCode)
                                                  .Where(x => x.ItemCode == storeItemAssignment.ItemCode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Store Item",
                    storeItemAssignment.StoreCode + " and " + storeItemAssignment.ItemCode + " already exists in the system.");
               
            }

           

            #endregion
            if (ModelState.IsValid)
            {
             
                    var entity = new StoreItemAssignment
                    {
                        StoreItemAssignmentID = storeItemAssignment.StoreItemAssignmentID,
                        StoreCode = storeItemAssignment.StoreCode,
                        ItemCategoryCode = storeItemAssignment.ItemCategoryCode,
                        ItemCode = storeItemAssignment.ItemCode,
                        ItemName = storeItemAssignment.ItemName,
                        ReOrderPoint = storeItemAssignment.ReOrderPoint,
                        SafetyPoint = storeItemAssignment.SafetyPoint,
                        DangerPoint = storeItemAssignment.DangerPoint,
                        MinPoint = storeItemAssignment.MinPoint,
                        MaxPoint = storeItemAssignment.MaxPoint,
                        Balance = storeItemAssignment.Balance,
                        AvgPrice = storeItemAssignment.AvgPrice,
                        ItemLocation = storeItemAssignment.ItemLocation,
                        BegAvgPrice = storeItemAssignment.BegAvgPrice,
                        BegBalance = storeItemAssignment.BegBalance,
                        Type = storeItemAssignment.Type,
                        CreatedBy =storeItemAssignment.CreatedBy,
                        DateCreated=storeItemAssignment.DateCreated,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now

                    };

                    db.StoreItemAssignments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store item ", "Success", User.Identity.Name, $"updated store item {entity.ItemCode} at store {entity.StoreCode}  ", Modules.Inventory);
                    #endregion
                
            }

            return Json(new[] { storeItemAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreItemAssignments_Destroy([DataSourceRequest]DataSourceRequest request, StoreItemAssignment storeItemAssignment)
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

        [AuthorizeRoles(InventoryConstants.PropertyBincard)]
        public JsonResult RecalculateTransaction(string assignmentid,string binid)
        {
            if(!InventoryConfigurationHelper.IsBinCardRecalculateAllowed())
            {
                var errorformattedData = new
                {
                    Message = "recalculating bin card transaction is not allowed.",
                    Status = "NOT_OK"
                };
                return Json(errorformattedData, JsonRequestBehavior.AllowGet);
            }

            if (assignmentid != null && binid != null)
            {
                int storeAssignmentId = int.Parse(assignmentid);
                int bincardId = int.Parse(binid);

                var storeitem = db.StoreItemAssignments.Find(storeAssignmentId);

                var startingLine = db.BinCardLines
                    .Where(x => x.StoreItemAssignmentID == storeAssignmentId)
                    .FirstOrDefault(x => x.BinCardLineID == bincardId);

                InventoryBalanceHelper.RecalculateTransactionBincardData(storeAssignmentId, bincardId);

                #region  log
                UserHelper.OperationLog(Request.UserHostAddress, "Bin Card recalculation", "Success", User.Identity.Name, $"Bin Card recalculation done starting for item" +
                   $" {storeitem?.InventoryItem?.ItemCode} {storeitem?.InventoryItem?.Name} from ID {startingLine?.BinCardLineID}-{startingLine?.ReferenceDoc}",Modules.Inventory);
                #endregion

                //job to update related inventory records
                if (InventoryConfigurationHelper.RecalculateSourceTransactionOnBincardUpdate())
                {
                    var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateInventoryRecordWithBincardData(assignmentid, binid));
                }

                var formattedData1 = new
                {
                    Message = "Operation complete.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);

            }

            var formattedData = new
            {
                Message = "Please select a record to update transaction.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult RecalculateTransaction_old(string assignmentid, string binid)
        //{
        //    if (assignmentid != null && binid != null)
        //    {
        //        int storeAssignmentId = int.Parse(assignmentid);
        //        int bincardId = int.Parse(binid);

        //        var storeitem = db.StoreItemAssignments.Find(storeAssignmentId);

        //        var startingLine = db.BinCardLines
        //            .Where(x => x.StoreItemAssignmentID == storeAssignmentId)
        //            .FirstOrDefault(x => x.BinCardLineID == bincardId);

        //        var cardLines = db.BinCardLines
        //            .Where(x => x.StoreItemAssignmentID == storeAssignmentId)
        //            .Where(x => x.BinCardLineID > bincardId).ToArray();

        //        if (cardLines.Length > 0)
        //        {
        //            for (int i = 0; i < cardLines.Length; i++)
        //            {
        //                decimal totalPrice = 0;
        //                decimal totalQty = 0;
        //                string text = "";
        //                //if (cardLines[i].BinCardLineID == 771031)
        //                //     text = "";

        //                #region update the arary

        //                if (i == 0 && startingLine != null)
        //                {
        //                    #region incrementing operation

        //                    if (cardLines[i].ReferenceDoc.StartsWith("Receive") ||
        //                        cardLines[i].ReferenceDoc.StartsWith("Transfer Receive"))
        //                    {
        //                        #region grn Transfer Receive

        //                        totalQty = startingLine.BalanceQTY + cardLines[i].ReceivedQTY;
        //                        cardLines[i].BalanceQTY = totalQty;

        //                        totalPrice =
        //                            (startingLine.BalanceQTY * startingLine.BalanceUPrice)
        //                            + (cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice);

        //                        cardLines[i].ReceivedTPrice = cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice;
        //                        cardLines[i].BalanceUPrice = totalQty == 0 ? 0 : totalPrice / totalQty;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Return"))
        //                    {
        //                        #region return

        //                        totalQty = startingLine.BalanceQTY + cardLines[i].ReceivedQTY;
        //                        cardLines[i].BalanceQTY = totalQty;

        //                        cardLines[i].ReceivedTPrice = cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice;
        //                        cardLines[i].BalanceUPrice = startingLine.BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Issue"))
        //                    {
        //                        #region Void Issue

        //                        totalQty = startingLine.BalanceQTY + cardLines[i].ReceivedQTY;
        //                        cardLines[i].BalanceQTY = totalQty;

        //                        cardLines[i].ReceivedUPrice = Math.Abs(cardLines[i].ReceivedUPrice);
        //                        cardLines[i].ReceivedTPrice = cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice;
        //                        cardLines[i].BalanceUPrice = startingLine.BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Transfer Send"))
        //                    {
        //                        #region Void Transfer Send

        //                        totalQty = startingLine.BalanceQTY + cardLines[i].ReceivedQTY;
        //                        cardLines[i].BalanceQTY = totalQty;

        //                        cardLines[i].ReceivedTPrice = cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice;
        //                        cardLines[i].BalanceUPrice = startingLine.BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    #endregion
        //                    #region dencrementing operation

        //                    if (cardLines[i].ReferenceDoc.StartsWith("Issue") ||
        //                        cardLines[i].ReferenceDoc.StartsWith("Transfer Send"))
        //                    {
        //                        #region issue

        //                        cardLines[i].IssuedUPrice = startingLine.BalanceUPrice;
        //                        cardLines[i].IssuedTPrice =
        //                            cardLines[i].IssuedUPrice * cardLines[i].IssuedQTY;

        //                        totalQty = startingLine.BalanceQTY - cardLines[i].IssuedQTY;

        //                        cardLines[i].BalanceQTY = totalQty;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice;
        //                        cardLines[i].BalanceUPrice = startingLine.BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }
        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Receive"))
        //                    {
        //                        //decimal totalpricebefore = 0;

        //                        #region Void Receive

        //                        totalQty = startingLine.BalanceQTY - cardLines[i].IssuedQTY;
        //                        //get issued price from original record if possible
        //                        //totalpricebefore = cardLines[i].BalanceTPrice -
        //                        //                   (cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice);

        //                        cardLines[i].BalanceQTY = totalQty;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice;
        //                        cardLines[i].BalanceUPrice = totalQty == 0 ? 0 : startingLine.BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Transfer Receive"))
        //                    {
        //                        decimal totalpricebefore = 0;

        //                        #region Void Transfer Receive

        //                        totalQty = startingLine.BalanceQTY - cardLines[i].IssuedQTY;
        //                        //get issued price from original record if possible
        //                        totalpricebefore = cardLines[i].BalanceTPrice -
        //                                           (cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice);

        //                        cardLines[i].BalanceQTY = totalQty;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice;
        //                        cardLines[i].BalanceUPrice = totalQty == 0 ? 0 : totalpricebefore / totalQty;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }
        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Return"))
        //                    {
        //                        #region Void Return

        //                        totalQty = startingLine.BalanceQTY - cardLines[i].IssuedQTY;

        //                        cardLines[i].BalanceQTY = totalQty;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice;
        //                        cardLines[i].BalanceUPrice = startingLine.BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region incrementing operation

        //                    if (cardLines[i].ReferenceDoc.StartsWith("Receive") ||
        //                        cardLines[i].ReferenceDoc.StartsWith("Transfer Receive"))
        //                    {
        //                        #region grn Transfer Receive

        //                        totalQty = cardLines[i - 1].BalanceQTY + cardLines[i].ReceivedQTY;
        //                        cardLines[i].BalanceQTY = totalQty;

        //                        totalPrice =
        //                            (cardLines[i - 1].BalanceQTY * cardLines[i - 1].BalanceUPrice)
        //                            + (cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice);

        //                        cardLines[i].ReceivedTPrice = cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice;
        //                        cardLines[i].BalanceUPrice = totalQty == 0 ? 0 : totalPrice / totalQty;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Return"))
        //                    {
        //                        #region return

        //                        totalQty = cardLines[i - 1].BalanceQTY + cardLines[i].ReceivedQTY;
        //                        cardLines[i].BalanceQTY = totalQty;

        //                        cardLines[i].ReceivedTPrice = cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice;
        //                        cardLines[i].BalanceUPrice = cardLines[i - 1].BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Issue"))
        //                    {
        //                        #region Void Issue

        //                        totalQty = cardLines[i - 1].BalanceQTY + cardLines[i].ReceivedQTY;
        //                        cardLines[i].BalanceQTY = totalQty;

        //                        cardLines[i].ReceivedUPrice = Math.Abs(cardLines[i].ReceivedUPrice);
        //                        cardLines[i].ReceivedTPrice = cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice;
        //                        cardLines[i].BalanceUPrice = cardLines[i - 1].BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Transfer Send"))
        //                    {
        //                        #region Void Transfer Send

        //                        totalQty = cardLines[i - 1].BalanceQTY + cardLines[i].ReceivedQTY;
        //                        cardLines[i].BalanceQTY = totalQty;

        //                        cardLines[i].ReceivedTPrice = cardLines[i].ReceivedQTY * cardLines[i].ReceivedUPrice;
        //                        cardLines[i].BalanceUPrice = cardLines[i - 1].BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    #endregion
        //                    #region dencrementing operation

        //                    if (cardLines[i].ReferenceDoc.StartsWith("Issue") ||
        //                        cardLines[i].ReferenceDoc.StartsWith("Transfer Send"))
        //                    {
        //                        #region issue

        //                        cardLines[i].IssuedUPrice = cardLines[i - 1].BalanceUPrice;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedUPrice * cardLines[i].IssuedQTY;

        //                        totalQty = cardLines[i - 1].BalanceQTY - cardLines[i].IssuedQTY;

        //                        cardLines[i].BalanceQTY = totalQty;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice;
        //                        cardLines[i].BalanceUPrice = cardLines[i - 1].BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }
        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Receive"))
        //                    {
        //                        //decimal totalpricebefore = 0;

        //                        #region Void Receive

        //                        totalQty = cardLines[i - 1].BalanceQTY - cardLines[i].IssuedQTY;
        //                        ////get issued price from original record if possible
        //                        //totalpricebefore = cardLines[i].BalanceTPrice -
        //                        //                   (cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice);

        //                        cardLines[i].IssuedUPrice = cardLines[i - 1].BalanceUPrice;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedUPrice * cardLines[i].IssuedQTY;

        //                        cardLines[i].BalanceQTY = totalQty;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice;
        //                        cardLines[i].BalanceUPrice = totalQty == 0 ? 0 : cardLines[i - 1].BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Transfer Receive"))
        //                    {
        //                        decimal totalpricebefore = 0;

        //                        #region Void Transfer Receive

        //                        totalQty = cardLines[i - 1].BalanceQTY - cardLines[i].IssuedQTY;
        //                        //get issued price from original record if possible
        //                        totalpricebefore = cardLines[i].BalanceTPrice -
        //                                           (cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice);

        //                        cardLines[i].BalanceQTY = totalQty;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice;
        //                        cardLines[i].BalanceUPrice = totalQty == 0 ? 0 : totalpricebefore / totalQty;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }
        //                    else if (cardLines[i].ReferenceDoc.StartsWith("Void Return"))
        //                    {
        //                        #region Void Return

        //                        totalQty = cardLines[i - 1].BalanceQTY - cardLines[i].IssuedQTY;

        //                        cardLines[i].BalanceQTY = totalQty;
        //                        cardLines[i].IssuedTPrice = cardLines[i].IssuedQTY * cardLines[i].IssuedUPrice;
        //                        cardLines[i].BalanceUPrice = cardLines[i - 1].BalanceUPrice;
        //                        cardLines[i].BalanceTPrice = cardLines[i].BalanceUPrice * cardLines[i].BalanceQTY;

        //                        #endregion
        //                    }

        //                    #endregion
        //                }

        //                #endregion

        //                db.Entry(cardLines[i]).State = EntityState.Modified;

        //                if (i == cardLines.Length - 1)
        //                {
        //                    if (storeitem != null)
        //                    {
        //                        storeitem.AvgPrice = cardLines[i].BalanceUPrice;
        //                        storeitem.Balance = cardLines[i].BalanceQTY;
        //                        db.Entry(storeitem).State = EntityState.Modified;
        //                    }
        //                }
        //                db.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            if (storeitem != null && startingLine != null)
        //            {
        //                storeitem.AvgPrice = startingLine.BalanceUPrice;
        //                storeitem.Balance = startingLine.BalanceQTY;
        //                db.Entry(storeitem).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }

        //        #region  log
        //        UserHelper.OperationLog(Request.UserHostAddress, "Bin Card recalculation", "Success", User.Identity.Name, $"Bin Card recalculation done starting for item" +
        //           $" {storeitem?.InventoryItem?.ItemCode} {storeitem?.InventoryItem?.Name} from ID {startingLine?.BinCardLineID}-{startingLine?.ReferenceDoc}",Modules.Inventory);
        //        #endregion

        //        //job to update related inventory records
        //        var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateInventoryRecordWithBincardData(assignmentid, binid));
        //        var formattedData1 = new
        //        {
        //            Message = "Operation complete.",
        //            Status = "OK"
        //        };
        //        return Json(formattedData1, JsonRequestBehavior.AllowGet);

        //    }

        //    var formattedData = new
        //    {
        //        Message = "Please select a record to update transaction.",
        //        Status = "NOT_OK"
        //    };
        //    return Json(formattedData, JsonRequestBehavior.AllowGet);

        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing); 
        }
       
    }
}
