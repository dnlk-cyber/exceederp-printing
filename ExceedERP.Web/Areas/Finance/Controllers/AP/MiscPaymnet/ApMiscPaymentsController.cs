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
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            //cuz it was not having branchid
            IQueryable<ApMiscellaneousPayment> miscPayments = _db.APMiscellaneousPayments
                  .Where(x => mybraches.Contains(x.BranchId));

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                miscPayments = miscPayments.Where(x => myBusinessUnits.Contains(x.BusinessUnitId) ||x.BusinessUnitId==0); //to include null
            }

            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetApMiscPaymentTranferableList();
                miscPayments =
                    miscPayments.Where(x => transfferableList.Contains(x.ApMiscellaneousPaymentId));
            }
            DataSourceResult result = miscPayments.ToDataSourceResult(request, miscPayment => new {
                ApMiscellaneousPaymentId = miscPayment.ApMiscellaneousPaymentId,
                BranchId = miscPayment.BranchId,
                BusinessUnitId = miscPayment.BusinessUnitId,
                CreatedAtBranchId = miscPayment.CreatedAtBranchId,
                CreatedAtBusinessUnitId = miscPayment.CreatedAtBusinessUnitId,
                IsDirectPayment = miscPayment.IsDirectPayment,
                SupplierId = miscPayment.SupplierId,
                IsNewSupplier = miscPayment.IsNewSupplier,
                SupplierName = miscPayment.SupplierName,
                Purchaser = miscPayment.Purchaser,
                Grn = miscPayment.Grn,
                Po = miscPayment.Po,
                PaymentType = miscPayment.PaymentType,
                PaymentMethod = miscPayment.PaymentMethod,
                PaymentReference = miscPayment.PaymentReference,
                Reference = miscPayment.Reference,
                PaymentDate = miscPayment.PaymentDate,
                LineAmount = Math.Round(miscPayment.LineAmount, 2),
                TaxAmount = Math.Round(miscPayment.TaxAmount, 2),
                TotalAmount = Math.Round(miscPayment.TotalAmount, 2),
                WithholdingAmount = Math.Round(miscPayment.WithholdingAmount, 2),
                WithholdTaxAmount = Math.Round(miscPayment.WithholdTaxAmount, 2),
                DeductAmount = Math.Round(miscPayment.DeductAmount, 2),
                NetPayable = Math.Round(miscPayment.NetPayable, 2),
                Remark = miscPayment.Remark,
                IsSendForApproval = miscPayment.IsSendForApproval,
                IsOnlineTransferred = miscPayment.IsOnlineTransferred,
                IsOnlineTransferredBy = miscPayment.IsOnlineTransferredBy,
                OnlineTransferredTime = miscPayment.OnlineTransferredTime,
                Status = miscPayment.Status,
                IsOnlineApproved = miscPayment.IsOnlineApproved,
                OnlineApprovedBy = miscPayment.OnlineApprovedBy,
                OnlineApprovedTime = miscPayment.OnlineApprovedTime,
                IsVoid = miscPayment.IsVoid,
                VoidBy = miscPayment.VoidBy,
                VoidTime = miscPayment.VoidTime,
                OrgBranchName = miscPayment.OrgBranchName,
                DateCreated = miscPayment.DateCreated,
                LastModified = miscPayment.LastModified,
                CreatedBy = miscPayment.CreatedBy,
                ModifiedBy = miscPayment.ModifiedBy
            });

            return Json(result);
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPayments_Create([DataSourceRequest]DataSourceRequest request, ApMiscellaneousPayment miscPayments)
        {
            #region validation


            if (miscPayments.IsNewSupplier && String.IsNullOrWhiteSpace(miscPayments.SupplierName))
                ModelState.AddModelError("supplier", "Please enter supplier name.");
            if (!miscPayments.IsNewSupplier && String.IsNullOrWhiteSpace(miscPayments.SupplierId))
                ModelState.AddModelError("supplier", "Please choose supplier.");

            if (!String.IsNullOrEmpty(miscPayments.PaymentReference))
            {
                var exists = _db.APMiscellaneousPayments.Where(x => x.PaymentReference == miscPayments.PaymentReference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("PaymentReference", $"{miscPayments.PaymentReference}  already exists in the system.");
            }

            if (!String.IsNullOrEmpty(miscPayments.Remark))
            {
                var exists = _db.APMiscellaneousPayments.Where(x => x.Remark == miscPayments.Remark && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Remark", $"{miscPayments.Remark}  already exists in the system.");
            }

            #endregion
            #region data
            if (!miscPayments.IsNewSupplier && !String.IsNullOrWhiteSpace(miscPayments.SupplierId))
            {
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == miscPayments.SupplierId);
                if (supplier != null) miscPayments.SupplierName = supplier.Name;
            }

            if (miscPayments.IsNewSupplier)
                miscPayments.SupplierId = "";

            miscPayments.BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            miscPayments.BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            miscPayments.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            miscPayments.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApMiscellaneousPayment
                {
                    BranchId = miscPayments.BranchId,
                    BusinessUnitId = miscPayments.BusinessUnitId,
                    CreatedAtBranchId = miscPayments.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = miscPayments.CreatedAtBusinessUnitId,
                    IsDirectPayment = miscPayments.IsDirectPayment,
                    SupplierId = miscPayments.SupplierId,
                    IsNewSupplier = miscPayments.IsNewSupplier,
                    SupplierName = miscPayments.SupplierName,
                    Purchaser = miscPayments.Purchaser,
                    Grn = miscPayments.Grn,
                    Po = miscPayments.Po,
                    PaymentType = miscPayments.PaymentType,
                    PaymentMethod = miscPayments.PaymentMethod,
                    PaymentReference = miscPayments.PaymentReference,
                    Reference = miscPayments.Reference,
                    PaymentDate = miscPayments.PaymentDate.Date,
                    LineAmount = Math.Round(miscPayments.LineAmount, 2),
                    TaxAmount = Math.Round(miscPayments.TaxAmount, 2),
                    TotalAmount = Math.Round(miscPayments.TotalAmount, 2),
                   
                    Remark = miscPayments.Remark,
                    IsSendForApproval = miscPayments.IsSendForApproval,
                    IsOnlineTransferred = miscPayments.IsOnlineTransferred,
                    IsOnlineTransferredBy = miscPayments.IsOnlineTransferredBy,
                    OnlineTransferredTime = miscPayments.OnlineTransferredTime,
                    Status = miscPayments.Status,
                    IsOnlineApproved = miscPayments.IsOnlineApproved,
                    OnlineApprovedBy = miscPayments.OnlineApprovedBy,
                    OnlineApprovedTime = miscPayments.OnlineApprovedTime,
                    IsVoid = miscPayments.IsVoid,
                    VoidBy = miscPayments.VoidBy,
                    VoidTime = miscPayments.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = miscPayments.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = miscPayments.ModifiedBy
                };

                _db.APMiscellaneousPayments.Add(entity);
                _db.SaveChanges();
                miscPayments.ApMiscellaneousPaymentId = entity.ApMiscellaneousPaymentId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment", "Success", User.Identity.Name, $"created misc payment {entity.ApMiscellaneousPaymentId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { miscPayments }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPayments_Update([DataSourceRequest]DataSourceRequest request, ApMiscellaneousPayment miscPayment)
        {
            #region validation

            if (miscPayment.IsNewSupplier && String.IsNullOrWhiteSpace(miscPayment.SupplierName))
                ModelState.AddModelError("supplier", "Please enter supplier name.");
            if (!miscPayment.IsNewSupplier && String.IsNullOrWhiteSpace(miscPayment.SupplierId))
                ModelState.AddModelError("supplier", "Please choose supplier.");

            if (!String.IsNullOrEmpty(miscPayment.PaymentReference))
            {
                var exists = _db.APMiscellaneousPayments
                    .Where(x=>x.ApMiscellaneousPaymentId!=miscPayment.ApMiscellaneousPaymentId)
                    .Where(x => x.PaymentReference == miscPayment.PaymentReference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("PaymentReference", $"{miscPayment.PaymentReference}  already exists in the system.");
            }

            if (!String.IsNullOrEmpty(miscPayment.Remark))
            {
                var exists = _db.APMiscellaneousPayments
                    .Where(x => x.ApMiscellaneousPaymentId != miscPayment.ApMiscellaneousPaymentId)
                    .Where(x => x.Remark == miscPayment.Remark && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Remark", $"{miscPayment.Remark}  already exists in the system.");
            }

            #endregion

            if (!miscPayment.IsNewSupplier && !String.IsNullOrWhiteSpace(miscPayment.SupplierId))
            {
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == miscPayment.SupplierId);
                if (supplier != null) miscPayment.SupplierName = supplier.Name;
            }


            if (miscPayment.IsNewSupplier)
                miscPayment.SupplierId = "";

            #region validated record

            var parent = _db.APMiscellaneousPayments.Find(miscPayment.ApMiscellaneousPaymentId);
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
                        BranchId = miscPayment.BranchId,
                        BusinessUnitId = miscPayment.BusinessUnitId,
                        CreatedAtBranchId = miscPayment.CreatedAtBranchId,
                        CreatedAtBusinessUnitId = miscPayment.CreatedAtBusinessUnitId,
                        IsDirectPayment = miscPayment.IsDirectPayment,
                        ApMiscellaneousPaymentId = miscPayment.ApMiscellaneousPaymentId,
                        SupplierId = miscPayment.SupplierId,
                        IsNewSupplier = miscPayment.IsNewSupplier,
                        SupplierName = miscPayment.SupplierName,
                        Purchaser = miscPayment.Purchaser,
                        Grn = miscPayment.Grn,
                        Po = miscPayment.Po,
                        PaymentType = miscPayment.PaymentType,
                        PaymentMethod = miscPayment.PaymentMethod,
                        PaymentReference = miscPayment.PaymentReference,
                        Reference = miscPayment.Reference,
                        PaymentDate = miscPayment.PaymentDate,
                        LineAmount = Math.Round(miscPayment.LineAmount, 2),
                        TaxAmount = Math.Round(miscPayment.TaxAmount, 2),
                        TotalAmount = Math.Round(miscPayment.TotalAmount, 2),
                       
                        Remark = miscPayment.Remark,
                        IsSendForApproval = miscPayment.IsSendForApproval,
                        IsOnlineTransferred = miscPayment.IsOnlineTransferred,
                        IsOnlineTransferredBy = miscPayment.IsOnlineTransferredBy,
                        OnlineTransferredTime = miscPayment.OnlineTransferredTime,
                        Status = miscPayment.Status,
                        IsOnlineApproved = miscPayment.IsOnlineApproved,
                        OnlineApprovedBy = miscPayment.OnlineApprovedBy,
                        OnlineApprovedTime = miscPayment.OnlineApprovedTime,
                        IsVoid = miscPayment.IsVoid,
                        VoidBy = miscPayment.VoidBy,
                        VoidTime = miscPayment.VoidTime,
                        OrgBranchName = miscPayment.OrgBranchName,
                        DateCreated = miscPayment.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = miscPayment.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.APMiscellaneousPayments.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment", "Success", User.Identity.Name, $"updated misc payment {entity.ApMiscellaneousPaymentId} ", Modules.Finance);
                #endregion
            }
            return Json(new[] { miscPayment }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPayments_Destroy([DataSourceRequest]DataSourceRequest request, ApMiscellaneousPayment miscPayment)
        {

            

            if (ModelState.IsValid)
            {
                    var entity = new ApMiscellaneousPayment
                    {
                        BranchId = miscPayment.BranchId,
                        BusinessUnitId = miscPayment.BusinessUnitId,
                        CreatedAtBranchId = miscPayment.CreatedAtBranchId,
                        CreatedAtBusinessUnitId = miscPayment.CreatedAtBusinessUnitId,
                        IsDirectPayment = miscPayment.IsDirectPayment,
                        ApMiscellaneousPaymentId = miscPayment.ApMiscellaneousPaymentId,
                        SupplierId = miscPayment.SupplierId,
                        IsNewSupplier = miscPayment.IsNewSupplier,
                        SupplierName = miscPayment.SupplierName,
                        Purchaser = miscPayment.Purchaser,
                        Grn = miscPayment.Grn,
                        Po = miscPayment.Po,
                        PaymentType = miscPayment.PaymentType,
                        PaymentMethod = miscPayment.PaymentMethod,
                        PaymentReference = miscPayment.PaymentReference,
                        Reference = miscPayment.Reference,
                        PaymentDate = miscPayment.PaymentDate,
                     
                        Remark = miscPayment.Remark,
                        IsSendForApproval = miscPayment.IsSendForApproval,
                        IsOnlineTransferred = miscPayment.IsOnlineTransferred,
                        IsOnlineTransferredBy = miscPayment.IsOnlineTransferredBy,
                        OnlineTransferredTime = miscPayment.OnlineTransferredTime,
                        Status = miscPayment.Status,
                        IsOnlineApproved = miscPayment.IsOnlineApproved,
                        OnlineApprovedBy = miscPayment.OnlineApprovedBy,
                        OnlineApprovedTime = miscPayment.OnlineApprovedTime,
                        IsVoid = miscPayment.IsVoid,
                        VoidBy = miscPayment.VoidBy,
                        VoidTime = miscPayment.VoidTime,
                        OrgBranchName = miscPayment.OrgBranchName,
                        DateCreated = miscPayment.DateCreated,
                        LastModified = miscPayment.LastModified,
                        CreatedBy = miscPayment.CreatedBy,
                        ModifiedBy = miscPayment.ModifiedBy
                    };

                    _db.APMiscellaneousPayments.Attach(entity);
                    _db.APMiscellaneousPayments.Remove(entity);
                    _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted misc. payment " + miscPayment.ApMiscellaneousPaymentId + " " + miscPayment.Remark, Modules.Finance);
                #endregion


            }
            return Json(new[] { miscPayment }.ToDataSourceResult(request, ModelState));
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

            var lines = _db.ApMiscellaneousPaymentLines.Where(x => x.ApMiscellaneousPaymentId == id);
            if (!lines.Any())
            {
                isValid = false;
                errMsg = "Please add lines first";
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
                var period = _db.GLPeriods
                    .Where(x => x.DateFrom <= record.PaymentDate.Date)
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
                            Message = "There are multiple periods defined that contain the payment date.",
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
                        BranchId = record.BranchId,
                        BusinessUnitId = record.BusinessUnitId,
                        InvoiceReferenceDoc = "",
                        DebitSum = debit,
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
        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        public JsonResult Batchtransfer(string id)
        {

            string _response = "SUCCESS";
            int did = int.Parse(id);
            var period = _db.GLPeriods.Find(did);

            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            var untransffered = _db.APMiscellaneousPayments
                .Where(x => x.PaymentDate >= period.DateFrom && x.PaymentDate <= period.DateTo)
                .Where(x => x.IsOnlineApproved && x.IsVoid == false && x.IsOnlineTransferred == false)
                 //.Where(x => mybraches.Contains(x.CreatedAtBranchId))
                 .Where(x => x.CreatedBy == User.Identity.Name)
                .AsQueryable();

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                untransffered = untransffered.Where(x => myBusinessUnits.Contains(x.CreatedAtBusinessUnitId));
            }

            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _response = FinanceTransferHelper.ApMiscPayment(record.ApMiscellaneousPaymentId, "Batch Transfer", User.Identity.Name);
                if (_response != "SUCCESS")
                    ;// Logger.Debug("AP Misc payment transfer ID:" + record.ApMiscellaneousPaymentId + _response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count()} records transffred.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.APMiscellaneousPayments.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
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
