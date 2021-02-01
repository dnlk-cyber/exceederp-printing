using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.PurchaseRequisitions
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser, InventoryConstants.PropertyAuthrizer)]
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

            var items = db.PurchaseRequisitionItems;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItmCode + "-" + i.ItemName,
                    ItmCode = i.ItmCode
                }).AsQueryable();

            ViewData["Items"] = selecteditems;

            var selectedcatedories = items.Select(
                i => new
                {
                    Name = i.CatCode + "-" + i.CategoryName,
                    CatCode = i.CatCode
                }).AsQueryable();
            ViewData["Category"] = selectedcatedories;
            ViewData["defaultCategory"] = selectedcatedories.FirstOrDefault();

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




        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]
        public ActionResult PurchaseRequisitions_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<PurchaseRequisition> purchaserequisitions = db.PurchaseRequisitions.Where(x=>x.ItemType==PurchaseItemType.Stock&&x.IsManual==false).Where(x => mybraches.Contains(x.OrgBranchName));
            var result = purchaserequisitions.ToDataSourceResult(request, purchaseRequisition => new
                {
                    PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                    StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                    BranchId = purchaseRequisition.BranchId,
                    ForBranchId = purchaseRequisition.ForBranchId,
                    OrgStructureId = purchaseRequisition.OrgStructureId,
                    EmployeeId = purchaseRequisition.EmployeeId,
                    ReferenceNo = purchaseRequisition.ReferenceNo,
                    ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                    RequestDate = purchaseRequisition.RequestDate,
                    ItemType = purchaseRequisition.ItemType,
                    GLPeriodId = purchaseRequisition.GLPeriodId,
                    ModelNo = purchaseRequisition.ModelNo,
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


        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyUser)]
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
                        BranchId = UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        ForBranchId = purchaseRequisition.ForBranchId,
                        OrgStructureId = UserInformationHelper.GetEmployeeCostCenterId(User.Identity.Name),
                        EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        ReferenceNo = purchaseRequisition.ReferenceNo,
                        ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                        RequestDate = purchaseRequisition.RequestDate.Date,
                        ProcurementMethod = purchaseRequisition.ProcurementMethod,
                        PurchaseCategory = purchaseRequisition.PurchaseCategory,
                        ItemType = PurchaseItemType.Stock,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        ModelNo = purchaseRequisition.ModelNo,
                        TenderType = purchaseRequisition.TenderType,
                        PlateNo = purchaseRequisition.PlateNo,

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
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        OrgBranchName = UserInformationHelper.GetEmployeeBranchId(User.Identity.Name).ToString(),
                    };

                    db.PurchaseRequisitions.Add(entity);
                    db.SaveChanges();
                    purchaseRequisition.PurchaseRequisitionID = entity.PurchaseRequisitionID;
                    GetStoreRequisitionItems(entity);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition", "Success", User.Identity.Name, $"created purchase requisition {entity.PurchaseRequisitionID} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]
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
                        ModelNo = purchaseRequisition.ModelNo,
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
                    UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition", "Success", User.Identity.Name, $"updated purchase requisition {entity.PurchaseRequisitionID} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
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
                        OrgStructureId = purchaseRequisition.OrgStructureId,
                        EmployeeId = purchaseRequisition.EmployeeId,
                        ReferenceNo = purchaseRequisition.ReferenceNo,
                        ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                        RequestDate = purchaseRequisition.RequestDate,
                        ProcurementMethod = purchaseRequisition.ProcurementMethod,
                        PurchaseCategory = purchaseRequisition.PurchaseCategory,
                        ItemType = purchaseRequisition.ItemType,
                        ModelNo = purchaseRequisition.ModelNo,
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
                        OnlineTransferredTime = purchaseRequisition.OnlineTransferredTime
                    };

                db.PurchaseRequisitions.Attach(entity);
                db.PurchaseRequisitions.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted purchase requisition number " + purchaseRequisition.StoreRequisitionID + " " + purchaseRequisition.ReferenceNo);
                #endregion
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        private void GetStoreRequisitionItems(PurchaseRequisition record)
        {
            var storerequistionItem = db.StoreRequisitionItems.Where(sri => sri.StoreRequisitionID == record.StoreRequisitionID).ToList();
            foreach (var requestedItem in storerequistionItem)
            {
                var purchaseRequisitionItem = new PurchaseRequisitionItem
                {
                    CatCode = requestedItem.ItemCategoryCode,
                    ItmCode = requestedItem.ItemCode,
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

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyUser)]
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

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition", "Success", User.Identity.Name, $"send for approval purchase requisition {id} ", Modules.Inventory);
                #endregion
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