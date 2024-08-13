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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    [AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class WagonDrillTimesheetController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();        
        public ActionResult Index(TimeSheetType wagonDrill)
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

            ViewBag.wagonDrill = wagonDrill;
            ViewData["fleet"] = db.Fleets.Select(x => new
            {
                Value = x.FleetsID,
                Text = x.EquipmentName + "-" + x.SerialNo
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
            var timesheets = db.WagonDrillTimesheets.ToList();
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
        public ActionResult WagonDrillTimesheets_Read([DataSourceRequest] DataSourceRequest request)
        {
            var wagonDrillTimesheets = db.WagonDrillTimesheets;
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
                CostSheetApprovedBy = wagonDrillTimesheet.CostSheetApprovedBy,
                CostSheetApprovedDate = wagonDrillTimesheet.CostSheetApprovedDate,
                CostSheetSendForAproval = wagonDrillTimesheet.CostSheetSendForAproval,
                
                TimeSheetStatus = wagonDrillTimesheet.TimeSheetStatus,
                TimeSheetApproved = wagonDrillTimesheet.TimeSheetApproved,
                TimeSheetApprovedBy = wagonDrillTimesheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = wagonDrillTimesheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval =   wagonDrillTimesheet.TimeSheetSendForAproval,
                OperatorId = wagonDrillTimesheet.OperatorId,
                ReasonForDown = wagonDrillTimesheet.ReasonForDown,
                ReasonForIdle = wagonDrillTimesheet.ReasonForIdle,
                //EquipmentCategoryId = wagonDrillTimesheet.EquipmentCategoryId,
                //EquipmentTypeId = wagonDrillTimesheet.EquipmentTypeId,
                //EquipmentNameId = wagonDrillTimesheet.EquipmentNameId
            });
            return Json(result);
        }
        public ActionResult WagonDrillTimesheets_Create([DataSourceRequest] DataSourceRequest request, WagonDrillTimesheet wagonDrillTimesheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new WagonDrillTimesheet
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
                    DateCreated = DateTime.Now.Date,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = wagonDrillTimesheet.ModifiedBy,
                    LastModified = wagonDrillTimesheet.LastModified,
                    OperationAmount = wagonDrillTimesheet.OperationAmount,
                    IdleAmount = wagonDrillTimesheet.IdleAmount,
                    TotalAmountBeforeVAT = wagonDrillTimesheet.TotalAmountBeforeVAT,
                    TotalAmount = wagonDrillTimesheet.TotalAmount,
                    FuelCost = wagonDrillTimesheet.FuelCost,
                    TotalCost = wagonDrillTimesheet.TotalCost,
                    NetAmount = wagonDrillTimesheet.NetAmount,
                    Perdium = wagonDrillTimesheet.Perdium,
                    Salary = wagonDrillTimesheet.Salary,
                    BranchId = wagonDrillTimesheet.BranchId,
                    BusinessUnitId = wagonDrillTimesheet.BusinessUnitId,
                    OperatorId = wagonDrillTimesheet.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    ReasonForDown = wagonDrillTimesheet.ReasonForDown,
                    ReasonForIdle = wagonDrillTimesheet.ReasonForIdle,
                    //EquipmentCategoryId = wagonDrillTimesheet.EquipmentCategoryId,
                    //EquipmentTypeId = wagonDrillTimesheet.EquipmentTypeId,
                    //EquipmentNameId = wagonDrillTimesheet.EquipmentNameId
                };
                db.WagonDrillTimesheets.Add(entity);
                db.SaveChanges();
                wagonDrillTimesheet.WagonDrillTimesheetId = entity.WagonDrillTimesheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dry loading timesheet registration", "Success", User.Identity.Name, $"created dry loading timesheet {entity.WagonDrillTimesheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { wagonDrillTimesheet }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WagonDrillTimesheets_Update([DataSourceRequest] DataSourceRequest request, WagonDrillTimesheet wagonDrillTimesheet)
        {
          if (ModelState.IsValid)
            {
                var entity = new WagonDrillTimesheet
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
                    ModifiedBy =User.Identity.Name,
                    LastModified =DateTime.Now.Date,
                    OperationAmount = wagonDrillTimesheet.OperationAmount,
                    IdleAmount = wagonDrillTimesheet.IdleAmount,
                    TotalAmountBeforeVAT = wagonDrillTimesheet.TotalAmountBeforeVAT,
                    TotalAmount = wagonDrillTimesheet.TotalAmount,
                    FuelCost = wagonDrillTimesheet.FuelCost,
                    TotalCost = wagonDrillTimesheet.TotalCost,
                    NetAmount = wagonDrillTimesheet.NetAmount,
                    Perdium = wagonDrillTimesheet.Perdium,
                    Salary = wagonDrillTimesheet.Salary,
                    BranchId = wagonDrillTimesheet.BranchId,
                    BusinessUnitId = wagonDrillTimesheet.BusinessUnitId,
                    OperatorId = wagonDrillTimesheet.OperatorId,
                    ReasonForDown = wagonDrillTimesheet.ReasonForDown,
                    ReasonForIdle = wagonDrillTimesheet.ReasonForIdle,
                    //EquipmentCategoryId = wagonDrillTimesheet.EquipmentCategoryId,
                    //EquipmentTypeId = wagonDrillTimesheet.EquipmentTypeId,
                    //EquipmentNameId = wagonDrillTimesheet.EquipmentNameId
                };
                db.WagonDrillTimesheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Wagon drill timesheet update", "Success", User.Identity.Name, $"updated wagon drill timesheet {entity.WagonDrillTimesheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { wagonDrillTimesheet }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult WagonDrillTimesheets_Destroy([DataSourceRequest] DataSourceRequest request, WagonDrillTimesheet wagonDrillTimesheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new WagonDrillTimesheet
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
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now.Date,
                    OperationAmount = wagonDrillTimesheet.OperationAmount,
                    IdleAmount = wagonDrillTimesheet.IdleAmount,
                    TotalAmountBeforeVAT = wagonDrillTimesheet.TotalAmountBeforeVAT,
                    TotalAmount = wagonDrillTimesheet.TotalAmount,
                    FuelCost = wagonDrillTimesheet.FuelCost,
                    TotalCost = wagonDrillTimesheet.TotalCost,
                    NetAmount = wagonDrillTimesheet.NetAmount,
                    Perdium = wagonDrillTimesheet.Perdium,
                    Salary = wagonDrillTimesheet.Salary,
                    BranchId = wagonDrillTimesheet.BranchId,
                    BusinessUnitId = wagonDrillTimesheet.BusinessUnitId,
                    OperatorId = wagonDrillTimesheet.OperatorId,
                    ReasonForDown = wagonDrillTimesheet.ReasonForDown,
                    ReasonForIdle = wagonDrillTimesheet.ReasonForIdle,
                    //EquipmentCategoryId = wagonDrillTimesheet.EquipmentCategoryId,
                    //EquipmentTypeId = wagonDrillTimesheet.EquipmentTypeId,
                    //EquipmentNameId = wagonDrillTimesheet.EquipmentNameId
                };
                db.WagonDrillTimesheets.Attach(entity);
                db.WagonDrillTimesheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted wagon drill timesheet {entity.WagonDrillTimesheetId }");
                #endregion
            }
            return Json(new[] { wagonDrillTimesheet }.ToDataSourceResult(request, ModelState));
        }



        //-------------------------------------approve---------------------------------------//


        #region Approval
        public JsonResult TimeSheetSendForApproval(int id)
        {

            var wagonDrillTimesheets = db.WagonDrillTimesheets.Find(id);

            if (wagonDrillTimesheets != null)
            {
                wagonDrillTimesheets.TimeSheetSendForAproval = true;
                wagonDrillTimesheets.TimeSheetSendForApprovalTime = DateTime.Now;
                wagonDrillTimesheets.TimeSheetSendForApprovalBy = User.Identity.Name;
                wagonDrillTimesheets.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.WagonDrillTimesheets.Attach(wagonDrillTimesheets);
                db.Entry(wagonDrillTimesheets).State = EntityState.Modified;
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
        public ActionResult WagonDrillTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == id && x.TimeSheetType == TimeSheetType.WagonDrill);
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
        public ActionResult WagonDrillTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int id)
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
                model.TimeSheetType = TimeSheetType.WagonDrill;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var wagonDrillTimesheets = db.WagonDrillTimesheets.Find(id);
            if (wagonDrillTimesheets != null)
            {
                wagonDrillTimesheets.TimeSheetApproved = true;
                wagonDrillTimesheets.TimeSheetStatus = MaintenanceConst.Approved;
                wagonDrillTimesheets.TimeSheetApprovedBy = User.Identity.Name;
                wagonDrillTimesheets.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(wagonDrillTimesheets).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var wagonDrillTimesheets = db.WagonDrillTimesheets.Find(id);
            if (wagonDrillTimesheets != null)
            {
                wagonDrillTimesheets.TimeSheetApproved = false;
                wagonDrillTimesheets.TimeSheetStatus = MaintenanceConst.Rejected;
                wagonDrillTimesheets.TimeSheetApprovedBy = User.Identity.Name;
                wagonDrillTimesheets.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(wagonDrillTimesheets).State = EntityState.Modified;
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