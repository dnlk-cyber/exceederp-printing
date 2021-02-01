using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Returns;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Return
{
    
    public class FixedAssetReturnValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetReturnValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetReturnValidation> fixedAssetReturnValidations = _db.FixedAssetReturnValidations.Where(ii => ii.FixedAssetReturnId == id);
            DataSourceResult result = fixedAssetReturnValidations.ToDataSourceResult(request, fixedAssetReturnValidation => new
            {
                FixedAssetReturnValidationId = fixedAssetReturnValidation.FixedAssetReturnValidationId,
                Status = fixedAssetReturnValidation.Status,
                Remark = fixedAssetReturnValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnValidations_Create([DataSourceRequest] DataSourceRequest request, FixedAssetReturnValidation fixedAssetReturnValidation, int id)
        {

            var record = _db.FixedAssetReturns.Find(id);

            #region Checking the return

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                {
                    if (fixedAssetReturnValidation.Status == ApprovalStatuses.Approve)
                    {
                        //foreach (FixedAssetReturnItem i in record.FixedAssetReturnItems)
                        //{
                           
                        //}
                    }
                   
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "Record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (fixedAssetReturnValidation.Status == ApprovalStatuses.Approve)
                    {
                        #region update asset information
                        foreach (var i in record.FixedAssetReturnItems)
                        {
                            UpdateAssetMovementAndUserCard(i);
                        }
                        #endregion
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                    }
                    if (fixedAssetReturnValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                   

                    var entity = new FixedAssetReturnValidation
                    {
                        FixedAssetReturnId = id,
                        Status = fixedAssetReturnValidation.Status,
                        Remark = fixedAssetReturnValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = fixedAssetReturnValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = fixedAssetReturnValidation.ModifiedBy
                    };

                    _db.FixedAssetReturnValidations.Add(entity);
                    _db.SaveChanges();
                    fixedAssetReturnValidation.FixedAssetReturnValidationId = entity.FixedAssetReturnValidationId;
                }
            }

            return Json(new[] { fixedAssetReturnValidation }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnValidations_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReturnValidation fixedAssetReturnValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnValidation
                {
                    FixedAssetReturnValidationId = fixedAssetReturnValidation.FixedAssetReturnValidationId,
                    Status = fixedAssetReturnValidation.Status,
                    Remark = fixedAssetReturnValidation.Remark,
                };

                _db.FixedAssetReturnValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { fixedAssetReturnValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturnValidations_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetReturnValidation fixedAssetReturnValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturnValidation
                {
                    FixedAssetReturnValidationId = fixedAssetReturnValidation.FixedAssetReturnValidationId,
                    Status = fixedAssetReturnValidation.Status,
                    Remark = fixedAssetReturnValidation.Remark,
                };

                _db.FixedAssetReturnValidations.Attach(entity);
                _db.FixedAssetReturnValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { fixedAssetReturnValidation }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateAssetMovementAndUserCard(FixedAssetReturnItem item)
        {

            var asset = _db.AssetRegistrations.Find(item.AssetRegistrationId);

            #region get employee and workunit

            string employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name).ToString();
            string orgid = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name).ToString();

            #endregion

            #region Update asset info

            if (asset != null)
            {
                asset.StoreCode = item.FixedAssetReturn.StoreCode;
                asset.EmployeeId = employeeid;
                asset.OrgStructureId = orgid;
                _db.Entry(asset).State = EntityState.Modified;
                _db.SaveChanges();
            }

            #endregion

            #region Update Location tracking to store

            if (asset != null)
            {
                var location = new LocationTracking
                {
                    AssetRegistrationId = asset.AssetRegistrationId,
                    StoreCode = item.FixedAssetReturn.StoreCode,
                    EmployeeId=employeeid,
                    OrgStructureId=orgid,
                    LocationType = LocationType.Store,
                    StartDate = DateTime.Today,
                    BranchId = InventoryHelper.GetStoreBranch(item.FixedAssetReturn.StoreCode),
                    TransactionType = "Return",
                    TransactionNo = item.FixedAssetReturnId.ToString(),
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };
                _db.LocationTrackings.Add(location);
                _db.SaveChanges();

                #region update previous location track as not current

                var prevs = _db.LocationTrackings.Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
                    .Where(x => x.LocationTrackingId != location.LocationTrackingId).ToList();
                foreach (var prev in prevs)
                {
                    prev.IsCurrent = false;
                    _db.Entry(prev).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                #endregion
            }

            #endregion
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
