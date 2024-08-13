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
    public class DryLoadingTimeSheetsController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index(TimeSheetType dryLoading)
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
            ViewData["fleet"] = db.Fleets.Select(x => new { x.FleetsID, x.PlateNumber }).ToList();
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            ViewBag.dryLoading = dryLoading;
            ViewData["branch"] = db.Branch.Select(s => new
            {
                Value = s.BranchId,
                Text = s.Name
            }).ToList();
            RepopulateTimeSheet();
            return View();
        }
        public void RepopulateTimeSheet()
        {
            var timesheets = db.DryLoadingTimeSheets.ToList();
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
        public ActionResult DryLoadingTimeSheets_Read([DataSourceRequest] DataSourceRequest request)
        {
            var dryLoadingTimeSheets = db.DryLoadingTimeSheets;
            DataSourceResult result = dryLoadingTimeSheets.ToDataSourceResult(request, dryLoadingTimeSheet => new DryLoadingTimeSheet
            {
                DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                ProcurementSupplierID = dryLoadingTimeSheet.ProcurementSupplierID,
                FleetsID = dryLoadingTimeSheet.FleetsID,
                WorkItem = dryLoadingTimeSheet.WorkItem,
                WorkingHour = dryLoadingTimeSheet.WorkingHour,
                TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,
                TripStartDate = dryLoadingTimeSheet.TripStartDate,
                TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                Asphalt = dryLoadingTimeSheet.Asphalt,
                Gravel = dryLoadingTimeSheet.Gravel,
                TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                PerDiem = dryLoadingTimeSheet.PerDiem,
                IdleHour = dryLoadingTimeSheet.IdleHour,
                ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,
                DateCreated = dryLoadingTimeSheet.DateCreated,
                CreatedBy = dryLoadingTimeSheet.CreatedBy,
                ModifiedBy = dryLoadingTimeSheet.ModifiedBy,
                LastModified = dryLoadingTimeSheet.LastModified,

                AsphaltRate = dryLoadingTimeSheet.AsphaltRate,
                FuelRate = dryLoadingTimeSheet.FuelRate,
                GravelRate = dryLoadingTimeSheet.GravelRate,
                LoadedType = dryLoadingTimeSheet.LoadedType,
                LoadingUnloadingCost = dryLoadingTimeSheet.LoadingUnloadingCost,
                LubricantCost = dryLoadingTimeSheet.LubricantCost,
                MaintenanceCost = dryLoadingTimeSheet.MaintenanceCost,
                NetworkingHour = dryLoadingTimeSheet.NetworkingHour,
                NoOfTripe = dryLoadingTimeSheet.NoOfTripe,
                Perdium = dryLoadingTimeSheet.Perdium,
                ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                Salary = dryLoadingTimeSheet.Salary,
                TireCost = dryLoadingTimeSheet.TireCost,
                BranchId = dryLoadingTimeSheet.BranchId,
                BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                Category = dryLoadingTimeSheet.Category,
                CostSheetApproved = dryLoadingTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = dryLoadingTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = dryLoadingTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = dryLoadingTimeSheet.CostSheetSendForAproval,

                TimeSheetStatus = dryLoadingTimeSheet.TimeSheetStatus,

                TimeSheetApproved = dryLoadingTimeSheet.TimeSheetApproved,
                TimeSheetApprovedBy = dryLoadingTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = dryLoadingTimeSheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval = dryLoadingTimeSheet.TimeSheetSendForAproval,
                OperatorId = dryLoadingTimeSheet.OperatorId,
                Downhour = dryLoadingTimeSheet.Downhour,
               EquipmentCategoryId = dryLoadingTimeSheet.EquipmentCategoryId,
               EquipmentTypeId = dryLoadingTimeSheet.EquipmentTypeId,
               EquipmentNameId = dryLoadingTimeSheet.EquipmentNameId

            });
            return Json(result);
        }
        public ActionResult DryLoadingTimeSheets_Create([DataSourceRequest] DataSourceRequest request, DryLoadingTimeSheet dryLoadingTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new DryLoadingTimeSheet
                {
                    DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                    ProcurementSupplierID = dryLoadingTimeSheet.ProcurementSupplierID,
                    FleetsID = dryLoadingTimeSheet.FleetsID,
                    WorkItem = dryLoadingTimeSheet.WorkItem,
                    WorkingHour = dryLoadingTimeSheet.WorkingHour,
                    TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                    TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,
                    TripStartDate = dryLoadingTimeSheet.TripStartDate,
                    TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                    Asphalt = dryLoadingTimeSheet.Asphalt,
                    Gravel = dryLoadingTimeSheet.Gravel,
                    TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                    FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                    PerDiem = dryLoadingTimeSheet.PerDiem,
                    IdleHour = dryLoadingTimeSheet.IdleHour,
                    ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,
                    DateCreated = DateTime.Today,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = dryLoadingTimeSheet.ModifiedBy,
                    LastModified = dryLoadingTimeSheet.LastModified,

                    AsphaltRate = dryLoadingTimeSheet.AsphaltRate,
                    FuelRate = dryLoadingTimeSheet.FuelRate,
                    GravelRate = dryLoadingTimeSheet.GravelRate,
                    LoadedType = dryLoadingTimeSheet.LoadedType,
                    LoadingUnloadingCost = dryLoadingTimeSheet.LoadingUnloadingCost,
                    LubricantCost = dryLoadingTimeSheet.LubricantCost,
                    MaintenanceCost = dryLoadingTimeSheet.MaintenanceCost,
                    NetworkingHour = dryLoadingTimeSheet.NetworkingHour,
                    NoOfTripe = dryLoadingTimeSheet.NoOfTripe,
                    Perdium = dryLoadingTimeSheet.Perdium,
                    ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                    Salary = dryLoadingTimeSheet.Salary,
                    TireCost = dryLoadingTimeSheet.TireCost,
                    BranchId = dryLoadingTimeSheet.BranchId,
                    BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                    Category = dryLoadingTimeSheet.Category,
                    OperatorId = dryLoadingTimeSheet.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    Downhour = dryLoadingTimeSheet.Downhour,
                    EquipmentCategoryId = dryLoadingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = dryLoadingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = dryLoadingTimeSheet.EquipmentNameId
                };
                db.DryLoadingTimeSheets.Add(entity);
                db.SaveChanges();
                dryLoadingTimeSheet.DryLoadingTimeSheetId = entity.DryLoadingTimeSheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dry loading timesheet registration", "Success", User.Identity.Name, $"created dry loading timesheet {entity.DryLoadingTimeSheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { dryLoadingTimeSheet }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DryLoadingTimeSheets_Update([DataSourceRequest] DataSourceRequest request, DryLoadingTimeSheet dryLoadingTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new DryLoadingTimeSheet
                {
                    DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                    ProcurementSupplierID = dryLoadingTimeSheet.ProcurementSupplierID,
                    WorkItem = dryLoadingTimeSheet.WorkItem,
                    FleetsID = dryLoadingTimeSheet.FleetsID,
                    WorkingHour = dryLoadingTimeSheet.WorkingHour,
                    TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                    TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,
                    TripStartDate = dryLoadingTimeSheet.TripStartDate,
                    TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                    Asphalt = dryLoadingTimeSheet.Asphalt,
                    Gravel = dryLoadingTimeSheet.Gravel,
                    TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                    FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                    PerDiem = dryLoadingTimeSheet.PerDiem,
                    IdleHour = dryLoadingTimeSheet.IdleHour,
                    ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,
                    DateCreated = dryLoadingTimeSheet.DateCreated,
                    CreatedBy = dryLoadingTimeSheet.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,

                    AsphaltRate = dryLoadingTimeSheet.AsphaltRate,
                    FuelRate = dryLoadingTimeSheet.FuelRate,
                    GravelRate = dryLoadingTimeSheet.GravelRate,
                    LoadedType = dryLoadingTimeSheet.LoadedType,
                    LoadingUnloadingCost = dryLoadingTimeSheet.LoadingUnloadingCost,
                    LubricantCost = dryLoadingTimeSheet.LubricantCost,
                    MaintenanceCost = dryLoadingTimeSheet.MaintenanceCost,
                    NetworkingHour = dryLoadingTimeSheet.NetworkingHour,
                    NoOfTripe = dryLoadingTimeSheet.NoOfTripe,
                    Perdium = dryLoadingTimeSheet.Perdium,
                    ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                    Salary = dryLoadingTimeSheet.Salary,
                    TireCost = dryLoadingTimeSheet.TireCost,
                    BranchId = dryLoadingTimeSheet.BranchId,
                    BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                    Category = dryLoadingTimeSheet.Category,
                    OperatorId = dryLoadingTimeSheet.OperatorId,
                    Downhour = dryLoadingTimeSheet.Downhour,
                    EquipmentCategoryId = dryLoadingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = dryLoadingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = dryLoadingTimeSheet.EquipmentNameId
                };
                db.DryLoadingTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dry loading timesheet update", "Success", User.Identity.Name, $"updated dry loading timesheet {entity.DryLoadingTimeSheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { dryLoadingTimeSheet }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DryLoadingTimeSheets_Destroy([DataSourceRequest] DataSourceRequest request, DryLoadingTimeSheet dryLoadingTimeSheet)
        {

            if (ModelState.IsValid)
            {
                var entity = new DryLoadingTimeSheet
                {
                    DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                    ProcurementSupplierID = dryLoadingTimeSheet.ProcurementSupplierID,
                    FleetsID = dryLoadingTimeSheet.FleetsID,
                    WorkItem = dryLoadingTimeSheet.WorkItem,
                    WorkingHour = dryLoadingTimeSheet.WorkingHour,
                    TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                    TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,
                    TripStartDate = dryLoadingTimeSheet.TripStartDate,
                    TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                    Asphalt = dryLoadingTimeSheet.Asphalt,
                    Gravel = dryLoadingTimeSheet.Gravel,
                    TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                    FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                    PerDiem = dryLoadingTimeSheet.PerDiem,
                    IdleHour = dryLoadingTimeSheet.IdleHour,
                    ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,
                    DateCreated = dryLoadingTimeSheet.DateCreated,
                    CreatedBy = dryLoadingTimeSheet.CreatedBy,
                    ModifiedBy = dryLoadingTimeSheet.ModifiedBy,
                    LastModified = dryLoadingTimeSheet.LastModified,

                    AsphaltRate = dryLoadingTimeSheet.AsphaltRate,
                    FuelRate = dryLoadingTimeSheet.FuelRate,
                    GravelRate = dryLoadingTimeSheet.GravelRate,
                    LoadedType = dryLoadingTimeSheet.LoadedType,
                    LoadingUnloadingCost = dryLoadingTimeSheet.LoadingUnloadingCost,
                    LubricantCost = dryLoadingTimeSheet.LubricantCost,
                    MaintenanceCost = dryLoadingTimeSheet.MaintenanceCost,
                    NetworkingHour = dryLoadingTimeSheet.NetworkingHour,
                    NoOfTripe = dryLoadingTimeSheet.NoOfTripe,
                    Perdium = dryLoadingTimeSheet.Perdium,
                    ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                    Salary = dryLoadingTimeSheet.Salary,
                    TireCost = dryLoadingTimeSheet.TireCost,
                    BranchId = dryLoadingTimeSheet.BranchId,
                    BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                    Category = dryLoadingTimeSheet.Category,
                    OperatorId = dryLoadingTimeSheet.OperatorId,
                    Downhour = dryLoadingTimeSheet.Downhour,
                    EquipmentCategoryId = dryLoadingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = dryLoadingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = dryLoadingTimeSheet.EquipmentNameId
                };

                db.DryLoadingTimeSheets.Attach(entity);
                db.DryLoadingTimeSheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted dry loading timesheet {entity.DryLoadingTimeSheetId }");
                #endregion
            }

            return Json(new[] { dryLoadingTimeSheet }.ToDataSourceResult(request, ModelState));
        }


        #region Approval
        public JsonResult TimeSheetSendForApprovalDryLoad(int id)
        {

            var dryLoadingTimeSheet = db.DryLoadingTimeSheets.Find(id);

            if (dryLoadingTimeSheet != null)
            {
                dryLoadingTimeSheet.TimeSheetSendForAproval = true;
                dryLoadingTimeSheet.TimeSheetSendForApprovalTime = DateTime.Now;
                dryLoadingTimeSheet.TimeSheetSendForApprovalBy = User.Identity.Name;
                dryLoadingTimeSheet.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.DryLoadingTimeSheets.Attach(dryLoadingTimeSheet);
                db.Entry(dryLoadingTimeSheet).State = EntityState.Modified;
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
        public ActionResult DryLoadingTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int dryLoadId)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == dryLoadId && x.TimeSheetType == TimeSheetType.DryLoading);
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
        public ActionResult DryLoadingTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int dryLoadId)
        {
            if (ModelState.IsValid)
            {
                if (model.Status == MaintenanceConst.Approved)
                {
                    Approved(dryLoadId);
                    model.IsApproved = true;

                }
                else
                {
                    Rejected(dryLoadId);
                    model.IsApproved = false;
                }
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.TimeSheetId = dryLoadId;
                model.TimeSheetType = TimeSheetType.DryLoading;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var dryLoading = db.DryLoadingTimeSheets.Find(id);
            if (dryLoading != null)
            {
                dryLoading.TimeSheetApproved = true;
                dryLoading.TimeSheetStatus = MaintenanceConst.Approved;
                dryLoading.TimeSheetApprovedBy = User.Identity.Name;
                dryLoading.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(dryLoading).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var dryLoading = db.DryLoadingTimeSheets.Find(id);
            if (dryLoading != null)
            {
                dryLoading.TimeSheetApproved = false;
                dryLoading.TimeSheetStatus = MaintenanceConst.Rejected;
                dryLoading.TimeSheetApprovedBy = User.Identity.Name;
                dryLoading.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(dryLoading).State = EntityState.Modified;
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