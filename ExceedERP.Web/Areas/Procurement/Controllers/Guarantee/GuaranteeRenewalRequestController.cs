using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Guarantee;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Guarantee
{
    public class GuaranteeRenewalRequestController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GuaranteeRenewalRequests_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GuaranteeRenewalRequest> guaranteerenewalrequests = db.GuaranteeRenewalRequests.Where(x=>x.GuaranteeId==id);
            DataSourceResult result = guaranteerenewalrequests.ToDataSourceResult(request, guaranteeRenewalRequest => new {
                GuaranteeRenewalRequestId = guaranteeRenewalRequest.GuaranteeRenewalRequestId,
                Date = guaranteeRenewalRequest.Date,
                Remark = guaranteeRenewalRequest.Remark,
                OrgBranchName = guaranteeRenewalRequest.OrgBranchName,
                DateCreated = guaranteeRenewalRequest.DateCreated,
                LastModified = guaranteeRenewalRequest.LastModified,
                CreatedBy = guaranteeRenewalRequest.CreatedBy,
                ModifiedBy = guaranteeRenewalRequest.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeRenewalRequests_Create(int id,[DataSourceRequest]DataSourceRequest request, GuaranteeRenewalRequest guaranteeRenewalRequest)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeRenewalRequest
                {
                    GuaranteeId=id,
                    Date = guaranteeRenewalRequest.Date,
                    Remark = guaranteeRenewalRequest.Remark,
                    OrgBranchName = guaranteeRenewalRequest.OrgBranchName,
                    DateCreated = guaranteeRenewalRequest.DateCreated,
                    LastModified = guaranteeRenewalRequest.LastModified,
                    CreatedBy = guaranteeRenewalRequest.CreatedBy,
                    ModifiedBy = guaranteeRenewalRequest.ModifiedBy
                };

                db.GuaranteeRenewalRequests.Add(entity);
                db.SaveChanges();
                guaranteeRenewalRequest.GuaranteeRenewalRequestId = entity.GuaranteeRenewalRequestId;

                #region update parent

                var guarantee = db.Guarantees.Find(id);
                if (guarantee != null)
                {
                    guarantee.RenewalStatus = GuaranteeRenewalStatus.Requested;
                    db.Guarantees.Attach(guarantee);
                    db.Entry(guarantee).State = EntityState.Modified;
                }

                db.SaveChanges();

                #endregion;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "guarantee renewal request", "Success", User.Identity.Name, $"created guarantee renewal request {entity.GuaranteeRenewalRequestId} under record {entity.GuaranteeId} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { guaranteeRenewalRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeRenewalRequests_Update([DataSourceRequest]DataSourceRequest request, GuaranteeRenewalRequest guaranteeRenewalRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeRenewalRequest
                {
                    GuaranteeRenewalRequestId = guaranteeRenewalRequest.GuaranteeRenewalRequestId,
                    Date = guaranteeRenewalRequest.Date,
                    Remark = guaranteeRenewalRequest.Remark,
                    OrgBranchName = guaranteeRenewalRequest.OrgBranchName,
                    DateCreated = guaranteeRenewalRequest.DateCreated,
                    LastModified = guaranteeRenewalRequest.LastModified,
                    CreatedBy = guaranteeRenewalRequest.CreatedBy,
                    ModifiedBy = guaranteeRenewalRequest.ModifiedBy
                };

                db.GuaranteeRenewalRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "guarantee renewal request", "Success", User.Identity.Name, $"updated guarantee renewal request {entity.GuaranteeRenewalRequestId} under record {entity.GuaranteeId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { guaranteeRenewalRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeRenewalRequests_Destroy([DataSourceRequest]DataSourceRequest request, GuaranteeRenewalRequest guaranteeRenewalRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeRenewalRequest
                {
                    GuaranteeRenewalRequestId = guaranteeRenewalRequest.GuaranteeRenewalRequestId,
                    Date = guaranteeRenewalRequest.Date,
                    Remark = guaranteeRenewalRequest.Remark,
                    OrgBranchName = guaranteeRenewalRequest.OrgBranchName,
                    DateCreated = guaranteeRenewalRequest.DateCreated,
                    LastModified = guaranteeRenewalRequest.LastModified,
                    CreatedBy = guaranteeRenewalRequest.CreatedBy,
                    ModifiedBy = guaranteeRenewalRequest.ModifiedBy
                };

                db.GuaranteeRenewalRequests.Attach(entity);
                db.GuaranteeRenewalRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { guaranteeRenewalRequest }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
