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
using ExceedERP.Helpers.CRM;


namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CashSalesUser)]
    public class CreditsSalesInvoiceController : Controller
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
            ViewData["SalesOrderVoucherList"] = db.SalesOrderVouchers.Where(s => s.LastObjectState != CRMConstants.Forwarded && s.PaymentMethod == CRMConstants.CreditPayment);
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
        public ActionResult CreditsSalesInvoices_Read([DataSourceRequest]DataSourceRequest request)
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

                    if (Customer != null && vou.PaymentMethod == CRMConstants.CreditPaymentMethod)
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditsSalesInvoices_Create([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            if (ModelState.IsValid)
            {
                OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == cashSalesInvoice.OrganizationCustomerId).FirstOrDefault();
                //if (!OrgCustormer.IsCredit && cashSalesInvoice.PaymentMethod == "CREDIT")
                //{
                //    Payment = "CASH";
                //}
                //else
                //{
                //    Payment = cashSalesInvoice.PaymentMethod;
                //}

                if (OrgCustormer.IsCredit)
                {
                    var entity = new CashSalesInvoice
                    {

                        OrganizationCustomerId = cashSalesInvoice.OrganizationCustomerId,
                        IssuedDate = cashSalesInvoice.IssuedDate,
                        IsIssued = cashSalesInvoice.IsIssued,
                        Date = cashSalesInvoice.Date,
                        IsVoid = cashSalesInvoice.IsVoid,
                        GrandTotal = cashSalesInvoice.GrandTotal,
                        Period = cashSalesInvoice.Period,
                        LastObjectState = CRMConstants.UnForwarded,
                        PaymentMethod = CRMConstants.CreditPaymentMethod,
                        RecieveMethod = cashSalesInvoice.RecieveMethod,
                        Remark = cashSalesInvoice.Remark,
                        References = CRMConstants.NoReference,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.CashSalesInvoices.Add(entity);
                    db.SaveChanges();
                    cashSalesInvoice.CashSalesInvoiceID = entity.CashSalesInvoiceID;
                }
            }

            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditsSalesInvoices_Update([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            if (ModelState.IsValid)
            {
                OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == cashSalesInvoice.OrganizationCustomerId).FirstOrDefault();
               
                if (OrgCustormer.IsCredit)
                {
                  
                    var entity = new CashSalesInvoice
                    {
                        CashSalesInvoiceID = cashSalesInvoice.CashSalesInvoiceID,
                        OrganizationCustomerId = cashSalesInvoice.OrganizationCustomerId,
                        IssuedDate = cashSalesInvoice.IssuedDate,
                        IsIssued = cashSalesInvoice.IsIssued,
                        Date = cashSalesInvoice.Date,
                        IsVoid = cashSalesInvoice.IsVoid,
                        GrandTotal = cashSalesInvoice.GrandTotal,
                        Period = cashSalesInvoice.Period,
                        LastObjectState = cashSalesInvoice.LastObjectState,
                        PaymentMethod = CRMConstants.CreditPaymentMethod,
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
        public ActionResult CreditsSalesInvoices_Destroy([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();

            try
            {

                var GLTaxRates = db.GLTaxRates.ToList();
                var PaymentMethods = db.PaymentMethods.ToList();
                var AdditionalCostLogs = db.AdditionalCostLogs.ToList();
                var AdditionalCosts = db.AdditionalCosts.ToList();
                var AccountMaps = db.AccountMaps.ToList();

                OrganizationCustomer OrgCustomer = db.OrganizationCustomers.Where(c => c.OrganizationCustomerID == cashSalesInvoice.OrganizationCustomerId).FirstOrDefault();

                if (OrgCustomer != null && OrgCustomer.IsCredit)
                {

                    CreditSetting creditSetting =
                        db.CreditSettings.Where(c => c.OrganizationID == OrgCustomer.OrganizationCustomerID)
                            .OrderByDescending(c => c.CreditSettingID)
                            .FirstOrDefault();
                    if (creditSetting != null)
                    {
                        if (creditSetting.Quantity < cashSalesInvoice.GrandTotal)
                        {
                            ModelState.AddModelError("InsufficentCredit",
                                " Customer Have Insufficient Credit");


                            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
                        }
                        else
                        {

                            List<LineItem> InvoiceItems =
                                db.LineItems.Where(
                                    l => l.Voucher == cashSalesInvoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2" && l.Status == CRMConstants.Pending)
                                    .ToList();

                            if (InvoiceItems != null)
                            {
                                foreach (var InvoiceItem in InvoiceItems)
                                {

                                    InvoiceItem.Status = CRMConstants.CreditPayment;
                                    db.LineItems.Attach(InvoiceItem);
                                    db.Entry(InvoiceItem).State = EntityState.Modified;
                                    db.SaveChanges();

                                    var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCreditInvoice(cashSalesInvoice, InvoiceItem, AdditionalCosts, AdditionalCostLogs, GLTaxRates, PaymentMethods, AccountMaps);

                                    if (SalesInvoicePaymentDistibutions.Any())
                                    {
                                        db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                                        db.SaveChanges();
                                    }
                                }


                                cashSalesInvoice.LastObjectState = CRMConstants.PaymentReceived;
                                dbb.CashSalesInvoices.Attach(cashSalesInvoice);
                                dbb.Entry(cashSalesInvoice).State = EntityState.Modified;
                                dbb.SaveChanges();

                                decimal FinalCredit = decimal.Parse(creditSetting.Quantity.ToString()) -
                                                cashSalesInvoice.GrandTotal;

                                creditSetting.Quantity = FinalCredit;
                                dbb.CreditSettings.Attach(creditSetting);
                                dbb.Entry(creditSetting).State = EntityState.Modified;
                                dbb.SaveChanges();



                                var CreditInvoice = dbb.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == cashSalesInvoice.CashSalesInvoiceID).FirstOrDefault();
                                if(CreditInvoice != null)
                                {
                                    CreditInvoice.IsOnlineApproved = true;
                                    CreditInvoice.OnlineApprovedBy = User.Identity.Name;
                                    CreditInvoice.OnlineApprovedTime = DateTime.Now;


                                    dbb.CashSalesInvoices.Attach(CreditInvoice);
                                    dbb.Entry(CreditInvoice).State = EntityState.Modified;
                                    dbb.SaveChanges();
                                }

                            }
                        }
                    }

                }

            }

            catch (Exception e)
            {

            }

            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
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

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
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


            return Json(paymentMethod, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCustomerInvoiceBalanceDDBind(string Voucher)
        {



            var lines =
                db.LineItems.Where(
                    x => x.Voucher.ToString() == Voucher && x.VoucherDefinitionID == "2").ToList();

            if (lines.Any())
            {
                return Json(new ExceedERP.Web.Areas.CRM.Models.BindingModel { Name = lines.Sum(x => x.TotalAmount).ToString(), Id = lines.Sum(x => x.TotalAmount).ToString() }, JsonRequestBehavior.AllowGet);
            }

            return Json(0, JsonRequestBehavior.AllowGet);
        }




        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
