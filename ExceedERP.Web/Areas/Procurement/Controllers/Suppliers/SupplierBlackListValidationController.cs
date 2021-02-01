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
    public class SupplierBlackListValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly DashboardHelper _dashboardHelper = new DashboardHelper();

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser, ProcurementConstants.ProcurementSupplierApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierUser, ProcurementConstants.ProcurementSupplierApprover)]
        public ActionResult SupplierBlackListValidations_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<SupplierBlackListValidation> supplierblacklistvalidations = db.SupplierBlackListValidations.Where(x=>x.SupplierBlackListId==id);
            DataSourceResult result = supplierblacklistvalidations.ToDataSourceResult(request, supplierBlackListValidation => new {
                SupplierBlackListValidationId = supplierBlackListValidation.SupplierBlackListValidationId,
                SupplierBlackListId = supplierBlackListValidation.SupplierBlackListId,
                Status = supplierBlackListValidation.Status,
                Remark = supplierBlackListValidation.Remark,
                OrgBranchName = supplierBlackListValidation.OrgBranchName,
                DateCreated = supplierBlackListValidation.DateCreated,
                LastModified = supplierBlackListValidation.LastModified,
                CreatedBy = supplierBlackListValidation.CreatedBy,
                ModifiedBy = supplierBlackListValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierBlackListValidations_Create([DataSourceRequest]DataSourceRequest request, SupplierBlackListValidation supplierBlackListValidation,int id)
        {
            var record = db.SupplierBlackLists.Find(id);
            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError("approved","The record is already approved.");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (supplierBlackListValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Supplier.IsBlackListed = true;
                        record.IsOnlineApproved = true;
                        record.Status = Enum.GetName(typeof(OperationStatuses), 1);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.SupplierBlackLists.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                        _dashboardHelper.AddNotofication(Request.UserHostAddress, "", "", record.DateBlackListed, "", "Supplier  : " + record.Supplier + " is black listed.");
                    }
                    if (supplierBlackListValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.SupplierBlackLists.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                   
                    var entity = new SupplierBlackListValidation
                    {
                        SupplierBlackListId = id,
                        Status = supplierBlackListValidation.Status,
                        Remark = supplierBlackListValidation.Remark,
                        OrgBranchName = supplierBlackListValidation.OrgBranchName,
                        DateCreated = supplierBlackListValidation.DateCreated,
                        LastModified = supplierBlackListValidation.LastModified,
                        CreatedBy = supplierBlackListValidation.CreatedBy,
                        ModifiedBy = supplierBlackListValidation.ModifiedBy
                    };

                    db.SupplierBlackListValidations.Add(entity);
                    db.SaveChanges();
                    supplierBlackListValidation.SupplierBlackListValidationId = entity.SupplierBlackListValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "supplier black list approval", "Success", User.Identity.Name, $"approved black list {entity.SupplierBlackListId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { supplierBlackListValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierBlackListValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, SupplierBlackListValidation supplierBlackListValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierBlackListValidation
                {
                    SupplierBlackListId = id,
                    SupplierBlackListValidationId = supplierBlackListValidation.SupplierBlackListValidationId,
                    Status = supplierBlackListValidation.Status,
                    Remark = supplierBlackListValidation.Remark,
                    OrgBranchName = supplierBlackListValidation.OrgBranchName,
                    DateCreated = supplierBlackListValidation.DateCreated,
                    LastModified = supplierBlackListValidation.LastModified,
                    CreatedBy = supplierBlackListValidation.CreatedBy,
                    ModifiedBy = supplierBlackListValidation.ModifiedBy
                };

                db.SupplierBlackListValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { supplierBlackListValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementSupplierApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupplierBlackListValidations_Destroy([DataSourceRequest]DataSourceRequest request, SupplierBlackListValidation supplierBlackListValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SupplierBlackListValidation
                {
                    SupplierBlackListValidationId = supplierBlackListValidation.SupplierBlackListValidationId,
                    Status = supplierBlackListValidation.Status,
                    Remark = supplierBlackListValidation.Remark,
                    OrgBranchName = supplierBlackListValidation.OrgBranchName,
                    DateCreated = supplierBlackListValidation.DateCreated,
                    LastModified = supplierBlackListValidation.LastModified,
                    CreatedBy = supplierBlackListValidation.CreatedBy,
                    ModifiedBy = supplierBlackListValidation.ModifiedBy
                };

                db.SupplierBlackListValidations.Attach(entity);
                db.SupplierBlackListValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplierBlackListValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
