using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
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

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.MachineTransferReceive
{
    public class MachineryAndVehicleTransferReceiveController : Controller
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
        public ActionResult MachineryAndVehicleTransferReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MachineryAndVehicleTransferReceive> machineryAndVehicleTransferReceives = _db.MachineryAndVehicleTransferReceives;
            DataSourceResult result = machineryAndVehicleTransferReceives.ToDataSourceResult(request, machineryAndVehicleTransferReceive => new {
                MachineryAndVehicleTransferReceiveId = machineryAndVehicleTransferReceive.MachineryAndVehicleTransferReceiveId,
                MachineryAndVehicleTransferId = machineryAndVehicleTransferReceive.MachineryAndVehicleTransferId,
                ReceiveDate = machineryAndVehicleTransferReceive.ReceiveDate,
                Status = machineryAndVehicleTransferReceive.Status,
                Remark = machineryAndVehicleTransferReceive.Remark,
                IsVoid = machineryAndVehicleTransferReceive.IsVoid,
                VoidBy = machineryAndVehicleTransferReceive.VoidBy,
                VoidTime = machineryAndVehicleTransferReceive.VoidTime,
                IsOnlineApproved = machineryAndVehicleTransferReceive.IsOnlineApproved,
                OnlineApprovedBy = machineryAndVehicleTransferReceive.OnlineApprovedBy,
                OnlineApprovedTime = machineryAndVehicleTransferReceive.OnlineApprovedTime,
                IsOnlineTransferred = machineryAndVehicleTransferReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = machineryAndVehicleTransferReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = machineryAndVehicleTransferReceive.OnlineTransferredTime,
                IsSendForApproval = machineryAndVehicleTransferReceive.IsSendForApproval,
                OrgBranchName = machineryAndVehicleTransferReceive.OrgBranchName,
                SendForApprovalBy = machineryAndVehicleTransferReceive.SendForApprovalBy,
                SendForApprovalTime = machineryAndVehicleTransferReceive.SendForApprovalTime,
                DateCreated = machineryAndVehicleTransferReceive.DateCreated,
                LastModified = machineryAndVehicleTransferReceive.LastModified,
                CreatedBy = machineryAndVehicleTransferReceive.CreatedBy,
                ModifiedBy = machineryAndVehicleTransferReceive.ModifiedBy
            });

            return Json(result);
        }

        //[AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransferReceives_Create([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransferReceive machineryAndVehicleTransferReceive)
        {

            #region validation
            var asset = _db.MachineryAndVehicleTransferReceives
                .Where(x=>x.IsOnlineApproved)
                .Where(x => x.IsVoid==false)
                .FirstOrDefault(x=>x.MachineryAndVehicleTransferId==machineryAndVehicleTransferReceive.MachineryAndVehicleTransferId);
            if (asset != null)
            {
                ModelState.AddModelError("exist", "The transfer is receive already.");
            }
            #endregion

            if (ModelState.IsValid)
            {
                
                    var entity = new MachineryAndVehicleTransferReceive
                    {
                        MachineryAndVehicleTransferId = machineryAndVehicleTransferReceive.MachineryAndVehicleTransferId,
                        ReceiveDate = machineryAndVehicleTransferReceive.ReceiveDate,
                        Status = machineryAndVehicleTransferReceive.Status,
                        Remark = machineryAndVehicleTransferReceive.Remark,
                        IsVoid = machineryAndVehicleTransferReceive.IsVoid,
                        VoidBy = machineryAndVehicleTransferReceive.VoidBy,
                        VoidTime = machineryAndVehicleTransferReceive.VoidTime,
                        IsOnlineApproved = machineryAndVehicleTransferReceive.IsOnlineApproved,
                        OnlineApprovedBy = machineryAndVehicleTransferReceive.OnlineApprovedBy,
                        OnlineApprovedTime = machineryAndVehicleTransferReceive.OnlineApprovedTime,
                        IsOnlineTransferred = machineryAndVehicleTransferReceive.IsOnlineTransferred,
                        IsOnlineTransferredBy = machineryAndVehicleTransferReceive.IsOnlineTransferredBy,
                        OnlineTransferredTime = machineryAndVehicleTransferReceive.OnlineTransferredTime,
                        IsSendForApproval = machineryAndVehicleTransferReceive.IsSendForApproval,
                        OrgBranchName = machineryAndVehicleTransferReceive.OrgBranchName,
                        SendForApprovalBy = machineryAndVehicleTransferReceive.SendForApprovalBy,
                        SendForApprovalTime = machineryAndVehicleTransferReceive.SendForApprovalTime,
                        DateCreated = machineryAndVehicleTransferReceive.DateCreated,
                        LastModified = machineryAndVehicleTransferReceive.LastModified,
                        CreatedBy = machineryAndVehicleTransferReceive.CreatedBy,
                        ModifiedBy = machineryAndVehicleTransferReceive.ModifiedBy
                    };

                    _db.MachineryAndVehicleTransferReceives.Add(entity);
                    _db.SaveChanges();
                    machineryAndVehicleTransferReceive.MachineryAndVehicleTransferReceiveId = entity.MachineryAndVehicleTransferReceiveId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "MachineryAndVehicleTransferReceive", "Success", User.Identity.Name, $"created MachineryAndVehicleTransferReceive {entity.MachineryAndVehicleTransferReceiveId} OrgBranchName {entity.OrgBranchName}", Modules.FixedAsset);
                #endregion

            }

            return Json(new[] { machineryAndVehicleTransferReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransferReceives_Update([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransferReceive machineryAndVehicleTransferReceive)
        {
            #region validation
            var asset = _db.MachineryAndVehicleTransferReceives
                .Where(x => x.IsOnlineApproved)
                .Where(x => x.IsVoid == false)
                .Where(x => x.MachineryAndVehicleTransferReceiveId != machineryAndVehicleTransferReceive.MachineryAndVehicleTransferReceiveId)
                .FirstOrDefault(x => x.MachineryAndVehicleTransferId == machineryAndVehicleTransferReceive.MachineryAndVehicleTransferId);
            if (asset != null)
            {
                ModelState.AddModelError("exist", "The transfer is receive already.");
            }
            #endregion

            if (ModelState.IsValid)
            {
                
                    var entity = new MachineryAndVehicleTransferReceive
                    {
                        MachineryAndVehicleTransferReceiveId = machineryAndVehicleTransferReceive.MachineryAndVehicleTransferReceiveId,
                        ReceiveDate = machineryAndVehicleTransferReceive.ReceiveDate,
                        Status = machineryAndVehicleTransferReceive.Status,
                        Remark = machineryAndVehicleTransferReceive.Remark,
                        IsVoid = machineryAndVehicleTransferReceive.IsVoid,
                        VoidBy = machineryAndVehicleTransferReceive.VoidBy,
                        VoidTime = machineryAndVehicleTransferReceive.VoidTime,
                        IsOnlineApproved = machineryAndVehicleTransferReceive.IsOnlineApproved,
                        OnlineApprovedBy = machineryAndVehicleTransferReceive.OnlineApprovedBy,
                        OnlineApprovedTime = machineryAndVehicleTransferReceive.OnlineApprovedTime,
                        IsOnlineTransferred = machineryAndVehicleTransferReceive.IsOnlineTransferred,
                        IsOnlineTransferredBy = machineryAndVehicleTransferReceive.IsOnlineTransferredBy,
                        OnlineTransferredTime = machineryAndVehicleTransferReceive.OnlineTransferredTime,
                        IsSendForApproval = machineryAndVehicleTransferReceive.IsSendForApproval,
                        OrgBranchName = machineryAndVehicleTransferReceive.OrgBranchName,
                        SendForApprovalBy = machineryAndVehicleTransferReceive.SendForApprovalBy,
                        SendForApprovalTime = machineryAndVehicleTransferReceive.SendForApprovalTime,
                        DateCreated = machineryAndVehicleTransferReceive.DateCreated,
                        LastModified = machineryAndVehicleTransferReceive.LastModified,
                        CreatedBy = machineryAndVehicleTransferReceive.CreatedBy,
                        ModifiedBy = machineryAndVehicleTransferReceive.ModifiedBy
                    };

                    _db.MachineryAndVehicleTransferReceives.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "MachineryAndVehicleTransferReceive", "Success", User.Identity.Name, $"update MachineryAndVehicleTransferReceive {entity.MachineryAndVehicleTransferReceiveId} OrgBranchName {entity.OrgBranchName}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { machineryAndVehicleTransferReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryAndVehicleTransferReceives_Destroy([DataSourceRequest]DataSourceRequest request, MachineryAndVehicleTransferReceive machineryAndVehicleTransferReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachineryAndVehicleTransferReceive
                {
                    MachineryAndVehicleTransferReceiveId = machineryAndVehicleTransferReceive.MachineryAndVehicleTransferReceiveId,
                    ReceiveDate = machineryAndVehicleTransferReceive.ReceiveDate,
                    Status = machineryAndVehicleTransferReceive.Status,
                    Remark = machineryAndVehicleTransferReceive.Remark,
                    IsVoid = machineryAndVehicleTransferReceive.IsVoid,
                    VoidBy = machineryAndVehicleTransferReceive.VoidBy,
                    VoidTime = machineryAndVehicleTransferReceive.VoidTime,
                    IsOnlineApproved = machineryAndVehicleTransferReceive.IsOnlineApproved,
                    OnlineApprovedBy = machineryAndVehicleTransferReceive.OnlineApprovedBy,
                    OnlineApprovedTime = machineryAndVehicleTransferReceive.OnlineApprovedTime,
                    IsOnlineTransferred = machineryAndVehicleTransferReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = machineryAndVehicleTransferReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = machineryAndVehicleTransferReceive.OnlineTransferredTime,
                    IsSendForApproval = machineryAndVehicleTransferReceive.IsSendForApproval,
                    OrgBranchName = machineryAndVehicleTransferReceive.OrgBranchName,
                    SendForApprovalBy = machineryAndVehicleTransferReceive.SendForApprovalBy,
                    SendForApprovalTime = machineryAndVehicleTransferReceive.SendForApprovalTime,
                    DateCreated = machineryAndVehicleTransferReceive.DateCreated,
                    LastModified = machineryAndVehicleTransferReceive.LastModified,
                    CreatedBy = machineryAndVehicleTransferReceive.CreatedBy,
                    ModifiedBy = machineryAndVehicleTransferReceive.ModifiedBy
                };

                _db.MachineryAndVehicleTransferReceives.Attach(entity);
                _db.MachineryAndVehicleTransferReceives.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete MachineryAndVehicleTransferReceive {machineryAndVehicleTransferReceive.MachineryAndVehicleTransferReceiveId} OrgBranchName {machineryAndVehicleTransferReceive.OrgBranchName}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { machineryAndVehicleTransferReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.MachineryAndVehicleTransferReceives.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.MachineryAndVehicleTransferReceives.Attach(record);
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
            var record = _db.MachineryAndVehicleTransferReceives.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                //reverse transaction here


                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.MachineryAndVehicleTransferReceives.Attach(record);
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
            var machineryAndVehicleTransferReceive = _db.MachineryAndVehicleTransferReceives.Find(id);
            if (machineryAndVehicleTransferReceive != null)
            {
                #region header
                var entity = new MachineryAndVehicleTransferReceive
                {
                    MachineryAndVehicleTransferId = machineryAndVehicleTransferReceive.MachineryAndVehicleTransferId,
                    ReceiveDate = machineryAndVehicleTransferReceive.ReceiveDate,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = machineryAndVehicleTransferReceive.OrgBranchName,

                };

                _db.MachineryAndVehicleTransferReceives.Add(entity);
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
