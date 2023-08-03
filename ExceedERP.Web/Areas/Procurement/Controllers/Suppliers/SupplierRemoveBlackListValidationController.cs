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
    public class SupplierRemoveBlackListValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly DashboardHelper _dashboardHelper = new DashboardHelper();

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierUser, ProcurementRoles.ProcurementSupplierApprover)]
        public ActionResult SupplierRemoveBlackListValidations_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<SupplierRemoveBlackListValidation> supplierremoveblacklistvalidations = db.SupplierRemoveBlackListValidations.Where(x=>x.SupplierRemoveBlackListId==id);
            DataSourceResult result = supplierremoveblacklistvalidations.ToDataSourceResult(request, supplierRemoveBlackListValidation => new {
                SupplierRemoveBlackListValidationId = supplierRemoveBlackListValidation.SupplierRemoveBlackListValidationId,
                SupplierRemoveBlackListId = supplierRemoveBlackListValidation.SupplierRemoveBlackListId,
                Status = supplierRemoveBlackListValidation.Status,
                Remark = supplierRemoveBlackListValidation.Remark,
                OrgBranchName = supplierRemoveBlackListValidation.OrgBranchName,
                DateCreated = supplierRemoveBlackListValidation.DateCreated,
                LastModified = supplierRemoveBlackListValidation.LastModified,
                CreatedBy = supplierRemoveBlackListValidation.CreatedBy,
                ModifiedBy = supplierRemoveBlackListValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierRemoveBlackListValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, SupplierRemoveBlackListValidation supplierRemoveBlackListValidation)
        {
            var record = db.SupplierRemoveBlackLists.Find(id);
            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError("approved", "The record is already approved.");
            }
            if (ModelState.IsValid)
            {
                
                if (record != null)
                {
                    if (supplierRemoveBlackListValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Supplier.IsBlackListed = false;
                        record.IsOnlineApproved = true;
                        record.Status = Enum.GetName(typeof(OperationStatuses), 1);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        db.SupplierRemoveBlackLists.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                        _dashboardHelper.AddNotofication(Request.UserHostAddress, "", "", record.ClearedOn, "", "Supplier  : " + record.Supplier + " is cleared from black list on " + record.ClearedOn);
                    }
                    if (supplierRemoveBlackListValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        db.SupplierRemoveBlackLists.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   
                    var entity = new SupplierRemoveBlackListValidation
                    {
                        SupplierRemoveBlackListId = id,
                        Status = supplierRemoveBlackListValidation.Status,
                        Remark = supplierRemoveBlackListValidation.Remark,
                        OrgBranchName = supplierRemoveBlackListValidation.OrgBranchName,
                        DateCreated = supplierRemoveBlackListValidation.DateCreated,
                        LastModified = supplierRemoveBlackListValidation.LastModified,
                        CreatedBy = supplierRemoveBlackListValidation.CreatedBy,
                        ModifiedBy = supplierRemoveBlackListValidation.ModifiedBy
                    };

                    db.SupplierRemoveBlackListValidations.Add(entity);
                    db.SaveChanges();
                    supplierRemoveBlackListValidation.SupplierRemoveBlackListValidationId =
                        entity.SupplierRemoveBlackListValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "supplier black list remove approval", "Success", User.Identity.Name, $"approved supplier black lsit remove {entity.SupplierRemoveBlackListId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { supplierRemoveBlackListValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierRemoveBlackListValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, SupplierRemoveBlackListValidation supplierRemoveBlackListValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierRemoveBlackListValidation
                {
                    SupplierRemoveBlackListId = id,
                    SupplierRemoveBlackListValidationId = supplierRemoveBlackListValidation.SupplierRemoveBlackListValidationId,
                    Status = supplierRemoveBlackListValidation.Status,
                    Remark = supplierRemoveBlackListValidation.Remark,
                    OrgBranchName = supplierRemoveBlackListValidation.OrgBranchName,
                    DateCreated = supplierRemoveBlackListValidation.DateCreated,
                    LastModified = supplierRemoveBlackListValidation.LastModified,
                    CreatedBy = supplierRemoveBlackListValidation.CreatedBy,
                    ModifiedBy = supplierRemoveBlackListValidation.ModifiedBy
                };

                db.SupplierRemoveBlackListValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { supplierRemoveBlackListValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementSupplierApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierRemoveBlackListValidations_Destroy([DataSourceRequest]DataSourceRequest request, SupplierRemoveBlackListValidation supplierRemoveBlackListValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierRemoveBlackListValidation
                {
                    SupplierRemoveBlackListValidationId = supplierRemoveBlackListValidation.SupplierRemoveBlackListValidationId,
                    Status = supplierRemoveBlackListValidation.Status,
                    Remark = supplierRemoveBlackListValidation.Remark,
                    OrgBranchName = supplierRemoveBlackListValidation.OrgBranchName,
                    DateCreated = supplierRemoveBlackListValidation.DateCreated,
                    LastModified = supplierRemoveBlackListValidation.LastModified,
                    CreatedBy = supplierRemoveBlackListValidation.CreatedBy,
                    ModifiedBy = supplierRemoveBlackListValidation.ModifiedBy
                };

                db.SupplierRemoveBlackListValidations.Attach(entity);
                db.SupplierRemoveBlackListValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplierRemoveBlackListValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
