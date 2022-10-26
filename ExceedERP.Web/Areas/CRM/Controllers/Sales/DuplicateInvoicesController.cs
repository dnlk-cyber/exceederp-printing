using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.CRM.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers.Sales
{
    public class DuplicateInvoicesController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: CRM/DuplicateInvoices
        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["VoucherExtension"] = db.VoucherExtensions.ToList();
            ViewData["Thing"] = db.Products.ToList();
            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;
            ViewData["VoucherList"] = db.SalesQuoteVouchers;
            ViewData["CartList"] = db.Carts;
            ViewData["PaymentMethod"] = db.PaymentMethods;
            var termList = db.Terms;
            var lookUpList = db.Lookups.ToList();
            foreach (Term term in termList)
            {
                Lookup selectedCaegory = lookUpList.Where(l => l.LookupId.ToString() == term.LookupId).ToList().FirstOrDefault();
                if (selectedCaegory != null)
                {
                    term.CategoryDescription = selectedCaegory.Description;
                }
            }

            ViewData["TermList"] = termList.AsQueryable();
            ViewData["SerialDefinitionList"] = db.SerialDefinitions;
            ViewData["VoucherReferenceList"] = db.SalesQuoteVouchers;
            ViewData["StoreList"] = db.Stores;
            ViewData["VoucherExtensionList"] = db.VoucherExtensions;
            ViewData["PeriodList"] = db.Periods;
            ViewData["SalesOrderVoucherList"] = db.SalesOrderVouchers.Where(s => s.LastObjectState != CRMConstants.Forwarded && s.PaymentMethod != CRMConstants.CreditPayment);

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();
            ViewData["PaymentTerms"] = db.PaymentTerms.Select(b => new { PaymentTermsID = b.PaymentTermsID, PaymentTermsDays = b.PaymentTermsDays + " Days" }).ToList();

            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesInvoice_Destroy([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            if (ModelState.IsValid)
            {
                var invoice = db.CashSalesInvoices.Find(cashSalesInvoice.CashSalesInvoiceID);
                if (invoice != null)
                {
                    var shipments = db.SalesShipments.Where(s => s.Source == ShipmentSource.SalesInvoice && s.CashSalesInvoiceID == invoice.CashSalesInvoiceID).ToList();
                    if (shipments.Any())
                    {
                        foreach (var shipmentRecord in shipments)
                        {
                            #region void shipment
                            if (shipmentRecord != null && !shipmentRecord.IsOnlineTransferred)
                            {
                                shipmentRecord.Status = Enum.GetName(typeof(OperationStatuses), 10);
                                shipmentRecord.IsOnlineApproved = false;
                                shipmentRecord.IsSendForApproval = false;
                                shipmentRecord.IsVoid = true;
                                shipmentRecord.VoidBy = User.Identity.Name;
                                shipmentRecord.VoidTime = DateTime.Now;
                                db.SalesShipments.Attach(shipmentRecord);
                                db.Entry(shipmentRecord).State = EntityState.Modified;
                                db.SaveChanges();

                                #region upddate stock
                                var shipmentLineItems = db.SalesShipmentLineItem.Where(li => li.SalesShipmentId == shipmentRecord.SalesShipmentId).ToList();
                                foreach (SalesShipmentLineItem shipmentLineItem in shipmentLineItems)
                                {
                                    var salesStore = db.SalesStores.Find(shipmentLineItem.SalesStoreId);
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

                                    var branchItem = db.BranchItemAssignments.Where(x => x.BranchId == shipmentRecord.BranchId).FirstOrDefault(x => x.ItemCode == product.ItemCode);
                                    InventoryItem stockItem = db.InventoryItems.Find(product.ItemCode);

                                    if (Store != null && product != null && storeItem != null)
                                    {
                                        decimal finalQuantity = storeItem.Balance + shipmentLineItem.ShipedQuantity;
                                        storeItem.Balance = finalQuantity;
                                        db.StoreItemAssignments.Attach(storeItem);
                                        db.Entry(storeItem).State = EntityState.Modified;
                                        //db.SaveChanges();

                                        #region update bin card

                                        #region bin card line
                                        var uPrice = InventoryHelper.GetItemAveragePrice(product.ItemCode, Store.StoreCode);
                                        BinCardLine binCardLine = new BinCardLine
                                        {
                                            StoreItemAssignment = storeItem,
                                            ReferenceDoc = "Void Shipment " + shipmentRecord.SalesShipmentId.ToString(),
                                            Reference = shipmentRecord.Source == ShipmentSource.SalesInvoice ? "Invoice " + shipmentRecord.CashSalesInvoiceID : "Sales Order " + shipmentRecord.SalesOrderVoucherID,
                                            Date = shipmentRecord.ShipmentDate,
                                            //IssuedQTY = shipmentLineItem.ShipedQuantity,
                                            //IssuedUPrice = shipmentLineItem.Price,
                                            //IssuedTPrice = shipmentLineItem.TotalAmount,
                                            ReceivedQTY = shipmentLineItem.ShipedQuantity,
                                            ReceivedUPrice = shipmentLineItem.Price,
                                            ReceivedTPrice = shipmentLineItem.TotalAmount,

                                            BalanceQTY = storeItem.Balance,
                                            BalanceUPrice = uPrice,
                                            BalanceTPrice = uPrice * storeItem.Balance,
                                            IssuedTo = "",
                                            //IssuedTo = shipmentRecord.OrganizationCustomerId.ToString(),
                                            //new    
                                            StoreCode = Store.StoreCode,
                                            GLPeriodId = GetGlPeriod(shipmentRecord.CRMPeriodId),
                                            TransactionType = BinCardTransactionTypes.Issue,
                                            StatusType = BinCardTransactionStatus.Void,
                                        };
                                        db.BinCardLines.Add(binCardLine);
                                        #endregion
                                        #endregion
                                    }
                                    if (branchItem != null)
                                    {
                                        branchItem.Balance = branchItem.Balance + shipmentLineItem.ShipedQuantity;
                                        db.Entry(branchItem).State = EntityState.Modified;
                                        //_db.SaveChanges();
                                    }
                                    if (stockItem != null)
                                    {
                                        stockItem.Balance = stockItem.Balance + shipmentLineItem.ShipedQuantity;
                                        db.Entry(stockItem).State = EntityState.Modified;
                                        // _db.SaveChanges();
                                    }
                                }

                                #endregion

                            }
                            #endregion
                        }
                        // save all changes
                        var shipModificationResult = db.SaveChanges();
                        if (shipModificationResult > 0)
                        {
                            #region delete invoice
                            db.CashSalesInvoices.Attach(invoice);
                            db.CashSalesInvoices.Remove(invoice);
                            var result = db.SaveChanges();
                            if (result == 1)
                            {
                                var lineItems = db.LineItems.Where(l => l.VoucherDefinitionID == "2" && l.Voucher == cashSalesInvoice.CashSalesInvoiceID.ToString()).ToList();
                                if (lineItems.Any())
                                {
                                    db.LineItems.RemoveRange(lineItems);
                                    db.SaveChanges();
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        #region delete invoice
                        db.CashSalesInvoices.Attach(invoice);
                        db.CashSalesInvoices.Remove(invoice);
                        var result = db.SaveChanges();
                        if (result == 1)
                        {
                            var lineItems = db.LineItems.Where(l => l.VoucherDefinitionID == "2" && l.Voucher == cashSalesInvoice.CashSalesInvoiceID.ToString()).ToList();
                            if (lineItems.Any())
                            {
                                db.LineItems.RemoveRange(lineItems);
                                db.SaveChanges();
                            }
                        }
                        #endregion
                    }
                }
            }

            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
        }
        
        private static int GetGlPeriod(int crmPeriodId)
        {
            var db = new ExceedDbContext();

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