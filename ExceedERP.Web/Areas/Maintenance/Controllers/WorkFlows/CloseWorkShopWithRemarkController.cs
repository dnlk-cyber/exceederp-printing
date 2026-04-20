using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{

    public class CloseWorkShopWithRemarkController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // private readonly DistributionHelper _distributionHelper = new DistributionHelper();
        // private InventoryTransferHelper transferHelper;

        //[AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CloseWithRemark_Read([DataSourceRequest] DataSourceRequest request, string id)
        {
           
            //if (!Guid.TryParse(id, out Guid ServiceCenterJobId))
            //{
            //    return Json(new { error = "Invalid Job ID. Must be a valid GUID." });
            //}
            var fetchRemark = db.CloseWorkShopWithRemarks.Where(x => x.JobId == id).ToList();

            // Convert the fetched remarks into a DataSourceResult
            var Result = fetchRemark.ToDataSourceResult(request, R => new
            {
                CloseWorkShopWithRemarkId = R.CloseWorkShopWithRemarkId,
                JobId = R.JobId,
                Remark = R.Remark,
                ServiceName = R.ServiceName,
                JobOrderId = R.JobOrderId
            });

            return Json(Result);
        }
        public ActionResult CloseWithRemarkForJobOrder_Read([DataSourceRequest] DataSourceRequest request, string id)
        {
           
            //if (!Guid.TryParse(id, out Guid ServiceCenterJobId))
            //{
            //    return Json(new { error = "Invalid Job ID. Must be a valid GUID." });
            //}
            var fetchRemark = db.CloseWorkShopWithRemarks.Where(x => x.JobOrderId == id).ToList();

            // Convert the fetched remarks into a DataSourceResult
            var Result = fetchRemark.ToDataSourceResult(request, R => new
            {
                CloseWorkShopWithRemarkId = R.CloseWorkShopWithRemarkId,
                JobId = R.JobId,
                Remark = R.Remark,
                ServiceName = R.ServiceName,
                JobOrderId = R.JobOrderId
            });

            return Json(Result);
        }
        public ActionResult CloseWithRemark_Destroy([DataSourceRequest]DataSourceRequest request ,CloseWorkShopWithRemark Workshop)
        {
            var CheckAvailability = db.CloseWorkShopWithRemarks.FirstOrDefault(x => x.CloseWorkShopWithRemarkId==Workshop.CloseWorkShopWithRemarkId);
            if (ModelState.IsValid)
            {
                db.CloseWorkShopWithRemarks.Remove(CheckAvailability);
                db.SaveChanges();     
            }
            return Json(new[] { Workshop }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult CloseWithRemark_Update([DataSourceRequest]DataSourceRequest request ,CloseWorkShopWithRemark Workshop)
        {
            var CheckAvailability = db.CloseWorkShopWithRemarks.FirstOrDefault(x => x.CloseWorkShopWithRemarkId==Workshop.CloseWorkShopWithRemarkId);
            if (ModelState.IsValid)
            {
                CheckAvailability.Remark = Workshop.Remark;
                db.Entry(CheckAvailability).State = EntityState.Modified;
                db.SaveChanges();     
            }
            return Json(new[] { Workshop }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult CloseWithRemark_Create([DataSourceRequest]DataSourceRequest request, CloseWorkShopWithRemark Workshop, string id)
        {
            //var FetchCrew = db.ServiceCrewWorkLogs.Where(x => x.JobId.ToString() == id);
            //if(FetchCrew.Any())
            //{
            //    ModelState.AddModelError("Maintenance started", "You can not close workshop with remark it is getting maintained");

            //}
            //if()
            //var FetchStoreRequisition = db.StoreRequisitions.Where(x => x.ServiceJobOrderNo == id);
            //if(FetchStoreRequisition.Any())
            //{
            //    ModelState.AddModelError("Maintenance started", "You can not close workshop with remark it is getting maintained");

            //}
            var CheckAvailability = db.CloseWorkShopWithRemarks.FirstOrDefault(x => x.JobId == id);
            if (CheckAvailability != null)
            {
                ModelState.AddModelError("Remark Availble", "Remark is already written");
            }
            var closeWorkShop = new CloseWorkShopWithRemark();
            closeWorkShop.JobId = id;
            var FetchServiceJob = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id);

            var FetchJobOrder = db.JobOrder.SingleOrDefault(x => x.JobOrderID == FetchServiceJob.JobOrderID);
            if (FetchJobOrder == null)
            {
                ModelState.AddModelError("No Job Order Found", "No Job Order Found");
            }
            var FetchService = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id);
            if (FetchService == null)
            {
                ModelState.AddModelError("No Job Service Found", "No Job Service Order Found");
            }
            if (FetchServiceJob.IsSubmittedToServiceCenter)

            {
                ModelState.AddModelError("Record Already Send To Service Center", "Record Already Send To Service Center");
            }
            closeWorkShop.ServiceName = db.RepairType.FirstOrDefault(x => x.RepairTypeID == FetchService.RepairTypeID).Name;
            closeWorkShop.JobOrderId = FetchJobOrder.JobOrderID.ToString();
            if (ModelState.IsValid)
            {
                
                    closeWorkShop.CloseWorkShopWithRemarkId = closeWorkShop.CloseWorkShopWithRemarkId;
                     closeWorkShop.JobId = closeWorkShop.JobId;
                    closeWorkShop.Remark = Workshop.Remark;
                    db.CloseWorkShopWithRemarks.Add(closeWorkShop);
                    db.SaveChanges();
                
            }
            return Json(new[] { Workshop }.ToDataSourceResult(request, ModelState));

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
