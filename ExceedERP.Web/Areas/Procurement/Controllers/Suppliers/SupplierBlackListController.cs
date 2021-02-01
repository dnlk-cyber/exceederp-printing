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
    public class SupplierBlackListController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser, ProcurementConstants.ProcurementSupplierApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser, ProcurementConstants.ProcurementSupplierApprover)]
        public ActionResult SupplierBlackLists_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SupplierBlackList> supplierblacklists = db.SupplierBlackLists.Where(x => x.ProcurementSupplierID == id);
            DataSourceResult result = supplierblacklists.ToDataSourceResult(request, supplierBlackList => new
            {
                ProcurementSupplierID = supplierBlackList.ProcurementSupplierID,
                SupplierBlackListId = supplierBlackList.SupplierBlackListId,
                Reason = supplierBlackList.Reason,
                BlackListType = supplierBlackList.BlackListType,
                DurationDays = supplierBlackList.DurationDays,
                DateBlackListed = supplierBlackList.DateBlackListed,
                DateBlackListedExpire = supplierBlackList.DateBlackListedExpire,
                Remark = supplierBlackList.Remark,
                Status = supplierBlackList.Status,
                IsOnlineApproved = supplierBlackList.IsOnlineApproved,
                OnlineApprovedBy = supplierBlackList.OnlineApprovedBy,
                OnlineApprovedTime = supplierBlackList.OnlineApprovedTime,
                IsSendForApproval = supplierBlackList.IsSendForApproval,
                SendForApprovalBy = supplierBlackList.SendForApprovalBy,
                SendForApprovalTime = supplierBlackList.SendForApprovalTime,
                isVoid = supplierBlackList.isVoid,
                VoidBy = supplierBlackList.VoidBy,
                VoidTime = supplierBlackList.VoidTime,
                OrgBranchName = supplierBlackList.OrgBranchName,
                DateCreated = supplierBlackList.DateCreated,
                LastModified = supplierBlackList.LastModified,
                CreatedBy = supplierBlackList.CreatedBy,
                ModifiedBy = supplierBlackList.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierBlackLists_Create(int id, [DataSourceRequest]DataSourceRequest request, SupplierBlackList supplierBlackList)
        {
            if (supplierBlackList.BlackListType == BlackListType.Bounded)
            {

            }
            if (ModelState.IsValid)
            {
                var entity = new SupplierBlackList
                {
                    ProcurementSupplierID = id,
                    Reason = supplierBlackList.Reason,
                    BlackListType = supplierBlackList.BlackListType,
                    DurationDays = supplierBlackList.DurationDays,
                    DateBlackListed = supplierBlackList.DateBlackListed,
                    DateBlackListedExpire = supplierBlackList.DateBlackListedExpire.AddDays(supplierBlackList.DurationDays),
                    Remark = supplierBlackList.Remark,
                    Status = supplierBlackList.Status,
                    IsOnlineApproved = supplierBlackList.IsOnlineApproved,
                    OnlineApprovedBy = supplierBlackList.OnlineApprovedBy,
                    OnlineApprovedTime = supplierBlackList.OnlineApprovedTime,
                    isVoid = supplierBlackList.isVoid,
                    VoidBy = supplierBlackList.VoidBy,
                    VoidTime = supplierBlackList.VoidTime,
                    OrgBranchName = supplierBlackList.OrgBranchName,
                    DateCreated = supplierBlackList.DateCreated,
                    LastModified = supplierBlackList.LastModified,
                    CreatedBy = supplierBlackList.CreatedBy,
                    ModifiedBy = supplierBlackList.ModifiedBy
                };

                db.SupplierBlackLists.Add(entity);
                db.SaveChanges();
                supplierBlackList.SupplierBlackListId = entity.SupplierBlackListId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier black list", "Success", User.Identity.Name, $"created supplier black list {entity.Supplier.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { supplierBlackList }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierBlackLists_Update(int id, [DataSourceRequest]DataSourceRequest request, SupplierBlackList supplierBlackList)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierBlackList
                {
                    ProcurementSupplierID = id,
                    SupplierBlackListId = supplierBlackList.SupplierBlackListId,
                    Reason = supplierBlackList.Reason,
                    BlackListType = supplierBlackList.BlackListType,
                    DurationDays = supplierBlackList.DurationDays,
                    DateBlackListed = supplierBlackList.DateBlackListed,
                    DateBlackListedExpire = supplierBlackList.DateBlackListedExpire.AddDays(supplierBlackList.DurationDays),
                    Remark = supplierBlackList.Remark,
                    Status = supplierBlackList.Status,
                    IsOnlineApproved = supplierBlackList.IsOnlineApproved,
                    OnlineApprovedBy = supplierBlackList.OnlineApprovedBy,
                    OnlineApprovedTime = supplierBlackList.OnlineApprovedTime,
                    isVoid = supplierBlackList.isVoid,
                    VoidBy = supplierBlackList.VoidBy,
                    VoidTime = supplierBlackList.VoidTime,
                    OrgBranchName = supplierBlackList.OrgBranchName,
                    DateCreated = supplierBlackList.DateCreated,
                    LastModified = supplierBlackList.LastModified,
                    CreatedBy = supplierBlackList.CreatedBy,
                    ModifiedBy = supplierBlackList.ModifiedBy
                };

                db.SupplierBlackLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier black list", "Success", User.Identity.Name, $"updated supplier black list {entity.Supplier.Name}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { supplierBlackList }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierBlackLists_Destroy([DataSourceRequest]DataSourceRequest request, SupplierBlackList supplierBlackList)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierBlackList
                {
                    SupplierBlackListId = supplierBlackList.SupplierBlackListId,
                    Reason = supplierBlackList.Reason,
                    BlackListType = supplierBlackList.BlackListType,
                    DurationDays = supplierBlackList.DurationDays,
                    DateBlackListed = supplierBlackList.DateBlackListed,
                    DateBlackListedExpire = supplierBlackList.DateBlackListedExpire,
                    Remark = supplierBlackList.Remark,
                    Status = supplierBlackList.Status,
                    IsOnlineApproved = supplierBlackList.IsOnlineApproved,
                    OnlineApprovedBy = supplierBlackList.OnlineApprovedBy,
                    OnlineApprovedTime = supplierBlackList.OnlineApprovedTime,
                    isVoid = supplierBlackList.isVoid,
                    VoidBy = supplierBlackList.VoidBy,
                    VoidTime = supplierBlackList.VoidTime,
                    OrgBranchName = supplierBlackList.OrgBranchName,
                    DateCreated = supplierBlackList.DateCreated,
                    LastModified = supplierBlackList.LastModified,
                    CreatedBy = supplierBlackList.CreatedBy,
                    ModifiedBy = supplierBlackList.ModifiedBy
                };

                db.SupplierBlackLists.Attach(entity);
                db.SupplierBlackLists.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplierBlackList }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.SupplierBlackLists.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.SupplierBlackLists.Attach(record);
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
