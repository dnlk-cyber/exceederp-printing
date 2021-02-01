using System;
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
    public class IfrsScoiOtherComprehensiveIncomeRangeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsScoiOtherComprehensiveIncomeRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsScoiOtherComprehensiveIncomeRange> ifrsscoiothercomprehensiveincomeranges = _db.IfrsScoiOtherComprehensiveIncomeRanges.Where(x=>x.IfrsScoiOtherComprehensiveIncomeId==id);
            DataSourceResult result = ifrsscoiothercomprehensiveincomeranges.ToDataSourceResult(request, ifrsScoiOtherComprehensiveIncomeRange => new {
                IfrsScoiOtherComprehensiveIncomeId=id,
                IfrsScoiOtherComprehensiveIncomeRangeId = ifrsScoiOtherComprehensiveIncomeRange.IfrsScoiOtherComprehensiveIncomeRangeId,
                AccountFrom = ifrsScoiOtherComprehensiveIncomeRange.AccountFrom,
                AccountTo = ifrsScoiOtherComprehensiveIncomeRange.AccountTo,
                DateCreated = ifrsScoiOtherComprehensiveIncomeRange.DateCreated,
                CreatedBy = ifrsScoiOtherComprehensiveIncomeRange.CreatedBy,
                LastModified = ifrsScoiOtherComprehensiveIncomeRange.LastModified,
                ModifiedBy = ifrsScoiOtherComprehensiveIncomeRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiOtherComprehensiveIncomeRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsScoiOtherComprehensiveIncomeRange ifrsScoiOtherComprehensiveIncomeRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiOtherComprehensiveIncomeRange
                {
                    IfrsScoiOtherComprehensiveIncomeId = id,
                    AccountFrom = ifrsScoiOtherComprehensiveIncomeRange.AccountFrom,
                    AccountTo = ifrsScoiOtherComprehensiveIncomeRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = ifrsScoiOtherComprehensiveIncomeRange.LastModified,
                    ModifiedBy = ifrsScoiOtherComprehensiveIncomeRange.ModifiedBy,
                };

                _db.IfrsScoiOtherComprehensiveIncomeRanges.Add(entity);
                _db.SaveChanges();
                ifrsScoiOtherComprehensiveIncomeRange.IfrsScoiOtherComprehensiveIncomeRangeId = entity.IfrsScoiOtherComprehensiveIncomeRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup other comprehensive income range", "Success", User.Identity.Name, $"create range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiOtherComprehensiveIncomeRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiOtherComprehensiveIncomeRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsScoiOtherComprehensiveIncomeRange ifrsScoiOtherComprehensiveIncomeRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiOtherComprehensiveIncomeRange
                {
                    IfrsScoiOtherComprehensiveIncomeId = id,
                    IfrsScoiOtherComprehensiveIncomeRangeId = ifrsScoiOtherComprehensiveIncomeRange.IfrsScoiOtherComprehensiveIncomeRangeId,
                    AccountFrom = ifrsScoiOtherComprehensiveIncomeRange.AccountFrom,
                    AccountTo = ifrsScoiOtherComprehensiveIncomeRange.AccountTo,
                    DateCreated = ifrsScoiOtherComprehensiveIncomeRange.DateCreated,
                    CreatedBy = ifrsScoiOtherComprehensiveIncomeRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.IfrsScoiOtherComprehensiveIncomeRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup other comprehensive income range", "Success", User.Identity.Name, $"update range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiOtherComprehensiveIncomeRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiOtherComprehensiveIncomeRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsScoiOtherComprehensiveIncomeRange ifrsScoiOtherComprehensiveIncomeRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiOtherComprehensiveIncomeRange
                {
                    IfrsScoiOtherComprehensiveIncomeRangeId = ifrsScoiOtherComprehensiveIncomeRange.IfrsScoiOtherComprehensiveIncomeRangeId,
                    AccountFrom = ifrsScoiOtherComprehensiveIncomeRange.AccountFrom,
                    AccountTo = ifrsScoiOtherComprehensiveIncomeRange.AccountTo
                };

                _db.IfrsScoiOtherComprehensiveIncomeRanges.Attach(entity);
                _db.IfrsScoiOtherComprehensiveIncomeRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted comprehensive income account range from {ifrsScoiOtherComprehensiveIncomeRange.AccountFrom} to {ifrsScoiOtherComprehensiveIncomeRange.AccountTo}");
                #endregion
            }

            return Json(new[] { ifrsScoiOtherComprehensiveIncomeRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
