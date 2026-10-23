using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Helpers.Finance;
using System.Collections.Generic;

namespace ExceedERP.Web.Areas.Budget.Controllers.FundTransfers
{
    [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
    public class FundTransferRequestBudgetController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        public ActionResult Index()
        {

            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {
            var employeName = _db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;

            var tax = _db.GLTaxRates.ToList();
            ViewData["GLTaxRate"] = tax;

            var branch = _db.Branch.ToList();
            ViewData["Branch"] = branch;

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = _db.BusinessUnits.ToList();
                ViewData["BusinessUnits"] = businessUnits;
            }

            var structure = _db.MainCompanyStructures.ToList();
            ViewData["OrgStructure"] = structure;

            var accounts = _db.BankAccounts.Select(c => new
            {
                Code = c.BankAccountId,
                Name = c.AccountNumber + "-" + c.AccountName
            });
            ViewData["BankAccounts"] = accounts;

        }



        public ActionResult FundTransfer_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<FundTransfer> fundTransfers = _db.FundTransfers
                .Where(x => mybraches.Contains(x.BranchId) && x.IsDepartmentOnlineApproved);

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                fundTransfers = fundTransfers.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }

            DataSourceResult result = fundTransfers.ToDataSourceResult(request, fundTransfer => new
            {
                FundTransferId = fundTransfer.FundTransferId,
                ReasonOfTransfer = fundTransfer.ReasonOfTransfer,
                EmployeeId = fundTransfer.EmployeeId,
                OrgStuructureId = fundTransfer.OrgStuructureId,
                BranchId = fundTransfer.BranchId,
                BusinessUnitId = fundTransfer.BusinessUnitId,

                FromBranchId = fundTransfer.FromBranchId,
                FromBusinessUnitId = fundTransfer.FromBusinessUnitId,
                CreatedAtBranchId = fundTransfer.CreatedAtBranchId,
                CreatedAtBusinessUnitId = fundTransfer.CreatedAtBusinessUnitId,
                CreatedAtOrgStructureId = fundTransfer.CreatedAtOrgStructureId,
                BankAccountId = fundTransfer.BankAccountId,
                RequestDate = fundTransfer.RequestDate,
                Paid = fundTransfer.Paid,
                PaymentAmount = fundTransfer.PaymentAmount,
                AmountDue = fundTransfer.AmountDue,
                Status = fundTransfer.Status,
                Remark = fundTransfer.Remark,
                IsVoid = fundTransfer.IsVoid,
                VoidBy = fundTransfer.VoidBy,
                VoidTime = fundTransfer.VoidTime,

                IsBudgetApproved = fundTransfer.IsBudgetApproved,
                BudgetApprovedBy = fundTransfer.BudgetApprovedBy,
                BudgetApprovedTime = fundTransfer.BudgetApprovedTime,

                IsDepartmentOnlineApproved = fundTransfer.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = fundTransfer.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = fundTransfer.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = fundTransfer.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = fundTransfer.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = fundTransfer.DepartmentSendForApprovalTime,

                ForwardedFrom = fundTransfer.ForwardedFrom,
                ForwardedTo = fundTransfer.ForwardedTo,
                IsFinalApproval = fundTransfer.IsFinalApproval,
                CurrentApprovalStep = fundTransfer.CurrentApprovalStep,

                IsOnlineApproved = fundTransfer.IsOnlineApproved,
                OnlineApprovedBy = fundTransfer.OnlineApprovedBy,
                OnlineApprovedTime = fundTransfer.OnlineApprovedTime,
                IsSendForApproval = fundTransfer.IsSendForApproval,
                SendForApprovalBy = fundTransfer.SendForApprovalBy,
                SendForApprovalTime = fundTransfer.SendForApprovalTime,
                IsOnlineTransferred = fundTransfer.IsOnlineTransferred,
                IsOnlineTransferredBy = fundTransfer.IsOnlineTransferredBy,
                OnlineTransferredTime = fundTransfer.OnlineTransferredTime,
                OrgBranchName = fundTransfer.OrgBranchName,

                DateCreated = fundTransfer.DateCreated,
                CreatedBy = fundTransfer.CreatedBy,
                LastModified = fundTransfer.LastModified,
                ModifiedBy = fundTransfer.ModifiedBy,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransfer_Create([DataSourceRequest] DataSourceRequest request, FundTransfer fundTransfer)
        {

            #region data
            fundTransfer.CurrentApprovalStep = 0;

            if (fundTransfer.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (fundTransfer.EmployeeId != 0)
            {
                var placemnt = _db.Placements.Where(x => x.EmployeeId == fundTransfer.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
                if (placemnt != null)
                {
                    fundTransfer.CreatedAtBranchId = placemnt.BranchId;
                    fundTransfer.CreatedAtBusinessUnitId = placemnt.BusinessUnitId;
                    fundTransfer.OrgStuructureId = placemnt.OrgStructureId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }



            fundTransfer.BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            fundTransfer.BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            //fundTransfer.OrgStuructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);

            #endregion

            if (ModelState.IsValid)
            {

                var entity = new FundTransfer
                {
                    EmployeeId = fundTransfer.EmployeeId,
                    BranchId = fundTransfer.BranchId,
                    BusinessUnitId = fundTransfer.BusinessUnitId,
                    OrgStuructureId = fundTransfer.OrgStuructureId,
                    FromBranchId = fundTransfer.FromBranchId,
                    FromBusinessUnitId = fundTransfer.FromBusinessUnitId,
                    CreatedAtBranchId = fundTransfer.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = fundTransfer.CreatedAtBusinessUnitId,
                    BankAccountId = fundTransfer.BankAccountId,
                    Paid = fundTransfer.Paid,
                    PaymentAmount = fundTransfer.PaymentAmount,
                    AmountDue = fundTransfer.AmountDue,
                    ReasonOfTransfer = fundTransfer.ReasonOfTransfer,
                    RequestDate = fundTransfer.RequestDate,
                    Remark = fundTransfer.Remark,
                    IsVoid = fundTransfer.IsVoid,
                    VoidBy = fundTransfer.VoidBy,
                    VoidTime = fundTransfer.VoidTime,
                    IsOnlineApproved = fundTransfer.IsOnlineApproved,
                    OnlineApprovedBy = fundTransfer.OnlineApprovedBy,
                    OnlineApprovedTime = fundTransfer.OnlineApprovedTime,
                    IsSendForApproval = fundTransfer.IsSendForApproval,
                    SendForApprovalBy = fundTransfer.SendForApprovalBy,
                    SendForApprovalTime = fundTransfer.SendForApprovalTime,
                    IsOnlineTransferred = fundTransfer.IsOnlineTransferred,
                    IsOnlineTransferredBy = fundTransfer.IsOnlineTransferredBy,
                    OnlineTransferredTime = fundTransfer.OnlineTransferredTime,

                    IsBudgetApproved = fundTransfer.IsBudgetApproved,
                    BudgetApprovedBy = fundTransfer.BudgetApprovedBy,
                    BudgetApprovedTime = fundTransfer.BudgetApprovedTime,

                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = fundTransfer.IsFinalApproval,
                    CurrentApprovalStep = fundTransfer.CurrentApprovalStep,

                    IsDepartmentOnlineApproved = fundTransfer.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = fundTransfer.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = fundTransfer.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = fundTransfer.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = fundTransfer.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = fundTransfer.DepartmentSendForApprovalTime,

                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = fundTransfer.LastModified,
                    ModifiedBy = fundTransfer.ModifiedBy,
                };

                _db.FundTransfers.Add(entity);
                _db.SaveChanges();
                fundTransfer.FundTransferId = entity.FundTransferId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care fund transfer", "Success", User.Identity.Name, $"created fund transfer {entity.FundTransferId} ", Modules.Finance);
                #endregion
            }



            return Json(new[] { fundTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransfer_Update([DataSourceRequest] DataSourceRequest request, FundTransfer fundTransfer)
        {


            #region data
            if (fundTransfer.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }

            if (fundTransfer.EmployeeId != 0)
            {
                var placemnt = _db.Placements.Where(x => x.EmployeeId == fundTransfer.EmployeeId).Where(x => x.isCurrent).FirstOrDefault();
                if (placemnt != null)
                {
                    fundTransfer.CreatedAtBranchId = placemnt.BranchId;
                    fundTransfer.CreatedAtBusinessUnitId = placemnt.BusinessUnitId;
                    fundTransfer.OrgStuructureId = placemnt.OrgStructureId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has no current placemnt");
                }
            }



            #endregion
            if (ModelState.IsValid)
            {
                var entity = new FundTransfer
                {
                    EmployeeId = fundTransfer.EmployeeId,
                    OrgStuructureId = fundTransfer.OrgStuructureId,
                    BranchId = fundTransfer.BranchId,
                    BusinessUnitId = fundTransfer.BusinessUnitId,
                    FundTransferId = fundTransfer.FundTransferId,
                    FromBranchId = fundTransfer.FromBranchId,
                    FromBusinessUnitId = fundTransfer.FromBusinessUnitId,
                    CreatedAtBranchId = fundTransfer.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = fundTransfer.CreatedAtBusinessUnitId,
                    BankAccountId = fundTransfer.BankAccountId,
                    Paid = fundTransfer.Paid,
                    PaymentAmount = fundTransfer.PaymentAmount,
                    AmountDue = fundTransfer.AmountDue,
                    ReasonOfTransfer = fundTransfer.ReasonOfTransfer,
                    RequestDate = fundTransfer.RequestDate,

                    IsBudgetApproved = fundTransfer.IsBudgetApproved,
                    BudgetApprovedBy = fundTransfer.BudgetApprovedBy,
                    BudgetApprovedTime = fundTransfer.BudgetApprovedTime,

                    ForwardedFrom = fundTransfer.ForwardedFrom,
                    ForwardedTo = fundTransfer.ForwardedTo,
                    IsFinalApproval = fundTransfer.IsFinalApproval,
                    CurrentApprovalStep = fundTransfer.CurrentApprovalStep,


                    IsDepartmentOnlineApproved = fundTransfer.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = fundTransfer.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = fundTransfer.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = fundTransfer.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = fundTransfer.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = fundTransfer.DepartmentSendForApprovalTime,

                    IsOnlineApproved = fundTransfer.IsOnlineApproved,
                    OnlineApprovedBy = fundTransfer.OnlineApprovedBy,
                    OnlineApprovedTime = fundTransfer.OnlineApprovedTime,
                    IsSendForApproval = fundTransfer.IsSendForApproval,
                    SendForApprovalBy = fundTransfer.SendForApprovalBy,
                    SendForApprovalTime = fundTransfer.SendForApprovalTime,
                    IsOnlineTransferred = fundTransfer.IsOnlineTransferred,
                    IsOnlineTransferredBy = fundTransfer.IsOnlineTransferredBy,
                    OnlineTransferredTime = fundTransfer.OnlineTransferredTime,
                    OrgBranchName = fundTransfer.OrgBranchName,

                    Remark = fundTransfer.Remark,
                    IsVoid = fundTransfer.IsVoid,
                    VoidBy = fundTransfer.VoidBy,
                    VoidTime = fundTransfer.VoidTime,


                    DateCreated = fundTransfer.DateCreated,
                    CreatedBy = fundTransfer.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.FundTransfers.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care fund transfer", "Success", User.Identity.Name, $"updated fund transfer {entity.FundTransferId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { fundTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransfer_Destroy([DataSourceRequest] DataSourceRequest request, FundTransfer fundTransfer)
        {

            if (ModelState.IsValid)
            {
                var entity = new FundTransfer
                {
                    EmployeeId = fundTransfer.EmployeeId,
                    OrgStuructureId = fundTransfer.OrgStuructureId,
                    BranchId = fundTransfer.BranchId,
                    BusinessUnitId = fundTransfer.BusinessUnitId,
                    FundTransferId = fundTransfer.FundTransferId,
                    FromBranchId = fundTransfer.FromBranchId,
                    FromBusinessUnitId = fundTransfer.FromBusinessUnitId,
                    CreatedAtBranchId = fundTransfer.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = fundTransfer.CreatedAtBusinessUnitId,
                    BankAccountId = fundTransfer.BankAccountId,
                    Paid = fundTransfer.Paid,
                    ReasonOfTransfer = fundTransfer.ReasonOfTransfer,
                    RequestDate = fundTransfer.RequestDate,
                    Remark = fundTransfer.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted fund transfer ID  " + fundTransfer.FundTransferId);
                #endregion
                _db.FundTransfers.Attach(entity);
                _db.FundTransfers.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { fundTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        public ActionResult ApproveBudget(int id)
        {
            var record = _db.FundTransfers.Find(id);
            if (record != null && !record.IsBudgetApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 17);
                record.IsBudgetApproved = true;
                record.BudgetApprovedBy = User.Identity.Name;
                record.BudgetApprovedTime = DateTime.Now;
                _db.FundTransfers.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                //_db.SaveChanges();
                var entity = new FundTransferBudgetValidation
                {
                    FundTransferId = record.FundTransferId,
                    Status = Core.Domain.Common.ApprovalStatuses.Approve,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                _db.FundTransferBudgetValidations.Add(entity);
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment approval", "Success", User.Identity.Name, $"approved fund transfer {entity.FundTransferId} ", Modules.Budget);
                #endregion

                var formattedData = new
                {
                    Message = "Budget Approved",
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

            if (record.IsBudgetApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved, you cannot approve again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        [AuthorizeRoles(BudgetRoles.BudgetVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.FundTransfers.Find(id);
            if (record != null && record.IsBudgetApproved && !record.IsOnlineApproved)
            {

                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsDepartmentOnlineApproved = false;
                record.IsDepartmentSendForApproval = false;
                record.IsBudgetApproved = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.FundTransfers.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment budget Void", "Success", User.Identity.Name, $"void Purchase Payment {id} ", Modules.Budget);
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

            if (!record.IsBudgetApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is approved at finance, it should be void there first.",
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
