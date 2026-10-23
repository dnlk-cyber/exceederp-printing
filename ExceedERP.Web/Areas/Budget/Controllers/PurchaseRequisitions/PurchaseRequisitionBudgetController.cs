using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.ProcurementPaymentRequest
{

    public class PurchaseRequisitionBudgetController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            //var categories = db.ItemCategorys;
            var items = db.PurchaseRequisitionItems;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItmCode + "-" + i.ItemName + "-" + i.Name,
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

        [AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        public ActionResult PurchaseRequisitionBudgets_Read([DataSourceRequest] DataSourceRequest request)
        {

            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<PurchaseRequisition> purchaserequisitions = db.PurchaseRequisitions
                .Where(x => x.IsDepartmentOnlineApproved && !x.IsManual || x.IsOnlineApproved && !x.IsManual || x.IsMiddleOnlineApproved && x.IsManual)
                .Where(x => mybraches.Contains(x.BranchId));

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                purchaserequisitions = purchaserequisitions.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }

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
                ProcurementMethod = purchaseRequisition.ProcurementMethod,
                PurchaseCategory = purchaseRequisition.PurchaseCategory,
                GLPeriodId = purchaseRequisition.GLPeriodId,
                ItemType = purchaseRequisition.ItemType,
                FromPreviousPurchase = purchaseRequisition.FromPreviousPurchase,
                PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
                PreviousPurchaseReference = purchaseRequisition.PreviousPurchaseReference,
                ModelNo = purchaseRequisition.ModelNo,
                TenderType = purchaseRequisition.TenderType,
                PlateNo = purchaseRequisition.PlateNo,
                NextApprover = purchaseRequisition.NextApprover,
                IsManual = purchaseRequisition.IsManual,
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

                IsMiddleSendForApproval = purchaseRequisition.IsMiddleSendForApproval,
                MiddleSendForApprovalTime = purchaseRequisition.MiddleSendForApprovalTime,
                MiddleSendForApprovalBy = purchaseRequisition.MiddleSendForApprovalBy,

                IsOnlineTransferred = purchaseRequisition.IsOnlineTransferred,
                IsOnlineTransferredBy = purchaseRequisition.IsOnlineTransferredBy,
                OnlineTransferredTime = purchaseRequisition.OnlineTransferredTime,

                IsSecondSendForApproval = purchaseRequisition.IsSecondSendForApproval,
                SecondSendForApprovalBy = purchaseRequisition.SecondSendForApprovalBy,
                SecondSendForApprovalTime = purchaseRequisition.SecondSendForApprovalTime,

                IsSecondOnlineApproved = purchaseRequisition.IsSecondOnlineApproved,
                SecondOnlineApprovedBy = purchaseRequisition.SecondOnlineApprovedBy,
                SecondOnlineApprovedTime = purchaseRequisition.SecondOnlineApprovedTime,

                IsSecondVoid = purchaseRequisition.IsSecondVoid,
                SecondVoidBy = purchaseRequisition.SecondVoidBy,
                SecondVoidTime = purchaseRequisition.SecondVoidTime,

                IsBudgetApproved = purchaseRequisition.IsBudgetApproved,
                BudgetApprovedBy = purchaseRequisition.BudgetApprovedBy,
                BudgetApprovedTime = purchaseRequisition.BudgetApprovedTime,

                CreatedBy = purchaseRequisition.CreatedBy,
                DateCreated = purchaseRequisition.DateCreated,
                ModifiedBy = purchaseRequisition.ModifiedBy,
                LastModified = purchaseRequisition.LastModified,
                OrgBranchName = purchaseRequisition.OrgBranchName
            });

            return Json(result);
        }


        [AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionBudgets_Create([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            if (purchaseRequisition.ProcurementMethod == ProcurementMethod.Tender && purchaseRequisition.TenderType == TenderType.NA)
                ModelState.AddModelError("tendertype", "Please choose correct tender type");

            if (purchaseRequisition.ProcurementMethod != ProcurementMethod.Tender && purchaseRequisition.TenderType != TenderType.NA)
                ModelState.AddModelError("tendertype", "Please choose correct tender type");

            bool valid = true;
            var storeRequisition = db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == purchaseRequisition.StoreRequisitionID);
            var fixedAssetRequisition = db.FixedAssetRequisitions.FirstOrDefault(sr => sr.FixedAssetRequisitionId == purchaseRequisition.StoreRequisitionID);

            if (purchaseRequisition.ItemType == PurchaseItemType.Stock)
                if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
                    ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");
            if (purchaseRequisition.ItemType == PurchaseItemType.FixedAsset)
                if (fixedAssetRequisition != null && fixedAssetRequisition.FixedAssetRequisitionItem.Count == 0)
                    ModelState.AddModelError("requestedItem", "No Requested Item Found in Fixed Asset Requisition Please Check the Fixed Asset Requisition");

            var exists = db.PurchaseRequisitions.Where(x => x.ReferenceNo == purchaseRequisition.ReferenceNo).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Reference", purchaseRequisition.ReferenceNo + " already exists in the system.");
                valid = false;
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
                        ReferenceNo = purchaseRequisition.ReferenceNo,
                        ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                        RequestDate = purchaseRequisition.RequestDate,
                        ProcurementMethod = purchaseRequisition.ProcurementMethod,
                        PurchaseCategory = purchaseRequisition.PurchaseCategory,
                        ItemType = purchaseRequisition.ItemType,
                        FromPreviousPurchase = purchaseRequisition.FromPreviousPurchase,
                        PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
                        PreviousPurchaseReference = purchaseRequisition.PreviousPurchaseReference,
                        ModelNo = purchaseRequisition.ModelNo,
                        TenderType = purchaseRequisition.TenderType,
                        PlateNo = purchaseRequisition.PlateNo,

                        NextApprover = purchaseRequisition.NextApprover,
                        IsManual = purchaseRequisition.IsManual,

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

                        IsSecondOnlineApproved = purchaseRequisition.IsSecondOnlineApproved,
                        SecondOnlineApprovedBy = purchaseRequisition.SecondOnlineApprovedBy,
                        SecondOnlineApprovedTime = purchaseRequisition.SecondOnlineApprovedTime,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        LastModified = purchaseRequisition.LastModified,
                        ModifiedBy = purchaseRequisition.ModifiedBy,
                        OrgBranchName = purchaseRequisition.BranchId.ToString()
                    };

                    db.PurchaseRequisitions.Add(entity);
                    db.SaveChanges();
                    purchaseRequisition.PurchaseRequisitionID = entity.PurchaseRequisitionID;
                    GetStoreRequisitionItems(entity);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition review", "Success", User.Identity.Name, $"created purchase requisition {entity.PurchaseRequisitionID} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        public ActionResult PurchaseRequisitionBudgets_Update([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {

            //if (purchaseRequisition.ProcurementMethod == ProcurementMethod.Tender && purchaseRequisition.TenderType == TenderType.NA)
            //    ModelState.AddModelError("tendertype", "Please choose correct tender type");

            //if (purchaseRequisition.ProcurementMethod != ProcurementMethod.Tender && purchaseRequisition.TenderType != TenderType.NA)
            //    ModelState.AddModelError("tendertype", "Please choose correct tender type");

            var storeRequisition = db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == purchaseRequisition.StoreRequisitionID);
            var fixedAssetRequisition = db.FixedAssetRequisitions.FirstOrDefault(sr => sr.FixedAssetRequisitionId == purchaseRequisition.StoreRequisitionID);

            if (purchaseRequisition.ItemType == PurchaseItemType.Stock)
                if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
                    ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");
            if (purchaseRequisition.ItemType == PurchaseItemType.FixedAsset)
                if (fixedAssetRequisition != null && fixedAssetRequisition.FixedAssetRequisitionItem.Count == 0)
                    ModelState.AddModelError("requestedItem", "No Requested Item Found in Fixed Asset Requisition Please Check the Fixed Asset Requisition");

            //var exists = db.PurchaseRequisitions.Where(x => x.PurchaseRequisitionID != purchaseRequisition.PurchaseRequisitionID)
            //    .Where(x => x.ReferenceNo == purchaseRequisition.ReferenceNo).ToList();

            //if (exists.Any())
            //{
            //    ModelState.AddModelError("Reference", purchaseRequisition.ReferenceNo + " already exists in the system.");
            //    valid = false;
            //}

            if (purchaseRequisition.ProcurementOfficer == 0)
                ModelState.AddModelError("purchaser", " Please assign procurement officer.");

            if (purchaseRequisition.ProcurementMethod != ProcurementMethod.DirectPurchase && purchaseRequisition.FromPreviousPurchase)
                ModelState.AddModelError("prev", "From previous purchase is only for direct purchase.");

            if (purchaseRequisition.ProcurementMethod == ProcurementMethod.DirectPurchase && purchaseRequisition.FromPreviousPurchase)
                if (String.IsNullOrWhiteSpace(purchaseRequisition.PreviousPurchaseReference))
                    ModelState.AddModelError("prevreference", "Please select previous purchase reference.");

            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisition
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
                    ProcurementMethod = purchaseRequisition.ProcurementMethod,
                    PurchaseCategory = purchaseRequisition.PurchaseCategory,
                    ItemType = purchaseRequisition.ItemType,
                    FromPreviousPurchase = purchaseRequisition.FromPreviousPurchase,
                    PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
                    PreviousPurchaseReference = purchaseRequisition.PreviousPurchaseReference,
                    ModelNo = purchaseRequisition.ModelNo,
                    TenderType = purchaseRequisition.TenderType,
                    PlateNo = purchaseRequisition.PlateNo,

                    NextApprover = purchaseRequisition.NextApprover,
                    IsManual = purchaseRequisition.IsManual,

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

                    IsMiddleSendForApproval = purchaseRequisition.IsMiddleSendForApproval,
                    MiddleSendForApprovalTime = purchaseRequisition.MiddleSendForApprovalTime,
                    MiddleSendForApprovalBy = purchaseRequisition.MiddleSendForApprovalBy,

                    IsOnlineTransferred = purchaseRequisition.IsOnlineTransferred,
                    IsOnlineTransferredBy = purchaseRequisition.IsOnlineTransferredBy,
                    OnlineTransferredTime = purchaseRequisition.OnlineTransferredTime,

                    IsSecondOnlineApproved = purchaseRequisition.IsSecondOnlineApproved,
                    SecondOnlineApprovedBy = purchaseRequisition.SecondOnlineApprovedBy,
                    SecondOnlineApprovedTime = purchaseRequisition.SecondOnlineApprovedTime,

                    CreatedBy = purchaseRequisition.CreatedBy,
                    DateCreated = purchaseRequisition.DateCreated,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    OrgBranchName = purchaseRequisition.BranchId.ToString()
                };

                db.PurchaseRequisitions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition review", "Success", User.Identity.Name, $"updated purchase requisition {entity.PurchaseRequisitionID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionBudgets_Destroy([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisition
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
                    ProcurementMethod = purchaseRequisition.ProcurementMethod,
                    PurchaseCategory = purchaseRequisition.PurchaseCategory,
                    ModelNo = purchaseRequisition.ModelNo,
                    TenderType = purchaseRequisition.TenderType,
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
                    IsSecondOnlineApproved = purchaseRequisition.IsSecondOnlineApproved,
                    SecondOnlineApprovedBy = purchaseRequisition.SecondOnlineApprovedBy,
                    SecondOnlineApprovedTime = purchaseRequisition.SecondOnlineApprovedTime
                };

                db.PurchaseRequisitions.Attach(entity);
                db.PurchaseRequisitions.Remove(entity);
                db.SaveChanges();
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
                    Quantity = requestedItem.IssuedQuantity,
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

        [AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        public ActionResult ApproveBudget(int id)
        {
            var record = db.PurchaseRequisitions.Find(id);
            if (record != null && !record.IsBudgetApproved && record.PurchaseRequisitionItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 17);
                record.IsBudgetApproved = true;
                record.BudgetApprovedBy = User.Identity.Name;
                record.BudgetApprovedTime = DateTime.Now;
                db.PurchaseRequisitions.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Budget Approved",
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

            if (record.PurchaseRequisitionItems.Count == 0 && record.PurchaseRequisitionItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsBudgetApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved, you cannot approve again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }


        [AuthorizeRoles(BudgetRoles.BudgetVoid)]
        public JsonResult Void(int id)
        {
            var record = db.PurchaseRequisitions.Find(id);
            if (record != null && record.IsBudgetApproved && !record.IsOnlineApproved)
            {

                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsBudgetApproved = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                db.PurchaseRequisitions.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();


                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "PR budget Void", "Success", User.Identity.Name, $"void PR {id} ", Modules.Budget);
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

            if (!record.IsBudgetApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is approved at finance, it should be void there first.",
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