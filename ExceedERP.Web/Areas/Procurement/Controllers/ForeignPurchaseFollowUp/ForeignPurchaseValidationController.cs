using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.ForeignPurchaseFollowUp
{
    public class ForeignPurchaseValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover, ProcurementConstants.ProcurementForeignPurchaseApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover, FinanceConstants.AccountPayableView, ProcurementConstants.ProcurementForeignPurchaseUser)]
        public ActionResult ForeignPurchaseValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ForeignPurchaseValidation> foreignPurchaseValidations = _db.ForeignPurchaseValidations.Where(x=>x.ForeignPurchaseId==id);
            DataSourceResult result = foreignPurchaseValidations.ToDataSourceResult(request, foreignPurchaseValidation => new {
                ForeignPurchaseValidationId = foreignPurchaseValidation.ForeignPurchaseValidationId,
                Status = foreignPurchaseValidation.Status,
                Remark = foreignPurchaseValidation.Remark,
                OrgBranchName = foreignPurchaseValidation.OrgBranchName,
                DateCreated = foreignPurchaseValidation.DateCreated,
                LastModified = foreignPurchaseValidation.LastModified,
                CreatedBy = foreignPurchaseValidation.CreatedBy,
                ModifiedBy = foreignPurchaseValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ForeignPurchaseValidation foreignPurchaseValidation)
        {
            var record = _db.ForeignPurchases.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (foreignPurchaseValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.Status = "Approved";
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.ForeignPurchases.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (foreignPurchaseValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.ForeignPurchases.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                   
                var entity = new ForeignPurchaseValidation
                {
                    ForeignPurchaseId = id,
                    Status = foreignPurchaseValidation.Status,
                    Remark = foreignPurchaseValidation.Remark,
                    OrgBranchName = foreignPurchaseValidation.OrgBranchName,
                    DateCreated = foreignPurchaseValidation.DateCreated,
                    LastModified = foreignPurchaseValidation.LastModified,
                    CreatedBy = foreignPurchaseValidation.CreatedBy,
                    ModifiedBy = foreignPurchaseValidation.ModifiedBy
                };

                _db.ForeignPurchaseValidations.Add(entity);
                _db.SaveChanges();
                foreignPurchaseValidation.ForeignPurchaseValidationId = entity.ForeignPurchaseValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "foreign purchase approval", "Success", User.Identity.Name, $"approved foreign purchase {entity.ForeignPurchaseId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { foreignPurchaseValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseValidations_Update([DataSourceRequest]DataSourceRequest request, ForeignPurchaseValidation foreignPurchaseValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseValidation
                {
                    ForeignPurchaseValidationId = foreignPurchaseValidation.ForeignPurchaseValidationId,
                    Status = foreignPurchaseValidation.Status,
                    Remark = foreignPurchaseValidation.Remark,
                    OrgBranchName = foreignPurchaseValidation.OrgBranchName,
                    DateCreated = foreignPurchaseValidation.DateCreated,
                    LastModified = foreignPurchaseValidation.LastModified,
                    CreatedBy = foreignPurchaseValidation.CreatedBy,
                    ModifiedBy = foreignPurchaseValidation.ModifiedBy
                };

                _db.ForeignPurchaseValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseValidations_Destroy([DataSourceRequest]DataSourceRequest request, ForeignPurchaseValidation foreignPurchaseValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseValidation
                {
                    ForeignPurchaseValidationId = foreignPurchaseValidation.ForeignPurchaseValidationId,
                    Status = foreignPurchaseValidation.Status,
                    Remark = foreignPurchaseValidation.Remark,
                    OrgBranchName = foreignPurchaseValidation.OrgBranchName,
                    DateCreated = foreignPurchaseValidation.DateCreated,
                    LastModified = foreignPurchaseValidation.LastModified,
                    CreatedBy = foreignPurchaseValidation.CreatedBy,
                    ModifiedBy = foreignPurchaseValidation.ModifiedBy
                };

                _db.ForeignPurchaseValidations.Attach(entity);
                _db.ForeignPurchaseValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
