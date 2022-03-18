using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
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
    public class ApMiscPaymentsController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();

       

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
        public ActionResult APMiscellaneousPayments_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<ApMiscellaneousPayment> apmiscellaneouspayments = _db.APMiscellaneousPayments.Where(x => mybraches.Contains(x.OrgBranchName));
            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetApMiscPaymentTranferableList();
                apmiscellaneouspayments =
                    apmiscellaneouspayments.Where(x => transfferableList.Contains(x.ApMiscellaneousPaymentId));
            }
            DataSourceResult result = apmiscellaneouspayments.ToDataSourceResult(request, aPMiscellaneousPayments => new {
                ApMiscellaneousPaymentId = aPMiscellaneousPayments.ApMiscellaneousPaymentId,
                IsDirectPayment = aPMiscellaneousPayments.IsDirectPayment,
                SupplierId = aPMiscellaneousPayments.SupplierId,
                IsNewSupplier = aPMiscellaneousPayments.IsNewSupplier,
                SupplierName = aPMiscellaneousPayments.SupplierName,
                Purchaser = aPMiscellaneousPayments.Purchaser,
                Grn = aPMiscellaneousPayments.Grn,
                Po = aPMiscellaneousPayments.Po,
                PaymentType = aPMiscellaneousPayments.PaymentType,
                PaymentMethod = aPMiscellaneousPayments.PaymentMethod,
                PaymentReference = aPMiscellaneousPayments.PaymentReference,
                PaymentDate = aPMiscellaneousPayments.PaymentDate,
                LineAmount = Math.Round(aPMiscellaneousPayments.LineAmount, 2),
                TaxAmount = Math.Round(aPMiscellaneousPayments.TaxAmount, 2),
                TotalAmount = Math.Round(aPMiscellaneousPayments.TotalAmount, 2),
                IsPostedToSecondLedger = aPMiscellaneousPayments.IsPostedToSecondLedger,
                Remark = aPMiscellaneousPayments.Remark,
                IsSendForApproval = aPMiscellaneousPayments.IsSendForApproval,
                IsOnlineTransferred = aPMiscellaneousPayments.IsOnlineTransferred,
                IsOnlineTransferredBy = aPMiscellaneousPayments.IsOnlineTransferredBy,
                OnlineTransferredTime = aPMiscellaneousPayments.OnlineTransferredTime,
                Status = aPMiscellaneousPayments.Status,
                IsOnlineApproved = aPMiscellaneousPayments.IsOnlineApproved,
                OnlineApprovedBy = aPMiscellaneousPayments.OnlineApprovedBy,
                OnlineApprovedTime = aPMiscellaneousPayments.OnlineApprovedTime,
                IsVoid = aPMiscellaneousPayments.IsVoid,
                VoidBy = aPMiscellaneousPayments.VoidBy,
                VoidTime = aPMiscellaneousPayments.VoidTime,
                OrgBranchName = aPMiscellaneousPayments.OrgBranchName,
                DateCreated = aPMiscellaneousPayments.DateCreated,
                LastModified = aPMiscellaneousPayments.LastModified,
                CreatedBy = aPMiscellaneousPayments.CreatedBy,
                ModifiedBy = aPMiscellaneousPayments.ModifiedBy
            });

            return Json(result);
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPayments_Create([DataSourceRequest]DataSourceRequest request, ApMiscellaneousPayment aPMiscellaneousPayments)
        {
            #region validation


            if (aPMiscellaneousPayments.IsNewSupplier && String.IsNullOrWhiteSpace(aPMiscellaneousPayments.SupplierName))
                ModelState.AddModelError("supplier", "Please enter supplier name.");
            if (!aPMiscellaneousPayments.IsNewSupplier && String.IsNullOrWhiteSpace(aPMiscellaneousPayments.SupplierId))
                ModelState.AddModelError("supplier", "Please choose supplier.");

            if (!String.IsNullOrEmpty(aPMiscellaneousPayments.PaymentReference))
            {
                var exists = _db.APMiscellaneousPayments.Where(x => x.PaymentReference == aPMiscellaneousPayments.PaymentReference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("PaymentReference", $"{aPMiscellaneousPayments.PaymentReference}  already exists in the system.");
            }

            if (!String.IsNullOrEmpty(aPMiscellaneousPayments.Remark))
            {
                var exists = _db.APMiscellaneousPayments.Where(x => x.Remark == aPMiscellaneousPayments.Remark && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Remark", $"{aPMiscellaneousPayments.Remark}  already exists in the system.");
            }

            #endregion

            if (!aPMiscellaneousPayments.IsNewSupplier && !String.IsNullOrWhiteSpace(aPMiscellaneousPayments.SupplierId))
            {
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == aPMiscellaneousPayments.SupplierId);
                if (supplier != null) aPMiscellaneousPayments.SupplierName = supplier.Name;
            }

            if (aPMiscellaneousPayments.IsNewSupplier)
                aPMiscellaneousPayments.SupplierId = "";

            if (ModelState.IsValid)
            {
                var entity = new ApMiscellaneousPayment
                {
                    IsDirectPayment = aPMiscellaneousPayments.IsDirectPayment,
                    SupplierId = aPMiscellaneousPayments.SupplierId,
                    IsNewSupplier = aPMiscellaneousPayments.IsNewSupplier,
                    SupplierName = aPMiscellaneousPayments.SupplierName,
                    Purchaser = aPMiscellaneousPayments.Purchaser,
                    Grn = aPMiscellaneousPayments.Grn,
                    Po = aPMiscellaneousPayments.Po,
                    PaymentType = aPMiscellaneousPayments.PaymentType,
                    PaymentMethod = aPMiscellaneousPayments.PaymentMethod,
                    PaymentReference = aPMiscellaneousPayments.PaymentReference,
                    PaymentDate = aPMiscellaneousPayments.PaymentDate.Date,
                    LineAmount = Math.Round(aPMiscellaneousPayments.LineAmount, 2),
                    TaxAmount = Math.Round(aPMiscellaneousPayments.TaxAmount, 2),
                    TotalAmount = Math.Round(aPMiscellaneousPayments.TotalAmount, 2),
                    IsPostedToSecondLedger = aPMiscellaneousPayments.IsPostedToSecondLedger,
                    Remark = aPMiscellaneousPayments.Remark,
                    IsSendForApproval = aPMiscellaneousPayments.IsSendForApproval,
                    IsOnlineTransferred = aPMiscellaneousPayments.IsOnlineTransferred,
                    IsOnlineTransferredBy = aPMiscellaneousPayments.IsOnlineTransferredBy,
                    OnlineTransferredTime = aPMiscellaneousPayments.OnlineTransferredTime,
                    Status = aPMiscellaneousPayments.Status,
                    IsOnlineApproved = aPMiscellaneousPayments.IsOnlineApproved,
                    OnlineApprovedBy = aPMiscellaneousPayments.OnlineApprovedBy,
                    OnlineApprovedTime = aPMiscellaneousPayments.OnlineApprovedTime,
                    IsVoid = aPMiscellaneousPayments.IsVoid,
                    VoidBy = aPMiscellaneousPayments.VoidBy,
                    VoidTime = aPMiscellaneousPayments.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = aPMiscellaneousPayments.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aPMiscellaneousPayments.ModifiedBy
                };

                _db.APMiscellaneousPayments.Add(entity);
                _db.SaveChanges();
                aPMiscellaneousPayments.ApMiscellaneousPaymentId = entity.ApMiscellaneousPaymentId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment", "Success", User.Identity.Name, $"created misc payment {entity.ApMiscellaneousPaymentId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPMiscellaneousPayments }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPayments_Update([DataSourceRequest]DataSourceRequest request, ApMiscellaneousPayment aPMiscellaneousPayments)
        {
            #region validation

            if (aPMiscellaneousPayments.IsNewSupplier && String.IsNullOrWhiteSpace(aPMiscellaneousPayments.SupplierName))
                ModelState.AddModelError("supplier", "Please enter supplier name.");
            if (!aPMiscellaneousPayments.IsNewSupplier && String.IsNullOrWhiteSpace(aPMiscellaneousPayments.SupplierId))
                ModelState.AddModelError("supplier", "Please choose supplier.");

            if (!String.IsNullOrEmpty(aPMiscellaneousPayments.PaymentReference))
            {
                var exists = _db.APMiscellaneousPayments
                    .Where(x=>x.ApMiscellaneousPaymentId!=aPMiscellaneousPayments.ApMiscellaneousPaymentId)
                    .Where(x => x.PaymentReference == aPMiscellaneousPayments.PaymentReference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("PaymentReference", $"{aPMiscellaneousPayments.PaymentReference}  already exists in the system.");
            }

            if (!String.IsNullOrEmpty(aPMiscellaneousPayments.Remark))
            {
                var exists = _db.APMiscellaneousPayments
                    .Where(x => x.ApMiscellaneousPaymentId != aPMiscellaneousPayments.ApMiscellaneousPaymentId)
                    .Where(x => x.Remark == aPMiscellaneousPayments.Remark && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Remark", $"{aPMiscellaneousPayments.Remark}  already exists in the system.");
            }

            #endregion

            if (!aPMiscellaneousPayments.IsNewSupplier && !String.IsNullOrWhiteSpace(aPMiscellaneousPayments.SupplierId))
            {
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == aPMiscellaneousPayments.SupplierId);
                if (supplier != null) aPMiscellaneousPayments.SupplierName = supplier.Name;
            }


            if (aPMiscellaneousPayments.IsNewSupplier)
                aPMiscellaneousPayments.SupplierId = "";

            #region validated record

            var parent = _db.APMiscellaneousPayments.Find(aPMiscellaneousPayments.ApMiscellaneousPaymentId);
            if (parent != null)
            {
                if (parent.IsOnlineApproved)
                    ModelState.AddModelError("approved", "You cannot manipulate this record");

                #region detach local version
                var local = _db.Set<ApMiscellaneousPayment>()
                    .Local
                    .FirstOrDefault(f => f.ApMiscellaneousPaymentId == parent.ApMiscellaneousPaymentId);
                if (local != null)
                {
                    _db.Entry(local).State = EntityState.Detached;
                }
                #endregion

            }


            #endregion


            if (ModelState.IsValid)
            {
                    var entity = new ApMiscellaneousPayment
                    {
                        IsDirectPayment = aPMiscellaneousPayments.IsDirectPayment,
                        ApMiscellaneousPaymentId = aPMiscellaneousPayments.ApMiscellaneousPaymentId,
                        SupplierId = aPMiscellaneousPayments.SupplierId,
                        IsNewSupplier = aPMiscellaneousPayments.IsNewSupplier,
                        SupplierName = aPMiscellaneousPayments.SupplierName,
                        Purchaser = aPMiscellaneousPayments.Purchaser,
                        Grn = aPMiscellaneousPayments.Grn,
                        Po = aPMiscellaneousPayments.Po,
                        PaymentType = aPMiscellaneousPayments.PaymentType,
                        PaymentMethod = aPMiscellaneousPayments.PaymentMethod,
                        PaymentReference = aPMiscellaneousPayments.PaymentReference,
                        PaymentDate = aPMiscellaneousPayments.PaymentDate,
                        LineAmount = Math.Round(aPMiscellaneousPayments.LineAmount, 2),
                        TaxAmount = Math.Round(aPMiscellaneousPayments.TaxAmount, 2),
                        TotalAmount = Math.Round(aPMiscellaneousPayments.TotalAmount, 2),
                        IsPostedToSecondLedger = aPMiscellaneousPayments.IsPostedToSecondLedger,
                        Remark = aPMiscellaneousPayments.Remark,
                        IsSendForApproval = aPMiscellaneousPayments.IsSendForApproval,
                        IsOnlineTransferred = aPMiscellaneousPayments.IsOnlineTransferred,
                        IsOnlineTransferredBy = aPMiscellaneousPayments.IsOnlineTransferredBy,
                        OnlineTransferredTime = aPMiscellaneousPayments.OnlineTransferredTime,
                        Status = aPMiscellaneousPayments.Status,
                        IsOnlineApproved = aPMiscellaneousPayments.IsOnlineApproved,
                        OnlineApprovedBy = aPMiscellaneousPayments.OnlineApprovedBy,
                        OnlineApprovedTime = aPMiscellaneousPayments.OnlineApprovedTime,
                        IsVoid = aPMiscellaneousPayments.IsVoid,
                        VoidBy = aPMiscellaneousPayments.VoidBy,
                        VoidTime = aPMiscellaneousPayments.VoidTime,
                        OrgBranchName = aPMiscellaneousPayments.OrgBranchName,
                        DateCreated = aPMiscellaneousPayments.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aPMiscellaneousPayments.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.APMiscellaneousPayments.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment", "Success", User.Identity.Name, $"updated misc payment {entity.ApMiscellaneousPaymentId} ", Modules.Finance);
                #endregion
            }
            return Json(new[] { aPMiscellaneousPayments }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPayments_Destroy([DataSourceRequest]DataSourceRequest request, ApMiscellaneousPayment aPMiscellaneousPayments)
        {

            

            if (ModelState.IsValid)
            {
                    var entity = new ApMiscellaneousPayment
                    {
                        IsDirectPayment = aPMiscellaneousPayments.IsDirectPayment,
                        ApMiscellaneousPaymentId = aPMiscellaneousPayments.ApMiscellaneousPaymentId,
                        SupplierId = aPMiscellaneousPayments.SupplierId,
                        IsNewSupplier = aPMiscellaneousPayments.IsNewSupplier,
                        SupplierName = aPMiscellaneousPayments.SupplierName,
                        Purchaser = aPMiscellaneousPayments.Purchaser,
                        Grn = aPMiscellaneousPayments.Grn,
                        Po = aPMiscellaneousPayments.Po,
                        PaymentType = aPMiscellaneousPayments.PaymentType,
                        PaymentMethod = aPMiscellaneousPayments.PaymentMethod,
                        PaymentReference = aPMiscellaneousPayments.PaymentReference,
                        PaymentDate = aPMiscellaneousPayments.PaymentDate,
                        IsPostedToSecondLedger = aPMiscellaneousPayments.IsPostedToSecondLedger,
                        Remark = aPMiscellaneousPayments.Remark,
                        IsSendForApproval = aPMiscellaneousPayments.IsSendForApproval,
                        IsOnlineTransferred = aPMiscellaneousPayments.IsOnlineTransferred,
                        IsOnlineTransferredBy = aPMiscellaneousPayments.IsOnlineTransferredBy,
                        OnlineTransferredTime = aPMiscellaneousPayments.OnlineTransferredTime,
                        Status = aPMiscellaneousPayments.Status,
                        IsOnlineApproved = aPMiscellaneousPayments.IsOnlineApproved,
                        OnlineApprovedBy = aPMiscellaneousPayments.OnlineApprovedBy,
                        OnlineApprovedTime = aPMiscellaneousPayments.OnlineApprovedTime,
                        IsVoid = aPMiscellaneousPayments.IsVoid,
                        VoidBy = aPMiscellaneousPayments.VoidBy,
                        VoidTime = aPMiscellaneousPayments.VoidTime,
                        OrgBranchName = aPMiscellaneousPayments.OrgBranchName,
                        DateCreated = aPMiscellaneousPayments.DateCreated,
                        LastModified = aPMiscellaneousPayments.LastModified,
                        CreatedBy = aPMiscellaneousPayments.CreatedBy,
                        ModifiedBy = aPMiscellaneousPayments.ModifiedBy
                    };

                    _db.APMiscellaneousPayments.Attach(entity);
                    _db.APMiscellaneousPayments.Remove(entity);
                    _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted misc. payment " + aPMiscellaneousPayments.ApMiscellaneousPaymentId + " " + aPMiscellaneousPayments.Remark, Modules.Finance);
                #endregion


            }
            return Json(new[] { aPMiscellaneousPayments }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView ,FinanceConstants.AccountPayableApprover ,FinanceConstants.AccountPayablePayment)]
        

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        public JsonResult SendforApproval(int id = 0)
        {
            var  record = _db.APMiscellaneousPayments.Find(id);
            #region check segment length validation

            var distributions = _db.APMiscellaneousPaymentDistributions.Where(x => x.ApMiscellaneousPaymentId == id).ToList();
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
            bool isValid = true;
            string errMsg = "";
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;

            if (record != null && record.IsOnlineApproved)
            {
                isValid = false;
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
                isValid = false;
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
                    sumTotalDisDebit = record.ApMiscellaneousPaymentDistribution.Sum(x => x.Debit);
                    sumTotalDisCredit = record.ApMiscellaneousPaymentDistribution.Sum(x => x.Credit);
                }
            }
            catch (Exception e)
            {
                isValid = false;
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
                isValid = false;
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
                isValid = false;
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

            }
            return null;
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
         public JsonResult TransferDistribution(string id, string description)
         {
             int myid = int.Parse(id);
             var record = _db.APMiscellaneousPayments.Find(myid);
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
                    .Where(x => x.Source == JournalSources.AP_MiscPayment.ToString())
                    .Where(x => x.SourceId == record.ApMiscellaneousPaymentId.ToString())
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
                foreach (var dist in record.ApMiscellaneousPaymentDistribution)
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
                 var distribution = record.ApMiscellaneousPaymentDistribution.ToList();
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
                        Message = "There are no periods defined that contain the Payment date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                
                 if (record.ApMiscellaneousPaymentDistribution.Any())
                 {
                    
                    GlRecordJournal journal = new GlRecordJournal
                     {
                         DateCreated = DateTime.Now,
                         CreatedBy = User.Identity.Name,
                         JournalName = "AP Misc Payment",
                         Description = !String.IsNullOrEmpty(description) ? description : "AP Misc Payment",
                         FiscalYear = glyear,
                         PeriodName = glperiod,
                         CategoryName = "AP Misc Payment",
                         SourcesName = "AP",
                         JournalNumber = "APMPAY." + record.ApMiscellaneousPaymentId.ToString(),
                         SourceId = record.ApMiscellaneousPaymentId.ToString(),
                         Source = JournalSources.AP_MiscPayment.ToString(),
                         EffectiveDate = record.PaymentDate.Date,
                         OrgBranchName = record.OrgBranchName,
                         Status = "Approved",
                         Remark = record.Remark,
                         PaymentReference = record.PaymentReference,
                         ReferenceDoc = "",
                         BranchId = 0,//to change
                         InvoiceReferenceDoc = "",
                         IsOnlineApproved = true,
                         OnlineApprovedBy = User.Identity.Name,
                         OnlineApprovedTime = DateTime.Now
                     };
                     _db.GLRecordJournals.Add(journal);
                     _db.SaveChanges();
                     foreach (var i in record.ApMiscellaneousPaymentDistribution)
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
            var record = _db.APMiscellaneousPayments.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ApMiscellaneousPaymentLines.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.APMiscellaneousPayments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "misc. payment", "Success", User.Identity.Name, $"void misc. payment {id} ", Modules.Finance);
                #endregion

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
            if (record.ApMiscellaneousPaymentLines.Count == 0)
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
