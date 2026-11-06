using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Requisitions
{
    /// <summary>
    /// Finance side of the fuel request: approve / reject forwarded requests and record Telebirr payment.
    /// </summary>
    public class FuelRequestFinanceController : Controller
    {
        private static readonly Regex PhoneRegex = new Regex(@"^(\+?251|0)?9\d{8}$");
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Fleets"] = db.Fleets
        .Select(s => new { Id = s.FleetsID, Name = s.PlateNumber + " - " + s.EquipmentName })
        .ToList();
            ViewData["Department"] = db.MainCompanyStructures
                .Where(x => x.Parent > 0)
                .Select(x => new { x.OrgStructureId, x.Name }).ToList();
            ViewData["Requestor"] = db.Person_Employee.Select(x => new
            {
                Value = x.EmployeeId,
                Text = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish
            }).AsQueryable();
            ViewData["Division"] = db.MainCompanyStructures
                .Where(x => x.Parent == 0)
                .Select(x => new { x.OrgStructureId, x.Name }).ToList();
            ViewData["Operators"] = db.Operator
                .Select(s => new { Id = s.OperatorID, Name = s.FullName }).ToList();
            return View();
        }

        public ActionResult FuelRequest_Read([DataSourceRequest] DataSourceRequest request)
        {
            var query = db.FuelRequests.Where(x => x.IsSendForApproval && !x.IsVoid);
            return Json(query.ToDataSourceResult(request));
        }

        /// <summary>Finance may correct the payee details (owner + Telebirr number) before paying.</summary>
        [HttpPost]
        public ActionResult FuelRequest_Update([DataSourceRequest] DataSourceRequest request, FuelRequest model)
        {
            var entity = db.FuelRequests.Find(model.FuelRequestId);
            if (entity == null)
            {
                ModelState.AddModelError("notfound", "Fuel request not found.");
            }
            else if (!string.IsNullOrWhiteSpace(model.PhoneNumber) && !PhoneRegex.IsMatch(model.PhoneNumber.Trim()))
            {
                ModelState.AddModelError("FuelOwnerPhoneNumber", "Enter a valid Ethiopian mobile number (e.g. 09xxxxxxxx).");
            }
            if (ModelState.IsValid)
            {
                entity.FuelOwnerName = model.FuelOwnerName;
                entity.PhoneNumber = model.PhoneNumber?.Trim();
                entity.LastModified = DateTime.Now;
                entity.ModifiedBy = User.Identity.Name;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                model = entity;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public JsonResult Approve(int id)
        {
            var entity = db.FuelRequests.Find(id);
            if (entity == null || !entity.IsSendForApproval) return Json(new { success = false, message = "Request not found or not forwarded to Finance." });
            if (entity.IsFinanceApproved) return Json(new { success = false, message = "Already approved." });

            entity.IsFinanceApproved = true;
            entity.FinanceApprovedBy = User.Identity.Name;
            entity.FinanceApprovedTime = DateTime.Now;
            entity.FinanceRejectionReason = null;
            entity.IsOnlineApproved = true;
            entity.OnlineApprovedBy = User.Identity.Name;
            entity.OnlineApprovedTime = DateTime.Now;
            entity.IsFinalApproval = true;
            entity.Status = FuelRequestStatus.FinanceApproved;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            UserHelper.OperationLog(Request.UserHostAddress, "Fuel Request Finance", "Success", User.Identity.Name, $"approved fuel request {id}", Modules.FleetManagement);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult Reject(int id, string reason)
        {
            var entity = db.FuelRequests.Find(id);
            if (entity == null || !entity.IsSendForApproval) return Json(new { success = false, message = "Request not found or not forwarded to Finance." });
            if (string.IsNullOrWhiteSpace(reason)) return Json(new { success = false, message = "Rejection reason is required." });

            // Return to Fleet so the requester can correct and resend.
            entity.IsSendForApproval = false;
            entity.IsFinanceApproved = false;
            entity.IsOnlineApproved = false;
            entity.IsFinalApproval = false;
            entity.FinanceRejectionReason = reason.Trim();
            entity.ForwardedTo = entity.ForwardedFrom;
            entity.ForwardedFrom = User.Identity.Name;
            entity.Status = FuelRequestStatus.Rejected;
            entity.LastModified = DateTime.Now;
            entity.ModifiedBy = User.Identity.Name;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            UserHelper.OperationLog(Request.UserHostAddress, "Fuel Request Finance", "Success", User.Identity.Name, $"rejected fuel request {id}: {reason}", Modules.FleetManagement);
            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
