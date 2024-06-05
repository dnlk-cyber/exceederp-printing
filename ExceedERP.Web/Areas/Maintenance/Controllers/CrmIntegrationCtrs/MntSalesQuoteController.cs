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
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.CRM;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Construction;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesQuoteSecondApprover)]
    public class MntSalesQuoteController : Controller
    {
        public ExceedDbContext db = new ExceedDbContext();
        List<SalesQuoteVoucherView> salesQuoteVoucherViews = new List<SalesQuoteVoucherView>();
       

        public ActionResult Index()
        {
            SetInitials();

            return View();
        }
        
        public void SetInitials()
        {
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["SalesStore"] = db.SalesStores.Select(s => new { s.SalesStoreId, s.Description }).ToList();
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

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();
            ViewData["QuotationTerms"] = db.QuotationTerms.Select(a => new { a.QuotationTermId, a.QuotationInDays, a.Desription }).ToList();

            ViewData["RepairTypes"] = db.RepairType.Select(r => new { RepairTypeId = r.RepairTypeID.ToString(), r.Name });
        }
       
       
        public ActionResult Vouchers_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<SalesQuoteVoucherView> voulist = new List<SalesQuoteVoucherView>();
            List<SalesQuoteVoucher> quoteVouchers = new List<SalesQuoteVoucher>();

            //var assignedStores = db.SalesPersonAssignments.Where(a => a.UserName == User.Identity.Name).Select(a => a.SalesStoreId).ToList();

            quoteVouchers = db.SalesQuoteVouchers
                    .Where(b => b.Source == SalesQuoteSource.Maintenance 
                            //b.IsMiddleOnlineApproved &&
                            //!b.IsSecondOnlineApproved &&
                            //assignedStores.Contains(b.StoreID)
                            ).ToList();
            foreach (var vou in quoteVouchers)
            {
                decimal tottax =0;
                decimal totgrand  =0;

                var lineItems = db.LineItems.Where(t => t.Voucher == vou.SalesQuoteVoucherID.ToString() && t.VoucherDefinitionID == "0").ToList();
                var serviceLineItems = db.ServiceLineItems.Where(t => t.VoucherId == vou.SalesQuoteVoucherID && t.VoucherType == CRMVoucherType.SalesQuote).ToList();

                //SalesQuoteVoucherView vsqvView = new SalesQuoteVoucherView();
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
                    LastObjectState = salesQuoteVoucher.LastObjectState,
                    PlateNo = salesQuoteVoucher.PlateNo,
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

       
        
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
