using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.AssetTransferReceive
{
    
    public class AssetTransferReceiveValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        
        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult AssetTransferReceiveValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<AssetTransferReceiveValidation> assetTransferReceiveValidations = _db.AssetTransferReceiveValidations.Where(ii => ii.AssetTransferReceiveId == id);
            DataSourceResult result = assetTransferReceiveValidations.ToDataSourceResult(request, assetTransferReceiveValidation => new
            {
                AssetTransferReceiveId = assetTransferReceiveValidation.AssetTransferReceiveId,
                AssetTransferReceiveValidationId = assetTransferReceiveValidation.AssetTransferReceiveValidationId,
                Status = assetTransferReceiveValidation.Status,
                Remark = assetTransferReceiveValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferReceiveValidations_Create([DataSourceRequest] DataSourceRequest request, AssetTransferReceiveValidation assetTransferReceiveValidation, int id)
        {

            var record = _db.AssetTransferReceives.Find(id);

            #region validation

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                {
                    if (assetTransferReceiveValidation.Status == ApprovalStatuses.Approve)
                    {
                        //foreach (var i in record.AssetTransferItems)
                        //{
                        //    if (!FixedAssetHelper.IsAssetCurrentLocationStore(i.AssetRegistrationId, i..StoreCode))
                        //    {
                        //        string message = "Asset with tag number " + i.TagNo + " does not exist in store " + i.FixedAssetIssue.StoreCode;
                        //        ModelState.AddModelError("doesnotexist", message);

                        //    }
                        //}
                    }
                    if (assetTransferReceiveValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "Issue record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (assetTransferReceiveValidation.Status == ApprovalStatuses.Approve)
                    {
                        #region update asset information
                        foreach (var i in record.AssetTransferReceiveItems)
                        {
                            UpdateAssetMovementAndUserCard(i);
                        }
                        #endregion
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;

                    }
                    if (assetTransferReceiveValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }

                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();

                    var entity = new AssetTransferReceiveValidation
                    {
                        AssetTransferReceiveId = id,
                        Status = assetTransferReceiveValidation.Status,
                        Remark = assetTransferReceiveValidation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = assetTransferReceiveValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = assetTransferReceiveValidation.ModifiedBy
                    };

                    _db.AssetTransferReceiveValidations.Add(entity);
                    _db.SaveChanges();
                    assetTransferReceiveValidation.AssetTransferReceiveValidationId = entity.AssetTransferReceiveValidationId;
                }
            }

            return Json(new[] { assetTransferReceiveValidation }.ToDataSourceResult(request, ModelState));

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferReceiveValidations_Update([DataSourceRequest]DataSourceRequest request, AssetTransferReceiveValidation assetTransferReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetTransferReceiveValidation
                {
                    AssetTransferReceiveValidationId = assetTransferReceiveValidation.AssetTransferReceiveValidationId,
                    Status = assetTransferReceiveValidation.Status,
                    Remark = assetTransferReceiveValidation.Remark,
                };

                _db.AssetTransferReceiveValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { assetTransferReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferReceiveValidations_Destroy([DataSourceRequest]DataSourceRequest request, AssetTransferReceiveValidation assetTransferReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetTransferReceiveValidation
                {
                    AssetTransferReceiveValidationId = assetTransferReceiveValidation.AssetTransferReceiveValidationId,
                    Status = assetTransferReceiveValidation.Status,
                    Remark = assetTransferReceiveValidation.Remark,
                };

                _db.AssetTransferReceiveValidations.Attach(entity);
                _db.AssetTransferReceiveValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { assetTransferReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateAssetMovementAndUserCard(AssetTransferReceiveItem item)
        {

            var asset = _db.AssetRegistrations.Find(item.AssetRegistrationId);
            if (item.TransferReceive.Transfer.Type == TransferType.PersonToPerson)
            {
                var employee = _db.Person_Employee.Find(item.TransferReceive.Transfer.TransferTo);
                var preEmployee = _db.Person_Employee.Find(item.TransferReceive.Transfer.CustodianBeforeTransfer);

                #region get employee and workunit

                string employeeid = employee?.EmployeeId.ToString() ?? "";
                string orgid = employee != null ? UserInformationHelper.GetEmployeeDepartmentIdByEmpId(employee.EmployeeId).ToString() : "";

                #endregion
                #region Update asset info

                if (asset != null)
                {
                    asset.EmployeeId = employeeid;
                    asset.OrgStructureId = orgid;
                    _db.Entry(asset).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                #endregion
                #region Update current UserCard

                UserCardItems userCard = new UserCardItems();
                if (employee != null)
                {
                    userCard.EmployeeId = employee.EmployeeId;
                    userCard.EmployeeFullName =
                        employee.FirstNameEnglish + "_" + employee.MiddleNameEnglish + "_" + employee.LastNameEnglish;
                }
                if (asset != null)
                {
                    userCard.AssetRegistrationId = asset.AssetRegistrationId;
                    userCard.Name = asset.Name;
                    userCard.Tag = asset.Tag;
                    userCard.IssueDate = item.TransferReceive.Transfer.TransferDate;
                    userCard.UnitPrice = asset.BookValue;
                    userCard.Quanity = item.Quantity;
                    userCard.SerialNo = item.SerialNo;
                    userCard.SIVNo = asset.SIV;
                }
                userCard.Active = true;
                userCard.DateCreated = DateTime.Now;
                userCard.CreatedBy = User.Identity.Name;
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
                        BranchId = UserInformationHelper.GetEmployeeBranchId(
                            UserInformationHelper.GetEmployeeUserId(int.Parse(employeeid))),
                        TransactionType = "Asset Transfer",
                        TransactionNo = item.AssetTransferReceiveId.ToString(),
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name
                    };
                    _db.LocationTrackings.Add(location);
                    _db.SaveChanges();
                }

                #endregion
                #region Update previous UserCard

                var prevUsercards = _db.UserCardItems.Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
                    .Where(x => x.EmployeeId == preEmployee.EmployeeId).ToList();
                foreach (var prev in prevUsercards)
                {
                    prev.Active = false;
                    _db.Entry(prev).State = EntityState.Modified;
                    _db.SaveChanges();
                }


                #endregion
            }
            else if (item.TransferReceive.Transfer.Type == TransferType.StoreToStore)
            {
                #region Update asset info

                if (asset != null)
                {
                    asset.StoreCode = item.TransferReceive.Transfer.ToStoreCode;
                    asset.OrgStructureId = "";
                    asset.BranchId = InventoryHelper.GetStoreBranch(item.TransferReceive.Transfer.ToStoreCode);
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
                        StoreCode = item.TransferReceive.Transfer.ToStoreCode,
                        LocationType = LocationType.Store,
                        StartDate = DateTime.Today,
                        BranchId = InventoryHelper.GetStoreBranch(item.TransferReceive.Transfer.ToStoreCode),
                        TransactionType = "Asset Transfer",
                        TransactionNo = item.AssetTransferReceiveId.ToString(),
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name
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
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
