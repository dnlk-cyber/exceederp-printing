using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CashSalesUser)]
    public class CashSalesInvoiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        List<CashSalesInvoice> salesQuoteVoucherViews = new List<CashSalesInvoice>();
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


                        var AditonalCostlog = db.AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList();
                        foreach (var log in AditonalCostlog)
                        {
                            log.LineItemId = lineItem.LineItemID;
                            db.AdditionalCostLogs.Add(log);
                            db.SaveChanges();
                        }

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
        public ActionResult CashSalesInvoices_Read([DataSourceRequest]DataSourceRequest request)
        {

            List<CashSalesInvoice> voulist = new List<CashSalesInvoice>();

            try
            {

                lineItems = dbb.LineItems.ToList();

                List<CashSalesInvoice> quoteVouchers = new List<CashSalesInvoice>();
                quoteVouchers = dbbb.CashSalesInvoices.ToList();
                foreach (var vou in quoteVouchers)
                {

                    var Customer = db.OrganizationCustomers.Where(x => x.OrganizationCustomerID == vou.OrganizationCustomerId).FirstOrDefault();

                    if (Customer != null && vou.PaymentMethod == CRMConstants.CashPaymentMethod)
                    {
                        SalesQuoteVoucherView vsqvView = new SalesQuoteVoucherView();
                        decimal totsub = lineItems.Where(t => t.Voucher == vou.CashSalesInvoiceID.ToString() && t.VoucherDefinitionID == "2").Sum(x => x.CalculatedCost);
                        decimal totgrand = lineItems.Where(t => t.Voucher == vou.CashSalesInvoiceID.ToString() && t.VoucherDefinitionID == "2").Sum(x => x.TotalAmount);


                        string SalesStatus = string.Empty;


                        voulist.Add(new CashSalesInvoice { CashSalesInvoiceID = vou.CashSalesInvoiceID, Period = vou.Period, OrganizationCustomerId = vou.OrganizationCustomerId, Date = vou.Date, GrandTotal = totgrand, IsIssued = vou.IsIssued, AdvancPayment = vou.AdvancPayment, OnHandPayment = vou.OnHandPayment, IssuedDate = vou.IssuedDate, IsVoid = vou.IsVoid, References = vou.References, IsSendForApproval = vou.IsSendForApproval, PaymentMethod = vou.PaymentMethod, Remark = vou.Remark, LastObjectState = vou.LastObjectState, RecieveMethod = vou.RecieveMethod, ModifiedBy = vou.ModifiedBy, LastModified = vou.LastModified, DateCreated = vou.DateCreated, CreatedBy = vou.CreatedBy, IsOnlineApproved = vou.IsOnlineApproved, IsOnlineTransferred = vou.IsOnlineTransferred, IsOnlineTransferredBy = vou.IsOnlineTransferredBy, OnlineApprovedBy = vou.OnlineApprovedBy, OnlineApprovedTime = vou.OnlineApprovedTime, OnlineTransferredTime = vou.OnlineTransferredTime, OrgBranchName = vou.OrgBranchName, SendForApprovalBy = vou.SendForApprovalBy, SendForApprovalTime = vou.SendForApprovalTime, Status = vou.LastObjectState });
                    }
                }

            }
            catch (Exception e)
            {

                string ee = e.InnerException.Message;
            }
            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


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
                catch (Exception e)
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
                    CRMPOSHelper.GenerateCashSalePrintXml(CashSalesInvoice, true,int.Parse( PartialId));
                    var formattedData = new
                    {
                        Message = "Successfully Generated.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
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
            var CashSalesInvoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == Id).FirstOrDefault();

            if (CashSalesInvoice != null)
            {

                return Json(CashSalesInvoice.SalesInvoicePaymentDistibution.Select(s => new
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
            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (cashSalesInvoice.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CreateTempCustomer(cashSalesInvoice.NewCustomer);
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
                    IsVoid = cashSalesInvoice.IsVoid,
                    GrandTotal = cashSalesInvoice.GrandTotal,
                    Period = cashSalesInvoice.Period,
                    LastObjectState = CRMConstants.UnForwarded,
                    PaymentMethod = Payment,
                    RecieveMethod = cashSalesInvoice.RecieveMethod,
                    Remark = cashSalesInvoice.Remark,
                    References = CRMConstants.NoReference,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.CashSalesInvoices.Add(entity);
                db.SaveChanges();
                cashSalesInvoice.CashSalesInvoiceID = entity.CashSalesInvoiceID;

                return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
            }
            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesInvoices_Update([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (cashSalesInvoice.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CreateTempCustomer(cashSalesInvoice.NewCustomer);
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
                    LastObjectState = cashSalesInvoice.LastObjectState,
                    PaymentMethod = Payment,
                    RecieveMethod = cashSalesInvoice.RecieveMethod,
                    References = cashSalesInvoice.References,
                    Remark = cashSalesInvoice.Remark,
                    CreatedBy = cashSalesInvoice.CreatedBy,
                    DateCreated = cashSalesInvoice.DateCreated,

                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.CashSalesInvoices.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Vouchers_Reject(ExceedERP.Web.Areas.CRM.Models.BindingModel model)
        {
            CashSalesInvoice csi = db.CashSalesInvoices.Where(so => so.CashSalesInvoiceID.ToString() == model.Id).FirstOrDefault();
            if (csi.References != CRMConstants.NoReference)
            {
                SalesOrderVoucher sov = db.SalesOrderVouchers.Where(x => x.SalesOrderVoucherID.ToString() == csi.References).FirstOrDefault();

                if (sov != null)
                {
                    List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == model.Id && Item.VoucherDefinitionID == "2").ToList();
                    foreach (var item in lineItemList)
                    {

                        db.LineItems.Attach(item);
                        db.LineItems.Remove(item);
                        db.SaveChanges();


                        var AditonalCostlog = db.AdditionalCostLogs.Where(x => x.LineItemId == item.LineItemID).ToList();
                        foreach (var log in AditonalCostlog)
                        {
                            db.AdditionalCostLogs.Attach(log);
                            db.AdditionalCostLogs.Remove(log);
                            db.SaveChanges();
                        }
                    }

                    db.CashSalesInvoices.Attach(csi);
                    db.CashSalesInvoices.Remove(csi);
                    db.SaveChanges();



                }


                sov.LastObjectState = CRMConstants.Rejected;
                sov.Remark = model.Name;
                sov.LastModified = DateTime.Now;
                sov.ModifiedBy = User.Identity.Name;
                db.SalesOrderVouchers.Attach(sov);
                db.Entry(sov).State = EntityState.Modified;
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

                            var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCashInvoice(CashSalesInvoice, InvoiceItem, AdditionalCosts, AdditionalCostLogs, GLTaxRates, PaymentMethods, AccountMaps);

                            if (SalesInvoicePaymentDistibutions.Any())
                            {
                                db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                                db.SaveChanges();
                            }

                            var ItemCategory = db.ItemCategorys.Where(x => x.ItemCategoryCode == InvoiceItem.Product.ItemCategoryCode).FirstOrDefault();

                            if (ItemCategory != null)
                            {
                                SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeProduct(CashSalesInvoice, ItemCategory, InvoiceItem, AccountMaps);

                                if (SalesInvoicePaymentDistibutions.Any())
                                {
                                    db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                                    db.SaveChanges();
                                }
                            }
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
                        CashSalesInvoice.AdvancPayment =   AdvanceResult.Item2;
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
                    var CashSalesPartialItemsall = db.CashSalesPartialItems.Where(x => x.CashSalesInvoiceId == CashSalesInvoice.CashSalesInvoiceID  ).ToList();

                    var CashSalesPartialItems = db.CashSalesPartialItems.Where(x => x.CashSalesInvoiceId == CashSalesInvoice.CashSalesInvoiceID && x.IsPaymentProcessed == false ).ToList();
                    decimal OnHandPayment = decimal.Zero;
                    if (CashSalesPartialItems.Any())
                    {
                        foreach (var ParialItem in CashSalesPartialItems)
                        {

                            if (ParialItem.CashSalesPartialLineItem.Any())
                            {
                               
                                foreach (var InvoiceItem in ParialItem.CashSalesPartialLineItem.DistinctByField(x=>x.LineItemID))
                                {
                                    
                                        OnHandPayment += (InvoiceItem.LineItem.TotalAmount/(InvoiceItem.LineItem.CalculatedCost/ InvoiceItem.LineItem.UnitAmount) )* ParialItem.CashSalesPartialLineItem.Where(x=>x.LineItemID == InvoiceItem.LineItemID).Sum(x=>x.PartialPayedAmount);

                                 //   InvoiceItem.LineItem.Quantity = 

                                    var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCashInvoice(CashSalesInvoice, InvoiceItem.LineItem, AdditionalCosts, AdditionalCostLogs, GLTaxRates, PaymentMethods, AccountMaps);

                                    if (SalesInvoicePaymentDistibutions.Any())
                                    {
                                        db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                                        db.SaveChanges();
                                    }

                                    var ItemCategory = db.ItemCategorys.Where(x => x.ItemCategoryCode == InvoiceItem.LineItem.Product.ItemCategoryCode).FirstOrDefault();

                                    if (ItemCategory != null)
                                    {
                                        SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeProduct(CashSalesInvoice, ItemCategory, InvoiceItem.LineItem, AccountMaps);

                                        if (SalesInvoicePaymentDistibutions.Any())
                                        {
                                            db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                                            db.SaveChanges();
                                        }
                                    }

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
                        CashSalesInvoice.OnHandPayment = CashSalesInvoice.OnHandPayment+ OnHandPayment;
               
                        CashSalesInvoice.IsOnlineApproved = true;
                        CashSalesInvoice.OnlineApprovedBy = User.Identity.Name;
                        CashSalesInvoice.OnlineApprovedTime =DateTime.Now;

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

                        var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCashInvoice(SalesInvoice, InvoiceItem, AdditionalCosts, AdditionalCostLogs, GLTaxRates, PaymentMethods, AccountMaps);

                        if (SalesInvoicePaymentDistibutions.Any())
                        {
                            db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                            db.SaveChanges();
                        }
                    }




                    SalesInvoice.LastObjectState = CRMConstants.PaymentReceived;

                    dbb.CashSalesInvoices.Attach(SalesInvoice);
                    dbb.Entry(SalesInvoice).State = EntityState.Modified;
                    dbb.SaveChanges();


                }


            }



            catch (Exception e)
            {

            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public OrganizationCustomer CreateTempCustomer(string Name)
        {
            var OrgCustomer = new OrganizationCustomer();

            var OrgCustomerCategory = db.OrganizationCategories.FirstOrDefault();
            if (OrgCustomerCategory != null)
            {
                OrgCustomer.BrandName = Name;
                OrgCustomer.TradeName = Name;
                OrgCustomer.Category = OrgCustomerCategory.OrganizationCategoryID.ToString();
                OrgCustomer.BusinessType = "Business Type";
                OrgCustomer.CustomerType = CustomerType.LOCAL;
                OrgCustomer.IsTemporary = true;
                OrgCustomer.IsActive = false;
                OrgCustomer.IsCredit = false;

                db.OrganizationCustomers.Add(OrgCustomer);
                int result = db.SaveChanges();

                if (result == 1)
                    return OrgCustomer;
            }
            return null;
        }
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

            var invoices = db.CashSalesInvoices.Where(x => x.OrganizationCustomerId.ToString() == Customer && x.PaymentMethod == "CREDIT" && (x.LastObjectState == CRMConstants.PaymentReceived || x.LastObjectState == CRMConstants.ProductIssued)).ToList();
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

           // paymentMethod.Add(new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject { Name="12",Id=12});

            return Json(paymentMethod, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCustomerInvoiceBalanceDDBind(string Voucher)
        {

            decimal RemainingAmount = decimal.Zero;
            var SettlementLine = db.LineCreditSettlements.Where(x => x.InvoiceId.ToString() == Voucher).ToList();
                  
            var lines =
                db.LineItems.Where(
                    x => x.Voucher.ToString() == Voucher && x.VoucherDefinitionID == "2").ToList();
            RemainingAmount = lines.Sum(x => x.TotalAmount) - SettlementLine.Sum(x=>x.SettelmentAmount);
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
                if (LineItems.Any())
                {
                    Invoice.Status = MaintenanceConst.WaitingAdminstrative;
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





        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
