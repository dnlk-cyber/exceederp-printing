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

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.MachineTransferReceive
{
    public class MachineryAndVehicleTransferReceiveValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult MachineryAndVehicleTransferReceiveValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MachineryAndVehicleTransferReceiveValidation> machineryAndVehicleTransferReceiveValidations = _db.MachineryAndVehicleTransferReceiveValidations.Where(x=>x.MachineryAndVehicleTransferReceiveId==id);
            DataSourceResult result = machineryAndVehicleTransferReceiveValidations.ToDataSourceResult(request, machineryAndVehicleTransferReceiveValidation => new {
                MachineryAndVehicleTransferReceiveId = machineryAndVehicleTransferReceiveValidation.MachineryAndVehicleTransferReceiveId,
                MachineryAndVehicleTransferReceiveValidationId = machineryAndVehicleTransferReceiveValidation.MachineryAndVehicleTransferReceiveValidationId,
                Status = machineryAndVehicleTransferReceiveValidation.Status,
                Remark = machineryAndVehicleTransferReceiveValidation.Remark,
                DateCreated = machineryAndVehicleTransferReceiveValidation.DateCreated,
                LastModified = machineryAndVehicleTransferReceiveValidation.LastModified,
                CreatedBy = machineryAndVehicleTransferReceiveValidation.CreatedBy,
                ModifiedBy = machineryAndVehicleTransferReceiveValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransferReceiveValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransferReceiveValidation machineryAndVehicleTransferReceiveValidation)
        {
            var record = _db.MachineryAndVehicleTransferReceives.Find(id);
            if (record != null && record.IsOnlineApproved)
            {
                string message = "You cannot manipulate this record,it  is already approved";
                ModelState.AddModelError("approved", message);
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (machineryAndVehicleTransferReceiveValidation.Status == ApprovalStatuses.Approve)
                    {

                        UpdateAssetMovementAndUserCard(record);

                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;

                    }
                    if (machineryAndVehicleTransferReceiveValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }

                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    var entity = new MachineryAndVehicleTransferReceiveValidation
                    {
                        MachineryAndVehicleTransferReceiveId = id,
                        Status = machineryAndVehicleTransferReceiveValidation.Status,
                        Remark = machineryAndVehicleTransferReceiveValidation.Remark,
                        DateCreated = machineryAndVehicleTransferReceiveValidation.DateCreated,
                        LastModified = machineryAndVehicleTransferReceiveValidation.LastModified,
                        CreatedBy = machineryAndVehicleTransferReceiveValidation.CreatedBy,
                        ModifiedBy = machineryAndVehicleTransferReceiveValidation.ModifiedBy
                    };

                    _db.MachineryAndVehicleTransferReceiveValidations.Add(entity);
                    _db.SaveChanges();
                    machineryAndVehicleTransferReceiveValidation.MachineryAndVehicleTransferReceiveValidationId =entity.MachineryAndVehicleTransferReceiveValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "MachineryAndVehicleTransferReceiveValidation", "Success", User.Identity.Name, $"created MachineryAndVehicleTransferReceiveValidation {entity.MachineryAndVehicleTransferReceiveValidationId} OrgBranchName {entity.OrgBranchName}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { machineryAndVehicleTransferReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransferReceiveValidations_Update([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransferReceiveValidation machineryAndVehicleTransferReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachineryAndVehicleTransferReceiveValidation
                {
                    MachineryAndVehicleTransferReceiveValidationId = machineryAndVehicleTransferReceiveValidation.MachineryAndVehicleTransferReceiveValidationId,
                    Status = machineryAndVehicleTransferReceiveValidation.Status,
                    Remark = machineryAndVehicleTransferReceiveValidation.Remark,
                    DateCreated = machineryAndVehicleTransferReceiveValidation.DateCreated,
                    LastModified = machineryAndVehicleTransferReceiveValidation.LastModified,
                    CreatedBy = machineryAndVehicleTransferReceiveValidation.CreatedBy,
                    ModifiedBy = machineryAndVehicleTransferReceiveValidation.ModifiedBy
                };

                _db.MachineryAndVehicleTransferReceiveValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "MachineryAndVehicleTransferReceiveValidation", "Success", User.Identity.Name, $"Update MachineryAndVehicleTransferReceiveValidation {entity.MachineryAndVehicleTransferReceiveValidationId} OrgBranchName {entity.OrgBranchName}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { machineryAndVehicleTransferReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransferReceiveValidations_Destroy([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransferReceiveValidation machineryAndVehicleTransferReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachineryAndVehicleTransferReceiveValidation
                {
                    MachineryAndVehicleTransferReceiveValidationId = machineryAndVehicleTransferReceiveValidation.MachineryAndVehicleTransferReceiveValidationId,
                    Status = machineryAndVehicleTransferReceiveValidation.Status,
                    Remark = machineryAndVehicleTransferReceiveValidation.Remark,
                    DateCreated = machineryAndVehicleTransferReceiveValidation.DateCreated,
                    LastModified = machineryAndVehicleTransferReceiveValidation.LastModified,
                    CreatedBy = machineryAndVehicleTransferReceiveValidation.CreatedBy,
                    ModifiedBy = machineryAndVehicleTransferReceiveValidation.ModifiedBy
                };

                _db.MachineryAndVehicleTransferReceiveValidations.Attach(entity);
                _db.MachineryAndVehicleTransferReceiveValidations.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "MachineryAndVehicleTransferReceiveValidation", "Success", User.Identity.Name, $"Delete MachineryAndVehicleTransferReceiveValidation {machineryAndVehicleTransferReceiveValidation.MachineryAndVehicleTransferReceiveValidationId} OrgBranchName {machineryAndVehicleTransferReceiveValidation.OrgBranchName}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { machineryAndVehicleTransferReceiveValidation }.ToDataSourceResult(request, ModelState));
        }


        private void UpdateAssetMovementAndUserCard(MachineryAndVehicleTransferReceive item)
        {
            
            var asset = _db.AssetRegistrations.Find(item.Transfer.AssetRegistrationId);
            if (item.Transfer.Type == TransferType.PersonToPerson)
            {
                var employee = _db.Person_Employee.Find(item.Transfer.TransferTo);
                var preEmployee = _db.Person_Employee.Find(item.Transfer.CustodianBeforeTransfer);

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

                #region Update current PersonEmployee

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
                    userCard.IssueDate = item.Transfer.TransferDate;
                    userCard.UnitPrice = asset.BookValue;
                    userCard.Quanity =1;
                    userCard.SerialNo = item.Transfer.SerialNo;
                    userCard.SIVNo = asset.SIV;
                }
                userCard.Active = true;
                userCard.DateCreated = DateTime.Now;
                userCard.CreatedBy = User.Identity.Name;
                _db.UserCardItems.Add(userCard);
                _db.SaveChanges();

                #endregion

                #region Update Location tracking for employee

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
                        TransactionType = FixedAssetTransactionType.MachineTransfer,//changed from string to enum
                        TransactionNo = item.MachineryAndVehicleTransferReceiveId.ToString(),
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name
                    };
                    _db.LocationTrackings.Add(location);
                    _db.SaveChanges();
                }

                #endregion

                #region Update previous PersonEmployee

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
            else if (item.Transfer.Type == TransferType.StoreToStore)
            {

                #region Update asset info

                if (asset != null)
                {
                    asset.StoreCode= item.Transfer.ToStoreCode;
                    asset.BranchId = InventoryHelper.GetStoreBranch(item.Transfer.ToStoreCode);
                    _db.Entry(asset).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                #endregion
                #region Update Location tracking for store

                if (asset != null)
                {
                    var location = new LocationTracking
                    {
                        AssetRegistrationId = asset.AssetRegistrationId,
                        StoreCode = item.Transfer.ToStoreCode,
                        LocationType = LocationType.Store,
                        StartDate = DateTime.Today,
                        BranchId = InventoryHelper.GetStoreBranch(item.Transfer.ToStoreCode),
                        TransactionType = FixedAssetTransactionType.MachineTransfer,//changed from string to enum
                        TransactionNo = item.MachineryAndVehicleTransferReceiveId.ToString(),
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
