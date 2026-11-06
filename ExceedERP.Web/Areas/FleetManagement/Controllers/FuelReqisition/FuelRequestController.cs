using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Requisitions
{
    /// <summary>
    /// Fleet-side fuel request: create -> send to Finance for approval and Telebirr payment.
    /// </summary>
    public class FuelRequestController : Controller
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
                .Select(x => new { x.OrgStructureId, x.Name }).ToList();
            ViewData["Requestor"] = db.Person_Employee.Select(x => new
            {
                Value = x.EmployeeId,
                Text = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish
            }).AsQueryable();
            ViewData["Division"] = db.MainCompanyStructures
                .Select(x => new { x.OrgStructureId, x.Name }).ToList();
            ViewData["Operators"] = db.Operator
                .Select(s => new { Id = s.OperatorID, Name = s.FullName }).ToList();
        }

        public JsonResult GetFuelRequestDepartments()
        {
            return Json(db.MainCompanyStructures.OrderBy(x => x.Name)
                .Select(x => new { x.OrgStructureId, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFuelRequestDivisions(int id)
        {
            return Json(db.MainCompanyStructures.Where(x => x.Parent==id).OrderBy(x => x.Name)
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
        //public JsonResult GetFleetInfo(Guid id, DateTime? requestDate, int excludeId = 0)
        //{
        //    var fleet = db.Fleets.Where(x => x.FleetsID == id)
        //        .Select(x => new { x.PlateNumber, x.UnitNum, x.OperatorName, x.EquipmentTypeID, EquipmentType = x.EquipmentType.EquipmentTypeName })
        //        .FirstOrDefault();
        //    if (fleet == null) return Json(null, JsonRequestBehavior.AllowGet);

        //    var op = db.Operator.Where(x => x.FleetsID == id && x.IsActive).Select(x => new { x.OperatorID, x.FullName }).FirstOrDefault();
        //    var date = (requestDate ?? DateTime.Today).Date;
        //    //var prev = FindPrevious(id, date, excludeId);

        //    return Json(new
        //    {
        //        PlateNo = fleet.PlateNumber,
        //        VehicleTypeId = fleet.EquipmentTypeID,
        //        VehicleType = fleet.EquipmentType,
        //        SideNo = fleet.UnitNum,
        //        OperatorId = op?.OperatorID,
        //        Driver = op?.FullName ?? fleet.OperatorName,
        //        HasPrevious = prev != null,
        //        OpeningKM = prev?.ClosingKM ?? 0m,
        //        FuelConsumedL = prev?.FuelFilledL ?? 0m,
        //        OperationalDays = prev != null ? (date - prev.RequestDate.Date).Days : 0
        //    }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult FuelRequest_Read([DataSourceRequest] DataSourceRequest request)
        {
            var branches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var query = db.FuelRequests.Where(x => !x.IsVoid && (branches.Contains(x.OrgBranchName) || x.CreatedBy == User.Identity.Name));
            return Json(query.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult FuelRequest_Create([DataSourceRequest] DataSourceRequest request, FuelRequest model)
        {
           
            Validate(model);
            if (ModelState.IsValid)
            {
                model.Status = FuelRequestStatus.Pending;
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                db.FuelRequests.Add(model);
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Fuel Request", "Success", User.Identity.Name, $"created fuel request {model.FuelRequestId}", Modules.FleetManagement);
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult FuelRequest_Update([DataSourceRequest] DataSourceRequest request, FuelRequest model)
        {
           // ApplySnapshots(model);
           // ApplyPrevious(model);
            Validate(model);
            var entity = db.FuelRequests.Find(model.FuelRequestId);
            if (entity == null)
            {
                ModelState.AddModelError("notfound", "Fuel request not found.");
            }
            else if (entity.IsSendForApproval)
            {
                ModelState.AddModelError("locked", "Request already sent to Finance and cannot be edited.");
            }
            if (ModelState.IsValid)
            {
                entity.RequestDate = model.RequestDate;
                entity.DepartmentId = model.DepartmentId;
                entity.RequestorId = model.RequestorId;
                entity.RecieptNo = model.RecieptNo;
               
                entity.FuelOwnerName = model.FuelOwnerName;
                entity.PhoneNumber = model.PhoneNumber;
                entity.Remark = model.Remark;
                //entity.PlateNo = model.PlateNo;               
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
        public ActionResult FuelRequest_Destroy([DataSourceRequest] DataSourceRequest request, FuelRequest model)
        {
            var entity = db.FuelRequests.Find(model.FuelRequestId);
            if (entity != null && !entity.IsSendForApproval)
            {
                db.FuelRequests.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Fuel Request " + model.FuelRequestId);
            }
            else if (entity != null)
            {
                ModelState.AddModelError("locked", "Request already sent to Finance and cannot be deleted.");
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public JsonResult SendForFinanceApproval(int id)
        {
            var entity = db.FuelRequests.Find(id);
            if (entity == null) return Json(new { success = false, message = "Fuel request not found." });
            if (entity.IsSendForApproval) return Json(new { success = false, message = "Already sent to Finance." });
            if (string.IsNullOrWhiteSpace(entity.PhoneNumber))
            {
                return Json(new { success = false, message = "Telebirr phone number of the fuel owner is required before sending to Finance." });
            }

            entity.IsSendForApproval = true;
            entity.SendForApprovalBy = User.Identity.Name;
            entity.SendForApprovalTime = DateTime.Now;
            entity.ForwardedFrom = User.Identity.Name;
            entity.ForwardedTo = "Finance";
            entity.Status = FuelRequestStatus.SentToFinance;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            UserHelper.OperationLog(Request.UserHostAddress, "Fuel Request", "Success", User.Identity.Name, $"sent fuel request {id} to Finance", Modules.FleetManagement);
            return Json(new { success = true });
        }

        private void Validate(FuelRequest model)
        {
            ModelState.Remove("FuelRequestId");
            ModelState.Remove("PlateNo");
            ModelState.Remove("DepartmentName");           
            if (model.RequestDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("RequestDate", "Request date cannot be in the future.");
            }
        }

        //private void ApplySnapshots(FuelRequest model)
        //{
        //    var fleet = db.Fleets.Where(x => x.FleetsID == model.FleetId)
        //        .Select(x => new { x.PlateNumber, x.UnitNum, x.OperatorName, x.EquipmentTypeID, EquipmentType = x.EquipmentType.EquipmentTypeName })
        //        .FirstOrDefault();
        //    if (fleet != null)
        //    {
        //        model.PlateNo = fleet.PlateNumber;
        //        model.VehicleTypeId = fleet.EquipmentTypeID;
        //        model.VehicleType = fleet.EquipmentType;
        //        model.SideNo = fleet.UnitNum;
        //        model.Driver = fleet.OperatorName;
        //    }
        //    if (model.OperatorId.HasValue)
        //    {
        //        var op = db.Operator.Where(x => x.OperatorID == model.OperatorId).Select(x => x.FullName).FirstOrDefault();
        //        if (!string.IsNullOrWhiteSpace(op)) model.Driver = op;
        //    }
        //    model.DepartmentName = db.MainCompanyStructures.Where(x => x.OrgStructureId == model.DepartmentId).Select(x => x.Name).FirstOrDefault();
        //    model.DivisionName = model.DivisionId.HasValue
        //        ? db.MainCompanyStructures.Where(x => x.OrgStructureId == model.DivisionId).Select(x => x.Name).FirstOrDefault()
        //        : null;
        //}

        ///// <summary>Opening KM = previous closing KM, fuel consumed = previous fuel filled, operational days = days since previous request.</summary>
        //private void ApplyPrevious(FuelRequestDetail model)
        //{
        //    var prev = FindPrevious(model.FleetId, model.RequestDate.Date, model.FuelRequestId);
        //    if (prev == null)
        //    {
        //        // First request for this vehicle: Opening KM is entered manually, nothing to consume/compare yet.
        //        model.FuelConsumedL = 0m;
        //        model.OperationalDays = 0;
        //        return;
        //    }
        //    model.OpeningKM = prev.ClosingKM;
        //    model.FuelConsumedL = prev.FuelFilledL;
        //    model.OperationalDays = (model.RequestDate.Date - prev.RequestDate.Date).Days;
        //}

        //private FuelRequest FindPrevious(Guid fleetId, DateTime requestDate, int excludeId)
        //{
        //    // Same-day requests are ordered by id; when editing, only rows created before the current one count as "previous".
        //    return db.FuelRequestDetail.AsNoTracking()
        //        .Where(x => x.FleetId == fleetId && x.FuelRequestId != excludeId)
        //        .Where(x => x.RequestDate < requestDate || (x.RequestDate == requestDate && (excludeId == 0 || x.FuelRequestId < excludeId)))
        //        .OrderByDescending(x => x.RequestDate).ThenByDescending(x => x.FuelRequestId)
        //        .FirstOrDefault();
        //}

        //private static void Recalculate(FuelRequest model)
        //{
        //    model.DistanceKM = model.ClosingKM - model.OpeningKM;
        //    model.ConsumptionKMPerL = model.FuelConsumedL > 0 ? Math.Round(model.DistanceKM / model.FuelConsumedL, 2) : 0m;
        //    model.TotalAmount = Math.Round(model.FuelFilledL * model.UnitPrice, 2);
        //}
        public ActionResult Doc_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var Doc = db.FuelRequestDocument.Where(x => x.FuelRequestId == id);
            return Json(Doc.ToDataSourceResult(request));
        }
        public ActionResult Doc_Delete([DataSourceRequest]DataSourceRequest request, FuelRequestDocument model)
        {
            var Doc = db.FuelRequestDocument.FirstOrDefault(x => x.FuelRequestDocumentId == model.FuelRequestDocumentId);
            db.FuelRequestDocument.Remove(Doc);
            db.SaveChanges();
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachment_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 2 * 1024 * 1024;

                string subPath = "~/App_Data/FuelRequest/";
                string shortPath = "/App_Data/FuelRequest/";

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                foreach (var file in attachments)
                {
                    if (file != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                        }
                        if (file.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"File size exceeds the {MaxFileSize / (1024 * 1024)} MB limit.");
                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string physicalPath = Server.MapPath(subPath + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            file.SaveAs(physicalPath);
                            db.FuelRequestDocument.Add(new FuelRequestDocument()
                            {
                                FileName = Path.GetFileName(file.FileName),
                                FuelRequestId = id,
                                ShortPath = shortPath,
                                FilePath = physicalPath

                            });
                            db.SaveChanges();
                        }
                    }
                }
               
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadBoloDocument(int CompanyDocumentId)
        {
            var record = db.FuelRequestDocument.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
