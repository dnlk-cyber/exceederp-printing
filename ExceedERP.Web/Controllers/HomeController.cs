using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.Inventory;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Web.Hubs;

using System.IO;
using System.Net.Mime;
using System.Xml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Helpers.Construction;
using ExceedERP.Helpers.Procurement;
using InventoryHelper = ExceedERP.Helpers.Inventory.InventoryHelper;
using Hangfire;
using ExceedERP.Core.Domain.Finance.AR;

namespace ExceedERP.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly DashboardHelper _dashboardHelper = new DashboardHelper();
        private readonly AccountHelper _accountHelper = new AccountHelper();
        private  readonly FinanceHelper _financeHelper = new FinanceHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);

            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {

                cookie = new HttpCookie("_culture")
                {
                    Value = culture,
                    Expires = DateTime.Now.AddYears(1)
                };
            }
            Response.Cookies.Add(cookie);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult IssueBr()
        {
            var issues = _db.Issues.ToList();
            foreach (var issue in issues)
            {
                var req = _db.StoreRequisitions.Where(x => x.StoreRequisitionID == issue.StoreRequisitionID).FirstOrDefault();

                if (req != null && req.IsForOther)
                {
                    issue.ForBranchId = (int)req.ForBranchId;

                    _db.Issues.Attach(issue);
                    _db.Entry(issue).State = EntityState.Modified;
                    _db.SaveChanges();
                }

            }
            return View("Success");
        }
        public ActionResult StanLine()
        {
            var lines = _db.ARStandardCollectionLines.Where(x=>x.BranchId==0).ToList();
            foreach(var line in lines)
            {
                string invoicetype = line.InvoiceNumber.Split('_')[0];
                string recordid = line.InvoiceNumber.Split('_')[1];
                int did = int.Parse(recordid);
                if (line.Source== InvoiceSource.Invoice)
                {
                    var invoice = _db.ARInvoices.Find(did);   
                    if(invoice!=null)
                      line.BranchId = invoice.BranchId;
                }
                else if (line.Source == InvoiceSource.SalesOrder)
                {
                    var sales = _db.ArSalesOrders.Find(did);
                    if (sales != null)
                        line.BranchId = sales.BranchId;
                }

                _db.ARStandardCollectionLines.Attach(line);
                _db.Entry(line).State = EntityState.Modified;
                _db.SaveChanges();

            }
            return View("Success");
        }
        public ActionResult DupAcc()
        {
            _financeHelper.DetectAndRemoveDuplicateLedgerAccountCode();
            return View("Success");
        }

        public ActionResult DupLegLine()
        {
            _financeHelper.DetectAndRemoveDuplicateLedgerLines();
            return View("Success");
        }
        public ActionResult TransferSend()
        {
            #region get ids of transfer send

            var transferSends = _db.BinCardLines
                .Where(x => x.ReferenceDoc.StartsWith("Transfer Send"))
                .Where(x=>x.StoreCode==null ||x.StoreCode=="")
                .ToList();

            foreach (var i in transferSends)
            {
                string id = i.ReferenceDoc.Split(' ')[2];
                int did = int.Parse(id);

                var itemTransfer = _db.ItemTransfers.Find(did);
                if (itemTransfer != null)
                {
                    i.StoreCode = itemTransfer.ToStore;
                    _db.Entry(i).State = EntityState.Modified;
                    _db.SaveChanges();
                }

            }

            #endregion
            return View("Success");
        }

        public ActionResult PayReq()
        {
            #region update old pay req

            var sentRequestIds = _db.RequestesStatus
               .Where(x => x.ReferenceType == "Payment")
               .Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL)
               .Select(x => x.RequestId).ToList();

            foreach (var i in sentRequestIds)
            {
                var paymentRequest = _db.PaymentRequests.Find(i);
                if (paymentRequest != null)
                {

                    paymentRequest.IsDepartmentOnlineApproved = true;
                    paymentRequest.IsDepartmentSendForApproval = true;

                    _db.PaymentRequests.Attach(paymentRequest);
                    _db.Entry(paymentRequest).State = EntityState.Modified;
                    _db.SaveChanges();
                }

            }

            #endregion
            return View("Success");
        }

        public ActionResult TransferReceive()
        {
            #region get ids of transfer receive

            var transferSends = _db.BinCardLines
                .Where(x => x.ReferenceDoc.StartsWith("Transfer Receive_"))
                .Where(x => x.StoreCode == null || x.StoreCode == "")
                .ToList();

            foreach (var i in transferSends)
            {
                string id = i.ReferenceDoc.Split('_')[1];
                int did = int.Parse(id);

                var itemTransfer = _db.ItemTransferReceive.Find(did);
                if (itemTransfer != null)
                {
                    i.StoreCode = itemTransfer.FromStore;
                    _db.Entry(i).State = EntityState.Modified;
                    _db.SaveChanges();
                }

            }

            #endregion
            return View("Success");
        }
        public ActionResult ReturnPrice(int id)
        {
            #region duplicate ledger line

            var returnIds = _db.StoreReturns
                .Where(x => x.StoreReturnID >=id)
                .ToList();

            foreach (var i in returnIds)
            {
                var items = _db.StoreReturnItemss
                    .Where(x => x.StoreReturnID==i.StoreReturnID).ToList();

                foreach (var item in items)
                {
                    var storeItem = _db.StoreItemAssignments
                        .Where(x => x.StoreCode == i.StoreCode).FirstOrDefault(x => x.ItemCode == item.ItemCode);
                    if (storeItem != null)
                    {
                        var bincard = _db.BinCardLines
                            .Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID).FirstOrDefault(x => x.ReferenceDoc == "Return_" + i.StoreReturnID.ToString());

                        if (bincard != null) item.UnitPrice = bincard.ReceivedUPrice;
                        _db.Entry(item).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
            }

            #endregion
            return View("Success");
        }

        public ActionResult SumBeg()
        {
            #region duplicate ledger line

            var duplicateLines = _db.GLLedgerPostingLines
                       .Where(x => x.Type == LedgerPostTypes.Begining)
                       .Where(x=>x.GLPeriodId==13)
                       .GroupBy(x => x.GLLedgerPostingID)
                       .Where(g => g.Count() > 1)
                       .Select(y => new { Element = y.Key, Counter = y.Count() })
                       .ToList();

            foreach (var duplicate in duplicateLines)
            {
                var duplicateledgerlines = _db.GLLedgerPostingLines
                      .Where(x => x.Type == LedgerPostTypes.Begining)
                      .Where(x => x.GLPeriodId == 13)
                      .Where(x => x.GLLedgerPostingID == duplicate.Element).ToList();
                //update first beg
                var minline = duplicateledgerlines.Min(x => x.GLLedgerPostingLinesID);
                var minelinObj = _db.GLLedgerPostingLines.Find(minline);
                minelinObj.Debit = duplicateledgerlines.Sum(x => x.Debit);
                minelinObj.Credit = duplicateledgerlines.Sum(x => x.Credit);
                _db.Entry(minelinObj).State = EntityState.Modified;
                _db.SaveChanges();
                //remove remaining
                var toberemoved = duplicateledgerlines.Where(x => x.GLLedgerPostingLinesID != minline).ToList();
                foreach(var rem in toberemoved)
                {
                    rem.Type = LedgerPostTypes.NA;
                    _db.Entry(rem).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                //_db.GLLedgerPostingLines.RemoveRange(toberemoved);
                //_db.SaveChanges();
                
            }

            #endregion
            return View("Success");
        }

        public ActionResult ClosePaymentRequestStatus()
        {
            FinanceTransactionHelper.ClosePaymentRequestStatus();
            return View("Success");
        }

        public ActionResult PoPaymentStatus()
        {
            ProcurementTransactionHelper.ClosePoPaymentStatus();
            return View("Success");
        }

        public ActionResult ContractPaymentStatus()
        {
            ProcurementTransactionHelper.CloseContractPaymentStatus();
            return View("Success");
        }

        public ActionResult ArBranch()
        {
            #region branch ar

            var arinvoiceJournals = _db.GLRecordJournals.Where(x => x.JournalNumber.StartsWith("ARINV.")).ToList();

            foreach (var journal in arinvoiceJournals)
            {
                int id = int.Parse(journal.JournalNumber.Split('.')[1]);
                var record = _db.ARInvoices.Find(id);

                if (record != null) journal.BranchId = record.BranchId;
                _db.Entry(journal).State = EntityState.Modified;
                _db.SaveChanges();

            }

            var armisccollectionJournals = _db.GLRecordJournals.Where(x => x.JournalNumber.StartsWith("ARSMC.")).ToList();

            foreach (var journal in armisccollectionJournals)
            {
                int id = int.Parse(journal.JournalNumber.Split('.')[1]);
                var record = _db.ARMiscelaneousCollections.Find(id);

                if (record != null) journal.BranchId = record.BranchId;
                _db.Entry(journal).State = EntityState.Modified;
                _db.SaveChanges();

            }

            var arstandardcollectionJournals = _db.GLRecordJournals.Where(x => x.JournalNumber.StartsWith("ARSTC.")).ToList();

            foreach (var journal in arstandardcollectionJournals)
            {
                int id = int.Parse(journal.JournalNumber.Split('.')[1]);
                var record = _db.ARStandardCollections.Find(id);

                var collectionline = _db.ARStandardCollectionLines
                    .FirstOrDefault(x => x.ArStandardCollectionId == record.ArStandardCollectionId);

                if (collectionline != null && collectionline.InvoiceNumber.StartsWith("SALESORDER_"))
                {
                    int salesid = int.Parse(journal.JournalNumber.Split('_')[1]);
                    var record2 = _db.ArSalesOrders.Find(salesid);
                    if (record2 != null) journal.BranchId = record2.BranchId;
                    _db.Entry(journal).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else if (collectionline != null && collectionline.InvoiceNumber.StartsWith("INVOICE_"))
                {
                    int salesid = int.Parse(journal.JournalNumber.Split('_')[1]);
                    var record1 = _db.ARInvoices.Find(salesid);
                    if (record1 != null) journal.BranchId = record1.BranchId;
                    _db.Entry(journal).State = EntityState.Modified;
                    _db.SaveChanges();
                }

            }

            var arssalesorderJournals = _db.GLRecordJournals.Where(x => x.JournalNumber.StartsWith("ARSALORD.")).ToList();

            foreach (var journal in arssalesorderJournals)
            {
                int id = int.Parse(journal.JournalNumber.Split('.')[1]);
                var record = _db.ArSalesOrders.Find(id);

                journal.BranchId = record.BranchId;
                _db.Entry(journal).State = EntityState.Modified;
                _db.SaveChanges();
            }

            #endregion
            return View("Success");
        }
        public ActionResult ProcBranch()
        {
            #region

            var purchaseOrders = _db.PurchaseOrders.Where(x=>x.BranchId==0).ToList();

            foreach (var purchaseOrder in purchaseOrders)
            {
               
                purchaseOrder.BranchId = EmployeeHelper.GetEmployeePlacementBranch(purchaseOrder.ProcurementOfficer);
                _db.Entry(purchaseOrder).State = EntityState.Modified;
                _db.SaveChanges();

            }

            var proformaPurchases = _db.ProformaPurchases.Where(x => x.BranchId == 0).ToList();

            foreach (var proformaPurchase in proformaPurchases)
            {

                proformaPurchase.BranchId = EmployeeHelper.GetEmployeePlacementBranch(proformaPurchase.ProcurementOfficer);
                _db.Entry(proformaPurchase).State = EntityState.Modified;
                _db.SaveChanges();

            }

            var tenders = _db.Tenders.Where(x => x.BranchId == 0).ToList();

            foreach (var tender in tenders)
            {

                tender.BranchId = EmployeeHelper.GetEmployeePlacementBranch(tender.ProcurementOfficer);
                _db.Entry(tender).State = EntityState.Modified;
                _db.SaveChanges();

            }

            var contractManagements = _db.ContractManagements.Where(x => x.BranchId == 0).ToList();

            foreach (var contractManagement in contractManagements)
            {

                contractManagement.BranchId = EmployeeHelper.GetEmployeePlacementBranch(contractManagement.ProcurementOfficer);
                _db.Entry(contractManagement).State = EntityState.Modified;
                _db.SaveChanges();

            }

            #endregion
            return View("Success");
        }
        public ActionResult Award()
        {
            #region

            var awardedSuppliers = _db.BidAwardedSuppliers.ToList();

            foreach (var awardedSupplier in awardedSuppliers)
            {
                var lot = _db.Lots.Find(awardedSupplier.LotID);
                awardedSupplier.TenderReference = lot?.Tender.TenderReference;
                _db.Entry(awardedSupplier).State = EntityState.Modified;
                _db.SaveChanges();

                var awardItems = _db.BidAwardedSupplierItems
                    .Where(x => x.BidAwardedSupplierID == awardedSupplier.BidAwardedSupplierID).ToList();
                foreach (var awardItem in awardItems)
                {
                    awardItem.FinalUPrice = awardItem.UPrice - awardItem.Discount;
                    var tax = _db.GLTaxRates.Find(awardItem.GLTaxRateID);
                    if (tax != null)
                    {
                        awardItem.TaxAmount = awardItem.FinalUPrice * awardItem.Quanity * tax.CalculatedRate;
                        awardItem.TPrice = awardItem.FinalUPrice * awardItem.Quanity +
                                                        (awardItem.FinalUPrice * awardItem.Quanity *
                                                         tax.CalculatedRate);
                    }

                    _db.Entry(awardItem).State = EntityState.Modified;
                    _db.SaveChanges();
                }

            }

            #endregion
            return View("Success");
        }

        public ActionResult DebitSum()
        {
            #region

            var journals = _db.GLRecordJournals.AsNoTracking().Select(x => x.GlRecordJournalId).ToList();

            foreach (var journalId in journals)
            {
                var lines = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == journalId);
                if (lines.Any())
                {
                    var journal = _db.GLRecordJournals.Find(journalId);
                    journal.DebitSum = lines.Sum(x => x.Debit);
                    _db.GLRecordJournals.Attach(journal);
                    _db.Entry(journal).State = EntityState.Modified;
                    _db.SaveChanges();
                }

            }

            #endregion
            return View("Success");
        }
        public ActionResult PlateNo()
        {
            #region

            var purchaseRequisitions = _db.PurchaseRequisitions
                .Where(x => x.ItemType == PurchaseItemType.Stock)
                .ToList();

            foreach (var line in purchaseRequisitions)
            {
              
               
                var record = _db.StoreRequisitions.Find(line.StoreRequisitionID);
                line.PlateNo = record?.PlateNo;
                _db.Entry(line).State = EntityState.Modified;
                _db.SaveChanges();

            }

            #endregion
            return View("Success");
        }
        public ActionResult PriceIndex()
        {
            #region

            var itemPriceIndices = _db.ItemPriceIndexes
                .ToList();

            foreach (var line in itemPriceIndices)
            {
                line.ItemName = _db.InventoryItems.Find(line.ItemCode)?.Name;
                _db.Entry(line).State = EntityState.Modified;
                _db.SaveChanges();

            }

            #endregion
            return View("Success");
        }

        public ActionResult PaymentReqUpdate()
        {
            var payments = _db.PaymentRequests.Where(x=>x.IsOnlineApproved&&x.IsVoid==false&&x.Paid==false)
                .AsNoTracking().ToList();

            foreach (var payment in payments)
            {
                string source = "PAYMENT-REQUEST_" + payment.PaymentRequestId.ToString();

                var directPayment = _db.ApDirectPayments.Where(x => x.SourceId == source)
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false).FirstOrDefault();

                if (directPayment != null)
                {
                    #region update paid status

                    payment.Paid = true;
                    _db.Entry(payment).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion

                }

            }

            return View("Success");
        }
        public ActionResult Update()
        {
            #region
            //collection
            var collections = _db.ARStandardCollections
                .AsNoTracking().ToList();

            foreach (var collection in collections)
            {

                if (collection != null)
                {
                    #region update parent balances
                    var lines = _db.ARStandardCollectionLines.Where(x => x.ArStandardCollectionId == collection.ArStandardCollectionId).ToList();
                    collection.PrepaymentBalance = lines.Where(x => x.IsPrepayment).Sum(x => x.ApplyAmount);
                    collection.TaxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                    collection.TotalAmount = Math.Round(lines.Sum(x => x.ApplyAmount), 2);
                    collection.LineAmount = collection.TotalAmount - collection.TaxAmount;
                    decimal totalWithInvoice = lines.Where(x => x.IsPrepayment == false).Where(x => x.InvoiceNumber != null).Where(x => x.InvoiceNumber != "").Sum(x => x.ApplyAmount);
                    decimal totalWithoutInvoice = lines.Where(x => x.IsPrepayment == false).Where(x => string.IsNullOrEmpty(x.InvoiceNumber)).Sum(x => x.ApplyAmount);

                    if (String.IsNullOrEmpty(collection.CustomerName))
                    {
                        collection.UnidentifiedBalance = totalWithoutInvoice;
                    }
                    else
                    {
                        collection.UnappliedBalance = totalWithoutInvoice;
                        collection.AppliedBalance = totalWithInvoice;
                    }
                    _db.Entry(collection).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion

                }

            }
            //misc collection
            var miscelaneousCollections = _db.ARMiscelaneousCollections
                .AsNoTracking().ToList();

            foreach (var collection in miscelaneousCollections)
            {

                if (collection != null)
                {
                    #region update parent balances
                    var lines = _db.ArMiscelaneousCollectionLines.Where(x => x.ARMiscelaneousCollectionID == collection.ARMiscelaneousCollectionID).ToList();
                    collection.LineAmount = Math.Round(lines.Sum(x => x.Amount), 2);
                    collection.TaxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                    collection.TotalAmount = Math.Round(lines.Sum(x => x.TotalAmount), 2);
                    _db.Entry(collection).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion

                }

            }
            //misc payment
            var miscellaneousPayments = _db.APMiscellaneousPayments
                .AsNoTracking().ToList();

            foreach (var payment in miscellaneousPayments)
            {

                if (payment != null)
                {
                    #region update parent balances
                    var lines = _db.ApMiscellaneousPaymentLines.Where(x => x.ApMiscellaneousPaymentId == payment.ApMiscellaneousPaymentId).ToList();
                    payment.LineAmount = Math.Round(lines.Sum(x => x.Amount), 2);
                    payment.TaxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                    payment.TotalAmount = Math.Round(lines.Sum(x => x.TotalAmount), 2);
                    _db.Entry(payment).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion

                }

            }

            #endregion
            return View("Success");
        }

        public ActionResult UpdateM()
        {
            var payments = _db.APMiscellaneousPayments
                .AsNoTracking().ToList();

            foreach (var payment in payments)
            {

                if (payment != null)
                {
                    #region update parent balances
                    var lines = _db.ApMiscellaneousPaymentLines.Where(x => x.ApMiscellaneousPaymentId == payment.ApMiscellaneousPaymentId).ToList();
                    payment.LineAmount = Math.Round(lines.Sum(x => x.Amount), 2);
                    payment.TaxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                    payment.TotalAmount = Math.Round(lines.Sum(x => x.TotalAmount), 2);

                    _db.Entry(payment).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion

                }

            }
           
            return View("Success");
        }
        public ActionResult LedgerRef()
        {
            #region

            var glLedgerPostingLines = _db.GLLedgerPostingLines
                .Where(x => x.Type == LedgerPostTypes.Standard)
                .Where(x => x.Remark == null ||x.PaymentReference==null)
                .ToList();

            foreach (var line in glLedgerPostingLines)
            {
                try
                {
                    string[] str = line.JournalNumber.Split('-');
                    int id = int.Parse(str[0]);
                    var record = _db.GLRecordJournals.Find(id);
                    line.Remark = record?.Remark;
                    line.ReferenceDoc = record?.ReferenceDoc;
                    line.PaymentReference = record?.PaymentReference;
                    line.InvoiceReferenceDoc = record?.InvoiceReferenceDoc;
                    _db.Entry(line).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logger.Debug(line.JournalNumber + ":" + e.Message);
                }

            }

            #endregion
            return View("Success");
        }
        public ActionResult JournalRef()
        {
            #region 

            var recordJournals = _db.GLRecordJournals.Where(x => x.Remark==null || x.PaymentReference == null).ToList();
            foreach (var journal in recordJournals)
            {
                try
                {
                    string [] str = journal.JournalNumber.Split('.');
                    int id = int.Parse(str[1]);
                    switch (str[0])
                    {
                        case "ARINV":
                            var record = _db.ARInvoices.Find(id);
                            journal.Remark = record?.Reference;
                            journal.ReferenceDoc = record?.Reference;
                            journal.PaymentReference = "";
                            journal.InvoiceReferenceDoc = "";
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "ARSMC":
                            var record1 = _db.ARMiscelaneousCollections.Find(id);
                            journal.Remark = record1?.ReceiptNumber;
                            journal.ReferenceDoc = "";
                            journal.PaymentReference = record1?.ReceiptNumber;
                            journal.InvoiceReferenceDoc = "";
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "ARSTC":
                            var record2 = _db.ARStandardCollections.Find(id);
                            var invoice = _db.ARStandardCollectionLines.Where(x => x.ArStandardCollectionId == id)
                                .Select(x => x.InvoiceNumber).ToList();
                            string invoices = "";
                            foreach (var i in invoice)
                            {
                                invoices = invoices + "," + i;
                            }
                            journal.Remark = record2?.Reference;
                            journal.ReferenceDoc = record2?.Reference;
                            journal.PaymentReference = record2?.ReceiptNumber;
                            journal.InvoiceReferenceDoc = invoices;
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "ARSALORD":
                            var record3 = _db.ArSalesOrders.Find(id);
                            journal.Remark = record3?.OrderReference;
                            journal.ReferenceDoc = record3?.OrderReference;
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "APDPAY":
                            var record4 = _db.ApDirectPayments.Find(id);
                            journal.Remark = record4?.PaymentReference;
                            journal.PaymentReference = record4?.PaymentReference;
                            journal.ReferenceDoc ="";
                            journal.InvoiceReferenceDoc = "";
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "APINV":
                            var record5 = _db.APPaybleInvoices.Find(id);
                            journal.Remark = record5?.Remark;
                            journal.PaymentReference = "";
                            journal.ReferenceDoc = "";
                            journal.InvoiceReferenceDoc = record5?.InvoiceNumber;
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "APMPAY":
                            var record6 = _db.APMiscellaneousPayments.Find(id);
                            journal.Remark = record6?.Remark;
                            journal.PaymentReference = record6?.PaymentReference;
                            journal.ReferenceDoc = "";
                            journal.InvoiceReferenceDoc ="";
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "APPAY":
                            var record7 = _db.APPayments.Find(id);
                            journal.Remark = record7?.Remark;
                            journal.PaymentReference = record7?.PaymentReference;
                            journal.ReferenceDoc = "";
                            journal.InvoiceReferenceDoc = record7.InvoiceNumber;
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "APEXPCLA":
                            var record8 = _db.ApTravelExpenseClaims.Find(id);
                            journal.Remark = record8?.Remark;
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                        case "APTRAVORD":
                            var record9 = _db.ApTravelOrders.Find(id);
                            journal.Remark = record9?.Remark;
                            _db.Entry(journal).State = EntityState.Modified;
                            _db.SaveChanges();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Debug(journal.JournalNumber+":"+e.Message);
                }
                
            }

            #endregion
            return View("Success");
        }
        private void  DefaultLanguage()
        {
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = "am";   // update cookie value
            else
            {

                cookie = new HttpCookie("_culture")
                {
                    Value = "am",
                    Expires = DateTime.Now.AddYears(1)
                };
            }
            Response.Cookies.Add(cookie);
        }
        public ActionResult CreateBeginning(string id)
        {
            InventoryHelper.CreateBeginning(id);
            return View("Success");
        }
       
        public ActionResult TransferIssue()
        {
            //var issues = _db.Issues.Where(x => x.IsOnlineApproved && x.IsVoid == false && x.IsFuel ==false).ToList();
            //foreach (var issue in issues)
            //{
            //    InventoryTransferHelper.Issue(issue.IssueID, "auto", "tester");
            //}
           
            return View("Success");
        }
    //public ActionResult UpdateReferences()
    //{
    //    #region issue

    //    var issues = _db.Issues.ToList();
    //    foreach (var c in issues)
    //    {
    //        string reff = "Issue " + c.IssueID.ToString();
    //        var card = _db.BinCardLines.FirstOrDefault(x => x.ReferenceDoc == reff);
    //        if (card != null)
    //        {
    //            //card.Reference = c.Remark;
    //            card.Date = c.IssueDate;
    //            _db.Entry(card).State = EntityState.Modified;
    //            _db.SaveChanges();
    //        }

    //    }

    //    #endregion
    //    #region receive
    //    var receives = _db.GoodReceives.ToList();
    //    foreach (var c in receives)
    //    {
    //        string reff = "Receive_" + c.GoodReceiveId.ToString();
    //        var card = _db.BinCardLines.FirstOrDefault(x => x.ReferenceDoc == reff);
    //        if (card != null)
    //        {
    //            //card.Reference = c.ReceiveRef;
    //            card.Date = c.ReceiveDate;
    //            _db.Entry(card).State = EntityState.Modified;
    //            _db.SaveChanges();
    //        }

    //    }
    //    #endregion
    //    #region return
    //    var returns = _db.StoreReturns.ToList();
    //    foreach (var c in returns)
    //    {
    //        string reff = "Return_" + c.StoreReturnID.ToString();
    //        var card = _db.BinCardLines.FirstOrDefault(x => x.ReferenceDoc == reff);
    //        if (card != null)
    //        {
    //            //card.Reference = c.Remark;
    //            card.Date = c.ReturnDate;
    //            _db.Entry(card).State = EntityState.Modified;
    //            _db.SaveChanges();
    //        }

    //    }
    //    #endregion
    //    #region transfer send
    //    var transfers = _db.ItemTransfers.ToList();
    //    foreach (var c in transfers)
    //    {
    //        string reff = "Transfer Send " + c.ItemTransferID;
    //        var card = _db.BinCardLines.FirstOrDefault(x => x.ReferenceDoc == reff);
    //        if (card != null)
    //        {
    //            // card.Reference = c.DocumentRef;
    //            card.Date = c.TransferDate;
    //            _db.Entry(card).State = EntityState.Modified;
    //            _db.SaveChanges();
    //        }

    //    }
    //    #endregion
    //    #region transfer receive
    //    var transferreceive = _db.ItemTransfers.ToList();
    //    foreach (var c in transferreceive)
    //    {
    //        string reff = "Transfer Receive_" + c.ItemTransferID.ToString();
    //        var card = _db.BinCardLines.FirstOrDefault(x => x.ReferenceDoc == reff);
    //        if (card != null)
    //        {
    //            //card.Reference = c.Remark;

    //            card.Date = (DateTime)c.TransferDate;
    //            _db.Entry(card).State = EntityState.Modified;
    //            _db.SaveChanges();
    //        }

    //    }
    //    #endregion

    //    return View("Success");
    //}
    //public ActionResult UpdateCount()
    //{
    //    var count = _db.PropertyCounts.ToList();
    //    foreach (var c in count)
    //    {
    //        var itmeList = _db.StoreItemAssignments.Where(x => x.StoreCode == c.StoreCode).ToList();
    //        foreach (var item in itmeList)
    //        {
    //            var exist =
    //                _db.PropertyCountItems.Where(x => x.ItemCode == item.ItemCode)
    //                    .FirstOrDefault(x => x.PropertyCountId == c.PropertyCountId);
    //            if (exist == null)
    //            {
    //                var propertyCountItem = new PropertyCountItem
    //                {
    //                    ItemCategoryCode = item.ItemCategoryCode,
    //                    ItemCode = item.ItemCode,
    //                    PropertyCountId = c.PropertyCountId,
    //                    Count = 0,
    //                    PreviousBalance = item.Balance,
    //                    Price = InventoryHelper.GetItemAveragePrice(item.ItemCode, c.StoreCode),
    //                    Status = false
    //                };


    //                _db.PropertyCountItems.Add(propertyCountItem);
    //                _db.SaveChanges();
    //            }
    //        }
    //    }

    //    return View("Success");
    //}
    //public ActionResult UpdateSb()
    //{
    //    var storeItemAssignments = _db.StoreItemAssignments.ToList();
    //    if (storeItemAssignments.Any())
    //    {
    //        foreach (var a in storeItemAssignments)
    //        {
    //            int maxid = a.BinCardLines.Max(x => x.BinCardLineID);
    //            var bincard = _db.BinCardLines.Find(maxid);
    //            if (bincard != null)
    //            {
    //                a.Balance = bincard.BalanceQTY;
    //                _db.Entry(a).State = EntityState.Modified;
    //                _db.SaveChanges();
    //            }

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult UpdateGrn()
    //{
    //    var goodReceives = _db.GoodReceives.ToList();
    //    if (goodReceives.Any())
    //    {
    //        foreach (var a in goodReceives)
    //        {
    //            a.OrgBranchName = a.BranchId.ToString();
    //            _db.Entry(a).State = EntityState.Modified;
    //            _db.SaveChanges();

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult Updatetransfer()
    //{
    //    var itemTransfers = _db.ItemTransfers.ToList();
    //    if (itemTransfers.Any())
    //    {
    //        foreach (var a in itemTransfers)
    //        {
    //            a.OrgBranchName = InventoryHelper.GetStoreBranch(a.FromStore).ToString();
    //            _db.Entry(a).State = EntityState.Modified;
    //            _db.SaveChanges();

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult Updateissue()
    //{
    //    DateTime dt1 = new DateTime(2017, 5, 1);
    //    DateTime dt2 = new DateTime(2017, 6, 30);
    //    var issues = _db.Issues.ToList();
    //    if (issues.Any())
    //    {
    //        foreach (var a in issues)
    //        {
    //            if ((a.IssueDate < dt1) || (a.IssueDate > dt2))
    //            {
    //                var storeRe = _db.StoreRequisitions.Find(a.StoreRequisitionID);
    //                if (storeRe != null)
    //                    a.IssueDate = storeRe.RequestDate;
    //            }
    //            a.OrgBranchName = InventoryHelper.GetStoreBranch(a.StoreCode).ToString();
    //            _db.Entry(a).State = EntityState.Modified;
    //            _db.SaveChanges();

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult Updatesr()
    //{
    //    var storeRequisitions = _db.StoreRequisitions.ToList();
    //    if (storeRequisitions.Any())
    //    {
    //        foreach (var a in storeRequisitions)
    //        {
    //            a.OrgBranchName = a.BranchId.ToString();
    //            _db.Entry(a).State = EntityState.Modified;
    //            _db.SaveChanges();

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult UpdateReturn()
    //{
    //    var returns = _db.StoreReturns.ToList();
    //    if (returns.Any())
    //    {
    //        foreach (var a in returns)
    //        {
    //            a.OrgBranchName = InventoryHelper.GetStoreBranch(a.StoreCode).ToString();
    //            _db.Entry(a).State = EntityState.Modified;
    //            _db.SaveChanges();

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult Updatepr()
    //{
    //    var purchaseRequisitions = _db.PurchaseRequisitions.ToList();
    //    if (purchaseRequisitions.Any())
    //    {
    //        foreach (var a in purchaseRequisitions)
    //        {
    //            a.OrgBranchName = a.BranchId.ToString();
    //            _db.Entry(a).State = EntityState.Modified;
    //            _db.SaveChanges();

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult Asset()
    //{
    //    var asset = _db.AssetRegistrations.ToList();
    //    if (asset.Any())
    //    {
    //        foreach (var a in asset)
    //        {
    //            var entity = new LocationTracking
    //            {
    //                AssetRegistrationId = a.AssetRegistrationId,
    //                BranchId = int.Parse(a.OrgBranchName),
    //                LocationType = LocationType.NotSet,
    //                StartDate = DateTime.Today,
    //                Description = "BEGINNING",
    //                IsCurrent = true
    //            };

    //            _db.LocationTrackings.Add(entity);
    //            _db.SaveChanges();

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult UpdateIssueDate()
    //{
    //    var issues = _db.Issues.ToList();
    //    if (issues.Any())
    //    {
    //        foreach (var a in issues)
    //        {
    //            var sr = _db.StoreRequisitions.Find(a.StoreRequisitionID);
    //            if (sr != null)
    //            {
    //                a.IssueDate = sr.RequestDate;
    //                _db.Entry(a).State = EntityState.Modified;
    //                _db.SaveChanges();
    //            }

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult UpdateReturnBinCard()
    //{
    //    var returns = _db.StoreReturns.ToList();
    //    if (returns.Any())
    //    {
    //        foreach (var a in returns)
    //        {
    //            foreach (var item in a.StoreReturnItems)
    //            {
    //                #region Store Item Balance

    //                var storeItem = _db.StoreItemAssignments.Where(x => x.StoreCode == a.StoreCode).FirstOrDefault(it => it.ItemCode == item.ItemCode);
    //                if (storeItem != null)
    //                {
    //                    string reff = "Return_" + a.StoreReturnID;
    //                    var exist =
    //                        _db.BinCardLines.Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID)
    //                            .Where(x => x.ReferenceDoc == reff)
    //                            .ToList();
    //                    if (!exist.Any())
    //                    {
    //                        #region bin card line

    //                        BinCardLine binCardLine = new BinCardLine
    //                        {
    //                            StoreItemAssignment = storeItem,
    //                            ReferenceDoc = reff,
    //                            Date = a.ReturnDate,
    //                            ReceivedQTY = item.Quantity,
    //                            ReceivedUPrice = item.UnitPrice,
    //                            ReceivedTPrice = item.UnitPrice * item.Quantity,
    //                            //BalanceQTY = storeItem.Balance,
    //                            //BalanceUPrice = item.UnitPrice,
    //                            //BalanceTPrice = item.UnitPrice * storeItem.Balance
    //                        };

    //                        _db.BinCardLines.Add(binCardLine);
    //                        _db.SaveChanges();

    //                        #endregion
    //                    }
    //                }


    //                #endregion
    //            }

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult UpdateTransferSendBinCard()
    //{
    //    var itemTransfers = _db.ItemTransfers.ToList();
    //    if (itemTransfers.Any())
    //    {
    //        foreach (var a in itemTransfers)
    //        {
    //            foreach (var item in a.ItemTransferItem)
    //            {
    //                #region Store Item Balance

    //                var storeItem = _db.StoreItemAssignments.Where(x => x.StoreCode == a.FromStore).FirstOrDefault(it => it.ItemCode == item.ItemCode);
    //                if (storeItem != null)
    //                {
    //                    string reff = "Transfer Send " + a.ItemTransferID;
    //                    var exist =
    //                        _db.BinCardLines.Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID)
    //                            .Where(x => x.ReferenceDoc == reff)
    //                            .ToList();
    //                    if (!exist.Any())
    //                    {
    //                        #region bin card line

    //                        BinCardLine binCardLine = new BinCardLine
    //                        {
    //                            StoreItemAssignment = storeItem,
    //                            ReferenceDoc = reff,
    //                            Date = a.TransferDate,
    //                            IssuedQTY = item.ShippedQuantity,
    //                            IssuedUPrice = item.UPrice,
    //                            IssuedTPrice = item.TPrice,
    //                            //BalanceQTY = storeItem.Balance,
    //                            //BalanceUPrice = item.UPrice,
    //                            //BalanceTPrice = item.UPrice * storeItem.Balance
    //                        };

    //                        _db.BinCardLines.Add(binCardLine);
    //                        _db.SaveChanges();

    //                        #endregion
    //                    }
    //                }


    //                #endregion
    //            }

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult UpdateTransferReceiveRemoveBinCard()
    //{
    //    var itemTransfers = _db.ItemTransfers.ToList();
    //    if (itemTransfers.Any())
    //    {
    //        foreach (var a in itemTransfers)
    //        {
    //            foreach (var item in a.ItemTransferItem)
    //            {
    //                #region Store Item Balance

    //                var storeItem = _db.StoreItemAssignments.Where(x => x.StoreCode == a.FromStore).FirstOrDefault(it => it.ItemCode == item.ItemCode);
    //                if (storeItem != null)
    //                {
    //                    string reff = "Transfer Receive_" + a.ItemTransferID;
    //                    var exist =
    //                        _db.BinCardLines.Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID)
    //                            .Where(x => x.ReferenceDoc == reff)
    //                            .ToList();
    //                    if (exist.Any())
    //                    {
    //                        #region remove

    //                        _db.BinCardLines.RemoveRange(exist);
    //                        _db.SaveChanges();

    //                        #endregion
    //                    }
    //                }


    //                #endregion
    //            }

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult UpdateTransferReceiveBinCard()
    //{
    //    var itemTransfers = _db.ItemTransfers.ToList();
    //    if (itemTransfers.Any())
    //    {
    //        foreach (var a in itemTransfers)
    //        {
    //            foreach (var item in a.ItemTransferItem)
    //            {
    //                #region Store Item Balance

    //                var storeItem = _db.StoreItemAssignments.Where(x => x.StoreCode == a.ToStore).FirstOrDefault(it => it.ItemCode == item.ItemCode);
    //                if (storeItem != null)
    //                {
    //                    string reff = "Transfer Receive_" + a.ItemTransferID;
    //                    var exist =
    //                        _db.BinCardLines.Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID)
    //                            .Where(x => x.ReferenceDoc == reff)
    //                            .ToList();
    //                    if (!exist.Any())
    //                    {
    //                        #region bin card line

    //                        BinCardLine binCardLine = new BinCardLine
    //                        {
    //                            StoreItemAssignment = storeItem,
    //                            ReferenceDoc = reff,
    //                            Date = a.TransferDate,
    //                            ReceivedQTY = item.ReceivedQuantity,
    //                            ReceivedUPrice = item.UPrice,
    //                            ReceivedTPrice = item.TPrice,
    //                            //BalanceQTY = storeItem.Balance,
    //                            //BalanceUPrice = storeItem.AvgPrice,
    //                            //BalanceTPrice = storeItem.AvgPrice * storeItem.Balance
    //                        };

    //                        _db.BinCardLines.Add(binCardLine);
    //                        _db.SaveChanges();

    //                        #endregion
    //                    }
    //                }


    //                #endregion
    //            }

    //        }
    //    }
    //    return View("Success");
    //}
    //public ActionResult Issue()
    //{
    //    var records = _db.Issues.Where(x => x.IsOnlineApproved).ToList();
    //    var pricesetting = _db.AveragePriceSettings.FirstOrDefault();
    //    foreach (var record in records)
    //    {
    //        #region update stock based on the issue

    //        string storecode = record.StoreCode;

    //        foreach (IssueItem i in record.IssueItems)
    //        {
    //            string itemCode = i.ItemCode;

    //            InventoryItem stockItem = _db.InventoryItems.Find(itemCode);
    //            if (stockItem != null)
    //            {
    //                int storeBranchId = InventoryHelper.GetStoreBranch(storecode);

    //                #region stock

    //                BranchItemAssignment branchItem =
    //                    _db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
    //                        .FirstOrDefault(x => x.ItemCode == i.ItemCode);

    //                if (branchItem != null)
    //                {
    //                    StoreItemAssignment storeItem =
    //                        _db.StoreItemAssignments.FirstOrDefault(
    //                            x => x.StoreCode == storecode && x.ItemCode == itemCode);

    //                    if (storeItem != null)
    //                    {
    //                        if (i.IssuedQuantity > storeItem.Balance)
    //                        {
    //                            i.IssuedQuantity = storeItem.Balance;
    //                            storeItem.Balance = 0;
    //                            branchItem.Balance = branchItem.Balance - i.IssuedQuantity;
    //                            stockItem.Balance = stockItem.Balance - i.IssuedQuantity;
    //                        }
    //                        else if (i.IssuedQuantity <= storeItem.Balance)
    //                        {
    //                            storeItem.Balance = storeItem.Balance - i.IssuedQuantity;
    //                            branchItem.Balance = branchItem.Balance - i.IssuedQuantity;
    //                            stockItem.Balance = stockItem.Balance - i.IssuedQuantity;
    //                        }

    //                        i.Total = i.IssuedQuantity * i.UnitPrice;
    //                        _db.Entry(i).State = EntityState.Modified;
    //                        _db.SaveChanges();

    //                        _db.Entry(storeItem).State = EntityState.Modified;
    //                        _db.SaveChanges();

    //                        _db.Entry(branchItem).State = EntityState.Modified;
    //                        _db.SaveChanges();

    //                        _db.Entry(stockItem).State = EntityState.Modified;
    //                        _db.SaveChanges();

    //                        #region update bin card

    //                        #region bin card line

    //                        BinCardLine binCardLine = new BinCardLine();
    //                        binCardLine.StoreItemAssignment = storeItem;
    //                        binCardLine.ReferenceDoc = "Issue " + record.IssueID.ToString();
    //                        binCardLine.Date = record.IssueDate;
    //                        binCardLine.IssuedQTY = i.IssuedQuantity;
    //                        binCardLine.IssuedUPrice = i.UnitPrice;
    //                        binCardLine.IssuedTPrice = i.Total;
    //                        binCardLine.BalanceQTY = storeItem.Balance;
    //                        binCardLine.BalanceUPrice = binCardLine.IssuedUPrice;
    //                        binCardLine.BalanceTPrice = branchItem.AvgPrice * binCardLine.BalanceQTY;
    //                        //can be item avg id by item 
    //                        _db.BinCardLines.Add(binCardLine);
    //                        _db.SaveChanges();

    //                        #endregion

    //                        #endregion

    //                    }


    //                }

    //                #endregion
    //            }

    //        }


    //        #endregion
    //    }
    //    return View("Success");
    //}
    //public ActionResult Grn()
    //{
    //    var records = _db.GoodReceives.Where(x => x.IsOnlineApproved).ToList();
    //    var pricesetting = _db.AveragePriceSettings.FirstOrDefault();
    //    foreach (var record in records)
    //    {
    //        #region update stock avg price and total

    //        var store = _db.InventoryStores.Find(record.StoreCode);
    //        foreach (var i in record.GoodReceiveItem)
    //        {
    //            var inventoryItem = _db.InventoryItems.Find(i.ItemCode);

    //            var branchItem =
    //                _db.BranchItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
    //                    .FirstOrDefault(x => x.BranchId == store.BranchId);
    //            var storeItem =
    //                _db.StoreItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
    //                    .FirstOrDefault(x => x.StoreCode == store.StoreCode);
    //            var priceIndex =
    //                _db.ItemPriceIndexes.Where(x => x.SourceType == SourceType.GRN)
    //                    .FirstOrDefault(x => x.ItemCode == i.ItemCode);

    //            if (inventoryItem != null)
    //            {
    //                inventoryItem.AvgPrice = ((inventoryItem.Balance * inventoryItem.AvgPrice) +
    //                                          (i.Quantity * i.UnitPrice))
    //                                         / (inventoryItem.Balance + i.Quantity);
    //                inventoryItem.Balance = inventoryItem.Balance + i.Quantity;
    //                _db.Entry(inventoryItem).State = EntityState.Modified;
    //                _db.SaveChanges();

    //                if (priceIndex != null)
    //                {
    //                    priceIndex.PriceBeforeVAT = i.UnitPrice;
    //                    _db.Entry(priceIndex).State = EntityState.Modified;
    //                    _db.SaveChanges();
    //                }

    //                if (priceIndex == null)
    //                {
    //                    var priceindex = new ItemPriceIndex
    //                    {
    //                        ItemCategoryCode = inventoryItem.ItemCategoryCode,
    //                        ItemCode = inventoryItem.ItemCode,
    //                        PriceBeforeVAT = i.UnitPrice,
    //                        SourceOfData = "GRN",
    //                        DateOfInformation = i.GoodReceive.ReceiveDate,
    //                        SourceType = SourceType.GRN
    //                    };
    //                    _db.ItemPriceIndexes.Add(priceindex);
    //                    _db.SaveChanges();
    //                }

    //                if (branchItem != null)
    //                {
    //                    branchItem.AvgPrice = ((branchItem.Balance * branchItem.AvgPrice) +
    //                                           (i.Quantity * i.UnitPrice))
    //                                          / (branchItem.Balance + i.Quantity);
    //                    branchItem.Balance = branchItem.Balance + i.Quantity;
    //                    _db.Entry(branchItem).State = EntityState.Modified;
    //                    _db.SaveChanges();
    //                }
    //                else
    //                {
    //                    branchItem = new BranchItemAssignment();
    //                    if (store != null) branchItem.BranchId = store.BranchId;
    //                    branchItem.ItemCategoryCode = inventoryItem.ItemCategoryCode;
    //                    branchItem.ItemCode = inventoryItem.ItemCode;
    //                    branchItem.Balance = i.Quantity;
    //                    branchItem.AvgPrice = i.UnitPrice;
    //                    _db.BranchItemAssignments.Add(branchItem);
    //                    _db.SaveChanges();
    //                }

    //                if (storeItem != null)
    //                {
    //                    storeItem.AvgPrice = ((storeItem.Balance * storeItem.AvgPrice) + (i.Quantity * i.UnitPrice))
    //                                         / (storeItem.Balance + i.Quantity);
    //                    storeItem.Balance = storeItem.Balance + i.Quantity;
    //                    _db.Entry(storeItem).State = EntityState.Modified;
    //                    _db.SaveChanges();

    //                    #region bin card line

    //                    BinCardLine binCardLine = new BinCardLine
    //                    {
    //                        StoreItemAssignment = storeItem,
    //                        ReferenceDoc = "Receive_" + record.GoodReceiveId.ToString(),
    //                        Date = record.ReceiveDate,
    //                        ReceivedQTY = i.Quantity,
    //                        ReceivedUPrice = i.UnitPrice,
    //                        ReceivedTPrice = i.Total,
    //                        BalanceQTY = storeItem.Balance,
    //                    };

    //                    #region bin card average price

    //                    if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
    //                    {
    //                        binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
    //                        binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
    //                    }

    //                    else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
    //                    {
    //                        binCardLine.BalanceUPrice = branchItem.AvgPrice;
    //                        binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
    //                    }
    //                    else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
    //                    {
    //                        binCardLine.BalanceUPrice = storeItem.AvgPrice;
    //                        binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
    //                    }


    //                    #endregion

    //                    _db.BinCardLines.Add(binCardLine);
    //                    _db.SaveChanges();

    //                    #endregion
    //                }
    //                else
    //                {
    //                    storeItem = new StoreItemAssignment
    //                    {
    //                        StoreCode = record.StoreCode,
    //                        ItemCategoryCode = inventoryItem.ItemCategoryCode,
    //                        ItemCode = inventoryItem.ItemCode,
    //                        Balance = i.Quantity,
    //                        AvgPrice = i.UnitPrice,
    //                    };
    //                    _db.StoreItemAssignments.Add(storeItem);
    //                    _db.SaveChanges();

    //                    #region bin card line

    //                    BinCardLine binCardLine = new BinCardLine
    //                    {
    //                        StoreItemAssignment = storeItem,
    //                        ReferenceDoc = "Receive_" + record.GoodReceiveId.ToString(),
    //                        Date = record.ReceiveDate,
    //                        ReceivedQTY = i.Quantity,
    //                        ReceivedUPrice = i.UnitPrice,
    //                        ReceivedTPrice = i.Total,
    //                        BalanceQTY = storeItem.Balance,
    //                    };

    //                    #region bin card average price

    //                    if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
    //                    {
    //                        binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
    //                        binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
    //                    }

    //                    else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
    //                    {
    //                        binCardLine.BalanceUPrice = branchItem.AvgPrice;
    //                        binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
    //                    }
    //                    else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
    //                    {
    //                        binCardLine.BalanceUPrice = storeItem.AvgPrice;
    //                        binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
    //                    }


    //                    #endregion

    //                    _db.BinCardLines.Add(binCardLine);
    //                    _db.SaveChanges();

    //                    #endregion
    //                }
    //            }
    //        }

    //        #endregion
    //    }
    //    return View("Success");
    //}

    //public ActionResult pr()
    //{
    //    var records = _db.PurchaseRequisitions.Where(x => x.IsSecondOnlineApproved).ToList();
    //    foreach (var record in records)
    //    {
    //        var approval = _db.PurchaseRequisitionPurchaseDepartmentValidations
    //            .Where(x => x.PurchaseRequisitionID == record.PurchaseRequisitionID).ToList();
    //        if (!approval.Any())
    //        {
    //            #region take to second validation

    //            var entity = new PurchaseRequisitionPurchaseDepartmentValidation
    //            {
    //                PurchaseRequisitionID = record.PurchaseRequisitionID,
    //                Status = ApprovalStatuses.Approve,
    //                Approver = User.Identity.Name,
    //                DateCreated = DateTime.Now,
    //                Remark = ""
    //            };

    //            _db.PurchaseRequisitionPurchaseDepartmentValidations.Add(entity);
    //            _db.SaveChanges();

    //            #endregion
    //        }
    //    }
    //    return View("Success");
    //}
    public ActionResult GrnBin()
        {
            var records = _db.GoodReceives.Where(x => x.IsOnlineApproved && !x.IsVoid && x.GoodReceiveId>= 11381).ToList();
            var pricesetting = _db.AveragePriceSettings.FirstOrDefault();
            foreach (var record in records)
            {
                #region take to bincard

                var store = _db.InventoryStores.Find(record.StoreCode);
                foreach (var i in record.GoodReceiveItem)
                {
                    var exist = _db.BinCardLines
                        .Where(x => x.ReferenceDoc == "Receive_" + record.GoodReceiveId.ToString()).ToList();
                    if (!exist.Any())
                    {
                        var inventoryItem = _db.InventoryItems.Find(i.ItemCode);

                        var branchItem =
                            _db.BranchItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
                                .FirstOrDefault(x => x.BranchId == store.BranchId);
                        var storeItem =
                            _db.StoreItemAssignments.Where(x => x.ItemCode == inventoryItem.ItemCode)
                                .FirstOrDefault(x => x.StoreCode == store.StoreCode);

                        if (inventoryItem != null)
                        {
                            if (storeItem != null)
                            {
                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Receive_" + record.GoodReceiveId.ToString(),
                                    Date = record.ReceiveDate,
                                    ReceivedQTY = i.Quantity,
                                    ReceivedUPrice = i.UnitPrice,
                                    ReceivedTPrice = i.Total,
                                    BalanceQTY = storeItem.Balance,
                                };

                                #region bin card average price

                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                {
                                    binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                    binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                }

                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                {
                                    binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                    binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                }
                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                {
                                    binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                    binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                }


                                #endregion

                                _db.BinCardLines.Add(binCardLine);
                                _db.SaveChanges();

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
                                _db.StoreItemAssignments.Add(storeItem);
                                _db.SaveChanges();

                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Receive_" + record.GoodReceiveId.ToString(),
                                    Date = record.ReceiveDate,
                                    ReceivedQTY = i.Quantity,
                                    ReceivedUPrice = i.UnitPrice,
                                    ReceivedTPrice = i.Total,
                                    BalanceQTY = storeItem.Balance,
                                };

                                #region bin card average price

                                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                                {
                                    binCardLine.BalanceUPrice = inventoryItem.AvgPrice;
                                    binCardLine.BalanceTPrice = inventoryItem.AvgPrice * storeItem.Balance;
                                }

                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                {
                                    binCardLine.BalanceUPrice = branchItem.AvgPrice;
                                    binCardLine.BalanceTPrice = branchItem.AvgPrice * storeItem.Balance;
                                }
                                else if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerStore)
                                {
                                    binCardLine.BalanceUPrice = storeItem.AvgPrice;
                                    binCardLine.BalanceTPrice = storeItem.AvgPrice * storeItem.Balance;
                                }


                                #endregion

                                _db.BinCardLines.Add(binCardLine);
                                _db.SaveChanges();

                                #endregion
                            }
                        }
                    }
                }

                #endregion
            }
            return View("Success");
        }

        public string Log()
        {
            Logger.Debug("I am debugging these.");
            return DateTime.Today.ToLongDateString();
        }
        public FileResult Download()
        {
            string file = @"c:\someFolder\foo.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }

        public void Index1()
        {
            //FinanceGlobalStaticValues.BATCH_POST_RUNNING = false;
            // Jobs();
            var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();

            // set any deviceInfo settings if necessary
            var deviceInfo = new System.Collections.Hashtable();

            // Depending on the report definition choose ONE of the following REPORT SOURCES
            //                  -1-
            // ***CLR (CSharp) report definitions***
            var reportSource = new Telerik.Reporting.TypeReportSource();

            // reportName is the Assembly Qualified Name of the report
            reportSource.TypeName = "ExceedERP.Reporting.Finance.Budget.View.BalanceSheetSingleQuarter, ExceedERP.Reporting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            //                  -1-

            ////                  -2-
            //// ***Declarative (TRDP/TRDX) report definitions***
            //var reportSource = new Telerik.Reporting.UriReportSource();

            //// reportName is the path to the TRDP/TRDX file
            //reportSource.Uri = reportName;
            ////                  -2-

            ////                  -3-
            //// ***Instance of the report definition***
            //var reportSource = new Telerik.Reporting.InstanceReportSource();

            //// Report1 is the class of the report. It should inherit Telerik.Reporting.Report class
            //reportSource.ReportDocument = new Report1();
            ////                  -3-

            // Pass parameter value with the Report Source if necessary
            object year = "4";
            object quarter = "1";
            reportSource.Parameters.Add("year", year);
            reportSource.Parameters.Add("quarter", quarter);

            Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("xlsx", reportSource, deviceInfo);

            string fileName = result.DocumentName + "." + result.Extension;
            string path = System.IO.Path.GetTempPath();
            string filePath = System.IO.Path.Combine(path, fileName);

            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
            }
            
        }
        public ActionResult Index()
        {
            //double t = 17859.80;
            //ViewBag.k=XAPI.NumberExtensions.toWords(t);
            return View();
        }

        public ActionResult PeriodIndex()
        {
            _fiscalYearHelper.UpdatePeriodIndex();
            return View("Success");
        }

        public ActionResult Dash()
        {
           //_dashboardHelper.UpdateProcurementDocStatusDict();
           //_dashboardHelper.UpdateFinanceDocStatusDict();
           //_dashboardHelper.UpdateInventoryDocStatusDict();
           //_dashboardHelper.UpdateFixedAssetDocStatusDict();
           //_dashboardHelper.UpdateDashboardDictionary();
           _dashboardHelper.UpdateDashboardFinanceRatio();

           //_dashboardHelper.UpdateDashboardItemBalance();

           // ConstructionDashboardHelper.AddConsPlanAcheivmentData();
           // ConstructionDashboardHelper.AddCostStatusData();
           // ConstructionDashboardHelper.AddTimeStatusData();
           // ConstructionDashboardHelper.AddProjectPaymentAmountData();
            return View("Success");
        }

        public ActionResult Sale()
        {
            var arInvoices = _db.ARInvoices.Where(x => x.ArSalesOrderId != 0).ToList();
            foreach (var invoice in arInvoices)
            {
                var invoiceLines = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == invoice.ArInvoiceId).ToArray();
                var orderLines = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == invoice.ArSalesOrderId).ToArray();
                for (int i = 0; i < invoiceLines.Length; i++)
                {
                    try
                    {
                        invoiceLines[i].ArSalesOrderLineId = orderLines[i].ArSalesOrderLineId;
                        _db.Entry(invoiceLines[i]).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                       Logger.Debug(e.Message+"\n"+e.StackTrace);
                    }
                }
                
            }
            return View("Success");
        }

        public ActionResult UpdateEarningForAYear()
        {
            ClosingHelper closingHelper = new ClosingHelper();
            var glFiscalYears = _db.GLFiscalYears.OrderBy(x => x.DateTo).ToList();
            foreach (var glFiscalYear in glFiscalYears)
            {
                closingHelper.UpdateEarningForAYear(glFiscalYear.GlFiscalYearId);
            }

            return View("Success");
        }
        public ActionResult CountName()
        {
            var items = _db.PropertyCountItems.Where(x=>x.ItemName==null).ToList();
            foreach (var propertyCountItem in items)
            {
                var inventoryitem = _db.InventoryItems.Find(propertyCountItem.ItemCode);
                if (inventoryitem != null) propertyCountItem.ItemName = inventoryitem.Name;
                _db.Entry(propertyCountItem).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return View("Success");
        }

        public ActionResult StoreItemtName()
        {
            var items = _db.StoreItemAssignments.Where(x => x.ItemName == null).ToList();
            foreach (var storeItemAssignment in items)
            {
                var inventoryitem = _db.InventoryItems.Find(storeItemAssignment.ItemCode);
                if (inventoryitem != null) storeItemAssignment.ItemName = inventoryitem.Name;
                _db.Entry(storeItemAssignment).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return View("Success");
        }

        public ActionResult TransferToGl()
        {

            #region ar invoice
            var records = _db.ARInvoices
                .Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).AsNoTracking().ToList();

            foreach (var record in records)
            {
                //#region check segment length validation

                //foreach (var dist in record.ArInvoiceDistribution)
                //{
                //    string message = accountHelper.ValidateSegmentCombinationLength(dist.Description);
                //    if (!String.IsNullOrEmpty(message))
                //    {
                //        var formattedData = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData, JsonRequestBehavior.AllowGet);
                //    }

                //}


                //#endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = _db.ARInvoiceDistributions.Where(x=>x.ArInvoiceId==record.ArInvoiceId).AsNoTracking().ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    //var formattedData = new
                    //{
                    //    Message = "Total debit and total credit is not balanced.",
                    //    Status = "NOT_OK"
                    //};
                    //return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    //var formattedData = new
                    //{
                    //    Message = "Total debit and total credit is zero.",
                    //    Status = "NOT_OK"
                    //};
                    //return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region send to GL

                var year = _db.GLFiscalYears.FirstOrDefault();
                string glyear = year.Name;
                string glperiod = year.GLPeriod.FirstOrDefault().Name;
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.Date)
                    .Where(x => x.DateTo >= record.Date).ToList();
                if (period.Any())
                {

                    if (period.Count() == 1)
                    {
                        var myPeriod = period.SingleOrDefault();
                        glperiod = myPeriod.Name;
                        glyear = myPeriod.GlFiscalYear.Name.ToString();
                        if (myPeriod.Status == FiscalYearStatus.Close)
                        {
                            //var formattedData = new
                            //{
                            //    Message = "The period " + glperiod + " is closed,the record cannot be transfered to closed period.",
                            //    Status = "NOT_OK"
                            //};
                            //return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (!period.Any())
                    {
                        //var formattedData = new
                        //{
                        //    Message = "There are no periods defined that contain the Invoice date.",
                        //    Status = "NOT_OK"
                        //};
                        //return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    else if (period.Any())
                    {
                        //var formattedData = new
                        //{
                        //    Message = "There are multiple periods defined that contain the Invoice date.",
                        //    Status = "NOT_OK"
                        //};
                        //return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }

                #endregion
                GlRecordJournal journal = new GlRecordJournal();
                if (record.ArInvoiceDistribution.Any())
                {
                    journal.DateCreated = DateTime.Now;
                    journal.CreatedBy = User.Identity.Name;
                    journal.JournalName = "AR Invoice";
                    journal.Description = "AR Invoice";
                    journal.FiscalYear = glyear;
                    journal.PeriodName = glperiod;
                    journal.CategoryName = "AR Invoice";
                    journal.SourcesName = "AR";
                    journal.JournalNumber = "ARINV." + record.ArInvoiceId.ToString();
                    journal.EffectiveDate = record.Date;
                    journal.OrgBranchName = record.OrgBranchName;
                    journal.Status = "Approved";
                    journal.IsOnlineApproved = true;
                    journal.OnlineApprovedBy = User.Identity.Name;
                    journal.OnlineApprovedTime = DateTime.Now;
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distribution)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion
                    

                }
                #endregion

            }

            #endregion

            #region misc payment
            var miscellaneousPayments = _db.APMiscellaneousPayments
                .Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).AsNoTracking().ToList();

            foreach (var record in miscellaneousPayments)
            {
                //#region check segment length validation

                //foreach (var dist in record.ApMiscellaneousPaymentDistribution)
                //{
                //    string message = accountHelper.ValidateSegmentCombinationLength(dist.Description);
                //    if (!String.IsNullOrEmpty(message))
                //    {
                //        var formattedData = new
                //        {
                //            Message = message,
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData, JsonRequestBehavior.AllowGet);
                //    }

                //}


                //#endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = _db.APMiscellaneousPaymentDistributions.Where(x=>x.ApMiscellaneousPaymentId==record.ApMiscellaneousPaymentId).AsNoTracking().ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    //var formattedData = new
                    //{
                    //    Message = "Total debit and total credit is not balanced.",
                    //    Status = "NOT_OK"
                    //};
                    //return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    //var formattedData = new
                    //{
                    //    Message = "Total debit and total credit is zero.",
                    //    Status = "NOT_OK"
                    //};
                    //return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region send to GL

                var year = _db.GLFiscalYears.FirstOrDefault();
                string glyear = year.Name;
                string glperiod = year.GLPeriod.FirstOrDefault().Name;
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.PaymentDate)
                    .Where(x => x.DateTo >= record.PaymentDate).ToList();
                if (period.Any())
                {

                    if (period.Count() == 1)
                    {
                        var myPeriod = period.SingleOrDefault();
                        glperiod = myPeriod.Name;
                        glyear = myPeriod.GlFiscalYear.Name.ToString();
                        if (myPeriod.Status == FiscalYearStatus.Close)
                        {
                            //var formattedData = new
                            //{
                            //    Message = "The period " + glperiod + " is closed,the record cannot be transfered to closed period.",
                            //    Status = "NOT_OK"
                            //};
                            //return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (!period.Any())
                    {
                        //var formattedData = new
                        //{
                        //    Message = "There are no periods defined that contain the Payment date.",
                        //    Status = "NOT_OK"
                        //};
                        //return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    else if (period.Any())
                    {
                        //var formattedData = new
                        //{
                        //    Message = "There are multiple periods defined that contain the Payment date.",
                        //    Status = "NOT_OK"
                        //};
                        //return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }

                #endregion
                GlRecordJournal journal = new GlRecordJournal();
                if (record.ApMiscellaneousPaymentDistribution.Any())
                {
                    journal.DateCreated = DateTime.Now;
                    journal.CreatedBy = User.Identity.Name;
                    journal.JournalName = "AP Misc Payment";
                    journal.Description = "AP Misc Payment";
                    journal.FiscalYear = glyear;
                    journal.PeriodName = glperiod;
                    journal.CategoryName = "AP Misc Payment";
                    journal.SourcesName = "AP";
                    journal.JournalNumber = "APMPAY." + record.ApMiscellaneousPaymentId.ToString();
                    journal.EffectiveDate = record.PaymentDate;
                    journal.OrgBranchName = record.OrgBranchName;
                    journal.Status = "Approved";
                    journal.IsOnlineApproved = true;
                    journal.OnlineApprovedBy = User.Identity.Name;
                    journal.OnlineApprovedTime = DateTime.Now;
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in distribution)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            AccountDesc = i.AccountDesc,
                            Debit = i.Debit,
                            Credit = i.Credit
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }

                    #region update record

                    record.IsOnlineTransferred = true;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();

                    #endregion

                    //var formattedData = new
                    //{
                    //    Message = "Successfully transfered.",
                    //    Status = "OK"
                    //};
                    //return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion

            }
            #endregion
            return View("success");
        }

        public class BinCardLineVm
        {
            public int BinCardLineID { get; set; }
            public int StoreItemAssignmentID { get; set; }
            public DateTime Date { get; set; }
            public string ReferenceDoc { get; set; }
            public decimal ReceivedQTY { get; set; }
            public decimal ReceivedUPrice { get; set; }
            public decimal ReceivedTPrice { get; set; }
            public decimal IssuedQTY { get; set; }
            public decimal IssuedUPrice { get; set; }
            public decimal IssuedTPrice { get; set; }
            public decimal BalanceQTY { get; set; }
            public decimal BalanceUPrice { get; set; }
            public decimal BalanceTPrice { get; set; }
            public int GlFiscalYearId { get; set; }
            public int GLPeriodId { get; set; }
            public string Reference { get; set; }
            public DateTime? TimeStamp { get; set; }
            public bool IsVoid { get; set; }
            public string SupplierName { get; set; }
            public string IssuedTo { get; set; }

        }
        //public ActionResult BinCard2(string store="",string start="")
        //{
        //   DateTime endDateTime = DateTime.Parse("2018-06-30 00:00:00.000");
        //    var store1 = store;
        //    var storeitems = _db.StoreItemAssignments.Where(x => x.StoreCode == store1).ToList();
        //    if (start != "0")
        //        storeitems = storeitems
        //            //.Where(x => x.Remark != "1")
        //            .ToList();

        //    var storeitemList = storeitems.ToList();
        //    foreach (var storeItem in storeitemList)
        //    {
        //        using (var context = new ExceedDbContext())
        //        {
        //            List<BinCardLineVm> nonbegbincards = new List<BinCardLineVm>();
        //            List<BinCardLineVm> sortedCardLineVms = new List<BinCardLineVm>();

        //            #region remove seconday

        //            var secondbinCard =
        //                context.BinCardLineSecondaries.Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID)
        //                    .ToList();
        //            context.BinCardLineSecondaries.RemoveRange(secondbinCard);
        //            context.SaveChanges();

        //            #endregion

        //            DateTime starDateTime = DateTime.Parse("2017-07-01 00:00:00.000");

        //            var bincardAll = context.BinCardLines
        //                .Where(x => x.StoreItemAssignmentID == storeItem.StoreItemAssignmentID)
        //                .ToList();

        //            var bincarbegin = bincardAll.FirstOrDefault(x => x.ReferenceDoc == "Count on date 6/30/2017");
        //            var bincardNonBeg = bincardAll.Where(x => x.ReferenceDoc != "Count on date 6/30/2017")
        //                .Where(x => x.Date >= starDateTime)
        //                .OrderBy(x => x.Date).ToList();



        //            if (bincarbegin == null)
        //            {
        //                #region initialize beginning

        //                var bincarbeginNew = new BinCardLineVm
        //                {
        //                    StoreItemAssignmentID = storeItem.StoreItemAssignmentID,
        //                    ReferenceDoc = "Ref Doc",
        //                    Reference = "Ref",
        //                    Date = starDateTime.Date, //remove time

        //                    ReceivedQTY = 0,
        //                    ReceivedUPrice = 0,
        //                    ReceivedTPrice = 0,

        //                    IssuedQTY = 0,
        //                    IssuedUPrice = 0,
        //                    IssuedTPrice = 0,

        //                    BalanceQTY = 0,
        //                    BalanceUPrice = 0,
        //                    BalanceTPrice = 0,
        //                    IssuedTo = "NA",
        //                    BinCardLineID = 0

        //                };
        //                sortedCardLineVms.Add(bincarbeginNew);

        //                #endregion
        //            }

        //            else
        //            {
        //                //to remove transactions entered before count
        //                bincardNonBeg = bincardNonBeg.Where(x => x.BinCardLineID > bincarbegin.BinCardLineID).ToList();

        //                #region copy beginning

        //                var bincarbeginNew = new BinCardLineVm
        //                {
        //                    StoreItemAssignmentID = storeItem.StoreItemAssignmentID,
        //                    ReferenceDoc = bincarbegin.ReferenceDoc,
        //                    Reference = bincarbegin.Reference,
        //                    Date = bincarbegin.Date.Date, //remove time

        //                    ReceivedQTY = bincarbegin.ReceivedQTY,
        //                    ReceivedUPrice = bincarbegin.ReceivedUPrice,
        //                    ReceivedTPrice = bincarbegin.ReceivedTPrice,

        //                    IssuedQTY = bincarbegin.IssuedQTY,
        //                    IssuedUPrice = bincarbegin.IssuedUPrice,
        //                    IssuedTPrice = bincarbegin.IssuedTPrice,

        //                    BalanceQTY = bincarbegin.BalanceQTY,
        //                    BalanceUPrice = bincarbegin.BalanceUPrice,
        //                    BalanceTPrice = bincarbegin.BalanceTPrice,
        //                    IssuedTo = bincarbegin.IssuedTo,

        //                    GlFiscalYearId = bincarbegin.GlFiscalYearId,
        //                    GLPeriodId = bincarbegin.GLPeriodId,
        //                    TimeStamp = bincarbegin.TimeStamp,
        //                    IsVoid = bincarbegin.IsVoid,
        //                    SupplierName = bincarbegin.SupplierName,
        //                    BinCardLineID = bincarbegin.BinCardLineID
        //                };
        //                sortedCardLineVms.Add(bincarbeginNew);

        //                #endregion
        //            }

        //            foreach (var binCardLine in bincardNonBeg)
        //            {
        //                #region copy non beginning and transform date

        //                var bincarbeginNew = new BinCardLineVm
        //                {
        //                    StoreItemAssignmentID = storeItem.StoreItemAssignmentID,
        //                    ReferenceDoc = binCardLine.ReferenceDoc,
        //                    Reference = binCardLine.Reference,
        //                    Date = binCardLine.Date.Date, //remove time

        //                    ReceivedQTY = binCardLine.ReceivedQTY,
        //                    ReceivedUPrice = binCardLine.ReceivedUPrice,
        //                    ReceivedTPrice = binCardLine.ReceivedTPrice,

        //                    IssuedQTY = binCardLine.IssuedQTY,
        //                    IssuedUPrice = binCardLine.IssuedUPrice,
        //                    IssuedTPrice = binCardLine.IssuedTPrice,

        //                    BalanceQTY = binCardLine.BalanceQTY,
        //                    BalanceUPrice = binCardLine.BalanceUPrice,
        //                    BalanceTPrice = binCardLine.BalanceTPrice,
        //                    IssuedTo = binCardLine.IssuedTo,

        //                    GlFiscalYearId = binCardLine.GlFiscalYearId,
        //                    GLPeriodId = binCardLine.GLPeriodId,
        //                    TimeStamp = binCardLine.TimeStamp,
        //                    IsVoid = binCardLine.IsVoid,
        //                    SupplierName = binCardLine.SupplierName,
        //                    BinCardLineID = binCardLine.BinCardLineID
        //                };

        //                //add 20 days to 
        //                if (bincarbeginNew.ReferenceDoc.StartsWith("Issue") ||
        //                    bincarbeginNew.ReferenceDoc.StartsWith("Transfer Send"))
        //                {
        //                    bincarbeginNew.Date = bincarbeginNew.Date.AddDays(0);
        //                    if (bincarbeginNew.Date > endDateTime)
        //                        bincarbeginNew.Date = endDateTime;
        //                    nonbegbincards.Add(bincarbeginNew);
        //                }
        //                else
        //                {
        //                    nonbegbincards.Add(bincarbeginNew);
        //                }

        //                #endregion
        //            }

        //            List<DateTime> uniquedateList =
        //                nonbegbincards.Select(x => x.Date).OrderBy(x => x.Date).Distinct().ToList();

        //            foreach (var date in uniquedateList)
        //            {
        //                #region rearrange

        //                #region grn

        //                var goodreceives = nonbegbincards.Where(x => x.Date == date)
        //                    .Where(x => x.ReferenceDoc.StartsWith("Receive"));

        //                foreach (var goodreceive in goodreceives)
        //                {
        //                    sortedCardLineVms.Add(goodreceive); //add record

        //                    string id = goodreceive.ReferenceDoc.Split('_')[1];
        //                    var voidRecord = nonbegbincards.FirstOrDefault(x => x.ReferenceDoc == "Void Receive " + id);

        //                    if (voidRecord != null)
        //                        sortedCardLineVms.Add(voidRecord); //add voidrecord
        //                }

        //                //var voidreceive = nonbegbincards.Where(x => x.Date == date).Where(x => x.ReferenceDoc.StartsWith("Void Receive"));
        //                //sortedCardLineVms.AddRange(voidreceive);

        //                #endregion

        //                #region transfer receive

        //                var transferreceives = nonbegbincards.Where(x => x.Date == date)
        //                    .Where(x => x.ReferenceDoc.StartsWith("Transfer Receive"));

        //                foreach (var transferreceive in transferreceives)
        //                {
        //                    sortedCardLineVms.Add(transferreceive);

        //                    string id = transferreceive.ReferenceDoc.Split('_')[1];
        //                    var voidRecord =
        //                        nonbegbincards.FirstOrDefault(x => x.ReferenceDoc == "Void Transfer Receive " + id);

        //                    if (voidRecord != null)
        //                        sortedCardLineVms.Add(voidRecord); //add voidrecord
        //                }

        //                //var voidtransferreceive = nonbegbincards.Where(x => x.Date == date).Where(x => x.ReferenceDoc.StartsWith("Void Transfer Receive"));
        //                //sortedCardLineVms.AddRange(voidtransferreceive);

        //                #endregion

        //                #region return

        //                var returns = nonbegbincards.Where(x => x.Date == date)
        //                    .Where(x => x.ReferenceDoc.StartsWith("Return"));

        //                foreach (var rReturn in returns)
        //                {
        //                    sortedCardLineVms.Add(rReturn);

        //                    string id = rReturn.ReferenceDoc.Split('_')[1];
        //                    var voidRecord = nonbegbincards.FirstOrDefault(x => x.ReferenceDoc == "Void Return_" + id);

        //                    if (voidRecord != null)
        //                        sortedCardLineVms.Add(voidRecord); //add voidrecord
        //                }

        //                //var voidreturns = nonbegbincards.Where(x => x.Date == date).Where(x => x.ReferenceDoc.StartsWith("Void Return"));
        //                //sortedCardLineVms.AddRange(voidreturns);

        //                #endregion

        //                #region issue

        //                var issues = nonbegbincards.Where(x => x.Date == date)
        //                    .Where(x => x.ReferenceDoc.StartsWith("Issue"));

        //                foreach (var issue in issues)
        //                {


        //                    string id = issue.ReferenceDoc.Split(' ')[1];
        //                    var voidRecord = nonbegbincards.FirstOrDefault(x => x.ReferenceDoc == "Void Issue " + id);

        //                    if (voidRecord == null) //if it is void don't add
        //                        sortedCardLineVms.Add(issue);

        //                    //if (voidRecord != null)
        //                    //{
        //                    //    voidRecord.Date = issue.Date;
        //                    //    sortedCardLineVms.Add(voidRecord);
        //                    //} //add voidrecord
        //                }


        //                //var voidissue = nonbegbincards.Where(x => x.Date == date).Where(x => x.ReferenceDoc.StartsWith("Void Issue"));
        //                //sortedCardLineVms.AddRange(voidissue);

        //                #endregion

        //                #region transfer send

        //                var transfersends = nonbegbincards.Where(x => x.Date == date)
        //                    .Where(x => x.ReferenceDoc.StartsWith("Transfer Send"));

        //                foreach (var transfersend in transfersends)
        //                {


        //                    string id = transfersend.ReferenceDoc.Split(' ')[2];
        //                    var voidRecord =
        //                        nonbegbincards.FirstOrDefault(x => x.ReferenceDoc == "Void Transfer Send " + id);

        //                    if (voidRecord == null) //if it is void don't add
        //                        sortedCardLineVms.Add(transfersend);

        //                    //if (voidRecord != null)
        //                    //{
        //                    //    voidRecord.Date = transfersend.Date;
        //                    //    sortedCardLineVms.Add(voidRecord);
        //                    //} //add voidrecord

        //                }

        //                //var voidtransfersend = nonbegbincards.Where(x => x.Date == date).Where(x => x.ReferenceDoc.StartsWith("Void Transfer Send"));
        //                //sortedCardLineVms.AddRange(voidtransfersend);

        //                #endregion

        //                #endregion
        //            }

        //            var sortedbinCardArray = sortedCardLineVms.ToArray();
        //            for (int i = 1; i < sortedbinCardArray.Length; i++)
        //            {
        //                decimal totalPrice = 0;
        //                decimal totalQty = 0;

        //                #region update the arary in ram


        //                #region incrementing operation

        //                if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Receive") ||
        //                    sortedbinCardArray[i].ReferenceDoc.StartsWith("Transfer Receive"))
        //                {
        //                    #region grn Transfer Receive

        //                    totalQty = sortedbinCardArray[i - 1].BalanceQTY + sortedbinCardArray[i].ReceivedQTY;
        //                    sortedbinCardArray[i].BalanceQTY = totalQty;

        //                    totalPrice =
        //                        (sortedbinCardArray[i - 1].BalanceQTY * sortedbinCardArray[i - 1].BalanceUPrice)
        //                        + (sortedbinCardArray[i].ReceivedQTY * sortedbinCardArray[i].ReceivedUPrice);

        //                    sortedbinCardArray[i].BalanceUPrice = totalQty == 0 ? 0 : totalPrice / totalQty;
        //                    sortedbinCardArray[i].BalanceTPrice =
        //                        sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

        //                    #endregion
        //                }

        //                else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Return"))
        //                {
        //                    #region return

        //                    totalQty = sortedbinCardArray[i - 1].BalanceQTY + sortedbinCardArray[i].ReceivedQTY;
        //                    sortedbinCardArray[i].BalanceQTY = totalQty;

        //                    sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
        //                    sortedbinCardArray[i].BalanceTPrice =
        //                        sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

        //                    #endregion
        //                }

        //                else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Issue"))
        //                {
        //                    #region Void Issue

        //                    totalQty = sortedbinCardArray[i - 1].BalanceQTY + sortedbinCardArray[i].ReceivedQTY;
        //                    sortedbinCardArray[i].BalanceQTY = totalQty;

        //                    sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
        //                    sortedbinCardArray[i].BalanceTPrice =
        //                        sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

        //                    #endregion
        //                }

        //                else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Transfer Send"))
        //                {
        //                    #region Void Transfer Send

        //                    totalQty = sortedbinCardArray[i - 1].BalanceQTY + sortedbinCardArray[i].ReceivedQTY;
        //                    sortedbinCardArray[i].BalanceQTY = totalQty;

        //                    sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
        //                    sortedbinCardArray[i].BalanceTPrice =
        //                        sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

        //                    #endregion
        //                }

        //                #endregion


        //                #region dencrementing operation

        //                if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Issue") ||
        //                    sortedbinCardArray[i].ReferenceDoc.StartsWith("Transfer Send"))
        //                {
        //                    #region issue

        //                    sortedbinCardArray[i].IssuedUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
        //                    sortedbinCardArray[i].IssuedTPrice =
        //                        sortedbinCardArray[i].IssuedUPrice * sortedbinCardArray[i].IssuedQTY;

        //                    totalQty = sortedbinCardArray[i - 1].BalanceQTY - sortedbinCardArray[i].IssuedQTY;

        //                    sortedbinCardArray[i].BalanceQTY = totalQty;
        //                    sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
        //                    sortedbinCardArray[i].BalanceTPrice =
        //                        sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

        //                    #endregion
        //                }
        //                else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Receive"))
        //                {
        //                    decimal totalpricebefore = 0;

        //                    #region Void Receive

        //                    totalQty = sortedbinCardArray[i - 1].BalanceQTY - sortedbinCardArray[i].IssuedQTY;
        //                    //get issued price from original record if possible
        //                    totalpricebefore = sortedbinCardArray[i].BalanceTPrice -
        //                                       (sortedbinCardArray[i].IssuedQTY * sortedbinCardArray[i].IssuedUPrice);

        //                    sortedbinCardArray[i].BalanceQTY = totalQty;
        //                    sortedbinCardArray[i].BalanceUPrice = totalQty == 0 ? 0 : totalpricebefore / totalQty;
        //                    sortedbinCardArray[i].BalanceTPrice =
        //                        sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

        //                    #endregion
        //                }

        //                else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Transfer Receive"))
        //                {
        //                    decimal totalpricebefore = 0;

        //                    #region Void Transfer Receive

        //                    totalQty = sortedbinCardArray[i - 1].BalanceQTY - sortedbinCardArray[i].IssuedQTY;
        //                    //get issued price from original record if possible
        //                    totalpricebefore = sortedbinCardArray[i].BalanceTPrice -
        //                                       (sortedbinCardArray[i].IssuedQTY * sortedbinCardArray[i].IssuedUPrice);

        //                    sortedbinCardArray[i].BalanceQTY = totalQty;
        //                    sortedbinCardArray[i].BalanceUPrice = totalQty == 0 ? 0 : totalpricebefore / totalQty;
        //                    sortedbinCardArray[i].BalanceTPrice =
        //                        sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

        //                    #endregion
        //                }
        //                else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Return"))
        //                {
        //                    #region Void Return

        //                    totalQty = sortedbinCardArray[i - 1].BalanceQTY - sortedbinCardArray[i].IssuedQTY;

        //                    sortedbinCardArray[i].BalanceQTY = totalQty;
        //                    sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
        //                    sortedbinCardArray[i].BalanceTPrice =
        //                        sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

        //                    #endregion
        //                }

        //                #endregion

        //                #endregion
        //            }

        //            #region save array

        //            foreach (var array in sortedCardLineVms)
        //            {
        //                try
        //                {
        //                    var binCardLineBeg = new BinCardLineSecondary
        //                    {
        //                        StoreItemAssignment = storeItem,
        //                        ReferenceDoc = array.ReferenceDoc,
        //                        Reference = array.Reference,
        //                        Date = array.Date,

        //                        ReceivedQTY = array.ReceivedQTY,
        //                        ReceivedUPrice = array.ReceivedUPrice,
        //                        ReceivedTPrice = array.ReceivedTPrice,

        //                        IssuedQTY = array.IssuedQTY,
        //                        IssuedUPrice = array.IssuedUPrice,
        //                        IssuedTPrice = array.IssuedTPrice,

        //                        BalanceQTY = array.BalanceQTY,
        //                        BalanceUPrice = array.BalanceUPrice,
        //                        BalanceTPrice = array.BalanceTPrice,
        //                        IssuedTo = array.IssuedTo,
        //                        BinCardLineID = array.BinCardLineID

        //                    };
        //                    context.BinCardLines.Add(binCardLineBeg);
        //                    context.SaveChanges();
        //                }
        //                catch (Exception e)
        //                {
        //                    ViewBag.Error = storeItem.ItemCode + " Assignment ID:" + storeItem.StoreItemAssignmentID +
        //                                    e.Message;
        //                    return View("Error");
        //                }
        //            }

        //            #endregion

        //            storeItem.Remark = "1";
        //            context.Entry(storeItem).State = EntityState.Modified;
        //            context.SaveChanges();
        //        }

        //    }

        //    return View("Success");
        //}
        public ActionResult BinCardUpdate()
        {
            var bincardlist = _db.BinCardLines.ToList();

            var bincardAll = bincardlist
                .Where(x => x.BinCardLineID >= 281891)
                .Where(x => x.BinCardLineID <= 295119)
                .OrderBy(x=>x.BinCardLineID)
                .ToList();

            var list = bincardAll.Select(x=>x.StoreItemAssignmentID).Distinct().ToList();
            //foreach (var storeItem in list)
            //{
            //    int minid = bincardAll.Where(x=>x.StoreItemAssignmentID==storeItem).Min(x => x.BinCardLineID);

            //    var before = bincardlist.Where(x => x.StoreItemAssignmentID == storeItem)
            //        .Where(x => x.BinCardLineID < minid).ToList();

            //    if (before.Any())
            //    {
            //        int maxid = before.Max(x => x.BinCardLineID);
            //        var beginning = _db.BinCardLines.Find(maxid);

            //        bincardAll.Add(beginning);
            //    }
            //    else
            //    {
            //        var bincarbeginNew = new BinCardLine
            //        {
            //            StoreItemAssignmentID = storeItem,
            //            ReferenceDoc = "Ref Doc",
            //            Reference = "Ref",
            //            Date =DateTime.Today,

            //            ReceivedQTY = 0,
            //            ReceivedUPrice = 0,
            //            ReceivedTPrice = 0,

            //            IssuedQTY = 0,
            //            IssuedUPrice = 0,
            //            IssuedTPrice = 0,

            //            BalanceQTY = 0,
            //            BalanceUPrice = 0,
            //            BalanceTPrice = 0,
            //            IssuedTo = "NA",
            //            BinCardLineID = 0

            //        };

            //        bincardAll.Add(bincarbeginNew);
            //    }

                
            //}
           

            foreach (var storeItem in list)
            {
                var itemBincard = bincardAll.Where(x => x.StoreItemAssignmentID == storeItem).ToList();
                #region get beginning
                int minid = itemBincard.Min(x => x.BinCardLineID);

                var before = bincardlist.Where(x => x.StoreItemAssignmentID == storeItem)
                    .Where(x => x.BinCardLineID < minid).ToList();

                if (before.Any())
                {
                    int maxid = before.Max(x => x.BinCardLineID);
                    var beginning = _db.BinCardLines.Find(maxid);

                    itemBincard.Add(beginning);
                }
                else
                {
                    var bincarbeginNew = new BinCardLine
                    {
                        StoreItemAssignmentID = storeItem,
                        ReferenceDoc = "Ref Doc",
                        Reference = "Ref",
                        Date = DateTime.Today,

                        ReceivedQTY = 0,
                        ReceivedUPrice = 0,
                        ReceivedTPrice = 0,

                        IssuedQTY = 0,
                        IssuedUPrice = 0,
                        IssuedTPrice = 0,

                        BalanceQTY = 0,
                        BalanceUPrice = 0,
                        BalanceTPrice = 0,
                        IssuedTo = "NA",
                        BinCardLineID = 0

                    };

                    itemBincard.Add(bincarbeginNew);
                }
                #endregion
                var sortedbinCardArray = itemBincard.OrderBy(x => x.BinCardLineID).ToArray();

                for (int i = 1; i < sortedbinCardArray.Length; i++)
                {
                    decimal totalPrice = 0;
                    decimal totalQty = 0;
                    #region update the arary in ram

                    if (sortedbinCardArray[i].ReceivedQTY > 0)
                    {
                        if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Receive") || sortedbinCardArray[i].ReferenceDoc.StartsWith("Transfer Receive"))
                        {
                            #region grn Transfer Receive

                            totalQty = sortedbinCardArray[i - 1].BalanceQTY + sortedbinCardArray[i].ReceivedQTY;

                            totalPrice = (sortedbinCardArray[i - 1].BalanceQTY * sortedbinCardArray[i - 1].BalanceUPrice)
                                         + (sortedbinCardArray[i].ReceivedQTY * sortedbinCardArray[i].ReceivedUPrice);

                            sortedbinCardArray[i].BalanceQTY= totalQty;
                            sortedbinCardArray[i].BalanceUPrice = totalQty == 0 ? 0 : totalPrice / totalQty;
                            sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Return"))
                        {
                            #region return

                            totalQty = sortedbinCardArray[i - 1].BalanceQTY + sortedbinCardArray[i].ReceivedQTY;

                            sortedbinCardArray[i].BalanceQTY = totalQty;
                            sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
                            sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Issue"))
                        {
                            #region Void Issue

                            totalQty = sortedbinCardArray[i - 1].BalanceQTY + sortedbinCardArray[i].ReceivedQTY;

                            sortedbinCardArray[i].BalanceQTY = totalQty;
                            sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
                            sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Transfer Send"))
                        {
                            #region Void Transfer Send

                            totalQty = sortedbinCardArray[i - 1].BalanceQTY + sortedbinCardArray[i].ReceivedQTY;

                            sortedbinCardArray[i].BalanceQTY = totalQty;
                            sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
                            sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            #endregion
                        }

                    }

                    else if (sortedbinCardArray[i].IssuedQTY > 0)
                    {
                        if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Issue") || sortedbinCardArray[i].ReferenceDoc.StartsWith("Transfer Send"))
                        {
                            #region issue

                            sortedbinCardArray[i].IssuedUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
                            sortedbinCardArray[i].IssuedTPrice = sortedbinCardArray[i].IssuedUPrice * sortedbinCardArray[i].IssuedQTY;

                            totalQty = sortedbinCardArray[i - 1].BalanceQTY - sortedbinCardArray[i].IssuedQTY;

                            sortedbinCardArray[i].BalanceQTY = totalQty;
                            sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
                            sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            #endregion
                        }
                        else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Receive"))
                        {
                            decimal totalpricebefore = 0;
                            #region Void Receive

                            totalQty = sortedbinCardArray[i - 1].BalanceQTY - sortedbinCardArray[i].IssuedQTY;
                            //get issued price from original record if possible
                            //totalpricebefore = sortedbinCardArray[i].BalanceTPrice -
                            //                   (sortedbinCardArray[i].IssuedQTY * sortedbinCardArray[i].IssuedUPrice);

                            //sortedbinCardArray[i].BalanceQTY = totalQty;
                            //sortedbinCardArray[i].BalanceUPrice = totalQty == 0 ? 0 : totalpricebefore / totalQty;
                            //sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            sortedbinCardArray[i].BalanceQTY = totalQty;
                            sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
                            sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            #endregion
                        }

                        else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Transfer Receive"))
                        {
                            decimal totalpricebefore = 0;

                            #region Void Transfer Receive

                            totalQty = sortedbinCardArray[i - 1].BalanceQTY - sortedbinCardArray[i].IssuedQTY;
                            //get issued price from original record if possible
                            //totalpricebefore = sortedbinCardArray[i].BalanceTPrice -
                            //                   (sortedbinCardArray[i].IssuedQTY * sortedbinCardArray[i].IssuedUPrice);

                            //sortedbinCardArray[i].BalanceQTY = totalQty;
                            //sortedbinCardArray[i].BalanceUPrice = totalQty == 0 ? 0 : totalpricebefore / totalQty;
                            //sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            sortedbinCardArray[i].BalanceQTY = totalQty;
                            sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
                            sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            #endregion
                        }
                        else if (sortedbinCardArray[i].ReferenceDoc.StartsWith("Void Return"))
                        {
                            #region Void Return

                            totalQty = sortedbinCardArray[i - 1].BalanceQTY - sortedbinCardArray[i].IssuedQTY;

                            sortedbinCardArray[i].BalanceQTY = totalQty;
                            sortedbinCardArray[i].BalanceUPrice = sortedbinCardArray[i - 1].BalanceUPrice;
                            sortedbinCardArray[i].BalanceTPrice = sortedbinCardArray[i].BalanceUPrice * sortedbinCardArray[i].BalanceQTY;

                            #endregion
                        }
                    }

                    #endregion

                    _db.Entry(sortedbinCardArray[i]).State = EntityState.Modified;
                    _db.SaveChanges();
                }

            }

            return View("Success");
        }
      
        public string IsBank(string ac)
        {
            return _accountHelper.IsBankAccount(ac).ToString();
        }

        public ActionResult ResetCache()
        {
            CacheHelper.ResetCache();
            return View("Success");
        }

        public ActionResult Role()
        {
            ApplicationRoleManager roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            ApplicationRole xx0 = new ApplicationRole("Admin", "Administrator", "Global");
            roleManager.Create(xx0);

            ApplicationRole xx1 = new ApplicationRole("Construction_Admin", "Construction Administrator", "Construction");
            roleManager.Create(xx1);

            ApplicationRole xx3 = new ApplicationRole("AR_SalesOrder", "Sales order create", "Finance");
            roleManager.Create(xx3);

            ApplicationRole xx4 = new ApplicationRole("AR_SalesOrder_view", "Sales order view", "Finance");
            roleManager.Create(xx4);

            ApplicationRole xx5 = new ApplicationRole("AR_SalesOrder_Approver", "Sales order approver", "Finance");
            roleManager.Create(xx5);

            return View("Success");
            
        }

        public ActionResult Roles()
        {
            ApplicationRoleManager roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            ApplicationRole xx0 = new ApplicationRole("Property_StoreKeeper", "Store Keeper", "Property");
            ApplicationRole xx = new ApplicationRole("User_Department", "Approve store requisitions", "Property");
            ApplicationRole yy = new ApplicationRole("Property_Count_User", "Physical count", "Property");
            ApplicationRole zz = new ApplicationRole("Property_Count_Approver", "Physical count Approver", "Property");
            ApplicationRole pp = new ApplicationRole("Insurance_Admin", "Manage settings and configurations", "Insurance");
            ApplicationRole cc = new ApplicationRole("Insurance_User", "Manage Insurance functionalities", "Insurance");

            ApplicationRole cc1 = new ApplicationRole("Procurement_Admin", "Manage Procurement setting", "Procurement");
            ApplicationRole cc2 = new ApplicationRole("Procurement_UserDepartment_User", "Service Requision and receive", "Procurement");
            ApplicationRole cc3 = new ApplicationRole("Procurment_UserDepartment_Approver", "Service Requision and receive", "Procurement");
            //ApplicationRole tt = new ApplicationRole("General_Ledger_Post_User", "Post approved journals", "Finance");
            //ApplicationRole qq = new ApplicationRole("General_SubLedger_Transfer", "Review and transfer sub ledger transactions to GL", "Finance");

            ApplicationRole qq = new ApplicationRole("HR_Recruitment_Requester", "Request Recruitment", "HR");
            ApplicationRole tt = new ApplicationRole("HR_Training_Requester", "Request Training", "HR");
            ApplicationRole xxx = new ApplicationRole("OverTime_Actual_Record_User ", "Overtime user", "Payroll");

            ApplicationRole zz1 = new ApplicationRole("Payroll_Input", "Manage Payroll input functionalities", "Payroll");

            ApplicationRole zz2 = new ApplicationRole("OverTime_Request_Approver", "Approve,Manage pend or reject all Over time requests", "Payroll");

            ApplicationRole zz3 = new ApplicationRole("OverTime_Request_User", "Request Over Time requests", "Payroll");

            ApplicationRole zz4 = new ApplicationRole("Payroll_Admin", "Manage Payroll Settings and configurations functionalities", "Payroll");
            ApplicationRole zz5 = new ApplicationRole("Payroll_Process", "Manage Payroll Process functionalities", "Payroll");

            ApplicationRole zz6 = new ApplicationRole("Payroll_Distribute", "Manage Payroll Distribution functionalities", "Payroll");

            ApplicationRole zz7 = new ApplicationRole("Payroll_Close", "Manage Payroll Closing functionalities", "Payroll");

            ApplicationRole zz8 = new ApplicationRole("Payroll_Transfer", "Manage Payroll Transfer functionalities", "Payroll");

            ApplicationRole zz9 = new ApplicationRole("Payroll_Approver", "Approve,Manage pend or reject all payroll requests", "Payroll");

            ApplicationRole zz10 = new ApplicationRole("Payroll_Report", "View payroll reports", "Payroll");

            ApplicationRole zz11 = new ApplicationRole("Payroll_View", "", "Payroll");

            ApplicationRole zz12 = new ApplicationRole("OverTime_Actual_Record_User", "Maintain Actual Employee over time record", "Payroll");

            ApplicationRole zz14 = new ApplicationRole("Procurement_UserDepartment_User", "User departments role", "Procurement");
            ApplicationRole zz15 = new ApplicationRole("Procurement_UserDepartment_Approver", "Approver of user departments", "Procurement");
            ApplicationRole zz15s = new ApplicationRole("Service_Requisition_Second_Approver", "Second level approver for service requisition", "Procurement");

            ApplicationRole zz142 = new ApplicationRole("Procurement_User", "Procurement User", "Procurement");
            ApplicationRole zz152 = new ApplicationRole("Procurement_Approver", "Approver of procurement activities", "Procurement");

            ApplicationRole zz16 = new ApplicationRole("Procurement_Bond_User", "Guarantee,bond and cpo", "Procurement");
            ApplicationRole zz17 = new ApplicationRole("Procurement_Bond_Approver", "Guarantee,bond and cpo approver", "Procurement");

            ApplicationRole zz18 = new ApplicationRole("Procurement_Supplier_User", "Supplier management user", "Procurement");
            ApplicationRole zz19 = new ApplicationRole("Procurement_Supplier_Approver", "Supplier management approver", "Procurement");

            ApplicationRole zz20 = new ApplicationRole("Procurement_Contract_User", "Contract management user", "Procurement");
            ApplicationRole zz21 = new ApplicationRole("Procurement_Contract_Approver", "Contract management approver", "Procurement");

            ApplicationRole zz22 = new ApplicationRole("Procurement_Plan_User", "Procurement need and plan management user", "Procurement");
            ApplicationRole zz23 = new ApplicationRole("Procurement_Plan_Approver", "Procurement need and plan management approver", "Procurement");

            ApplicationRole zz24 = new ApplicationRole("Procurement_Tender_User", "Tender,proforma and PO management user", "Procurement");
            ApplicationRole zz25 = new ApplicationRole("Procurement_Tender_Approver", "Tender,proforma and PO management user approver", "Procurement");

            ApplicationRole fa1 = new ApplicationRole("FixedAsset_Requisition_User", "Fixed asset requisition user", "Fixed Asset");
            ApplicationRole fa2 = new ApplicationRole("FixedAsset_Requisition_Approver", "Fixed asset requisition approver", "Fixed Asset");
            ApplicationRole fa2new = new ApplicationRole("FixedAsset_Requisition_Second_Approver", "Fixed asset requisition second approver", "Fixed Asset");
            ApplicationRole fa3 = new ApplicationRole("FixedAsset_User", "Fixed asset user", "Fixed Asset");
            ApplicationRole fa4 = new ApplicationRole("FixedAsset_Approver", "Fixed asset approver", "Fixed Asset");
            ApplicationRole fa5 = new ApplicationRole("FixedAsset_Authorizer", "Fixed Asset Authorizer", "Fixed Asset");
            ApplicationRole fa6 = new ApplicationRole("FixedAsset_Count_User", "Fixed asset count user", "Fixed Asset");
            ApplicationRole fa7 = new ApplicationRole("FixedAsset_Count_Approver", "Fixed Asset count approver", "Fixed Asset");
            ApplicationRole fa8 = new ApplicationRole("FixedAsset_Admin", "Fixed asset settings administrator", "Fixed Asset");
            ApplicationRole fa9 = new ApplicationRole("FixedAsset_Report", "Fixed Asset report user", "Fixed Asset");
            ApplicationRole fa10 = new ApplicationRole("FixedAsset_Import", "Fixed asset import", "Fixed Asset");
            ApplicationRole fa11 = new ApplicationRole("FixedAsset_StoreKeeper", "Fixed Asset store keeper", "Fixed Asset");
            ApplicationRole fa12 = new ApplicationRole("FixedAsset_Void", "Fixed asset void", "Fixed Asset");

            roleManager.Create(cc1);
            roleManager.Create(cc2);
            roleManager.Create(cc3);
            roleManager.Create(zz142);
            roleManager.Create(zz152);
            roleManager.Create(zz15s);

            roleManager.Create(fa1);
            roleManager.Create(fa2);
            roleManager.Create(fa2new);
            roleManager.Create(fa3);
            roleManager.Create(fa4);
            roleManager.Create(fa5);
            roleManager.Create(fa6);
            roleManager.Create(fa7);
            roleManager.Create(fa8);
            roleManager.Create(fa9);
            roleManager.Create(fa10);
            roleManager.Create(fa11);
            roleManager.Create(fa12);

            roleManager.Create(zz14);
            roleManager.Create(zz15);
            roleManager.Create(zz16);
            roleManager.Create(zz17);
            roleManager.Create(zz18);
            roleManager.Create(zz19);
            roleManager.Create(zz20);
            roleManager.Create(zz21);
            roleManager.Create(zz22);
            roleManager.Create(zz23);
            roleManager.Create(zz24);
            roleManager.Create(zz25);


            roleManager.Create(zz1);
            roleManager.Create(zz2);
            roleManager.Create(zz3);
            roleManager.Create(zz4);
            roleManager.Create(zz5);
            roleManager.Create(zz6);
            roleManager.Create(zz7);
            roleManager.Create(zz8);
            roleManager.Create(zz9);
            roleManager.Create(zz10);
            roleManager.Create(zz11);
            roleManager.Create(zz12);

            roleManager.Create(xxx);
            roleManager.Create(xx);
            roleManager.Create(xx0);
            roleManager.Create(yy);
            roleManager.Create(zz);

            roleManager.Create(pp);
            roleManager.Create(cc);
            roleManager.Create(tt);
            roleManager.Create(qq);

            return View("Success");
        }
        public ActionResult Roles3()
        {

            #region Role Insert

            ApplicationRoleManager roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();



            ApplicationRole b1 = new ApplicationRole("Budget_Admin", "Budget Administrator", "Budget");
            ApplicationRole b2 = new ApplicationRole("Budget_Approver", "Budget Approver", "Budget");
            ApplicationRole b3 = new ApplicationRole("Budget_Report", "Budget Report", "Budget");
            ApplicationRole b4 = new ApplicationRole("Budget_Requestor", "Budget Requester", "Budget");
            ApplicationRole b5 = new ApplicationRole("Budget_User", "Budget User", "Budget");

            roleManager.Create(b1);
            roleManager.Create(b2);
            roleManager.Create(b3);
            roleManager.Create(b4);
            roleManager.Create(b5);

            ApplicationRole c1 = new ApplicationRole("CRM_Admin", "CRM Administrator", "CRM");
            ApplicationRole c2 = new ApplicationRole("Sales_Report", "Sales Report", "CRM");
            ApplicationRole c3 = new ApplicationRole("Marketing_Report", "Marketing Report", "CRM");
            ApplicationRole c4 = new ApplicationRole("Service_Support_report", "Service Support_report", "CRM");
            ApplicationRole c5 = new ApplicationRole("Sales_Quote_User", "Sales Quote User", "CRM");
            ApplicationRole c6 = new ApplicationRole("Sales_Order_User", "Sales Order User", "CRM");
            ApplicationRole c7 = new ApplicationRole("Cash_Sales_User", "Cash Sales User", "CRM");
            ApplicationRole c8 = new ApplicationRole("Sales_Quote_Approver", "Sales Quote Approver", "CRM");
            ApplicationRole c9 = new ApplicationRole("Sales_Order_Approver", "Sales Order Approver", "CRM");
            ApplicationRole c10 = new ApplicationRole("Cash_Sales_Approver", "Cash Sales Approver", "CRM");
            ApplicationRole c11 = new ApplicationRole("Marketing_User", "Marketing User", "CRM");
            ApplicationRole c12 = new ApplicationRole("Service_Support_User", "Service_Support_User", "CRM");
            ApplicationRole c13 = new ApplicationRole("Inventory_Issue_User", "Inventory Issue User", "CRM");
            ApplicationRole c14 = new ApplicationRole("Inventory_Issue_Approver", "Inventory Issue Approver", "CRM");
            ApplicationRole c15 = new ApplicationRole("Customer_Service_User", "Customer Service User", "CRM");
            ApplicationRole c16 = new ApplicationRole("Customer_Service_Approver", "Customer Service Approver", "CRM");
            ApplicationRole c17 = new ApplicationRole("Customer_Service_Report", "Customer Service Report", "CRM");
            ApplicationRole c18 = new ApplicationRole("Credit_Settlement_User", "Credit Settlement User", "CRM");



            roleManager.Create(c1);
            roleManager.Create(c2);
            roleManager.Create(c3);
            roleManager.Create(c4);
            roleManager.Create(c5);
            roleManager.Create(c6);
            roleManager.Create(c7);
            roleManager.Create(c8);
            roleManager.Create(c9);
            roleManager.Create(c10);
            roleManager.Create(c11);
            roleManager.Create(c12);
            roleManager.Create(c13);
            roleManager.Create(c14);
            roleManager.Create(c15);
            roleManager.Create(c16);
            roleManager.Create(c17);
            roleManager.Create(c18);

            ApplicationRole f1 = new ApplicationRole("Fleet_Admin", "Fleet Administrator", "FLEET");
            ApplicationRole f2 = new ApplicationRole("Registration_User", "Registration User", "FLEET");
            ApplicationRole f3 = new ApplicationRole("Registration_Approver", "Registration Approver", "FLEET");
            ApplicationRole f4 = new ApplicationRole("Rent_Requestor", "Rent Requester", "FLEET");
            ApplicationRole f5 = new ApplicationRole("Rent_User", "Rent User", "FLEET");
            ApplicationRole f6 = new ApplicationRole("Rent_Approver", "Rent Approver", "FLEET");
            ApplicationRole f7 = new ApplicationRole("Maintenance_Requestor", "Maintenance Requester", "FLEET");
            ApplicationRole f8 = new ApplicationRole("Maintenance_Request_Approver", "Maintenance Request Approver", "FLEET");
            ApplicationRole f9 = new ApplicationRole("Maintenance_Order_User", "Maintenance Order User", "FLEET");
            ApplicationRole f10 = new ApplicationRole("Maintenance_Service_Approver", "Maintenance Service Approver", "FLEET");
            ApplicationRole f11 = new ApplicationRole("Service_User", "Service User", "FLEET");
            ApplicationRole f12 = new ApplicationRole("Fuel_Requestor", "Fuel Requester", "FLEET");
            ApplicationRole f13 = new ApplicationRole("Fuel_Approver", "Fuel Approver", "FLEET");
            ApplicationRole f14 = new ApplicationRole("Fuel_User", "Fuel User", "FLEET");
            ApplicationRole f15 = new ApplicationRole("Inspection_User", "Inspection User", "FLEET");
            ApplicationRole f16 = new ApplicationRole("Inspection_Approver", "Inspection Approver", "FLEET");
            ApplicationRole f17 = new ApplicationRole("Resource_Requestor", "Resource Requester", "FLEET");
            ApplicationRole f18 = new ApplicationRole("Resource_Distributor", "Resource Distributor", "FLEET");
            ApplicationRole f19 = new ApplicationRole("Resource_Return_User", "Resource Return_User", "FLEET");
            ApplicationRole f20 = new ApplicationRole("Resource_User", "Resource User", "FLEET");
            ApplicationRole f21 = new ApplicationRole("Resource_Request_Approver", "Resource Request Approver", "FLEET");
            ApplicationRole f22 = new ApplicationRole("Resource_Approver", "Resource Approver", "FLEET");
            ApplicationRole f23 = new ApplicationRole("Tyre_Requestor", "Tyre Requester", "FLEET");
            ApplicationRole f24 = new ApplicationRole("Tyre_Approver", "Tyre Approver", "FLEET");
            ApplicationRole f25 = new ApplicationRole("Tyre_User", "Tyre User", "FLEET");
            ApplicationRole f26 = new ApplicationRole("Fleet_Training_user", "Fleet Training user", "FLEET");
            ApplicationRole f27 = new ApplicationRole("Fleet_Maintenance_Report", "Fleet Maintenance Report", "FLEET");
            ApplicationRole f28 = new ApplicationRole("Fleet_Report", "Fleet Report", "FLEET");
            ApplicationRole f29 = new ApplicationRole("Fleet_Resource_Report", "Fleet Resource Report", "FLEET");
            ApplicationRole f30 = new ApplicationRole("Equipment_Plan_User", "Equipment Plan User", "FLEET");
            ApplicationRole f31 = new ApplicationRole("Equipment_Plan_Approver", "Equipment Plan Approver", "FLEET");
            ApplicationRole f32 = new ApplicationRole("Salvage_Store_Issue_User", "Salvage Store Issue User", "FLEET");
            ApplicationRole f33 = new ApplicationRole("Salvage_Store_Issue_Approver", "Salvage Store Issue Approver", "FLEET");
            ApplicationRole f34 = new ApplicationRole("Store_Good_Receiving_User", "Store Good Receiving User", "FLEET");
            ApplicationRole f35 = new ApplicationRole("Store_Good_Receiving_Approver", "Store Good Receiving Approver", "FLEET");
            ApplicationRole f36 = new ApplicationRole("Fleet_Registration_Report", "Fleet Registration Report", "FLEET");

            roleManager.Create(f1);
            roleManager.Create(f2);
            roleManager.Create(f3);
            roleManager.Create(f4);
            roleManager.Create(f5);
            roleManager.Create(f6);
            roleManager.Create(f7);
            roleManager.Create(f8);
            roleManager.Create(f9);
            roleManager.Create(f10);
            roleManager.Create(f11);
            roleManager.Create(f12);
            roleManager.Create(f13);
            roleManager.Create(f14);
            roleManager.Create(f15);
            roleManager.Create(f16);
            roleManager.Create(f17);
            roleManager.Create(f18);
            roleManager.Create(f19);
            roleManager.Create(f20);
            roleManager.Create(f21);
            roleManager.Create(f22);
            roleManager.Create(f23);
            roleManager.Create(f24);
            roleManager.Create(f25);
            roleManager.Create(f26);
            roleManager.Create(f27);
            roleManager.Create(f28);
            roleManager.Create(f29);
            roleManager.Create(f30);
            roleManager.Create(f31);
            roleManager.Create(f32);
            roleManager.Create(f33);
            roleManager.Create(f34);
            roleManager.Create(f35);
            roleManager.Create(f36);


            #endregion


            return View("Success");
        }
        public ActionResult UpdateBranchItems(int id)
        {
            InventoryItemService service=new InventoryItemService();
            service.UpdateAllBranchItems(id);
            return View("Success");
        }

        public ActionResult UpdateItems()
        {
            //InventoryHelper.UpdateAllInventoryItems();
            return View("Success");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

      

        public ActionResult HrDate()
        {
            var lines = _db.Person_Employee.ToList();
            foreach (var l in lines)
            {
                l.HiredDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(l.HiredDate.Day, l.HiredDate.Month, l.HiredDate.Year);
                _db.Entry(l).State = EntityState.Modified;
                _db.SaveChanges();
            }

            var placementHistories = _db.PlacementHistories.ToList();
            foreach (var l in placementHistories)
            {
                l.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(l.From.Day, l.From.Month, l.From.Year);
                l.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(l.To.Day, l.To.Month, l.To.Year);
                _db.Entry(l).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return View("Success");
        }


        public ActionResult HrNotifications()
        {
            #region Contract
            var contract = _db.Contract.OrderByDescending(x => x.To);
            foreach (var c in contract)
            {

                var fullname = c.Employee.FirstName + " " + c.Employee.MiddleName + " " + c.Employee.LastName;
               _dashboardHelper.AddNotofication(Request.UserHostAddress, "HR_Profile_Admin,HR_Admin", User.Identity.Name, c.To, "", fullname + " -Employee Promotion");
            }
            #endregion

            #region Promotion
            int promotionPerYear = 0;
            int checkDiffrence = 0;
            var configration = _db.Configurations.Where(x => x.Reference == "YEARLY_PROMOTION_PER_YEAR" && x.Attribute == "Promotion").FirstOrDefault();
            if (configration != null)
            {
                promotionPerYear = Convert.ToInt32(configration.CurrentValue);
            }
            var emplist = _db.Person_Employee.Where(x => x.IsActive).ToList();


            if (emplist.Any())
            {
                foreach (var e in emplist)
                {

                    var placements = _db.Placements.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent).FirstOrDefault();

                    if (placements != null)
                    {
                        checkDiffrence = DateTime.Now.Year - placements.AssignmentDate.Year;
                        if (checkDiffrence >= promotionPerYear)
                        {
                            var fullname = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                           _dashboardHelper.AddNotofication(Request.UserHostAddress, "HR_Placement_Admin,HR_Admin,HR_Placement_User", User.Identity.Name, DateTime.Now, "", fullname + " -Employee Contract");

                        }
                    }
                }
            }

            #endregion
            #region Termination
            var terminations = _db.Terminations.OrderByDescending(x => x.TerminationDate);
            foreach (var c in terminations)
            {

                var fullname = c.Employee.FirstName + " " + c.Employee.MiddleName + " " + c.Employee.LastName;
               _dashboardHelper.AddNotofication(Request.UserHostAddress, "HR_Termination_User,HR_Admin", User.Identity.Name, c.TerminationDate, "", fullname + " -Employee Termination");
            }
            #endregion

            #region Probation
            var probations = _db.Prohibitions.Where(x => x.LookupId == EmployeeStatus.PROBATION).OrderByDescending(x => x.EndDate);
            foreach (var c in probations)
            {

                var fullname = c.Employee.FirstName + " " + c.Employee.MiddleName + " " + c.Employee.LastName;
               _dashboardHelper.AddNotofication(Request.UserHostAddress, "HR_Recruitment_Admin,HR_Recruitment_User,HR_Admin", User.Identity.Name, c.EndDate, "", fullname + " -Employee Termination");
            }
            #endregion

            return View("Success");
        }

        #region finance
        public ActionResult SplitLedger()
        {
            _accountHelper.SplitLedgerAccountCombination();
            return View("Success");
        }

     

        public ActionResult SplitJournal()
        {
           
            _accountHelper.SplitJournalAccountCombination();
            return View("Success");
        }

        public ActionResult SplitBank()
        {

            _accountHelper.SplitBookAccountCombination();
            return View("Success");
        }

        public ActionResult SplitItemIndex()
        {
            var items = _db.InventoryItems.ToList();
            foreach (var item in items)
            {
                string[] codes = item.ItemCode.Split('-');
                if (codes.Length == 3)
                {
                    item.Prefix = codes[0] + "-" + codes[1];
                    item.ItemIndex = int.Parse(codes[2]);
                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                if (codes.Length == 2)
                {
                    item.Prefix = codes[0];
                    item.ItemIndex = int.Parse(codes[1]);
                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            return View("Success");
        }
        public ActionResult TakeToBeg(string id)
        {
            string searchKey = id + "-";
            var lines = _db.GLLedgerPostingLines.Where(x => x.JournalNumber.StartsWith(searchKey)).ToList();
            foreach (var l in lines)
            {
                if (l.GLLedgerPosting.AccountType == GlAccountTypes.Asset ||
                    l.GLLedgerPosting.AccountType == GlAccountTypes.Expense)
                {
                    l.Balance = l.Debit - l.Credit;
                }
                else if (l.GLLedgerPosting.AccountType == GlAccountTypes.Liability ||
                         l.GLLedgerPosting.AccountType == GlAccountTypes.Capital ||
                         l.GLLedgerPosting.AccountType == GlAccountTypes.Revenue)
                {
                    l.Balance = l.Credit - l.Debit;
                }

                l.Type = LedgerPostTypes.Begining;
                _db.Entry(l).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return View("Success");
        }
    
       
        //public ActionResult PostBegEnd(string id)
        //{
        //    if (!String.IsNullOrEmpty(id))
        //    {
        //        var ledgerPostings = _db.GLLedgerPostings.FirstOrDefault(x => x.Description == id);
        //        if (ledgerPostings != null) _financeHelper.UpdateBegEndBalances(ledgerPostings.Description);
        //        return View("Success");
        //    }
        //    return View("Error");
           
        //}

        //public ActionResult PostAllBegEnd()
        //{
        //    _financeHelper.UpdateAllBegEndBalances();
        //    return View("Success");

        //}

        
        public ActionResult UpdateBegEndFastP(string id)
        {
            _financeHelper.UpdateAllBegEndBalancesFastP(int.Parse(id),User.Identity.Name);
            return View("Success");

        }

        public ActionResult UpdateStandardFastP(string id)
        {
            _financeHelper.UpdateAllStandardBalancesFastP(int.Parse(id),User.Identity.Name);
            return View("Success");

        }

        public ActionResult UpdateStandardFastMp(string id)
        {
            _financeHelper.UpdateAllStandardBalancesMonthlyFastP(int.Parse(id), User.Identity.Name);
            return View("Success");

        }


        #endregion
        public ActionResult Test()
        {
           ExceedDbContext context = new ExceedDbContext();
            DateTime start = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                Notification objNotif = new Notification
                {
                    SentTo = "test",
                    Title = "test title",
                    Details = "test message",
                    Date = DateTime.Now,
                    DetailsUrl = "test url",
                    DeadLine = "test deadline"
                };
                context.SaveChanges();
                if(i%10000==0)
                    context = new ExceedDbContext();
                context.Notifications.Add(objNotif);
               
            }
           
            DateTime end = DateTime.Now;
            decimal t = (end - start).Seconds;
            ViewBag.Error = t;
            return View("error");
        }
    }
}