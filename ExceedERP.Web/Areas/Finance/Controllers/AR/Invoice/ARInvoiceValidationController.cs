using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Invoice
{
    public class ArInvoiceValidationController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
        public ActionResult ARInvoiceValidations_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArInvoiceValidation> arinvoicevalidations = _db.ARInvoiceValidations.Where(x => x.ArInvoiceId == id);
            DataSourceResult result = arinvoicevalidations.ToDataSourceResult(request, aRInvoiceValidation => new
            {
                ArInvoiceValidationId = aRInvoiceValidation.ArInvoiceValidationId,
                ArInvoiceId = id,
                Status = aRInvoiceValidation.Status,
                Remark = aRInvoiceValidation.Remark,
                OrgBranchName = aRInvoiceValidation.OrgBranchName,
                DateCreated = aRInvoiceValidation.DateCreated,
                LastModified = aRInvoiceValidation.LastModified,
                CreatedBy = aRInvoiceValidation.CreatedBy,
                ModifiedBy = aRInvoiceValidation.ModifiedBy
            });

            return Json(result);
        }

         [AuthorizeRoles(FinanceConstants.AccountReceivableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceValidations_Create(int id, [DataSourceRequest]DataSourceRequest request, ArInvoiceValidation aRInvoiceValidation)
        {

            #region validated record
           
            var record = _db.ARInvoices.Find(id);

            if (record ==null)
                ModelState.AddModelError("null", "Invoice Record not found.");

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approve", "You cannot manipulate this record,it is already approved");

            if (record != null && record.TransactionClass != TransactionClass.Invoice && record.SourceInvoiceId==null)
                ModelState.AddModelError("previousinvoice", "Credit and Debit Memo Invoice should refer the original invoice number.");
            #endregion
            if (ModelState.IsValid)
            {
                
                #region ger record record status

                if (record != null)
                {
                    if (aRInvoiceValidation.Status == ApprovalStatuses.Approve)
                    {
                       // PostToSecondaryLedger(id);
                        record.IsPostedToSecondLedger = true;

                        if(record.TransactionClass==TransactionClass.CreditMemo)
                            UpdateRelatedInvoiceTotals(record.ArInvoiceId);

                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (aRInvoiceValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    #endregion

                    var entity = new ArInvoiceValidation
                    {
                        ArInvoiceId = id,
                        Status = aRInvoiceValidation.Status,
                        Remark = aRInvoiceValidation.Remark,
                        OrgBranchName = aRInvoiceValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aRInvoiceValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aRInvoiceValidation.ModifiedBy
                    };

                    _db.ARInvoiceValidations.Add(entity);
                    _db.SaveChanges();
                    aRInvoiceValidation.ArInvoiceValidationId = entity.ArInvoiceValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AR invoice approval", "Success", User.Identity.Name, $"approved invoice {entity.ArInvoiceId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aRInvoiceValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles( FinanceConstants.AccountReceivableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceValidations_Update(int id, [DataSourceRequest]DataSourceRequest request, ArInvoiceValidation aRInvoiceValidation)
        {
            aRInvoiceValidation.ArInvoiceId = id;
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record
                ArInvoice record = _db.ARInvoices.Find(id);

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }
                #endregion
                #region ger record record status

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (aRInvoiceValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (aRInvoiceValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {
                    var entity = new ArInvoiceValidation
                    {
                        ArInvoiceId = id,
                        ArInvoiceValidationId = aRInvoiceValidation.ArInvoiceValidationId,
                        Status = aRInvoiceValidation.Status,
                        Remark = aRInvoiceValidation.Remark,
                        OrgBranchName = aRInvoiceValidation.OrgBranchName,
                        DateCreated = aRInvoiceValidation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRInvoiceValidation.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARInvoiceValidations.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { aRInvoiceValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles( FinanceConstants.AccountReceivableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceValidations_Destroy([DataSourceRequest]DataSourceRequest request, ArInvoiceValidation aRInvoiceValidation)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record
                ArInvoice record = _db.ARInvoices.Find(aRInvoiceValidation.ArInvoiceId);

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }
                #endregion
                #region ger record record status

                if (record != null && record.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record,it is already approved";
                    //return View("Error");
                }
                else if (record != null && !record.IsOnlineApproved)
                {
                    //validate 
                    if (aRInvoiceValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                    }
                    if (aRInvoiceValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                #endregion

                if (isvalid)
                {

                    var entity = new ArInvoiceValidation
                    {
                        ArInvoiceValidationId = aRInvoiceValidation.ArInvoiceValidationId,
                        Status = aRInvoiceValidation.Status,
                        Remark = aRInvoiceValidation.Remark,
                        OrgBranchName = aRInvoiceValidation.OrgBranchName,
                        DateCreated = aRInvoiceValidation.DateCreated,
                        LastModified = aRInvoiceValidation.LastModified,
                        CreatedBy = aRInvoiceValidation.CreatedBy,
                        ModifiedBy = aRInvoiceValidation.ModifiedBy
                    };

                    _db.ARInvoiceValidations.Attach(entity);
                    _db.ARInvoiceValidations.Remove(entity);
                    _db.SaveChanges();
                }
            }

            return Json(new[] { aRInvoiceValidation }.ToDataSourceResult(request, ModelState));
        }
        
        private void PostToSecondaryLedger(int id)
        {
            SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
            string user = User.Identity.Name;
            var record = _db.ARInvoices.Find(id);
            if (record != null)
            {
                #region header

                var ledger = new PostJournalVm
                {
                    PostJournalVmId = id,
                    JournalName = "",
                    JournalNumber = "",
                    Description = "",
                    CategoryName = "",
                    SourcesName = "AR Invoice",
                    EffectiveDate = record.Date,
                    GlPeriodId = _fiscalYearHelper.GetPeriodOfDate(record.Date),
                    GlFiscalYearId = _fiscalYearHelper.GetFiscalYearOfDate(record.Date),
                    PostingSource = JournalSources.AR_Invoice,
                    PostJournalLineVms = new List<PostJournalVm.PostJournalLineVm>(),
                };

                #endregion
                #region distribution

                var distributions = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == id).ToList();

                foreach (var distribution in distributions)
                {
                    var ledgerline = new PostJournalVm.PostJournalLineVm
                    {
                        PostJournalLineVmId = distribution.ArInvoiceDistributionId,
                        AccountCode = distribution.AccountCode,
                        Debit = distribution.Debit,
                        Credit = distribution.Credit
                    };
                    ledger.PostJournalLineVms.Add(ledgerline);
                }

                #endregion

                secondLedgerHelper.PostToSecondLedger(ledger,user);
            }
        }

        private void UpdateRelatedInvoiceTotals(int id)
        {
            var aRInvoice = _db.ARInvoices.Find(id);

            if (aRInvoice != null)
            {
                var lines = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == id).ToList();

                if (lines.Any())
                {
                    decimal lineamount = Math.Round(lines.Sum(x => x.SubTotal), 2);
                    decimal taxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                    decimal totalAmount = Math.Round(lines.Sum(x => x.TotalAmount), 2);

                    if (aRInvoice.TransactionClass == TransactionClass.CreditMemo)
                    {
                        if (aRInvoice.SourceInvoiceId != null)
                        {
                            #region update related invoice
                            var orignalInvoice = _db.ARInvoices.Find(aRInvoice.SourceInvoiceId);
                            if (orignalInvoice != null)
                            {
                                orignalInvoice.InvoiceStatus = InvoiceStatus.Open;
                                orignalInvoice.LineAmount = orignalInvoice.LineAmount - lineamount;
                                orignalInvoice.TaxAmount = orignalInvoice.TaxAmount - taxAmount;
                                orignalInvoice.TotalAmount = orignalInvoice.TotalAmount - totalAmount;
                                orignalInvoice.AmountDue = orignalInvoice.AmountDue - totalAmount;
                                _db.Entry(orignalInvoice).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                            #endregion
                        }
                    }
                    //else if (aRInvoice.TransactionClass == TransactionClass.DebitMemo)
                    //{
                    //    if (aRInvoice.SourceInvoiceId != null)
                    //    {
                    //        #region update related invoice
                    //        var orignalInvoice = _db.ARInvoices.Find(aRInvoice.SourceInvoiceId);
                    //        if (orignalInvoice != null)
                    //        {
                    //            orignalInvoice.InvoiceStatus = InvoiceStatus.Open;
                    //            orignalInvoice.LineAmount = orignalInvoice.LineAmount + lineamount;
                    //            orignalInvoice.TaxAmount = orignalInvoice.TaxAmount + taxAmount;
                    //            orignalInvoice.TotalAmount = orignalInvoice.TotalAmount + totalAmount;
                    //            orignalInvoice.AmountDue = orignalInvoice.AmountDue + totalAmount;
                    //            _db.Entry(orignalInvoice).State = EntityState.Modified;
                    //            _db.SaveChanges();
                    //        }

                    //        #endregion
                    //    }
                    //}
                }

            }


        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
