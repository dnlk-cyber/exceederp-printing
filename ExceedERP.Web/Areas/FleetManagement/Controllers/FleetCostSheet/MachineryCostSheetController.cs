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
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetCostSheet
{
    public class MachineryCostSheetController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            ViewData["fleet"] = db.Fleets.Select(x => new
            {
                Value = x.FleetsID,
                Text = x.SerialNo
            }).ToList();

            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();

            return View();
        }
       
        public ActionResult MachineryCostSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
          
            var machineryWorkingTimeSheets = db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved);           
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
                CostSheetApproved = machineryWorkingTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = machineryWorkingTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = machineryWorkingTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = machineryWorkingTimeSheet.CostSheetSendForAproval,
                CreatedBy = machineryWorkingTimeSheet.CreatedBy,
                DateCreated = machineryWorkingTimeSheet.DateCreated,
                LastModified = machineryWorkingTimeSheet.LastModified,
                ModifiedBy = machineryWorkingTimeSheet.ModifiedBy,
                TimeSheetApproved = machineryWorkingTimeSheet.TimeSheetApproved,
                TimeSheetApprovedBy = machineryWorkingTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = machineryWorkingTimeSheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval = machineryWorkingTimeSheet.TimeSheetSendForAproval,
                TimeSheetStatus = machineryWorkingTimeSheet.TimeSheetStatus,
                CostSheetStatus = machineryWorkingTimeSheet.CostSheetStatus

            });
            return Json(result);
        }
        

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryCostSheet_Update([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
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
                    BranchId = machineryWorkingTimeSheet.BranchId,
                    BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                    CostSheetApproved = machineryWorkingTimeSheet.CostSheetApproved,
                    CostSheetApprovedBy = machineryWorkingTimeSheet.CostSheetApprovedBy,
                    CostSheetApprovedDate = machineryWorkingTimeSheet.CostSheetApprovedDate,
                    CostSheetSendForAproval = machineryWorkingTimeSheet.CostSheetSendForAproval,
                    CreatedBy = machineryWorkingTimeSheet.CreatedBy,
                    DateCreated = machineryWorkingTimeSheet.DateCreated,
                    LastModified = machineryWorkingTimeSheet.LastModified,
                    ModifiedBy = machineryWorkingTimeSheet.ModifiedBy,
                    TimeSheetApproved = machineryWorkingTimeSheet.TimeSheetApproved,
                    TimeSheetApprovedBy = machineryWorkingTimeSheet.TimeSheetApprovedBy,
                    TimeSheetApprovedDate = machineryWorkingTimeSheet.TimeSheetApprovedDate,
                    TimeSheetSendForAproval = machineryWorkingTimeSheet.TimeSheetSendForAproval,
                    TimeSheetStatus = machineryWorkingTimeSheet.TimeSheetStatus,
                    CostSheetStatus = machineryWorkingTimeSheet.CostSheetStatus,
                    
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


        public ActionResult MachineryCostSheet_Destroy([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
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
                    BranchId = machineryWorkingTimeSheet.BranchId,
                    BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                    CostSheetApproved = machineryWorkingTimeSheet.CostSheetApproved,
                    CostSheetApprovedBy = machineryWorkingTimeSheet.CostSheetApprovedBy,
                    CostSheetApprovedDate = machineryWorkingTimeSheet.CostSheetApprovedDate,
                    CostSheetSendForAproval = machineryWorkingTimeSheet.CostSheetSendForAproval,
                    CreatedBy = machineryWorkingTimeSheet.CreatedBy,
                    DateCreated = machineryWorkingTimeSheet.DateCreated,
                    LastModified = machineryWorkingTimeSheet.LastModified,
                    ModifiedBy = machineryWorkingTimeSheet.ModifiedBy,
                    TimeSheetApproved = machineryWorkingTimeSheet.TimeSheetApproved,
                    TimeSheetApprovedBy = machineryWorkingTimeSheet.TimeSheetApprovedBy,
                    TimeSheetApprovedDate = machineryWorkingTimeSheet.TimeSheetApprovedDate,
                    TimeSheetSendForAproval = machineryWorkingTimeSheet.TimeSheetSendForAproval,
                    TimeSheetStatus = machineryWorkingTimeSheet.TimeSheetStatus,
                    CostSheetStatus = machineryWorkingTimeSheet.CostSheetStatus


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



        //-----------------------------------------------------------Approval------------------------------------------------------------------//

        #region Approval
        public JsonResult CostSheetSendForApproval(int machineryWorkingTimeSheetId)
        {

            var machineryCostSheet = db.MachineryWorkingTimeSheets.Find(machineryWorkingTimeSheetId);

            if (machineryCostSheet != null)
            {
                machineryCostSheet.CostSheetSendForAproval = true;
                machineryCostSheet.CostSheetSendForApprovalTime = DateTime.Now;
                machineryCostSheet.CostSheetSendForApprovalBy = User.Identity.Name;
                machineryCostSheet.CostSheetStatus = MaintenanceConst.WaitingApproval;

                db.MachineryWorkingTimeSheets.Attach(machineryCostSheet);
                db.Entry(machineryCostSheet).State = EntityState.Modified;
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
        public ActionResult MachineryCostSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.CostSheetApprovals.Where(x => x.CostSheetId == id && x.TimeSheetType == TimeSheetType.Machinery);
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
        public ActionResult MachineryCostSheetApproval_Create([DataSourceRequest] DataSourceRequest request, CostSheetApproval model, int id)
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
                model.TimeSheetType = TimeSheetType.Machinery;
                db.CostSheetApprovals.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var machineryCostSheet = db.MachineryWorkingTimeSheets.Find(id);
            if (machineryCostSheet != null)
            {
                machineryCostSheet.CostSheetApproved = true;
                machineryCostSheet.CostSheetStatus = MaintenanceConst.Approved;
                machineryCostSheet.CostSheetApprovedBy = User.Identity.Name;
                machineryCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(machineryCostSheet).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private void Rejected(int id)
        {
            var machineryCostSheet = db.MachineryWorkingTimeSheets.Find(id);
            if (machineryCostSheet != null)
            {
                machineryCostSheet.CostSheetApproved = false;
                machineryCostSheet.CostSheetStatus = MaintenanceConst.Rejected;
                machineryCostSheet.CostSheetApprovedBy = User.Identity.Name;
                machineryCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(machineryCostSheet).State = EntityState.Modified;
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