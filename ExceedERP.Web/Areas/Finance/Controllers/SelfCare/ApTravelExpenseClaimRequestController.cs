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
    public class ApTravelExpenseClaimRequestController : Controller
    {
        // GET: Finance/ApTravelExpenseClaimRequest

        private readonly ExceedDbContext _db = new ExceedDbContext();
        public static int EmployeeId { get; set; }
        public static bool IsForOther { get; set; }
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
            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;

            var branches = _db.Branch;
            var costCenter = _db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();



            ViewData["costCenters"] = selectedcostCenter;

            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }
        public string CheckStatus(int masterId)
        {
            var reques = _db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "TravelExpenseClaim");
            if (reques != null)
            {
                var checkApprove = _db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "TravelExpenseClaim" && x.Status == RequestStatus.APPROVE);
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = _db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "TravelExpenseClaim" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
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
            var employeeList = _db.Person_Employee.AsQueryable();
            if (employeeList.Any())
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
            var employees = EmployeeViews.FirstOrDefault(p => p.FullName == fullName);
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

        public ActionResult ApTravelExpenseClaims_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseClaim> aptravelexpenseclaims = _db.ApTravelExpenseClaims;
            DataSourceResult result = aptravelexpenseclaims.ToDataSourceResult(request, apTravelExpenseClaim => new
            {
                ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,
                
                BranchId = apTravelExpenseClaim.BranchId,
                EmployeeId = apTravelExpenseClaim.EmployeeId,
                IsForOther = apTravelExpenseClaim.IsForOther,
                OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                ClaimDate = apTravelExpenseClaim.ClaimDate,
                
                Reference = apTravelExpenseClaim.Reference,
                ForwardedFrom = apTravelExpenseClaim.ForwardedFrom,
                ForwardedTo = apTravelExpenseClaim.ForwardedTo,

                IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
                IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
                OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
                Status = apTravelExpenseClaim.Status,
                IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
                OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
                OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
                IsVoid = apTravelExpenseClaim.IsVoid,
                VoidBy = apTravelExpenseClaim.VoidBy,
                VoidTime = apTravelExpenseClaim.VoidTime,
                IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
                SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
                SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
                OrgBranchName = apTravelExpenseClaim.OrgBranchName,
                DateCreated = apTravelExpenseClaim.DateCreated,
                LastModified = apTravelExpenseClaim.LastModified,
                CreatedBy = apTravelExpenseClaim.CreatedBy,
                ModifiedBy = apTravelExpenseClaim.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseClaims_Create([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
        {

            if (ModelState.IsValid)
            {
                if (!apTravelExpenseClaim.IsForOther)
                {
                    EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                }
                else
                {
                    EmployeeId = apTravelExpenseClaim.EmployeeId;
                }
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var branch = UserInformationHelper.GetEmployeeBranchId(User.Identity.Name);
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
                int orgId = 0;
                if (placement != null)
                {
                    orgId = placement.OrgStructureId;
                }
                var entity = new ApTravelExpenseClaim
                {
                    ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                    
                    BranchId = branch,
                    EmployeeId = EmployeeId,
                    OrgStructureId = orgId,
                    IsForOther = apTravelExpenseClaim.IsForOther,
                 
                    PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                    ClaimDate = apTravelExpenseClaim.ClaimDate,
                    
                    Reference = apTravelExpenseClaim.Reference,
                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,

                    IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
                    Status = apTravelExpenseClaim.Status,
                    IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
                    OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
                    IsVoid = apTravelExpenseClaim.IsVoid,
                    VoidBy = apTravelExpenseClaim.VoidBy,
                    VoidTime = apTravelExpenseClaim.VoidTime,
                    IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
                    SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
                    SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = apTravelExpenseClaim.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = apTravelExpenseClaim.ModifiedBy
                };

                _db.ApTravelExpenseClaims.Add(entity);
                _db.SaveChanges();
                apTravelExpenseClaim.ApTravelExpenseClaimId = entity.ApTravelExpenseClaimId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel expense claim", "Success", User.Identity.Name, $"created claim {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseClaim }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseClaims_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseClaim
                {
                    ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                    ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,
                    
                    BranchId = apTravelExpenseClaim.BranchId,
                    EmployeeId = apTravelExpenseClaim.EmployeeId,
                    IsForOther = apTravelExpenseClaim.IsForOther,
                    OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                    PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                    ClaimDate = apTravelExpenseClaim.ClaimDate,
                    
                    Reference = apTravelExpenseClaim.Reference,
                    ForwardedFrom = apTravelExpenseClaim.ForwardedFrom,
                    ForwardedTo = apTravelExpenseClaim.ForwardedTo,
                    IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
                    Status = apTravelExpenseClaim.Status,
                    IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
                    OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
                    IsVoid = apTravelExpenseClaim.IsVoid,
                    VoidBy = apTravelExpenseClaim.VoidBy,
                    VoidTime = apTravelExpenseClaim.VoidTime,
                    IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
                    SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
                    SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
                    OrgBranchName = apTravelExpenseClaim.OrgBranchName,
                    DateCreated = apTravelExpenseClaim.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = apTravelExpenseClaim.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ApTravelExpenseClaims.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel expense claim", "Success", User.Identity.Name, $"updated claim {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseClaim }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseClaims_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseClaim
                {
                    ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                    ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,
                    
                    BranchId = apTravelExpenseClaim.BranchId,
                    EmployeeId = apTravelExpenseClaim.EmployeeId,
                    IsForOther = apTravelExpenseClaim.IsForOther,
                    OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                    PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                    ClaimDate = apTravelExpenseClaim.ClaimDate,
                    
                    Reference = apTravelExpenseClaim.Reference,
                    IsOnlineTransferred = apTravelExpenseClaim.IsOnlineTransferred,
                    IsOnlineTransferredBy = apTravelExpenseClaim.IsOnlineTransferredBy,
                    OnlineTransferredTime = apTravelExpenseClaim.OnlineTransferredTime,
                    Status = apTravelExpenseClaim.Status,
                    IsOnlineApproved = apTravelExpenseClaim.IsOnlineApproved,
                    OnlineApprovedBy = apTravelExpenseClaim.OnlineApprovedBy,
                    OnlineApprovedTime = apTravelExpenseClaim.OnlineApprovedTime,
                    IsVoid = apTravelExpenseClaim.IsVoid,
                    VoidBy = apTravelExpenseClaim.VoidBy,
                    VoidTime = apTravelExpenseClaim.VoidTime,
                    IsSendForApproval = apTravelExpenseClaim.IsSendForApproval,
                    SendForApprovalBy = apTravelExpenseClaim.SendForApprovalBy,
                    SendForApprovalTime = apTravelExpenseClaim.SendForApprovalTime,
                    OrgBranchName = apTravelExpenseClaim.OrgBranchName,
                    DateCreated = apTravelExpenseClaim.DateCreated,
                    LastModified = apTravelExpenseClaim.LastModified,
                    CreatedBy = apTravelExpenseClaim.CreatedBy,
                    ModifiedBy = apTravelExpenseClaim.ModifiedBy
                };

                _db.ApTravelExpenseClaims.Attach(entity);
                _db.ApTravelExpenseClaims.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { apTravelExpenseClaim }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult SendForApproval()
        {
            RequestesStatus requestesStatus;
           // _db = new ExceedDbContext();
            requestesStatus =
                _db.RequestesStatus.FirstOrDefault(x => x.RequestId == HRMInputs.RequestId &&
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
            //_db = new ExceedDbContext();
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

                _db.RequestesStatus.Add(entityy);
                _db.SaveChanges();
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

                //_db.RequestesStatus.Attach(entity);
                _db.RequestesStatus.Remove(entity);
                _db.SaveChanges();
                //_db = new ExceedDbContext();

                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                _db.RequestesStatus.Add(entityy);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
