using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{
    public class ContractPaymentController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, FinanceConstants.AccountPayableView, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult ContractPayments_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<ContractPayment> contractPayments = _db.ContractPayments.Where(x => x.ContractManagementID==id);
            DataSourceResult result = contractPayments.ToDataSourceResult(request, contractPayment => new {
                ContractManagementID = contractPayment.ContractManagementID,
                ContractPaymentId = contractPayment.ContractPaymentId,
                Date = contractPayment.Date,
                SupplierInvoice = contractPayment.SupplierInvoice,
                WithOutGRN = contractPayment.WithOutGRN,
                PaymentAmount = contractPayment.PaymentAmount,
                ApplyPenality = contractPayment.ApplyPenality,
                PenalityAmount = contractPayment.PenalityAmount,
                NetPayable = contractPayment.NetPayable,
                Remark = contractPayment.Remark,
                Status = contractPayment.Status,
                IsOnlineApproved = contractPayment.IsOnlineApproved,
                OnlineApprovedBy = contractPayment.OnlineApprovedBy,
                OnlineApprovedTime = contractPayment.OnlineApprovedTime,
                IsSendForApproval = contractPayment.IsSendForApproval,
                SendForApprovalBy = contractPayment.SendForApprovalBy,
                SendForApprovalTime = contractPayment.SendForApprovalTime,
                IsVoid = contractPayment.IsVoid,
                VoidBy = contractPayment.VoidBy,
                VoidTime = contractPayment.VoidTime,
                OrgBranchName = contractPayment.OrgBranchName,
                DateCreated = contractPayment.DateCreated,
                LastModified = contractPayment.LastModified,
                CreatedBy = contractPayment.CreatedBy,
                ModifiedBy = contractPayment.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPayments_Create(int id,[DataSourceRequest]DataSourceRequest request, ContractPayment contractPayment)
        {
            #region validation 

            var contract = _db.ContractManagements.Find(id);

            if (contract != null && contract.ContractStatus==ContractStatus.Terminated)
                ModelState.AddModelError("terminated", "The contract is terminated,hence can't request payment.");
            if (contractPayment.PenalityAmount >= contractPayment.PaymentAmount)
                ModelState.AddModelError("penaltyoverpayment", "The penalty amount is more than the payment.");
            if (contractPayment.ApplyPenality)
                contractPayment.NetPayable = contractPayment.PaymentAmount - contractPayment.PenalityAmount;

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ContractPayment
                {
                    ContractManagementID = id,
                    Date = contractPayment.Date.Date,
                    SupplierInvoice = contractPayment.SupplierInvoice,
                    WithOutGRN = contractPayment.WithOutGRN,
                    PaymentAmount = contractPayment.PaymentAmount,
                    ApplyPenality = contractPayment.ApplyPenality,
                    PenalityAmount = contractPayment.PenalityAmount,
                    NetPayable = contractPayment.NetPayable,
                    Remark = contractPayment.Remark,
                    IsOnlineApproved = contractPayment.IsOnlineApproved,
                    OnlineApprovedBy = contractPayment.OnlineApprovedBy,
                    OnlineApprovedTime = contractPayment.OnlineApprovedTime,
                    IsVoid = contractPayment.IsVoid,
                    VoidBy = contractPayment.VoidBy,
                    VoidTime = contractPayment.VoidTime,
                    OrgBranchName = contractPayment.OrgBranchName,
                    DateCreated = contractPayment.DateCreated,
                    LastModified = contractPayment.LastModified,
                    CreatedBy = contractPayment.CreatedBy,
                    ModifiedBy = contractPayment.ModifiedBy
                };

                _db.ContractPayments.Add(entity);
                _db.SaveChanges();
                contractPayment.ContractPaymentId = entity.ContractPaymentId;

                GetGrnList(id, entity.ContractPaymentId);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract payment", "Success", User.Identity.Name, $"created contract payment {entity.PaymentAmount} under record {entity.ContractManagementID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractPayment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPayments_Update([DataSourceRequest]DataSourceRequest request, ContractPayment contractPayment)
        {
            #region validation 
            var contract = _db.ContractManagements.Find(contractPayment.ContractManagementID);

            if (contract != null && contract.ContractStatus == ContractStatus.Terminated)
                ModelState.AddModelError("terminated", "The contract is terminated,hence can't request payment.");

            if (contractPayment.PenalityAmount>= contractPayment.PaymentAmount)
                ModelState.AddModelError("penaltyoverpayment","The penalty amount is more than the payment.");
            if (contractPayment.ApplyPenality)
                contractPayment.NetPayable = contractPayment.PaymentAmount - contractPayment.PenalityAmount;

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ContractPayment
                {
                    ContractManagementID = contractPayment.ContractManagementID,
                    ContractPaymentId = contractPayment.ContractPaymentId,
                    Date = contractPayment.Date,
                    SupplierInvoice = contractPayment.SupplierInvoice,
                    WithOutGRN = contractPayment.WithOutGRN,
                    PaymentAmount = contractPayment.PaymentAmount,
                    ApplyPenality = contractPayment.ApplyPenality,
                    PenalityAmount = contractPayment.PenalityAmount,
                    NetPayable = contractPayment.NetPayable,
                    Remark = contractPayment.Remark,
                    IsOnlineApproved = contractPayment.IsOnlineApproved,
                    OnlineApprovedBy = contractPayment.OnlineApprovedBy,
                    OnlineApprovedTime = contractPayment.OnlineApprovedTime,
                    IsVoid = contractPayment.IsVoid,
                    VoidBy = contractPayment.VoidBy,
                    VoidTime = contractPayment.VoidTime,
                    OrgBranchName = contractPayment.OrgBranchName,
                    DateCreated = contractPayment.DateCreated,
                    LastModified = contractPayment.LastModified,
                    CreatedBy = contractPayment.CreatedBy,
                    ModifiedBy = contractPayment.ModifiedBy
                };

                _db.ContractPayments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract payment", "Success", User.Identity.Name, $"updated contract payment {entity.PaymentAmount} under record {entity.ContractManagementID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractPayment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPayments_Destroy([DataSourceRequest]DataSourceRequest request, ContractPayment contractPayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractPayment
                {
                    ContractManagementID = contractPayment.ContractManagementID,
                    ContractPaymentId = contractPayment.ContractPaymentId,
                    Date = contractPayment.Date,
                    WithOutGRN = contractPayment.WithOutGRN,
                    PaymentAmount = contractPayment.PaymentAmount,
                    ApplyPenality = contractPayment.ApplyPenality,
                    Remark = contractPayment.Remark,
                    IsOnlineApproved = contractPayment.IsOnlineApproved,
                    OnlineApprovedBy = contractPayment.OnlineApprovedBy,
                    OnlineApprovedTime = contractPayment.OnlineApprovedTime,
                    IsVoid = contractPayment.IsVoid,
                    VoidBy = contractPayment.VoidBy,
                    VoidTime = contractPayment.VoidTime,
                    OrgBranchName = contractPayment.OrgBranchName,
                    DateCreated = contractPayment.DateCreated,
                    LastModified = contractPayment.LastModified,
                    CreatedBy = contractPayment.CreatedBy,
                    ModifiedBy = contractPayment.ModifiedBy
                };

                _db.ContractPayments.Attach(entity);
                _db.ContractPayments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { contractPayment }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult SendForApproval(int id)
        {
            var record = _db.ContractPayments.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ContractPayments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
           
            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
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

        private void GetGrnList(int contractid,int paymentid)
        {
           
            var contract = _db.ContractManagements.Find(contractid);
            var receiveType = _db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.Tender);
            if (contract != null && contract.ProcurementRefNo != 0)
            {
                #region grn stock 

                var stockReceives = _db.GoodReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                    .Where(x => x.ReceiveRef == contract.ProcurementRefNo.ToString()).ToList();

                foreach (var goodReceive in stockReceives)
                {
                    #region grn 

                    var grn = new ContractPaymentGrn
                    {
                        ContractPaymentId = paymentid,
                        GrnNo = goodReceive.GoodReceiveId,
                        ItemType = PurchaseItemType.Stock,
                        DateCreated = DateTime.Now,
                        LastModified = goodReceive.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = goodReceive.ModifiedBy
                    };
                    _db.ContractPaymentGrns.Add(grn);
                    _db.SaveChanges();

                    #endregion
                }

                #endregion

                #region grn service 

                var serviceReceives = _db.ServiceReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                    .Where(x => x.ReceiveRef == contract.ProcurementRefNo.ToString()).ToList();

                foreach (var serviceReceive in serviceReceives)
                {
                    #region grn service header

                    var grn = new ContractPaymentGrn
                    {
                        ContractPaymentId = paymentid,
                        ItemType = PurchaseItemType.Service,
                        DateCreated = DateTime.Now,
                        LastModified = serviceReceive.LastModified,
                        CreatedBy = User.Identity.Name,
                    };
                    _db.ContractPaymentGrns.Add(grn);
                    _db.SaveChanges();

                    #endregion
                }

                #endregion

                #region grn fixed asset 

                var fixedAssetReceives = _db.FixedAssetReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                    .Where(x => x.ReceiveRef == contract.ProcurementRefNo.ToString()).ToList();

                foreach (var fixedAssetReceive in fixedAssetReceives)
                {
                    #region grn fixed asset 

                    var grn = new ContractPaymentGrn
                    {
                        ContractPaymentId = paymentid,
                        ItemType = PurchaseItemType.FixedAsset,
                        DateCreated = DateTime.Now,
                        LastModified = fixedAssetReceive.LastModified,
                        CreatedBy = User.Identity.Name,
                    };
                    _db.ContractPaymentGrns.Add(grn);
                    _db.SaveChanges();

                    #endregion
                }

                #endregion
            }

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
