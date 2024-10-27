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
    public class SupplierPerformanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult SupplierPerformances_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SupplierPerformance> supplierperformances = db.SupplierPerformances.Where(x => x.ProcurementSupplierID == id);
            DataSourceResult result = supplierperformances.ToDataSourceResult(request, supplierPerformance => new
            {
                ProcurementSupplierID = supplierPerformance.ProcurementSupplierID,
                SupplierPerformanceId = supplierPerformance.SupplierPerformanceId,
                ProfurementRef = supplierPerformance.ProfurementRef,
                Quality = supplierPerformance.Quality,
                PlannedDeliveryPeriod = supplierPerformance.PlannedDeliveryPeriod,
                ActualDeliveryPeriod = supplierPerformance.ActualDeliveryPeriod,
                Difference = supplierPerformance.Difference,
                Remark = supplierPerformance.Remark,
                Status = supplierPerformance.Status,
                IsOnlineApproved = supplierPerformance.IsOnlineApproved,
                OnlineApprovedBy = supplierPerformance.OnlineApprovedBy,
                OnlineApprovedTime = supplierPerformance.OnlineApprovedTime,
                IsSendForApproval = supplierPerformance.IsSendForApproval,
                SendForApprovalBy = supplierPerformance.SendForApprovalBy,
                SendForApprovalTime = supplierPerformance.SendForApprovalTime,
                isVoid = supplierPerformance.isVoid,
                VoidBy = supplierPerformance.VoidBy,
                VoidTime = supplierPerformance.VoidTime,
                OrgBranchName = supplierPerformance.OrgBranchName,
                DateCreated = supplierPerformance.DateCreated,
                LastModified = supplierPerformance.LastModified,
                CreatedBy = supplierPerformance.CreatedBy,
                ModifiedBy = supplierPerformance.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierPerformances_Create(int id, [DataSourceRequest]DataSourceRequest request, SupplierPerformance supplierPerformance)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierPerformance
                {
                    ProcurementSupplierID = id,
                    ProfurementRef = supplierPerformance.ProfurementRef,
                    Quality = supplierPerformance.Quality,
                    PlannedDeliveryPeriod = supplierPerformance.PlannedDeliveryPeriod,
                    ActualDeliveryPeriod = supplierPerformance.ActualDeliveryPeriod,
                    Difference = supplierPerformance.ActualDeliveryPeriod - supplierPerformance.ActualDeliveryPeriod,
                    Remark = supplierPerformance.Remark,
                    Status = supplierPerformance.Status,
                    IsOnlineApproved = supplierPerformance.IsOnlineApproved,
                    OnlineApprovedBy = supplierPerformance.OnlineApprovedBy,
                    OnlineApprovedTime = supplierPerformance.OnlineApprovedTime,
                    isVoid = supplierPerformance.isVoid,
                    VoidBy = supplierPerformance.VoidBy,
                    VoidTime = supplierPerformance.VoidTime,
                    OrgBranchName = supplierPerformance.OrgBranchName,
                    DateCreated = supplierPerformance.DateCreated,
                    LastModified = supplierPerformance.LastModified,
                    CreatedBy = supplierPerformance.CreatedBy,
                    ModifiedBy = supplierPerformance.ModifiedBy
                };

                db.SupplierPerformances.Add(entity);
                db.SaveChanges();
                supplierPerformance.SupplierPerformanceId = entity.SupplierPerformanceId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier performance ", "Success", User.Identity.Name, $"created supplier performance for {entity.ProcurementSupplierID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { supplierPerformance }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierPerformances_Update(int id, [DataSourceRequest]DataSourceRequest request, SupplierPerformance supplierPerformance)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierPerformance
                {
                    ProcurementSupplierID = id,
                    SupplierPerformanceId = supplierPerformance.SupplierPerformanceId,
                    ProfurementRef = supplierPerformance.ProfurementRef,
                    Quality = supplierPerformance.Quality,
                    PlannedDeliveryPeriod = supplierPerformance.PlannedDeliveryPeriod,
                    ActualDeliveryPeriod = supplierPerformance.ActualDeliveryPeriod,
                    Difference = supplierPerformance.ActualDeliveryPeriod - supplierPerformance.ActualDeliveryPeriod,
                    Remark = supplierPerformance.Remark,
                    Status = supplierPerformance.Status,
                    IsOnlineApproved = supplierPerformance.IsOnlineApproved,
                    OnlineApprovedBy = supplierPerformance.OnlineApprovedBy,
                    OnlineApprovedTime = supplierPerformance.OnlineApprovedTime,
                    isVoid = supplierPerformance.isVoid,
                    VoidBy = supplierPerformance.VoidBy,
                    VoidTime = supplierPerformance.VoidTime,
                    OrgBranchName = supplierPerformance.OrgBranchName,
                    DateCreated = supplierPerformance.DateCreated,
                    LastModified = supplierPerformance.LastModified,
                    CreatedBy = supplierPerformance.CreatedBy,
                    ModifiedBy = supplierPerformance.ModifiedBy
                };

                db.SupplierPerformances.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier performance ", "Success", User.Identity.Name, $"updated supplier performance for {entity.ProcurementSupplierID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { supplierPerformance }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierPerformances_Destroy([DataSourceRequest]DataSourceRequest request, SupplierPerformance supplierPerformance)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierPerformance
                {
                    SupplierPerformanceId = supplierPerformance.SupplierPerformanceId,
                    ProfurementRef = supplierPerformance.ProfurementRef,
                    Quality = supplierPerformance.Quality,
                    PlannedDeliveryPeriod = supplierPerformance.PlannedDeliveryPeriod,
                    ActualDeliveryPeriod = supplierPerformance.ActualDeliveryPeriod,
                    Difference = supplierPerformance.Difference,
                    Remark = supplierPerformance.Remark,
                    Status = supplierPerformance.Status,
                    IsOnlineApproved = supplierPerformance.IsOnlineApproved,
                    OnlineApprovedBy = supplierPerformance.OnlineApprovedBy,
                    OnlineApprovedTime = supplierPerformance.OnlineApprovedTime,
                    isVoid = supplierPerformance.isVoid,
                    VoidBy = supplierPerformance.VoidBy,
                    VoidTime = supplierPerformance.VoidTime,
                    OrgBranchName = supplierPerformance.OrgBranchName,
                    DateCreated = supplierPerformance.DateCreated,
                    LastModified = supplierPerformance.LastModified,
                    CreatedBy = supplierPerformance.CreatedBy,
                    ModifiedBy = supplierPerformance.ModifiedBy
                };

                db.SupplierPerformances.Attach(entity);
                db.SupplierPerformances.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplierPerformance }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult SendForApproval(int id)
        {
            var record = db.SupplierPerformances.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.SupplierPerformances.Attach(record);
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
