using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.InventoryIssueUser , CRMRoles.InventoryIssueApprover )] 
    public class SalesInventoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        List<CashSalesInvoice> salesQuoteVoucherViews = new List<CashSalesInvoice>();
        public ExceedDbContext dbb = new ExceedDbContext();
        public ExceedDbContext dbbb = new ExceedDbContext();
        public List<LineItem> lineItems = new List<LineItem>();
        //
        // GET: /CRM/Inventory/
        public ActionResult Index()
        {
            SetInitials();
            ViewBag.Data = db.LineItems.ToList();
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
            ViewData["SalesOrderVoucherList"] = db.SalesOrderVouchers.Where(s => s.LastObjectState != CRMConstants.Forwarded);
        }



        public ActionResult Inventorys_Read([DataSourceRequest]DataSourceRequest request)
        {
            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();
            lineItems = dbb.LineItems.ToList();
            List<CashSalesInvoice> voulist = new List<CashSalesInvoice>();
            List<CashSalesInvoice> quoteVouchers = new List<CashSalesInvoice>();
            var CashPartialLineItems = dbb.CashSalesPartialLineItems.ToList();
            //|| == CRMConstants.PaymentReceived || x.LastObjectState == CRMConstants.ProductIssued || x.LastObjectState == CRMConstants.PartialPaymentReceived
            quoteVouchers = dbbb.CashSalesInvoices.Where(x => x.IsOnlineApproved == true ).ToList();
            foreach (var vou in quoteVouchers)
            {
              
                decimal Remaining = decimal.Zero;
                SalesQuoteVoucherView vsqvView = new SalesQuoteVoucherView();

                decimal totgrand = lineItems.Where(t => t.Voucher == vou.CashSalesInvoiceID.ToString() && t.VoucherDefinitionID == "2").Sum(x => x.Quantity);

                if (vou.Status == CRMConstants.PartialPaymentReceived)
                {
                    totgrand = decimal.Zero;
                 
                    var CashSalesPartialItems = db.CashSalesPartialItems.Where(x => x.CashSalesInvoiceId == vou.CashSalesInvoiceID).ToList();
                   var LineItemOrginals = lineItems.Where(t => t.Voucher == vou.CashSalesInvoiceID.ToString() && t.VoucherDefinitionID == "2").ToList();
                   totgrand = CRMSalesHelper.GetPartialLinesAmount(LineItemOrginals, CashPartialLineItems);
                    
                //    foreach (var LineItemOrginal in LineItemOrginals)
                //    {

                //        foreach (var CashSalesPartialItem in CashSalesPartialItems)
                //        {
                //            var CashSalesPartialILinetems = db.CashSalesPartialLineItems.Where(x => x.LineItemID == LineItemOrginal.LineItemID && x.CashSalesPartialItemId == CashSalesPartialItem.CashSalesPartialItemId).ToList();

                //            var PartialQuantity = CashSalesPartialILinetems.Where(x => x.LineItemID == LineItemOrginal.LineItemID).Sum(x => x.PartialPayedAmount);
                //            totgrand = totgrand + PartialQuantity;
                //        }
                //    }
                 }
                var history = db.IssueHistorys.Where(x => x.InvoiceId == vou.CashSalesInvoiceID).ToList();
                Remaining = totgrand;
                if (history.Any())
                {
                    Remaining = totgrand - history.Sum(x => x.Amount);
                }

                voulist.Add(new CashSalesInvoice { CashSalesInvoiceID = vou.CashSalesInvoiceID, Period = vou.Period, OrganizationCustomerId = vou.OrganizationCustomer.OrganizationCustomerID, Date = vou.Date, GrandTotal = totgrand, IsIssued = vou.IsIssued, IssuedDate = vou.IssuedDate, IsVoid = vou.IsVoid, References = vou.References, PaymentMethod = vou.PaymentMethod, Remark = vou.Remark, Status = Remaining.ToString(), LastObjectState = vou.LastObjectState, RecieveMethod = vou.RecieveMethod, ModifiedBy = vou.ModifiedBy, LastModified = vou.LastModified, DateCreated = vou.DateCreated, CreatedBy = vou.CreatedBy, IsOnlineApproved = vou.IsOnlineApproved, IsOnlineTransferred = vou.IsOnlineTransferred, IsOnlineTransferredBy = vou.IsOnlineTransferredBy, OnlineApprovedBy = vou.OnlineApprovedBy, OnlineApprovedTime = vou.OnlineApprovedTime, OnlineTransferredTime = vou.OnlineTransferredTime, OrgBranchName = vou.OrgBranchName, SendForApprovalBy = vou.SendForApprovalBy, SendForApprovalTime = vou.SendForApprovalTime, IsClosed = vou.IsClosed });
            }


            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }

            [AuthorizeRoles( CRMRoles.InventoryIssueApprover)] 
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Inventorys_Destroy([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {

            try
            {
                CashSalesInvoice cashSalesInvoiceCREDIT = db.CashSalesInvoices.Where(c => c.CashSalesInvoiceID == cashSalesInvoice.CashSalesInvoiceID).FirstOrDefault();
                string InvoiceID = cashSalesInvoiceCREDIT.CashSalesInvoiceID.ToString();
                CashSalesInvoice sqvup =
                                    dbb.CashSalesInvoices.Where(
                                        so => so.CashSalesInvoiceID == cashSalesInvoice.CashSalesInvoiceID)
                                        .FirstOrDefault();

                sqvup.IsOnlineApproved = true;
                sqvup.IsClosed = true;
                sqvup.LastObjectState = CRMConstants.ProductIssued;
                dbb.CashSalesInvoices.Attach(sqvup);
                dbb.Entry(sqvup).State = EntityState.Modified;
                dbb.SaveChanges();

            }
            catch (Exception e)
            {
            
            }




            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult IssueHistory_Read([DataSourceRequest]DataSourceRequest request, int InvoiceId)
        {
            var voulist = db.IssueHistorys.Where(x => x.InvoiceId == InvoiceId).ToList();

            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }
        public ActionResult Issue(GenralIssueBindingModel model)
        {
            try
            {

                decimal Remaining = decimal.Zero;
                decimal beforeTransaction = 0;
                decimal afterTransaction = 0;
                decimal TransactionQuantity = 0;

                if (model.Property.Any())
                {
                    foreach (var item in model.Property)
                    {
                        if (item.Checked == "on")
                        {
                            LineItem items = db.LineItems.Where(x => x.LineItemID.ToString() == item.Id && x.VoucherDefinitionID == "2").FirstOrDefault();
                            if (items.Quantity > 0)
                            {
                                

                                CashSalesInvoice sales = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID.ToString() == items.Voucher).FirstOrDefault();
                                InventoryStore Store =
                                  db.InventoryStores.Where(
                                      x => x.StoreCode == items.SalesStore.StoreCode)
                                      .FirstOrDefault();
                                Product IssueProduct =
                                    db.Products.Where(x => x.ProductID == items.ThingID)
                                        .FirstOrDefault();
                              
                                StoreItemAssignment sb =
            db.StoreItemAssignments.Where(
                x =>
                    x.InventoryStore.StoreCode == Store.StoreCode && x.InventoryItem.ItemCode == IssueProduct.ItemCode).OrderByDescending(x => x.StoreItemAssignmentID)
                .FirstOrDefault();
                                if (items.Quantity > sb.Balance)
                                {



                                }
                                else
                                {

                                    decimal FinalQuantity = sb.Balance - decimal.Parse(item.Name);
                                    beforeTransaction = sb.Balance;
                                    sb.Balance = FinalQuantity;

                                    afterTransaction = FinalQuantity;
                                    TransactionQuantity = decimal.Parse(item.Name);
                                    db.StoreItemAssignments.Attach(sb);
                                    db.Entry(sb).State = EntityState.Modified;
                                    db.SaveChanges();

                                    items.Status = CRMConstants.ProductIssued;
                                    items.Quantity = items.Quantity - decimal.Parse(item.Name);
                                    db.LineItems.Attach(items);
                                    db.Entry(items).State = EntityState.Modified;
                                    db.SaveChanges();


                                    var entity = new IssueHistory()
                                    {

                                        Amount = (int) decimal.Parse(item.Name.ToString()),
                                        InvoiceId = int.Parse(items.Voucher.ToString()),
                                        DateCreated = DateTime.Now,
                                        Remark = items.ThingID.ToString()
                                    };

                                    db.IssueHistorys.Add(entity);
                                    db.SaveChanges();

                                    var Transaction = new StoreTransaction
                                    {
                                        Voucher = items.Voucher.ToString(),
                                        Source = items.SalesStore.Description + " -- " + items.Product.Name,
                                        Destination = sales.OrganizationCustomer.TradeName,
                                        Amount = (items.UnitAmount * decimal.Parse(item.Name)).ToString(),
                                        Quantity = TransactionQuantity,
                                        beforeStock = beforeTransaction,
                                        afterStock = afterTransaction,
                                        TransactionType = sales.PaymentMethod
                                    };

                                    db.StoreTransactions.Add(Transaction);
                                    db.SaveChanges();
                                }
                            }
                        }



                    }
                }
            }

            catch (Exception e)
            {
        
            }
            return RedirectToAction("Index", "SalesInventory", new { area ="CRM"});
        }
    }
}