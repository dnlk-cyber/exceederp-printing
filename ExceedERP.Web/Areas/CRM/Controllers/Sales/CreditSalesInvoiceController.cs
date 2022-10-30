using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.CRM;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CreditSalesApprover, CRMRoles.CreditSalesUser)]
    public class CreditsSalesInvoiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        List<CashSalesInvoice> salesQuoteVoucherViews = new List<CashSalesInvoice>();
        //public ExceedDbContext db = new ExceedDbContext();
        //public ExceedDbContext dbbb = new ExceedDbContext();
        public List<LineItem> lineItems = new List<LineItem>();
        public ActionResult Index()
        {
            SetInitials();
            // temp
            CheckAndUpdateOldDueDates();
            CheckUnUpdatedGrandTotal();
            //

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
            ViewData["SalesOrderVoucherList"] = db.SalesOrderVouchers.Where(s => s.LastObjectState != CRMConstants.Forwarded && s.PaymentMethod == CRMConstants.CreditPayment);

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();
            ViewData["PaymentTerms"] = db.PaymentTerms.Select(b => new { PaymentTermsID = b.PaymentTermsID, PaymentTermsDays = b.PaymentTermsDays + " Days" }).ToList();
            //ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();
            ViewData["RepairTypes"] = db.RepairType.Select(r => new { RepairTypeId = r.RepairTypeID.ToString(), r.Name });
        }
        [Authorize(Users = "tester")]
        public ActionResult AddDistribution()
        {
            var cashSalesInvoices = db.CashSalesInvoices.Where(cs => !cs.IsOnlineTransferred && cs.PaymentMethod == CRMConstants.CreditPaymentMethod).ToList();
            foreach (CashSalesInvoice invoice in cashSalesInvoices)
            {
                new CRMPaymentDistibutionHelper().DistributeCreditInvoice(invoice);
            }
            return Redirect(nameof(Index));
        }
        private void CheckAndUpdateOldDueDates()
        {
            var dirtyData = db.CashSalesInvoices.Where(c => c.PaymentTermsID != 0 && c.DueDate == null).ToList();
            if (dirtyData.Any())
            {
                foreach (var invoice in dirtyData)
                {
                    var selectedPaymentTerm = db.PaymentTerms.Find(invoice.PaymentTermsID);
                    if (selectedPaymentTerm != null)
                    {
                        invoice.DueDate = invoice.Date.AddDays(selectedPaymentTerm.PaymentTermsDays);

                        db.CashSalesInvoices.Attach(invoice);
                        db.Entry(invoice).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        private void CheckUnUpdatedGrandTotal()
        {
            var records = db.CashSalesInvoices.Where(c => c.GrandTotal == 0 || c.AmountDue == 0).ToList();
            if (records.Any())
            {
                foreach (var rec in records)
                {
                    decimal total = 0;
                    var lineItems = db.LineItems.Where(t => t.Voucher == rec.CashSalesInvoiceID.ToString() && t.VoucherDefinitionID == "2").ToList();
                    if (lineItems.Any())
                    {
                        total = lineItems.Sum(l => l.TotalAmount);
                    }
                    if (total > 0)
                    {
                        var settlementLineItems =
                           db.LineCreditSettlements.Where(l => l.InvoiceId == rec.CashSalesInvoiceID).ToList();

                        decimal settledAmount = settlementLineItems.Any() ? settlementLineItems.Sum(l => l.SettelmentAmount) : 0;

                        rec.GrandTotal = total;


                        rec.AmountDue = total - settledAmount;

                        db.CashSalesInvoices.Attach(rec);
                        db.Entry(rec).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        public ActionResult CreditsSalesInvoices_Read([DataSourceRequest]DataSourceRequest request, bool showTransferableOnly = false)
        {

            List<CashSalesInvoice> voulist = new List<CashSalesInvoice>();

            //try
            //{

            //lineItems = db.LineItems.ToList();

            //List<CashSalesInvoice> invoiceList = new List<CashSalesInvoice>();

            //var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            //var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));
            var assignedStores = db.SalesPersonAssignments.Where(a => a.UserName == User.Identity.Name).Select(a => a.SalesStoreId).ToList();

            var invoiceList = db.CashSalesInvoices.Where(d => assignedStores.Contains(d.StoreID) && d.PaymentMethod == CRMConstants.CreditPaymentMethod).ToList();
            if (showTransferableOnly)
            {
                invoiceList = invoiceList.Where(i => i.IsOnlineApproved && !i.IsOnlineTransferred).ToList();
            }
            foreach (var vou in invoiceList)
            {
                //var Customer = db.OrganizationCustomers.Where(x => x.OrganizationCustomerID == vou.OrganizationCustomerId).FirstOrDefault();

                //if (Customer != null && vou.PaymentMethod == CRMConstants.CreditPaymentMethod)
                //{
                //SalesQuoteVoucherView vsqvView = new SalesQuoteVoucherView();
                //var tempLines = lineItems.Where(t => t.Voucher == vou.CashSalesInvoiceID.ToString() && t.VoucherDefinitionID == "2");
                //decimal totsub = tempLines.Any() ? tempLines.Sum(s => s.CalculatedCost) : 0;
                //decimal totgrand = lineItems.Where(t => t.Voucher == vou.CashSalesInvoiceID.ToString() && t.VoucherDefinitionID == "2").Sum(x => x.TotalAmount);

                //var settlementLineItems =
                //    db.LineCreditSettlements.Where(l => l.InvoiceId == vou.CashSalesInvoiceID).ToList();
                //decimal settledAmount = settlementLineItems.Any() ? settlementLineItems.Sum(l => l.SettelmentAmount) : 0;

                string SalesStatus = string.Empty;

                if (vou.Source == SalesInvoiceSource.SalesOrder && !string.IsNullOrEmpty(vou.References))
                {
                    List<InvoiceReferenceSelectViewModel> referencedIds = new List<InvoiceReferenceSelectViewModel>();
                    var concatnatedRef = vou.References.Trim();
                    if (!string.IsNullOrEmpty(concatnatedRef))
                    {
                        var idsArray = concatnatedRef.Split(',');
                        if (idsArray.Length > 0)
                        {
                            for (int i = 0; i < idsArray.Length; i++)
                            {
                                int vl = 0;
                                int.TryParse(idsArray[i], out vl);
                                var record = new InvoiceReferenceSelectViewModel
                                {
                                    Display = idsArray[i],
                                    Value = vl
                                };
                                referencedIds.Add(record);
                            }
                        }
                    }
                    //vou.ReferencedSalesOrderIds = referencedIds.ToArray();
                    vou.ReferencedSalesOrderIds = referencedIds;
                }
                voulist.Add(new CashSalesInvoice
                {
                    CashSalesInvoiceID = vou.CashSalesInvoiceID,
                    Period = vou.Period,
                    OrganizationCustomerId = vou.OrganizationCustomerId,
                    Date = vou.Date,
                    GrandTotal = vou.GrandTotal,
                    AmountDue = vou.AmountDue,
                    //GrandTotal = totgrand,
                    //AmountDue = totgrand - settledAmount,
                    DueDate = vou.DueDate,
                    IsIssued = vou.IsIssued,
                    AdvancPayment = vou.AdvancPayment,
                    OnHandPayment = vou.OnHandPayment,
                    IssuedDate = vou.IssuedDate,
                    IsVoid = vou.IsVoid,
                    References = vou.References,
                    ReferencedSalesOrderIds = vou.ReferencedSalesOrderIds,
                    IsSendForApproval = vou.IsSendForApproval,
                    PaymentMethod = vou.PaymentMethod,
                    CRMPeriodId = vou.CRMPeriodId,
                    BranchId = vou.BranchId,
                    StoreID = vou.StoreID,
                    PaymentTermsID = vou.PaymentTermsID,
                    ReceiptReference = vou.ReceiptReference,
                    IsManual = vou.IsManual,
                    Source = vou.Source,

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
                    JobOrderNumber = vou.JobOrderNumber,
                    PlateNo = vou.PlateNo,
                    //IsFromMaintenance = vou.IsFromMaintenance,

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
                //}
            }

            //}
            //catch (Exception e)
            //{

            //    string ee = e.InnerException.Message;
            //}
            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditsSalesInvoices_Create([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            if (cashSalesInvoice.IsManual && string.IsNullOrEmpty(cashSalesInvoice.ReceiptReference))
            {
                ModelState.AddModelError("", "Receipt reference number is required for manual purchase invoices");
            }
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
                var concatnatedRef = "";
                var concatnatedJO = "";
                var concatnatedPlateNo = "";
                //bool isFromMaintenance = false;
                int length = cashSalesInvoice.ReferencedSalesOrderIds.Count;
                var selectedIds = cashSalesInvoice.ReferencedSalesOrderIds;
                if (length > 0)
                {
                    if (length == 1)
                    {
                        concatnatedRef += selectedIds[0].Value;
                        var so = db.SalesOrderVouchers.Find(selectedIds[0].Value);
                        if(so!= null)
                        {
                            concatnatedJO += so.JobOrderNumber;
                            concatnatedPlateNo += so.PlateNo;
                        }
                    }
                    else
                    {
                        concatnatedRef += selectedIds[0].Value;
                        var so = db.SalesOrderVouchers.Find(selectedIds[0].Value);
                        if (so != null)
                        {
                            concatnatedJO += so.JobOrderNumber;
                            concatnatedPlateNo += so.PlateNo;
                        }
                        for (int i = 1; i < length - 1; i++)
                        {
                            concatnatedRef = concatnatedRef + "," + selectedIds[i].Value;
                            var soi = db.SalesOrderVouchers.Find(selectedIds[i]);
                            if (soi != null)
                            {
                                concatnatedJO = concatnatedJO + "," + soi.JobOrderNumber;
                                concatnatedPlateNo = concatnatedPlateNo + "," + soi.PlateNo;
                            }
                        }
                        concatnatedRef += "," + selectedIds[length - 1].Value;
                        var solast = db.SalesOrderVouchers.Find(selectedIds[length - 1].Value);
                        if (solast != null)
                        {
                            concatnatedJO = concatnatedJO + "," + solast.JobOrderNumber;
                            concatnatedPlateNo = concatnatedPlateNo + "," + solast.PlateNo;
                        }
                    }
                }
                if (OrgCustormer.IsCredit)
                {
                    var selectedPaymentTerm = db.PaymentTerms.Find(cashSalesInvoice.PaymentTermsID);
                    DateTime dueDate = cashSalesInvoice.Date;
                    if (selectedPaymentTerm != null)
                    {
                        dueDate = dueDate.AddDays(selectedPaymentTerm.PaymentTermsDays);
                    }
                    var entity = new CashSalesInvoice
                    {

                        OrganizationCustomerId = cashSalesInvoice.OrganizationCustomerId,
                        IssuedDate = cashSalesInvoice.IssuedDate,
                        IsIssued = cashSalesInvoice.IsIssued,
                        Date = cashSalesInvoice.Date,
                        DueDate = dueDate,
                        FsInvoiceNo = cashSalesInvoice.FsInvoiceNo,

                        CRMPeriodId = cashSalesInvoice.CRMPeriodId,
                        BranchId = cashSalesInvoice.BranchId,
                        StoreID = cashSalesInvoice.StoreID,
                        PaymentTermsID = cashSalesInvoice.PaymentTermsID,
                        ReceiptReference = cashSalesInvoice.ReceiptReference,
                        IsManual = cashSalesInvoice.IsManual,
                        IsVoid = cashSalesInvoice.IsVoid,
                        GrandTotal = cashSalesInvoice.GrandTotal,
                        Period = cashSalesInvoice.Period,
                        LastObjectState = CRMVoucherStatusConstatns.Editing,
                        PaymentMethod = CRMConstants.CreditPaymentMethod,
                        RecieveMethod = cashSalesInvoice.RecieveMethod,
                        Remark = cashSalesInvoice.Remark,
                        //References = CRMConstants.NoReference,
                        Source = cashSalesInvoice.Source,
                        References = concatnatedRef,
                        JobOrderNumber = concatnatedJO,
                        PlateNo = concatnatedPlateNo,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.CashSalesInvoices.Add(entity);
                    db.SaveChanges();
                    cashSalesInvoice.CashSalesInvoiceID = entity.CashSalesInvoiceID;

                    entity.InvoiceNumber = entity.CashSalesInvoiceID.ToString();
                    db.CashSalesInvoices.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    if (entity.Source == SalesInvoiceSource.SalesOrder)
                    {
                        this.PopulatedReferencedSalesOrderDataToInvoice(cashSalesInvoice.ReferencedSalesOrderIds, entity);
                    }

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Credit Sales Create", "Success", User.Identity.Name, $"Credit Sales {entity.CashSalesInvoiceID} Created.", Modules.CRM);
                    #endregion
                }
            }

            return Json(new[] { cashSalesInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditsSalesInvoices_Update([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
        {
            if (cashSalesInvoice.IsManual && string.IsNullOrEmpty(cashSalesInvoice.ReceiptReference))
            {
                ModelState.AddModelError("", "Receipt reference number is required for manual purchase invoices");
            }
            if (ModelState.IsValid)
            {
                OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == cashSalesInvoice.OrganizationCustomerId).FirstOrDefault();

                if (OrgCustormer.IsCredit)
                {
                    var selectedPaymentTerm = db.PaymentTerms.Find(cashSalesInvoice.PaymentTermsID);
                    DateTime dueDate = cashSalesInvoice.Date;
                    if (selectedPaymentTerm != null)
                    {
                        dueDate = dueDate.AddDays(selectedPaymentTerm.PaymentTermsDays);
                    }
                    var entity = new CashSalesInvoice
                    {
                        CashSalesInvoiceID = cashSalesInvoice.CashSalesInvoiceID,
                        OrganizationCustomerId = cashSalesInvoice.OrganizationCustomerId,
                        IssuedDate = cashSalesInvoice.IssuedDate,
                        IsIssued = cashSalesInvoice.IsIssued,
                        Date = cashSalesInvoice.Date,
                        DueDate = dueDate,
                        IsVoid = cashSalesInvoice.IsVoid,
                        GrandTotal = cashSalesInvoice.GrandTotal,
                        Period = cashSalesInvoice.Period,
                        LastObjectState = cashSalesInvoice.LastObjectState,
                        PaymentMethod = CRMConstants.CreditPaymentMethod,
                        CRMPeriodId = cashSalesInvoice.CRMPeriodId,
                        BranchId = cashSalesInvoice.BranchId,
                        StoreID = cashSalesInvoice.StoreID,
                        PaymentTermsID = cashSalesInvoice.PaymentTermsID,
                        ReceiptReference = cashSalesInvoice.ReceiptReference,
                        IsManual = cashSalesInvoice.IsManual,
                        RecieveMethod = cashSalesInvoice.RecieveMethod,
                        References = cashSalesInvoice.References,
                        Source = cashSalesInvoice.Source,
                        Remark = cashSalesInvoice.Remark,
                        CreatedBy = cashSalesInvoice.CreatedBy,
                        DateCreated = cashSalesInvoice.DateCreated,

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

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Credit Sales Edit", "Success", User.Identity.Name, $"Credit Sales {entity.CashSalesInvoiceID} updated.", Modules.CRM);
                    #endregion
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


                        //var AditonalCostlog = db.AdditionalCostLogs.Where(x => x.LineItemId == item.LineItemID).ToList();
                        //foreach (var log in AditonalCostlog)
                        //{
                        //    db.AdditionalCostLogs.Attach(log);
                        //    db.AdditionalCostLogs.Remove(log);
                        //    db.SaveChanges();
                        //}
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
        public ActionResult CreditSalesInvoice_Destroy([DataSourceRequest]DataSourceRequest request, CashSalesInvoice cashSalesInvoice)
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
                    }
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Credit Sales Delete", "Success", User.Identity.Name, $"Credit Sales {invoice.CashSalesInvoiceID} deleted." );
                    #endregion
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
        public JsonResult RegenerateDistribution(int id)
        {
            var record = db.CashSalesInvoices.Find(id);
            if ((record != null && !record.IsOnlineTransferred))
            {
                //new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(record);
                new CRMPaymentDistibutionHelper().DistributeCreditInvoice(record);
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
        //public ActionResult PopulateSalseOrderVoucherForReference([DataSourceRequest]DataSourceRequest request, string salesOrderVoucherID)
        //{
        //    db = new ExceedDbContext();
        //    decimal Grandtot = 0;

        //    SalesOrderVoucher sov = db.SalesOrderVouchers.Where(sq => sq.SalesOrderVoucherID.ToString() == salesOrderVoucherID).SingleOrDefault();
        //    List<LineItem> items = db.LineItems.Where(l => l.Voucher == salesOrderVoucherID && l.VoucherDefinitionID == "1" && (l.Status == CRMConstants.Pending || l.Status == CRMConstants.Rejected)).ToList();//.Sum(s => s.CalculatedCost);

        //    var AlreadyForwarded = db.CashSalesInvoices.Where(x => x.References == sov.SalesOrderVoucherID.ToString()).ToList();

        //    if (AlreadyForwarded.Any())
        //    {
        //        ModelState.AddModelError("DuplicateIndexEntry", "The Voucher Already Forwarded!!!");
        //        return Json("The Voucher Already Forwarded!", JsonRequestBehavior.AllowGet);

        //    }

        //    foreach (var item in items)
        //    {
        //        Grandtot = Grandtot + item.TotalAmount;
        //    }
        //    if (Grandtot < 1)
        //    {
        //        ModelState.AddModelError("DuplicateIndexEntry", " Grand Total balance Must not be 0 !!!");
        //        return Json("Grand Total balance Must not be 0 !", JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        db = new ExceedDbContext();
        //        int referedVoucherID;
        //        LineItem lineItem;
        //        VoucherTerm _voucherTerm;
        //        VoucherExtensionTransaction _voucherExtensionTransaction;
        //        if (sov != null)
        //        {
        //            var entity = new CashSalesInvoice
        //            {
        //                OrganizationCustomerId = sov.OrganizationCustomerId,
        //                IssuedDate = sov.IssuedDate,
        //                IsIssued = sov.IsIssued,
        //                Date = sov.Date,
        //                IsVoid = sov.IsVoid,
        //                GrandTotal = sov.GrandTotal,
        //                Period = sov.Period,
        //                LastObjectState = sov.LastObjectState,
        //                PaymentMethod = sov.PaymentMethod,
        //                CreatedBy = User.Identity.Name,
        //                DateCreated = DateTime.Now,
        //                References = salesOrderVoucherID,

        //                Remark = sov.Remark,
        //                CRMPeriodId = sov.CRMPeriodId,
        //                BranchId = sov.BranchId,
        //                PaymentTermsID = sov.BranchId,
        //            };

        //            db.CashSalesInvoices.Add(entity);
        //            db.SaveChanges();
        //            referedVoucherID = entity.CashSalesInvoiceID;

        //            List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == salesOrderVoucherID && Item.VoucherDefinitionID == "1" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();
        //            foreach (LineItem Item in lineItemList)
        //            {
        //                lineItem = new LineItem();
        //                //This may be amended to be the uptodated value
        //                lineItem.CalculatedCost = Item.CalculatedCost;
        //                lineItem.References = salesOrderVoucherID;
        //                lineItem.PriceTag = Item.PriceTag;
        //                lineItem.Quantity = Item.Quantity;
        //                lineItem.Remark = Item.Remark;
        //                lineItem.Class = Item.Class;
        //                lineItem.Variety = Item.Variety;
        //                //This may be amended to be the uptodated value
        //                lineItem.StockBalance = Item.StockBalance;
        //                //This may be attached with StoreTransaction table
        //                lineItem.StoreID = Item.StoreID;
        //                lineItem.Tax = Item.Tax;
        //                lineItem.TaxAmount = Item.TaxAmount;
        //                lineItem.ThingID = Item.ThingID;
        //                lineItem.TotalAmount = Item.TotalAmount;
        //                lineItem.UnitAmount = Item.UnitAmount;
        //                lineItem.Voucher = referedVoucherID.ToString();
        //                lineItem.VoucherDefinitionID = "2";
        //                lineItem.WithholdingType = Item.WithholdingType;
        //                lineItem.WithholdingAmount = Item.WithholdingAmount;
        //                lineItem.WithholdTax = Item.WithholdTax;
        //                lineItem.WithholdTaxAmount = Item.WithholdTaxAmount;
        //                lineItem.Vat = Item.Vat;

        //                lineItem.Status = CRMConstants.Pending;

        //                db.LineItems.Add(lineItem);
        //                db.SaveChanges();


        //                var AditonalCostlog = db.AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList();
        //                foreach (var log in AditonalCostlog)
        //                {
        //                    log.LineItemId = lineItem.LineItemID;
        //                    db.AdditionalCostLogs.Add(log);
        //                    db.SaveChanges();
        //                }

        //            }

        //            foreach (LineItem Item in lineItemList)
        //            {
        //                Item.Status = CRMConstants.Forwarded;
        //                db.LineItems.Attach(Item);
        //                db.Entry(entity).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //            List<VoucherTerm> voucherTermList = db.VoucherTerms.Where(vt => vt.Voucher == sov.References).ToList();
        //            foreach (VoucherTerm voucherTerm in voucherTermList)
        //            {
        //                _voucherTerm = new VoucherTerm();
        //                _voucherTerm.Remark = voucherTerm.Remark;
        //                _voucherTerm.Term = voucherTerm.Term;
        //                _voucherTerm.Value = voucherTerm.Value;
        //                _voucherTerm.Voucher = referedVoucherID.ToString();
        //                _voucherTerm.VoucherDefinitionID = 2;
        //                db.VoucherTerms.Add(_voucherTerm);
        //                db.SaveChanges();
        //            }
        //            List<VoucherExtensionTransaction> voucherExtensionTransactionList = db.VoucherExtensionTransactions.Where(ve => ve.Voucher == sov.References).ToList();
        //            foreach (VoucherExtensionTransaction voucherExtensionTrxn in voucherExtensionTransactionList)
        //            {
        //                _voucherExtensionTransaction = new VoucherExtensionTransaction();
        //                _voucherExtensionTransaction.Number = voucherExtensionTrxn.Number;
        //                _voucherExtensionTransaction.Remark = voucherExtensionTrxn.Remark;
        //                _voucherExtensionTransaction.Voucher = referedVoucherID.ToString();

        //                _voucherExtensionTransaction.VoucherExtension = voucherExtensionTrxn.VoucherExtension;
        //                db.VoucherExtensionTransactions.Add(_voucherExtensionTransaction);
        //                db.SaveChanges();
        //            }

        //        }
        //        ExceedDbContext db = new ExceedDbContext();
        //        SalesOrderVoucher sovup = db.SalesOrderVouchers.Where(so => so.SalesOrderVoucherID.ToString() == salesOrderVoucherID).FirstOrDefault();
        //        sovup.LastObjectState = CRMConstants.Forwarded;
        //        db.SalesOrderVouchers.Attach(sovup);
        //        db.Entry(sovup).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    return Json(string.Empty, JsonRequestBehavior.AllowGet);
        //}
        private ActionResult PopulatedReferencedSalesOrderDataToInvoice(List<InvoiceReferenceSelectViewModel> selectedIds, CashSalesInvoice invoice)
        {
            var db = new ExceedDbContext();
            decimal Grandtot = 0;

            for (int i = 0; i < selectedIds.Count; i++)
            {
                var referencedSalesOrderVoucherId = selectedIds[i].Value.ToString();

                var salesOrderVoucher = db.SalesOrderVouchers.FirstOrDefault(sq => sq.SalesOrderVoucherID.ToString() == referencedSalesOrderVoucherId);

                //List<LineItem> items = db.LineItems.Where(l => l.Voucher == referencedSalesOrderVoucherId && l.VoucherDefinitionID == "1" && (l.Status == CRMConstants.Pending || l.Status == CRMConstants.Rejected)).ToList();//.Sum(s => s.CalculatedCost);
                List<LineItem> items = db.LineItems.Where(l => l.Voucher == referencedSalesOrderVoucherId && l.VoucherDefinitionID == "1").ToList();//.Sum(s => s.CalculatedCost);
                List<ServiceLineItem> serviceItems = db.ServiceLineItems.Where(l => l.VoucherId.ToString() == referencedSalesOrderVoucherId && l.VoucherType == CRMVoucherType.SalesOrder).ToList();//.Sum(s => s.CalculatedCost);


                //SalesOrderVoucher sov = db.SalesOrderVouchers.Where(sq => sq.SalesOrderVoucherID.ToString() == salesOrderVoucherID).SingleOrDefault();
                if (salesOrderVoucher != null)
                {
                    // check if this very Order voucher is referenced in Invoice vouchers other than the current invoice (invoice passed as a parameter this method)
                    var AlreadyForwarded = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID != invoice.CashSalesInvoiceID && x.References == salesOrderVoucher.SalesOrderVoucherID.ToString()).ToList();

                    if (AlreadyForwarded.Any())
                    {
                        ModelState.AddModelError("DuplicateIndexEntry", "The Voucher with Sales order voucher Number " + salesOrderVoucher.SalesOrderVoucherID.ToString() + " is  Already Forwarded!");
                        return Json("The Voucher with Sales order voucher Number " + salesOrderVoucher.SalesOrderVoucherID.ToString() + " is  Already Forwarded!", JsonRequestBehavior.AllowGet);
                    }

                    foreach (var item in items)
                    {
                        Grandtot  += item.TotalAmount;
                    }
                    foreach (var item in serviceItems)
                    {
                        Grandtot += item.TotalAmount;
                    }
                    if (Grandtot <= 0)
                    {
                        ModelState.AddModelError("ZeroGrandTotal", " Grand Total balance Must not be 0 !!!");
                        return Json("Grand Total balance Must not be 0 !", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db = new ExceedDbContext();
                        //int referedVoucherID;
                        LineItem lineItem;
                        
                        //List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == salesOrderVoucher.SalesOrderVoucherID.ToString() && Item.VoucherDefinitionID == "1" && (Item.Status == CRMConstants.Pending || Item.Status == CRMConstants.Rejected)).ToList();
                        List<LineItem> lineItemList = db.LineItems.ToList().Where(Item => Item.Voucher == salesOrderVoucher.SalesOrderVoucherID.ToString() && Item.VoucherDefinitionID == "1").ToList();

                        foreach (LineItem Item in lineItemList)
                        {
                            lineItem = new LineItem();
                            //This may be amended to be the uptodated value
                            lineItem.CalculatedCost = Item.CalculatedCost;
                            lineItem.ShipmentNumber = Item.ShipmentNumber;
                            lineItem.StoreRequisitionNumber = Item.StoreRequisitionNumber;
                            lineItem.References = salesOrderVoucher.SalesOrderVoucherID.ToString();
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
                            lineItem.Discount = Item.Discount;

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
                        }

                        foreach (LineItem Item in lineItemList)
                        {
                            Item.Status = CRMConstants.Forwarded;
                            db.LineItems.Attach(Item);
                            db.Entry(Item).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        #region Services
                        var services = db.ServiceLineItems.Where(s => s.VoucherType == CRMVoucherType.SalesOrder && s.VoucherId == salesOrderVoucher.SalesOrderVoucherID).ToList();
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


                    new CRMPaymentDistibutionHelper().DistributeCreditInvoice(invoice);
                }
                else
                {
                    return null;
                }
                //return null;
            }
            return Json("Invoice Created and all Line Items from reference Sales Orders were Added !", JsonRequestBehavior.AllowGet);
        }

        public JsonResult AcceptPayment(int id)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
            //try
            //{
            var db = new ExceedDbContext();

            var currentInvoice = db.CashSalesInvoices.Find(id);
            var GLTaxRates = db.GLTaxRates.ToList();
            var PaymentMethods = db.PaymentMethods.ToList();
            var AdditionalCostLogs = db.AdditionalCostLogs.ToList();
            var AdditionalCosts = db.AdditionalCosts.ToList();
            var AccountMaps = db.AccountMaps.ToList();
            if (currentInvoice == null)
            {
                return Json(new { Message = "Invoice not found!", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                OrganizationCustomer OrgCustomer = db.OrganizationCustomers.Where(c => c.OrganizationCustomerID == currentInvoice.OrganizationCustomerId).FirstOrDefault();

                if (OrgCustomer != null && OrgCustomer.IsCredit)
                {

                    CreditSetting creditSetting =
                        db.CreditSettings.Where(c => c.OrganizationID == OrgCustomer.OrganizationCustomerID)
                            .OrderByDescending(c => c.CreditSettingID)
                            .FirstOrDefault();
                    if (creditSetting != null)
                    {
                        if (creditSetting.Quantity < currentInvoice.GrandTotal)
                        {
                            ModelState.AddModelError("InsufficentCredit",
                                "Customer has Insufficient Credit");


                            return Json(new { Message = "Customer has Insufficient Credit", Status = "Error" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            List<LineItem> InvoiceItems =
                                db.LineItems.Where(
                                    l => l.Voucher == currentInvoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2" && l.Status == CRMConstants.Pending)
                                    .ToList();

                            if (InvoiceItems != null)
                            {
                                foreach (var InvoiceItem in InvoiceItems)
                                {

                                    InvoiceItem.Status = CRMConstants.CreditPayment;
                                    db.LineItems.Attach(InvoiceItem);
                                    db.Entry(InvoiceItem).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCreditInvoice(currentInvoice, InvoiceItem, AdditionalCosts, AdditionalCostLogs, GLTaxRates, PaymentMethods, AccountMaps);

                                    //if (SalesInvoicePaymentDistibutions.Any())
                                    //{
                                    //    db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                                    //    db.SaveChanges();
                                    //}
                                }


                                currentInvoice.LastObjectState = CRMConstants.PaymentReceived;
                                db.CashSalesInvoices.Attach(currentInvoice);
                                db.Entry(currentInvoice).State = EntityState.Modified;
                                db.SaveChanges();

                                decimal FinalCredit = decimal.Parse(creditSetting.Quantity.ToString()) -
                                                currentInvoice.GrandTotal;

                                creditSetting.Quantity = FinalCredit;
                                db.CreditSettings.Attach(creditSetting);
                                db.Entry(creditSetting).State = EntityState.Modified;
                                db.SaveChanges();



                                var CreditInvoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == currentInvoice.CashSalesInvoiceID).FirstOrDefault();
                                if (CreditInvoice != null)
                                {
                                    CreditInvoice.IsOnlineApproved = true;
                                    CreditInvoice.OnlineApprovedBy = User.Identity.Name;
                                    CreditInvoice.OnlineApprovedTime = DateTime.Now;


                                    db.CashSalesInvoices.Attach(CreditInvoice);
                                    db.Entry(CreditInvoice).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                            }
                        }
                    }

                }



                return Json(new { Message = "Payment approved!", Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }

            //}

            //catch (Exception e)
            //{

            //}
        }
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
                    PaymentType = "2", // credit 
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
