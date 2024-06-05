using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Travel
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class TravelOrderRequestController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
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

            var subAccountSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
            var subAccounts =_db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId);
            var chartOfAccountSubAccounts = subAccounts;
            var subaccounts = chartOfAccountSubAccounts.Select(
                i => new
                {
                    Name = i.Description,
                    Code = i.Values
                }).AsQueryable();


            ViewData["SubAccounts"] = subaccounts;

            ViewData["Structure"] = structure;

            ViewData["Branches"] = branches;
            var items = _db.ApSetupItems.ToList();
            ViewData["Items"] = items;
        }

        public ActionResult TravelOrderRequest_Read([DataSourceRequest]DataSourceRequest request, string vdId)
        {
            Guid.TryParse(vdId, out Guid vehicleDistributionId);
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            IQueryable<ApTravelOrder> aptravelorders = _db.ApTravelOrders.Where(x => x.VehicleDistributionID == vehicleDistributionId);

            if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService)
                 && System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HRDepartmentHead))
            {

                aptravelorders = aptravelorders.Where(x => userworkunit.Contains(x.OrgStructureId)
                || x.ForwardedTo == User.Identity.Name
                || x.ForwardedFrom == User.Identity.Name
                || x.CreatedBy == User.Identity.Name);

            }
            else if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService))
            {
                aptravelorders = aptravelorders.Where(x => x.CreatedBy == User.Identity.Name);
            }

            DataSourceResult result = aptravelorders.ToDataSourceResult(request, apTravelOrder => new ApTravelOrder
            {
                ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                BranchId = apTravelOrder.BranchId,
                BusinessUnitId = apTravelOrder.BusinessUnitId,
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
                ModifiedBy = apTravelOrder.ModifiedBy,
                VehicleDistributionID = apTravelOrder.VehicleDistributionID,
                FromBranchId = apTravelOrder.FromBranchId,
                FromBusinessUnitId = apTravelOrder.FromBranchId,
                CreatedAtOrgStructureId = apTravelOrder.CreatedAtOrgStructureId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelOrderRequest_Create([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder, string vdId)
        {
            Guid.TryParse(vdId, out Guid vehicleDistributionId);
            var vehicleDistribution = _db.VehicleDistribution.FirstOrDefault(x => x.VehicleDistributionID == vehicleDistributionId);
           
            var requestEq = _db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber == vehicleDistribution.RequestID);
           
            var assignedEquipment = _db.AssignedEquipment.FirstOrDefault(x => x.VehicleDemandId == requestEq.VehicleDemandId);
            if(assignedEquipment == null)
            {
                ModelState.AddModelError(string.Empty, "Equipment has not been assigned, Please assign equipment first");
                return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
            }
            var oprator = _db.Operator.FirstOrDefault(x => x.OperatorID == assignedEquipment.OperatorId);
            if(oprator == null)
            {
                ModelState.AddModelError(string.Empty, "Operator not found");
                return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
            }
            var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == oprator.EmployeeId);
            if(employee == null)
            {
                ModelState.AddModelError(string.Empty, "Employee not found");
                return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
            }
            apTravelOrder.EmployeeId = employee.EmployeeId;
            #region data 

            if (apTravelOrder.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelOrder.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apTravelOrder.OrgStructureId = placement.OrgStructureId;
                    apTravelOrder.BranchId = placement.BranchId;
                    apTravelOrder.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            apTravelOrder.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            apTravelOrder.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            #endregion

            if (ModelState.IsValid)
            {

                
                    var entity = new ApTravelOrder
                    {

                        EmployeeId = apTravelOrder.EmployeeId,
                        BranchId = apTravelOrder.BranchId,
                        BusinessUnitId = apTravelOrder.BusinessUnitId,
                        CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
                        CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
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

                        ForwardedFrom = User.Identity.Name,
                        ForwardedTo = User.Identity.Name,
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
                        ModifiedBy = apTravelOrder.ModifiedBy,
                        FromBranchId = apTravelOrder.FromBranchId,
                        FromBusinessUnitId = apTravelOrder.FromBranchId,
                        CreatedAtOrgStructureId = apTravelOrder.CreatedAtOrgStructureId
                    };
                    entity.VehicleDistributionID = vehicleDistribution.VehicleDistributionID;
                    _db.ApTravelOrders.Add(entity);
                    _db.SaveChanges();
                    apTravelOrder.ApTravelOrderId = entity.ApTravelOrderId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order", "Success", User.Identity.Name, $"created expense claim {entity.ApTravelOrderId} ", Modules.Finance);
                    #endregion
                
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelOrderRequest_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            #region data 
            if (apTravelOrder.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (apTravelOrder.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == apTravelOrder.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    apTravelOrder.OrgStructureId = placement.OrgStructureId;
                    apTravelOrder.BranchId = placement.BranchId;
                    apTravelOrder.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrder
                {
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                    BranchId = apTravelOrder.BranchId,
                    BusinessUnitId = apTravelOrder.BusinessUnitId,
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
                    ModifiedBy = User.Identity.Name,
                    FromBranchId = apTravelOrder.FromBranchId,
                    FromBusinessUnitId = apTravelOrder.FromBranchId,
                    CreatedAtOrgStructureId = apTravelOrder.CreatedAtOrgStructureId
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelOrderRequest_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrder apTravelOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrder
                {
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,                  
                    BranchId = apTravelOrder.BranchId,
                    BusinessUnitId = apTravelOrder.BusinessUnitId,
                    CreatedAtBranchId = apTravelOrder.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = apTravelOrder.CreatedAtBusinessUnitId,
                    EmployeeId = apTravelOrder.EmployeeId,
                    IsForOther = apTravelOrder.IsForOther,
                    OrgStructureId = apTravelOrder.OrgStructureId,
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
                    ModifiedBy = apTravelOrder.ModifiedBy,
                    FromBranchId = apTravelOrder.FromBranchId,
                    FromBusinessUnitId = apTravelOrder.FromBranchId,
                    CreatedAtOrgStructureId = apTravelOrder.CreatedAtOrgStructureId
                };

                _db.ApTravelOrders.Attach(entity);
                _db.ApTravelOrders.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }


        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = _db.ApTravelOrders.Find(id);
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

            var lines = _db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == id);
            if (!lines.Any())
            {
                errMsg = "Please add departure items.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (!lines.Any(x=>x.Type==DepartureType.Departure))
            {
                errMsg = "Please add departure line.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (!lines.Any(x => x.Type == DepartureType.Return))
            {
                errMsg = "Please add return line.";
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
                var validations = record.ApTravelOrderDepartmentValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = record.AdvanceAmount;
                int docid = record.ApTravelOrderId;
                int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApTravelOrderSelfCare, isNewRecord,step);
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