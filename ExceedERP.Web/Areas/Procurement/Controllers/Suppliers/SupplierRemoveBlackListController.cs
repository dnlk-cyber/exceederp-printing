using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Suppliers
{
    public class SupplierRemoveBlackListController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser, ProcurementConstants.ProcurementSupplierApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser, ProcurementConstants.ProcurementSupplierApprover)]
        public ActionResult SupplierRemoveBlackLists_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<SupplierRemoveBlackList> supplierremoveblacklists = db.SupplierRemoveBlackLists.Where(x => x.ProcurementSupplierID == id);
            DataSourceResult result = supplierremoveblacklists.ToDataSourceResult(request, supplierRemoveBlackList => new {
                ProcurementSupplierID = supplierRemoveBlackList.ProcurementSupplierID,
                SupplierRemoveBlackListId = supplierRemoveBlackList.SupplierRemoveBlackListId,
                Description = supplierRemoveBlackList.Description,
                
                ClearedOn = supplierRemoveBlackList.ClearedOn,
                Remark = supplierRemoveBlackList.Remark,
                Status = supplierRemoveBlackList.Status,
                IsOnlineApproved = supplierRemoveBlackList.IsOnlineApproved,
                OnlineApprovedBy = supplierRemoveBlackList.OnlineApprovedBy,
                OnlineApprovedTime = supplierRemoveBlackList.OnlineApprovedTime,
                IsSendForApproval = supplierRemoveBlackList.IsSendForApproval,
                SendForApprovalBy = supplierRemoveBlackList.SendForApprovalBy,
                SendForApprovalTime = supplierRemoveBlackList.SendForApprovalTime,
                isVoid = supplierRemoveBlackList.isVoid,
                VoidBy = supplierRemoveBlackList.VoidBy,
                VoidTime = supplierRemoveBlackList.VoidTime,
                OrgBranchName = supplierRemoveBlackList.OrgBranchName,
                DateCreated = supplierRemoveBlackList.DateCreated,
                LastModified = supplierRemoveBlackList.LastModified,
                CreatedBy = supplierRemoveBlackList.CreatedBy,
                ModifiedBy = supplierRemoveBlackList.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierRemoveBlackLists_Create(int id,[DataSourceRequest]DataSourceRequest request, SupplierRemoveBlackList supplierRemoveBlackList)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierRemoveBlackList
                {
                    ProcurementSupplierID = id,
                    Description = supplierRemoveBlackList.Description,
                    ClearedOn = supplierRemoveBlackList.ClearedOn,
                    Remark = supplierRemoveBlackList.Remark,
                    Status = supplierRemoveBlackList.Status,
                    IsOnlineApproved = supplierRemoveBlackList.IsOnlineApproved,
                    OnlineApprovedBy = supplierRemoveBlackList.OnlineApprovedBy,
                    OnlineApprovedTime = supplierRemoveBlackList.OnlineApprovedTime,
                    isVoid = supplierRemoveBlackList.isVoid,
                    VoidBy = supplierRemoveBlackList.VoidBy,
                    VoidTime = supplierRemoveBlackList.VoidTime,
                    OrgBranchName = supplierRemoveBlackList.OrgBranchName,
                    DateCreated = supplierRemoveBlackList.DateCreated,
                    LastModified = supplierRemoveBlackList.LastModified,
                    CreatedBy = supplierRemoveBlackList.CreatedBy,
                    ModifiedBy = supplierRemoveBlackList.ModifiedBy
                };

                db.SupplierRemoveBlackLists.Add(entity);
                db.SaveChanges();
                supplierRemoveBlackList.SupplierRemoveBlackListId = entity.SupplierRemoveBlackListId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier remove from black list ", "Success", User.Identity.Name, $"created supplier black list removal for {entity.Supplier.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { supplierRemoveBlackList }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierRemoveBlackLists_Update(int id,[DataSourceRequest]DataSourceRequest request, SupplierRemoveBlackList supplierRemoveBlackList)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierRemoveBlackList
                {
                    ProcurementSupplierID = id,
                    SupplierRemoveBlackListId = supplierRemoveBlackList.SupplierRemoveBlackListId,
                    Description = supplierRemoveBlackList.Description,
                    
                    ClearedOn = supplierRemoveBlackList.ClearedOn,
                    Remark = supplierRemoveBlackList.Remark,
                    Status = supplierRemoveBlackList.Status,
                    IsOnlineApproved = supplierRemoveBlackList.IsOnlineApproved,
                    OnlineApprovedBy = supplierRemoveBlackList.OnlineApprovedBy,
                    OnlineApprovedTime = supplierRemoveBlackList.OnlineApprovedTime,
                    isVoid = supplierRemoveBlackList.isVoid,
                    VoidBy = supplierRemoveBlackList.VoidBy,
                    VoidTime = supplierRemoveBlackList.VoidTime,
                    OrgBranchName = supplierRemoveBlackList.OrgBranchName,
                    DateCreated = supplierRemoveBlackList.DateCreated,
                    LastModified = supplierRemoveBlackList.LastModified,
                    CreatedBy = supplierRemoveBlackList.CreatedBy,
                    ModifiedBy = supplierRemoveBlackList.ModifiedBy
                };

                db.SupplierRemoveBlackLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier remove from black list ", "Success", User.Identity.Name, $"updated supplier black list removal for {entity.Supplier.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { supplierRemoveBlackList }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierRemoveBlackLists_Destroy([DataSourceRequest]DataSourceRequest request, SupplierRemoveBlackList supplierRemoveBlackList)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierRemoveBlackList
                {
                    SupplierRemoveBlackListId = supplierRemoveBlackList.SupplierRemoveBlackListId,
                    Description = supplierRemoveBlackList.Description,
                    
                    ClearedOn = supplierRemoveBlackList.ClearedOn,
                    Remark = supplierRemoveBlackList.Remark,
                    Status = supplierRemoveBlackList.Status,
                    IsOnlineApproved = supplierRemoveBlackList.IsOnlineApproved,
                    OnlineApprovedBy = supplierRemoveBlackList.OnlineApprovedBy,
                    OnlineApprovedTime = supplierRemoveBlackList.OnlineApprovedTime,
                    isVoid = supplierRemoveBlackList.isVoid,
                    VoidBy = supplierRemoveBlackList.VoidBy,
                    VoidTime = supplierRemoveBlackList.VoidTime,
                    OrgBranchName = supplierRemoveBlackList.OrgBranchName,
                    DateCreated = supplierRemoveBlackList.DateCreated,
                    LastModified = supplierRemoveBlackList.LastModified,
                    CreatedBy = supplierRemoveBlackList.CreatedBy,
                    ModifiedBy = supplierRemoveBlackList.ModifiedBy
                };

                db.SupplierRemoveBlackLists.Attach(entity);
                db.SupplierRemoveBlackLists.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplierRemoveBlackList }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult SendForApproval(int id)
        {
            var record = db.SupplierRemoveBlackLists.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.SupplierRemoveBlackLists.Attach(record);
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
