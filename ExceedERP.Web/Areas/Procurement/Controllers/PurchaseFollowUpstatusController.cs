using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    public class PurchaseFollowUpstatusController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult PurchaseFollowUpSystemStatus_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchaseFollowUpSystemStatus> purchaseFollowUpStatuses = db.PurchaseFollowUpSystemStatus.Where(pri => pri.PurchaseFollowUpID == id);
            DataSourceResult result = purchaseFollowUpStatuses.ToDataSourceResult(request, purchaseFollowUpStatus => new
            {
                PurchaseFollowUpID = purchaseFollowUpStatus.PurchaseFollowUpID,
                Date = purchaseFollowUpStatus.Date,
                PurchaseFollowUpSystemStatusId = purchaseFollowUpStatus.PurchaseFollowUpSystemStatusId,
                Description = purchaseFollowUpStatus.Description,
                User = purchaseFollowUpStatus.User
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult PurchaseFollowUpstatus_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<PurchaseFollowUpStatus> purchaseFollowUpStatuses = db.PurchaseStatus.Where(pri => pri.PurchaseFollowUpID == id);
            DataSourceResult result = purchaseFollowUpStatuses.ToDataSourceResult(request, purchaseFollowUpStatus => new {
                Date = purchaseFollowUpStatus.Date,
                PurchaseFollowUpID = purchaseFollowUpStatus.PurchaseFollowUpID,
                PurchaseFollowUpStatusId = purchaseFollowUpStatus.PurchaseFollowUpStatusId,
                Description = purchaseFollowUpStatus.Description,
                Remark = purchaseFollowUpStatus.Remark
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseFollowUpstatus_Create([DataSourceRequest]DataSourceRequest request, PurchaseFollowUpStatus purchaseFollowUpStatus , int id )
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseFollowUpStatus
                {
                    PurchaseFollowUpID = id,
                    Date=purchaseFollowUpStatus.Date,
                    PurchaseFollowUpStatusId = purchaseFollowUpStatus.PurchaseFollowUpStatusId,
                    Description = purchaseFollowUpStatus.Description,
                    Remark = purchaseFollowUpStatus.Remark
                };

                db.PurchaseStatus.Add(entity);
                db.SaveChanges();
                purchaseFollowUpStatus.PurchaseFollowUpStatusId = entity.PurchaseFollowUpStatusId;
            }

            return Json(new[] { purchaseFollowUpStatus }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseFollowUpstatus_Update([DataSourceRequest]DataSourceRequest request, PurchaseFollowUpStatus purchaseFollowUpStatus)
        {
            if (ModelState.IsValid)
            {
                 var entity = new PurchaseFollowUpStatus
                {
                    PurchaseFollowUpID = purchaseFollowUpStatus.PurchaseFollowUpID,
                    Date = purchaseFollowUpStatus.Date,
                    PurchaseFollowUpStatusId = purchaseFollowUpStatus.PurchaseFollowUpStatusId,
                    Description = purchaseFollowUpStatus.Description,
                    Remark = purchaseFollowUpStatus.Remark
                };


                db.PurchaseStatus.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { purchaseFollowUpStatus }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
       public ActionResult PurchaseFollowUpstatus_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseFollowUpStatus purchaseFollowUpStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseFollowUpStatus
                {
                    PurchaseFollowUpID = purchaseFollowUpStatus.PurchaseFollowUpID,
                    Date = purchaseFollowUpStatus.Date,
                    PurchaseFollowUpStatusId = purchaseFollowUpStatus.PurchaseFollowUpStatusId,
                    Description = purchaseFollowUpStatus.Description,
                    Remark = purchaseFollowUpStatus.Remark
                };

                db.PurchaseStatus.Attach(entity);
                db.PurchaseStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { purchaseFollowUpStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
