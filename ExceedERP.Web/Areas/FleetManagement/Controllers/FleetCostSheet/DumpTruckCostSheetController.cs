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
    public class DumpTruckCostSheetController : Controller
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
            ViewData["Fleets"] = db.Fleets.Select(x => new { x.FleetsID, x.PlateNumber }).ToList();
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
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

            return View();
        }



        public ActionResult DumpTruckCostSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var dumpTruckTimeSheets = db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
            .Where(x => mybraches.Contains(x.BranchId));

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
                CostSheetStatus = dumpTruckTimeSheet.CostSheetStatus,

                TimeSheetApproved = dumpTruckTimeSheet.TimeSheetApproved,
                TimeSheetApprovedBy = dumpTruckTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = dumpTruckTimeSheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval = dumpTruckTimeSheet.TimeSheetSendForAproval,
                TimeSheetStatus = dumpTruckTimeSheet.TimeSheetStatus,
                CostSheetSendForApprovalBy = dumpTruckTimeSheet.CostSheetSendForApprovalBy,
                CostSheetSendForApprovalTime = dumpTruckTimeSheet.CostSheetSendForApprovalTime,
                TimeSheetSendForApprovalBy = dumpTruckTimeSheet.TimeSheetSendForApprovalBy,
                TimeSheetSendForApprovalTime = dumpTruckTimeSheet.TimeSheetSendForApprovalTime,
                EquipmentCategoryId = dumpTruckTimeSheet.EquipmentCategoryId,
                EquipmentNameId = dumpTruckTimeSheet.EquipmentNameId,
                EquipmentTypeId = dumpTruckTimeSheet.EquipmentTypeId,
                
                

            });

            return Json(result);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DumpTruckCostSheet_Update([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheet dumpTruckTimeSheet)
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
                    CostSheetApproved = dumpTruckTimeSheet.CostSheetApproved,
                    CostSheetApprovedBy = dumpTruckTimeSheet.CostSheetApprovedBy,
                    CostSheetApprovedDate = dumpTruckTimeSheet.CostSheetApprovedDate,
                    CostSheetSendForAproval = dumpTruckTimeSheet.CostSheetSendForAproval,
                    CostSheetStatus = dumpTruckTimeSheet.CostSheetStatus,
                    
                    TimeSheetApproved = dumpTruckTimeSheet.TimeSheetApproved,
                    TimeSheetApprovedBy = dumpTruckTimeSheet.TimeSheetApprovedBy,
                    TimeSheetApprovedDate = dumpTruckTimeSheet.TimeSheetApprovedDate,
                    TimeSheetSendForAproval = dumpTruckTimeSheet.TimeSheetSendForAproval,
                    TimeSheetStatus = dumpTruckTimeSheet.TimeSheetStatus,
                    EquipmentCategoryId = dumpTruckTimeSheet.EquipmentCategoryId,
                    EquipmentNameId = dumpTruckTimeSheet.EquipmentNameId,
                    EquipmentTypeId = dumpTruckTimeSheet.EquipmentTypeId,
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


        #region Approval

        public JsonResult VoidCostSheet(int id)
        {

            var dumpTruckTimeSheet = db.DumpTruckTimeSheets.Find(id);

            if (dumpTruckTimeSheet != null)
            {
                dumpTruckTimeSheet.CostSheetSendForAproval = false;
                dumpTruckTimeSheet.CostSheetApproved = false;
                dumpTruckTimeSheet.CostSheetApprovedDate = DateTime.Now;
                dumpTruckTimeSheet.CostSheetApprovedBy = User.Identity.Name;
                dumpTruckTimeSheet.CostSheetStatus = MaintenanceConst.Rejected;

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
         public JsonResult CostSheetSendForApproval(int id)
        {

            var dumpTruckTimeSheet = db.DumpTruckTimeSheets.Find(id);

            if (dumpTruckTimeSheet != null)
            {
                dumpTruckTimeSheet.CostSheetSendForAproval = true;
                dumpTruckTimeSheet.CostSheetSendForApprovalTime = DateTime.Now;
                dumpTruckTimeSheet.CostSheetSendForApprovalBy = User.Identity.Name;
                dumpTruckTimeSheet.CostSheetStatus = MaintenanceConst.WaitingApproval;

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

        public ActionResult DumpTruckCostSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.CostSheetApprovals.Where(x => x.CostSheetId == id && x.TimeSheetType == TimeSheetType.DumpTruck);
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
        public ActionResult DumpTruckCostSheetApproval_Create([DataSourceRequest] DataSourceRequest request, CostSheetApproval model, int id)
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
                model.TimeSheetType = TimeSheetType.DumpTruck;
                db.CostSheetApprovals.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        private void Approved(int id)
        {
            var dumpTruckCostSheet = db.DumpTruckTimeSheets.Find(id);
            if (dumpTruckCostSheet != null)
            {
                dumpTruckCostSheet.CostSheetApproved = true;
                dumpTruckCostSheet.CostSheetStatus = MaintenanceConst.Approved;
                dumpTruckCostSheet.CostSheetApprovedBy = User.Identity.Name;
                dumpTruckCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(dumpTruckCostSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var dumpTruckCostSheet = db.DumpTruckTimeSheets.Find(id);
            if (dumpTruckCostSheet != null)
            {
                dumpTruckCostSheet.CostSheetApproved = false;
                dumpTruckCostSheet.CostSheetStatus = MaintenanceConst.Rejected;
                dumpTruckCostSheet.CostSheetApprovedBy = User.Identity.Name;
                dumpTruckCostSheet.CostSheetApprovedDate = DateTime.Now;
                db.Entry(dumpTruckCostSheet).State = EntityState.Modified;
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