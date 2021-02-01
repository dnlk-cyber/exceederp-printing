using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.CRM;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesOrderUser, CRMRoles.SalesOrderApprover)]
    public class SalesOrderVoucherController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly List<SalesQuoteVoucherView> salesQuoteVoucherViews = new List<SalesQuoteVoucherView>();
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
            ViewData["VoucherExtension"] = db.VoucherExtensions.ToList();
            ViewData["Thing"] = db.Products.ToList();
            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;

            ViewData["CartList"] = db.Carts;
            ViewData["SalesStore"] = db.SalesStores;
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
            ViewData["VoucherReferenceList"] = db.SalesQuoteVouchers.AsQueryable();
            ViewData["StoreList"] = db.Stores;
            ViewData["VoucherExtensionList"] = db.VoucherExtensions;
            ViewData["PeriodList"] = db.Periods;
            UpdateSalesQuoteStatus();
            ViewData["SalesQuoteVoucherList"] = db.SalesQuoteVouchers.Where(sqv => sqv.IsVoid == false && sqv.LastObjectState != CRMConstants.Forwarded);

        }


        private void UpdateSalesQuoteStatus()
        {
            List<VoucherTerm> voucherTerms = db.VoucherTerms.ToList();
            List<SalesQuoteVoucher> salesQuoteVoucherList = db.SalesQuoteVouchers.ToList();
            foreach (SalesQuoteVoucher sqv in salesQuoteVoucherList)
            {
                VoucherTerm vTerm = voucherTerms.Where(vt => vt.Voucher == sqv.SalesQuoteVoucherID.ToString() && vt.Term.LookupId.ToString() == SystemConstatnt.LOOKUP_TERMS_OF_DELIVERY).FirstOrDefault();

                if (vTerm != null)
                {
                    if (sqv.Date.Date.AddDays(int.Parse(vTerm.Value)) < DateTime.Today)
                    {
                        sqv.IsVoid = true;
                        db.SalesQuoteVouchers.Attach(sqv);
                        db.Entry(sqv).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
        }
        public ActionResult SalesOrderVouchers_Read([DataSourceRequest]DataSourceRequest request)
        {

            lineItems = dbb.LineItems.ToList();
            List<SalesOrderVoucher> voulist = new List<SalesOrderVoucher>();
            List<SalesOrderVoucher> quoteVouchers = new List<SalesOrderVoucher>();
            quoteVouchers = dbbb.SalesOrderVouchers.ToList();
            foreach (var vou in quoteVouchers)
            {
                SalesQuoteVoucherView vsqvView = new SalesQuoteVoucherView();
                //  decimal tottax = lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").Sum(x => x.TaxAmount);
                decimal totsub = lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").Sum(x => x.CalculatedCost);
                decimal totgrand = lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").Sum(x => x.TotalAmount);
                voulist.Add(new SalesOrderVoucher { SalesOrderVoucherID = vou.SalesOrderVoucherID, Period = vou.Period, OrganizationCustomerId = vou.OrganizationCustomerId, Date = vou.Date, GrandTotal = totgrand, IsIssued = vou.IsIssued, IssuedDate = vou.IssuedDate, IsVoid = vou.IsVoid, References = vou.References, PaymentMethod = vou.PaymentMethod, LastObjectState = vou.LastObjectState, Remark = vou.Remark, ModifiedBy = vou.ModifiedBy, LastModified = vou.LastModified, DateCreated = vou.DateCreated, CreatedBy = vou.CreatedBy, IsOnlineApproved = vou.IsOnlineApproved, IsOnlineTransferred = vou.IsOnlineTransferred, IsOnlineTransferredBy = vou.IsOnlineTransferredBy, OnlineApprovedBy = vou.OnlineApprovedBy, OnlineApprovedTime = vou.OnlineApprovedTime, OnlineTransferredTime = vou.OnlineTransferredTime, OrgBranchName = vou.OrgBranchName, SendForApprovalBy = vou.SendForApprovalBy, SendForApprovalTime = vou.SendForApprovalTime, Status = vou.LastObjectState });
            }

            voulist.OrderByDescending(x => x.SalesOrderVoucherID);

            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesOrderVouchers_Create([DataSourceRequest]DataSourceRequest request, SalesOrderVoucher salesOrderVoucher)
        {

            if (ModelState.IsValid)
            {
                OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesOrderVoucher.OrganizationCustomerId).FirstOrDefault();
                string Payment = "CASH";
                if (!OrgCustormer.IsCredit && salesOrderVoucher.PaymentMethod == "CREDIT")
                {
                    Payment = "CASH";
                }
                else
                {
                    Payment = salesOrderVoucher.PaymentMethod;
                }
                var entity = new SalesOrderVoucher
                {
                    SalesOrderVoucherID = salesOrderVoucher.SalesOrderVoucherID,
                    OrganizationCustomerId = salesOrderVoucher.OrganizationCustomerId,
                    IssuedDate = salesOrderVoucher.IssuedDate,
                    Date = salesOrderVoucher.Date,
                    IsVoid = salesOrderVoucher.IsVoid,
                    IsIssued = true,
                    GrandTotal = salesOrderVoucher.GrandTotal,
                    Period = salesOrderVoucher.Period,
                    LastObjectState = CRMConstants.UnForwarded,
                    PaymentMethod = Payment,
                    References = CRMConstants.NoReference,
                    Remark = salesOrderVoucher.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.SalesOrderVouchers.Add(entity);
                db.SaveChanges();
                salesOrderVoucher.SalesOrderVoucherID = entity.SalesOrderVoucherID;
            }

            return Json(new[] { salesOrderVoucher }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesOrderVouchers_Update([DataSourceRequest]DataSourceRequest request, SalesOrderVoucher salesOrderVoucher)
        {

            if (ModelState.IsValid)
            {
                //  SalesOrderVoucher sov = db.SalesOrderVouchers.Where(x => x.SalesOrderVoucherID == salesOrderVoucher.SalesOrderVoucherID).FirstOrDefault();
                OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesOrderVoucher.OrganizationCustomerId).FirstOrDefault();
                string Payment = "CASH";
                if (!OrgCustormer.IsCredit)
                {
                    Payment = "CASH";
                }
                else
                {
                    Payment = salesOrderVoucher.PaymentMethod;
                }

                var entity = new SalesOrderVoucher
                {
                    SalesOrderVoucherID = salesOrderVoucher.SalesOrderVoucherID,
                    OrganizationCustomerId = salesOrderVoucher.OrganizationCustomerId,
                    IssuedDate = salesOrderVoucher.IssuedDate,
                    IsIssued = salesOrderVoucher.IsIssued,
                    Date = salesOrderVoucher.Date,
                    IsVoid = salesOrderVoucher.IsVoid,
                    GrandTotal = salesOrderVoucher.GrandTotal,
                    Period = salesOrderVoucher.Period,
                    LastObjectState = salesOrderVoucher.LastObjectState,
                    PaymentMethod = Payment,
                    Remark = salesOrderVoucher.Remark,
                    LastModified = DateTime.Now,
                    References = salesOrderVoucher.References,
                    CreatedBy = salesOrderVoucher.CreatedBy,
                    DateCreated = salesOrderVoucher.DateCreated,
                    ModifiedBy = User.Identity.Name
                };

                db.SalesOrderVouchers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new[] { salesOrderVoucher }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesOrderVouchers_Destroy([DataSourceRequest]DataSourceRequest request, SalesOrderVoucher salesOrderVoucher)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesOrderVoucher
                {
                    SalesOrderVoucherID = salesOrderVoucher.SalesOrderVoucherID,
                    OrganizationCustomerId = salesOrderVoucher.OrganizationCustomerId,
                    IssuedDate = salesOrderVoucher.IssuedDate,
                    IsIssued = salesOrderVoucher.IsIssued,
                    Date = salesOrderVoucher.Date,
                    IsVoid = salesOrderVoucher.IsVoid,
                    GrandTotal = salesOrderVoucher.GrandTotal,
                    Period = salesOrderVoucher.Period,
                    LastObjectState = salesOrderVoucher.LastObjectState,
                    PaymentMethod = salesOrderVoucher.PaymentMethod,
                    Remark = salesOrderVoucher.Remark
                };

                db.SalesOrderVouchers.Attach(entity);
                db.SalesOrderVouchers.Remove(entity);
                db.SaveChanges();


                List<LineItem> lineItems = db.LineItems.Where(l => l.Voucher == salesOrderVoucher.SalesOrderVoucherID.ToString() && l.VoucherDefinitionID == "1").ToList();


                foreach (var lineItem in lineItems)
                {

                    db.LineItems.Attach(lineItem);
                    db.LineItems.Remove(lineItem);
                    db.SaveChanges();

                }
            }

            return Json(new[] { salesOrderVoucher }.ToDataSourceResult(request, ModelState));
        }




        public ActionResult PopulateSalseQuoteVoucherForReference([DataSourceRequest]DataSourceRequest request, string salesQuoteVoucherID)
        {
            db = new ExceedDbContext();
            decimal Grandtot = 0;

            SalesQuoteVoucher sqv = db.SalesQuoteVouchers.Where(sq => sq.SalesQuoteVoucherID.ToString() == salesQuoteVoucherID).SingleOrDefault();
            List<LineItem> items = db.LineItems.Where(l => l.Voucher == salesQuoteVoucherID && l.VoucherDefinitionID == "0" && (l.Status == CRMConstants.Pending || l.Status == CRMConstants.Rejected)).ToList();

            var AlreadyForwarded = db.SalesOrderVouchers.Where(x => x.References == sqv.SalesQuoteVoucherID.ToString()).ToList();

            if (AlreadyForwarded.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", "The Voucher Already Forwarded!!!");
                return Json("The Voucher Already Forwarded!", JsonRequestBehavior.AllowGet);
            }
            else
            {
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
                    int referedVoucherID;
                    LineItem lineItem;
                    VoucherTerm _voucherTerm;
                    VoucherExtensionTransaction _voucherExtensionTransaction;
                    if (sqv != null)
                    {
                        var entity = new SalesOrderVoucher
                        {

                            OrganizationCustomerId = sqv.OrganizationCustomerId,
                            References = salesQuoteVoucherID,
                            IssuedDate = sqv.IssuedDate,
                            IsIssued = sqv.IsIssued,
                            Date = sqv.Date,
                            IsVoid = sqv.IsVoid,
                            GrandTotal = Grandtot,

                            Period = sqv.Period,
                            LastObjectState = sqv.LastObjectState,
                            PaymentMethod = sqv.PaymentMethod,
                            Remark = sqv.Remark

                        };

                        db.SalesOrderVouchers.Add(entity);
                        db.SaveChanges();
                        referedVoucherID = entity.SalesOrderVoucherID;

                        List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == salesQuoteVoucherID.ToString() && Item.VoucherDefinitionID == "0" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();
                        foreach (LineItem Item in lineItemList)
                        {
                            lineItem = new LineItem
                            {
                                VoucherDefinitionID = SystemConstatnt.SALES_ORDER_VOUCHER,
                                References = salesQuoteVoucherID,
                                CalculatedCost = Item.CalculatedCost,
                                PriceTag = Item.PriceTag,
                                Quantity = Item.Quantity,
                                Remark = Item.Remark,
                                //This may be amended to be the uptodated value
                                StockBalance = Item.StockBalance,
                                Variety = Item.Variety,
                                Class = Item.Class,
                                //This may be attached with StoreTransaction table
                                StoreID = Item.StoreID,
                                Tax = Item.Tax,
                                TaxAmount = Item.TaxAmount,
                                ThingID = Item.ThingID,
                                TotalAmount = Item.TotalAmount,
                                UnitAmount = Item.UnitAmount,
                                Voucher = referedVoucherID.ToString()
                            };
                            lineItem.VoucherDefinitionID = "1";
                            lineItem.Vat = Item.Vat;
                            lineItem.WithholdingType = Item.WithholdingType;
                            lineItem.WithholdingAmount = Item.WithholdingAmount;
                            lineItem.WithholdTax = Item.WithholdTax;
                            lineItem.WithholdTaxAmount = Item.WithholdTaxAmount;
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
                        List<VoucherTerm> voucherTermList = db.VoucherTerms.Where(vt => vt.Voucher == salesQuoteVoucherID).ToList();
                        foreach (VoucherTerm voucherTerm in voucherTermList)
                        {
                            _voucherTerm = new VoucherTerm
                            {
                                Remark = voucherTerm.Remark,
                                Term = voucherTerm.Term,
                                Value = voucherTerm.Value,
                                Voucher = referedVoucherID.ToString(),
                                VoucherDefinitionID = 1
                            };
                            db.VoucherTerms.Add(_voucherTerm);
                            db.SaveChanges();
                        }

                        List<VoucherExtensionTransaction> voucherExtensionTransactionList = db.VoucherExtensionTransactions.Where(ve => ve.Voucher == salesQuoteVoucherID).ToList();
                        foreach (VoucherExtensionTransaction voucherExtensionTrxn in voucherExtensionTransactionList)
                        {
                            _voucherExtensionTransaction = new VoucherExtensionTransaction
                            {
                                Number = voucherExtensionTrxn.Number,
                                Remark = voucherExtensionTrxn.Remark,
                                Voucher = referedVoucherID.ToString(),
                                VoucherExtension = voucherExtensionTrxn.VoucherExtension
                            };
                            db.VoucherExtensionTransactions.Add(_voucherExtensionTransaction);
                            db.SaveChanges();
                        }
                    }
                }
            }
            IQueryable<SalesOrderVoucher> salesordervouchers = db.SalesOrderVouchers;
            DataSourceResult result = salesordervouchers.ToDataSourceResult(request, salesOrderVoucher => new
            {
                SalesOrderVoucherID = salesOrderVoucher.SalesOrderVoucherID,
                OrganizationCustomerId = salesOrderVoucher.OrganizationCustomerId,
                IssuedDate = salesOrderVoucher.IssuedDate,
                IsIssued = salesOrderVoucher.IsIssued,
                Date = salesOrderVoucher.Date,
                IsVoid = salesOrderVoucher.IsVoid,
                GrandTotal = salesOrderVoucher.GrandTotal,
                Period = salesOrderVoucher.Period,
                LastObjectState = salesOrderVoucher.LastObjectState,
                PaymentMethod = salesOrderVoucher.PaymentMethod,
                Remark = salesOrderVoucher.Remark
            });
            ExceedDbContext dbb = new ExceedDbContext();
            SalesQuoteVoucher sqvup = dbb.SalesQuoteVouchers.Where(so => so.SalesQuoteVoucherID.ToString() == salesQuoteVoucherID).FirstOrDefault();
            sqvup.LastObjectState = CRMConstants.Forwarded;
            dbb.SalesQuoteVouchers.Attach(sqvup);
            dbb.Entry(sqvup).State = EntityState.Modified;
            dbb.SaveChanges();

            IQueryable<SalesOrderVoucher> salesordervoucherresult = db.SalesOrderVouchers;
            DataSourceResult salesordervouchersresult = salesordervoucherresult.ToDataSourceResult(request, salesOrderVoucher => new
            {
                SalesOrderVoucherID = salesOrderVoucher.SalesOrderVoucherID,
                OrganizationCustomerId = salesOrderVoucher.OrganizationCustomerId,
                IssuedDate = salesOrderVoucher.IssuedDate,
                IsIssued = salesOrderVoucher.IsIssued,
                Date = salesOrderVoucher.Date,
                IsVoid = salesOrderVoucher.IsVoid,
                GrandTotal = salesOrderVoucher.GrandTotal,
                Period = salesOrderVoucher.Period,
                LastObjectState = salesOrderVoucher.LastObjectState,
                PaymentMethod = salesOrderVoucher.PaymentMethod,
                Remark = salesOrderVoucher.Remark
            });


            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Vouchers_Reject(BindingModel model)
        {
            try
            {
                SalesOrderVoucher sov = db.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == model.Id).FirstOrDefault();
                if (sov.References != CRMConstants.NoReference)
                {
                    SalesQuoteVoucher sqv = db.SalesQuoteVouchers.Where(x => x.SalesQuoteVoucherID.ToString() == sov.References).FirstOrDefault();

                    if (sqv != null)
                    {
                        List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == sov.SalesOrderVoucherID.ToString() && Item.VoucherDefinitionID == "1").ToList();
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
                        db.SalesOrderVouchers.Attach(sov);
                        db.SalesOrderVouchers.Remove(sov);
                        db.SaveChanges();

                    }


                    sqv.LastObjectState = CRMConstants.Rejected;
                    sqv.Remark = model.Name;
                    sqv.LastModified = DateTime.Now;
                    sqv.ModifiedBy = User.Identity.Name;
                    db.SalesQuoteVouchers.Attach(sqv);
                    db.Entry(sqv).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Index");
        }
        public ActionResult Vouchers_Forward([DataSourceRequest]DataSourceRequest request, string salesOrderVoucherID)
        {
            db = new ExceedDbContext();

            SalesOrderVoucher sov = db.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == salesOrderVoucherID).FirstOrDefault();
            SalesOrderVoucher saleforwarded = db.SalesOrderVouchers.Where(l => l.SalesOrderVoucherID.ToString() == salesOrderVoucherID).FirstOrDefault();

            lineItems = db.LineItems.ToList();
            decimal GrandTotal = lineItems.Where(t => t.Voucher == salesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1" && (t.Status == CRMConstants.Pending || t.Status == CRMConstants.Rejected)).Sum(x => x.TotalAmount);
            if (GrandTotal < 1)
            {
                ModelState.AddModelError("DuplicateIndexEntry", " Grand Total balance Must not be 0 !!!");
            }
            else if (saleforwarded.LastObjectState == CRMConstants.Forwarded)
            {
                ModelState.AddModelError("DuplicateForwarded", " Order Voucher is Already Forwarded !!!");

            }
            else
            {
                db = new ExceedDbContext();
                int referedVoucherID;
                string Receive = "CASH";
                LineItem lineItem;
                VoucherTerm _voucherTerm;
                VoucherExtensionTransaction _voucherExtensionTransaction;
                var PaymentMethod = db.PaymentMethods.FirstOrDefault();
                if (PaymentMethod != null)
                {

                    Receive = PaymentMethod.PaymentMethodID.ToString();
                }
                if (sov != null)
                {
                    var entity = new CashSalesInvoice
                    {
                        OrganizationCustomerId = sov.OrganizationCustomerId,
                        References = salesOrderVoucherID,
                        IssuedDate = sov.IssuedDate,
                        IsIssued = sov.IsIssued,
                        Date = sov.Date,
                        IsVoid = sov.IsVoid,
                        GrandTotal = GrandTotal,
                        Period = sov.Period,
                        LastObjectState = CRMConstants.UnForwarded,
                        PaymentMethod = sov.PaymentMethod,
                        RecieveMethod = Receive,
                        Remark = sov.Remark,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,


                    };

                    db.CashSalesInvoices.Add(entity);
                    db.SaveChanges();
                    referedVoucherID = entity.CashSalesInvoiceID;

                    List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == sov.SalesOrderVoucherID.ToString() && Item.VoucherDefinitionID == "1" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();
                    foreach (LineItem Item in lineItemList)
                    {
                        lineItem = new LineItem
                        {
                            //This may be amended to be the uptodated value
                            CalculatedCost = Item.CalculatedCost,
                            PriceTag = Item.PriceTag,
                            Quantity = Item.Quantity,
                            Remark = Item.Remark,
                            //This may be amended to be the uptodated value
                            StockBalance = Item.StockBalance,
                            Variety = Item.Variety,
                            Class = Item.Class,
                            //This may be attached with StoreTransaction table
                            StoreID = Item.StoreID,
                            Tax = Item.Tax,
                            TaxAmount = Item.TaxAmount,
                            ThingID = Item.ThingID,
                            TotalAmount = Item.TotalAmount,
                            UnitAmount = Item.UnitAmount,
                            Voucher = referedVoucherID.ToString(),
                            VoucherDefinitionID = "2",
                            Vat = Item.Vat,
                            WithholdingType = Item.WithholdingType,
                            WithholdingAmount = Item.WithholdingAmount,
                            WithholdTax = Item.WithholdTax,
                            WithholdTaxAmount = Item.WithholdTaxAmount,
                            Status = CRMConstants.Pending,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now
                        };

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
                        _voucherTerm = new VoucherTerm
                        {
                            Remark = voucherTerm.Remark,
                            Term = voucherTerm.Term,
                            Value = voucherTerm.Value,
                            Voucher = referedVoucherID.ToString()
                        };
                        db.VoucherTerms.Add(_voucherTerm);
                        db.SaveChanges();
                    }
                    List<VoucherExtensionTransaction> voucherExtensionTransactionList = db.VoucherExtensionTransactions.Where(ve => ve.Voucher == sov.References).ToList();
                    foreach (VoucherExtensionTransaction voucherExtensionTrxn in voucherExtensionTransactionList)
                    {
                        _voucherExtensionTransaction = new VoucherExtensionTransaction
                        {
                            Number = voucherExtensionTrxn.Number,
                            Remark = voucherExtensionTrxn.Remark,
                            Voucher = referedVoucherID.ToString(),
                            VoucherExtension = voucherExtensionTrxn.VoucherExtension
                        };
                        db.VoucherExtensionTransactions.Add(_voucherExtensionTransaction);
                        db.SaveChanges();
                    }

                }
                ExceedDbContext dbb = new ExceedDbContext();
                SalesOrderVoucher sovup = dbb.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == salesOrderVoucherID).FirstOrDefault();
                sovup.LastObjectState = CRMConstants.Forwarded;
                sovup.SendForApprovalBy = User.Identity.Name;
                sovup.OnlineApprovedBy = User.Identity.Name;
                sovup.OnlineApprovedTime = DateTime.Now;
                sovup.IsOnlineApproved = true;
                dbb.SalesOrderVouchers.Attach(sovup);
                dbb.Entry(sovup).State = EntityState.Modified;
                dbb.SaveChanges();
            }
            lineItems = db.LineItems.ToList();
            List<SalesOrderVoucher> voulist = new List<SalesOrderVoucher>();
            List<SalesOrderVoucher> quoteVouchers = new List<SalesOrderVoucher>();
            quoteVouchers = dbbb.SalesOrderVouchers.ToList();
            foreach (var vou in quoteVouchers)
            {
                SalesQuoteVoucherView vsqvView = new SalesQuoteVoucherView();
                decimal tottax = lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").Sum(x => x.TaxAmount);
                decimal totsub = lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").Sum(x => x.CalculatedCost) - lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString()).Sum(x => x.TaxAmount);
                decimal totgrand = lineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").Sum(x => x.TotalAmount);
                voulist.Add(new SalesOrderVoucher { SalesOrderVoucherID = vou.SalesOrderVoucherID, Period = vou.Period, OrganizationCustomerId = vou.OrganizationCustomerId, Date = vou.Date, GrandTotal = totgrand, IsIssued = vou.IsIssued, IssuedDate = vou.IssuedDate, IsVoid = vou.IsVoid });
            }

            return Json(voulist.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }




        public string GetStatus(int Id)
        {

            SalesOrderVoucher Voucher = db.SalesOrderVouchers.Where(j => j.SalesOrderVoucherID == Id).FirstOrDefault();
            if (Voucher != null)
            {
                return Voucher.LastObjectState;
            }
            else
            {
                return CRMConstants.Pending;
            }

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
