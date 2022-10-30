using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.CRM.Sales;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CRM;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Hangfire;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.ShipmentApprover, CRMRoles.ShipmentUser, CRMRoles.IncompleteShipmentAdmin)]
    public class ShipmentsController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: CRM/Shipments
        public ActionResult Index()
        {
            ViewData["SalesInvoices"] = db.CashSalesInvoices.Select(s => new { CashSalesInvoiceID = s.CashSalesInvoiceID, InvoiceNumberDisplay = s.CashSalesInvoiceID }).ToList();
            ViewData["SalesOrderVouchers"] = db.SalesOrderVouchers.Select(s => new { SalesOrderVoucherID = s.SalesOrderVoucherID }).ToList();
            ViewData["StoreList"] = db.Stores;
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["Thing"] = db.Products.Select(s => new { s.ProductID, Name = s.ItemCode + " " + s.Name}).ToList();
            ViewData["Tax"] = db.GLTaxRates.ToList();

            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            ViewData["InventoryItems"] = db.InventoryItems.Select(i => new {Name = i.ItemCode + " " + i.Name, i.ItemCode}).ToList();
            ViewData["InventoryCategory"] = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode}).ToList();

            if (ShipmentWithEmptyStoreExist())
            {
                UpdateEmptyStores();
            }

            return View();
        }
        public ActionResult UpdateEmptyStores()
        {
            //
            // assign store id from its line item
            var emptyStores = db.SalesShipments.Where(s => s.StoreID == 0).ToList();
            foreach (var sh in emptyStores)
            {
                var lineItem = db.SalesShipmentLineItem.FirstOrDefault(l => l.SalesShipmentId == sh.SalesShipmentId);
                if (lineItem != null)
                {
                    sh.StoreID = lineItem.SalesStoreId;

                    db.SalesShipments.Attach(sh);
                    db.Entry(sh).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            //

            return Redirect(nameof(Index));
        }
        private bool ShipmentWithEmptyStoreExist()
        {
            return db.SalesShipments.Any(s => s.StoreID == 0);
        }
        public ActionResult AddFsInvoiceNumber()
        {
            var emptyRecords = db.SalesShipments.Where(s => s.Source == ShipmentSource.SalesInvoice && s.FsInvoiceNumber == null).ToList();
            foreach (var sh in emptyRecords)
            {
                var invoice = db.CashSalesInvoices.Find(sh.CashSalesInvoiceID);
                if (invoice != null)
                {
                    sh.FsInvoiceNumber = invoice.FsInvoiceNo;

                    db.SalesShipments.Attach(sh);
                    db.Entry(sh).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Redirect(nameof(Index));
        }
        /// <summary>
        /// add distributions manually 
        /// </summary>
        /// <returns></returns>
        [Authorize(Users = "tester")]
        public ActionResult AddDistribution()
        {
            var allShipments = db.SalesShipments.Where(s => !s.IsOnlineTransferred).ToList();
            foreach (var shipment in allShipments)
            {
                new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(shipment);
            }
            return Redirect(nameof(Index));
        }



        //[Authorize(Users = "tester")]
        [AuthorizeRoles(CRMRoles.IncompleteShipmentAdmin)]
        public ActionResult IncompleteShipments()
        {
            ViewData["SalesInvoices"] = db.CashSalesInvoices.Select(s => new { CashSalesInvoiceID = s.CashSalesInvoiceID, InvoiceNumberDisplay = s.CashSalesInvoiceID }).ToList();
            ViewData["SalesOrderVouchers"] = db.SalesOrderVouchers.Select(s => new { SalesOrderVoucherID = s.SalesOrderVoucherID }).ToList();
            ViewData["StoreList"] = db.Stores;
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["Thing"] = db.Products.ToList();
            ViewData["Tax"] = db.GLTaxRates.ToList();

            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            return View();
        }
        //[Authorize(Users = "tester")]
        [AuthorizeRoles(CRMRoles.IncompleteShipmentAdmin)]
        public ActionResult PartiallyIncompleteShipments()
        {
            ViewData["SalesInvoices"] = db.CashSalesInvoices.Select(s => new { CashSalesInvoiceID = s.CashSalesInvoiceID, InvoiceNumberDisplay = s.CashSalesInvoiceID }).ToList();
            ViewData["SalesOrderVouchers"] = db.SalesOrderVouchers.Select(s => new { SalesOrderVoucherID = s.SalesOrderVoucherID }).ToList();
            ViewData["StoreList"] = db.Stores;
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["Thing"] = db.Products.ToList();
            ViewData["Tax"] = db.GLTaxRates.ToList();

            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            return View();
        }
        //[Authorize(Users = "tester")]
        [AuthorizeRoles(CRMRoles.IncompleteShipmentAdmin)]
        public JsonResult SendMissingRecordToBincard(int id)
        {
            var record = db.SalesShipments.Find(id);

            if (record == null)
            {
                //ModelState.AddModelError("Not Found", "Shipment record not found");
                return Json(new { Message = "Shipment record not found", Status = "Erorr" });
            }

            var result = UpdateChangesOnShipmentApprove(record);
            if (result.Succeed)
            {
                return Json(new { Message = "Shipment no. " + record.SalesShipmentId + " transfered to store transaction and Bincard Successfully", Status = "OK" });
            }
            else
            {
                //ModelState.AddModelError("", "Something Went Wrong in Processing the Transaction!");
                return Json(new { Message = result.ErrorMessage, Status = "Erorr" });

            }
        }
        //[Authorize(Users = "tester")]
        [AuthorizeRoles(CRMRoles.IncompleteShipmentAdmin)]
        public JsonResult SendMissingLineItemToBincard(int itemId)
        {
            var record = db.SalesShipmentLineItem.Find(itemId);

            if (record == null)
            {
                return Json(new { Message = "Shipment line item not found", Status = "Erorr" });
            }

            var statusResult = UpdateChangesOnShipmentLineItemApprove(record);
            if (statusResult.Succeed)
            {
                return Json(new { Message = "Shipment Line item  transfered to store transaction and Bincard Successfully", Status = "OK" });
            }
            else
            {
                //ModelState.AddModelError("", "Something Went Wrong in Processing the Transaction!");
                //return Json(new { Message = "Something Went Wrong in Processing the Transaction", Status = "Erorr" });
                return Json(new { Message = statusResult.ErrorMessage, Status = "Erorr" });

            }
        }
        private CRMTransactionValidationStatusViewModel UpdateChangesOnShipmentLineItemApprove(SalesShipmentLineItem shipmentLineItem)
        {
            CRMTransactionValidationStatusViewModel statusModel = new CRMTransactionValidationStatusViewModel
            {
                Succeed = false,
            };
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            List<BinCardLine> binCardLines = new List<BinCardLine>();

            //bool transactionCompletedSuccessfully = false;

            //var shipmentLineItems = db.SalesShipmentLineItem.Where(li => li.SalesShipmentId == shipmentRecord.SalesShipmentId).ToList();
            //foreach (SalesShipmentLineItem shipmentLineItem in shipmentLineItems)
            //{
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

                //if (shipmentLineItem.ShipedQuantity > storeItem.Balance)
                //{
                //    //finalQuantity = 0;
                //    statusModel.Succeed = false;
                //    statusModel.ErrorMessage = $"No enough quantity To Issue(or ship) for item  {storeItem.ItemCode}. Current balance is {storeItem.Balance} and requested quantity is {shipmentLineItem.ShipedQuantity}";
                //    return statusModel;
                //}
                //else
                //{
                beforeTransaction = storeItem.Balance;
                storeItem.Balance = finalQuantity;

                afterTransaction = finalQuantity;
                TransactionQuantity = shipmentLineItem.ShipedQuantity;

                db.StoreItemAssignments.Attach(storeItem);
                db.Entry(storeItem).State = EntityState.Modified;
                //db.SaveChanges();

                string payMethod = "";
                string customer = "";
                if (shipmentLineItem.SalesShipment.Source == ShipmentSource.SalesInvoice)
                {
                    var invoice = db.CashSalesInvoices.FirstOrDefault(i => i.CashSalesInvoiceID == shipmentLineItem.SalesShipment.CashSalesInvoiceID);
                    if (invoice != null)
                    {
                        customer = invoice.OrganizationCustomer.TradeName;
                        payMethod = invoice.PaymentMethod;
                    }
                }
                else
                {
                    var order = db.SalesOrderVouchers.FirstOrDefault(i => i.SalesOrderVoucherID == shipmentLineItem.SalesShipment.SalesOrderVoucherID);
                    if (order != null)
                    {
                        customer = order.OrganizationCustomer.TradeName;
                        payMethod = order.PaymentMethod;
                    }
                }


                #region update bin card

                #region bin card line
                var uPrice = InventoryHelper.GetItemAveragePrice(product.ItemCode, Store.StoreCode);
                BinCardLine binCardLine = new BinCardLine
                {
                    StoreItemAssignment = storeItem,
                    ReferenceDoc = "Shipment " + shipmentLineItem.SalesShipment.SalesShipmentId.ToString(),
                    Reference = shipmentLineItem.SalesShipment.Source == ShipmentSource.SalesInvoice ? "Invoice " + shipmentLineItem.SalesShipment.CashSalesInvoiceID : "Sales Order " + shipmentLineItem.SalesShipment.SalesOrderVoucherID,
                    Date = shipmentLineItem.SalesShipment.ShipmentDate,
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
                    GLPeriodId = GetGlPeriod(shipmentLineItem.SalesShipment.CRMPeriodId),
                    TransactionType = BinCardTransactionTypes.Issue,
                    StatusType = BinCardTransactionStatus.Regular,

                };
                db.BinCardLines.Add(binCardLine);
                binCardLines.Add(binCardLine);
                // db.SaveChanges();

                #endregion

                #endregion

                //transactionCompletedSuccessfully = true;
                //}
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
            //}
            var result = db.SaveChanges();
            if (result >= 1)
            {
                statusModel.Succeed = true;
                List<int> ids = binCardLines.Select(b => b.BinCardLineID).ToList();
                var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.AutoRecalculateBincardLines(ids));
            }
            return statusModel;
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

                    //if (shipmentLineItem.ShipedQuantity > storeItem.Balance)
                    //{
                    //    //finalQuantity = 0;
                    //    statusModel.Succeed = false;
                    //    statusModel.ErrorMessage = $"No enough quantity To Issue(or ship) for item  {storeItem.ItemCode}. Current balance is {storeItem.Balance} and requested quantity is {shipmentLineItem.ShipedQuantity}";
                    //    return statusModel;
                    //}
                    //else
                    //{
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
                        StoreItemAssignment = storeItem,
                        ReferenceDoc = "Shipment " + shipmentRecord.SalesShipmentId.ToString(),
                        Reference = shipmentRecord.Remark,
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
                    binCardLines.Add(binCardLine);
                    // db.SaveChanges();

                    #endregion

                    #endregion

                    //transactionCompletedSuccessfully = true;
                    //}
                }

                if (storeItem == null && product != null && Store != null)
                {
                    statusModel.Succeed = false;
                    statusModel.ErrorMessage = $"Item {product.ItemCode} not found in store  {Store.StoreCode}";
                    return statusModel;
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
            if (result >= 1)
            {
                statusModel.Succeed = true;
                List<int> ids = binCardLines.Select(b => b.BinCardLineID).ToList();
                var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.AutoRecalculateBincardLines(ids));
            }
            return statusModel;
        }
        private string GetInventoryStoreCode(int salesStoreId)
        {
            return db.SalesStores.Find(salesStoreId)?.StoreCode;
        }

        [Authorize(Users = "tester")]
        public ActionResult UpdateInvoiceShippedStatus()
        {
            var allInvoice = db.CashSalesInvoices.ToList();
            foreach (var invoice in allInvoice)
            {
                if (new CRMSalesHelper().SalesInvoiceHasUnShipedLineLitems(invoice, "2") == false)
                {
                    invoice.IsShipped = true;

                    db.CashSalesInvoices.Attach(invoice);
                    db.Entry(invoice).State = EntityState.Modified;
                }
                else
                {
                    invoice.IsShipped = false;

                    db.CashSalesInvoices.Attach(invoice);
                    db.Entry(invoice).State = EntityState.Modified;
                }
                db.SaveChanges();

            }

            return Redirect(nameof(Index));
        }

        [Authorize(Users = "tester")]
        public ActionResult UpdateSalesOrderShippedStatus()
        {
            var allOrders = db.SalesOrderVouchers.ToList();
            foreach (var order in allOrders)
            {
                if (new CRMSalesHelper().SalesOrderVoucherHasUnShipedLineLitems(order, "1") == false)
                {
                    order.IsShipped = true;

                    db.SalesOrderVouchers.Attach(order);
                    db.Entry(order).State = EntityState.Modified;
                }
                else
                {
                    order.IsShipped = false;

                    db.SalesOrderVouchers.Attach(order);
                    db.Entry(order).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Redirect(nameof(Index));
        }

        //[AuthorizeRoles(CRMRoles.IncompleteShipmentAdmin)]
        //public ActionResult AutoSyncIncompleteShipments()
        //{
        //    #region find missing shipments
        //    var concatnatedRef =
        //         db.SalesShipments.Where(s => s.IsOnlineApproved)
        //         .Select(s => new
        //         {
        //             Id = s.SalesShipmentId,
        //             ConcRef = "Shipment " + s.SalesShipmentId.ToString()
        //         }).ToList();

        //    var binCardFromShipment = new HashSet<string>(db.BinCardLines.Where(b => b.ReferenceDoc.Contains("Shipment")).Select(s => s.ReferenceDoc));
        //    var missingId = new HashSet<int>(concatnatedRef.Where(c => !binCardFromShipment.Contains(c.ConcRef)).ToList().Select(s => s.Id));
        //    var missingShipment = db.SalesShipments.Where(s => s.IsOnlineApproved && missingId.Contains(s.SalesShipmentId)).ToList();
        //    #endregion
        //    #region automatically syc each missing 
        //    foreach (var salesShipment in missingShipment)
        //    {
        //        UpdateChangesOnShipmentApprove(salesShipment);
        //    }
        //    #endregion

        //    return Redirect(nameof(IncompleteShipments));
        //}

        //[AuthorizeRoles(CRMRoles.IncompleteShipmentAdmin)]
        //public ActionResult AutoSyncPartiallyIncompleteShipments()
        //{
        //    #region find partially missing shipments
        //    List<SalesShipment> headerList = new List<SalesShipment>();
        //    var allApprovedShipments = db.SalesShipments.Where(s => s.IsOnlineApproved).ToList();

        //    foreach (var shipment in allApprovedShipments)
        //    {
        //        var lines = db.SalesShipmentLineItem.Where(l => l.SalesShipmentId == shipment.SalesShipmentId).ToList();
        //        var binCardLines = db.BinCardLines.Where(b => b.ReferenceDoc == "Shipment " + shipment.SalesShipmentId).ToList();

        //        // there are missing items
        //        // in case of void, there may be more lines in bincard than in shipment
        //        // lines.Count != binCardLines.Count will fail in that case.
        //        if (lines.Count > binCardLines.Count && binCardLines.Count > 0)
        //        {
        //            headerList.Add(shipment);
        //        }
        //    }
        //    #endregion

        //    #region auto sync missing lines to bin-card
        //    foreach (var shipment in headerList)
        //    {
        //        List<SalesShipmentLineItem> missingLines = new List<SalesShipmentLineItem>();
        //        var allLines = db.SalesShipmentLineItem.Where(itm => itm.SalesShipmentId == shipment.SalesShipmentId).ToList();
        //        var binCardLines = db.BinCardLines.Where(b => b.ReferenceDoc == "Shipment " + shipment.SalesShipmentId).ToList();
        //        if (allLines.Count != binCardLines.Count && binCardLines.Count > 0)
        //        {
        //            foreach (var shipmentLine in allLines)
        //            {
        //                var salesStore = db.SalesStores.Find(shipmentLine.SalesStoreId);

        //                InventoryStore Store =
        //                    db.InventoryStores.FirstOrDefault(
        //                    x => x.StoreCode == salesStore.StoreCode);
        //                Product product =
        //                    db.Products.FirstOrDefault(x => x.ProductID == shipmentLine.ProductId);

        //                StoreItemAssignment storeItem =
        //                    db.StoreItemAssignments
        //                    .Where(x => x.InventoryStore.StoreCode == Store.StoreCode &&
        //                                x.InventoryItem.ItemCode == product.ItemCode)
        //                                .OrderByDescending(x => x.StoreItemAssignmentID).FirstOrDefault();
        //                if (storeItem != null)
        //                {
        //                    var respectiveBinOfCurrentItem =
        //                        binCardLines.FirstOrDefault(b => b.StoreItemAssignmentID == storeItem.StoreItemAssignmentID && b.StoreCode == Store.StoreCode);
        //                    if (respectiveBinOfCurrentItem == null)
        //                    {
        //                        missingLines.Add(shipmentLine);
        //                    }
        //                }

        //                foreach (var missingLine in missingLines)
        //                {
        //                    UpdateChangesOnShipmentLineItemApprove(missingLine);
        //                }
        //            }
        //        }
        //    }
        //    #endregion

        //    return Redirect(nameof(PartiallyIncompleteShipments));
        //}
        [Authorize(Users ="tester")]
        public ActionResult GetDuplicateBinCard()
        {
            
            var duplicateItems = db.Database
                .SqlQuery<DuplicateBincardViewModel>
                (
                    "SELECT ReferenceDoc,StoreItemAssignmentID, Count(ReferenceDoc) as TotalOccurance " +
                    " FROM EXCEED.BinCardLine " +
                    " WHERE ReferenceDoc NOT LIKE 'Count on date%' AND ReferenceDoc  LIKE 'Shipment%'" +
                    " GROUP BY ReferenceDoc,StoreItemAssignmentID " +
                    " HAVING COUNT(ReferenceDoc) > 1").ToList();
            return Json(duplicateItems, JsonRequestBehavior.AllowGet);
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
    }
}