using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.AssetTransferReceive
{


    public class AssetTransferReceiveController : Controller
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
        public ActionResult AssetTransferReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<Core.Domain.FixedAsset.Transfers.AssetTransferReceive> assetTransferReceives = _db.AssetTransferReceives;//.Where(x => userStores.Contains(x.CustodianBeforeTransfer));
            
            DataSourceResult result = assetTransferReceives.ToDataSourceResult(request, assetTransferReceive => new
            {
                AssetTransferId = assetTransferReceive.AssetTransferId,
                AssetTransferReceiveId = assetTransferReceive.AssetTransferReceiveId,
                ReceiveDate = assetTransferReceive.ReceiveDate,

                AssetTransferConfirm = assetTransferReceive.AssetTransferConfirm,//added

                Status = assetTransferReceive.Status,
                Remark = assetTransferReceive.Remark,
                IsVoid = assetTransferReceive.IsVoid,
                VoidBy = assetTransferReceive.VoidBy,
                VoidTime = assetTransferReceive.VoidTime,
                IsOnlineApproved = assetTransferReceive.IsOnlineApproved,
                OnlineApprovedBy = assetTransferReceive.OnlineApprovedBy,
                OnlineApprovedTime = assetTransferReceive.OnlineApprovedTime,
                IsSendForApproval = assetTransferReceive.IsSendForApproval,
                SendForApprovalBy = assetTransferReceive.SendForApprovalBy,
                SendForApprovalTime = assetTransferReceive.SendForApprovalTime,
                IsOnlineTransferred = assetTransferReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = assetTransferReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = assetTransferReceive.OnlineTransferredTime,

                DateCreated = assetTransferReceive.DateCreated,
                CreatedBy = assetTransferReceive.CreatedBy,
                LastModified = assetTransferReceive.LastModified,
                ModifiedBy = assetTransferReceive.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferReceives_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.FixedAsset.Transfers.AssetTransferReceive assetTransferReceive)
        {
            #region validation

          

            #endregion

         
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.FixedAsset.Transfers.AssetTransferReceive
                {
                    AssetTransferId = assetTransferReceive.AssetTransferId,
                    ReceiveDate = assetTransferReceive.ReceiveDate,

                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = assetTransferReceive.Remark,

                    AssetTransferConfirm = assetTransferReceive.AssetTransferConfirm,//added

                    IsVoid = assetTransferReceive.IsVoid,
                    VoidBy = assetTransferReceive.VoidBy,
                    VoidTime = assetTransferReceive.VoidTime,
                    IsOnlineApproved = assetTransferReceive.IsOnlineApproved,
                    OnlineApprovedBy = assetTransferReceive.OnlineApprovedBy,
                    OnlineApprovedTime = assetTransferReceive.OnlineApprovedTime,
                    IsOnlineTransferred = assetTransferReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = assetTransferReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = assetTransferReceive.OnlineTransferredTime,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = assetTransferReceive.LastModified,
                    ModifiedBy = assetTransferReceive.ModifiedBy,
                };

                _db.AssetTransferReceives.Add(entity);
                _db.SaveChanges();
                assetTransferReceive.AssetTransferReceiveId = entity.AssetTransferReceiveId;
                GeTransferSendItems(entity);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AssetTransferReceive", "Success", User.Identity.Name, $"created AssetTransferReceive {entity.AssetTransferReceiveId} OrgBranchName {entity.OrgBranchName}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { assetTransferReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferReceives_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.FixedAsset.Transfers.AssetTransferReceive assetTransferReceive)
        {
            #region validation

            

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.FixedAsset.Transfers.AssetTransferReceive
                {
                    AssetTransferId = assetTransferReceive.AssetTransferId,
                    AssetTransferReceiveId = assetTransferReceive.AssetTransferReceiveId,
                    ReceiveDate = assetTransferReceive.ReceiveDate,

                    Status = assetTransferReceive.Status,
                    Remark = assetTransferReceive.Remark,

                    AssetTransferConfirm = assetTransferReceive.AssetTransferConfirm,//added

                    IsVoid = assetTransferReceive.IsVoid,
                    VoidBy = assetTransferReceive.VoidBy,
                    VoidTime = assetTransferReceive.VoidTime,
                    IsOnlineApproved = assetTransferReceive.IsOnlineApproved,
                    OnlineApprovedBy = assetTransferReceive.OnlineApprovedBy,
                    OnlineApprovedTime = assetTransferReceive.OnlineApprovedTime,
                    IsOnlineTransferred = assetTransferReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = assetTransferReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = assetTransferReceive.OnlineTransferredTime,

                    DateCreated = assetTransferReceive.DateCreated,
                    CreatedBy = assetTransferReceive.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.AssetTransferReceives.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AssetTransferReceive", "Success", User.Identity.Name, $"updated AssetTransferReceive {entity.AssetTransferReceiveId} OrgBranchName {entity.OrgBranchName}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { assetTransferReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetTransferReceives_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.FixedAsset.Transfers.AssetTransferReceive assetTransferReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.FixedAsset.Transfers.AssetTransferReceive
                {
                    AssetTransferId = assetTransferReceive.AssetTransferId,
                    AssetTransferReceiveId = assetTransferReceive.AssetTransferReceiveId,
                    ReceiveDate = assetTransferReceive.ReceiveDate,
                    Status = assetTransferReceive.Status,
                    Remark = assetTransferReceive.Remark,
                    IsVoid = assetTransferReceive.IsVoid,
                    VoidBy = assetTransferReceive.VoidBy,
                    VoidTime = assetTransferReceive.VoidTime,
                    IsOnlineApproved = assetTransferReceive.IsOnlineApproved,
                    OnlineApprovedBy = assetTransferReceive.OnlineApprovedBy,
                    OnlineApprovedTime = assetTransferReceive.OnlineApprovedTime,
                    IsOnlineTransferred = assetTransferReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = assetTransferReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = assetTransferReceive.OnlineTransferredTime,

                    DateCreated = assetTransferReceive.DateCreated,
                    CreatedBy = assetTransferReceive.CreatedBy,
                    LastModified = assetTransferReceive.LastModified,
                    ModifiedBy = assetTransferReceive.ModifiedBy,
                };

                _db.AssetTransferReceives.Attach(entity);
                _db.AssetTransferReceives.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete AssetTransferReceive {assetTransferReceive.AssetTransferReceiveId} OrgBranchName {assetTransferReceive.OrgBranchName}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { assetTransferReceive }.ToDataSourceResult(request, ModelState));
        }


        public FileResult DownloadTemplate()
        {

            string physicalPath = "~/Content/InventoryTemplate/AssetTransferReceiveTemplate.xlsx";
            return File(physicalPath, "application/force-download", Path.GetFileName(physicalPath));
        }

       

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.AssetTransferReceives.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.AssetTransferReceiveItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.AssetTransferReceives.Attach(record);
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

            else if (record.AssetTransferReceiveItems.Count == 0)
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
            var record = _db.AssetTransferReceives.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.AssetTransferReceiveItems.Count > 0)
            {
                //reverse transaction here


                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.AssetTransferReceives.Attach(record);
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

            if (record.AssetTransferReceiveItems.Count == 0)
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
            var assetTransferReceive = _db.AssetTransferReceives.Find(id);
            if (assetTransferReceive != null)
            {
                #region header
                var entity = new Core.Domain.FixedAsset.Transfers.AssetTransferReceive
                {
                    AssetTransferId = assetTransferReceive.AssetTransferId,
                    AssetTransferReceiveId = assetTransferReceive.AssetTransferReceiveId,
                    ReceiveDate = assetTransferReceive.ReceiveDate,

                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = assetTransferReceive.Remark,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = assetTransferReceive.OrgBranchName,

                };

                _db.AssetTransferReceives.Add(entity);
                _db.SaveChanges();
                #endregion
                foreach (var assetTransferReceiveItem in assetTransferReceive.AssetTransferReceiveItems)
                {
                    #region lines
                    var line = new AssetTransferReceiveItem
                    {
                        AssetTransferReceiveId = entity.AssetTransferReceiveId,
                        FixedAssetCategoryId = assetTransferReceiveItem.FixedAssetCategoryId,
                        FixedAssetSubCategoryId = assetTransferReceiveItem.FixedAssetSubCategoryId,
                        AssetRegistrationId = assetTransferReceiveItem.AssetRegistrationId,
                        ItemName = assetTransferReceiveItem.ItemName,
                        Unit = assetTransferReceiveItem.Unit,
                        UnitPrice = assetTransferReceiveItem.UnitPrice,
                        Total = assetTransferReceiveItem.Total,
                        Name = assetTransferReceiveItem.Name,
                        Size = assetTransferReceiveItem.Size,
                        GrnNo = assetTransferReceiveItem.GrnNo,
                        ModelNo = assetTransferReceiveItem.ModelNo,
                        SerialNo = assetTransferReceiveItem.SerialNo,
                        TagNo = assetTransferReceiveItem.TagNo,
                        Quantity = assetTransferReceiveItem.Quantity,
                    };

                    _db.AssetTransferReceiveItems.Add(line);
                    _db.SaveChanges();

                    #endregion

                }

            }

        }


        private void GeTransferSendItems(Core.Domain.FixedAsset.Transfers.AssetTransferReceive assetTransferReceive)
        {
            var transferItems = _db.AssetTransferItems.Where(sr => sr.AssetTransferId== assetTransferReceive.AssetTransferId).ToList();
           foreach (var item in transferItems)
            {
                #region fixed asset
                var assetTransferReceiveItem = new AssetTransferReceiveItem
                {
                    AssetTransferReceiveId = assetTransferReceive.AssetTransferReceiveId,
                    FixedAssetCategoryId = item.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = item.FixedAssetSubCategoryId,
                    AssetRegistrationId = item.AssetRegistrationId,
                    ItemName = item.ItemName,
                    Unit = item.Unit,
                    UnitPrice = item.UnitPrice,
                    Total = item.Total,
                    Name = item.Name,
                    Size = item.Size,
                    GrnNo = item.GrnNo,
                    ModelNo = item.ModelNo,
                    SerialNo = item.SerialNo,
                    TagNo = item.TagNo,
                    Quantity = item.Quantity,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now
                };

                _db.AssetTransferReceiveItems.Add(assetTransferReceiveItem);
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
