using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Requisitions
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]
    public class FuelRequisitionPropertyValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FuelRequisitionPropertyValidations_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<StoreRequisitionPropertyValidation> storerequisitionpropertyvalidations = db.StoreRequisitionPropertyValidations.Where(x => x.StoreRequisitionID == id);
            DataSourceResult result = storerequisitionpropertyvalidations.ToDataSourceResult(request, storeRequisitionPropertyValidation => new {
                StoreRequisitionPropertyValidationID = storeRequisitionPropertyValidation.StoreRequisitionPropertyValidationID,
                StoreRequisitionID = storeRequisitionPropertyValidation.StoreRequisitionID,
                Status = storeRequisitionPropertyValidation.Status,
                Remark = storeRequisitionPropertyValidation.Remark,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelRequisitionPropertyValidations_Create([DataSourceRequest]DataSourceRequest request, StoreRequisitionPropertyValidation storeRequisitionPropertyValidation , int id)
        {
            StoreRequisition storeRequisition = db.StoreRequisitions.Find(id);
            if (storeRequisition != null && storeRequisition.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate this Record: It is Already Approved");
            }
            if (storeRequisitionPropertyValidation.Status == ApprovalStatuses.Approve)
            {
                if (storeRequisition != null)
                {
                    storeRequisition.Status = "Property Approved";
                    storeRequisition.IsOnlineApproved = true;
                    storeRequisition.IsSecondOnlineApproved = true;
                    storeRequisition.OnlineApprovedBy = User.Identity.Name;
                    storeRequisition.OnlineApprovedTime = DateTime.Now.Date;
                }
            }
            else if (storeRequisitionPropertyValidation.Status == ApprovalStatuses.Reject)
            {
                if (storeRequisition != null)
                {
                    storeRequisition.Status = Enum.GetName(typeof(Core.Domain.FleetManagement.FleetOperationStatuses), 8);
                    storeRequisition.IsSecondSendForApproval = false;
                }
            }
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionPropertyValidation
                {
                    StoreRequisitionID = id,
                    Status = storeRequisitionPropertyValidation.Status,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = storeRequisitionPropertyValidation.Remark,
                };

                db.StoreRequisitionPropertyValidations.Add(entity);
                db.SaveChanges();
                storeRequisitionPropertyValidation.StoreRequisitionPropertyValidationID = entity.StoreRequisitionPropertyValidationID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "fuel requisition property approval", "Success", User.Identity.Name, $"approved fuel requisition {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion
            }
            db.Entry(storeRequisition).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new[] { storeRequisitionPropertyValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelRequisitionPropertyValidations_Update([DataSourceRequest]DataSourceRequest request, StoreRequisitionPropertyValidation storeRequisitionPropertyValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionPropertyValidation
                {
                    StoreRequisitionPropertyValidationID = storeRequisitionPropertyValidation.StoreRequisitionPropertyValidationID,
                    StoreRequisitionID = storeRequisitionPropertyValidation.StoreRequisitionID,
                    Status = storeRequisitionPropertyValidation.Status,
                    Remark = storeRequisitionPropertyValidation.Remark,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now
                };

                db.StoreRequisitionPropertyValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { storeRequisitionPropertyValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelRequisitionPropertyValidations_Destroy([DataSourceRequest]DataSourceRequest request, StoreRequisitionPropertyValidation storeRequisitionPropertyValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionPropertyValidation
                {
                    StoreRequisitionPropertyValidationID = storeRequisitionPropertyValidation.StoreRequisitionPropertyValidationID,
                    Status = storeRequisitionPropertyValidation.Status,
                    Remark = storeRequisitionPropertyValidation.Remark,
                    StoreRequisitionID = storeRequisitionPropertyValidation.StoreRequisitionID,
                };

                db.StoreRequisitionPropertyValidations.Attach(entity);
                db.StoreRequisitionPropertyValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { storeRequisitionPropertyValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
