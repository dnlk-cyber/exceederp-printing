using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Invoice
{
    public class ArInvoiceTempController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();



        [AuthorizeRoles(FinanceConstants.AccountReceivableView)]
        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }

        private void PopulateViewData()
        {
            var terms = _db.PaymentTerms.ToList();

            ViewData["terms"] = terms.Select(p => new PaymentTerms
            {
                PaymentTermsDays = p.PaymentTermsDays
            });

            var customers = _db.ARCustomerProfiles.Where(x => x.IsActive).ToList();

            ViewData["customers"] = customers.Select(c => new ArCustomerProfile
            {
                CustomerId = c.CustomerId,
                CompanyName = c.CompanyName
            });


            var tax = _db.GLTaxRates.Where(x => x.ApplyToAr).ToList();

            ViewData["taxes"] = tax.Select(p => new
            {
                Name = p.TaxName,
                GLTaxRateID = p.GLTaxRateID
            });


            var productorService = _db.ARSetupProductServiceAccounts.ToList();

            ViewData["productorServices"] = productorService
                .Select(e => new ARSetupProductServiceAccount
                {
                    Product = e.Product
                })
                .OrderBy(e => e.Product);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView)]
        public ActionResult ARInvoices_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
        {
            IQueryable<ArInvoice> arinvoices = _db.ARInvoices.Where(x=>x.CreatedBy== "System");//.Where(x=>x.IsManual);
            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetArInvoiceManualTranferableList();
                arinvoices =
                    arinvoices.Where(x => transfferableList.Contains(x.ArInvoiceId));
            }
            DataSourceResult result = arinvoices.ToDataSourceResult(request, aRInvoice => new
            {
                ArInvoiceId = aRInvoice.ArInvoiceId,
                CustomerName = aRInvoice.CustomerName,
                CustomerId = aRInvoice.CustomerId,
                TransactionClass = aRInvoice.TransactionClass,
                Date = aRInvoice.Date,
                PaymentTerm = aRInvoice.PaymentTerm,
                DueDate = aRInvoice.DueDate,
                LineAmount = Math.Round(aRInvoice.LineAmount,2),
                TaxAmount =  Math.Round(aRInvoice.TaxAmount, 2),
                TotalAmount = Math.Round(aRInvoice.TotalAmount, 2),
                AmountDue =  Math.Round(aRInvoice.AmountDue, 2),
                InvoiceStatus = aRInvoice.InvoiceStatus,
                IsManual = aRInvoice.IsManual,
                Reference = aRInvoice.Reference,
                BranchId = aRInvoice.BranchId,
                ArSalesOrderId = aRInvoice.ArSalesOrderId,
                ArSalesOrderPartialPrintId = aRInvoice.ArSalesOrderPartialPrintId,
                SourceInvoiceId = aRInvoice.SourceInvoiceId,
                IsSendForApproval = aRInvoice.IsSendForApproval,
                IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                Status = aRInvoice.Status,
                IsOnlineApproved = aRInvoice.IsOnlineApproved,
                OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                IsVoid = aRInvoice.IsVoid,
                VoidBy = aRInvoice.VoidBy,
                VoidTime = aRInvoice.VoidTime,
                OrgBranchName = aRInvoice.OrgBranchName,
                DateCreated = aRInvoice.DateCreated,
                LastModified = aRInvoice.LastModified,
                CreatedBy = aRInvoice.CreatedBy,
                ModifiedBy = aRInvoice.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoices_Create([DataSourceRequest]DataSourceRequest request, ArInvoice aRInvoice)
        {
            #region validation
            string customername = "";

            if (String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                ModelState.AddModelError("exisit", "Customer ID can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                customername = _db.ARCustomerProfiles.Where(c => c.CustomerId == aRInvoice.CustomerId)
                    .Select(x => x.CompanyName).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                ModelState.AddModelError("exisit", "Payment Term can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                aRInvoice.DueDate = aRInvoice.Date.AddDays(int.Parse(aRInvoice.PaymentTerm));

            if (!String.IsNullOrEmpty(aRInvoice.Reference))
            {
                var exists = _db.ARInvoices
                    .Where(x => x.Reference == aRInvoice.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", $"{aRInvoice.Reference}  already exists in the system.");
            }

            if(aRInvoice.TransactionClass!=TransactionClass.Invoice&& aRInvoice.SourceInvoiceId==0)
                ModelState.AddModelError("sourceinvoice", $"Pease choose invoice for {aRInvoice.TransactionClass}");

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ArInvoice
                {
                    CustomerName = customername,
                    CustomerId = aRInvoice.CustomerId,
                    TransactionClass = aRInvoice.TransactionClass,
                    Date = aRInvoice.Date.Date,
                    PaymentTerm = aRInvoice.PaymentTerm,
                    DueDate = aRInvoice.DueDate.Date,
                    LineAmount = aRInvoice.LineAmount,
                    TaxAmount = aRInvoice.TaxAmount,
                    TotalAmount = aRInvoice.TotalAmount,
                    AmountDue = aRInvoice.AmountDue,
                    InvoiceStatus = aRInvoice.InvoiceStatus,
                    IsManual = true,
                    Reference = aRInvoice.Reference,
                    BranchId = aRInvoice.BranchId,
                    ArSalesOrderId = aRInvoice.ArSalesOrderId,
                    SourceInvoiceId = aRInvoice.SourceInvoiceId,
                    IsSendForApproval = aRInvoice.IsSendForApproval,
                    IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                    IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                    OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    IsOnlineApproved = aRInvoice.IsOnlineApproved,
                    OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                    OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                    IsVoid = aRInvoice.IsVoid,
                    VoidBy = aRInvoice.VoidBy,
                    VoidTime = aRInvoice.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = aRInvoice.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRInvoice.ModifiedBy
                };

                _db.ARInvoices.Add(entity);
                _db.SaveChanges();
                aRInvoice.ArInvoiceId = entity.ArInvoiceId;
            }

            return Json(new[] { aRInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoices_Update([DataSourceRequest]DataSourceRequest request, ArInvoice aRInvoice)
        {

            #region validated record

            string customername = "";

            if (String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                ModelState.AddModelError("exisit", "Customer ID can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                customername = _db.ARCustomerProfiles.Where(c => c.CustomerId == aRInvoice.CustomerId)
                    .Select(x => x.CompanyName).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                ModelState.AddModelError("exisit", "Payment Term can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                aRInvoice.DueDate = aRInvoice.Date.AddDays(int.Parse(aRInvoice.PaymentTerm));

            if (!String.IsNullOrEmpty(aRInvoice.Reference))
            {
                //var exists = _db.ARInvoices
                //    .Where(x => x.ArInvoiceId != aRInvoice.ArInvoiceId)
                //    .Where(x => x.Reference == aRInvoice.Reference && x.IsVoid == false).ToList();

                //if (exists.Any())
                //    ModelState.AddModelError("Reference", $"{aRInvoice.Reference}  already exists in the system.");
            }

            //var parent = _db.ARInvoices.Find(aRInvoice.ArInvoiceId);
            //if (parent != null)
            //{
            //    if (parent.IsOnlineApproved)
            //    {
            //        ModelState.AddModelError("approved", "You cannot manipulate this record");
            //    }

            //    #region detach local version
            //    var local = _db.Set<ArInvoice>()
            //        .Local
            //        .FirstOrDefault(f => f.ArInvoiceId == parent.ArInvoiceId);
            //    if (local != null)
            //    {
            //        _db.Entry(local).State = EntityState.Detached;
            //    }
            //    #endregion

            //}


            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new ArInvoice
                    {
                        ArInvoiceId = aRInvoice.ArInvoiceId,
                        CustomerName = customername,
                        CustomerId = aRInvoice.CustomerId,
                        TransactionClass = aRInvoice.TransactionClass,
                        Date = aRInvoice.Date,
                        PaymentTerm = aRInvoice.PaymentTerm,
                        DueDate = aRInvoice.DueDate,
                        LineAmount = aRInvoice.LineAmount,
                        TaxAmount = aRInvoice.TaxAmount,
                        TotalAmount = aRInvoice.TotalAmount,
                        AmountDue = aRInvoice.AmountDue,
                        InvoiceStatus = aRInvoice.InvoiceStatus,
                        IsManual = aRInvoice.IsManual,
                        Reference = aRInvoice.Reference,
                        BranchId = aRInvoice.BranchId,
                        BusinessUnitId = aRInvoice.BusinessUnitId,
                        ArSalesOrderId = aRInvoice.ArSalesOrderId,
                        ArSalesOrderPartialPrintId = aRInvoice.ArSalesOrderPartialPrintId,
                        SourceInvoiceId = aRInvoice.SourceInvoiceId,
                        IsSendForApproval = aRInvoice.IsSendForApproval,
                        IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                        Status = aRInvoice.Status,
                        IsOnlineApproved = aRInvoice.IsOnlineApproved,
                        OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                        OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                        IsVoid = aRInvoice.IsVoid,
                        VoidBy = aRInvoice.VoidBy,
                        VoidTime = aRInvoice.VoidTime,
                        OrgBranchName = aRInvoice.OrgBranchName,
                        DateCreated = aRInvoice.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRInvoice.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARInvoices.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                
            }

            return Json(new[] { aRInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoices_Destroy([DataSourceRequest]DataSourceRequest request, ArInvoice aRInvoice)
        {
            if (ModelState.IsValid)
            {
               

                var entity = new ArInvoice
                    {
                        ArInvoiceId = aRInvoice.ArInvoiceId,
                        CustomerName = aRInvoice.CustomerName,
                        CustomerId = aRInvoice.CustomerId,
                        TransactionClass = aRInvoice.TransactionClass,
                        Date = aRInvoice.Date,
                        PaymentTerm = aRInvoice.PaymentTerm,
                        DueDate = aRInvoice.DueDate,
                        LineAmount = aRInvoice.LineAmount,
                        TaxAmount = aRInvoice.TaxAmount,
                        TotalAmount = aRInvoice.TotalAmount,
                        AmountDue = aRInvoice.AmountDue,
                        InvoiceStatus = aRInvoice.InvoiceStatus,
                        BranchId = aRInvoice.BranchId,
                        IsSendForApproval = aRInvoice.IsSendForApproval,
                        IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                        Status = aRInvoice.Status,
                        IsManual = aRInvoice.IsManual,
                        ArSalesOrderId = aRInvoice.ArSalesOrderId,
                        IsOnlineApproved = aRInvoice.IsOnlineApproved,
                        OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                        OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                        IsVoid = aRInvoice.IsVoid,
                        VoidBy = aRInvoice.VoidBy,
                        VoidTime = aRInvoice.VoidTime,
                        OrgBranchName = aRInvoice.OrgBranchName,
                        DateCreated = aRInvoice.DateCreated,
                        LastModified = aRInvoice.LastModified,
                        CreatedBy = aRInvoice.CreatedBy,
                        ModifiedBy = aRInvoice.ModifiedBy
                    };

                    _db.ARInvoices.Attach(entity);
                    _db.ARInvoices.Remove(entity);
                    _db.SaveChanges();
                
            }

            return Json(new[] { aRInvoice }.ToDataSourceResult(request, ModelState));
        }

       
      

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
