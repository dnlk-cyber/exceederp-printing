using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Reporting.Fleet.ViewModel;
//using ExceedERP.Reporting.Fleet.ViewModel;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetCostSheet
{
    [AuthorizeRoles(FleetManagementRoles.FleetCostSheetUser, FleetManagementRoles.FleetCostSheetApprover)]
    public class WagonDrillCostSheetController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["category"] = db.EquipmentIdentity.Select(s => new
            {
                Text = s.EquipmentCategoryName,
                Value = s.EquipmentIdentityID
            }).ToList();
            ViewData["type"] = db.EquipmentType.Select(s => new
            {
                Text = s.EquipmentTypeName,
                Value = s.EquipmentTypeID
            }).ToList();
            ViewData["name"] = db.EquipmentName.Select(s => new
            {
                Text = s.Name,
                Value = s.EquipmentNameID
            }).ToList();
            ViewData["fleet"] = db.Fleets.Select(x => new
            {
                Text = x.FleetsID,
                Value = x.EquipmentName
            }).ToList();
            ViewData["branch"] = db.Branch.Select(s => new
            {
                Value = s.BranchId,
                Text = s.Name
            }).ToList();
            ViewData["operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.FullName
            }).ToList();
            ViewData["businessunit"] = db.BusinessUnits.Select(s => new
            {
                Value = s.BusinessUnitId,
                Text = s.Name
            }).ToList();

            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            return View();
        }
        public ActionResult WagonDrillCostSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var wagonDrillTimesheets = db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved).Where(x => mybraches.Contains(x.BranchId));
            DataSourceResult result = wagonDrillTimesheets.ToDataSourceResult(request, wagonDrillTimesheet => new WagonDrillTimesheet
            {
                WagonDrillTimesheetId = wagonDrillTimesheet.WagonDrillTimesheetId,
                FleetsID = wagonDrillTimesheet.FleetsID,
                ProcurementSupplierID = wagonDrillTimesheet.ProcurementSupplierID,
                Date = wagonDrillTimesheet.Date,
                WorkItem = wagonDrillTimesheet.WorkItem,
                OperationNetWorkingHour = wagonDrillTimesheet.OperationNetWorkingHour,
                OperationDrilledHoleDepth = wagonDrillTimesheet.OperationDrilledHoleDepth,
                NumberOfHolesDrilled = wagonDrillTimesheet.NumberOfHolesDrilled,
                OperationRateForMeter = wagonDrillTimesheet.OperationRateForMeter,
                OperationAmount = wagonDrillTimesheet.OperationAmount,
                IdleAmount = wagonDrillTimesheet.IdleAmount,
                TotalAmountBeforeVAT = wagonDrillTimesheet.TotalAmountBeforeVAT,
                TotalAmount = wagonDrillTimesheet.TotalAmount,
                FuelCost = wagonDrillTimesheet.FuelCost,
                TotalCost = wagonDrillTimesheet.TotalCost,
                NetAmount = wagonDrillTimesheet.NetAmount,
                IdleHour = wagonDrillTimesheet.IdleHour,
                IdleRateHour = wagonDrillTimesheet.IdleRateHour,
                DownTime = wagonDrillTimesheet.DownTime,
                VAT = wagonDrillTimesheet.VAT,
                FuelInLiter = wagonDrillTimesheet.FuelInLiter,
                FuelRate = wagonDrillTimesheet.FuelRate,
                ServiceCharge = wagonDrillTimesheet.ServiceCharge,
                LubricantCost = wagonDrillTimesheet.LubricantCost,
                MaintenanceCost = wagonDrillTimesheet.MaintenanceCost,
                TireCost = wagonDrillTimesheet.TireCost,
                DateCreated = wagonDrillTimesheet.DateCreated,
                CreatedBy = wagonDrillTimesheet.CreatedBy,
                ModifiedBy = wagonDrillTimesheet.ModifiedBy,
                LastModified = wagonDrillTimesheet.LastModified,
                Perdium = wagonDrillTimesheet.Perdium,
                Salary = wagonDrillTimesheet.Salary,
                BranchId = wagonDrillTimesheet.BranchId,
                BusinessUnitId = wagonDrillTimesheet.BusinessUnitId,
                CostSheetApproved = wagonDrillTimesheet.CostSheetApproved,
                CostSheetApprovedBy = wagonDrillTimesheet.CostSheetApprovedBy,
                CostSheetApprovedDate = wagonDrillTimesheet.CostSheetApprovedDate,
                CostSheetSendForAproval = wagonDrillTimesheet.CostSheetSendForAproval,
                CostSheetStatus = wagonDrillTimesheet.CostSheetStatus,

                TimeSheetApproved = wagonDrillTimesheet.TimeSheetApproved,
                TimeSheetApprovedBy = wagonDrillTimesheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = wagonDrillTimesheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval = wagonDrillTimesheet.TimeSheetSendForAproval,
                TimeSheetStatus = wagonDrillTimesheet.TimeSheetStatus,
                CostSheetSendForApprovalBy = wagonDrillTimesheet.CostSheetSendForApprovalBy,
                CostSheetSendForApprovalTime = wagonDrillTimesheet.CostSheetSendForApprovalTime,
                TimeSheetSendForApprovalBy = wagonDrillTimesheet.TimeSheetSendForApprovalBy,
                TimeSheetSendForApprovalTime = wagonDrillTimesheet.TimeSheetSendForApprovalTime,
                ExternalOperator = wagonDrillTimesheet.ExternalOperator,
                RateInMeter = wagonDrillTimesheet.RateInMeter,
                ReasonForDown = wagonDrillTimesheet.ReasonForDown,
                ReasonForIdle = wagonDrillTimesheet.ReasonForIdle,
                EquipmentCategoryId = wagonDrillTimesheet.EquipmentCategoryId,
                EquipmentNameId = wagonDrillTimesheet.EquipmentNameId,
                EquipmentTypeId = wagonDrillTimesheet.EquipmentTypeId,
                OperatorId = wagonDrillTimesheet.OperatorId,
                

            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WagonDrillCostSheet_Update([DataSourceRequest] DataSourceRequest request, WagonDrillTimesheet model)
        {
            if (ModelState.IsValid)
            {
                var entity = new WagonDrillTimesheet
                {
                    BranchId = model.BranchId,
                    BusinessUnitId = model.BusinessUnitId,
                    CostSheetApproved = model.CostSheetApproved,
                    CostSheetApprovedBy = model.CostSheetApprovedBy,
                    CostSheetApprovedDate = model.CostSheetApprovedDate,
                    CostSheetSendForApprovalBy = model.CostSheetSendForApprovalBy,
                    CostSheetSendForApprovalTime = model.CostSheetSendForApprovalTime,
                    CostSheetSendForAproval = model.CostSheetSendForAproval,
                    CostSheetStatus = model.CostSheetStatus,
                    CreatedBy = model.CreatedBy,
                    Date = model.Date,
                    DateCreated = model.DateCreated,
                    DownTime = model.DownTime,
                    EquipmentCategoryId = model.EquipmentCategoryId,
                    EquipmentNameId = model.EquipmentNameId,
                    EquipmentTypeId = model.EquipmentTypeId,
                    ExternalOperator = model.ExternalOperator,
                    FleetsID = model.FleetsID,
                    FuelCost = model.FuelCost,
                    FuelInLiter = model.FuelInLiter,
                    FuelRate = model.FuelRate,
                    
                    IdleAmount = model.IdleAmount,
                    IdleHour = model.IdleHour,
                    IdleRateHour = model.IdleRateHour,
                    LastModified = model.LastModified,
                    LubricantCost = model.LubricantCost,
                    MaintenanceCost = model.MaintenanceCost,
                    ModifiedBy = model.ModifiedBy,
                    NetAmount = model.NetAmount,
                    NumberOfHolesDrilled = model.NumberOfHolesDrilled,
                    OperationAmount = model.OperationAmount,
                    OperationDrilledHoleDepth = model.OperationDrilledHoleDepth,
                    OperationNetWorkingHour = model.OperationNetWorkingHour,
                    OperationRateForMeter = model.OperationRateForMeter,
                    OperatorId = model.OperatorId,
                    Perdium = model.Perdium,
                    ProcurementSupplierID = model.ProcurementSupplierID,
                    RateInMeter = model.RateInMeter,
                    ReasonForDown = model.ReasonForDown,
                    ReasonForIdle = model.ReasonForIdle,
                    Salary = model.Salary,
                    ServiceCharge = model.ServiceCharge,
                    TimeSheetApproved = model.TimeSheetApproved,
                    TimeSheetApprovedBy = model.TimeSheetApprovedBy,
                    TimeSheetApprovedDate = model.TimeSheetApprovedDate,
                    TimeSheetSendForApprovalBy = model.TimeSheetSendForApprovalBy,
                    TimeSheetSendForApprovalTime = model.TimeSheetSendForApprovalTime,
                    TimeSheetSendForAproval = model.TimeSheetSendForAproval,
                    TimeSheetStatus = model.TimeSheetStatus,
                    TireCost = model.TireCost,
                    TotalAmount = model.TotalAmount,
                    TotalAmountBeforeVAT = model.TotalAmountBeforeVAT,
                    TotalCost = model.TotalCost,
                    VAT = model.VAT,
                    WagonDrillTimesheetId = model.WagonDrillTimesheetId,
                    WorkItem = model.WorkItem,
                    
                };
                db.WagonDrillTimesheets.Attach(model);
                db.Entry(model).State = EntityState.Modified;

                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Wagon drill timesheet update", "Success", User.Identity.Name, $"updated wagon drill timesheet {model.WagonDrillTimesheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }



        #region Approval
        public JsonResult CostSheetSendForApproval(int id)
        {

            var wagonDrillCostSheet = db.WagonDrillTimesheets.Find(id);

            if (wagonDrillCostSheet != null)
            {
                wagonDrillCostSheet.CostSheetSendForAproval = true;
                wagonDrillCostSheet.CostSheetSendForApprovalTime = DateTime.Now;
                wagonDrillCostSheet.CostSheetSendForApprovalBy = User.Identity.Name;
                wagonDrillCostSheet.CostSheetStatus = MaintenanceConst.WaitingApproval;

                db.WagonDrillTimesheets.Attach(wagonDrillCostSheet);
                db.Entry(wagonDrillCostSheet).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

            }

            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        }
        public JsonResult VoidTimeSheet(int id)
        {

            var wagonDrillCostSheet = db.WagonDrillTimesheets.Find(id);

            if (wagonDrillCostSheet != null)
            {
                wagonDrillCostSheet.CostSheetSendForAproval = false;
                wagonDrillCostSheet.CostSheetApproved = false;
                wagonDrillCostSheet.CostSheetApprovedDate = DateTime.Now;
                wagonDrillCostSheet.CostSheetApprovedBy = User.Identity.Name;
                wagonDrillCostSheet.CostSheetStatus = MaintenanceConst.Rejected;

                db.WagonDrillTimesheets.Attach(wagonDrillCostSheet);
                db.Entry(wagonDrillCostSheet).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

            }

            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        }
        public ActionResult WagonDrillCostSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.CostSheetApprovals.Where(x => x.CostSheetId == id && x.TimeSheetType == TimeSheetType.WagonDrill);
            DataSourceResult result = approval.ToDataSourceResult(request, model => new CostSheetApproval
            {
                CostSheetId = model.CostSheetId,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                IsApproved = model.IsApproved,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                Remark = model.Remark,
                Status = model.Status,
                CostSheetApprovalId = model.CostSheetApprovalId,
                TimeSheetType = model.TimeSheetType
            });
            return Json(result);
        }
        [AuthorizeRoles(FleetManagementRoles.FleetCostSheetApprover)]
        public ActionResult WagonDrillCostSheetApproval_Create([DataSourceRequest] DataSourceRequest request, CostSheetApproval model, int id)
        {
            if (ModelState.IsValid)
            {
                if (model.Status == MaintenanceConst.Approved)
                {
                    Approved(id);
                    model.IsApproved = true;

                }
                else
                {
                    Rejected(id);
                    model.IsApproved = false;
                }
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.CostSheetId = id;
                model.TimeSheetType = TimeSheetType.WagonDrill;
                db.CostSheetApprovals.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var wagonDrillCostSheet = db.WagonDrillTimesheets.Find(id);
            if (wagonDrillCostSheet != null)
            {
                wagonDrillCostSheet.CostSheetApproved = true;
                wagonDrillCostSheet.CostSheetStatus = MaintenanceConst.Approved;
                wagonDrillCostSheet.CostSheetApprovedBy = User.Identity.Name;
                wagonDrillCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(wagonDrillCostSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var wagonDrillCostSheet = db.WagonDrillTimesheets.Find(id);
            if (wagonDrillCostSheet != null)
            {
                wagonDrillCostSheet.CostSheetApproved = false;
                wagonDrillCostSheet.CostSheetStatus = MaintenanceConst.Rejected;
                wagonDrillCostSheet.CostSheetApprovedBy = User.Identity.Name;
                wagonDrillCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(wagonDrillCostSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}