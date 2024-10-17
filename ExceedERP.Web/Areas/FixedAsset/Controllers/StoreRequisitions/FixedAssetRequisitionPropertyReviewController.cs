using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.StoreRequisitions
{

    public class FixedAssetRequisitionPropertyReviewController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var assetCategories = _db.FixedAssetCategories;
            var assetSubCategories = _db.FixedAssetSubCategories;
            var fixedAsset = _db.AssetRegistrations;


            var costCenter = _db.BranchCostCenter;
            var branches = _db.Branch;

            var selectedAsset = fixedAsset.Select(
             i => new
             {
                 Name = i.Tag + "-" + i.Name,
                 AssetRegistrationId = i.AssetRegistrationId
             }).AsQueryable();

            ViewData["Branches"] = branches;
            ViewData["SubCategory"] = assetSubCategories;
            ViewData["Category"] = assetCategories;
            ViewData["FixedAsset"] = selectedAsset;

            var store = _db.InventoryStores.AsQueryable();
            ViewData["storesInCompany"] = store;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstName + " " + i.MiddleName + " " + i.LastName,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]

        public ActionResult FixedAssetRequisitionPropertyReviews_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<FixedAssetRequisition> storerequisitions = _db.FixedAssetRequisitions.Where(x => x.IsMiddleOnlineApproved);
            var result = storerequisitions.ToDataSourceResult(request, storeRequisition => new
                {
                    FixedAssetRequisitionId = storeRequisition.FixedAssetRequisitionId,
                    Reference = storeRequisition.Reference,
                    OrgStructureId = storeRequisition.OrgStructureId,
                CreatedAtOrgStructureId = storeRequisition.CreatedAtOrgStructureId,
                BranchId = storeRequisition.BranchId,
                    IsTransfer = storeRequisition.IsTransfer,
                    ToStoreCode = storeRequisition.ToStoreCode,
                    EmployeeId = storeRequisition.EmployeeId,
                    CreatedBy = storeRequisition.CreatedBy,
                    DateCreated = storeRequisition.DateCreated,
                    RequestDate = storeRequisition.RequestDate,
                    PlateNo = storeRequisition.PlateNo,
                    StoreCode = storeRequisition.StoreCode,
                    Status = storeRequisition.Status,
                    Remark = storeRequisition.Remark,
                    IsVoid = storeRequisition.IsVoid,
                    VoidBy = storeRequisition.VoidBy,
                    VoidTime = storeRequisition.VoidTime,
                    IsOnlineApproved = storeRequisition.IsOnlineApproved,
                    OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                    OnlineApprovedTime = storeRequisition.OnlineApprovedTime,

                    IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                    SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                    SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,

                    IsSendForApproval = storeRequisition.IsSendForApproval,
                    SendForApprovalBy = storeRequisition.SendForApprovalBy,
                    SendForApprovalTime = storeRequisition.SendForApprovalTime,

                    IsSecondSendForApproval = storeRequisition.IsSecondSendForApproval,
                    SecondSendForApprovalBy = storeRequisition.SecondSendForApprovalBy,
                    SecondSendForApprovalTime = storeRequisition.SecondSendForApprovalTime,

                    IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                    IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                    OnlineTransferredTime = storeRequisition.OnlineTransferredTime,
                IsFuel = storeRequisition.IsFuel
                });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionPropertyReviews_Create([DataSourceRequest] DataSourceRequest request, FixedAssetRequisition storeRequisition)
        {
            if (storeRequisition.IsTransfer && storeRequisition.ToStoreCode == null)
            {
                ModelState.AddModelError("invalidEntry", "Please select the Requester Store for Transfer");
            }
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.FixedAssetRequisitions.Where(x => x.Reference == storeRequisition.Reference).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", storeRequisition.Reference + " is already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new FixedAssetRequisition
                    {
                        Reference = storeRequisition.Reference,
                        OrgStructureId = UserInformationHelper.GetEmployeeCostCenterId(User.Identity.Name),
                        CreatedAtOrgStructureId = storeRequisition.CreatedAtOrgStructureId,
                        BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
                        EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        RequestDate = DateTime.Today,
                        StoreCode = storeRequisition.StoreCode,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = storeRequisition.Remark,
                        IsVoid = storeRequisition.IsVoid,
                        IsFuel = false,
                        IsTransfer = storeRequisition.IsTransfer,
                        ToStoreCode = storeRequisition.ToStoreCode,
                        PlateNo = storeRequisition.PlateNo,
                        VoidBy = storeRequisition.VoidBy,
                        VoidTime = storeRequisition.VoidTime,
                        IsOnlineApproved = storeRequisition.IsOnlineApproved,
                        OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                        OnlineApprovedTime = storeRequisition.OnlineApprovedTime,
                        IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                        SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                        SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,
                        IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = storeRequisition.OnlineTransferredTime,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now.Date
                    };

                    _db.FixedAssetRequisitions.Add(entity);
                    _db.SaveChanges();
                    storeRequisition.FixedAssetRequisitionId = entity.FixedAssetRequisitionId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisition", "Success", User.Identity.Name, $"created FixedAssetRequisition {entity.FixedAssetRequisitionId} Tag {entity.EmployeeId}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionPropertyReviews_Update([DataSourceRequest] DataSourceRequest request, FixedAssetRequisition storeRequisition)
        {
            if (storeRequisition.IsTransfer && storeRequisition.ToStoreCode == null)
            {
                ModelState.AddModelError("invalidEntry", "Please select the Requester Store for Transfer");
            }

            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.FixedAssetRequisitions.Where(x => x.FixedAssetRequisitionId != storeRequisition.FixedAssetRequisitionId).Where(x => x.Reference == storeRequisition.Reference).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", storeRequisition.Reference + " is already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new FixedAssetRequisition
                    {
                        FixedAssetRequisitionId = storeRequisition.FixedAssetRequisitionId,
                        Reference = storeRequisition.Reference,
                        BranchId = storeRequisition.BranchId,
                        OrgStructureId = UserInformationHelper.GetEmployeeCostCenterId(User.Identity.Name),
                        CreatedAtOrgStructureId = storeRequisition.CreatedAtOrgStructureId,
                        EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        RequestDate = storeRequisition.RequestDate,
                        StoreCode = storeRequisition.StoreCode,
                        Status = storeRequisition.Status,
                        Remark = storeRequisition.Remark,
                        IsFuel = storeRequisition.IsFuel,
                        IsTransfer = storeRequisition.IsTransfer,
                        ToStoreCode = storeRequisition.ToStoreCode,
                        PlateNo = storeRequisition.PlateNo,
                        IsVoid = storeRequisition.IsVoid,
                        VoidBy = storeRequisition.VoidBy,
                        VoidTime = storeRequisition.VoidTime,
                        IsOnlineApproved = storeRequisition.IsOnlineApproved,
                        OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                        OnlineApprovedTime = storeRequisition.OnlineApprovedTime,
                        IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                        SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                        SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,
                        IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = storeRequisition.OnlineTransferredTime,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now.Date
                    };

                    _db.FixedAssetRequisitions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisition", "Success", User.Identity.Name, $"updated FixedAssetRequisition {entity.FixedAssetRequisitionId} Tag {entity.EmployeeId}", Modules.FixedAsset);
                    #endregion
                }

            }

            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionPropertyReviews_Destroy([DataSourceRequest] DataSourceRequest request, FixedAssetRequisition storeRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisition
                    {
                        FixedAssetRequisitionId = storeRequisition.FixedAssetRequisitionId,
                        Reference = storeRequisition.Reference,
                        BranchId = storeRequisition.BranchId,
                        OrgStructureId = storeRequisition.OrgStructureId,
                    CreatedAtOrgStructureId = storeRequisition.CreatedAtOrgStructureId,
                    EmployeeId = storeRequisition.EmployeeId,
                        RequestDate = storeRequisition.RequestDate,
                        StoreCode = storeRequisition.StoreCode,
                        Status = storeRequisition.Status,
                        Remark = storeRequisition.Remark,
                        IsVoid = storeRequisition.IsVoid,
                        IsTransfer = storeRequisition.IsTransfer,
                        ToStoreCode = storeRequisition.ToStoreCode,
                        PlateNo = storeRequisition.PlateNo,
                    VoidBy = storeRequisition.VoidBy,
                        VoidTime = storeRequisition.VoidTime,
                        IsOnlineApproved = storeRequisition.IsOnlineApproved,
                        OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                        OnlineApprovedTime = storeRequisition.OnlineApprovedTime,
                        IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                        SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                        SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,
                        IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = storeRequisition.OnlineTransferredTime
                    };

                _db.FixedAssetRequisitions.Attach(entity);
                _db.FixedAssetRequisitions.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete FixedAssetRequisition {storeRequisition.FixedAssetRequisitionId} Tag {storeRequisition.EmployeeId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { storeRequisition }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]

        public JsonResult SendForApproval(int id)
        {
            var record = _db.FixedAssetRequisitions.Find(id);
            if (record != null && !record.IsSecondSendForApproval && !record.IsSecondOnlineApproved && record.FixedAssetRequisitionItem.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                //record.IsSendForApproval = true;
                record.IsSecondSendForApproval = true; //changed
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.FixedAssetRequisitions.Attach(record);
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

            else if (record.FixedAssetRequisitionItem.Count == 0)
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
            var record = _db.FixedAssetRequisitions.Find(id);
            #region check if purchase is started
            #endregion
            if (record != null && record.IsSecondOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                #region validate issue
                var issues=_db.FixedAssetIssues.Where(x=>x.FixedAssetRequisitionId==id).ToList();
                if (issues.Any())
                {
                    var approvedissue=issues.Where(x=>x.IsVoid==false && x.IsOnlineApproved).ToList();
                    if (approvedissue.Any())
                    {

                        var formattedData3 = new
                        {
                            Message = "The Requsition is issued already, can't be void.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);

                    }
                    
                }
                #endregion

                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsSecondOnlineApproved = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.FixedAssetRequisitions.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();



                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fixed SR Review", "Success", User.Identity.Name, $"void requisition {id} ", Modules.FixedAsset);
                #endregion

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
            if (!record.IsSecondOnlineApproved)
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
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}