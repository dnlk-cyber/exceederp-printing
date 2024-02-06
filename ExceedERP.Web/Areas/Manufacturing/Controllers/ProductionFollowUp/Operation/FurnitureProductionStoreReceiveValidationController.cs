using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpFinishedGoodUser)]
    public class FurnitureProductionStoreReceiveValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover, InventoryConstants.PropertyInspector)]
        public ActionResult ReceiveValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureProductionFinishedGoodsStoreReceiveValidation> receivevalidations = db.FurnitureProductionFinishedGoodsStoreReceiveValidations.Where(gi => gi.FurnitureFinishedGoodsStoreReceiveId == id);
            DataSourceResult result = receivevalidations.ToDataSourceResult(request, receiveValidation => new
            {
                FinishedGoodsStoreReceiveId = receiveValidation.FurnitureFinishedGoodsStoreReceiveId,
                Status = receiveValidation.Status,
                Remark = receiveValidation.Remark,
                OrgBranchName = receiveValidation.OrgBranchName,
            });

            return Json(result);
        }

        //[AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveValidations_Create([DataSourceRequest] DataSourceRequest request, FurnitureProductionFinishedGoodsStoreReceiveValidation receiveValidation, int id)
        {
            var record = db.FurnitureProductionFinishedGoodsStoreReceives.Find(id);
            //var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            //if (pricesetting == null)
            //    ModelState.AddModelError("price setting", "Average price setting is not defined.");
            #region Checking the grn

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (record.ProductionFinishedGoodsStoreReceiveItems.Count == 0)
                {
                    string message = "The Record Does not Contain Any Item";
                    ModelState.AddModelError(String.Empty, message);
                }
                if (receiveValidation.Status == ApprovalStatuses.Approve)
                {
                    foreach (var i in record.ProductionFinishedGoodsStoreReceiveItems)
                    {
                        var item = db.InventoryItems.FirstOrDefault(ic => ic.ItemCode == i.ItemCode);
                        if (item == null)
                        {
                            string message = "Item " + i.ItemCode + " is not defined in the system.";
                            ModelState.AddModelError("stockItem", message);
                        }
                    }
                }
                if (receiveValidation.Status == ApprovalStatuses.Reject)
                {
                    record.IsSendForApproval = false;
                    record.Status = "Rejected";
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "GRN record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (receiveValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.FurnitureFinishedGoodsStoreReceiveId = id;
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                       

                        var store = db.InventoryStores.Find(record.StoreCode);
                        foreach (var i in record.ProductionFinishedGoodsStoreReceiveItems)
                        {
                            if (i.Quantity > 0)
                            {
                                #region update stock avg price and total


                                var inventoryItem = db.InventoryItems.Find(i.ItemCode);

                                //var branchItem =
                                //    db.BranchItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
                                //        .FirstOrDefault(x => x.BranchId == store.BranchId);
                                var storeItem =
                                    db.StoreItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
                                        .FirstOrDefault(x => x.StoreCode == store.StoreCode);
                                var priceIndex =
                                    db.ItemPriceIndexes.Where(x => x.SourceType == SourceType.GRN)
                                        .FirstOrDefault(x => x.ItemCode == i.ItemCode);

                                if (inventoryItem != null)
                                {
                                    //if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                    //{
                                        inventoryItem.AvgPrice = ((inventoryItem.Balance * inventoryItem.AvgPrice) +
                                                                  (i.Quantity * i.UnitPrice))
                                                                 / (inventoryItem.Balance + i.Quantity);
                                    //}

                                    inventoryItem.Balance = inventoryItem.Balance + i.Quantity;
                                    db.Entry(inventoryItem).State = EntityState.Modified;
                                    db.SaveChanges();

                                    if (priceIndex != null)
                                    {
                                        priceIndex.PriceBeforeVAT = i.UnitPrice;
                                        db.Entry(priceIndex).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    if (priceIndex == null)
                                    {
                                        var priceindex = new ItemPriceIndex
                                        {
                                            BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode),
                                            ItemCategoryCode = inventoryItem.ItemCategoryCode,
                                            ItemCode = inventoryItem.ItemCode,
                                            PriceBeforeVAT = i.UnitPrice,
                                            SourceOfData = "GRN",
                                            GLPeriodId = record.FurnitureProductionPeriodId,
                                            DateOfInformation = i.PrintingProductionFinishedGoodsStoreReceive.ReceiveDate,
                                            SourceType = SourceType.GRN
                                        };
                                        db.ItemPriceIndexes.Add(priceindex);
                                        db.SaveChanges();
                                    }

                                    //if (branchItem != null)
                                    //{
                                    //    if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                    //    {
                                    //        branchItem.AvgPrice = ((branchItem.Balance * branchItem.AvgPrice) +
                                    //                               (i.Quantity * i.UnitPrice))
                                    //                              / (branchItem.Balance + i.Quantity);
                                    //    }

                                    //    branchItem.Balance = branchItem.Balance + i.Quantity;
                                    //    db.Entry(branchItem).State = EntityState.Modified;
                                    //    db.SaveChanges();
                                    //}
                                    //else
                                    //{
                                    //    branchItem = new BranchItemAssignment();
                                    //    if (store != null) branchItem.BranchId = store.BranchId;
                                    //    branchItem.ItemCategoryCode = inventoryItem.ItemCategoryCode;
                                    //    branchItem.ItemCode = inventoryItem.ItemCode;
                                    //    branchItem.Balance = i.Quantity;
                                    //    branchItem.AvgPrice = i.UnitPrice;
                                    //    db.BranchItemAssignments.Add(branchItem);
                                    //    db.SaveChanges();
                                    //}

                                    if (storeItem != null)
                                    {
                                        storeItem.AvgPrice = ((storeItem.Balance * storeItem.AvgPrice) +
                                                                  (i.Quantity * i.UnitPrice))
                                                                 / (storeItem.Balance + i.Quantity);

                                        storeItem.Balance = storeItem.Balance + i.Quantity;
                                        db.Entry(storeItem).State = EntityState.Modified;
                                        db.SaveChanges();

                                        #region bin card line

                                        BinCardLine binCardLine = new BinCardLine
                                        {
                                            StoreItemAssignment = storeItem,
                                            ReferenceDoc = "Receive_" + record.FurnitureFinishedGoodsStoreReceiveId.ToString(),
                                            //Reference = !String.IsNullOrEmpty(record.ReceiveRef) ? record.ReceiveRef : record.Reference,
                                            Date = record.ReceiveDate,
                                            ReceivedQTY = i.Quantity,
                                            ReceivedUPrice = i.UnitPrice,
                                            ReceivedTPrice = i.Total,
                                            BalanceQTY = storeItem.Balance,
                                            SupplierName = ""
                                        };

                                        #region bin card average price

                                        //if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                        //{
                                        //    binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                        //    binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                        //}

                                        //else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                        //{
                                        //    binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                        //    binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                        //}
                                        //else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                        //{
                                            binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                            binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                        //}


                                        #endregion

                                        db.BinCardLines.Add(binCardLine);
                                        db.SaveChanges();

                                        #endregion
                                    }
                                    else
                                    {
                                        storeItem = new StoreItemAssignment
                                        {
                                            StoreCode = record.StoreCode,
                                            ItemCategoryCode = inventoryItem.ItemCategoryCode,
                                            ItemCode = inventoryItem.ItemCode,
                                            Balance = i.Quantity,
                                            AvgPrice = i.UnitPrice,
                                        };
                                        db.StoreItemAssignments.Add(storeItem);
                                        db.SaveChanges();

                                        #region bin card line

                                        BinCardLine binCardLine = new BinCardLine
                                        {
                                            StoreItemAssignment = storeItem,
                                            ReferenceDoc = "Receive_" + record.ReceiveDate.ToString(),
                                            //Reference = !String.IsNullOrEmpty(record.ReceiveRef) ? record.ReceiveRef : record.Reference,
                                            Date = record.ReceiveDate,
                                            ReceivedQTY = i.Quantity,
                                            ReceivedUPrice = i.UnitPrice,
                                            ReceivedTPrice = i.Total,
                                            BalanceQTY = storeItem.Balance,
                                            SupplierName = ""

                                        };

                                        #region bin card average price

                                        //if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                        //{
                                        //    binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                        //    binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                        //}

                                        //else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                        //{
                                        //    binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                        //    binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                        //}
                                        //else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                        //{
                                            binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                            binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                        //}


                                        #endregion

                                        db.BinCardLines.Add(binCardLine);
                                        db.SaveChanges();

                                        #endregion
                                    }
                                }

                                #endregion
                            }
                        }

                        #region update production order item status
                        var storeRecieveItems = record.ProductionFinishedGoodsStoreReceiveItems.ToList();
                        //if (storeRecieveItems.Any())
                        //{
                        //    if (record.ProductType == ProductType.Seed)
                        //    {
                        //        var orderItemIds = new HashSet<int>(storeRecieveItems.Select(i => i.SeedProductionOrderItemId));
                        //        var productionOrderItems = db.SeedProductionOrderItems.Where(poi => orderItemIds.Contains(poi.SeedProductionOrderItemId)).ToList();
                        //        foreach(var productionOrderItem in productionOrderItems)
                        //        {
                        //            productionOrderItem.Status = ProductStatus.RawProduct;

                        //            db.SeedProductionOrderItems.Attach(productionOrderItem);
                        //            db.Entry(productionOrderItem).State = EntityState.Modified;
                        //            db.SaveChanges();
                        //        }
                        //    }
                        //    else if (record.ProductType == ProductType.NaturalGum)
                        //    {
                        //        var orderItemIds = new HashSet<int>(storeRecieveItems.Select(i => i.GumProductionOrderItemId));
                        //        var productionOrderItems = db.GumProductionOrderItems.Where(poi => orderItemIds.Contains(poi.GumProductionOrderItemId)).ToList();
                        //        foreach (var productionOrderItem in productionOrderItems)
                        //        {
                        //            productionOrderItem.Status = ProductStatus.RawProduct;

                        //            db.GumProductionOrderItems.Attach(productionOrderItem);
                        //            db.Entry(productionOrderItem).State = EntityState.Modified;
                        //            db.SaveChanges();
                        //        }
                        //    }
                        //}
                        #endregion
                    }
                    if (receiveValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var entity = new FurnitureProductionFinishedGoodsStoreReceiveValidation
                    {
                        FurnitureFinishedGoodsStoreReceiveId = id,
                        Status = receiveValidation.Status,
                        Remark = receiveValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = receiveValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = receiveValidation.ModifiedBy
                    };

                    db.FurnitureProductionFinishedGoodsStoreReceiveValidations.Add(entity);
                    db.SaveChanges();
                    receiveValidation.FurnitureFinishedGoodsStoreReceiveValidationId = entity.FurnitureFinishedGoodsStoreReceiveValidationId;
                }

            }


            return Json(new[] { receiveValidation }.ToDataSourceResult(request, ModelState));
        }

        //[AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveValidations_Update([DataSourceRequest]DataSourceRequest request, FurnitureProductionFinishedGoodsStoreReceiveValidation receiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionFinishedGoodsStoreReceiveValidation
                {
                    FurnitureFinishedGoodsStoreReceiveValidationId = receiveValidation.FurnitureFinishedGoodsStoreReceiveValidationId,
                    FurnitureFinishedGoodsStoreReceiveId = receiveValidation.FurnitureFinishedGoodsStoreReceiveId,
                    Status = receiveValidation.Status,
                    Remark = receiveValidation.Remark,
                    //Approver = User.Identity.Name,
                    OrgBranchName = receiveValidation.OrgBranchName,
                };

                db.FurnitureProductionFinishedGoodsStoreReceiveValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { receiveValidation }.ToDataSourceResult(request, ModelState));
        }

        //[AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveValidations_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionFinishedGoodsStoreReceiveValidation receiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionFinishedGoodsStoreReceiveValidation
                {
                    FurnitureFinishedGoodsStoreReceiveValidationId = receiveValidation.FurnitureFinishedGoodsStoreReceiveValidationId,
                    FurnitureFinishedGoodsStoreReceiveId = receiveValidation.FurnitureFinishedGoodsStoreReceiveId,
                    Status = receiveValidation.Status,
                    Remark = receiveValidation.Remark,
                    OrgBranchName = receiveValidation.OrgBranchName,
                };

                db.FurnitureProductionFinishedGoodsStoreReceiveValidations.Attach(entity);
                db.FurnitureProductionFinishedGoodsStoreReceiveValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { receiveValidation }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}