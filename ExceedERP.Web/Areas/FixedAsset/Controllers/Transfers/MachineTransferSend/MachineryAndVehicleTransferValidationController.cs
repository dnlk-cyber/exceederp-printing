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

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.MachineTransferSend
{
    public class MachineryAndVehicleTransferValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult MachineryAndVehicleTransferValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MachineryAndVehicleTransferValidation> machineryandvehicletransfervalidations = _db.MachineryAndVehicleTransferValidations.Where(x=>x.MachineryAndVehicleTransferId==id);
            DataSourceResult result = machineryandvehicletransfervalidations.ToDataSourceResult(request, machineryAndVehicleTransferValidation => new {
                MachineryAndVehicleTransferId = machineryAndVehicleTransferValidation.MachineryAndVehicleTransferId,
                MachineryAndVehicleTransferValidationId = machineryAndVehicleTransferValidation.MachineryAndVehicleTransferValidationId,
                Status = machineryAndVehicleTransferValidation.Status,
                Remark = machineryAndVehicleTransferValidation.Remark,
                DateCreated = machineryAndVehicleTransferValidation.DateCreated,
                LastModified = machineryAndVehicleTransferValidation.LastModified,
                CreatedBy = machineryAndVehicleTransferValidation.CreatedBy,
                ModifiedBy = machineryAndVehicleTransferValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransferValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransferValidation machineryAndVehicleTransferValidation)
        {
            var record = _db.MachineryAndVehicleTransfers.Find(id);
            if (record != null && record.IsOnlineApproved)
            {
                string message = "You cannot manipulate this record,it  is already approved";
                ModelState.AddModelError("approved", message);
            }
            if (record.Type == TransferType.PersonToPerson)
            {
                if (record.CustodianBeforeTransfer == null || record.TransferTo == null)
                    ModelState.AddModelError("employee", "Please choose custodian before and receiver.");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    //validate 
                    if (machineryAndVehicleTransferValidation.Status == ApprovalStatuses.Approve)
                    {

                        UpdateAssetMovementAndUserCard(record);

                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;

                    }
                    if (machineryAndVehicleTransferValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }

                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    var entity = new MachineryAndVehicleTransferValidation
                    {
                        MachineryAndVehicleTransferId = id,
                        Status = machineryAndVehicleTransferValidation.Status,
                        Remark = machineryAndVehicleTransferValidation.Remark,
                        DateCreated = machineryAndVehicleTransferValidation.DateCreated,
                        LastModified = machineryAndVehicleTransferValidation.LastModified,
                        CreatedBy = machineryAndVehicleTransferValidation.CreatedBy,
                        ModifiedBy = machineryAndVehicleTransferValidation.ModifiedBy
                    };

                    _db.MachineryAndVehicleTransferValidations.Add(entity);
                    _db.SaveChanges();
                    machineryAndVehicleTransferValidation.MachineryAndVehicleTransferValidationId =entity.MachineryAndVehicleTransferValidationId;
                }
            }

            return Json(new[] { machineryAndVehicleTransferValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransferValidations_Update([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransferValidation machineryAndVehicleTransferValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachineryAndVehicleTransferValidation
                {
                    MachineryAndVehicleTransferValidationId = machineryAndVehicleTransferValidation.MachineryAndVehicleTransferValidationId,
                    Status = machineryAndVehicleTransferValidation.Status,
                    Remark = machineryAndVehicleTransferValidation.Remark,
                    DateCreated = machineryAndVehicleTransferValidation.DateCreated,
                    LastModified = machineryAndVehicleTransferValidation.LastModified,
                    CreatedBy = machineryAndVehicleTransferValidation.CreatedBy,
                    ModifiedBy = machineryAndVehicleTransferValidation.ModifiedBy
                };

                _db.MachineryAndVehicleTransferValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { machineryAndVehicleTransferValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransferValidations_Destroy([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransferValidation machineryAndVehicleTransferValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachineryAndVehicleTransferValidation
                {
                    MachineryAndVehicleTransferValidationId = machineryAndVehicleTransferValidation.MachineryAndVehicleTransferValidationId,
                    Status = machineryAndVehicleTransferValidation.Status,
                    Remark = machineryAndVehicleTransferValidation.Remark,
                    DateCreated = machineryAndVehicleTransferValidation.DateCreated,
                    LastModified = machineryAndVehicleTransferValidation.LastModified,
                    CreatedBy = machineryAndVehicleTransferValidation.CreatedBy,
                    ModifiedBy = machineryAndVehicleTransferValidation.ModifiedBy
                };

                _db.MachineryAndVehicleTransferValidations.Attach(entity);
                _db.MachineryAndVehicleTransferValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { machineryAndVehicleTransferValidation }.ToDataSourceResult(request, ModelState));
        }


        private void UpdateAssetMovementAndUserCard(MachineryAndVehicleTransfer item)
        {
            
            var asset = _db.AssetRegistrations.Find(item.AssetRegistrationId);
            if (item.Type == TransferType.PersonToPerson)
            {
                var employee = _db.Person_Employee.Find(item.TransferTo);
                var preEmployee = _db.Person_Employee.Find(item.CustodianBeforeTransfer);

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
                    userCard.IssueDate = item.TransferDate;
                    userCard.UnitPrice = asset.BookValue;
                    userCard.Quanity =1;
                    userCard.SerialNo = item.SerialNo;
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
                        BranchId = UserInformationHelper.GetEmployeeBranchId(
                            UserInformationHelper.GetEmployeeUserId(int.Parse(employeeid))),
                        TransactionType = "Machine Transfer",
                        TransactionNo = item.MachineryAndVehicleTransferId.ToString(),
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
            else if (item.Type == TransferType.StoreToStore)
            {

                #region Update asset info

                if (asset != null)
                {
                    asset.StoreCode= item.ToStoreCode;
                    asset.BranchId = InventoryHelper.GetStoreBranch(item.ToStoreCode);
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
                        StoreCode = item.ToStoreCode,
                        LocationType = LocationType.Store,
                        StartDate = DateTime.Today,
                        BranchId = InventoryHelper.GetStoreBranch(item.ToStoreCode),
                        TransactionType = "Machine Transfer",
                        TransactionNo = item.MachineryAndVehicleTransferId.ToString(),
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
