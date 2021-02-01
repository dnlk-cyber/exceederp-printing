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
    public class GaapCashFlowReceiptController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapCashFlowReceipts_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapCashFlowReceipt> gaapCashFlowReceipts = _db.GaapCashFlowReceipts.Where(x=>x.GaapCashFlowId==id);
            DataSourceResult result = gaapCashFlowReceipts.ToDataSourceResult(request, gaapCashFlowReceipt => new {
                GaapCashFlowReceiptId = gaapCashFlowReceipt.GaapCashFlowReceiptId,
                GaapCashFlowId = gaapCashFlowReceipt.GaapCashFlowId,

                Description = gaapCashFlowReceipt.Description,
                NoteNumber = gaapCashFlowReceipt.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowReceipts_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapCashFlowReceipt gaapCashFlowReceipt)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowReceipt
                {
                    GaapCashFlowId =id,
                   
                    Description = gaapCashFlowReceipt.Description,
                    NoteNumber = gaapCashFlowReceipt.NoteNumber
                };

                _db.GaapCashFlowReceipts.Add(entity);
                _db.SaveChanges();
                gaapCashFlowReceipt.GaapCashFlowReceiptId = entity.GaapCashFlowReceiptId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup cash flow receipt", "Success", User.Identity.Name, $"created rule {entity.Description} under {entity.GaapCashFlowId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapCashFlowReceipt }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowReceipts_Update([DataSourceRequest]DataSourceRequest request, GaapCashFlowReceipt gaapCashFlowReceipt)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowReceipt
                {
                    GaapCashFlowReceiptId = gaapCashFlowReceipt.GaapCashFlowReceiptId,
                    GaapCashFlowId = gaapCashFlowReceipt.GaapCashFlowId,
                   
                    Description = gaapCashFlowReceipt.Description,
                    NoteNumber = gaapCashFlowReceipt.NoteNumber
                };

                _db.GaapCashFlowReceipts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup cash flow receipt", "Success", User.Identity.Name, $"updated rule {entity.Description} under {entity.GaapCashFlowId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapCashFlowReceipt }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowReceipts_Destroy([DataSourceRequest]DataSourceRequest request, GaapCashFlowReceipt gaapCashFlowReceipt)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowReceipt
                {
                    GaapCashFlowReceiptId = gaapCashFlowReceipt.GaapCashFlowReceiptId,
                    GaapCashFlowId = gaapCashFlowReceipt.GaapCashFlowId,

                    Description = gaapCashFlowReceipt.Description,
                    NoteNumber = gaapCashFlowReceipt.NoteNumber
                };

                _db.GaapCashFlowReceipts.Attach(entity);
                _db.GaapCashFlowReceipts.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap cash flow receiptt {gaapCashFlowReceipt.Description}");
                #endregion
            }

            return Json(new[] { gaapCashFlowReceipt }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
