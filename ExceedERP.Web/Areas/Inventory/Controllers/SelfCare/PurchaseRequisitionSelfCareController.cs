using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.SelfCare
{
    public class PurchaseRequisitionSelfCareController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        private readonly ApplicationDbContext userdb = new ApplicationDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]
        public ActionResult Index()
        {
            PopulateViewData();
            ViewBag.User = User.Identity.Name;
            return View();
        }
        private void PopulateViewData()
        {
            //var categories = db.ServiceCategories;
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

        [AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]
        public ActionResult PurchaseRequisitions_Read([DataSourceRequest] DataSourceRequest request)
        {
             List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //var xb = System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.PropertyRequistion);
            var user = userdb.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
           // var storekeepers = db.StoreKeeperAssignments.Where(x => x.StoreKeeper == user.Id).ToList();
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);

            var purchaserequisitions = db.PurchaseRequisitions.Where(x => x.ItemType == PurchaseItemType.Stock && x.IsManual).ToList(); 
            if (System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.UserDepartment))
            {
                if (userworkunit.Any())
                {
                    purchaserequisitions = purchaserequisitions.Where(x => userworkunit.Contains(x.OrgStructureId) && userworkunit != null
                                                                     || x.CreatedBy == User.Identity.Name).ToList();
                }
                else
                {
                    purchaserequisitions = purchaserequisitions.Where(x => x.CreatedBy == User.Identity.Name).ToList();
                }
            }
            else if (System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.PropertyRequistion))
            {
                purchaserequisitions = purchaserequisitions.Where(x => x.CreatedBy == User.Identity.Name).ToList();
            }           
            

            var result = purchaserequisitions.ToDataSourceResult(request, purchaseRequisition => new
            {
                PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                BranchId = purchaseRequisition.BranchId,
                BusinessUnitId = purchaseRequisition.BusinessUnitId,
                OrgStructureId = purchaseRequisition.OrgStructureId,
                EmployeeId = purchaseRequisition.EmployeeId,
                ReferenceNo = purchaseRequisition.ReferenceNo,
                ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                RequestDate = purchaseRequisition.RequestDate,
                ProcurementMethod = purchaseRequisition.ProcurementMethod,
                PurchaseCategory = purchaseRequisition.PurchaseCategory,
                ItemType = purchaseRequisition.ItemType,
                GLPeriodId = purchaseRequisition.GLPeriodId,
                NextApprover = purchaseRequisition.NextApprover,
                Status = purchaseRequisition.Status,
                ForBranchId = purchaseRequisition.ForBranchId,
                PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
                TenderType = purchaseRequisition.TenderType,
                IsManual = purchaseRequisition.IsManual,
                PlateNo = purchaseRequisition.PlateNo,

                DateCreated = purchaseRequisition.DateCreated,
                CreatedBy = purchaseRequisition.CreatedBy,
                LastModified = purchaseRequisition.LastModified,
                ModifiedBy = purchaseRequisition.ModifiedBy,

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


        [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitions_Create([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            #region validation 

            var storeRequisition = db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == purchaseRequisition.StoreRequisitionID);

            if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
                ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");
            if (!String.IsNullOrEmpty(purchaseRequisition.ReferenceNo))
            {

                var exists = db.PurchaseRequisitions
                .Where(x => x.ReferenceNo == purchaseRequisition.ReferenceNo).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", purchaseRequisition.ReferenceNo + " is already exists in the system.");
            }

            if (purchaseRequisition.RequestDate.Date > DateTime.Today)
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");

            //if (purchaseRequisition.EmployeeId == 0)
            //    ModelState.AddModelError("requester", "Please choose requetster");

            //if (purchaseRequisition.BranchId == 0)
            //    ModelState.AddModelError("requester", "Please choose branch");

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisition
                {
                    StoreRequisitionID = 0,
                    BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
                    BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                    OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(purchaseRequisition.EmployeeId),
                    EmployeeId = purchaseRequisition.EmployeeId,// UserInformationHelper.GetEmployeeId(User.Identity.Name),
                    ReferenceNo = purchaseRequisition.ReferenceNo,
                    ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                    RequestDate = purchaseRequisition.RequestDate.Date,
                    ProcurementMethod = purchaseRequisition.ProcurementMethod,
                    PurchaseCategory = purchaseRequisition.PurchaseCategory,
                    ItemType = PurchaseItemType.Stock,
                    IsManual = true,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    NextApprover = purchaseRequisition.NextApprover,
                    ForBranchId = purchaseRequisition.ForBranchId,
                    PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
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
                    LastModified = purchaseRequisition.LastModified,
                    ModifiedBy = purchaseRequisition.ModifiedBy,
                    OrgBranchName = purchaseRequisition.BranchId.ToString(),
                };

                db.PurchaseRequisitions.Add(entity);
                db.SaveChanges();
                purchaseRequisition.PurchaseRequisitionID = entity.PurchaseRequisitionID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "self care purchase requisition", "Success", User.Identity.Name, $"created purchase requisition {entity.PurchaseRequisitionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        public ActionResult PurchaseRequisitions_Update([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            #region validation 

            var storeRequisition = db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == purchaseRequisition.StoreRequisitionID);

            if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
                ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");

            var exists = db.PurchaseRequisitions
                .Where(x => x.PurchaseRequisitionID != purchaseRequisition.PurchaseRequisitionID)
                .Where(x => x.ReferenceNo == purchaseRequisition.ReferenceNo).ToList();

            if (exists.Any())
                ModelState.AddModelError("Reference", purchaseRequisition.ReferenceNo + " is already exists in the system.");

            if (purchaseRequisition.RequestDate.Date > DateTime.Today)
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");

            //if (purchaseRequisition.EmployeeId == 0)
            //    ModelState.AddModelError("requester", "Please choose requetster");

            //if (purchaseRequisition.BranchId == 0)
            //    ModelState.AddModelError("requester", "Please choose branch");

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisition
                {
                    StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                    BranchId = purchaseRequisition.BranchId,
                    BusinessUnitId = purchaseRequisition.BusinessUnitId,
                    OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(purchaseRequisition.EmployeeId),
                    EmployeeId = purchaseRequisition.EmployeeId,
                    PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                    ReferenceNo = purchaseRequisition.ReferenceNo,
                    ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                    RequestDate = purchaseRequisition.RequestDate,
                    ProcurementMethod = purchaseRequisition.ProcurementMethod,
                    PurchaseCategory = purchaseRequisition.PurchaseCategory,
                    ItemType = purchaseRequisition.ItemType,
                    NextApprover = purchaseRequisition.NextApprover,
                    ForBranchId = purchaseRequisition.ForBranchId,
                    PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
                    TenderType = purchaseRequisition.TenderType,
                    IsManual = purchaseRequisition.IsManual,
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
                    DateCreated = purchaseRequisition.DateCreated,
                    CreatedBy = purchaseRequisition.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    OrgBranchName = purchaseRequisition.BranchId.ToString(),
                };

                db.PurchaseRequisitions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "self care purchase requisition", "Success", User.Identity.Name, $"updated purchase requisition {entity.PurchaseRequisitionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
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
                    OrgStructureId = purchaseRequisition.OrgStructureId,
                    EmployeeId = purchaseRequisition.EmployeeId,
                    ReferenceNo = purchaseRequisition.ReferenceNo,
                    ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                    RequestDate = purchaseRequisition.RequestDate,
                    ProcurementMethod = purchaseRequisition.ProcurementMethod,
                    PurchaseCategory = purchaseRequisition.PurchaseCategory,
                    ItemType = purchaseRequisition.ItemType,
                    NextApprover = purchaseRequisition.NextApprover,
                    ForBranchId = purchaseRequisition.ForBranchId,
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


        [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
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
                UserHelper.OperationLog(Request.UserHostAddress, "self care purchase requisition", "Success", User.Identity.Name, $"send for approval purchase requisition {id} ", Modules.Inventory);
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

        [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        public JsonResult SendForSecondApproval(int id, string user)
        {
            var record = db.PurchaseRequisitions.Find(id);
            if (record != null && !record.IsMiddleSendForApproval && !record.IsMiddleOnlineApproved && record.PurchaseRequisitionItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 13);
                record.NextApprover = user;
                record.IsMiddleSendForApproval = true;
                record.MiddleSendForApprovalBy = User.Identity.Name;
                record.MiddleSendForApprovalTime = DateTime.Now;
                db.PurchaseRequisitions.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "self care purchase requisition", "Success", User.Identity.Name, $"send for second approval purchase requisition {id} ", Modules.Inventory);
                #endregion

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

            if (record.PurchaseRequisitionItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
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

            if (record.IsMiddleOnlineApproved)
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