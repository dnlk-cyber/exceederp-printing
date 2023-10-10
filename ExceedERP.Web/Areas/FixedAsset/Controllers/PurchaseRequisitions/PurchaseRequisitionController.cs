using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.PurchaseRequisitions
{

    public class PurchaseRequisitionController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            var assetCategories = db.FixedAssetCategories;
            var assetSubCategories = db.FixedAssetSubCategories;


            ViewData["SubCategory"] = assetSubCategories;
            ViewData["Category"] = assetCategories;

            var store = db.InventoryStores.AsQueryable();
            ViewData["storesInCompany"] = store;

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var costCenter = db.BranchCostCenter;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;

            var branches = db.Branch;
            ViewData["Branches"] = branches;

        }




        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistionApprover, FixedAssetConstants.FixedAssetRequistion)]
        public ActionResult PurchaseRequisitions_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<PurchaseRequisition> purchaserequisitions = db.PurchaseRequisitions.Where(x => x.ItemType == PurchaseItemType.FixedAsset && x.IsManual == false).Where(x => mybraches.Contains(x.OrgBranchName));
            var result = purchaserequisitions.ToDataSourceResult(request, purchaseRequisition => new
            {
                PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                BranchId = purchaseRequisition.BranchId,
                BusinessUnitId = purchaseRequisition.BusinessUnitId,
                ForBranchId = purchaseRequisition.ForBranchId,
                OrgStructureId = purchaseRequisition.OrgStructureId,
                EmployeeId = purchaseRequisition.EmployeeId,
                ReferenceNo = purchaseRequisition.ReferenceNo,
                ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                RequestDate = purchaseRequisition.RequestDate,
                ItemType = purchaseRequisition.ItemType,
                GLPeriodId = purchaseRequisition.GLPeriodId,
                TenderType = purchaseRequisition.TenderType,
                PlateNo = purchaseRequisition.PlateNo,


                Status = purchaseRequisition.Status,
                Remark = purchaseRequisition.Remark,
                IsVoid = purchaseRequisition.IsVoid,
                VoidBy = purchaseRequisition.VoidBy,
                VoidTime = purchaseRequisition.VoidTime,
                IsSendForApproval = purchaseRequisition.IsSendForApproval,
                SendForApprovalBy = purchaseRequisition.SendForApprovalBy,
                SendForApprovalTime = purchaseRequisition.SendForApprovalTime,

                IsBudgetApproved = purchaseRequisition.IsBudgetApproved,
                BudgetApprovedBy = purchaseRequisition.BudgetApprovedBy,
                BudgetApprovedTime = purchaseRequisition.BudgetApprovedTime,

                IsOnlineApproved = purchaseRequisition.IsOnlineApproved,
                OnlineApprovedBy = purchaseRequisition.OnlineApprovedBy,
                OnlineApprovedTime = purchaseRequisition.OnlineApprovedTime,
                IsOnlineTransferred = purchaseRequisition.IsOnlineTransferred,
                IsOnlineTransferredBy = purchaseRequisition.IsOnlineTransferredBy,
                OnlineTransferredTime = purchaseRequisition.OnlineTransferredTime,

                OrgBranchName = purchaseRequisition.OrgBranchName,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitions_Create([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            bool valid = true;
            var storeRequisition = db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == purchaseRequisition.StoreRequisitionID);

            if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
            {
                ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");
                valid = false;
            }

            var exists = db.PurchaseRequisitions.Where(x => x.ReferenceNo == purchaseRequisition.ReferenceNo).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Reference", purchaseRequisition.ReferenceNo + " is already exists in the system.");
                valid = false;
            }
            if (purchaseRequisition.RequestDate > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }
            if (ModelState.IsValid)
            {

                if (valid)
                {
                    var entity = new PurchaseRequisition
                    {
                        StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                        BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
                        BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                        ForBranchId = purchaseRequisition.ForBranchId,
                        OrgStructureId = UserInformationHelper.GetEmployeeCostCenterId(User.Identity.Name),
                        EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        ReferenceNo = purchaseRequisition.ReferenceNo,
                        ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                        RequestDate = purchaseRequisition.RequestDate,
                        ProcurementMethod = purchaseRequisition.ProcurementMethod,
                        PurchaseCategory = purchaseRequisition.PurchaseCategory,
                        ItemType = PurchaseItemType.FixedAsset,
                        TenderType = purchaseRequisition.TenderType,
                        PlateNo = purchaseRequisition.PlateNo,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = purchaseRequisition.Remark,
                        IsVoid = purchaseRequisition.IsVoid,
                        VoidBy = purchaseRequisition.VoidBy,
                        VoidTime = purchaseRequisition.VoidTime,
                        IsOnlineApproved = purchaseRequisition.IsOnlineApproved,
                        OnlineApprovedBy = purchaseRequisition.OnlineApprovedBy,
                        OnlineApprovedTime = purchaseRequisition.OnlineApprovedTime,
                        IsOnlineTransferred = purchaseRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = purchaseRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = purchaseRequisition.OnlineTransferredTime,
                        IsBudgetApproved = purchaseRequisition.IsBudgetApproved,
                        BudgetApprovedBy = purchaseRequisition.BudgetApprovedBy,
                        BudgetApprovedTime = purchaseRequisition.BudgetApprovedTime,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name).ToString(),
                    };

                    db.PurchaseRequisitions.Add(entity);
                    db.SaveChanges();
                    purchaseRequisition.PurchaseRequisitionID = entity.PurchaseRequisitionID;
                    GetStoreRequisitionItems(entity);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "PurchaseRequisition", "Success", User.Identity.Name, $"created PurchaseRequisition {entity.PurchaseRequisitionID} Tag {entity.ReferenceNo}", Modules.FixedAsset);
                    #endregion

                }
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistionApprover, FixedAssetConstants.FixedAssetRequistion)]
        public ActionResult PurchaseRequisitions_Update([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            bool valid = true;
            var storeRequisition = db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == purchaseRequisition.StoreRequisitionID);

            if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
            {
                ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");
                valid = false;
            }

            var exists = db.PurchaseRequisitions.Where(x => x.PurchaseRequisitionID != purchaseRequisition.PurchaseRequisitionID)
                .Where(x => x.ReferenceNo == purchaseRequisition.ReferenceNo).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Reference", purchaseRequisition.ReferenceNo + " is already exists in the system.");
                valid = false;
            }

            if (purchaseRequisition.RequestDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (ModelState.IsValid)
            {

                if (valid)
                {
                    var entity = new PurchaseRequisition
                    {
                        StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                        BranchId = purchaseRequisition.BranchId,
                        BusinessUnitId = purchaseRequisition.BusinessUnitId,
                        ForBranchId = purchaseRequisition.ForBranchId,
                        OrgStructureId = purchaseRequisition.OrgStructureId,
                        EmployeeId = purchaseRequisition.EmployeeId,

                        PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                        ReferenceNo = purchaseRequisition.ReferenceNo,
                        ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                        RequestDate = purchaseRequisition.RequestDate,
                        ProcurementMethod = purchaseRequisition.ProcurementMethod,
                        PurchaseCategory = purchaseRequisition.PurchaseCategory,
                        ItemType = purchaseRequisition.ItemType,
                        TenderType = purchaseRequisition.TenderType,
                        PlateNo = purchaseRequisition.PlateNo,
                        Status = purchaseRequisition.Status,
                        Remark = purchaseRequisition.Remark,
                        IsVoid = purchaseRequisition.IsVoid,
                        VoidBy = purchaseRequisition.VoidBy,
                        VoidTime = purchaseRequisition.VoidTime,
                        IsOnlineApproved = purchaseRequisition.IsOnlineApproved,
                        OnlineApprovedBy = purchaseRequisition.OnlineApprovedBy,
                        OnlineApprovedTime = purchaseRequisition.OnlineApprovedTime,
                        IsOnlineTransferred = purchaseRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = purchaseRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = purchaseRequisition.OnlineTransferredTime,
                        IsBudgetApproved = purchaseRequisition.IsBudgetApproved,
                        BudgetApprovedBy = purchaseRequisition.BudgetApprovedBy,
                        BudgetApprovedTime = purchaseRequisition.BudgetApprovedTime,

                        CreatedBy = purchaseRequisition.CreatedBy,
                        DateCreated = purchaseRequisition.DateCreated,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now,
                        OrgBranchName = purchaseRequisition.BranchId.ToString(),
                    };

                    db.PurchaseRequisitions.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "PurchaseRequisition", "Success", User.Identity.Name, $"updated PurchaseRequisition {entity.PurchaseRequisitionID} Tag {entity.ReferenceNo}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitions_Destroy([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisition
                {
                    PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                    StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                    BranchId = purchaseRequisition.BranchId,
                    BusinessUnitId = purchaseRequisition.BusinessUnitId,
                    ForBranchId = purchaseRequisition.ForBranchId,
                    OrgStructureId = purchaseRequisition.OrgStructureId,
                    EmployeeId = purchaseRequisition.EmployeeId,
                    ReferenceNo = purchaseRequisition.ReferenceNo,
                    ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                    RequestDate = purchaseRequisition.RequestDate,
                    ProcurementMethod = purchaseRequisition.ProcurementMethod,
                    PurchaseCategory = purchaseRequisition.PurchaseCategory,
                    ItemType = purchaseRequisition.ItemType,
                    Status = purchaseRequisition.Status,
                    Remark = purchaseRequisition.Remark,
                    IsVoid = purchaseRequisition.IsVoid,
                    VoidBy = purchaseRequisition.VoidBy,
                    VoidTime = purchaseRequisition.VoidTime,
                    IsOnlineApproved = purchaseRequisition.IsOnlineApproved,
                    OnlineApprovedBy = purchaseRequisition.OnlineApprovedBy,
                    OnlineApprovedTime = purchaseRequisition.OnlineApprovedTime,
                    IsOnlineTransferred = purchaseRequisition.IsOnlineTransferred,
                    IsOnlineTransferredBy = purchaseRequisition.IsOnlineTransferredBy,
                    OnlineTransferredTime = purchaseRequisition.OnlineTransferredTime,
                    IsBudgetApproved = purchaseRequisition.IsBudgetApproved,
                    BudgetApprovedBy = purchaseRequisition.BudgetApprovedBy,
                    BudgetApprovedTime = purchaseRequisition.BudgetApprovedTime,
                };

                db.PurchaseRequisitions.Attach(entity);
                db.PurchaseRequisitions.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted purchase requisition number " + purchaseRequisition.StoreRequisitionID + " " + purchaseRequisition.ReferenceNo);
                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "delete", "Success", User.Identity.Name, $"delete PurchaseRequisition {purchaseRequisition.PurchaseRequisitionID} Tag {purchaseRequisition.ReferenceNo}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        private void GetStoreRequisitionItems(PurchaseRequisition record)
        {
            var storerequistionItem = db.FixedAssetRequisitionItems.Where(sri => sri.FixedAssetRequisitionId == record.StoreRequisitionID).ToList();
            foreach (var requestedItem in storerequistionItem)
            {
                var purchaseRequisitionItem = new PurchaseRequisitionItem
                {
                    CatCode = requestedItem.FixedAssetCategoryID.ToString(),
                    ItmCode = requestedItem.FixedAssetSubCategoryID.ToString(),
                    PartNo = requestedItem.PartNo,
                    ModelNo = requestedItem.ModelNo,
                    SerialNo = requestedItem.SerialNo,
                    ChasisNo = requestedItem.ChasisNo,
                    PurchaseRequisitionID = record.PurchaseRequisitionID,
                    RequestedQuantity = requestedItem.PurchaseQuantity,
                    Quantity = requestedItem.PurchaseQuantity,
                    ItemSpecification = requestedItem.ItemSpecification,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now.Date,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now.Date
                };


                db.PurchaseRequisitionItems.Add(purchaseRequisitionItem);
                db.SaveChanges();
            }
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistion)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.PurchaseRequisitions.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.PurchaseRequisitionItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.PurchaseRequisitions.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
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

            else if (record.PurchaseRequisitionItems.Count == 0)
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}