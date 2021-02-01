using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Assetregistration
{
    public class LocationTrackingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult LocationTrackings_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<LocationTracking> locationtrackings = db.LocationTrackings.Where(x => x.AssetRegistrationId == id);
            DataSourceResult result = locationtrackings.ToDataSourceResult(request, locationTracking => new
            {
                LocationTrackingId = locationTracking.LocationTrackingId,
                LocationType = locationTracking.LocationType,
                StartDate = locationTracking.StartDate,
                EndDateDate = locationTracking.EndDateDate,
                BranchId = locationTracking.BranchId,
                StoreCode = locationTracking.StoreCode,
                EmployeeId = locationTracking.EmployeeId,
                Description = locationTracking.Description,
                TransactionType = locationTracking.TransactionType,
                TransactionNo = locationTracking.TransactionNo,
                OrgBranchName = locationTracking.OrgBranchName,
                DateCreated = locationTracking.DateCreated,
                LastModified = locationTracking.LastModified,
                CreatedBy = locationTracking.CreatedBy,
                ModifiedBy = locationTracking.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationTrackings_Create([DataSourceRequest]DataSourceRequest request, LocationTracking locationTracking,int id)
        {
            #region validation
            if(locationTracking.LocationType==LocationType.NotSet)
                ModelState.AddModelError("type","Please choose location type");
            if (locationTracking.BranchId == 0)
                ModelState.AddModelError("branch", "Please choose branch");
            if (locationTracking.LocationType == LocationType.Employee && String.IsNullOrEmpty(locationTracking.EmployeeId))
                ModelState.AddModelError("type", "Please choose employee");
            if (locationTracking.LocationType == LocationType.Store && String.IsNullOrEmpty(locationTracking.StoreCode))
                ModelState.AddModelError("type", "Please choose store");
            #endregion

            
            if (ModelState.IsValid&& locationTracking.LocationTrackingId == 0)
            {
                var entity = new LocationTracking
                {
                    AssetRegistrationId = id,
                    IsCurrent=true,
                    LocationType = locationTracking.LocationType,
                    BranchId = locationTracking.BranchId,
                    EmployeeId = locationTracking.EmployeeId,
                    StoreCode = locationTracking.StoreCode,
                    StartDate = locationTracking.StartDate,
                    EndDateDate = locationTracking.EndDateDate,
                    Description = locationTracking.Description,
                    TransactionType = locationTracking.TransactionType,
                    TransactionNo = locationTracking.TransactionNo,
                    OrgBranchName = locationTracking.OrgBranchName,
                    DateCreated = locationTracking.DateCreated,
                    LastModified = locationTracking.LastModified,
                    CreatedBy = locationTracking.CreatedBy,
                    ModifiedBy = locationTracking.ModifiedBy
                };

                db.LocationTrackings.Add(entity);
                db.SaveChanges();
                locationTracking.LocationTrackingId = entity.LocationTrackingId;

                #region update previous location track as not current

                var prevs = db.LocationTrackings.Where(x => x.AssetRegistrationId == id)
                    .Where(x => x.LocationTrackingId != entity.LocationTrackingId).ToList();
                foreach (var prev in prevs)
                {
                    prev.IsCurrent = false;
                    db.Entry(prev).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                #endregion
                #region update store  to registration

                    if (locationTracking.LocationType == LocationType.Store &&
                    !String.IsNullOrEmpty(locationTracking.StoreCode))
                {
                    var asset = db.AssetRegistrations.Find(id);
                    if (asset != null)
                    {
                        asset.StoreCode = locationTracking.StoreCode;
                        db.Entry(asset).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                #endregion
                }

            return Json(new[] { locationTracking }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
