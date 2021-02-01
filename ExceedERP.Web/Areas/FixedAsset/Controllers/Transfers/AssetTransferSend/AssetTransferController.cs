using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.AssetTransferSend
{


    public class AssetTransferController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
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
                  Name = i.MainCompanyStructure.Name,
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
                  Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                  Code = i.EmployeeId
              }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Branches"] = branches;
            ViewData["SubCategory"] = assetSubCategories;
            ViewData["Category"] = assetCategories;
            ViewData["FixedAsset"] = selectedAsset;
            ViewData["stores"] = stores;
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult AssetTransfers_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<AssetTransfer> assetTransfers = _db.AssetTransfers;//.Where(x => userStores.Contains(x.CustodianBeforeTransfer));
            
            DataSourceResult result = assetTransfers.ToDataSourceResult(request, assetTransfer => new
            {
                AssetTransferId = assetTransfer.AssetTransferId,
                RequestNumber = assetTransfer.RequestNumber,
                TransferDate = assetTransfer.TransferDate,
                TransferDesc = assetTransfer.TransferDesc,

                Type = assetTransfer.Type,

                CustodianBeforeTransfer = assetTransfer.CustodianBeforeTransfer,
                TransferTo = assetTransfer.TransferTo,

                FromStoreCode = assetTransfer.FromStoreCode,
                ToStoreCode = assetTransfer.ToStoreCode,

                FromBranchId = assetTransfer.FromBranchId,
                ToBranchId = assetTransfer.ToBranchId,

                DriverType = assetTransfer.DriverType,
                SupplierName = assetTransfer.SupplierName,
                PlateNo = assetTransfer.PlateNo,
                DeliveredBy = assetTransfer.DeliveredBy,

                RecipientType = assetTransfer.RecipientType,
                RecipientEmployee = assetTransfer.RecipientEmployee,
                RecipientSupplier = assetTransfer.RecipientSupplier,

                Status = assetTransfer.Status,
                Remark = assetTransfer.Remark,
                IsVoid = assetTransfer.IsVoid,
                VoidBy = assetTransfer.VoidBy,
                VoidTime = assetTransfer.VoidTime,
                IsOnlineApproved = assetTransfer.IsOnlineApproved,
                OnlineApprovedBy = assetTransfer.OnlineApprovedBy,
                OnlineApprovedTime = assetTransfer.OnlineApprovedTime,
                IsSendForApproval = assetTransfer.IsSendForApproval,
                SendForApprovalBy = assetTransfer.SendForApprovalBy,
                SendForApprovalTime = assetTransfer.SendForApprovalTime,
                IsOnlineTransferred = assetTransfer.IsOnlineTransferred,
                IsOnlineTransferredBy = assetTransfer.IsOnlineTransferredBy,
                OnlineTransferredTime = assetTransfer.OnlineTransferredTime,

                DateCreated = assetTransfer.DateCreated,
                CreatedBy = assetTransfer.CreatedBy,
                LastModified = assetTransfer.LastModified,
                ModifiedBy = assetTransfer.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransfers_Create([DataSourceRequest]DataSourceRequest request, AssetTransfer assetTransfer)
        {
            #region validation

            var fixedAssetRequisition = _db.FixedAssetRequisitions.FirstOrDefault(sr => sr.FixedAssetRequisitionId.ToString() == assetTransfer.RequestNumber);
            if (fixedAssetRequisition != null && fixedAssetRequisition.FixedAssetRequisitionItem.Count == 0)
                ModelState.AddModelError("requestedItem", "The requisition does not have line items.");

            if (assetTransfer.Type == TransferType.PersonToPerson)
            {
                if (assetTransfer.CustodianBeforeTransfer==null ||assetTransfer.TransferTo==null)
                    ModelState.AddModelError("employee", "Please choose custodian before and receiver.");
            }

            if (assetTransfer.Type == TransferType.StoreToStore)
            {
                if (string.IsNullOrEmpty(assetTransfer.FromStoreCode)  || string.IsNullOrEmpty(assetTransfer.ToStoreCode))
                    ModelState.AddModelError("employee", "Please choose from store and to store.");
                else
                {
                    assetTransfer.FromBranchId = InventoryHelper.GetStoreBranch(assetTransfer.FromStoreCode);
                    assetTransfer.ToBranchId = InventoryHelper.GetStoreBranch(assetTransfer.ToStoreCode);
                }
            }

            #endregion


            if (ModelState.IsValid)
            {
                var entity = new AssetTransfer
                {
                    RequestNumber = assetTransfer.RequestNumber,
                    TransferDate = assetTransfer.TransferDate,
                    TransferDesc = assetTransfer.TransferDesc,
                    Type = assetTransfer.Type,

                    CustodianBeforeTransfer = assetTransfer.CustodianBeforeTransfer,
                    TransferTo = assetTransfer.TransferTo,

                    FromStoreCode = assetTransfer.FromStoreCode,
                    ToStoreCode = assetTransfer.ToStoreCode,

                    FromBranchId = assetTransfer.FromBranchId,
                    ToBranchId = assetTransfer.ToBranchId,

                    DriverType = assetTransfer.DriverType,
                    SupplierName = assetTransfer.SupplierName,
                    PlateNo = assetTransfer.PlateNo,
                    DeliveredBy = assetTransfer.DeliveredBy,

                    RecipientType = assetTransfer.RecipientType,
                    RecipientEmployee = assetTransfer.RecipientEmployee,
                    RecipientSupplier = assetTransfer.RecipientSupplier,

                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = assetTransfer.Remark,


                    IsVoid = assetTransfer.IsVoid,
                    VoidBy = assetTransfer.VoidBy,
                    VoidTime = assetTransfer.VoidTime,
                    IsOnlineApproved = assetTransfer.IsOnlineApproved,
                    OnlineApprovedBy = assetTransfer.OnlineApprovedBy,
                    OnlineApprovedTime = assetTransfer.OnlineApprovedTime,
                    IsOnlineTransferred = assetTransfer.IsOnlineTransferred,
                    IsOnlineTransferredBy = assetTransfer.IsOnlineTransferredBy,
                    OnlineTransferredTime = assetTransfer.OnlineTransferredTime,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = assetTransfer.LastModified,
                    ModifiedBy = assetTransfer.ModifiedBy,
                };

                _db.AssetTransfers.Add(entity);
                _db.SaveChanges();
                assetTransfer.AssetTransferId = entity.AssetTransferId;
                GetStoreRequisitionItems(entity);
            }

            return Json(new[] { assetTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransfers_Update([DataSourceRequest]DataSourceRequest request, AssetTransfer assetTransfer)
        {
            #region validation

            var fixedAssetRequisition = _db.FixedAssetRequisitions.FirstOrDefault(sr => sr.FixedAssetRequisitionId.ToString() == assetTransfer.RequestNumber);
            if (fixedAssetRequisition != null && fixedAssetRequisition.FixedAssetRequisitionItem.Count == 0)
                ModelState.AddModelError("requestedItem", "The requisition does not have line items.");

            if (assetTransfer.Type == TransferType.PersonToPerson)
            {
                if (assetTransfer.CustodianBeforeTransfer == null || assetTransfer.TransferTo == null)
                    ModelState.AddModelError("employee", "Please choose custodian before and receiver.");
            }

            if (assetTransfer.Type == TransferType.StoreToStore)
            {
                if (string.IsNullOrEmpty(assetTransfer.FromStoreCode) || string.IsNullOrEmpty(assetTransfer.ToStoreCode))
                    ModelState.AddModelError("employee", "Please choose from store and to store.");
                else
                {
                    assetTransfer.FromBranchId = InventoryHelper.GetStoreBranch(assetTransfer.FromStoreCode);
                    assetTransfer.ToBranchId = InventoryHelper.GetStoreBranch(assetTransfer.ToStoreCode);
                }
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new AssetTransfer
                {
                    AssetTransferId = assetTransfer.AssetTransferId,
                    RequestNumber = assetTransfer.RequestNumber,
                    TransferDate = assetTransfer.TransferDate,
                    TransferDesc = assetTransfer.TransferDesc,

                    Type = assetTransfer.Type,

                    CustodianBeforeTransfer = assetTransfer.CustodianBeforeTransfer,
                    TransferTo = assetTransfer.TransferTo,

                    FromStoreCode = assetTransfer.FromStoreCode,
                    ToStoreCode = assetTransfer.ToStoreCode,

                    FromBranchId = assetTransfer.FromBranchId,
                    ToBranchId = assetTransfer.ToBranchId,

                    DriverType = assetTransfer.DriverType,
                    SupplierName = assetTransfer.SupplierName,
                    PlateNo = assetTransfer.PlateNo,
                    DeliveredBy = assetTransfer.DeliveredBy,

                    RecipientType = assetTransfer.RecipientType,
                    RecipientEmployee = assetTransfer.RecipientEmployee,
                    RecipientSupplier = assetTransfer.RecipientSupplier,

                    Status = assetTransfer.Status,
                    Remark = assetTransfer.Remark,


                    IsVoid = assetTransfer.IsVoid,
                    VoidBy = assetTransfer.VoidBy,
                    VoidTime = assetTransfer.VoidTime,
                    IsOnlineApproved = assetTransfer.IsOnlineApproved,
                    OnlineApprovedBy = assetTransfer.OnlineApprovedBy,
                    OnlineApprovedTime = assetTransfer.OnlineApprovedTime,
                    IsOnlineTransferred = assetTransfer.IsOnlineTransferred,
                    IsOnlineTransferredBy = assetTransfer.IsOnlineTransferredBy,
                    OnlineTransferredTime = assetTransfer.OnlineTransferredTime,

                    DateCreated = assetTransfer.DateCreated,
                    CreatedBy = assetTransfer.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.AssetTransfers.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { assetTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransfers_Destroy([DataSourceRequest]DataSourceRequest request, AssetTransfer assetTransfer)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetTransfer
                {
                    RequestNumber = assetTransfer.RequestNumber,
                    TransferDate = assetTransfer.TransferDate,
                    TransferDesc = assetTransfer.TransferDesc,
                    AssetTransferId = assetTransfer.AssetTransferId,
                    Type = assetTransfer.Type,

                    CustodianBeforeTransfer = assetTransfer.CustodianBeforeTransfer,
                    TransferTo = assetTransfer.TransferTo,

                    FromStoreCode = assetTransfer.FromStoreCode,
                    ToStoreCode = assetTransfer.ToStoreCode,

                    FromBranchId = assetTransfer.FromBranchId,
                    ToBranchId = assetTransfer.ToBranchId,
                    Status = assetTransfer.Status,
                    Remark = assetTransfer.Remark,
                    IsVoid = assetTransfer.IsVoid,
                    VoidBy = assetTransfer.VoidBy,
                    VoidTime = assetTransfer.VoidTime,
                    IsOnlineApproved = assetTransfer.IsOnlineApproved,
                    OnlineApprovedBy = assetTransfer.OnlineApprovedBy,
                    OnlineApprovedTime = assetTransfer.OnlineApprovedTime,
                    IsOnlineTransferred = assetTransfer.IsOnlineTransferred,
                    IsOnlineTransferredBy = assetTransfer.IsOnlineTransferredBy,
                    OnlineTransferredTime = assetTransfer.OnlineTransferredTime,

                    DateCreated = assetTransfer.DateCreated,
                    CreatedBy = assetTransfer.CreatedBy,
                    LastModified = assetTransfer.LastModified,
                    ModifiedBy = assetTransfer.ModifiedBy,
                };

                _db.AssetTransfers.Attach(entity);
                _db.AssetTransfers.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { assetTransfer }.ToDataSourceResult(request, ModelState));
        }


        public FileResult DownloadTemplate()
        {

            string physicalPath = "~/Content/InventoryTemplate/AssetTransferTemplate.xlsx";
            return File(physicalPath, "application/force-download", Path.GetFileName(physicalPath));
        }

       

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.AssetTransfers.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.AssetTransferItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.AssetTransfers.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.AssetTransferItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
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
            var record = _db.AssetTransfers.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.AssetTransferItems.Count > 0)
            {
                //reverse transaction here


                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.AssetTransfers.Attach(record);
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

            if (record.AssetTransferItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
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
            var assetTransfer = _db.AssetTransfers.Find(id);
            if (assetTransfer != null)
            {
                #region header
                var entity = new AssetTransfer
                {
                    RequestNumber = assetTransfer.RequestNumber,
                    TransferDate = assetTransfer.TransferDate,
                    TransferDesc = assetTransfer.TransferDesc,
                    Type = assetTransfer.Type,

                    CustodianBeforeTransfer = assetTransfer.CustodianBeforeTransfer,
                    TransferTo = assetTransfer.TransferTo,

                    FromStoreCode = assetTransfer.FromStoreCode,
                    ToStoreCode = assetTransfer.ToStoreCode,

                    FromBranchId = assetTransfer.FromBranchId,
                    ToBranchId = assetTransfer.ToBranchId,

                    DriverType = assetTransfer.DriverType,
                    SupplierName = assetTransfer.SupplierName,
                    PlateNo = assetTransfer.PlateNo,
                    DeliveredBy = assetTransfer.DeliveredBy,

                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = assetTransfer.Remark,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = assetTransfer.OrgBranchName,

                };

                _db.AssetTransfers.Add(entity);
                _db.SaveChanges();
                #endregion
                foreach (var assetTransferItem in assetTransfer.AssetTransferItems)
                {
                    #region lines
                    var line = new AssetTransferItem
                    {
                        AssetTransferId = entity.AssetTransferId,
                        FixedAssetCategoryId = assetTransferItem.FixedAssetCategoryId,
                        FixedAssetSubCategoryId = assetTransferItem.FixedAssetSubCategoryId,
                        AssetRegistrationId = assetTransferItem.AssetRegistrationId,
                        ItemName = assetTransferItem.ItemName,
                        Unit = assetTransferItem.Unit,
                        UnitPrice = assetTransferItem.UnitPrice,
                        Total = assetTransferItem.Total,
                        Name = assetTransferItem.Name,
                        Size = assetTransferItem.Size,
                        GrnNo = assetTransferItem.GrnNo,
                        ModelNo = assetTransferItem.ModelNo,
                        SerialNo = assetTransferItem.SerialNo,
                        TagNo = assetTransferItem.TagNo,
                        Quantity = assetTransferItem.Quantity,
                    };

                    _db.AssetTransferItems.Add(line);
                    _db.SaveChanges();

                    #endregion

                }

            }

        }


        private void GetStoreRequisitionItems(AssetTransfer assetTransfer)
        {
            var storeRequisitionItem = _db.FixedAssetRequisitionItems.Where(sr => sr.FixedAssetRequisitionId.ToString() == assetTransfer.RequestNumber).ToList();
           foreach (var requestedItem in storeRequisitionItem)
            {
                #region fixed asset
                for (int i = 0; i < requestedItem.RequestedQuantity; i++)
                {
                    var assetTransferItem = new AssetTransferItem
                    {
                        AssetTransferId = assetTransfer.AssetTransferId,
                        FixedAssetCategoryId = requestedItem.FixedAssetCategoryID,
                        FixedAssetSubCategoryId = requestedItem.FixedAssetSubCategoryID,
                        
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Today
                    };

                    _db.AssetTransferItems.Add(assetTransferItem);
                    _db.SaveChanges();
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
