using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
//using ExceedERP.Helpers.CRM;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
//using ExceedERP.Core.Domain.Construction;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesQuoteSecondApprover)]
    public class MntSalesQuoteController : Controller
    {
        public ExceedDbContext db = new ExceedDbContext();
        private readonly List<SalesQuoteVoucherView> salesQuoteVoucherViews = new List<SalesQuoteVoucherView>();
        private readonly CacheData cacheData = new CacheData();

        public ActionResult Index()
        {
            SetupViewData();

            return View();
        }

        private void SetupViewData()
        {
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();
            ViewData["QuotationTerms"] = db.QuotationTerms.Select(a => new { a.QuotationTermId, a.QuotationInDays, a.Desription }).ToList();
            ViewData["SalesStore"] = cacheData.GetSalesStoresCache().Select(s => new { SalesStoreId = s.Id, Description = s.Name }).ToList();
            //ViewData["Thing"] = cacheData.GetCrmProductsCache().Select(i => new { ProductID = i.Id, i.Name });
            ViewData["OrganizationCustomers"] = cacheData.GetCrmCustomersCache().Select(i => new { OrganizationCustomerID = i.Id, TradeName = i.Name });
            ViewData["Branches"] = cacheData.GetBranchCaches().Select(i => new { i.Name, BranchId = i.Id });
            ViewData["CRMPeriods"] = cacheData.GetCrmPeriodsCache().Select(b => new { CRMPeriodId = b.Id, b.Name });
            ViewData["RepairTypes"] = cacheData.GetRepairTypesCache().Select(r => new { RepairTypeId = r.Id, r.Name });
            ViewData["BankAccounts"] = cacheData.GetBankAccountsCache().Select(p => new { AccountName = p.Name, BankAccountId = p.Id });
        }

        public ActionResult Vouchers_Read([DataSourceRequest]DataSourceRequest request)
        {

            var quoteVouchers = db.SalesQuoteVouchers
                    .Where(b => b.Source == SalesQuoteSource.Maintenance)
                    .OrderByDescending(b => b.SalesQuoteVoucherID);

            var result = quoteVouchers.ToDataSourceResult(request, vou => new SalesQuoteVoucherView
            {
                SalesQuoteVoucherID = vou.SalesQuoteVoucherID,
                Source = vou.Source,
                //Period = vou.Period,
                OrganizationCustomerId = vou.OrganizationCustomerId,
                Date = vou.Date,
                GrandTotal = vou.GrandTotal,
                //IsIssued = vou.IsIssued,
                //IssuedDate = vou.IssuedDate,
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
            return Json(result);
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
                    //IssuedDate = salesQuoteVoucher.IssuedDate,
                    //IsIssued = salesQuoteVoucher.IsIssued,
                    Date = salesQuoteVoucher.Date,
                    IsVoid = salesQuoteVoucher.IsVoid,
                    GrandTotal = salesQuoteVoucher.GrandTotal,
                   // Period = salesQuoteVoucher.Period,
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
