using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
    public class FinanceDashboardRangeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FinanceDashboardRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FinanceDashboardRange> financedashboardranges = _db.FinanceDashboardRanges.Where(x=>x.FinanceDashboardId==id);
            DataSourceResult result = financedashboardranges.ToDataSourceResult(request, financeDashboardRange => new {
                FinanceDashboardRangeId = financeDashboardRange.FinanceDashboardRangeId,
                FinanceDashboardId = financeDashboardRange.FinanceDashboardId,
                AccountFrom = financeDashboardRange.AccountFrom,
                AccountTo = financeDashboardRange.AccountTo
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceDashboardRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, FinanceDashboardRange financeDashboardRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinanceDashboardRange
                {
                    FinanceDashboardId=id,
                    AccountFrom = financeDashboardRange.AccountFrom,
                    AccountTo = financeDashboardRange.AccountTo
                };

                _db.FinanceDashboardRanges.Add(entity);
                _db.SaveChanges();
                financeDashboardRange.FinanceDashboardRangeId = entity.FinanceDashboardRangeId;
            }

            return Json(new[] { financeDashboardRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceDashboardRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, FinanceDashboardRange financeDashboardRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinanceDashboardRange
                {
                    FinanceDashboardId = financeDashboardRange.FinanceDashboardId,
                    FinanceDashboardRangeId = financeDashboardRange.FinanceDashboardRangeId,
                    AccountFrom = financeDashboardRange.AccountFrom,
                    AccountTo = financeDashboardRange.AccountTo
                };

                _db.FinanceDashboardRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { financeDashboardRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceDashboardRanges_Destroy([DataSourceRequest]DataSourceRequest request, FinanceDashboardRange financeDashboardRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinanceDashboardRange
                {
                    FinanceDashboardRangeId = financeDashboardRange.FinanceDashboardRangeId,
                    AccountFrom = financeDashboardRange.AccountFrom,
                    AccountTo = financeDashboardRange.AccountTo
                };

                _db.FinanceDashboardRanges.Attach(entity);
                _db.FinanceDashboardRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted finance dashboard account range from {financeDashboardRange.AccountFrom} to {financeDashboardRange.AccountTo}");
                #endregion
            }

            return Json(new[] { financeDashboardRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
