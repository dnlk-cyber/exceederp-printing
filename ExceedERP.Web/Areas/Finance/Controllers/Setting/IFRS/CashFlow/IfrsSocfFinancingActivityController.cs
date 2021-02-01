using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Report.IFRS;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting.IFRS
{
    public class IfrsSocfFinancingActivityController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSocfFinancingActivities_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSocfFinancingActivity> ifrssocffinancingactivities = _db.IfrsSocfFinancingActivities.Where(x=>x.IfrsStatementOfCashFlowId==id);
            DataSourceResult result = ifrssocffinancingactivities.ToDataSourceResult(request, ifrsSocfFinancingActivity => new {
                IfrsSocfFinancingActivityId = ifrsSocfFinancingActivity.IfrsSocfFinancingActivityId,
                IfrsStatementOfCashFlowId = id,
                Category = ifrsSocfFinancingActivity.Category,
                Description = ifrsSocfFinancingActivity.Description,
                Rule = ifrsSocfFinancingActivity.Rule,
                NoteNumber = ifrsSocfFinancingActivity.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfFinancingActivities_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfFinancingActivity ifrsSocfFinancingActivity)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }

            #region validation

            var exists =_db.IfrsSocfFinancingActivities.Where(x => x.NoteNumber == ifrsSocfFinancingActivity.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist","The note number already exists under financial activities.");
            }

            var exists1 =_db.IfrsSocfOperatingActivities.Where(x => x.NoteNumber == ifrsSocfFinancingActivity.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under operating activities.");
            }
            var exists2 = _db.IfrsSocfInvestingActivities.Where(x => x.NoteNumber == ifrsSocfFinancingActivity.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under investing activities.");
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfFinancingActivity
                {
                    IfrsStatementOfCashFlowId = id,
                    Category = ifrsSocfFinancingActivity.Category,
                    Description = ifrsSocfFinancingActivity.Description,
                    Rule = ifrsSocfFinancingActivity.Rule,
                    NoteNumber = ifrsSocfFinancingActivity.NoteNumber
                };

                _db.IfrsSocfFinancingActivities.Add(entity);
                _db.SaveChanges();
                ifrsSocfFinancingActivity.IfrsSocfFinancingActivityId = entity.IfrsSocfFinancingActivityId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financing activity", "Success", User.Identity.Name, $"created rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfFinancingActivity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfFinancingActivities_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfFinancingActivity ifrsSocfFinancingActivity)
        {
            #region validation

            var exists = _db.IfrsSocfFinancingActivities.Where(x => x.IfrsSocfFinancingActivityId != ifrsSocfFinancingActivity.IfrsSocfFinancingActivityId)
                                                        .Where(x => x.NoteNumber == ifrsSocfFinancingActivity.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under financial activities.");
            }

            var exists1 = _db.IfrsSocfOperatingActivities.Where(x => x.NoteNumber == ifrsSocfFinancingActivity.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under operating activities.");
            }
            var exists2 = _db.IfrsSocfInvestingActivities.Where(x => x.NoteNumber == ifrsSocfFinancingActivity.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under investing activities.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfFinancingActivity
                {
                    IfrsStatementOfCashFlowId = id,
                    IfrsSocfFinancingActivityId = ifrsSocfFinancingActivity.IfrsSocfFinancingActivityId,
                    Category = ifrsSocfFinancingActivity.Category,
                    Description = ifrsSocfFinancingActivity.Description,
                    Rule = ifrsSocfFinancingActivity.Rule,
                    NoteNumber = ifrsSocfFinancingActivity.NoteNumber
                };

                _db.IfrsSocfFinancingActivities.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financing activity", "Success", User.Identity.Name, $"updated rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfFinancingActivity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfFinancingActivities_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSocfFinancingActivity ifrsSocfFinancingActivity)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfFinancingActivity
                {
                    IfrsSocfFinancingActivityId = ifrsSocfFinancingActivity.IfrsSocfFinancingActivityId,
                    Category = ifrsSocfFinancingActivity.Category,
                    Description = ifrsSocfFinancingActivity.Description,
                    Rule = ifrsSocfFinancingActivity.Rule,
                    NoteNumber = ifrsSocfFinancingActivity.NoteNumber
                };

                _db.IfrsSocfFinancingActivities.Attach(entity);
                _db.IfrsSocfFinancingActivities.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted financial activty {ifrsSocfFinancingActivity.Description}");
                #endregion
            }

            return Json(new[] { ifrsSocfFinancingActivity }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
