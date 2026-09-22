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
    public class EquipmentOperationPlanValidationController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        // ✅ READ
        public ActionResult _Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var data = db.EquipmentOperationPlanValidation
                         .Where(x => x.EquipmentOperationPlanId == id)
                         .ToList();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // ✅ CREATE
        [HttpPost]
        public ActionResult _Create([DataSourceRequest] DataSourceRequest request, EquipmentOperationPlanValidation model, int id)
        {
            if (model != null)
            {
                // 🔒 Rule 1: Prevent duplicate SAME STATUS
                bool exists = db.EquipmentOperationPlanValidation.Any(x =>
                    x.EquipmentOperationPlanId == id &&
                    x.Status == model.Status);

                if (exists)
                {
                    ModelState.AddModelError("", "This status is already recorded for this record.");
                }

                // 🔒 Rule 2: Prevent approval if already approved
                bool alreadyApproved = db.EquipmentOperationPlanValidation.Any(x =>
                    x.EquipmentOperationPlanId == id &&
                    x.Status == ApprovalStatuses.Approve);

                if (alreadyApproved)
                {
                    ModelState.AddModelError("", "This record is already approved.");
                }

                if (ModelState.IsValid)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.DateCreated = DateTime.Now;
                    model.EquipmentOperationPlanId = id;
                    db.EquipmentOperationPlanValidation.Add(model);
                    db.SaveChanges();

                    // 🔄 Update main table
                    var main = db.EquipmentOperationPlan
                                 .FirstOrDefault(x => x.EquipmentOperationPlanId == id);

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

        // 🚫 DELETE
        [HttpPost]
        public ActionResult _Destroy([DataSourceRequest] DataSourceRequest request, EquipmentOperationPlanValidation model,int id)
        {
            if (model != null)
            {
                var entity = db.EquipmentOperationPlanValidation
                               .FirstOrDefault(x => x.EquipmentOperationPlanValidationId == id);

                if (entity != null)
                {
                    // 🔒 Block delete if approved
                    if (entity.Status == ApprovalStatuses.Approve)
                    {
                        ModelState.AddModelError("", "Approved records cannot be deleted.");
                    }

                    if (ModelState.IsValid)
                    {
                        db.EquipmentOperationPlanValidation.Remove(entity);
                        db.SaveChanges();
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ✅ SEND FOR APPROVAL
        [HttpPost]
        public JsonResult SendForApproval(int id)
        {
            var entity = db.EquipmentOperationPlan
                           .FirstOrDefault(x => x.EquipmentOperationPlanId == id);

            if (entity == null)
            {
                return Json(new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // 🔒 Rule 1: Prevent re-sending
            if (entity.IsSendForApproval == true)
            {
                return Json(new
                {
                    Message = "The record is already send for approval.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // 🔒 Rule 2: Prevent sending void records
            if (entity.IsVoid == true)
            {
                return Json(new
                {
                    Message = "The record is Void.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // 🔒 Rule 3: Prevent sending already approved
            if (entity.IsOnlineApproved == true)
            {
                return Json(new
                {
                    Message = "The record is Already Approved.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // ✅ Update main table
            entity.IsSendForApproval = true;
            entity.SendForApprovalBy = User.Identity.Name;
            entity.SendForApprovalTime = DateTime.Now;

            db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            // (Optional log — kept commented just like your original style)
            /*
            var validation = new EquipmentOperationPlanValidation
            {
                EquipmentOperationPlanId = entity.EquipmentOperationPlanId,
                Status = ApprovalStatuses.Pending,
                Remark = "Sent for approval",
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now
            };

            db.EquipmentOperationPlanValidation.Add(validation);
            db.SaveChanges();
            */

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