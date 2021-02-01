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
    public class IfrsSocfOperatingActivityController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSocfOperatingActivities_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSocfOperatingActivity> ifrssocfoperatingactivities = _db.IfrsSocfOperatingActivities.Where(x=>x.IfrsStatementOfCashFlowId==id);
            DataSourceResult result = ifrssocfoperatingactivities.ToDataSourceResult(request, ifrsSocfOperatingActivity => new {
                IfrsSocfOperatingActivityId = ifrsSocfOperatingActivity.IfrsSocfOperatingActivityId,
                IfrsStatementOfCashFlowId=id,
                Category = ifrsSocfOperatingActivity.Category,
                Description = ifrsSocfOperatingActivity.Description,
                Rule = ifrsSocfOperatingActivity.Rule,
                NoteNumber = ifrsSocfOperatingActivity.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfOperatingActivities_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfOperatingActivity ifrsSocfOperatingActivity)
        {

            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }
            #region validation

            var exists = _db.IfrsSocfFinancingActivities.Where(x => x.NoteNumber == ifrsSocfOperatingActivity.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under financial activities.");
            }

            var exists1 = _db.IfrsSocfOperatingActivities.Where(x => x.NoteNumber == ifrsSocfOperatingActivity.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under operating activities.");
            }
            var exists2 = _db.IfrsSocfInvestingActivities.Where(x => x.NoteNumber == ifrsSocfOperatingActivity.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under investing activities.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfOperatingActivity
                {
                    IfrsStatementOfCashFlowId = id,
                    Category = ifrsSocfOperatingActivity.Category,
                    Description = ifrsSocfOperatingActivity.Description,
                    Rule = ifrsSocfOperatingActivity.Rule,
                    NoteNumber = ifrsSocfOperatingActivity.NoteNumber
                };

                _db.IfrsSocfOperatingActivities.Add(entity);
                _db.SaveChanges();
                ifrsSocfOperatingActivity.IfrsSocfOperatingActivityId = entity.IfrsSocfOperatingActivityId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup operating activity", "Success", User.Identity.Name, $"created rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfOperatingActivity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfOperatingActivities_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfOperatingActivity ifrsSocfOperatingActivity)
        {
            #region validation

            var exists = _db.IfrsSocfFinancingActivities.Where(x => x.NoteNumber == ifrsSocfOperatingActivity.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under financial activities.");
            }

            var exists1 = _db.IfrsSocfOperatingActivities.Where(x => x.IfrsSocfOperatingActivityId != ifrsSocfOperatingActivity.IfrsSocfOperatingActivityId)
                                                         .Where(x => x.NoteNumber == ifrsSocfOperatingActivity.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under operating activities.");
            }
            var exists2 = _db.IfrsSocfInvestingActivities.Where(x => x.NoteNumber == ifrsSocfOperatingActivity.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under investing activities.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfOperatingActivity
                {
                    IfrsStatementOfCashFlowId = id,
                    IfrsSocfOperatingActivityId = ifrsSocfOperatingActivity.IfrsSocfOperatingActivityId,
                    Category = ifrsSocfOperatingActivity.Category,
                    Description = ifrsSocfOperatingActivity.Description,
                    Rule = ifrsSocfOperatingActivity.Rule,
                    NoteNumber = ifrsSocfOperatingActivity.NoteNumber
                };

                _db.IfrsSocfOperatingActivities.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup operating activity", "Success", User.Identity.Name, $"updated rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfOperatingActivity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfOperatingActivities_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSocfOperatingActivity ifrsSocfOperatingActivity)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfOperatingActivity
                {
                    IfrsSocfOperatingActivityId = ifrsSocfOperatingActivity.IfrsSocfOperatingActivityId,
                    Category = ifrsSocfOperatingActivity.Category,
                    Description = ifrsSocfOperatingActivity.Description,
                    Rule = ifrsSocfOperatingActivity.Rule,
                    NoteNumber = ifrsSocfOperatingActivity.NoteNumber
                };

                _db.IfrsSocfOperatingActivities.Attach(entity);
                _db.IfrsSocfOperatingActivities.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted operating activity {ifrsSocfOperatingActivity.Description}");
                #endregion
            }

            return Json(new[] { ifrsSocfOperatingActivity }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
