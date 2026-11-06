using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Requisitions
{
    /// <summary>
    /// Fleet-side fuel request: create -> send to Finance for approval and Telebirr payment.
    /// </summary>
    public class FuelRequestDetailController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateLookups();
            return View();
        }

        private void PopulateLookups()
        {
            ViewData["Fleets"] = db.Fleets
                .Select(s => new { Id = s.FleetsID, Name = s.PlateNumber + " - " + s.EquipmentName })
                .ToList();
            ViewData["Department"] = db.MainCompanyStructures
                .Where(x => x.Parent > 0)
                .Select(x => new { x.OrgStructureId, x.Name }).ToList();
            ViewData["Division"] = db.MainCompanyStructures
                .Where(x => x.Parent == 0)
                .Select(x => new { x.OrgStructureId, x.Name }).ToList();
            ViewData["Operators"] = db.Operator
                .Select(s => new { Id = s.OperatorID, Name = s.FullName }).ToList();
        }

        public JsonResult GetFuelRequestDepartments()
        {
            return Json(db.MainCompanyStructures.Where(x => x.Parent > 0).OrderBy(x => x.Name)
                .Select(x => new { x.OrgStructureId, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFuelRequestDivisions()
        {
            return Json(db.MainCompanyStructures.Where(x => x.Parent == 0).OrderBy(x => x.Name)
                .Select(x => new { x.OrgStructureId, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFleetsForFuelRequest(string text)
        {
            var fleets = db.Fleets.Where(x => !x.IsReturned && !x.IsContractTerminated);
            if (!string.IsNullOrWhiteSpace(text))
            {
                fleets = fleets.Where(x => x.PlateNumber.Contains(text) || x.SerialNo.Contains(text) || x.EquipmentName.Contains(text));
            }
            return Json(fleets.OrderBy(x => x.PlateNumber).Take(100)
                .Select(s => new { Value = s.FleetsID, Text = s.PlateNumber + " - " + s.EquipmentName }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOperatorsForFuelRequest(string text)
        {
            var ops = db.Operator.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                ops = ops.Where(x => x.FullName.Contains(text));
            }
            return Json(ops.OrderBy(x => x.FullName).Take(100)
                .Select(s => new { Value = s.OperatorID, Text = s.FullName }).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Fleet snapshot used by the editor to prefill plate, type, side no, driver and the previous-request derived figures.</summary>
        public JsonResult GetFleetInfo(Guid id, DateTime? requestDate, int excludeId = 0)
        {
            var fleet = db.Fleets.Where(x => x.FleetsID == id)
                .Select(x => new { x.PlateNumber, x.UnitNum, x.OperatorName, x.EquipmentTypeID, EquipmentType = x.EquipmentType.EquipmentTypeName })
                .FirstOrDefault();
            if (fleet == null) return Json(null, JsonRequestBehavior.AllowGet);

            var op = db.Operator.Where(x => x.FleetsID == id && x.IsActive).Select(x => new { x.OperatorID, x.FullName }).FirstOrDefault();
            var date = (requestDate ?? DateTime.Today).Date;
            var prev = db.FuelRequestDetail.AsNoTracking()
                .Where(x => x.FleetId == id)
                .Where(x => x.RequestDate < requestDate.Value.Date)
                .OrderByDescending(x => x.RequestDate).ThenByDescending(x => x.FuelRequestDetailId)
                .FirstOrDefault();

            return Json(new
            {
                PlateNo = fleet.PlateNumber,
                VehicleTypeId = fleet.EquipmentTypeID,
                VehicleType = fleet.EquipmentType,
                SideNo = fleet.UnitNum,
                OperatorId = op?.OperatorID,
                Driver = op?.FullName ?? fleet.OperatorName,
                HasPrevious = prev != null,
                OpeningKM = prev?.ClosingKM ?? 0m,
                FuelConsumedL = prev?.FuelFilledL ?? 0m,
                OperationalDays = prev != null ? (date - prev.RequestDate.Date).Days : 0
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FuelRequest_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            var branches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var query = db.FuelRequestDetail.Where(x =>x.FuelRequestId==id);
            return Json(query.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult FuelRequest_Create([DataSourceRequest] DataSourceRequest request, FuelRequestDetail model,int id)
        {
            ApplySnapshots(model);
            ApplyPrevious(model);
            Validate(model);
            if (ModelState.IsValid)
            {
                Recalculate(model);
                //model.Status = FuelRequestStatus.Pending;
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.DistanceKM = model.ClosingKM - model.OpeningKM;
                model.ConsumptionKMPerL = model.FuelConsumedL > 0 ? Math.Round(model.DistanceKM / model.FuelConsumedL, 2) : 0m;
                model.TotalAmount = Math.Round(model.FuelFilledL * model.UnitPrice, 2);
                var parent = db.FuelRequests.FirstOrDefault(x => x.FuelRequestId == id);
                parent.TotalAmount += model.TotalAmount;
                model.FuelRequestId = id;
              
                // model.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                db.FuelRequestDetail.Add(model);
                db.SaveChanges();
                db.Entry(parent).State = EntityState.Modified;
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Fuel Request", "Success", User.Identity.Name, $"created fuel request {model.FuelRequestId}", Modules.FleetManagement);
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult FuelRequest_Update([DataSourceRequest] DataSourceRequest request, FuelRequestDetail model)
        {
            ApplySnapshots(model);
            ApplyPrevious(model);
            Validate(model);
            var entity = db.FuelRequestDetail.Find(model.FuelRequestDetailId);
            if (entity == null)
            {
                ModelState.AddModelError("notfound", "Fuel request not found.");
            }
            if (ModelState.IsValid)
            {
                entity.RequestDate = model.RequestDate;
                entity.FleetId = model.FleetId;
                entity.OperatorId = model.OperatorId;
                entity.DepartmentId = model.DepartmentId;
                entity.DivisionId = model.DivisionId;
                entity.OpeningKM = model.OpeningKM;
                entity.RecieptNo = model.RecieptNo;
                entity.ClosingKM = model.ClosingKM;
                entity.FuelConsumedL = model.FuelConsumedL;
                entity.FuelFilledL = model.FuelFilledL;
                entity.OperationalDays = model.OperationalDays;
                entity.UnitPrice = model.UnitPrice;
                entity.FuelOwnerName = model.FuelOwnerName;
                entity.FuelOwnerPhoneNumber = model.FuelOwnerPhoneNumber;
                entity.PlateNo = model.PlateNo;
                entity.VehicleTypeId = model.VehicleTypeId;
                entity.VehicleType = model.VehicleType;
                entity.SideNo = model.SideNo;
                entity.Driver = model.Driver;
                entity.DepartmentName = model.DepartmentName;
                entity.DivisionName = model.DivisionName;
                Recalculate(entity);
                entity.LastModified = DateTime.Now;
                entity.ModifiedBy = User.Identity.Name;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Fuel Request", "Success", User.Identity.Name, $"updated fuel request {entity.FuelRequestId}", Modules.FleetManagement);
                model = entity;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult FuelRequest_Destroy([DataSourceRequest] DataSourceRequest request, FuelRequestDetail model)
        {
            var entity = db.FuelRequestDetail.Find(model.FuelRequestDetailId);
            if (entity != null)
            {
                db.FuelRequestDetail.Remove(entity);
                db.SaveChanges();
            }
            
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

       

        private void Validate(FuelRequestDetail model)
        {
            ModelState.Remove("FuelRequestId");
            ModelState.Remove("PlateNo");
            ModelState.Remove("DepartmentName");
            if (model.ClosingKM < model.OpeningKM)
            {
                ModelState.AddModelError("ClosingKM", "Closing KM must be greater than or equal to Opening KM.");
            }
            if (model.FuelFilledL <= 0)
            {
                ModelState.AddModelError("FuelFilledL", "Fuel filled must be greater than zero.");
            }
        }

        private void ApplySnapshots(FuelRequestDetail model)
        {
            var fleet = db.Fleets.Where(x => x.FleetsID == model.FleetId)
                .Select(x => new { x.PlateNumber, x.UnitNum, x.OperatorName, x.EquipmentTypeID, EquipmentType = x.EquipmentType.EquipmentTypeName })
                .FirstOrDefault();
            if (fleet != null)
            {
                model.PlateNo = fleet.PlateNumber;
                model.VehicleTypeId = fleet.EquipmentTypeID;
                model.VehicleType = fleet.EquipmentType;
                model.SideNo = model.SideNo;
                model.Driver = fleet.OperatorName;
            }
            if (model.OperatorId.HasValue)
            {
                var op = db.Operator.Where(x => x.OperatorID == model.OperatorId).Select(x => x.FullName).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(op)) model.Driver = op;
            }
            model.DepartmentName = db.MainCompanyStructures.Where(x => x.OrgStructureId == model.DepartmentId).Select(x => x.Name).FirstOrDefault();
            model.DivisionName = model.DivisionId.HasValue
                ? db.MainCompanyStructures.Where(x => x.OrgStructureId == model.DivisionId).Select(x => x.Name).FirstOrDefault()
                : null;
        }

        /// <summary>Opening KM = previous closing KM, fuel consumed = previous fuel filled, operational days = days since previous request.</summary>
        private void ApplyPrevious(FuelRequestDetail model)
        {
            var prev = db.FuelRequestDetail.AsNoTracking()
                .Where(x => x.FleetId == model.FleetId)
                .Where(x => x.RequestDate < model.RequestDate.Date)
                .OrderByDescending(x => x.RequestDate).ThenByDescending(x => x.FuelRequestDetailId)
                .FirstOrDefault();
            if (prev == null)
            {
                // First request for this vehicle: Opening KM is entered manually, nothing to consume/compare yet.
                model.FuelConsumedL = 0m;
                model.OperationalDays = 0;
                return;
            }
            model.OpeningKM = prev.ClosingKM;
            model.FuelConsumedL = prev.FuelFilledL;
            model.OperationalDays = (model.RequestDate.Date - prev.RequestDate.Date).Days;
        }

        

        private static void Recalculate(FuelRequestDetail model)
        {
            model.DistanceKM = model.ClosingKM - model.OpeningKM;
            model.ConsumptionKMPerL = model.FuelConsumedL > 0 ? Math.Round(model.DistanceKM / model.FuelConsumedL, 2) : 0m;
            model.TotalAmount = Math.Round(model.FuelFilledL * model.UnitPrice, 2);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
