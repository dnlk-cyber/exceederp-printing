using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.TravelRequests
{
    [Authorize]
    public class TravelOrderRequestBudgetController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            var branches = _db.Branch;
            var structure = _db.MainCompanyStructures;

            //var subAccountSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
            //var subAccounts =_db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId);
            //var chartOfAccountSubAccounts = subAccounts;
            //var subaccounts = chartOfAccountSubAccounts.Select(
            //    i => new
            //    {
            //        Name = i.Description,
            //        Code = i.Values
            //    }).AsQueryable();


            //ViewData["SubAccounts"] = subaccounts;

            ViewData["Structure"] = structure;

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = _db.BusinessUnits.ToList();
                ViewData["BusinessUnits"] = businessUnits;
            }

            ViewData["Branches"] = branches;

            var items = _db.ApSetupItems.ToList();
            ViewData["Items"] = items;
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult ApTravelOrderRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrder> aptravelorders = _db.ApTravelOrders.Where(x => x.IsDepartmentOnlineApproved);
            DataSourceResult result = aptravelorders.ToDataSourceResult(request, apTravelOrder => new
            {
                ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                BranchId = apTravelOrder.BranchId,
                BusinessUnitId = apTravelOrder.BusinessUnitId,
                FromBranchId = apTravelOrder.FromBranchId,
                FromBusinessUnitId = apTravelOrder.FromBusinessUnitId,
                CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
                CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
                CreatedAtOrgStructureId = apTravelOrder.CreatedAtOrgStructureId,
                EmployeeId = apTravelOrder.EmployeeId,
                IsForOther = apTravelOrder.IsForOther,
                OrgStructureId = apTravelOrder.OrgStructureId,
                PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                RequestDate = apTravelOrder.RequestDate,
                AdvanceAmount = apTravelOrder.AdvanceAmount,
                TotalDuration = apTravelOrder.TotalDuration,
                TransportReference = apTravelOrder.TransportReference,
                Reference = apTravelOrder.Reference,
                VehicleDistributionID = apTravelOrder.VehicleDistributionID,
                Claimed = apTravelOrder.Claimed,
                Paid = apTravelOrder.Paid,

                //EpmSalary = apTravelOrder.EpmSalary,
                //TotalPayable = apTravelOrder.TotalPayable,
                //AmountClaimed = apTravelOrder.AmountClaimed,

                IsDepartmentOnlineApproved = apTravelOrder.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = apTravelOrder.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = apTravelOrder.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = apTravelOrder.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = apTravelOrder.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = apTravelOrder.DepartmentSendForApprovalTime,

                ForwardedFrom = apTravelOrder.ForwardedFrom,
                ForwardedTo = apTravelOrder.ForwardedTo,
                IsFinalApproval = apTravelOrder.IsFinalApproval,
                CurrentApprovalStep = apTravelOrder.CurrentApprovalStep,

                IsBudgetApproved = apTravelOrder.IsBudgetApproved,
                BudgetApprovedBy = apTravelOrder.BudgetApprovedBy,
                BudgetApprovedTime = apTravelOrder.BudgetApprovedTime,

                IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                Status = apTravelOrder.Status,
                IsOnlineApproved = apTravelOrder.IsOnlineApproved,
                OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
                OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
                IsVoid = apTravelOrder.IsVoid,
                VoidBy = apTravelOrder.VoidBy,
                VoidTime = apTravelOrder.VoidTime,
                IsSendForApproval = apTravelOrder.IsSendForApproval,
                SendForApprovalBy = apTravelOrder.SendForApprovalBy,
                SendForApprovalTime = apTravelOrder.SendForApprovalTime,
                OrgBranchName = apTravelOrder.OrgBranchName,
                DateCreated = apTravelOrder.DateCreated,
                LastModified = apTravelOrder.LastModified,
                CreatedBy = apTravelOrder.CreatedBy,
                ModifiedBy = apTravelOrder.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderRequest_Create([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            

            if (ModelState.IsValid)
            {
               
                var entity = new ApTravelOrder
                {
                    BranchId = apTravelOrder.BranchId,
                    BusinessUnitId = apTravelOrder.BusinessUnitId,
                    FromBranchId = apTravelOrder.FromBranchId,
                    FromBusinessUnitId = apTravelOrder.FromBusinessUnitId,
                    CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
                    EmployeeId = apTravelOrder.EmployeeId,
                    IsForOther = apTravelOrder.IsForOther,
                    OrgStructureId = apTravelOrder.OrgStructureId,                   
                    PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                    RequestDate = apTravelOrder.RequestDate,
                    AdvanceAmount = apTravelOrder.AdvanceAmount,
                    TotalDuration = apTravelOrder.TotalDuration,
                    TransportReference = apTravelOrder.TransportReference,
                    Reference = apTravelOrder.Reference,
                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                    Status = apTravelOrder.Status,
                    Claimed = apTravelOrder.Claimed,
                    Paid = apTravelOrder.Paid,

                    ForwardedFrom = apTravelOrder.ForwardedFrom,
                    ForwardedTo = apTravelOrder.ForwardedTo,
                    IsFinalApproval = apTravelOrder.IsFinalApproval,
                    CurrentApprovalStep = apTravelOrder.CurrentApprovalStep,

                    IsBudgetApproved = apTravelOrder.IsBudgetApproved,
                    BudgetApprovedBy = apTravelOrder.BudgetApprovedBy,
                    BudgetApprovedTime = apTravelOrder.BudgetApprovedTime,

                    IsDepartmentOnlineApproved = apTravelOrder.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = apTravelOrder.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = apTravelOrder.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = apTravelOrder.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = apTravelOrder.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = apTravelOrder.DepartmentSendForApprovalTime,

                    IsOnlineApproved = apTravelOrder.IsOnlineApproved,
                    OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
                    IsVoid = apTravelOrder.IsVoid,
                    VoidBy = apTravelOrder.VoidBy,
                    VoidTime = apTravelOrder.VoidTime,
                    IsSendForApproval = apTravelOrder.IsSendForApproval,
                    SendForApprovalBy = apTravelOrder.SendForApprovalBy,
                    SendForApprovalTime = apTravelOrder.SendForApprovalTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = apTravelOrder.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = apTravelOrder.ModifiedBy
                };

                _db.ApTravelOrders.Add(entity);
                _db.SaveChanges();
                apTravelOrder.ApTravelOrderId = entity.ApTravelOrderId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order", "Success", User.Identity.Name, $"created expense claim {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderRequest_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrder
                {
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                    BranchId = apTravelOrder.BranchId,
                    BusinessUnitId = apTravelOrder.BusinessUnitId,
                    FromBranchId = apTravelOrder.FromBranchId,
                    FromBusinessUnitId = apTravelOrder.FromBusinessUnitId,
                    CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
                    EmployeeId = apTravelOrder.EmployeeId,
                    IsForOther = apTravelOrder.IsForOther,
                    PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                    RequestDate = apTravelOrder.RequestDate,
                    AdvanceAmount = apTravelOrder.AdvanceAmount,
                    TotalDuration = apTravelOrder.TotalDuration,
                    TransportReference = apTravelOrder.TransportReference,
                    Reference = apTravelOrder.Reference,
                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                    Status = apTravelOrder.Status,
                    Claimed = apTravelOrder.Claimed,
                    Paid = apTravelOrder.Paid,

                    ForwardedFrom = apTravelOrder.ForwardedFrom,
                    ForwardedTo = apTravelOrder.ForwardedTo,
                    IsFinalApproval = apTravelOrder.IsFinalApproval,
                    CurrentApprovalStep = apTravelOrder.CurrentApprovalStep,

                    IsBudgetApproved = apTravelOrder.IsBudgetApproved,
                    BudgetApprovedBy = apTravelOrder.BudgetApprovedBy,
                    BudgetApprovedTime = apTravelOrder.BudgetApprovedTime,

                    IsDepartmentOnlineApproved = apTravelOrder.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = apTravelOrder.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = apTravelOrder.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = apTravelOrder.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = apTravelOrder.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = apTravelOrder.DepartmentSendForApprovalTime,

                    IsOnlineApproved = apTravelOrder.IsOnlineApproved,
                    OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
                    IsVoid = apTravelOrder.IsVoid,
                    VoidBy = apTravelOrder.VoidBy,
                    VoidTime = apTravelOrder.VoidTime,
                    IsSendForApproval = apTravelOrder.IsSendForApproval,
                    SendForApprovalBy = apTravelOrder.SendForApprovalBy,
                    SendForApprovalTime = apTravelOrder.SendForApprovalTime,
                    OrgBranchName = apTravelOrder.OrgBranchName,
                    DateCreated = apTravelOrder.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = apTravelOrder.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ApTravelOrders.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order", "Success", User.Identity.Name, $"updated expense claim {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderRequest_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrder
                {
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                    BranchId = apTravelOrder.BranchId,
                    BusinessUnitId = apTravelOrder.BusinessUnitId,
                    FromBranchId = apTravelOrder.FromBranchId,
                    FromBusinessUnitId = apTravelOrder.FromBusinessUnitId,
                    CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
                    EmployeeId = apTravelOrder.EmployeeId,
                    IsForOther = apTravelOrder.IsForOther,
                    PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                    RequestDate = apTravelOrder.RequestDate,
                    AdvanceAmount = apTravelOrder.AdvanceAmount,
                    TotalDuration = apTravelOrder.TotalDuration,
                    Reference = apTravelOrder.Reference,
                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                    Status = apTravelOrder.Status,
                    IsOnlineApproved = apTravelOrder.IsOnlineApproved,
                    OnlineApprovedBy = apTravelOrder.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelOrder.OnlineApprovedTime,
                    IsVoid = apTravelOrder.IsVoid,
                    VoidBy = apTravelOrder.VoidBy,
                    VoidTime = apTravelOrder.VoidTime,
                    IsSendForApproval = apTravelOrder.IsSendForApproval,
                    SendForApprovalBy = apTravelOrder.SendForApprovalBy,
                    SendForApprovalTime = apTravelOrder.SendForApprovalTime,
                    OrgBranchName = apTravelOrder.OrgBranchName,
                    DateCreated = apTravelOrder.DateCreated,
                    LastModified = apTravelOrder.LastModified,
                    CreatedBy = apTravelOrder.CreatedBy,
                    ModifiedBy = apTravelOrder.ModifiedBy
                };

                _db.ApTravelOrders.Attach(entity);
                _db.ApTravelOrders.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }



        //[AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        //public ActionResult ApproveBudget(int id)
        //{
        //    var record = _db.ApTravelOrders.Find(id);
        //    if (record != null && !record.IsBudgetApproved)
        //    {
        //        record.Status = Enum.GetName(typeof(OperationStatuses), 17);
        //        record.IsBudgetApproved = true;
        //        record.BudgetApprovedBy = User.Identity.Name;
        //        record.BudgetApprovedTime = DateTime.Now;
        //        _db.ApTravelOrders.Attach(record);
        //        _db.Entry(record).State = EntityState.Modified;
        //        _db.SaveChanges();
        //        //var entity = new PaymentRequestBudgetValidation
        //        //{
        //        //    PurchasePaymentId = record.PurchasePaymentId,
        //        //    Status = Core.Domain.Common.ApprovalStatuses.Approve,
        //        //    DateCreated = DateTime.Now,
        //        //    CreatedBy = User.Identity.Name,
        //        //};

        //        //_db.PurchasePaymentBudgetValidations.Add(entity);
        //        //_db.SaveChanges();

        //        #region operation log
        //        UserHelper.OperationLog(Request.UserHostAddress, "Travel order budget request approval", "Success", User.Identity.Name, $"approved payment {id} ", Modules.Budget);
        //        #endregion

        //        var formattedData = new
        //        {
        //            Message = "Budget Approved",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    if (record == null)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is not found.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    if (record.IsBudgetApproved)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is already approved, you cannot approve again.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }


        //    return null;
        //}

        [AuthorizeRoles(BudgetRoles.BudgetVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ApTravelOrders.Find(id);
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
                _db.ApTravelOrders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                #region remove related validations

                _db.ApTravelOrderDepartmentValidations.RemoveRange(record.ApTravelOrderDepartmentValidations);
                _db.SaveChanges();

                #endregion

                //copy
                //CopyRecord(id);

                #region approval notification

                #region notify creator
                NotifModels notif1 = new NotifModels
                {
                    UserId = record.CreatedBy,
                    Title = "Void of Travel request at Budget",
                    Message = $"Travel payment document {record.ApTravelOrderId} is void by User: {User.Identity.Name} at {DateTime.Now}",
                    URL = "/Finance/TravelOrderRequest/"
                };

                Notify.NotifyUser(notif1);
                #endregion
                #region notify requestor
                NotifModels notif2 = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(record.EmployeeId),
                    Title = "Void of Travel request at Budget",
                    Message = $"Travel payment document {record.ApTravelOrderId} is void by User: {User.Identity.Name} at {DateTime.Now}",
                    URL = "/Finance/TravelOrderRequest/"
                };

                Notify.NotifyUser(notif2);
                #endregion

                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "PR budget Void", "Success", User.Identity.Name, $"void PR {id} ", Modules.Budget);
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