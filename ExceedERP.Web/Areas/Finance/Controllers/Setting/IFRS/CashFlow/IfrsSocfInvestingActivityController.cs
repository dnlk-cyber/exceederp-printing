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
    public class IfrsSocfInvestingActivityController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSocfInvestingActivities_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSocfInvestingActivity> ifrssocfinvestingactivities = _db.IfrsSocfInvestingActivities.Where(x=>x.IfrsStatementOfCashFlowId==id);
            DataSourceResult result = ifrssocfinvestingactivities.ToDataSourceResult(request, ifrsSocfInvestingActivity => new {
                IfrsSocfInvestingActivityId = ifrsSocfInvestingActivity.IfrsSocfInvestingActivityId,
                IfrsStatementOfCashFlowId = id,
                Category = ifrsSocfInvestingActivity.Category,
                Description = ifrsSocfInvestingActivity.Description,
                Rule = ifrsSocfInvestingActivity.Rule,
                NoteNumber = ifrsSocfInvestingActivity.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfInvestingActivities_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfInvestingActivity ifrsSocfInvestingActivity)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }
            #region validation

            var exists = _db.IfrsSocfFinancingActivities.Where(x => x.NoteNumber == ifrsSocfInvestingActivity.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under financial activities.");
            }

            var exists1 = _db.IfrsSocfOperatingActivities.Where(x => x.NoteNumber == ifrsSocfInvestingActivity.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under operating activities.");
            }
            var exists2 = _db.IfrsSocfInvestingActivities.Where(x => x.NoteNumber == ifrsSocfInvestingActivity.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under investing activities.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfInvestingActivity
                {
                    IfrsStatementOfCashFlowId = id,
                    Category = ifrsSocfInvestingActivity.Category,
                    Description = ifrsSocfInvestingActivity.Description,
                    Rule = ifrsSocfInvestingActivity.Rule,
                    NoteNumber = ifrsSocfInvestingActivity.NoteNumber
                };

                _db.IfrsSocfInvestingActivities.Add(entity);
                _db.SaveChanges();
                ifrsSocfInvestingActivity.IfrsSocfInvestingActivityId = entity.IfrsSocfInvestingActivityId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup investing activity", "Success", User.Identity.Name, $"created rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfInvestingActivity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfInvestingActivities_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfInvestingActivity ifrsSocfInvestingActivity)
        {
            #region validation

            var exists = _db.IfrsSocfFinancingActivities.Where(x => x.NoteNumber == ifrsSocfInvestingActivity.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under financial activities.");
            }

            var exists1 = _db.IfrsSocfOperatingActivities.Where(x => x.NoteNumber == ifrsSocfInvestingActivity.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under operating activities.");
            }
            var exists2 = _db.IfrsSocfInvestingActivities.Where(x => x.IfrsSocfInvestingActivityId != ifrsSocfInvestingActivity.IfrsSocfInvestingActivityId)
                                                         .Where(x => x.NoteNumber == ifrsSocfInvestingActivity.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under investing activities.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfInvestingActivity
                {
                    IfrsStatementOfCashFlowId = id,
                    IfrsSocfInvestingActivityId = ifrsSocfInvestingActivity.IfrsSocfInvestingActivityId,
                    Category = ifrsSocfInvestingActivity.Category,
                    Description = ifrsSocfInvestingActivity.Description,
                    Rule = ifrsSocfInvestingActivity.Rule,
                    NoteNumber = ifrsSocfInvestingActivity.NoteNumber
                };

                _db.IfrsSocfInvestingActivities.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup investing activity", "Success", User.Identity.Name, $"updated rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfInvestingActivity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfInvestingActivities_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSocfInvestingActivity ifrsSocfInvestingActivity)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfInvestingActivity
                {
                    IfrsSocfInvestingActivityId = ifrsSocfInvestingActivity.IfrsSocfInvestingActivityId,
                    Category = ifrsSocfInvestingActivity.Category,
                    Description = ifrsSocfInvestingActivity.Description,
                    Rule = ifrsSocfInvestingActivity.Rule,
                    NoteNumber = ifrsSocfInvestingActivity.NoteNumber
                };

                _db.IfrsSocfInvestingActivities.Attach(entity);
                _db.IfrsSocfInvestingActivities.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted investing activity {ifrsSocfInvestingActivity.Description}");
                #endregion
            }

            return Json(new[] { ifrsSocfInvestingActivity }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
