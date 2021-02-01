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
    public class IfrsSofpEquityAndLiablityRangeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSofpEquityAndLiablityRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSofpEquityAndLiablityRange> ifrssofpequityandliablityranges = _db.IfrsSofpEquityAndLiablityRanges.Where(x=>x.IfrsSofpEquityAndLiablityId==id);
            DataSourceResult result = ifrssofpequityandliablityranges.ToDataSourceResult(request, ifrsSofpEquityAndLiablityRange => new {
                IfrsSofpEquityAndLiablityId=id,
                IfrsSofpEquityAndLiablityRangeId = ifrsSofpEquityAndLiablityRange.IfrsSofpEquityAndLiablityRangeId,
                AccountFrom = ifrsSofpEquityAndLiablityRange.AccountFrom,
                AccountTo = ifrsSofpEquityAndLiablityRange.AccountTo,
                DateCreated = ifrsSofpEquityAndLiablityRange.DateCreated,
                CreatedBy = ifrsSofpEquityAndLiablityRange.CreatedBy,
                LastModified = ifrsSofpEquityAndLiablityRange.LastModified,
                ModifiedBy = ifrsSofpEquityAndLiablityRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpEquityAndLiablityRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsSofpEquityAndLiablityRange ifrsSofpEquityAndLiablityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpEquityAndLiablityRange
                {
                    IfrsSofpEquityAndLiablityId = id,
                    AccountFrom = ifrsSofpEquityAndLiablityRange.AccountFrom,
                    AccountTo = ifrsSofpEquityAndLiablityRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = ifrsSofpEquityAndLiablityRange.LastModified,
                    ModifiedBy = ifrsSofpEquityAndLiablityRange.ModifiedBy,
                };

                _db.IfrsSofpEquityAndLiablityRanges.Add(entity);
                _db.SaveChanges();
                ifrsSofpEquityAndLiablityRange.IfrsSofpEquityAndLiablityRangeId = entity.IfrsSofpEquityAndLiablityRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financial position equity and liability range", "Success", User.Identity.Name, $"create range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSofpEquityAndLiablityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpEquityAndLiablityRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsSofpEquityAndLiablityRange ifrsSofpEquityAndLiablityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpEquityAndLiablityRange
                {
                    IfrsSofpEquityAndLiablityId = id,
                    IfrsSofpEquityAndLiablityRangeId = ifrsSofpEquityAndLiablityRange.IfrsSofpEquityAndLiablityRangeId,
                    AccountFrom = ifrsSofpEquityAndLiablityRange.AccountFrom,
                    AccountTo = ifrsSofpEquityAndLiablityRange.AccountTo,
                    DateCreated = ifrsSofpEquityAndLiablityRange.DateCreated,
                    CreatedBy = ifrsSofpEquityAndLiablityRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.IfrsSofpEquityAndLiablityRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financial position equity and liability range", "Success", User.Identity.Name, $"update range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSofpEquityAndLiablityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpEquityAndLiablityRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSofpEquityAndLiablityRange ifrsSofpEquityAndLiablityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpEquityAndLiablityRange
                {
                    IfrsSofpEquityAndLiablityRangeId = ifrsSofpEquityAndLiablityRange.IfrsSofpEquityAndLiablityRangeId,
                    AccountFrom = ifrsSofpEquityAndLiablityRange.AccountFrom,
                    AccountTo = ifrsSofpEquityAndLiablityRange.AccountTo
                };

                _db.IfrsSofpEquityAndLiablityRanges.Attach(entity);
                _db.IfrsSofpEquityAndLiablityRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted financial position equity and liability account range from {ifrsSofpEquityAndLiablityRange.AccountFrom} to {ifrsSofpEquityAndLiablityRange.AccountTo}");
                #endregion
            }

            return Json(new[] { ifrsSofpEquityAndLiablityRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
