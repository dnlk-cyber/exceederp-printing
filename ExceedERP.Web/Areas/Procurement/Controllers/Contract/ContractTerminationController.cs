using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Finance;
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
    public class ContractTerminationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, FinanceConstants.AccountPayableView, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult ContractTerminations_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<ContractTermination> contractTerminations = _db.ContractTerminations.Where(x => x.ContractManagementID==id);
            DataSourceResult result = contractTerminations.ToDataSourceResult(request, contractTermination => new {
                ContractManagementID = contractTermination.ContractManagementID,
                ContractTerminationId = contractTermination.ContractTerminationId,
                Date = contractTermination.Date,
                Reason = contractTermination.Reason,
                GiveAwardToNextSupplier = contractTermination.GiveAwardToNextSupplier,
                Remark = contractTermination.Remark,
                Status = contractTermination.Status,
                IsOnlineApproved = contractTermination.IsOnlineApproved,
                OnlineApprovedBy = contractTermination.OnlineApprovedBy,
                OnlineApprovedTime = contractTermination.OnlineApprovedTime,
                IsSendForApproval = contractTermination.IsSendForApproval,
                SendForApprovalBy = contractTermination.SendForApprovalBy,
                SendForApprovalTime = contractTermination.SendForApprovalTime,
                IsVoid = contractTermination.IsVoid,
                VoidBy = contractTermination.VoidBy,
                VoidTime = contractTermination.VoidTime,
                OrgBranchName = contractTermination.OrgBranchName,
                DateCreated = contractTermination.DateCreated,
                LastModified = contractTermination.LastModified,
                CreatedBy = contractTermination.CreatedBy,
                ModifiedBy = contractTermination.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractTerminations_Create(int id,[DataSourceRequest]DataSourceRequest request, ContractTermination contractTermination)
        {
           

            if (ModelState.IsValid)
            {
                var entity = new ContractTermination
                {
                    ContractManagementID = id,
                    Date = contractTermination.Date.Date,
                    Reason = contractTermination.Reason,
                    GiveAwardToNextSupplier = contractTermination.GiveAwardToNextSupplier,
                    Remark = contractTermination.Remark,
                    IsOnlineApproved = contractTermination.IsOnlineApproved,
                    OnlineApprovedBy = contractTermination.OnlineApprovedBy,
                    OnlineApprovedTime = contractTermination.OnlineApprovedTime,
                    IsVoid = contractTermination.IsVoid,
                    VoidBy = contractTermination.VoidBy,
                    VoidTime = contractTermination.VoidTime,
                    OrgBranchName = contractTermination.OrgBranchName,
                    DateCreated = contractTermination.DateCreated,
                    LastModified = contractTermination.LastModified,
                    CreatedBy = contractTermination.CreatedBy,
                    ModifiedBy = contractTermination.ModifiedBy
                };

                _db.ContractTerminations.Add(entity);
                _db.SaveChanges();
                contractTermination.ContractTerminationId = entity.ContractTerminationId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract termination", "Success", User.Identity.Name, $"created contract termination {entity.ContractTerminationId} under record {entity.ContractManagementID} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { contractTermination }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractTerminations_Update([DataSourceRequest]DataSourceRequest request, ContractTermination contractTermination)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new ContractTermination
                {
                    ContractManagementID = contractTermination.ContractManagementID,
                    ContractTerminationId = contractTermination.ContractTerminationId,
                    Date = contractTermination.Date,
                    Reason = contractTermination.Reason,
                    GiveAwardToNextSupplier = contractTermination.GiveAwardToNextSupplier,
                    Remark = contractTermination.Remark,
                    IsOnlineApproved = contractTermination.IsOnlineApproved,
                    OnlineApprovedBy = contractTermination.OnlineApprovedBy,
                    OnlineApprovedTime = contractTermination.OnlineApprovedTime,
                    IsVoid = contractTermination.IsVoid,
                    VoidBy = contractTermination.VoidBy,
                    VoidTime = contractTermination.VoidTime,
                    OrgBranchName = contractTermination.OrgBranchName,
                    DateCreated = contractTermination.DateCreated,
                    LastModified = contractTermination.LastModified,
                    CreatedBy = contractTermination.CreatedBy,
                    ModifiedBy = contractTermination.ModifiedBy
                };

                _db.ContractTerminations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract termination", "Success", User.Identity.Name, $"updated contract termination {entity.ContractTerminationId} under record {entity.ContractManagementID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractTermination }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractTerminations_Destroy([DataSourceRequest]DataSourceRequest request, ContractTermination contractTermination)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractTermination
                {
                    ContractManagementID = contractTermination.ContractManagementID,
                    ContractTerminationId = contractTermination.ContractTerminationId,
                    Date = contractTermination.Date,
                    GiveAwardToNextSupplier = contractTermination.GiveAwardToNextSupplier,
                    Remark = contractTermination.Remark,
                    IsOnlineApproved = contractTermination.IsOnlineApproved,
                    OnlineApprovedBy = contractTermination.OnlineApprovedBy,
                    OnlineApprovedTime = contractTermination.OnlineApprovedTime,
                    IsVoid = contractTermination.IsVoid,
                    VoidBy = contractTermination.VoidBy,
                    VoidTime = contractTermination.VoidTime,
                    OrgBranchName = contractTermination.OrgBranchName,
                    DateCreated = contractTermination.DateCreated,
                    LastModified = contractTermination.LastModified,
                    CreatedBy = contractTermination.CreatedBy,
                    ModifiedBy = contractTermination.ModifiedBy
                };

                _db.ContractTerminations.Attach(entity);
                _db.ContractTerminations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { contractTermination }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ContractTerminations.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ContractTerminations.Attach(record);
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

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
