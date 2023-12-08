using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
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
    [AuthorizeRoles(CRMRoles.SalesOrderUser, CRMRoles.SalesOrderApprover, CRMRoles.CRMVoid)]
    public class SalesOrderVoucherController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly List<SalesQuoteVoucherView> salesQuoteVoucherViews = new List<SalesQuoteVoucherView>();
        public ExceedDbContext dbb = new ExceedDbContext();
        public ExceedDbContext dbbb = new ExceedDbContext();
        //public List<LineItem> lineItems = new List<LineItem>();

        public ActionResult Index()
        {
            SetInitials();
            return View();
        }
        public void SetInitials()
        {

            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["VoucherExtension"] = db.VoucherExtensions.ToList();
            ViewData["Thing"] = db.Products.Select(s => new { s.ProductID, Name = s.ItemCode + " " + s.Name }).ToList();
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
            //UpdateSalesQuoteStatus();
            ViewData["SalesQuoteVoucherList"] = db.SalesQuoteVouchers.Where(sqv => sqv.IsVoid == false && sqv.LastObjectState != CRMConstants.Forwarded);

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();
            ViewData["RepairTypes"] = db.RepairType.Select(r => new { RepairTypeId = r.RepairTypeID.ToString(), r.Name });

        }

        public ActionResult CleanAndUpdate()
        {
            //
            // assign store id from its line item
            var emptyStores = db.SalesOrderVouchers.Where(s => s.StoreID == 0).ToList();
            foreach (var so in emptyStores)
            {
                var lineItem = db.LineItems.FirstOrDefault(l => l.Voucher == so.SalesOrderVoucherID.ToString() && l.VoucherDefinitionID == "1");
                if (lineItem != null)
                {
                    so.StoreID = lineItem.StoreID;

                    db.SalesOrderVouchers.Attach(so);
                    db.Entry(so).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            //

            return Redirect(nameof(Index));
        }
        //private void UpdateSalesQuoteStatus()
        //{
        //    List<VoucherTerm> voucherTerms = db.VoucherTerms.ToList();
        //    List<SalesQuoteVoucher> salesQuoteVoucherList = db.SalesQuoteVouchers.ToList();
        //    foreach (SalesQuoteVoucher sqv in salesQuoteVoucherList)
        //    {
        //        VoucherTerm vTerm = voucherTerms.Where(vt => vt.Voucher == sqv.SalesQuoteVoucherID.ToString() && vt.Term.LookupId.ToString() == SystemConstatnt.LOOKUP_TERMS_OF_DELIVERY).FirstOrDefault();

        //        if (vTerm != null)
        //        {
        //            if (sqv.Date.Date.AddDays(int.Parse(vTerm.Value)) < DateTime.Today)
        //            {
        //                sqv.IsVoid = true;
        //                db.SalesQuoteVouchers.Attach(sqv);
        //                db.Entry(sqv).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }

        //        }
        //    }
        //}
        public ActionResult SalesOrderVouchers_Read([DataSourceRequest]DataSourceRequest request)
        {

            //var lineItems = dbb.LineItems.ToList();
            List<SalesOrderVoucher> voulist = new List<SalesOrderVoucher>();
            List<SalesOrderVoucher> quoteVouchers = new List<SalesOrderVoucher>();

            //var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            //var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));
            var assignedStores = db.SalesPersonAssignments.Where(a => a.UserName == User.Identity.Name).Select(a => a.SalesStoreId).ToList();

            quoteVouchers = dbbb.SalesOrderVouchers.Where(o => assignedStores.Contains(o.StoreID)).ToList();
            foreach (var vou in quoteVouchers)
            {
                //SalesQuoteVoucherView vsqvView = new SalesQuoteVoucherView();

                decimal tottax = 0;
                decimal totgrand = 0;

                var lines = db.LineItems.Where(t => t.Voucher == vou.SalesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1").ToList();
                var serviceLineItems = db.ServiceLineItems.Where(t => t.VoucherId == vou.SalesOrderVoucherID && t.VoucherType == CRMVoucherType.SalesOrder).ToList();

                if (lines.Any())
                {
                    tottax += lines.Sum(x => x.TaxAmount);
                    totgrand += lines.Sum(x => x.TotalAmount);
                }
                if (serviceLineItems.Any())
                {
                    tottax += serviceLineItems.Sum(x => x.TaxAmount);
                    totgrand += serviceLineItems.Sum(x => x.TotalAmount);
                }

                voulist.Add(new SalesOrderVoucher
                {
                    SalesOrderVoucherID = vou.SalesOrderVoucherID,
                    Period = vou.Period,
                    Source = vou.Source,
                    OrganizationCustomerId = vou.OrganizationCustomerId,
                    Date = vou.Date,
                    GrandTotal = totgrand,
                    IsIssued = vou.IsIssued,
                    IssuedDate = vou.IssuedDate,
                    IsVoid = vou.IsVoid,
                    References = vou.References,
                    PaymentMethod = vou.PaymentMethod,
                    LastObjectState = vou.LastObjectState,
                    Remark = vou.Remark,
                    BranchId = vou.BranchId,
                    StoreID = vou.StoreID,
                    CRMPeriodId = vou.CRMPeriodId,
                    JobOrderNumber = vou.JobOrderNumber,
                    PlateNo=vou.PlateNo,
                    ReceiptReference = vou.ReceiptReference,
                    IsShipped = vou.IsShipped,
                    
                    //IsFromMaintenance = vou.IsFromMaintenance,
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
                    IsSendForApproval = vou.IsSendForApproval,
                    SendForApprovalTime = vou.SendForApprovalTime,
                    Status = vou.LastObjectState
                });
            }


            voulist.OrderByDescending(x => x.SalesOrderVoucherID);

            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesOrderVouchers_Create([DataSourceRequest]DataSourceRequest request, SalesOrderVoucher salesOrderVoucher)
        {
            if (salesOrderVoucher.OrganizationCustomerId == 0 && string.IsNullOrEmpty(salesOrderVoucher.NewCustomer))
            {
                ModelState.AddModelError("", "Select Existing customer or Insert Name of new one");
            }
            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (salesOrderVoucher.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CRMSalesHelper.CreateTempCustomer(salesOrderVoucher.NewCustomer);
                }
                else
                {
                    OrgCustomer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesOrderVoucher.OrganizationCustomerId).FirstOrDefault();
                }

                //OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesOrderVoucher.OrganizationCustomerId).FirstOrDefault();
                string Payment = "CASH";
                if (!OrgCustomer.IsCredit && salesOrderVoucher.PaymentMethod == "CREDIT")
                {
                    Payment = "CASH";
                }
                else
                {
                    Payment = salesOrderVoucher.PaymentMethod;
                }
                if (salesOrderVoucher.Source == SalesOrderSource.SalesQuote && string.IsNullOrEmpty(salesOrderVoucher.References))
                {
                    ModelState.AddModelError("", "Referenece is required for Sales Order from Sales Quote");
                }
                else
                {
                    var entity = new SalesOrderVoucher
                    {
                        SalesOrderVoucherID = salesOrderVoucher.SalesOrderVoucherID,
                        OrganizationCustomerId = OrgCustomer.OrganizationCustomerID,
                        IssuedDate = salesOrderVoucher.IssuedDate,
                        Date = salesOrderVoucher.Date,
                        IsVoid = salesOrderVoucher.IsVoid,
                        IsIssued = true,
                        GrandTotal = salesOrderVoucher.GrandTotal,
                        Period = salesOrderVoucher.Period,
                        JobOrderNumber = salesOrderVoucher.JobOrderNumber,
                        PlateNo = salesOrderVoucher.PlateNo,
                        ReceiptReference = salesOrderVoucher.ReceiptReference,
                        //IsFromMaintenance = salesOrderVoucher.IsFromMaintenance,
                        LastObjectState = CRMVoucherStatusConstatns.Editing,
                        PaymentMethod = Payment,
                        BranchId = salesOrderVoucher.BranchId,
                        StoreID = salesOrderVoucher.StoreID,
                        CRMPeriodId = salesOrderVoucher.CRMPeriodId,
                        Source = salesOrderVoucher.Source,
                        //References = CRMConstants.NoReference,
                        References = salesOrderVoucher.References,
                        Remark = salesOrderVoucher.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };
                    if (entity.Source == SalesOrderSource.SalesQuote)
                    {
                        // create reference to sales JO (if any) from sales quote
                        var salesQuote = db.SalesQuoteVouchers.FirstOrDefault(q => q.SalesQuoteVoucherID.ToString() == entity.References);
                        if (salesQuote != null && salesQuote.Source == SalesQuoteSource.Maintenance)
                        {
                            entity.JobOrderNumber = salesQuote.JobOrderNumber;
                            entity.IsShipped = true; // items are out of store before already because its from maintenance

                            entity.PlateNo = salesQuote.PlateNo;
                        }
                    }

                    db.SalesOrderVouchers.Add(entity);
                    db.SaveChanges();
                    salesOrderVoucher.SalesOrderVoucherID = entity.SalesOrderVoucherID;


                    if (entity.Source == SalesOrderSource.SalesQuote)
                        this.PopulateSalseQuoteVoucherFromReference(entity, entity.References);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Sales Order Create", "Success", User.Identity.Name, $"Sales order  {entity.SalesOrderVoucherID} created.", Modules.CRM);
                    #endregion
                }
            }

            return Json(new[] { salesOrderVoucher }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesOrderVouchers_Update([DataSourceRequest]DataSourceRequest request, SalesOrderVoucher salesOrderVoucher)
        {
            if (salesOrderVoucher.OrganizationCustomerId == 0 && string.IsNullOrEmpty(salesOrderVoucher.NewCustomer))
            {
                ModelState.AddModelError("", "Select Existing customer or Insert Name of new one");
            }

            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (salesOrderVoucher.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CRMSalesHelper.CreateTempCustomer(salesOrderVoucher.NewCustomer);
                }
                else
                {
                    OrgCustomer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesOrderVoucher.OrganizationCustomerId).FirstOrDefault();
                }

                //  SalesOrderVoucher sov = db.SalesOrderVouchers.Where(x => x.SalesOrderVoucherID == salesOrderVoucher.SalesOrderVoucherID).FirstOrDefault();
                //OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesOrderVoucher.OrganizationCustomerId).FirstOrDefault();
                string Payment = "CASH";
                if (!OrgCustomer.IsCredit)
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
                    Source = salesOrderVoucher.Source,
                    Date = salesOrderVoucher.Date,
                    IsVoid = salesOrderVoucher.IsVoid,
                    GrandTotal = salesOrderVoucher.GrandTotal,
                    Period = salesOrderVoucher.Period,
                    LastObjectState = salesOrderVoucher.LastObjectState,
                    PaymentMethod = Payment,
                    Remark = salesOrderVoucher.Remark,
                    BranchId = salesOrderVoucher.BranchId,
                    StoreID = salesOrderVoucher.StoreID,
                    CRMPeriodId = salesOrderVoucher.CRMPeriodId,
                    JobOrderNumber = salesOrderVoucher.JobOrderNumber,
                    PlateNo = salesOrderVoucher.PlateNo,
                     ReceiptReference = salesOrderVoucher.ReceiptReference,
                    //IsFromMaintenance = salesOrderVoucher.IsFromMaintenance,

                    LastModified = DateTime.Now,
                    References = salesOrderVoucher.References,
                    CreatedBy = salesOrderVoucher.CreatedBy,
                    DateCreated = salesOrderVoucher.DateCreated,
                    ModifiedBy = User.Identity.Name
                };

                db.SalesOrderVouchers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Sales Order Edit", "Success", User.Identity.Name, $"Sales order  {entity.SalesOrderVoucherID} updated.", Modules.CRM);
                #endregion
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
                    Source = salesOrderVoucher.Source,
                    GrandTotal = salesOrderVoucher.GrandTotal,
                    Period = salesOrderVoucher.Period,
                    LastObjectState = salesOrderVoucher.LastObjectState,
                    JobOrderNumber = salesOrderVoucher.JobOrderNumber,
                    PlateNo = salesOrderVoucher.PlateNo,
                    ReceiptReference = salesOrderVoucher.ReceiptReference,
                    //IsFromMaintenance = salesOrderVoucher.IsFromMaintenance,
                    PaymentMethod = salesOrderVoucher.PaymentMethod,
                    Remark = salesOrderVoucher.Remark,
                    BranchId = salesOrderVoucher.BranchId,
                    StoreID = salesOrderVoucher.StoreID,
                    CRMPeriodId = salesOrderVoucher.CRMPeriodId,

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

                UserHelper.DeleteLog(Request.UserHostAddress, "Sales order delete", "Success", User.Identity.Name, $"Sales order {entity.SalesOrderVoucherID} deleted.");

            }

            return Json(new[] { salesOrderVoucher }.ToDataSourceResult(request, ModelState));
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
                            BranchId = sqv.BranchId,
                            StoreID = sqv.StoreID,
                            CRMPeriodId = sqv.CRMPeriodId,

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
                Remark = salesOrderVoucher.Remark,
                BranchId = salesOrderVoucher.BranchId,
                CRMPeriodId = salesOrderVoucher.CRMPeriodId,
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
                Remark = salesOrderVoucher.Remark,
                BranchId = salesOrderVoucher.BranchId,
                StoreID = salesOrderVoucher.StoreID,
                CRMPeriodId = salesOrderVoucher.CRMPeriodId,
            });


            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PopulateSalseQuoteVoucherFromReference(SalesOrderVoucher salesOrder, string salesQuoteVoucherID)
        {
            db = new ExceedDbContext();
            decimal Grandtot = 0;

            SalesQuoteVoucher sqv = db.SalesQuoteVouchers.Where(sq => sq.SalesQuoteVoucherID.ToString() == salesQuoteVoucherID).SingleOrDefault();
            //List<LineItem> items = db.LineItems.Where(l => l.Voucher == salesQuoteVoucherID && l.VoucherDefinitionID == "0" && (l.Status == CRMConstants.Pending || l.Status == CRMConstants.Rejected)).ToList();
            List<LineItem> items = db.LineItems.Where(l => l.Voucher == salesQuoteVoucherID && l.VoucherDefinitionID == "0").ToList();
            List<ServiceLineItem> serviceLines = db.ServiceLineItems.Where(l => l.VoucherId.ToString() == salesQuoteVoucherID && l.VoucherType == CRMVoucherType.SalesQuote).ToList();

            var AlreadyForwarded = db.SalesOrderVouchers.Where(x => x.SalesOrderVoucherID != salesOrder.SalesOrderVoucherID && x.References == sqv.SalesQuoteVoucherID.ToString()).ToList();

            if (AlreadyForwarded.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", "This Sales Quote voucher is already used in other Sales Order");
                return Json("The Voucher Already Forwarded!", JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (serviceLines.Any())
                    Grandtot += serviceLines.Sum(s => s.TotalAmount);
                if (items.Any())
                    Grandtot += items.Sum(i => i.TotalAmount);
                if (Grandtot <= 0)
                {
                    ModelState.AddModelError("DuplicateIndexEntry", "Grand Total of Referenced Sales Quote Should be greater than  0. No Line items found.");
                    return Json("Grand Total of Referenced Sales Quote Should be greater than  0. No Line items found.", JsonRequestBehavior.AllowGet);
                }
                else
                {
         
                    if (sqv != null)
                    {
                        foreach (LineItem Item in items)
                        {
                            var lineItem = new LineItem
                            {
                                VoucherDefinitionID = "1",
                                References = Item.LineItemID.ToString(),
                                CalculatedCost = Item.CalculatedCost,
                                PriceTag = Item.PriceTag,
                                Quantity = Item.Quantity,
                                Discount = Item.Discount,
                                ShipmentNumber = Item.ShipmentNumber,
                                StoreRequisitionNumber = Item.StoreRequisitionNumber,
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
                                //Voucher = referedVoucherID.ToString()
                                Voucher = salesOrder.SalesOrderVoucherID.ToString()
                            };
                            //lineItem.VoucherDefinitionID = "1";
                            lineItem.Vat = Item.Vat;
                            lineItem.WithholdingType = Item.WithholdingType;
                            lineItem.WithholdingAmount = Item.WithholdingAmount;
                            lineItem.WithholdTax = Item.WithholdTax;
                            lineItem.WithholdTaxAmount = Item.WithholdTaxAmount;
                            lineItem.Status = CRMConstants.Pending;
                            db.LineItems.Add(lineItem);
                            db.SaveChanges();
                        }
                        #region Services
                        var services = db.ServiceLineItems.Where(s => s.VoucherType == CRMVoucherType.SalesQuote && s.VoucherId == sqv.SalesQuoteVoucherID).ToList();
                        foreach (var service in services)
                        {
                            var salesOrderService = new ServiceLineItem
                            {
                                VoucherId = salesOrder.SalesOrderVoucherID,
                                VoucherType = CRMVoucherType.SalesOrder,
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
                }
            }
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

                            //var AditonalCostlog = db.AdditionalCostLogs.Where(x => x.LineItemId == item.LineItemID).ToList();
                            //foreach (var log in AditonalCostlog)
                            //{
                            //    db.AdditionalCostLogs.Attach(log);
                            //    db.AdditionalCostLogs.Remove(log);
                            //    db.SaveChanges();
                            //}
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

            var items = db.LineItems.ToList();
            decimal GrandTotal = items.Where(t => t.Voucher == salesOrderVoucherID.ToString() && t.VoucherDefinitionID == "1" && (t.Status == CRMConstants.Pending || t.Status == CRMConstants.Rejected)).Sum(x => x.TotalAmount);
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
            var lineItems = db.LineItems.ToList();
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

        public JsonResult SendForApproval(int id)
        {
            var record = db.SalesOrderVouchers.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {
                #region validate line items
                var lineItems = db.LineItems.Where(li => li.Voucher == record.SalesOrderVoucherID.ToString() && li.VoucherDefinitionID == "1").ToList();
                var services = db.ServiceLineItems.Where(li => li.VoucherId == record.SalesOrderVoucherID && li.VoucherType == CRMVoucherType.SalesOrder).ToList();
                if (!lineItems.Any() && !services.Any())
                {
                    var d = new
                    {
                        Message = "No Line Items found. Please add Line Items first",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (LineItem lineItem in lineItems)
                    {
                        if (lineItem.Quantity < 0)
                        {
                            return Json(new { Message = "Some line items have invalid  quantity.", Status = "NOT_OK" });
                        }
                        if (lineItem.TotalAmount < 0)
                        {
                            return Json(new { Message = "Some line items have invalid  amount.", Status = "NOT_OK" });
                        }
                    }
                }
                #endregion

                record.Status = OperationStatuses.WaitingApproval.ToString();
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                record.LastObjectState = OperationStatuses.WaitingApproval.ToString();
                db.SalesOrderVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Sales Order  not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
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

        [AuthorizeRoles(CRMRoles.CRMVoid)]
        public JsonResult Void(int id)
        {
            var record = db.SalesOrderVouchers.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.LastObjectState = CRMConstants.UnForwarded;
                db.SalesOrderVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
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

            var lineItems = db.LineItems.Where(l => l.Voucher == record.SalesOrderVoucherID.ToString() && l.VoucherDefinitionID == "1").ToList();
            if (!lineItems.Any())
            {
                var formattedData = new
                {
                    Message = "Please add lines and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void. You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
