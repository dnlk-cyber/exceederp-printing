using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class ServiceRequisitionSelfCareController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();


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


        public ActionResult PurchaseRequisitions_Read([DataSourceRequest] DataSourceRequest request)
        {
            int userEmpId = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);

            IQueryable<PurchaseRequisition> purchaserequisitions = db.PurchaseRequisitions.Where(x => x.ItemType == PurchaseItemType.Service);//.Where(x => mybraches.Contains(x.OrgStructureId));



            if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService)
                   && System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HRDepartmentHead))
            {

                purchaserequisitions = purchaserequisitions.Where(x => userworkunit.Contains(x.OrgStructureId)
                || x.OrgStructureId == 0//if they are not vis
                || x.ForwardedTo == User.Identity.Name
                || x.ForwardedFrom == User.Identity.Name
                || x.CreatedBy == User.Identity.Name
                || x.EmployeeId == userEmpId);

            }
            else if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService))
            {
                purchaserequisitions = purchaserequisitions
                    .Where(x => x.CreatedBy == User.Identity.Name
                    || x.EmployeeId == userEmpId);
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

                DateCreated = purchaseRequisition.DateCreated,
                CreatedBy = purchaseRequisition.CreatedBy,
                LastModified = purchaseRequisition.LastModified,
                ModifiedBy = purchaseRequisition.ModifiedBy,

                IsDepartmentOnlineApproved = purchaseRequisition.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = purchaseRequisition.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = purchaseRequisition.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = purchaseRequisition.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = purchaseRequisition.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = purchaseRequisition.DepartmentSendForApprovalTime,

                ForwardedFrom = purchaseRequisition.ForwardedFrom,
                ForwardedTo = purchaseRequisition.ForwardedTo,
                IsFinalApproval = purchaseRequisition.IsFinalApproval,
                CurrentApprovalStep = purchaseRequisition.CurrentApprovalStep,


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

                OrgBranchName = purchaseRequisition.OrgBranchName
            });

            return Json(result);
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitions_Create([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            #region validation 

            var storeRequisition = db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == purchaseRequisition.StoreRequisitionID);

            if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
                ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");

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

            if (purchaseRequisition.EmployeeId == 0)
                ModelState.AddModelError("requester", "Please choose requetster");


            #endregion

            #region data 
            if (purchaseRequisition.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (purchaseRequisition.EmployeeId != 0)
            {


                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == purchaseRequisition.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    purchaseRequisition.OrgStructureId = placement.OrgStructureId;
                    purchaseRequisition.BranchId = placement.BranchId;
                    purchaseRequisition.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }


            #endregion

            #region owner approval

            bool canOwnerApprove = DocApprovalHelper.CanOwnerApprove(ApprovalDocumentType.PurchaseRequisitionSelfCare);
            if (canOwnerApprove)
                purchaseRequisition.ForwardedTo = User.Identity.Name;

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisition
                {
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
                    ItemType = PurchaseItemType.Service,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    NextApprover = purchaseRequisition.NextApprover,
                    ForBranchId = purchaseRequisition.ForBranchId,
                    PreviousProcurementMethod = purchaseRequisition.PreviousProcurementMethod,
                    TenderType = purchaseRequisition.TenderType,

                    Remark = purchaseRequisition.Remark,
                    IsVoid = purchaseRequisition.IsVoid,
                    VoidBy = purchaseRequisition.VoidBy,
                    VoidTime = purchaseRequisition.VoidTime,

                    IsDepartmentOnlineApproved = purchaseRequisition.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = purchaseRequisition.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = purchaseRequisition.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = purchaseRequisition.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = purchaseRequisition.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = purchaseRequisition.DepartmentSendForApprovalTime,

                    ForwardedFrom = purchaseRequisition.ForwardedFrom,
                    ForwardedTo = purchaseRequisition.ForwardedTo,
                    IsFinalApproval = purchaseRequisition.IsFinalApproval,
                    CurrentApprovalStep = purchaseRequisition.CurrentApprovalStep,

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
                UserHelper.OperationLog(Request.UserHostAddress, "service requisition self care", "Success", User.Identity.Name, $"created service requisition {entity.PurchaseRequisitionID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult PurchaseRequisitions_Update([DataSourceRequest] DataSourceRequest request, PurchaseRequisition purchaseRequisition)
        {
            #region validation 

            var storeRequisition = db.StoreRequisitions.FirstOrDefault(sr => sr.StoreRequisitionID == purchaseRequisition.StoreRequisitionID);

            if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count == 0)
                ModelState.AddModelError("requestedItem", "No Requested Item Found in Store Requisition Please Check the Store Requisition");
            if (!String.IsNullOrEmpty(purchaseRequisition.ReferenceNo))
            {
                var exists = db.PurchaseRequisitions
                    .Where(x => x.PurchaseRequisitionID != purchaseRequisition.PurchaseRequisitionID)
                    .Where(x => x.ReferenceNo == purchaseRequisition.ReferenceNo).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference",
                        purchaseRequisition.ReferenceNo + " is already exists in the system.");
            }

            if (purchaseRequisition.RequestDate.Date > DateTime.Today)
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");

            if (purchaseRequisition.EmployeeId == 0)
                ModelState.AddModelError("requester", "Please choose requetster");

            //if (purchaseRequisition.BranchId == 0)
            //    ModelState.AddModelError("branch", "Please choose branch");

            #endregion

            #region data 
            if (purchaseRequisition.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (purchaseRequisition.EmployeeId != 0)
            {


                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == purchaseRequisition.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    purchaseRequisition.OrgStructureId = placement.OrgStructureId;
                    purchaseRequisition.BranchId = placement.BranchId;
                    purchaseRequisition.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisition
                {
                    StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                    BranchId = purchaseRequisition.BranchId,
                    BusinessUnitId = purchaseRequisition.BusinessUnitId,
                    OrgStructureId = purchaseRequisition.OrgStructureId,
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

                    Status = purchaseRequisition.Status,
                    Remark = purchaseRequisition.Remark,
                    IsVoid = purchaseRequisition.IsVoid,
                    VoidBy = purchaseRequisition.VoidBy,
                    VoidTime = purchaseRequisition.VoidTime,

                    IsDepartmentOnlineApproved = purchaseRequisition.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = purchaseRequisition.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = purchaseRequisition.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = purchaseRequisition.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = purchaseRequisition.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = purchaseRequisition.DepartmentSendForApprovalTime,

                    ForwardedFrom = purchaseRequisition.ForwardedFrom,
                    ForwardedTo = purchaseRequisition.ForwardedTo,
                    IsFinalApproval = purchaseRequisition.IsFinalApproval,
                    CurrentApprovalStep = purchaseRequisition.CurrentApprovalStep,

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
                UserHelper.OperationLog(Request.UserHostAddress, "service requisition self care", "Success", User.Identity.Name, $"updated service requisition {entity.PurchaseRequisitionID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisition }.ToDataSourceResult(request, ModelState));
        }


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

                    IsDepartmentOnlineApproved = purchaseRequisition.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = purchaseRequisition.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = purchaseRequisition.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = purchaseRequisition.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = purchaseRequisition.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = purchaseRequisition.DepartmentSendForApprovalTime,

                    ForwardedFrom = purchaseRequisition.ForwardedFrom,
                    ForwardedTo = purchaseRequisition.ForwardedTo,
                    IsFinalApproval = purchaseRequisition.IsFinalApproval,
                    CurrentApprovalStep = purchaseRequisition.CurrentApprovalStep,
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



        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = db.PurchaseRequisitions.Find(id);
            string errMsg = "";

            if (record != null && record.IsDepartmentOnlineApproved)
            {
                errMsg = "You cannot manipulate this record,it is already approved";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsDepartmentSendForApproval)
            {
                errMsg = "This record is already sent for department approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var lines = db.PurchaseRequisitionItems.Where(x => x.PurchaseRequisitionID == id);
            if (!lines.Any())
            {
                errMsg = "Please add items.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (record != null && !record.IsDepartmentOnlineApproved && !record.IsDepartmentSendForApproval)
            {

                record.Status = Enum.GetName(typeof(OperationStatuses), 16);
                record.IsDepartmentSendForApproval = true;
                record.DepartmentSendForApprovalBy = User.Identity.Name;
                record.DepartmentSendForApprovalTime = DateTime.Now;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                #region approval workflow

                bool isNewRecord = true;
                var validations = record.PurchaseRequisitionUserDepartmentValidation.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = 0;
                var items = db.PurchaseRequisitionItems.Where(x => x.PurchaseRequisitionID == record.PurchaseRequisitionID).ToList();
                if (items.Any())
                    docamount = items.Count;//no price
                int docid = record.PurchaseRequisitionID;
                int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.PurchaseRequisitionSelfCare, isNewRecord, step);
                #endregion

                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }
            return null;
        }


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