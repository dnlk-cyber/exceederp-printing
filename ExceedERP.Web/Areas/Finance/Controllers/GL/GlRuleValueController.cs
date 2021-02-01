using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlRuleValueController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GlRuleValues_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlRuleValue> glrulevalues = _db.GlRuleValues;
            DataSourceResult result = glrulevalues.ToDataSourceResult(request, glRuleValue => new {
                GlRuleValueId = glRuleValue.GlRuleValueId,
                NumberOfSegment = glRuleValue.NumberOfSegment,
                DynamicCoA = glRuleValue.DynamicCoA,
                CalculateProfitTax = glRuleValue.CalculateProfitTax,
                DateCreated = glRuleValue.DateCreated,
                LastModified = glRuleValue.LastModified,
                CreatedBy = glRuleValue.CreatedBy,
                ModifiedBy = glRuleValue.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlRuleValues_Create([DataSourceRequest]DataSourceRequest request, GlRuleValue glRuleValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlRuleValue
                {
                    NumberOfSegment = glRuleValue.NumberOfSegment,
                    DynamicCoA = glRuleValue.DynamicCoA,
                    CalculateProfitTax = glRuleValue.CalculateProfitTax,
                    DateCreated = glRuleValue.DateCreated,
                    LastModified = glRuleValue.LastModified,
                    CreatedBy = glRuleValue.CreatedBy,
                    ModifiedBy = glRuleValue.ModifiedBy
                };

                _db.GlRuleValues.Add(entity);
                _db.SaveChanges();
                glRuleValue.GlRuleValueId = entity.GlRuleValueId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL rrule value setup", "Success", User.Identity.Name, $"created rule value setup {entity.GlRuleValueId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glRuleValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlRuleValues_Update([DataSourceRequest]DataSourceRequest request, GlRuleValue glRuleValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlRuleValue
                {
                    GlRuleValueId = glRuleValue.GlRuleValueId,
                    NumberOfSegment = glRuleValue.NumberOfSegment,
                    DynamicCoA = glRuleValue.DynamicCoA,
                    CalculateProfitTax = glRuleValue.CalculateProfitTax,
                    DateCreated = glRuleValue.DateCreated,
                    LastModified = glRuleValue.LastModified,
                    CreatedBy = glRuleValue.CreatedBy,
                    ModifiedBy = glRuleValue.ModifiedBy
                };

                _db.GlRuleValues.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL rrule value setup", "Success", User.Identity.Name, $"updated rule value setup {entity.GlRuleValueId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glRuleValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlRuleValues_Destroy([DataSourceRequest]DataSourceRequest request, GlRuleValue glRuleValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlRuleValue
                {
                    GlRuleValueId = glRuleValue.GlRuleValueId,
                    NumberOfSegment = glRuleValue.NumberOfSegment,
                    DynamicCoA = glRuleValue.DynamicCoA,
                    CalculateProfitTax = glRuleValue.CalculateProfitTax,
                    DateCreated = glRuleValue.DateCreated,
                    LastModified = glRuleValue.LastModified,
                    CreatedBy = glRuleValue.CreatedBy,
                    ModifiedBy = glRuleValue.ModifiedBy
                };

                _db.GlRuleValues.Attach(entity);
                _db.GlRuleValues.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { glRuleValue }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
