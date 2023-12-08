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
using ExceedERP.Web.CRM;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.CRM;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Web.Hubs;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesQuoteUser, CRMRoles.SalesQuoteApprover, CRMRoles.CRMVoid)]
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
        public ActionResult CleanAndUpdate()
        {
            //
            // assign store id from its line item
            var emptyStores = db.SalesQuoteVouchers.Where(s => s.StoreID == 0).ToList();
            foreach (var sq in emptyStores)
            {
                var lineItem = db.LineItems.FirstOrDefault(l => l.Voucher == sq.SalesQuoteVoucherID.ToString() && l.VoucherDefinitionID == "0");
                if (lineItem != null)
                {
                    sq.StoreID = lineItem.StoreID;

                    db.SalesQuoteVouchers.Attach(sq);
                    db.Entry(sq).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            //

            return Redirect(nameof(Index));
        }
        //public ActionResult Update
        public void SetInitials()
        {
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["SalesStore"] = db.SalesStores.Select(s => new { s.SalesStoreId, s.Description }).ToList();
            ViewData["VoucherExtension"] = db.VoucherExtensions.AsQueryable();
            ViewData["Thing"] = db.Products.Select(s => new { s.ProductID, Name = s.ItemCode + " " + s.Name }).ToList();
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

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();
            ViewData["QuotationTerms"] = db.QuotationTerms.Select(a => new { a.QuotationTermId, a.QuotationInDays, a.Desription }).ToList();

            ViewData["RepairTypes"] = db.RepairType.Select(r => new { RepairTypeId = r.RepairTypeID.ToString(), r.Name });
            ViewData["BankAccounts"] = db.BankAccounts.Select(p => new { AccountName = p.AccountNumber + " " + p.AccountName + " " + p.BankBranch + " Branch", BankAccountId = p.BankAccountId });
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

            //var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            //var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));
            var assignedStores = db.SalesPersonAssignments.Where(a => a.UserName == User.Identity.Name).Select(a => a.SalesStoreId).ToList();

            var sOIds = new HashSet<string>(db.CashSalesInvoices.Where(c => c.Source == SalesInvoiceSource.SalesOrder && c.IsOnlineApproved).Select(c => c.References));
            var sQIds = new HashSet<string>(db.SalesOrderVouchers.Where(so => so.Source == SalesOrderSource.SalesQuote && so.IsOnlineApproved && sOIds.Contains(so.SalesOrderVoucherID.ToString())).Select(so => so.References));


            quoteVouchers = dbbb.SalesQuoteVouchers.Where(b => assignedStores.Contains(b.StoreID) && !sQIds.Contains(b.SalesQuoteVoucherID.ToString())).ToList();
            foreach (var vou in quoteVouchers)
            {
                decimal tottax = 0;
                decimal totgrand = 0;

                var lineItems = db.LineItems.Where(t => t.Voucher == vou.SalesQuoteVoucherID.ToString() && t.VoucherDefinitionID == "0").ToList();
                var serviceLineItems = db.ServiceLineItems.Where(t => t.VoucherId == vou.SalesQuoteVoucherID && t.VoucherType == CRMVoucherType.SalesQuote).ToList();

                if (lineItems.Any())
                {
                    tottax += lineItems.Sum(x => x.TaxAmount);
                    totgrand += lineItems.Sum(x => x.TotalAmount);
                }
                if (serviceLineItems.Any())
                {
                    tottax += serviceLineItems.Sum(x => x.TaxAmount);
                    totgrand += serviceLineItems.Sum(x => x.TotalAmount);
                }
                voulist.Add(new SalesQuoteVoucherView
                {
                    SalesQuoteVoucherID = vou.SalesQuoteVoucherID,
                    Source = vou.Source,
                    Period = vou.Period,
                    OrganizationCustomerId = vou.OrganizationCustomerId,
                    Date = vou.Date,
                    GrandTotal = totgrand,
                    IsIssued = vou.IsIssued,
                    IssuedDate = vou.IssuedDate,
                    DueDate = vou.DueDate,
                    QuotationTermId = vou.QuotationTermId,
                    IsVoid = vou.IsVoid,
                    BranchId = vou.BranchId,
                    BusinessUnitId = vou.BusinessUnitId,
                    OrgStructureId = vou.OrgStructureId,
                    CreatedAtBranchId = vou.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = vou.CreatedAtBusinessUnitId,
                    StoreID = vou.StoreID,
                    CRMPeriodId = vou.CRMPeriodId,
                    SubTotal = 0,
                    PaymentMethod = vou.PaymentMethod,
                    LastObjectState = vou.LastObjectState,
                    Remark = vou.Remark,
                    ModifiedBy = vou.ModifiedBy,
                    LastModified = vou.LastModified,
                    DateCreated = vou.DateCreated,
                    CreatedBy = vou.CreatedBy,
                    Status = vou.LastObjectState,
                    JobOrderNumber = vou.JobOrderNumber,
                    PlateNo = vou.PlateNo,
                    BankAccountId = vou.BankAccountId,

                    IsMiddleSendForApproval = vou.IsMiddleSendForApproval,
                    MiddleOnlineApprovedBy = vou.MiddleOnlineApprovedBy,
                    MiddleOnlineApprovedTime = vou.MiddleOnlineApprovedTime,
                    MiddleSendForApprovalBy = vou.MiddleSendForApprovalBy,
                    MiddleSendForApprovalTime = vou.MiddleSendForApprovalTime,
                    IsMiddleOnlineApproved = vou.IsMiddleOnlineApproved,

                    SecondOnlineApprovedBy = vou.SecondOnlineApprovedBy,
                    SecondSendForApprovalTime = vou.SecondSendForApprovalTime,
                    SecondOnlineApprovedTime = vou.SecondOnlineApprovedTime,
                    SecondSendForApprovalBy = vou.SecondSendForApprovalBy,
                    IsSecondSendForApproval = vou.IsSecondSendForApproval,
                    IsSecondOnlineApproved = vou.IsSecondOnlineApproved,
                    SecondVoidBy = vou.SecondVoidBy,
                    SecondVoidTime = vou.SecondVoidTime,
                    IsSecondVoid = vou.IsSecondVoid,

                    IsOnlineApproved = vou.IsOnlineApproved,
                    IsOnlineTransferred = vou.IsOnlineTransferred,
                    IsSendForApproval = vou.IsSendForApproval,
                    OnlineApprovedBy = vou.OnlineApprovedBy,
                    IsOnlineTransferredBy = vou.IsOnlineTransferredBy,
                    SendForApprovalTime = vou.SendForApprovalTime,
                    SendForApprovalBy = vou.SendForApprovalBy,
                    OnlineApprovedTime = vou.OnlineApprovedTime,
                    OnlineTransferredTime = vou.OnlineTransferredTime,

                });
            }
            voulist.OrderByDescending(x => x.SalesQuoteVoucherID);

            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }
        //public ActionResult Vouchers_Reada([DataSourceRequest]DataSourceRequest request, int newId)
        //{
        //    IQueryable<SalesQuoteVoucher> vouchers = db.SalesQuoteVouchers.Where(r => r.SalesQuoteVoucherID == newId);
        //    SalesQuoteVoucher sqvView = null;
        //    List<SalesQuoteVoucher> salesQuoteVoucherViews = new List<SalesQuoteVoucher>();
        //    foreach (SalesQuoteVoucher sqv in vouchers)
        //    {
        //        sqvView = new SalesQuoteVoucher();

        //        sqvView.OrganizationCustomerId = sqv.OrganizationCustomerId;
        //        sqvView.Date = sqv.Date;
        //        sqvView.GrandTotal = sqv.GrandTotal;
        //        sqvView.PaymentMethod = sqv.PaymentMethod;
        //        sqvView.Period = sqv.Period;
        //        sqvView.SalesQuoteVoucherID = sqv.SalesQuoteVoucherID;
        //        salesQuoteVoucherViews.Add(sqvView);

        //    }
        //    DataSourceResult result = salesQuoteVoucherViews.ToDataSourceResult(request, salesQuoteVoucher => new
        //    {
        //        SalesQuoteVoucherID = salesQuoteVoucher.SalesQuoteVoucherID,
        //        OrganizationCustomerId = salesQuoteVoucher.OrganizationCustomerId,
        //        IssuedDate = salesQuoteVoucher.IssuedDate,
        //        IsIssued = salesQuoteVoucher.IsIssued,
        //        Date = salesQuoteVoucher.Date,
        //        IsVoid = salesQuoteVoucher.IsVoid,
        //        GrandTotal = salesQuoteVoucher.GrandTotal,
        //        Period = salesQuoteVoucher.Period,
        //        LastObjectState = salesQuoteVoucher.LastObjectState,

        //        Remark = salesQuoteVoucher.Remark
        //    });

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult Vouchers_Forward([DataSourceRequest]DataSourceRequest request, int newId)
        //{


        //    SalesQuoteVoucher salesQuoteVoucher = db.SalesQuoteVouchers.FirstOrDefault(r => r.SalesQuoteVoucherID == newId);

        //    db = new ExceedDbContext();
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new SalesQuoteVoucher
        //        {
        //            SalesQuoteVoucherID = salesQuoteVoucher.SalesQuoteVoucherID,
        //            OrganizationCustomerId = salesQuoteVoucher.OrganizationCustomerId,
        //            IssuedDate = salesQuoteVoucher.IssuedDate,
        //            IsIssued = salesQuoteVoucher.IsIssued,
        //            Date = salesQuoteVoucher.Date,
        //            IsVoid = salesQuoteVoucher.IsVoid,
        //            BranchId = salesQuoteVoucher.BranchId,
        //            CRMPeriodId = salesQuoteVoucher.CRMPeriodId,
        //            GrandTotal = salesQuoteVoucher.GrandTotal,
        //            Period = salesQuoteVoucher.Period,
        //            LastObjectState = CRMConstants.Forwarded,
        //            Remark = salesQuoteVoucher.Remark
        //        };

        //        db.SalesQuoteVouchers.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //        Vouchers_Read(request);
        //    }

        //    return Json(new[] { salesQuoteVoucher }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);

        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vouchers_Create([DataSourceRequest]DataSourceRequest request, SalesQuoteVoucher salesQuoteVoucher)
        {
            ModelState.Remove("IssuedDate");
            ModelState.Remove("Date");
            if (salesQuoteVoucher.OrganizationCustomerId == 0 && string.IsNullOrEmpty(salesQuoteVoucher.NewCustomer))
            {
                ModelState.AddModelError("", "Select Existing customer or Insert Name of new one");
            }
            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (salesQuoteVoucher.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CRMSalesHelper.CreateTempCustomer(salesQuoteVoucher.NewCustomer);
                }
                else
                {
                    OrgCustomer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesQuoteVoucher.OrganizationCustomerId).FirstOrDefault();
                }

                //OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesQuoteVoucher.OrganizationCustomerId).FirstOrDefault();
                string Payment = "CASH";
                if (!OrgCustomer.IsCredit && salesQuoteVoucher.PaymentMethod == "CREDIT")
                {
                    Payment = "CASH";
                }
                else
                {
                    Payment = salesQuoteVoucher.PaymentMethod;
                }
                var selectedQuotationTerm = db.QuotationTerms.Find(salesQuoteVoucher.QuotationTermId);
                DateTime dueDate = salesQuoteVoucher.Date;
                if (selectedQuotationTerm != null)
                {
                    dueDate = dueDate.AddDays(selectedQuotationTerm.QuotationInDays);
                }
                var entity = new SalesQuoteVoucher
                {

                    OrganizationCustomerId = OrgCustomer.OrganizationCustomerID,
                    Source = SalesQuoteSource.Manual,
                    IssuedDate = salesQuoteVoucher.IssuedDate,
                    //Based on the setting of issuing effect
                    IsIssued = salesQuoteVoucher.IsIssued,
                    Date = salesQuoteVoucher.Date,
                    //Based on the current status of the given voucher
                    IsVoid = salesQuoteVoucher.IsVoid,
                    GrandTotal = salesQuoteVoucher.GrandTotal,
                    JobOrderNumber = salesQuoteVoucher.JobOrderNumber,
                    PlateNo = salesQuoteVoucher.PlateNo,
                    BankAccountId = salesQuoteVoucher.BankAccountId,
                    //Method to retrive date and asssign the selected period.
                    Period = salesQuoteVoucher.Period,
                    QuotationTermId = salesQuoteVoucher.QuotationTermId,
                    DueDate = dueDate,
                    PaymentMethod = Payment,
                    BranchId = salesQuoteVoucher.BranchId,
                    StoreID = salesQuoteVoucher.StoreID,
                    CRMPeriodId = salesQuoteVoucher.CRMPeriodId,
                    LastObjectState = CRMVoucherStatusConstatns.Editing,
                    Remark = salesQuoteVoucher.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.SalesQuoteVouchers.Add(entity);
                db.SaveChanges();
                salesQuoteVoucher.SalesQuoteVoucherID = entity.SalesQuoteVoucherID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Sales Quote Create", "Success", User.Identity.Name, $"Sales Quote No. {entity.SalesQuoteVoucherID} created.", Modules.CRM);
                #endregion
            }

            return Json(new[] { salesQuoteVoucher }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vouchers_Update([DataSourceRequest]DataSourceRequest request, SalesQuoteVoucher salesQuoteVoucher)
        {
            ModelState.Remove("IssuedDate");
            ModelState.Remove("Date");
            if (salesQuoteVoucher.OrganizationCustomerId == 0 && string.IsNullOrEmpty(salesQuoteVoucher.NewCustomer))
            {
                ModelState.AddModelError("", "Select Existing customer or Insert Name of new one");
            }
            if (ModelState.IsValid)
            {
                var OrgCustomer = new OrganizationCustomer();

                if (salesQuoteVoucher.OrganizationCustomerId == 0)
                {
                    OrgCustomer = CRMSalesHelper.CreateTempCustomer(salesQuoteVoucher.NewCustomer);
                }
                else
                {
                    OrgCustomer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesQuoteVoucher.OrganizationCustomerId).FirstOrDefault();
                }
                //OrganizationCustomer OrgCustormer = db.OrganizationCustomers.Where(o => o.OrganizationCustomerID == salesQuoteVoucher.OrganizationCustomerId).FirstOrDefault();
                string Payment = "CASH";
                if (!OrgCustomer.IsCredit)
                {
                    Payment = "CASH";
                }
                else
                {
                    Payment = salesQuoteVoucher.PaymentMethod;
                }
                var selectedQuotationTerm = db.QuotationTerms.Find(salesQuoteVoucher.QuotationTermId);
                DateTime dueDate = salesQuoteVoucher.Date;
                if (selectedQuotationTerm != null)
                {
                    dueDate = dueDate.AddDays(selectedQuotationTerm.QuotationInDays);
                }
                var entity = new SalesQuoteVoucher
                {
                    SalesQuoteVoucherID = salesQuoteVoucher.SalesQuoteVoucherID,
                    Source = salesQuoteVoucher.Source,
                    OrganizationCustomerId = OrgCustomer.OrganizationCustomerID,
                    IssuedDate = salesQuoteVoucher.IssuedDate,
                    IsIssued = salesQuoteVoucher.IsIssued,
                    Date = salesQuoteVoucher.Date,
                    IsVoid = salesQuoteVoucher.IsVoid,
                    GrandTotal = salesQuoteVoucher.GrandTotal,
                    Period = salesQuoteVoucher.Period,
                    PaymentMethod = Payment,
                    QuotationTermId = salesQuoteVoucher.QuotationTermId,
                    DueDate = dueDate,
                    JobOrderNumber = salesQuoteVoucher.JobOrderNumber,
                    PlateNo = salesQuoteVoucher.PlateNo,
                    BankAccountId = salesQuoteVoucher.BankAccountId,
                    LastObjectState = salesQuoteVoucher.LastObjectState,
                    Remark = salesQuoteVoucher.Remark,
                    BranchId = salesQuoteVoucher.BranchId,
                    StoreID = salesQuoteVoucher.StoreID,
                    CRMPeriodId = salesQuoteVoucher.CRMPeriodId,
                    LastModified = DateTime.Now,
                    CreatedBy = salesQuoteVoucher.CreatedBy,
                    DateCreated = salesQuoteVoucher.DateCreated,
                    ModifiedBy = User.Identity.Name
                };

                db.SalesQuoteVouchers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Sales Quote Edit", "Success", User.Identity.Name, $"Sales Quote No. {entity.SalesQuoteVoucherID} updated.", Modules.CRM);
                #endregion
            }


            return Json(new[] { salesQuoteVoucher }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vouchers_Delete([DataSourceRequest]DataSourceRequest request, SalesQuoteVoucher salesQuoteVoucher)
        {

            if (ModelState.IsValid)
            {

                var entity = new SalesQuoteVoucher
                {
                    SalesQuoteVoucherID = salesQuoteVoucher.SalesQuoteVoucherID,
                    Source = salesQuoteVoucher.Source,
                    OrganizationCustomerId = salesQuoteVoucher.OrganizationCustomerId,
                    IssuedDate = salesQuoteVoucher.IssuedDate,
                    IsIssued = salesQuoteVoucher.IsIssued,
                    Date = salesQuoteVoucher.Date,
                    IsVoid = salesQuoteVoucher.IsVoid,
                    GrandTotal = salesQuoteVoucher.GrandTotal,
                    Period = salesQuoteVoucher.Period,
                    PaymentMethod = salesQuoteVoucher.PaymentMethod,
                    QuotationTermId = salesQuoteVoucher.QuotationTermId,
                    DueDate = salesQuoteVoucher.DueDate,
                    IsOnlineApproved = salesQuoteVoucher.IsOnlineApproved,
                    IsOnlineTransferred = salesQuoteVoucher.IsOnlineTransferred,
                    IsOnlineTransferredBy = salesQuoteVoucher.IsOnlineTransferredBy,
                    IsSendForApproval = salesQuoteVoucher.IsSendForApproval,
                    OnlineApprovedBy = salesQuoteVoucher.OnlineApprovedBy,
                    OnlineApprovedTime = salesQuoteVoucher.OnlineApprovedTime,
                    OnlineTransferredTime = salesQuoteVoucher.OnlineTransferredTime,
                    SendForApprovalBy = salesQuoteVoucher.SendForApprovalBy,
                    SendForApprovalTime = salesQuoteVoucher.SendForApprovalTime,
                    Status = salesQuoteVoucher.Status,
                    VoidBy = salesQuoteVoucher.VoidBy,
                    VoidTime = salesQuoteVoucher.VoidTime,
                    LastObjectState = salesQuoteVoucher.LastObjectState,
                    JobOrderNumber = salesQuoteVoucher.JobOrderNumber,
                    PlateNo = salesQuoteVoucher.PlateNo,
                    BankAccountId = salesQuoteVoucher.BankAccountId,
                    Remark = salesQuoteVoucher.Remark,
                    BranchId = salesQuoteVoucher.BranchId,
                    StoreID = salesQuoteVoucher.StoreID,
                    CRMPeriodId = salesQuoteVoucher.CRMPeriodId,
                    LastModified = salesQuoteVoucher.LastModified,
                    CreatedBy = salesQuoteVoucher.CreatedBy,
                    DateCreated = salesQuoteVoucher.DateCreated,
                    ModifiedBy = User.Identity.Name
                };

                db.SalesQuoteVouchers.Attach(entity);
                db.SalesQuoteVouchers.Remove(entity);
                var result = db.SaveChanges();
                if (result > 0)
                {
                    DeleteVourcherLineItem(entity);
                }
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Sales Quote number " + entity.SalesQuoteVoucherID);
                #endregion
            }


            return Json(new[] { salesQuoteVoucher }.ToDataSourceResult(request, ModelState));
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
                        BranchId = sqv.BranchId,
                        CRMPeriodId = sqv.CRMPeriodId,
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
                        lineItem.Discount = Item.Discount;
                        lineItem.Voucher = referedVoucherID.ToString();
                        lineItem.VoucherDefinitionID = "1";
                        lineItem.Vat = Item.Vat;
                        lineItem.Status = CRMConstants.Pending;
                        lineItem.CreatedBy = User.Identity.Name;
                        lineItem.DateCreated = DateTime.Now;

                        db.LineItems.Add(lineItem);
                        db.SaveChanges();




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
                    BranchId = salesQuoteVoucher.BranchId,
                    CRMPeriodId = salesQuoteVoucher.CRMPeriodId,
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
                BranchId = salesQuoteVoucher.BranchId,
                CRMPeriodId = salesQuoteVoucher.CRMPeriodId,
                IsVoid = salesOrderVoucher.IsVoid,
                GrandTotal = salesOrderVoucher.GrandTotal,
                Period = salesOrderVoucher.Period,
                LastObjectState = salesOrderVoucher.LastObjectState,
                PaymentMethod = salesOrderVoucher.PaymentMethod,
                Remark = salesOrderVoucher.Remark
            });


            return Json(new[] { salesordervouchersresult }.ToDataSourceResult(request, ModelState));
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
        public JsonResult SendForApproval(int id)
        {
            var record = db.SalesQuoteVouchers.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {
                #region validate line items
                var lineItems = db.LineItems.Where(li => li.Voucher == record.SalesQuoteVoucherID.ToString() && li.VoucherDefinitionID == "0").ToList();
                var services = db.ServiceLineItems.Where(li => li.VoucherId == record.SalesQuoteVoucherID && li.VoucherType == CRMVoucherType.SalesQuote).ToList();
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
                record.LastObjectState = OperationStatuses.WaitingApproval.ToString();
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.SalesQuoteVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region notify approvers
                var iDb = new ApplicationDbContext();
                var approver = iDb.Roles.FirstOrDefault(r => r.Name == CRMRoles.SalesQuoteApprover);
                if (approver != null)
                {
                    var userRoles = approver.Users;
                    foreach (var uRole in userRoles)
                    {
                        var user = iDb.Users.Find(uRole.UserId);
                        if (user != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = user.UserName,
                                Title = "Sales quote sent for final approval",
                                Message = $"{UserInformationHelper.GetEmployeeFullName(User.Identity.Name)} " +
                                    $"has sent sales quote {record.SalesQuoteVoucherID} for final approval.",
                                URL = "/CRM/SalesQuoteVoucher",
                                DeadLine = DateTime.Now,

                            };
                            Notify.NotifyUser(notif);
                        }
                    }
                }
                #endregion
                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Sales Quote not found!",
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
        public JsonResult SendForSecondApproval(int id)
        {
            var record = db.SalesQuoteVouchers.Find(id);
            if ((record != null && !record.IsSecondSendForApproval && !record.IsSecondOnlineApproved))
            {
                #region validate line items
                var lineItems = db.LineItems.Where(li => li.Voucher == record.SalesQuoteVoucherID.ToString() && li.VoucherDefinitionID == "0").ToList();
                var services = db.ServiceLineItems.Where(li => li.VoucherId == record.SalesQuoteVoucherID && li.VoucherType == CRMVoucherType.SalesQuote).ToList();
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

                record.LastObjectState = CRMVoucherStatusConstatns.WaitingSecondApproval;
                record.IsSecondSendForApproval = true;
                record.SecondSendForApprovalBy = User.Identity.Name;
                record.SecondSendForApprovalTime = DateTime.Now;
                db.SalesQuoteVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region notify approvers
                var iDb = new ApplicationDbContext();
                var approver = iDb.Roles.FirstOrDefault(r => r.Name == CRMRoles.SalesQuoteSecondApprover);
                if (approver != null)
                {
                    var userRoles = approver.Users;
                    foreach (var uRole in userRoles)
                    {
                        var user = iDb.Users.Find(uRole.UserId);
                        if (user != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = user.UserName,
                                Title = "Sales quote sent for second approval",
                                Message = $"{UserInformationHelper.GetEmployeeFullName(User.Identity.Name)} " +
                                    $"has sent sales quote {record.SalesQuoteVoucherID} for second approval.",
                                URL = "/CRM/SalesQuoteVoucher",
                                DeadLine = DateTime.Now,

                            };
                            Notify.NotifyUser(notif);
                        }
                    }
                }
                #endregion

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Sales Quote not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSecondSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSecondOnlineApproved)
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
        public JsonResult SendForFirstApproval(int id)
        {
            var record = db.SalesQuoteVouchers.Find(id);
            if ((record != null && !record.IsMiddleSendForApproval && !record.IsMiddleOnlineApproved))
            {
                #region validate line items
                var lineItems = db.LineItems.Where(li => li.Voucher == record.SalesQuoteVoucherID.ToString() && li.VoucherDefinitionID == "0").ToList();
                var services = db.ServiceLineItems.Where(li => li.VoucherId == record.SalesQuoteVoucherID && li.VoucherType == CRMVoucherType.SalesQuote).ToList();
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

                record.LastObjectState = CRMVoucherStatusConstatns.WaitingFirstApproval;
                record.IsMiddleSendForApproval = true;
                record.MiddleSendForApprovalBy = User.Identity.Name;
                record.MiddleSendForApprovalTime = DateTime.Now;
                db.SalesQuoteVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();


                #region notify approvers
                var iDb = new ApplicationDbContext();
                var firstApprover = iDb.Roles.FirstOrDefault(r => r.Name == CRMRoles.SalesQuoteFirstApprover);
                if (firstApprover != null)
                {
                    var userRoles = firstApprover.Users;
                    foreach (var uRole in userRoles)
                    {
                        var user = iDb.Users.Find(uRole.UserId);
                        if (user != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = user.UserName,
                                Title = "Sales quote sent for first approval",
                                Message = $"{UserInformationHelper.GetEmployeeFullName(User.Identity.Name)} " +
                                    $"has sent sales quote {record.SalesQuoteVoucherID} for first approval.",
                                URL = "/CRM/SalesQuoteVoucher",
                                DeadLine = DateTime.Now,

                            };
                            Notify.NotifyUser(notif);
                        }
                    }
                }
                #endregion
                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Sales Quote not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.IsMiddleSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsMiddleOnlineApproved)
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

        [AuthorizeRoles(CRMRoles.CRMVoid)]
        public JsonResult Void(int id)
        {
            var record = db.SalesQuoteVouchers.Find(id);

            if (record != null && (record.IsOnlineApproved || record.IsMiddleOnlineApproved || record.IsSecondOnlineApproved) && !record.IsOnlineTransferred)
            {
                record.LastObjectState = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsMiddleSendForApproval = false;
                record.IsMiddleOnlineApproved = false;
                record.IsSecondSendForApproval = false;
                record.IsSecondOnlineApproved = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
                db.SalesQuoteVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                //copy
                //CopyRecord(id);
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

            var lineItems = db.LineItems.Where(l => l.Voucher == record.SalesQuoteVoucherID.ToString() && l.VoucherDefinitionID == "0").ToList();
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
            //if (!record.IsOnlineApproved)
            //{
            //    var formattedData = new
            //    {
            //        Message = "The record is not approved hence you can modify by editing.",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            return null;

        }

        public JsonResult GenerateSQFromJo(string joNumber)
        {
            var currentJobOrder = db.JobOrder.FirstOrDefault(j => j.Num.ToString() == joNumber);
            if(currentJobOrder != null)
            {
                //var result = CRMMaintenanceIntegrationHelper.CreateSalesQuoteVoucherFromMaintenance(currentJobOrder, User.Identity.Name);
                //if (result.Succeed)
                //{
                //    return Json(new
                //    {
                //        Message = result.SuccessMessage,
                //        Status = "Ok"
                //    }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(new
                //    {
                //        Message = result.ErrorMessage,
                //        Status = "Error"
                //    }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new
                {
                    Message = "Error",
                    Status = "Error"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "JO not found", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public void DeleteVourcherLineItem(SalesQuoteVoucher salesQuoteVoucher)
        {
            var lineItems = db.LineItems.Where(l => l.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString() && l.VoucherDefinitionID == "0").ToList();
            if (lineItems.Any())
            {
                db.LineItems.RemoveRange(lineItems);
                db.SaveChanges();
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
