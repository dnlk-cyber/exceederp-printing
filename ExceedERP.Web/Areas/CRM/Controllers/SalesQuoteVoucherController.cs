﻿using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.CRM;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesQuoteUser, CRMRoles.SalesQuoteApprover)]
    public class SalesQuoteVoucherController : Controller
    {
        public ExceedDbContext db = new ExceedDbContext();
        List<SalesQuoteVoucherView> salesQuoteVoucherViews = new List<SalesQuoteVoucherView>();
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
            ViewData["VoucherExtension"] = db.VoucherExtensions.AsQueryable();
            ViewData["Thing"] = db.Products.AsQueryable();
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;



            ViewData["OrganizationCust"] = db.OrganizationCustomers.ToList();

            ViewData["VoucherList"] = db.SalesQuoteVouchers.AsQueryable();
            ViewData["CartList"] = db.Carts.AsQueryable();
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

            ViewData["TermList"] = termList.ToList();
            ViewData["SerialDefinitionList"] = db.SerialDefinitions.AsQueryable();
            ViewData["VoucherReferenceList"] = db.SalesQuoteVouchers.AsQueryable();
            ViewData["StoreList"] = db.Stores.AsQueryable();
            ViewData["VoucherExtensionList"] = db.VoucherExtensions.AsQueryable();


        }
        public void SetVoucherExtension()
        {

        }

        public void SetThing()
        {

        }

        private void GetVoucherMap()
        {
            SalesQuoteVoucherView sqvView;

            foreach (var sqv in db.SalesQuoteVouchers)
            {
                sqvView = new SalesQuoteVoucherView();
                //sqvView.AdditionalCharge = (decimal)0.0;
                //sqvView.Discount = (decimal)0.0;
                //sqvView.SubTotal = (decimal)0.0;
                sqvView.OrganizationCustomerId = sqv.OrganizationCustomerId;
                sqvView.Date = sqv.Date;
                sqvView.GrandTotal = sqv.GrandTotal;
                //     sqvView.PaymentMethod = sqv.PaymentMethod;
                sqvView.Period = sqv.Period;
                sqvView.SalesQuoteVoucherID = sqv.SalesQuoteVoucherID;
                salesQuoteVoucherViews.Add(sqvView);

            }

        }

        public ActionResult Vouchers_Read([DataSourceRequest]DataSourceRequest request)
        {
            //  GetVoucherMap();
            lineItems = dbb.LineItems.ToList();
            List<SalesQuoteVoucherView> voulist = new List<SalesQuoteVoucherView>();
            List<SalesQuoteVoucher> quoteVouchers = new List<SalesQuoteVoucher>();
            quoteVouchers = dbbb.SalesQuoteVouchers.ToList();
            foreach (var vou in quoteVouchers)
            {
                SalesQuoteVoucherView vsqvView = new SalesQuoteVoucherView();
                decimal tottax = lineItems.Where(t => t.Voucher == vou.SalesQuoteVoucherID.ToString() && t.VoucherDefinitionID == "0").Sum(x => x.TaxAmount);
                decimal totgrand = lineItems.Where(t => t.Voucher == vou.SalesQuoteVoucherID.ToString() && t.VoucherDefinitionID == "0").Sum(x => x.TotalAmount);
                voulist.Add(new SalesQuoteVoucherView { SalesQuoteVoucherID = vou.SalesQuoteVoucherID, Period = vou.Period, OrganizationCustomerId = vou.OrganizationCustomerId, Date = vou.Date, GrandTotal = totgrand, IsIssued = vou.IsIssued, IssuedDate = vou.IssuedDate, IsVoid = vou.IsVoid, SubTotal = 0, PaymentMethod = vou.PaymentMethod, LastObjectState = vou.LastObjectState, Remark = vou.Remark, ModifiedBy = vou.ModifiedBy, LastModified = vou.LastModified, DateCreated = vou.DateCreated, CreatedBy = vou.CreatedBy, Status = vou.LastObjectState });
            }
            voulist.OrderByDescending(x => x.SalesQuoteVoucherID);

            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }
        public ActionResult Vouchers_Reada([DataSourceRequest]DataSourceRequest request, int newId)
        {
            IQueryable<SalesQuoteVoucher> vouchers = db.SalesQuoteVouchers.Where(r => r.SalesQuoteVoucherID == newId);
            SalesQuoteVoucher sqvView = null;
            List<SalesQuoteVoucher> salesQuoteVoucherViews = new List<SalesQuoteVoucher>();
            foreach (SalesQuoteVoucher sqv in vouchers)
            {
                sqvView = new SalesQuoteVoucher();

                sqvView.OrganizationCustomerId = sqv.OrganizationCustomerId;
                sqvView.Date = sqv.Date;
                sqvView.GrandTotal = sqv.GrandTotal;
                sqvView.PaymentMethod = sqv.PaymentMethod;
                sqvView.Period = sqv.Period;
                sqvView.SalesQuoteVoucherID = sqv.SalesQuoteVoucherID;
                salesQuoteVoucherViews.Add(sqvView);

            }
            DataSourceResult result = salesQuoteVoucherViews.ToDataSourceResult(request, salesQuoteVoucher => new
            {
                SalesQuoteVoucherID = salesQuoteVoucher.SalesQuoteVoucherID,
                OrganizationCustomerId = salesQuoteVoucher.OrganizationCustomerId,
                IssuedDate = salesQuoteVoucher.IssuedDate,
                IsIssued = salesQuoteVoucher.IsIssued,
                Date = salesQuoteVoucher.Date,
                IsVoid = salesQuoteVoucher.IsVoid,
                GrandTotal = salesQuoteVoucher.GrandTotal,
                Period = salesQuoteVoucher.Period,
                LastObjectState = salesQuoteVoucher.LastObjectState,

                Remark = salesQuoteVoucher.Remark
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Vouchers_Forward([DataSourceRequest]DataSourceRequest request, int newId)
        {


            SalesQuoteVoucher salesQuoteVoucher = db.SalesQuoteVouchers.FirstOrDefault(r => r.SalesQuoteVoucherID == newId);

            db = new ExceedDbContext();
            if (ModelState.IsValid)
            {
                var entity = new SalesQuoteVoucher
                {
                    SalesQuoteVoucherID = salesQuoteVoucher.SalesQuoteVoucherID,
                    OrganizationCustomerId = salesQuoteVoucher.OrganizationCustomerId,
                    IssuedDate = salesQuoteVoucher.IssuedDate,
                    IsIssued = salesQuoteVoucher.IsIssued,
                    Date = salesQuoteVoucher.Date,
                    IsVoid = salesQuoteVoucher.IsVoid,
                    GrandTotal = salesQuoteVoucher.GrandTotal,
                    Period = salesQuoteVoucher.Period,
                    LastObjectState = CRMConstants.Forwarded,
                    Remark = salesQuoteVoucher.Remark
                };

                db.SalesQuoteVouchers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                Vouchers_Read(request);
            }

            return Json(new[] { salesQuoteVoucher }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vouchers_Create([DataSourceRequest]DataSourceRequest request, SalesQuoteVoucher salesQuoteVoucher)
        {
            ModelState.Remove("IssuedDate");
            ModelState.Remove("Date");

            if (ModelState.IsValid)
            {

                OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesQuoteVoucher.OrganizationCustomerId).FirstOrDefault();
                string Payment = "CASH";
                if (!OrgCustormer.IsCredit && salesQuoteVoucher.PaymentMethod == "CREDIT")
                {
                    Payment = "CASH";
                }
                else
                {
                    Payment = salesQuoteVoucher.PaymentMethod;
                }
                var entity = new SalesQuoteVoucher
                {

                    OrganizationCustomerId = salesQuoteVoucher.OrganizationCustomerId,
                    IssuedDate = salesQuoteVoucher.IssuedDate,
                    //Based on the setting of issuing effect
                    IsIssued = salesQuoteVoucher.IsIssued,
                    Date = salesQuoteVoucher.Date,
                    //Based on the current status of the given voucher
                    IsVoid = salesQuoteVoucher.IsVoid,
                    GrandTotal = salesQuoteVoucher.GrandTotal,
                    //Method to retrive date and asssign the selected period.
                    Period = salesQuoteVoucher.Period,
                    PaymentMethod = Payment,
                    LastObjectState = CRMConstants.UnForwarded,
                    Remark = salesQuoteVoucher.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.SalesQuoteVouchers.Add(entity);
                db.SaveChanges();
                salesQuoteVoucher.SalesQuoteVoucherID = entity.SalesQuoteVoucherID;
            }

            return Json(new[] { salesQuoteVoucher }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vouchers_Update([DataSourceRequest]DataSourceRequest request, SalesQuoteVoucher salesQuoteVoucher)
        {
            ModelState.Remove("IssuedDate");
            ModelState.Remove("Date");
            if (ModelState.IsValid)
            {
                OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesQuoteVoucher.OrganizationCustomerId).FirstOrDefault();
                string Payment = "CASH";
                if (!OrgCustormer.IsCredit)
                {
                    Payment = "CASH";
                }
                else
                {
                    Payment = salesQuoteVoucher.PaymentMethod;
                }

                var entity = new SalesQuoteVoucher
                {
                    SalesQuoteVoucherID = salesQuoteVoucher.SalesQuoteVoucherID,
                    OrganizationCustomerId = salesQuoteVoucher.OrganizationCustomerId,
                    IssuedDate = salesQuoteVoucher.IssuedDate,
                    IsIssued = salesQuoteVoucher.IsIssued,
                    Date = salesQuoteVoucher.Date,
                    IsVoid = salesQuoteVoucher.IsVoid,
                    GrandTotal = salesQuoteVoucher.GrandTotal,
                    Period = salesQuoteVoucher.Period,
                    PaymentMethod = Payment,
                    LastObjectState = salesQuoteVoucher.LastObjectState,
                    Remark = salesQuoteVoucher.Remark,
                    LastModified = DateTime.Now,
                    CreatedBy = salesQuoteVoucher.CreatedBy,
                    DateCreated = salesQuoteVoucher.DateCreated,
                    ModifiedBy = User.Identity.Name
                };

                db.SalesQuoteVouchers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new[] { salesQuoteVoucher }.ToDataSourceResult(request, ModelState));
        }



        public ActionResult ForwardSalseQuoteVoucher([DataSourceRequest]DataSourceRequest request, SalesQuoteVoucherView salesQuoteVoucher)
        {
            db = new ExceedDbContext();
            decimal Grandtot = 0;

            SalesQuoteVoucher sqv = db.SalesQuoteVouchers.Where(sq => sq.SalesQuoteVoucherID.ToString() == salesQuoteVoucher.SalesQuoteVoucherID.ToString()).SingleOrDefault();
            List<LineItem> items = db.LineItems.Where(l => l.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString() && l.VoucherDefinitionID == "0").ToList();
            SalesQuoteVoucher saleforwarded = db.SalesQuoteVouchers.Where(l => l.SalesQuoteVoucherID == salesQuoteVoucher.SalesQuoteVoucherID).FirstOrDefault();

            foreach (var item in items)
            {
                Grandtot = Grandtot + item.TotalAmount;
            }
            if (Grandtot < 1)
            {
                ModelState.AddModelError("DuplicateIndexEntry", " Grand Total balance Must not be 0 !!!");

            }
            else if (saleforwarded.LastObjectState == CRMConstants.Forwarded)
            {
                ModelState.AddModelError("DuplicateForwarded", " Quote Voucher is Already Forwarded !!!");

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
                        References = salesQuoteVoucher.SalesQuoteVoucherID.ToString(),
                        IssuedDate = sqv.IssuedDate,
                        IsIssued = sqv.IsIssued,
                        Date = sqv.Date,
                        IsVoid = sqv.IsVoid,
                        GrandTotal = Grandtot,
                        Period = sqv.Period,
                        LastObjectState = CRMConstants.UnForwarded,
                        PaymentMethod = sqv.PaymentMethod,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = sqv.Remark
                    };

                    db.SalesOrderVouchers.Add(entity);
                    db.SaveChanges();
                    referedVoucherID = entity.SalesOrderVoucherID;

                    List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString() && Item.VoucherDefinitionID == "0" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();
                    foreach (LineItem Item in lineItemList)
                    {
                        lineItem = new LineItem();
                        //This may be amended to be the uptodated value
                        lineItem.VoucherDefinitionID = SystemConstatnt.SALES_ORDER_VOUCHER;
                        lineItem.References = Item.LineItemID.ToString();
                        lineItem.CalculatedCost = Item.CalculatedCost;
                        lineItem.PriceTag = Item.PriceTag;
                        lineItem.Quantity = Item.Quantity;
                        lineItem.Remark = Item.Remark;
                        lineItem.Variety = Item.Variety;
                        lineItem.Class = Item.Class;
                        //This may be amended to be the uptodated value
                        lineItem.StockBalance = Item.StockBalance;
                        //This may be attached with StoreTransaction table
                        lineItem.StoreID = Item.StoreID;
                        lineItem.Tax = Item.Tax;
                        lineItem.TaxAmount = Item.TaxAmount;
                        lineItem.ThingID = Item.ThingID;
                        lineItem.TotalAmount = Item.TotalAmount;
                        lineItem.UnitAmount = Item.UnitAmount;
                        lineItem.Vat = Item.Vat;
                        lineItem.WithholdingType = Item.WithholdingType;
                        lineItem.WithholdingAmount = Item.WithholdingAmount;
                        lineItem.WithholdTax = Item.WithholdTax;
                        lineItem.WithholdTaxAmount = Item.WithholdTaxAmount;
                        lineItem.Voucher = referedVoucherID.ToString();
                        lineItem.VoucherDefinitionID = "1";
                        lineItem.Vat = Item.Vat;
                        lineItem.Status = CRMConstants.Pending;
                       lineItem.CreatedBy = User.Identity.Name;
                       lineItem.DateCreated = DateTime.Now;
                     
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

                    List<VoucherTerm> voucherTermList = db.VoucherTerms.Where(vt => vt.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString()).ToList();
                    foreach (VoucherTerm voucherTerm in voucherTermList)
                    {
                        _voucherTerm = new VoucherTerm();
                        _voucherTerm.Remark = voucherTerm.Remark;
                        _voucherTerm.Term = voucherTerm.Term;
                        _voucherTerm.Value = voucherTerm.Value;
                        _voucherTerm.Voucher = referedVoucherID.ToString();
                        db.VoucherTerms.Add(_voucherTerm);
                        db.SaveChanges();
                    }
                    List<VoucherExtensionTransaction> voucherExtensionTransactionList = db.VoucherExtensionTransactions.Where(ve => ve.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString()).ToList();
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
                SalesQuoteVoucher sqvup = dbb.SalesQuoteVouchers.Where(so => so.SalesQuoteVoucherID.ToString() == salesQuoteVoucher.SalesQuoteVoucherID.ToString()).FirstOrDefault();
                sqvup.OnlineApprovedBy = User.Identity.Name;
                sqvup.SendForApprovalBy = User.Identity.Name;
                sqvup.OnlineApprovedTime = DateTime.Now;
                sqvup.IsOnlineApproved = true;
                sqvup.LastObjectState = CRMConstants.Forwarded;
                dbb.SalesQuoteVouchers.Attach(sqvup);
                dbb.Entry(sqvup).State = EntityState.Modified;
                dbb.SaveChanges();
            }



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


            return Json(new[] { salesordervouchersresult }.ToDataSourceResult(request, ModelState));
        }

        public string GetStatus(int Id)
        {

            SalesQuoteVoucher voucher = db.SalesQuoteVouchers.Where(j => j.SalesQuoteVoucherID == Id).FirstOrDefault();
            if (voucher != null)
            {
                return voucher.LastObjectState;
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
