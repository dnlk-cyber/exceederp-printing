using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Returns;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.FixedAsset;
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

            var fixedAssetReturn = _db.FixedAssetReturns.Find(id);

            #region Checking the return

            if (fixedAssetReturn != null)
            {
                if (fixedAssetReturn.IsOnlineApproved)
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
                if (fixedAssetReturn != null)
                {
                    //validate 
                    if (fixedAssetReturnValidation.Status == ApprovalStatuses.Approve)
                    {
                        #region update locationTracking and userCard
                        foreach (var fixedAssetReturnItem in fixedAssetReturn.FixedAssetReturnItems)
                        {
                            //UpdateAssetMovementAndUserCard(fixedAssetReturnItem);
                            //UpdateLocationTrackingAndUserCard(fixedAssetReturnItem);

                            // Update LocationTracking
                            FixedAssetHelper.UpdateLocationTracking(fixedAssetReturnItem, User.Identity.Name);

                            // Update AssetInfo
                            FixedAssetHelper.UpdateAssetInfo(fixedAssetReturnItem);

                            // Remove from UserCard
                            FixedAssetHelper.RemovePreviousUserCard(fixedAssetReturnItem);
                        }
                        #endregion
                        fixedAssetReturn.Status = "Approved";
                        fixedAssetReturn.IsOnlineApproved = true;
                        fixedAssetReturn.OnlineApprovedBy = User.Identity.Name;
                        fixedAssetReturn.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(fixedAssetReturn).State = EntityState.Modified;
                        _db.SaveChanges();



                    }
                    if (fixedAssetReturnValidation.Status == ApprovalStatuses.Reject)
                    {
                        fixedAssetReturn.IsSendForApproval = false;
                        fixedAssetReturn.Status = "Rejected";
                        _db.Entry(fixedAssetReturn).State = EntityState.Modified;
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
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReturnValidation", "Success", User.Identity.Name, $"created FixedAssetReturnValidation {entity.FixedAssetReturnValidationId} FixedAssetReturnId {entity.FixedAssetReturnId}", Modules.FixedAsset);
                    #endregion
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReturnValidation", "Success", User.Identity.Name, $"update FixedAssetReturnValidation {entity.FixedAssetReturnValidationId} FixedAssetReturnId {entity.FixedAssetReturnId}", Modules.FixedAsset);
                #endregion
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"delete FixedAssetReturnValidation {fixedAssetReturnValidation.FixedAssetReturnValidationId} FixedAssetReturnId {fixedAssetReturnValidation.FixedAssetReturnId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReturnValidation }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateLocationTrackingAndUserCard(FixedAssetReturnItem fixedAssetReturnItem)
        {
            //var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name).ToString();
            //var orgId = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name).ToString();
            //var user = User.Identity.Name;
            
            // Update LocationTracking
            //FixedAssetHelper.UpdateLocationTracking(fixedAssetReturnItem, User.Identity.Name);

            //// Update AssetInfo
            //FixedAssetHelper.UpdateAssetInfo(fixedAssetReturnItem);

            //// Remove from UserCard
            //FixedAssetHelper.RemovePreviousUserCard(fixedAssetReturnItem);
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
                    TransactionType = FixedAssetTransactionType.Return,
                    TransactionNo = item.FixedAssetReturnId.ToString(),
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };
                _db.LocationTrackings.Add(location);
                _db.SaveChanges();

                #region update previous location track as not current

                var userCard = _db.UserCardItems.Where(q => q.AssetRegistrationId == asset.AssetRegistrationId).FirstOrDefault();
                if(userCard!=null)
                {
                    _db.UserCardItems.Attach(userCard);
                    _db.UserCardItems.Remove(userCard);
                }

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
