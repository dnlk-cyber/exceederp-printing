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
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetTimesheet
{
    //[AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class LightVehicleTimeSheetController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index(TimeSheetType lightVehicle)
        {
            ViewBag.machineryType = lightVehicle;
            ViewData["fleet"] = db.Fleets.Select(x => new { x.FleetsID, Name = x.PlateNumber + "-" + x.SerialNo }).ToList();
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();

            return View();
        }
       
        public ActionResult LightVehicleTimeSheets_Read([DataSourceRequest] DataSourceRequest request)
        {
          
            var lightVehicleTimesheet = db.LightVehicleTimeSheets;           
            DataSourceResult result = lightVehicleTimesheet.ToDataSourceResult(request, model => new LightVehicleTimeSheet
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
                TimeSheetStatus = model.TimeSheetStatus,
                CostSheetApproved = model.CostSheetApproved,
                CostSheetApprovedBy = model.CostSheetApprovedBy,
                CostSheetApprovedDate = model.CostSheetApprovedDate,
                CostSheetSendForAproval = model.CostSheetSendForAproval,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                TimeSheetApproved = model.TimeSheetApproved,
                TimeSheetSendForAproval = model.TimeSheetSendForAproval,
                TimeSheetApprovedBy = model.TimeSheetApprovedBy,
                TimeSheetApprovedDate = model.TimeSheetApprovedDate,

                OperatorId = model.OperatorId

            });
            return Json(result);
        }
        public ActionResult LightVehicleTimeSheets_Create([DataSourceRequest] DataSourceRequest request, LightVehicleTimeSheet model)
        {
            #region valiadtion
            #endregion
            #region data
            var operationAmount = model.RentalRate * model.NetHour;
            var idleAmount = model.IdleRate * model.IdleHours;
            var totalAmountBeforeVAT = operationAmount + idleAmount;
            var totalAmount = totalAmountBeforeVAT * model.VAT;
            var fuelCost = model.FuelConsumption * model.FuelRate;
            var totalCost = fuelCost + model.MaintenanceCost + model.LubricantCost + model.ServiceRecharge;

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new LightVehicleTimeSheet
                {
                    FleetsID = model.FleetsID,
                    
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
                    OperationAmount = operationAmount,
                    IdleRate = model.IdleRate,
                    IdleAmount = idleAmount,
                    TotalAmountBeforeVAT = totalAmountBeforeVAT,
                    VAT = model.VAT,
                    TotalAmount = totalAmount,
                    FuelCost = fuelCost,
                    ServiceRecharge = model.ServiceRecharge,
                    LubricantCost = model.LubricantCost,
                    MaintenanceCost = model.MaintenanceCost,
                    TotalCost = totalCost,
                    NetAmount = totalAmount-totalCost,

                    DownHourRate = model.DownHourRate,
                    OperationRevenue = model.OperationRevenue,
                    Perdium = model.Perdium,
                    Salary = model.Salary,
                    TireCost = model.TireCost,
                    BranchId = model.BranchId,
                    BusinessUnitId = model.BusinessUnitId,
                    OperatorId = model.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today
                };

                db.LightVehicleTimeSheets.Add(entity);
                db.SaveChanges();
                model.LightVehicleTimeSheetId = entity.LightVehicleTimeSheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Light Vehicle timesheet", "Success", User.Identity.Name, $"created light vehicle working timesheet {entity.LightVehicleTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LightVehicleTimeSheets_Update([DataSourceRequest] DataSourceRequest request, LightVehicleTimeSheet model)
        {
            #region data
            var operationAmount = model.RentalRate * model.NetHour;
            var idleAmount = model.IdleRate * model.IdleHours;
            var totalAmountBeforeVAT = operationAmount + idleAmount;
            var totalAmount = totalAmountBeforeVAT * model.VAT;
            var fuelCost = model.FuelConsumption * model.FuelRate;
            var totalCost = fuelCost + model.MaintenanceCost + model.LubricantCost + model.ServiceRecharge;


            TimeSpan morningTime = model.MorningTimeEnd - model.MorningTimeStart;
            TimeSpan afternoonTime = model.AfternoonTimeEnd - model.AfternoonTimeStart;
            TimeSpan nightTime = model.NightTimeEnd - model.NightTimeStart;

            if (morningTime < TimeSpan.Zero && afternoonTime < TimeSpan.Zero && nightTime < TimeSpan.Zero)
            {
                // Convert time differences to decimal format
                double morningDecimal = morningTime.TotalHours;
                double afternoonDecimal = afternoonTime.TotalHours;
                double nightDecimal = nightTime.TotalHours;

                Console.WriteLine("Morning Time (decimal): " + morningDecimal);
                Console.WriteLine("Afternoon Time (decimal): " + afternoonDecimal);
                Console.WriteLine("Night Time (decimal): " + nightDecimal);
            }

            #endregion

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
                    OperationAmount = operationAmount,
                    IdleRate = model.IdleRate,
                    IdleAmount = idleAmount,
                    TotalAmountBeforeVAT = totalAmountBeforeVAT,
                    VAT = model.VAT,
                    TotalAmount = totalAmount,
                    FuelCost = fuelCost,
                    ServiceRecharge = model.ServiceRecharge,
                    LubricantCost = model.LubricantCost,
                    MaintenanceCost = model.MaintenanceCost,
                    TotalCost = totalCost,
                    NetAmount = totalAmount - totalCost,

                    DownHourRate = model.DownHourRate,
                    OperationRevenue = model.OperationRevenue,
                    Perdium = model.Perdium,
                    Salary = model.Salary,
                    TireCost = model.TireCost,
                    BranchId = model.BranchId,
                    BusinessUnitId = model.BusinessUnitId,
                    OperatorId = model.OperatorId
                };

                
                db.LightVehicleTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Light Vehicle registration", "Success", User.Identity.Name, $"updated machinery working timesheet {entity.LightVehicleTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult LightVehicleTimeSheets_Destroy([DataSourceRequest] DataSourceRequest request, LightVehicleTimeSheet model)
        {
            if (ModelState.IsValid)
            {
                var lightVehicleTimeSheet = db.LightVehicleTimeSheets.Find(model.LightVehicleTimeSheetId);
                db.LightVehicleTimeSheets.Attach(lightVehicleTimeSheet);
                db.LightVehicleTimeSheets.Remove(lightVehicleTimeSheet);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted working light Vehicle timesheet {model.LightVehicleTimeSheetId }");
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetFleetValues(Guid fleetId)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            if (fleet != null)
            {
                return Json(fleet, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }



        #region Approval
        public JsonResult TimeSheetSendForApproval(int lightVehicleTimeSheetId)
        {

            var lightVehicle = db.LightVehicleTimeSheets.Find(lightVehicleTimeSheetId);

            if (lightVehicle != null)
            {
                lightVehicle.TimeSheetSendForAproval = true;
                lightVehicle.TimeSheetSendForApprovalTime = DateTime.Now;
                lightVehicle.TimeSheetSendForApprovalBy = User.Identity.Name;
                lightVehicle.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.LightVehicleTimeSheets.Attach(lightVehicle);
                db.Entry(lightVehicle).State = EntityState.Modified;
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
        public ActionResult LightVehicleTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == id && x.TimeSheetType == TimeSheetType.LightVehicle);
            DataSourceResult result = approval.ToDataSourceResult(request, model => new TimeSheetApprovalNew
            {
                TimeSheetId = model.TimeSheetId,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                IsApproved = model.IsApproved,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                Remark = model.Remark,
                Status = model.Status,
                TimeSheetType = model.TimeSheetType,
                TimeSheetApprovalNewId = model.TimeSheetApprovalNewId
            });
            return Json(result);
        }
        public ActionResult LightVehicleTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int id)
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
                model.TimeSheetId = id;
                model.TimeSheetType = TimeSheetType.LightVehicle;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var lightVehicle = db.LightVehicleTimeSheets.Find(id);
            if (lightVehicle != null)
            {
                lightVehicle.TimeSheetApproved = true;
                lightVehicle.TimeSheetStatus = MaintenanceConst.Approved;
                lightVehicle.TimeSheetApprovedBy = User.Identity.Name;
                lightVehicle.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(lightVehicle).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var lightVehicle = db.LightVehicleTimeSheets.Find(id);
            if (lightVehicle != null)
            {
                lightVehicle.TimeSheetApproved = false;
                lightVehicle.TimeSheetStatus = MaintenanceConst.Rejected;
                lightVehicle.TimeSheetApprovedBy = User.Identity.Name;
                lightVehicle.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(lightVehicle).State = EntityState.Modified;
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