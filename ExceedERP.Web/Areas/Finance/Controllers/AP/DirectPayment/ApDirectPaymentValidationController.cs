using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{

    public class ApDirectPaymentValidationController : Controller
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
        public ActionResult APDirectPaymentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApDirectPaymentValidation> apDirectpaymentvalidations = _db.ApDirectPaymentValidations.Where(x=>x.ApDirectPaymentId==id);
            DataSourceResult result = apDirectpaymentvalidations.ToDataSourceResult(request, aPDirectPaymentValidation => new {
                ApDirectPaymentValidationId = aPDirectPaymentValidation.ApDirectPaymentValidationId,
                ApDirectPaymentId =id,
                Status = aPDirectPaymentValidation.Status,
                Remark = aPDirectPaymentValidation.Remark,
                OrgBranchName = aPDirectPaymentValidation.OrgBranchName,
                DateCreated = aPDirectPaymentValidation.DateCreated,
                LastModified = aPDirectPaymentValidation.LastModified,
                CreatedBy = aPDirectPaymentValidation.CreatedBy,
                ModifiedBy = aPDirectPaymentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APDirectPaymentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApDirectPaymentValidation aPDirectPaymentValidation)
        {
            #region validated record
            var record = _db.ApDirectPayments.Find(id);

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            var lines = _db.ApDirectPaymentLines.Where(x => x.ApDirectPaymentId == id);
            if (!lines.Any())
                ModelState.AddModelError("line", "Please add lines first");

            decimal currentPayment = 0;
            decimal previousPayment = 0;
            decimal totalPayment = 0;

            List<int> previousPaymentIds = _db.ApDirectPayments
                .Where(x => x.SourceId == record.SourceId)
                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Where(x => x.ApDirectPaymentId != record.ApDirectPaymentId).Select(x => x.ApDirectPaymentId).ToList();

            var previLines = _db.ApDirectPaymentLines.Where(x => previousPaymentIds.Contains(x.ApDirectPaymentId)).ToList();

            if (lines.Any())
                currentPayment = lines.Sum(l => l.Amount);

            if (previLines.Any())
                previousPayment = previLines.Sum(l => l.Amount);

            totalPayment = currentPayment + previousPayment;

            #region validate amount
            int did = int.Parse(record.SourceId.Split('_')[1]);
            string sourceType = record.SourceId.Split('_')[0];
            if (aPDirectPaymentValidation.Status == ApprovalStatuses.Approve)
            {               
                switch (sourceType)
                {
                    case "LC":
                        if (previousPaymentIds.Any())
                            ModelState.AddModelError("exisit", "This LC payment is done before.");
                        break;
                    case "PAYMENT-REQUEST":
                        var paymentRequest = _db.PaymentRequests.Find(did);
                        if (paymentRequest != null)
                        {
                            decimal totalAllowed = 0;
                            var fundLines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == did);

                            if (fundLines.Any())
                                totalAllowed = fundLines.Sum(x => x.Amount);

                            if (totalPayment > totalAllowed)
                            {
                                ModelState.AddModelError("over", $"Total payment including previos transactions can't be more than approved payment request.\n Approved Transfer: {totalAllowed}, Current Transfer : {currentPayment}, Previous transfer, {previousPayment}");
                            }


                        }

                        break;
                    case "TRAVEL-ORDER":
                        if (previousPaymentIds.Any())
                            ModelState.AddModelError("exisit", "This Travel Order payment is done before.");
                        break;
                    case "EXPENSE-CLAIM":
                        if (previousPaymentIds.Any())
                            ModelState.AddModelError("exisit", "This Expense Claim payment is done before.");

                        break;
                    case "FUND-TRANSFER":

                        var fundTransfer = _db.FundTransfers.Find(did);
                        if (fundTransfer != null)
                        {
                            decimal totalAllowed = 0;
                            var fundLines = _db.FundTransferDetails.Where(x => x.FundTransferId == did);

                            if (fundLines.Any())
                                totalAllowed = fundLines.Sum(x => x.Approved);

                            if (totalPayment > totalAllowed)
                            {
                                ModelState.AddModelError("over", $"Total fund trasfer including previos transactions can't be more than approved transfer request.\n Approved Transfer: {totalAllowed}, Current Transfer : {currentPayment}, Previous transfer, {previousPayment}");
                            }


                        }

                        break;

                    case "SALES-RETURN":

                        if (previousPaymentIds.Any())
                            ModelState.AddModelError("exisit", "This Sales Return payment is done before.");
                        break;
                }

            }
            #endregion

            #endregion
            if (ModelState.IsValid)
            {
                if (aPDirectPaymentValidation.Status == ApprovalStatuses.Approve)
                {
                    record.Status = "Approved";
                    record.IsOnlineApproved = true;
                    record.IsVoid = false;
                    record.OnlineApprovedBy = User.Identity.Name;
                    record.OnlineApprovedTime = DateTime.Now;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    if(sourceType== "FUND-TRANSFER")
                    {
                        #region create fund receive 
                        var fundtransfer = _db.FundTransfers.Find(did);
                        var fundReceive = new FundReceive
                        {
                            FundTransferId=fundtransfer.FundTransferId,
                            BranchId = fundtransfer.BranchId,
                            BusinessUnitId = fundtransfer.BusinessUnitId,
                            FromBranchId = fundtransfer.FromBranchId,
                            FromBusinessUnitId = fundtransfer.FromBusinessUnitId,
                            CreatedAtBranchId = fundtransfer.CreatedAtBranchId,
                            CreatedAtBusinessUnitId = fundtransfer.CreatedAtBusinessUnitId,
                            CreatedAtOrgStructureId = fundtransfer.CreatedAtOrgStructureId,
                            BankAccountId = fundtransfer.BankAccountId,
                            AmountReceived = currentPayment,
                            ReceiveDate = DateTime.Today,
                            Remark = fundtransfer.Remark,                            


                            ForwardedFrom = User.Identity.Name,
                            ForwardedTo = User.Identity.Name,
                            IsFinalApproval = fundtransfer.IsFinalApproval,
                            CurrentApprovalStep = fundtransfer.CurrentApprovalStep,

                            IsDepartmentOnlineApproved = fundtransfer.IsDepartmentOnlineApproved,
                            DepartmentOnlineApprovedBy = fundtransfer.DepartmentOnlineApprovedBy,
                            DepartmentOnlineApprovedTime = fundtransfer.DepartmentOnlineApprovedTime,
                            IsDepartmentSendForApproval = fundtransfer.IsDepartmentSendForApproval,
                            DepartmentSendForApprovalBy = fundtransfer.DepartmentSendForApprovalBy,
                            DepartmentSendForApprovalTime = fundtransfer.DepartmentSendForApprovalTime,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name

                        };

                        _db.FundReceives.Add(fundReceive);
                        _db.SaveChanges();

                        foreach(var d in record.ApDirectPaymentDistribution)
                        {
                            var dist = new FundReceiveDistribution
                            {
                                FundReceiveId = fundReceive.FundReceiveId,
                                AccountCode = d.AccountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(d.AccountCode),
                                AdditionalDescription = d.AdditionalDescription,
                                Debit = d.Debit,
                                Credit = d.Credit,
                                OrgBranchName = d.OrgBranchName,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name
                            };

                            _db.FundReceiveDistributions.Add(dist);
                            _db.SaveChanges();
                        }
                        #endregion
                    }


                }

                if (aPDirectPaymentValidation.Status == ApprovalStatuses.Reject)
                {
                    record.IsSendForApproval = false;
                    record.Status = "Rejected";
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                var entity = new ApDirectPaymentValidation
                {
                    ApDirectPaymentId = id,
                    Status = aPDirectPaymentValidation.Status,
                    Remark = aPDirectPaymentValidation.Remark,
                    OrgBranchName = aPDirectPaymentValidation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = aPDirectPaymentValidation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aPDirectPaymentValidation.ModifiedBy
                };

                _db.ApDirectPaymentValidations.Add(entity);
                _db.SaveChanges();
                aPDirectPaymentValidation.ApDirectPaymentValidationId =
                    entity.ApDirectPaymentValidationId;

                if (aPDirectPaymentValidation.Status == ApprovalStatuses.Approve)
                {
                    #region update payment requests
                    switch (sourceType)
                    {
                        case "LC":
                            var lc = _db.ForeignPurchaseCosts.Find(did);
                            if (lc != null)
                            {
                                lc.Paid = true;
                                _db.Entry(lc).State = EntityState.Modified;
                            }
                            _db.SaveChanges();
                            break;
                        case "PAYMENT-REQUEST":
                            var paymentRequest = _db.PaymentRequests.Find(did);
                            var paymentRequestLines = _db.ApDirectPaymentLines.Where(x => x.ApDirectPaymentId == id).ToList();
                            if (paymentRequest != null)
                            {
                                decimal paymentAmount = paymentRequestLines.Any() ? paymentRequestLines.Sum(x => x.Amount) : 0;
                                decimal invoiceamountdue = paymentRequest.AmountDue - paymentAmount;
                                paymentRequest.AmountDue = invoiceamountdue >= 0 ? invoiceamountdue : 0;
                                if (invoiceamountdue <= 0)
                                {
                                    paymentRequest.Paid = true;
                                    paymentRequest.Status = "Paid";
                                }
                                else
                                {
                                    ;
                                }

                                _db.Entry(paymentRequest).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                            break;
                        case "TRAVEL-ORDER":

                            var travelOrder = _db.ApTravelOrders.Find(did);
                            if (travelOrder != null)
                            {
                                travelOrder.Paid = true;
                                _db.Entry(travelOrder).State = EntityState.Modified;
                            }
                            _db.SaveChanges();
                            break;
                        case "EXPENSE-CLAIM":

                            var expenseClaim = _db.ApTravelExpenseClaims.Find(did);
                            if (expenseClaim != null)
                            {
                                expenseClaim.Paid = true;
                                _db.Entry(expenseClaim).State = EntityState.Modified;
                            }
                            _db.SaveChanges();
                            break;
                        case "FUND-TRANSFER":

                            var fundTransfer = _db.FundTransfers.Find(did);
                            if (fundTransfer != null)
                            {
                                decimal totalAllowed = 0;
                                var fundLines = _db.FundTransferDetails.Where(x => x.FundTransferId == did);

                                if (fundLines.Any())
                                    totalAllowed = fundLines.Sum(x => x.Approved);

                                if (totalPayment == totalAllowed)
                                {
                                    fundTransfer.Paid = true;
                                }

                                _db.Entry(fundTransfer).State = EntityState.Modified;
                            }
                            _db.SaveChanges();
                            break;

                        case "SALES-RETURN":

                            var refund = _db.SalesReturn.Find(did);
                            if (refund != null)
                            {
                                refund.Paid = true;
                                _db.Entry(refund).State = EntityState.Modified;
                            }
                            _db.SaveChanges();
                            break;
                    }


                    #endregion
                }

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP direct payment approval", "Success", User.Identity.Name, $"approved direct payment {entity.ApDirectPaymentId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { aPDirectPaymentValidation }.ToDataSourceResult(request, ModelState));
        }



      
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
