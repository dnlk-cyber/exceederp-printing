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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetTimeSheetSimple
{
    [AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class DumpTruckTimeSheetSimpleController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Index()
        {
            ViewData["Fleets"] = db.Fleets.Select(x => new { x.FleetsID, x.PlateNumber }).ToList();
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();


            return View();
        }



        public ActionResult DumpTruckTimeSheets_Read([DataSourceRequest] DataSourceRequest request)
        {
            var dumpTruckTimeSheets = db.DumpTruckTimeSheets;

            DataSourceResult result = dumpTruckTimeSheets.ToDataSourceResult(request, dumpTruckTimeSheet => new DumpTruckTimeSheet
            {
                DumpTruckTimeSheetId = dumpTruckTimeSheet.DumpTruckTimeSheetId,
                FleetsID = dumpTruckTimeSheet.FleetsID,
                ProcurementSupplierID = dumpTruckTimeSheet.ProcurementSupplierID,
                Date = dumpTruckTimeSheet.Date,
                TypeOfWorkExcuted=dumpTruckTimeSheet.TypeOfWorkExcuted,
                LoadingLocationFrom=dumpTruckTimeSheet.LoadingLocationFrom,
                LoadingLOactionTo=dumpTruckTimeSheet.LoadingLOactionTo,
                Capacity=dumpTruckTimeSheet.Capacity,
                ActualDistance=dumpTruckTimeSheet.ActualDistance,
                NumberOfTrip=dumpTruckTimeSheet.NumberOfTrip,
                TotalHaulage = dumpTruckTimeSheet.TotalHaulage,
                FuelConsumption=dumpTruckTimeSheet.FuelConsumption,
                OilConsumption = dumpTruckTimeSheet.OilConsumption,
                GreaseConsumption=dumpTruckTimeSheet.GreaseConsumption,
                IdleHour= dumpTruckTimeSheet.IdleHour,
                ReasonForIdle=dumpTruckTimeSheet.ReasonForIdle,
                DownHour = dumpTruckTimeSheet.DownHour,
                ReasonForDown = dumpTruckTimeSheet.ReasonForDown,
                DateCreated = dumpTruckTimeSheet.DateCreated,
                CreatedBy = dumpTruckTimeSheet.CreatedBy,
                ModifiedBy = dumpTruckTimeSheet.ModifiedBy,
                LastModified = dumpTruckTimeSheet.LastModified,
                RentalRate=dumpTruckTimeSheet.RentalRate,
                IdealRentalRate=dumpTruckTimeSheet.IdealRentalRate,
                LubricantCost = dumpTruckTimeSheet.LubricantCost,
                FuelRate = dumpTruckTimeSheet.FuelRate,
                MaintenanceCost = dumpTruckTimeSheet.MaintenanceCost,
                NetworkingHour = dumpTruckTimeSheet.NetworkingHour,
                Perdium = dumpTruckTimeSheet.Perdium,
                Salary = dumpTruckTimeSheet.Salary,
                TireCost = dumpTruckTimeSheet.TireCost,
                BranchId = dumpTruckTimeSheet.BranchId,
                BusinessUnitId = dumpTruckTimeSheet.BusinessUnitId,
                CostSheetApproved = dumpTruckTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = dumpTruckTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = dumpTruckTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = dumpTruckTimeSheet.CostSheetSendForAproval,
                TimeSheetStatus = dumpTruckTimeSheet.TimeSheetStatus,
                TimeSheetApproved = dumpTruckTimeSheet.TimeSheetApproved,
                TimeSheetApprovedBy = dumpTruckTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = dumpTruckTimeSheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval = dumpTruckTimeSheet.TimeSheetSendForAproval,
                OperatorId = dumpTruckTimeSheet.OperatorId,
                ExternalOperator = dumpTruckTimeSheet.ExternalOperator

            });

            return Json(result);
        }
        public ActionResult DumpTruckTimeSheets_Create([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheet dumpTruckTimeSheet)
        {


            if (ModelState.IsValid)
            {
                var entity = new DumpTruckTimeSheet
                {
                    DumpTruckTimeSheetId = dumpTruckTimeSheet.DumpTruckTimeSheetId,
                    FleetsID = dumpTruckTimeSheet.FleetsID,
                    ProcurementSupplierID = dumpTruckTimeSheet.ProcurementSupplierID,
                    Date = dumpTruckTimeSheet.Date,
                    TypeOfWorkExcuted = dumpTruckTimeSheet.TypeOfWorkExcuted,
                    LoadingLocationFrom = dumpTruckTimeSheet.LoadingLocationFrom,
                    LoadingLOactionTo = dumpTruckTimeSheet.LoadingLOactionTo,
                    Capacity = dumpTruckTimeSheet.Capacity,
                    ActualDistance = dumpTruckTimeSheet.ActualDistance,
                    NumberOfTrip = dumpTruckTimeSheet.NumberOfTrip,
                    TotalHaulage = dumpTruckTimeSheet.TotalHaulage,
                    FuelConsumption = dumpTruckTimeSheet.FuelConsumption,
                    OilConsumption = dumpTruckTimeSheet.OilConsumption,
                    GreaseConsumption = dumpTruckTimeSheet.GreaseConsumption,
                    IdleHour = dumpTruckTimeSheet.IdleHour,
                    ReasonForIdle = dumpTruckTimeSheet.ReasonForIdle,
                    DownHour = dumpTruckTimeSheet.DownHour,
                    ReasonForDown = dumpTruckTimeSheet.ReasonForDown,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = dumpTruckTimeSheet.ModifiedBy,
                    LastModified = dumpTruckTimeSheet.LastModified,
                    RentalRate = dumpTruckTimeSheet.RentalRate,
                    IdealRentalRate = dumpTruckTimeSheet.IdealRentalRate,
                    // added
                    LubricantCost = dumpTruckTimeSheet.LubricantCost,
                    FuelRate = dumpTruckTimeSheet.FuelRate,
                    MaintenanceCost = dumpTruckTimeSheet.MaintenanceCost,
                    NetworkingHour = dumpTruckTimeSheet.NetworkingHour,
                    Perdium = dumpTruckTimeSheet.Perdium,
                    Salary = dumpTruckTimeSheet.Salary,
                    TireCost = dumpTruckTimeSheet.TireCost,
                    BranchId = dumpTruckTimeSheet.BranchId,
                    BusinessUnitId = dumpTruckTimeSheet.BusinessUnitId,
                    OperatorId = dumpTruckTimeSheet.OperatorId,
                    ExternalOperator = dumpTruckTimeSheet.ExternalOperator
                };

                db.DumpTruckTimeSheets.Add(entity);
                db.SaveChanges();
                dumpTruckTimeSheet.DumpTruckTimeSheetId = entity.DumpTruckTimeSheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dump truck timesheet registration", "Success", User.Identity.Name, $"created dump truck timesheet {entity.DumpTruckTimeSheetId} ", Modules.FleetManagement);
                #endregion

            }

            return Json(new[] { dumpTruckTimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DumpTruckTimeSheets_Update([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheet dumpTruckTimeSheet)
        {
          if (ModelState.IsValid)
            {
                var entity = new DumpTruckTimeSheet
                {
                    DumpTruckTimeSheetId = dumpTruckTimeSheet.DumpTruckTimeSheetId,
                    FleetsID = dumpTruckTimeSheet.FleetsID,
                    ProcurementSupplierID = dumpTruckTimeSheet.ProcurementSupplierID,
                    Date = dumpTruckTimeSheet.Date,
                    TypeOfWorkExcuted = dumpTruckTimeSheet.TypeOfWorkExcuted,
                    LoadingLocationFrom = dumpTruckTimeSheet.LoadingLocationFrom,
                    LoadingLOactionTo = dumpTruckTimeSheet.LoadingLOactionTo,
                    Capacity = dumpTruckTimeSheet.Capacity,
                    ActualDistance = dumpTruckTimeSheet.ActualDistance,
                    NumberOfTrip = dumpTruckTimeSheet.NumberOfTrip,
                    TotalHaulage = dumpTruckTimeSheet.TotalHaulage,
                    FuelConsumption = dumpTruckTimeSheet.FuelConsumption,
                    OilConsumption = dumpTruckTimeSheet.OilConsumption,
                    GreaseConsumption = dumpTruckTimeSheet.GreaseConsumption,
                    IdleHour = dumpTruckTimeSheet.IdleHour,
                    ReasonForIdle = dumpTruckTimeSheet.ReasonForIdle,
                    DownHour = dumpTruckTimeSheet.DownHour,
                    ReasonForDown = dumpTruckTimeSheet.ReasonForDown,
                    DateCreated = dumpTruckTimeSheet.DateCreated,
                    CreatedBy = dumpTruckTimeSheet.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    RentalRate = dumpTruckTimeSheet.RentalRate,
                    IdealRentalRate = dumpTruckTimeSheet.IdealRentalRate,
                    LubricantCost = dumpTruckTimeSheet.LubricantCost,
                    FuelRate = dumpTruckTimeSheet.FuelRate,
                    MaintenanceCost = dumpTruckTimeSheet.MaintenanceCost,
                    NetworkingHour = dumpTruckTimeSheet.NetworkingHour,
                    Perdium = dumpTruckTimeSheet.Perdium,
                    Salary = dumpTruckTimeSheet.Salary,
                    TireCost = dumpTruckTimeSheet.TireCost,
                    BranchId = dumpTruckTimeSheet.BranchId,
                    BusinessUnitId = dumpTruckTimeSheet.BusinessUnitId,
                    OperatorId = dumpTruckTimeSheet.OperatorId,
                    ExternalOperator = dumpTruckTimeSheet.ExternalOperator

                };

                db.DumpTruckTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dump truck timesheet update", "Success", User.Identity.Name, $"updated dump truck timesheet {entity.DumpTruckTimeSheetId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { dumpTruckTimeSheet }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult DumpTruckTimeSheets_Destroy([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheet dumpTruckTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new DumpTruckTimeSheet
                {
                    DumpTruckTimeSheetId = dumpTruckTimeSheet.DumpTruckTimeSheetId,
                    FleetsID = dumpTruckTimeSheet.FleetsID,
                    ProcurementSupplierID = dumpTruckTimeSheet.ProcurementSupplierID,
                    Date = dumpTruckTimeSheet.Date,
                    TypeOfWorkExcuted = dumpTruckTimeSheet.TypeOfWorkExcuted,
                    LoadingLocationFrom = dumpTruckTimeSheet.LoadingLocationFrom,
                    LoadingLOactionTo = dumpTruckTimeSheet.LoadingLOactionTo,
                    Capacity = dumpTruckTimeSheet.Capacity,
                    ActualDistance = dumpTruckTimeSheet.ActualDistance,
                    NumberOfTrip = dumpTruckTimeSheet.NumberOfTrip,
                    TotalHaulage = dumpTruckTimeSheet.TotalHaulage,
                    FuelConsumption = dumpTruckTimeSheet.FuelConsumption,
                    OilConsumption = dumpTruckTimeSheet.OilConsumption,
                    GreaseConsumption = dumpTruckTimeSheet.GreaseConsumption,
                    IdleHour = dumpTruckTimeSheet.IdleHour,
                    ReasonForIdle = dumpTruckTimeSheet.ReasonForIdle,
                    DownHour = dumpTruckTimeSheet.DownHour,
                    ReasonForDown = dumpTruckTimeSheet.ReasonForDown,
                    DateCreated = dumpTruckTimeSheet.DateCreated,
                    CreatedBy = dumpTruckTimeSheet.CreatedBy,
                    ModifiedBy = dumpTruckTimeSheet.ModifiedBy,
                    LastModified = dumpTruckTimeSheet.LastModified,
                    RentalRate = dumpTruckTimeSheet.RentalRate,
                    IdealRentalRate = dumpTruckTimeSheet.IdealRentalRate,
                    LubricantCost = dumpTruckTimeSheet.LubricantCost,
                    FuelRate = dumpTruckTimeSheet.FuelRate,
                    MaintenanceCost = dumpTruckTimeSheet.MaintenanceCost,
                    NetworkingHour = dumpTruckTimeSheet.NetworkingHour,
                    Perdium = dumpTruckTimeSheet.Perdium,
                    Salary = dumpTruckTimeSheet.Salary,
                    TireCost = dumpTruckTimeSheet.TireCost,
                    BranchId = dumpTruckTimeSheet.BranchId,
                    BusinessUnitId = dumpTruckTimeSheet.BusinessUnitId,
                    OperatorId = dumpTruckTimeSheet.OperatorId,
                    ExternalOperator = dumpTruckTimeSheet.ExternalOperator

                };

                db.DumpTruckTimeSheets.Attach(entity);
                db.DumpTruckTimeSheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted dump truck timesheet {entity.DumpTruckTimeSheetId }");
                #endregion
            }

            return Json(new[] { dumpTruckTimeSheet }.ToDataSourceResult(request, ModelState));
        }


        //---------------------------------- Approval Section --------------------------------------//








        #region Approval
        public JsonResult TimeSheetSendForApproval(int id)
        {

            var dumpTruckTimeSheet = db.DumpTruckTimeSheets.Find(id);

            if (dumpTruckTimeSheet != null)
            {
                dumpTruckTimeSheet.TimeSheetSendForAproval = true;
                dumpTruckTimeSheet.TimeSheetSendForApprovalTime = DateTime.Now;
                dumpTruckTimeSheet.TimeSheetSendForApprovalBy = User.Identity.Name;
                dumpTruckTimeSheet.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.DumpTruckTimeSheets.Attach(dumpTruckTimeSheet);
                db.Entry(dumpTruckTimeSheet).State = EntityState.Modified;
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
        public ActionResult DumpTruckTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == id);
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
                TimeSheetApprovalNewId = model.TimeSheetApprovalNewId
            });
            return Json(result);
        }
        public ActionResult DumpTruckTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int id)
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
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var dumpTruckTimeSheet = db.DumpTruckTimeSheets.Find(id);
            if (dumpTruckTimeSheet != null)
            {
                dumpTruckTimeSheet.TimeSheetApproved = true;
                dumpTruckTimeSheet.TimeSheetStatus = MaintenanceConst.Approved;
                dumpTruckTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                dumpTruckTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(dumpTruckTimeSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var dumpTruckTimeSheet = db.DumpTruckTimeSheets.Find(id);
            if (dumpTruckTimeSheet != null)
            {
                dumpTruckTimeSheet.TimeSheetApproved = false;
                dumpTruckTimeSheet.TimeSheetStatus = MaintenanceConst.Rejected;
                dumpTruckTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                dumpTruckTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(dumpTruckTimeSheet).State = EntityState.Modified;
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