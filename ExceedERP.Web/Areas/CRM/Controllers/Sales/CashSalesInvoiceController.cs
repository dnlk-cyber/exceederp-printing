using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CashSalesUser, CRMRoles.CashSalesApprover)]
    public class CashSalesInvoiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly List<CashSalesInvoice> salesQuoteVoucherViews = new List<CashSalesInvoice>();
        public ExceedDbContext dbb = new ExceedDbContext();
        public ExceedDbContext dbbb = new ExceedDbContext();
        public List<LineItem> lineItems = new List<LineItem>();
        public ActionResult Index()
        {
            SetInitials();

            return View();
        }
        public void SetInitials()
        {
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["VoucherExtension"] = db.VoucherExtensions.ToList();
            ViewData["Thing"] = db.Products.Select(s => new { s.ProductID, Name = s.ItemCode + " " + s.Name }).ToList();
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
            ViewData["RepairTypes"] = db.RepairType.Select(r => new { RepairTypeId = r.RepairTypeID.ToString(), r.Name });
        }
        /// <summary>
        /// update store id of header from line items
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateEmptyStores()
        {
            //
            // assign store id from its line item
            var emptyStores = db.CashSalesInvoices.Where(s => s.StoreID == 0).ToList();
            foreach (var ci in emptyStores)
            {
                var lineItem = db.LineItems.FirstOrDefault(l => l.Voucher == ci.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2");
                if (lineItem != null)
                {
                    ci.StoreID = lineItem.StoreID;

                    db.CashSalesInvoices.Attach(ci);
                    db.Entry(ci).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            //
            InitializeInvoiceNumber();
            CheckAndUpdateHoldSales();
            return Redirect(nameof(Index));
        }
        public ActionResult UpdateEmptyBranches()
        {
            var emptyBranches = db.CashSalesInvoices.Where(i => i.BranchId == 0).ToList();
            foreach (var invoice in emptyBranches)
            {
                var recordStore = db.SalesStores.Find(invoice.StoreID);
                if (recordStore != null)
                {
                    invoice.BranchId = recordStore.InventoryStore.BranchId;

                    db.CashSalesInvoices.Attach(invoice);
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Redirect(nameof(Index));
        }
        [Authorize(Users = "tester")]
        public ActionResult AddDistribution()
        {
            var cashSalesInvoices = db.CashSalesInvoices.Where(cs => !cs.IsOnlineTransferred && cs.PaymentMethod == CRMConstants.CashPaymentMethod).ToList();
            foreach (CashSalesInvoice invoice in cashSalesInvoices)
            {
                new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(invoice);
            }
            return Redirect(nameof(Index));
        }
        private void InitializeInvoiceNumber()
        {
            var data = db.CashSalesInvoices.Where(c => c.PosPrintType == PosPrintType.CRM && c.InvoiceNumber == null).ToList();
            foreach (var invoice in data)
            {
                invoice.InvoiceNumber = invoice.CashSalesInvoiceID.ToString();

                db.CashSalesInvoices.Attach(invoice);
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private void CheckAndUpdateHoldSales()
        {
            var records = db.CashSalesInvoices.Where(c => c.IsReceiptPrinted && !c.IsSendForHold).ToList();
            if (records.Any())
            {
                foreach (CashSalesInvoice invoice in records)
                {
                    invoice.IsSendForHold = true;
                    invoice.LastObjectState = CRMConstants.PrintedSalesInvoiceStatus;

                    db.CashSalesInvoices.Attach(invoice);
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        [Authorize(Users = "tester")]
        public ActionResult UpdateEmptyPeriods()
        {
            var emptyPeriodInvoices = db.CashSalesInvoices.Where(c => c.CRMPeriodId == 0).ToList();
            if (emptyPeriodInvoices.Any())
            {
                foreach (var invoice in emptyPeriodInvoices)
                {
                    var matchingPeriod = db.CRMPeriods.FirstOrDefault(p => p.DateFrom <= invoice.Date && p.DateTo >= invoice.Date);
                    if (matchingPeriod != null)
                    {
                        invoice.CRMPeriodId = matchingPeriod.CRMPeriodId;

                        db.CashSalesInvoices.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return Redirect(nameof(Index));
        }
        [Authorize(Users = "tester")]
        public ActionResult UpdatePaymentMethodOfOfflineInvoices()
        {
            var offlineInvoices = db.CashSalesInvoices.Where(i => i.PosPrintType == PosPrintType.Offline).ToList();
            foreach (var invoice in offlineInvoices)
            {
                var assignedUser = db.SalesPersonAssignments.FirstOrDefault(a => a.SalesStoreId == invoice.StoreID);
                if (assignedUser != null)
                {
                    invoice.RecieveMethod = assignedUser.PaymentMethodId.ToString();

                    db.CashSalesInvoices.Attach(invoice);
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Redirect(nameof(Index));
        }
        [Authorize(Users = "tester")]
        public ActionResult UpdatePaymentMethodOfInvoicesWithNoPaymentMethod()
        {
            var emptyInvoices = db.CashSalesInvoices.Where(i => string.IsNullOrEmpty(i.RecieveMethod)).ToList();
            foreach (var invoice in emptyInvoices)
            {
                var assignedUser = db.SalesPersonAssignments.FirstOrDefault(a => a.SalesStoreId == invoice.StoreID);
                if (assignedUser != null)
                {
                    invoice.RecieveMethod = assignedUser.PaymentMethodId.ToString();

                    db.CashSalesInvoices.Attach(invoice);
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Redirect(nameof(Index));
        }
        public ActionResult PopulateSalseOrderVoucherForReference([DataSourceRequest]DataSourceRequest request, string salesOrderVoucherID)
        {
            db = new ExceedDbContext();
            decimal Grandtot = 0;

            SalesOrderVoucher sov = db.SalesOrderVouchers.Where(sq => sq.SalesOrderVoucherID.ToString() == salesOrderVoucherID).SingleOrDefault();
            List<LineItem> items = db.LineItems.Where(l => l.Voucher == salesOrderVoucherID && l.VoucherDefinitionID == "1" && (l.Status == CRMConstants.Pending || l.Status == CRMConstants.Rejected)).ToList();//.Sum(s => s.CalculatedCost);

            var AlreadyForwarded = db.CashSalesInvoices.Where(x => x.References == sov.SalesOrderVoucherID.ToString()).ToList();

            if (AlreadyForwarded.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", "The Voucher Already Forwarded!!!");
                return Json("The Voucher Already Forwarded!", JsonRequestBehavior.AllowGet);

            }

            foreach (var item in items)
            {
                Grandtot = Grandtot + item.TotalAmount;
            }
            if (Grandtot < 1)
            {
                ModelState.AddModelError("DuplicateIndexEntry", " Grand Total balance Must not be 0 !!!");
                return Json("Grand Total balance Must not be 0 !", JsonRequestBehavior.AllowGet);
            }
            else
            {
                db = new ExceedDbContext();
                int referedVoucherID;
                LineItem lineItem;
                VoucherTerm _voucherTerm;
                VoucherExtensionTransaction _voucherExtensionTransaction;
                var PaymentMethod = db.PaymentMethods.FirstOrDefault();
                var Recieve = string.Empty;

                if (PaymentMethod != null)
                {

                    Recieve = PaymentMethod.PaymentMethodID.ToString();
                }
                if (sov != null)
                {
                    var entity = new CashSalesInvoice
                    {
                        OrganizationCustomerId = sov.OrganizationCustomerId,
                        IssuedDate = sov.IssuedDate,
                        IsIssued = sov.IsIssued,
                        Date = sov.Date,
                        IsVoid = sov.IsVoid,
                        GrandTotal = sov.GrandTotal,
                        Period = sov.Period,
                        LastObjectState = sov.LastObjectState,
                        PaymentMethod = sov.PaymentMethod,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        References = salesOrderVoucherID,
                        RecieveMethod = Recieve,
                        Remark = sov.Remark
                    };

                    db.CashSalesInvoices.Add(entity);
                    db.SaveChanges();
                    referedVoucherID = entity.CashSalesInvoiceID;

                    List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == salesOrderVoucherID && Item.VoucherDefinitionID == "1" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();
                    foreach (LineItem Item in lineItemList)
                    {
                        lineItem = new LineItem();
                        //This may be amended to be the uptodated value
                        lineItem.CalculatedCost = Item.CalculatedCost;
                        lineItem.References = salesOrderVoucherID;
                        lineItem.PriceTag = Item.PriceTag;
                        lineItem.Quantity = Item.Quantity;
                        lineItem.Remark = Item.Remark;
                        lineItem.Class = Item.Class;
                        lineItem.Variety = Item.Variety;
                        //This may be amended to be the uptodated value
                        lineItem.StockBalance = Item.StockBalance;
                        //This may be attached with StoreTransaction table
                        lineItem.StoreID = Item.StoreID;
                        lineItem.Tax = Item.Tax;
                        lineItem.TaxAmount = Item.TaxAmount;
                        lineItem.ThingID = Item.ThingID;
                        lineItem.TotalAmount = Item.TotalAmount;
                        lineItem.UnitAmount = Item.UnitAmount;
                        lineItem.Voucher = referedVoucherID.ToString();
                        lineItem.VoucherDefinitionID = "2";
                        lineItem.WithholdingType = Item.WithholdingType;
                        lineItem.WithholdingAmount = Item.WithholdingAmount;
                        lineItem.WithholdTax = Item.WithholdTax;
                        lineItem.WithholdTaxAmount = Item.WithholdTaxAmount;
                        lineItem.Vat = Item.Vat;
                        lineItem.Status = CRMConstants.Pending;

                        db.LineItems.Add(lineItem);
                        db.SaveChanges();


                        //var AditonalCostlog = db.AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList();
                        //foreach (var log in AditonalCostlog)
                        //{
                        //    log.LineItemId = lineItem.LineItemID;
                        //    db.AdditionalCostLogs.Add(log);
                        //    db.SaveChanges();
                        //}

                    }

                    foreach (LineItem Item in lineItemList)
                    {
                        Item.Status = CRMConstants.Forwarded;
                        db.LineItems.Attach(Item);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    List<VoucherTerm> voucherTermList = db.VoucherTerms.Where(vt => vt.Voucher == sov.References).ToList();
                    foreach (VoucherTerm voucherTerm in voucherTermList)
                    {
                        _voucherTerm = new VoucherTerm();
                        _voucherTerm.Remark = voucherTerm.Remark;
                        _voucherTerm.Term = voucherTerm.Term;
                        _voucherTerm.Value = voucherTerm.Value;
                        _voucherTerm.Voucher = referedVoucherID.ToString();
                        _voucherTerm.VoucherDefinitionID = 2;
                        db.VoucherTerms.Add(_voucherTerm);
                        db.SaveChanges();
                    }
                    List<VoucherExtensionTransaction> voucherExtensionTransactionList = db.VoucherExtensionTransactions.Where(ve => ve.Voucher == sov.References).ToList();
                    foreach (VoucherExtensionTransaction voucherExtensionTrxn in voucherExtensionTransactionList)
                    {
                        _voucherExtensionTransaction = new VoucherExtensionTransaction();
                        _voucherExtensionTransaction.Number = voucherExtensionTrxn.Number;
                        _voucherExtensionTransaction.Remark = voucherExtensionTrxn.Remark;
                        _voucherExtensionTransaction.Voucher = referedVoucherID.ToString();

                        _voucherExtensionTransaction.VoucherExtension = voucherExtensionTrxn.VoucherExtension;
                        db.VoucherExtensionTransactions.Add(_voucherExtensionTransaction);
                        db.SaveChanges();
                    }

                }
                ExceedDbContext dbb = new ExceedDbContext();
                SalesOrderVoucher sovup = dbb.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == salesOrderVoucherID).FirstOrDefault();
                sovup.LastObjectState = CRMConstants.Forwarded;
                dbb.SalesOrderVouchers.Attach(sovup);
                dbb.Entry(sovup).State = EntityState.Modified;
                dbb.SaveChanges();
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddSalesInvoiceFromReference(CashSalesInvoice invoice)
        {
            db = new ExceedDbContext();
            decimal Grandtot = 0;

            SalesOrderVoucher sov = db.SalesOrderVouchers.Where(sq => sq.SalesOrderVoucherID.ToString() == invoice.References).SingleOrDefault();
            List<LineItem> items = db.LineItems.Where(l => l.Voucher == invoice.References && l.VoucherDefinitionID == "1").ToList();//.Sum(s => s.CalculatedCost);
            List<ServiceLineItem> serviceLines = db.ServiceLineItems.Where(l => l.VoucherId.ToString() == invoice.References && l.VoucherType == CRMVoucherType.SalesOrder).ToList();//.Sum(s => s.CalculatedCost);

            var AlreadyForwarded = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID != invoice.CashSalesInvoiceID && x.References == sov.SalesOrderVoucherID.ToString()).ToList();

            if (AlreadyForwarded.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", "This Sales Order Voucher is  already used in other Invoice");
                var inv = db.CashSalesInvoices.Find(invoice.CashSalesInvoiceID);
                if (inv != null)
                {
                    db.CashSalesInvoices.Attach(inv);
                    db.CashSalesInvoices.Remove(inv);
                    db.SaveChanges();
                }
                return Json("This Sales Order Voucher is  already used in other Invoice!", JsonRequestBehavior.AllowGet);

            }

            //foreach (var item in items)
            //{
            //    Grandtot = Grandtot + item.TotalAmount;
            //}
            if (serviceLines.Any())
                Grandtot += serviceLines.Sum(s => s.TotalAmount);
            if (items.Any())
                Grandtot += items.Sum(i => i.TotalAmount);
            if (Grandtot <= 0)
            {
                ModelState.AddModelError("DuplicateIndexEntry", "Grand Total of Referenced Sales Order Should be greater than  0. No Line items found.");
                var inv = db.CashSalesInvoices.Find(invoice.CashSalesInvoiceID);
                if (inv != null)
                {
                    db.CashSalesInvoices.Attach(inv);
                    db.CashSalesInvoices.Remove(inv);
                    db.SaveChanges();
                }
                return Json("Grand Total of Referenced Sales Order Should be greater than  0. No Line items found.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                db = new ExceedDbContext();
                //int referedVoucherID;
                //LineItem lineItem;
                //VoucherTerm _voucherTerm;
                //VoucherExtensionTransaction _voucherExtensionTransaction;
                var PaymentMethod = db.PaymentMethods.FirstOrDefault();
                var Recieve = string.Empty;

                if (PaymentMethod != null)
                {
                    Recieve = PaymentMethod.PaymentMethodID.ToString();
                }
                if (sov != null)
                {
                    //var entity = new CashSalesInvoice
                    //{
                    //    OrganizationCustomerId = sov.OrganizationCustomerId,
                    //    IssuedDate = sov.IssuedDate,
                    //    IsIssued = sov.IsIssued,
                    //    Date = sov.Date,
                    //    IsVoid = sov.IsVoid,
                    //    GrandTotal = sov.GrandTotal,
                    //    Period = sov.Period,
                    //    LastObjectState = sov.LastObjectState,
                    //    PaymentMethod = sov.PaymentMethod,
                    //    CreatedBy = User.Identity.Name,
                    //    DateCreated = DateTime.Now,
                    //    References = salesOrderVoucherID,
                    //    RecieveMethod = Recieve,
                    //    Remark = sov.Remark
                    //};

                    //db.CashSalesInvoices.Add(entity);
                    //db.SaveChanges();
                    //referedVoucherID = entity.CashSalesInvoiceID;

                    //List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == invoice.References && Item.VoucherDefinitionID == "1" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();
                    List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == sov.SalesOrderVoucherID.ToString() && Item.VoucherDefinitionID == "1").ToList();
                    foreach (LineItem Item in lineItemList)
                    {
                        var lineItem = new LineItem();
                        //This may be amended to be the uptodated value
                        lineItem.CalculatedCost = Item.CalculatedCost;
                        lineItem.ShipmentNumber = Item.ShipmentNumber;
                        lineItem.StoreRequisitionNumber = Item.StoreRequisitionNumber;
                        lineItem.References = Item.LineItemID.ToString();
                        lineItem.PriceTag = Item.PriceTag;
                        lineItem.Quantity = Item.Quantity;
                        lineItem.Remark = Item.Remark;
                        lineItem.Class = Item.Class;
                        lineItem.Variety = Item.Variety;
                        //This may be amended to be the uptodated value
                        lineItem.StockBalance = Item.StockBalance;
                        //This may be attached with StoreTransaction table
                        lineItem.StoreID = Item.StoreID;
                        lineItem.Tax = Item.Tax;
                        lineItem.TaxAmount = Item.TaxAmount;
                        lineItem.ThingID = Item.ThingID;
                        lineItem.TotalAmount = Item.TotalAmount;
                        lineItem.UnitAmount = Item.UnitAmount;
                        lineItem.Voucher = invoice.CashSalesInvoiceID.ToString();
                        lineItem.VoucherDefinitionID = "2";
                        lineItem.WithholdingType = Item.WithholdingType;
                        lineItem.WithholdingAmount = Item.WithholdingAmount;
                        lineItem.WithholdTax = Item.WithholdTax;
                        lineItem.WithholdTaxAmount = Item.WithholdTaxAmount;
                        lineItem.Vat = Item.Vat;
                        lineItem.Status = CRMConstants.Pending;

                        db.LineItems.Add(lineItem);
                        db.SaveChanges();


                        //var AditonalCostlog = db.AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList();
                        //foreach (var log in AditonalCostlog)
                        //{
                        //    log.LineItemId = lineItem.LineItemID;
                        //    db.AdditionalCostLogs.Add(log);
                        //    db.SaveChanges();
                        //}

                    }

                    #region Services
                    var services = db.ServiceLineItems.Where(s => s.VoucherType == CRMVoucherType.SalesOrder && s.VoucherId == sov.SalesOrderVoucherID).ToList();
                    foreach (var service in services)
                    {
                        var salesOrderService = new ServiceLineItem
                        {
                            VoucherId = invoice.CashSalesInvoiceID,
                            VoucherType = CRMVoucherType.SalesInvoice,
                            ServiceTypeId = service.ServiceTypeId,
                            ServiceTypeCode = service.ServiceTypeCode,
                            Quantity = service.Quantity,
                            UnitAmount = service.UnitAmount,
                            SubTotal = service.SubTotal,
                            TaxTypeId = service.TaxTypeId,
                            TaxAmount = service.TaxAmount,
                            WithholdTax = service.WithholdTax,
                            WithholdingTypeId = service.WithholdingTypeId,
                            WithholdingAmount = service.WithholdingAmount,
                            TotalAmount = service.TotalAmount,
                            WithholdTaxAmount = service.WithholdTaxAmount,

                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            LastModified = null,
                            ModifiedBy = null
                        };
                        db.ServiceLineItems.Add(salesOrderService);
                        db.SaveChanges();
                    }
                    #endregion


                }


                new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(invoice);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CashSalesInvoices_Read([DataSourceRequest]DataSourceRequest request, bool showTransferableOnly = false)
        {
            var assignedStores = db.SalesPersonAssignments.Where(a => a.UserName == User.Identity.Name).Select(a => a.SalesStoreId).ToList();

            var invoices = dbbb.CashSalesInvoices.Where(d => assignedStores.Contains(d.StoreID) && d.PaymentMethod == CRMConstants.CashPaymentMethod).ToList();

            if (showTransferableOnly)
            {
                invoices = invoices.Where(i => i.IsOnlineApproved && !i.IsOnlineTransferred).ToList();
            }

            DataSourceResult result = invoices.ToDataSourceResult(request, vou => new CashSalesInvoice
            {
                CashSalesInvoiceID = vou.CashSalesInvoiceID,
                Period = vou.Period,
                OrganizationCustomerId = vou.OrganizationCustomerId,
                Date = vou.Date,
                GrandTotal = vou.GrandTotal,
                Source = vou.Source,
                ShipmentGenerationMethod = vou.ShipmentGenerationMethod,
                IsIssued = vou.IsIssued,
                AdvancPayment = vou.AdvancPayment,
                OnHandPayment = vou.OnHandPayment,
                IssuedDate = vou.IssuedDate,
                IsVoid = vou.IsVoid,
                References = vou.References,
                CRMPeriodId = vou.CRMPeriodId,
                BranchId = vou.BranchId,
                StoreID = vou.StoreID,
                PaymentTermsID = vou.PaymentTermsID,
                ReceiptReference = vou.ReceiptReference,
                IsManual = vou.IsManual,
                IsPaidWithCash = vou.IsPaidWithCash,

                IsSendForApproval = vou.IsSendForApproval,
                PaymentMethod = vou.PaymentMethod,
                Remark = vou.Remark,
                LastObjectState = vou.LastObjectState,
                RecieveMethod = vou.RecieveMethod,
                ModifiedBy = vou.ModifiedBy,
                LastModified = vou.LastModified,
                DateCreated = vou.DateCreated,
                CreatedBy = vou.CreatedBy,
                IsOnlineApproved = vou.IsOnlineApproved,
                IsOnlineTransferred = vou.IsOnlineTransferred,
                IsOnlineTransferredBy = vou.IsOnlineTransferredBy,
                OnlineApprovedBy = vou.OnlineApprovedBy,
                OnlineApprovedTime = vou.OnlineApprovedTime,
                OnlineTransferredTime = vou.OnlineTransferredTime,
                OrgBranchName = vou.OrgBranchName,
                SendForApprovalBy = vou.SendForApprovalBy,
                SendForApprovalTime = vou.SendForApprovalTime,
                Status = vou.LastObjectState,
                //IsFromMaintenance = vou.IsFromMaintenance,
                JobOrderNumber = vou.JobOrderNumber,
                PlateNo = vou.PlateNo,

                IsSendForHold = vou.IsSendForHold,
                SendForHoldTime = vou.SendForHoldTime,
                SendForHoldBy = vou.SendForHoldBy,
                IsReceiptPrinted = vou.IsReceiptPrinted,
                InvoicePrintTime = vou.InvoicePrintTime,
                InvoicePrintedBy = vou.InvoicePrintedBy,
                InvoiceNumber = vou.InvoiceNumber,
                FsInvoiceNo = vou.FsInvoiceNo,
                MachineID = vou.MachineID,
                PosPrintType = vou.PosPrintType,
                CustomerTINNumber = vou.OrganizationCustomer.TinNo,
            });
            return Json(result);

        }
        public ActionResult DuplicateSalesInvoices_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var assignedStores = db.SalesPersonAssignments.Where(a => a.UserName == User.Identity.Name).Select(a => a.SalesStoreId).ToList();

            var duplicateGroup = db.CashSalesInvoices.GroupBy(c => new { c.InvoiceNumber, c.MachineID, c.FsInvoiceNo })
                                            .Where(i => i.Count() > 1)
                                            .Select(i => new { i.Key.InvoiceNumber, i.Key.MachineID })
                                            .ToList();
            var invNubers = new HashSet<string>(duplicateGroup.Select(g => g.InvoiceNumber));

            var invoices = dbbb.CashSalesInvoices.Where(d => invNubers.Contains(d.InvoiceNumber)).ToList();

            //if (showTransferableOnly)
            //{
            //    invoices = invoices.Where(i => i.IsOnlineApproved && !i.IsOnlineTransferred).ToList();
            //}
            //List<CashSalesInvoice> listToReturn = new List<CashSalesInvoice>();
            //foreach (var invoice in invoices)
            //{
            //    var shipmentRecord = db.SalesShipments.Any()
            //}
            DataSourceResult result = invoices.ToDataSourceResult(request, vou => new CashSalesInvoice
            {
                CashSalesInvoiceID = vou.CashSalesInvoiceID,
                Period = vou.Period,
                OrganizationCustomerId = vou.OrganizationCustomerId,
                Date = vou.Date,
                GrandTotal = vou.GrandTotal,
                Source = vou.Source,
                ShipmentGenerationMethod = vou.ShipmentGenerationMethod,
                IsIssued = vou.IsIssued,
                AdvancPayment = vou.AdvancPayment,
                OnHandPayment = vou.OnHandPayment,
                IssuedDate = vou.IssuedDate,
                IsVoid = vou.IsVoid,
                References = vou.References,
                CRMPeriodId = vou.CRMPeriodId,
                BranchId = vou.BranchId,
                StoreID = vou.StoreID,
                PaymentTermsID = vou.PaymentTermsID,
                ReceiptReference = vou.ReceiptReference,
                IsManual = vou.IsManual,
                IsPaidWithCash = vou.IsPaidWithCash,

                IsSendForApproval = vou.IsSendForApproval,
                PaymentMethod = vou.PaymentMethod,
                Remark = vou.Remark,
                LastObjectState = vou.LastObjectState,
                RecieveMethod = vou.RecieveMethod,
                ModifiedBy = vou.ModifiedBy,
                LastModified = vou.LastModified,
                DateCreated = vou.DateCreated,
                CreatedBy = vou.CreatedBy,
                IsOnlineApproved = vou.IsOnlineApproved,
                IsOnlineTransferred = vou.IsOnlineTransferred,
                IsOnlineTransferredBy = vou.IsOnlineTransferredBy,
                OnlineApprovedBy = vou.OnlineApprovedBy,
                OnlineApprovedTime = vou.OnlineApprovedTime,
                OnlineTransferredTime = vou.OnlineTransferredTime,
                OrgBranchName = vou.OrgBranchName,
                SendForApprovalBy = vou.SendForApprovalBy,
                SendForApprovalTime = vou.SendForApprovalTime,
                Status = vou.LastObjectState,
                //IsFromMaintenance = vou.IsFromMaintenance,
                JobOrderNumber = vou.JobOrderNumber,

                IsSendForHold = vou.IsSendForHold,
                SendForHoldTime = vou.SendForHoldTime,
                SendForHoldBy = vou.SendForHoldBy,
                IsReceiptPrinted = vou.IsReceiptPrinted,
                InvoicePrintTime = vou.InvoicePrintTime,
                InvoicePrintedBy = vou.InvoicePrintedBy,
                InvoiceNumber = vou.InvoiceNumber,
                FsInvoiceNo = vou.FsInvoiceNo,
                MachineID = vou.MachineID,
                PosPrintType = vou.PosPrintType,
                CustomerTINNumber = vou.OrganizationCustomer.TinNo,

                CanBeDeletedIfDuplicate = !vou.IsOnlineTransferred
            });
            return Json(result);

        }

        public ActionResult PrintPos(string InvoiceId)
        {
            CRMPOSHelper CRMPOSHelper = new CRMPOSHelper();
            var CashSalesInvoice = db.CashSalesInvoices.Find(int.Parse(InvoiceId));

            if (CashSalesInvoice != null && !CashSalesInvoice.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }



            if (CashSalesInvoice != null && CashSalesInvoice.IsReceiptPrinted)
            {
                var formattedData = new
                {
                    Message = "The receiet is already generated.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }




            if (CashSalesInvoice != null)
            {
                try
                {
                    CRMPOSHelper.GenerateCashSalePrintXml(CashSalesInvoice, false, 0);
                    var formattedData = new
                    {
                        Message = "Successfully Generated.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    var formattedData = new
                    {
                        Message = "The operation failed,try again.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }
            return null;

        }
        public ActionResult PrintPartialPos(string InvoiceId, string PartialId)
        {
            CRMPOSHelper CRMPOSHelper = new CRMPOSHelper();
            var CashSalesInvoice = db.CashSalesInvoices.Find(int.Parse(InvoiceId));
            if (CashSalesInvoice != null && !CashSalesInvoice.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not sent for Payment.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var CashPartialItem = db.CashSalesPartialItems.Where(x => x.CashSalesPartialItemId.ToString() == PartialId).FirstOrDefault();

            if (CashPartialItem != null && CashPartialItem.IsReceiptPrinted)
            {
                var formattedData = new
                {
                    Message = "The receiet is already generated.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }




            if (CashSalesInvoice != null)
            {
                try
                {
                    CRMPOSHelper.GenerateCashSalePrintXml(CashSalesInvoice, true, int.Parse(PartialId));
                    var formattedData = new
                    {
                        Message = "Successfully Generated.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    var formattedData = new
                    {
                        Message = "The operation failed,try again.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }
            return null;

        }


        public ActionResult CashSalesInvoiceDistrubtions_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            //var CashSalesInvoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == Id).FirstOrDefault();
            var distributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == Id).OrderByDescending(d => d.Debit).ToList();
            if (distributions.Any())
            {

                return Json(distributions.Select(s => new SalesInvoicePaymentDistibution
                {
                    SalesInvoicePaymentDistibutionId = s.SalesInvoicePaymentDistibutionId,
                    AccountCode = s.AccountCode,
                    AccountDesc = s.AccountDesc,
                    Credit = s.Credit,
                    Debit = s.Debit,
                }
                ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }

            return Json(new { }, JsonRequestBehavior.AllowGet);


        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesInvoices_Create([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            if (cashSalesInvoice.OrganizationCustomerId == 0 && string.IsNullOrEmpty(cashSalesInvoice.NewCustomer))
            {
                ModelState.AddModelError("", "Select Existing customer or Insert Name of new one");
            }
            if (cashSalesInvoice.IsManual && string.IsNullOrEmpty(cashSalesInvoice.ReceiptReference))
            {
                ModelState.AddModelError("", "Receipt reference number is required for manual purchase invoices");
            }
            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (cashSalesInvoice.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CRMSalesHelper.CreateTempCustomer(cashSalesInvoice.NewCustomer, cashSalesInvoice.CustomerTINNumber);
                }
                else
                {
                    OrgCustomer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == cashSalesInvoice.OrganizationCustomerId).FirstOrDefault();
                }
                string Payment = CRMConstants.CashPaymentMethod;


                var entity = new CashSalesInvoice
                {

                    OrganizationCustomerId = OrgCustomer.OrganizationCustomerID,
                    IssuedDate = cashSalesInvoice.IssuedDate,
                    IsIssued = cashSalesInvoice.IsIssued,
                    Date = cashSalesInvoice.Date,
                    CRMPeriodId = cashSalesInvoice.CRMPeriodId,
                    BranchId = cashSalesInvoice.BranchId,
                    StoreID = cashSalesInvoice.StoreID,
                    PaymentTermsID = cashSalesInvoice.PaymentTermsID,
                    ReceiptReference = cashSalesInvoice.ReceiptReference,
                    IsManual = cashSalesInvoice.IsManual,
                    IsPaidWithCash = cashSalesInvoice.IsPaidWithCash,
                    IsVoid = cashSalesInvoice.IsVoid,
                    GrandTotal = cashSalesInvoice.GrandTotal,
                    Period = cashSalesInvoice.Period,
                    LastObjectState = CRMVoucherStatusConstatns.Editing,
                    PaymentMethod = Payment,
                    RecieveMethod = cashSalesInvoice.RecieveMethod,
                    Remark = cashSalesInvoice.Remark,
                    //References = CRMConstants.NoReference,
                    Source = cashSalesInvoice.Source,
                    ShipmentGenerationMethod = cashSalesInvoice.ShipmentGenerationMethod,

                    References = cashSalesInvoice.References,

                    FsInvoiceNo = cashSalesInvoice.FsInvoiceNo,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };
                if (entity.Source == SalesInvoiceSource.SalesOrder)
                {
                    // create reference to sales JO (if any) from sales order
                    var salesOrder = db.SalesOrderVouchers.FirstOrDefault(q => q.SalesOrderVoucherID.ToString() == entity.References);
                    if (salesOrder != null && (!string.IsNullOrEmpty(salesOrder.JobOrderNumber)))
                    {
                        entity.JobOrderNumber = salesOrder.JobOrderNumber;
                        entity.IsShipped = salesOrder.IsShipped; // if sales order is from maintenance, its already shipped
                        //entity.IsFromMaintenance = true;
                        entity.PlateNo = salesOrder.PlateNo;
                    }
                }

                db.CashSalesInvoices.Add(entity);
                db.SaveChanges();
                cashSalesInvoice.CashSalesInvoiceID = entity.CashSalesInvoiceID;

                entity.InvoiceNumber = entity.CashSalesInvoiceID.ToString();
                db.CashSalesInvoices.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                if (entity.Source == SalesInvoiceSource.SalesOrder)
                {
                    AddSalesInvoiceFromReference(entity);
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Cash Sales Invoice Create", "Success", User.Identity.Name, $"Cash Sales Invoice {entity.CashSalesInvoiceID} Created.", Modules.CRM);
                #endregion
                return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
            }
            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesInvoices_Update([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            if (cashSalesInvoice.OrganizationCustomerId == 0 && string.IsNullOrEmpty(cashSalesInvoice.NewCustomer))
            {
                ModelState.AddModelError("", "Select Existing customer or Insert Name of new one");
            }
            if (cashSalesInvoice.IsManual && string.IsNullOrEmpty(cashSalesInvoice.ReceiptReference))
            {
                ModelState.AddModelError("", "Receipt reference number is required for manual purchase invoices");
            }
            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (cashSalesInvoice.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CRMSalesHelper.CreateTempCustomer(cashSalesInvoice.NewCustomer);
                }
                else
                {
                    OrgCustomer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == cashSalesInvoice.OrganizationCustomerId).FirstOrDefault();
                }

                string Payment = CRMConstants.CashPaymentMethod;

                var entity = new CashSalesInvoice
                {
                    CashSalesInvoiceID = cashSalesInvoice.CashSalesInvoiceID,
                    OrganizationCustomerId = OrgCustomer.OrganizationCustomerID,
                    IssuedDate = cashSalesInvoice.IssuedDate,
                    IsIssued = cashSalesInvoice.IsIssued,
                    Date = cashSalesInvoice.Date,
                    IsVoid = cashSalesInvoice.IsVoid,
                    GrandTotal = cashSalesInvoice.GrandTotal,
                    Period = cashSalesInvoice.Period,
                    CRMPeriodId = cashSalesInvoice.CRMPeriodId,
                    BranchId = cashSalesInvoice.BranchId,
                    StoreID = cashSalesInvoice.StoreID,
                    PaymentTermsID = cashSalesInvoice.PaymentTermsID,
                    ReceiptReference = cashSalesInvoice.ReceiptReference,
                    IsManual = cashSalesInvoice.IsManual,
                    IsPaidWithCash = cashSalesInvoice.IsPaidWithCash,
                    LastObjectState = cashSalesInvoice.LastObjectState,
                    PaymentMethod = Payment,
                    RecieveMethod = cashSalesInvoice.RecieveMethod,
                    References = cashSalesInvoice.References,
                    Remark = cashSalesInvoice.Remark,
                    CreatedBy = cashSalesInvoice.CreatedBy,
                    DateCreated = cashSalesInvoice.DateCreated,
                    Source = cashSalesInvoice.Source,
                    ShipmentGenerationMethod = cashSalesInvoice.ShipmentGenerationMethod,
                    //IsFromMaintenance = cashSalesInvoice.IsFromMaintenance,
                    JobOrderNumber = cashSalesInvoice.JobOrderNumber,
                    PlateNo = cashSalesInvoice.PlateNo,
                    EJNo = cashSalesInvoice.EJNo,

                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,


                    IsSendForHold = cashSalesInvoice.IsSendForHold,
                    SendForHoldTime = cashSalesInvoice.SendForHoldTime,
                    SendForHoldBy = cashSalesInvoice.SendForHoldBy,
                    IsReceiptPrinted = cashSalesInvoice.IsReceiptPrinted,
                    InvoicePrintTime = cashSalesInvoice.InvoicePrintTime,
                    InvoicePrintedBy = cashSalesInvoice.InvoicePrintedBy,
                    InvoiceNumber = cashSalesInvoice.InvoiceNumber,
                    FsInvoiceNo = cashSalesInvoice.FsInvoiceNo,
                    MachineID = cashSalesInvoice.MachineID,
                    PosPrintType = cashSalesInvoice.PosPrintType,
                };

                db.CashSalesInvoices.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                if (entity.Source == SalesInvoiceSource.SalesOrder)
                {
                    new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(entity);
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Cash Sales Invoice Edit", "Success", User.Identity.Name, $"Cash Sales Invoice {entity.CashSalesInvoiceID} Updated.", Modules.CRM);
                #endregion
            }

            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesInvoice_Destroy([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            if (ModelState.IsValid)
            {
                var invoice = db.CashSalesInvoices.Find(cashSalesInvoice.CashSalesInvoiceID);
                if (invoice != null)
                {
                    db.CashSalesInvoices.Attach(invoice);
                    db.CashSalesInvoices.Remove(invoice);
                    var result = db.SaveChanges();
                    if (result == 1)
                    {
                        db.LineItems.Where(l => l.VoucherDefinitionID == "2" && l.Voucher == cashSalesInvoice.CashSalesInvoiceID.ToString()).ToList();
                        if (lineItems.Any())
                        {
                            db.LineItems.RemoveRange(lineItems);
                            db.SaveChanges();
                        }

                        #region delete log
                        UserHelper.DeleteLog(Request.UserHostAddress, "Cash Sales Invoice Delete", "Success", User.Identity.Name, $"Cash Sales Invoice {invoice.CashSalesInvoiceID} deleted.");
                        #endregion
                    }
                }
            }

            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        //public async Task<ActionResult> PrintInvoiceToPos(InvoicePosPrintBindingViewModel model)
        public async Task<ActionResult> PrintInvoiceToPos(string selectedInvoiceId, string baseUrl, string posId, string userName, string password)
        {
            var model = new InvoicePosPrintBindingViewModel
            {
                Id = selectedInvoiceId,
                BaseURL = baseUrl,
                PosId = posId,
                UserName = userName,
                Password = password
            };
            if (string.IsNullOrEmpty(selectedInvoiceId) ||
                string.IsNullOrEmpty(baseUrl) ||
                string.IsNullOrEmpty(posId) ||
                string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Missing_Fields", "Missing Fields");
                return Json(new { Message = "Please fill Required Fields.", Status = "Error" });

            }
            //var data = "Invoice Printed Successfully";
            //bool operationSucceeded = false;
            var currentInvoice = await db.CashSalesInvoices.FirstOrDefaultAsync(inv => inv.CashSalesInvoiceID.ToString() == model.Id);
            if (currentInvoice == null)
            {
                ModelState.AddModelError("Not_Found", "Invoice Not Found");
                return Json(new { Message = "Invoice not Found.", Status = "Error" });
            }
            else
            {
                if (!currentInvoice.IsOnlineApproved)
                {
                    ModelState.AddModelError("Not_Approved", "Invoice Not Approved. Operation Canceled");
                    return Json(new { Message = "Invoice Not Approved. Operation Canceled", Status = "Error" });
                }
                if (currentInvoice.IsSendForHold)
                {
                    ModelState.AddModelError("OnHold", "Invoice is Sent to POS and its on hold");
                    return Json(new { Message = "Invoice is Sent to POS and its on hold", Status = "Error" });

                }
                if (currentInvoice.IsReceiptPrinted)
                {
                    ModelState.AddModelError("Printed", "Invoice Printed already");
                    return Json(new { Message = "Invoice Printed already", Status = "Error" });

                }
                if (string.IsNullOrEmpty(model.BaseURL))
                {
                    ModelState.AddModelError("BaseURL_Not_Set", "Base URL is empty. Please Provide one");
                    return Json(new { Message = "Base URL is empty. Please Provide one", Status = "Error" });
                }
            }
            if (ModelState.IsValid)
            {
                var completeUrl = model.BaseURL + "/PEDS/API/HoldSalesService/Add";
                #region create content
                var record = new HoldSales
                {
                    //HoldSalesIdentifierId = "inv-" + currentInvoice.CashSalesInvoiceID,
                    HoldSalesIdentifierId = currentInvoice.InvoiceNumber,
                    TransactionType = "0",// Sales
                    InvoiceNo = currentInvoice.InvoiceNumber,
                    PaymentType = "0", // cash 
                    SalesPerson = currentInvoice.CreatedBy,
                    Date = currentInvoice.Date,
                    CustomerName = currentInvoice.OrganizationCustomer.TradeName,
                    CustomerTIN = "",
                    CustomerVAT = "",
                    HoldMemo = "",
                    TableNumber = "",
                    CashierUpdated = User.Identity.Name,
                    POSId = model.PosId,
                };
                var lineItems = db.LineItems.Where(l => l.VoucherDefinitionID == "2" && l.Voucher == currentInvoice.CashSalesInvoiceID.ToString()).ToList();
                if (lineItems.Any())
                {
                    foreach (LineItem item in lineItems)
                    {
                        var product = item.Product;
                        var holdSaleItem = new HoldSalesItem
                        {
                            HoldSalesItemIdentifierId = record.HoldSalesIdentifierId + "-" + product.ItemCode,
                            //CategoryIdentifierId = product.ItemCategoryCode,
                            CategoryIdentifierId = product.ItemCategoryCode,
                            CategoryName = GetProductCategory(product.CategoryID),
                            ItemIdentifierId = product.ItemCode,
                            //ItemIdentifierId = "i" + product.ProductID,
                            ItemDescription = product.Name,
                            ItemCode = product.ItemCode,
                            //ItemCode = product.ProductID.ToString(),
                            UomIdentifierId = product.UOM,
                            UomName = product.UOM,
                            Quantity = item.Quantity,
                            SalesUnitPrice = item.UnitAmount,
                            TaxType = item.TaxAmount > 0 ? 1 : 4,
                        };
                        holdSaleItem.CategoryIdentifierId = GetInventoryCategoryCode(holdSaleItem.CategoryName);
                        record.HoldSalesItems.Add(holdSaleItem);
                    }
                }
                var stringPayload = JsonConvert.SerializeObject(record);
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                #endregion
                using (HttpClient client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(completeUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    //var byteArray = Encoding.ASCII.GetBytes("username:password1234");
                    var byteArray = Encoding.ASCII.GetBytes(model.UserName + ":" + model.Password);
                    AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Authorization = authHeader;
                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(completeUrl, httpContent);

                        // If the response contains content we want to read it!
                        if (response.IsSuccessStatusCode)
                        {
                            string operationDescription = "Send POST Request to " + completeUrl +
                                                           "to Print Invoice with id " + selectedInvoiceId +
                                                           "POS ID " + posId +
                                                           "User Name " + userName;

                            UserHelper.OperationLog(Request.UserHostAddress, "POST Api Request", "Success", User.Identity.Name, operationDescription, Modules.CRM);

                            if (response.Content != null)
                            {
                                var responseContent = await response.Content.ReadAsStringAsync();

                                // From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net

                                HoldSalesResponseViewModel returnedObject = JsonConvert.DeserializeObject<HoldSalesResponseViewModel>(responseContent);
                                //foreach (var returnedObject in returnedObjects)
                                //{
                                if (returnedObject.Success)
                                {
                                    //operationSucceeded = true;
                                    currentInvoice.LastObjectState = CRMConstants.HoldSalesInvoiceStatus;
                                    currentInvoice.IsSendForHold = true;
                                    currentInvoice.SendForHoldBy = User.Identity.Name;
                                    currentInvoice.SendForHoldTime = DateTime.Now;


                                    db.CashSalesInvoices.Attach(currentInvoice);
                                    db.Entry(currentInvoice).State = EntityState.Modified;
                                    db.SaveChanges();

                                    return Json(new
                                    {
                                        Message = returnedObject.Message,
                                        Status = "Ok"
                                    });
                                }
                                else
                                {
                                    ModelState.AddModelError("", returnedObject.Message);
                                    return Json(new
                                    {
                                        Message = returnedObject.Message,
                                        Status = "Error"
                                    });
                                }
                                //}
                            }
                        }
                        else
                        {
                            //return Json(new
                            //{
                            //    //Message = response.Message,
                            //    Status = "Error"
                            //});
                        }
                    }
                    catch (Exception exception)
                    {
                        var msg = exception.Message;
                        if (exception.InnerException != null)
                        {
                            msg = exception.InnerException.Message;
                            if (exception.InnerException.InnerException != null)
                            {
                                msg = exception.InnerException.InnerException.Message;
                            }
                        }

                        #region log error
                        string operationDescription = "Send POST Request to " + completeUrl +
                                                          " to Print Invoice with id " + selectedInvoiceId +
                                                          " POS ID " + posId +
                                                          " User Name " + userName +
                                                          " Resulted in Error with Exception Message: " + msg;


                        UserHelper.OperationLog(Request.UserHostAddress, "POST Api Request", "Error", User.Identity.Name, operationDescription, Modules.CRM);

                        #endregion

                        return Json(new
                        {
                            Message = msg,
                            Status = "Error"
                        });
                    }

                }
            }
            return Redirect(nameof(Index));
        }
        public async Task<ActionResult> SyncInvoicesFromPos(InvoiceSyncFromPosBindingViewModel model)
        {

            if (DateTime.Now <= model.FromTime)
            {
                ModelState.AddModelError("Invalid_Range", "Invalid Time Range");
            }
            if (ModelState.IsValid)
            {
                var completeUrl = model.BaseURL + "/PEDS/API/HoldSalesService/GetSalesByTime";

                using (HttpClient client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(completeUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    //var byteArray = Encoding.ASCII.GetBytes("username:password1234");
                    var byteArray = Encoding.ASCII.GetBytes(model.UserName + ":" + model.Password);
                    AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    HttpContent httpContent = new StringContent((DateTime.Now - model.FromTime).Minutes.ToString());

                    HttpResponseMessage response = await client.PostAsync(completeUrl, httpContent);

                    // If the response contains content we want to read it!
                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                        // From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net

                        PosSyncViewModel returnedObject = JsonConvert.DeserializeObject<PosSyncViewModel>(responseContent);
                        if (returnedObject.Success)
                        {
                            // process the returned result
                            if (returnedObject.Content.Any())
                            {
                                foreach (PosSalesDataViewModel salesData in returnedObject.Content)
                                {
                                    var currentOrganization = db.OrganizationCustomers.FirstOrDefault(c => c.TradeName.ToLower() == salesData.CustomerName.ToLower());
                                    if (currentOrganization == null)
                                    {
                                        currentOrganization = CRMSalesHelper.CreateTempCustomer(salesData.CustomerName);
                                    }

                                    var paymentType = "";
                                    if (salesData.PaymentType == "0")
                                    {
                                        paymentType = CRMConstants.CashPaymentMethod;
                                    }
                                    else if (salesData.PaymentType == "1")
                                    {
                                        paymentType = "CHECK";
                                    }
                                    else if (salesData.PaymentType == "2")
                                    {
                                        paymentType = CRMConstants.CreditPaymentMethod;
                                    }
                                    #region Invoice header
                                    CashSalesInvoice invoiceToAdd = new CashSalesInvoice
                                    {
                                        Date = DateTime.Now,
                                        PaymentMethod = paymentType,
                                        Source = SalesInvoiceSource.Manual,
                                        IsOnlineApproved = true,
                                        OnlineApprovedBy = User.Identity.Name,
                                        OnlineApprovedTime = DateTime.Now,
                                        DueDate = DateTime.Now,
                                        IsReceiptPrinted = true,
                                        IsSendForApproval = true,
                                        SendForApprovalBy = User.Identity.Name,
                                        SendForApprovalTime = DateTime.Now,
                                        LastObjectState = CRMConstants.Approved,
                                        Remark = "Synced from POS with machine Id : " + salesData.MachineID,
                                        OrganizationCustomerId = currentOrganization.OrganizationCustomerID,
                                        ReceiptReference = salesData.InvoiceNumber,
                                        Status = CRMConstants.Approved,


                                        CreatedBy = User.Identity.Name,
                                        DateCreated = DateTime.Now,
                                    };
                                    db.CashSalesInvoices.Add(invoiceToAdd);
                                    db.SaveChanges();

                                    #endregion
                                    #region Invoice line items
                                    if (salesData.SalesDataDetails.Any())
                                    {
                                        var defaultStore = db.SalesStores.FirstOrDefault();
                                        foreach (PosSalesDataDetailViewModel item in salesData.SalesDataDetails)
                                        {
                                            var product = db.Products.FirstOrDefault(p => p.Name.ToLower() == item.ItemName.ToLower());
                                            if (product != null && defaultStore != null)
                                            {
                                                var lineItemToAdd = new LineItem
                                                {
                                                    VoucherDefinitionID = "2",
                                                    Voucher = invoiceToAdd.CashSalesInvoiceID.ToString(),
                                                    ThingID = product.ProductID,
                                                    UnitAmount = item.ItemUnitPrice,
                                                    Quantity = item.Quantity,
                                                    CalculatedCost = item.TotalItemBeforeTax,
                                                    Discount = item.TotalItemDiscount,
                                                    TotalAmount = item.TotalAmount,
                                                    StoreID = defaultStore.SalesStoreId,
                                                    TaxAmount = item.ItemTax,

                                                };
                                                db.LineItems.Add(lineItemToAdd);
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", returnedObject.Message);
                        }
                    }
                }
            }

            return Redirect(nameof(Index));
        }
        public ActionResult Vouchers_Reject(ExceedERP.Web.Areas.CRM.Models.BindingModel model)
        {
            CashSalesInvoice csi = db.CashSalesInvoices.Where(so => so.CashSalesInvoiceID.ToString() == model.Id).FirstOrDefault();
            if (csi != null)
            {
                //SalesOrderVoucher sov = db.SalesOrderVouchers.Where(x => x.SalesOrderVoucherID.ToString() == csi.References).FirstOrDefault();

                //if (sov != null)
                //{
                //    List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == model.Id && Item.VoucherDefinitionID == "2").ToList();
                //    foreach (var item in lineItemList)
                //    {

                //        db.LineItems.Attach(item);
                //        db.LineItems.Remove(item);
                //        db.SaveChanges();


                //        //var AditonalCostlog = db.AdditionalCostLogs.Where(x => x.LineItemId == item.LineItemID).ToList();
                //        //foreach (var log in AditonalCostlog)
                //        //{
                //        //    db.AdditionalCostLogs.Attach(log);
                //        //    db.AdditionalCostLogs.Remove(log);
                //        //    db.SaveChanges();
                //        //}
                //    }

                //    db.CashSalesInvoices.Attach(csi);
                //    db.CashSalesInvoices.Remove(csi);
                //    db.SaveChanges();



                //}


                //sov.LastObjectState = CRMConstants.Rejected;
                //sov.Remark = model.Name;
                //sov.LastModified = DateTime.Now;
                //sov.ModifiedBy = User.Identity.Name;
                //db.SalesOrderVouchers.Attach(sov);
                //db.Entry(sov).State = EntityState.Modified;
                //db.SaveChanges();

                csi.IsSendForApproval = false;
                csi.Status = "Rejected";
                csi.LastObjectState = CRMConstants.Rejected;

                db.CashSalesInvoices.Attach(csi);
                db.Entry(csi).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashInvoicePaymentProcess(CashPaymentViewModel CashPaymentViewModel)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();
            try
            {
                var CashSalesInvoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID.ToString() == CashPaymentViewModel.Id).FirstOrDefault();

                var GLTaxRates = db.GLTaxRates.ToList();
                var PaymentMethods = db.PaymentMethods.ToList();
                var AdditionalCostLogs = db.AdditionalCostLogs.ToList();
                var AdditionalCosts = db.AdditionalCosts.ToList();
                var AccountMaps = db.AccountMaps.ToList();
                var AdvancePaymentLines = db.AdvancePaymentLines.ToList();
                var AdvancePayments = db.AdvancePayments.ToList();

                if (CashPaymentViewModel.PaymentType == decimal.Zero.ToString())
                {

                    List<LineItem> InvoiceItems =
                                           db.LineItems.Where(
                                               l => l.Voucher == CashSalesInvoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2" && l.Status == CRMConstants.Pending)
                                               .ToList();
                    if (InvoiceItems.Any())
                    {
                        foreach (var InvoiceItem in InvoiceItems)
                        {


                            InvoiceItem.Status = CRMConstants.Paid;

                            db.LineItems.Attach(InvoiceItem);
                            db.Entry(InvoiceItem).State = EntityState.Modified;
                            db.SaveChanges();

                            //var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCashInvoice(CashSalesInvoice, InvoiceItem, AdditionalCosts, AdditionalCostLogs, GLTaxRates, PaymentMethods, AccountMaps);

                            //if (SalesInvoicePaymentDistibutions.Any())
                            //{
                            //    db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                            //    db.SaveChanges();
                            //}

                            //var ItemCategory = db.ItemCategorys.Where(x => x.ItemCategoryCode == InvoiceItem.Product.ItemCategoryCode).FirstOrDefault();

                            //if (ItemCategory != null)
                            //{
                            //    SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeProduct(CashSalesInvoice, ItemCategory, InvoiceItem, AccountMaps);

                            //    if (SalesInvoicePaymentDistibutions.Any())
                            //    {
                            //        db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                            //        db.SaveChanges();
                            //    }
                            //}
                        }

                        var AdvanceResult = CRMSalesHelper.ProcessAdvancePayment(CashSalesInvoice, InvoiceItems, AdvancePaymentLines, AdvancePayments);

                        if (AdvanceResult != null && AdvanceResult.Item1 != null && AdvanceResult.Item1.Any())
                        {
                            ExceedDbContext exceedDb = new ExceedDbContext();
                            foreach (var item in AdvanceResult.Item1)
                            {
                                var Adlines = exceedDb.AdvancePaymentLines.Where(x => x.AdvancePaymentLineId == item.AdvancePaymentLineId).FirstOrDefault();
                                if (Adlines != null)
                                {
                                    Adlines.Value = item.Amount;
                                    exceedDb.AdvancePaymentLines.Attach(Adlines);
                                    exceedDb.Entry(Adlines).State = System.Data.Entity.EntityState.Modified;
                                    exceedDb.SaveChanges();
                                }
                            }
                            // db.AdvancePaymentLogs.AddRange(AdvanceResult.Item1);

                            db.SaveChanges();
                        }

                        CashSalesInvoice.LastObjectState = CRMConstants.PaymentReceived;



                        CashSalesInvoice.OnHandPayment = InvoiceItems.Sum(x => x.TotalAmount) - AdvanceResult.Item2;
                        CashSalesInvoice.AdvancPayment = AdvanceResult.Item2;
                        CashSalesInvoice.IsOnlineApproved = true;
                        CashSalesInvoice.OnlineApprovedBy = User.Identity.Name;
                        CashSalesInvoice.OnlineApprovedTime = DateTime.Now;

                        db.CashSalesInvoices.Attach(CashSalesInvoice);
                        db.Entry(CashSalesInvoice).State = EntityState.Modified;
                        db.SaveChanges();


                    }


                }
                else
                {
                    var CashSalesPartialItemsall = db.CashSalesPartialItems.Where(x => x.CashSalesInvoiceId == CashSalesInvoice.CashSalesInvoiceID).ToList();

                    var CashSalesPartialItems = db.CashSalesPartialItems.Where(x => x.CashSalesInvoiceId == CashSalesInvoice.CashSalesInvoiceID && x.IsPaymentProcessed == false).ToList();
                    decimal OnHandPayment = decimal.Zero;
                    if (CashSalesPartialItems.Any())
                    {
                        foreach (var ParialItem in CashSalesPartialItems)
                        {

                            if (ParialItem.CashSalesPartialLineItem.Any())
                            {

                                foreach (var InvoiceItem in ParialItem.CashSalesPartialLineItem.DistinctByField(x => x.LineItemID))
                                {

                                    OnHandPayment += (InvoiceItem.LineItem.TotalAmount / (InvoiceItem.LineItem.CalculatedCost / InvoiceItem.LineItem.UnitAmount)) * ParialItem.CashSalesPartialLineItem.Where(x => x.LineItemID == InvoiceItem.LineItemID).Sum(x => x.PartialPayedAmount);

                                    //   InvoiceItem.LineItem.Quantity = 

                                    //var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCashInvoice(CashSalesInvoice, InvoiceItem.LineItem, AdditionalCosts, AdditionalCostLogs, GLTaxRates, PaymentMethods, AccountMaps);

                                    //if (SalesInvoicePaymentDistibutions.Any())
                                    //{
                                    //    db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                                    //    db.SaveChanges();
                                    //}

                                    //var ItemCategory = db.ItemCategorys.Where(x => x.ItemCategoryCode == InvoiceItem.LineItem.Product.ItemCategoryCode).FirstOrDefault();

                                    //if (ItemCategory != null)
                                    //{
                                    //    SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeProduct(CashSalesInvoice, ItemCategory, InvoiceItem.LineItem, AccountMaps);

                                    //    if (SalesInvoicePaymentDistibutions.Any())
                                    //    {
                                    //        db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                                    //        db.SaveChanges();
                                    //    }
                                    //}

                                    var LineItem = db.LineItems.Where(x => x.LineItemID == InvoiceItem.LineItem.LineItemID).FirstOrDefault();
                                    LineItem.Status = CRMConstants.Paid;
                                    LineItem.Quantity = CashSalesPartialItemsall.Where(x => x.CashSalesPartialLineItem.Where(y => y.LineItemID == InvoiceItem.LineItemID).Any()).Sum(x => x.CashSalesPartialLineItem.Sum(p => p.PartialPayedAmount));

                                    db.LineItems.Attach(LineItem);
                                    db.Entry(LineItem).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                ParialItem.IsPaymentProcessed = true;

                                db.CashSalesPartialItems.Attach(ParialItem);
                                db.Entry(ParialItem).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        CashSalesInvoice.LastObjectState = CRMConstants.PartialPaymentReceived;
                        CashSalesInvoice.OnHandPayment = CashSalesInvoice.OnHandPayment + OnHandPayment;

                        CashSalesInvoice.IsOnlineApproved = true;
                        CashSalesInvoice.OnlineApprovedBy = User.Identity.Name;
                        CashSalesInvoice.OnlineApprovedTime = DateTime.Now;

                        db.CashSalesInvoices.Attach(CashSalesInvoice);
                        db.Entry(CashSalesInvoice).State = EntityState.Modified;
                        db.SaveChanges();




                    }
                }
            }



            catch (Exception e)
            {
                string ee = e.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult UndoForOnlineApproval(string Id)
        {
            var Invoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID.ToString() == Id).FirstOrDefault();
            if (Invoice != null)
            {
                Invoice.Status = CRMConstants.UnForwarded;
                Invoice.IsSendForApproval = false;
                Invoice.SendForApprovalBy = User.Identity.Name;
                Invoice.SendForApprovalTime = DateTime.Now;

                if (Invoice != null)
                {

                    db.CashSalesInvoices.Attach(Invoice);
                    db.Entry(Invoice).State = EntityState.Modified;
                    db.SaveChanges();


                }
            }
            return RedirectToAction("Index");

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessPayment(string Id)
        {

            try
            {
                CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
                CashSalesInvoice SalesInvoice = db.CashSalesInvoices.Where(c => c.CashSalesInvoiceID.ToString() == Id).FirstOrDefault();

                var GLTaxRates = db.GLTaxRates.ToList();
                var PaymentMethods = db.PaymentMethods.ToList();
                var AdditionalCostLogs = db.AdditionalCostLogs.ToList();
                var AdditionalCosts = db.AdditionalCosts.ToList();
                var AccountMaps = db.AccountMaps.ToList();


                //     OrganizationCustomer OrgCustomer = db.OrganizationCustomers.Where(o => o.TradeName == SalesInvoice.Consignee).FirstOrDefault();

                List<LineItem> InvoiceItems =
                            db.LineItems.Where(
                                l => l.Voucher == SalesInvoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2" && l.Status == CRMConstants.Pending)
                                .ToList();
                //  var AdvancePayment = db.AdvancePayments.Where(x => x.Customer == SalesInvoice.Consignee).FirstOrDefault();

                if (InvoiceItems.Any())
                {
                    foreach (var InvoiceItem in InvoiceItems)
                    {

                        InvoiceItem.Status = CRMConstants.Paid;

                        db.LineItems.Attach(InvoiceItem);
                        db.Entry(InvoiceItem).State = EntityState.Modified;
                        db.SaveChanges();

                        //var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCashInvoice(SalesInvoice, InvoiceItem, AdditionalCosts, AdditionalCostLogs, GLTaxRates, PaymentMethods, AccountMaps);

                        //if (SalesInvoicePaymentDistibutions.Any())
                        //{
                        //    db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                        //    db.SaveChanges();
                        //}
                    }




                    SalesInvoice.LastObjectState = CRMConstants.PaymentReceived;

                    dbb.CashSalesInvoices.Attach(SalesInvoice);
                    dbb.Entry(SalesInvoice).State = EntityState.Modified;
                    dbb.SaveChanges();


                }


            }



            catch (Exception)
            {

            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        //public OrganizationCustomer CreateTempCustomer(string Name)
        //{
        //    var OrgCustomer = new OrganizationCustomer();

        //    var OrgCustomerCategory = db.OrganizationCategories.FirstOrDefault();
        //    if (OrgCustomerCategory != null)
        //    {
        //        OrgCustomer.BrandName = Name;
        //        OrgCustomer.TradeName = Name;
        //        OrgCustomer.Category = OrgCustomerCategory.OrganizationCategoryID.ToString();
        //        OrgCustomer.BusinessType = "Business Type";
        //        OrgCustomer.CustomerType = CustomerType.LOCAL;
        //        OrgCustomer.IsTemporary = true;
        //        OrgCustomer.IsActive = false;
        //        OrgCustomer.IsCredit = false;

        //        db.OrganizationCustomers.Add(OrgCustomer);
        //        int result = db.SaveChanges();

        //        if (result == 1)
        //            return OrgCustomer;
        //    }
        //    return null;
        //}
        public string GetStatus(int Id)
        {

            CashSalesInvoice Invoice = db.CashSalesInvoices.Where(j => j.CashSalesInvoiceID == Id).FirstOrDefault();
            if (Invoice != null)
            {
                return Invoice.LastObjectState;
            }
            else
            {
                return CRMConstants.Pending;
            }

        }
        public ActionResult GetCustomerInvoiceDDBind(string Customer)
        {

            var invoices =
                db.CashSalesInvoices
                .Where(
                    x => x.OrganizationCustomerId.ToString() == Customer &&
                    x.AmountDue > 0 &&
                    x.PaymentMethod == "CREDIT" &&
                    (x.LastObjectState == CRMConstants.PaymentReceived || x.LastObjectState == CRMConstants.ProductIssued || x.LastObjectState == CRMConstants.Forwarded)
                    ).ToList();
            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> paymentMethod = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();

            if (invoices.Any())
            {

                foreach (var invoice in invoices)
                {

                    var lines =
                        db.LineItems.Where(
                            x => x.Voucher == invoice.CashSalesInvoiceID.ToString() && x.VoucherDefinitionID == "2")
                            .ToList();
                    if (lines.Any())
                    {
                        if (lines.Sum(x => x.TotalAmount) > 0)
                        {
                            paymentMethod.Add(new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject { Name = invoice.CashSalesInvoiceID.ToString(), Id = invoice.CashSalesInvoiceID });

                        }

                    }
                }


            }


            return Json(paymentMethod, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCustomerInvoiceBalanceDDBind(string Voucher)
        {

            decimal RemainingAmount = decimal.Zero;
            var SettlementLine = db.LineCreditSettlements.Where(x => x.InvoiceId.ToString() == Voucher).ToList();

            var lines =
                db.LineItems.Where(
                    x => x.Voucher.ToString() == Voucher && x.VoucherDefinitionID == "2").ToList();
            RemainingAmount = lines.Sum(x => x.TotalAmount) - SettlementLine.Sum(x => x.SettelmentAmount);
            if (lines.Any())
            {
                return Json(new ExceedERP.Web.Areas.CRM.Models.BindingModel { Name = RemainingAmount.ToString(), Id = RemainingAmount.ToString() }, JsonRequestBehavior.AllowGet);
            }

            return Json(0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendForOnlineApproval(string Id)
        {
            var Invoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID.ToString() == Id).FirstOrDefault();
            if (Invoice != null)
            {
                var LineItems = db.LineItems.Where(x => x.VoucherDefinitionID == "2" && x.Voucher == Invoice.CashSalesInvoiceID.ToString()).ToList();
                var services = db.ServiceLineItems.Where(x => x.VoucherType == CRMVoucherType.SalesInvoice && x.VoucherId == Invoice.CashSalesInvoiceID).ToList();
                if (LineItems.Any() || services.Any())
                {
                    Invoice.LastObjectState = CRMVoucherStatusConstatns.WaitingApproval;
                    Invoice.IsSendForApproval = true;
                    Invoice.SendForApprovalBy = User.Identity.Name;
                    Invoice.SendForApprovalTime = DateTime.Now;

                    if (Invoice != null)
                    {

                        db.CashSalesInvoices.Attach(Invoice);
                        db.Entry(Invoice).State = EntityState.Modified;
                        db.SaveChanges();


                    }
                }
            }
            return RedirectToAction("Index");

        }

        [AuthorizeRoles(CRMRoles.CRMVoid)]
        public JsonResult Void(int id)
        {
            var record = db.CashSalesInvoices.Find(id);

            if (record != null && record.IsOnlineApproved /*&& !record.IsVoid */&& !record.IsOnlineTransferred)
            {
                record.Status = OperationStatuses.Void.ToString();
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.LastObjectState = OperationStatuses.Void.ToString();
                db.CashSalesInvoices.Attach(record);
                db.Entry(record).State = EntityState.Modified;


                #region remove validation lines
                var validationLine = db.CashSalesInvoiceValidation.Where(l => l.CashSalesInvoiceId == record.CashSalesInvoiceID).ToList();
                db.CashSalesInvoiceValidation.RemoveRange(validationLine);
                #endregion

                #region change line item status
                var invoiceLineItems = db.LineItems.Where(l => l.VoucherDefinitionID == "2" && l.Voucher == record.CashSalesInvoiceID.ToString()).ToList();
                foreach (var item in invoiceLineItems)
                {
                    item.Status = CRMConstants.Pending;

                    db.LineItems.Attach(item);
                    db.Entry(item).State = EntityState.Modified;
                }
                #endregion

                db.SaveChanges();

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

            var lineItems = db.LineItems.Where(l => l.Voucher == record.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2").ToList();
            if (!lineItems.Any())
            {
                var formattedData = new
                {
                    Message = "Please add lines and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            //if (record.IsVoid)
            //{
            //    var formattedData = new
            //    {
            //        Message = "The record is already void. You can not void again.",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
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
        public JsonResult RegenerateDistribution(int id)
        {
            var record = db.CashSalesInvoices.Find(id);
            if ((record != null && !record.IsOnlineTransferred))
            {
                //new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(record);
                new CRMPaymentDistibutionHelper().UpdateCashSalesInvoiceDistribution(record);
                return Json(new { Message = $"Distribution Generated for Invoice number {record.CashSalesInvoiceID}.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Invoice not found!",
                    Status = "NOT_OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is already transfered you cannot regenerate distribution.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = db.CashSalesInvoices.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var formattedData = new
                {
                    Message = "The record is already transffered, it can't be retransferred.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

                #endregion

            }
            if (record != null && !record.IsOnlineTransferred)
            {

                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                var distributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == record.CashSalesInvoiceID).ToList();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                //var distribution = record.ArSalesOrderDistribution.ToList();
                if (distributions.Any())
                {
                    debit = distributions.Sum(x => x.Debit);
                    credit = distributions.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit <= 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit should be positive number.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period
                var period = db.GLPeriods.Where(x => x.DateFrom <= record.Date.Date)
                    .Where(x => x.DateTo >= record.Date.Date).ToList();
                if (period.Any())
                {
                    if (period.Count() == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name.ToString();
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod +
                                              " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var formattedData = new
                            {
                                Message = "Period not found.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the Invoice date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There are no periods defined that contain the Sales Invoice date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion

                if (distributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "CRM Sales Invoice",
                        Description = !String.IsNullOrEmpty(description) ? description : "CRM Sales Invoice",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "CRM Sales Invoice",
                        SourcesName = "CRM",
                        JournalNumber = "CRMSALINV." + record.CashSalesInvoiceID.ToString(),
                        SourceId = record.CashSalesInvoiceID.ToString(),
                        Source = JournalSources.Sales.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId = record.BranchId,
                        Status = "Approved",
                        Remark = record.ReceiptReference,
                        PaymentReference = record.ReceiptReference,
                        ReferenceDoc = "",
                        InvoiceReferenceDoc = "",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distributions)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    record.OnlineTransferredTime = DateTime.Now;
                    record.IsOnlineTransferredBy = User.Identity.Name;

                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion

                    #region bank reconciliation
                    //CRMSalesHelper.SendSalesToBankReconciliation(record, User.Identity.Name);
                    #endregion

                    var formattedData = new
                    {
                        Message = "Successfully transfered.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion

            }
            else
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
        private string GetProductCategory(string id)
        {
            return db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == id)?.Description;
        }
        private string GetInventoryCategoryCode(string crmCategoryName)
        {
            return db.InventoryCateoryMaps.FirstOrDefault(c => c.Category == crmCategoryName)?.InventoryCategoryId;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
