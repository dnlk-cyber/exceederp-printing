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
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetCostSheet
{
    [AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class WagonDrillCostSheetController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();        
        public ActionResult Index()
        {            
            ViewData["fleet"] = db.Fleets.Select(x => new { x.FleetsID, x.EquipmentName }).ToList();
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            return View();
        }
        public ActionResult WagonDrillCostSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            var wagonDrillTimesheets = db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved);
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
                OperationAmount=wagonDrillTimesheet.OperationAmount,
                IdleAmount=wagonDrillTimesheet.IdleAmount,
                TotalAmountBeforeVAT=wagonDrillTimesheet.TotalAmountBeforeVAT,
                TotalAmount=wagonDrillTimesheet.TotalAmount,
                FuelCost=wagonDrillTimesheet.FuelCost,
                TotalCost=wagonDrillTimesheet.TotalCost,
                NetAmount=wagonDrillTimesheet.NetAmount,
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
                CostSheetApprovedBy =wagonDrillTimesheet.CostSheetApprovedBy,
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
                ReasonForIdle = wagonDrillTimesheet.ReasonForIdle
                
            });
            return Json(result);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WagonDrillCostSheet_Update([DataSourceRequest] DataSourceRequest request, WagonDrillTimesheet wagonDrillTimesheet)
        {
          if (ModelState.IsValid)
            {
                
                db.WagonDrillTimesheets.Attach(wagonDrillTimesheet);
                db.Entry(wagonDrillTimesheet).State = EntityState.Modified;

                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Wagon drill timesheet update", "Success", User.Identity.Name, $"updated wagon drill timesheet {wagonDrillTimesheet.WagonDrillTimesheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { wagonDrillTimesheet }.ToDataSourceResult(request, ModelState));
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