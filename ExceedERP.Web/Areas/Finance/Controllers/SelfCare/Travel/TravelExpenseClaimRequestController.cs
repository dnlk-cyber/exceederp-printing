using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare.Travel
{
    [Authorize]
    public class TravelExpenseClaimRequestController : Controller
    {
        // GET: Finance/ApTravelExpenseClaimRequest

        private readonly ExceedDbContext _db = new ExceedDbContext();
   
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
       

        public ActionResult TravelExpenseClaims_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseClaim> aptravelexpenseclaims = _db.ApTravelExpenseClaims;
            DataSourceResult result = aptravelexpenseclaims.ToDataSourceResult(request, apTravelExpenseClaim => new
            {
                ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,
                
                BranchId = apTravelExpenseClaim.BranchId,
                BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
                CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
                CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
                EmployeeId = apTravelExpenseClaim.EmployeeId,
                IsForOther = apTravelExpenseClaim.IsForOther,
                OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                ClaimDate = apTravelExpenseClaim.ClaimDate,

                IsDepartmentOnlineApproved = apTravelExpenseClaim.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = apTravelExpenseClaim.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = apTravelExpenseClaim.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = apTravelExpenseClaim.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = apTravelExpenseClaim.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = apTravelExpenseClaim.DepartmentSendForApprovalTime,

                ForwardedFrom = apTravelExpenseClaim.ForwardedFrom,
                ForwardedTo = apTravelExpenseClaim.ForwardedTo,
                IsFinalApproval = apTravelExpenseClaim.IsFinalApproval,
                CurrentApprovalStep = apTravelExpenseClaim.CurrentApprovalStep,

                IsBudgetApproved = apTravelExpenseClaim.IsBudgetApproved,
                BudgetApprovedBy = apTravelExpenseClaim.BudgetApprovedBy,
                BudgetApprovedTime = apTravelExpenseClaim.BudgetApprovedTime,

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
        public ActionResult TravelExpenseClaims_Create([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
        {
            #region data 
            if (apTravelExpenseClaim.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (apTravelExpenseClaim.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelExpenseClaim.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apTravelExpenseClaim.OrgStructureId = placement.OrgStructureId;
                    apTravelExpenseClaim.BranchId = placement.BranchId;
                    apTravelExpenseClaim.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            apTravelExpenseClaim.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            apTravelExpenseClaim.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            #endregion

            if (ModelState.IsValid)
            {
               
                var entity = new ApTravelExpenseClaim
                {
                    ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,

                    EmployeeId = apTravelExpenseClaim.EmployeeId,
                    BranchId = apTravelExpenseClaim.BranchId,
                    BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
                    CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
                    OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                    IsForOther = apTravelExpenseClaim.IsForOther,
                 
                    PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                    ClaimDate = apTravelExpenseClaim.ClaimDate,

                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = apTravelExpenseClaim.IsFinalApproval,
                    CurrentApprovalStep = apTravelExpenseClaim.CurrentApprovalStep,

                    IsBudgetApproved = apTravelExpenseClaim.IsBudgetApproved,
                    BudgetApprovedBy = apTravelExpenseClaim.BudgetApprovedBy,
                    BudgetApprovedTime = apTravelExpenseClaim.BudgetApprovedTime,

                    IsDepartmentOnlineApproved = apTravelExpenseClaim.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = apTravelExpenseClaim.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = apTravelExpenseClaim.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = apTravelExpenseClaim.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = apTravelExpenseClaim.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = apTravelExpenseClaim.DepartmentSendForApprovalTime,

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
        public ActionResult TravelExpenseClaims_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
        {
            #region data 
            if (apTravelExpenseClaim.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (apTravelExpenseClaim.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelExpenseClaim.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apTravelExpenseClaim.OrgStructureId = placement.OrgStructureId;
                    apTravelExpenseClaim.BranchId = placement.BranchId;
                    apTravelExpenseClaim.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

             #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseClaim
                {
                    ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                    ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,                    
                    BranchId = apTravelExpenseClaim.BranchId,
                    BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
                    CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
                    EmployeeId = apTravelExpenseClaim.EmployeeId,
                    IsForOther = apTravelExpenseClaim.IsForOther,
                    OrgStructureId = apTravelExpenseClaim.OrgStructureId,
                    PurposeOfTravel = apTravelExpenseClaim.PurposeOfTravel,
                    ClaimDate = apTravelExpenseClaim.ClaimDate,
                    
                    Reference = apTravelExpenseClaim.Reference,

                    ForwardedFrom = apTravelExpenseClaim.ForwardedFrom,
                    ForwardedTo = apTravelExpenseClaim.ForwardedTo,
                    IsFinalApproval = apTravelExpenseClaim.IsFinalApproval,
                    CurrentApprovalStep = apTravelExpenseClaim.CurrentApprovalStep,

                    IsBudgetApproved = apTravelExpenseClaim.IsBudgetApproved,
                    BudgetApprovedBy = apTravelExpenseClaim.BudgetApprovedBy,
                    BudgetApprovedTime = apTravelExpenseClaim.BudgetApprovedTime,

                    IsDepartmentOnlineApproved = apTravelExpenseClaim.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = apTravelExpenseClaim.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = apTravelExpenseClaim.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = apTravelExpenseClaim.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = apTravelExpenseClaim.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = apTravelExpenseClaim.DepartmentSendForApprovalTime,

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
        public ActionResult TravelExpenseClaims_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseClaim apTravelExpenseClaim)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseClaim
                {
                    ApTravelOrderId = apTravelExpenseClaim.ApTravelOrderId,
                    ApTravelExpenseClaimId = apTravelExpenseClaim.ApTravelExpenseClaimId,
                    
                    BranchId = apTravelExpenseClaim.BranchId,
                    BusinessUnitId = apTravelExpenseClaim.BusinessUnitId,
                    CreatedAtBranchId = apTravelExpenseClaim.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelExpenseClaim.CreatedAtBusinessUnitId,
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


        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = _db.ApTravelExpenseClaims.Find(id);
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

            var lines = _db.ApTravelExpenseExpenses.Where(x => x.ApTravelExpenseClaimId == id);
            if (!lines.Any())
            {
                errMsg = "Please add line expense items.";
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
                var validations = record.TravelExpenseDepartmentValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = record.Payments.Sum(x => x.Amount);
                int docid = record.ApTravelExpenseClaimId;
                int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApTravelExpenseClaimSelfCare, isNewRecord,step);
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
        
        public ActionResult SendForApproval_1()
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
