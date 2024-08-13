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
    public class MachineryWorkingTimeSheetController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index(TimeSheetType machinery)
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
            ViewBag.machineryType = machinery;
            ViewData["fleet"] = db.Fleets.Select(x => new
            {
                Value = x.FleetsID,
                Text = x.SerialNo
            }).ToList();
            ViewData["branch"] = db.Branch.Select(s => new
            {
                Value = s.BranchId,
                Text = s.Name
            }).ToList();
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            RepopulateTimeSheet();
            return View();
        }
        public void RepopulateTimeSheet()
        {
            var timesheets = db.MachineryWorkingTimeSheets.ToList();
            if (timesheets.Any())
            {
                foreach (var item in timesheets)
                {
                    item.EquipmentCategoryId = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID)?.EquipmentIdentityID;
                    item.EquipmentTypeId = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID)?.EquipmentTypeID;
                    item.EquipmentNameId = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID)?.EquipmentNameID;
                    db.Entry(item).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

        }
        public ActionResult MachineryWorkingTimeSheets_Read([DataSourceRequest] DataSourceRequest request)
        {
          
            var machineryWorkingTimeSheets = db.MachineryWorkingTimeSheets;           
            DataSourceResult result = machineryWorkingTimeSheets.ToDataSourceResult(request, machineryWorkingTimeSheet => new MachineryWorkingTimeSheet
            {
                
                FleetsID = machineryWorkingTimeSheet.FleetsID,
                MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                Date= machineryWorkingTimeSheet.Date,
                MorningTimeStart= machineryWorkingTimeSheet.MorningTimeStart,
                MorningTimeEnd= machineryWorkingTimeSheet.MorningTimeEnd,
                AfternoonTimeStart= machineryWorkingTimeSheet.AfternoonTimeStart,
                AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                NightTimeStart= machineryWorkingTimeSheet.NightTimeStart,
                NightTimeEnd= machineryWorkingTimeSheet.NightTimeEnd,
                NumberOfHoles= machineryWorkingTimeSheet.NumberOfHoles,
                HoleDepth= machineryWorkingTimeSheet.HoleDepth,
                FuelConsumption= machineryWorkingTimeSheet.FuelConsumption,
                FuelRate = machineryWorkingTimeSheet.FuelRate,
                GreaseConsumption= machineryWorkingTimeSheet.GreaseConsumption,
                IdleHours= machineryWorkingTimeSheet.IdleHours,
                IdleReason= machineryWorkingTimeSheet.IdleReason,
                DownHours= machineryWorkingTimeSheet.DownHours,
                DownReason= machineryWorkingTimeSheet.DownReason,
                NetHour= machineryWorkingTimeSheet.NetHour,
                WorkType= machineryWorkingTimeSheet.WorkType,
                Remark = machineryWorkingTimeSheet.Remark,
                RentalRate = machineryWorkingTimeSheet.RentalRate,
                IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                OperationAmount = machineryWorkingTimeSheet.OperationAmount,
                IdleRate = machineryWorkingTimeSheet.IdleRate,
                IdleAmount = machineryWorkingTimeSheet.IdleAmount,
                TotalAmountBeforeVAT = machineryWorkingTimeSheet.TotalAmountBeforeVAT,
                VAT = machineryWorkingTimeSheet.VAT,
                TotalAmount = machineryWorkingTimeSheet.TotalAmount,
                FuelCost = machineryWorkingTimeSheet.FuelCost,
                ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,
                LubricantCost = machineryWorkingTimeSheet.LubricantCost,
                MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                TotalCost = machineryWorkingTimeSheet.TotalCost,
                NetAmount = machineryWorkingTimeSheet.NetAmount,

                DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                Perdium = machineryWorkingTimeSheet.Perdium,
                Salary = machineryWorkingTimeSheet.Salary,
                TireCost = machineryWorkingTimeSheet.TireCost,
                BranchId = machineryWorkingTimeSheet.BranchId,
                BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                TimeSheetStatus = machineryWorkingTimeSheet.TimeSheetStatus,
                CostSheetApproved = machineryWorkingTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = machineryWorkingTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = machineryWorkingTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = machineryWorkingTimeSheet.CostSheetSendForAproval,
                CreatedBy = machineryWorkingTimeSheet.CreatedBy,
                DateCreated = machineryWorkingTimeSheet.DateCreated,
                LastModified = machineryWorkingTimeSheet.LastModified,
                ModifiedBy = machineryWorkingTimeSheet.ModifiedBy,
                TimeSheetApproved = machineryWorkingTimeSheet.TimeSheetApproved,
                TimeSheetSendForAproval = machineryWorkingTimeSheet.TimeSheetSendForAproval,
                TimeSheetApprovedBy = machineryWorkingTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = machineryWorkingTimeSheet.TimeSheetApprovedDate,
                TimeSheetSendForApprovalBy = machineryWorkingTimeSheet.TimeSheetSendForApprovalBy,
                TimeSheetSendForApprovalTime = machineryWorkingTimeSheet.TimeSheetSendForApprovalTime,
                OperatorId = machineryWorkingTimeSheet.OperatorId,
                GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId

            });
            return Json(result);
        }
        public ActionResult MachineryWorkingTimeSheets_Create([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
        {
            #region valiadtion
            #endregion
            #region data
            var operationAmount = machineryWorkingTimeSheet.RentalRate * machineryWorkingTimeSheet.NetHour;
            var idleAmount = machineryWorkingTimeSheet.IdleRate * machineryWorkingTimeSheet.IdleHours;
            var totalAmountBeforeVAT = operationAmount + idleAmount;
            var totalAmount = totalAmountBeforeVAT * machineryWorkingTimeSheet.VAT;
            var fuelCost = machineryWorkingTimeSheet.FuelConsumption * machineryWorkingTimeSheet.FuelRate;
            var totalCost = fuelCost + machineryWorkingTimeSheet.MaintenanceCost + machineryWorkingTimeSheet.LubricantCost + machineryWorkingTimeSheet.ServiceRecharge;

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new MachineryWorkingTimeSheet
                {
                    FleetsID = machineryWorkingTimeSheet.FleetsID,
                    MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                    Date = machineryWorkingTimeSheet.Date,
                    MorningTimeStart = machineryWorkingTimeSheet.MorningTimeStart,
                    MorningTimeEnd = machineryWorkingTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = machineryWorkingTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = machineryWorkingTimeSheet.NightTimeStart,
                    NightTimeEnd = machineryWorkingTimeSheet.NightTimeEnd,
                    NumberOfHoles = machineryWorkingTimeSheet.NumberOfHoles,
                    HoleDepth = machineryWorkingTimeSheet.HoleDepth,
                    FuelConsumption = machineryWorkingTimeSheet.FuelConsumption,
                    FuelRate = machineryWorkingTimeSheet.FuelRate,
                    GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                    IdleHours = machineryWorkingTimeSheet.IdleHours,
                    IdleReason = machineryWorkingTimeSheet.IdleReason,
                    DownHours = machineryWorkingTimeSheet.DownHours,
                    DownReason = machineryWorkingTimeSheet.DownReason,
                    NetHour = machineryWorkingTimeSheet.NetHour,
                    WorkType = machineryWorkingTimeSheet.WorkType,
                    Remark = machineryWorkingTimeSheet.Remark,
                    RentalRate = machineryWorkingTimeSheet.RentalRate,
                    IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                    ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                    OperationAmount = operationAmount,
                    IdleRate = machineryWorkingTimeSheet.IdleRate,
                    IdleAmount = idleAmount,
                    TotalAmountBeforeVAT = totalAmountBeforeVAT,
                    VAT = machineryWorkingTimeSheet.VAT,
                    TotalAmount = totalAmount,
                    FuelCost = fuelCost,
                    ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,
                    LubricantCost = machineryWorkingTimeSheet.LubricantCost,
                    MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                    TotalCost = totalCost,
                    NetAmount = totalAmount-totalCost,

                    DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                    OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                    Perdium = machineryWorkingTimeSheet.Perdium,
                    Salary = machineryWorkingTimeSheet.Salary,
                    TireCost = machineryWorkingTimeSheet.TireCost,
                    BranchId = machineryWorkingTimeSheet.BranchId,
                    BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                    OperatorId = machineryWorkingTimeSheet.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                    EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId
                };

                db.MachineryWorkingTimeSheets.Add(entity);
                db.SaveChanges();
                machineryWorkingTimeSheet.MachineryWorkingTimeSheetId = entity.MachineryWorkingTimeSheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Machinery working timesheet", "Success", User.Identity.Name, $"created machinery working timesheet {entity.MachineryWorkingTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { machineryWorkingTimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryWorkingTimeSheets_Update([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
        {
            #region data
            var operationAmount = machineryWorkingTimeSheet.RentalRate * machineryWorkingTimeSheet.NetHour;
            var idleAmount = machineryWorkingTimeSheet.IdleRate * machineryWorkingTimeSheet.IdleHours;
            var totalAmountBeforeVAT = operationAmount + idleAmount;
            var totalAmount = totalAmountBeforeVAT * machineryWorkingTimeSheet.VAT;
            var fuelCost = machineryWorkingTimeSheet.FuelConsumption * machineryWorkingTimeSheet.FuelRate;
            var totalCost = fuelCost + machineryWorkingTimeSheet.MaintenanceCost + machineryWorkingTimeSheet.LubricantCost + machineryWorkingTimeSheet.ServiceRecharge;


            TimeSpan morningTime = machineryWorkingTimeSheet.MorningTimeEnd - machineryWorkingTimeSheet.MorningTimeStart;
            TimeSpan afternoonTime = machineryWorkingTimeSheet.AfternoonTimeEnd - machineryWorkingTimeSheet.AfternoonTimeStart;
            TimeSpan nightTime = machineryWorkingTimeSheet.NightTimeEnd - machineryWorkingTimeSheet.NightTimeStart;

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
                var entity = new MachineryWorkingTimeSheet
                {

                    FleetsID = machineryWorkingTimeSheet.FleetsID,
                    MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                    Date = machineryWorkingTimeSheet.Date,
                    MorningTimeStart = machineryWorkingTimeSheet.MorningTimeStart,
                    MorningTimeEnd = machineryWorkingTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = machineryWorkingTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = machineryWorkingTimeSheet.NightTimeStart,
                    NightTimeEnd = machineryWorkingTimeSheet.NightTimeEnd,
                    NumberOfHoles = machineryWorkingTimeSheet.NumberOfHoles,
                    HoleDepth = machineryWorkingTimeSheet.HoleDepth,
                    FuelConsumption = machineryWorkingTimeSheet.FuelConsumption,
                    FuelRate = machineryWorkingTimeSheet.FuelRate,
                    GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                    IdleHours = machineryWorkingTimeSheet.IdleHours,
                    IdleReason = machineryWorkingTimeSheet.IdleReason,
                    DownHours = machineryWorkingTimeSheet.DownHours,
                    DownReason = machineryWorkingTimeSheet.DownReason,
                    NetHour = machineryWorkingTimeSheet.NetHour,
                    WorkType = machineryWorkingTimeSheet.WorkType,
                    Remark = machineryWorkingTimeSheet.Remark,
                    RentalRate = machineryWorkingTimeSheet.RentalRate,
                    IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                    ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                    OperationAmount = operationAmount,
                    IdleRate = machineryWorkingTimeSheet.IdleRate,
                    IdleAmount = idleAmount,
                    TotalAmountBeforeVAT = totalAmountBeforeVAT,
                    VAT = machineryWorkingTimeSheet.VAT,
                    TotalAmount = totalAmount,
                    FuelCost = fuelCost,
                    ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,
                    LubricantCost = machineryWorkingTimeSheet.LubricantCost,
                    MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                    TotalCost = totalCost,
                    NetAmount = totalAmount - totalCost,

                    DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                    OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                    Perdium = machineryWorkingTimeSheet.Perdium,
                    Salary = machineryWorkingTimeSheet.Salary,
                    TireCost = machineryWorkingTimeSheet.TireCost,
                    BranchId = machineryWorkingTimeSheet.BranchId,
                    BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                    OperatorId = machineryWorkingTimeSheet.OperatorId,
                    GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                    EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId
                };

                
                db.MachineryWorkingTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Working machinery registration", "Success", User.Identity.Name, $"updated machinery working timesheet {entity.MachineryWorkingTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { machineryWorkingTimeSheet }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult InsuranceCompanies_Destroy([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachineryWorkingTimeSheet
                {
                    FleetsID = machineryWorkingTimeSheet.FleetsID,
                    MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                    Date = machineryWorkingTimeSheet.Date,
                    MorningTimeStart = machineryWorkingTimeSheet.MorningTimeStart,
                    MorningTimeEnd = machineryWorkingTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = machineryWorkingTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = machineryWorkingTimeSheet.NightTimeStart,
                    NightTimeEnd = machineryWorkingTimeSheet.NightTimeEnd,
                    NumberOfHoles = machineryWorkingTimeSheet.NumberOfHoles,
                    HoleDepth = machineryWorkingTimeSheet.HoleDepth,
                    FuelConsumption = machineryWorkingTimeSheet.FuelConsumption,
                    FuelRate = machineryWorkingTimeSheet.FuelRate,
                    GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                    IdleHours = machineryWorkingTimeSheet.IdleHours,
                    IdleReason = machineryWorkingTimeSheet.IdleReason,
                    DownHours = machineryWorkingTimeSheet.DownHours,
                    DownReason = machineryWorkingTimeSheet.DownReason,
                    NetHour = machineryWorkingTimeSheet.NetHour,
                    WorkType = machineryWorkingTimeSheet.WorkType,
                    Remark = machineryWorkingTimeSheet.Remark,
                    RentalRate = machineryWorkingTimeSheet.RentalRate,
                    IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                    ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                    OperationAmount = machineryWorkingTimeSheet.OperationAmount,
                    IdleRate = machineryWorkingTimeSheet.IdleRate,
                    IdleAmount = machineryWorkingTimeSheet.IdleAmount,
                    TotalAmountBeforeVAT = machineryWorkingTimeSheet.TotalAmountBeforeVAT,
                    VAT = machineryWorkingTimeSheet.VAT,
                    TotalAmount = machineryWorkingTimeSheet.TotalAmount,
                    FuelCost = machineryWorkingTimeSheet.FuelCost,
                    ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,
                    LubricantCost = machineryWorkingTimeSheet.LubricantCost,
                    MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                    TotalCost = machineryWorkingTimeSheet.TotalCost,
                    NetAmount = machineryWorkingTimeSheet.NetAmount,

                    DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                    OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                    Perdium = machineryWorkingTimeSheet.Perdium,
                    Salary = machineryWorkingTimeSheet.Salary,
                    TireCost = machineryWorkingTimeSheet.TireCost,

                    OperatorId = machineryWorkingTimeSheet.OperatorId,
                    GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                    EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId
                };

                db.MachineryWorkingTimeSheets.Attach(entity);
                db.MachineryWorkingTimeSheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted working machinery timesheet {entity.MachineryWorkingTimeSheetId }");
                #endregion
            }

            return Json(new[] { machineryWorkingTimeSheet }.ToDataSourceResult(request, ModelState));
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
        public JsonResult TimeSheetSendForApproval(int machineryWorkingTimeSheetId)
        {

            var machineryTimeSheet = db.MachineryWorkingTimeSheets.Find(machineryWorkingTimeSheetId);

            if (machineryTimeSheet != null)
            {
                machineryTimeSheet.TimeSheetSendForAproval = true;
                machineryTimeSheet.TimeSheetSendForApprovalTime = DateTime.Now;
                machineryTimeSheet.TimeSheetSendForApprovalBy = User.Identity.Name;
                machineryTimeSheet.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.MachineryWorkingTimeSheets.Attach(machineryTimeSheet);
                db.Entry(machineryTimeSheet).State = EntityState.Modified;
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
        public ActionResult MachineryTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == id && x.TimeSheetType == TimeSheetType.Machinery);
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
                TimeSheetApprovalNewId = model.TimeSheetApprovalNewId,
                TimeSheetType = model.TimeSheetType
            });
            return Json(result);
        }
        public ActionResult MachineryTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int id)
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
                model.TimeSheetType = TimeSheetType.Machinery;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var machineryTimeSheet = db.MachineryWorkingTimeSheets.Find(id);
            if (machineryTimeSheet != null)
            {
                machineryTimeSheet.TimeSheetApproved = true;
                machineryTimeSheet.TimeSheetStatus = MaintenanceConst.Approved;
                machineryTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                machineryTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(machineryTimeSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var machineryTimeSheet = db.MachineryWorkingTimeSheets.Find(id);
            if (machineryTimeSheet != null)
            {
                machineryTimeSheet.TimeSheetApproved = false;
                machineryTimeSheet.TimeSheetStatus = MaintenanceConst.Rejected;
                machineryTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                machineryTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(machineryTimeSheet).State = EntityState.Modified;
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