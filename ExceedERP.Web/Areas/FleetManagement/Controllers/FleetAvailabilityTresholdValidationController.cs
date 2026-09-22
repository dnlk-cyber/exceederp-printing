using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class FleetAvailabilityTresholdValidationController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        // ✅ READ
        public ActionResult _Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var data = db.FleetAvailabilityTresholdValidation
                         .Where(x => x.FleetAvailabilityTresholdId == id)
                         .ToList();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // ✅ CREATE (Add Validation / Approval Step)
        [HttpPost]
        public ActionResult _Create([DataSourceRequest] DataSourceRequest request, FleetAvailabilityTresholdValidation model, int id)
        {
            if (model != null)
            {
                // 🔒 Rule 1: Prevent duplicate SAME STATUS for same record (optional)
                bool exists = db.FleetAvailabilityTresholdValidation.Any(x =>
                    x.FleetAvailabilityTresholdId == id &&
                    x.Status == model.Status);

                if (exists)
                {
                    ModelState.AddModelError("", "This status is already recorded for this item.");
                }

                // 🔒 Rule 2: Prevent approval if already approved
                bool alreadyApproved = db.FleetAvailabilityTresholdValidation.Any(x =>
                    x.FleetAvailabilityTresholdId == id &&
                    x.Status == ApprovalStatuses.Approve);

                if (alreadyApproved)
                {
                    ModelState.AddModelError("", "This record is already approved.");
                }

                if (ModelState.IsValid)
                {
                    model.FleetAvailabilityTresholdId = id;
                    db.FleetAvailabilityTresholdValidation.Add(model);
                    db.SaveChanges();

                    // 🔄 Optional: Update main table status
                    var main = db.FleetAvailabilityTreshold
                                 .FirstOrDefault(x => x.FleetAvailabilityTresholdId == id);

                    if (main != null)
                    {
                        if (model.Status == ApprovalStatuses.Approve)
                        {
                            main.IsOnlineApproved = true;
                            main.OnlineApprovedBy = User.Identity.Name;
                            main.OnlineApprovedTime = DateTime.Now;
                        }
                        else if (model.Status == ApprovalStatuses.Reject)
                        {
                            main.IsOnlineApproved = false;
                            main.IsSendForApproval = false;
                        }
                        db.Entry(main).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ❌ UPDATE (Usually NOT allowed for audit trail)
        // If you want it, tell me — but generally validation logs should not be edited.

        // 🚫 DELETE (Optional – restrict strongly)
        [HttpPost]
        public ActionResult _Destroy([DataSourceRequest] DataSourceRequest request, FleetAvailabilityTresholdValidation model)
        {
            if (model != null)
            {
                var entity = db.FleetAvailabilityTresholdValidation
                               .FirstOrDefault(x => x.FleetAvailabilityTresholdValidationId == model.FleetAvailabilityTresholdValidationId);

                if (entity != null)
                {
                    // 🔒 Block delete if approved
                    if (entity.Status == ApprovalStatuses.Approve)
                    {
                        ModelState.AddModelError("", "Approved validation records cannot be deleted.");
                    }

                    if (ModelState.IsValid)
                    {
                        db.FleetAvailabilityTresholdValidation.Remove(entity);
                        db.SaveChanges();
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public JsonResult SendForApproval(int id)
        {
            var entity = db.FleetAvailabilityTreshold
                           .FirstOrDefault(x => x.FleetAvailabilityTresholdId == id);

            if (entity == null)
            {
                
                return Json(new {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // 🔒 Rule 1: Prevent re-sending
            if (entity.IsSendForApproval == true)
            {

                
                return Json(new { 
                    Message = "The record is already send for approval.",
                    Status = "NOT_OK"
            }, JsonRequestBehavior.AllowGet);
            }

            // 🔒 Rule 2: Prevent sending void records
            if (entity.IsVoid == true)
            {
                return Json(new {
                    Message = "The record is Void.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // 🔒 Rule 3: Prevent sending already approved
            if (entity.IsOnlineApproved == true)
            {
                return Json(new {
                    Message = "The record is Already Approved.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // ✅ Update main table
            entity.IsSendForApproval = true;
            entity.SendForApprovalBy = User.Identity.Name;
            entity.SendForApprovalTime = DateTime.Now;

            //// ✅ Add validation log
            //var validation = new FleetAvailabilityTresholdValidation
            //{
            //    FleetAvailabilityTresholdId = entity.FleetAvailabilityTresholdId,
            //    Status = ApprovalStatuses.Pending, // 🔥 important
            //    Remark = "Sent for approval",
            //    CreatedBy = User.Identity.Name,
            //    DateCreated = DateTime.Now
            //};

            //db.FleetAvailabilityTresholdValidation.Add(validation);

            //db.SaveChanges();
            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Send for approval successfully.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}