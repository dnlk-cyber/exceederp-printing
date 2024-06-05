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
    public class DryLoadingCostSheetController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();       
        public ActionResult Index()
        {
            ViewData["fleet"] = db.Fleets.Select(x => new { x.FleetsID, x.PlateNumber }).ToList();
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            return View();
        }
        public ActionResult DryLoadingCostSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            var dryLoadingTimeSheets = db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved);
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
                
                Salary = dryLoadingTimeSheet.Salary,
                TireCost = dryLoadingTimeSheet.TireCost,
                BranchId = dryLoadingTimeSheet.BranchId,
                BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                Category = dryLoadingTimeSheet.Category,
                CostSheetApproved = dryLoadingTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = dryLoadingTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = dryLoadingTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = dryLoadingTimeSheet.CostSheetSendForAproval,
                CostSheetStatus =  dryLoadingTimeSheet.CostSheetStatus,
                
                ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                TimeSheetApproved = dryLoadingTimeSheet.TimeSheetApproved,
                TimeSheetApprovedBy = dryLoadingTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = dryLoadingTimeSheet.TimeSheetApprovedDate,
                CostSheetSendForApprovalBy = dryLoadingTimeSheet.CostSheetSendForApprovalBy,
                CostSheetSendForApprovalTime = dryLoadingTimeSheet.CostSheetSendForApprovalTime,
                TimeSheetSendForApprovalBy = dryLoadingTimeSheet.TimeSheetSendForApprovalBy,
                TimeSheetSendForApprovalTime = dryLoadingTimeSheet.TimeSheetSendForApprovalTime,
                TimeSheetSendForAproval = dryLoadingTimeSheet.TimeSheetSendForAproval,
                TimeSheetStatus = dryLoadingTimeSheet.TimeSheetStatus,
                RentalRate = dryLoadingTimeSheet.RentalRate,
                OperatorId = dryLoadingTimeSheet.OperatorId,
                ExternalOperator = dryLoadingTimeSheet.ExternalOperator
                
                
                
            });
            return Json(result);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DryLoadingCostSheet_Update([DataSourceRequest] DataSourceRequest request, DryLoadingTimeSheet dryLoadingTimeSheet)
        {
          
          if (ModelState.IsValid)
            {
                dryLoadingTimeSheet.LastModified = DateTime.Now.Date;
                dryLoadingTimeSheet.ModifiedBy = User.Identity.Name;

                db.DryLoadingTimeSheets.Attach(dryLoadingTimeSheet);
                db.Entry(dryLoadingTimeSheet).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dry loading timesheet update", "Success", User.Identity.Name, $"updated dry loading timesheet {dryLoadingTimeSheet.DryLoadingTimeSheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { dryLoadingTimeSheet }.ToDataSourceResult(request, ModelState));
        }




        #region Approval
        public JsonResult CostSheetSendForApproval(int id)
        {

            var dryLoadingCostSheet = db.DryLoadingTimeSheets.Find(id);

            if (dryLoadingCostSheet != null)
            {
                dryLoadingCostSheet.CostSheetSendForAproval = true;
                dryLoadingCostSheet.CostSheetSendForApprovalTime = DateTime.Now;
                dryLoadingCostSheet.CostSheetSendForApprovalBy = User.Identity.Name;
                dryLoadingCostSheet.CostSheetStatus = MaintenanceConst.WaitingApproval;

                db.DryLoadingTimeSheets.Attach(dryLoadingCostSheet);
                db.Entry(dryLoadingCostSheet).State = EntityState.Modified;
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


        public ActionResult DryLoadingCostSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.CostSheetApprovals.Where(x => x.CostSheetId == id && x.TimeSheetType == TimeSheetType.DryLoading);
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


        public ActionResult DryLoadingCostSheetApproval_Create([DataSourceRequest] DataSourceRequest request, CostSheetApproval model, int id)
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
                model.TimeSheetType = TimeSheetType.DryLoading;
                db.CostSheetApprovals.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var dryLoading = db.DryLoadingTimeSheets.Find(id);
            if (dryLoading != null)
            {
                dryLoading.CostSheetApproved = true;
                dryLoading.CostSheetStatus = MaintenanceConst.Approved;
                dryLoading.CostSheetApprovedBy = User.Identity.Name;
                dryLoading.CostSheetApprovedDate = DateTime.Now;
                db.Entry(dryLoading).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var dryLoading = db.DryLoadingTimeSheets.Find(id);
            if (dryLoading != null)
            {
                dryLoading.CostSheetApproved = false;
                dryLoading.CostSheetStatus = MaintenanceConst.Rejected;
                dryLoading.CostSheetApprovedBy = User.Identity.Name;
                dryLoading.CostSheetApprovedDate = DateTime.Now;
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