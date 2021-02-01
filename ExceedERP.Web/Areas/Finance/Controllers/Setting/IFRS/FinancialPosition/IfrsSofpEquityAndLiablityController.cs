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
    public class IfrsSofpEquityAndLiablityController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSofpEquityAndLiablities_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSofpEquityAndLiablity> ifrssofpequityandliablities = _db.IfrsSofpEquityAndLiablities.Where(x=>x.IfrsStatementOfFinancialPositionId==id);
            DataSourceResult result = ifrssofpequityandliablities.ToDataSourceResult(request, ifrsSofpEquityAndLiablity => new {
                IfrsSofpEquityAndLiablityId = ifrsSofpEquityAndLiablity.IfrsSofpEquityAndLiablityId,
                IfrsStatementOfFinancialPositionId = id,
                Category = ifrsSofpEquityAndLiablity.Category,
                Description = ifrsSofpEquityAndLiablity.Description,
                NoteNumber = ifrsSofpEquityAndLiablity.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpEquityAndLiablities_Create(int id, [DataSourceRequest]DataSourceRequest request, IfrsSofpEquityAndLiablity ifrsSofpEquityAndLiablity)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }

            #region validation

            var exists = _db.IfrsSofpEquityAndLiablities.Where(x => x.NoteNumber == ifrsSofpEquityAndLiablity.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under equity and liability.");
            }

            var exists1 = _db.IfrsSofpAssets.Where(x => x.NoteNumber == ifrsSofpEquityAndLiablity.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }
           

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpEquityAndLiablity
                {
                    IfrsStatementOfFinancialPositionId = id,
                    Category = ifrsSofpEquityAndLiablity.Category,
                    Description = ifrsSofpEquityAndLiablity.Description,
                    NoteNumber = ifrsSofpEquityAndLiablity.NoteNumber
                };

                _db.IfrsSofpEquityAndLiablities.Add(entity);
                _db.SaveChanges();
                ifrsSofpEquityAndLiablity.IfrsSofpEquityAndLiablityId = entity.IfrsSofpEquityAndLiablityId;


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financial position equity and liability", "Success", User.Identity.Name, $"created rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSofpEquityAndLiablity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpEquityAndLiablities_Update(int id, [DataSourceRequest]DataSourceRequest request, IfrsSofpEquityAndLiablity ifrsSofpEquityAndLiablity)
        {
            #region validation

            var exists = _db.IfrsSofpEquityAndLiablities.Where(x => x.IfrsSofpEquityAndLiablityId != ifrsSofpEquityAndLiablity.IfrsSofpEquityAndLiablityId)
                                                        .Where(x => x.NoteNumber == ifrsSofpEquityAndLiablity.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under equity and liability.");
            }

            var exists1 = _db.IfrsSofpAssets.Where(x => x.NoteNumber == ifrsSofpEquityAndLiablity.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpEquityAndLiablity
                {
                    IfrsStatementOfFinancialPositionId = id,
                    IfrsSofpEquityAndLiablityId = ifrsSofpEquityAndLiablity.IfrsSofpEquityAndLiablityId,
                    Category = ifrsSofpEquityAndLiablity.Category,
                    Description = ifrsSofpEquityAndLiablity.Description,
                    NoteNumber = ifrsSofpEquityAndLiablity.NoteNumber
                };

                _db.IfrsSofpEquityAndLiablities.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financial position equity and liability", "Success", User.Identity.Name, $"created rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSofpEquityAndLiablity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpEquityAndLiablities_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSofpEquityAndLiablity ifrsSofpEquityAndLiablity)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpEquityAndLiablity
                {
                    IfrsSofpEquityAndLiablityId = ifrsSofpEquityAndLiablity.IfrsSofpEquityAndLiablityId,
                    Category = ifrsSofpEquityAndLiablity.Category,
                    Description = ifrsSofpEquityAndLiablity.Description,
                    NoteNumber = ifrsSofpEquityAndLiablity.NoteNumber
                };

                _db.IfrsSofpEquityAndLiablities.Attach(entity);
                _db.IfrsSofpEquityAndLiablities.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted financial position equity and liability {ifrsSofpEquityAndLiablity.Description}");
                #endregion
            }

            return Json(new[] { ifrsSofpEquityAndLiablity }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
