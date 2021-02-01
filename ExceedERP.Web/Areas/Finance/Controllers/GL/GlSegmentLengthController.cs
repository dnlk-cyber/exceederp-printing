using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlSegmentLengthController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
       
       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
            return View();
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult GlSegmentLengths_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlSegmentLength> glsegmentlengths = _db.GlSegmentLengths;
            DataSourceResult result = glsegmentlengths.ToDataSourceResult(request, glSegmentLength => new {
                GlSegmentLengthId = glSegmentLength.GlSegmentLengthId,
                AccountLength = glSegmentLength.AccountLength,
                LocationLength = glSegmentLength.LocationLength,
                CostCenterLength = glSegmentLength.CostCenterLength,
                SubAccountLength = glSegmentLength.SubAccountLength,
                DateCreated = glSegmentLength.DateCreated,
                LastModified = glSegmentLength.LastModified,
                CreatedBy = glSegmentLength.CreatedBy,
                ModifiedBy = glSegmentLength.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentLengths_Create([DataSourceRequest]DataSourceRequest request, GlSegmentLength glSegmentLength)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlSegmentLength
                {
                    AccountLength = glSegmentLength.AccountLength,
                    LocationLength = glSegmentLength.LocationLength,
                    CostCenterLength = glSegmentLength.CostCenterLength,
                    SubAccountLength = glSegmentLength.SubAccountLength,
                    DateCreated = DateTime.Now,
                    LastModified = glSegmentLength.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = glSegmentLength.ModifiedBy
                };

                _db.GlSegmentLengths.Add(entity);
                _db.SaveChanges();
                glSegmentLength.GlSegmentLengthId = entity.GlSegmentLengthId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL segment length setup", "Success", User.Identity.Name, $"created segment length setup {entity.GlSegmentLengthId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glSegmentLength }.ToDataSourceResult(request, ModelState));
        }
        
       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentLengths_Update([DataSourceRequest]DataSourceRequest request, GlSegmentLength glSegmentLength)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlSegmentLength
                {
                    GlSegmentLengthId = glSegmentLength.GlSegmentLengthId,
                    AccountLength = glSegmentLength.AccountLength,
                    LocationLength = glSegmentLength.LocationLength,
                    CostCenterLength = glSegmentLength.CostCenterLength,
                    SubAccountLength = glSegmentLength.SubAccountLength,
                    DateCreated = glSegmentLength.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = glSegmentLength.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.GlSegmentLengths.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL segment length setup", "Success", User.Identity.Name, $"updated segment length setup {entity.GlSegmentLengthId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glSegmentLength }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlSegmentLengths_Destroy([DataSourceRequest]DataSourceRequest request, GlSegmentLength glSegmentLength)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlSegmentLength
                {
                    GlSegmentLengthId = glSegmentLength.GlSegmentLengthId,
                    AccountLength = glSegmentLength.AccountLength,
                    LocationLength = glSegmentLength.LocationLength,
                    CostCenterLength = glSegmentLength.CostCenterLength,
                    SubAccountLength = glSegmentLength.SubAccountLength,
                    DateCreated = glSegmentLength.DateCreated,
                    LastModified = glSegmentLength.LastModified,
                    CreatedBy = glSegmentLength.CreatedBy,
                    ModifiedBy = glSegmentLength.ModifiedBy
                };

                _db.GlSegmentLengths.Attach(entity);
                _db.GlSegmentLengths.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { glSegmentLength }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
