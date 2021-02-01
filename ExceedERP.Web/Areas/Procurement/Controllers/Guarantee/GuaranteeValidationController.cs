using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Guarantee;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Guarantee
{
    public class GuaranteeValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementBondUser, ProcurementConstants.ProcurementBondApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementBondUser, ProcurementConstants.ProcurementBondApprover)]
        public ActionResult GuaranteeValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<GuaranteeValidation> guaranteevalidations = db.GuaranteeValidations.Where(x => x.GuaranteeId == id);
            DataSourceResult result = guaranteevalidations.ToDataSourceResult(request, guaranteeValidation => new
            {
                GuaranteeValidationId = guaranteeValidation.GuaranteeValidationId,
                GuaranteeId = guaranteeValidation.GuaranteeId,
                Status = guaranteeValidation.Status,
                Remark = guaranteeValidation.Remark,
                OrgBranchName = guaranteeValidation.OrgBranchName,
                DateCreated = guaranteeValidation.DateCreated,
                LastModified = guaranteeValidation.LastModified,
                CreatedBy = guaranteeValidation.CreatedBy,
                ModifiedBy = guaranteeValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementBondApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeValidations_Create(int id, [DataSourceRequest]DataSourceRequest request, GuaranteeValidation guaranteeValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeValidation
                {
                    GuaranteeId = id,
                    Status = guaranteeValidation.Status,
                    Remark = guaranteeValidation.Remark,
                    OrgBranchName = guaranteeValidation.OrgBranchName,
                    DateCreated = guaranteeValidation.DateCreated,
                    LastModified = guaranteeValidation.LastModified,
                    CreatedBy = guaranteeValidation.CreatedBy,
                    ModifiedBy = guaranteeValidation.ModifiedBy
                };

                db.GuaranteeValidations.Add(entity);
                db.SaveChanges();
                guaranteeValidation.GuaranteeValidationId = entity.GuaranteeValidationId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "guarantee approval", "Success", User.Identity.Name, $"approved guarantee {entity.GuaranteeId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { guaranteeValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementBondApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeValidations_Update(int id, [DataSourceRequest]DataSourceRequest request, GuaranteeValidation guaranteeValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeValidation
                {
                    GuaranteeId = id,
                    GuaranteeValidationId = guaranteeValidation.GuaranteeValidationId,
                    Status = guaranteeValidation.Status,
                    Remark = guaranteeValidation.Remark,
                    OrgBranchName = guaranteeValidation.OrgBranchName,
                    DateCreated = guaranteeValidation.DateCreated,
                    LastModified = guaranteeValidation.LastModified,
                    CreatedBy = guaranteeValidation.CreatedBy,
                    ModifiedBy = guaranteeValidation.ModifiedBy
                };

                db.GuaranteeValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { guaranteeValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementBondApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeValidations_Destroy([DataSourceRequest]DataSourceRequest request, GuaranteeValidation guaranteeValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeValidation
                {
                    GuaranteeValidationId = guaranteeValidation.GuaranteeValidationId,
                    Status = guaranteeValidation.Status,
                    Remark = guaranteeValidation.Remark,
                    OrgBranchName = guaranteeValidation.OrgBranchName,
                    DateCreated = guaranteeValidation.DateCreated,
                    LastModified = guaranteeValidation.LastModified,
                    CreatedBy = guaranteeValidation.CreatedBy,
                    ModifiedBy = guaranteeValidation.ModifiedBy
                };

                db.GuaranteeValidations.Attach(entity);
                db.GuaranteeValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { guaranteeValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
