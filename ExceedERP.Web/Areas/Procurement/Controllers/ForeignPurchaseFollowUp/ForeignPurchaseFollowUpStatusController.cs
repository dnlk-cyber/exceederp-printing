using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.ForeignPurchaseFollowUp
{
    public class ForeignPurchaseFollowUpStatusController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ForeignPurchaseFollowUpStatus_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ForeignPurchaseFollowUpStatus> foreignpurchasefollowupstatus = db.ForeignPurchaseFollowUpStatus.Where(x => x.ForeignPurchaseId == id);
            DataSourceResult result = foreignpurchasefollowupstatus.ToDataSourceResult(request, foreignPurchaseFollowUpStatus => new
            {
                ForeignPurchaseFollowUpStatusId = foreignPurchaseFollowUpStatus.ForeignPurchaseFollowUpStatusId,
                Date = foreignPurchaseFollowUpStatus.Date,
                Description = foreignPurchaseFollowUpStatus.Description,
                Remark = foreignPurchaseFollowUpStatus.Remark,
                DateCreated = foreignPurchaseFollowUpStatus.DateCreated,
                LastModified = foreignPurchaseFollowUpStatus.LastModified,
                CreatedBy = foreignPurchaseFollowUpStatus.CreatedBy,
                ModifiedBy = foreignPurchaseFollowUpStatus.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseFollowUpStatus_Create(int id,[DataSourceRequest]DataSourceRequest request, ForeignPurchaseFollowUpStatus foreignPurchaseFollowUpStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseFollowUpStatus
                {
                    ForeignPurchaseId = id,
                    Date = foreignPurchaseFollowUpStatus.Date,
                    Description = foreignPurchaseFollowUpStatus.Description,
                    Remark = foreignPurchaseFollowUpStatus.Remark,
                    DateCreated = foreignPurchaseFollowUpStatus.DateCreated,
                    LastModified = foreignPurchaseFollowUpStatus.LastModified,
                    CreatedBy = foreignPurchaseFollowUpStatus.CreatedBy,
                    ModifiedBy = foreignPurchaseFollowUpStatus.ModifiedBy
                };

                db.ForeignPurchaseFollowUpStatus.Add(entity);
                db.SaveChanges();
                foreignPurchaseFollowUpStatus.ForeignPurchaseFollowUpStatusId = entity.ForeignPurchaseFollowUpStatusId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase follow up status", "Success", User.Identity.Name, $"created item {entity.Description} under record {entity.ForeignPurchaseId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseFollowUpStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseFollowUpStatus_Update([DataSourceRequest]DataSourceRequest request, ForeignPurchaseFollowUpStatus foreignPurchaseFollowUpStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseFollowUpStatus
                {
                    ForeignPurchaseFollowUpStatusId = foreignPurchaseFollowUpStatus.ForeignPurchaseFollowUpStatusId,
                    ForeignPurchaseId = foreignPurchaseFollowUpStatus.ForeignPurchaseId,
                    Date = foreignPurchaseFollowUpStatus.Date,
                    Description = foreignPurchaseFollowUpStatus.Description,
                    Remark = foreignPurchaseFollowUpStatus.Remark,
                    DateCreated = foreignPurchaseFollowUpStatus.DateCreated,
                    LastModified = foreignPurchaseFollowUpStatus.LastModified,
                    CreatedBy = foreignPurchaseFollowUpStatus.CreatedBy,
                    ModifiedBy = foreignPurchaseFollowUpStatus.ModifiedBy
                };

                db.ForeignPurchaseFollowUpStatus.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase follow up status", "Success", User.Identity.Name, $"updated item {entity.Description} under record {entity.ForeignPurchaseId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseFollowUpStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseFollowUpStatus_Destroy([DataSourceRequest]DataSourceRequest request, ForeignPurchaseFollowUpStatus foreignPurchaseFollowUpStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseFollowUpStatus
                {
                    ForeignPurchaseFollowUpStatusId = foreignPurchaseFollowUpStatus.ForeignPurchaseFollowUpStatusId,
                    ForeignPurchaseId = foreignPurchaseFollowUpStatus.ForeignPurchaseId,
                    Date = foreignPurchaseFollowUpStatus.Date,
                    Description = foreignPurchaseFollowUpStatus.Description,
                    Remark = foreignPurchaseFollowUpStatus.Remark,
                    DateCreated = foreignPurchaseFollowUpStatus.DateCreated,
                    LastModified = foreignPurchaseFollowUpStatus.LastModified,
                    CreatedBy = foreignPurchaseFollowUpStatus.CreatedBy,
                    ModifiedBy = foreignPurchaseFollowUpStatus.ModifiedBy
                };

                db.ForeignPurchaseFollowUpStatus.Attach(entity);
                db.ForeignPurchaseFollowUpStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseFollowUpStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
