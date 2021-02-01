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
    public class GuaranteeRenewalController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementBondUser, ProcurementConstants.ProcurementBondApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementBondUser, ProcurementConstants.ProcurementBondApprover)]
        public ActionResult GuaranteeRenewals_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<GuaranteeRenewal> guaranteerenewals = db.GuaranteeRenewals.Where(x => x.GuaranteeId == id);
            DataSourceResult result = guaranteerenewals.ToDataSourceResult(request, guaranteeRenewal => new
            {
                GuaranteeRenewalId = guaranteeRenewal.GuaranteeRenewalId,
                GuaranteeId = guaranteeRenewal.GuaranteeId,
                Date = guaranteeRenewal.Date,
                ValidityPeriod = guaranteeRenewal.ValidityPeriod,
                GuaranteeRefNo = guaranteeRenewal.GuaranteeRefNo,
                Remark = guaranteeRenewal.Remark,
                OrgBranchName = guaranteeRenewal.OrgBranchName,
                DateCreated = guaranteeRenewal.DateCreated,
                LastModified = guaranteeRenewal.LastModified,
                CreatedBy = guaranteeRenewal.CreatedBy,
                ModifiedBy = guaranteeRenewal.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementBondUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeRenewals_Create(int id, [DataSourceRequest]DataSourceRequest request, GuaranteeRenewal guaranteeRenewal)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeRenewal
                {
                    GuaranteeId = id,
                    Date = guaranteeRenewal.Date.Date,
                    ValidityPeriod = guaranteeRenewal.ValidityPeriod,
                    GuaranteeRefNo = guaranteeRenewal.GuaranteeRefNo,
                    Remark = guaranteeRenewal.Remark,
                    OrgBranchName = guaranteeRenewal.OrgBranchName,
                    DateCreated = guaranteeRenewal.DateCreated,
                    LastModified = guaranteeRenewal.LastModified,
                    CreatedBy = guaranteeRenewal.CreatedBy,
                    ModifiedBy = guaranteeRenewal.ModifiedBy
                };

                db.GuaranteeRenewals.Add(entity);
                db.SaveChanges();
                guaranteeRenewal.GuaranteeRenewalId = entity.GuaranteeRenewalId;
                #region update parent

                var guarantee = db.Guarantees.Find(id);
                if (guarantee != null)
                {
                    guarantee.RenewalStatus = GuaranteeRenewalStatus.Renewed;
                    db.Guarantees.Attach(guarantee);
                    db.Entry(guarantee).State = EntityState.Modified;
                }

                db.SaveChanges();

                #endregion;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "guarantee renewal", "Success", User.Identity.Name, $"created guarantee renewal {entity.GuaranteeId} under record {entity.GuaranteeId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { guaranteeRenewal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementBondUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeRenewals_Update(int id, [DataSourceRequest]DataSourceRequest request, GuaranteeRenewal guaranteeRenewal)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeRenewal
                {
                    GuaranteeId = id,
                    GuaranteeRenewalId = guaranteeRenewal.GuaranteeRenewalId,
                    Date = guaranteeRenewal.Date,
                    ValidityPeriod = guaranteeRenewal.ValidityPeriod,
                    GuaranteeRefNo = guaranteeRenewal.GuaranteeRefNo,
                    Remark = guaranteeRenewal.Remark,
                    OrgBranchName = guaranteeRenewal.OrgBranchName,
                    DateCreated = guaranteeRenewal.DateCreated,
                    LastModified = guaranteeRenewal.LastModified,
                    CreatedBy = guaranteeRenewal.CreatedBy,
                    ModifiedBy = guaranteeRenewal.ModifiedBy
                };

                db.GuaranteeRenewals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "guarantee renewal", "Success", User.Identity.Name, $"updated guarantee renewal {entity.GuaranteeId} under record {entity.GuaranteeId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { guaranteeRenewal }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementBondUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeRenewals_Destroy([DataSourceRequest]DataSourceRequest request, GuaranteeRenewal guaranteeRenewal)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeRenewal
                {
                    GuaranteeRenewalId = guaranteeRenewal.GuaranteeRenewalId,
                    Date = guaranteeRenewal.Date,
                    ValidityPeriod = guaranteeRenewal.ValidityPeriod,
                    GuaranteeRefNo = guaranteeRenewal.GuaranteeRefNo,
                    Remark = guaranteeRenewal.Remark,
                    OrgBranchName = guaranteeRenewal.OrgBranchName,
                    DateCreated = guaranteeRenewal.DateCreated,
                    LastModified = guaranteeRenewal.LastModified,
                    CreatedBy = guaranteeRenewal.CreatedBy,
                    ModifiedBy = guaranteeRenewal.ModifiedBy
                };

                db.GuaranteeRenewals.Attach(entity);
                db.GuaranteeRenewals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { guaranteeRenewal }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
