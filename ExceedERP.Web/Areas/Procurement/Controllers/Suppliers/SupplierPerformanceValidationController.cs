using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
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
    public class SupplierPerformanceValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult SupplierPerformanceValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SupplierPerformanceValidation> supplierperformancevalidations = db.SupplierPerformanceValidations.Where(x => x.SupplierPerformanceId == id);
            DataSourceResult result = supplierperformancevalidations.ToDataSourceResult(request, supplierPerformanceValidation => new
            {
                SupplierPerformanceValidationId = supplierPerformanceValidation.SupplierPerformanceValidationId,
                SupplierPerformanceId = supplierPerformanceValidation.SupplierPerformanceId,
                Status = supplierPerformanceValidation.Status,
                Remark = supplierPerformanceValidation.Remark,
                OrgBranchName = supplierPerformanceValidation.OrgBranchName,
                DateCreated = supplierPerformanceValidation.DateCreated,
                LastModified = supplierPerformanceValidation.LastModified,
                CreatedBy = supplierPerformanceValidation.CreatedBy,
                ModifiedBy = supplierPerformanceValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierPerformanceValidations_Create(int id, [DataSourceRequest]DataSourceRequest request, SupplierPerformanceValidation supplierPerformanceValidation)
        {
            var record = db.SupplierPerformances.Find(id);
            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError("approved", "The record is already approved.");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (supplierPerformanceValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Supplier.IsBlackListed = false;
                        record.IsOnlineApproved = true;
                        record.Status = Enum.GetName(typeof(OperationStatuses), 1);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                    }
                    if (supplierPerformanceValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;

                    }
                    db.SupplierPerformances.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    var entity = new SupplierPerformanceValidation
                    {
                        SupplierPerformanceId = id,
                        Status = supplierPerformanceValidation.Status,
                        Remark = supplierPerformanceValidation.Remark,
                        OrgBranchName = supplierPerformanceValidation.OrgBranchName,
                        DateCreated = supplierPerformanceValidation.DateCreated,
                        LastModified = supplierPerformanceValidation.LastModified,
                        CreatedBy = supplierPerformanceValidation.CreatedBy,
                        ModifiedBy = supplierPerformanceValidation.ModifiedBy
                    };

                    db.SupplierPerformanceValidations.Add(entity);
                    db.SaveChanges();
                    supplierPerformanceValidation.SupplierPerformanceValidationId =
                        entity.SupplierPerformanceValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "supplier performance approval", "Success", User.Identity.Name, $"approved supplier performance {entity.SupplierPerformanceId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { supplierPerformanceValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierPerformanceValidations_Update(int id, [DataSourceRequest]DataSourceRequest request, SupplierPerformanceValidation supplierPerformanceValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierPerformanceValidation
                {
                    SupplierPerformanceId = id,
                    SupplierPerformanceValidationId = supplierPerformanceValidation.SupplierPerformanceValidationId,
                    Status = supplierPerformanceValidation.Status,
                    Remark = supplierPerformanceValidation.Remark,
                    OrgBranchName = supplierPerformanceValidation.OrgBranchName,
                    DateCreated = supplierPerformanceValidation.DateCreated,
                    LastModified = supplierPerformanceValidation.LastModified,
                    CreatedBy = supplierPerformanceValidation.CreatedBy,
                    ModifiedBy = supplierPerformanceValidation.ModifiedBy
                };

                db.SupplierPerformanceValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { supplierPerformanceValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierPerformanceValidations_Destroy([DataSourceRequest]DataSourceRequest request, SupplierPerformanceValidation supplierPerformanceValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierPerformanceValidation
                {
                    SupplierPerformanceValidationId = supplierPerformanceValidation.SupplierPerformanceValidationId,
                    Status = supplierPerformanceValidation.Status,
                    Remark = supplierPerformanceValidation.Remark,
                    OrgBranchName = supplierPerformanceValidation.OrgBranchName,
                    DateCreated = supplierPerformanceValidation.DateCreated,
                    LastModified = supplierPerformanceValidation.LastModified,
                    CreatedBy = supplierPerformanceValidation.CreatedBy,
                    ModifiedBy = supplierPerformanceValidation.ModifiedBy
                };

                db.SupplierPerformanceValidations.Attach(entity);
                db.SupplierPerformanceValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplierPerformanceValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
