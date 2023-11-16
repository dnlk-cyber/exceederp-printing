using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare.FundTransfers
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class FundTransferRequestController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        public ActionResult Index()
        {
            
            PopulateViewData();
            UpdateData();

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

        private void UpdateData()
        {
            var list = _db.FundTransfers.Where(x => x.CreatedAtOrgStructureId == 0).ToList();

            foreach (var record in list)
            {
                record.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(record.CreatedBy);
                _db.FundTransfers.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

            }
        }

        public ActionResult FundTransfer_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            IQueryable<FundTransfer> fundTransfers = _db.FundTransfers;
            if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService)
                 && System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HRDepartmentHead))
            {

                fundTransfers = fundTransfers.Where(x => userworkunit.Contains(x.CreatedAtOrgStructureId)
                || x.CreatedAtOrgStructureId == 0//if they are not vis
                || x.ForwardedTo == User.Identity.Name
                || x.ForwardedFrom == User.Identity.Name
                || x.CreatedBy == User.Identity.Name);

            }
            else if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService))
            {
                fundTransfers = fundTransfers.Where(x => x.CreatedBy == User.Identity.Name);
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
                Status =fundTransfer.Status,
                Remark =fundTransfer.Remark,
                IsVoid =fundTransfer.IsVoid,
                VoidBy =fundTransfer.VoidBy,
                VoidTime =fundTransfer.VoidTime,

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

                IsOnlineApproved =fundTransfer.IsOnlineApproved,
                OnlineApprovedBy =fundTransfer.OnlineApprovedBy,
                OnlineApprovedTime =fundTransfer.OnlineApprovedTime,
                IsSendForApproval =fundTransfer.IsSendForApproval,
                SendForApprovalBy =fundTransfer.SendForApprovalBy,
                SendForApprovalTime =fundTransfer.SendForApprovalTime,
                IsOnlineTransferred =fundTransfer.IsOnlineTransferred,
                IsOnlineTransferredBy =fundTransfer.IsOnlineTransferredBy,
                OnlineTransferredTime =fundTransfer.OnlineTransferredTime,
                OrgBranchName = fundTransfer.OrgBranchName,

                DateCreated =fundTransfer.DateCreated,
                CreatedBy =fundTransfer.CreatedBy,
                LastModified =fundTransfer.LastModified,
                ModifiedBy =fundTransfer.ModifiedBy,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransfer_Create([DataSourceRequest]DataSourceRequest request, FundTransfer fundTransfer)
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
                    fundTransfer.BranchId = placemnt.BranchId;
                    fundTransfer.BusinessUnitId = placemnt.BusinessUnitId;
                    fundTransfer.OrgStuructureId = placemnt.OrgStructureId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }



            fundTransfer.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            fundTransfer.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            fundTransfer.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);

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
                    CreatedAtOrgStructureId = fundTransfer.CreatedAtOrgStructureId,
                    BankAccountId = fundTransfer.BankAccountId,
                    Paid = fundTransfer.Paid,
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
        public ActionResult FundTransfer_Update([DataSourceRequest]DataSourceRequest request, FundTransfer fundTransfer)
        {


            #region data
            if (fundTransfer.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }

            if (fundTransfer.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (fundTransfer.EmployeeId != 0)
            {
                var placemnt = _db.Placements.Where(x => x.EmployeeId == fundTransfer.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
                if (placemnt != null)
                {
                    fundTransfer.BranchId = placemnt.BranchId;
                    fundTransfer.BusinessUnitId = placemnt.BusinessUnitId;
                    fundTransfer.OrgStuructureId = placemnt.OrgStructureId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }



            #endregion
            if (ModelState.IsValid)
            {
                var entity = new FundTransfer
                {
                    EmployeeId = fundTransfer.EmployeeId,
                    OrgStuructureId=fundTransfer.OrgStuructureId,
                    BranchId = fundTransfer.BranchId,
                    BusinessUnitId = fundTransfer.BusinessUnitId,
                    FundTransferId = fundTransfer.FundTransferId,
                    FromBranchId = fundTransfer.FromBranchId,
                    FromBusinessUnitId = fundTransfer.FromBusinessUnitId,
                    CreatedAtBranchId = fundTransfer.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = fundTransfer.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = fundTransfer.CreatedAtOrgStructureId,
                    BankAccountId = fundTransfer.BankAccountId,
                    Paid = fundTransfer.Paid,
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
                    LastModified =DateTime.Now,
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
        public ActionResult FundTransfer_Destroy([DataSourceRequest]DataSourceRequest request, FundTransfer fundTransfer)
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
                    CreatedAtOrgStructureId = fundTransfer.CreatedAtOrgStructureId,
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
       
        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = _db.FundTransfers.Find(id);
            string errMsg = "";
           
            if (record != null && record.IsDepartmentOnlineApproved)
            {
                errMsg = "You cannot manipulate this record,it is already approved";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsDepartmentSendForApproval)
            {
                errMsg = "This record is already sent for department approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var lines = _db.FundTransferDetails.Where(x => x.FundTransferId == id);
            if (!lines.Any())
            {
                errMsg = "Please add fund transfer line items.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

           
            if (record != null && !record.IsDepartmentOnlineApproved && !record.IsDepartmentSendForApproval)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 16);
                record.IsDepartmentSendForApproval = true;
                record.DepartmentSendForApprovalBy = User.Identity.Name;
                record.DepartmentSendForApprovalTime = DateTime.Now;
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region approval workflow

                bool isNewRecord = true;
                var validations = record.FundTransferDepartmentValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;

                decimal docamount = 0;
                var requestlines = _db.FundTransferDetails.Where(x => x.FundTransferId == id);
                docamount = requestlines.Any() ? requestlines.Sum(x => x.Approved) : 0;
                int docid = record.FundTransferId;
                int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApFundTransferSelfCare, isNewRecord,step);
                #endregion

                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
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
