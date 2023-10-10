using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
   
    public class ApMiscPaymentValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult Index()  
        {
            return View();
        }

         [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult APMiscellaneousPaymentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApMiscellaneousPaymentValidation> apmiscellaneouspaymentvalidations = _db.APMiscellaneousPaymentValidations.Where(x=>x.ApMiscellaneousPaymentId==id);
            DataSourceResult result = apmiscellaneouspaymentvalidations.ToDataSourceResult(request, aPMiscellaneousPaymentValidation => new {
                ApMiscellaneousPaymentValidationId = aPMiscellaneousPaymentValidation.ApMiscellaneousPaymentValidationId,
                ApMiscellaneousPaymentId =id,
                Status = aPMiscellaneousPaymentValidation.Status,
                Remark = aPMiscellaneousPaymentValidation.Remark,
                OrgBranchName = aPMiscellaneousPaymentValidation.OrgBranchName,
                DateCreated = aPMiscellaneousPaymentValidation.DateCreated,
                LastModified = aPMiscellaneousPaymentValidation.LastModified,
                CreatedBy = aPMiscellaneousPaymentValidation.CreatedBy,
                ModifiedBy = aPMiscellaneousPaymentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPaymentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApMiscellaneousPaymentValidation aPMiscellaneousPaymentValidation)
        {
            #region validated record
            var record = _db.APMiscellaneousPayments.Find(id);

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");
            var lines = _db.ApMiscellaneousPaymentLines.Where(x => x.ApMiscellaneousPaymentId == id);

            if(!lines.Any())
                ModelState.AddModelError("lines", "Please add lines first");
            #endregion
            if (ModelState.IsValid)
            {
                if (record != null && !record.IsOnlineApproved)
                {
                    if (aPMiscellaneousPaymentValidation.Status == ApprovalStatuses.Approve)
                    {
                        
                        #region pass data for reconciliation

                        //foreach (var distribution in record.ApMiscellaneousPaymentDistribution)
                        //{
                        //    string sourceid = record.ApMiscellaneousPaymentId + "-" + distribution.ApMiscellaneousPaymentDistributionId;
                        //    //check if this is bank account
                        //    if (_accountHelper.IsBankAccount(distribution.AccountCode))
                        //    {
                        //        var book = new ReconciliationBookAccount
                        //        {
                        //            CreatedBy = User.Identity.Name,
                        //            DateCreated = DateTime.Now,
                        //            AccountCode = distribution.AccountCode,
                        //            Debit = distribution.Debit,
                        //            Credit = distribution.Credit,
                        //            Date =record.PaymentDate,
                        //            Type = "Misc Payment",
                        //            ReferenceDoc = !String.IsNullOrWhiteSpace(record.Remark) ? record.Remark : "NA",
                        //            Reference = !String.IsNullOrWhiteSpace(record.PaymentReference) ? record.PaymentReference : "NA",
                        //            Source = JournalSources.AP_MiscPayment.ToString(),
                        //            SourceID = sourceid,
                        //            AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                        //            SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                        //        };
                        //        _db.ReconcilationBookAccounts.Add(book);
                        //        _db.SaveChanges();
                        //    }
                        //}

                        #endregion
                        //PostToSecondaryLedger(id);
                     
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        //_db.SaveChanges();
                    }

                    if (aPMiscellaneousPaymentValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        //_db.SaveChanges();
                    }

                   


                    var entity = new ApMiscellaneousPaymentValidation
                    {
                        ApMiscellaneousPaymentId = id,
                        Status = aPMiscellaneousPaymentValidation.Status,
                        Remark = aPMiscellaneousPaymentValidation.Remark,
                        OrgBranchName = aPMiscellaneousPaymentValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPMiscellaneousPaymentValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPMiscellaneousPaymentValidation.ModifiedBy
                    };

                    _db.APMiscellaneousPaymentValidations.Add(entity);
                    //_db.SaveChanges();
                    aPMiscellaneousPaymentValidation.ApMiscellaneousPaymentValidationId =
                        entity.ApMiscellaneousPaymentValidationId;

                    if(FinanceConfigurationHelper.IsTelebirrPaymentMethodEnabled() && record.PaymentMethod.ToUpper()== KnownPaymentMethods.TELEBIRR.ToString())
                    {
                        #region create tb payment
                        var supplier = _db.ProcurementSuppliers.Where(x => x.SupplierId == record.SupplierId).FirstOrDefault();
                        if (supplier != null)
                        {
                            var tbirr = new ApTPayment
                            {
                                PaymentNum = record.ApMiscellaneousPaymentId,
                                SupplierName = record.SupplierName,
                                SupplierId = record.SupplierId,
                                PaymentType = PaymentSource.MiscPayment,
                                PaymentDate = record.PaymentDate,
                                TCode = supplier.TCode,
                                TAccount = supplier.TAccount,
                                Amount = record.TotalAmount,
                                PaymentReference = $"Misc.{record.ApMiscellaneousPaymentId}",
                                DateCreated = DateTime.Now,
                                CreatedBy = "System",
                            };

                            _db.ApTPayments.Add(tbirr);
                        }
                        #endregion
                    }

                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment approval", "Success", User.Identity.Name, $"approved misc payment {entity.ApMiscellaneousPaymentId} ", Modules.Finance);
                    #endregion
                }

            }

            return Json(new[] { aPMiscellaneousPaymentValidation }.ToDataSourceResult(request, ModelState));
        }



        

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
