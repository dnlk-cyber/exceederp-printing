using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{
    public class ContractRenewalController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, FinanceConstants.AccountPayableView, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult ContractRenewals_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<ContractRenewal> contractrenewals = db.ContractRenewals.Where(x => x.ContractManagementID==id);
            DataSourceResult result = contractrenewals.ToDataSourceResult(request, contractRenewal => new {
                ContractManagementID = contractRenewal.ContractManagementID,
                ContractRenewalId = contractRenewal.ContractRenewalId,
                SigningDate = contractRenewal.SigningDate,
                DaysAdded = contractRenewal.DaysAdded,
                AdditionalPayment = contractRenewal.AdditionalPayment,
                Remark = contractRenewal.Remark,
                Status = contractRenewal.Status,
                IsOnlineApproved = contractRenewal.IsOnlineApproved,
                OnlineApprovedBy = contractRenewal.OnlineApprovedBy,
                OnlineApprovedTime = contractRenewal.OnlineApprovedTime,
                IsSendForApproval = contractRenewal.IsSendForApproval,
                SendForApprovalBy = contractRenewal.SendForApprovalBy,
                SendForApprovalTime = contractRenewal.SendForApprovalTime,
                IsVoid = contractRenewal.IsVoid,
                VoidBy = contractRenewal.VoidBy,
                VoidTime = contractRenewal.VoidTime,
                OrgBranchName = contractRenewal.OrgBranchName,
                DateCreated = contractRenewal.DateCreated,
                LastModified = contractRenewal.LastModified,
                CreatedBy = contractRenewal.CreatedBy,
                ModifiedBy = contractRenewal.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractRenewals_Create(int id,[DataSourceRequest]DataSourceRequest request, ContractRenewal contractRenewal)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractRenewal
                {
                    ContractManagementID = id,
                    SigningDate = contractRenewal.SigningDate.Date,
                    DaysAdded = contractRenewal.DaysAdded,
                    AdditionalPayment = contractRenewal.AdditionalPayment,
                    Remark = contractRenewal.Remark,
                    IsOnlineApproved = contractRenewal.IsOnlineApproved,
                    OnlineApprovedBy = contractRenewal.OnlineApprovedBy,
                    OnlineApprovedTime = contractRenewal.OnlineApprovedTime,
                    IsVoid = contractRenewal.IsVoid,
                    VoidBy = contractRenewal.VoidBy,
                    VoidTime = contractRenewal.VoidTime,
                    OrgBranchName = contractRenewal.OrgBranchName,
                    DateCreated = contractRenewal.DateCreated,
                    LastModified = contractRenewal.LastModified,
                    CreatedBy = contractRenewal.CreatedBy,
                    ModifiedBy = contractRenewal.ModifiedBy
                };

                db.ContractRenewals.Add(entity);
                db.SaveChanges();
                contractRenewal.ContractRenewalId = entity.ContractRenewalId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract renewal", "Success", User.Identity.Name, $"created contract renewal {entity.ContractRenewalId} under record {entity.ContractManagementID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractRenewal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractRenewals_Update([DataSourceRequest]DataSourceRequest request, ContractRenewal contractRenewal)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractRenewal
                {
                    ContractManagementID = contractRenewal.ContractManagementID,
                    ContractRenewalId = contractRenewal.ContractRenewalId,
                    SigningDate = contractRenewal.SigningDate,
                    DaysAdded = contractRenewal.DaysAdded,
                    AdditionalPayment = contractRenewal.AdditionalPayment,
                    Remark = contractRenewal.Remark,
                    IsOnlineApproved = contractRenewal.IsOnlineApproved,
                    OnlineApprovedBy = contractRenewal.OnlineApprovedBy,
                    OnlineApprovedTime = contractRenewal.OnlineApprovedTime,
                    IsVoid = contractRenewal.IsVoid,
                    VoidBy = contractRenewal.VoidBy,
                    VoidTime = contractRenewal.VoidTime,
                    OrgBranchName = contractRenewal.OrgBranchName,
                    DateCreated = contractRenewal.DateCreated,
                    LastModified = contractRenewal.LastModified,
                    CreatedBy = contractRenewal.CreatedBy,
                    ModifiedBy = contractRenewal.ModifiedBy
                };

                db.ContractRenewals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract renewal", "Success", User.Identity.Name, $"updated contract renewal {entity.ContractRenewalId} under record {entity.ContractManagementID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractRenewal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractRenewals_Destroy([DataSourceRequest]DataSourceRequest request, ContractRenewal contractRenewal)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractRenewal
                {
                    ContractManagementID = contractRenewal.ContractManagementID,
                    ContractRenewalId = contractRenewal.ContractRenewalId,
                    SigningDate = contractRenewal.SigningDate,
                    DaysAdded = contractRenewal.DaysAdded,
                    AdditionalPayment = contractRenewal.AdditionalPayment,
                    Remark = contractRenewal.Remark,
                    IsOnlineApproved = contractRenewal.IsOnlineApproved,
                    OnlineApprovedBy = contractRenewal.OnlineApprovedBy,
                    OnlineApprovedTime = contractRenewal.OnlineApprovedTime,
                    IsVoid = contractRenewal.IsVoid,
                    VoidBy = contractRenewal.VoidBy,
                    VoidTime = contractRenewal.VoidTime,
                    OrgBranchName = contractRenewal.OrgBranchName,
                    DateCreated = contractRenewal.DateCreated,
                    LastModified = contractRenewal.LastModified,
                    CreatedBy = contractRenewal.CreatedBy,
                    ModifiedBy = contractRenewal.ModifiedBy
                };

                db.ContractRenewals.Attach(entity);
                db.ContractRenewals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { contractRenewal }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult SendForApproval(int id)
        {
            var record = db.ContractRenewals.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.ContractRenewals.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
