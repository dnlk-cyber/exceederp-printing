using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    public class ApTravelOrderRequestController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        public static int EmployeeId { get; set; }
        public static bool IsForOther { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            var branches = db.Branch;
            var structure = db.MainCompanyStructures;
            var chartOfAccountSubAccounts = db.GLChartOfAccountSubAccounts;
            var subaccounts = chartOfAccountSubAccounts.Select(
                i => new
                {
                    Name = i.Description,
                    Code = i.Values
                }).AsQueryable();


            ViewData["SubAccounts"] = subaccounts;

            ViewData["Structure"] = structure;

            ViewData["Branches"] = branches;
        }
        public string CheckStatus(int masterId)
        {
            var reques = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "TravelOrder");
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "TravelOrder" && x.Status == RequestStatus.APPROVE);
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "TravelOrder" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "true";
            }
            return "false";
        }
        public void SetEmployeeId(string fullName)
        {
            List<EmployeeIdView> EmployeeViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            if (employeeList.Count() > 0)
            {
                employeeList = employeeList.Where(l => l.IsActive).AsQueryable();
            }
            foreach (var emp in employeeList)
            {
                EmployeeView = new EmployeeIdView();
                EmployeeView.PersonId = emp.EmployeeId;
                EmployeeView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeViews.Add(EmployeeView);
            }
            var employees = EmployeeViews.Where(p => p.FullName == fullName).FirstOrDefault();
            if (employees != null)
            {
                HRMInputs.EmployeeId = employees.PersonId;
            }
         ;
        }
        public int SetEmployee(string x)
        {
            IsForOther = false;

            if (x == "true")
            {
                var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var list = EmployeeHelper.GetSubordinates(employeeId).ToList();

                IsForOther = true;
                return list.Count();

            }
            return 0;
        }
        public ActionResult ApTravelOrderRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrder> aptravelorders = db.ApTravelOrders;
            DataSourceResult result = aptravelorders.ToDataSourceResult(request, apTravelOrder => new
            {
                ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                //Name = apTravelOrder.Name,
                BranchId = apTravelOrder.BranchId,
                EmployeeId = apTravelOrder.EmployeeId,
                IsForOther = apTravelOrder.IsForOther,
                OrgStructureId = apTravelOrder.OrgStructureId,
                PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                RequestDate = apTravelOrder.RequestDate,
                AdvanceAmount = apTravelOrder.AdvanceAmount,
                //PaymentMethod = apTravelOrder.PaymentMethod,
                Reference = apTravelOrder.Reference,

                ForwardedFrom = apTravelOrder.ForwardedFrom,
                ForwardedTo = apTravelOrder.ForwardedTo,
                IsFinalApproval = apTravelOrder.IsFinalApproval,

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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderRequest_Create([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {


            if (ModelState.IsValid)
            {
                if (!apTravelOrder.IsForOther)
                {
                    EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                }
                else
                {
                    EmployeeId = apTravelOrder.EmployeeId;
                }
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var branch = UserInformationHelper.GetEmployeeBranchId(User.Identity.Name);
                var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == empId && x.isCurrent);
                int orgId = 0 ;
                if(placement != null)
                {
                    orgId = placement.OrgStructureId;
                }
                var entity = new ApTravelOrder
                {
                    //Name = apTravelOrder.Name,
                    EmployeeId = EmployeeId,
                    BranchId = branch,
                    OrgStructureId = orgId,
                    PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                    RequestDate = apTravelOrder.RequestDate,
                    AdvanceAmount = apTravelOrder.AdvanceAmount,
                    //PaymentMethod = apTravelOrder.PaymentMethod,
                    Reference = apTravelOrder.Reference,
                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                    Status = apTravelOrder.Status,
                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
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

                db.ApTravelOrders.Add(entity);
                db.SaveChanges();
                apTravelOrder.ApTravelOrderId = entity.ApTravelOrderId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order", "Success", User.Identity.Name, $"created expense claim {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderRequest_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrder
                {
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                    BranchId = apTravelOrder.BranchId,
                    EmployeeId = apTravelOrder.EmployeeId,
                    IsForOther = apTravelOrder.IsForOther,
                    OrgStructureId = apTravelOrder.OrgStructureId,
                    PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                    RequestDate = apTravelOrder.RequestDate,
                    AdvanceAmount = apTravelOrder.AdvanceAmount,
                    Reference = apTravelOrder.Reference,
                    IsOnlineTransferred = apTravelOrder.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelOrder.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelOrder.OnlineTransferredTime,
                    Status = apTravelOrder.Status,
                    ForwardedFrom = apTravelOrder.ForwardedFrom,
                    ForwardedTo = apTravelOrder.ForwardedTo,
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

                db.ApTravelOrders.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order", "Success", User.Identity.Name, $"updated expense claim {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderRequest_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrder
                {
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                    //Name = apTravelOrder.Name,
                    BranchId = apTravelOrder.BranchId,
                    EmployeeId = apTravelOrder.EmployeeId,
                    IsForOther = apTravelOrder.IsForOther,
                    OrgStructureId = apTravelOrder.OrgStructureId,
                    PurposeOfTravel = apTravelOrder.PurposeOfTravel,
                    RequestDate = apTravelOrder.RequestDate,
                    AdvanceAmount = apTravelOrder.AdvanceAmount,
                    //PaymentMethod = apTravelOrder.PaymentMethod,
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

                db.ApTravelOrders.Attach(entity);
                db.ApTravelOrders.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult SendForApproval()
        {
            RequestesStatus requestesStatus;
            db = new ExceedDbContext();
            requestesStatus =
                db.RequestesStatus.FirstOrDefault(x => x.RequestId == HRMInputs.RequestId &&
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
            db = new ExceedDbContext();
            if (requestesStatus == null)
            {
                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }
            else
            {
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
                    Remark = requestesStatus.Remark
                };

                db.RequestesStatus.Attach(entity);
                db.RequestesStatus.Remove(entity);
                db.SaveChanges();
                db = new ExceedDbContext();

                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}