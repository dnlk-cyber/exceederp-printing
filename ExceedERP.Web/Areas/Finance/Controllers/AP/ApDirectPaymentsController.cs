using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Drawing;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApDirectPaymentsController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();
       

        [AuthorizeRoles(FinanceConstants.AccountPayableView ,FinanceConstants.AccountPayableApprover ,FinanceConstants.AccountPayablePayment)]
        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }

        private void PopulateViewData()
        {
            var paymentMethod = _db.PaymentMethods.ToList();

            ViewData["methods"] = paymentMethod;

            var tax = _db.GLTaxRates.Where(x => x.ApplyToAp).ToList();

            ViewData["taxes"] = tax;

            var witholdings = _db.GLTaxRates.Where(x => x.ApplyToWithholding).Where(x => x.ApplyToAp).ToList();

            ViewData["witholdingType"] = witholdings;
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView ,FinanceConstants.AccountPayableApprover ,FinanceConstants.AccountPayablePayment)]
        public ActionResult APDirectPayments_Read([DataSourceRequest]DataSourceRequest request,bool isNotTransffered)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);

            IQueryable<ApDirectPayment> apDirectpayments = _db.ApDirectPayments.Where(x => mybraches.Contains(x.OrgBranchName));
            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetApDirectPaymentTranferableList();
                apDirectpayments =
                    apDirectpayments.Where(x => transfferableList.Contains(x.ApDirectPaymentId));
            }
            DataSourceResult result = apDirectpayments.ToDataSourceResult(request, aPDirectPayments => new {
                ApDirectPaymentId = aPDirectPayments.ApDirectPaymentId,
                IsDirectPayment = aPDirectPayments.IsDirectPayment,
                SourceId = aPDirectPayments.SourceId,
                SupplierId = aPDirectPayments.SupplierId,
                IsNewSupplier = aPDirectPayments.IsNewSupplier,
                SupplierName = aPDirectPayments.SupplierName,
                Purchaser = aPDirectPayments.Purchaser,
                Grn = aPDirectPayments.Grn,
                Po = aPDirectPayments.Po,
                PaymentType = aPDirectPayments.PaymentType,
                PaymentMethod = aPDirectPayments.PaymentMethod,
                PaymentReference = aPDirectPayments.PaymentReference,
                PaymentDate = aPDirectPayments.PaymentDate,
                IsPostedToSecondLedger = aPDirectPayments.IsPostedToSecondLedger,
                Remark = aPDirectPayments.Remark,
                IsSendForApproval = aPDirectPayments.IsSendForApproval,
                IsOnlineTransferred = aPDirectPayments.IsOnlineTransferred,
                IsOnlineTransferredBy = aPDirectPayments.IsOnlineTransferredBy,
                OnlineTransferredTime = aPDirectPayments.OnlineTransferredTime,
                Status = aPDirectPayments.Status,
                IsOnlineApproved = aPDirectPayments.IsOnlineApproved,
                OnlineApprovedBy = aPDirectPayments.OnlineApprovedBy,
                OnlineApprovedTime = aPDirectPayments.OnlineApprovedTime,
                IsVoid = aPDirectPayments.IsVoid,
                VoidBy = aPDirectPayments.VoidBy,
                VoidTime = aPDirectPayments.VoidTime,
                OrgBranchName = aPDirectPayments.OrgBranchName,
                DateCreated = aPDirectPayments.DateCreated,
                LastModified = aPDirectPayments.LastModified,
                CreatedBy = aPDirectPayments.CreatedBy,
                ModifiedBy = aPDirectPayments.ModifiedBy
            });

            return Json(result);
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APDirectPayments_Create([DataSourceRequest]DataSourceRequest request, ApDirectPayment aPDirectPayments)
        {
            #region validation


            if (aPDirectPayments.IsNewSupplier && String.IsNullOrWhiteSpace(aPDirectPayments.SourceId))
                ModelState.AddModelError("source", "Please select payment source.");

            if (!String.IsNullOrEmpty(aPDirectPayments.PaymentReference))
            {
                var exists = _db.ApDirectPayments.Where(x => x.PaymentReference == aPDirectPayments.PaymentReference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Payment Reference", $"{aPDirectPayments.PaymentReference}  already exists in the system.");
            }
            #endregion

            if (!String.IsNullOrWhiteSpace(aPDirectPayments.SourceId))
            {

                #region data

                int did = int.Parse(aPDirectPayments.SourceId.Split('_')[1]);
                string sourceType = aPDirectPayments.SourceId.Split('_')[0];
                switch (sourceType)
                {
                    case "LC":
                        var purchaseCost = _db.ForeignPurchaseCosts.Find(did);
                        if (purchaseCost != null)
                            aPDirectPayments.SupplierName = purchaseCost.Payee;
                        break;
                    case "PAYMENT-REQUEST":
                        var paymentRequest = _db.PaymentRequests.Find(did);
                        if (paymentRequest != null)
                        {
                            aPDirectPayments.SupplierName = paymentRequest.Supplier;
                            aPDirectPayments.Purchaser = paymentRequest.Purchaser??0;
                            aPDirectPayments.Grn = paymentRequest.GRN;
                            aPDirectPayments.Po = paymentRequest.PONo;
                        }
                        break;
                    case "TRAVEL-ORDER":
                        break;
                    case "EXPENSE-CLAIM":
                        break;
                }

                #endregion
                
            }

            if (ModelState.IsValid)
            {
                var entity = new ApDirectPayment
                {
                    SourceId = aPDirectPayments.SourceId,
                    IsDirectPayment = aPDirectPayments.IsDirectPayment,
                    SupplierId = aPDirectPayments.SupplierId,
                    IsNewSupplier = aPDirectPayments.IsNewSupplier,
                    SupplierName = aPDirectPayments.SupplierName,
                    Purchaser = aPDirectPayments.Purchaser,
                    Grn = aPDirectPayments.Grn,
                    Po = aPDirectPayments.Po,
                    PaymentType = aPDirectPayments.PaymentType,
                    PaymentMethod = aPDirectPayments.PaymentMethod,
                    PaymentReference = aPDirectPayments.PaymentReference,
                    PaymentDate = aPDirectPayments.PaymentDate.Date,
                    IsPostedToSecondLedger = aPDirectPayments.IsPostedToSecondLedger,
                    Remark = aPDirectPayments.Remark,
                    IsSendForApproval = aPDirectPayments.IsSendForApproval,
                    IsOnlineTransferred = aPDirectPayments.IsOnlineTransferred,
                    IsOnlineTransferredBy = aPDirectPayments.IsOnlineTransferredBy,
                    OnlineTransferredTime = aPDirectPayments.OnlineTransferredTime,
                    Status = aPDirectPayments.Status,
                    IsOnlineApproved = aPDirectPayments.IsOnlineApproved,
                    OnlineApprovedBy = aPDirectPayments.OnlineApprovedBy,
                    OnlineApprovedTime = aPDirectPayments.OnlineApprovedTime,
                    IsVoid = aPDirectPayments.IsVoid,
                    VoidBy = aPDirectPayments.VoidBy,
                    VoidTime = aPDirectPayments.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = aPDirectPayments.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aPDirectPayments.ModifiedBy
                };

                _db.ApDirectPayments.Add(entity);
                _db.SaveChanges();
                aPDirectPayments.ApDirectPaymentId = entity.ApDirectPaymentId;

                GetPaymentItems(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "direct payment", "Success", User.Identity.Name, $"created direct payment {entity.ApDirectPaymentId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPDirectPayments }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APDirectPayments_Update([DataSourceRequest]DataSourceRequest request, ApDirectPayment aPDirectPayments)
        {
            #region validation


            if (aPDirectPayments.IsNewSupplier && String.IsNullOrWhiteSpace(aPDirectPayments.SourceId))
                ModelState.AddModelError("source", "Please select payment source.");

            if (!String.IsNullOrEmpty(aPDirectPayments.PaymentReference))
            {
                var exists = _db.ApDirectPayments
                    .Where(x=>x.ApDirectPaymentId!=aPDirectPayments.ApDirectPaymentId).Where(x => x.PaymentReference == aPDirectPayments.PaymentReference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Payment Reference", $"{aPDirectPayments.PaymentReference}  already exists in the system.");
            }

            #endregion

            if (!String.IsNullOrWhiteSpace(aPDirectPayments.SourceId))
            {

                #region data

                int did = int.Parse(aPDirectPayments.SourceId.Split('_')[1]);
                string sourceType = aPDirectPayments.SourceId.Split('_')[0];
                switch (sourceType)
                {
                    case "LC":
                        var purchaseCost = _db.ForeignPurchaseCosts.Find(did);
                        if (purchaseCost != null)
                            aPDirectPayments.SupplierName = purchaseCost.Payee;
                        break;
                    case "PAYMENT-REQUEST":
                        var paymentRequest = _db.PaymentRequests.Find(did);
                        if (paymentRequest != null)
                        {
                            aPDirectPayments.SupplierName = paymentRequest.Supplier;
                            aPDirectPayments.Purchaser = paymentRequest.Purchaser ?? 0;
                            aPDirectPayments.Grn = paymentRequest.GRN;
                            aPDirectPayments.Po = paymentRequest.PONo;
                        }
                        break;
                    case "TRAVEL-ORDER":
                        break;
                    case "EXPENSE-CLAIM":
                        break;
                }

                #endregion

            }

            #region validated record

            var parent = _db.ApDirectPayments.Find(aPDirectPayments.ApDirectPaymentId);
            if (parent != null)
            {
                if (parent.IsOnlineApproved)
                    ModelState.AddModelError("approved", "You cannot manipulate this record");

                #region detach local version
                var local = _db.Set<ApDirectPayment>()
                    .Local
                    .FirstOrDefault(f => f.ApDirectPaymentId == parent.ApDirectPaymentId);
                if (local != null)
                {
                    _db.Entry(local).State = EntityState.Detached;
                }
                #endregion

            }


            #endregion


            if (ModelState.IsValid)
            {
                    var entity = new ApDirectPayment
                    {
                        SourceId = aPDirectPayments.SourceId,
                        IsDirectPayment = aPDirectPayments.IsDirectPayment,
                        ApDirectPaymentId = aPDirectPayments.ApDirectPaymentId,
                        SupplierId = aPDirectPayments.SupplierId,
                        IsNewSupplier = aPDirectPayments.IsNewSupplier,
                        SupplierName = aPDirectPayments.SupplierName,
                        Purchaser = aPDirectPayments.Purchaser,
                        Grn = aPDirectPayments.Grn,
                        Po = aPDirectPayments.Po,
                        PaymentType = aPDirectPayments.PaymentType,
                        PaymentMethod = aPDirectPayments.PaymentMethod,
                        PaymentReference = aPDirectPayments.PaymentReference,
                        PaymentDate = aPDirectPayments.PaymentDate,
                        IsPostedToSecondLedger = aPDirectPayments.IsPostedToSecondLedger,
                        Remark = aPDirectPayments.Remark,
                        IsSendForApproval = aPDirectPayments.IsSendForApproval,
                        IsOnlineTransferred = aPDirectPayments.IsOnlineTransferred,
                        IsOnlineTransferredBy = aPDirectPayments.IsOnlineTransferredBy,
                        OnlineTransferredTime = aPDirectPayments.OnlineTransferredTime,
                        Status = aPDirectPayments.Status,
                        IsOnlineApproved = aPDirectPayments.IsOnlineApproved,
                        OnlineApprovedBy = aPDirectPayments.OnlineApprovedBy,
                        OnlineApprovedTime = aPDirectPayments.OnlineApprovedTime,
                        IsVoid = aPDirectPayments.IsVoid,
                        VoidBy = aPDirectPayments.VoidBy,
                        VoidTime = aPDirectPayments.VoidTime,
                        OrgBranchName = aPDirectPayments.OrgBranchName,
                        DateCreated = aPDirectPayments.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aPDirectPayments.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ApDirectPayments.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                GetPaymentItems(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "direct payment", "Success", User.Identity.Name, $"updated direct payment {entity.ApDirectPaymentId} ", Modules.Finance);
                #endregion
            }
            return Json(new[] { aPDirectPayments }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APDirectPayments_Destroy([DataSourceRequest]DataSourceRequest request, ApDirectPayment aPDirectPayments)
        {

            

            if (ModelState.IsValid)
            {
                    var entity = new ApDirectPayment
                    {
                        IsDirectPayment = aPDirectPayments.IsDirectPayment,
                        ApDirectPaymentId = aPDirectPayments.ApDirectPaymentId,
                        SupplierId = aPDirectPayments.SupplierId,
                        IsNewSupplier = aPDirectPayments.IsNewSupplier,
                        SupplierName = aPDirectPayments.SupplierName,
                        Purchaser = aPDirectPayments.Purchaser,
                        Grn = aPDirectPayments.Grn,
                        Po = aPDirectPayments.Po,
                        PaymentType = aPDirectPayments.PaymentType,
                        PaymentMethod = aPDirectPayments.PaymentMethod,
                        PaymentReference = aPDirectPayments.PaymentReference,
                        PaymentDate = aPDirectPayments.PaymentDate,
                        IsPostedToSecondLedger = aPDirectPayments.IsPostedToSecondLedger,
                        Remark = aPDirectPayments.Remark,
                        IsSendForApproval = aPDirectPayments.IsSendForApproval,
                        IsOnlineTransferred = aPDirectPayments.IsOnlineTransferred,
                        IsOnlineTransferredBy = aPDirectPayments.IsOnlineTransferredBy,
                        OnlineTransferredTime = aPDirectPayments.OnlineTransferredTime,
                        Status = aPDirectPayments.Status,
                        IsOnlineApproved = aPDirectPayments.IsOnlineApproved,
                        OnlineApprovedBy = aPDirectPayments.OnlineApprovedBy,
                        OnlineApprovedTime = aPDirectPayments.OnlineApprovedTime,
                        IsVoid = aPDirectPayments.IsVoid,
                        VoidBy = aPDirectPayments.VoidBy,
                        VoidTime = aPDirectPayments.VoidTime,
                        OrgBranchName = aPDirectPayments.OrgBranchName,
                        DateCreated = aPDirectPayments.DateCreated,
                        LastModified = aPDirectPayments.LastModified,
                        CreatedBy = aPDirectPayments.CreatedBy,
                        ModifiedBy = aPDirectPayments.ModifiedBy
                    };

                    _db.ApDirectPayments.Attach(entity);
                    _db.ApDirectPayments.Remove(entity);
                    _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted direct payment" + aPDirectPayments.ApDirectPaymentId + " " + aPDirectPayments.PaymentReference, Modules.Finance);
                #endregion
            }
            return Json(new[] { aPDirectPayments }.ToDataSourceResult(request, ModelState));
        }

        private void GetPaymentItems(ApDirectPayment record)
        {
            int did = int.Parse(record.SourceId.Split('_')[1]);
            string sourceType = record.SourceId.Split('_')[0];
            switch (sourceType)
            {
                case "LC":
                    #region foreign purchase

                    var purchaseCost = _db.ForeignPurchaseCosts.Find(did);
                    if (purchaseCost != null)
                    {
                        var entity = new ApDirectPaymentLine
                        {
                            ApDirectPaymentId = record.ApDirectPaymentId,
                            Purpose = purchaseCost.CostType,
                            Amount = purchaseCost.Amount,
                            ForexAmount = purchaseCost.ForexAmount,
                            ExchangeRate = purchaseCost.ExchangeRate,
                            Currency = purchaseCost.Currency,
                            AccountCode = "NA",
                            GLTaxRateID = 0,
                            TaxAmount = 0,
                            WithholdingType = 0,
                            WithholdingAmount = 0,
                            WithholdTax = false,
                            WithholdTaxAmount = 0,
                            TotalAmount = 0,
                            NetPayable = 0,
                            Remark = purchaseCost.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = purchaseCost.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = purchaseCost.ModifiedBy
                        };

                        _db.ApDirectPaymentLines.Add(entity);
                        _db.SaveChanges();
                    }

                    #endregion
                    break;
                case "PAYMENT-REQUEST":
                    #region payment request

                    var paymentRequestLines = _db.PaymentRequestLines.Where(x=>x.PaymentRequestId==did).ToList();
                    foreach (var requestLine in paymentRequestLines)
                    {
                        var apitem = _db.ApSetupItems.Find(int.Parse(requestLine.ItemName));
                        if (apitem != null)
                        {
                            var entity = new ApDirectPaymentLine
                            {
                                ApDirectPaymentId = record.ApDirectPaymentId,
                                Purpose = requestLine.PaymentRequest.ReasonOfPayment!=null? requestLine.PaymentRequest.ReasonOfPayment:"NA",
                                Amount = requestLine.Amount,
                                ForexAmount = requestLine.Amount,
                                ExchangeRate = 1,
                                Currency = "NA",
                                AccountCode = apitem.AccountCode,
                                GLTaxRateID = requestLine.GLTaxRateID,
                                TaxAmount = requestLine.TaxAmount,
                                WithholdingType = 0,
                                WithholdingAmount = 0,
                                WithholdTax = false,
                                WithholdTaxAmount = 0,
                                TotalAmount = requestLine.TotalAmount,
                                NetPayable = requestLine.TotalAmount,
                                Remark = requestLine.Remark,
                                DateCreated = DateTime.Now,
                                LastModified = requestLine.LastModified,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = requestLine.ModifiedBy
                            };

                            _db.ApDirectPaymentLines.Add(entity);
                        }

                        _db.SaveChanges();
                    }

                    #endregion
                    break;
                case "TRAVEL-ORDER":
                    #region travel order

                    var apTravelOrders = _db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == did).ToList();
                    foreach (var travelOrder in apTravelOrders)
                    {
                        var entity = new ApDirectPaymentLine
                        {
                            ApDirectPaymentId = record.ApDirectPaymentId,
                            Purpose = travelOrder.TravelOrder.PurposeOfTravel,
                            Amount = travelOrder.Total,
                            ForexAmount = travelOrder.Total,
                            ExchangeRate = 1,
                            Currency = "NA",
                            AccountCode = "NA",
                            GLTaxRateID = 0,
                            TaxAmount = 0,
                            WithholdingType = 0,
                            WithholdingAmount = 0,
                            WithholdTax = false,
                            WithholdTaxAmount = 0,
                            TotalAmount = travelOrder.Total,
                            NetPayable = travelOrder.Total,
                            Remark = "",
                            DateCreated = DateTime.Now,
                            LastModified = travelOrder.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = travelOrder.ModifiedBy
                        };

                        _db.ApDirectPaymentLines.Add(entity);
                        _db.SaveChanges();
                    }

                    #endregion
                    break;
                case "EXPENSE-CLAIM":
                    #region travel expense claim

                    var expensePayments = _db.ApTravelExpensePayments.Where(x => x.ApTravelExpenseClaimId == did).ToList();
                    foreach (var payment in expensePayments)
                    {
                        var entity = new ApDirectPaymentLine
                        {
                            ApDirectPaymentId = record.ApDirectPaymentId,
                            Purpose = payment.ApTravelExpenseClaim.PurposeOfTravel,
                            Amount = payment.Amount,
                            ForexAmount = payment.Amount,
                            ExchangeRate = 1,
                            Currency = "NA",
                            AccountCode = "NA",
                            GLTaxRateID = 0,
                            TaxAmount = 0,
                            WithholdingType = 0,
                            WithholdingAmount = 0,
                            WithholdTax = false,
                            WithholdTaxAmount = 0,
                            TotalAmount = payment.Amount,
                            NetPayable = payment.Amount,
                            Remark = "",
                            DateCreated = DateTime.Now,
                            LastModified = payment.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = payment.ModifiedBy
                        };

                        _db.ApDirectPaymentLines.Add(entity);
                        _db.SaveChanges();
                    }

                    #endregion
                    break;
            }
            #region update distribution
            _distributionHelper.UpdateApDirectPaymentDistribution(record.ApDirectPaymentId);
            #endregion
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        public JsonResult SendforApproval(int id = 0)
        {
            var  record = _db.ApDirectPayments.Find(id);
            string errMsg = "";
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            #region check segment length validation

            var distributions = _db.ApDirectPaymentDistributions.Where(x => x.ApDirectPaymentId == id).ToList();
            AccountHelper accountHelper = new AccountHelper();
            foreach (var dist in distributions)
            {
                string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                if (!String.IsNullOrEmpty(message))
                {
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #region debit credit validation
                if (dist.Debit != 0 && dist.Credit != 0)
                {
                    message = $"{dist.AccountCode} Account has both debit and credit value.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit == 0 && dist.Credit == 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit > 0 && dist.Credit > 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit < 0 || dist.Credit < 0)
                {
                    message = $"{dist.AccountCode} Negative values are not allowed.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

            }


            #endregion

            if (record != null && record.IsOnlineApproved)
            {
                errMsg = "You cannot manipulate this record";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsSendForApproval)
            {
                errMsg = "This record is already sent for approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

          

            try
            {
                if (record != null)
                {
                    sumTotalDisDebit = record.ApDirectPaymentDistribution.Sum(x => x.Debit);
                    sumTotalDisCredit = record.ApDirectPaymentDistribution.Sum(x => x.Credit);
                }
            }
            catch (Exception e)
            {
                errMsg = "Please add distribution and try again";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (sumTotalDisDebit != sumTotalDisCredit)
            {
                errMsg = "Total Debit and Total Credit is not equal.Please correct it and try again";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (sumTotalDisDebit == 0 || sumTotalDisCredit == 0)
            {
                errMsg = "Total balance should be more than 0.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineApproved && !record.IsSendForApproval)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
         public JsonResult TransferDistribution(string id, string description)
         {
             int myid = int.Parse(id);
             var record = _db.ApDirectPayments.Find(myid);
             if (record != null && !record.IsOnlineApproved)
             {
                 var formattedData = new
                 {
                     Message = "The record is not approved.",
                     Status = "NOT_OK"
                 };
                 return Json(formattedData, JsonRequestBehavior.AllowGet);
             }

             if (record != null && !record.IsOnlineTransferred)
             {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                    .Where(x => x.Source == JournalSources.AP_Direct_Payment.ToString())
                    .Where(x => x.SourceId == record.ApDirectPaymentId.ToString())
                    .FirstOrDefault();
                if (exist != null)
                {
                    var formattedData = new
                    {
                        Message = "The record is already transffered, it can't be retransferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.ApDirectPaymentDistribution)
                 {
                     string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                     if (!String.IsNullOrEmpty(message))
                     {
                         var formattedData = new
                         {
                             Message = message,
                             Status = "NOT_OK"
                         };
                         return Json(formattedData, JsonRequestBehavior.AllowGet);
                     }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                 decimal credit = 0;
                 var distribution = record.ApDirectPaymentDistribution.ToList();
                 if (distribution.Any())
                 {
                     debit = distribution.Sum(x => x.Debit);
                     credit = distribution.Sum(x => x.Credit);
                 }
                 if (debit != credit)
                 {
                     var formattedData = new
                     {
                         Message = "Total debit and total credit is not balanced.",
                         Status = "NOT_OK"
                     };
                     return Json(formattedData, JsonRequestBehavior.AllowGet);
                 }
                 if (debit == 0)
                 {
                     var formattedData = new
                     {
                         Message = "Total debit and total credit is zero.",
                         Status = "NOT_OK"
                     };
                     return Json(formattedData, JsonRequestBehavior.AllowGet);
                 }
                #endregion
                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.PaymentDate.Date)
                    .Where(x => x.DateTo >= record.PaymentDate.Date).ToList();
                if (period.Any())
                {

                    if (period.Count() == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name.ToString();
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod +
                                              " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var formattedData = new
                            {
                                Message = "Period not found.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the Invoice date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There are no periods defined that contain the payment date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
               
                 if (record.ApDirectPaymentDistribution.Any())
                 {
                     GlRecordJournal journal = new GlRecordJournal
                     {
                         DateCreated = DateTime.Now,
                         CreatedBy = User.Identity.Name,
                         JournalName = "AP Direct Payment",
                         Description = !String.IsNullOrEmpty(description) ? description : "AP Direct Payment",
                         FiscalYear = glyear,
                         PeriodName = glperiod,
                         CategoryName = "AP Direct Payment",
                         SourcesName = "AP",
                         JournalNumber = "APDPAY." + record.ApDirectPaymentId.ToString(),
                         SourceId = record.ApDirectPaymentId.ToString(),
                         Source = JournalSources.AP_Direct_Payment.ToString(),
                         EffectiveDate = record.PaymentDate.Date,
                         OrgBranchName = record.OrgBranchName,
                         Status = "Approved",
                         Remark = record.PaymentReference,
                         PaymentReference = record.PaymentReference,
                         ReferenceDoc=record.Remark,
                         InvoiceReferenceDoc="",
                         IsOnlineApproved = true,
                         OnlineApprovedBy = User.Identity.Name,
                         OnlineApprovedTime = DateTime.Now
                     };
                     _db.GLRecordJournals.Add(journal);
                     _db.SaveChanges();
                     foreach (var i in record.ApDirectPaymentDistribution)
                     {
                         #region debit and credit

                         var journalDetail = new GLRecordJournalDetail
                         {
                             DateCreated = DateTime.Now,
                             CreatedBy = User.Identity.Name,
                             GLRecordJournal = journal,
                             AccountCode = i.AccountCode,
                             AccountDesc = i.AccountDesc,
                             Debit = i.Debit,
                             Credit = i.Credit,
                             AdditionalDescription = i.AdditionalDescription,
                         };
                         _db.GLRecordJournalDetails.Add(journalDetail);
                         _db.SaveChanges();
                         #endregion
                     }

                     #region update record

                     record.IsOnlineTransferred = true;
                     _db.Entry(record).State = EntityState.Modified;
                     _db.SaveChanges();

                    #endregion

                     #region split journal
                     var journaldetails = _db.GLRecordJournalDetails
                         .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                     foreach (var detail in journaldetails)
                     {
                         AccountHelper helper = new AccountHelper();
                         helper.SplitJournal(detail.GlRecordJournalDetailId);
                     }
                     #endregion

                    var formattedData = new
                     {
                         Message = "Successfully transfered.",
                         Status = "OK"
                     };
                     return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                 #endregion
                
             }
             else
             {
                 var formattedData = new
                 {
                     Message = "You cannot transfer this transaction, it is already transfered.",
                     Status = "NOT_OK"
                 };
                 return Json(formattedData, JsonRequestBehavior.AllowGet);
             }

             return null;
         }

        [AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ApDirectPayments.Find(id);
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ApDirectPaymentLines.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ApDirectPayments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                #region update payment request

                int requestid = int.Parse(record.SourceId);
                var payment = _db.ForeignPurchaseCosts.Find(requestid);
                if (payment != null)
                {
                    payment.Paid = false;
                    _db.Entry(payment).State = EntityState.Modified;
                }

                _db.SaveChanges();
                #endregion
                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "direct payment", "Success", User.Identity.Name, $"void direct payment {id} ", Modules.Finance);
                #endregion

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
           
            if (record.ApDirectPaymentLines.Count == 0)
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
                    Message = "The record is already void.You can not void again.",
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
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
