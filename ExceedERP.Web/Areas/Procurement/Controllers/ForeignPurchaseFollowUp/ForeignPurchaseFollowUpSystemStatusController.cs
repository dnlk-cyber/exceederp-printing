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
    public class ForeignPurchaseFollowUpSystemStatusController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ForeignPurchaseFollowUpSystemStatus_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<ForeignPurchaseFollowUpSystemStatus> foreignpurchasefollowupsystemstatus = db.ForeignPurchaseFollowUpSystemStatus.Where(x=>x.ForeignPurchaseId==id);
            DataSourceResult result = foreignpurchasefollowupsystemstatus.ToDataSourceResult(request, foreignPurchaseFollowUpSystemStatus => new {
                ForeignPurchaseFollowUpSystemStatusId = foreignPurchaseFollowUpSystemStatus.ForeignPurchaseFollowUpSystemStatusId,
                Date = foreignPurchaseFollowUpSystemStatus.Date,
                Description = foreignPurchaseFollowUpSystemStatus.Description,
                User = foreignPurchaseFollowUpSystemStatus.User
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseFollowUpSystemStatus_Create([DataSourceRequest]DataSourceRequest request, ForeignPurchaseFollowUpSystemStatus foreignPurchaseFollowUpSystemStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseFollowUpSystemStatus
                {
                    Date = foreignPurchaseFollowUpSystemStatus.Date,
                    Description = foreignPurchaseFollowUpSystemStatus.Description,
                    User = foreignPurchaseFollowUpSystemStatus.User
                };

                db.ForeignPurchaseFollowUpSystemStatus.Add(entity);
                db.SaveChanges();
                foreignPurchaseFollowUpSystemStatus.ForeignPurchaseFollowUpSystemStatusId = entity.ForeignPurchaseFollowUpSystemStatusId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase follow up system status", "Success", User.Identity.Name, $"created status {entity.Description} under record {entity.ForeignPurchaseId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseFollowUpSystemStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseFollowUpSystemStatus_Update([DataSourceRequest]DataSourceRequest request, ForeignPurchaseFollowUpSystemStatus foreignPurchaseFollowUpSystemStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseFollowUpSystemStatus
                {
                    ForeignPurchaseFollowUpSystemStatusId = foreignPurchaseFollowUpSystemStatus.ForeignPurchaseFollowUpSystemStatusId,
                    Date = foreignPurchaseFollowUpSystemStatus.Date,
                    Description = foreignPurchaseFollowUpSystemStatus.Description,
                    User = foreignPurchaseFollowUpSystemStatus.User
                };

                db.ForeignPurchaseFollowUpSystemStatus.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase follow up system status", "Success", User.Identity.Name, $"updated status {entity.Description} under record {entity.ForeignPurchaseId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseFollowUpSystemStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseFollowUpSystemStatus_Destroy([DataSourceRequest]DataSourceRequest request, ForeignPurchaseFollowUpSystemStatus foreignPurchaseFollowUpSystemStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseFollowUpSystemStatus
                {
                    ForeignPurchaseFollowUpSystemStatusId = foreignPurchaseFollowUpSystemStatus.ForeignPurchaseFollowUpSystemStatusId,
                    Date = foreignPurchaseFollowUpSystemStatus.Date,
                    Description = foreignPurchaseFollowUpSystemStatus.Description,
                    User = foreignPurchaseFollowUpSystemStatus.User
                };

                db.ForeignPurchaseFollowUpSystemStatus.Attach(entity);
                db.ForeignPurchaseFollowUpSystemStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseFollowUpSystemStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
