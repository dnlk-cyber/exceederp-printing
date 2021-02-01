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
    public class ForeignPurchaseCostValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover, ProcurementConstants.ProcurementForeignPurchaseApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover, FinanceConstants.AccountPayableView, ProcurementConstants.ProcurementForeignPurchaseUser)]
        public ActionResult ForeignPurchaseCostValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ForeignPurchaseCostValidation> foreignPurchaseCostValidations = _db.ForeignPurchaseCostValidations.Where(x=>x.ForeignPurchaseCostId==id);
            DataSourceResult result = foreignPurchaseCostValidations.ToDataSourceResult(request, foreignPurchaseCostValidation => new {
                ForeignPurchaseCostValidationId = foreignPurchaseCostValidation.ForeignPurchaseCostValidationId,
                Status = foreignPurchaseCostValidation.Status,
                Remark = foreignPurchaseCostValidation.Remark,
                OrgBranchName = foreignPurchaseCostValidation.OrgBranchName,
                DateCreated = foreignPurchaseCostValidation.DateCreated,
                LastModified = foreignPurchaseCostValidation.LastModified,
                CreatedBy = foreignPurchaseCostValidation.CreatedBy,
                ModifiedBy = foreignPurchaseCostValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseCostValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ForeignPurchaseCostValidation foreignPurchaseCostValidation)
        {
            var record = _db.ForeignPurchaseCosts.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (foreignPurchaseCostValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.Status = "Approved";
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.ForeignPurchaseCosts.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (foreignPurchaseCostValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.ForeignPurchaseCosts.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                   
                var entity = new ForeignPurchaseCostValidation
                {
                    ForeignPurchaseCostId = id,
                    Status = foreignPurchaseCostValidation.Status,
                    Remark = foreignPurchaseCostValidation.Remark,
                    OrgBranchName = foreignPurchaseCostValidation.OrgBranchName,
                    DateCreated = foreignPurchaseCostValidation.DateCreated,
                    LastModified = foreignPurchaseCostValidation.LastModified,
                    CreatedBy = foreignPurchaseCostValidation.CreatedBy,
                    ModifiedBy = foreignPurchaseCostValidation.ModifiedBy
                };

                _db.ForeignPurchaseCostValidations.Add(entity);
                _db.SaveChanges();
                foreignPurchaseCostValidation.ForeignPurchaseCostValidationId = entity.ForeignPurchaseCostValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "foreign purchase cost approval", "Success", User.Identity.Name, $"approved cost {entity.ForeignPurchaseCostId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { foreignPurchaseCostValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseCostValidations_Update([DataSourceRequest]DataSourceRequest request, ForeignPurchaseCostValidation foreignPurchaseCostValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseCostValidation
                {
                    ForeignPurchaseCostValidationId = foreignPurchaseCostValidation.ForeignPurchaseCostValidationId,
                    Status = foreignPurchaseCostValidation.Status,
                    Remark = foreignPurchaseCostValidation.Remark,
                    OrgBranchName = foreignPurchaseCostValidation.OrgBranchName,
                    DateCreated = foreignPurchaseCostValidation.DateCreated,
                    LastModified = foreignPurchaseCostValidation.LastModified,
                    CreatedBy = foreignPurchaseCostValidation.CreatedBy,
                    ModifiedBy = foreignPurchaseCostValidation.ModifiedBy
                };

                _db.ForeignPurchaseCostValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseCostValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseCostValidations_Destroy([DataSourceRequest]DataSourceRequest request, ForeignPurchaseCostValidation foreignPurchaseCostValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseCostValidation
                {
                    ForeignPurchaseCostValidationId = foreignPurchaseCostValidation.ForeignPurchaseCostValidationId,
                    Status = foreignPurchaseCostValidation.Status,
                    Remark = foreignPurchaseCostValidation.Remark,
                    OrgBranchName = foreignPurchaseCostValidation.OrgBranchName,
                    DateCreated = foreignPurchaseCostValidation.DateCreated,
                    LastModified = foreignPurchaseCostValidation.LastModified,
                    CreatedBy = foreignPurchaseCostValidation.CreatedBy,
                    ModifiedBy = foreignPurchaseCostValidation.ModifiedBy
                };

                _db.ForeignPurchaseCostValidations.Attach(entity);
                _db.ForeignPurchaseCostValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseCostValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
