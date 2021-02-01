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
    public class GaapCashFlowPayOutController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapCashFlowPayOuts_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapCashFlowPayOut> gaapCashFlowPayOuts = _db.GaapCashFlowPayOuts.Where(x=>x.GaapCashFlowId==id);
            DataSourceResult result = gaapCashFlowPayOuts.ToDataSourceResult(request, gaapCashFlowPayOut => new {
                GaapCashFlowPayOutId = gaapCashFlowPayOut.GaapCashFlowPayOutId,
                GaapCashFlowId = gaapCashFlowPayOut.GaapCashFlowId,
                Description = gaapCashFlowPayOut.Description,
                NoteNumber = gaapCashFlowPayOut.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowPayOuts_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapCashFlowPayOut gaapCashFlowPayOut)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowPayOut
                {
                    GaapCashFlowId =id,
                   
                    Description = gaapCashFlowPayOut.Description,
                    NoteNumber = gaapCashFlowPayOut.NoteNumber
                };

                _db.GaapCashFlowPayOuts.Add(entity);
                _db.SaveChanges();
                gaapCashFlowPayOut.GaapCashFlowPayOutId = entity.GaapCashFlowPayOutId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup cash flow pay out", "Success", User.Identity.Name, $"created rule {entity.Description} under {entity.GaapCashFlowId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapCashFlowPayOut }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowPayOuts_Update([DataSourceRequest]DataSourceRequest request, GaapCashFlowPayOut gaapCashFlowPayOut)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowPayOut
                {
                    GaapCashFlowPayOutId = gaapCashFlowPayOut.GaapCashFlowPayOutId,
                    GaapCashFlowId = gaapCashFlowPayOut.GaapCashFlowId,
                   
                    Description = gaapCashFlowPayOut.Description,
                    NoteNumber = gaapCashFlowPayOut.NoteNumber
                };

                _db.GaapCashFlowPayOuts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup cash flow pay out", "Success", User.Identity.Name, $"updated rule {entity.Description} under {entity.GaapCashFlowId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapCashFlowPayOut }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowPayOuts_Destroy([DataSourceRequest]DataSourceRequest request, GaapCashFlowPayOut gaapCashFlowPayOut)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowPayOut
                {
                    GaapCashFlowPayOutId = gaapCashFlowPayOut.GaapCashFlowPayOutId,
                    GaapCashFlowId = gaapCashFlowPayOut.GaapCashFlowId,
                    Description = gaapCashFlowPayOut.Description,
                    NoteNumber = gaapCashFlowPayOut.NoteNumber
                };

                _db.GaapCashFlowPayOuts.Attach(entity);
                _db.GaapCashFlowPayOuts.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap cash flow pay out {gaapCashFlowPayOut.Description}");
                #endregion
            }

            return Json(new[] { gaapCashFlowPayOut }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
