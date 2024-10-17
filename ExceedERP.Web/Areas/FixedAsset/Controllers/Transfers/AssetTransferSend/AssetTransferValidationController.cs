using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.AssetTransferSend
{

    public class AssetTransferValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult AssetTransferValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<AssetTransferValidation> assetTransferValidations = _db.AssetTransferValidations.Where(ii => ii.AssetTransferId == id);
            DataSourceResult result = assetTransferValidations.ToDataSourceResult(request, assetTransferValidation => new
            {
                AssetTransferId = assetTransferValidation.AssetTransferId,
                AssetTransferValidationId = assetTransferValidation.AssetTransferValidationId,
                Status = assetTransferValidation.Status,
                Remark = assetTransferValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferValidations_Create([DataSourceRequest] DataSourceRequest request, AssetTransferValidation assetTransferValidation, int id)
        {

            var assetTransfer = _db.AssetTransfers.Find(id);

            var assetTransferItems = assetTransfer.AssetTransferItems;

            var user = User.Identity.Name;
            #region validation

            

            if (assetTransfer != null)
            {
                if (!assetTransferItems.Any()) ModelState.AddModelError(string.Empty, "No Items to transfer");

                if (assetTransfer.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                //{
                //    if (assetTransferValidation.Status == ApprovalStatuses.Approve)
                //    {
                //        foreach (var i in assetTransfer.AssetTransferItems)
                //        {
                //            if (!FixedAssetHelper.IsAssetCurrentLocationStore(i.AssetRegistrationId, i..StoreCode))
                //            {
                //                string message = "Asset with tag number " + i.TagNo + " does not exist in store " + i.FixedAssetIssue.StoreCode;
                //                ModelState.AddModelError("doesnotexist", message);

                //            }
                //        }
                //    }
                //    if (assetTransferValidation.Status == ApprovalStatuses.Reject)
                //    {
                //        assetTransfer.IsSendForApproval = false;
                //        assetTransfer.Status = "Rejected";
                //    }
                //}
            }
            else
            {
                ModelState.AddModelError("No Found", "Issue record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (assetTransfer != null && assetTransferItems.Any())
                {
                    //validate 
                    if (assetTransferValidation.Status == ApprovalStatuses.Approve)
                    {
                        //#region update Movement in locationTracking and userCard

                        //foreach (var assetTransferItem in assetTransferItems)
                        //{
                        //    // Update UserCard
                        //    FixedAssetHelper.UpdateUserCard(assetTransferItem, user);

                        //    // Update LocationTracking
                        //    FixedAssetHelper.UpdateLocationTrackingForStore(assetTransferItem, user);
                        //}
                        //#endregion
                        assetTransfer.Status = "Approved";
                        assetTransfer.IsOnlineApproved = true;
                        assetTransfer.OnlineApprovedBy = User.Identity.Name;
                        assetTransfer.OnlineApprovedTime = DateTime.Now;
                        
                    }
                    if (assetTransferValidation.Status == ApprovalStatuses.Reject)
                    {
                        assetTransfer.IsSendForApproval = false;
                        assetTransfer.Status = "Rejected";
                    }

                    _db.Entry(assetTransfer).State = EntityState.Modified;
                    _db.SaveChanges();

                    var entity = new AssetTransferValidation
                    {
                        AssetTransferId = id,
                        Status = assetTransferValidation.Status,
                        Remark = assetTransferValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = assetTransferValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = assetTransferValidation.ModifiedBy
                    };

                    _db.AssetTransferValidations.Add(entity);
                    _db.SaveChanges();
                    assetTransferValidation.AssetTransferValidationId = entity.AssetTransferValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AssetTransferValidation", "Success", User.Identity.Name, $"created AssetTransferValidation {entity.AssetTransferValidationId} Tag {entity.AssetTransferId}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { assetTransferValidation }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferValidations_Update([DataSourceRequest]DataSourceRequest request, AssetTransferValidation assetTransferValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetTransferValidation
                {
                    AssetTransferValidationId = assetTransferValidation.AssetTransferValidationId,
                    Status = assetTransferValidation.Status,
                    Remark = assetTransferValidation.Remark,
                };

                _db.AssetTransferValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AssetTransferValidation", "Success", User.Identity.Name, $"update AssetTransferValidation {entity.AssetTransferValidationId} Tag {entity.AssetTransferId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { assetTransferValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferValidations_Destroy([DataSourceRequest]DataSourceRequest request, AssetTransferValidation assetTransferValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetTransferValidation
                {
                    AssetTransferValidationId = assetTransferValidation.AssetTransferValidationId,
                    Status = assetTransferValidation.Status,
                    Remark = assetTransferValidation.Remark,
                };

                _db.AssetTransferValidations.Attach(entity);
                _db.AssetTransferValidations.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete AssetTransferValidation {assetTransferValidation.AssetTransferValidationId} Tag {assetTransferValidation.AssetTransferId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { assetTransferValidation }.ToDataSourceResult(request, ModelState));
        }

        
        /// <summary>
        /// updates userCard, assetInfo, and location tracking 
        /// </summary>
        /// <param name="item"></param>
        private void UpdateAssetMovementAndUserCard(AssetTransferItem item)
        {

            var asset = _db.AssetRegistrations.Find(item.AssetRegistrationId);

            //if transfer type is person to person
            if (item.Transfer.Type == TransferType.PersonToPerson)
            {
                var transferTo = _db.Person_Employee.Find(item.Transfer.TransferTo);
                var custodianBeforeTransfer = _db.Person_Employee.Find(item.Transfer.CustodianBeforeTransfer);

                #region get employee id and workunit

                string employeeid = transferTo?.EmployeeId.ToString() ?? "";
                string orgid = transferTo != null ? UserInformationHelper.GetEmployeeDepartmentIdByEmpId(transferTo.EmployeeId).ToString() : "";

                #endregion
                #region Update asset info

                if (asset != null)
                {
                    asset.EmployeeId = employeeid;
                    asset.OrgStructureId = orgid;

                    //save on db
                    _db.Entry(asset).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                #endregion
                #region Update current UserCard

                UserCardItems userCard = new UserCardItems();
                if (transferTo != null)
                {
                    userCard.EmployeeId = transferTo.EmployeeId;
                    userCard.EmployeeFullName =
                        transferTo.FirstNameEnglish + "_" + transferTo.MiddleNameEnglish + "_" + transferTo.LastNameEnglish;
                }
                if (asset != null)
                {
                    userCard.AssetRegistrationId = asset.AssetRegistrationId;
                    userCard.Name = asset.Name;
                    userCard.Tag = asset.Tag;
                    userCard.IssueDate = item.Transfer.TransferDate;
                    userCard.UnitPrice = asset.BookValue;
                    userCard.Quanity = item.Quantity;
                    userCard.SerialNo = item.SerialNo;
                    userCard.SIVNo = asset.SIV;
                }
                userCard.Active = true;
                userCard.DateCreated = DateTime.Now;
                userCard.CreatedBy = User.Identity.Name;

                //save on db
                _db.UserCardItems.Add(userCard);
                _db.SaveChanges();

                #endregion
                #region Update Location tracking

                if (asset != null)
                {
                    var location = new LocationTracking
                    {
                        AssetRegistrationId = asset.AssetRegistrationId,
                        LocationType = LocationType.Employee,
                        EmployeeId = employeeid,
                        OrgStructureId = orgid,
                        StartDate = DateTime.Today,
                        BranchId = UserInformationHelper.GetUserDefaultBranchId(
                            UserInformationHelper.GetEmployeeUserId(int.Parse(employeeid))),
                        TransactionType = FixedAssetTransactionType.AssetTransfer,//changed from string to enum
                        TransactionNo = item.AssetTransferId.ToString(),
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name
                    };
                    _db.LocationTrackings.Add(location);
                    _db.SaveChanges();
                }

                #endregion

                #region Update previous usercard

                var prevUsercards = _db.UserCardItems.Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
                    .Where(x => x.EmployeeId == custodianBeforeTransfer.EmployeeId).ToList();
                foreach (var prev in prevUsercards)
                {
                    prev.Active = false;
                    _db.Entry(prev).State = EntityState.Modified;
                    _db.SaveChanges();
                }


                #endregion
            }
            //If TransferType is Store To Store
            else if (item.Transfer.Type == TransferType.StoreToStore)
            {
                #region Update asset info

                if (asset != null)
                {
                    asset.StoreCode = item.Transfer.ToStoreCode;
                    asset.OrgStructureId = "";
                    asset.BranchId = InventoryHelper.GetStoreBranch(item.Transfer.ToStoreCode);
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
                        StoreCode = item.Transfer.ToStoreCode,
                        LocationType = LocationType.Store,
                        StartDate = DateTime.Today,
                        BranchId = InventoryHelper.GetStoreBranch(item.Transfer.ToStoreCode),
                        TransactionType = FixedAssetTransactionType.AssetTransfer,
                        TransactionNo = item.AssetTransferId.ToString(),
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name
                    };
                    _db.LocationTrackings.Add(location);
                    _db.SaveChanges();

                   

                    #region update previous location tracking as not current 

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
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
