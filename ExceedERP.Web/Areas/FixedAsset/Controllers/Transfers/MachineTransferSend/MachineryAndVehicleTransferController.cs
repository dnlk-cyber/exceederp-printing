using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.MachineTransferSend
{
    public class MachineryAndVehicleTransferController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var stores = _db.InventoryStores;
            var assetCategories = _db.FixedAssetCategories;
            var assetSubCategories = _db.FixedAssetSubCategories;
            var fixedAsset = _db.AssetRegistrations;
            var branches = _db.Branch;
            var costCenter = _db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.Branch.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var selectedAsset = fixedAsset.Select(
              i => new
              {
                  Name = i.Tag + "-" + i.Name,
                  AssetRegistrationId = i.AssetRegistrationId
              }).AsQueryable();

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
              i => new
              {
                  Name = i.FirstName + " " + i.MiddleName + " " + i.LastName,
                  Code = i.EmployeeId
              }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Branches"] = branches;
            ViewData["SubCategory"] = assetSubCategories;
            ViewData["Category"] = assetCategories;
            ViewData["FixedAsset"] = fixedAsset;
            ViewData["stores"] = stores;
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult MachineryAndVehicleTransfers_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MachineryAndVehicleTransfer> machineryandvehicletransfers = _db.MachineryAndVehicleTransfers;
            DataSourceResult result = machineryandvehicletransfers.ToDataSourceResult(request, machineryAndVehicleTransfer => new {
                MachineryAndVehicleTransferId = machineryAndVehicleTransfer.MachineryAndVehicleTransferId,
                RequestNumber = machineryAndVehicleTransfer.RequestNumber,
                FixedAssetCategoryID = machineryAndVehicleTransfer.FixedAssetCategoryID,
                FixedAssetSubCategoryID = machineryAndVehicleTransfer.FixedAssetSubCategoryID,
                AssetRegistrationId = machineryAndVehicleTransfer.AssetRegistrationId,

                FromStoreCode = machineryAndVehicleTransfer.FromStoreCode,
                ToStoreCode = machineryAndVehicleTransfer.ToStoreCode,

                FromBranchId = machineryAndVehicleTransfer.FromBranchId,
                ToBranchId = machineryAndVehicleTransfer.ToBranchId,

                TransferDate = machineryAndVehicleTransfer.TransferDate,
                UsageDays = machineryAndVehicleTransfer.UsageDays,
                TransferDesc = machineryAndVehicleTransfer.TransferDesc,
                CustodianBeforeTransfer = machineryAndVehicleTransfer.CustodianBeforeTransfer,
                LocationBeforeTransfer = machineryAndVehicleTransfer.LocationBeforeTransfer,
                TransferTo = machineryAndVehicleTransfer.TransferTo,
                TransferReason = machineryAndVehicleTransfer.TransferReason,
                TransferBy = machineryAndVehicleTransfer.TransferBy,
                DriverName = machineryAndVehicleTransfer.DriverName,
                TransferPlateNo = machineryAndVehicleTransfer.TransferPlateNo,
                Manufacturer = machineryAndVehicleTransfer.Manufacturer,
                ModelNo = machineryAndVehicleTransfer.ModelNo,
                ChassisNo = machineryAndVehicleTransfer.ChassisNo,
                SerialNo = machineryAndVehicleTransfer.SerialNo,
                PlateNo = machineryAndVehicleTransfer.PlateNo,
                EngineNo = machineryAndVehicleTransfer.EngineNo,
                TagNo = machineryAndVehicleTransfer.TagNo,
                BarCodeNo = machineryAndVehicleTransfer.BarCodeNo,
                KmHourReading = machineryAndVehicleTransfer.KmHourReading,
                UPrice = machineryAndVehicleTransfer.UPrice,
                Unit = machineryAndVehicleTransfer.Unit,

                FuelVolume = machineryAndVehicleTransfer.FuelVolume,
                HydraulicOilVolume = machineryAndVehicleTransfer.HydraulicOilVolume,
                BatteryAmpere = machineryAndVehicleTransfer.BatteryAmpere,
                BatteryStatus = machineryAndVehicleTransfer.BatteryStatus,
                BatteryQuantity = machineryAndVehicleTransfer.BatteryQuantity,
                Status = machineryAndVehicleTransfer.Status,
                Remark = machineryAndVehicleTransfer.Remark,
                IsVoid = machineryAndVehicleTransfer.IsVoid,
                VoidBy = machineryAndVehicleTransfer.VoidBy,
                VoidTime = machineryAndVehicleTransfer.VoidTime,
                IsOnlineApproved = machineryAndVehicleTransfer.IsOnlineApproved,
                OnlineApprovedBy = machineryAndVehicleTransfer.OnlineApprovedBy,
                OnlineApprovedTime = machineryAndVehicleTransfer.OnlineApprovedTime,
                IsOnlineTransferred = machineryAndVehicleTransfer.IsOnlineTransferred,
                IsOnlineTransferredBy = machineryAndVehicleTransfer.IsOnlineTransferredBy,
                OnlineTransferredTime = machineryAndVehicleTransfer.OnlineTransferredTime,
                IsSendForApproval = machineryAndVehicleTransfer.IsSendForApproval,
                OrgBranchName = machineryAndVehicleTransfer.OrgBranchName,
                SendForApprovalBy = machineryAndVehicleTransfer.SendForApprovalBy,
                SendForApprovalTime = machineryAndVehicleTransfer.SendForApprovalTime,
                DateCreated = machineryAndVehicleTransfer.DateCreated,
                LastModified = machineryAndVehicleTransfer.LastModified,
                CreatedBy = machineryAndVehicleTransfer.CreatedBy,
                ModifiedBy = machineryAndVehicleTransfer.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransfers_Create([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransfer machineryAndVehicleTransfer)
        {

            #region validation
            var asset = _db.AssetRegistrations.Find(machineryAndVehicleTransfer.AssetRegistrationId);
            if (asset == null)
            {
                ModelState.AddModelError("register", "Please select an asset.");
            }

            var fixedAssetRequisition = _db.FixedAssetRequisitions.FirstOrDefault(sr => sr.FixedAssetRequisitionId.ToString() == machineryAndVehicleTransfer.RequestNumber);
            if (fixedAssetRequisition != null && fixedAssetRequisition.FixedAssetRequisitionItem.Count == 0)
                ModelState.AddModelError("requestedItem", "The requisition does not have line items.");

            if (machineryAndVehicleTransfer.Type == TransferType.PersonToPerson)
            {
                if (machineryAndVehicleTransfer.CustodianBeforeTransfer == null || machineryAndVehicleTransfer.TransferTo == null)
                    ModelState.AddModelError("employee", "Please choose custodian before and receiver.");
            }

            if (machineryAndVehicleTransfer.Type == TransferType.StoreToStore)
            {
                if (string.IsNullOrEmpty(machineryAndVehicleTransfer.FromStoreCode) || string.IsNullOrEmpty(machineryAndVehicleTransfer.ToStoreCode))
                    ModelState.AddModelError("employee", "Please choose from store and to store.");
                else
                {
                    machineryAndVehicleTransfer.FromBranchId = InventoryHelper.GetStoreBranch(machineryAndVehicleTransfer.FromStoreCode);
                    machineryAndVehicleTransfer.ToBranchId = InventoryHelper.GetStoreBranch(machineryAndVehicleTransfer.ToStoreCode);
                }
            }

            #endregion
            if (ModelState.IsValid)
            {
                if (asset != null)
                {
                    var entity = new MachineryAndVehicleTransfer
                    {
                        RequestNumber = machineryAndVehicleTransfer.RequestNumber,
                        FixedAssetCategoryID = machineryAndVehicleTransfer.FixedAssetCategoryID,
                        FixedAssetSubCategoryID = machineryAndVehicleTransfer.FixedAssetSubCategoryID,
                        AssetRegistrationId = machineryAndVehicleTransfer.AssetRegistrationId,

                        FromStoreCode = machineryAndVehicleTransfer.FromStoreCode,
                        ToStoreCode = machineryAndVehicleTransfer.ToStoreCode,

                        FromBranchId = machineryAndVehicleTransfer.FromBranchId,
                        ToBranchId = machineryAndVehicleTransfer.ToBranchId,

                        TransferDate = machineryAndVehicleTransfer.TransferDate,
                        UsageDays = machineryAndVehicleTransfer.UsageDays,
                        TransferDesc = machineryAndVehicleTransfer.TransferDesc,
                        CustodianBeforeTransfer = machineryAndVehicleTransfer.CustodianBeforeTransfer,
                        LocationBeforeTransfer = machineryAndVehicleTransfer.LocationBeforeTransfer,
                        TransferTo = machineryAndVehicleTransfer.TransferTo,
                        TransferReason = machineryAndVehicleTransfer.TransferReason,
                        TransferBy = machineryAndVehicleTransfer.TransferBy,
                        DriverName = machineryAndVehicleTransfer.DriverName,
                        TransferPlateNo = machineryAndVehicleTransfer.TransferPlateNo,

                        Manufacturer = asset.Manufacturer,
                        ModelNo = asset.ModelNo,
                        ChassisNo = asset.ChassisNo,
                        SerialNo = asset.SerialNo,
                        PlateNo = asset.PlateNo,
                        EngineNo = asset.EngineNo,
                        TagNo = asset.Tag,
                        BarCodeNo = asset.BarCodeNo,
                        UPrice = asset.BookValue,
                        Unit = asset.Unit,

                        KmHourReading = machineryAndVehicleTransfer.KmHourReading,

                        FuelVolume = machineryAndVehicleTransfer.FuelVolume,
                        HydraulicOilVolume = machineryAndVehicleTransfer.HydraulicOilVolume,
                        BatteryAmpere = machineryAndVehicleTransfer.BatteryAmpere,
                        BatteryStatus = machineryAndVehicleTransfer.BatteryStatus,
                        BatteryQuantity = machineryAndVehicleTransfer.BatteryQuantity,
                        Status = machineryAndVehicleTransfer.Status,
                        Remark = machineryAndVehicleTransfer.Remark,
                        IsVoid = machineryAndVehicleTransfer.IsVoid,
                        VoidBy = machineryAndVehicleTransfer.VoidBy,
                        VoidTime = machineryAndVehicleTransfer.VoidTime,
                        IsOnlineApproved = machineryAndVehicleTransfer.IsOnlineApproved,
                        OnlineApprovedBy = machineryAndVehicleTransfer.OnlineApprovedBy,
                        OnlineApprovedTime = machineryAndVehicleTransfer.OnlineApprovedTime,
                        IsOnlineTransferred = machineryAndVehicleTransfer.IsOnlineTransferred,
                        IsOnlineTransferredBy = machineryAndVehicleTransfer.IsOnlineTransferredBy,
                        OnlineTransferredTime = machineryAndVehicleTransfer.OnlineTransferredTime,
                        IsSendForApproval = machineryAndVehicleTransfer.IsSendForApproval,
                        OrgBranchName = machineryAndVehicleTransfer.OrgBranchName,
                        SendForApprovalBy = machineryAndVehicleTransfer.SendForApprovalBy,
                        SendForApprovalTime = machineryAndVehicleTransfer.SendForApprovalTime,
                        DateCreated = machineryAndVehicleTransfer.DateCreated,
                        LastModified = machineryAndVehicleTransfer.LastModified,
                        CreatedBy = machineryAndVehicleTransfer.CreatedBy,
                        ModifiedBy = machineryAndVehicleTransfer.ModifiedBy
                    };

                    _db.MachineryAndVehicleTransfers.Add(entity);
                    _db.SaveChanges();
                    machineryAndVehicleTransfer.MachineryAndVehicleTransferId = entity.MachineryAndVehicleTransferId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "MachineryAndVehicleTransfer", "Success", User.Identity.Name, $"created MachineryAndVehicleTransfer {entity.MachineryAndVehicleTransferId} ChasisNo {entity.ChassisNo}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { machineryAndVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransfers_Update([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransfer machineryAndVehicleTransfer)
        {
            #region validation
            var asset = _db.AssetRegistrations.Find(machineryAndVehicleTransfer.AssetRegistrationId);
            if (asset == null)
            {
                ModelState.AddModelError("register", "Please select an asset.");
            }

            var fixedAssetRequisition = _db.FixedAssetRequisitions.FirstOrDefault(sr => sr.FixedAssetRequisitionId.ToString() == machineryAndVehicleTransfer.RequestNumber);
            if (fixedAssetRequisition != null && fixedAssetRequisition.FixedAssetRequisitionItem.Count == 0)
                ModelState.AddModelError("requestedItem", "The requisition does not have line items.");

            if (machineryAndVehicleTransfer.Type == TransferType.PersonToPerson)
            {
                if (machineryAndVehicleTransfer.CustodianBeforeTransfer == null || machineryAndVehicleTransfer.TransferTo == null)
                    ModelState.AddModelError("employee", "Please choose custodian before and receiver.");
            }

            if (machineryAndVehicleTransfer.Type == TransferType.StoreToStore)
            {
                if (string.IsNullOrEmpty(machineryAndVehicleTransfer.FromStoreCode) || string.IsNullOrEmpty(machineryAndVehicleTransfer.ToStoreCode))
                    ModelState.AddModelError("employee", "Please choose from store and to store.");
                else
                {
                    machineryAndVehicleTransfer.FromBranchId = InventoryHelper.GetStoreBranch(machineryAndVehicleTransfer.FromStoreCode);
                    machineryAndVehicleTransfer.ToBranchId = InventoryHelper.GetStoreBranch(machineryAndVehicleTransfer.ToStoreCode);
                }
            }

            #endregion
            if (ModelState.IsValid)
            {
                if (asset != null)
                {
                    var entity = new MachineryAndVehicleTransfer
                    {
                        MachineryAndVehicleTransferId = machineryAndVehicleTransfer.MachineryAndVehicleTransferId,
                        FixedAssetCategoryID = machineryAndVehicleTransfer.FixedAssetCategoryID,
                        FixedAssetSubCategoryID = machineryAndVehicleTransfer.FixedAssetSubCategoryID,
                        AssetRegistrationId = machineryAndVehicleTransfer.AssetRegistrationId,

                        FromStoreCode = machineryAndVehicleTransfer.FromStoreCode,
                        ToStoreCode = machineryAndVehicleTransfer.ToStoreCode,

                        FromBranchId = machineryAndVehicleTransfer.FromBranchId,
                        ToBranchId = machineryAndVehicleTransfer.ToBranchId,

                        RequestNumber = machineryAndVehicleTransfer.RequestNumber,
                        TransferDate = machineryAndVehicleTransfer.TransferDate,
                        UsageDays = machineryAndVehicleTransfer.UsageDays,
                        TransferDesc = machineryAndVehicleTransfer.TransferDesc,
                        CustodianBeforeTransfer = machineryAndVehicleTransfer.CustodianBeforeTransfer,
                        LocationBeforeTransfer = machineryAndVehicleTransfer.LocationBeforeTransfer,
                        TransferTo = machineryAndVehicleTransfer.TransferTo,
                        TransferReason = machineryAndVehicleTransfer.TransferReason,
                        TransferBy = machineryAndVehicleTransfer.TransferBy,
                        DriverName = machineryAndVehicleTransfer.DriverName,
                        TransferPlateNo = machineryAndVehicleTransfer.TransferPlateNo,

                        Manufacturer = asset.Manufacturer,
                        ModelNo = asset.ModelNo,
                        ChassisNo = asset.ChassisNo,
                        SerialNo = asset.SerialNo,
                        PlateNo = asset.PlateNo,
                        EngineNo = asset.EngineNo,
                        TagNo = asset.Tag,
                        BarCodeNo = asset.BarCodeNo,
                        UPrice = asset.BookValue,
                        Unit = asset.Unit,

                        KmHourReading = machineryAndVehicleTransfer.KmHourReading,
                       

                        FuelVolume = machineryAndVehicleTransfer.FuelVolume,
                        HydraulicOilVolume = machineryAndVehicleTransfer.HydraulicOilVolume,
                        BatteryAmpere = machineryAndVehicleTransfer.BatteryAmpere,
                        BatteryStatus = machineryAndVehicleTransfer.BatteryStatus,
                        BatteryQuantity = machineryAndVehicleTransfer.BatteryQuantity,
                        Status = machineryAndVehicleTransfer.Status,
                        Remark = machineryAndVehicleTransfer.Remark,
                        IsVoid = machineryAndVehicleTransfer.IsVoid,
                        VoidBy = machineryAndVehicleTransfer.VoidBy,
                        VoidTime = machineryAndVehicleTransfer.VoidTime,
                        IsOnlineApproved = machineryAndVehicleTransfer.IsOnlineApproved,
                        OnlineApprovedBy = machineryAndVehicleTransfer.OnlineApprovedBy,
                        OnlineApprovedTime = machineryAndVehicleTransfer.OnlineApprovedTime,
                        IsOnlineTransferred = machineryAndVehicleTransfer.IsOnlineTransferred,
                        IsOnlineTransferredBy = machineryAndVehicleTransfer.IsOnlineTransferredBy,
                        OnlineTransferredTime = machineryAndVehicleTransfer.OnlineTransferredTime,
                        IsSendForApproval = machineryAndVehicleTransfer.IsSendForApproval,
                        OrgBranchName = machineryAndVehicleTransfer.OrgBranchName,
                        SendForApprovalBy = machineryAndVehicleTransfer.SendForApprovalBy,
                        SendForApprovalTime = machineryAndVehicleTransfer.SendForApprovalTime,
                        DateCreated = machineryAndVehicleTransfer.DateCreated,
                        LastModified = machineryAndVehicleTransfer.LastModified,
                        CreatedBy = machineryAndVehicleTransfer.CreatedBy,
                        ModifiedBy = machineryAndVehicleTransfer.ModifiedBy
                    };

                    _db.MachineryAndVehicleTransfers.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "MachineryAndVehicleTransfer", "Success", User.Identity.Name, $"update MachineryAndVehicleTransfer {entity.MachineryAndVehicleTransferId} ChasisNo {entity.ChassisNo}", Modules.FixedAsset);
                    #endregion
                }
                _db.SaveChanges();
            }

            return Json(new[] { machineryAndVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransfers_Destroy([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransfer machineryAndVehicleTransfer)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachineryAndVehicleTransfer
                {
                    MachineryAndVehicleTransferId = machineryAndVehicleTransfer.MachineryAndVehicleTransferId,
                    FixedAssetCategoryID = machineryAndVehicleTransfer.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = machineryAndVehicleTransfer.FixedAssetSubCategoryID,
                    AssetRegistrationId = machineryAndVehicleTransfer.AssetRegistrationId,

                    FromStoreCode = machineryAndVehicleTransfer.FromStoreCode,
                    ToStoreCode = machineryAndVehicleTransfer.ToStoreCode,

                    FromBranchId = machineryAndVehicleTransfer.FromBranchId,
                    ToBranchId = machineryAndVehicleTransfer.ToBranchId,

                    RequestNumber = machineryAndVehicleTransfer.RequestNumber,
                    TransferDate = machineryAndVehicleTransfer.TransferDate,
                    UsageDays = machineryAndVehicleTransfer.UsageDays,
                    TransferDesc = machineryAndVehicleTransfer.TransferDesc,
                    CustodianBeforeTransfer = machineryAndVehicleTransfer.CustodianBeforeTransfer,
                    LocationBeforeTransfer = machineryAndVehicleTransfer.LocationBeforeTransfer,
                    TransferTo = machineryAndVehicleTransfer.TransferTo,
                    TransferReason = machineryAndVehicleTransfer.TransferReason,
                    TransferBy = machineryAndVehicleTransfer.TransferBy,
                    DriverName = machineryAndVehicleTransfer.DriverName,
                    TransferPlateNo = machineryAndVehicleTransfer.TransferPlateNo,
                    Manufacturer = machineryAndVehicleTransfer.Manufacturer,
                    ModelNo = machineryAndVehicleTransfer.ModelNo,
                    ChassisNo = machineryAndVehicleTransfer.ChassisNo,
                    SerialNo = machineryAndVehicleTransfer.SerialNo,
                    PlateNo = machineryAndVehicleTransfer.PlateNo,
                    EngineNo = machineryAndVehicleTransfer.EngineNo,
                    TagNo = machineryAndVehicleTransfer.TagNo,
                    BarCodeNo = machineryAndVehicleTransfer.BarCodeNo,
                    KmHourReading = machineryAndVehicleTransfer.KmHourReading,
                    FuelVolume = machineryAndVehicleTransfer.FuelVolume,
                    HydraulicOilVolume = machineryAndVehicleTransfer.HydraulicOilVolume,
                    BatteryAmpere = machineryAndVehicleTransfer.BatteryAmpere,
                    BatteryStatus = machineryAndVehicleTransfer.BatteryStatus,
                    BatteryQuantity = machineryAndVehicleTransfer.BatteryQuantity,
                    UPrice = machineryAndVehicleTransfer.UPrice,
                    Unit = machineryAndVehicleTransfer.Unit,

                    Status = machineryAndVehicleTransfer.Status,
                    Remark = machineryAndVehicleTransfer.Remark,
                    IsVoid = machineryAndVehicleTransfer.IsVoid,
                    VoidBy = machineryAndVehicleTransfer.VoidBy,
                    VoidTime = machineryAndVehicleTransfer.VoidTime,
                    IsOnlineApproved = machineryAndVehicleTransfer.IsOnlineApproved,
                    OnlineApprovedBy = machineryAndVehicleTransfer.OnlineApprovedBy,
                    OnlineApprovedTime = machineryAndVehicleTransfer.OnlineApprovedTime,
                    IsOnlineTransferred = machineryAndVehicleTransfer.IsOnlineTransferred,
                    IsOnlineTransferredBy = machineryAndVehicleTransfer.IsOnlineTransferredBy,
                    OnlineTransferredTime = machineryAndVehicleTransfer.OnlineTransferredTime,
                    IsSendForApproval = machineryAndVehicleTransfer.IsSendForApproval,
                    OrgBranchName = machineryAndVehicleTransfer.OrgBranchName,
                    SendForApprovalBy = machineryAndVehicleTransfer.SendForApprovalBy,
                    SendForApprovalTime = machineryAndVehicleTransfer.SendForApprovalTime,
                    DateCreated = machineryAndVehicleTransfer.DateCreated,
                    LastModified = machineryAndVehicleTransfer.LastModified,
                    CreatedBy = machineryAndVehicleTransfer.CreatedBy,
                    ModifiedBy = machineryAndVehicleTransfer.ModifiedBy
                };

                _db.MachineryAndVehicleTransfers.Attach(entity);
                _db.MachineryAndVehicleTransfers.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete MachineryAndVehicleTransfer {machineryAndVehicleTransfer.MachineryAndVehicleTransferId} ChasisNo {machineryAndVehicleTransfer.ChassisNo}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { machineryAndVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.MachineryAndVehicleTransfers.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.MachineryAndVehicleTransfers.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        [AuthorizeRoles(FixedAssetConstants.FixedAssetVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.MachineryAndVehicleTransfers.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                //reverse transaction here


                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.MachineryAndVehicleTransfers.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                //copy
                CopyRecord(id);

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            
            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        private void CopyRecord(int id)
        {
            var machineryAndVehicleTransfer = _db.MachineryAndVehicleTransfers.Find(id);
            if (machineryAndVehicleTransfer != null)
            {
                #region header
                var entity = new MachineryAndVehicleTransfer
                {
                    RequestNumber = machineryAndVehicleTransfer.RequestNumber,
                    FixedAssetCategoryID = machineryAndVehicleTransfer.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = machineryAndVehicleTransfer.FixedAssetSubCategoryID,
                    AssetRegistrationId = machineryAndVehicleTransfer.AssetRegistrationId,

                    FromStoreCode = machineryAndVehicleTransfer.FromStoreCode,
                    ToStoreCode = machineryAndVehicleTransfer.ToStoreCode,

                    FromBranchId = machineryAndVehicleTransfer.FromBranchId,
                    ToBranchId = machineryAndVehicleTransfer.ToBranchId,

                    TransferDate = machineryAndVehicleTransfer.TransferDate,
                    UsageDays = machineryAndVehicleTransfer.UsageDays,
                    TransferDesc = machineryAndVehicleTransfer.TransferDesc,
                    CustodianBeforeTransfer = machineryAndVehicleTransfer.CustodianBeforeTransfer,
                    LocationBeforeTransfer = machineryAndVehicleTransfer.LocationBeforeTransfer,
                    TransferTo = machineryAndVehicleTransfer.TransferTo,
                    TransferReason = machineryAndVehicleTransfer.TransferReason,
                    TransferBy = machineryAndVehicleTransfer.TransferBy,
                    DriverName = machineryAndVehicleTransfer.DriverName,
                    TransferPlateNo = machineryAndVehicleTransfer.TransferPlateNo,

                    Manufacturer = machineryAndVehicleTransfer.Manufacturer,
                    ModelNo = machineryAndVehicleTransfer.ModelNo,
                    ChassisNo = machineryAndVehicleTransfer.ChassisNo,
                    SerialNo = machineryAndVehicleTransfer.SerialNo,
                    PlateNo = machineryAndVehicleTransfer.PlateNo,
                    EngineNo = machineryAndVehicleTransfer.EngineNo,
                    TagNo = machineryAndVehicleTransfer.TagNo,
                    BarCodeNo = machineryAndVehicleTransfer.BarCodeNo,

                    KmHourReading = machineryAndVehicleTransfer.KmHourReading,

                    FuelVolume = machineryAndVehicleTransfer.FuelVolume,
                    HydraulicOilVolume = machineryAndVehicleTransfer.HydraulicOilVolume,
                    BatteryAmpere = machineryAndVehicleTransfer.BatteryAmpere,
                    BatteryStatus = machineryAndVehicleTransfer.BatteryStatus,
                    BatteryQuantity = machineryAndVehicleTransfer.BatteryQuantity,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = machineryAndVehicleTransfer.OrgBranchName,

                };

                _db.MachineryAndVehicleTransfers.Add(entity);
                _db.SaveChanges();
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
