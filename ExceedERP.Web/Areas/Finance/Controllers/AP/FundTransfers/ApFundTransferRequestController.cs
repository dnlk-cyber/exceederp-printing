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
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.FundTransfers
{
    [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableFundTransferApprover, FinanceConstants.AccountPayableFundTransfer)]
    public class ApFundTransferRequestController : Controller
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

            if (GlobalHelper.IsPayrollModebranchAndBusinessUnit())
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


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableFundTransferApprover, FinanceConstants.AccountPayableFundTransfer)]
        public ActionResult FundTransfer_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<FundTransfer> fundTransfers = _db.FundTransfers
                .Where(x => mybraches.Contains(x.BranchId))
                .Where(x => x.IsDepartmentOnlineApproved);

            if (FinanceConfigurationHelper.IsPRBudgetControlEnabled())
            {
                fundTransfers = fundTransfers.Where(x => x.IsBudgetApproved);
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

        [AuthorizeRoles(FinanceConstants.AccountPayableFundTransfer)]
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

        [AuthorizeRoles(FinanceConstants.AccountPayableFundTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundTransfer_Update([DataSourceRequest]DataSourceRequest request, FundTransfer fundTransfer)
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
                    OrgStuructureId=fundTransfer.OrgStuructureId,
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

        [AuthorizeRoles(FinanceConstants.AccountPayableFundTransfer)]
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

        [AuthorizeRoles(FinanceConstants.AccountPayableFundTransfer)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.FundTransfers.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.FundTransferDetails.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.FundTransfers.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region approval workflow
                //bool isNewRecord = true;
                //var validations = record.ApTravelOrderValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                //if (validations.Any())
                //    isNewRecord = false;
                //decimal docamount = record.AdvanceAmount;
                //int docid = record.ApTravelOrderId;
                //DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", User.Identity.Name, ApprovalDocumentType.ApTravelOrder, isNewRecord);
                #endregion

                var formattedData = new
                {
                    Message = "Successfully Sent.",
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

            if (record.FundTransferDetails.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add  transfer line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
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


        [AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ApTravelOrders.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ApTravelOrderDepartures.Count > 0)
            {
                #region validate claim

                var payments = _db.ApTravelOrders.Where(x => x.ApTravelOrderId == record.ApTravelOrderId)
                    .ToList();
                if (payments.Any())
                {
                    var formattedData1 = new
                    {
                        Message = $"The travel order number {record.ApTravelOrderId} is already applied in Travel Order \n hence cannot be void.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ApTravelOrders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "travel order", "Success", User.Identity.Name, $"void travel order {id} ", Modules.Finance);
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

            if (record.ApTravelOrderDepartures.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add departure lines and try again.",
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
