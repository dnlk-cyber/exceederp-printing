using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;
using System.Collections.Generic;
using ExceedERP.Reporting.Fleet.ViewModel;
//using ExceedERP.Reporting.Fleet.ViewModel;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetCostSheet
{
    [AuthorizeRoles(FleetManagementRoles.FleetCostSheetUser, FleetManagementRoles.FleetCostSheetApprover)]
    public class LightVehicleCostSheetController : Controller
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

            ViewData["fleet"] = db.Fleets.Select(x => new { x.FleetsID, x.PlateNumber }).ToList();
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();

            return View();
        }
       
        public ActionResult LightVehicleCostSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var lightVehicleCostSheet = db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => mybraches.Contains(x.BranchId));
            DataSourceResult result = lightVehicleCostSheet.ToDataSourceResult(request, model => new LightVehicleTimeSheet
            {
                
                FleetsID = model.FleetsID,
                LightVehicleTimeSheetId = model.LightVehicleTimeSheetId,
                Date= model.Date,
                MorningTimeStart= model.MorningTimeStart,
                MorningTimeEnd= model.MorningTimeEnd,
                AfternoonTimeStart= model.AfternoonTimeStart,
                AfternoonTimeEnd = model.AfternoonTimeEnd,
                NightTimeStart= model.NightTimeStart,
                NightTimeEnd= model.NightTimeEnd,
                NumberOfHoles= model.NumberOfHoles,
                HoleDepth= model.HoleDepth,
                FuelConsumption= model.FuelConsumption,
                FuelRate = model.FuelRate,
                GreaseConsumption= model.GreaseConsumption,
                IdleHours= model.IdleHours,
                IdleReason= model.IdleReason,
                DownHours= model.DownHours,
                DownReason= model.DownReason,
                NetHour= model.NetHour,
                WorkType= model.WorkType,
                Remark = model.Remark,
                RentalRate = model.RentalRate,
                IdealRentalRate = model.IdealRentalRate,
                ProcurementSupplierID = model.ProcurementSupplierID,
                OperationAmount = model.OperationAmount,
                IdleRate = model.IdleRate,
                IdleAmount = model.IdleAmount,
                TotalAmountBeforeVAT = model.TotalAmountBeforeVAT,
                VAT = model.VAT,
                LubricantCost = model.LubricantCost,
                TotalAmount = model.TotalAmount,
                FuelCost = model.FuelCost,
                ServiceRecharge = model.ServiceRecharge,
                MaintenanceCost = model.MaintenanceCost,
                TotalCost = model.TotalCost,
                NetAmount = model.NetAmount,

                DownHourRate = model.DownHourRate,
                OperationRevenue = model.OperationRevenue,
                Perdium = model.Perdium,
                Salary = model.Salary,
                TireCost = model.TireCost,
                BranchId = model.BranchId,
                BusinessUnitId = model.BusinessUnitId,
                CostSheetApproved = model.CostSheetApproved,
                CostSheetApprovedBy = model.CostSheetApprovedBy,
                CostSheetApprovedDate = model.CostSheetApprovedDate,
                CostSheetSendForAproval = model.CostSheetSendForAproval,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                TimeSheetApproved = model.TimeSheetApproved,
                TimeSheetApprovedBy = model.TimeSheetApprovedBy,
                TimeSheetApprovedDate = model.TimeSheetApprovedDate,
                TimeSheetSendForAproval = model.TimeSheetSendForAproval,
                TimeSheetStatus = model.TimeSheetStatus,
                CostSheetStatus = model.CostSheetStatus,
                GreaseRate = model.GreaseRate,
                CostSheetSendForApprovalBy = model.CostSheetSendForApprovalBy,
                CostSheetSendForApprovalTime = model.CostSheetSendForApprovalTime,
                OperatorId = model.OperatorId,
                TimeSheetSendForApprovalBy = model.TimeSheetSendForApprovalBy,
                TimeSheetSendForApprovalTime = model.TimeSheetSendForApprovalTime,
                EquipmentCategoryId = model.EquipmentCategoryId,
                EquipmentNameId = model.EquipmentNameId,
                EquipmentTypeId = model.EquipmentTypeId

            });
            return Json(result);
        }
        

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LightVehicleCostSheet_Update([DataSourceRequest] DataSourceRequest request, LightVehicleTimeSheet model)
        {
            

            if (ModelState.IsValid)
            {
                var entity = new LightVehicleTimeSheet
                {

                    FleetsID = model.FleetsID,
                    LightVehicleTimeSheetId = model.LightVehicleTimeSheetId,
                    Date = model.Date,
                    MorningTimeStart = model.MorningTimeStart,
                    MorningTimeEnd = model.MorningTimeEnd,
                    AfternoonTimeStart = model.AfternoonTimeStart,
                    AfternoonTimeEnd = model.AfternoonTimeEnd,
                    NightTimeStart = model.NightTimeStart,
                    NightTimeEnd = model.NightTimeEnd,
                    NumberOfHoles = model.NumberOfHoles,
                    HoleDepth = model.HoleDepth,
                    FuelConsumption = model.FuelConsumption,
                    FuelRate = model.FuelRate,
                    GreaseConsumption = model.GreaseConsumption,
                    IdleHours = model.IdleHours,
                    IdleReason = model.IdleReason,
                    DownHours = model.DownHours,
                    DownReason = model.DownReason,
                    NetHour = model.NetHour,
                    WorkType = model.WorkType,
                    Remark = model.Remark,
                    RentalRate = model.RentalRate,
                    IdealRentalRate = model.IdealRentalRate,
                    ProcurementSupplierID = model.ProcurementSupplierID,
                    OperationAmount = model.OperationAmount,
                    IdleRate = model.IdleRate,
                    IdleAmount = model.IdleAmount,
                    TotalAmountBeforeVAT = model.TotalAmountBeforeVAT,
                    VAT = model.VAT,
                    TotalAmount = model.TotalAmount,
                    FuelCost = model.FuelCost,
                    ServiceRecharge = model.ServiceRecharge,
                    LubricantCost = model.LubricantCost,
                    MaintenanceCost = model.MaintenanceCost,
                    TotalCost = model.TotalCost,
                    NetAmount = model.NetAmount,

                    DownHourRate = model.DownHourRate,
                    OperationRevenue = model.OperationRevenue,
                    Perdium = model.Perdium,
                    Salary = model.Salary,
                    TireCost = model.TireCost,
                    BranchId = model.BranchId,
                    BusinessUnitId = model.BusinessUnitId,
                    CostSheetApproved = model.CostSheetApproved,
                    CostSheetApprovedBy = model.CostSheetApprovedBy,
                    CostSheetApprovedDate = model.CostSheetApprovedDate,
                    CostSheetSendForAproval = model.CostSheetSendForAproval,
                    CreatedBy = model.CreatedBy,
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    ModifiedBy = model.ModifiedBy,
                    TimeSheetApproved = model.TimeSheetApproved,
                    TimeSheetApprovedBy = model.TimeSheetApprovedBy,
                    TimeSheetApprovedDate = model.TimeSheetApprovedDate,
                    TimeSheetSendForAproval = model.TimeSheetSendForAproval,
                    TimeSheetStatus = model.TimeSheetStatus,
                    CostSheetStatus = model.CostSheetStatus,
                    GreaseRate = model.GreaseRate,
                    EquipmentCategoryId = model.EquipmentCategoryId,
                    EquipmentNameId = model.EquipmentNameId,
                    EquipmentTypeId = model.EquipmentTypeId,
                    OperatorId = model.OperatorId,
                    TimeSheetSendForApprovalBy = model.TimeSheetSendForApprovalBy,
                    CostSheetSendForApprovalBy = model.CostSheetSendForApprovalBy,
                    CostSheetSendForApprovalTime = model.CostSheetSendForApprovalTime,
                    TimeSheetSendForApprovalTime = model.TimeSheetSendForApprovalTime
                    
                };

                
                db.LightVehicleTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Working light vehicle registration", "Success", User.Identity.Name, $"updated Light Vehicle working timesheet {entity.LightVehicleTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult LightVehicleCostSheet_Destroy([DataSourceRequest] DataSourceRequest request, LightVehicleTimeSheet model)
        {
            if (ModelState.IsValid)
            {
                var entity = new LightVehicleTimeSheet
                {
                    FleetsID = model.FleetsID,
                    LightVehicleTimeSheetId = model.LightVehicleTimeSheetId,
                    Date = model.Date,
                    MorningTimeStart = model.MorningTimeStart,
                    MorningTimeEnd = model.MorningTimeEnd,
                    AfternoonTimeStart = model.AfternoonTimeStart,
                    AfternoonTimeEnd = model.AfternoonTimeEnd,
                    NightTimeStart = model.NightTimeStart,
                    NightTimeEnd = model.NightTimeEnd,
                    NumberOfHoles = model.NumberOfHoles,
                    HoleDepth = model.HoleDepth,
                    FuelConsumption = model.FuelConsumption,
                    FuelRate = model.FuelRate,
                    GreaseConsumption = model.GreaseConsumption,
                    IdleHours = model.IdleHours,
                    IdleReason = model.IdleReason,
                    DownHours = model.DownHours,
                    DownReason = model.DownReason,
                    NetHour = model.NetHour,
                    WorkType = model.WorkType,
                    Remark = model.Remark,
                    RentalRate = model.RentalRate,
                    IdealRentalRate = model.IdealRentalRate,
                    ProcurementSupplierID = model.ProcurementSupplierID,
                    OperationAmount = model.OperationAmount,
                    IdleRate = model.IdleRate,
                    IdleAmount = model.IdleAmount,
                    TotalAmountBeforeVAT = model.TotalAmountBeforeVAT,
                    VAT = model.VAT,
                    TotalAmount = model.TotalAmount,
                    FuelCost = model.FuelCost,
                    ServiceRecharge = model.ServiceRecharge,
                    LubricantCost = model.LubricantCost,
                    MaintenanceCost = model.MaintenanceCost,
                    TotalCost = model.TotalCost,
                    NetAmount = model.NetAmount,

                    DownHourRate = model.DownHourRate,
                    OperationRevenue = model.OperationRevenue,
                    Perdium = model.Perdium,
                    Salary = model.Salary,
                    TireCost = model.TireCost,
                    BranchId = model.BranchId,
                    BusinessUnitId = model.BusinessUnitId,
                    CostSheetApproved = model.CostSheetApproved,
                    CostSheetApprovedBy = model.CostSheetApprovedBy,
                    CostSheetApprovedDate = model.CostSheetApprovedDate,
                    CostSheetSendForAproval = model.CostSheetSendForAproval,
                    CreatedBy = model.CreatedBy,
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    ModifiedBy = model.ModifiedBy,
                    TimeSheetApproved = model.TimeSheetApproved,
                    TimeSheetApprovedBy = model.TimeSheetApprovedBy,
                    TimeSheetApprovedDate = model.TimeSheetApprovedDate,
                    TimeSheetSendForAproval = model.TimeSheetSendForAproval,
                    TimeSheetStatus = model.TimeSheetStatus,
                    CostSheetStatus = model.CostSheetStatus,
                    GreaseRate = model.GreaseRate

                    
                };

                db.LightVehicleTimeSheets.Attach(entity);
                db.LightVehicleTimeSheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted working machinery timesheet {entity.LightVehicleTimeSheetId }");
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }



        //-----------------------------------------------------------Approval------------------------------------------------------------------//

        #region Approval
        public JsonResult CostSheetSendForApproval(int lightVehicleTimeSheetId)
        {

            var lightVehicleCostSheet = db.LightVehicleTimeSheets.Find(lightVehicleTimeSheetId);

            if (lightVehicleCostSheet != null)
            {
                lightVehicleCostSheet.CostSheetSendForAproval = true;
                lightVehicleCostSheet.CostSheetSendForApprovalTime = DateTime.Now;
                lightVehicleCostSheet.CostSheetSendForApprovalBy = User.Identity.Name;
                lightVehicleCostSheet.CostSheetStatus = MaintenanceConst.WaitingApproval;

                db.LightVehicleTimeSheets.Attach(lightVehicleCostSheet);
                db.Entry(lightVehicleCostSheet).State = EntityState.Modified;
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
        public JsonResult VoidCostSheet(int lightVehicleTimeSheetId)
        {

            var lightVehicleCostSheet = db.LightVehicleTimeSheets.Find(lightVehicleTimeSheetId);

            if (lightVehicleCostSheet != null)
            {
                lightVehicleCostSheet.CostSheetSendForAproval = false;
                lightVehicleCostSheet.CostSheetApproved = false;
                lightVehicleCostSheet.CostSheetApprovedDate = DateTime.Now;
                lightVehicleCostSheet.CostSheetApprovedBy = User.Identity.Name;
                lightVehicleCostSheet.CostSheetStatus = MaintenanceConst.Rejected;

                db.LightVehicleTimeSheets.Attach(lightVehicleCostSheet);
                db.Entry(lightVehicleCostSheet).State = EntityState.Modified;
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
        public ActionResult LightVehicleCostSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.CostSheetApprovals.Where(x => x.CostSheetId == id && x.TimeSheetType == TimeSheetType.LightVehicle);
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
                TimeSheetType = model.TimeSheetType,
                CostSheetApprovalId = model.CostSheetApprovalId
            });
            return Json(result);
        }
        [AuthorizeRoles(FleetManagementRoles.FleetCostSheetApprover)]
        public ActionResult LightVehicleCostSheetApproval_Create([DataSourceRequest] DataSourceRequest request, CostSheetApproval model, int id)
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
                model.TimeSheetType = TimeSheetType.LightVehicle;
                db.CostSheetApprovals.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var lightVehicleCostSheet = db.LightVehicleTimeSheets.Find(id);
            if (lightVehicleCostSheet != null)
            {
                lightVehicleCostSheet.CostSheetApproved = true;
                lightVehicleCostSheet.CostSheetStatus = MaintenanceConst.Approved;
                lightVehicleCostSheet.CostSheetApprovedBy = User.Identity.Name;
                lightVehicleCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(lightVehicleCostSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var lightVehicleCostSheet = db.LightVehicleTimeSheets.Find(id);
            if (lightVehicleCostSheet != null)
            {
                lightVehicleCostSheet.CostSheetApproved = false;
                lightVehicleCostSheet.CostSheetStatus = MaintenanceConst.Rejected;
                lightVehicleCostSheet.CostSheetApprovedBy = User.Identity.Name;
                lightVehicleCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(lightVehicleCostSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        #endregion



        public JsonResult GetFleetValues(Guid fleetId)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            if (fleet != null)
            {
                return Json(fleet, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}