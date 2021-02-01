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
    public class GaapIncomeStatementOperationRevenueController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapIncomeStatementOperationRevenues_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapIncomeStatementOperationRevenue> gaapincomestatementoperationrevenues = db.GaapIncomeStatementOperationRevenues.Where(x=>x.GaapIncomeStatementId==id);
            DataSourceResult result = gaapincomestatementoperationrevenues.ToDataSourceResult(request, gaapIncomeStatementOperationRevenue => new {
                GaapIncomeStatementId = gaapIncomeStatementOperationRevenue.GaapIncomeStatementId,
                GaapIncomeStatementOperationRevenueId = gaapIncomeStatementOperationRevenue.GaapIncomeStatementOperationRevenueId,
                Category = gaapIncomeStatementOperationRevenue.Category,
                Description = gaapIncomeStatementOperationRevenue.Description,
                NoteNumber = gaapIncomeStatementOperationRevenue.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationRevenues_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationRevenue gaapIncomeStatementOperationRevenue)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationRevenue
                {
                    GaapIncomeStatementId =id,
                    Category = gaapIncomeStatementOperationRevenue.Category,
                    Description = gaapIncomeStatementOperationRevenue.Description,
                    NoteNumber = gaapIncomeStatementOperationRevenue.NoteNumber
                };

                db.GaapIncomeStatementOperationRevenues.Add(entity);
                db.SaveChanges();
                gaapIncomeStatementOperationRevenue.GaapIncomeStatementOperationRevenueId = entity.GaapIncomeStatementOperationRevenueId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement operational revenue", "Success", User.Identity.Name, $"created rule {entity.Description} under {entity.GaapIncomeStatementId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationRevenue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationRevenues_Update([DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationRevenue gaapIncomeStatementOperationRevenue)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationRevenue
                {
                    GaapIncomeStatementOperationRevenueId = gaapIncomeStatementOperationRevenue.GaapIncomeStatementOperationRevenueId,
                    GaapIncomeStatementId = gaapIncomeStatementOperationRevenue.GaapIncomeStatementId,
                    Category = gaapIncomeStatementOperationRevenue.Category,
                    Description = gaapIncomeStatementOperationRevenue.Description,
                    NoteNumber = gaapIncomeStatementOperationRevenue.NoteNumber
                };

                db.GaapIncomeStatementOperationRevenues.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement operational revenue", "Success", User.Identity.Name, $"created rule {entity.Description} under {entity.GaapIncomeStatementId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationRevenue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationRevenues_Destroy([DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationRevenue gaapIncomeStatementOperationRevenue)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationRevenue
                {
                    GaapIncomeStatementOperationRevenueId = gaapIncomeStatementOperationRevenue.GaapIncomeStatementOperationRevenueId,
                    Category = gaapIncomeStatementOperationRevenue.Category,
                    Description = gaapIncomeStatementOperationRevenue.Description,
                    NoteNumber = gaapIncomeStatementOperationRevenue.NoteNumber
                };

                db.GaapIncomeStatementOperationRevenues.Attach(entity);
                db.GaapIncomeStatementOperationRevenues.Remove(entity);
                db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap income statement operational revenue {gaapIncomeStatementOperationRevenue.Description}");
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationRevenue }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
