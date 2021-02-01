using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting.GAAP
{
    public class GaapCashFlowController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = _db.GaapCashFlows.Count();
            return View();
        }

        public ActionResult GaapCashFlows_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapCashFlow> gaapCashFlows = _db.GaapCashFlows;
            DataSourceResult result = gaapCashFlows.ToDataSourceResult(request, gaapCashFlow => new {
                GaapCashFlowId = gaapCashFlow.GaapCashFlowId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlows_Create([DataSourceRequest]DataSourceRequest request, GaapCashFlow gaapCashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlow
                {
                };

                _db.GaapCashFlows.Add(entity);
                _db.SaveChanges();
                gaapCashFlow.GaapCashFlowId = entity.GaapCashFlowId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup cash flow ", "Success", User.Identity.Name, $"created rule {entity.GaapCashFlowId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapCashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlows_Update([DataSourceRequest]DataSourceRequest request, GaapCashFlow gaapCashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlow
                {
                    GaapCashFlowId = gaapCashFlow.GaapCashFlowId,
                };

                _db.GaapCashFlows.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup cash flow ", "Success", User.Identity.Name, $"updated rule {entity.GaapCashFlowId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapCashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlows_Destroy([DataSourceRequest]DataSourceRequest request, GaapCashFlow gaapCashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlow
                {
                    GaapCashFlowId = gaapCashFlow.GaapCashFlowId,
                };

                _db.GaapCashFlows.Attach(entity);
                _db.GaapCashFlows.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap cashflow {gaapCashFlow.GaapCashFlowId}");
                #endregion
            }

            return Json(new[] { gaapCashFlow }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
