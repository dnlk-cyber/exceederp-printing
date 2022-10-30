using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.ShipmentApprover)]
    public class SalesShipmentValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SalesShipmentValidation> validations = db.SalesShipmentValidation.Where(v => v.SalesShipmentId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.SalesShipmentValidationId,
                validation.SalesShipmentId,

                validation.OrgBranchName,
                validation.DateCreated,
                validation.LastModified,
                validation.CreatedBy,
                validation.ModifiedBy,

                validation.Status,
                validation.Remark
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, SalesShipmentValidation validation, int id)
        {
            var record = db.SalesShipments.Find(id);

            #region Checking the  record

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                else if (!record.IsSendForApproval)
                {
                    ModelState.AddModelError("Not_Sent_for_Approval", "Record not Waiting Administrative approval. Operation Canceled!");
                }
                else
                {

                }
            }
            else
            {
                ModelState.AddModelError("Not Found", "Shipment record not found");
            }
            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (validation.Status == ApprovalStatuses.Approve)
                    {
                        var result = UpdateChangesOnShipmentApprove(record);
                        if (result.Succeed)
                        {
                            #region update
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;

                            #endregion
                        }
                        else
                        {
                            ModelState.AddModelError("", result.ErrorMessage);
                        }
                    }
                    if (validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                    db.SalesShipments.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    //var res =   db.SaveChanges();
                    //if(res >0 && )
                }
                if (ModelState.IsValid)
                {
                    var entity = new SalesShipmentValidation
                    {
                        //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                        SalesShipmentId = id,
                        Status = validation.Status,
                        Remark = validation.Remark,

                        OrgBranchName = validation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = validation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = validation.ModifiedBy,
                    };

                    db.SalesShipmentValidation.Add(entity);
                    db.SaveChanges();
                    validation.SalesShipmentValidationId = entity.SalesShipmentValidationId;
                }
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, SalesShipmentValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesShipmentValidation
                {
                    SalesShipmentValidationId = validation.SalesShipmentValidationId,
                    SalesShipmentId = validation.SalesShipmentId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.SalesShipmentValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, SalesShipmentValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesShipmentValidation
                {
                    SalesShipmentValidationId = validation.SalesShipmentValidationId,
                    SalesShipmentId = validation.SalesShipmentId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.SalesShipmentValidation.Attach(entity);
                db.SalesShipmentValidation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        private CRMTransactionValidationStatusViewModel UpdateChangesOnShipmentApprove(SalesShipment shipmentRecord)
        {
            CRMTransactionValidationStatusViewModel statusModel = new CRMTransactionValidationStatusViewModel
            {
                Succeed = false,
            };
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();

            List<BinCardLine> binCardLines = new List<BinCardLine>();

            //bool transactionCompletedSuccessfully = false;
            var shipmentLineItems = db.SalesShipmentLineItem.Where(li => li.SalesShipmentId == shipmentRecord.SalesShipmentId).ToList();
            foreach (SalesShipmentLineItem shipmentLineItem in shipmentLineItems)
            {
                decimal beforeTransaction = 0;
                decimal afterTransaction = 0;
                decimal TransactionQuantity = 0;

                //LineItem items = db.LineItems.Where(x => x.LineItemID.ToString() == item.Id && x.VoucherDefinitionID == "2").FirstOrDefault();
                //if (items.Quantity > 0)
                //{

                var salesStore = db.SalesStores.Find(shipmentLineItem.SalesStoreId);
                //CashSalesInvoice sales = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID.ToString() == items.Voucher).FirstOrDefault();
                InventoryStore Store =
                  db.InventoryStores.FirstOrDefault(
                      x => x.StoreCode == salesStore.StoreCode);
                Product product =
                    db.Products.FirstOrDefault(x => x.ProductID == shipmentLineItem.ProductId);

                StoreItemAssignment storeItem =
                    db.StoreItemAssignments
                    .Where(x => x.InventoryStore.StoreCode == Store.StoreCode &&
                                x.InventoryItem.ItemCode == product.ItemCode)
                                .OrderByDescending(x => x.StoreItemAssignmentID).FirstOrDefault();
                if (Store != null && product != null && storeItem != null)
                {
                    decimal finalQuantity = storeItem.Balance - shipmentLineItem.ShipedQuantity;

                    if (shipmentLineItem.ShipedQuantity > storeItem.Balance)
                    {
                        //finalQuantity = 0;
                        statusModel.Succeed = false;
                        statusModel.ErrorMessage = $"No enough quantity To Issue(or ship) for item  {storeItem.ItemCode}. Current balance is {storeItem.Balance} and requested quantity is {shipmentLineItem.ShipedQuantity}";
                        return statusModel;
                    }
                    else
                    {
                        beforeTransaction = storeItem.Balance;
                        storeItem.Balance = finalQuantity;

                        afterTransaction = finalQuantity;
                        TransactionQuantity = shipmentLineItem.ShipedQuantity;

                        db.StoreItemAssignments.Attach(storeItem);
                        db.Entry(storeItem).State = EntityState.Modified;
                        //db.SaveChanges();

                        string payMethod = "";
                        string customer = "";
                        if (shipmentRecord.Source == ShipmentSource.SalesInvoice)
                        {
                            var invoice = db.CashSalesInvoices.FirstOrDefault(i => i.CashSalesInvoiceID == shipmentRecord.CashSalesInvoiceID);
                            if (invoice != null)
                            {
                                customer = invoice.OrganizationCustomer.TradeName;
                                payMethod = invoice.PaymentMethod;
                            }
                        }
                        else
                        {
                            var order = db.SalesOrderVouchers.FirstOrDefault(i => i.SalesOrderVoucherID == shipmentRecord.SalesOrderVoucherID);
                            if (order != null)
                            {
                                customer = order.OrganizationCustomer.TradeName;
                                payMethod = order.PaymentMethod;
                            }
                        }
                        //var Transaction = new StoreTransaction
                        //{
                        //    Voucher = shipmentLineItem.SalesShipment.CashSalesInvoiceID.ToString(),
                        //    Source = salesStore.Description + " -- " + product.Name,
                        //    //Destination = shipmentRecord.CashSalesInvoice.OrganizationCustomer.TradeName,
                        //    Destination = customer,
                        //    Amount = (shipmentLineItem.Price * shipmentLineItem.ShipedQuantity).ToString(),
                        //    Quantity = TransactionQuantity,
                        //    beforeStock = beforeTransaction,
                        //    afterStock = afterTransaction,
                        //    //TransactionType = shipmentRecord.CashSalesInvoice.PaymentMethod
                        //    TransactionType = payMethod
                        //};

                        //db.StoreTransactions.Add(Transaction);
                        //db.SaveChanges();

                        #region update bin card

                        #region bin card line

                        var uPrice = InventoryHelper.GetItemAveragePrice(product.ItemCode, Store.StoreCode);
                        BinCardLine binCardLine = new BinCardLine
                        {
                            StoreItemAssignmentID = storeItem.StoreItemAssignmentID,
                            StoreItemAssignment = storeItem,
                            ReferenceDoc = "Shipment " + shipmentRecord.SalesShipmentId.ToString(),
                            Reference = shipmentRecord.Source == ShipmentSource.SalesInvoice ? "Invoice " + shipmentRecord.CashSalesInvoiceID : "Sales Order " + shipmentRecord.SalesOrderVoucherID,
                            Date = shipmentRecord.ShipmentDate,
                            IssuedQTY = shipmentLineItem.ShipedQuantity,
                            IssuedUPrice = shipmentLineItem.Price,
                            IssuedTPrice = shipmentLineItem.TotalAmount,
                            BalanceQTY = storeItem.Balance,
                            BalanceUPrice = uPrice,
                            BalanceTPrice = uPrice * storeItem.Balance,
                            //IssuedTo = shipmentRecord.OrganizationCustomerId.ToString(),
                            IssuedTo = "",

                            //new    
                            StoreCode = GetInventoryStoreCode(shipmentLineItem.SalesStoreId),
                            GLPeriodId = GetGlPeriod(shipmentRecord.CRMPeriodId),
                            TransactionType = BinCardTransactionTypes.Issue,
                            StatusType = BinCardTransactionStatus.Regular,

                        };
                        db.BinCardLines.Add(binCardLine);
                        // db.SaveChanges();
                        binCardLines.Add(binCardLine);
                        #endregion

                        #endregion

                        //transactionCompletedSuccessfully = true;
                    }
                }


                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                {
                    int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(salesStore?.StoreCode);
                    var branchItem = db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
                        .FirstOrDefault(x => x.ItemCode == product.ItemCode);
                    if (branchItem != null)
                    {
                        branchItem.Balance = branchItem.Balance - shipmentLineItem.ShipedQuantity;
                        db.Entry(branchItem).State = EntityState.Modified;
                        // db.SaveChanges();
                    }
                }

                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                {
                    var stockItem = db.InventoryItems.Find(product.ItemCode);
                    if (stockItem != null)
                    {
                        stockItem.Balance = stockItem.Balance - shipmentLineItem.ShipedQuantity;
                        db.Entry(stockItem).State = EntityState.Modified;
                        // db.SaveChanges();
                    }
                }
            }
            var result = db.SaveChanges();
            if (result > 0)
            {
                statusModel.Succeed = true;
                //InventoryBalanceHelper.AutoRecalculateBincardLines(binCardLines);
                List<int> ids = binCardLines.Select(b => b.BinCardLineID).ToList();
                var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.AutoRecalculateBincardLines(ids));
            }
            return statusModel;
        }
        private string GetInventoryStoreCode(int salesStoreId)
        {
            return db.SalesStores.Find(salesStoreId)?.StoreCode;
        }
        private int GetGlPeriod(int crmPeriodId)
        {
            int id = 0;
            var crmPeriod = db.CRMPeriods.Find(crmPeriodId);
            if (crmPeriod != null)
            {
                var glPeriod = db.GLPeriods.FirstOrDefault(p => p.Name == crmPeriod.Name);
                if (glPeriod != null)
                {
                    id = glPeriod.GLPeriodId;
                }
            }
            return id;
        }
        
       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}