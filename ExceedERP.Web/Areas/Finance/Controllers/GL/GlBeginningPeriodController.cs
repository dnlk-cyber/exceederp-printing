using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlBeginningPeriodController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = _db.GlBeginningPeriods.Count();
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var periods = _db.GLPeriods;
            var periodSelected = periods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.GLPeriodId
                }).AsQueryable();

            ViewData["Periods"] = periodSelected;

            var fiscalYears = _db.GLFiscalYears;
            var yearsSelected = fiscalYears.Select(
                i => new
                {
                    Name = i.Name,
                    GlFiscalYearId = i.GlFiscalYearId
                }).AsQueryable();
            ViewData["Years"] = yearsSelected;

        }

        public ActionResult GlBeginningPeriods_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlBeginningPeriod> glbeginningperiods = _db.GlBeginningPeriods;
            DataSourceResult result = glbeginningperiods.ToDataSourceResult(request, glBeginningPeriod => new {
                GlBeginningPeriodId = glBeginningPeriod.GlBeginningPeriodId,
                GlFiscalYearId = glBeginningPeriod.GlFiscalYearId,
                GLPeriodId = glBeginningPeriod.GLPeriodId,
                DateCreated = glBeginningPeriod.DateCreated,
                LastModified = glBeginningPeriod.LastModified,
                CreatedBy = glBeginningPeriod.CreatedBy,
                ModifiedBy = glBeginningPeriod.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlBeginningPeriods_Create([DataSourceRequest]DataSourceRequest request, GlBeginningPeriod glBeginningPeriod)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlBeginningPeriod
                {
                    GlFiscalYearId = glBeginningPeriod.GlFiscalYearId,
                    GLPeriodId = glBeginningPeriod.GLPeriodId,
                    DateCreated = glBeginningPeriod.DateCreated,
                    LastModified = glBeginningPeriod.LastModified,
                    CreatedBy = glBeginningPeriod.CreatedBy,
                    ModifiedBy = glBeginningPeriod.ModifiedBy
                };

                _db.GlBeginningPeriods.Add(entity);
                _db.SaveChanges();
                glBeginningPeriod.GlBeginningPeriodId = entity.GlBeginningPeriodId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL Beginning period setup", "Success", User.Identity.Name, $"created beginning period {entity.GLPeriodId} under fiscal year {entity.GlFiscalYearId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { glBeginningPeriod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlBeginningPeriods_Update([DataSourceRequest]DataSourceRequest request, GlBeginningPeriod glBeginningPeriod)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlBeginningPeriod
                {
                    GlBeginningPeriodId = glBeginningPeriod.GlBeginningPeriodId,
                    GlFiscalYearId = glBeginningPeriod.GlFiscalYearId,
                    GLPeriodId = glBeginningPeriod.GLPeriodId,
                    DateCreated = glBeginningPeriod.DateCreated,
                    LastModified = glBeginningPeriod.LastModified,
                    CreatedBy = glBeginningPeriod.CreatedBy,
                    ModifiedBy = glBeginningPeriod.ModifiedBy
                };

                _db.GlBeginningPeriods.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL Beginning period setup", "Success", User.Identity.Name, $"updated beginning period {entity.GLPeriodId} under fiscal year {entity.GlFiscalYearId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { glBeginningPeriod }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
