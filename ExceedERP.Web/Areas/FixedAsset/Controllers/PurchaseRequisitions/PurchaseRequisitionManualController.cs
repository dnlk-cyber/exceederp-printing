using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.PurchaseRequisitions
{

    public class PurchaseRequisitionManualController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.User = User.Identity.Name;
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
            IQueryable<PurchaseRequisition> purchaserequisitions = db.PurchaseRequisitions.Where(x => x.ItemType == PurchaseItemType.FixedAsset).Where(x=>x.IsManual).Where(x => mybraches.Contains(x.OrgBranchName));

            //new 
            int userworkunit = UserInformationHelper.GetUserReportsTo(User.Identity.Name);

            if (System.Web.HttpContext.Current.User.IsInRole(ProcurementRoles.ProcurementServiceRequisitionSecondApprover)
                && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionApprover)
                && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistion))
            {
                purchaserequisitions = purchaserequisitions.Where(x => x.NextApprover == User.Identity.Name || x.OrgStructureId == userworkunit || x.CreatedBy == User.Identity.Name);
            }

            else if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionApprover)
                && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistion))
            {
                purchaserequisitions = purchaserequisitions.Where(x => x.OrgStructureId == userworkunit || x.CreatedBy == User.Identity.Name);
            }
            else if (System.Web.HttpContext.Current.User.IsInRole(ProcurementRoles.ProcurementServiceRequisitionSecondApprover)
                && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistion))
            {
                purchaserequisitions = purchaserequisitions.Where(x => x.NextApprover == User.Identity.Name || x.CreatedBy == User.Identity.Name);
            }
            else if (System.Web.HttpContext.Current.User.IsInRole(ProcurementRoles.ProcurementServiceRequisitionSecondApprover)
                && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionApprover))
            {
                purchaserequisitions = purchaserequisitions.Where(x => x.NextApprover == User.Identity.Name || x.OrgStructureId == userworkunit);
            }

            else if (System.Web.HttpContext.Current.User.IsInRole(ProcurementRoles.ProcurementServiceRequisitionSecondApprover))
            {
                purchaserequisitions = purchaserequisitions.Where(x => x.NextApprover == User.Identity.Name);
            }
            else if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionApprover))
            {
                purchaserequisitions = purchaserequisitions.Where(x => x.OrgStructureId == userworkunit);
            }
            else if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistion))
            {
                purchaserequisitions = purchaserequisitions.Where(x => x.CreatedBy == User.Identity.Name).AsQueryable();
            }


            var result = purchaserequisitions.ToDataSourceResult(request, purchaseRequisition => new
                {
                    PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                    StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                    BranchId = purchaseRequisition.BranchId,
                    OrgStructureId = purchaseRequisition.OrgStructureId,
                    EmployeeId = purchaseRequisition.EmployeeId,
                    ReferenceNo = purchaseRequisition.ReferenceNo,
                    ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                    RequestDate = purchaseRequisition.RequestDate,
                    ItemType = purchaseRequisition.ItemType,
                    GLPeriodId = purchaseRequisition.GLPeriodId,
                    TenderType = purchaseRequisition.TenderType,
                    IsManual = purchaseRequisition.IsManual,
                    PurchaseCategory = purchaseRequisition.PurchaseCategory,
                    ProcurementMethod = purchaseRequisition.ProcurementMethod,
                    PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
                    NextApprover = purchaseRequisition.NextApprover,

                DateCreated = purchaseRequisition.DateCreated,
                CreatedBy = purchaseRequisition.CreatedBy,
                LastModified = purchaseRequisition.LastModified,
                ModifiedBy = purchaseRequisition.ModifiedBy,

                Status = purchaseRequisition.Status,
                Remark = purchaseRequisition.Remark,
                IsVoid = purchaseRequisition.IsVoid,
                VoidBy = purchaseRequisition.VoidBy,
                VoidTime = purchaseRequisition.VoidTime,
                IsOnlineApproved = purchaseRequisition.IsOnlineApproved,
                OnlineApprovedBy = purchaseRequisition.OnlineApprovedBy,
                OnlineApprovedTime = purchaseRequisition.OnlineApprovedTime,

                IsMiddleOnlineApproved = purchaseRequisition.IsMiddleOnlineApproved,
                MiddleOnlineApprovedBy = purchaseRequisition.MiddleOnlineApprovedBy,
                MiddleOnlineApprovedTime = purchaseRequisition.MiddleOnlineApprovedTime,

                IsSecondOnlineApproved = purchaseRequisition.IsSecondOnlineApproved,
                SecondOnlineApprovedBy = purchaseRequisition.SecondOnlineApprovedBy,
                SecondOnlineApprovedTime = purchaseRequisition.SecondOnlineApprovedTime,

                IsSendForApproval = purchaseRequisition.IsSendForApproval,
                SendForApprovalBy = purchaseRequisition.SendForApprovalBy,
                SendForApprovalTime = purchaseRequisition.SendForApprovalTime,

                IsMiddleSendForApproval = purchaseRequisition.IsMiddleSendForApproval,
                MiddleSendForApprovalBy = purchaseRequisition.MiddleSendForApprovalBy,
                MiddleSendForApprovalTime = purchaseRequisition.MiddleSendForApprovalTime,

                IsSecondSendForApproval = purchaseRequisition.IsSecondSendForApproval,
                SecondSendForApprovalBy = purchaseRequisition.SecondSendForApprovalBy,
                SecondSendForApprovalTime = purchaseRequisition.SecondSendForApprovalTime,

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
            #region validation 


            if (!String.IsNullOrEmpty(purchaseRequisition.ReferenceNo))
            {
                var exists = db.PurchaseRequisitions.Where(x => x.ReferenceNo == purchaseRequisition.ReferenceNo)
                    .ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference",
                        purchaseRequisition.ReferenceNo + " is already exists in the system.");
            }
            if (purchaseRequisition.RequestDate.Date > DateTime.Today)
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");

            //if (purchaseRequisition.EmployeeId == 0)
            //    ModelState.AddModelError("requester", "Please choose requetster");

            //if (purchaseRequisition.BranchId == 0)
            //    ModelState.AddModelError("branch", "Please choose branch");

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new PurchaseRequisition
                    {
                        StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                        IsManual=true,
                        BranchId = purchaseRequisition.BranchId,//UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        OrgStructureId = EmployeeHelper.GetEmployeeWorkUnitId(purchaseRequisition.EmployeeId),
                        EmployeeId = purchaseRequisition.EmployeeId,// UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        ReferenceNo = purchaseRequisition.ReferenceNo,
                        ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                        RequestDate = purchaseRequisition.RequestDate,
                        ProcurementMethod = purchaseRequisition.ProcurementMethod,
                        PurchaseCategory = purchaseRequisition.PurchaseCategory,
                        ItemType = PurchaseItemType.FixedAsset,
                        TenderType = purchaseRequisition.TenderType,
                        PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,


                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        NextApprover = purchaseRequisition.NextApprover,

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
                        OrgBranchName = purchaseRequisition.BranchId.ToString(),
                    };

                    db.PurchaseRequisitions.Add(entity);
                    db.SaveChanges();
                    purchaseRequisition.PurchaseRequisitionID = entity.PurchaseRequisitionID;
                    GetStoreRequisitionItems(entity);
                
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetRequistionApprover, FixedAssetConstants.FixedAssetRequistion)]
        public ActionResult PurchaseRequisitions_Update([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            #region validation 



            if (!String.IsNullOrEmpty(purchaseRequisition.ReferenceNo))
            {
                var exists = db.PurchaseRequisitions.Where(x => x.ReferenceNo == purchaseRequisition.ReferenceNo)
                    .ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference",
                        purchaseRequisition.ReferenceNo + " is already exists in the system.");
            }

            if (purchaseRequisition.RequestDate.Date > DateTime.Today)
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");

            //if (purchaseRequisition.EmployeeId == 0)
            //    ModelState.AddModelError("requester", "Please choose requetster");

            //if (purchaseRequisition.BranchId == 0)
            //    ModelState.AddModelError("branch", "Please choose branch");

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisition
                    {
                        StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                    BranchId = purchaseRequisition.BranchId,//UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        OrgStructureId = EmployeeHelper.GetEmployeeWorkUnitId(purchaseRequisition.EmployeeId),
                        EmployeeId = purchaseRequisition.EmployeeId,// UserInformationHelper.GetEmployeeId(User.Identity.Name),

                    
                        PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                        ReferenceNo = purchaseRequisition.ReferenceNo,
                        ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                        RequestDate = purchaseRequisition.RequestDate,
                        ItemType = purchaseRequisition.ItemType,
                        TenderType = purchaseRequisition.TenderType,
                        IsManual = purchaseRequisition.IsManual,
                        PurchaseCategory = purchaseRequisition.PurchaseCategory,
                        ProcurementMethod = purchaseRequisition.ProcurementMethod,
                        PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
                        NextApprover = purchaseRequisition.NextApprover,

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
                        OrgStructureId = purchaseRequisition.OrgStructureId,
                        EmployeeId = purchaseRequisition.EmployeeId,
                        ReferenceNo = purchaseRequisition.ReferenceNo,
                        ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                        RequestDate = purchaseRequisition.RequestDate,
                        ItemType = purchaseRequisition.ItemType,
                        PurchaseCategory = purchaseRequisition.PurchaseCategory,
                        ProcurementMethod = purchaseRequisition.ProcurementMethod,
                        PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
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
                    PurchaseRequisitionID = record.PurchaseRequisitionID,
                    RequestedQuantity = requestedItem.PurchaseQuantity,
                    Quantity = requestedItem.PurchaseQuantity,
                    ItemSpecification = requestedItem.ItemSpecification,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now
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